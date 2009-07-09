using System;

namespace Halftone
{
    /// <summary>
    /// Matrix error-diffusion filter.
    /// </summary>
    /// <see cref="ErrorMatrix"/>
    [Serializable]
    public class MatrixErrorFilter : ErrorFilter
    {
        // matrix of error filter weights
        private ErrorMatrix _matrix;

        public ErrorMatrix Matrix {
            get { return _matrix; }
            set {
                if (value != null) {
                    // Resize the buffer if a matrix with different _height is set
                    // Note: buffer _width depends on image _size
                    if ((Buffer != null) && (value.Height != _matrix.Height)) {
                        Buffer.resize(_matrix.Height, Buffer.Width);
                    }
                    _matrix = value;
                }
            }
        }

        // error buffer
        [NonSerialized]
        private MatrixErrorBuffer _buffer;

        public MatrixErrorBuffer Buffer {
            get { return _buffer; }
            private set { _buffer = value; }
        }
        
        public MatrixErrorFilter(ErrorMatrix matrix) {
            Matrix = matrix;
        }

        public MatrixErrorFilter() {
            _matrix = ErrorMatrix.Samples.Default;
        }

        public override double getError() {
            return Buffer.getError();
        }

        // diffuse error value from given pixel to neighbor pixels
        public override void setError(double error) {
            Matrix.apply((int y, int x, double coeff) =>
                {
                    Buffer.setError(y, x, coeff * error);
                }
            );
        }

        public override void moveNext() {
            Buffer.moveNext();
        }

        public override void init(Image.ImageRunInfo imageRunInfo) {
            base.init(imageRunInfo);
            Matrix.init(imageRunInfo);
            Buffer = ErrorBuffer.createFromScanningOrder(
                imageRunInfo.ScanOrder, Matrix.Height,
                imageRunInfo.Width) as MatrixErrorBuffer;
            // null if the created result is not a MatrixErrorBuffer
            Initialized = Buffer != null;
        }
    }
}
