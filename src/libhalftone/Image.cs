using System;
using System.Collections.Generic;
using System.Text;

namespace Halftone
{
    public interface Image
    {
        // variants:
        // - image with iterator (using a iteration order)
        // - image with direct access

        // - scanning order
        //   - scanline
        //   - serpentine
        //   - SFC (Hilbert, Peano, ...)

        Pixel this[Coords coords] { get; set; }

        int Width { get; }
        int Height { get; }

        // static Create
    }

    public class Coords {
        int _x, _y;

        public int X
        {
            get { return _x; }
            set { _x = value; }
        }

        public int Y
        {
            get { return _y; }
            set { _y = value; }
        }

        public Coords(int x, int y) {
            _x = x;
            _y = y;
        }
    }
}
