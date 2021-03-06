﻿// Copyright (c) 2009 Bohumir Zamecnik <bohumir@zamecnik.org>
// License: The MIT License, see the LICENSE file

using System;
using Gtk;
using HalftoneLab;

namespace HalftoneLab.GUI.Gtk
{
    /// <summary>
    /// Randomized matrix error filter configuration dialog.
    /// </summary>
    /// <see cref="HalftoneLab.RandomizedMatrixErrorFilter"/>
    public class RandomizedMatrixErrorFilterDialog : ModuleDialog
    {
        private RandomizedMatrixErrorFilter module;
        private Table table;
        private CheckButton randCoeffCountCheckButton;
        private ErrorMatrixPanel matrixPanel;

        public RandomizedMatrixErrorFilterDialog()
            : this(new RandomizedMatrixErrorFilter()) { }

        public RandomizedMatrixErrorFilterDialog(
            RandomizedMatrixErrorFilter existingModule)
            : base(existingModule)
        {
            module = modifiedModule as RandomizedMatrixErrorFilter;
            if (module == null) {
                modifiedModule = new RandomizedMatrixErrorFilter();
                module = modifiedModule as RandomizedMatrixErrorFilter;
            }

            matrixPanel = new ErrorMatrixPanel((uint)module.Matrix.Height,
                (uint)module.Matrix.Width);
            matrixPanel.Matrix = module.Matrix;

            randCoeffCountCheckButton = new CheckButton();
            randCoeffCountCheckButton.Active =
                module.RandomizeCoeffCount;
            randCoeffCountCheckButton.Toggled += delegate
            {
                module.RandomizeCoeffCount =
                    randCoeffCountCheckButton.Active;
            };

            table = new Table(3, 2, false)
                { ColumnSpacing = 5, RowSpacing = 5, BorderWidth = 5 };
            table.Attach(new Label("Error matrix:") { Xalign = 0.0f },
                0, 1, 0, 1, AttachOptions.Fill, AttachOptions.Shrink,
                0, 0);
            table.Attach(matrixPanel, 0, 2, 1, 2,
                AttachOptions.Fill | AttachOptions.Expand,
                AttachOptions.Fill | AttachOptions.Expand, 0, 0);
            table.Attach(new Label("Randomize coefficient count?")
                { Xalign = 0.0f }, 0, 1, 2, 3, AttachOptions.Fill,
                AttachOptions.Shrink, 0, 0);
            table.Attach(randCoeffCountCheckButton, 1, 2, 2, 3,
                AttachOptions.Fill | AttachOptions.Expand,
                AttachOptions.Shrink, 0, 0);
            table.ShowAll();
            VBox.PackStart(table);
        }

        protected override void save() {
            module.Matrix = matrixPanel.Matrix;
        }
    }
}
