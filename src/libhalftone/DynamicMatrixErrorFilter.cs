using System;
using System.Collections.Generic;
using System.Linq;

namespace Halftone
{
    // TODO:
    // - join common code of DynamicMatrixErrorFilter and DynamicThresholdFilter
    // - the table should be a fixed size array of records (not a list)
    //   - it is presumed that the each intensity in the table should have one record

    /// <summary>
    /// Error-diffusion filter with matrices dynamically selected depending
    /// upon source pixel intensity.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Each intensity or intensity range can have its own error matrix. This
    /// is done before running the dither algorithm using addRecord() or
    /// deleteRecord() functions.
    /// </para>
    /// <para>
    /// The whole range of intensities is divided into ranges. There is a
    /// matrix for each of thoes ranges. Each range is given by its lower
    /// intensity
    /// </para>
    /// </remarks>
    [Serializable]
    public class DynamicMatrixErrorFilter : MatrixErrorFilter, DynamicErrorFilter
    {
        /// <summary>
        /// Intensity range record containing error matrix and starting
        /// intensity of the range.
        /// </summary>
        /// <remarks>
        /// It is comparable to support sorting a table of these records
        /// and searching there.
        /// </remarks>
        [Serializable]
        class ErrorTableRecord : IComparable<ErrorTableRecord>
        {
            public int intensityRangeStart;
            public ErrorMatrix matrix;

            public int CompareTo(ErrorTableRecord other) {
                return intensityRangeStart.CompareTo(other.intensityRangeStart);
            }
        }

        /// <summary>
        /// Table of intensity range records.
        /// </summary>
        /// <remarks>
        /// <para>
        /// The table is sorted to make searching there faster. The assumption
        /// is that records in the table will be set and changed rarely but
        /// accessed very intensively.
        /// </para>
        /// <para>
        /// Probably better approach would be to use plain fixed-size array
        /// instad of a list. On the other size using a sorted list is simpler.
        /// </para>
        /// </remarks>
        private SortedList<int, ErrorTableRecord> _recordTable;

        /// <summary>
        /// Default intensity range record spanning the whole intensity range
        /// and containing a default error matrix.
        /// </summary>
        /// <remarks>
        /// It can be used when the intensity range table is empty.
        /// </remarks>
        private static ErrorTableRecord _defaultRecord = new ErrorTableRecord()
        {
            intensityRangeStart = 0,
            matrix = ErrorMatrix.Samples.Default
        };

        /// <summary>
        /// Create a dynamic matrix error filter.
        /// </summary>
        public DynamicMatrixErrorFilter() {
            _recordTable = new SortedList<int, ErrorTableRecord>();
        }

        /// <summary>
        /// Without intensity parameter we don't know which error matrix to
        /// select, thus setError(double error) is made inaccessible.
        /// </summary>
        /// <param name="error"></param>
        private new void setError(double error) { }

        /// <summary>
        /// Set error according to a matrix dynamically selected depending
        /// upon pixel intensity.
        /// </summary>
        /// <param name="error">Quantization error value</param>
        /// <param name="intensity">Source pixel intensity</param>
        public void setError(double error, int intensity) {
            Matrix = getRecord(intensity).matrix;
            base.setError(error);
        }

        /// <summary>
        /// Get proper error matrix given a pixel intensity.
        /// </summary>
        /// <param name="intensity">Pixel intensity (0-255)</param>
        /// <returns>Proper intensity range record of a default one if the
        /// intensity range table is empty</returns>
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

        /// <summary>
        /// Add a new intensity range record to the table.
        /// </summary>
        /// <remarks>
        /// If there already is a record with the same intensity move it one
        /// step further (if there is a free place), otherwise it is
        /// overwritten.
        /// </remarks>
        /// <param name="intensityRangeStart">Start intensity of the range</param>
        /// <param name="matrix">Error matrix for that range</param>
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

        /// <summary>
        /// Delete and existing intensity range record.
        /// </summary>
        /// <remarks>
        /// Do nothing if there is no such a record.
        /// </remarks>
        /// <param name="intensityRangeStart">Start intensity of the range</param>
        public void deleteRecord(int intensityRangeStart) {
            _recordTable.Remove(intensityRangeStart);
        }
    }
}
