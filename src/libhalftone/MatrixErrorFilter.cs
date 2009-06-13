using System;

namespace Halftone
{
    [Serializable]
    public class MatrixErrorFilter : ErrorFilter
    {
        // matrix of error filter weights
        ErrorMatrix _matrix;

        public ErrorMatrix ErrorMatrix {
            get { return _matrix; }
            protected set {
                // Resize the buffer if a matrix with different _height is set
                // Note: buffer _width depends on image _size
                if ((Buffer != null) && (value.Height != _matrix.Height)) {
                    Buffer.resize(_matrix.Height, Buffer.Width);
                }
                _matrix = value;
            }
        }

        // error buffer
        [NonSerialized]
        MatrixErrorBuffer _buffer;

        public MatrixErrorBuffer Buffer {
            get { return _buffer; }
            private set { _buffer = value; }
        }
        
        public MatrixErrorFilter(ErrorMatrix matrix) {
            ErrorMatrix = matrix;
        }

        public MatrixErrorFilter() {
            _matrix = ErrorMatrix.Samples.Default;
        }

        public override double getError() {
            return Buffer.getError();
        }

        // diffuse error value from given pixel to neighbor pixels
        public override void setError(double error) {
            ErrorMatrix.apply(
                (int y, int x, double coeff) => { Buffer.setError(y, x, coeff * error); }
                );
        }

        public override void moveNext() {
            Buffer.moveNext();
        }

        public override bool initBuffer(
            ScanningOrder scanningOrder,
            int imageHeight,
            int imageWidth)
        {
            // null if the created result is not a MatrixErrorBuffer
            Buffer = ErrorBuffer.createFromScanningOrder(scanningOrder,
                ErrorMatrix.Height, imageWidth) as MatrixErrorBuffer;
            return Buffer != null;
        }
    }
}
