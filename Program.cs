namespace TFlexApp
{
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.

            // Инициализация визуальных стилей Windows
            ApplicationConfiguration.Initialize();

            // 1. Создаем View (Форму)
            var form = new Form1();

            // 2. Создаем Presenter и передаем ему форму
            _ = new Presenter(form);

            Application.Run(form);
        }
    }
}