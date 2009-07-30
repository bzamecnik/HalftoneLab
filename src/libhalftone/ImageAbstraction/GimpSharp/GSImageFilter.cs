using System;

namespace HalftoneLab
{
    /// <summary>
    /// An image filter skeleton specialized on %GIMP# images (GSImage).
    /// </summary>
    /// <remarks>
    /// The filter is stored in form of an anonymous function.
    /// </remarks>
    [Serializable]
    public class GSImageFilter : Module, IImageFilter
    {
        /// <summary>
        /// Delegate for filters working on a GSImage.
        /// </summary>
        /// <param name="image">Image to be processed - acts both as input and
        /// output</param>
        public delegate void GSFilter(GSImage image);

        /// <summary>
        /// fFilter working on a GSImage.
        /// </summary>
        public GSFilter runGSFilter;

        /// <summary>
        /// Apply the filter.
        /// </summary>
        /// <remarks>
        /// The image is converted to GSImage and runGSFilter is applied onto
        /// it. If the image is not a GSImage nothing happens.
        /// </remarks>
        /// <param name="image">Image to be processed - acts both as input and
        /// output</param>
        public void run(Image image) {
            GSImage gsImage = image as GSImage;
            if ((gsImage != null) && (runGSFilter != null)) {
                runGSFilter(gsImage);
                gsImage.Drawable.Flush();
                gsImage.Drawable.Update();
            }
        }
    }
}
