using System;
using System.Collections.Generic;
using Gimp;

namespace Halftone
{
    /// <summary>
    /// Randomized matrix error-diffusion filter is able to generate
    /// matrix coefficients randomly.
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
        private Random RandomGenerator {
            get {
                if (_randomGenerator == null) {
                    _randomGenerator = new Random();
                }
                return _randomGenerator;
            }
        }

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
            if (RandomizeCoeffCount) {
                // generate new coefficient count from interval [1; CoefficientCapacity]
                newCoeffCount = RandomGenerator.Next(Matrix.CoefficientCapacity) + 1;
            } else {
                // use present coefficient count
                newCoeffCount = Matrix.CoefficientCount;
            }
            // TODO:
            // - the average value of weights is decreasing!
            // - the weight are distributed continuously not randomly
            //   (eg. there are no gaps when there are less weights than is capacity for them)
            double remaining = 1.0;
            IEnumerator<Coordinate<int>> iter = Matrix.getCoeffOffsets().GetEnumerator();
            for (int i = 0; i < newCoeffCount; i++) {
                if (!iter.MoveNext()) {
                    break;
                }
                double newWeight = RandomGenerator.NextDouble() * remaining;
                Coordinate<int> coords = iter.Current;
                Matrix[coords.Y, coords.X] = newWeight;
                remaining -= newWeight;
            }
            // set the last weight to the remainder to have the sum of all weights equal to 1.0
            if (iter.MoveNext()) {
                Coordinate<int> coords = iter.Current;
                Matrix[coords.Y, coords.X] = remaining;
            }
        }

        public override void init(Image.ImageRunInfo imageRunInfo) {
            base.init(imageRunInfo);
        }
    }
}
