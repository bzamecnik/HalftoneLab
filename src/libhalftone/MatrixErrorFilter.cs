using System;
using Gimp;

namespace Halftone
{
    public class MatrixErrorFilter : ErrorFilter
    {
        // matrix of error filter weights
        ErrorMatrix _matrix;

        // NOTE: to be serialized
        public ErrorMatrix ErrorMatrix {
            get { return _matrix; }
            protected set {
                // Resize the buffer if a matrix with different height is set
                // Note: buffer width depends on image size
                if ((Buffer != null) && (value.Height != _matrix.Height)) {
                    Buffer.resize(_matrix.Height, Buffer.Width);
                }
                _matrix = value;
            }
        }

        // error buffer
        MatrixErrorBuffer _buffer;

        public MatrixErrorBuffer Buffer {
            get { return _buffer; }
            private set { _buffer = value; }
        }
        
        public MatrixErrorFilter(ErrorMatrix matrix, MatrixErrorBuffer buffer) {
            ErrorMatrix = matrix;
            Buffer = buffer;
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

        public override void initBuffer(
            ScanningOrder scanningOrder,
            int imageHeight,
            int imageWidth)
        {
            // null if the created result is not a MatrixErrorBuffer
            Buffer = ErrorBuffer.createFromScanningOrder(scanningOrder,
                ErrorMatrix.Height, imageWidth) as MatrixErrorBuffer;
        }
    }
}
