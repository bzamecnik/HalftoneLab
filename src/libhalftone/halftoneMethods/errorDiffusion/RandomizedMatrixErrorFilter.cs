// Copyright (c) 2009 Bohumir Zamecnik <bohumir@zamecnik.org>
// License: The MIT License, see the LICENSE file

using System;
using System.Collections.Generic;
using Gimp;
using System.Linq;

namespace HalftoneLab
{
    /// <summary>
    /// Randomized matrix error-diffusion filter is able to generate
    /// matrix coefficients randomly for each pixel.
    /// </summary>
    /// <remarks>
    /// It can use an existing matrix as a template for coefficient positions
    /// and position of the source pixel. The other way is to randomize
    /// even the number of coefficients up to the the original matrix
    /// capacity.
    /// </remarks>
    /// <see cref="ErrorMatrix"/>
    /// <see cref="MatrixErrorFilter"/>
    [Serializable]
    [Module(TypeName = "Randomized matrix error filter")]
    public class RandomizedMatrixErrorFilter : MatrixErrorFilter
    {
        /// <summary>
        /// Randomize the number of coefficients in the matrix
        /// or match the template matrix?
        /// </summary>
        public bool RandomizeCoeffCount { get; set; }

        [NonSerialized]
        private Random _randomGenerator = null;

        /// <summary>
        /// Random generator for noise perturbation.
        /// </summary>
        /// <remarks>
        /// Instantiated on demand.
        /// </remarks>
        private Random RandomGenerator {
            get {
                if (_randomGenerator == null) {
                    _randomGenerator = new Random();
                }
                return _randomGenerator;
            }
        }

        /// <summary>
        /// Coordinates of existing non-zero coefficients in the template
        /// matrix.
        /// </summary>
        [NonSerialized]
        private Coordinate<int>[] templateCoords;

        /// <summary>
        /// Coordinates of all coefficients up to the capacity of the template
        /// matrix.
        /// </summary>
        [NonSerialized]
        private Coordinate<int>[] allCoords;

        /// <summary>
        /// Create a randomized matrix error filter using an existing
        /// matrix as a template.
        /// </summary>
        /// <param name="matrix">Template error matrix</param>
        public RandomizedMatrixErrorFilter(ErrorMatrix matrix)
            : base(matrix) { }

        /// <summary>
        /// Create a randomized matrix error filter using a default
        /// error matrix as a template.
        /// </summary>
        public RandomizedMatrixErrorFilter() : base() { }

        public override void moveNext() {
            base.moveNext();
            generateCoefficients();
        }

        /// <summary>
        /// Generate matrix coefficients randomly.
        /// </summary>
        /// <remarks>
        /// Ensure that sum of the coefficitents is equal to 1.0 in order
        /// to distribute exactly 100% of the quantization error.
        /// </remarks>
        private void generateCoefficients() {
            int newCoeffCount;
            double[] newCoeffs;
            Coordinate<int>[] coords;
            if (RandomizeCoeffCount) {
                // generate new coefficient count from interval [1; CoefficientCapacity]
                newCoeffCount = RandomGenerator.Next(Matrix.CoefficientCapacity) + 1;
                newCoeffs = new double[Matrix.CoefficientCapacity];
                coords = allCoords;
            } else {
                // use present coefficient count
                newCoeffCount = Matrix.CoefficientCount;
                newCoeffs = new double[newCoeffCount];
                coords = templateCoords;
            }
            
            // generate new weights
            double remaining = 1.0;
            for (int i = 0; i < newCoeffCount - 1; i++) {
                double newWeight = RandomGenerator.NextDouble() * remaining;
                newCoeffs[i] = newWeight;
                remaining -= newWeight;
            }
            // set the last weight to the remainder to have the sum of all weights equal to 1.0
            newCoeffs[newCoeffCount - 1] = remaining;

            // shuffle the weights to prevent the average value
            // of weights to decrease - swap pairs of weights
            for (int i = 0; i < newCoeffs.Length; i++) {
                int srcIndex = RandomGenerator.Next(newCoeffs.Length);
                int dstIndex = RandomGenerator.Next(newCoeffs.Length);
                double tmp = newCoeffs[dstIndex];
                newCoeffs[dstIndex] = newCoeffs[srcIndex];
                newCoeffs[srcIndex] = tmp;
            }

            // set new weights
            for (int i= 0; i< coords.Length; i++) {
                Coordinate<int> coord = coords[i];
                Matrix[coord.Y, coord.X] = newCoeffs[i];
            }
        }

        public override void init(Image.ImageRunInfo imageRunInfo) {
            base.init(imageRunInfo);
            List<Coordinate<int>> templateCoordsList =
                Matrix.getCoeffOffsets().ToList();
            allCoords = templateCoordsList.ToArray();
            // remove all coordinates with zero coefficients
            templateCoordsList.RemoveAll(coord =>
                Matrix.DefinitionMatrix[coord.Y, coord.X] == 0);
            templateCoords = templateCoordsList.ToArray();
            generateCoefficients();
        }
    }
}
