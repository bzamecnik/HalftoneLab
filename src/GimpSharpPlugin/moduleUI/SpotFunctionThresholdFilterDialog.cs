using System;
using Gtk;
using Halftone;

namespace Gimp.HalftoneLab
{
    public class SpotFunctionThresholdFilterDialog : ModuleDialog
    {
        private SpotFunctionThresholdFilter module;
        private SpotFunctionPanel spotFunctionPanel;

        public SpotFunctionThresholdFilterDialog()
            : this(new SpotFunctionThresholdFilter()) { }

        public SpotFunctionThresholdFilterDialog(
            SpotFunctionThresholdFilter existingModule)
            : base(existingModule)
        {
            module = modifiedModule as SpotFunctionThresholdFilter;
            if (module == null) {
                modifiedModule = new SpotFunctionThresholdFilter();
                module = modifiedModule as SpotFunctionThresholdFilter;
            }

            spotFunctionPanel = new SpotFunctionPanel();
            spotFunctionPanel.SpotFunc = module.SpotFunc;

            Frame spotFunctionFrame = new Frame("Spot function")
            {
                BorderWidth = 5
            };
            spotFunctionFrame.Add(spotFunctionPanel);
            
            VBox.PackStart(spotFunctionFrame);
            ShowAll();
        }

        protected override void save() {
            module.SpotFunc = spotFunctionPanel.SpotFunc;
        }
    }
}
