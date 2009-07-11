using System;
using System.Collections.Generic;
using Gtk;
using Halftone;

namespace Gimp.HalftoneLab
{
    /// <summary>
    /// Halftone Laboratory user interface as a GIMP plugin.
    /// </summary>
    class HalftoneLaboratory : Plugin
    {
        private List<HalftoneAlgorithm> algorithms;
        private HalftoneAlgorithm selectedAlgorithm;
        //[SaveAttribute("algorithm")]
        private string selectedAlgorithmName;
        private ConfigManager configManager;

        public static void Main(string[] args) {
            new HalftoneLaboratory(args);
        }

        HalftoneLaboratory(string[] args)
            : base(args, "HalftoneLaboratory")
        {
            selectedAlgorithmName = "";
            Console.Out.WriteLine("HalftoneLaboratory()");
        }

        override protected IEnumerable<Procedure> ListProcedures() {
            yield return new Procedure("plug_in_halftone_laboratory",
                    _("Halftone Laboratory"),
                    _("Halftone Laboratory"),
                    "Bohumir Zamecnik",
                    "(C) Bohumir Zamecnik",
                    "2009",
                    _("Halftone Laboratory"),
                    "GRAY*") { MenuPath = "<Image>/Filters/Distorts" };
        }

        override protected GimpDialog CreateDialog() {
            gimp_ui_init("HalftoneLab", true);

            GimpDialog dialog = DialogNew("Halftone Laboratory", "HalftoneLab",
                IntPtr.Zero, 0, Gimp.StandardHelpFunc, "HalftoneLab");

            //VBox vbox = new VBox(false, 12) { BorderWidth = 12 };
            //dialog.VBox.PackStart(vbox, true, true, 0);

            //loadAlgorithms();
            //ComboBox algorithmCombo = ComboBox.NewText();
            //foreach (HalftoneAlgorithm alg in algorithms) {
            //    algorithmCombo.AppendText(alg.Name);
            //}
            //algorithmCombo.Changed += delegate
            //{
            //    selectedAlgorithm = algorithms.Find(
            //        alg => alg.Name == algorithmCombo.ActiveText);
            //    if (selectedAlgorithm != null) {
            //        selectedAlgorithmName = selectedAlgorithm.Name;
            //    }
            //};
            //algorithmCombo.Active = 0;

            //vbox.PackStart(algorithmCombo, false, false, 0);

            //Button editButton = new Button("Edit ThresholdHalftoneAlgorithm");
            //editButton.Clicked += delegate
            //{
            //    ThresholdHalftoneAlgorithmDialog algDialog =
            //        new ThresholdHalftoneAlgorithmDialog();
            //    ThresholdHalftoneAlgorithm module =
            //        algDialog.runConfiguration() as ThresholdHalftoneAlgorithm;
            //    Console.WriteLine(module);
            //};
            //editButton.Show();
            //dialog.VBox.PackStart(editButton);

            SubmoduleSelector<HalftoneAlgorithm>
                halftoneAlgorithmSelector =
                new SubmoduleSelector<HalftoneAlgorithm>();
            halftoneAlgorithmSelector.ModuleChanged += delegate
            {
                selectedAlgorithm = halftoneAlgorithmSelector.Module;
                Console.WriteLine(selectedAlgorithm);
            };

            Table table = new Table(1, 2, false)
                { ColumnSpacing = 5, RowSpacing = 5, BorderWidth = 5 };
            table.Attach(new Label("Halftone algorithm"), 0, 1, 0, 1,
                AttachOptions.Fill, AttachOptions.Shrink, 0, 0);
            table.Attach(halftoneAlgorithmSelector, 1, 2, 0, 1,
                AttachOptions.Fill | AttachOptions.Expand,
                AttachOptions.Shrink, 0, 0);
            table.ShowAll();

            dialog.VBox.PackStart(table);

            return dialog;
        }

        override protected void Render(Drawable drawable) {
            Halftone.Image image = new GSImage(drawable);
            if (algorithms == null) {
                loadAlgorithms();
            }
            if ((selectedAlgorithm == null) && (algorithms != null)) {
                selectedAlgorithm = algorithms.Find(
                    alg => alg.Name == selectedAlgorithmName);
            }
            if (selectedAlgorithm != null) {
                Console.Out.WriteLine("algorithm name: {0}", selectedAlgorithm.Name);
                Console.Out.WriteLine("description: {0}", selectedAlgorithm.Description);
                selectedAlgorithm.run(image);
            }
        }

        private void loadAlgorithms() {
            Console.Out.WriteLine("loadAlgorithms()");
            configManager = new ConfigManager() { ConfigFileName = "halftonelab.cfg" };
            configManager.load();
            algorithms = configManager.findAllModules(
                module => module is HalftoneAlgorithm
                ).ConvertAll<HalftoneAlgorithm>(module => module as HalftoneAlgorithm);
        }
    }
}
