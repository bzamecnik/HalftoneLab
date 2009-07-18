using System;
using Gimp;

namespace Halftone
{
    /// <summary>
    /// Threshold halftone algorithm acts as a base class for halftone algorithms
    /// which perform bi-level intensity quantization using a threshold filter.
    /// Optional error-diffusion can be accomplished as well.
    /// </summary>
    /// <remarks>
    /// Order in which image pixels are processed is given by a
    /// ScanningOrder module.
    /// </remarks>
    [Module(TypeName="Thresholding algorithm")]
    [Serializable]
    public class ThresholdHalftoneMethod : PointHalftoneMethod
    {
        /// <summary>
        /// Threshold filter module. Mandatory.
        /// </summary>
        public ThresholdFilter ThresholdFilter { get; set; }

        private ErrorFilter _errorFilter;
        /// <summary>
        /// Error filter module. Optional (leave it null not to use it).
        /// </summary>
        public ErrorFilter ErrorFilter {
            get { return _errorFilter; }
            set {
                _errorFilter = value;
                UseErrorFilter = _errorFilter != null;
            }
        }

        /// <summary>
        /// Turn error filter on/off.
        /// </summary>
        public bool UseErrorFilter { get; set; }

        /// <summary>
        /// Module giving order in which image pixels are processed.
        /// </summary>
        public ScanningOrder ScanningOrder { get; set; }

        /// <summary>
        /// Is error filter set and ready to use?
        /// </summary>
        private bool ErrorFilterEnabled {
            get {
                return (UseErrorFilter) && (ErrorFilter != null) &&
                    ErrorFilter.Initialized;
            }
        }

        /// <summary>
        /// Create a threshold halftone algorithm skeleton.
        /// </summary>
        /// <param name="thresholdFilter">Threshold filter</param>
        /// <param name="errorFilter">Error filter (optional)</param>
        /// <param name="scanningOrder">Scanning order</param>
        public ThresholdHalftoneMethod(
            ThresholdFilter thresholdFilter,
            ErrorFilter errorFilter,
            ScanningOrder scanningOrder
            )
        {
            ThresholdFilter = thresholdFilter;
            ErrorFilter = errorFilter;
            UseErrorFilter = errorFilter != null;
            ScanningOrder = scanningOrder;
        }

        /// <summary>
        /// Create a threshold halftone algorithm skeleton with no error filter
        /// and default scanning order (scanline).
        /// </summary>
        /// <param name="thresholdFilter"></param>
        public ThresholdHalftoneMethod(ThresholdFilter thresholdFilter)
            : this(thresholdFilter, null, new ScanlineScanningOrder()) {}

        /// <summary>
        /// Create a threshold halftone algorithm skeleton with default threshold
        /// filter (MatrixThresholdFilter), no error filter and default scanning
        /// order (scanline).
        /// </summary>
        public ThresholdHalftoneMethod()
            : this(new MatrixThresholdFilter(), null,
            new ScanlineScanningOrder()) {}

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
                    Pixel quantized = ThresholdFilter.quantize(original,
                        pixel.X, pixel.Y);
                    ErrorFilter.setError(original - (double)quantized[0],
                        (int)original);
                    ErrorFilter.moveNext();
                    return quantized;
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
