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
            AppDomain.CurrentDomain.UnhandledException += (sender, e) =>
            {
                var ex = e.ExceptionObject as Exception;
                System.IO.File.AppendAllText(
                    System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "crash.log"),
                    $"[{DateTime.Now}] UnhandledException (IsTerminating={e.IsTerminating}):\n{ex}\n\n");
                System.Windows.Forms.MessageBox.Show(
                    "Необработанное исключение:\n\n" + (ex?.ToString() ?? e.ExceptionObject?.ToString() ?? "null"),
                    "Крах", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
            };

            try
            {
                APILoader.Initialize();
                RunApplication();
            }
            catch (AccessViolationException ex)
            {
                System.IO.File.AppendAllText(
                    System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "crash.log"),
                    $"[{DateTime.Now}] AccessViolationException:\n{ex}\n\n");
                System.Windows.Forms.MessageBox.Show("AccessViolation (нативный крах T-Flex):\n\n" + ex,
                    "Крах", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                System.IO.File.AppendAllText(
                    System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "crash.log"),
                    $"[{DateTime.Now}] {ex.GetType().Name}:\n{ex}\n\n");
                System.Windows.Forms.MessageBox.Show(ex.ToString(), "Крах",
                    System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
            }
        }

        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.NoInlining)]
        private static void RunApplication()
        {
            APILoader.InitializeTFlexCADAPI();

            ApplicationConfiguration.Initialize();

            // 1. Создание View
            var form = new Form1();
            form.InitializeTFlexControl();

            // 2. Создаём CadFacade
            var cadFacade = new CadFacade(form.tfControl!);

            // 3. Создаём презентер
            _ = new Presenter(form, cadFacade);

            // 4. Запуск приложения
            Application.Run(form);

            // 5. Завершение работы T-Flex CAD API
            APILoader.Terminate();
        }
    }
}