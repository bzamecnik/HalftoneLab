using System;

namespace Halftone
{
    /// <summary>
    /// Error diffusion filter module. This kind of filter diffuses quantization 
    /// error to current pixel neighbourhood.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Quantization error is a difference between original pixel value
    /// (plus any error diffused from other pixels) and output
    /// pixel value. Error diffusion in general greatly improves overall
    /// image quality, especially in details. However, the effect heavily
    /// depends on the actual error filter.
    /// </para>
    /// <para>
    /// Descendants of this class define behaviour how the error is diffused
    /// and usually use a kind of internal buffer. The internal buffer
    /// usually holds a current position. getError() and setError() functions
    /// use this position. moveNext() function advances to next position.
    /// To clear the error filter for further use initialize it again with
    /// init().
    /// </para>
    /// <para>
    /// Note: The internal buffer (if the is any) is initialized according
    /// to some image-dependent parameters using virutal
    /// init(Image.ImageRunInfo). Initialized property is set to true
    /// if the buffer has been initialized successfully.
    /// </para>
    /// </remarks>
    [Serializable]
    public abstract class ErrorFilter : Module
	{
        /// <summary>
        /// Indicates being successful prepared after Module.init() function
        /// call. This usually means that an internal error buffer is alright.
        /// </summary>
        public bool Initialized { get; protected set; }

        /// <summary>
        /// ErrorFilter constructor. Error filter must be initialized
        /// via init() function before it can be used.
        /// </summary>
        public ErrorFilter() {
            Initialized = false;
        }

        /// <summary>
        /// <para>
        /// Get accumulated error value for given pixel.
        /// </para>
        /// <para>
        /// Pixel intensity is of intensity scale 0.0-255.0. However,
        /// the actual error value can lie outside this interval.
        /// </para>
        /// </summary>
        /// <returns>Accumulated error for current pixel position.</returns>
        public abstract double getError();
        
        /// <summary>
        /// <para>
        /// Diffuse quantization error value from current pixel to
        /// its neighbour pixels according to some specific rules.
        /// </para>
        /// <para>
        /// Pixel intensity is of intensity scale 0.0-255.0. However,
        /// the actual error value can lie outside this interval.
        /// </para>
        /// </summary>
        /// <param name="error">Quantization error to be diffused from
        /// the current pixel</param>
        public abstract void setError(double error);

        /// <summary>
        /// Move to next pixel position. Tell the buffer to move.
        /// It might use different scanning order strategies.
        /// </summary>
        public abstract void moveNext();
	}
}
