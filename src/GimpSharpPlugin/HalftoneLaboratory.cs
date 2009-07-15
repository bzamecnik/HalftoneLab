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
        private HalftoneAlgorithm selectedAlgorithm;
        private ConfigManager configManager;

        public static void Main(string[] args) {
            new HalftoneLaboratory(args);
        }

        HalftoneLaboratory(string[] args)
            : base(args, "HalftoneLaboratory")
        {
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

            Table table = new Table(2, 1, false);

            initConfigManager();

            ConfigPanel<HalftoneAlgorithm> configPanel =
                new ConfigPanel<HalftoneAlgorithm>(configManager);
            HalftoneAlgorithmPanel algorithmPanel =
                new HalftoneAlgorithmPanel(selectedAlgorithm);

            configPanel.ModuleChanged += delegate
            {
                algorithmPanel.Module = configPanel.CurrentModule;
            };
            algorithmPanel.ModuleChanged += delegate
            {
                selectedAlgorithm = algorithmPanel.Module;
                configPanel.CurrentModule = selectedAlgorithm;
            };

            table.Attach(configPanel, 0, 1, 0, 1, AttachOptions.Fill,
                AttachOptions.Shrink, 0, 0);
            table.Attach(algorithmPanel, 0, 1, 1, 2, AttachOptions.Fill |
                AttachOptions.Expand, AttachOptions.Shrink, 0, 0);

            dialog.VBox.PackStart(table);

            dialog.Response += delegate {
                // save the last used algorithm to the config manager
                selectedAlgorithm.Name = "_LAST";
                configManager.saveModule(selectedAlgorithm);
            };

            return dialog;
        }

        override protected void Render(Drawable drawable) {
            Halftone.Image image = new GSImage(drawable);
            if (configManager == null) {
                initConfigManager();
            }
            if (selectedAlgorithm != null) {
                Console.Out.WriteLine("Algorithm name: {0}", selectedAlgorithm.Name);
                Console.Out.WriteLine("Description: {0}", selectedAlgorithm.Description);
                selectedAlgorithm.run(image);
            }
        }

        private void initConfigManager() {
            configManager = new ConfigManager()
            {
                // TODO: find user home directory
                ConfigFileName = "halftonelab.cfg"
            };
            configManager.load();

            // load last used algorithm
            selectedAlgorithm = configManager.getModule<HalftoneAlgorithm>(
                "_LAST");
            if (selectedAlgorithm == null) {
                selectedAlgorithm = new HalftoneAlgorithm();
            }
        }
    }
}
