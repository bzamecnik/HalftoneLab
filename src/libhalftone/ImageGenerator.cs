using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Halftone
{
    /// <summary>
    /// Image generator uses a spot function to generate an image,
    /// then it can optionally apply some effects on that.
    /// </summary>
    [Serializable]
    public class ImageGenerator : Module
    {
        /// <summary>
        /// Delegate to a function applying an effect on the image.
        /// </summary>
        /// <param name="image">GimpSharp image - input & output</param>
        public delegate void GSEffectDelegate(GSImage image);

        /// <summary>
        /// Spot function to generate the basic image.
        /// </summary>
        public SpotFunction SpotFunction { get; set; }

        /// <summary>
        /// Multiple effects sequentially applied on the image.
        /// </summary>
        public List<GSEffectDelegate> Effects { get; set; }

        public ImageGenerator() {
            Effects = new List<GSEffectDelegate>();
        }

        /// <summary>
        /// Fill the image with spot function and then sequentially
        /// apply effects on it.
        /// </summary>
        /// <remarks>
        /// Effects will be applied only a GimpSharp image (GSImage).
        /// </remarks>
        /// <param name="image"></param>
        public void generateImage(Image image) {
            image.initBuffer();
            image.IterateSrcDestByRows((pixel) =>
            {
                pixel[0] = SpotFunction.SpotFunc(pixel.X, pixel.Y);
                return pixel;
            }, new ScanlineScanningOrder());
            image.flushBuffer();
            GSImage gsImage = image as GSImage;
            if ((gsImage != null) && (Effects != null)) {
                foreach (GSEffectDelegate effect in Effects) {
                    if (effect != null) {
                        effect(gsImage);
                    }
                }
                gsImage.Drawable.Flush();
                gsImage.Drawable.Update();
            }
            image.initBuffer();
        }

        public override void init(Image.ImageRunInfo imageRunInfo) {
            base.init(imageRunInfo);
            SpotFunction.init(imageRunInfo);
        }

        public static class Samples
        {
            public static GSEffectDelegate rippleEffect;
            public static GSEffectDelegate noiseEffect;
            public static GSEffectDelegate pixelizeEffect;
            public static GSEffectDelegate canvasEffect;

            static Samples() {
                rippleEffect = (GSImage image) =>
                {
                    Console.Out.WriteLine("Ripple effect");
                    Gimp.Procedure procedure =
                           new Gimp.Procedure("plug_in_ripple");
                    procedure.Run(image.Image, image.Drawable,
                        20, 5, 0, 1, 1, 1, 0);
                    procedure.Run(image.Image, image.Drawable,
                        20, 5, 1, 1, 1, 1, 0);
                };
                noiseEffect = (GSImage image) =>
                {
                    Console.Out.WriteLine("Noise effect");
                    Gimp.Procedure procedure =
                           new Gimp.Procedure("plug_in_rgb_noise");
                    procedure.Run(image.Image, image.Drawable,
                        1, 1, 0.2);
                };
                pixelizeEffect = (GSImage image) =>
                {
                    Console.Out.WriteLine("Pixelize effect");
                    Gimp.Procedure procedure =
                           new Gimp.Procedure("plug_in_pixelize");
                    procedure.Run(image.Drawable.Image, image.Drawable, 4);
                };
                canvasEffect = (GSImage image) =>
                {
                    Console.Out.WriteLine("Canvas effect");
                    Gimp.Procedure procedure =
                           new Gimp.Procedure("plug_in_apply_canvas");
                    procedure.Run(image.Drawable.Image, image.Drawable, 0, 4);
                };
            }
        }
    }
}
