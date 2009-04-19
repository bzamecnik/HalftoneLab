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

        public delegate IEnumerable<Coordinate<int>> IterFuncScanning(int width, int height);
        public delegate Pixel IterFuncSrcDest(Pixel src);

        public abstract void IterateSrcDestNoOrder(
            IterFuncSrcDest pixelFunc);

        public abstract void IterateSrcDestDirect(
            IterFuncSrcDest pixelFunc,
            IterFuncScanning scanFunc);

        public abstract void IterateSrcDestByRows(
            IterFuncSrcDest pixelFunc,
            IterFuncScanning scanFunc);
    }
}
