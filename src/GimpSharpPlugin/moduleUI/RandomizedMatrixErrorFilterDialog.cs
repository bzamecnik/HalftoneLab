using System;
using Gtk;
using Halftone;

namespace Gimp.HalftoneLab
{
    public class RandomizedMatrixErrorFilterDialog : ConfigDialog
    {
        private RandomizedMatrixErrorFilter module;
        private Table table;

        public RandomizedMatrixErrorFilterDialog()
            : this(new RandomizedMatrixErrorFilter()) { }

        public RandomizedMatrixErrorFilterDialog(
            RandomizedMatrixErrorFilter existingModule)
            : base(existingModule)
        {
        }
    }
}
