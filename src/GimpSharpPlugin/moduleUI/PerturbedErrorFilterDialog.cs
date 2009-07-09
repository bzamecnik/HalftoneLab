using System;
using Gtk;
using Halftone;

namespace Gimp.HalftoneLab
{
    public class PerturbedErrorFilterDialog : ConfigDialog
    {
        private PerturbedErrorFilter module;
        private SubmoduleSelector<MatrixErrorFilter> childFilterSelector;
        private SpinButton perturbationAmplitudeSpinButton;
        private Table table;

        public PerturbedErrorFilterDialog()
            : this(new PerturbedErrorFilter()) { }

        public PerturbedErrorFilterDialog(
            PerturbedErrorFilter existingModule)
            : base(existingModule)
        {
            module = modifiedModule as PerturbedErrorFilter;
            if (module == null) {
                modifiedModule = new PerturbedErrorFilter();
                module = modifiedModule as PerturbedErrorFilter;
            }

            childFilterSelector = new SubmoduleSelector<MatrixErrorFilter>(
                module.ChildFilter);
            childFilterSelector.ModuleChanged += delegate
            {
                module.ChildFilter = childFilterSelector.Module;
            };

            perturbationAmplitudeSpinButton = new SpinButton(0.0, 1.0, 0.1);
            perturbationAmplitudeSpinButton.Value =
                module.PerturbationAmplitude;
            perturbationAmplitudeSpinButton.Changed += delegate
            {
                module.PerturbationAmplitude = perturbationAmplitudeSpinButton.Value;
            };

            table = new Table(2, 2, false)
                { ColumnSpacing = 5, RowSpacing = 5, BorderWidth = 5 };
            table.Attach(new Label("Child filter") { Xalign = 0.0f},
                0, 1, 0, 1, AttachOptions.Fill, AttachOptions.Shrink,
                0, 0);
            table.Attach(childFilterSelector, 1, 2, 0, 1,
                AttachOptions.Fill | AttachOptions.Expand,
                AttachOptions.Shrink, 0, 0);
            table.Attach(new Label("Perturbation amplitude")
                { Xalign = 0.0f }, 0, 1, 1, 2, AttachOptions.Fill,
                AttachOptions.Shrink, 0, 0);
            table.Attach(perturbationAmplitudeSpinButton, 1, 2, 1, 2,
                AttachOptions.Fill | AttachOptions.Expand,
                AttachOptions.Shrink, 0, 0);
            table.ShowAll();
            VBox.PackStart(table);
        }
    }
}
