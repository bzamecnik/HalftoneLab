using System;
using Gtk;
using Halftone;

namespace Gimp.HalftoneLab
{
    public class VectorErrorFilterDialog : ConfigDialog
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
            vectorPanel.Matrix = module.Matrix.DefinitionMatrix;
            vectorPanel.Divisor = module.Matrix.Divisor;
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
            module.Matrix = new ErrorMatrix(vectorPanel.Matrix,
                vectorPanel.SourceOffsetX, vectorPanel.Divisor);
        }
    }
}
