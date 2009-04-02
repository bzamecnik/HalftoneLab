using System;
using System.Collections.Generic;
using Gimp;

namespace Halftone
{
    class GSImage : Image
    {
        Drawable _drawable;
        Rectangle _rectangle;

        public Drawable Drawable {
            get { return _drawable; }
        }

        public GSImage(Drawable drawable) {
            _drawable = drawable;
            _rectangle = _drawable.MaskBounds;
        }
        
        public override void IterateSrcDest(IterFuncSrcDest pixelFunc,
                IterFuncScanning scanFunc
            ) {
            PixelRgn srcPR = new PixelRgn(_drawable, _rectangle, false, false);
            PixelRgn destPR = new PixelRgn(_drawable, _rectangle, true, true);

            for (IntPtr pr = PixelRgn.Register(srcPR, destPR); pr != IntPtr.Zero;
                pr = PixelRgn.Process(pr)) {
                foreach (Coordinate<int> coords in scanFunc(srcPR.W, srcPR.H)) {
                    int x = srcPR.X + coords.X;
                    int y = srcPR.Y + coords.Y;
                    destPR[y, x] = pixelFunc(srcPR[y, x]);
                }
            }
            _drawable.Flush();
            _drawable.MergeShadow(true);
            _drawable.Update(_rectangle);
        }
    }
}
