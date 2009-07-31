// Copyright (c) 2009 Bohumir Zamecnik <bohumir@zamecnik.org>
// License: The MIT License, see the LICENSE file

using System;
using Gtk;
using HalftoneLab;

namespace HalftoneLab.GUI.Gtk
{
    /// <summary>
    /// %Matrix error filter configuration dialog.
    /// </summary>
    /// <see cref="HalftoneLab.MatrixErrorFilter"/>
    class MatrixErrorFilterDialog : ModuleDialog
    {
        private MatrixErrorFilter module;
        private Table table;
        private ErrorMatrixPanel matrixPanel;

        public MatrixErrorFilterDialog()
            : this(new MatrixErrorFilter()) { }

        public MatrixErrorFilterDialog(MatrixErrorFilter existingModule)
            : base(existingModule)
        {
            module = modifiedModule as MatrixErrorFilter;
            if (module == null) {
                modifiedModule = new MatrixErrorFilter();
                module = modifiedModule as MatrixErrorFilter;
            }

            matrixPanel = new ErrorMatrixPanel((uint)module.Matrix.Height,
                (uint)module.Matrix.Width);
            matrixPanel.Matrix = module.Matrix;

            table = new Table(2, 1, false)
                { ColumnSpacing = 5, RowSpacing = 5, BorderWidth = 5 };
            table.Attach(new Label("Error matrix:") { Xalign = 0.0f },
                0, 1, 0, 1, AttachOptions.Fill, AttachOptions.Shrink, 0, 0);
            table.Attach(matrixPanel, 0, 1, 1, 2,
                AttachOptions.Fill | AttachOptions.Expand,
                AttachOptions.Fill | AttachOptions.Expand, 0, 0);
            table.ShowAll();
            VBox.PackStart(table);
        }

        protected override void save() {
            module.Matrix = matrixPanel.Matrix;
        }
    }
}
