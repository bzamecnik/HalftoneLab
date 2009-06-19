using System;
using System.Collections.Generic;
using System.Linq;
using Gimp;

namespace Halftone
{
    [Serializable]
    public class DynamicTresholdFilter : TresholdFilter
    {
        [Serializable]
        class TresholdTableRecord : IComparable<TresholdTableRecord>
        {
            public int intensityRangeStart;
            public double noiseAmplitude; // [0.0; 1.0]could be int
            public TresholdMatrix matrix;

            public int CompareTo(TresholdTableRecord other) {
                return intensityRangeStart.CompareTo(other.intensityRangeStart);
            }
        }

        public bool NoiseEnabled { get; set; }

        private SortedList<int, TresholdTableRecord> _tresholdTable;
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

        static TresholdTableRecord defaultRecord = new TresholdTableRecord()
        {
            intensityRangeStart = 0,
            noiseAmplitude = 0,
            matrix = TresholdMatrix.Generator.simpleTreshold
        };

        public DynamicTresholdFilter() {
            _tresholdTable = new SortedList<int, TresholdTableRecord>();
            NoiseEnabled = false;
        }

        protected override int treshold(int intensity, int x, int y) {
            TresholdTableRecord record = getTresholdRecord(intensity);
            TresholdMatrix matrix = record.matrix;
            int treshold = matrix[x, y];
            if (NoiseEnabled) {
                // add noise from interval [-amplitude;amplitude)
                // TODO: find out maximum absolute noise amplitude
                treshold += (int)((RandomGenerator.NextDouble() - 0.5) * record.noiseAmplitude * 127);
            }
            return treshold;
        }

        TresholdTableRecord getTresholdRecord(int intensity) {
            // this is an upper bound, lower bound idea from:
            // http://stackoverflow.com/questions/594518/is-there-a-lower-bound-function-in-c-on-a-sortedlist
            TresholdTableRecord record = _tresholdTable.LastOrDefault(
                x => x.Key <= intensity).Value;
            if (record == null) {
                record = defaultRecord;
            }
            return record;
        }

        public void addTresholdRecord(int intensityRangeStart, TresholdMatrix matrix) {
            addTresholdRecord(intensityRangeStart, matrix, 0.0);
        }

        public void addTresholdRecord(
            int intensityRangeStart,
            TresholdMatrix matrix,
            double noiseAmplitude) {
            //if ((intensityRangeStart < 0) || (intensityRangeStart > 255)) { return; }
            TresholdTableRecord newRecord = new TresholdTableRecord()
            {
                intensityRangeStart = intensityRangeStart,
                matrix = matrix,
                noiseAmplitude = noiseAmplitude
            };
            if (_tresholdTable.ContainsKey(intensityRangeStart)) {
                if (!_tresholdTable.ContainsKey(intensityRangeStart + 1)) {
                    // copy existing record one step further
                    _tresholdTable.Add(intensityRangeStart + 1,
                        _tresholdTable[intensityRangeStart]);
                }
                // replace the record with new one
                _tresholdTable[intensityRangeStart] = newRecord;
            } else {
                // add a new record
                _tresholdTable.Add(intensityRangeStart, newRecord);
            }
        }

        public void deleteTresholdRecord(int intensityRangeStart) {
            _tresholdTable.Remove(intensityRangeStart);
        }

        public void clearTresholdRecords() {
            _tresholdTable.Clear();
        }

        // TODO: functions to modify records in _tresholdTable
        // Note: such an interface should be available in a Prototype which then
        // creates instances of this class
    }
}
