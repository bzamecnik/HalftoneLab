// Copyright (c) 2009 Bohumir Zamecnik <bohumir@zamecnik.org>
// License: The MIT License, see the LICENSE file

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Gtk;
using HalftoneLab;

namespace HalftoneLab.GUI.Gtk
{
    /// <summary>
    /// %Image generator configuration panel.
    /// </summary>
    /// <see cref="HalftoneLab.ImageGenerator"/>
    public class ImageGeneratorPanel : Table
    {
        private ImageGenerator module;
        private ComboBox presetComboBox;
        private List<ImageGenerator> presets;
        private List<string> presetsNames;
        private SpotFunctionPanel spotFunctionPanel;
        private Label effectsLabel;

        public ImageGeneratorPanel()
            : base(3, 2, false)
        {
            presets = new List<ImageGenerator>(ImageGenerator.Samples.list());
            presetsNames = (from preset in presets select preset.Name).ToList();
            presetComboBox = new ComboBox(presetsNames.ToArray());
            presetComboBox.Changed += delegate
            {
                int active = presetComboBox.Active;
                if (active >= 0) {
                    Generator = presets[active];
                }
            };

            ColumnSpacing = RowSpacing = BorderWidth = 5;

            spotFunctionPanel = new SpotFunctionPanel();

            effectsLabel = new Label() { Xalign = 0.0f };

            Frame spotFunctionFrame = new Frame("Spot function")
            {
                BorderWidth = 5
            };
            spotFunctionFrame.Add(spotFunctionPanel);

            Attach(new Label("Preset:") { Xalign = 0.0f }, 0, 1, 0, 1,
                AttachOptions.Fill,
                AttachOptions.Shrink, 0, 0);
            Attach(presetComboBox, 1, 2, 0, 1,
                AttachOptions.Fill,
                AttachOptions.Shrink, 0, 0);

            Attach(spotFunctionFrame, 0, 2, 1, 2,
                AttachOptions.Fill | AttachOptions.Expand,
                AttachOptions.Fill | AttachOptions.Expand, 0, 0);

            Attach(new Label("Effects:") { Xalign = 0.0f },
                0, 2, 2, 3,
                AttachOptions.Fill,
                AttachOptions.Shrink, 0, 0);

            Attach(effectsLabel, 0, 2, 3, 4,
                AttachOptions.Fill | AttachOptions.Expand,
                AttachOptions.Shrink, 0, 0);

            ShowAll();
        }

        public ImageGenerator Generator {
            get {
                module.SpotFunction = spotFunctionPanel.SpotFunc;
                return module;
            }
            set {
                module = value;
                spotFunctionPanel.SpotFunc = module.SpotFunction;
                presetComboBox.Active = presetsNames.IndexOf(module.Name);
                StringBuilder effectsSb = new StringBuilder();
                foreach (Module effect in module.Effects) {
                    effectsSb.AppendFormat("{0} ({1})", effect.Name,
                        effect.Description);
                    effectsSb.AppendLine();
                }
                effectsLabel.Text = effectsSb.ToString();
            }
        }
    }
}