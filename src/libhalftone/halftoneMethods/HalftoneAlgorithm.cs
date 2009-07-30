using System;

namespace HalftoneLab
{
    /// <summary>
    /// A wrapper over HalftoneMethod enabling some pre- and post-processing.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Processing order:
    /// - pre-processing (optional)
    ///    - PreResize - resize (usually upscaling)
    ///    - PreDotGain - dot gain correction
    ///    - PreSharpen -sharpening
    /// - HalftoneMethod (mandatory)
    /// - post-processing (optional)
    ///    - PostResize - resize (usually downscaling)
    ///    - PostSmoothen - smoothening
    /// </para>
    /// <para>
    /// The filters are currently implemented using %Gimp#.
    /// </para>
    /// </remarks>
    /// <see cref="HalftoneMethod"/>
    [Serializable]
    [Module(TypeName = "Halftone algorithm")]
    public class HalftoneAlgorithm : Module, IImageFilter
    {
        /// <summary>
        /// Pre-processing resize filter.
        /// </summary>
        public Resize PreResize { get; set; }
        /// <summary>
        /// Pre-processing dot-gain correction filter.
        /// </summary>
        public DotGainCorrection PreDotGain { get; set; }
        /// <summary>
        /// Pre-processing sharpening filter.
        /// </summary>
        public Sharpen PreSharpen { get; set; }
        /// <summary>
        /// Halftoning method.
        /// </summary>
        public HalftoneMethod Method { get; set; }
        /// <summary>
        /// Post-processing resize filter.
        /// </summary>
        public Resize PostResize { get; set; }
        /// <summary>
        /// Use supersampling? Supersampling means that the post resize factor
        /// is the inverse of the pre resize factor.
        /// </summary>
        public bool SupersamplingEnabled { get; set; }
        /// <summary>
        /// Pre-processing smoothening filter.
        /// </summary>
        public Smoothen PostSmoothen { get; set; }

        /// <summary>
        /// Create a new HalftoneAlgorithm with a default halftone method
        /// and no pre- and post-processing.
        /// </summary>
        public HalftoneAlgorithm() {
            Method = HalftoneMethod.createDefault();
        }

        public void run(Image image) {
            // merge undo history
            GSImage gsImage = image as GSImage;
            if (gsImage != null) {
                gsImage.Image.UndoGroupStart();
            }

            if (PreResize != null) {
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

            if (gsImage != null) {
                gsImage.Image.UndoGroupEnd();
            }
        }

        /// <summary>
        /// Resize the image by given factor and interpolation.
        /// </summary>
        [Serializable]
        public class Resize : GSImageFilter
        {
            // TODO: Maintain the same image size if resizing
            // back and forward with the same (inversely) factor.
            // Due to round errors the original and supersampled
            // image dimensions differ. Remember the original ones.

            /// <summary>
            /// Factor by which the image is scaled.
            /// </summary>
            public double Factor { get; set; }

            /// <summary>
            /// The direction of scaling. True - scale by the factor,
            /// false - scale by the factor inverse.
            /// </summary>
            public bool Forward { get; set; }

            /// <summary>
            /// The type of interpolation.
            /// </summary>
            public InterpolationType Interpolation { get; set; }

            public enum InterpolationType {
                NearestNeighbour,
                Bilinear,
                Bicubic,
                Lanczos
            }

            /// <summary>
            /// Create a new resize filter.
            /// </summary>
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

        /// <summary>
        /// Dot gain correction filter. There can be different implementations,
        /// so this is an abstarct class.
        /// </summary>
        [Serializable]
        public abstract class DotGainCorrection : GSImageFilter
        {
        }

        /// <summary>
        /// Dot gain correction approximated by the gamma curve.
        /// </summary>
        [Serializable]
        public class GammaCorrection : DotGainCorrection
        {
            private double _gamma;
            /// <summary>
            /// The parameter of gamma curve (0.1-10).
            /// </summary>
            public double Gamma {
                get { return _gamma; }
                set {
                    if ((value >= 0.1) & (value <= 10)) {
                        _gamma = value;
                    }
                }
            }

            /// <summary>
            /// Create a new gamma dot gain correction filter.
            /// </summary>
            public GammaCorrection() {
                Gamma = 1.0;
                runGSFilter = (GSImage image) =>
                {
                    image.Drawable.Levels(Gimp.HistogramChannel.Value,
                        0, 255, Gamma, 0, 255);
                };
            }
        }

        /// <summary>
        /// Sharpening filter.
        /// </summary>
        [Serializable]
        public class Sharpen : GSImageFilter
        {
            /// <summary>
            /// The ammount of sharpening (0.0-1.0).
            /// </summary>
            public double Amount { get; set; }

            /// <summary>
            /// Create a sharpening fitler.
            /// </summary>
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

        /// <summary>
        /// Smoothening filter. Currently implemented as Gaussian blur
        /// + Levels.
        /// </summary>
        [Serializable]
        public class Smoothen : GSImageFilter
        {
            /// <summary>
            /// Radius of bluring (>0).
            /// </summary>
            public double Radius { get; set; }

            /// <summary>
            /// Create a new smootening filter.
            /// </summary>
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
