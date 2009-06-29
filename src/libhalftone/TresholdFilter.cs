using System;
using Gimp;

namespace Halftone
{
    /// <summary>
    /// Treshold dither algorithm acts as a base class for dither algorithms
    /// which perform bi-level intensity quantization using a treshold.
    /// </summary>
    /// <remarks>
    /// Its dither() function acts as a comparer, where treshold is supplied
    /// by by treshold() function implemented in child classes.
    /// Note: In future this could be generalized to perform multi-level
    /// quantization.
    /// </remarks>
    [Serializable]
    public abstract class TresholdFilter : Module
	{
		public Pixel dither(Pixel pixel) {
            // TODO:
            // get rid of condition:
            // 
            pixel[0] = (pixel[0] < treshold(pixel[0], pixel.X, pixel.Y)) ? 0 : 255;
            // show tresholding map:
            //pixel[0] = treshold(pixel[0], pixel.X, pixel.Y); // DEBUG
			return pixel;
		}

        public Pixel dither(double intensity, int x, int y) {
            Pixel pixel = new Pixel(1) { X = x, Y = y};
            pixel[0] = (intensity < treshold((int)intensity, x, y)) ? 0 : 255;
            //pixel[0] = treshold((int)intensity, x, y); // DEBUG
            return pixel;
        }

        protected abstract int treshold(int intensity, int x, int y);
	}
}
