// MatrixTresholdFilter.cs created with MonoDevelop
// User: bohous at 15:28 26.3.2009
//

using System;
using Gimp;

namespace Halftone
{
	public class MatrixTresholdFilter : TresholdFilter
	{
		private TresholdMatrix _tresholdMatrix;
		
		public MatrixTresholdFilter(TresholdMatrix matrix) {
			this._tresholdMatrix = matrix;
		}

        protected override int treshold(Pixel pixel)
		{
			return _tresholdMatrix[pixel.Y, pixel.X];
		}		
	}
}
