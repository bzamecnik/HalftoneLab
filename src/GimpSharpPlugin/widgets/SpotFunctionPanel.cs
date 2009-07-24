using System;
using System.Collections.Generic;
using System.Linq;
using Gtk;
using HalftoneLab;

namespace HalftoneLab.GUI.Gtk
{
    public class SpotFunctionPanel : Table
    {
        private SpotFunction module;
        private ComboBox presetComboBox;
        private List<SpotFunction> presets;
        private List<string> presetsNames;
        private HScale angleHScale;
        private SpinButton distanceSpinButton;

        public SpotFunctionPanel()
            : base(4, 2, false)
        {
            // TODO: use degrees instead of radians
            angleHScale = new HScale(0, 2, 0.01);
            distanceSpinButton = new SpinButton(1, 50, 1);

            presets = new List<SpotFunction>(SpotFunction.Samples.list());
            presetsNames = (from preset in presets select preset.Name).ToList();
            presetComboBox = new ComboBox(presetsNames.ToArray());
            presetComboBox.Changed += delegate
            {
                int active = presetComboBox.Active;
                if (active >= 0) {
                    module = (SpotFunction)presets[active].deepCopy();
                }
            };

            ColumnSpacing = RowSpacing = BorderWidth = 5;

            Attach(new Label("Preset:") { Xalign = 0.0f }, 0, 1, 0, 1,
                AttachOptions.Fill,
                AttachOptions.Shrink, 0, 0);
            Attach(presetComboBox, 1, 2, 0, 1,
                AttachOptions.Fill | AttachOptions.Expand,
                AttachOptions.Shrink, 0, 0);

            Attach(new Label("Screen angle (rad):") { Xalign = 0.0f }, 0, 1, 1, 2,
                AttachOptions.Fill,
                AttachOptions.Shrink, 0, 0);
            Attach(angleHScale, 1, 2, 1, 2,
                AttachOptions.Fill | AttachOptions.Expand,
                AttachOptions.Shrink, 0, 0);

            Attach(new Label("Screen line distance (px):") { Xalign = 0.0f },
                0, 1, 2, 3,
                AttachOptions.Fill,
                AttachOptions.Shrink, 0, 0);
            Attach(distanceSpinButton, 1, 2, 2, 3,
                AttachOptions.Fill | AttachOptions.Expand,
                AttachOptions.Shrink, 0, 0);

            ShowAll();
        }

        public SpotFunction SpotFunc {
            get {
                module.Angle = Math.PI * angleHScale.Value;
                module.Distance = distanceSpinButton.Value;
                return module;
            }
            set {
                module = value;
                angleHScale.Value = module.Angle / Math.PI;
                distanceSpinButton.Value = module.Distance;
                presetComboBox.Active = presetsNames.IndexOf(module.Name);
            }
        }
    }
}
