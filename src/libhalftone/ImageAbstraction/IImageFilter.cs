// Copyright (c) 2009 Bohumir Zamecnik <bohumir@zamecnik.org>
// License: The MIT License, see the LICENSE file

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
