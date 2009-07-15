using System;
using System.Collections.Generic;
using Gimp;

namespace Halftone
{

    // TODO:
    // - solve problem of outputting the last cell
    //   - how to ask the scanning order whether it has more pixels?
    // - compute max cell size adaptively
    //   - it would be better to separate the gradient computing or
    //     approximation to another module
    //   - candidate gradient computing algorithms:
    //     - difference with last pixel
    //     - Sobel matrix
    //     - external Laplace transformation of the whole image

    /// <summary>
    /// Adaptive clustering algorithm on a Space-Filling Curve by
    /// Velho & Gomes.
    /// <remarks>
    /// The quantization error is distributed within whole cells.
    /// </remarks>
    /// </summary>
    [Module(TypeName="SFC clustering algorithm")]
    [Serializable]
    public class SFCClusteringMethod : CellHalftoneMethod
    {
        // error filter (optional)
        public VectorErrorFilter ErrorFilter {
            get;
            set;
        }

        public SFCScanningOrder ScanningOrder {
            get;
            set;
        }

        bool ErrorFilterEnabled {
            get {
                return UseErrorFilter && (ErrorFilter != null) &&
                    ErrorFilter.Initialized;
            }
        }

        public bool UseErrorFilter { get; set; }

        // Maximum cell size
        public int MaxCellSize { get; set; }

        // Maximum cell size
        public int MinCellSize { get; set; }

        // Position the cluster to the darkest pixel within the cell?
        public bool UseClusterPositioning { get; set; }

        // Adjust cluster sizes to amount of local detail?
        public bool UseAdaptiveClustering { get; set; }

        public SFCClusteringMethod() {
            ErrorFilter = new VectorErrorFilter();
            UseErrorFilter = true;
            ScanningOrder = new HilbertScanningOrder();
            MaxCellSize = 31;
            MinCellSize = 3;
            UseClusterPositioning = true;
            UseAdaptiveClustering = false; // TODO: true
        }

        public override void run(Image image) {
            //UseClusterPositioning = false;
            //ErrorFilter = null;
            Console.Out.WriteLine("MaxCellSize: {0}", MaxCellSize);
            Console.Out.WriteLine("MinCellSize: {0}", MinCellSize);
            Console.Out.WriteLine("cluster positioning: {0}", UseClusterPositioning);
            Console.Out.WriteLine("adaptive clustering: {0}", UseAdaptiveClustering);
            Console.Out.WriteLine("error filter enabled: {0}", ErrorFilterEnabled);

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

            // pixels in the current cell waiting being filled (or not)
            Coordinate<int>[] cellPixels = new Coordinate<int>[MaxCellSize];

            int clusterSize = 0;
            int clusterStartPos = 0;

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
                Coordinate<int> coords = scanOrderEnum.Current;

                // collect intensities and coordinates of cell's pixels
                Pixel pixel = image.getPixel(coords.X, coords.Y);
                // for computing path direction vector
                previousIntensity = intensity;
                intensity = pixel[0];
                double intensityWithError = intensity;
                if (ErrorFilterEnabled) {
                    intensityWithError += ErrorFilter.getError();
                    //Console.Out.WriteLine("get error: {0}", ErrorFilter.getError());
                }
                totalCellIntensity += intensityWithError;
                if (intensityWithError < minCellIntensity) {
                    minCellIntensity = intensityWithError;
                    darkestCellPixel = currentCellPixel;
                }

                // adaptive clustering:
                // * compute maximum allowed cell size for this pixel
                //   * compute (approximated) gradient for current pixel
                // * adjust currentCellSize according to computed value
                if (UseAdaptiveClustering) {
                    double derivative = 0.0;
                    // TODO: compute the derivative
                    // ...

                    // difference with the previous pixel
                    derivative = (intensity - previousIntensity) / 255.0;
                    //Console.Out.WriteLine("derivative: {0}", derivative);

                    int maxAllowedClusterSize = Math.Max(Math.Min((int)(
                            Math.Pow(2, (1 - Math.Abs(derivative)) * maxCellSizeLog2)
                        ), MaxCellSize), MinCellSize);
                    currentCellSize = Math.Max(maxAllowedClusterSize, currentCellPixel);
                    //Console.Out.WriteLine("currentCellSize: {0}", currentCellSize);
                }

                cellPixels[currentCellPixel - 1] = coords;

                // What about if the cell is not completed?
                // - don't overwrite previously filled pixels remaining in the cellPixels array

                if ((currentCellPixel < currentCellSize)) { // TODO: ... && order.isNext()
                    // walk the cell
                    currentCellPixel++;
                } else {
                    // cell is walked, fill the proper ratio of cells

                    double whitePixelsRatio = totalCellIntensity / 255.0;
                    int whitePixelsNumber = (int)Math.Truncate(whitePixelsRatio + 0.5);
                    //int whitePixelsNumber = (int)whitePixelsRatio;
                    error = (whitePixelsRatio - whitePixelsNumber) * 255.0;
                    clusterSize = currentCellSize - whitePixelsNumber;
                    //Console.WriteLine("totalCellIntensity: {0}", totalCellIntensity);
                    //Console.WriteLine("whitePixelsNumber: {0}", whitePixelsNumber);
                    //Console.WriteLine("clusterSize: {0}", clusterSize);

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
                        Coordinate<int> c = cellPixels[i];
                        image.setPixel(c.X, c.Y, whitePixel);
                    }

                    for (int i = clusterStartPos; i < clusterSize + clusterStartPos; i++) {
                        // Fill a cluster proportional to cell's average intensity with black.
                        Coordinate<int> c = cellPixels[i];
                        image.setPixel(c.X, c.Y, blackPixel);
                    }

                    for (int i = clusterSize + clusterStartPos; i < currentCellSize; i++) {
                        Coordinate<int> c = cellPixels[i];
                        image.setPixel(c.X, c.Y, whitePixel);
                    }

                    // start a new cell
                    currentCellPixel = 1;
                    darkestCellPixel = 1;
                    minCellIntensity = 255;
                    totalCellIntensity = 0;
                    if (ErrorFilterEnabled) {
                        //Console.Out.WriteLine("set error: {0}", error);
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
