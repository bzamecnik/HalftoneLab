using System;
using Gimp;

namespace Halftone
{
    /// <summary>
    /// Threshold filter with a tileable matrix.
    /// </summary>
    /// <remarks>
    /// Threshold values are stored in a matrix repeatedly tiling the plane
    /// (ie. the image). ThresholdMatrix module is used for that.
    /// </remarks>
    [Serializable]
    public class MatrixThresholdFilter : ThresholdFilter
    {
        private ThresholdMatrix _matrix;

        /// <summary>
        /// Tileable matrix of threshold values.
        /// </summary>
        /// <remarks>
        /// set(): null value is treated as setting the default matrix.
        /// </remarks>
        public ThresholdMatrix Matrix {
            get { return _matrix; }
            set {
                if (value != null) {
                    _matrix = value;
                } else {
                    _matrix = ThresholdMatrix.Generator.simpleThreshold;
                }
            }
        }

        /// <summary>
        /// Create a matrix threshold filter with given matrix.
        /// </summary>
        /// <param name="matrix"></param>
        public MatrixThresholdFilter(ThresholdMatrix matrix) {
            Matrix = matrix;
        }

        /// <summary>
        /// Create a matrix threshold filter with the default matrix
        /// (currently 1x1 matrix with threshold at 50% grey).
        /// </summary>
        public MatrixThresholdFilter() {
            Matrix = ThresholdMatrix.Generator.simpleThreshold;
        }

        /// <summary>
        /// Get a threshold value for given pixel from the tileable matrix.
        /// </summary>
        /// <param name="intensity">Pixel intensity (not used)</param>
        /// <param name="x">Pixel X coordinate in the image</param>
        /// <param name="y">Pixel Y coordinate in the image</param>
        /// <returns></returns>
        protected override int threshold(int intensity, int x, int y) {
            return Matrix[y, x];
        }
    }
}
