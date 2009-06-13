using System.Text;

namespace Halftone
{
    public abstract class ErrorBuffer
    {
        public static ErrorBuffer createFromScanningOrder(
            ScanningOrder scanningOrder,
            int height,
            int width)
        {
            // TODO: This is not quite nice. How to make it better?

            if (scanningOrder is ScanlineScanningOrder) {
                return new ScanlineErrorBuffer(height, width);
            //} else if (scanOrder is SerpentineScanningOrder) {
            //    return new SerpentineErrorBuffer(_height, _width);
            } else if (scanningOrder is SFCScanningOrder) {
                return new LineErrorBuffer(width);
            } else {
                return null;
            }
        }

        public abstract double getError();
        public abstract void moveNext();
    }

    public abstract class MatrixErrorBuffer : ErrorBuffer
    {
        protected double[,] _buffer;
        // current offset of the error buffer
        protected int _currentOffsetX, _currentOffsetY;

        public int Height { get { return _buffer.GetLength(0); } }
        public int Width { get { return _buffer.GetLength(1); } }

        public MatrixErrorBuffer(int height, int width) {
            _buffer = new double[height, width];
            _currentOffsetX = _currentOffsetY = 0;
        }

        public void resize(int height, int width) {
            _buffer = new double[height, width];
        }

        public abstract void setError(int offsetY, int offsetX, double error);

        public static MatrixErrorBuffer createDefaultBuffer(int height, int width) {
            return new ScanlineErrorBuffer(height, width);
        }
    }

    public class ScanlineErrorBuffer : MatrixErrorBuffer
    {

        public ScanlineErrorBuffer(int height, int width) : base(height, width) { }

        public override double getError() {
            return _buffer[_currentOffsetY, _currentOffsetX];
        }

        //public override double getError(int offsetY, int offsetX) {
        //    int x = _currentOffsetX + offsetX;
        //    int y = (_currentOffsetY + offsetY) % _buffer.GetLength(0);
        //    double error = 0;
        //    if ((x >= 0) && (x < _buffer.GetLength(1))) {
        //        error = _buffer[y, x];
        //    }
        //    return error;
        //}

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

    // TODO:
    //public class SerpentineErrorBuffer : MatrixErrorBuffer { }

    // A simple cyclic buffer (to be used by ClusteringDitherAlgorithm with SFCScanningOrder)
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
