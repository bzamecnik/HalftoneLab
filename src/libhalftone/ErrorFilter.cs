// ErrorFilter.cs created with MonoDevelop
// User: bohous at 15:30 26.3.2009
//

using System;

namespace Halftone
{
    // Error diffusion filter
    public abstract class ErrorFilter : Module
	{
        // get accumulated error value for given pixel
        public abstract double getError();
        
        // diffuse error value from given pixel to neighbor pixels
        public abstract void setError(double error);

        // for dynamic error filters
        // Note: this is not quite nice design
        public abstract void setError(double error, int intensity);

        // move to next pixel (according to the scanning order, ie. error buffer type)
        public abstract void moveNext();
	}

    public abstract class ErrorBuffer {
        public static ErrorBuffer createFromScanningOrder(ScanningOrder scanOrder,
            int bufferHeight,
            int bufferWidth) {
            // TODO: this is a bit ugly!

            if ((scanOrder is ScanlineScanningOrder) || (scanOrder is SFCScanningOrder)) {
                return new ScanlineErrorBuffer(bufferHeight, bufferWidth);
            //} else if (scanOrder is SerpentineScanningOrder) {
            //    return new SerpentineErrorBuffer(bufferHeight, bufferWidth);
            //} else if (scanOrder is SFCScanningOrder) {
            //    return new SFCErrorBuffer(bufferHeight, bufferWidth);
            } else {
                return null;
            }
        }

        public static ErrorBuffer createDefaultErrorBuffer() {
            return new ScanlineErrorBuffer();
        }

        public abstract double getError();
        public abstract double getError(int offsetY, int offsetX);
        public abstract void setError(int offsetY, int offsetX, double error);
        public abstract void moveNext();
    }

    public class ScanlineErrorBuffer : ErrorBuffer
    {
        double[,] _buffer;
        // group2 offset the error buffer
        protected int _currentOffsetX, _currentOffsetY;

        public ScanlineErrorBuffer(int height, int width)
        {
            _buffer = new double[height, width];
            _currentOffsetX = _currentOffsetY = 0;
        }

        public ScanlineErrorBuffer() : this(1, 1) {}

        public override double getError() {
            return _buffer[_currentOffsetY, _currentOffsetX];
        }

        public override double getError(int offsetY, int offsetX) {
            int x = _currentOffsetX + offsetX;
            int y = (_currentOffsetY + offsetY) % _buffer.GetLength(0);
            double error = 0;
            if ((x >= 0) && (x < _buffer.GetLength(1))) {
                error = _buffer[y, x];
            }
            return error;
        }

        public override void setError(int offsetY, int offsetX, double error) {
            int x = _currentOffsetX + offsetX;
            int y = (_currentOffsetY + offsetY) % _buffer.GetLength(0);
            if ((x >= 0) && (x < _buffer.GetLength(1))) {
                // discard error being set outside the buffer
                _buffer[y, x] += error;
            }
        }

        public override void moveNext() {
            // move to next pixel, cycle the buffer if necessary
            _currentOffsetX = (_currentOffsetX + 1) % _buffer.GetLength(1);
            if (_currentOffsetX == 0) {
                int lastLineY = _currentOffsetY;
                _currentOffsetY = (_currentOffsetY + 1) % _buffer.GetLength(0);
                // clear the last line
                for (int x = 0; x < _buffer.GetLength(1); x++) {
                    _buffer[lastLineY, x] = 0;
                }
            }
        }
    }
}
