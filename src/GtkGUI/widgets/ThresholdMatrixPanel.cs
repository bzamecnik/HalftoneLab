// Copyright (c) 2009 Bohumir Zamecnik <bohumir@zamecnik.org>
// License: The MIT License, see the LICENSE file

using System;
using Gtk;
using HalftoneLab;

namespace HalftoneLab.GUI.Gtk
{
    /// <summary>
    /// Threshold matrix configuration panel.
    /// </summary>
    /// <see cref="HalftoneLab.ThresholdMatrix"/>
    class ThresholdMatrixPanel : Table
    {
        MatrixPanel matrixPanel;
        CheckButton scaledCheckButton;

        public ThresholdMatrixPanel(uint rows, uint cols)
            : base(2, 1, false)
        {
            scaledCheckButton = new CheckButton(
                "Coefficients already scaled?");

            matrixPanel = new MatrixPanel(rows, cols);

            ColumnSpacing = 2;
            RowSpacing = 2;
            BorderWidth = 2;

            Attach(matrixPanel, 0, 1, 0, 1,
                    AttachOptions.Fill | AttachOptions.Expand,
                    AttachOptions.Fill | AttachOptions.Expand, 0, 0);
            
            Attach(scaledCheckButton, 0, 1, 1, 2,
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

        public bool Scaled {
            get { return scaledCheckButton.Active; }
            set { scaledCheckButton.Active = value; }
        }
    }
}
