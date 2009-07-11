using System;
using System.Collections.Generic;
using System.Linq;

namespace Halftone
{
    [Serializable]
    public class DynamicMatrixTable<T>
        where T : DynamicMatrixTable<T>.Record, new()
    {
        [Serializable]
        public abstract class Record : IComparable<Record>
        {
            /// <summary>
            /// Starting intensity of the range (0-255).
            /// </summary>
            public int keyRangeStart;

            public int CompareTo(Record other) {
                return keyRangeStart.CompareTo(other.keyRangeStart);
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
        private SortedList<int, T> _table;

        public DynamicMatrixTable() {
            _table = new SortedList<int, T>();
        }

        /// <summary>
        /// Add a new intensity range record to the table.
        /// </summary>
        /// <remarks>
        /// If there already is a record with the same intensity it is
        /// overwritten.
        /// </remarks>
        /// <param name="intensityRangeStart">Start intensity of the range
        /// (0-255)</param>
        /// <param name="matrix">Error matrix for that range</param>
        public bool addRecord(T record) {
            //if ((record.keyRangeStart < 0) || (record.keyRangeStart > 255)) { return; }
            if (_table.ContainsKey(record.keyRangeStart)) {
                ////move it one step further (if there is a free place)
                //if (!_table.ContainsKey(keyRangeStart + 1)) {
                //    // copy existing record one step further
                //    _table.Add(record.keyRangeStart + 1,
                //        _table[record.keyRangeStart]);
                //}
                // replace the record with new one
                _table[record.keyRangeStart] = record;
                return false;
            } else {
                // add a new record
                _table.Add(record.keyRangeStart, record);
            }
            return true;
        }

        /// <summary>
        /// Get an intensity range record (containing an error matrix)
        /// associated with given pixel intensity.
        /// </summary>
        /// <param name="intensity">Pixel intensity (0-255)</param>
        /// <returns>Proper intensity range record or a default one if the
        /// intensity range table is empty</returns>
        public T getRecord(int key) {
            return getRecord(key, true);
        }

        /// <summary>
        /// Get an intensity range record (containing an error matrix)
        /// associated with given pixel intensity.
        /// </summary>
        /// <param name="intensity">Pixel intensity (0-255)</param>
        /// <param name="getDefault">Replace null with a default record?</param>
        /// <returns>Proper intensity range record or a null if the
        /// intensity range table is empty</returns>
        public T getRecord(int key, bool getDefault) {
            // this is an upper bound, lower bound idea from:
            // http://stackoverflow.com/questions/594518/is-there-a-lower-bound-function-in-c-on-a-sortedlist
            T record = _table.LastOrDefault(
                x => x.Key <= key).Value;
            if ((record == null) && getDefault) {
                record = new T();
            }
            return record;
        }

        /// <summary>
        /// Delete and existing intensity range record.
        /// </summary>
        /// <remarks>
        /// Do nothing if there is no such a record.
        /// </remarks>
        /// <param name="intensityRangeStart">Start intensity of the range
        /// (0-255)</param>
        public void deleteRecord(int key) {
            _table.Remove(key);
        }

        public IEnumerable<T> listRecords() {
            return _table.Values.AsEnumerable();
        }

        /// <summary>
        /// Clear all records.
        /// </summary>
        public void clearRecords() {
            _table.Clear();
        }
    }
}
