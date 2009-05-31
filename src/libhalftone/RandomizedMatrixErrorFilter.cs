using System;
using System.Collections.Generic;
using Gimp;

namespace Halftone
{
    public class RandomizedMatrixErrorFilter : MatrixErrorFilter
    {
        public bool RandomizeCoeffCount { get; set; }
        Random _random = new Random();

        public RandomizedMatrixErrorFilter(ErrorMatrix matrix, ErrorBuffer buffer)
            : base(matrix, buffer) { }

        public RandomizedMatrixErrorFilter() : base() { }

        public override void moveNext() {
            base.moveNext();
            generateCoefficients();
        }

        void generateCoefficients() {
            int newCoeffCount;
            if (RandomizeCoeffCount) {
                // generate new coefficient count from interval [1; CoefficientCapacity]
                newCoeffCount = _random.Next(ErrorMatrix.CoefficientCapacity) + 1;
            } else {
                // use present coefficient count
                newCoeffCount = ErrorMatrix.CoefficientCount;
            }
            // TODO:
            // - the average value of weights is decreasing!
            // - the weight are distributed continuously not randomly
            //   (eg. there are no gaps when there are less weights than is capacity for them)
            double remaining = 1.0;
            IEnumerator<Coordinate<int>> iter = ErrorMatrix.GetWeights().GetEnumerator();
            for (int i = 0; i < newCoeffCount; i++) {
                if (!iter.MoveNext()) {
                    break;
                }
                double newWeight = _random.NextDouble() * remaining;
                Coordinate<int> coords = iter.Current;
                ErrorMatrix[coords.Y, coords.X] = newWeight;
                remaining -= newWeight;
            }
            // set the last weight to the remainder to have the sum of all weights equal to 1.0
            if (iter.MoveNext()) {
                Coordinate<int> coords = iter.Current;
                ErrorMatrix[coords.Y, coords.X] = remaining;
            }
        }
    }
}
