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
            //ModuleRegistry regitry = ModuleRegistry.Instance;
            //MatrixErrorBuffer buffer = new MatrixErrorBuffer(3, 5);
            //for (int i = 0; i < 31; i++) {
            //    buffer.moveNext();
            //}
            DynamicMatrixErrorFilter filter = new DynamicMatrixErrorFilter();
            filter.MatrixTable.addDefinitionRecord(
                new DynamicMatrixErrorFilter.ErrorRecord(0,
                    ErrorMatrix.Samples.floydSteinberg));
            filter.MatrixTable.addDefinitionRecord(
                new DynamicMatrixErrorFilter.ErrorRecord(100,
                    new ErrorMatrix(new int[,] {{0, 0}}, 0)));
            filter.MatrixTable.addDefinitionRecord(
                new DynamicMatrixErrorFilter.ErrorRecord(150,
                    ErrorMatrix.Samples.stucki));
            filter.init(new HalftoneLab.Image.ImageRunInfo()
                {
                    Height = 20,
                    Width = 30,
                    ScanOrder = new ScanlineScanningOrder()
                }
            );
            filter.setError(10, 50);
            filter.setError(20, 120);
            filter.setError(20, 160);
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
