using System;
using Gtk;
using HalftoneLab;

namespace HalftoneLab.GUI.Gtk
{
    public class PerturbedErrorFilterDialog : ModuleDialog
    {
        private PerturbedErrorFilter module;
        private SubmoduleSelector<MatrixErrorFilter> childFilterSelector;
        private HScale perturbationAmplitudeHScale;
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

            perturbationAmplitudeHScale = new HScale(0.0, 1.0, 0.01);
            perturbationAmplitudeHScale.Value =
                module.PerturbationAmplitude;
            perturbationAmplitudeHScale.ValueChanged += delegate
            {
                module.PerturbationAmplitude = perturbationAmplitudeHScale.Value;
            };

            table = new Table(2, 2, false)
                { ColumnSpacing = 5, RowSpacing = 5, BorderWidth = 5 };
            table.Attach(new Label("Child filter:") { Xalign = 0.0f},
                0, 1, 0, 1, AttachOptions.Fill, AttachOptions.Shrink,
                0, 0);
            table.Attach(childFilterSelector, 1, 2, 0, 1,
                AttachOptions.Fill | AttachOptions.Expand,
                AttachOptions.Shrink, 0, 0);
            table.Attach(new Label("Perturbation amplitude:")
                { Xalign = 0.0f }, 0, 1, 1, 2, AttachOptions.Fill,
                AttachOptions.Shrink, 0, 0);
            table.Attach(perturbationAmplitudeHScale, 1, 2, 1, 2,
                AttachOptions.Fill | AttachOptions.Expand,
                AttachOptions.Shrink, 0, 0);
            table.ShowAll();
            VBox.PackStart(table);
        }
    }
}
