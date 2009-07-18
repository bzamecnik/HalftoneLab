using System;
using Gtk;

namespace Gimp.HalftoneLab
{
    class MatrixPanel : VBox
    {
        Table matrixTable;
        ScrolledWindow scroll;
        // direct references to matrix cells
        SpinButton[,] spinButtonMatrix;
        SpinButton matrixRows;
        SpinButton matrixCols;

        public event EventHandler MatrixResized;

        public MatrixPanel(uint rows, uint columns)
            : this(rows, columns, true) { }

        public MatrixPanel(uint rows, uint columns, bool resizable) {
            scroll = new ScrolledWindow();
            resize(rows, columns);
            if (resizable) {
                Table table = new Table(2, 1, false);
                matrixRows = new SpinButton(1, 16, 1) { Value = rows };
                matrixCols = new SpinButton(1, 16, 1) { Value = columns };
                Button resizeButton = new Button("Resize");
                resizeButton.Clicked += delegate
                {
                    // Preserve values from the original matrix if possible.
                    int[,] origMatrix = Matrix;
                    resize((uint)matrixRows.ValueAsInt,
                           (uint)matrixCols.ValueAsInt);
                    if (origMatrix != null) {
                        setMatrix(origMatrix, false);
                    }
                };
                table.Attach(scroll, 0, 1, 0, 1,
                    AttachOptions.Fill | AttachOptions.Expand,
                    AttachOptions.Fill | AttachOptions.Expand, 0, 0);
                HBox hbox = new HBox(false, 2) { BorderWidth = 2 };
                hbox.PackStart(new Label("Rows:"));
                hbox.PackStart(matrixRows);
                hbox.PackStart(new Label("Columns:"));
                hbox.PackStart(matrixCols);
                hbox.PackStart(resizeButton);
                table.Attach(hbox, 0, 1, 1, 2,
                    AttachOptions.Fill, AttachOptions.Shrink, 0, 0);
                table.ShowAll();
                Add(table);
            } else {
                Add(scroll);
            }
        }

        public int[,] Matrix {
            get {
                if ((matrixTable == null) || (spinButtonMatrix == null)) {
                    return null;
                }
                int[,] matrix = new int[spinButtonMatrix.GetLength(0),
                    spinButtonMatrix.GetLength(1)];
                for (uint row = 0; row < spinButtonMatrix.GetLength(0); row++) {
                    for (uint col = 0; col < spinButtonMatrix.GetLength(1); col++) {
                        SpinButton spin = spinButtonMatrix[row, col];
                        if (spin != null) {
                            matrix[row, col] = spin.ValueAsInt;
                        }
                    }
                }
                return matrix;
            }

            set {
                if ((matrixTable != null) &&(spinButtonMatrix != null) &&
                    (value != null) ) {
                    setMatrix(value, true);
                }
            }
        }

        public uint Rows {
            get { return (matrixTable != null) ? matrixTable.NRows : 0; }
        }

        public uint Columns {
            get { return (matrixTable != null) ? matrixTable.NColumns : 0; }
        }

        void setMatrix(int[,] matrix, bool doResize) {
            int newRows = matrix.GetLength(0);
            int newCols = matrix.GetLength(1);
            if (doResize && ((newRows != matrixTable.NRows) ||
                           (newCols != matrixTable.NRows))) {
                resize((uint)newRows, (uint)newCols);
            }
            int commonRows, commonCols;
            if (doResize) {
                commonRows = newRows;
                commonCols = newCols;
            } else {
                commonRows = Math.Min(spinButtonMatrix.GetLength(0), newRows);
                commonCols = Math.Min(spinButtonMatrix.GetLength(1), newCols);
            }
            for (uint row = 0; row < commonRows; row++) {
                for (uint col = 0; col < commonCols; col++) {
                    SpinButton spin = spinButtonMatrix[row, col];
                    if (spin != null) {
                        spin.Value = matrix[row, col];
                    }
                }
            }
        }

        public void resize(uint rows, uint columns) {
            if ((matrixTable != null) && (matrixTable.NRows == rows) &&
                (matrixTable.NColumns == columns))
            {
                return; // no resize needed
            }
            if (matrixTable != null) {
                matrixTable.Destroy();
            }
            matrixTable = new Table(rows, columns, true);
            spinButtonMatrix = new SpinButton[rows, columns];
            for (uint row = 0; row < rows; row++) {
                for (uint col = 0; col < columns; col++) {
                    SpinButton spin = new SpinButton(0, 1000, 1)
                        { HasFrame = false };
                    spinButtonMatrix[row, col] = spin;
                    matrixTable.Attach(spin,
                        col, col + 1, row, row + 1,
                        AttachOptions.Fill, AttachOptions.Shrink, 0, 0);
                }
            }
            matrixTable.ShowAll();
            scroll.AddWithViewport(matrixTable);
            if ((matrixRows != null) && (matrixCols != null)) {
                matrixRows.Value = rows;
                matrixCols.Value = columns;
            }
            if (MatrixResized != null) {
                MatrixResized(this, new EventArgs());
            }
        }
    }
}
    