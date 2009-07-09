using System;
using Gtk;
using Halftone;

namespace Gimp.HalftoneLab
{
    class MatrixErrorFilterDialog : ConfigDialog
    {
        private MatrixErrorFilter module;
        private Button errorMatrixEditButton;
        private Table table;

        public MatrixErrorFilterDialog()
            : this(new MatrixErrorFilter()) { }

        public MatrixErrorFilterDialog(MatrixErrorFilter existingModule)
            : base(existingModule)
        {
            module = modifiedModule as MatrixErrorFilter;
            if (module == null) {
                modifiedModule = new MatrixErrorFilter();
                module = modifiedModule as MatrixErrorFilter;
            }

            errorMatrixEditButton = new Button("Edit");
            errorMatrixEditButton.Clicked += delegate
            {
                if (module != null) {
                    ErrorMatrix configuredMatrix = null;
                    configuredMatrix = ConfigDialog.configureModule(
                        "ErrorMatrix", module.Matrix) as ErrorMatrix;
                    if (configuredMatrix != null) {
                        module.Matrix = configuredMatrix;
                    }
                }
            };

            table = new Table(2, 2, false)
                { ColumnSpacing = 5, RowSpacing = 5, BorderWidth = 5 };
            table.Attach(new Label("Error matrix") { Xalign = 0.0f },
                0, 1, 0, 1, AttachOptions.Fill, AttachOptions.Shrink,
                0, 0);
            table.Attach(errorMatrixEditButton, 1, 2, 0, 1,
                AttachOptions.Fill | AttachOptions.Expand,
                AttachOptions.Shrink, 0, 0);
            table.ShowAll();
            VBox.PackStart(table);
        }
    }
}
