namespace TFlexApp
{
    public class Model
    {
        private const string DesignationPrefix = "АБВГ";

        // Свойства для хранения габаритов детали
        public double Length { get; set; }
        public double Height { get; set; }
        public double Thickness { get; set; }
        public double HoleDiameter { get; set; }
        public bool HasHole { get; set; }

        // Метод для генерации обозначения детали
        public string Designation
        {
            get
            {
                string holePart = HasHole ? $"-D{HoleDiameter}" : "";
                return $"{DesignationPrefix}-{Length}-{Height}-{Thickness}{holePart}";
            }
        }
        // Метод для генерации наименования детали
        public string PartName
        {
            get
            {
                string holePart = HasHole ? $" с отверстием {HoleDiameter}" : "";
                return $"деталь {Length}*{Height}*{Thickness}{holePart}";
            }
        } 
    }
}
