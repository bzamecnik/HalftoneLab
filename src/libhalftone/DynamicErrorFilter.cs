namespace Halftone
{
    /// <summary>
    /// Dynamic error filter where error setting behaviour can be
    /// dependent on source pixel intensity.
    /// </summary>
    public interface DynamicErrorFilter
    {
        /// <summary>
        /// Diffuse quantization error value from source pixel to
        /// its neighbour pixels according to some specific rules with
        /// possible dependence on source pixel intensity.
        /// </summary>
        /// <param name="error">Quantization error to be diffused from
        /// the current pixel</param>
        /// <param name="intensity">Source pixel intensity (0-255)</param>
        void setError(double error, int intensity);
    }
}
