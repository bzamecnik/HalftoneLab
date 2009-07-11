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
                setMatrix(value, true);
            }
        }

        // error buffer
        [NonSerialized]
        private MatrixErrorBuffer _buffer;

        public MatrixErrorBuffer Buffer {
            get { return _buffer; }
            protected set { _buffer = value; }
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

        protected void setMatrix(ErrorMatrix matrix, bool resizeBuffer) {
            if (matrix != null) {
                // Resize the buffer if a matrix with different _height is set
                // Note: buffer _width depends on image _size
                if (resizeBuffer && (Buffer != null) &&
                    (matrix.Height != _matrix.Height)) {
                    Buffer.resize(_matrix.Height, Buffer.Width);
                }
                _matrix = matrix;
            }
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
