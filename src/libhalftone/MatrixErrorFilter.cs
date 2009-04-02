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
        Pixel[,] _matrix;
        Coordinate<int> _sourcePixelPosition;

        MatrixErrorFilter(Pixel[,] matrix, Coordinate<int> sourcePixelPosition) {
            // check if matrix is ok
            _matrix = matrix;
            // check if source pixel position lies in the matrix
            _sourcePixelPosition = sourcePixelPosition;
        }

        public override Pixel getError(Coordinate<int> coords) {
            return new Pixel(0);
        }

        // diffuse error value from given pixel to neighbor pixels
        public override void setError(Coordinate<int> coords, Pixel pixel) {
        
        }
	}
}
