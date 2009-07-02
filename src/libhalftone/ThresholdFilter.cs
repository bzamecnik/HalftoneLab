using System;
using Gimp;

namespace Halftone
{
    /// <summary>
    /// Threshold dither algorithm acts as a base class for dither algorithms
    /// that perform bi-level intensity quantization using a threshold.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Intensities lower than threshold are turned to black, intensities
    /// equal or higher than threshold are turned to white.
    /// </para>
    /// <para>
    /// Its quantize() function acts as a comparer, where threshold is supplied
    /// by by threshold() function implemented in child classes.
    /// </para>
    /// <para>
    /// Note: In future this could be generalized to perform multi-level
    /// quantization.
    /// </para>
    /// </remarks>
    [Serializable]
    public abstract class ThresholdFilter : Module
	{
        /// <summary>
        /// Quantize pixel intensity to two levels. Input and output pixel
        /// is of Pixel type.
        /// </summary>
        /// <param name="pixel">Input pixel with intensity (0-255) and
        /// coordinates</param>
        /// <returns>Pixel with quantized intensity (0-255)</returns>
		public Pixel quantize(Pixel pixel) {
            // TODO: get rid of the condition:
            pixel[0] = (pixel[0] < threshold(pixel[0], pixel.X, pixel.Y))
                ? 0 : 255;
            // show thresholding map:
            //pixel[0] = threshold(pixel[0], pixel.X, pixel.Y); // DEBUG
			return pixel;
		}

        /// <summary>
        /// Quantize pixel intensity to two levels.
        /// Intensity and pixel coordinates are given separately.
        /// Intensity can include diffused error, so it's of double type.
        /// </summary>
        /// <param name="intensity">Intensity to be quantized (0.0-255.0)
        /// </param>
        /// <param name="x">Pixel X coordinate</param>
        /// <param name="y">Pixel Y coordinate</param>
        /// <returns>Pixel with quantized intensity (0-255)</returns>
        public Pixel quantize(double intensity, int x, int y) {
            Pixel pixel = new Pixel(1) { X = x, Y = y};
            pixel[0] = (intensity < threshold((int)intensity, x, y)) ? 0 : 255;
            //pixel[0] = ((int)Math.Floor((intensity - threshold((int)intensity, x, y))/256.0 + 1)) * 255;
            //pixel[0] = threshold((int)intensity, x, y); // DEBUG
            return pixel;
        }

        /// <summary>
        /// Quantize pixel intensity to two levels.
        /// Intensity and pixel coordinates are given separately.
        /// </summary>
        /// <param name="intensity">Intensity to be quantized (0-255)
        /// </param>
        /// <param name="x">Pixel X coordinate</param>
        /// <param name="y">Pixel X coordinate</param>
        /// <returns>Quantized intensity (0-255)</returns>
        public int quantize(int intensity, int x, int y) {
            return (intensity < threshold(intensity, x, y)) ? 0 : 255;
            //return threshold((int)intensity, x, y); // DEBUG
        }

        /// <summary>
        /// Compute threshold value for a pixel given its intensity and
        /// coordinates.
        /// </summary>
        /// <param name="intensity">Pixel intensity (0-255)</param>
        /// <param name="x">Pixel X coordinate</param>
        /// <param name="y">Pixel Y coordinate</param>
        /// <returns>Threshold value (0-255)</returns>
        protected abstract int threshold(int intensity, int x, int y);
	}
}
