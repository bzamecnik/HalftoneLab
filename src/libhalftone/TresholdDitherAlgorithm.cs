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
                // error diffusion enabled
                pixelFunc = ((pixel) => 
                {
                    Coordinate<int> coords = new Coordinate<int>(pixel.X, pixel.Y);
                    double error = errorFilter.getError();
                    double original = (double)pixel[0] + error;
                    Pixel dithered = tresholdFilter.dither(original, pixel.X, pixel.Y);
                    errorFilter.setError(original - (double)dithered[0]);
                    errorFilter.moveNext();
                    return dithered;
                });
            } else {
                // error diffusion disabled
                pixelFunc = ((pixel) => tresholdFilter.dither(pixel));
            }
            //image.IterateSrcDestByRows(pixelFunc, scanningOrder);
            image.IterateSrcDestDirect(pixelFunc, scanningOrder);
            //image.IterateSrcDestNoOrder(pixelFunc);
		}
	}
}
