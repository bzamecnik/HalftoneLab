// TresholdDitherAlgorithm.cs created with MonoDevelop
// User: bohous at 15:04 26.3.2009
//

using System;
//using Gimp;

namespace Halftone
{
	
	public class TresholdDitherAlgorithm : DitherAlgorithm
	{
		// treshold filter
		TresholdFilter tresholdFilter;
		
        // error filter (optional)
		ErrorFilter errorFilter;

        ScanningOrder scanningOrder;
		
		public TresholdDitherAlgorithm(
            TresholdFilter tresholdFilter,
            ErrorFilter errorFilter,
            ScanningOrder scanningOrder
            )
		{
			this.tresholdFilter = tresholdFilter;
			this.errorFilter = errorFilter;
            this.scanningOrder = scanningOrder;
		}
		
		public TresholdDitherAlgorithm(TresholdFilter tresholdFilter)
		: this(tresholdFilter, null, new ScanlineScanningOrder())
		{
		}
		
		public override void run(Image input, Image output) {
            foreach (Coords coords in
                scanningOrder.getCoordsIterator(input.Width, input.Height))
            {
                // if there is an error filter:
                // TODO: error should be of 'double' type
                Pixel original = input[coords] + errorFilter.getError(coords);
                Pixel dithered = tresholdFilter.dither(original);
                errorFilter.setError(coords, original - dithered);
                output[coords] = dithered;

                // if there's no error filter:
                //output[coords] = tresholdFilter.dither(input[coords]);
            }
		}
	}
}
