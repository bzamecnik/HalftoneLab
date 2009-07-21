using System;

namespace Halftone
{
    [Serializable]
    [Module(TypeName = "Halftone algorithm")]
    public class HalftoneAlgorithm : Module, IImageFilter
    {
        public Resize PreResize { get; set; }
        public DotGainCorrection PreDotGain { get; set; }
        public Sharpen PreSharpen { get; set; }
        public HalftoneMethod Method { get; set; }
        public Resize PostResize { get; set; }
        // supersamling = post resize is inverse of pre resize
        public bool SupersamplingEnabled { get; set; }
        public Smoothen PostSmoothen { get; set; }

        public HalftoneAlgorithm() {
            Method = HalftoneMethod.createDefault();
        }

        // processing order
        // - pre-processing (optional)
        //    - resize (up)
        //    - dot gain correction
        //    - sharpen
        // - HalftoneMethod (mandatory)
        // - post-processing (optional)
        //    - resize (down)
        //    - smoothen
        public void run(Image image) {
            if (PreResize != null) {
                //PreResize.Forward = true;
                PreResize.run(image);
            }
            if (PreDotGain != null) {
                PreDotGain.run(image);
            }
            if (PreSharpen != null) {
                PreSharpen.run(image);
            }
            Method.run(image);
            if (SupersamplingEnabled && (PreResize != null)) {
                PreResize.Forward = true;
                if (PostResize == null) {
                    PostResize = new Resize();
                }
                PostResize.Factor = PreResize.Factor;
                PostResize.Forward = false;
            }
            if (PostResize != null) {
                PostResize.run(image);
            }
            if (PostSmoothen != null) {
                PostSmoothen.run(image);
            }
        }

        [Serializable]
        public class Resize : GSImageFilter
        {
            // TODO: maintain the same image size if resizing
            // back and forward with the same (inversely) factor.
            // Probably use transform direction and tune clipping.

            public double Factor { get; set; }

            public bool Forward { get; set; }

            public InterpolationType Interpolation { get; set; }

            public enum InterpolationType {
                NearestNeighbour,
                Bilinear,
                Bicubic,
                Lanczos
            }

            public Resize() {
                Factor = 1.0;
                Forward = true;
                Interpolation = InterpolationType.Bicubic;
                runGSFilter = (GSImage image) =>
                {
                    Gimp.InterpolationType interpolation
                        = Gimp.InterpolationType.None;
                    switch (Interpolation) {
                        case InterpolationType.NearestNeighbour:
                            interpolation = Gimp.InterpolationType.None;
                            break;
                        case InterpolationType.Bilinear:
                            interpolation = Gimp.InterpolationType.Linear;
                            break;
                        case InterpolationType.Bicubic:
                            interpolation = Gimp.InterpolationType.Cubic;
                            break;
                        case InterpolationType.Lanczos:
                            interpolation = Gimp.InterpolationType.Lanczos;
                            break;
                    }
                    Gimp.TransformDirection direction =
                        (Forward) ? Gimp.TransformDirection.Forward :
                            Gimp.TransformDirection.Backward;
                    image.scale(Factor, interpolation, direction);
                };
            }
        }

        [Serializable]
        public abstract class DotGainCorrection : GSImageFilter
        {
        }

        [Serializable]
        public class GammaCorrection : DotGainCorrection
        {
            private double _gamma;
            public double Gamma {
                get { return _gamma; }
                set {
                    if ((value >= 0.1) & (value <= 10)) {
                        _gamma = value;
                    }
                }
            }

            public GammaCorrection() {
                Gamma = 1.0;
                runGSFilter = (GSImage image) =>
                {
                    image.Drawable.Levels(Gimp.HistogramChannel.Value,
                        0, 255, Gamma, 0, 255);
                };
            }
        }

        [Serializable]
        public class Sharpen : GSImageFilter
        {
            public double Amount { get; set; }

            public Sharpen() {
                Amount = 0.1;
                runGSFilter = (GSImage image) =>
                {
                    Gimp.Procedure procedure =
                        new Gimp.Procedure("plug_in_sharpen");
                    procedure.Run(image.Image, image.Drawable,
                        (int)(Amount * 100));
                };
            }
        }

        [Serializable]
        public class Smoothen : GSImageFilter
        {
            public double Radius { get; set; }

            public Smoothen() {
                Radius = 5;
                runGSFilter = (GSImage image) =>
                {
                    Gimp.Procedure procedure =
                        new Gimp.Procedure("plug_in_gauss");
                    procedure.Run(image.Image, image.Drawable,
                        Radius, Radius, 0);
                    image.Drawable.Levels(Gimp.HistogramChannel.Value,
                        110, 145, 1.0, 0, 255);
                };
            }
        }
    }
}
