using System;
using System.Collections.Generic;
using System.Linq;
using Gtk;
using HalftoneLab;

namespace HalftoneLab.GUI.Gtk
{
    class ErrorMatrixPanel : Table
    {
        ErrorMatrix module;
        MatrixPanel matrixPanel;
        SpinButton divisorSpinButton;
        CheckButton customDivisorCheckButton;
        SpinButton sourceOffsetXSpinButton;
        ComboBox presetComboBox;
        List<ErrorMatrix> presets;

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

            presets = new List<ErrorMatrix>(ErrorMatrix.Samples.list());
            var presetsNames = from preset in presets select preset.Name;
            presetComboBox = new ComboBox(presetsNames.ToArray());
            presetComboBox.Changed += delegate
            {
                int active = presetComboBox.Active;
                if (active >= 0) {
                    Matrix = presets[active];
                }

            };

            ColumnSpacing = 2;
            RowSpacing = 2;
            BorderWidth = 2;

            Attach(new Label("Preset:") { Xalign = 0.0f }, 0, 1, 0, 1,
                AttachOptions.Fill | AttachOptions.Expand,
                AttachOptions.Shrink, 0, 0);
            Attach(presetComboBox, 1, 2, 0, 1,
                AttachOptions.Fill | AttachOptions.Expand,
                AttachOptions.Shrink, 0, 0);

            Attach(matrixPanel, 0, 2, 1, 2,
                AttachOptions.Fill | AttachOptions.Expand,
                AttachOptions.Fill | AttachOptions.Expand, 0, 0);

            Attach(new Label("Source offset X:") { Xalign = 0.0f },
                0, 1, 2, 3, AttachOptions.Fill | AttachOptions.Expand,
                AttachOptions.Shrink, 0, 0);
            Attach(sourceOffsetXSpinButton, 1, 2, 2, 3,
                AttachOptions.Fill, AttachOptions.Shrink, 0, 0);

            Attach(customDivisorCheckButton, 0, 2, 3, 4,
                AttachOptions.Fill | AttachOptions.Expand,
                AttachOptions.Shrink, 0, 0);

            Attach(new Label("Divisor:") { Xalign = 0.0f }, 0, 1, 4, 5,
                AttachOptions.Fill | AttachOptions.Expand,
                AttachOptions.Shrink, 0, 0);
            Attach(divisorSpinButton, 1, 2, 4, 5,
                AttachOptions.Fill, AttachOptions.Shrink, 0, 0);
        }

        public ErrorMatrix Matrix {
            get {
                if (UseCustomDivisor) {
                    module = new ErrorMatrix(BareMatrix, SourceOffsetX, Divisor);
                } else {
                    module = new ErrorMatrix(BareMatrix, SourceOffsetX);
                }
                return module;
            }
            set {
                module = value;
                if (module == null) {
                    module = new ErrorMatrix();
                }
                BareMatrix = module.DefinitionMatrix;
                Divisor = module.Divisor;
                UseCustomDivisor = false;
                SourceOffsetX = module.SourceOffsetX;
            }
        }

        private int[,] BareMatrix {
            get {
                return matrixPanel.Matrix;
            }
            set {
                matrixPanel.Matrix = value;
            }
        }

        private int SourceOffsetX {
            get { return sourceOffsetXSpinButton.ValueAsInt - 1; }
            set { sourceOffsetXSpinButton.Value = value + 1; }
        }

        private int Divisor {
            get { return divisorSpinButton.ValueAsInt; }
            set { divisorSpinButton.Value = value; }
        }

        private bool UseCustomDivisor {
            get { return customDivisorCheckButton.Active; }
            set {
                customDivisorCheckButton.Active = value;
                divisorSpinButton.Sensitive = value;
            }
        }
    }
}
