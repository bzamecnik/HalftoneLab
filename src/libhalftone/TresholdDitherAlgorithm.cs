// TresholdDitherAlgorithm.cs created with MonoDevelop
// User: bohous at 15:04 26.3.2009
//

using System;
using Gimp;

namespace Halftone
{
	
	public class TresholdDitherAlgorithm : DitherAlgorithm
	{
		// treshold filter
		TresholdFilter tresholdFilter;
		// error filter (optional)
		ErrorFilter errorFilter;
		
		public TresholdDitherAlgorithm(TresholdFilter tresholdFilter, ErrorFilter errorFilter)
		{
			this.tresholdFilter = tresholdFilter;
			this.errorFilter = errorFilter;
		}
		
		public TresholdDitherAlgorithm(TresholdFilter tresholdFilter)
		: this(tresholdFilter, null)
		{
		}
		
		public override void run(Gimp.Drawable drawable) {
			RgnIterator iter = new RgnIterator(drawable, RunMode.Interactive);
			iter.Progress = new Progress("Treshold Dither Algorithm");
			iter.IterateSrcDest(pixel => tresholdFilter.dither(pixel));
		}
	}
}
