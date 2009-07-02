using System;
using Gimp;

namespace Halftone
{
    /// <summary>
    /// Treshold filter with a tileable matrix.
    /// </summary>
    /// <remarks>
    /// Treshold values are stored in a matrix repeatedly tiling the plane
    /// (ie. the image). TresholdMatrix module is used for that.
    /// </remarks>
    [Serializable]
    public class MatrixTresholdFilter : TresholdFilter
    {
        private TresholdMatrix _matrix;

        /// <summary>
        /// Tileable matrix of treshold values.
        /// </summary>
        /// <remarks>
        /// set(): null value is treated as setting the default matrix.
        /// </remarks>
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

        /// <summary>
        /// Create a matrix treshold filter with given matrix.
        /// </summary>
        /// <param name="matrix"></param>
        public MatrixTresholdFilter(TresholdMatrix matrix) {
            Matrix = matrix;
        }

        /// <summary>
        /// Create a matrix treshold filter with the default matrix
        /// (currently 1x1 matrix with treshold at 50% grey).
        /// </summary>
        public MatrixTresholdFilter() {
            Matrix = TresholdMatrix.Generator.simpleTreshold;
        }

        /// <summary>
        /// Get a treshold value for given pixel from the tileable matrix.
        /// </summary>
        /// <param name="intensity">Pixel intensity (not used)</param>
        /// <param name="x">Pixel X coordinate in the image</param>
        /// <param name="y">Pixel Y coordinate in the image</param>
        /// <returns></returns>
        protected override int treshold(int intensity, int x, int y) {
            return Matrix[y, x];
        }
    }
}
