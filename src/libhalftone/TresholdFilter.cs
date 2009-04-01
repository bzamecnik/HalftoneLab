// TresholdFilter.cs created with MonoDevelop
// User: bohous at 15:28 26.3.2009
//

using System;
//using Gimp;

namespace Halftone
{
	public abstract class TresholdFilter
	{
		public Pixel dither(Pixel pixel) {
            pixel.Intensity = (pixel.Intensity < treshold(pixel)) ? 0 : 255;
			return pixel;
		}
		public abstract int treshold(Pixel pixel);
	}
}
