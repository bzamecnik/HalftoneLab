using System;

namespace Halftone
{
    /// <summary>
    /// Threshold filter where threshold values are computed directly using
    /// a spot function.
    /// </summary>
    [Serializable]
    public class SpotFunctionThresholdFilter : ThresholdFilter
    {
        public SpotFunction SpotFunc {get; set;}

        public SpotFunctionThresholdFilter() {
            SpotFunc = SpotFunction.createDefault();
        }

        public SpotFunctionThresholdFilter(SpotFunction spotFunc) {
            SpotFunc = spotFunc;
        }

        protected override int threshold(int intensity, int x, int y) {
            return SpotFunc.SpotFunc(x, y);
        }
    }
}
