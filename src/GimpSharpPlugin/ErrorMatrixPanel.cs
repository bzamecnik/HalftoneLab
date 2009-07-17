using System;
using Gtk;
using Halftone;

namespace Gimp.HalftoneLab
{
    class ErrorMatrixPanel : Table
    {
        MatrixPanel matrixPanel;
        SpinButton divisorSpinButton;
        CheckButton customDivisorCheckButton;
        SpinButton sourceOffsetXSpinButton;

        public ErrorMatrixPanel(uint rows, uint cols)
            : base(4, 2, false)
        {
            divisorSpinButton = new SpinButton(1, 10000, 1);
            sourceOffsetXSpinButton = new SpinButton(1, cols, 1);
            customDivisorCheckButton = new CheckButton("Use a custom divisor?");
            customDivisorCheckButton.Toggled += delegate
            {
                divisorSpinButton.Sensitive = customDivisorCheckButton.Active;
            };

            matrixPanel = new MatrixPanel(rows, cols);
            matrixPanel.MatrixResized += delegate
            {
                sourceOffsetXSpinButton.Adjustment.Upper =
                    matrixPanel.Columns;
            };

            ColumnSpacing = 2;
            RowSpacing = 2;
            BorderWidth = 2;

            Attach(matrixPanel, 0, 2, 0, 1,
                AttachOptions.Fill | AttachOptions.Expand,
                AttachOptions.Fill | AttachOptions.Expand, 0, 0);

            Attach(new Label("Source offset X:") { Xalign = 0.0f },
                0, 1, 1, 2, AttachOptions.Fill | AttachOptions.Expand,
                AttachOptions.Shrink, 0, 0);
            Attach(sourceOffsetXSpinButton, 1, 2, 1, 2,
                AttachOptions.Fill, AttachOptions.Shrink, 0, 0);

            Attach(customDivisorCheckButton, 0, 2, 2, 3,
                AttachOptions.Fill | AttachOptions.Expand,
                AttachOptions.Shrink, 0, 0);

            Attach(new Label("Divisor:") { Xalign = 0.0f }, 0, 1, 3, 4,
                AttachOptions.Fill | AttachOptions.Expand,
                AttachOptions.Shrink, 0, 0);
            Attach(divisorSpinButton, 1, 2, 3, 4,
                AttachOptions.Fill, AttachOptions.Shrink, 0, 0);
        }

        public ErrorMatrix Matrix {
            get {
                if (UseCustomDivisor) {
                    return new ErrorMatrix(BareMatrix, SourceOffsetX, Divisor);
                } else {
                    return new ErrorMatrix(BareMatrix, SourceOffsetX);
                }
            }
        }

        public int[,] BareMatrix {
            get {
                return matrixPanel.Matrix;
            }
            set {
                matrixPanel.Matrix = value;
            }
        }

        public int SourceOffsetX {
            get { return sourceOffsetXSpinButton.ValueAsInt - 1; }
            set { sourceOffsetXSpinButton.Value = value + 1; }
        }

        public int Divisor {
            get { return divisorSpinButton.ValueAsInt; }
            set { divisorSpinButton.Value = value; }
        }

        public bool UseCustomDivisor {
            get { return customDivisorCheckButton.Active; }
            set {
                customDivisorCheckButton.Active = value;
                divisorSpinButton.Sensitive = value;
            }
        }
    }
}
