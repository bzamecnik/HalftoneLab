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

        static int loop = 100;

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
                    //if ((loop > 0) && (pixel.Y < 5) && ((pixel.X < 10) || ((image.Width - pixel.X) < 10))) {
                    //    Console.WriteLine("[{0}, {1}]: pixel {2} + error {3} = {4} (dithered: {5})",
                    //        pixel.Y, pixel.X, pixel[0], error, original, dithered[0]);
                    //    loop--;
                    //}
                    //if ((loop > 0) && (pixel.Y < 2)) {
                    //    Console.WriteLine("[{0}, {1}], error: {2}", pixel.Y, pixel.X, error);
                    //    loop--;
                    //}
                    errorFilter.setError(original - (double)dithered[0]);
                    errorFilter.moveNext();
                    return dithered;
                });
            } else {
                // error diffusion disabled
                pixelFunc = ((pixel) => tresholdFilter.dither(pixel));
            }
            image.IterateSrcDestByRows(pixelFunc, scanningOrder.getCoordsEnumerator);
		}
	}
}
