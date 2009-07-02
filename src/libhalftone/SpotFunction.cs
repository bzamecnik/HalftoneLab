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
    /// 
    /// </remarks>
    [Serializable]
    public class SpotFunction : Module
    {
        public delegate SpotFuncDelegate SpotFuncPrototypeDelegate(double angle, double distance);
        public delegate int SpotFuncDelegate(int x, int y);

        private double _angle = 0;
        public double Angle {
            get { return _angle; }
            set {
                _angle = value;
                initSpotFunction();
            }
        }
        private double _distance = 10;
        public double Distance {
            get { return _distance; }
            set {
                _distance = value;
                initSpotFunction();
            }
        }

        private SpotFuncPrototypeDelegate _spotFuncPrototype;
        public SpotFuncPrototypeDelegate SpotFuncPrototype {
            get { return _spotFuncPrototype; }
            set {
                _spotFuncPrototype = value;
                initSpotFunction();
            }
        }

        [NonSerialized]
        private SpotFuncDelegate _spotFunc;
        public SpotFuncDelegate SpotFunc {
            get {
                if (_spotFunc == null) {
                    initSpotFunction();
                }
                return _spotFunc;
            }
            private set { _spotFunc = value; }
        }

        public SpotFunction(SpotFuncPrototypeDelegate spotFuncPrototype,
            double angle, double distance)
        {
            Angle = angle;
            Distance = distance;
            SpotFuncPrototype = spotFuncPrototype;            
        }

        public static SpotFunction createDefault()
        {
            return new SpotFunction(
                SpotFunction.Samples.euclidDot, Math.PI * 0.25, 8);
        }

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
            /// Rotate coordinates (inX, inY) by a given angle to
            /// (outX, outY).
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
            public static SpotFuncPrototypeDelegate euclidDot;
            public static SpotFuncPrototypeDelegate perturbedEuclidDot;
            public static SpotFuncPrototypeDelegate squareDot;
            public static SpotFuncPrototypeDelegate line;
            public static SpotFuncPrototypeDelegate triangle;
            public static SpotFuncPrototypeDelegate circleDot;

            static Samples() {
                euclidDot = (double angle, double distance) =>
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
                };

                perturbedEuclidDot = (double angle, double distance) =>
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
                };

                squareDot = (double angle, double distance) =>
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
                };

                line = (double angle, double distance) =>
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
                };

                triangle = (double angle, double distance) =>
                {
                    double invDistance = 1 / distance;

                    return (int x, int y) =>
                    {
                        return (int)(255 * 0.5 * (
                        ((invDistance * x) % 1) +
                        ((invDistance * y) % 1)
                        ));
                    };
                };

                circleDot = (double angle, double distance) =>
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
                };       
            }
        }
    }
}

