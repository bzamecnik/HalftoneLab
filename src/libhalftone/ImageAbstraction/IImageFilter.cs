namespace HalftoneLab
{
    /// <summary>
    /// Interface for image filters.
    /// </summary>
    public interface IImageFilter
    {
        /// <summary>
        /// Perform an operation on the image.
        /// </summary>
        /// <param name="image">Image acting both as input and output</param>
        void run(Image image);
    }
}
