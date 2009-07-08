using System;
using Gtk;
using Halftone;

namespace Gimp.HalftoneLab
{
    public class DynamicMatrixErrorFilterDialog : ConfigDialog
    {
        private DynamicMatrixErrorFilter module;
        private Table table;

        public DynamicMatrixErrorFilterDialog()
            : this(new DynamicMatrixErrorFilter()) { }

        public DynamicMatrixErrorFilterDialog(
            DynamicMatrixErrorFilter existingModule)
            : base(existingModule)
        {
        }
    }
}
