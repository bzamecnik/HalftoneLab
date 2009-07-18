using System;

namespace Halftone
{
    public interface IImageFilter
    {
        void run(Image image);
    }

    [Serializable]
    public class GSImageFilter : Module, IImageFilter
    {
        public void run(Image image) {
            GSImage gsImage = image as GSImage;
            if ((gsImage != null) && (runGSFilter != null)) {
                runGSFilter(gsImage);
                gsImage.Drawable.Flush();
                gsImage.Drawable.Update();
            }
        }

        public delegate void GSFilter(GSImage image);

        public GSFilter runGSFilter;
    }
}
