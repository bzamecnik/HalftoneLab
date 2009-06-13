using System;
using System.Collections.Generic;
using Gimp;

namespace Halftone
{
    [Serializable]
    public abstract class ScanningOrder : Module
    {
        public int CurrentX { get; protected set; } // (from 0)
        public int CurrentY { get; protected set; } // (from 0) 
        //[NonSerialized]
        //private Coordinate<int> _currentCoords;
        //public Coordinate<int> CurrentCoords {
        //    get {
        //        if (_currentCoords == null) {
        //            _currentCoords = new Coordinate<int>()
        //            {
        //                X = CurrentX,
        //                Y = CurrentY
        //            };
        //        }
        //        return _currentCoords;
        //    }
        //    protected set {
        //        _currentCoords = value;
        //    }
        //}
        [NonSerialized]
        protected int _width; // image dimensions
        [NonSerialized]
        protected int _height;

        public abstract void init(int width, int height);
        public abstract void next();
        public abstract bool hasNext();
        public abstract IEnumerable<Coordinate<int>> getCoordsEnumerable();
    }

    [Serializable]
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
            // TODO: buggy
            return ((CurrentX + 1) < _width) && ((CurrentY + 1) < _height);
        }
    }

    [Serializable]
    public class SerpentineScanningOrder : ScanningOrder
    {
        [NonSerialized]
        private int _xDiff; // movement: 1: --->, -1: <---

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
            _xDiff = 1;
        }

        public override void next() {
            CurrentX = (CurrentX + _xDiff) % _width;
            if (CurrentX == (_width - 1)) {
                // end of even line
                CurrentY++;
                _xDiff = -1;
            } else if (CurrentX == 0) {
                // end of odd line
                CurrentY++;
                _xDiff = 1;
            }
        }

        public override bool hasNext() {
            // TODO: buggy
            return (CurrentX < _width) && (CurrentY < _height);
        }
    }

    [Serializable]
    public abstract class SFCScanningOrder : ScanningOrder
    {
    }

    [Serializable]
    public class HilbertScanningOrder : SFCScanningOrder {
        // - Algorithm 781 - generating Hilbert's space-filling curve by recursion.pdf
        //   - by GREG BREINHOLT and CHRISTOPH SCHIERZ
        //   - Swiss Federal Institute of Technology

        [NonSerialized]
        private int _size; // _size of a square side (2^N)
        [NonSerialized]
        private IEnumerator<Coordinate<int>> _imageEnumerator;
        [NonSerialized]
        private IEnumerable<Coordinate<int>> _squareEnumerable;

        public override IEnumerable<Coordinate<int>> getCoordsEnumerable() {
            int pixelsRemaining = _width * _height;
            foreach (Coordinate<int> coords in _squareEnumerable) {
                // skip all coordinates outside the image, yet inside the square
                // TODO: this optimization could be done inside hilbert()
                if ((pixelsRemaining > 0) &&(coords.X < _width) && (coords.Y < _height)) {
                    pixelsRemaining--;
                    yield return coords;
                } else {
                    continue;
                }
            }
            yield break;
        }

        public override void init(int width, int height) {
            _width = width;
            _height = height;
            // find the smallest square that can contain the image
            _size = 1;
            while ((_size < width) || (_size < height)) {
                _size *= 2; // or: _size += _size;
            }
            //Console.Out.WriteLine("hilbert _size: {0}x{0}", _size);
            // TODO: the shape orientation could be controlled from outside
            // Note: an optimization of unit hilbert shape orientation
            // according to image orientation
            _squareEnumerable = hilbert(0, 0, _size, 0, (width > height) ? 1 : 0);
            _imageEnumerator = getCoordsEnumerable().GetEnumerator();
        }

        public override void next() {
            if (_imageEnumerator != null) {
                if (_imageEnumerator.MoveNext()) {
                    CurrentX = _imageEnumerator.Current.X;
                    CurrentY = _imageEnumerator.Current.Y;
                } else {
                    _imageEnumerator = null;
                }
            }
        }

        public override bool hasNext() {
            return (_imageEnumerator != null);
        }

        // x, y - bottom-left corner of the block
        // _size - must be power of two (2^N)
        // i1, i2 (0-1) - orientation of the shape
        static IEnumerable<Coordinate<int>> hilbert(int x, int y, int size, int i1, int i2) {
            size /= 2; // could be: _size >>= 1;

            int angle = 0;
            int angleOrientation;

            if (i1 == 1) {
                x += size; // top-right corner is the starting point
                y += size;
                angle = 2;
            }

            if (i1 == i2) {
                angle++;
                angleOrientation = 3; // counter-clockwise
            } else {
                angleOrientation = 1; // clockwise
            }

            if (size > 1) {
                // recurse down to four quadrants
                int _i1, _i2;
                angleToCoding(angle + angleOrientation, out _i1, out _i2);
                foreach (Coordinate<int> coords in hilbert(x, y, size, _i1, _i2)) {
                    yield return coords;
                }

                step(ref x, ref y, angle, size);
                angleToCoding(angle, out _i1, out _i2);
                foreach (Coordinate<int> coords in hilbert(x, y, size, _i1, _i2)) {
                    yield return coords;
                }

                angle += angleOrientation;
                step(ref x, ref y, angle, size);
                angleToCoding(angle - angleOrientation, out _i1, out _i2);
                foreach (Coordinate<int> coords in hilbert(x, y, size, _i1, _i2)) {
                    yield return coords;
                }

                angle += angleOrientation;
                step(ref x, ref y, angle, size);
                angleToCoding(angle + angleOrientation, out _i1, out _i2);
                foreach (Coordinate<int> coords in hilbert(x, y, size, _i1, _i2)) {
                    yield return coords;
                }

            } else if (size == 1) {
                // unit shape - plot four points according to the orientation
                for (int i = 0; i < 3; i++) {
                    //plot(x, y);
                    yield return new Coordinate<int>(x, y);
                    step(ref x, ref y, angle, size);
                    angle += angleOrientation;
                }
                //plot(x, y);
                yield return new Coordinate<int>(x, y);
            }
            // else {
            //    // error
            //}
        }

        private static void step(ref int x, ref int y, int angle, int stepSize) {
            // angle in [0, 3]
            angle %= 4;
            switch (angle) {
                case 0: x += stepSize; break; // right
                case 1: y += stepSize; break; // up
                case 2: x -= stepSize; break; // left
                case 3: y -= stepSize; break; // down
            }
        }

        private static void angleToCoding(int angle, out int i1, out int i2) {
            angle %= 4;
            i1 = (angle > 1) ? 1 : 0;
            i2 = ((angle == 0) || (angle == 3)) ? 1 : 0;
        }
    }
}
