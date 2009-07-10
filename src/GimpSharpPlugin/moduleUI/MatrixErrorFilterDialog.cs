using System;
using Gtk;
using Halftone;

namespace Gimp.HalftoneLab
{
    class MatrixErrorFilterDialog : ConfigDialog
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
            matrixPanel.Matrix = module.Matrix.DefinitionMatrix;
            matrixPanel.Divisor = module.Matrix.Divisor;
            matrixPanel.SourceOffsetX = module.Matrix.SourceOffsetX;

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
            module.Matrix = new ErrorMatrix(matrixPanel.Matrix,
                    matrixPanel.SourceOffsetX, matrixPanel.Divisor);
        }
    }
}
