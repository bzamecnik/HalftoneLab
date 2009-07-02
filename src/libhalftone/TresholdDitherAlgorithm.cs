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
    /// Order in which image pixels are processed is given by a
    /// ScanningOrder module.
    /// </remarks>
    [Serializable]
	public class TresholdDitherAlgorithm : DitherAlgorithm
	{
        /// <summary>
        /// Treshold filter module. Mandatory.
        /// </summary>
        public TresholdFilter TresholdFilter {
            get;
            set;
        }
		
        /// <summary>
        /// Error filter module. Optional (leave it null not to use it).
        /// </summary>
        public ErrorFilter ErrorFilter {
            get;
            set;
        }

        /// <summary>
        /// Module giving order in which image pixels are processed.
        /// </summary>
        public ScanningOrder ScanningOrder {
            get;
            set;
        }

        /// <summary>
        /// Is error filter set and ready to use?
        /// </summary>
        private bool ErrorFilterEnabled {
            get {
                return (ErrorFilter != null) && ErrorFilter.Initialized;
            }
        }

        /// <summary>
        /// Create a treshold dither algorithm skeleton.
        /// </summary>
        /// <param name="tresholdFilter">Treshold filter</param>
        /// <param name="errorFilter">Error filter (optional)</param>
        /// <param name="scanningOrder">Scanning order</param>
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
		
        /// <summary>
        /// Create a treshold dither algorithm skeleton with no error filter
        /// and default scanning order (scanline).
        /// </summary>
        /// <param name="tresholdFilter"></param>
		public TresholdDitherAlgorithm(TresholdFilter tresholdFilter)
		: this(tresholdFilter, null, new ScanlineScanningOrder())
		{
		}

        /// <summary>
        /// Create a treshold dither algorithm skeleton with default treshold
        /// filter (MatrixTresholdFilter), no error filter and default scanning
        /// order (scanline).
        /// </summary>
        public TresholdDitherAlgorithm()
            : this(new MatrixTresholdFilter(), null, new ScanlineScanningOrder()) {
        }

        /// <summary>
        /// Run the algorithm over the image.
        /// </summary>
        /// <remarks>
        /// Tip: You can copy the input image, use the original data, write
        /// to the copy and at the end replace the original with the copy.
        /// </remarks>
        /// <param name="image">Image - both input and output</param>
		public override void run(Image image) {
            if ((TresholdFilter == null) || (ScanningOrder == null)) {
                return; // TODO: throw an exception
            }
            Image.ImageRunInfo imageRunInfo = new Image.ImageRunInfo()
            {
                ScanOrder = ScanningOrder,
                Height = image.Height,
                Width = image.Width
            };
            init(imageRunInfo);
            
            Image.IterFuncSrcDest pixelFunc;
            if (ErrorFilterEnabled) {
                // error diffusion enabled
                pixelFunc = ((pixel) => 
                {
                    double error = ErrorFilter.getError();
                    double original = (double)pixel[0] + error;
                    Pixel dithered = TresholdFilter.quantize(original, pixel.X, pixel.Y);
                    ErrorFilter.setError(original - (double)dithered[0]);
                    ErrorFilter.moveNext();
                    return dithered;
                });
            } else {
                // error diffusion disabled
                pixelFunc = ((pixel) => TresholdFilter.quantize(pixel));
            }
            image.IterateSrcDestDirect(pixelFunc, ScanningOrder);
		}

        public override void init(Image.ImageRunInfo imageRunInfo) {
            base.init(imageRunInfo);
            if (ErrorFilter != null) {
                ErrorFilter.init(imageRunInfo);
            }
            TresholdFilter.init(imageRunInfo);
            ScanningOrder.init(imageRunInfo);
        }
	}
}
