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
        public TresholdFilter TresholdFilter {
            get;
            set;
        }
		
        // error filter (optional)
        public ErrorFilter ErrorFilter {
            get;
            set;
        }

        public ScanningOrder ScanningOrder {
            get;
            set;
        }
		
		public TresholdDitherAlgorithm(
            TresholdFilter tresholdFilter,
            ErrorFilter errorFilter,
            ScanningOrder scanningOrder
            )
		{
			TresholdFilter = tresholdFilter;
			ErrorFilter = errorFilter;
            ScanningOrder = scanningOrder;
		}
		
		public TresholdDitherAlgorithm(TresholdFilter tresholdFilter)
		: this(tresholdFilter, null, new ScanlineScanningOrder())
		{
		}

        public TresholdDitherAlgorithm()
            : this(new MatrixTresholdFilter(), null, new ScanlineScanningOrder()) {
        }

		public override void run(Image image) {
            Image.IterFuncSrcDest pixelFunc;
            if (ErrorFilter != null) {
                // error diffusion enabled
                pixelFunc = ((pixel) => 
                {
                    Coordinate<int> coords = new Coordinate<int>(pixel.X, pixel.Y);
                    double error = ErrorFilter.getError();
                    double original = (double)pixel[0] + error;
                    Pixel dithered = TresholdFilter.dither(original, pixel.X, pixel.Y);
                    ErrorFilter.setError(original - (double)dithered[0]);
                    ErrorFilter.moveNext();
                    return dithered;
                });
            } else {
                // error diffusion disabled
                pixelFunc = ((pixel) => TresholdFilter.dither(pixel));
            }
            //image.IterateSrcDestByRows(pixelFunc, ScanningOrder);
            image.IterateSrcDestDirect(pixelFunc, ScanningOrder);
            //image.IterateSrcDestNoOrder(pixelFunc);
		}
	}
}
