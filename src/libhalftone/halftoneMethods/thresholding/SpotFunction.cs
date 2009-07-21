using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Halftone
{
    /// <summary>
    /// Spot functions define screening dot growth depending on intensity.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Spot functions define continuous periodic 2-D functions with values
    /// in range (0.0-1.0). They are parametrized by angle and period
    /// (distance).
    /// </para>
    /// <para>
    /// Technically spot functions are defined as prototypes using lambda
    /// functions. When parameters are known, they are 'instantiated' to
    /// other lambda functions as normal real value 2-D functions.
    /// </para>
    /// <para>
    /// To use a spot function you have to set SpotFuncPrototype property
    /// and optionally set Angle and Distance properties. Then the spot
    /// function is available in SpotFunc property.
    /// </para>
    /// </remarks>
    [Serializable]
    [Module(TypeName = "Spot function")]
    public class SpotFunction : Module
    {
        public delegate SpotFuncDelegate SpotFuncPrototypeDelegate(double angle, double distance);
        public delegate int SpotFuncDelegate(int x, int y);

        private double _angle = Math.PI * 0.25;
        
        /// <summary>
        /// Angle of rotation of the screen.
        /// </summary>
        /// <value>
        /// In radians. Default: 0.
        /// </value>
        public double Angle {
            get { return _angle; }
            set {
                _angle = value;
                initSpotFunction();
            }
        }
        private double _distance = 10;

        /// <summary>
        /// Distance between screen elements (inverse of frequency)
        /// = PPI (pixels per inch) / LPI (lines per inch).
        /// </summary>
        /// <value>
        /// In 2-D plane units corresponding to pixels.
        /// </value>
        public double Distance {
            get { return _distance; }
            set {
                _distance = value;
                initSpotFunction();
            }
        }

        private SpotFuncPrototypeDelegate _spotFuncPrototype;

        /// <summary>
        /// Spot function prototype. There you set the prototype and the
        /// finished spot function will be available in SpotFunc property.
        /// </summary>
        public SpotFuncPrototypeDelegate SpotFuncPrototype {
            get { return _spotFuncPrototype; }
            set {
                _spotFuncPrototype = value;
                initSpotFunction();
            }
        }

        [NonSerialized]
        private SpotFuncDelegate _spotFunc;

        /// <summary>
        /// Spot function made of a prototype which has been set to
        /// SpotFuncPrototype property.
        /// </summary>
        public SpotFuncDelegate SpotFunc {
            get {
                if (_spotFunc == null) {
                    initSpotFunction();
                }
                return _spotFunc;
            }
            private set { _spotFunc = value; }
        }

        /// <summary>
        /// Create a spot function with some default parameters.
        /// </summary>
        public SpotFunction()
            : this(SpotFunction.Samples.euclidDot.SpotFuncPrototype) {}

        public SpotFunction(SpotFuncPrototypeDelegate spotFuncPrototype) {
            SpotFuncPrototype = spotFuncPrototype;
        }

        /// <summary>
        /// Create a spot function given a prototype and some parameters.
        /// </summary>
        /// <param name="spotFuncPrototype">Spot function prototype</param>
        /// <param name="angle">Screen angle in radians (usually 0-2*PI but
        /// any real value will do the job)</param>
        /// <param name="distance">Distance between screen elements</param>
        public SpotFunction(SpotFuncPrototypeDelegate spotFuncPrototype,
            double angle, double distance)
        {
            Angle = angle;
            Distance = distance;
            SpotFuncPrototype = spotFuncPrototype;            
        }

        /// <summary>
        /// Initialize the spot function from its prototype.
        /// </summary>
        private void initSpotFunction() {
            if (SpotFuncPrototype != null) {
                SpotFunc = SpotFuncPrototype(Angle, Distance);
            }
        }

        public override void init(Image.ImageRunInfo imageRunInfo) {
            base.init(imageRunInfo);
            initSpotFunction();
        }

        /// <summary>
        /// Utility functions for SpotFunction.
        /// </summary>
        [Serializable]
        protected class Util
        {
            /// <summary>
            /// Rotate coordinates (inX, inY) by given angle to (outX, outY).
            /// </summary>
            /// <param name="inX">Input X coordinate</param>
            /// <param name="inY">Input Y coordinate</param>
            /// <param name="angle">Rotation angle in radians.</param>
            /// <param name="outX">Output X coordinate</param>
            /// <param name="outY">Output Y coordinate</param>
            public static void rotate(
                double inX, double inY,
                double angle,
                out double outX, out double outY)
            {
                double sin = Math.Sin(angle);
                double cos = Math.Cos(angle);
                outX = inX * sin + inY * cos;
                outY = -inX * cos + inY * sin;
            }
        }

        /// <summary>
        /// Samples of commonly used spot functions.
        /// </summary>
        [Serializable]
        public class Samples
        {
            public static SpotFunction euclidDot;
            public static SpotFunction perturbedEuclidDot;
            public static SpotFunction squareDot;
            public static SpotFunction line;
            public static SpotFunction triangle;
            public static SpotFunction circleDot;

            private static List<SpotFunction> _list;
            public static IEnumerable<SpotFunction> list() {
                return _list;
            }

            static Samples() {
                _list = new List<SpotFunction>();

                euclidDot = new SpotFunction((double angle, double distance) =>
                {
                    double pixelDivisor = 2.0 / distance;
                    return (int x, int y) =>
                    {
                        double rotatedX, rotatedY;
                        Util.rotate(
                            x * pixelDivisor, y * pixelDivisor,
                            angle, out rotatedX, out rotatedY);
                        return (int)(
                            255 * (0.5 - (0.25 * (
                            // to make an elipse multiply sin/cos
                            // arguments with different multipliers
                            Math.Sin(Math.PI * (rotatedX + 0.5)) +
                            Math.Cos(Math.PI * rotatedY)))));
                    };
                })
                {
                    Name = "Euclid dot",
                    Description = "circular dot changing to square at 50% grey"
                };
                _list.Add(euclidDot);

                perturbedEuclidDot = new SpotFunction(
                    (double angle, double distance) =>
                {
                    Random random = new Random();
                    double noiseAmplitude = 0.01;
                    double pixelDivisor = 2.0 / distance;
                    return (int x, int y) =>
                    {
                        double rotatedX, rotatedY;
                        SpotFunction.Util.rotate(
                            x * pixelDivisor, y * pixelDivisor,
                            angle, out rotatedX, out rotatedY);
                        return (int)(
                            255 * (
                            ((random.NextDouble() - 0.5) * 2 * noiseAmplitude) +
                            (0.5 - (0.25 * (
                            // to make an elipse multiply sin/cos
                            // arguments with different multipliers
                            Math.Sin(Math.PI * (rotatedX + 0.5)) +
                            Math.Cos(Math.PI * rotatedY))))));
                    };
                })
                {
                    Name = "Euclid dot, perturbed",
                    Description = "Euclid dot with some noise"
                };
                _list.Add(perturbedEuclidDot);

                squareDot = new SpotFunction((double angle, double distance) =>
                {
                    return (int x, int y) =>
                    {
                        double rotatedX, rotatedY;
                        SpotFunction.Util.rotate(x, y, angle, out rotatedX, out rotatedY);
                        return (int)(255 *
                            (1 - (0.5 * (
                            Math.Abs(((Math.Abs(rotatedX) / (distance * 0.5)) % 2) - 1) +
                            Math.Abs(((Math.Abs(rotatedY) / (distance * 0.5)) % 2) - 1)
                            ))));
                    };
                })
                {
                    Name = "Square dot"
                };
                _list.Add(squareDot);

                line = new SpotFunction((double angle, double distance) =>
                {
                    angle %= Math.PI * 0.5;
                    double invDistance = 1 / distance;
                    double sinInvDistance = Math.Sin(angle) * invDistance;
                    double cosInvDistance = Math.Cos(angle) * invDistance;

                    return (int x, int y) =>
                    {
                        double value = (sinInvDistance * x) + (cosInvDistance * y);
                        return (int)(255 * (Math.Abs(
                            Math.Sign(value) * value % Math.Sign(value))));
                    };
                })
                {
                    Name = "Line"
                };
                _list.Add(line);

                triangle = new SpotFunction((double angle, double distance) =>
                {
                    double invDistance = 1 / distance;

                    return (int x, int y) =>
                    {
                        return (int)(255 * 0.5 * (
                        ((invDistance * x) % 1) +
                        ((invDistance * y) % 1)
                        ));
                    };
                })
                {
                    Name = "Triangle"
                };
                _list.Add(triangle);

                circleDot = new SpotFunction((double angle, double distance) =>
                {
                    double invDistance = 1 / distance;

                    return (int x, int y) =>
                    {
                        double xSq = x - distance * 0.5;
                        xSq = (xSq * xSq) % distance;
                        double ySq = y - distance * 0.5;
                        ySq = (ySq * ySq) % distance;
                        return (int)(255 *
                            //((1 - Math.Sqrt(
                            //(invDistance * (x*x) % 1) +
                            //(invDistance * (y*y) % 1)
                            //) * 0.5)));
                            ((1 - invDistance * Math.Sqrt(2*(xSq + ySq)))));
                    };
                })
                {
                    Name = "Circle dot"
                };
                _list.Add(circleDot);
            }
        }
    }
}
