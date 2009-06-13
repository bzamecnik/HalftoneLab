using System;
//using Gimp;

namespace Halftone
{
    [Serializable]
	public abstract class DitherAlgorithm : Module
	{
		// input image
		// output image
        public abstract void run(Image image);

        // progress
	}
}
