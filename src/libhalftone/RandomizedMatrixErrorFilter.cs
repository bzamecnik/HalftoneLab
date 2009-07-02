using System;
using System.Collections.Generic;
using Gimp;

namespace Halftone
{
    [Serializable]
    public class RandomizedMatrixErrorFilter : MatrixErrorFilter
    {
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

        public RandomizedMatrixErrorFilter(ErrorMatrix matrix)
            : base(matrix) { }

        public RandomizedMatrixErrorFilter() : base() { }

        public override void moveNext() {
            base.moveNext();
            generateCoefficients();
        }

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
    }
}
