// MatrixErrorFilter.cs created with MonoDevelop
// User: bohous at 15:31 26.3.2009
//

using System;
using Gimp;

namespace Halftone
{
    public class MatrixErrorFilter : ErrorFilter
    {
        // TODO: properties for matrix and buffer
        // - make a new buffer if a matrix with different dimensions is set

        // matrix of error filter weights
        ErrorMatrix _matrix;

        public ErrorMatrix ErrorMatrix {
            get { return _matrix; }
            private set { _matrix = value; }
        }

        // error buffer
        ErrorBuffer _buffer;

        public ErrorBuffer ErrorBuffer {
            get { return _buffer; }
            private set { _buffer = value; }
        }

        public MatrixErrorFilter(ErrorMatrix matrix, ErrorBuffer buffer) {
            ErrorMatrix = matrix;
            ErrorBuffer = buffer;
        }

        public MatrixErrorFilter() {
            _matrix = new ErrorMatrix();
            _buffer = ErrorBuffer.createDefaultErrorBuffer();
        }

        public override double getError() {
            return _buffer.getError();
        }

        // diffuse error value from given pixel to neighbor pixels
        public override void setError(double error) {
            _matrix.apply(
                (int y, int x, double coeff) => { _buffer.setError(y, x, coeff * error); }
                );
        }

        public override void setError(double error, int intensity) {
            setError(error);
        }

        public override void moveNext() {
            _buffer.moveNext();
        }
    }
}
