namespace TFlexApp
{
    public interface ICadFacade
    {
        /// <summary>
        /// Создаёт документ 3D-модель, стандартные проекции с размерами и заполняемую основную надпись.
        /// </summary>
        void CreatePartDrawing(Model model);
    }
}
