using System;

namespace Halftone
{
    [Serializable]
    public class VectorErrorFilter : ErrorFilter
    {
        ErrorMatrix _matrix; // must be of unit _height
        public ErrorMatrix ErrorMatrix {
            get { return _matrix; }
            protected set {
                if (value.Height != 1) {
                    return;
                }
                // Resize the buffer if a matrix with different _width is set
                if ((Buffer != null) && (value.Width != _matrix.Width)) {
                    Buffer.resize(_matrix.Width);
                }
                _matrix = value;
            }
        }

        [NonSerialized]
        LineErrorBuffer _buffer;
        public LineErrorBuffer Buffer {
            get { return _buffer; }
            private set { _buffer = value; }
        }

        public VectorErrorFilter(ErrorMatrix matrix) {
            ErrorMatrix = matrix;
        }

        public VectorErrorFilter() {
            _matrix = ErrorMatrix.Samples.nextPixel;
        }

        public override double getError() {
            return Buffer.getError();
        }

        // diffuse error value from given pixel to neighbor pixels
        public override void setError(double error) {
            ErrorMatrix.apply(
                (int y, int x, double coeff) => { Buffer.setError(x, coeff * error); }
                );
        }

        public override void moveNext() {
            Buffer.moveNext();
        }

        public override bool initBuffer(
            ScanningOrder scanningOrder,
            int imageHeight,
            int imageWidth) {
            // null if the created result is not a MatrixErrorBuffer
            Buffer = ErrorBuffer.createFromScanningOrder(scanningOrder,
                ErrorMatrix.Height, ErrorMatrix.Width) as LineErrorBuffer;
            return Buffer != null;
        }
    }
}
