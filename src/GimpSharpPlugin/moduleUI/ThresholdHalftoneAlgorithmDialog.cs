using System;
using Gtk;
using Halftone;

namespace Gimp.HalftoneLab
{
    public class ThresholdHalftoneAlgorithmDialog : ConfigDialog
    {
        private ThresholdHalftoneAlgorithm module;
        private Label thresholdFilterLabel;
        private Label errorFilterLabel;
        private Label scanningOrderLabel;
        private SubmoduleSelector<ThresholdFilter> thresholdFilterSelector;
        private SubmoduleSelector<ErrorFilter> errorFilterSelector;
        private SubmoduleSelector<ScanningOrder> scanningOrderSelector;
        private Table table;

        public ThresholdHalftoneAlgorithmDialog()
            : this(new ThresholdHalftoneAlgorithm()) { }

        public ThresholdHalftoneAlgorithmDialog(
            ThresholdHalftoneAlgorithm existingModule)
            : base(existingModule) {
            module = modifiedModule as ThresholdHalftoneAlgorithm;
            thresholdFilterLabel = new Label("Threshold filter");
            thresholdFilterLabel.Show();
            errorFilterLabel = new Label("Error filter");
            errorFilterLabel.Show();
            scanningOrderLabel = new Label("Scanning order");
            scanningOrderLabel.Show();

            thresholdFilterSelector = new SubmoduleSelector<ThresholdFilter>(
                module.ThresholdFilter);
            thresholdFilterSelector.ModuleChanged += delegate
            {
                module.ThresholdFilter = thresholdFilterSelector.Module;
            };

            errorFilterSelector = new SubmoduleSelector<ErrorFilter>(
                module.ErrorFilter)
                {
                    AllowNull = true
                };
            errorFilterSelector.ModuleChanged += delegate
            {
                module.ErrorFilter = errorFilterSelector.Module;
            };

            scanningOrderSelector = new SubmoduleSelector<ScanningOrder>(
                module.ScanningOrder);
            scanningOrderSelector.ModuleChanged += delegate
            {
                module.ScanningOrder = scanningOrderSelector.Module;
            };

            table = new Table(3, 2, false);
            table.Attach(thresholdFilterLabel, 0, 1, 0, 1,
                AttachOptions.Fill, AttachOptions.Shrink, 0, 0);
            table.Attach(thresholdFilterSelector, 1, 2, 0, 1,
                AttachOptions.Fill | AttachOptions.Expand,
                AttachOptions.Shrink, 0, 0);
            table.Attach(errorFilterLabel, 0, 1, 1, 2,
                AttachOptions.Fill, AttachOptions.Shrink, 0, 0);
            table.Attach(errorFilterSelector, 1, 2, 1, 2,
                AttachOptions.Fill | AttachOptions.Expand,
                AttachOptions.Shrink, 0, 0);
            table.Attach(scanningOrderLabel, 0, 1, 2, 3,
                AttachOptions.Fill, AttachOptions.Shrink, 0, 0);
            table.Attach(scanningOrderSelector, 1, 2, 2, 3,
                AttachOptions.Fill | AttachOptions.Expand,
                AttachOptions.Shrink, 0, 0);
            table.Show();
            VBox.PackStart(table);
        }
    }
}
