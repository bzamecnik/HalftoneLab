using System;
using System.Collections.Generic;
using Gimp;

namespace Halftone
{
    [Serializable]
    public class PerturbedErrorFilter : ErrorFilter
    {
        private MatrixErrorFilter _childFilter;
        public MatrixErrorFilter ChildFilter {
            get {
                if (_childFilter == null) {
                    _childFilter = new MatrixErrorFilter();
                }

                return _childFilter;
            }
            set {
                // TODO: on deserialization call this insted just setting _childFilter
                _childFilter = value;
                _originalMatrix = _childFilter.ErrorMatrix;
                _perturbedMatrix = (ErrorMatrix)OriginalMatrix.Clone();
                _weightGroups = new List<WeightGroup>();
                preparePerturbationGroups();
                computePerturbation();
            }
        }

        [NonSerialized]
        private List<WeightGroup> _weightGroups;
        
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

        [NonSerialized]
        private ErrorMatrix _originalMatrix;
        private ErrorMatrix OriginalMatrix {
            get {
                return _originalMatrix;
            }
        }

        // Note: Creating an empty matrix with the same _size and source offset
        // as in original matrix would be enough. The values are overwritten
        // in computePerturbation() anyway.
        [NonSerialized]
        private ErrorMatrix _perturbedMatrix; // temporary
        private ErrorMatrix PerturbedMatrix {
            get {
                return _perturbedMatrix;
            }
        }

        private double _perturbationAmplitude;
        public double PerturbationAmplitude { // 0.0-1.0
            get { return _perturbationAmplitude; }
            set {
                if ((value >= 0.0) && (value <= 1.0)) {
                    _perturbationAmplitude = value;
                }
            }
        }

        public PerturbedErrorFilter(MatrixErrorFilter childFilter) {
            PerturbationAmplitude = 1.0;
            ChildFilter = childFilter;
        }

        public override double getError() {
            return ChildFilter.getError();
        }

        public override void setError(double error) {
            PerturbedMatrix.apply(
                (int y, int x, double coeff) =>
                {
                    ChildFilter.Buffer.setError(y, x, coeff * error);
                }
                );
        }

        public override void moveNext() {
            ChildFilter.moveNext();
            computePerturbation();
        }

        private void preparePerturbationGroups() {
            // Note: this must be called everytime the original matrix changes!

            List<WeightGroup> weights = new List<WeightGroup>();
            // make a temporary list of weights (groups contain single weights)
            OriginalMatrix.apply(
                (int y, int x, double coeff) =>
                {
                    WeightGroup group = new WeightGroup();
                    group.addWeight(new Coordinate<int>(x + OriginalMatrix.SourceOffset, y), coeff);
                    weights.Add(group);
                }
                );
            // sort the weights by their value
            weights.Sort(
                (WeightGroup g1, WeightGroup g2) =>
                {
                    return g1.MaxNoiseAmplitude.CompareTo(g2.MaxNoiseAmplitude);
                }
                );
            // Group the weight to pairs, make a group of thee if there's an odd weight.
            // Each group will carry its minimum value.
            List<WeightGroup>.Enumerator weightIter = weights.GetEnumerator();
            while (weightIter.MoveNext()) {
                WeightGroup group1 = weightIter.Current;
                if (weightIter.MoveNext()) {
                    // bind two weights together
                    WeightGroup group2 = weightIter.Current;
                    group1.addWeight(group2);
                    _weightGroups.Add(group1);
                } else if (weights.Count > 0) {
                    // a space odd weight, bind to to the last group
                    // (it will contain 3 weights)
                    _weightGroups[_weightGroups.Count - 1].addWeight(group1);
                }
                // else: single weight, don't perturb at all
            }
            foreach (WeightGroup group in _weightGroups) {
                foreach(Coordinate<int> coords in group.WeightCoords) {
                }
            }
        }

        private void computePerturbation() {
            foreach (WeightGroup group in _weightGroups) {
                // [-MaxNoiseAmplitude;+MaxNoiseAmplitude]
                double perturbation = group.MaxNoiseAmplitude * (RandomGenerator.NextDouble() * 2 - 1);
                if (group.WeightCoords.Count == 2) {
                    Coordinate<int> coords = group.WeightCoords[0];
                    PerturbedMatrix[coords.Y, coords.X] =
                        OriginalMatrix[coords.Y, coords.X] + perturbation;

                    coords = group.WeightCoords[1];
                    PerturbedMatrix[coords.Y, coords.X] =
                        OriginalMatrix[coords.Y, coords.X] - perturbation;
                } else if (group.WeightCoords.Count == 3) {
                    Coordinate<int> coords = group.WeightCoords[0];
                    PerturbedMatrix[coords.Y, coords.X] =
                        OriginalMatrix[coords.Y, coords.X] + perturbation;

                    perturbation *= 0.5; // perturbation/2
                    for (int i = 1; i <= 2; i++) {
                        coords = group.WeightCoords[i];
                        PerturbedMatrix[coords.Y, coords.X] =
                            OriginalMatrix[coords.Y, coords.X] - perturbation;
                    }
                }
            }
        }

        public override bool initBuffer(
            ScanningOrder scanningOrder, int imageHeight, int imageWidth)
        {
            return ChildFilter.initBuffer(scanningOrder, imageHeight, imageWidth);
        }

        private class WeightGroup
        {
            // maximal allowed noise amplitude = minimal coefficient
            public Double MaxNoiseAmplitude = Double.MaxValue;
            public List<Coordinate<int>> WeightCoords;

            public WeightGroup() {
                WeightCoords = new List<Coordinate<int>>();
            }

            public void addWeight(Coordinate<int> coords, double coeff) {
                MaxNoiseAmplitude = Math.Min(MaxNoiseAmplitude, coeff);
                WeightCoords.Add(coords);
            }

            public void addWeight(WeightGroup otherGroup) {
                if (otherGroup.WeightCoords.Count > 0) {
                    addWeight(otherGroup.WeightCoords[0], otherGroup.MaxNoiseAmplitude);
                }
            }
        }
    }
}
