using System;

namespace Halftone
{
    /// <summary>
    /// Treshold filter where treshold values are computed directly using
    /// a spot function.
    /// </summary>
    [Serializable]
    public class SpotFunctionTresholdFilter : TresholdFilter
    {
        public SpotFunction SpotFunc {get; set;}

        public SpotFunctionTresholdFilter() {
            SpotFunc = SpotFunction.createDefault();
        }

        public SpotFunctionTresholdFilter(SpotFunction spotFunc) {
            SpotFunc = spotFunc;
        }

        protected override int treshold(int intensity, int x, int y) {
            return SpotFunc.SpotFunc(x, y);
        }
    }
}
