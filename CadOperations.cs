using System.Collections.ObjectModel;
using System.Reflection.Metadata;
using TFlex;
using TFlex.Model;
using TFlex.Model.Model2D;
using TFlex.Model.Model3D;

namespace TFlexApp
{
    public class CadOperations: ICadOperations
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
            StandardWorkplane swp = new(document, StandardWorkplane.StandardType.Left);

            //построение профиля для выдавливания на рабочей плоскости
            AreaProfile areaProfile = new(document) { Area = area, WorkSurface = swp };

            //операция выдавливания на заданную толщину
            ThickenExtrusion part_ext = new(document)
            {
                LengthType = ThickenExtrusion.LengthValue.AutoValue,
                ForwardLength = model.Thickness,
            }; 
            part_ext.Profile.Add(areaProfile.Geometry.SheetContour);

            document.EndChanges();
            //привязываем документ к TFlex.Control для отображения
            if (_tfControl != null)
            {
                _tfControl.Document = document;
                _tfControl.RefreshTabs();
                _tfControl.Invalidate(true);
            }
        }

    }
}
