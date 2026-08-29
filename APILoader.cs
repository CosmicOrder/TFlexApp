using Microsoft.Win32;

namespace TFlexApp
{
    internal static class APILoader
    {
        private static string? _tflexPath;

        static public void Initialize()
        {
            if (_tflexPath != null)
                return;

            _tflexPath = GetPath(@"T-FLEX CAD 3D 17\Rus");
            if (string.IsNullOrEmpty(_tflexPath))
                throw new System.IO.FileNotFoundException("T-FLEX CAD не найден в реестре. Убедитесь, что T-FLEX CAD 17 установлен.");

            var currentPath = Environment.GetEnvironmentVariable("PATH") ?? "";
            Environment.SetEnvironmentVariable("PATH", _tflexPath + ";" + currentPath);

            System.IO.Directory.SetCurrentDirectory(_tflexPath);

            AppDomain.CurrentDomain.AssemblyResolve += AssemblyResolve;
        }

        static public bool InitializeTFlexCADAPI()
        {
            if (_tflexPath == null)
                throw new InvalidOperationException("Call Initialize first");

            // Предзагружаем TFlexAPI3D для .NET 10
            string tflexApi3DPath = System.IO.Path.Combine(_tflexPath, "TFlexAPI3D.dll");
            if (System.IO.File.Exists(tflexApi3DPath))
            {
                System.Reflection.Assembly.LoadFrom(tflexApi3DPath);
            }

            //Перед работой с API T-FLEX CAD его необходимо инициализировать
            //В зависимости от параметров инициализации, будут или не будут
            //доступны функции изменения документов и сохранение документов в файл.
            //За это отвечает параметр setup.ReadOnly.
            //Если setup.ReadOnly = false, то для работы программы требуется
            //лицензия на сам T-FLEX CAD
            TFlex.ApplicationSessionSetup setup = new TFlex.ApplicationSessionSetup();
            setup.ReadOnly = true;
            setup.Enable3D = true;
            setup.ProtectionLicense = TFlex.ApplicationSessionSetup.License.TFlexAPI;
            return TFlex.Application.InitSession(setup);
        }

        static public void Terminate()
        {
            if (_tflexPath == null)
                return;

            TFlex.Application.ExitSession();

            _tflexPath = null;
        }

        static private string GetPath(string product)
        {
            if (string.IsNullOrEmpty(product))
                return "";

            string regPath = string.Format(@"SOFTWARE\Top Systems\{0}\", product);

            RegistryKey? key = Registry.LocalMachine.OpenSubKey(regPath, RegistryKeyPermissionCheck.ReadSubTree, System.Security.AccessControl.RegistryRights.ReadKey);
            if (key == null)
                return "";

            string path = (string)key.GetValue("ProgramFolder", string.Empty);
            if (string.IsNullOrEmpty(path))
                path = (string)key.GetValue("SetupHelpPath", string.Empty);

            key.Close();

            if (path.Length > 0 && path[path.Length - 1] != '\\')
                path += @"\";

            return path;
        }

        static private System.Reflection.Assembly? AssemblyResolve(object? sender, ResolveEventArgs args)
        {
            if (string.IsNullOrEmpty(_tflexPath))
                return null;

            try
            {
                string name = args.Name;
                int index = name.IndexOf(",");
                if (index > 0)
                    name = name.Substring(0, index);

                string fileName = string.Format("{0}{1}.dll", _tflexPath, name);

                if (!System.IO.File.Exists(fileName))
                    return null;

                return System.Reflection.Assembly.LoadFrom(fileName);
            }
            catch
            {
                return null;
            }
        }
    }
}
