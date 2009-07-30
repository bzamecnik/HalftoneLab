using System;
using System.Collections.Generic;
using Gimp;

namespace HalftoneLab
{
    /// <summary>
    /// Scanning order iterates over image pixels giving their coordinates.
    /// </summary>
    /// <remarks>
    /// <para>
    /// It passes through all pixel exactly once.
    /// </para>
    /// <para>
    /// Scanning order needs to be initialized before being used.
    /// </para>
    /// <para>
    /// There are two interfaces for iterating over image pixels.
    /// You can enumerate coordinates using getCoordsEnumerable().
    /// It is easy to use and simple to implement, but you can't query on
    /// next pixel existence. The other is a combination of hasNext(),
    /// next(), CurrentX and CurrentY.
    /// </para>
    /// </remarks>
    /// <example>Example of using getCoordsEnumerable() interface and
    /// a foreach loop:
    /// <code>
    /// ScanningOrder scanOrder = new ScanlineScaningOrder();
    /// scanOrder.init(width, height);
    /// foreach (Coordinate<int> coords in scanOrder.getCoordsEnumerable()) {
    ///     // use coords.X, coords.Y somehow
    /// }
    /// </code>
    /// </example>
    /// <example>Example of using getCoordsEnumerable() interface and
    /// an enumerator:
    /// <code>
    /// ScanningOrder scanOrder = new ScanlineScaningOrder();
    /// scanOrder.init(width, height);
    /// IEnumerator<Coordinate<int>> coordsEnum =
    ///     scanOrder.getCoordsEnumerable().GetEnumerator();
    /// for (... other loop ...) {
    ///     if (coordsEnum.MoveNext()) {
    ///         // use coordsEnum.Current.X, coordsEnum.Current.Y somehow
    ///     } else {
    ///         break;
    ///     }
    /// }
    /// </code>
    /// </example>
    /// <example>Example of using hasNext(), next() interface:
    /// <code>
    /// ScanningOrder scanOrder = new ScanlineScaningOrder();
    /// scanOrder.init(width, height);
    /// while (scanOrder.hasNext()) {
    ///     scanOrder.next();
    ///     // use scanOrder.CurrentX, scanOrder.CurrentY somehow
    /// }
    /// </code>
    /// </example>
    [Serializable]
    [Module(TypeName = "Image scanning order")]
    public abstract class ScanningOrder : Module
    {
        /// <summary>
        /// Current X coordinate (numbered from 0).
        /// </summary>
        public int CurrentX { get; protected set; }
        /// <summary>
        /// Current X coordinate (numbered from 0).
        /// </summary>
        public int CurrentY { get; protected set; }

        //[NonSerialized]
        //private Coordinate<int> _currentCoords;
        //public Coordinate<int> CurrentCoords {
        //    get {
        //        if (_currentCoords == null) {
        //            _currentCoords = new Coordinate<int>()
        //            {
        //                X = CurrentX,
        //                Y = CurrentY
        //            };
        //        }
        //        return _currentCoords;
        //    }
        //    protected set {
        //        _currentCoords = value;
        //    }
        //}

        [NonSerialized]
        protected int _width; // image dimensions
        [NonSerialized]
        protected int _height;

        public override void init(Image.ImageRunInfo imageRunInfo) {
            base.init(imageRunInfo);
            init(imageRunInfo.Width, imageRunInfo.Height);
        }

        /// <summary>
        /// Initialize the scanning order with image dimensions.
        /// </summary>
        /// <param name="width">Image width</param>
        /// <param name="height">Image height</param>
        public abstract void init(int width, int height);

        /// <summary>
        /// Move to next coordinates.
        /// </summary>
        public abstract void next();
        
        /// <summary>
        /// Is there any pixel not yet visited?
        /// </summary>
        /// <returns>True if there are still pixel to visit</returns>
        public abstract bool hasNext();
        
        /// <summary>
        /// Coordinates iterator.
        /// </summary>
        /// <returns>Enumerable of coordinates.</returns>
        public abstract IEnumerable<Coordinate<int>> getCoordsEnumerable();
    }

    /// <summary>
    /// Scanning order traversing along a space-filling curve.
    /// </summary>
    [Serializable]
    public abstract class SFCScanningOrder : ScanningOrder
    {
    }
}
