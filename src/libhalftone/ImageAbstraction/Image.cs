using System;
using System.Collections.Generic;
using Gimp;

namespace Halftone
{
    public abstract class Image
    {
        // variants:
        // - image with iterator (using a iteration order)
        // - image with direct access

        // - scanning order
        //   - scanline
        //   - serpentine
        //   - SFC (Hilbert, Peano, ...)

        //Pixel this[Coords coords] { get; set; }

        public abstract int Width { get; }
        public abstract int Height { get; }

        // static Create

        //public delegate IEnumerable<Coordinate<int>> IterFuncScanning(int _width, int _height);
        public delegate Pixel IterFuncSrcDest(Pixel src);

        public abstract void IterateSrcDestNoOrder(
            IterFuncSrcDest pixelFunc);

        public abstract void IterateSrcDestDirect(
            IterFuncSrcDest pixelFunc,
            ScanningOrder scanOrder);
            //IterFuncScanning scanFunc);

        public abstract void IterateSrcDestByRows(
            IterFuncSrcDest pixelFunc,
            ScanningOrder scanOrder);
            //IterFuncScanning scanFunc);
     
        public abstract Pixel getPixel(int x, int y);
        public abstract void setPixel(int x, int y, Pixel pixel);

        public abstract void initBuffer();
        public abstract void flushBuffer();
    }
}
