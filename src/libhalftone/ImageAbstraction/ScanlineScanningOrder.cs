// Copyright (c) 2009 Bohumir Zamecnik <bohumir@zamecnik.org>
// License: The MIT License, see the LICENSE file

using System;
using System.Collections.Generic;
using Gimp;

namespace HalftoneLab
{

    /// <summary>
    /// Scanline scanning order. Scan in natural order, line by line.
    /// </summary>
    /// <remarks>
    /// It passes through the image in lines going in X coordinate
    /// (0 -> width -1) direction ordeded in Y direction (0 -> height - 1).
    /// </remarks>
    [Serializable]
    [Module(TypeName = "Scanline scanning order")]
    public class ScanlineScanningOrder : ScanningOrder
    {
        public override IEnumerable<Coordinate<int>> getCoordsEnumerable() {
            for (int y = 0; y < _height; y++)
                for (int x = 0; x < _width; x++)
                    yield return new Coordinate<int>(x, y);
        }

        public override void init(int width, int height) {
            _width = width;
            _height = height;
            CurrentX = CurrentY = -1;
        }

        public override void next() {
            CurrentX = (CurrentX + 1) % _width;
            if (CurrentX == 0) {
                CurrentY++;
            }
        }

        public override bool hasNext() {
            // TODO: fix this
            return ((CurrentX + 1) < _width) && ((CurrentY + 1) < _height);
        }
    }
}
