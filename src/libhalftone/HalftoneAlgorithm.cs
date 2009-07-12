using System;

namespace Halftone
{
    [Serializable]
    public class HalftoneAlgorithm : Module, IImageFilter
    {
        public Resize PreResize { get; set; }
        public Sharpen PreSharpen { get; set; }
        public HalftoneMethod Method { get; set; }
        public Resize PostResize { get; set; }
        public Smoothen PostSmoothen { get; set; }

        public HalftoneAlgorithm() {
            Method = HalftoneMethod.createDefault();
        }

        // processing order
        // - pre-processing (optional)
        //    - resize (up)
        //    - sharpen
        // - HalftoneMethod (mandatory)
        // - post-processing (optional)
        //    - resize (down)
        //    - smoothen
        public void run(Image image) {
            // TODO: merge undo history
            if (PreResize != null) {
                PreResize.run(image);
            }
            if (PreSharpen != null) {
                PreSharpen.run(image);
            }
            Method.run(image);
            if (PostResize != null) {
                PostResize.run(image);
            }
            if (PostSmoothen != null) {
                PostSmoothen.run(image);
            }
        }

        [Serializable]
        public class Resize : Module, IImageFilter {
            // TODO: maintain the same image size if resizing
            // back and forward with the same (inversely) factor.
            // Probably use transform direction and tune clipping.

            public double Factor { get; set; }

            public InterpolationType Interpolation { get; set; }

            public enum InterpolationType {
                NearestNeighbour,
                Bilinear,
                Bicubic,
                Lanczos
            }

            public Resize() {
                Factor = 1.0;
                Interpolation = InterpolationType.Bicubic;
            }

            public void run(Image image) {
                GSImage gsimage = image as GSImage;
                if (gsimage != null) {
                    Gimp.InterpolationType gimpInterpolationType
                        = Gimp.InterpolationType.None;
                    switch (Interpolation) {
                        case InterpolationType.NearestNeighbour:
                            gimpInterpolationType = Gimp.InterpolationType.None;
                            break;
                        case InterpolationType.Bilinear:
                            gimpInterpolationType = Gimp.InterpolationType.Linear;
                            break;
                        case InterpolationType.Bicubic:
                            gimpInterpolationType = Gimp.InterpolationType.Cubic;
                            break;
                        case InterpolationType.Lanczos:
                            gimpInterpolationType = Gimp.InterpolationType.Lanczos;
                            break;
                    }
                    double newHeight = image.Height * Factor;
                    double newWidth = image.Width * Factor;
                    gsimage.Drawable.TransformScale(0, 0, newWidth, newHeight,
                        Gimp.TransformDirection.Forward,
                        gimpInterpolationType, false, 1, true);
                }
            }
        }

        [Serializable]
        public class Sharpen : Module, IImageFilter
        {
            public double Amount { get; set; }

            public Sharpen() {
                Amount = 0.1;
            }

            public void run(Image image) {
                GSImage gsimage = image as GSImage;
                if (gsimage != null) {
                    Gimp.Procedure procedure =
                        new Gimp.Procedure("plug_in_sharpen");
                    procedure.Run(gsimage.Image, gsimage.Drawable,
                        (int) (Amount * 100));
                }
            }
        }

        [Serializable]
        public class Smoothen : Module, IImageFilter
        {
            public double Radius { get; set; }

            public Smoothen() {
                Radius = 5;
            }

            public void run(Image image) {
                GSImage gsimage = image as GSImage;
                if (gsimage != null) {
                    Gimp.Procedure procedure =
                        new Gimp.Procedure("plug_in_gauss");
                    procedure.Run(gsimage.Image, gsimage.Drawable,
                        Radius, Radius, 0);
                    gsimage.Drawable.Levels(Gimp.HistogramChannel.Value,
                        110, 145, 1.0, 0, 255);
                }
            }
        }
    }
}
