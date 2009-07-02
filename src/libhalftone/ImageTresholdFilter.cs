using System;

namespace Halftone
{
    /// <summary>
    /// Treshold filter where treshold values are precomputed into an image
    /// using a spot function. The image can be manipulated to produce
    /// artistic effects.
    /// </summary>
    /// <remarks>
    /// The image acts as a matix, but it can be a native GIMP image.
    /// So it is possible to manipulate it efficiently with GIMP filters.
    /// </remarks>
    [Serializable]
    public class ImageTresholdFilter : TresholdFilter
    {
        [NonSerialized]
        private Image _tresholdImage;
        public Generator ImageGenerator { get; set; }

        protected override int treshold(int intensity, int x, int y) {
            return _tresholdImage.getPixel(x, y)[0];
        }

        public override void init(Image.ImageRunInfo imageRunInfo) {
            base.init(imageRunInfo);
            ImageGenerator.init(imageRunInfo);
            _tresholdImage = Image.createDefatult(
                imageRunInfo.Width, imageRunInfo.Height);
            ImageGenerator.generateImage(_tresholdImage);
        }

        [Serializable]
        public class Generator : Module
        {
            public delegate void EffectsDelegate(GSImage image);

            public SpotFunction SpotFunction { get; set; }
            public EffectsDelegate Effects { get; set; }

            public void generateImage(Image image) {
                image.initBuffer();
                image.IterateSrcDestByRows((pixel) =>
                {
                    pixel[0] = SpotFunction.SpotFunc(pixel.X, pixel.Y);
                    return pixel;
                }, new ScanlineScanningOrder());
                image.flushBuffer();
                if ((Effects != null) && (image is GSImage)) {
                    Effects((GSImage)image);
                }
            }

            public static EffectsDelegate rippleEffect;
            public static EffectsDelegate noiseEffect;
            public static EffectsDelegate pixelizeEffect;
            static Generator() {
                rippleEffect = (GSImage image) =>
                    {
                        Console.Out.WriteLine("Ripple effect");
                        Gimp.Procedure procedure =
                               new Gimp.Procedure("plug_in_ripple");
                        procedure.Run(image.Image, image.Drawable,
                            20, 5, 0, 0, 1, 1, 0);
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
            }
        }
    }
}
