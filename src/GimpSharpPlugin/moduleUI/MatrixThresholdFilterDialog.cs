using System;
using Gtk;
using Halftone;

namespace Gimp.HalftoneLab
{
    public class MatrixThresholdFilterDialog : ConfigDialog
    {
        private MatrixThresholdFilter module;
        private Table table;

        public MatrixThresholdFilterDialog()
            : this(new MatrixThresholdFilter()) { }

        public MatrixThresholdFilterDialog(
            MatrixThresholdFilter existingModule)
            : base(existingModule)
        {
        }
    }
}
