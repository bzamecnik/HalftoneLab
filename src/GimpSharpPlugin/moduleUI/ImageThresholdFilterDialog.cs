using System;
using Gtk;
using Halftone;

namespace Gimp.HalftoneLab
{
    public class ImageThresholdFilterDialog : ConfigDialog
    {
        private ImageThresholdFilter module;
        private Table table;

        public ImageThresholdFilterDialog()
            : this(new ImageThresholdFilter()) { }

        public ImageThresholdFilterDialog(
            ImageThresholdFilter existingModule)
            : base(existingModule)
        {
        }
    }
}
