using System;
using Gtk;
using Halftone;

namespace Gimp.HalftoneLab
{
    public class ThresholdMatrixDialog : ConfigDialog
    {
        private ThresholdMatrix module;
        private Table table;

        public ThresholdMatrixDialog()
            : this(new ThresholdMatrix()) { }

        public ThresholdMatrixDialog(ThresholdMatrix existingModule)
            : base(existingModule)
        {
        }
    }
}
