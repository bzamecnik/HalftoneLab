using System;
using System.Collections.Generic;
using Gimp;

namespace Halftone
{
    /// <summary>
    /// Scanning order iterates over image pixels giving their coordinates.
    /// </summary>
    /// <remarks>
    /// <para>
    /// It passes through all pixel exactly once.
    /// </para>
    /// <para>
    /// Scanning order needs to be initialized before being used.
    /// </para>
    /// <para>
    /// There are two interfaces for iterating over image pixels.
    /// You can enumerate coordinates using getCoordsEnumerable().
    /// It is easy to use and simple to implement, but you can't query on
    /// next pixel existence. The other is a combination of hasNext(),
    /// next(), CurrentX and CurrentY.
    /// </para>
    /// </remarks>
    /// <example>Example of using getCoordsEnumerable() interface and
    /// a foreach loop:
    /// <code>
    /// ScanningOrder scanOrder = new ScanlineScaningOrder();
    /// scanOrder.init(width, height);
    /// foreach (Coordinate<int> coords in scanOrder.getCoordsEnumerable()) {
    ///     // use coords.X, coords.Y somehow
    /// }
    /// </code>
    /// </example>
    /// <example>Example of using getCoordsEnumerable() interface and
    /// an enumerator:
    /// <code>
    /// ScanningOrder scanOrder = new ScanlineScaningOrder();
    /// scanOrder.init(width, height);
    /// IEnumerator<Coordinate<int>> coordsEnum =
    ///     scanOrder.getCoordsEnumerable().GetEnumerator();
    /// for (... other loop ...) {
    ///     if (coordsEnum.MoveNext()) {
    ///         // use coordsEnum.Current.X, coordsEnum.Current.Y somehow
    ///     } else {
    ///         break;
    ///     }
    /// }
    /// </code>
    /// </example>
    /// <example>Example of using hasNext(), next() interface:
    /// <code>
    /// ScanningOrder scanOrder = new ScanlineScaningOrder();
    /// scanOrder.init(width, height);
    /// while (scanOrder.hasNext()) {
    ///     scanOrder.next();
    ///     // use scanOrder.CurrentX, scanOrder.CurrentY somehow
    /// }
    /// </code>
    /// </example>
    [Serializable]
    public abstract class ScanningOrder : Module
    {
        /// <summary>
        /// Current X coordinate (numbered from 0).
        /// </summary>
        public int CurrentX { get; protected set; }
        /// <summary>
        /// Current X coordinate (numbered from 0).
        /// </summary>
        public int CurrentY { get; protected set; }

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

        public override void init(Image.ImageRunInfo imageRunInfo) {
            base.init(imageRunInfo);
            init(imageRunInfo.Width, imageRunInfo.Height);
        }

        /// <summary>
        /// Initialize the scanning order with image dimensions.
        /// </summary>
        /// <param name="width">Image width</param>
        /// <param name="height">Image height</param>
        public abstract void init(int width, int height);

        /// <summary>
        /// Move to next coordinates.
        /// </summary>
        public abstract void next();
        
        /// <summary>
        /// Is there any pixel not yet visited?
        /// </summary>
        /// <returns>True if there are still pixel to visit</returns>
        public abstract bool hasNext();
        
        /// <summary>
        /// Coordinates iterator.
        /// </summary>
        /// <remarks>
        
        /// </remarks>
        /// <returns></returns>
        public abstract IEnumerable<Coordinate<int>> getCoordsEnumerable();
    }

    /// <summary>
    /// Scanline scanning order. Scan in natural order, line by line.
    /// </summary>
    /// <remarks>
    /// It passes through the image in lines going in X coordinate
    /// (0 -> width -1) direction ordeded in Y direction (0 -> height - 1).
    /// </remarks>
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

    /// <summary>
    /// Serpentine scanning order. Scan in zig-zag lines.
    /// </summary>
    /// <remarks>
    /// It passes through the image in lines going ordeded in Y direction
    /// (0 -> height - 1). Odd lines go in X coordinate (0 -> width -1)
    /// direction, even line in the opposite one (width - 1 -> 0).
    /// </remarks>
    [Serializable]
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

    /// <summary>
    /// Scanning order traversing a space-filling curve.
    /// </summary>
    [Serializable]
    public abstract class SFCScanningOrder : ScanningOrder
    {
    }

    /// <summary>
    /// Scanning order traversing Hilbert space-filling curve.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The algorithm used for generating Hilbert curve is taken from paper
    /// Greg Breinholt & Christoph Schierz:
    ///   Generating Hilbert's space-filling curve by recursion
    /// (Swiss Federal Institute of Technology)
    /// </para>
    /// <para>
    /// As Hilbert curve is defined only for squares of 2^N side we find
    /// the smallest such square containing the image rectangle and skip
    /// all the coordinates outside the image.
    /// </para>
    /// </remarks>
    [Serializable]
    public class HilbertScanningOrder : SFCScanningOrder {
        

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
