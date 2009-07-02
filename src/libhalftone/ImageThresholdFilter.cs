using System;
using System.Collections.Generic;

namespace Halftone
{
    /// <summary>
    /// Threshold filter where threshold values are precomputed into an image
    /// using a spot function. The image can be manipulated to produce
    /// artistic effects.
    /// </summary>
    /// <remarks>
    /// The image acts as a matix, but it can be a native GIMP image.
    /// So it is possible to manipulate it efficiently with GIMP filters.
    /// </remarks>
    /// <see cref="Image"/>
    /// <see cref="SpotFunction"/>
    [Serializable]
    public class ImageThresholdFilter : ThresholdFilter
    {
        [NonSerialized]
        private Image _thresholdImage;

        /// <summary>
        /// Generator to generate the image.
        /// </summary>
        public Generator ImageGenerator { get; set; }

        protected override int threshold(int intensity, int x, int y) {
            return _thresholdImage.getPixel(x, y)[0];
        }

        public override void init(Image.ImageRunInfo imageRunInfo) {
            base.init(imageRunInfo);
            ImageGenerator.init(imageRunInfo);
            _thresholdImage = Image.createDefatult(
                imageRunInfo.Width, imageRunInfo.Height);
            ImageGenerator.generateImage(_thresholdImage);
            //if (_thresholdImage is GSImage) {
            //    new Gimp.Display((_thresholdImage as GSImage).Image);
            //}
        }

        /// <summary>
        /// Image generator uses a spot function to generate an image,
        /// then it can optionally apply some effects on that.
        /// </summary>
        [Serializable]
        public class Generator : Module
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

            public Generator() {
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

            public static GSEffectDelegate rippleEffect;
            public static GSEffectDelegate noiseEffect;
            public static GSEffectDelegate pixelizeEffect;
            public static GSEffectDelegate canvasEffect;

            static Generator() {
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
