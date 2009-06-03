using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Halftone
{
    public abstract class Matrix<T>
    {
        private T[,] _matrix; // TODO: maybe 'protected'

        //public Matrix(int width, int height) {
        //    _matrix = new T[height, width];
        //}

        //public Matrix(T[,] matrix) {
        //    _matrix = matrix;
        //}

        protected Matrix() {
            _matrix = new T[1,1];
        }

        public int Height {
            get { return _matrix.GetLength(0); }
        }

        public int Width {
            get { return _matrix.GetLength(1); }
        }

        public T this[int y, int x] {
            get {
                return _matrix[y % Height, x % Width];
            }
            set {
                _matrix[y % Height, x % Width] = value;
            }
        }

        public T[,] TheMatrix {
            get { return _matrix; } // TODO: maybe Clone()
            set { _matrix = value; }
        }

        public abstract Matrix<T> Clone();
    }
}
