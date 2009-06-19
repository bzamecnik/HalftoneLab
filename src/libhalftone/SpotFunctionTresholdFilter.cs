using System;

namespace Halftone
{
    [Serializable]
    public class SpotFunctionTresholdFilter : TresholdFilter
    {
        public SpotFunction SpotFunc {get; set;}

        public SpotFunctionTresholdFilter() {
            SpotFunc = new SpotFunction() {
                Angle = Math.PI * 0.25,
                Distance = 32,
                SpotFuncPrototype = Samples.euclidDot
            };
        }

        protected override int treshold(int intensity, int x, int y) {
            return SpotFunc.SpotFunc(x, y);
        }

        [Serializable]
        public class SpotFunction {
            public delegate SpotFuncDelegate SpotFuncPrototypeDelegate(double angle, int distance);
            public delegate int SpotFuncDelegate(int x, int y);

            private double _angle = 0;
            public double Angle {
                get { return _angle; }
                set {
                    _angle = value;
                    initSpotFunction();
                }
            }
            private int _distance = 10;
            public int Distance {
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

            private void initSpotFunction() {
                if (SpotFuncPrototype != null) {
                    SpotFunc = SpotFuncPrototype(Angle, Distance);
                }
            }

            [Serializable]
            public class Util {
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
        }

        [Serializable]
        public class Samples
        {
            public static SpotFunction.SpotFuncPrototypeDelegate euclidDot;
            public static SpotFunction.SpotFuncPrototypeDelegate squareDot;
            static Samples() {
                euclidDot = (double angle, int distance) =>
                {
                    double pixelDivisor = 2.0 / distance;
                    return (int x, int y) =>
                    {
                        double rotatedX, rotatedY;
                        SpotFunction.Util.rotate(
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

                squareDot = (double angle, int distance) =>
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
            }
        }
    }
}
