using System;
//using Gimp;

namespace Halftone
{
    /// <summary>
    /// A base class for halftoning algorithm skeletons.
    /// </summary>
    /// <remarks>
    /// Halftone algorithms implement the run() function and usually use
    /// plugable submodules to do
    /// </remarks>
    [Serializable]
	public abstract class HalfoneAlgorithm : Module
	{
        /// <summary>
        /// Run the algoritm with current configuration.
        /// Image contents are overwritten with its halftoned version.
        /// </summary>
        /// <remarks>
        /// Module.init(Image.ImageRunInfo) is called in the beginning
        /// as soon as all the image run-time information is known.
        /// </remarks>
        /// <param name="image">Both input and output image.</param>
        public abstract void run(Image image);
        // TODO: global progress
	}
}
