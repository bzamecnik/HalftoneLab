// MatrixErrorFilter.cs created with MonoDevelop
// User: bohous at 15:31 26.3.2009
//

using System;
using Gimp;

namespace Halftone
{
	public class MatrixErrorFilter : ErrorFilter
	{
        // matrix of error filter weights
        double[,] _matrix;
        
        // offset of source pixel, that is pixel currently processed
        // from where the quantization error is distributed
        Coordinate<int> _sourcePixelOffset;
        
        // error buffer
        ErrorBuffer _buffer;

        public MatrixErrorFilter(
            double[,] matrix,
            Coordinate<int> sourcePixelOffset,
            ErrorBuffer buffer)
        {
            // TODO: check if matrix is ok
            _matrix = matrix;
            // scale down the coefficients (their sum must be 1.0)
            // TODO: scaling should be done in a Prototype
            int coeffSum = 0;
            foreach (int coef in matrix) {
                coeffSum += coef;
            }
            if (coeffSum != 0) {
                for (int y = 0; y < matrix.GetLength(0); y++) {
                    for (int x = 0; x < matrix.GetLength(1); x++) {
                        matrix[y, x] /= coeffSum;
                    }
                }
            }

            // check if source pixel position lies in the matrix
            _sourcePixelOffset = sourcePixelOffset;
            _buffer = buffer;
        }

        public override double getError() {
            return _buffer.getError();
        }
        
        // diffuse error value from given pixel to neighbor pixels
        public override void setError(double error) {
            //first line goes from the source position
            for (int x = _sourcePixelOffset.X; x < _matrix.GetLength(1); x++) {
                int y = _sourcePixelOffset.Y;
                _buffer.setError(x - _sourcePixelOffset.X, 0, _matrix[y, x] * error);
            }
            for (int y = _sourcePixelOffset.Y + 1; y < _matrix.GetLength(0); y++) {
                for (int x = 0; x < _matrix.GetLength(1); x++) {
                    _buffer.setError(x - _sourcePixelOffset.X, y - _sourcePixelOffset.Y,
                        _matrix[y, x] * error);
                }
            }
        }

        public override void moveNext() {
            _buffer.moveNext();
        }
	}
}
