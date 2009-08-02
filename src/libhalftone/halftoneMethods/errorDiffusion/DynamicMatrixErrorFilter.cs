// Copyright (c) 2009 Bohumir Zamecnik <bohumir@zamecnik.org>
// License: The MIT License, see the LICENSE file

using System;
using System.Collections.Generic;
using System.Linq;

namespace HalftoneLab
{
    /// <summary>
    /// Error-diffusion filter with matrices dynamically selected depending
    /// on source pixel intensity.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Each intensity or intensity range can have its own error matrix. This
    /// can be defined before running the halftone algorithm using MatrixTable.
    /// </para>
    /// <para>
    /// The whole range of intensities is divided into ranges. There is a
    /// matrix for each of thoes ranges. Each range is defined by its starting
    /// (lowest) intensity.
    /// </para>
    /// </remarks>
    /// <see cref="DynamicMatrixTable"/>
    [Serializable]
    [Module(TypeName = "Dynamic matrix error filter")]
    public class DynamicMatrixErrorFilter : MatrixErrorFilter
    {
        /// <summary>
        /// A table of intensity range records.
        /// </summary>
        public DynamicMatrixTable<ErrorRecord> MatrixTable { get; set; }

        /// <summary>
        /// Intensity range record containing error matrix and starting
        /// intensity of the range.
        /// </summary>
        /// <remarks>
        /// It implements IComparable to support sorting a table of these
        /// records and searching there.
        /// </remarks>
        [Serializable]
        public class ErrorRecord : DynamicMatrixTable<ErrorRecord>.Record
        {
            // TODO: use properties

            /// <summary>
            /// Error matrix which will be used for intensity range covered
            /// by the record.
            /// </summary>
            public ErrorMatrix matrix;

            /// <summary>
            /// Create a new record.
            /// </summary>
            /// <param name="intensityRangeStart">Starting intenstity (0-255)
            /// </param>
            /// <param name="matrix">Error matrix</param>
            public ErrorRecord(int intensityRangeStart,
                ErrorMatrix matrix) {
                this.keyRangeStart = intensityRangeStart;
                this.matrix = (ErrorMatrix)matrix.Clone();
            }

            /// <summary>
            /// Create a default record starting at 0 intensity and using
            /// a default error matrix.
            /// </summary>
            public ErrorRecord()
                : this(0, ErrorMatrix.Samples.Default) { }

            public override void init(Image.ImageRunInfo imageRunInfo) {
                base.init(imageRunInfo);
                matrix.init(imageRunInfo);
            }

            public override string ToString() {
                return matrix.ToString();
            }
        }

        /// <summary>
        /// Create a dynamic matrix error filter.
        /// </summary>
        public DynamicMatrixErrorFilter() {
            MatrixTable = new DynamicMatrixTable<ErrorRecord>();
        }

        /// <summary>
        /// Set error according to a matrix dynamically selected depending
        /// upon pixel intensity.
        /// </summary>
        /// <param name="error">Quantization error value</param>
        /// <param name="intensity">Source pixel intensity</param>
        public override void setError(double error, int intensity) {
            // intensity for dynamic table is clipped here to 0-255 range
            setMatrix(MatrixTable.getWorkingRecord(
                Math.Max(Math.Min(intensity, 255), 0)).matrix, false);
            base.setError(error, intensity);
        }

        public override void init(Image.ImageRunInfo imageRunInfo) {
            //base.init(imageRunInfo);
            MatrixTable.init(imageRunInfo);
            int maxMatrixHeight = MatrixTable.DefaultRecord.matrix.Height;
            foreach (ErrorRecord record in MatrixTable.listDefinitionRecords()) {
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
