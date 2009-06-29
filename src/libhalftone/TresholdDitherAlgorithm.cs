using System;
using Gimp;

namespace Halftone
{
    /// <summary>
    /// Treshold dither algorithm acts as a base class for dither algorithms
    /// which perform bi-level intensity quantization using a treshold filter.
    /// Optional error-diffusion can be accomplished as well.
    /// </summary>
    /// <remarks>
    /// Order in which image pixels are passed through is given by a
    /// ScanningOrder module.
    /// </remarks>
    [Serializable]
	public class TresholdDitherAlgorithm : DitherAlgorithm
	{
        public TresholdFilter TresholdFilter {
            get;
            set;
        }
		
        // (optional)
        public ErrorFilter ErrorFilter {
            get;
            set;
        }

        public ScanningOrder ScanningOrder {
            get;
            set;
        }

        private bool ErrorFilterEnabled {
            get {
                return (ErrorFilter != null) && ErrorFilter.Initialized;
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
            Image.ImageRunInfo imageRunInfo = new Image.ImageRunInfo()
            {
                ScanOrder = ScanningOrder,
                Height = image.Height,
                Width = image.Width
            };
            Console.Out.WriteLine("TresholdDitherAlgorithm.run()");
            init(imageRunInfo);
            
            Image.IterFuncSrcDest pixelFunc;
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
            image.IterateSrcDestDirect(pixelFunc, ScanningOrder);
		}

        public override void init(Image.ImageRunInfo imageRunInfo) {
            Console.Out.WriteLine("TresholdDitherAlgorithm.init ()");
            base.init(imageRunInfo);
            if (ErrorFilter != null) {
                ErrorFilter.init(imageRunInfo);
            }
            TresholdFilter.init(imageRunInfo);
            ScanningOrder.init(imageRunInfo);
        }
	}
}
