using System;
using System.Collections.Generic;
using Gimp;

namespace Halftone
{
    /// <summary>
    /// Matrix of error-diffusion coefficients. 
    /// </summary>
    /// <remarks>
    /// <para>
    /// In error-diffusion the quantization error from a source pixel is
    /// distributed to its neighbourhood. Error matrix defines what
    /// proportion of the error goes to which pixel.
    /// </para>
    /// <para>
    /// To define these ratios error matrix contains a rational coefficients
    /// in form of an integer definition matrix and a divisor. The coefficient
    /// values are then precomputed and cached to a matrix of doubles. The
    /// purpose is to enable the user to edit the matrix comfortably without
    /// a loss of precision.
    /// </para>
    /// <para>
    /// Currently ErrorMatrix doesn't support omni-directional error matrices.
    /// To support that you need to integrate _sourcePixelOffsetY variable
    /// and treat CoefficientCount and CoefficientCapacity differently.
    /// </para>
    /// </remarks>
    [Serializable]
    public class ErrorMatrix : Module
    {
        private int[,] _definitionMatrix;
        private int _divisor;
        /// <summary>
        /// A cache matrix containing elements from _definitionMatrix divided
        /// by _divisor. It needs to be computed again after deserialization
        /// (in init() function).
        /// </summary>
        [NonSerialized]
        private double[,] _workingMatrix;

        /// <summary>
        /// Matrix height (= max Y coordinate + 1).
        /// </summary>
        public int Height {
            get { return _workingMatrix.GetLength(0); }
        }

        /// <summary>
        /// Matrix width (= max X coordinate + 1).
        /// </summary>
        public int Width {
            get { return _workingMatrix.GetLength(1); }
        }

        /// <summary>
        /// Access matrix elements using by coordinates with modulo
        /// operation applied.
        /// </summary>
        /// <param name="y">Y coordinate (zero based)</param>
        /// <param name="x">X coordinate (zero based)</param>
        /// <returns></returns>
        public double this[int y, int x] {
            get {
                return _workingMatrix[y % Height, x % Width];
            }
            set {
                _workingMatrix[y % Height, x % Width] = value;
            }
        }

        private int _sourcePixelOffsetX;

        /// <summary>
        /// Horizontal (X) offset of source pixel, that is pixel currently processed
        /// from where the quantization error is distributed.
        /// </summary>
        /// <remarks>
        /// A vertical (Y) offset > 0 wouldn't make sense here as we cannot
        /// set error to already computed pixels (it would make sense only
        /// for omni-directional error diffusion).
        /// </remarks>
        public int SourceOffsetX {
            get { return _sourcePixelOffsetX; }
            private set { _sourcePixelOffsetX = value; }
        }

        /// <summary>
        /// Number of coefficients (weights) in the matrix, ie. non-zero
        /// ones only after source pixel offset.
        /// </summary>
        public int CoefficientCount {
            get {
                // TODO: exclude zero weights ( |offset| < epsilon)
                return Height * Width - SourceOffsetX; // this is a stub
            }
        }

        /// <summary>
        /// Number of coefficients (weights) in the matrix after source
        /// pixel offset, including zero ones.
        /// </summary>
        public int CoefficientCapacity {
            get {
                return Height * Width - SourceOffsetX;
            }
        }

        /// <summary>
        /// Delegate to a function to be applied over the matrix.
        /// </summary>
        /// <param name="y">Y offset coordinate</param>
        /// <param name="x">X offset coordinate</param>
        /// <param name="coeff">matrix coefficient at that offset</param>
        public delegate void ApplyFunc(int y, int x, double coeff);

        /// <summary>
        /// Create an error matrix. Coefficients are divided automatically
        /// by their sum. 
        /// </summary>
        /// <param name="coeffs">Coefficients definition, deep copied</param>
        /// <param name="sourcePixelOffsetX">X offset of the source pixel</param>
        public ErrorMatrix(int[,] coeffs, int sourcePixelOffsetX) {
            SourceOffsetX = sourcePixelOffsetX;
            _definitionMatrix = (int[,])coeffs.Clone();
            // compute the divisor as a sum of all coefficients.
            int coeffSum = 0;
            foreach (int coef in coeffs) {
                coeffSum += coef;
            }
            _divisor = coeffSum;
            computeWorkingMatrix();
        }

        /// <summary>
        /// Create an error matrix. Coefficients are divided by a given
        /// divisor.
        /// </summary>
        /// <param name="coeffs">Coefficients definition, deep copied</param>
        /// <param name="sourcePixelOffsetX">X offset of the source pixel</param>
        /// <param name="divisor">Divisor of coefficients</param>
        public ErrorMatrix(int[,] coeffs, int sourcePixelOffsetX, int divisor) {
            SourceOffsetX = sourcePixelOffsetX;
            _definitionMatrix = (int[,])coeffs.Clone();
            _divisor = divisor;
            computeWorkingMatrix();
        }

        /// <summary>
        /// Compute a cached matrix of coefficient values.
        /// Scale the working matrix coefficients if necessary.
        /// Their sum must be equal to 1.0.
        /// </summary>
        private void computeWorkingMatrix() {
            _workingMatrix = new double[
                _definitionMatrix.GetLength(0),
                _definitionMatrix.GetLength(1)];
            if ((_divisor != 0) || (_divisor != 1)) {
                double divisorInverse = 1.0 / (double)_divisor;
                for (int y = 0; y < Height; y++) {
                    for (int x = 0; x < Width; x++) {
                        _workingMatrix[y, x] = _definitionMatrix[y, x] * divisorInverse;
                    }
                }
            }
        }

        public override void init(Image.ImageRunInfo imageRunInfo) {
            base.init(imageRunInfo);
            computeWorkingMatrix();
        }

        /// <summary>
        /// Apply a function on each of matrix coefficients and their offsets.
        /// </summary>
        /// <param name="func">Function to be applied</param>
        public void apply(ApplyFunc func) {
            // the first line goes from the source position
            foreach (Coordinate<int> offset in getCoeffOffsets()) {
                func(offset.Y, offset.X - SourceOffsetX, _workingMatrix[offset.Y, offset.X]);
            }

            //// old code:
            //for (int x = SourceOffsetX; x < Width; x++) {
            //    func(0, x - SourceOffsetX, this[0, x]);
            //}
            //for (int y = 1; y < Height; y++) {
            //    for (int x = 0; x < Width; x++) {
            //        func(y, x - SourceOffsetX, this[y, x]);
            //    }
            //}
        }

        /// <summary>
        /// Clone the matrix.
        /// </summary>
        /// <returns>Cloned error matrix</returns>
        public ErrorMatrix Clone() {
            return new ErrorMatrix(_definitionMatrix, SourceOffsetX, _divisor);
        }

        /// <summary>
        /// Enumerate offsets of all coefficients after the source pixel
        /// offset.
        /// </summary>
        /// <returns>Enumerable of offset coordinates</returns>
        public IEnumerable<Coordinate<int>> getCoeffOffsets() {
            // the first line goes from the source position
            for (int x = SourceOffsetX + 1; x < Width; x++) {
                yield return new Coordinate<int>(x, 0);
            }
            for (int y = 1; y < Height; y++) {
                for (int x = 0; x < Width; x++) {
                    yield return new Coordinate<int>(x, y);
                }
            }
            yield break;
        }

        /// <summary>
        /// Samples of most commonly used error-diffusion matrices.
        /// </summary>
        public static class Samples
        {
            public static ErrorMatrix Default;
            // the simplest error-diffusion matrix
            public static ErrorMatrix nextPixel;
            public static ErrorMatrix nextTwoPixels;
            public static ErrorMatrix simpleNeighborhood;

            // Floyd-Steinberg
            public static ErrorMatrix floydSteinberg;
            // Jarvis-Judice-Ninke 
            public static ErrorMatrix jarvisJudiceNinke;
            // Stucki
            public static ErrorMatrix stucki;
            // Burkes
            public static ErrorMatrix burkes;
            // Fan
            public static ErrorMatrix fan;
            // Shiau-Fan #1
            public static ErrorMatrix shiauFan1;
            // Shiau-Fan #2
            public static ErrorMatrix shiauFan2;
            // Sierra
            public static ErrorMatrix sierra;
            // Two-row Sierra
            public static ErrorMatrix sierraTwoRow;
            // Sierra's Filter Lite
            public static ErrorMatrix sierraFilterLite;
            // Atkinson
            public static ErrorMatrix atkinson;
            // Error matrix of Floyd-Steinberg size optimal for
            // serpentine scanning.
            // Source: Reinstating Floyd-Steinberg - Improved Metrics for
            // Quality Assessment of Error Diffusion Algorithms
            public static ErrorMatrix serpentineOptimal;

            static Samples() {
                nextPixel = new ErrorMatrix(
                    new int[1, 2] {
                        { 0, 1 }
                    }, 0, 1);
                Default = nextPixel;
                nextTwoPixels = new ErrorMatrix(
                    new int[1, 3] {
                        { 0, 7, 3 }
                    }, 0, 10);
                simpleNeighborhood = new ErrorMatrix(
                    new int[2, 2] {
                        { 0, 2 },
                        { 1, 1 }
                    }, 0, 4);
                floydSteinberg = new ErrorMatrix(
                    new int[2, 3] {
                        { 0, 0, 7 },
                        { 3, 5, 1 }
                    }, 1, 16);
                jarvisJudiceNinke = new ErrorMatrix(
                    new int[3, 5] {
                        { 0, 0, 0, 7, 5 },
                        { 3, 5, 7, 5, 3 },
                        { 1, 3, 5, 3, 1 }
                    }, 2, 48);
                stucki = new ErrorMatrix(
                    new int[3, 5] {
                        { 0, 0, 0, 8, 4 },
                        { 2, 4, 8, 4, 2 },
                        { 1, 2, 4, 2, 1 }
                    }, 2, 42);
                burkes = new ErrorMatrix(
                    new int[2, 5] {
                        { 0, 0, 0, 4, 2 },
                        { 1, 2, 4, 2, 1 }
                    }, 2, 16);
                fan = new ErrorMatrix(
                    new int[2, 3] {
                        { 0, 0, 7 },
                        { 1, 3, 5 }
                    }, 1, 16);
                shiauFan1 = new ErrorMatrix(
                    new int[2, 4] {
                        { 0, 0, 0, 4 },
                        { 1, 1, 2, 0 }
                    }, 2, 8);
                shiauFan2 = new ErrorMatrix(
                    new int[2, 5] {
                        { 0, 0, 0, 0, 8 },
                        { 1, 1, 2, 4, 0 }
                    }, 3, 16);
                sierra = new ErrorMatrix(
                    new int[3, 5] {
                        { 0, 0, 0, 5, 3 },
                        { 2, 4, 5, 4, 2 },
                        { 0, 2, 3, 2, 0 }
                    }, 2, 32);
                sierraTwoRow = new ErrorMatrix(
                    new int[2, 5] {
                        { 0, 0, 0, 4, 3 },
                        { 1, 2, 3, 2, 1 }
                    }, 2, 16);
                sierraFilterLite = new ErrorMatrix(
                    new int[2, 3] {
                        { 0, 0, 2 },
                        { 1, 1, 0 }
                    }, 1, 4);
                atkinson = new ErrorMatrix(
                    new int[3, 4] {
                        { 0, 0, 1, 1 },
                        { 1, 1, 1, 0 },
                        { 0, 1, 0, 0 }
                    }, 1, 8);
                serpentineOptimal = new ErrorMatrix(
                    new int[2, 3] {
                        { 0, 0, 7 },
                        { 4, 5, 0 }
                    }, 1, 16);
            }
        }
    }
}