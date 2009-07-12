using System;
using Gtk;
using Halftone;

namespace Gimp.HalftoneLab
{
    public class SpotFunctionThresholdFilterDialog : ModuleDialog
    {
        private SpotFunctionThresholdFilter module;
        private Table table;

        public SpotFunctionThresholdFilterDialog()
            : this(new SpotFunctionThresholdFilter()) { }

        public SpotFunctionThresholdFilterDialog(
            SpotFunctionThresholdFilter existingModule)
            : base(existingModule)
        {
        }
    }
}
