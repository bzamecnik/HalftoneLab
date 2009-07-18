using System;
using Gtk;
using Halftone;

namespace Gimp.HalftoneLab
{
    public class ImageThresholdFilterDialog : ModuleDialog
    {
        private ImageThresholdFilter module;
        private Table table;
        private ImageGeneratorPanel imageGeneratorPanel;

        public ImageThresholdFilterDialog()
            : this(new ImageThresholdFilter()) { }

        public ImageThresholdFilterDialog(
            ImageThresholdFilter existingModule)
            : base(existingModule)
        {
            module = modifiedModule as ImageThresholdFilter;
            if (module == null) {
                modifiedModule = new ImageThresholdFilter();
                module = modifiedModule as ImageThresholdFilter;
            }

            imageGeneratorPanel = new ImageGeneratorPanel();
            imageGeneratorPanel.Generator = module.ImageGenerator;

            Frame imageGeneratorFrame = new Frame("Image generator")
            {
                BorderWidth = 5
            };
            imageGeneratorFrame.Add(imageGeneratorPanel);

            VBox.PackStart(imageGeneratorFrame);
            ShowAll();
        }

        protected override void save() {
            module.ImageGenerator = imageGeneratorPanel.Generator;
        }
    }
}
