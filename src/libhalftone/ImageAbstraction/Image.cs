using System;
using System.Collections.Generic;
using Gimp;

namespace Halftone
{
    /// <summary>
    /// Image represents an abstract 2-D grid of pixels which can be
    /// manipulated. Pixels could be accessed directly or processed using
    /// a scanning order.
    /// </summary>
    /// <remarks>
    /// Image can use an internal buffer to speed up the operations.
    /// </remarks>
    /// <see cref="ScanningOrder"/>
    public abstract class Image
    {
        //Pixel this[Coords coords] { get; set; }

        /// <summary>
        /// Image width.
        /// </summary>
        public abstract int Width { get; }

        /// <summary>
        /// Image height.
        /// </summary>
        public abstract int Height { get; }

        /// <summary>
        /// Create a new image using a default impmlementation.
        /// </summary>
        /// <param name="width">Image width</param>
        /// <param name="height">Image height</param>
        /// <returns>Crated image</returns>
        public static Image createDefatult(int width, int height) {
            return new GSImage(width, height);
        }

        /// <summary>
        /// Function to be applied on a pixel intended to modify it.
        /// </summary>
        /// <param name="src">Input pixel</param>
        /// <returns>Output pixel</returns>
        public delegate Pixel IterFuncSrcDest(Pixel src);

        /// <summary>
        /// Iterate over all image pixels with no explicit order and modify
        /// each pixel with given function.
        /// </summary>
        /// <param name="pixelFunc">Function to modify pixels</param>
        //public abstract void IterateSrcDestNoOrder(
        //    IterFuncSrcDest pixelFunc);

        /// <summary>
        /// Iterate over all image pixels using given scanning order
        /// and modify each pixel with given function. Use direct access
        /// to image pixels.
        /// </summary>
        /// <param name="pixelFunc">Function to modify pixels</param>
        /// <param name="scanOrder">Scanning order</param>
        public abstract void IterateSrcDestDirect(
            IterFuncSrcDest pixelFunc,
            ScanningOrder scanOrder);

        /// <summary>
        /// Iterate over all image pixels using given scanning order
        /// and modify each pixel with given function. Access pixels in
        /// blocks (rows).
        /// </summary>
        /// <param name="pixelFunc">Function to modify pixels</param>
        /// <param name="scanOrder">Scanning order</param>
        public abstract void IterateSrcDestByRows(
            IterFuncSrcDest pixelFunc,
            ScanningOrder scanOrder);
     
        // TODO:
        // - differentiate getPixel()/setPixel() with direct access
        //   and buffered access!
        //   - when there is any buffer use it, otherwise
        //     access the underlying image directly

        /// <summary>
        /// Get pixel from given coordinates.
        /// </summary>
        /// <param name="x">X coordinate</param>
        /// <param name="y">Y coordinate</param>
        /// <returns>Pixel on that coordinates</returns>
        public abstract Pixel getPixel(int x, int y);

        /// <summary>
        /// Set pixel on given coordinates.
        /// </summary>
        /// <param name="x">X coordinate</param>
        /// <param name="y">Y coordinate</param>
        /// <param name="pixel">New pixel value</param>
        public abstract void setPixel(int x, int y, Pixel pixel);

        /// <summary>
        /// Initialize the internal buffer.
        /// </summary>
        public abstract void initBuffer();

        /// <summary>
        /// Flush the internal buffer.
        /// </summary>
        public abstract void flushBuffer();

        /// <summary>
        /// Run-time information about the image and its processing.
        /// </summary>
        public class ImageRunInfo
        {
            /// <summary>
            /// Current scanning order.
            /// </summary>
            public ScanningOrder ScanOrder { get; set; }

            /// <summary>
            /// Image height.
            /// </summary>
            public int Height { get; set; }

            /// <summary>
            /// Image width.
            /// </summary>
            public int Width { get; set; }
        }
    }
}
