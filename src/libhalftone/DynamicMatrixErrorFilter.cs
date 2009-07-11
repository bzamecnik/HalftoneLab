using System;
using System.Collections.Generic;
using System.Linq;

namespace Halftone
{
    // TODO:
    // - the table should be a fixed size array of records (not a list)
    //   - it is presumed that the each intensity in the table should have one record

    /// <summary>
    /// Error-diffusion filter with matrices dynamically selected depending
    /// upon source pixel intensity.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Each intensity or intensity range can have its own error matrix. This
    /// is done before running the halftone algorithm using addRecord() or
    /// deleteRecord() functions.
    /// </para>
    /// <para>
    /// The whole range of intensities is divided into ranges. There is a
    /// matrix for each of thoes ranges. Each range is given by its lower
    /// intensity
    /// </para>
    /// </remarks>
    [Serializable]
    public class DynamicMatrixErrorFilter
        : MatrixErrorFilter, DynamicErrorFilter
    {
        public DynamicMatrixTable<ErrorRecord> MatrixTable { get; set; }

        /// <summary>
        /// Intensity range record containing error matrix and starting
        /// intensity of the range.
        /// </summary>
        /// <remarks>
        /// It is comparable to support sorting a table of these records
        /// and searching there.
        /// </remarks>
        [Serializable]
        public class ErrorRecord : DynamicMatrixTable<ErrorRecord>.Record
        {
            // TODO: use properties

            public ErrorMatrix matrix;

            public ErrorRecord(int intensityRangeStart,
                ErrorMatrix matrix) {
                this.keyRangeStart = intensityRangeStart;
                this.matrix = matrix;
            }

            public ErrorRecord()
                : this(0, ErrorMatrix.Samples.Default) { }
        }

        /// <summary>
        /// Create a dynamic matrix error filter.
        /// </summary>
        public DynamicMatrixErrorFilter() {
            MatrixTable = new DynamicMatrixTable<ErrorRecord>();
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
            // TODO: intensity should be clipped here to 0-255 range!
            setMatrix(MatrixTable.getRecord(intensity).matrix, false);
            base.setError(error);
        }

        int debugCount = 0;

        public override void init(Image.ImageRunInfo imageRunInfo) {
            //base.init(imageRunInfo);
            int maxMatrixHeight = 0;
            foreach (ErrorRecord record in MatrixTable.listRecords()) {
                record.matrix.init(imageRunInfo);
                maxMatrixHeight = Math.Max(maxMatrixHeight,
                    record.matrix.Height);
            }
            Buffer = ErrorBuffer.createFromScanningOrder(
                imageRunInfo.ScanOrder, maxMatrixHeight,
                imageRunInfo.Width) as MatrixErrorBuffer;
            Initialized = Buffer != null;
        }
    }
}
