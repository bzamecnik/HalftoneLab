using System;
using System.Collections.Generic;
using Gimp;

namespace Halftone
{
    public class GSImage : Image
    {
        Drawable _drawable;
        Rectangle _rectangle;

        public Drawable Drawable {
            get { return _drawable; }
        }

        public override int Width { get { return _rectangle.Width; } }
        public override int Height { get { return _rectangle.Height; } }

        public GSImage(Drawable drawable) {
            _drawable = drawable;
            _rectangle = _drawable.MaskBounds;
            Tile.CacheDefault(_drawable);
        }

        public override void IterateSrcDestDirect(
            IterFuncSrcDest pixelFunc,
            IterFuncScanning scanFunc)
        {
            //// TODO: it should be: PixelFetcher(_drawable, true) to support undo
            //// but for now it doesn;t work
            //using (PixelFetcher pf = new PixelFetcher(_drawable, false)) {
            //    foreach (Coordinate<int> coords in scanFunc(Width, Height)) {
            //        int y = _rectangle.Y1 + coords.Y;
            //        int x = _rectangle.X1 + coords.X;
            //        pf[y, x] = pixelFunc(pf[y, x]);
            //    }
            //}
            //_drawable.Flush();
            ////_drawable.MergeShadow(true);
            //_drawable.Update(_rectangle);

            
        }

        public override void IterateSrcDestByRows(
            IterFuncSrcDest pixelFunc,
            IterFuncScanning scanFunc)
        {
            PixelRgn srcPR = new PixelRgn(_drawable, _rectangle, false, false);
            PixelRgn destPR = new PixelRgn(_drawable, _rectangle, true, true);

            //for (IntPtr pr = PixelRgn.Register(srcPR, destPR); pr != IntPtr.Zero;
            //    pr = PixelRgn.Process(pr)) {
            for (int y = _rectangle.Y1; y < _rectangle.Y2; y++) {
                Pixel[] row = srcPR.GetRow(srcPR.X, srcPR.Y + y, srcPR.W);
                
                for (int x = 0; x < row.Length; x++) {
                    row[x] = pixelFunc(row[x]);
                }
                destPR.SetRow(row, destPR.X, destPR.Y + y);
            }
            //foreach (Coordinate<int> coords in scanFunc(srcPR.W, srcPR.H)) {
            //    destPR[coords.Y, coords.X] = pixelFunc(srcPR[coords.Y, coords.X]);
            //}
            //}
            _drawable.Flush();
            _drawable.MergeShadow(true);
            _drawable.Update(_rectangle);
        }

        public override void IterateSrcDestNoOrder(
            IterFuncSrcDest pixelFunc)
        {
            PixelRgn srcPR = new PixelRgn(_drawable, _rectangle, false, false);
            PixelRgn destPR = new PixelRgn(_drawable, _rectangle, true, true);

            //for (IntPtr pr = PixelRgn.Register(srcPR, destPR); pr != IntPtr.Zero;
            //    pr = PixelRgn.Process(pr)) {
                for (int y = srcPR.Y; y < srcPR.Y + srcPR.H; y++) {
	              for (int x = srcPR.X; x < srcPR.X + srcPR.W; x++) {
		              destPR[y, x] = pixelFunc(srcPR[y, x]);
                    }
                }
            //}
            _drawable.Flush();
            _drawable.MergeShadow(true);
            _drawable.Update(_rectangle);
        }
    }
}
