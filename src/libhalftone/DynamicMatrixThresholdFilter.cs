using System;
using System.Collections.Generic;
using System.Linq;

namespace Halftone
{
    /// <summary>
    /// Matrix threshold filter with multiple matrices for different
    /// intensity ranges and optional random threshold value perturbation.
    /// </summary>
    /// <see cref="DynamicMatrixErrorFilter"/>
    [Serializable]
    public class DynamicMatrixThresholdFilter : ThresholdFilter
    {
        /// <summary>
        /// Intensity range record containing threshold matrix, starting
        /// intensity of the range and perturbation noise amplitude
        /// </summary>
        /// <remarks>
        /// It is comparable to support sorting a table of these records
        /// and searching there.
        /// </remarks>
        [Serializable]
        class ThresholdTableRecord : IComparable<ThresholdTableRecord>
        {
            /// <summary>
            /// Starting intensity of the range (0-255).
            /// </summary>
            public int intensityRangeStart;
            /// <summary>
            /// Perturbation noise amplitude (0.0-1.0).
            /// </summary>
            public double noiseAmplitude; // TODO: it could be integer
            /// <summary>
            /// Threshold matrix for that range.
            /// </summary>
            public ThresholdMatrix matrix;

            public int CompareTo(ThresholdTableRecord other) {
                return intensityRangeStart.CompareTo(other.intensityRangeStart);
            }
        }

        /// <summary>
        /// Is perturbation of threshold values enabled?
        /// </summary>
        public bool NoiseEnabled { get; set; }

        /// <summary>
        /// Table of intensity range records.
        /// </summary>
        private SortedList<int, ThresholdTableRecord> _thresholdTable;

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
        /// Default intensity range record spanning the whole intensity
        /// range, containing a default threshold matrix and no noise.
        /// </summary>
        /// <remarks>
        /// It can be used when the intensity range table is empty.
        /// </remarks>
        static ThresholdTableRecord defaultRecord = new ThresholdTableRecord()
        {
            intensityRangeStart = 0,
            noiseAmplitude = 0,
            matrix = ThresholdMatrix.Generator.simpleThreshold
        };

        /// <summary>
        /// Create a dynamic matrix threshold filter.
        /// </summary>
        public DynamicMatrixThresholdFilter() {
            _thresholdTable = new SortedList<int, ThresholdTableRecord>();
            NoiseEnabled = false;
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
            // TODO: intensity should be clipped here to 0-255 range!
            ThresholdTableRecord record = getThresholdRecord(intensity);
            ThresholdMatrix matrix = record.matrix;
            int threshold = matrix[x, y];
            if (NoiseEnabled) {
                // add noise from interval [-amplitude;amplitude)
                // TODO: find out maximum absolute noise amplitude
                threshold += (int)((RandomGenerator.NextDouble() - 0.5) * record.noiseAmplitude * 127);
            }
            return threshold;
        }

        /// <summary>
        /// Get an intensity range record (containing a threshold matrix)
        /// associated with given pixel intensity.
        /// </summary>
        /// <param name="intensity">Pixel intensity (0-255)</param>
        /// <returns>Proper intensity range record of a default one if the
        /// intensity range table is empty</returns>
        ThresholdTableRecord getThresholdRecord(int intensity) {
            // this is an upper bound, lower bound idea from:
            // http://stackoverflow.com/questions/594518/is-there-a-lower-bound-function-in-c-on-a-sortedlist
            ThresholdTableRecord record = _thresholdTable.LastOrDefault(
                x => x.Key <= intensity).Value;
            if (record == null) {
                record = defaultRecord;
            }
            return record;
        }

        /// <summary>
        /// Add a new intensity range record to the table. Set no noise.
        /// </summary>
        /// <param name="intensityRangeStart">Start intensity of the range
        /// (0-255)</param>
        /// <param name="matrix"></param>
        public void addThresholdRecord(int intensityRangeStart, ThresholdMatrix matrix) {
            addThresholdRecord(intensityRangeStart, matrix, 0.0);
        }

        /// <summary>
        /// Add a new intensity range record to the table.
        /// </summary>
        /// <remarks>
        /// If there already is a record with the same intensity move it one
        /// step further (if there is a free place), otherwise it is
        /// overwritten.
        /// </remarks>
        /// <param name="intensityRangeStart">Start intensity of the range
        /// (0-255)</param>
        /// <param name="matrix">Threshold matrix for that range</param>
        /// <param name="noiseAmplitude">Noise amplidute (0.0-1.0)</param>
        public void addThresholdRecord(
            int intensityRangeStart,
            ThresholdMatrix matrix,
            double noiseAmplitude) {
            //if ((intensityRangeStart < 0) || (intensityRangeStart > 255)) { return; }
            ThresholdTableRecord newRecord = new ThresholdTableRecord()
            {
                intensityRangeStart = intensityRangeStart,
                matrix = matrix,
                noiseAmplitude = noiseAmplitude
            };
            if (_thresholdTable.ContainsKey(intensityRangeStart)) {
                if (!_thresholdTable.ContainsKey(intensityRangeStart + 1)) {
                    // copy existing record one step further
                    _thresholdTable.Add(intensityRangeStart + 1,
                        _thresholdTable[intensityRangeStart]);
                }
                // replace the record with new one
                _thresholdTable[intensityRangeStart] = newRecord;
            } else {
                // add a new record
                _thresholdTable.Add(intensityRangeStart, newRecord);
            }
        }

        /// <summary>
        /// Delete and existing intensity range record.
        /// </summary>
        /// <remarks>
        /// Do nothing if there is no such a record.
        /// </remarks>
        /// <param name="intensityRangeStart">Start intensity of the range
        /// (0-255)</param>
        public void deleteThresholdRecord(int intensityRangeStart) {
            _thresholdTable.Remove(intensityRangeStart);
        }

        /// <summary>
        /// Clear all intensity range records.
        /// </summary>
        public void clearThresholdRecords() {
            _thresholdTable.Clear();
        }
    }
}
