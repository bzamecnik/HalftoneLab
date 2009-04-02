// TresholdDitherAlgorithm.cs created with MonoDevelop
// User: bohous at 15:04 26.3.2009
//

using System;
using Gimp;

namespace Halftone
{
	
	public class TresholdDitherAlgorithm : DitherAlgorithm
	{
		// treshold filter
		TresholdFilter tresholdFilter;
		
        // error filter (optional)
		ErrorFilter errorFilter;

        ScanningOrder scanningOrder;
		
		public TresholdDitherAlgorithm(
            TresholdFilter tresholdFilter,
            ErrorFilter errorFilter,
            ScanningOrder scanningOrder
            )
		{
			this.tresholdFilter = tresholdFilter;
			this.errorFilter = errorFilter;
            this.scanningOrder = scanningOrder;
		}
		
		public TresholdDitherAlgorithm(TresholdFilter tresholdFilter)
		: this(tresholdFilter, null, new ScanlineScanningOrder())
		{
		}
		
		public override void run(Image image) {
            Image.IterFuncSrcDest pixelFunc;
            if (errorFilter != null) {
                pixelFunc = ((pixel) => 
                // if there is an error filter:
                // TODO: error should be of 'double' type
                {
                    Coordinate<int> coords = new Coordinate<int>(pixel.X, pixel.Y);
                    Pixel original = pixel + errorFilter.getError(coords);
                    Pixel dithered = tresholdFilter.dither(original);
                    errorFilter.setError(coords, original - dithered);
                    return dithered;
                });
            } else {
                // if there's no error filter:
                pixelFunc = ((pixel) => tresholdFilter.dither(pixel));
            }
            image.IterateSrcDest(pixelFunc, scanningOrder.getCoordsEnumerator);
		}
	}
}
