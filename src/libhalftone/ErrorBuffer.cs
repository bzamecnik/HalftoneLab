using System.Text;

namespace Halftone
{
    public abstract class ErrorBuffer
    {
        public static ErrorBuffer createFromScanningOrder(
            ScanningOrder scanningOrder,
            int height,
            int width) {
            // TODO: This is not quite nice. How to make it better?

            if (scanningOrder is ScanlineScanningOrder) {
                return new MatrixErrorBuffer(height, width);
            } else if (scanningOrder is SerpentineScanningOrder) {
                return new MatrixErrorBuffer(height, width) { SerpentineEnabled = true };
            } else if (scanningOrder is SFCScanningOrder) {
                return new LineErrorBuffer(width);
            } else {
                return null;
            }
        }

        public abstract double getError();
        public abstract void moveNext();
    }

    public class MatrixErrorBuffer : ErrorBuffer
    {
        // Note:
        // There are currently two modes:
        //   * scanline row scanning (default)
        //   * serpentine row scanning (SerpentineEnabled == true)
        // For now it seems that scanline and serpentine scanning is enough
        // and the need for different behavior is very unlikely.
        // The code is more compact and readable this way.
        // To support different behavior, make this class abstract and split
        // the code into several descentant classes.

        protected double[,] _buffer;
        // current offset of the error buffer
        protected int _currentOffsetX, _currentOffsetY;

        public bool SerpentineEnabled { get; set; }
        // x step
        // for scanline order: false
        // for serpentine order: true, false
        private bool _backMovement;

        public int Height { get; protected set; }
        public int Width { get; protected set; }

        public MatrixErrorBuffer(int height, int width) {
            SerpentineEnabled = false;
            init(height, width);
        }

        public void resize(int height, int width) {
            init(height, width);
        }

        protected virtual void init(int height, int width) {
            _buffer = new double[height, width];
            Height = _buffer.GetLength(0);
            Width = _buffer.GetLength(1);
            _currentOffsetX = _currentOffsetY = 0;
            _backMovement = false;
        }

        public override double getError() {
            return _buffer[_currentOffsetY, _currentOffsetX];
        }

        public void setError(int offsetY, int offsetX, double error) {
            bool oddLine = _backMovement;
            oddLine = (SerpentineEnabled && ((offsetY % 2) == 1)) ? !oddLine : oddLine;
            int x = _currentOffsetX + (oddLine ? -offsetX : offsetX);
            int y = (_currentOffsetY + offsetY) % Height;
            if ((x >= 0) && (x < Width)) {
                // discard error being set outside the buffer
                _buffer[y, x] += error;
            }
        }

        public override void moveNext() {
            // move to next pixel, cycle the buffer if necessary
            bool endOfRow = (_backMovement  && (_currentOffsetX == 0))
                         || (!_backMovement && (_currentOffsetX == (Width - 1)));
            if (endOfRow) {
                // the end of a row was reached, move to the next row
                int lastRowY = _currentOffsetY;
                _currentOffsetY = (_currentOffsetY + 1) % Height;
                // clear the last line
                for (int x = 0; x < Width; x++) {
                    _buffer[lastRowY, x] = 0;
                }
            }
            if (!(SerpentineEnabled && endOfRow)) {
                _currentOffsetX = (_currentOffsetX + (_backMovement ? Width - 1 : 1))
                    % Width;
            } else {
                // change the row scanning direction
                _backMovement = !_backMovement;
            }
        }
    }

    // A simple cyclic buffer (to be used by CellHalftoneAlgorithm with SFCScanningOrder)
    // The reason not to use MatrixErrorBuffer is different signature of constructor,
    // and resize(), setError().
    public class LineErrorBuffer : ErrorBuffer
    {
        private double[] _buffer;
        private int _currentOffset = 0;

        public int Length {
            get {
                return (_buffer != null) ? _buffer.Length : 0;
            }
        }

        public LineErrorBuffer(int size) {
            _buffer = new double[size + 1];
        }

        public void resize(int size) {
            _buffer = new double[size + 1];
        }

        public override double getError() {
            return _buffer[_currentOffset];
        }

        public void setError(int offset, double error) {
            int pos = (_currentOffset + offset) % _buffer.Length;
            if ((pos >= 0) && (pos < _buffer.Length)) {
                // discard error being set outside the buffer
                _buffer[pos] += error;
            }
        }

        public override void moveNext() {
            // move to next pixel, cycle the buffer if necessary
            _buffer[_currentOffset] = 0;
            _currentOffset = (_currentOffset + 1) % _buffer.Length;
        }

        public override string ToString() {
            StringBuilder sb = new StringBuilder();
            sb.Append("buffer [");
            foreach (double item in _buffer) {
                sb.AppendFormat("{0}, ", item);
            }
            sb.Append("]");
            return sb.ToString();
        }
    }
}
