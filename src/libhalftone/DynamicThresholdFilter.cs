using System;
using System.Collections.Generic;
using System.Linq;
using Gimp;

namespace Halftone
{
    /// <summary>
    /// Matrix threshold filter with multiple matrices for different
    /// intensity ranges and optional random threshold value perturbation.
    /// </summary>
    [Serializable]
    public class DynamicThresholdFilter : ThresholdFilter
    {
        [Serializable]
        class ThresholdTableRecord : IComparable<ThresholdTableRecord>
        {
            public int intensityRangeStart;
            public double noiseAmplitude; // [0.0; 1.0], could be int
            public ThresholdMatrix matrix;

            public int CompareTo(ThresholdTableRecord other) {
                return intensityRangeStart.CompareTo(other.intensityRangeStart);
            }
        }

        public bool NoiseEnabled { get; set; }

        private SortedList<int, ThresholdTableRecord> _thresholdTable;
        [NonSerialized]
        Random _randomGenerator = null;

        private Random RandomGenerator {
            get {
                if (_randomGenerator == null) {
                    _randomGenerator = new Random();
                }
                return _randomGenerator;
            }
        }

        static ThresholdTableRecord defaultRecord = new ThresholdTableRecord()
        {
            intensityRangeStart = 0,
            noiseAmplitude = 0,
            matrix = ThresholdMatrix.Generator.simpleThreshold
        };

        public DynamicThresholdFilter() {
            _thresholdTable = new SortedList<int, ThresholdTableRecord>();
            NoiseEnabled = false;
        }

        protected override int threshold(int intensity, int x, int y) {
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

        public void addThresholdRecord(int intensityRangeStart, ThresholdMatrix matrix) {
            addThresholdRecord(intensityRangeStart, matrix, 0.0);
        }

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

        public void deleteThresholdRecord(int intensityRangeStart) {
            _thresholdTable.Remove(intensityRangeStart);
        }

        public void clearThresholdRecords() {
            _thresholdTable.Clear();
        }

        // TODO: functions to modify records in _thresholdTable
        // Note: such an interface should be available in a Prototype which then
        // creates instances of this class
    }
}
