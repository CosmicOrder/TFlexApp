using System;
using System.IO;
using System.Windows.Forms;
using TFlex;
using TFlex.Model;

namespace TFlexApp
{
    public class Factory : PluginFactory
    {
        public override Plugin CreateInstance()
        {
            return new TFlexAppPlugin(this);
        }

        public override Guid ID
        {
            get { return new Guid("{B4B70ED3-8AD9-4D92-BCD1-B1AC6A2D74A9}"); }
        }

        public override string Name
        {
            get { return "TFlexApp - Генератор деталей"; }
        }
    }

    internal enum Commands
    {
        BuildPart = 1,
    }

    public class TFlexAppPlugin : Plugin
    {
        public TFlexAppPlugin(Factory factory) : base(factory)
        {
        }

        protected override void OnInitialize()
        {
            base.OnInitialize();

            string? tflexPath = Path.GetDirectoryName(typeof(TFlex.Application).Assembly.Location);
            if (!string.IsNullOrEmpty(tflexPath) && Directory.Exists(tflexPath))
                Directory.SetCurrentDirectory(tflexPath);
        }

        protected override void OnCreateTools()
        {
            RegisterCommand((int)Commands.BuildPart, "Построить деталь", null, null);

            TFlex.Menu submenu = new TFlex.Menu();
            submenu.CreatePopup();
            submenu.Append((int)Commands.BuildPart, "&Построить деталь", this);
            TFlex.Application.ActiveMainWindow.InsertPluginSubMenu(
                "Генератор деталей", submenu, MainWindow.InsertMenuPosition.PluginSamples, this);

            TFlex.RibbonGroup ribbonGroup = TFlex.RibbonBar.ApplicationsTab.AddGroup("Генератор деталей");
            ribbonGroup.AddButton((int)Commands.BuildPart, this);
        }

        protected override void OnCommand(Document document, int id)
        {
            if (id == (int)Commands.BuildPart)
            {
                Form1 form = new Form1();
                form.InitializeTFlexControl();

                CadFacade cadFacade = new CadFacade(form.tfControl!);
                new Presenter(form, cadFacade);

                form.ShowDialog();
            }
            else
            {
                base.OnCommand(document, id);
            }
        }
    }
}
