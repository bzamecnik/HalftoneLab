using System;
using Gtk;
using Halftone;

namespace Gimp.HalftoneLab
{
    public class PerturbedErrorFilterDialog : ConfigDialog
    {
        private PerturbedErrorFilter module;
        private Table table;

        public PerturbedErrorFilterDialog()
            : this(new PerturbedErrorFilter()) { }

        public PerturbedErrorFilterDialog(
            PerturbedErrorFilter existingModule)
            : base(existingModule)
        {
        }
    }
}
