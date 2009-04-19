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
        // get accumulated error value for given pixel
        public abstract double getError();
        
        // diffuse error value from given pixel to neighbor pixels
        public abstract void setError(double error);

        // move to next pixel (according to the scanning order, ie. error buffer type)
        public abstract void moveNext();
	}

    public abstract class ErrorBuffer {
        //public static ErrorBuffer createFromScanningOrder(ScanningOrder scanOrder,
        //    int imageHeight,
        //    int imageWidth)
        //{
        //    // TODO: this is ugly!

        //    if (scanOrder is ScanlineScanningOrder) {
        //        return new ScanlineErrorBuffer(imageHeight, imageWidth);
        //    //} else if (scanOrder is SerpentineScanningOrder) {
        //    //    return new SerpentineErrorBuffer(imageHeight, imageWidth);
        //    //} else if (scanOrder is SFCScanningOrder) {
        //    //    return new SFCErrorBuffer(imageHeight, imageWidth);
        //    } else {
        //        return null;
        //    }
        //}

        public abstract double getError();
        public abstract double getError(Coordinate<int> offsetCoords);
        public abstract void setError(Coordinate<int> offsetCoords, double error);
        public abstract void moveNext();
    }

    public class ScanlineErrorBuffer : ErrorBuffer
    {
        double[,] _buffer;
        // current offset the error buffer
        protected Coordinate<int> _currentCoords;

        public ScanlineErrorBuffer(int height, int width)
        {
            _buffer = new double[height, width];
            _currentCoords = new Coordinate<int>(0, 0);
        }

        public override double getError() {
            return _buffer[_currentCoords.Y, _currentCoords.X];
        }

        public override double getError(Coordinate<int> offsetCoords) {
            int x = _currentCoords.X + offsetCoords.X;
            int y = (_currentCoords.Y + offsetCoords.Y) % _buffer.GetLength(0);

            if ((x >= 0) && (x < _buffer.GetLength(1))) {
                double error = _buffer[y, x];
                //if (loop > 0) {
                //    Console.WriteLine("ScanlineErrorBuffer.getError(): y: {0}, x: {1}, error: {2}",
                //        y, x, error);
                //}
                return error;
            } else {
                return 0;
            }
        }

        public override void setError(Coordinate<int> offsetCoords, double error) {
            int x = _currentCoords.X + offsetCoords.X;
            int y = (_currentCoords.Y + offsetCoords.Y) % _buffer.GetLength(0);

            //if (loop > 0) {
            //    Console.WriteLine("ScanlineErrorBuffer.setError(), y: {0}, x: {1}, error: {2}, _buffer: [{3},{4}]",
            //        y, x, error, _buffer.GetLength(0), _buffer.GetLength(1));
            //}
            if ((x >= 0) && (x < _buffer.GetLength(1))) {
                _buffer[y, x] += error;
                //if (loop > 0) {
                //    Console.WriteLine("buffer[{0}, {1}] = {2}", y, x, _buffer[y, x]);
                //}
            }
            //if (loop > 0) {
            //    loop--;
            //}
        }

        public override void moveNext() {
            // move to next pixel, cycle the buffer if necessary
            _currentCoords.X = (_currentCoords.X + 1) % _buffer.GetLength(1);
            if (_currentCoords.X == 0) {
                int lastLineY = _currentCoords.Y;
                _currentCoords.Y = (_currentCoords.Y + 1) % _buffer.GetLength(0);
                
                //Console.WriteLine("last line of the error buffer:");
                //for (int x = 0; x < _buffer.GetLength(1); x++) {
                //    Console.Write("{0}, ", _buffer[_currentCoords.Y, x]);
                //}
                //Console.WriteLine();

                // clear the last line
                for (int x = 0; x < _buffer.GetLength(1); x++) {
                    _buffer[lastLineY, x] = 0;
                }
            }
        }
    }
}
