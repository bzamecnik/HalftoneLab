using System;
using Gtk;
using Halftone;

namespace Gimp.HalftoneLab
{
    public class ErrorMatrixDialog : ConfigDialog
    {
        private ErrorMatrix module;
        private ErrorMatrixPanel matrixPanel;

        public ErrorMatrixDialog()
            : this(new ErrorMatrix()) { }

        public ErrorMatrixDialog(ErrorMatrix existingModule)
            : base(existingModule)
        {
            module = modifiedModule as ErrorMatrix;
            if (module == null) {
                modifiedModule = new ErrorMatrix();
                module = modifiedModule as ErrorMatrix;
            }
        }
    }
}
