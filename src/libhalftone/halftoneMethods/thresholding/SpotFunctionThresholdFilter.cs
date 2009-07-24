using System;

namespace HalftoneLab
{
    /// <summary>
    /// Threshold filter where threshold values are computed directly using
    /// a spot function.
    /// </summary>
    /// <see cref="SpotFunction"/>
    [Serializable]
    [Module(TypeName = "Spot function threshold filter")]
    public class SpotFunctionThresholdFilter : ThresholdFilter
    {
        /// <summary>
        /// Spot function to use for computing the threshold.
        /// </summary>
        public SpotFunction SpotFunc {get; set;}

        /// <summary>
        /// Create a new spot function threshold filter with a default
        /// spot function.
        /// </summary>
        public SpotFunctionThresholdFilter() {
            SpotFunc = new SpotFunction();
        }

        /// <summary>
        /// Create a new spot function threshold filter with given spot
        /// function.
        /// </summary>
        /// <param name="spotFunc"></param>
        public SpotFunctionThresholdFilter(SpotFunction spotFunc) {
            SpotFunc = spotFunc;
        }

        protected override int threshold(int intensity, int x, int y) {
            return SpotFunc.SpotFunc(x, y);
        }

        public override void init(HalftoneLab.Image.ImageRunInfo imageRunInfo) {
            base.init(imageRunInfo);
            SpotFunc.init(imageRunInfo);
        }
    }
}
