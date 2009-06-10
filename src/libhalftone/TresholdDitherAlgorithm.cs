using System;
using Gimp;

namespace Halftone
{
	
	public class TresholdDitherAlgorithm : DitherAlgorithm
	{
		// treshold filter
        // NOTE: to be serialized
        public TresholdFilter TresholdFilter {
            get;
            set;
        }
		
        // error filter (optional)
        // NOTE: to be serialized
        public ErrorFilter ErrorFilter {
            get;
            set;
        }

        // NOTE: to be serialized
        public ScanningOrder ScanningOrder {
            get;
            set;
        }

        bool ErrorFilterEnabled {
            get {
                return ErrorFilter != null;
            }
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
            if (ErrorFilterEnabled &&
                !ErrorFilter.initBuffer(ScanningOrder, image.Height, image.Width)) {
                ErrorFilter = null; // disable the filter if the buffer is not ok
            }
            if (ErrorFilterEnabled) {
                // error diffusion enabled
                pixelFunc = ((pixel) => 
                {
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
            //pixelFunc = ((pixel) => { pixel[0] = 127; return pixel; });
            //int i = 0;
            //pixelFunc = ((pixel) => { pixel[0] = i; i = (i + 2) % 256; return pixel; });
            //image.IterateSrcDestByRows(pixelFunc, ScanningOrder);
            image.IterateSrcDestDirect(pixelFunc, ScanningOrder);
            //image.IterateSrcDestNoOrder(pixelFunc);
		}
	}
}
