﻿using System;
using Gtk;

namespace Gimp.HalftoneLab
{
    class ErrorVectorPanel : Table
    {
        Table vectorTable;
        ScrolledWindow scroll;
        // direct references to matrix cells
        SpinButton[] spinButtonVector;
        SpinButton vectorLengthSpinButton;
        SpinButton divisorSpinButton;
        SpinButton sourceOffsetXSpinButton;

        public ErrorVectorPanel(uint length)
            : base(4, 3, false)
        {
            divisorSpinButton = new SpinButton(1, 1000, 1);
            sourceOffsetXSpinButton = new SpinButton(1, length, 1);

            ColumnSpacing = 2;
            RowSpacing = 2;
            BorderWidth = 2;

            scroll = new ScrolledWindow();
            resize(length);
            Table table = new Table(2, 1, false);
            vectorLengthSpinButton = new SpinButton(1, Math.Max(16.0, length), 1)
                { Value = length };
            Button resizeButton = new Button("Resize");
            resizeButton.Clicked += delegate
            {
                // Preserve values from the original matrix if possible.
                int[] origVector = Vector;
                resize((uint)vectorLengthSpinButton.ValueAsInt);
                if (origVector != null) {
                    setVector(origVector, false);
                }
            };

            Attach(scroll, 0, 5, 0, 1,
                    AttachOptions.Fill | AttachOptions.Expand,
                    AttachOptions.Fill | AttachOptions.Expand, 0, 0);
            
            Attach(new Label("Length:") { Xalign = 0.0f }, 0, 1, 1, 2,
                    AttachOptions.Fill, AttachOptions.Shrink, 0, 0);
            Attach(vectorLengthSpinButton, 1, 2, 1, 2,
                    AttachOptions.Fill, AttachOptions.Shrink, 0, 0);
            Attach(resizeButton, 2, 3, 1, 2,
                    AttachOptions.Fill, AttachOptions.Shrink, 0, 0);

            Attach(new Label("Divisor:") { Xalign = 0.0f }, 0, 1, 2, 3,
                    AttachOptions.Fill | AttachOptions.Expand,
                    AttachOptions.Shrink, 0, 0);
            Attach(divisorSpinButton, 1, 2, 2, 3,
                    AttachOptions.Fill, AttachOptions.Shrink, 0, 0);

            Attach(new Label("Source offset X:") { Xalign = 0.0f}, 0, 1, 3, 4,
                    AttachOptions.Fill | AttachOptions.Expand,
                    AttachOptions.Shrink, 0, 0);
            Attach(sourceOffsetXSpinButton, 1, 2, 3, 4,
                    AttachOptions.Fill, AttachOptions.Shrink, 0, 0);
        }

        public void resize(uint length) {
            if ((vectorTable != null) && (vectorTable.NColumns == length)) {
                return; // no resize needed
            }
            if (vectorTable != null) {
                vectorTable.Destroy();
            }
            vectorTable = new Table(1, length, true);
            spinButtonVector = new SpinButton[length];
            for (uint col = 0; col < length; col++) {
                SpinButton spin = new SpinButton(0, 1000, 1)
                    { HasFrame = false };
                spinButtonVector[col] = spin;
                vectorTable.Attach(spin,
                    col, col + 1, 0, 1,
                    AttachOptions.Fill, AttachOptions.Shrink, 0, 0);
            }
            vectorTable.ShowAll();
            scroll.AddWithViewport(vectorTable);
            if (vectorLengthSpinButton != null) {
                vectorLengthSpinButton.Value = length;
            }
        }

        void setVector(int[] vector, bool doResize) {
            int newLength = vector.Length;
            if (doResize && (newLength != vectorTable.NColumns)) {
                resize((uint)newLength);
            }
            int commonLength;
            if (doResize) {
                commonLength = newLength;
            } else {
                commonLength = Math.Min(spinButtonVector.GetLength(0),
                    newLength);
            }
            for (uint i = 0; i < commonLength; i++) {
                SpinButton spin = spinButtonVector[i];
                if (spin != null) {
                    spin.Value = vector[i];
                }
            }
            sourceOffsetXSpinButton.Adjustment.Upper = vectorTable.NColumns;
        }

        public int[,] Matrix {
            get {
                if ((vectorTable == null) || (spinButtonVector == null)) {
                    return null;
                }
                int[,] matrix = new int[1, spinButtonVector.Length];
                for (int i = 0; i < spinButtonVector.Length; i++) {
                    SpinButton spin = spinButtonVector[i];
                    if (spin != null) {
                        matrix[0, i] = spin.ValueAsInt;
                    }
                }
                return matrix;
            }

            set {
                if ((value != null) && (vectorTable != null) &&
                    (spinButtonVector != null))
                {
                    int[] vector = new int[value.GetLength(1)];
                    for (int i = 0; i < value.GetLength(1); i++) {
                        vector[i] = value[0, i];
                    }
                    setVector(vector, true);
                }
            }
        }

        public int[] Vector {
            get {
                if ((vectorTable == null) || (spinButtonVector == null)) {
                    return null;
                }
                int[] vector = new int[spinButtonVector.Length];
                for (int i = 0; i < spinButtonVector.Length; i++) {
                    SpinButton spin = spinButtonVector[i];
                    if (spin != null) {
                        vector[i] = spin.ValueAsInt;
                    }
                }
                return vector;
            }

            set {
                if ((value != null) && (vectorTable != null) &&
                    (spinButtonVector != null)) {
                    setVector(value, true);
                }
            }
        }

        public int Divisor {
            get { return divisorSpinButton.ValueAsInt; }
            set { divisorSpinButton.Value = value; }
        }

        public int SourceOffsetX {
            get { return sourceOffsetXSpinButton.ValueAsInt - 1; }
            set { sourceOffsetXSpinButton.Value = value + 1; }
        }
    }
}
