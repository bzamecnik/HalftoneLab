// ErrorFilter.cs created with MonoDevelop
// User: bohous at 15:30 26.3.2009
//

using System;

namespace Halftone
{
	public abstract class ErrorFilter
	{
        //public abstract Pixel getNextError();
        //public abstract void setNextError(Pixel pixel);

        // get accumulated error value for given pixel
        public abstract Pixel getError(Coords coords);
        
        // diffuse error value from given pixel to neighbor pixels
        public abstract void setError(Coords coords, Pixel pixel);
	}

    abstract class ErrorBuffer {
        protected Coords _currentCoords;

        public ErrorBuffer(int width, int height, Coords coords) {
            _currentCoords = coords;
        }
        public abstract Pixel getError();
        public abstract void setError(Coords offsetCoords, Pixel error);
        public abstract void moveNext();
    }

    class ScanlineErrorBuffer : ErrorBuffer
    {
        Pixel[,] _buffer;

        public ScanlineErrorBuffer(int width, int height, Coords coords)
            : base(width, height, coords)
        {
            _buffer = new Pixel[height, width];
        }

        public override Pixel getError() {
            return _buffer[0, _currentCoords.X];
        }
        public override void setError(Coords offsetCoords, Pixel error) {
            _buffer[offsetCoords.Y, _currentCoords.X + offsetCoords.Y] = error;
        }

        public override void moveNext() {
            // cycle the buffer
            // - clear current line, make it the last
        }
    }
}
