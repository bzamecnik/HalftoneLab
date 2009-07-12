using System;
using Gtk;
using Halftone;

namespace Gimp.HalftoneLab
{
    public class ThresholdHalftoneMethodDialog : ConfigDialog
    {
        private ThresholdHalftoneMethod module;
        private SubmoduleSelector<ThresholdFilter> thresholdFilterSelector;
        private SubmoduleSelector<ErrorFilter> errorFilterSelector;
        private CheckButton useErrorFilterCheckButton;
        private SubmoduleSelector<ScanningOrder> scanningOrderSelector;
        private Table table;

        public ThresholdHalftoneMethodDialog()
            : this(new ThresholdHalftoneMethod()) { }

        public ThresholdHalftoneMethodDialog(
            ThresholdHalftoneMethod existingModule)
            : base(existingModule)
        {
            module = modifiedModule as ThresholdHalftoneMethod;
            if (module == null) {
                modifiedModule = new ThresholdHalftoneMethod();
                module = modifiedModule as ThresholdHalftoneMethod;
            }

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
                useErrorFilterCheckButton.Active &=
                    errorFilterSelector.Module != null;
                useErrorFilterCheckButton.Sensitive =
                    errorFilterSelector.Module != null;
            };

            useErrorFilterCheckButton = new CheckButton(
                "Use error filter?");
            useErrorFilterCheckButton.Active =
                module.UseErrorFilter;
            useErrorFilterCheckButton.Sensitive =
                    errorFilterSelector.Module != null;
            useErrorFilterCheckButton.Toggled += delegate
            {
                module.UseErrorFilter =
                    useErrorFilterCheckButton.Active;
            };

            scanningOrderSelector = new SubmoduleSelector<ScanningOrder>(
                module.ScanningOrder);
            scanningOrderSelector.ModuleChanged += delegate
            {
                module.ScanningOrder = scanningOrderSelector.Module;
            };

            table = new Table(3, 2, false)
                { ColumnSpacing = 5, RowSpacing = 5, BorderWidth = 5 };
            
            table.Attach(new Label("Threshold filter:") { Xalign = 0.0f}, 0, 1, 0, 1,
                AttachOptions.Fill, AttachOptions.Shrink, 0, 0);
            table.Attach(thresholdFilterSelector, 1, 2, 0, 1,
                AttachOptions.Fill | AttachOptions.Expand,
                AttachOptions.Shrink, 0, 0);

            table.Attach(new Label("Error filter:") { Xalign = 0.0f }, 0, 1, 1, 2,
                AttachOptions.Fill, AttachOptions.Shrink, 0, 0);
            table.Attach(errorFilterSelector, 1, 2, 1, 2,
                AttachOptions.Fill | AttachOptions.Expand,
                AttachOptions.Shrink, 0, 0);

            table.Attach(useErrorFilterCheckButton, 0, 2, 2, 3,
                AttachOptions.Fill | AttachOptions.Expand,
                AttachOptions.Shrink, 0, 0);

            table.Attach(new Label("Scanning order:") { Xalign = 0.0f },
                0, 1, 3, 4, AttachOptions.Fill, AttachOptions.Shrink, 0, 0);
            table.Attach(scanningOrderSelector, 1, 2, 3, 4,
                AttachOptions.Fill | AttachOptions.Expand,
                AttachOptions.Shrink, 0, 0);

            table.ShowAll();
            VBox.PackStart(table);
        }
    }
}
