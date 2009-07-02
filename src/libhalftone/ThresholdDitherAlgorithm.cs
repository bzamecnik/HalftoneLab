using System;
using Gimp;

namespace Halftone
{
    /// <summary>
    /// Threshold dither algorithm acts as a base class for dither algorithms
    /// which perform bi-level intensity quantization using a threshold filter.
    /// Optional error-diffusion can be accomplished as well.
    /// </summary>
    /// <remarks>
    /// Order in which image pixels are processed is given by a
    /// ScanningOrder module.
    /// </remarks>
    [Serializable]
	public class ThresholdDitherAlgorithm : DitherAlgorithm
	{
        /// <summary>
        /// Threshold filter module. Mandatory.
        /// </summary>
        public ThresholdFilter ThresholdFilter {
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
        /// Create a threshold dither algorithm skeleton.
        /// </summary>
        /// <param name="thresholdFilter">Threshold filter</param>
        /// <param name="errorFilter">Error filter (optional)</param>
        /// <param name="scanningOrder">Scanning order</param>
		public ThresholdDitherAlgorithm(
            ThresholdFilter thresholdFilter,
            ErrorFilter errorFilter,
            ScanningOrder scanningOrder
            )
		{
			ThresholdFilter = thresholdFilter;
			ErrorFilter = errorFilter;
            ScanningOrder = scanningOrder;
		}
		
        /// <summary>
        /// Create a threshold dither algorithm skeleton with no error filter
        /// and default scanning order (scanline).
        /// </summary>
        /// <param name="thresholdFilter"></param>
		public ThresholdDitherAlgorithm(ThresholdFilter thresholdFilter)
		: this(thresholdFilter, null, new ScanlineScanningOrder())
		{
		}

        /// <summary>
        /// Create a threshold dither algorithm skeleton with default threshold
        /// filter (MatrixThresholdFilter), no error filter and default scanning
        /// order (scanline).
        /// </summary>
        public ThresholdDitherAlgorithm()
            : this(new MatrixThresholdFilter(), null, new ScanlineScanningOrder()) {
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
            if ((ThresholdFilter == null) || (ScanningOrder == null)) {
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
                    Pixel dithered = ThresholdFilter.quantize(original, pixel.X, pixel.Y);
                    ErrorFilter.setError(original - (double)dithered[0]);
                    ErrorFilter.moveNext();
                    return dithered;
                });
            } else {
                // error diffusion disabled
                pixelFunc = ((pixel) => ThresholdFilter.quantize(pixel));
            }
            image.IterateSrcDestDirect(pixelFunc, ScanningOrder);
		}

        public override void init(Image.ImageRunInfo imageRunInfo) {
            base.init(imageRunInfo);
            if (ErrorFilter != null) {
                ErrorFilter.init(imageRunInfo);
            }
            ThresholdFilter.init(imageRunInfo);
            ScanningOrder.init(imageRunInfo);
        }
	}
}
