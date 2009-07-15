using System;
using Gtk;
using Halftone;

namespace Gimp.HalftoneLab
{
    public class SFCClusteringMethodDialog : ModuleDialog
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
                useErrorFilterCheckButton.Active =
                    errorFilterSelector.Module != null;
                useErrorFilterCheckButton.Sensitive =
                    errorFilterSelector.Module != null;
            };

            useErrorFilterCheckButton = new CheckButton("Use error filter?");
            useErrorFilterCheckButton.Active = module.UseErrorFilter;

            scanningOrderSelector = new SubmoduleSelector<SFCScanningOrder>(
                module.ScanningOrder);

            maxCellSizeSpinButton = new SpinButton(1, 100, 1);
            maxCellSizeSpinButton.Value = module.MaxCellSize;

            minCellSizeSpinButton = new SpinButton(1, 99, 1);
            minCellSizeSpinButton.Value = module.MinCellSize;

            clusterPositioningCheckButton =
                new CheckButton("Use cluster positioning?");
            clusterPositioningCheckButton.Active =
                module.UseClusterPositioning;

            adaptiveClusteringCheckButton =
                new CheckButton("Use adaptive clustering?");
            adaptiveClusteringCheckButton.Active =
                module.UseAdaptiveClustering;

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

        protected override void save() {
            module.ErrorFilter = errorFilterSelector.Module;
            module.UseErrorFilter =
                    useErrorFilterCheckButton.Active;
            module.ScanningOrder = scanningOrderSelector.Module;
            module.MaxCellSize = maxCellSizeSpinButton.ValueAsInt;
            module.MinCellSize = minCellSizeSpinButton.ValueAsInt;
            module.UseAdaptiveClustering =
                    adaptiveClusteringCheckButton.Active;
            module.UseClusterPositioning =
                    clusterPositioningCheckButton.Active;
        }
    }
}
