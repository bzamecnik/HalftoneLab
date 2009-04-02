// DitherAlgorithm.cs created with MonoDevelop
// User: bohous at 14:59 26.3.2009

using System;
//using Gimp;

namespace Halftone
{
	public abstract class DitherAlgorithm
	{
		// input image
		// output image
		// scanning order
        public abstract void run(Image image);

        // progress
	}
}
