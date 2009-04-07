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

        //int Width { get; }
        //int Height { get; }

        // static Create

        public delegate IEnumerable<Coordinate<int>> IterFuncScanning(int width, int height);
        public delegate Pixel IterFuncSrcDest(Pixel src);

        public abstract void IterateSrcDest(
            IterFuncSrcDest pixelFunc,
            IterFuncScanning scanFunc);
    }
}
