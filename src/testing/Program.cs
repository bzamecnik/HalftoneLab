using System;
using HalftoneLab;
using Gtk;
using HalftoneLab.GUI.Gtk;

namespace testing
{
    class Program
    {
        static void Main(string[] args) {
            //ConfigManagerTest.run();
            //ConfigGUI.run();
            //SubmoduleVisitor.printModules(SubmoduleVisitor.listModules());
            ModuleRegistry regitry = ModuleRegistry.Instance;
        }
    }

    class ConfigGUI {
        public static void run() {
            Application.Init();

            Window window = new Window("Halftone Laboratory");
            window.DeleteEvent += new DeleteEventHandler(
                delegate { Application.Quit(); });

            //HalftoneMethod module;
            //SubmoduleSelector<HalftoneMethod>
            //    halftoneAlgorithmSelector =
            //    new SubmoduleSelector<HalftoneMethod>();
            //halftoneAlgorithmSelector.ModuleChanged += delegate
            //{
            //    module = halftoneAlgorithmSelector.Module;
            //    Console.WriteLine(module);
            //};

            //Table table = new Table(1, 2, false)
            //    { ColumnSpacing = 5, RowSpacing = 5, BorderWidth = 5 };
            //table.Attach(new Label("Halftone algorithm"), 0, 1, 0, 1,
            //    AttachOptions.Fill, AttachOptions.Shrink, 0, 0);
            //table.Attach(halftoneAlgorithmSelector, 1, 2, 0, 1,
            //    AttachOptions.Fill | AttachOptions.Expand,
            //    AttachOptions.Shrink, 0, 0);
            //table.ShowAll();

            //window.Add(table);

            // ---------- ok ----------------

            Table table = new Table(2, 1, false);

            HalftoneAlgorithm algorithm = null;

            ConfigManager config = new ConfigManager()
            {
                ConfigFileName = "halftonelab.cfg"
            };
            config.load();

            ConfigPanel<HalftoneAlgorithm> configPanel =
                new ConfigPanel<HalftoneAlgorithm>(config);
            HalftoneAlgorithmPanel algorithmPanel =
                new HalftoneAlgorithmPanel(algorithm);

            configPanel.ModuleChanged += delegate
            {
                algorithmPanel.Module = configPanel.CurrentModule;
            };
            algorithmPanel.ModuleChanged += delegate
            {
                algorithm = algorithmPanel.Module;
                configPanel.CurrentModule = algorithm;
                Console.WriteLine(algorithm);
            };

            table.Attach(configPanel, 0, 1, 0, 1, AttachOptions.Fill,
                AttachOptions.Shrink, 0, 0);
            table.Attach(algorithmPanel, 0, 1, 1, 2, AttachOptions.Fill |
                AttachOptions.Expand, AttachOptions.Shrink, 0, 0);

            window.Add(table);

            window.ShowAll();

            Application.Run();
        }
    }

}
