// Copyright (c) 2009 Bohumir Zamecnik <bohumir@zamecnik.org>
// License: The MIT License, see the LICENSE file

using System;
using System.Text;

namespace HalftoneLab
{
    /// <summary>
    /// Buffer for error-diffusion filters.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Error-diffusion performs a neighbourhood-wide operation on image
    /// pixels. As the error filter processes the pixels it distributes
    /// (adds) a fractional part of the error to the other pixels. Until
    /// those pixel are processes, the accumulated error has to be store
    /// somewhere. The original image is not the best place for two reasons:
    /// 1) It might not be the same as the output image (or be writable
    /// at all).
    /// 2) It might not be able to represent the error in such a precision.
    /// Thus a separate buffer to store an accumulated error for pixels
    /// waiting to be processed becomes useful.
    /// </para>
    /// <para>
    /// Error buffer behavior depends on image scanning order, so there are
    /// several types of error buffers.
    /// </para>
    /// </remarks>
    public abstract class ErrorBuffer
    {
        public static ErrorBuffer createFromScanningOrder(
            ScanningOrder scanningOrder,
            int height,
            int width) {
            // TODO: This is not quite nice. How to make it better?

            if (scanningOrder is ScanlineScanningOrder) {
                return new MatrixErrorBuffer(height, width);
            } else if (scanningOrder is SerpentineScanningOrder) {
                return new MatrixErrorBuffer(height, width)
                { SerpentineEnabled = true };
            } else if (scanningOrder is SFCScanningOrder) {
                return new VectorErrorBuffer(width);
            } else {
                return null;
            }
        }

        /// <summary>
        /// Get accumulated error value for given pixel.
        /// </summary>
        /// <returns>Accumulated error for current pixel position.</returns>
        public abstract double getError();

        /// <summary>
        /// Move to next pixel position.
        /// </summary>
        public abstract void moveNext();
    }

    /// <summary>
    /// A two-dimensional error buffer.
    /// </summary>
    /// <remarks>
    /// <para>
    /// There are currently two modes: scanline row scanning (default) and
    /// serpentine row scanning (with SerpentineEnabled == true).
    /// </para>
    /// <para>
    /// The buffer holds the current pixel position and moves it on its own.
    /// There are usually several image lines buffered.
    /// </para>
    /// <para>
    /// Note:
    /// For now it seems that scanline and serpentine scanning is enough
    /// and the need for a different behavior is quite unlikely.
    /// The code is more compact and readable than with having to separate
    /// classes doing almost the same.
    /// </para>
    /// <para>
    /// To support different behaviour make this class abstract and split
    /// the code into several descentant classes.
    /// </para>
    /// </remarks>
    public class MatrixErrorBuffer : ErrorBuffer
    {
        /// <summary>
        /// The error buffer.
        /// </summary>
        protected double[,] _buffer;
        
        /// <summary>
        /// Current X position of the source pixel in the buffer.
        /// </summary>
        protected int _currentOffsetX;
        /// <summary>
        /// Current Y position of the source pixel in the buffer.
        /// </summary>
        protected int _currentOffsetY;

        /// <summary>
        /// Switch between scanline and serpentine mode.
        /// </summary>
        /// <value>
        /// Scanline (false), serpentine (true).
        /// </value>
        public bool SerpentineEnabled { get; set; }
        /// <summary>
        /// X step. Possible values:
        /// For scanline order: false.
        /// For serpentine order: true, false.
        /// </summary>
        private bool _backMovement;

        /// <summary>
        /// Buffer height.
        /// </summary>
        public int Height { get; protected set; }
        /// <summary>
        /// Buffer width.
        /// </summary>
        public int Width { get; protected set; }

        /// <summary>
        /// Create a matrix error buffer with scanline order by default.
        /// </summary>
        /// <param name="height">Buffer height</param>
        /// <param name="width">Buffer width</param>
        public MatrixErrorBuffer(int height, int width) {
            SerpentineEnabled = false;
            init(height, width);
        }

        /// <summary>
        /// Change the buffer dimensions.
        /// </summary>
        /// <param name="height">New buffer height</param>
        /// <param name="width">New buffer width</param>
        public void resize(int height, int width) {
            init(height, width);
        }

        /// <summary>
        /// Initialize the buffer.
        /// </summary>
        /// <param name="height">Buffer height</param>
        /// <param name="width">Buffer width</param>
        protected virtual void init(int height, int width) {
            _buffer = new double[height, width];
            Height = _buffer.GetLength(0);
            Width = _buffer.GetLength(1);
            _currentOffsetX = _currentOffsetY = 0;
            _backMovement = false;
        }

        public override double getError() {
            return _buffer[_currentOffsetY, _currentOffsetX];
        }

        /// <summary>
        /// Add quantization error to a pixel on given position
        /// relative to current source pixel position.
        /// </summary>
        /// <remarks>
        /// Error being set outside the buffer is discarded!!
        /// </remarks>
        /// <param name="offsetY">Relative Y coordinate of destination pixel
        /// </param>
        /// <param name="offsetX">Relative X coordinate of destination pixel
        /// </param>
        /// <param name="error">Quantization error part for that pixel
        /// </param>
        public void setError(int offsetY, int offsetX, double error) {
            bool oddLine = _backMovement;
            oddLine = (SerpentineEnabled && ((offsetY % 2) == 1))
                ? !oddLine : oddLine;
            int x = _currentOffsetX + (oddLine ? -offsetX : offsetX);
            int y = (_currentOffsetY + offsetY) % Height;
            if ((x >= 0) && (x < Width)) {
                // discard error being set outside the buffer
                _buffer[y, x] += error;
            }
        }

        public override void moveNext() {
            // move to next pixel, cycle the buffer if necessary
            bool endOfRow = (_backMovement  && (_currentOffsetX == 0))
                         || (!_backMovement && (_currentOffsetX == (Width - 1)));
            if (endOfRow) {
                // the end of a row was reached, move to the next row
                int lastRowY = _currentOffsetY;
                _currentOffsetY = (_currentOffsetY + 1) % Height;
                // clear the last line
                for (int x = 0; x < Width; x++) {
                    _buffer[lastRowY, x] = 0;
                }
            }
            if (!(SerpentineEnabled && endOfRow)) {
                _currentOffsetX = (_currentOffsetX +
                    (_backMovement ? Width - 1 : 1) ) % Width;
            } else {
                // change the row scanning direction
                _backMovement = !_backMovement;
            }
        }

        public override string ToString() {
            StringBuilder sb = new StringBuilder();
            sb.Append("buffer [");
            sb.AppendFormat("@ {0}x{1}", _currentOffsetY, _currentOffsetX);
            sb.AppendLine();
            for (int i = 0; i < _buffer.GetLength(0); i++) {
                for (int j = 0; j < _buffer.GetLength(1); j++) {
                    sb.AppendFormat("{0}, ", _buffer[i, j]);
                }
                sb.AppendLine();
            }
            sb.Append("]");
            sb.AppendLine();
            return sb.ToString();
        }
    }

    /// <summary>
    /// A simple cyclic 1-D error buffer.
    /// </summary>
    /// <remarks>
    /// It is intended to be used by CellHalftoneMethod with
    /// SFCScanningOrder. The reason not to use MatrixErrorBuffer is
    /// different signature of constructor, resize() and setError()
    /// functions.
    /// </remarks>
    public class VectorErrorBuffer : ErrorBuffer
    {
        /// <summary>
        /// The error buffer.
        /// </summary>
        private double[] _buffer;

        /// <summary>
        /// Current position of the source pixel in the buffer.
        /// </summary>
        private int _currentOffset;

        /// <summary>
        /// Length of the buffer.
        /// </summary>
        public int Length {
            get {
                return (_buffer != null) ? _buffer.Length : 0;
            }
        }

        /// <summary>
        /// Create a line error buffer.
        /// </summary>
        /// <param name="length">Buffer length</param>
        public VectorErrorBuffer(int length) {
            _buffer = new double[length + 1];
            _currentOffset = 0;
        }

        /// <summary>
        /// Change the buffer length.
        /// </summary>
        /// <param name="length">New buffer length</param>
        public void resize(int length) {
            _buffer = new double[length + 1];
            _currentOffset = 0;
        }

        public override double getError() {
            return _buffer[_currentOffset];
        }

        /// <summary>
        /// Add quantization error to a pixel on given offset relative to
        /// current source pixel position.
        /// </summary>
        /// <remarks>
        /// Error being set outside the buffer is discarded!!
        /// </remarks>
        /// <param name="offset">Relative offset of destination pixel</param>
        /// <param name="error">Quantization error part for that pixel
        /// </param>
        public void setError(int offset, double error) {
            int pos = (_currentOffset + offset) % _buffer.Length;
            if ((pos >= 0) && (pos < _buffer.Length)) {
                // discard error being set outside the buffer
                _buffer[pos] += error;
            }
        }

        public override void moveNext() {
            // move to next pixel, cycle the buffer if necessary
            _buffer[_currentOffset] = 0;
            _currentOffset = (_currentOffset + 1) % _buffer.Length;
        }

        public override string ToString() {
            StringBuilder sb = new StringBuilder();
            sb.Append("buffer [");
            foreach (double item in _buffer) {
                sb.AppendFormat("{0}, ", item);
            }
            sb.Append("]");
            return sb.ToString();
        }
    }
}
