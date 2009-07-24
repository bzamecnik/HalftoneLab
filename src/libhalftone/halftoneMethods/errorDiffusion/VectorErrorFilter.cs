using System;

namespace HalftoneLab
{
    /// <summary>
    /// Error-diffusion filter with a one-dimensional error matrix and buffer. This can
    /// be useful eg. for image scanning order based on a space-filling curve.
    /// </summary>
    /// <see cref="ErrorMatrix"/>
    [Serializable]
    [Module(TypeName = "Vector error filter")]
    public class VectorErrorFilter : ErrorFilter
    {
        private ErrorMatrix _matrix;

        /// <summary>
        /// Error vector (one-dimensional error matrix of unit height).
        /// </summary>
        /// <remarks>
        /// The internal error buffer is resized automatically when the matrix
        /// size changes.
        /// </remarks>
        public ErrorMatrix Matrix {
            get { return _matrix; }
            set {
                if ((value == null) || (value.Height != 1)) {
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
        private VectorErrorBuffer _buffer;

        /// <summary>
        /// One-dimensional error buffer.
        /// </summary>
        private VectorErrorBuffer Buffer {
            get { return _buffer; }
            set { _buffer = value; }
        }

        /// <summary>
        /// Create a vector error filter with given error matrix.
        /// </summary>
        /// <param name="matrix">Error matrix</param>
        public VectorErrorFilter(ErrorMatrix matrix) {
            Matrix = matrix;
        }

        /// <summary>
        /// Create a vector error filter with a default error matrix.
        /// </summary>
        public VectorErrorFilter() {
            _matrix = ErrorMatrix.Samples.nextPixel;
        }

        public override double getError() {
            return Buffer.getError();
        }

        public override void setError(double error, int intensity) {
            Matrix.apply((int y, int x, double coeff) =>
                {
                    Buffer.setError(x, coeff * error);
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
                Matrix.Width) as VectorErrorBuffer;
            // null if the created result is not a VectorErrorBuffer
            Initialized = Buffer != null;
        }
    }
}
