using System.Collections.ObjectModel;
using System.IO;
using System.Reflection.Metadata;
using TFlex;
using TFlex.Model;
using TFlex.Model.Model2D;
using TFlex.Model.Model3D;

namespace TFlexApp
{
    public class CadOperations : ICadOperations
    {
        private readonly TFlex.Control _tfControl;

        public CadOperations(TFlex.Control tfControl) => _tfControl = tfControl;

        public void Create3DPart(Model model)
        {
            //int openCount = TFlex.Application.Documents.Count();
            //System.Windows.Forms.MessageBox.Show($"Открыто документов: {openCount}", "Информация",
            //    System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Information);

            var document = TFlex.Application.NewDocument(true) ?? throw new InvalidOperationException("Не удалось создать документ T-Flex CAD.");

            document.BeginChanges("Операция выталкивания");//Открытие блока изменений документа

            //создание узлов (углов прямоугольника)
            FreeNode n1 = new(document, 0, 0);
            FreeNode n2 = new(document, model.Length, 0);
            FreeNode n3 = new(document, model.Length, model.Height);
            FreeNode n4 = new(document, 0, model.Height);

            //создание области и добавление на неё контуров для выдавливания
            Area area = new(document);
            Contour contour = area.AppendContour();

            //добавление сегментов в контур
            ConstructionContourSegment s1 = new(contour) { StartNode = n1, EndNode = n2 };
            ConstructionContourSegment s2 = new(contour) { StartNode = n2, EndNode = n3 };
            ConstructionContourSegment s3 = new(contour) { StartNode = n3, EndNode = n4 };
            ConstructionContourSegment s4 = new(contour) { StartNode = n4, EndNode = n1 };

            if (model.HasHole)
            {
                FreeNode center = new(document, model.Length / 2, model.Height / 2);
                CircleConstruction circle = new(document);
                circle.SetCenterAndRadius(center, model.HoleDiameter / 2);

                Contour holeContour = area.AppendContour();
                ConstructionContourSegment s5 = new(holeContour) { Construction = circle };
            }

            //создание рабочей плоскости (вид слева), на которой будет построен эскиз для выдавливания
            StandardWorkplane frontPlane = new(document, StandardWorkplane.StandardType.Front);

            //построение профиля для выдавливания на рабочей плоскости
            AreaProfile areaProfile = new(document) { Area = area, WorkSurface = frontPlane };

            //операция выдавливания на заданную толщину
            ThickenExtrusion part_ext = new(document)
            {
                LengthType = ThickenExtrusion.LengthValue.AutoValue,
                ForwardLength = model.Thickness,
            };
            part_ext.Profile.Add(areaProfile.Geometry.SheetContour);
            document.EndChanges();

            document.BeginChanges("Добавление форматки");
            Page page = document.ActivePage;

            //вставляем форматку с основной надписью (первый лист, ГОСТ 2.104-68)
            string stampPath = Path.Combine("..", "Библиотеки", "Служебные", "Форматки", "Конструкторский чертеж. Первый лист. ГОСТ 2.104-68.grb");
            Fragment stamp = new(document, stampPath);
            document.EndChanges();

            AddStandardProjections(document, page, part_ext);

            //привязываем документ к TFlex.Control для отображения
            if (_tfControl != null)
            {
                _tfControl.Document = document;
                _tfControl.RefreshTabs();
                _tfControl.Invalidate(true);
            }
        }

        // Добавляем стандартные проекции на чертеж
        static void AddStandardProjections(TFlex.Model.Document document, Page page, ThickenExtrusion part_ext)
        {
            double margin = 25;  //отступ от границ листа
            double gap = 20;     //зазор между проекциями

            //доступная область листа (лист начинается в 0,0)
            var pageRect = page.Rectangle;
            double areaWidth = pageRect.Width - 2 * margin;
            double areaHeight = pageRect.Height - 2 * margin;

            //бюджет на один вид: два вида по горизонтали и два по вертикали
            double viewWidth = (areaWidth - gap) / 2;
            double viewHeight = (areaHeight - gap) / 2;

            //создаём три проекции в нулевой точке
            var projectionFront = CreateProjection(document, page, part_ext, ProjectionType.FrontProjection, viewWidth, viewHeight);
            var projectionLeft = CreateProjection(document, page, part_ext, ProjectionType.LeftProjection, viewWidth, viewHeight);
            var projectionTop = CreateProjection(document, page, part_ext, ProjectionType.TopProjection, viewWidth, viewHeight);

            var frontRect = projectionFront.BoundRect;
            var leftRect = projectionLeft.BoundRect;
            var topRect = projectionTop.BoundRect;

            //габариты блока из трёх видов
            double blockWidth = frontRect.Width + gap + leftRect.Width;
            double blockHeight = frontRect.Height + gap + topRect.Height;

            //левый нижний угол блока: центрируем в доступной области
            double blockX = margin + Math.Max(0, (areaWidth - blockWidth) / 2);
            double blockY = margin + Math.Max(0, (areaHeight - blockHeight) / 2);

            //главный вид сверху блока, вид сверху снизу от него (ось Y направлена вверх)
            double frontLeft = blockX;
            double frontBottom = blockY + topRect.Height + gap;

            document.BeginChanges("Размещение проекций");
            MoveProjection(projectionFront, frontRect, frontLeft, frontBottom);
            MoveProjection(projectionLeft, leftRect, frontLeft + frontRect.Width + gap, frontBottom);
            MoveProjection(projectionTop, topRect, frontLeft, blockY);
            document.EndChanges();
        }

        // Создаём проекцию заданного типа в нулевой точке привязки и вписываем в заданный размер
        private static SimpleDrawingProjection CreateProjection(TFlex.Model.Document document, Page page,
            ThickenExtrusion part_ext, ProjectionType viewType, double fitWidth, double fitHeight)
        {
            document.BeginChanges($"Добавление проекции {viewType}");
            SimpleDrawingProjection projection = new(document, page);
            projection.AddOperation(part_ext);
            projection.SetViewType(viewType);
            projection.SetTiePoint(0, 0);
            projection.ScaleFitToPageSize(fitWidth, fitHeight);
            document.EndChanges();
            return projection;
        }

        // Смещаем проекцию так, чтобы её левый нижний угол попал в заданную точку
        private static void MoveProjection(SimpleDrawingProjection projection, TFlex.Drawing.Rectangle currentRect,
            double targetLeft, double targetBottom)
        {
            //точка привязки была в (0,0), поэтому текущий Left/Bottom — это смещение габаритов от привязки
            projection.SetTiePoint(targetLeft - currentRect.Left, targetBottom - currentRect.Bottom);
        }

    }
}
