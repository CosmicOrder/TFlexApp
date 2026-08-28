using System;
using System.Collections.Generic;
using System.Text;

namespace TFlexApp
{
    public class Model
    {
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
                return $"АБВГ-{Length}-{Height}-{Thickness}{holePart}";
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
