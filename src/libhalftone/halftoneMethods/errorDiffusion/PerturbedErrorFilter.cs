using System;
using System.Collections.Generic;
using System.Linq;
using Gimp;

namespace Halftone
{
    /// <summary>
    /// Perturbed error filter acts as a wrapper over a MatrixErrorFilter
    /// and adds random noise (perturbations) to its matrix coefficients.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The sum of all perturbations is ensured to be zero. The amount of
    /// perturbation can be controlled via PerturbationAmplitude property.
    /// </para>
    /// <para>
    /// Coefficients in the original matrix are sorted according to their
    /// value and grouped in pairs. A random perturbation of varying
    /// magnitude is added to one coefficient and subtracted from the other.
    /// Perturbation magnitude depends on the lesser of the two coeficients.
    /// If there is an odd number of coefficients the last perturbation is
    /// divided into a group of three coefficients.
    /// </para>
    /// </remarks>
    [Serializable]
    [Module(TypeName = "Perturbed error filter")]
    public class PerturbedErrorFilter : ErrorFilter
    {
        private MatrixErrorFilter _childFilter;

        /// <summary>
        /// Child matrix error filter containing the original matrix.
        /// </summary>
        public MatrixErrorFilter ChildFilter {
            get {
                if (_childFilter == null) {
                    _childFilter = new MatrixErrorFilter();
                }

                return _childFilter;
            }
            set {
                _childFilter = value;
                //prepare();
            }
        }

        /// <summary>
        /// Groups of coefficients (pairs or possible a group of three).
        /// </summary>
        [NonSerialized]
        private List<CoeffGroup> _coeffGroups;
        
        [NonSerialized]
        private Random _randomGenerator = null;

        /// <summary>
        /// Random generator used for generating the perturbations.
        /// </summary>
        /// <remarks>
        /// It is created when it is first needed.
        /// </remarks>
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

        /// <summary>
        /// Original matrix reference. Coefficient groups are created when
        /// the matrix is set.
        /// </summary>
        private ErrorMatrix OriginalMatrix {
            get {
                return _originalMatrix;
            }
            set {
                _originalMatrix = value;
                preparePerturbationGroups();
            }
        }

        [NonSerialized]
        private ErrorMatrix _perturbedMatrix;
        /// <summary>
        /// Temporary matrix to store pertubed coefficients from the original
        /// matrix.
        /// </summary>
        private ErrorMatrix PerturbedMatrix {
            get {
                return _perturbedMatrix;
            }
            set {
                _perturbedMatrix = value;
            }
        }

        private double _perturbationAmplitude;
        /// <summary>
        /// Amplitude of perturbation (0.0-1.0)
        /// </summary>
        public double PerturbationAmplitude { 
            get { return _perturbationAmplitude; }
            set {
                if ((value >= 0.0) && (value <= 1.0)) {
                    _perturbationAmplitude = value;
                }
            }
        }

        /// <summary>
        /// Create an perturbed error filter over an existing matrix error
        /// filter.
        /// </summary>
        /// <remarks>
        /// Noise amplitude is at its maximum value by default.
        /// </remarks>
        /// <param name="childFilter">Child filter to be perturbed</param>
        public PerturbedErrorFilter(MatrixErrorFilter childFilter) {
            PerturbationAmplitude = 1.0;
            ChildFilter = childFilter;
        }

        /// <summary>
        /// Create a default perturbed error filter.
        /// </summary>
        public PerturbedErrorFilter()
            : this(new MatrixErrorFilter()) {}

        public override double getError() {
            return ChildFilter.getError();
        }

        public override void setError(double error, int intensity) {
            PerturbedMatrix.apply( (int y, int x, double coeff) =>
                {
                    ChildFilter.Buffer.setError(y, x, coeff * error);
                }
            );
        }

        public override void moveNext() {
            ChildFilter.moveNext();
            computePerturbation();
        }

        /// <summary>
        /// Prepare matrices, perturbation groups and compute perturbations.
        /// </summary>
        private void prepare() {
            if (_coeffGroups == null) {
                _coeffGroups = new List<CoeffGroup>();
            } else {
                _coeffGroups.Clear();
            }
            OriginalMatrix = (ErrorMatrix)ChildFilter.Matrix.Clone();
            PerturbedMatrix = (ErrorMatrix)OriginalMatrix.Clone();
            computePerturbation();
        }

        /// <summary>
        /// Distbute coefficients from the OriginalMatrix to perturbation
        /// groups.
        /// </summary>
        private void preparePerturbationGroups() {
            // Note: this must be called everytime the original matrix changes!

            List<CoeffGroup> coeffs = new List<CoeffGroup>();
            // make a temporary list of coeffs (groups contain single coeffs)
            OriginalMatrix.apply((int y, int x, double coeff) =>
                {
                    CoeffGroup group = new CoeffGroup();
                    group.addCoeff(new Coordinate<int>(x + OriginalMatrix.SourceOffsetX, y), coeff);
                    coeffs.Add(group);
                }
            );
            // sort the coeffs by their value
            coeffs.Sort((CoeffGroup g1, CoeffGroup g2) =>
                    { return g1.MaxNoiseAmplitude.CompareTo(
                        g2.MaxNoiseAmplitude); } );
            //// or this way (which is only a little bit slower)
            //coeffs.OrderBy((group) => group.MaxNoiseAmplitude);

            // Group the coefficients to pairs, make a group of three if
            // there's an odd coefficient.
            // Each group will carry its minimum value.
            List<CoeffGroup>.Enumerator coeffIter = coeffs.GetEnumerator();
            while (coeffIter.MoveNext()) {
                CoeffGroup group1 = coeffIter.Current;
                if (coeffIter.MoveNext()) {
                    // bind two coeffs together
                    CoeffGroup group2 = coeffIter.Current;
                    group1.addCoeff(group2);
                    _coeffGroups.Add(group1);
                } else if (coeffs.Count > 0) {
                    // a space odd coeff, bind to to the last group
                    // (it will contain 3 coeffs)
                    if (_coeffGroups.Count > 0) {
                        _coeffGroups[_coeffGroups.Count - 1].addCoeff(group1);
                    }
                    // else: single coeff, don't perturb at all
                }
            }
            //// ???
            //foreach (CoeffGroup group in _coeffGroups) {
            //    foreach(Coordinate<int> coords in group.CoeffCoords) {
            //    }
            //}
        }

        /// <summary>
        /// Perturb OriginalMatrix with results stored to PerturbedMatrix.
        /// </summary>
        private void computePerturbation() {
            foreach (CoeffGroup group in _coeffGroups) {
                // [-MaxNoiseAmplitude;+MaxNoiseAmplitude]
                double perturbation = group.MaxNoiseAmplitude *
                    (RandomGenerator.NextDouble() * 2 - 1);
                if (group.CoeffCoords.Count == 2) {
                    Coordinate<int> coords = group.CoeffCoords[0];
                    PerturbedMatrix[coords.Y, coords.X] =
                        OriginalMatrix[coords.Y, coords.X] + perturbation;

                    coords = group.CoeffCoords[1];
                    PerturbedMatrix[coords.Y, coords.X] =
                        OriginalMatrix[coords.Y, coords.X] - perturbation;
                } else if (group.CoeffCoords.Count == 3) {
                    Coordinate<int> coords = group.CoeffCoords[0];
                    PerturbedMatrix[coords.Y, coords.X] =
                        OriginalMatrix[coords.Y, coords.X] + perturbation;

                    perturbation *= 0.5; // perturbation/2
                    for (int i = 1; i <= 2; i++) {
                        coords = group.CoeffCoords[i];
                        PerturbedMatrix[coords.Y, coords.X] =
                            OriginalMatrix[coords.Y, coords.X] - perturbation;
                    }
                }
            }
        }

        public override void init(Image.ImageRunInfo imageRunInfo) {
            base.init(imageRunInfo);
            ChildFilter.init(imageRunInfo);
            prepare();
            Initialized = ChildFilter.Initialized;
        }

        /// <summary>
        /// Group of coefficients along with maximum allowed noise amplidute.
        /// </summary>
        private class CoeffGroup
        {
            /// <summary>
            /// Maximal allowed noise amplitude = minimal coefficient value.
            /// </summary>
            public Double MaxNoiseAmplitude = Double.MaxValue;
            /// <summary>
            /// List of coefficient coordinates.
            /// </summary>
            /// <remarks>
            /// Values are not relevant here. Moreover they are stored
            /// in the OriginalMatrix.
            /// </remarks>
            public List<Coordinate<int>> CoeffCoords;

            /// <summary>
            /// Create a coefficient group.
            /// </summary>
            public CoeffGroup() {
                CoeffCoords = new List<Coordinate<int>>();
            }

            /// <summary>
            /// Add a coefficient to the group.
            /// </summary>
            /// <param name="coords">Coefficient coordinates</param>
            /// <param name="coeff">Coefficient value</param>
            public void addCoeff(Coordinate<int> coords, double coeff) {
                MaxNoiseAmplitude = Math.Min(MaxNoiseAmplitude, coeff);
                CoeffCoords.Add(coords);
            }

            /// <summary>
            /// Add coefficients from another group.
            /// </summary>
            /// <param name="otherGroup">Other group of coefficients</param>
            public void addCoeff(CoeffGroup otherGroup) {
                if (otherGroup.CoeffCoords.Count > 0) {
                    addCoeff(otherGroup.CoeffCoords[0],
                        otherGroup.MaxNoiseAmplitude);
                }
            }
        }
    }
}
