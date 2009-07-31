// Copyright (c) 2009 Bohumir Zamecnik <bohumir@zamecnik.org>
// License: The MIT License, see the LICENSE file

using System;
using Gtk;
using HalftoneLab;

namespace HalftoneLab.GUI.Gtk
{
    /// <summary>
    /// %Image threshold filter configuration dialog.
    /// </summary>
    /// <see cref="HalftoneLab.ImageThresholdFilter"/>
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
