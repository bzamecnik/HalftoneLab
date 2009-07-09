using System;
using Gtk;
using Halftone;

namespace Gimp.HalftoneLab
{
    public class MatrixThresholdFilterDialog : ConfigDialog
    {
        private MatrixThresholdFilter module;
        private Button thresholdMatrixEditButton;
        private Table table;

        public MatrixThresholdFilterDialog()
            : this(new MatrixThresholdFilter()) { }

        public MatrixThresholdFilterDialog(
            MatrixThresholdFilter existingModule)
            : base(existingModule)
        {
            module = modifiedModule as MatrixThresholdFilter;
            if (module == null) {
                modifiedModule = new MatrixThresholdFilter();
                module = modifiedModule as MatrixThresholdFilter;
            }

            thresholdMatrixEditButton = new Button("Edit");
            thresholdMatrixEditButton.Clicked += delegate
            {
                if (module != null) {
                    ThresholdMatrix configuredMatrix = null;
                    configuredMatrix = ConfigDialog.configureModule(
                        "ThresholdMatrix", module.Matrix)
                            as ThresholdMatrix;
                    if (configuredMatrix != null) {
                        module.Matrix = configuredMatrix;
                    }
                }
            };

            table = new Table(2, 2, false)
                { ColumnSpacing = 5, RowSpacing = 5, BorderWidth = 5 };
            table.Attach(new Label("Threshold matrix") { Xalign = 0.0f },
                0, 1, 0, 1, AttachOptions.Fill, AttachOptions.Shrink,
                0, 0);
            table.Attach(thresholdMatrixEditButton, 1, 2, 0, 1,
                AttachOptions.Fill | AttachOptions.Expand,
                AttachOptions.Shrink, 0, 0);
            table.ShowAll();
            VBox.PackStart(table);
        }
    }
}
