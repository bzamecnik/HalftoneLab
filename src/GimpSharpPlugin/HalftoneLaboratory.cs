using System;
using System.Collections.Generic;
using Gtk;
using HalftoneLab;
using HalftoneLab.GUI.Gtk;

namespace Gimp.HalftoneLaboratory
{
    /// <summary>
    /// Halftone Laboratory GIMP plugin.
    /// </summary>
    /// <remarks>
    /// A thin layer between HalftoneLab library and GUI on one side, and
    /// GIMP on the other side.
    /// </remarks>
    class HalftoneLaboratory : Plugin
    {
        /// <summary>
        /// Name of the last used algorithm configuration.
        /// </summary>
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

        /// <summary>
        /// Create a plug-in dialog.
        /// </summary>
        /// <remarks>
        /// Save the last used algorithm configuration to a file.
        /// </remarks>
        /// <returns>Gimp plug-in dialog</returns>
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
                Console.Out.WriteLine("Algorithm name: {0}", selectedAlgorithm.Name);
                Console.Out.WriteLine("Description: {0}", selectedAlgorithm.Description);
                // save the last used algorithm to the config manager
                selectedAlgorithm.Name = "_LAST";
                configManager.saveModule(selectedAlgorithm);
            };

            return dialog;
        }

        /// <summary>
        /// Run the halftoning algorithm.
        /// </summary>
        /// <remarks>
        /// The halftoning algorithm can be either selected inteactively or
        /// in case of non-interactive processing it is the previously used
        /// one.
        /// </remarks>
        /// <param name="drawable">The image - both input and output</param>
        override protected void Render(Drawable drawable) {
            HalftoneLab.Image image = new GSImage(drawable);
            if (configManager == null) {
                initConfigManager();
            }
            if (selectedAlgorithm != null) {
                selectedAlgorithm.run(image);
            }
        }

        /// <summary>
        /// Initialize the config manager.
        /// </summary>
        /// <remarks>
        /// Load the configurations from file, if possible, and get the last
        /// used algorithm.
        /// </remarks>
        private void initConfigManager() {
            configManager = new ConfigManager()
            {
                ConfigFileName =
                System.IO.Path.Combine(Environment.GetFolderPath(
                    Environment.SpecialFolder.ApplicationData),
                    "halftonelab.cfg")
            };
            configManager.load();

            // load the last used algorithm
            selectedAlgorithm = configManager.getModule<HalftoneAlgorithm>(
                "_LAST");
            if (selectedAlgorithm == null) {
                selectedAlgorithm = new HalftoneAlgorithm();
            }
        }
    }
}
