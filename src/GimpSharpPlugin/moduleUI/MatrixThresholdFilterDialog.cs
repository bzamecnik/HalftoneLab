using System;
using Gtk;
using HalftoneLab;

namespace HalftoneLab.GUI.Gtk
{
    public class MatrixThresholdFilterDialog : ModuleDialog
    {
        private MatrixThresholdFilter module;
        private Table table;
        private ThresholdMatrixPanel matrixPanel;

        public MatrixThresholdFilterDialog()
            : this(new MatrixThresholdFilter()) { }

        public MatrixThresholdFilterDialog(
            MatrixThresholdFilter existingModule)
            : base(existingModule)
        {
            module = modifiedModule as MatrixThresholdFilter;
            if (module == null) {
                modifiedModule = new MatrixThresholdFilter();
                module = modifiedModule as MatrixThresholdFilter;
            }

            matrixPanel = new ThresholdMatrixPanel(
                (uint)module.Matrix.Height,
                (uint)module.Matrix.Width);
            matrixPanel.Matrix = module.Matrix.DefinitionMatrix;
            matrixPanel.Scaled = !module.Matrix.Iterative;

            table = new Table(2, 1, false)
                { ColumnSpacing = 5, RowSpacing = 5, BorderWidth = 5 };
            table.Attach(new Label("Threshold matrix:") { Xalign = 0.0f },
                0, 1, 0, 1, AttachOptions.Fill, AttachOptions.Shrink,
                0, 0);
            table.Attach(matrixPanel, 0, 1, 1, 2,
                AttachOptions.Fill | AttachOptions.Expand,
                AttachOptions.Fill | AttachOptions.Expand, 0, 0);
            table.ShowAll();
            VBox.PackStart(table);
        }

        protected override void save() {
            module.Matrix = new ThresholdMatrix(matrixPanel.Matrix,
                !matrixPanel.Scaled);
        }
    }
}
