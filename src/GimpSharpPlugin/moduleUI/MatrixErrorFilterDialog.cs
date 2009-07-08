using System;
using Gtk;
using Halftone;

namespace Gimp.HalftoneLab
{
    class MatrixErrorFilterDialog : ConfigDialog
    {
        private MatrixErrorFilter module;
        private Table table;

        public MatrixErrorFilterDialog()
            : this(new MatrixErrorFilter()) { }

        public MatrixErrorFilterDialog(MatrixErrorFilter existingModule)
            : base(existingModule)
        {
        }
    }
}
