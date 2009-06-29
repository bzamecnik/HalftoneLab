using System;
using Gimp;

namespace Halftone
{
    /// <summary>
    /// Treshold filter with a tiling matrix.
    /// </summary>
    /// <remarks>
    /// Treshold values are stored in a matrix repeatedly tiling the plane.
    /// TresholdMatrix module is used for that.
    /// </remarks>
    [Serializable]
    public class MatrixTresholdFilter : TresholdFilter
    {
        private TresholdMatrix _matrix;
        public TresholdMatrix Matrix {
            get { return _matrix; }
            set {
                if (value != null) {
                    _matrix = value;
                } else {
                    _matrix = TresholdMatrix.Generator.simpleTreshold;
                }
            }
        }

        public MatrixTresholdFilter(TresholdMatrix matrix) {
            Matrix = matrix;
        }

        public MatrixTresholdFilter() {
            Matrix = TresholdMatrix.Generator.simpleTreshold;
        }

        protected override int treshold(int intensity, int x, int y) {
            return Matrix[y, x];
        }
    }
}
