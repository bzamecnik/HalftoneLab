using System;
using Gtk;
using Halftone;

namespace Gimp.HalftoneLab
{
    public class DynamicMatrixThresholdFilterDialog : ConfigDialog
    {
        private DynamicMatrixThresholdFilter module;
        private Table table;

        public DynamicMatrixThresholdFilterDialog()
            : this(new DynamicMatrixThresholdFilter()) { }

        public DynamicMatrixThresholdFilterDialog(
            DynamicMatrixThresholdFilter existingModule)
            : base(existingModule)
        {
        }
    }
}
