using System;
using System.Collections.Generic;
using Gimp;

namespace HalftoneLab
{
    /// <summary>
    /// Serpentine scanning order. Scan in zig-zag lines.
    /// </summary>
    /// <remarks>
    /// It passes through the image in lines going ordeded in Y direction
    /// (0 -> height - 1). Odd lines go in X coordinate (0 -> width -1)
    /// direction, even line in the opposite one (width - 1 -> 0).
    /// </remarks>
    [Serializable]
    [Module(TypeName = "Serpentine scanning order")]
    public class SerpentineScanningOrder : ScanningOrder
    {
        [NonSerialized]
        private int _xStep; // movement: 1: --->, -1: <---

        public override IEnumerable<Coordinate<int>> getCoordsEnumerable() {
            // zig-zag order: odd lines go left to right, even lines right to left
            for (int y = 0; y < _height; y++) {
                for (int x = 0; x < _width; x++)
                    yield return new Coordinate<int>((y % 2 == 0) ? x : _width - 1 - x, y);
            }
        }

        public override void init(int width, int height) {
            _width = width;
            _height = height;
            CurrentX = CurrentY = 0;
            _xStep = 1;
        }

        public override void next() {
            CurrentX = (CurrentX + _xStep) % _width;
            if (CurrentX == (_width - 1)) {
                // end of even line
                CurrentY++;
                _xStep = -1;
            } else if (CurrentX == 0) {
                // end of odd line
                CurrentY++;
                _xStep = 1;
            }
        }

        public override bool hasNext() {
            // TODO: buggy
            return (CurrentX < _width) && (CurrentY < _height);
        }
    }
}
