using System;
using Gtk;
using Halftone;

namespace Gimp.HalftoneLab
{
    public class VectorErrorFilterDialog : ConfigDialog
    {
        private VectorErrorFilter module;
        private Button errorMatrixEditButton;
        private Table table;

        public VectorErrorFilterDialog()
            : this(new VectorErrorFilter()) { }

        public VectorErrorFilterDialog(VectorErrorFilter existingModule)
            : base(existingModule)
        {
            module = modifiedModule as VectorErrorFilter;
            if (module == null) {
                modifiedModule = new VectorErrorFilter();
                module = modifiedModule as VectorErrorFilter;
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

            table = new Table(2, 2, false) { ColumnSpacing = 5, RowSpacing = 5, BorderWidth = 5 };
            table.Attach(new Label("Error vector") { Xalign = 0.0f },
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
