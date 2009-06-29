using System;
using System.Collections.Generic;
using Halftone;

using Gtk;

namespace Gimp.HalftoneLab
{
    class HalftoneLaboratory : Plugin
    {
        private List<DitherAlgorithm> algorithms;
        private DitherAlgorithm selectedAlgorithm;
        [SaveAttribute("algorithm")]
        private string selectedAlgorithmName;
        private ConfigManager configManager;

        public static void Main(string[] args) {
            new HalftoneLaboratory(args);
        }

        HalftoneLaboratory(string[] args)
            : base(args, "HalftoneLaboratory")
        {
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

            VBox vbox = new VBox(false, 12) { BorderWidth = 12 };
            dialog.VBox.PackStart(vbox, true, true, 0);

            loadAlgorithms();
            ComboBox algorithmCombo = ComboBox.NewText();
            foreach (DitherAlgorithm alg in algorithms) {
                algorithmCombo.AppendText(alg.Name);
            }
            algorithmCombo.Changed += delegate
            {
                selectedAlgorithm = algorithms.Find(
                    alg => alg.Name == algorithmCombo.ActiveText);
                if (selectedAlgorithm != null) {
                    selectedAlgorithmName = selectedAlgorithm.Name;
                }
            };
            algorithmCombo.Active = 0;

            vbox.PackStart(algorithmCombo, false, false, 0);

            return dialog;
        }

        override protected void Render(Drawable drawable) {
            //RunProcedure("plug_in_pixelize", drawable.Image, drawable, 4);
            //return;

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

            ////DynamicMatrixErrorFilter dynamicMatrixErrorFilter = new DynamicMatrixErrorFilter();
            ////dynamicMatrixErrorFilter.addRecord(0, ErrorMatrix.Samples.floydSteinberg);
            ////dynamicMatrixErrorFilter.addRecord(64, ErrorMatrix.Samples.jarvisJudiceNinke);
            ////dynamicMatrixErrorFilter.addRecord(128, ErrorMatrix.Samples.stucki);
            ////dynamicMatrixErrorFilter.addRecord(192, ErrorMatrix.Samples.nextPixel);

            //TresholdDitherAlgorithm tresholdDitherAlgorithm = new TresholdDitherAlgorithm();
            //tresholdDitherAlgorithm.ErrorFilter = new MatrixErrorFilter(ErrorMatrix.Samples.stucki);
            ////tresholdDitherAlgorithm.ErrorFilter = new PerturbedErrorFilter(
            ////    new MatrixErrorFilter(ErrorMatrix.Samples.stucki))
            ////    {
            ////        PerturbationAmplitude = 0.5
            ////    };
            ////tresholdDitherAlgorithm.ErrorFilter = new RandomizedMatrixErrorFilter(ErrorMatrix.Samples.floydSteinberg);
            //tresholdDitherAlgorithm.run(image);
        }

        private void loadAlgorithms() {
            Console.Out.WriteLine("loadAlgorithms()");
            configManager = new ConfigManager() { ConfigFileName = "halftonelab.cfg" };
            configManager.load();
            algorithms = configManager.findAllModules(
                module => module is DitherAlgorithm
                ).ConvertAll<DitherAlgorithm>(module => module as DitherAlgorithm);
        }
    }
}
