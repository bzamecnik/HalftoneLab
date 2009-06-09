using System;
using System.Collections.Generic;
using System.Linq;

namespace Halftone
{
    // TODO:
    // - join common code of DynamicMatrixErrorFilter and DynamicTresholdFilter
    // - the table should be a fixed size array of records (not a list)
    //   - it is presumed that the each intensity in the table should have one record

    public class DynamicMatrixErrorFilter : MatrixErrorFilter, DynamicErrorFilter
    {
        class ErrorTableRecord : IComparable<ErrorTableRecord>
        {
            public int intensityRangeStart;
            public ErrorMatrix matrix;

            public int CompareTo(ErrorTableRecord other) {
                return intensityRangeStart.CompareTo(other.intensityRangeStart);
            }
        }

        SortedList<int, ErrorTableRecord> _recordTable;

        static ErrorTableRecord defaultRecord = new ErrorTableRecord()
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
            ErrorMatrix = getTresholdRecord(intensity).matrix;
            base.setError(error);
        }

        ErrorTableRecord getTresholdRecord(int intensity) {
            // this is an upper bound, lower bound idea from:
            // http://stackoverflow.com/questions/594518/is-there-a-lower-bound-function-in-c-on-a-sortedlist
            ErrorTableRecord record = _recordTable.LastOrDefault(
                x => x.Key <= intensity).Value;
            if (record == null) {
                record = defaultRecord;
            }
            return record;
        }

        public void addTresholdRecord(int intensityRangeStart, ErrorMatrix matrix) {
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

        public void deleteTresholdRecord(int intensityRangeStart) {
            _recordTable.Remove(intensityRangeStart);
        }
    }
}
