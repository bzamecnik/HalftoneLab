// Copyright (c) 2009 Bohumir Zamecnik <bohumir@zamecnik.org>
// License: The MIT License, see the LICENSE file

using System;

namespace HalftoneLab
{
    // TODO: incremental -> normalized (with opposite meaning)

    /// <summary>
    /// Tileable matrix of threshold values.
    /// </summary>
    /// <remarks>
    /// Coefficients are scaled to 0-255 range.
    /// </remarks>
    [Serializable]
    [Module(TypeName = "Threshold matrix")]
    public class ThresholdMatrix : Matrix<int, int>
    {
        [NonSerialized]
        private int[,] _workingMatrix;
        protected override int[,] WorkingMatrix {
            get { return _workingMatrix; }
            set { _workingMatrix = value; }
        }

        // 
        private bool _incremental;
        /// <summary>
        /// Is the definition matrix in incremental form (true) or already
        /// normalized (false)?
        /// </summary>
        public bool Incremental {
            get { return _incremental; }
        }

        /// <summary>
        /// Create a threshold matrix from given definition matrix.
        /// </summary>
        /// <param name="matrix">Definition matrix</param>
        /// <param name="incremental">True - matrix is incremental, false - it
        /// is normalized</param>
        public ThresholdMatrix(int[,] matrix, bool incremental) {
            _incremental = incremental;
            DefinitionMatrix = matrix;
        }

        /// <summary>
        /// Create a threshold matrix from given incremental matrix.
        /// </summary>
        /// <param name="matrix">Definition matrix in incremental form</param>
        public ThresholdMatrix(int[,] matrix)
            : this(matrix, true) { }

        /// <summary>
        /// Create a default threshold matrix with one coefficient at 128.
        /// </summary>
        public ThresholdMatrix()
            : this(new int[1, 1] { { 128 } }, false) { }

        public override Matrix<int, int> Clone() {
            return new ThresholdMatrix(DefinitionMatrix);
        }

        protected override void computeWorkingMatrix() {
            _workingMatrix = (_incremental) ?
                normalizeFromIncrementalMatrix(DefinitionMatrix) :
                DefinitionMatrix;
        }

        /// <summary>
        /// Normalize (scale) coefficients from incremental matrix to 0-255
        /// range.
        /// </summary>
        /// <param name="incrMatrix">matrix with coefficients in
        /// range 1-(h*w)</param>
        /// <returns>scaled matrix with coefficients in 0-255 range</returns>
        private static int[,] normalizeFromIncrementalMatrix(int[,] incrMatrix) {
            int height = incrMatrix.GetLength(0);
            int width = incrMatrix.GetLength(1);
            //return normalizeFromIncrementalMatrix(incrMatrix,
            //    height * width + 1);
            
            // used (the maximum matrix coefficient + 1) as the divisor
            int maxCoeff = 0;
            for (int y = 0; y < height; y++) {
                for (int x = 0; x < width; x++) {
                    maxCoeff = Math.Max(maxCoeff, incrMatrix[y, x]);
                }
            }
            return normalizeFromIncrementalMatrix(incrMatrix, maxCoeff + 1);

        }

        /// <summary>
        /// Normalize (scale) matrix coefficients from incremental form to
        /// 0-255 range.
        /// </summary>
        /// <param name="incrMatrix">Matrix in incremental form</param>
        /// <param name="divisor">Divisor</param>
        /// <returns>Normalized matrix</returns>
        private static int[,] normalizeFromIncrementalMatrix(
            int[,] incrMatrix, int divisor)
        {
            int height = incrMatrix.GetLength(0);
            int width = incrMatrix.GetLength(1);
            double coeff = 255.0 / (double)divisor;
            int[,] matrix = new int[height, width];
            for (int y = 0; y < height; y++) {
                for (int x = 0; x < width; x++) {
                    matrix[y, x] = (int)(incrMatrix[y, x] * coeff);
                }
            }
            return matrix;
        }

        public override void init(HalftoneLab.Image.ImageRunInfo imageRunInfo) {
            base.init(imageRunInfo);
        }

        /// <summary>
        /// A utility class for generating threshold matrices.
        /// </summary>
        public class Generator
        {
            public static ThresholdMatrix sampleMatrix;
            public static ThresholdMatrix simpleThreshold;
            static Generator() {
                simpleThreshold = new ThresholdMatrix(new int[1, 1] { { 128 } }, false);
                sampleMatrix = new ThresholdMatrix(
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
                    return new ThresholdMatrix(new int[1, 1] { { 0 } });
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
                    return new ThresholdMatrix(matrix);
                }
            }
        }
    }
}
