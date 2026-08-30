using System.IO;
using TFlex.Model;
using TFlex.Model.Model2D;
using TFlex.Model.Model3D;

namespace TFlexApp
{
    public class CadFacade : ICadFacade
    {
        //форматка с основной надписью (первый лист, ГОСТ 2.104-68) относительно папки Program
        private const string FirstSheetStampPath = @"..\Библиотеки\Служебные\Форматки\Конструкторский чертеж. Первый лист. ГОСТ 2.104-68.grb";
        private const double Margin = 25;  //отступ от границ листа
        private const double Gap = 20;     //зазор между проекциями
        private const double DimensionOffset = 10; //отступ линии размера от габарита вида

        //ряд масштабов по ГОСТ 2.302-68, от крупного к мелкому
        private static readonly (double Value, string Text)[] StandardScales =
        [
            (100, "100:1"), (50, "50:1"), (40, "40:1"), (20, "20:1"), (10, "10:1"),
            (5, "5:1"), (4, "4:1"), (2.5, "2.5:1"), (2, "2:1"), (1, "1:1"),
            (0.5, "1:2"), (0.4, "1:2.5"), (0.25, "1:4"), (0.2, "1:5"), (0.1, "1:10"),
            (1.0 / 15, "1:15"), (0.05, "1:20"), (0.04, "1:25"), (0.025, "1:40"),
            (0.02, "1:50"), (1.0 / 75, "1:75"), (0.01, "1:100")
        ];

        // Результат размещения проекций: масштаб и габариты видов на листе
        private sealed record ProjectionsLayout(
            double Scale,
            string ScaleText,
            TFlex.Drawing.Rectangle FrontRect,
            TFlex.Drawing.Rectangle LeftRect,
            TFlex.Drawing.Rectangle TopRect);

        private readonly TFlex.Control _tfControl;

        public CadFacade(TFlex.Control tfControl) => _tfControl = tfControl;

        // Создаём 3D-модель, стандартные проекции и заполняемая основную надпись
        public void CreatePartDrawing(Model model)
        {
            var document = TFlex.Application.NewDocument(true)
                ?? throw new InvalidOperationException(Messages.ErrDocumentCreation);

            // Строим 3D-модель детали
            ThickenExtrusion part = Build3DModel(document, model);

            // Добавляем на первый лист стандартные проекции и размеры
            Page page = document.ActivePage;
            var layout = AddStandardProjections(document, page, part);

            // Добавляем размеры
            AddDimensions(document, page, model, layout);

            // Добавляем форматку с основной надписью на первый лист, заполняем её и устанавливаем масштаб
            Fragment firstStamp = AddFirstSheetStamp(document);
            SetStampScale(document, firstStamp, layout.ScaleText);
            FillStamp(document, firstStamp, model);

            ShowDocument(document);
        }

        // Строим 3D-модель: выдавливание прямоугольного профиля с опциональным отверстием
        private static ThickenExtrusion Build3DModel(TFlex.Model.Document document, Model model)
        {
            document.BeginChanges("Операция выталкивания");

            //погашенный слой: эскиз остаётся в модели, но не отображается на листе
            Layer sketchLayer = new(document) { Name = "Эскиз", Hidden = true };

            //создание узлов (углов прямоугольника)
            FreeNode n1 = new(document, 0, 0) { Layer = sketchLayer };
            FreeNode n2 = new(document, model.Length, 0) { Layer = sketchLayer };
            FreeNode n3 = new(document, model.Length, model.Height) { Layer = sketchLayer };
            FreeNode n4 = new(document, 0, model.Height) { Layer = sketchLayer };

            //создание области и добавление на неё контуров для выдавливания
            Area area = new(document) { Layer = sketchLayer };
            Contour contour = area.AppendContour();

            //добавление сегментов в контур
            ConstructionContourSegment s1 = new(contour) { StartNode = n1, EndNode = n2 };
            ConstructionContourSegment s2 = new(contour) { StartNode = n2, EndNode = n3 };
            ConstructionContourSegment s3 = new(contour) { StartNode = n3, EndNode = n4 };
            ConstructionContourSegment s4 = new(contour) { StartNode = n4, EndNode = n1 };

            if (model.HasHole)
            {
                FreeNode center = new(document, model.Length / 2, model.Height / 2) { Layer = sketchLayer };
                CircleConstruction circle = new(document) { Layer = sketchLayer };
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
        // Добавляем стандартные проекции на чертеж и возвращаем масштаб с точками привязки видов
        private static ProjectionsLayout AddStandardProjections(Document document, Page page, ThickenExtrusion part)
        {
            //доступная область листа (лист начинается в 0,0)
            var pageRect = page.Rectangle;
            double areaWidth = pageRect.Width - 2 * Margin;
            double areaHeight = pageRect.Height - 2 * Margin;

            //создаём три проекции в нулевой точке и в натуральную величину
            var projectionFront = CreateProjection(document, page, part, ProjectionType.FrontProjection);
            var projectionLeft = CreateProjection(document, page, part, ProjectionType.LeftProjection);
            var projectionTop = CreateProjection(document, page, part, ProjectionType.TopProjection);

            //все виды одного чертежа должны быть в едином масштабе
            double modelWidth = projectionFront.BoundRect.Width + projectionLeft.BoundRect.Width;
            double modelHeight = projectionFront.BoundRect.Height + projectionTop.BoundRect.Height;
            var scale = SelectStandardScale(modelWidth, modelHeight, areaWidth - Gap, areaHeight - Gap);

            document.BeginChanges("Масштаб проекций");
            projectionFront.Scale = scale.Value;
            projectionLeft.Scale = scale.Value;
            projectionTop.Scale = scale.Value;
            document.EndChanges();

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

            //габариты после перемещения — фактические координаты видов на листе
            return new ProjectionsLayout(
                scale.Value, scale.Text,
                projectionFront.BoundRect, 
                projectionLeft.BoundRect, 
                projectionTop.BoundRect);
        }

        // Подбираем крупнейший масштаб из ряда ГОСТ, при котором виды влезают в область
        internal static (double Value, string Text) SelectStandardScale(double modelWidth, double modelHeight,
            double availableWidth, double availableHeight)
        {
            double required = Math.Min(availableWidth / modelWidth, availableHeight / modelHeight);

            foreach (var scale in StandardScales)
            {
                if (scale.Value <= required)
                {
                    return scale;
                }
            }
            return StandardScales[^1];
        }

        // Создаём проекцию заданного типа в нулевой точке привязки
        private static SimpleDrawingProjection CreateProjection(TFlex.Model.Document document, Page page,
            ThickenExtrusion part, ProjectionType viewType)
        {
            document.BeginChanges($"Добавление проекции {viewType}");
            SimpleDrawingProjection projection = new(document, page);
            projection.AddOperation(part);
            projection.SetViewType(viewType);
            projection.SetTiePoint(0, 0);
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

        // Ставим размеры от центра габаритного прямоугольника вида: деталь симметрична ему
        private static void AddDimensions(Document document, Page page, Model model, ProjectionsLayout layout)
        {
            document.BeginChanges("Добавление размеров");

            //скрытые узлы только несут размеры и не отображаются
            Layer nodeLayer = new(document) { Name = "Узлы размеров", Hidden = true };
            FreeNode Node(double x, double y) => new(document, x, y) { Layer = nodeLayer, Page = page };

            //главный вид: длина сверху
            var front = PartCorners(layout.FrontRect, model.Length, model.Height, layout.Scale);
            AddLinearDimension(document, page, Node(front.Left, front.Top), Node(front.Right, front.Top),
                DimensionAlignType.Horizontal, -DimensionOffset, layout.Scale);

            //вид слева: высота справа
            var left = PartCorners(layout.LeftRect, model.Thickness, model.Height, layout.Scale);
            AddLinearDimension(document, page, Node(left.Right, left.Bottom), Node(left.Right, left.Top),
                DimensionAlignType.Vertical, DimensionOffset, layout.Scale);

            //вид сверху: толщина слева (вертикально)
            var top = PartCorners(layout.TopRect, model.Length, model.Thickness, layout.Scale);
            AddLinearDimension(document, page, Node(top.Left, top.Bottom), Node(top.Left, top.Top),
                DimensionAlignType.Vertical, -DimensionOffset, layout.Scale);

            //диаметр отверстия в центре главного вида
            if (model.HasHole)
            {
                double radius = model.HoleDiameter / 2 * layout.Scale;
                //видимая окружность для привязки диаметрального размера
                FreeNode holeCenter = Node(front.CenterX, front.CenterY);
                CircleConstruction holeCircle = new(document)
                {
                    Layer = nodeLayer,
                    Page = page,
                };
                holeCircle.SetCenterAndRadius(holeCenter, radius);

                //45° — наклон размерной линии, DimensionOffset — вынос числа за окружность
                double angle = Math.PI / 4;
                CircularDimension holeDim = new(document) 
                { 
                    Page = page,
                    LeaderDirection = DimensionLeaderDirection.Left,
                    ScaleFactorType = DimensionScaleFactorType.Custom,
                    ScaleFactor = 1 / layout.Scale,
                };
                holeDim.SetDiametral(holeCircle, DiametralDimensionType.Normal, null, angle, DimensionOffset);                
            }

            document.EndChanges();
        }

        // Углы детали на листе: от центра габаритного прямоугольника вида откладываем половину размера детали в масштабе листа
        internal static (double Left, double Right, double Bottom, double Top, double CenterX, double CenterY) PartCorners(
            TFlex.Drawing.Rectangle viewRect, double modelWidth, double modelHeight, double scale)
        {
            double centerX = viewRect.Left + viewRect.Width / 2;
            double centerY = viewRect.Bottom + viewRect.Height / 2;
            double halfWidth = modelWidth * scale / 2;
            double halfHeight = modelHeight * scale / 2;

            return (centerX - halfWidth, centerX + halfWidth, centerY - halfHeight, centerY + halfHeight, centerX, centerY);
        }

        // Линейный размер между двумя узлами с учётом масштаба вида
        private static LinearDimension AddLinearDimension(Document document, Page page, Node startNode, Node endNode,
            DimensionAlignType align, double offset, double scale)
        {
            LinearDimension dimension = new(document) { Page = page };
            dimension.SetTwoNodes(startNode, endNode, align);
            dimension.SetOffsets(null, offset, null, 0, null, 0);

            //узлы стоят в мм листа, поэтому возвращаем значение к натуральному
            dimension.ScaleFactorType = DimensionScaleFactorType.Custom;
            dimension.ScaleFactor = 1 / scale;
            return dimension;
        }

        // Вставляем форматку с основной надписью и возвращаем её фрагмент
        private static Fragment AddFirstSheetStamp(Document document)
        {
            document.BeginChanges("Добавление форматки на первый лист");
            Fragment firstSheetStamp = new(document, FirstSheetStampPath);
            document.EndChanges();
            return firstSheetStamp;
        }

        // Записываем масштаб в основную надпись
        private static void SetStampScale(Document document, Fragment stamp, string scaleText)
        {
            document.BeginChanges("Масштаб в основной надписи");
            SetStampVariable(stamp, "$maschtab", scaleText);
            document.EndChanges();
        }

        // Заполняем основную надпись: обозначение и наименование детали
        private static void FillStamp(Document document, Fragment stamp, Model model)
        {
            document.BeginChanges("Заполнение основной надписи");
            SetStampVariable(stamp, "$oboznach", model.Designation);
            SetStampVariable(stamp, "$naimen1", model.PartName);
            document.EndChanges();
        }

        // Присваиваем текстовое значение внешней переменной форматки
        private static void SetStampVariable(Fragment stamp, string name, string value)
        {
            foreach (FragmentVariableValue variable in stamp.GetVariables())
            {
                if (string.Equals(variable.Name, name, StringComparison.OrdinalIgnoreCase))
                {
                    variable.TextValue = value;
                    return;
                }
            }
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
