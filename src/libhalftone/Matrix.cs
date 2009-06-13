using System;
using System.Text;

namespace Halftone
{
    [Serializable]
    public abstract class Matrix<T> : Module
    {
        private T[,] _matrix; // TODO: maybe 'protected'

        //public Matrix(int _width, int _height) {
        //    _matrix = new T[_height, _width];
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

        public override string ToString() {
            StringBuilder sb = new StringBuilder();
            for (int y = 0; y < Height; y++) {
                for (int x = 0; x < Width; x++) {
                    sb.AppendFormat("{0} ", this[y, x]);
                }
                sb.AppendLine();
            }
            return sb.ToString();
        }
    }
}
