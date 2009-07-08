using System;
using Gtk;
using Halftone;

namespace Gimp.HalftoneLab
{
    public class SpotFunctionDialog : ConfigDialog
    {
        private SpotFunction module;
        private Table table;

        public SpotFunctionDialog()
            : this(new SpotFunction()) { }

        public SpotFunctionDialog(SpotFunction existingModule)
            : base(existingModule)
        {
        }
    }
}
