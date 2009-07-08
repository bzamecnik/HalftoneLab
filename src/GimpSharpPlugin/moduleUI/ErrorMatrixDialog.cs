using System;
using Gtk;
using Halftone;

namespace Gimp.HalftoneLab
{
    public class ErrorMatrixDialog : ConfigDialog
    {
        private ErrorMatrix module;
        private Table table;

        public ErrorMatrixDialog()
            : this(new ErrorMatrix()) { }

        public ErrorMatrixDialog(ErrorMatrix existingModule)
            : base(existingModule)
        {
        }
    }
}
