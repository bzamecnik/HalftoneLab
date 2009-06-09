using System;
using System.Collections.Generic;
using Gimp;

namespace Halftone
{
    public class PerturbedErrorFilter : ErrorFilter
    {
        private MatrixErrorFilter _childFilter;
        private ErrorMatrix _originalMatrix;
        private ErrorMatrix _perturbedMatrix; // temporary
        private List<WeightGroup> _weightGroups;
        
        private Random _randomGenerator = null;
        private Random RandomGenerator {
            get {
                if (_randomGenerator == null) {
                    _randomGenerator = new Random();
                }
                return _randomGenerator;
            }
        }

        private double _perturbationAmplitude;
        // TODO: serializable
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
            _weightGroups = new List<WeightGroup>();
            _childFilter = childFilter;
            _originalMatrix = _childFilter.ErrorMatrix;
            // Note: Creating an empty matrix with the same size and source offset
            // as in original matrix would be enough. The values are overwritten
            // in computePerturbation() anyway.
            _perturbedMatrix = (ErrorMatrix)_originalMatrix.Clone();
            preparePerturbationGroups();
            computePerturbation();
        }

        public override double getError() {
            return _childFilter.getError();
        }

        public override void setError(double error) {
            _perturbedMatrix.apply(
                (int y, int x, double coeff) =>
                {
                    _childFilter.Buffer.setError(y, x, coeff * error);
                }
                );
        }

        public override void moveNext() {
            _childFilter.moveNext();
            computePerturbation();
        }

        private void preparePerturbationGroups() {
            // Note: this must be called everytime the original matrix changes!

            List<WeightGroup> weights = new List<WeightGroup>();
            // make a temporary list of weights (groups contain single weights)
            _originalMatrix.apply(
                (int y, int x, double coeff) =>
                {
                    WeightGroup group = new WeightGroup();
                    group.addWeight(new Coordinate<int>(x + _originalMatrix.SourceOffset, y), coeff);
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
                    _perturbedMatrix[coords.Y, coords.X] =
                        _originalMatrix[coords.Y, coords.X] + perturbation;

                    coords = group.WeightCoords[1];
                    _perturbedMatrix[coords.Y, coords.X] =
                        _originalMatrix[coords.Y, coords.X] - perturbation;
                } else if (group.WeightCoords.Count == 3) {
                    Coordinate<int> coords = group.WeightCoords[0];
                    _perturbedMatrix[coords.Y, coords.X] =
                        _originalMatrix[coords.Y, coords.X] + perturbation;

                    perturbation *= 0.5; // perturbation/2
                    for (int i = 1; i <= 2; i++) {
                        coords = group.WeightCoords[i];
                        _perturbedMatrix[coords.Y, coords.X] =
                            _originalMatrix[coords.Y, coords.X] - perturbation;
                    }
                }
            }
        }

        public override void initBuffer(
            ScanningOrder scanningOrder, int imageHeight, int imageWidth)
        {
            _childFilter.initBuffer(scanningOrder, imageHeight, imageWidth);
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
