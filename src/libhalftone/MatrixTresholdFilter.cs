using System;
using Gimp;

namespace Halftone
{
    [Serializable]
    public class MatrixTresholdFilter : TresholdFilter
    {
        private TresholdMatrix _tresholdMatrix;

        public MatrixTresholdFilter(TresholdMatrix matrix) {
            _tresholdMatrix = matrix;
        }

        public MatrixTresholdFilter() {
            _tresholdMatrix = new TresholdMatrix(new int[1, 1] { { 128 } });
        }

        protected override int treshold(Pixel pixel) {
            return _tresholdMatrix[pixel.Y, pixel.X];
        }
    }
}
