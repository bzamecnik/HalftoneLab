using System;
using System.Text;

namespace Halftone
{
    /// <summary>
    /// Generic tileable matrix.
    /// </summary>
    /// <remarks>
    /// Elements of the matrix are accessed through coordinates modulo
    /// matrix width (or height). The purpose of this class is to provide
    /// convenience functions to its descendants with concrete element
    /// types.
    /// </remarks>
    /// <typeparam name="T">Matrix element type</typeparam>
    [Serializable]
    public class Matrix<T> : Module
    {
        /// <summary>
        /// The matrix itself.
        /// </summary>
        public T[,] matrix;

        /// <summary>
        /// Matrix height (= max Y coordinate + 1).
        /// </summary>
        public int Height {
            get { return matrix.GetLength(0); }
        }

        /// <summary>
        /// Matrix width (= max X coordinate + 1).
        /// </summary>
        public int Width {
            get { return matrix.GetLength(1); }
        }

        /// <summary>
        /// Access matrix elements using by coordinates with modulo
        /// operation applied.
        /// </summary>
        /// <param name="y">Y coordinate (zero based)</param>
        /// <param name="x">X coordinate (zero based)</param>
        /// <returns></returns>
        public T this[int y, int x] {
            get {
                return matrix[y % Height, x % Width];
            }
            set {
                matrix[y % Height, x % Width] = value;
            }
        }

        public Matrix(int height, int width) {
            matrix = new T[height, width];
        }

        public Matrix(T[,] matrix) {
            if (matrix != null) {
                this.matrix = matrix;
            } else {
                this.matrix = new T[1, 1]; // TODO: throw an exception
            }
        }

        /// <summary>
        /// Clone the matrix. Make a shallow copy.
        /// </summary>
        /// <returns>Cloned matrix.</returns>
        public virtual Matrix<T> Clone() {
            return new Matrix<T>(matrix);
        }

        /// <summary>
        /// Convert the matrix to a human readable string.
        /// </summary>
        /// <returns>String representaion of the matrix.</returns>
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
