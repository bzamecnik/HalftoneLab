// Copyright (c) 2009 Bohumir Zamecnik <bohumir@zamecnik.org>
// License: The MIT License, see the LICENSE file

using System;
using System.Collections.Generic;
using System.Linq;

namespace HalftoneLab
{
    /// <summary>
    /// Matrix threshold filter with multiple matrices for different
    /// intensity ranges and optional random threshold value perturbation.
    /// </summary>
    /// <see cref="DynamicMatrixErrorFilter"/>
    /// <see cref="DynamicMatrixTable"/>
    [Serializable]
    [Module(TypeName = "Dynamic matrix threshold filter")]
    public class DynamicMatrixThresholdFilter : ThresholdFilter
    {
        /// <summary>
        /// A table of intensity range records.
        /// </summary>
        public DynamicMatrixTable<ThresholdRecord> MatrixTable { get; set; }

        /// <summary>
        /// Intensity range record containing threshold matrix, starting
        /// intensity of the range and perturbation noise amplitude
        /// </summary>
        [Serializable]
        public class ThresholdRecord
            : DynamicMatrixTable<ThresholdRecord>.Record
        {
            /// <summary>
            /// Perturbation noise amplitude (0.0-1.0).
            /// </summary>
            public double noiseAmplitude; // TODO: it could be integer

            /// <summary>
            /// Threshold matrix for that range.
            /// </summary>
            public ThresholdMatrix matrix;

            /// <summary>
            /// Create a new record.
            /// </summary>
            /// <param name="intensityRangeStart">Starting intensity (0-255)
            /// </param>
            /// <param name="matrix">Threshold matrix</param>
            /// <param name="noiseAmplitude">Noise amplitude (0.0-1.0)</param>
            public ThresholdRecord(int intensityRangeStart,
                ThresholdMatrix matrix, double noiseAmplitude) {
                this.keyRangeStart = intensityRangeStart;
                this.matrix = (ThresholdMatrix)matrix.Clone();
                this.noiseAmplitude = noiseAmplitude;
            }

            /// <summary>
            /// Create a record with no noise.
            /// </summary>
            /// <param name="intensityRangeStart">Starting intensity (0-255)
            /// </param>
            /// <param name="matrix">Threshold matrix</param>
            public ThresholdRecord(int intensityRangeStart,
                ThresholdMatrix matrix)
                : this(intensityRangeStart, matrix, 0) {}

            /// <summary>
            /// Create a default record spanning the whole intensity range,
            /// containing a default threshold matrix and no noise.
            /// </summary>
            public ThresholdRecord()
                : this(0, new ThresholdMatrix()) { }

            public override void init(Image.ImageRunInfo imageRunInfo) {
                base.init(imageRunInfo);
                matrix.init(imageRunInfo);
            }
        }

        /// <summary>
        /// Is perturbation of threshold values enabled?
        /// </summary>
        public bool NoiseEnabled { get; set; }

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
        /// Create a dynamic matrix threshold filter.
        /// </summary>
        public DynamicMatrixThresholdFilter() {
            MatrixTable = new DynamicMatrixTable<ThresholdRecord>();
            NoiseEnabled = true;
        }

        /// <summary>
        /// Get threshold from matrix associated with given intensity.
        /// Optionally add a random perturbation.
        /// </summary>
        /// <remarks>
        /// Perturbation adds white noise from interval
        /// [-amplitude;amplitude) to the threshold value. Noise amplidute
        /// is taken from current intensity range record.
        /// </remarks>
        /// <param name="intensity">Pixel intensity (0-255)</param>
        /// <param name="x">Pixel X coordinate (> 0)</param>
        /// <param name="y">Pixel Y coordinate (> 0)</param>
        /// <returns></returns>
        protected override int threshold(int intensity, int x, int y) {
            // intensity for dynamic table is clipped here to 0-255 range
            ThresholdRecord record = MatrixTable.getWorkingRecord(
                Math.Max(Math.Min(intensity, 255), 0));
            ThresholdMatrix matrix = record.matrix;
            int threshold = matrix[x, y];
            if (NoiseEnabled) {
                // add noise from interval [-amplitude;amplitude)
                // TODO: find out maximum absolute noise amplitude
                threshold += (int)((RandomGenerator.NextDouble() - 0.5) *
                    record.noiseAmplitude * 255);
            }
            return threshold;
        }

        public override void init(Image.ImageRunInfo imageRunInfo) {
            base.init(imageRunInfo);
            MatrixTable.init(imageRunInfo);
        }
    }
}
