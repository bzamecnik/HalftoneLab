using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using Gimp;

namespace Halftone {
    public class ErrorMatrix : Matrix<double>
    {
        // TheMatrix property - matrix of error filter coefficients (ie. weights)

        // Horizontal (X) offset of source pixel, that is pixel currently processed
        // from where the quantization error is distributed.
        // A vertical (Y) offset > 0 wouldn't make sense as we cannot set error to
        // already computed pixels.
        private int _sourcePixelOffsetX;

        public ErrorMatrix(double[,] coeffs, int sourcePixelOffsetX) {
            _sourcePixelOffsetX = sourcePixelOffsetX;
            TheMatrix = (double[,])coeffs.Clone();
            // scale down the coefficients if necessary (their sum must be 1.0)
            double coeffSum = 0;
            foreach (double coef in TheMatrix) {
                coeffSum += coef;
            }
            if ((coeffSum != 0) || (coeffSum != 1.0)) {
                for (int y = 0; y < Height; y++) {
                    for (int x = 0; x < Width; x++) {
                        this[y, x] /= coeffSum;
                    }
                }
            }
        }

        public ErrorMatrix()
            : this(new double[1, 1] { { 1 } }, 0) { }

        public int SourceOffset {
            get { return _sourcePixelOffsetX; }
        }

        // number of coefficients (weights)
        public int CoefficientCount {
            get {
                // including only zero weights past source pixel offset
                // TODO: exclude all zero weights ( |offset| < epsilon)
                return Height * Width - SourceOffset;
            }
        }

        public int CoefficientCapacity {
            get {
                // including all weights past source pixel offset
                return Height * Width - SourceOffset;
            }
        }

        public delegate void ApplyFunc(int y, int x, double coeff);

        public void apply(ApplyFunc func) {
            // the first line goes from the source position
            foreach (Coordinate<int> coords in GetWeights()) {
                func(coords.Y, coords.X - SourceOffset, this[coords.Y, coords.X]);
            }

            //// old code
            //for (int x = SourceOffset; x < Width; x++) {
            //    func(0, x - SourceOffset, this[0, x]);
            //}
            //for (int y = 1; y < Height; y++) {
            //    for (int x = 0; x < Width; x++) {
            //        func(y, x - SourceOffset, this[y, x]);
            //    }
            //}
        }

        public override Matrix<double> Clone() {
            return new ErrorMatrix(TheMatrix, _sourcePixelOffsetX);
        }

        public IEnumerable<Coordinate<int>> GetWeights() {
            // the first line goes from the source position
            for (int x = SourceOffset + 1; x < Width; x++) {
                yield return new Coordinate<int>(x, 0);
            }
            for (int y = 1; y < Height; y++) {
                for (int x = 0; x < Width; x++) {
                    yield return new Coordinate<int>(x, y);
                }
            }
            yield break;
        }

        public override string ToString() {
            StringBuilder sb = new StringBuilder();
            for (int y = 0; y < Height; y++) {
                for (int x = 0; x < Width; x++) {
                    sb.AppendFormat("{0} ", this[y, x]);
                }
                sb.AppendLine();
            }
            return sb.ToString();
        }
    }
}