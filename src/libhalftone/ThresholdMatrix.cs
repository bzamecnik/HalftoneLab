using System;
using Gimp;

namespace Halftone
{
    // TODO:
    // - make a definition matrix (which could be iterative) and scaled
    //   working one
    // - working matrix could be of double type

    /// <summary>
    /// Tileable matrix of threshold values.
    /// </summary>
    /// <remarks>
    /// Coefficients are scaled to 0-255 range.
    /// </remarks>
    [Serializable]
    public class ThresholdMatrix : Matrix<int>
    {
        /// <summary>
        /// Create a threshold matrix of given size.
        /// </summary>
        /// <param name="height">matrix height (max Y size), > 0</param>
        /// <param name="width">matrix height (max X size), > 0</param>
        public ThresholdMatrix(int height, int width)
            : base(height, width) {}

        /// <summary>
        /// Create a threshold matrix from given scaled matrix.
        /// </summary>
        /// <param name="matrix">matrix with coefficients scaled to 0-255
        /// range.</param>
        public ThresholdMatrix(int[,] matrix)
            : base(matrix) { }

        public override Matrix<int> Clone() {
            return new ThresholdMatrix(matrix);
        }

        public class Generator
        {
            public static ThresholdMatrix sampleMatrix;
            public static ThresholdMatrix simpleThreshold;
            static Generator() {
                simpleThreshold = new ThresholdMatrix(new int[1, 1] { { 128 } });
                sampleMatrix =
                    ThresholdMatrix.Generator.createFromIterativeMatrix(
                    new int[,] {
                    //{ 16,  5,  9, 13 },
                    //{ 12,  1,  2,  6 },
                    //{  8,  3,  4, 10 },
                    //{ 15, 11,  7, 14 }
                    
                    //{62, 54, 41, 23, 35, 43, 59, 63},
                    //{58, 50, 25, 20, 29, 38, 51, 55},
                    //{46, 30, 13,  5, 12, 16, 34, 42},
                    //{21, 17,  9,  1,  4,  8, 28, 37},
                    //{26, 18,  6,  2,  3, 11, 23, 33},
                    //{48, 31, 14, 10,  7, 15, 40, 44},
                    //{53, 49, 32, 19, 27, 36, 52, 59},
                    //{61, 57, 45, 22, 39, 47, 56, 64}

                    //{ 46, 35, 23, 22, 32, 43, 48},
                    //{ 36, 16, 11, 10, 15, 34, 40},
                    //{ 24, 12,  4,  3,  8, 20, 28},
                    //{ 21,  9,  2,  1,  6, 18, 25},
                    //{ 31, 14,  7,  5, 13, 30, 37},
                    //{ 42, 33, 19, 17, 29, 41, 44},
                    //{ 47, 39, 27, 26, 38, 45, 49},

                    //{ 16,  5,  9, 13, 32, 19, 25, 39 },
                    //{ 12,  1,  2, 6,  28, 17, 18, 22 },
                    //{  8,  4,  3, 10, 24, 20, 19, 26 },
                    //{ 15, 11,  7, 14, 31, 27, 23, 30 },
                    //{ 32, 19, 25, 39, 16,  5,  9, 13 },
                    //{ 28, 17, 18, 22, 12,  1,  2,  6 },
                    //{ 24, 20, 19, 26,  8,  4,  3, 10 },
                    //{ 31, 27, 23, 30, 15, 11,  7, 14 }

                    { 32, 10, 18, 26, 34, 56, 48, 40 },
                    { 24,  2,  4, 12, 42, 64, 62, 54 },
                    { 16,  8,  6, 20, 50, 58, 60, 46 },
                    { 30, 22, 14, 28, 36, 44, 52, 38 },
                    { 34, 56, 48, 40, 32, 10, 18, 26 },
                    { 42, 64, 62, 54, 24,  2,  4, 12 },
                    { 50, 58, 60, 46, 16,  8,  6, 20 },
                    { 36, 44, 52, 38, 30, 22, 14, 28 }
                    
                });
            }

            /// <summary>
            /// Scale down coefficients from iterative matrix to 0-255 range.
            /// </summary>
            /// <param name="userMatrix">matrix with coefficients in range 1-(h*w)</param>
            /// <returns>matrix with coefficients in range 0-255</returns>
            public static ThresholdMatrix createFromIterativeMatrix(int[,] userMatrix) {
                int height = userMatrix.GetLength(0);
                int width = userMatrix.GetLength(1);
                double coeff = (double)255 / (height * width + 1);
                ThresholdMatrix matrix = new ThresholdMatrix(height, width);
                for (int y = 0; y < height; y++) {
                    for (int x = 0; x < width; x++) {
                        matrix[y, x] = (int)(userMatrix[y, x] * coeff);
                    }
                }
                return matrix;
            }

            /// <summary>
            /// Create a Bayer dispersed dot matrix (recursive tesselation
            /// matrix) of size 2^N x 2^N (where N is magnitude).
            /// </summary>
            /// <remarks>
            /// It is able to represent (2^N*2^N)-1 tones.
            /// A recursive algorithm is used.
            /// </remarks>
            /// <param name="magnitude">log_2 of matrix size, range: [0, 8]</param>
            /// <returns>scaled Bayer matrix</returns>
            public static ThresholdMatrix createBayerDispersedDotMatrix(int magnitude) {
                if ((magnitude < 0) || (magnitude > 8)) {
                    return null;
                }
                if (magnitude == 0) {
                    return createFromIterativeMatrix(new int[1, 1] { { 0 } });
                } else {
                    int[,] offsets = { { 0, 0 }, { 1, 1 }, { 0, 1 }, { 1, 0 } };
                    int[,] matrix = new int[2, 2] { { 0, 2 }, { 3, 1 } };
                    int[,] newMatrix;
                    for (int i = 1; i < magnitude; i++) {
                        int oldSize = matrix.GetLength(0);
                        newMatrix = new int[oldSize * 2, oldSize * 2];
                        for (int quadrant = 0; quadrant < 4; quadrant++) {
                            for (int y = 0; y < oldSize; y++) {
                                for (int x = 0; x < oldSize; x++) {
                                    newMatrix[offsets[quadrant, 0] * oldSize + y,
                                              offsets[quadrant, 1] * oldSize + x] =
                                        4 * matrix[y, x] + quadrant;
                                }
                            }
                        }
                        matrix = newMatrix;
                    }
                    for (int y = 0; y < matrix.GetLength(0); y++) {
                        for (int x = 0; x < matrix.GetLength(1); x++) {
                            matrix[y, x]++;
                        }
                    }
                    return createFromIterativeMatrix(matrix);
                }
            }
        }
    }
}