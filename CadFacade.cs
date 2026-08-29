using System.IO;
using TFlex.Model;
using TFlex.Model.Model2D;
using TFlex.Model.Model3D;

namespace TFlexApp
{
    public class CadFacade : ICadFacade
    {
        //форматка с основной надписью (первый лист, ГОСТ 2.104-68) относительно папки Program
        private const string StampPath = @"..\Библиотеки\Служебные\Форматки\Конструкторский чертеж. Первый лист. ГОСТ 2.104-68.grb";
        private const double Margin = 25;  //отступ от границ листа
        private const double Gap = 20;     //зазор между проекциями

        private readonly TFlex.Control _tfControl;

        public CadFacade(TFlex.Control tfControl) => _tfControl = tfControl;

        // Создаём документ: 3D-модель, штамп, стандартные проекции
        public void CreatePartDrawing(Model model)
        {
            var document = TFlex.Application.NewDocument(true) 
                ?? throw new InvalidOperationException("Не удалось создать документ T-Flex CAD.");

            ThickenExtrusion part = Build3DModel(document, model);
            Page page = AddStamp(document);
            AddStandardProjections(document, page, part);

            ShowDocument(document);
        }

        // Строим 3D-модель: выдавливание прямоугольного профиля с опциональным отверстием
        private static ThickenExtrusion Build3DModel(TFlex.Model.Document document, Model model)
        {
            document.BeginChanges("Операция выталкивания");

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

            //рабочая плоскость, на которой строится эскиз для выдавливания
            StandardWorkplane frontPlane = new(document, StandardWorkplane.StandardType.Front);

            //профиль для выдавливания на рабочей плоскости
            AreaProfile areaProfile = new(document) { Area = area, WorkSurface = frontPlane };

            //операция выдавливания на заданную толщину
            ThickenExtrusion part = new(document)
            {
                LengthType = ThickenExtrusion.LengthValue.AutoValue,
                ForwardLength = model.Thickness,
            };
            part.Profile.Add(areaProfile.Geometry.SheetContour);

            document.EndChanges();
            return part;
        }

        // Вставляем форматку с основной надписью и возвращаем страницу чертежа
        private static Page AddStamp(Document document)
        {
            document.BeginChanges("Добавление форматки");
            Page page = document.ActivePage;
            Fragment stamp = new(document, StampPath);
            document.EndChanges();
            return page;
        }

        // Добавляем стандартные проекции на чертеж
        private static void AddStandardProjections(Document document, Page page, ThickenExtrusion part)
        {
            //доступная область листа (лист начинается в 0,0)
            var pageRect = page.Rectangle;
            double areaWidth = pageRect.Width - 2 * Margin;
            double areaHeight = pageRect.Height - 2 * Margin;

            //размер, в который вписывается один вид: два вида по горизонтали и два по вертикали
            double viewWidth = (areaWidth - Gap) / 2;
            double viewHeight = (areaHeight - Gap) / 2;

            //создаём три проекции в нулевой точке
            var projectionFront = CreateProjection(document, page, part, ProjectionType.FrontProjection, viewWidth, viewHeight);
            var projectionLeft = CreateProjection(document, page, part, ProjectionType.LeftProjection, viewWidth, viewHeight);
            var projectionTop = CreateProjection(document, page, part, ProjectionType.TopProjection, viewWidth, viewHeight);

            var frontRect = projectionFront.BoundRect;
            var leftRect = projectionLeft.BoundRect;
            var topRect = projectionTop.BoundRect;

            //габариты блока из трёх видов
            double blockWidth = frontRect.Width + Gap + leftRect.Width;
            double blockHeight = frontRect.Height + Gap + topRect.Height;

            //левый нижний угол блока: центрируем в доступной области
            double blockX = Margin + Math.Max(0, (areaWidth - blockWidth) / 2);
            double blockY = Margin + Math.Max(0, (areaHeight - blockHeight) / 2);

            //главный вид сверху блока, вид сверху снизу от него (ось Y направлена вверх)
            double frontLeft = blockX;
            double frontBottom = blockY + topRect.Height + Gap;

            document.BeginChanges("Размещение проекций");
            MoveProjection(projectionFront, frontRect, frontLeft, frontBottom);
            MoveProjection(projectionLeft, leftRect, frontLeft + frontRect.Width + Gap, frontBottom);
            MoveProjection(projectionTop, topRect, frontLeft, blockY);
            document.EndChanges();
        }

        // Создаём проекцию заданного типа в нулевой точке привязки и вписываем в заданный размер
        private static SimpleDrawingProjection CreateProjection(TFlex.Model.Document document, Page page,
            ThickenExtrusion part, ProjectionType viewType, double fitWidth, double fitHeight)
        {
            document.BeginChanges($"Добавление проекции {viewType}");
            SimpleDrawingProjection projection = new(document, page);
            projection.AddOperation(part);
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

        // Привязываем документ к TFlex.Control для отображения
        private void ShowDocument(TFlex.Model.Document document)
        {
            if (_tfControl == null)
            {
                return;
            }

            _tfControl.Document = document;
            _tfControl.RefreshTabs();
            _tfControl.Invalidate(true);
        }
    }
}
