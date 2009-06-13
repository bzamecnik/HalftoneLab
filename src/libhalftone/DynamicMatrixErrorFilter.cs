using System;
using System.Collections.Generic;
using System.Linq;

namespace Halftone
{
    // TODO:
    // - join common code of DynamicMatrixErrorFilter and DynamicTresholdFilter
    // - the table should be a fixed _size array of records (not a list)
    //   - it is presumed that the each intensity in the table should have one record

    [Serializable]
    public class DynamicMatrixErrorFilter : MatrixErrorFilter, DynamicErrorFilter
    {
        [Serializable]
        class ErrorTableRecord : IComparable<ErrorTableRecord>
        {
            public int intensityRangeStart;
            public ErrorMatrix matrix;

            public int CompareTo(ErrorTableRecord other) {
                return intensityRangeStart.CompareTo(other.intensityRangeStart);
            }
        }

        private SortedList<int, ErrorTableRecord> _recordTable;

        private static ErrorTableRecord _defaultRecord = new ErrorTableRecord()
        {
            intensityRangeStart = 0,
            matrix = ErrorMatrix.Samples.Default
        };

        public DynamicMatrixErrorFilter() {
            _recordTable = new SortedList<int, ErrorTableRecord>();
        }

        // make setError(double error) inaccessible
        private new void setError(double error) { }

        public void setError(double error, int intensity) {
            ErrorMatrix = getRecord(intensity).matrix;
            base.setError(error);
        }

        ErrorTableRecord getRecord(int intensity) {
            // this is an upper bound, lower bound idea from:
            // http://stackoverflow.com/questions/594518/is-there-a-lower-bound-function-in-c-on-a-sortedlist
            ErrorTableRecord record = _recordTable.LastOrDefault(
                x => x.Key <= intensity).Value;
            if (record == null) {
                record = _defaultRecord;
            }
            return record;
        }

        public void addRecord(int intensityRangeStart, ErrorMatrix matrix) {
            //if ((intensityRangeStart < 0) || (intensityRangeStart > 255)) { return; }
            ErrorTableRecord newRecord = new ErrorTableRecord()
            {
                intensityRangeStart = intensityRangeStart,
                matrix = matrix,
            };
            if (_recordTable.ContainsKey(intensityRangeStart)) {
                if (!_recordTable.ContainsKey(intensityRangeStart + 1)) {
                    // copy existing record one step further
                    _recordTable.Add(intensityRangeStart + 1,
                        _recordTable[intensityRangeStart]);
                }
                // replace the record with new one
                _recordTable[intensityRangeStart] = newRecord;
            } else {
                // add a new record
                _recordTable.Add(intensityRangeStart, newRecord);
            }
        }

        public void deleteRecord(int intensityRangeStart) {
            _recordTable.Remove(intensityRangeStart);
        }
    }
}
