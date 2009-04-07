// ErrorFilter.cs created with MonoDevelop
// User: bohous at 15:30 26.3.2009
//

using System;
using Gimp;

namespace Halftone
{
    // Error diffusion filter
	public abstract class ErrorFilter
	{
        //ScanningOrder _scanOrder;
        //public ErrorFilter(ScanningOrder scanOrder) {
        //    _scanOrder = scanOrder;
        //}

        // get accumulated error value for given pixel
        public abstract double getError();
        
        // diffuse error value from given pixel to neighbor pixels
        public abstract void setError(double error);

        // move to next pixel according to the scanning order
        public abstract void moveNext();
	}

    public abstract class ErrorBuffer {
        protected Coordinate<int> _currentCoords;

        protected ErrorBuffer(int width, int height, Coordinate<int> coords) {
            _currentCoords = coords;
        }

        public static ErrorBuffer createFromScanningOrder(ScanningOrder scanOrder,
            int width,
            int height,
            Coordinate<int> coords)
        {
            // TODO: this is ugly!

            if (scanOrder is ScanlineScanningOrder) {
                return new ScanlineErrorBuffer(width, height, coords);
            //} else if (scanOrder is SerpentineScanningOrder) {
            //    return new SerpentineErrorBuffer(width, height, coords);
            //} else if (scanOrder is SFCScanningOrder) {
            //    return new SFCErrorBuffer(width, height, coords);
            } else {
                return null;
            }
        }

        public abstract double getError();
        public abstract double getError(Coordinate<int> offsetCoords);
        public abstract void setError(Coordinate<int> offsetCoords, double error);
        public abstract void moveNext();
    }

    class ScanlineErrorBuffer : ErrorBuffer
    {
        double[,] _buffer;

        public ScanlineErrorBuffer(int width, int height, Coordinate<int> coords)
            : base(width, height, coords)
        {
            _buffer = new double[height, width];
        }

        public override double getError() {
            return _buffer[_currentCoords.Y, _currentCoords.X];
        }

        public override double getError(Coordinate<int> offsetCoords) {
            return _buffer[_currentCoords.Y + offsetCoords.Y,
                _currentCoords.X + offsetCoords.X];
        }

        public override void setError(Coordinate<int> offsetCoords, double error) {
            _buffer[_currentCoords.Y + offsetCoords.Y,
                _currentCoords.X + offsetCoords.X] = error;
        }

        public override void moveNext() {
            // move to next pixel, cycle the buffer if necessary
            _currentCoords.X = (_currentCoords.X + 1) % _buffer.GetLength(1);
            if (_currentCoords.X == 0) {
                int lastLineY = _currentCoords.Y;
                _currentCoords.Y = (_currentCoords.Y + 1) % _buffer.GetLength(0);
                // clear the last line
                for (int x = 0; x < _buffer.GetLength(1); x++) {
                    _buffer[lastLineY, x] = 0;
                }
            }
        }
    }
}
