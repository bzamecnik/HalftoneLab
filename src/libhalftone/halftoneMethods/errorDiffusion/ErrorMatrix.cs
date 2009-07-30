using System;
using System.Collections.Generic;
//using System.Linq;
using System.Text;
using Gimp;

namespace HalftoneLab
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
    /// in form of an integer definition matrix (numerators) and a divisor
    /// (denominator). The coefficient values are then precomputed and cached
    /// to the working matrix (of doubles). The purpose is to enable the user
    /// to edit the matrix comfortably without any loss of precision.
    /// </para>
    /// <para>
    /// Currently, ErrorMatrix doesn't support omni-directional error matrices.
    /// To support that you need to integrate SourcePixelOffsetY variable
    /// and treat CoefficientCount and CoefficientCapacity differently.
    /// </para>
    /// </remarks>
    [Serializable]
    [Module(TypeName = "Error matrix")]
    public class ErrorMatrix : Matrix<int, double>
    {
        private int _divisor;
        /// <summary>
        /// Divisor which acts a denominator for numerators in the 
        /// DefinitionMatrix.
        /// </summary>
        public int Divisor {
            get { return _divisor; }
            protected set { _divisor = value; }
        }

        [NonSerialized]
        private double[,] _workingMatrix;
        /// <summary>
        /// A cache matrix containing elements from the DefinitionMatrix
        /// divided by the Divisor. It needs to be computed again after
        /// deserialization (in init() function).
        /// </summary>
        protected override double[,] WorkingMatrix {
            get { return _workingMatrix; }
            set { _workingMatrix = value; }
        }

        private int _sourcePixelOffsetX;
        /// <summary>
        /// Horizontal (X) offset of source pixel, that is pixel currently
        /// processed from where the quantization error is distributed.
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
                int sum = 0;
                foreach (Coordinate<int> coord in getCoeffOffsets()) {
                    if (DefinitionMatrix[coord.Y, coord.X] != 0) {
                        sum++;
                    }
                }
                return sum;
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
        /// Create an error matrix. Coefficients are divided automatically
        /// by their sum. 
        /// </summary>
        /// <param name="coeffs">Coefficients definition, deep copied</param>
        /// <param name="sourcePixelOffsetX">X offset of the source pixel
        /// </param>
        public ErrorMatrix(int[,] coeffs, int sourcePixelOffsetX) {
            SourceOffsetX = sourcePixelOffsetX;
            // compute the divisor as a sum of all coefficients.
            int coeffSum = 0;
            // TODO: omit coefficients before source pixel X (including it)!
            foreach (int coef in coeffs) {
                coeffSum += coef;
            }
            Divisor = coeffSum;
            DefinitionMatrix = coeffs;
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
            Divisor = divisor;
            DefinitionMatrix = coeffs;
        }

        /// <summary>
        /// Create a default error matrix.
        /// </summary>
        public ErrorMatrix()
            : this(new int[,] {{0, 1}}, 1, 1) { }

        /// <summary>
        /// Compute the coefficients of working matrix. If necessary, they are
        /// normalized, as their sum must be equal to 1.0.
        /// </summary>
        protected override void computeWorkingMatrix() {
            WorkingMatrix = new double[Height, Width];
            if (Divisor != 0) {
                double divisorInverse = 1.0;
                if (Divisor != 1) {
                    divisorInverse = 1.0 / (double)Divisor;
                }
                for (int y = 0; y < Height; y++) {
                    for (int x = 0; x < Width; x++) {
                        WorkingMatrix[y, x] =
                            DefinitionMatrix[y, x] * divisorInverse;
                    }
                }
            }
        }

        public override void init(Image.ImageRunInfo imageRunInfo) {
            base.init(imageRunInfo);
        }

        //public void setDefinitionMatrix(int[,] definitionMatrix, int divisor) {
        //    Divisor = divisor;
        //    DefinitionMatrix = definitionMatrix;
        //}

        /// <summary>
        /// Delegate to a function to be applied over the matrix.
        /// </summary>
        /// <param name="y">Y offset coordinate</param>
        /// <param name="x">X offset coordinate</param>
        /// <param name="coeff">matrix coefficient at that offset</param>
        public delegate void ApplyFunc(int y, int x, double coeff);

        /// <summary>
        /// Apply a function on each of matrix coefficients and their offsets.
        /// </summary>
        /// <param name="func">Function to be applied</param>
        public void apply(ApplyFunc func) {
            // the first line goes from the source position
            foreach (Coordinate<int> offset in getCoeffOffsets()) {
                func(offset.Y, offset.X - SourceOffsetX,
                    WorkingMatrix[offset.Y, offset.X]);
            }

            // TODO: check if this variant is faster
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
        public override Matrix<int, double> Clone() {
            return new ErrorMatrix(DefinitionMatrix, SourceOffsetX, Divisor);
        }

        /// <summary>
        /// Iterate over offsets of all coefficients after the source pixel
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
        /// Convert to a human-readable representation.
        /// </summary>
        /// <returns></returns>
        public override string ToString() {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("Source offset X: {0}", SourceOffsetX);
            sb.AppendLine();
            sb.AppendLine(base.ToString());
            sb.AppendLine();
            if (DefinitionMatrix != null) {
                for (int y = 0; y < Height; y++) {
                    for (int x = 0; x < Width; x++) {
                        sb.AppendFormat("{0} ", DefinitionMatrix[y, x]);
                    }
                    sb.AppendLine();
                }
            }
            return sb.ToString();
        }

        /// <summary>
        /// Samples of most commonly used error-diffusion matrices.
        /// </summary>
        public static class Samples
        {
            public static ErrorMatrix Default;
            public static ErrorMatrix nextPixel;
            public static ErrorMatrix nextTwoPixels;
            public static ErrorMatrix floydSteinberg;
            public static ErrorMatrix jarvisJudiceNinke;
            public static ErrorMatrix stucki;
            public static ErrorMatrix burkes;
            public static ErrorMatrix fan;
            public static ErrorMatrix shiauFan1;
            public static ErrorMatrix shiauFan2;
            public static ErrorMatrix sierra;
            public static ErrorMatrix sierraTwoRow;
            public static ErrorMatrix sierraFilterLite;
            public static ErrorMatrix atkinson;
            public static ErrorMatrix hocevarNiger;

            private static List<ErrorMatrix> _list;
            /// <summary>
            /// Iterate over the list of the samples.
            /// </summary>
            /// <returns>Enumerable of sample error matrices.</returns>
            public static IEnumerable<ErrorMatrix> list() {
                return _list;
            }

            static Samples() {
                _list = new List<ErrorMatrix>();
                nextPixel = new ErrorMatrix(
                    new int[1, 2] {
                        { 0, 1 }
                    }, 0, 1)
                    {
                        Name = "Next pixel",
                        Description = "The simplest error-diffusion matrix"
                    };
                _list.Add(nextPixel);
                Default = nextPixel;
                nextTwoPixels = new ErrorMatrix(
                    new int[1, 3] {
                        { 0, 7, 3 }
                    }, 0, 10)
                    {
                        Name = "Next two pixels"
                    };
                _list.Add(nextTwoPixels);
                floydSteinberg = new ErrorMatrix(
                    new int[2, 3] {
                        { 0, 0, 7 },
                        { 3, 5, 1 }
                    }, 1, 16)
                    {
                        Name = "Floyd-Steinberg"
                    };
                _list.Add(floydSteinberg);
                jarvisJudiceNinke = new ErrorMatrix(
                    new int[3, 5] {
                        { 0, 0, 0, 7, 5 },
                        { 3, 5, 7, 5, 3 },
                        { 1, 3, 5, 3, 1 }
                    }, 2, 48)
                    {
                        Name = "Jarvis-Judice-Ninke"
                    };
                _list.Add(jarvisJudiceNinke);
                stucki = new ErrorMatrix(
                    new int[3, 5] {
                        { 0, 0, 0, 8, 4 },
                        { 2, 4, 8, 4, 2 },
                        { 1, 2, 4, 2, 1 }
                    }, 2, 42)
                    {
                        Name = "Stucki"
                    };
                _list.Add(stucki);
                burkes = new ErrorMatrix(
                    new int[2, 5] {
                        { 0, 0, 0, 4, 2 },
                        { 1, 2, 4, 2, 1 }
                    }, 2, 16)
                    {
                        Name = "Burkes"
                    };
                _list.Add(burkes);
                fan = new ErrorMatrix(
                    new int[2, 4] {
                        { 0, 0, 0, 7 },
                        { 1, 3, 5, 0 }
                    }, 2, 16)
                    {
                        Name = "Fan"
                    };
                _list.Add(fan);
                shiauFan1 = new ErrorMatrix(
                    new int[2, 4] {
                        { 0, 0, 0, 4 },
                        { 1, 1, 2, 0 }
                    }, 2, 8)
                    {
                        Name = "Shiau-Fan 1"
                    };
                _list.Add(shiauFan1);
                shiauFan2 = new ErrorMatrix(
                    new int[2, 5] {
                        { 0, 0, 0, 0, 8 },
                        { 1, 1, 2, 4, 0 }
                    }, 3, 16)
                    {
                        Name = "Shiau-Fan 2"
                    };
                _list.Add(shiauFan2);
                sierra = new ErrorMatrix(
                    new int[3, 5] {
                        { 0, 0, 0, 5, 3 },
                        { 2, 4, 5, 4, 2 },
                        { 0, 2, 3, 2, 0 }
                    }, 2, 32)
                    {
                        Name = "Sierra"
                    };
                _list.Add(sierra);
                sierraTwoRow = new ErrorMatrix(
                    new int[2, 5] {
                        { 0, 0, 0, 4, 3 },
                        { 1, 2, 3, 2, 1 }
                    }, 2, 16)
                    {
                        Name = "Sierra two row"
                    };
                _list.Add(sierraTwoRow);
                sierraFilterLite = new ErrorMatrix(
                    new int[2, 3] {
                        { 0, 0, 2 },
                        { 1, 1, 0 }
                    }, 1, 4)
                    {
                        Name = "Sierra filter lite"
                    };
                _list.Add(sierraFilterLite);
                atkinson = new ErrorMatrix(
                    new int[3, 4] {
                        { 0, 0, 1, 1 },
                        { 1, 1, 1, 0 },
                        { 0, 1, 0, 0 }
                    }, 1, 8)
                    {
                        Name = "Atkinson"
                    };
                _list.Add(atkinson);
                hocevarNiger = new ErrorMatrix(
                    new int[2, 3] {
                        { 0, 0, 7 },
                        { 4, 5, 0 }
                    }, 1, 16)
                {
                    Name = "Hocevar-Niger",
                    Description = 
                    "Error matrix of Floyd-Steinberg size optimal for " +
                    "serpentine scanning. " +
                    "Source: Reinstating Floyd-Steinberg - Improved Metrics for " +
                    "Quality Assessment of Error Diffusion Algorithms"
                };
                _list.Add(hocevarNiger);
                //_list.OrderBy((matrix) => matrix.Name);
            }
        }
    }
}
