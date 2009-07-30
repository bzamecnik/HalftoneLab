using System;
using System.Collections.Generic;
using Gimp;

namespace HalftoneLab
{
    // TODO:
    // - separate the cell processing to CellHalftoneAlgorithm
    // *- compute max cell size adaptively
    //   - it would be better to separate the gradient computing or
    //     approximation to another module
    //   - candidate gradient computing algorithms:
    //     *- difference with last pixel
    //     - Sobel filter - internal or preferablby external (via
    //       a GIMP plug-in)
    //     - external Laplace transformation of the whole image

    /// <summary>
    /// Adaptive clustering algorithm along a space-filling curve by
    /// Velho & Gomes.
    /// <remarks>
    /// <para>
    /// The image is processed along a SFC not by separate pixels, but rather
    /// by block of consecutive pixels, so called "cells". An average
    /// intensity in each cell is computed and a proportional number of pixels
    /// inside the cell is turned black (the others are white). The
    /// quantization error is distributed within whole cells using an optional
    /// ErrorFilter. The cell size is controlled via MaxCellSize parameter.
    /// </para>
    /// <para>
    /// Two enhancements of the algorithm can be optionally employed -
    /// separately of together. Cluster positioning moves the cluster center
    /// to the darkest pixel withing the cell. Adaptive clustering varies the
    /// cell size between MinCellSize and MaxCellSize according to the amount
    /// of local detail. This is currently approximated by the different
    /// between previous and current pixel intensity.
    /// </para>
    /// </remarks>
    /// </summary>
    [Module(TypeName="SFC clustering method")]
    [Serializable]
    public class SFCClusteringMethod : CellHalftoneMethod
    {
        /// <summary>
        /// Vector error filter (optional). If set to null, it will not be
        /// used.
        /// </summary>
        public VectorErrorFilter ErrorFilter {
            get;
            set;
        }

        /// <summary>
        /// The type of image scanning limited to space-filling curves.
        /// </summary>
        public SFCScanningOrder ScanningOrder {
            get;
            set;
        }

        /// <summary>
        /// Error filter is set and enabled.
        /// </summary>
        bool ErrorFilterEnabled {
            get {
                return UseErrorFilter && (ErrorFilter != null) &&
                    ErrorFilter.Initialized;
            }
        }

        /// <summary>
        /// Enable or disable error filter without deleting it.
        /// </summary>
        public bool UseErrorFilter { get; set; }

        /// <summary>
        /// Maximum cell size.
        /// </summary>
        public int MaxCellSize { get; set; }

        /// <summary>
        /// Maximum cell size. It is taken into consideration only if
        /// adaptive clustering is enabled. It must be lower or equal than
        /// MaxCellSize.
        /// </summary>
        public int MinCellSize { get; set; }

        /// <summary>
        /// Position the cluster to the darkest pixel within the cell?
        /// </summary>
        public bool UseClusterPositioning { get; set; }

        /// <summary>
        /// Adjust cell sizes to the amount of local detail?
        /// </summary>
        public bool UseAdaptiveClustering { get; set; }

        /// <summary>
        /// Create a new SFC clustering method instance using some
        /// default parameters.
        /// </summary>
        public SFCClusteringMethod() {
            ErrorFilter = new VectorErrorFilter();
            UseErrorFilter = true;
            ScanningOrder = new HilbertScanningOrder();
            MaxCellSize = 7;
            MinCellSize = 2;
            UseClusterPositioning = true;
            UseAdaptiveClustering = true;
        }

        public override void run(Image image) {
            Image.ImageRunInfo imageRunInfo = new Image.ImageRunInfo()
            {
                ScanOrder = ScanningOrder,
                Height = image.Height,
                Width = image.Width
            };
            init(imageRunInfo);

            // Size of the current cluster
            int currentCellSize = MaxCellSize;
            // Index of current pixel in the current cell
            int currentCellPixel = 1; // 1..currentCellSize
            // Cell with the least brightness in the current cluster,
            // center of the filled part of cluster will be positioned onto it
            int darkestCellPixel = 1; // 1..currentCellSize
            // Intensity of the darkestCellPixel
            double minCellIntensity = 255; // TODO: max pixel value
            double totalCellIntensity = 0;

            Pixel blackPixel = new Pixel(new byte[1] { 0 });
            Pixel whitePixel = new Pixel(new byte[1] { 255 });

            // Pixels in the current cell waiting being filled (or not).
            // X: cellPixels[*, 0], Y: cellPixels[*, 1]
            int[,] cellPixels = new int[MaxCellSize, 2];

            int clusterSize = 0;
            int clusterStartPos = 0;

            int visitedPixels = 0;
            int totalPixels = image.Height * image.Width;

            //// adaptive clustering - computing amount of local detail
            //int[,] sobelMatrix = {{-1, 0, 1}, {-2, 0, 2}, {-1, 0, 1}};

            int intensity = 0;
            int previousIntensity = 0;

            double error = 0.0; // current quantization error

            // accelerator:
            double maxCellSizeLog2 = Math.Log(MaxCellSize) / Math.Log(2);

            IEnumerator<Coordinate<int>> scanOrderEnum =
                ScanningOrder.getCoordsEnumerable().GetEnumerator();
            image.initBuffer();

            while (scanOrderEnum.MoveNext()) {
                int x, y; // current coordinates
                x = scanOrderEnum.Current.X;
                y = scanOrderEnum.Current.Y;
                visitedPixels++;

                // collect intensities and coordinates of cell's pixels
                Pixel pixel = image.getPixel(x, y);
                // for computing path direction vector
                previousIntensity = intensity;
                intensity = pixel[0];
                double intensityWithError = intensity;
                if (ErrorFilterEnabled) {
                    intensityWithError += ErrorFilter.getError();
                }
                totalCellIntensity += intensityWithError;
                if (intensityWithError < minCellIntensity) {
                    minCellIntensity = intensityWithError;
                    darkestCellPixel = currentCellPixel;
                }

                // adaptive clustering
                if (UseAdaptiveClustering) {
                    // Compute (approximated) gradient for current pixel.
                    double derivative = 0.0;

                    // difference with the previous pixel
                    derivative = (intensity - previousIntensity) / 255.0;

                    // Compute maximum allowed cell size for this pixel.
                    // Adjust currentCellSize according to computed value.
                    int maxAllowedClusterSize = Math.Max(Math.Min((int)(
                            Math.Pow(2, (1 - Math.Abs(derivative)) * maxCellSizeLog2)
                        ), MaxCellSize), MinCellSize);
                    currentCellSize = Math.Max(maxAllowedClusterSize, currentCellPixel);
                }

                cellPixels[currentCellPixel - 1, 0] = x;
                cellPixels[currentCellPixel - 1, 1] = y;

                // What about if the cell is not completed?
                // - don't overwrite previously filled pixels remaining in the cellPixels array

                if ((currentCellPixel < currentCellSize) &&
                    (visitedPixels < totalPixels)) // TODO: ... && order.isNext()
                {
                    // walk the cell
                    currentCellPixel++;
                } else {
                    // cell is walked, fill the proper ratio of cells

                    double whitePixelsRatio = totalCellIntensity / 255.0;
                    int whitePixelsNumber = (int)Math.Truncate(whitePixelsRatio + 0.5);
                    //int whitePixelsNumber = (int)whitePixelsRatio;
                    error = (whitePixelsRatio - whitePixelsNumber) * 255.0;
                    clusterSize = currentCellSize - whitePixelsNumber;

                    if (UseClusterPositioning) {
                        // move the cluster - position its center to the darkestCellPixel

                        // compute the starting position
                        // THINK: try to add a bit of randomness inside here
                        clusterStartPos = darkestCellPixel - (int)(clusterSize / 2.0);
                        // correct the position if it exceeds the cell
                        clusterStartPos = Math.Max(clusterStartPos, 0);
                        clusterStartPos = Math.Min(clusterStartPos, currentCellSize - clusterSize);
                    }

                    for (int i = 0; i < clusterStartPos; i++) {
                        image.setPixel(cellPixels[i, 0], cellPixels[i, 1], whitePixel);
                    }

                    for (int i = clusterStartPos; i < clusterSize + clusterStartPos; i++) {
                        // Fill a cluster proportional to cell's average intensity with black.
                        image.setPixel(cellPixels[i, 0], cellPixels[i, 1], blackPixel);
                    }

                    for (int i = clusterSize + clusterStartPos; i < currentCellSize; i++) {
                        image.setPixel(cellPixels[i, 0], cellPixels[i, 1], whitePixel);
                    }

                    // start a new cell
                    currentCellPixel = 1;
                    darkestCellPixel = 1;
                    minCellIntensity = 255;
                    totalCellIntensity = 0;
                    if (ErrorFilterEnabled) {
                        ErrorFilter.setError(error, 0);
                    }
                    currentCellSize = MaxCellSize;
                }
                if (ErrorFilterEnabled) {
                    ErrorFilter.moveNext();
                }
            }
            image.flushBuffer();
        }

        public override void init(Image.ImageRunInfo imageRunInfo) {
            base.init(imageRunInfo);
            if (ErrorFilter != null) {
                ErrorFilter.init(imageRunInfo);
            }
            ScanningOrder.init(imageRunInfo);
        }
    }

}
