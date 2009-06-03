// Main.cs created with MonoDevelop
// User: bohous at 17:01 26.3.2009
//
using System;
using System.Collections.Generic;
using Halftone;

namespace Gimp.HalftoneLab
{
    class HalftoneLaboratory : Plugin
    {
        public static void Main(string[] args) {
            new HalftoneLaboratory(args);
        }

        HalftoneLaboratory(string[] args)
            : base(args, "HalftoneLaboratory") {
        }

        override protected IEnumerable<Procedure> ListProcedures() {
            yield return new Procedure("plug_in_halftone_laboratory",
                    _("Halftone Laboratory"),
                    _("Halftone Laboratory"),
                    "Bohumir Zamecnik",
                    "(C) Bohumir Zamecnik",
                    "2009",
                    _("Halftone Laboratory"),
                    "GRAY*") { MenuPath = "<Image>/Filters/Distorts" };
        }

        override protected void Render(Drawable drawable) {
            Halftone.Image image = new GSImage(drawable);

            TresholdFilter tresholdFilter = new MatrixTresholdFilter(
                new TresholdMatrix(new int[1, 1] { { 128 } }));
                //TresholdMatrix.Generator.sampleMatrix);
            ScanningOrder scanOrder = new ScanlineScanningOrder();
            //ErrorFilter errorFilter = null;
            ErrorMatrix errorMatrix = new ErrorMatrix(
                new double[2, 3] { { 0, 0, 7 }, { 3, 5, 1 } }, 1); // Floyd-Steinberg
                //new double[2, 3] { { 0, 0, 1 }, {1, 1, 1} }, 1);
            ErrorFilter errorFilter = //new PerturbedErrorFilter(
                new RandomizedMatrixErrorFilter(
                    errorMatrix,
                    ErrorBuffer.createFromScanningOrder(
                    scanOrder, errorMatrix.Height, image.Width));
            TresholdDitherAlgorithm alg = new TresholdDitherAlgorithm(
                tresholdFilter, errorFilter, scanOrder);

            //	new MatrixTresholdFilter(MatrixTresholdFilter.Generator.createBayerDispersedDotMatrix(4)));
            //    new MatrixTresholdFilter(MatrixTresholdFilter.sampleMatrix));
            //TresholdDitherAlgorithm alg = new TresholdDitherAlgorithm(new RandomTresholdFilter());
            alg.run(image);
        }
    }
}
