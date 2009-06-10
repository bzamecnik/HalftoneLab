namespace Halftone
{
    // Error diffusion filter
    public abstract class ErrorFilter : Module
	{
        // get accumulated error value for given pixel
        public abstract double getError();
        
        // diffuse error value from given pixel to neighbor pixels
        public abstract void setError(double error);

        // move to next pixel (according to the scanning order, ie. error buffer type)
        public abstract void moveNext();

        // initialize internal buffer according to some image-dependent parameters
        // return true, if buffer initialized successfully
        public abstract bool initBuffer(ScanningOrder scanningOrder,
            int imageHeight, int imageWidth);
	}
}
