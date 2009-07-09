using System;
using Gtk;
using Halftone;

namespace Gimp.HalftoneLab
{
    public class RandomizedMatrixErrorFilterDialog : ConfigDialog
    {
        private RandomizedMatrixErrorFilter module;
        private Button errorMatrixEditButton;
        private CheckButton randCoeffCountCheckButton;
        private Table table;

        public RandomizedMatrixErrorFilterDialog()
            : this(new RandomizedMatrixErrorFilter()) { }

        public RandomizedMatrixErrorFilterDialog(
            RandomizedMatrixErrorFilter existingModule)
            : base(existingModule)
        {
            module = modifiedModule as RandomizedMatrixErrorFilter;
            if (module == null) {
                modifiedModule = new RandomizedMatrixErrorFilter();
                module = modifiedModule as RandomizedMatrixErrorFilter;
            }

            errorMatrixEditButton = new Button("gtk-edit");
            errorMatrixEditButton.Clicked += delegate
            {
                ErrorMatrix configuredMatrix = null;
                configuredMatrix = ConfigDialog.configureModule(
                    "ErrorMatrix", module.Matrix) as ErrorMatrix;
                if (configuredMatrix != null) {
                    module.Matrix = configuredMatrix;
                }
            };

            randCoeffCountCheckButton = new CheckButton();
            randCoeffCountCheckButton.Active =
                module.RandomizeCoeffCount;
            randCoeffCountCheckButton.Toggled += delegate
            {
                module.RandomizeCoeffCount =
                    randCoeffCountCheckButton.Active;
            };

            table = new Table(2, 2, false)
                { ColumnSpacing = 5, RowSpacing = 5, BorderWidth = 5 };
            table.Attach(new Label("Error matrix") { Xalign = 0.0f },
                0, 1, 0, 1, AttachOptions.Fill, AttachOptions.Shrink,
                0, 0);
            table.Attach(errorMatrixEditButton, 1, 2, 0, 1,
                AttachOptions.Fill | AttachOptions.Expand,
                AttachOptions.Shrink, 0, 0);
            table.Attach(new Label("Randomize coefficient count?")
                { Xalign = 0.0f }, 0, 1, 1, 2, AttachOptions.Fill,
                AttachOptions.Shrink, 0, 0);
            table.Attach(randCoeffCountCheckButton, 1, 2, 1, 2,
                AttachOptions.Fill | AttachOptions.Expand,
                AttachOptions.Shrink, 0, 0);
            table.ShowAll();
            VBox.PackStart(table);
        }
    }
}
