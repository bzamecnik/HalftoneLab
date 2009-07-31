// Copyright (c) 2009 Bohumir Zamecnik <bohumir@zamecnik.org>
// License: The MIT License, see the LICENSE file

using System;

namespace HalftoneLab
{
    // TODO: global progress

    /// <summary>
    /// A base class for halftoning algorithm skeletons.
    /// </summary>
    /// <remarks>
    /// Halftone algorithms implement the run() function and usually use
    /// plugable submodules to do
    /// </remarks>
    [Serializable]
    [Module(TypeName = "Halftone method")]
    public abstract class HalftoneMethod : Module, IImageFilter
    {
        /// <summary>
        /// Run the algoritm with the current configuration.
        /// Image contents are overwritten with its halftoned version.
        /// </summary>
        /// <remarks>
        /// Module.init(Image.ImageRunInfo) is called in the beginning
        /// as soon as all the image run-time information is known.
        /// </remarks>
        /// <param name="image">Both input and output image.</param>
        public abstract void run(Image image);

        /// <summary>
        /// Create a default HalftoneMethod (currently it is
        /// ThresholdHalftoneMethod). The Factory design pattern is applied
        /// to solve the need for a default module contructor.
        /// </summary>
        /// <returns>Default halftone method instance.</returns>
        public static HalftoneMethod createDefault() {
            return new ThresholdHalftoneMethod();
        }
    }
}
