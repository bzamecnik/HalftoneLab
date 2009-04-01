// MatrixErrorFilter.cs created with MonoDevelop
// User: bohous at 15:31 26.3.2009
//

using System;

namespace Halftone
{
	public class MatrixErrorFilter : ErrorFilter
	{
        ErrorBuffer _buffer;
        Pixel[,] _matrix;
        Coords _sourcePixelPosition;

        MatrixErrorFilter(Pixel[,] matrix, Coords sourcePixelPosition) {
            // check if matrix is ok
            _matrix = matrix;
            // check if source pixel position lies in the matrix
            _sourcePixelPosition = sourcePixelPosition;
        }

        public override Pixel getError(Coords coords) {
            return new Pixel(0);
        }

        // diffuse error value from given pixel to neighbor pixels
        public override void setError(Coords coords, Pixel pixel) {
        
        }
	}
}
