using System;

namespace HalftoneLab
{
    [Serializable]
    public class GSImageFilter : Module, IImageFilter
    {
        public delegate void GSFilter(GSImage image);

        public GSFilter runGSFilter;

        public void run(Image image) {
            GSImage gsImage = image as GSImage;
            if ((gsImage != null) && (runGSFilter != null)) {
                runGSFilter(gsImage);
                gsImage.Drawable.Flush();
                gsImage.Drawable.Update();
            }
        }
    }
}
