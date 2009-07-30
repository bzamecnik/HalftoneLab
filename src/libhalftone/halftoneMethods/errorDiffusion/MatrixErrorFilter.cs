using System;
using System.Text;

namespace HalftoneLab
{
    /// <summary>
    /// Error-diffusion filter with a single error matrix.
    /// </summary>
    /// <see cref="ErrorMatrix"/>
    [Serializable]
    [Module(TypeName = "Matrix error filter")]
    public class MatrixErrorFilter : ErrorFilter
    {
        
        private ErrorMatrix _matrix;
        
        /// <summary>
        /// Error matrix.
        /// </summary>
        /// <remarks>
        /// After the matrix is set, the error buffer might be resized,
        /// if it does not match the new matrix.
        /// </remarks>
        public ErrorMatrix Matrix {
            get { return _matrix; }
            set {
                setMatrix(value, true);
            }
        }

        [NonSerialized]
        private MatrixErrorBuffer _buffer;

        /// <summary>
        /// Error bufer for storing error diffused to not yet processed
        /// neighbour pixels.
        /// </summary>
        public MatrixErrorBuffer Buffer {
            get { return _buffer; }
            protected set { _buffer = value; }
        }

        //public event EventHandler MatrixChanged;
        
        /// <summary>
        /// Create a new matrix error filter.
        /// </summary>
        /// <param name="matrix">Error matrix</param>
        public MatrixErrorFilter(ErrorMatrix matrix) {
            Matrix = matrix;
        }

        /// <summary>
        /// Create a new error filter with default error matrix.
        /// </summary>
        public MatrixErrorFilter() {
            _matrix = ErrorMatrix.Samples.Default;
        }

        /// <summary>
        /// Set the matrix with optional buffer resize.
        /// </summary>
        /// <remarks>
        /// Resize the buffer if a matrix with different height is set.
        /// Note: buffer width depends on image size.
        /// </remarks>
        /// <param name="matrix">New error matrix</param>
        /// <param name="resizeBuffer">True if buffer can be resized</param>
        protected void setMatrix(ErrorMatrix matrix, bool resizeBuffer) {
            if (matrix != null) {
                
                if (resizeBuffer && (Buffer != null) &&
                    (matrix.Height != _matrix.Height)) {
                    Buffer.resize(_matrix.Height, Buffer.Width);
                }
                _matrix = matrix;
                //if (MatrixChanged != null) {
                //    MatrixChanged(this, new EventArgs());
                //}
            }
        }

        public override double getError() {
            return Buffer.getError();
        }

        public override void setError(double error, int intensity) {
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
