// RandomDitherAlgorithm.cs created with MonoDevelop
// User: bohous at 21:16 26.3.2009
//

using System;
//using Gimp;


namespace Halftone
{
	public class RandomTresholdFilter : TresholdFilter
	{
		Random rnd;
		public RandomTresholdFilter()
		{
			rnd = new Random();
		}
		
		public override int treshold(Pixel pixel)
		{
			return rnd.Next(255);
		}
	}
}
