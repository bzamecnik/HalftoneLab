// Copyright (c) 2009 Bohumir Zamecnik <bohumir@zamecnik.org>
// License: The MIT License, see the LICENSE file

using System;
using Gtk;
using HalftoneLab;

namespace HalftoneLab.GUI.Gtk
{
    /// <summary>
    /// Vector error filter configuration dialog.
    /// </summary>
    /// <see cref="HalftoneLab.VectorErrorFilter"/>
    public class VectorErrorFilterDialog : ModuleDialog
    {
        private VectorErrorFilter module;
        private Table table;
        private ErrorVectorPanel vectorPanel;

        public VectorErrorFilterDialog()
            : this(new VectorErrorFilter()) { }

        public VectorErrorFilterDialog(VectorErrorFilter existingModule)
            : base(existingModule)
        {
            module = modifiedModule as VectorErrorFilter;
            if (module == null) {
                modifiedModule = new VectorErrorFilter();
                module = modifiedModule as VectorErrorFilter;
            }

            vectorPanel = new ErrorVectorPanel((uint)module.Matrix.Width);
            vectorPanel.BareMatrix = module.Matrix.DefinitionMatrix;
            vectorPanel.Divisor = module.Matrix.Divisor;
            vectorPanel.UseCustomDivisor = false;
            vectorPanel.SourceOffsetX = module.Matrix.SourceOffsetX;

            table = new Table(2, 1, false)
                { ColumnSpacing = 5, RowSpacing = 5, BorderWidth = 5 };
            table.Attach(new Label("Error vector:") { Xalign = 0.0f },
                0, 1, 0, 1, AttachOptions.Fill, AttachOptions.Shrink, 0, 0);
            table.Attach(vectorPanel, 0, 1, 1, 2,
                AttachOptions.Fill | AttachOptions.Expand,
                AttachOptions.Fill | AttachOptions.Expand, 0, 0);
            table.ShowAll();
            VBox.PackStart(table);
        }

        protected override void save() {
            module.Matrix = vectorPanel.Matrix;
        }
    }
}
