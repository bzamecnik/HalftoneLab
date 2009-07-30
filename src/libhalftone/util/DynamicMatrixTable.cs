using System;
using System.Collections.Generic;
using System.Linq;

namespace HalftoneLab
{
    /// <summary>
    /// A table of tone-dependent matrices.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The idea is to select a proper error or threshold matrix for each
    /// intensity. The whole intensity range is divided into subranges. Each
    /// subrange has its own record and spans between its starting intensity
    /// and the start of next record of the end of the whole range.
    /// </para>
    /// <para>
    /// The table is definition is done in design-time using functions:
    /// addDefinitionRecord(), deleteDefinitionRecord(),
    /// clearDefinitionRecords(), getDefinitionRecord(). Then before runtime
    /// when the table is initialized via init() function, it is for
    /// efficiency converted to a fixed sized array, which is then accessible
    /// via getWorkingRecord() function.
    /// </para>
    /// <para>
    /// Record abstract class provides a skeleton of a table record, it must
    /// be implemented elsewhere.
    /// </para>
    /// </remarks>
    /// <typeparam name="T">Type of table records</typeparam>
    [Serializable]
    public class DynamicMatrixTable<T> : Module
        where T : DynamicMatrixTable<T>.Record, new()
    {
        /// <summary>
        /// Intensity range record skeleton containing only intensity
        /// of the range start.
        /// </summary>
        /// <remarks>
        /// It implements IComparable to support sorting a table of these
        /// records and searching there.
        /// </remarks>
        [Serializable]
        public abstract class Record : Module, IComparable<Record>
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
        private SortedList<int, T> _definitionTable;
        private T[] _workingTable;

        /// <summary>
        /// Default table record which will be used if the table is empty.
        /// </summary>
        public T DefaultRecord { get; protected set; }

        /// <summary>
        /// Create a new dynamic matrix table.
        /// </summary>
        public DynamicMatrixTable() {
            _definitionTable = new SortedList<int, T>();
            _workingTable = new T[256]; // intensity range
            DefaultRecord = new T();
        }

        /// <summary>
        /// Add a new intensity range record to the table.
        /// </summary>
        /// <remarks>
        /// If there already is a record with the same intensity it is
        /// overwritten.
        /// </remarks>
        /// <param name="record">Intensity range record</param>
        public bool addDefinitionRecord(T record) {
            //if ((record.keyRangeStart < 0) || (record.keyRangeStart > 255)) { return; }
            if (_definitionTable.ContainsKey(record.keyRangeStart)) {
                ////move it one step further (if there is a free place)
                //if (!_definitionTable.ContainsKey(keyRangeStart + 1)) {
                //    // copy existing record one step further
                //    _definitionTable.Add(record.keyRangeStart + 1,
                //        _definitionTable[record.keyRangeStart]);
                //}
                // replace the record with new one
                _definitionTable[record.keyRangeStart] = record;
                return false;
            } else {
                // add a new record
                _definitionTable.Add(record.keyRangeStart, record);
            }
            return true;
        }

        /// <summary>
        /// Get an intensity range record (containing an error matrix)
        /// associated with given pixel intensity.
        /// </summary>
        /// <param name="key">Record range start as intensity (0-255)</param>
        /// <returns>Proper intensity range record or a default one if the
        /// intensity range table is empty</returns>
        public T getDefinitionRecord(int key) {
            return getDefinitionRecord(key, true);
        }

        /// <summary>
        /// Get an intensity range record (containing an error matrix)
        /// associated with given pixel intensity.
        /// </summary>
        /// <param name="key">Record range start as intensity (0-255)</param>
        /// <param name="getDefault">Replace null with a default record?
        /// </param>
        /// <returns>Proper intensity range record or a null if the
        /// intensity range table is empty</returns>
        public T getDefinitionRecord(int key, bool getDefault) {
            // this is an upper bound, lower bound idea from:
            // http://stackoverflow.com/questions/594518/is-there-a-lower-bound-function-in-c-on-a-sortedlist
            T record = _definitionTable.LastOrDefault(
                x => x.Key <= key).Value;
            if ((record == null) && getDefault) {
                record = DefaultRecord;
            }
            return record;
        }

        /// <summary>
        /// Get a table record for given intensity.
        /// </summary>
        /// <remarks>
        /// To use this functions the table must be initialized via init()
        /// function.
        /// </remarks>
        /// <param name="key"></param>
        /// <returns></returns>
        public T getWorkingRecord(int key) {
            return _workingTable[key];
        }

        /// <summary>
        /// Delete and existing intensity range record.
        /// </summary>
        /// <remarks>
        /// Do nothing if there is no such a record.
        /// </remarks>
        /// <param name="key">Start intensity of the range (0-255)</param>
        public void deleteDefinitionRecord(int key) {
            _definitionTable.Remove(key);
        }

        /// <summary>
        /// Iterate over all table definition records.
        /// </summary>
        /// <returns>Enumerable collection of table records.</returns>
        public IEnumerable<T> listDefinitionRecords() {
            return _definitionTable.Values.AsEnumerable();
        }

        /// <summary>
        /// Clear all records.
        /// </summary>
        public void clearDefinitionRecords() {
            _definitionTable.Clear();
        }

        /// <summary>
        /// Fill the working table (array) with the records from table
        /// definition to enable efficient retrieval of records.
        /// </summary>
        private void computeWorkingTable() {
            for (int i = 0; i < 256; i++) {
                _workingTable[i] = getDefinitionRecord(i, true);
            }
        }

        public override void init(Image.ImageRunInfo imageRunInfo) {
            base.init(imageRunInfo);
            foreach (Record record in _definitionTable.Values) {
                record.init(imageRunInfo);
            }
            DefaultRecord.init(imageRunInfo);
            computeWorkingTable();
        }
    }
}
