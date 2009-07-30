using System;
using System.Collections.Generic;

namespace HalftoneLab
{
    /// <summary>
    /// Threshold filter where threshold values are precomputed into an image
    /// using a spot function. The image can be manipulated to produce
    /// artistic effects.
    /// </summary>
    /// <remarks>
    /// The image acts as a matix, but it can be imlemented as a native GIMP
    /// image. So it is possible to manipulate it efficiently with GIMP
    /// filters.
    /// </remarks>
    /// <see cref="Image"/>
    /// <see cref="ImageGenerator"/>
    /// <see cref="SpotFunction"/>
    [Serializable]
    [Module(TypeName = "Image threshold filter")]
    public class ImageThresholdFilter : ThresholdFilter
    {
        [NonSerialized]
        private Image _thresholdImage;

        /// <summary>
        /// Image generator to generate the image.
        /// </summary>
        public ImageGenerator ImageGenerator { get; set; }

        /// <summary>
        /// Create a new image threshold filter with a default image generator.
        /// </summary>
        public ImageThresholdFilter() : this(new ImageGenerator()) { }

        /// <summary>
        /// Create a new image threshold filter.
        /// </summary>
        /// <param name="imageGenerator">Initial image generator</param>
        public ImageThresholdFilter(ImageGenerator imageGenerator) {
            ImageGenerator = imageGenerator;
        }

        protected override int threshold(int intensity, int x, int y) {
            return _thresholdImage.getPixel(x, y)[0];
        }

        public override void init(Image.ImageRunInfo imageRunInfo) {
            base.init(imageRunInfo);
            _thresholdImage = Image.createDefatult(
                    imageRunInfo.Width, imageRunInfo.Height);
            if (ImageGenerator != null) {
                ImageGenerator.init(imageRunInfo);
                ImageGenerator.generateImage(_thresholdImage);
            }
            // DEBUG: display the threshold image
            //if (_thresholdImage is GSImage) {
            //    new Gimp.Display((_thresholdImage as GSImage).Image);
            //}
        }
    }
}
