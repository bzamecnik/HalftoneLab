// Main.cs created with MonoDevelop
// User: bohous at 17:01 26.3.2009
//
using System;
using System.Collections.Generic;
using Halftone;

namespace Gimp.HalftoneLaboratory
{
	class HalftoneLaboratory : Plugin
	{
		public static void Main(string[] args)
		{
			new HalftoneLaboratory(args);
		}

	    HalftoneLaboratory(string[] args) : base(args, "HalftoneLaboratory")
	    {
	    }

	    override protected IEnumerable<Procedure> ListProcedures()
	    {
			yield return new Procedure("plug_in_halftone_laboratory",
					_("Halftone Laboratory"),
					_("Halftone Laboratory"),
					"Bohumir Zamecnik",
					"(C) Bohumir Zamecnik",
					"2009",
					_("Halftone Laboratory"),
					"GRAY*")
			{MenuPath = "<Image>/Filters/Distorts"};
	    }

	    override protected void Render(Drawable drawable)
	    {
			TresholdDitherAlgorithm alg = new TresholdDitherAlgorithm(
				new MatrixTresholdFilter(MatrixTresholdFilter.Generator.createDispersedDotMatrix(4)));
			//new MatrixTresholdFilter(MatrixTresholdFilter.sampleMatrix));
			//TresholdDitherAlgorithm alg = new TresholdDitherAlgorithm(new RandomTresholdFilter());
			alg.run(drawable);
		}		
	}
}
