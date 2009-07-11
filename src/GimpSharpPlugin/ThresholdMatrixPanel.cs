using System;
using Gtk;
using Halftone;

namespace Gimp.HalftoneLab
{
    class ThresholdMatrixPanel : Table
    {
        MatrixPanel matrixPanel;
        CheckButton iterativeCheckButton;

        public ThresholdMatrixPanel(uint rows, uint cols)
            : base(2, 1, false)
        {
            iterativeCheckButton = new CheckButton("Iterative matrix?");

            matrixPanel = new MatrixPanel(rows, cols);

            ColumnSpacing = 2;
            RowSpacing = 2;
            BorderWidth = 2;

            Attach(matrixPanel, 0, 1, 0, 1,
                    AttachOptions.Fill | AttachOptions.Expand,
                    AttachOptions.Fill | AttachOptions.Expand, 0, 0);
            
            Attach(iterativeCheckButton, 0, 1, 1, 2,
                    AttachOptions.Fill, AttachOptions.Shrink, 0, 0);
        }

        public int[,] Matrix {
            get {
                return matrixPanel.Matrix;
            }
            set {
                matrixPanel.Matrix = value;
            }
        }

        public bool Iterative {
            get { return iterativeCheckButton.Active; }
            set { iterativeCheckButton.Active = value; }
        }
    }
}
