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
    [Module(TypeName = "Threshold image generator")]
    public class ImageGenerator : Module
    {
        /// <summary>
        /// Spot function to generate the basic image.
        /// </summary>
        public SpotFunction SpotFunction { get; set; }

        /// <summary>
        /// Multiple effects sequentially applied on the image.
        /// </summary>
        public List<IImageFilter> Effects { get; set; }

        public ImageGenerator() {
            SpotFunction = new SpotFunction();
            Effects = new List<IImageFilter>();
        }

        /// <summary>
        /// Fill the image with spot function and then sequentially
        /// apply effects on it.
        /// </summary>
        /// <param name="image"></param>
        public void generateImage(Image image) {
            image.initBuffer();
            if (SpotFunction != null) {
                image.IterateSrcDestByRows((pixel) =>
                {
                    pixel[0] = SpotFunction.SpotFunc(pixel.X, pixel.Y);
                    return pixel;
                }, new ScanlineScanningOrder());
                image.flushBuffer();
            }
            if ((image != null) && (Effects != null)) {
                foreach (IImageFilter effect in Effects) {
                    if (effect != null) {
                        effect.run(image);
                    }
                }
            }
            image.initBuffer();
        }

        public override void init(Image.ImageRunInfo imageRunInfo) {
            base.init(imageRunInfo);
            if (SpotFunction != null) {
                SpotFunction.init(imageRunInfo);
            }
        }

        public static class Samples
        {
            public static IImageFilter rippleEffect;
            public static IImageFilter noiseEffect;
            public static IImageFilter pixelizeEffect;
            public static IImageFilter canvasEffect;

            public static ImageGenerator euclidRippleGenerator;
            public static ImageGenerator euclidNoiseGenerator;
            public static ImageGenerator euclidPixelizeGenerator;
            public static ImageGenerator euclidCanvasGenerator;

            private static List<ImageGenerator> _list;
            public static IEnumerable<ImageGenerator> list() {
                return _list;
            }

            static Samples() {
                _list = new List<ImageGenerator>();

                rippleEffect = new GSImageFilter()
                {
                    Name = "Ripple 2x",
                    Description = "period: 20, amplitude: 5, vert. + horiz.",
                    runGSFilter = (GSImage image) =>
                    {
                        Gimp.Procedure procedure =
                           new Gimp.Procedure("plug_in_ripple");
                        procedure.Run(image.Image, image.Drawable,
                            20, 5, 0, 1, 1, 1, 0);
                        procedure.Run(image.Image, image.Drawable,
                            20, 5, 1, 1, 1, 1, 0);
                    }
                };
                _list.Add(new ImageGenerator() {
                    Name = "Euclid + ripple",
                    SpotFunction = SpotFunction.Samples.euclidDot,
                    Effects = { rippleEffect }
                });


                noiseEffect = new GSImageFilter()
                {
                    Name = "Noise",
                    Description = "RGB noise, independent, corelated, amount: 0.2",
                    runGSFilter = (GSImage image) =>
                    {
                        Gimp.Procedure procedure =
                           new Gimp.Procedure("plug_in_rgb_noise");
                        procedure.Run(image.Image, image.Drawable,
                            1, 1, 0.2);
                    }
                };
                _list.Add(new ImageGenerator()
                {
                    Name = "Euclid + noise",
                    SpotFunction = SpotFunction.Samples.euclidDot,
                    Effects = { noiseEffect }
                });

                pixelizeEffect = new GSImageFilter()
                {
                    Name = "Pixelize",
                    Description = "block: 4px",
                    runGSFilter = (GSImage image) =>
                    {
                        Gimp.Procedure procedure =
                            new Gimp.Procedure("plug_in_pixelize");
                        procedure.Run(image.Image, image.Drawable, 4);
                    }
                };
                _list.Add(new ImageGenerator()
                {
                    Name = "Euclid + pixelize",
                    SpotFunction = SpotFunction.Samples.euclidDot,
                    Effects = { pixelizeEffect }
                });

                canvasEffect = new GSImageFilter()
                {
                    Name = "Canvas",
                    Description = "direction: 0, depth: 4",
                    runGSFilter = (GSImage image) =>
                    {
                        Gimp.Procedure procedure =
                           new Gimp.Procedure("plug_in_apply_canvas");
                        procedure.Run(image.Image, image.Drawable, 0, 4);
                    }
                };
                _list.Add(new ImageGenerator()
                {
                    Name = "Euclid + canvas",
                    SpotFunction = SpotFunction.Samples.euclidDot,
                    Effects = { canvasEffect }
                });

                _list.Add(new ImageGenerator()
                {
                    Name = "Euclid + pixelize + ripple",
                    SpotFunction = SpotFunction.Samples.euclidDot,
                    Effects = { pixelizeEffect, rippleEffect }
                });
            }
        }
    }
}
