// MatrixErrorFilter.cs created with MonoDevelop
// User: bohous at 15:31 26.3.2009
//

using System;
using Gimp;

namespace Halftone
{
	public class MatrixErrorFilter : ErrorFilter
	{
        ErrorBuffer _buffer;
        double[,] _matrix;
        Coordinate<int> _sourcePixelOffset;

        MatrixErrorFilter(
            double[,] matrix,
            Coordinate<int> sourcePixelOffset,
            ScanningOrder scanOrder)
        {
            // check if matrix is ok
            _matrix = matrix;
            // check if source pixel position lies in the matrix
            _sourcePixelOffset = sourcePixelOffset;
            _buffer = ErrorBuffer.createFromScanningOrder(
                scanOrder,
                matrix.GetLength(1),
                matrix.GetLength(0),
                new Coordinate<int>(0, 0));
        }

        public override double getError() {
            return _buffer.getError(_sourcePixelOffset);
        }

        // diffuse error value from given pixel to neighbor pixels
        public override void setError(double error) {
            for (int y = 0; y < _matrix.GetLength(0); y++) {
                for (int x = 0; x < _matrix.GetLength(1); x++) {
                    _buffer.setError(new Coordinate<int>(x, y), _matrix[y, x] * error);
                }
            }
        }

        public override void moveNext() {
            _buffer.moveNext();
        }
	}
}
