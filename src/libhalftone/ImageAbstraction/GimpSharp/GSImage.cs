using System;
using System.Collections.Generic;
using Gimp;

namespace Halftone
{
    public class GSImage : Image
    {
        private Drawable _drawable;
        private Rectangle _rectangle;
        private byte[] _imageBuffer; // TODO: this should be a local variable probably

        public Drawable Drawable {
            get { return _drawable; }
        }

        public Progress Progress { get;  set; }

        public override int Width { get { return _rectangle.Width; } }
        public override int Height { get { return _rectangle.Height; } }

        public GSImage(Drawable drawable) {
            _drawable = drawable;
            _rectangle = _drawable.MaskBounds;
            Tile.CacheDefault(_drawable);
            Progress = new Progress("Halftone Laboratory");
        }

        public override void IterateSrcDestDirect(
            IterFuncSrcDest pixelFunc,
            ScanningOrder scanOrder)
        {
            //// TODO: it should be: PixelFetcher(_drawable, true) to support undo
            //// but for now it doesn't work
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
            
            //foreach (Coordinate<int> coords in scanFunc(Width, Height)) {
            //    int y = _rectangle.Y1 + coords.Y;
            //    int x = _rectangle.X1 + coords.X;
            //    //if (y % 10 == 0) { Console.WriteLine("[y,x]: {0}, {1}", y, x); }
            //    putPixel(x, y, pixelFunc(getPixel(x, y)));
            //}

            initBuffer();

            // Note: getting coords using iterator or calling next() is almost
            // the same in speed, although using next() allows better handling
            // progess updates.

            int x = 0;
            int y = 0;

            //// loop with progress count using next()
            scanOrder.init(Width, Height);
            IEnumerator<Coordinate<int>> coordsEnum = scanOrder.getCoordsEnumerable().GetEnumerator();
            // calulate a reasonable progress update count            
            int blockCount = Math.Max((int)Math.Ceiling(
                Math.Ceiling(Math.Sqrt(Width * Height)) / 10.0), 1);
            int blockSize = (int)Math.Ceiling(Height * Width / (double)blockCount);
            double progressPercentage = 0;
            double progressUnit = 1.0 / (double)blockCount;
            for (int i = 0; i < blockCount; i++) {
                for (int block = 0; block < blockSize; block++) {
                    //if (scanOrder.hasNext()) {
                    if (coordsEnum.MoveNext()) {
                        x = coordsEnum.Current.X;
                        y = coordsEnum.Current.Y;
                        //scanOrder.next();
                        //x = scanOrder.CurrentX;
                        //y = scanOrder.CurrentY;
                        setPixel(x, y, pixelFunc(getPixel(x, y)));
                    } else {
                        break;
                    }
                }
                progressPercentage += progressUnit;
                Progress.Update(progressPercentage);
            }

            //scanOrder.init(Width, Height);
            //int blockCount = 100;
            //int blockSize = Width * Height / blockCount;
            //double progressPercentage = 0;
            //double progressUnit = 1.0 / (double)blockCount;
            //int i = 0;
            //for (scanOrder.init(Width, Height); scanOrder.hasNext(); scanOrder.next(out x, out y)) {
            //    setPixel(x, y, pixelFunc(getPixel(x, y)));
            //    if ((i++ % blockSize) == 0) {
            //        progressPercentage += progressUnit;
            //        Progress.Update(progressPercentage);
            //    }
            //}

            // loop without progress count using next()
            //for (scanOrder.init(Width, Height); scanOrder.hasNext(); scanOrder.next(out x, out y)) {
            //    setPixel(x, y, pixelFunc(getPixel(x, y)));
            //}

            // loop without progress count using iterator
            //foreach (Coordinate<int> coords in scanOrder.getCoordsEnumerable(Width, Height)) {
            //    setPixel(coords.X, coords.Y, pixelFunc(getPixel(coords.X, coords.Y)));
            //}

            flushBuffer();

            //PixelRgn srcPR = new PixelRgn(_drawable, _rectangle, false, false);
            //PixelRgn destPR = new PixelRgn(_drawable, _rectangle, true, true);
            
            //foreach (Coordinate<int> coords in scanFunc(Width, Height)) {
            //    int y = _rectangle.Y1 + coords.Y;
            //    int x = _rectangle.X1 + coords.X;
            //    //rgn.GetPixel(px.Bytes, x, y);
            //    srcPR.GetPixel(byteBuf, x, y);
            //    px.Bytes = byteBuf;
            //    px.X = x; px.Y = y;
            //    px = pixelFunc(px);
            //    destPR.SetPixel(px.Bytes, x, y);
            //}
            //_drawable.Flush();
            //_drawable.MergeShadow(true);
            //_drawable.Update(_rectangle);
        }

        public override void IterateSrcDestByRows(
            IterFuncSrcDest pixelFunc,
            ScanningOrder scanOrder)
        {
            //PixelRgn srcPR = new PixelRgn(_drawable, _rectangle, false, false);
            //PixelRgn destPR = new PixelRgn(_drawable, _rectangle, true, true);
            
            //_imageBuffer = new byte[_rectangle.Width * _rectangle.Height * _drawable.Bpp];

            initBuffer();

            int bufferIndexY = 0;
            int rowstride = _rectangle.Width * _drawable.Bpp;
            //byte[] row = new byte[rowstride];
            Pixel tmpPixel = new Pixel(_drawable.Bpp);
            byte[] pixelBytes = new byte[_drawable.Bpp];

            int blockCount = 100;
            double progressPercentage = 0;
            double progressUnit = (double)blockCount / (double)_rectangle.Height;

            for (int y = _rectangle.Y1; y < _rectangle.Y2;
                y++, bufferIndexY += rowstride)
            {
                //Array.Copy(_imageBuffer, bufferIndexY, row, 0, rowstride);
                //Pixel[] pixelRow = srcPR.GetRow(_rectangle.X1, y, _rectangle.Width);
                //Pixel[] row = srcPR.GetRow(_rectangle.X1, y, srcPR.W);
                for (int x = 0, bufferIndex = bufferIndexY; x < _rectangle.Width;
                    x++, bufferIndex += _drawable.Bpp)
                {
                    // setPixel(x, y, pixelFunc(pixelRow[x]));

                    tmpPixel.CopyFrom(_imageBuffer, bufferIndex);
                    tmpPixel.X = x; tmpPixel.Y = y;
                    tmpPixel = pixelFunc(tmpPixel);
                    tmpPixel.CopyTo(_imageBuffer, bufferIndex);
                }
                if ((y % blockCount) == 0) {
                    progressPercentage += progressUnit;
                    Progress.Update(progressPercentage);
                }
            }
            flushBuffer();

            //_drawable.Flush();
            //_drawable.MergeShadow(true);
            //_drawable.Update(_rectangle);
        }

        public override void IterateSrcDestNoOrder(
            IterFuncSrcDest pixelFunc)
        {
            PixelRgn srcPR = new PixelRgn(_drawable, _rectangle, false, false);
            PixelRgn destPR = new PixelRgn(_drawable, _rectangle, true, true);
            double progressPercentage = 0;
            double progressUnit = (double)(Gimp.Gimp.TileWidth * Gimp.Gimp.TileHeight) / (double)(Width * Height);
            for (IntPtr pr = PixelRgn.Register(srcPR, destPR); pr != IntPtr.Zero;
                pr = PixelRgn.Process(pr))
            {
                int yEnd = destPR.Y + destPR.H;
                int xEnd = destPR.X + destPR.W;
                for (int y = destPR.Y; y < yEnd; y++) {
                    for (int x = destPR.X; x < xEnd; x++) {
                        destPR[y, x] = pixelFunc(srcPR[y, x]);
                    }
                }
                progressPercentage += progressUnit;
                Progress.Update(progressPercentage);
            }
            _drawable.Flush();
            _drawable.MergeShadow(true);
            _drawable.Update(_rectangle);

        }

        public override Pixel getPixel(int x, int y) {
            Pixel pixel = new Pixel(_drawable.Bpp);
            pixel.X = x;
            pixel.Y = y;
            pixel.CopyFrom(_imageBuffer, (y * _rectangle.Width + x) * _drawable.Bpp);
            return pixel;
        }

        public override void setPixel(int x, int y, Pixel pixel) {
            pixel.CopyTo(_imageBuffer, (y * _rectangle.Width + x) * _drawable.Bpp);
        }

        public override void initBuffer() {
            PixelRgn rgn = new PixelRgn(_drawable, false, false);
            _imageBuffer = rgn.GetRect(_rectangle.X1, _rectangle.Y1,
                _rectangle.Width, _rectangle.Height);
        }

        public override void flushBuffer() {
            PixelRgn destPR = new PixelRgn(_drawable, true, true);
            destPR.SetRect(_imageBuffer, _rectangle.X1, _rectangle.Y1,
                _rectangle.Width, _rectangle.Height);
            _drawable.Flush();
            _drawable.MergeShadow(true);
            _drawable.Update(_rectangle);
        }
    }
}
