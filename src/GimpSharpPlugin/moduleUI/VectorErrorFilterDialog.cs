using System;
using Gtk;
using Halftone;

namespace Gimp.HalftoneLab
{
    public class VectorErrorFilterDialog : ConfigDialog
    {
        private VectorErrorFilter module;
        private Table table;

        public VectorErrorFilterDialog()
            : this(new VectorErrorFilter()) { }

        public VectorErrorFilterDialog(VectorErrorFilter existingModule)
            : base(existingModule)
        {
        }
    }
}
