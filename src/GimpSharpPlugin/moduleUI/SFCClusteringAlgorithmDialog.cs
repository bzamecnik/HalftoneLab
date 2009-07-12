using System;
using Gtk;
using Halftone;

namespace Gimp.HalftoneLab
{
    public class SFCClusteringMethodDialog : ConfigDialog
    {
        private SFCClusteringMethod module;
        private Table table;
        private SubmoduleSelector<VectorErrorFilter> errorFilterSelector;
        private CheckButton useErrorFilterCheckButton;
        private SubmoduleSelector<SFCScanningOrder> scanningOrderSelector;
        private SpinButton maxCellSizeSpinButton;
        private SpinButton minCellSizeSpinButton;
        private CheckButton clusterPositioningCheckButton;
        private CheckButton adaptiveClusteringCheckButton;
        

        public SFCClusteringMethodDialog()
            : this(new SFCClusteringMethod()) { }

        public SFCClusteringMethodDialog(
            SFCClusteringMethod existingModule)
            : base(existingModule)
        {
            module = modifiedModule as SFCClusteringMethod;
            if (module == null) {
                modifiedModule = new SFCClusteringMethod();
                module = modifiedModule as SFCClusteringMethod;
            }

            errorFilterSelector = new SubmoduleSelector<VectorErrorFilter>(
                module.ErrorFilter);
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
            useErrorFilterCheckButton.Toggled += delegate
            {
                module.UseErrorFilter =
                    useErrorFilterCheckButton.Active;
            };

            scanningOrderSelector = new SubmoduleSelector<SFCScanningOrder>(
                module.ScanningOrder);
            scanningOrderSelector.ModuleChanged += delegate
            {
                module.ScanningOrder = scanningOrderSelector.Module;
            };

            maxCellSizeSpinButton = new SpinButton(1, 100, 1)
            {
                Numeric = true
            };
            maxCellSizeSpinButton.Value = module.MaxCellSize;
            maxCellSizeSpinButton.Changed += delegate
            {
                module.MaxCellSize = maxCellSizeSpinButton.ValueAsInt;
            };

            minCellSizeSpinButton = new SpinButton(1, 99, 1)
            {
                Numeric = true
            };
            minCellSizeSpinButton.Value = module.MinCellSize;
            minCellSizeSpinButton.Changed += delegate
            {
                module.MinCellSize = minCellSizeSpinButton.ValueAsInt;
            };

            clusterPositioningCheckButton =
                new CheckButton("Use cluster positioning?");
            clusterPositioningCheckButton.Active =
                module.UseClusterPositioning;
            clusterPositioningCheckButton.Toggled += delegate
            {
                module.UseClusterPositioning =
                    clusterPositioningCheckButton.Active;
            };

            adaptiveClusteringCheckButton =
                new CheckButton("Use adaptive clustering?");
            adaptiveClusteringCheckButton.Active =
                module.UseAdaptiveClustering;
            adaptiveClusteringCheckButton.Toggled += delegate
            {
                module.UseAdaptiveClustering =
                    adaptiveClusteringCheckButton.Active;
            };

            table = new Table(7, 2, false)
                { ColumnSpacing = 5, RowSpacing = 5, BorderWidth = 5 };

            table.Attach(new Label("Error filter:") { Xalign = 0.0f },
                0, 1, 0, 1, AttachOptions.Fill, AttachOptions.Shrink, 0, 0);
            table.Attach(errorFilterSelector, 1, 2, 0, 1,
                AttachOptions.Fill | AttachOptions.Expand,
                AttachOptions.Shrink, 0, 0);

            table.Attach(useErrorFilterCheckButton, 0, 2, 1, 2,
                AttachOptions.Fill | AttachOptions.Expand,
                AttachOptions.Shrink, 0, 0);

            table.Attach(new Label("Scanning order:") { Xalign = 0.0f },
                0, 1, 2, 3, AttachOptions.Fill, AttachOptions.Shrink,
                0, 0);
            table.Attach(scanningOrderSelector, 1, 2, 2, 3,
                AttachOptions.Fill | AttachOptions.Expand,
                AttachOptions.Shrink, 0, 0);

            table.Attach(new Label("Max cell size:") { Xalign = 0.0f },
                0, 1, 3, 4, AttachOptions.Fill, AttachOptions.Shrink, 0, 0);
            table.Attach(maxCellSizeSpinButton, 1, 2, 3, 4,
                AttachOptions.Fill | AttachOptions.Expand,
                AttachOptions.Shrink, 0, 0);

            table.Attach(new Label("Min cell size:") { Xalign = 0.0f },
                0, 1, 4, 5, AttachOptions.Fill, AttachOptions.Shrink, 0, 0);
            table.Attach(minCellSizeSpinButton, 1, 2, 4, 5,
                AttachOptions.Fill | AttachOptions.Expand,
                AttachOptions.Shrink, 0, 0);

            table.Attach(adaptiveClusteringCheckButton, 0, 2, 5, 6,
                AttachOptions.Fill | AttachOptions.Expand,
                AttachOptions.Shrink, 0, 0);

            table.Attach(clusterPositioningCheckButton, 0, 2, 6, 7,
                AttachOptions.Fill | AttachOptions.Expand,
                AttachOptions.Shrink, 0, 0);

            table.ShowAll();
            VBox.PackStart(table);
        }
    }
}
