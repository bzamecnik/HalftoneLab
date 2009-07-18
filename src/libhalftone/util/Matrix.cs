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
    public abstract class Matrix<DefinitionType, WorkingType> : Module
    {
        /// <summary>
        /// The matrix itself.
        /// </summary>
        private DefinitionType[,] _definitionMatrix;
        public DefinitionType[,] DefinitionMatrix {
            get { return _definitionMatrix; }
            set {
                if (value != null) {
                    _definitionMatrix = (DefinitionType[,])value.Clone();
                    computeWorkingMatrix();
                }
            }
        }

        /// <summary>
        /// Matrix height (= max Y coordinate + 1).
        /// </summary>
        public int Height {
            get { return DefinitionMatrix.GetLength(0); }
        }

        /// <summary>
        /// Matrix width (= max X coordinate + 1).
        /// </summary>
        public int Width {
            get { return DefinitionMatrix.GetLength(1); }
        }

        protected abstract WorkingType[,] WorkingMatrix {
            get;
            set;
        }

        /// <summary>
        /// Access matrix elements using by coordinates with modulo
        /// operation applied.
        /// </summary>
        /// <param name="y">Y coordinate (zero based)</param>
        /// <param name="x">X coordinate (zero based)</param>
        /// <returns></returns>
        public WorkingType this[int y, int x] {
            get {
                return WorkingMatrix[y % Height, x % Width];
            }
            set {
                WorkingMatrix[y % Height, x % Width] = value;
            }
        }

        protected abstract void computeWorkingMatrix();

        /// <summary>
        /// Clone the matrix. Make a shallow copy.
        /// </summary>
        /// <returns>Cloned matrix.</returns>
        public abstract Matrix<DefinitionType, WorkingType> Clone();

        public override void init(Image.ImageRunInfo imageRunInfo) {
            base.init(imageRunInfo);
            computeWorkingMatrix();
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
