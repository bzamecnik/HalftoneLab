using System;

namespace Halftone
{
    public interface IImageFilter
    {
        void run(Image image);
    }
}
