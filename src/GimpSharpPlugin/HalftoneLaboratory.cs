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

            //ScanningOrder scanningOrder = new HilbertScanningOrder();
            //scanningOrder.init(image.Width, image.Height);
            //while (scanningOrder.hasNext()) {
            //    scanningOrder.next();
            //    Console.Out.WriteLine("x: {0}, y: {1}", scanningOrder.CurrentX, scanningOrder.CurrentY);
            //}

            //IEnumerator<Coordinate<int>> coordsEnum =
            //    scanningOrder.getCoordsEnumerable().GetEnumerator();
            //while (coordsEnum.MoveNext()) {
            //    Console.Out.WriteLine("x: {0}, y: {1}", coordsEnum.Current.X, coordsEnum.Current.Y);
            //}

            //return;

            //DynamicTresholdFilter tresholdFilter = new DynamicTresholdFilter();
            //tresholdFilter.NoiseEnabled = true;
            //tresholdFilter.addTresholdRecord(0, TresholdMatrix.Generator.sampleMatrix, 0.0);
            //tresholdFilter.addTresholdRecord(32, TresholdMatrix.Generator.sampleMatrix, 0.125);
            //tresholdFilter.addTresholdRecord(64, TresholdMatrix.Generator.sampleMatrix, 0.25);
            //tresholdFilter.addTresholdRecord(96, TresholdMatrix.Generator.sampleMatrix, 0.375);
            //tresholdFilter.addTresholdRecord(128, TresholdMatrix.Generator.sampleMatrix, 0.5);
            //tresholdFilter.addTresholdRecord(160, TresholdMatrix.Generator.sampleMatrix, 0.625);
            //tresholdFilter.addTresholdRecord(192, TresholdMatrix.Generator.sampleMatrix, 0.75);
            //tresholdFilter.addTresholdRecord(224, TresholdMatrix.Generator.sampleMatrix, 0.875);
            
            //tresholdFilter.addTresholdRecord(100, TresholdMatrix.Generator.simpleTreshold);
            //tresholdFilter.addTresholdRecord(150, TresholdMatrix.Generator.createBayerDispersedDotMatrix(3));
            TresholdFilter tresholdFilter = new MatrixTresholdFilter(
                TresholdMatrix.Generator.simpleTreshold);
                //TresholdMatrix.Generator.sampleMatrix);
            //ScanningOrder scanOrder = new ScanlineScanningOrder();
            ScanningOrder scanOrder = new HilbertScanningOrder();
            //ErrorFilter errorFilter = null;
            ErrorFilter errorFilter = new VectorErrorFilter(
                new ErrorMatrix(new double[1,5] {{0, 2, 2, 1, 1}}, 0));
            //ErrorFilter errorFilter = //new PerturbedErrorFilter(
            //    new MatrixErrorFilter(
            //        ErrorMatrix.Samples.floydSteinberg,
            //        ErrorBuffer.createFromScanningOrder(
            //        scanOrder, errorMatrix.Height, image.Width));
            TresholdDitherAlgorithm alg = new TresholdDitherAlgorithm(
                tresholdFilter, errorFilter, scanOrder);

            //	new MatrixTresholdFilter(MatrixTresholdFilter.Generator.createBayerDispersedDotMatrix(4)));
            //    new MatrixTresholdFilter(MatrixTresholdFilter.sampleMatrix));
            //TresholdDitherAlgorithm alg = new TresholdDitherAlgorithm(new RandomTresholdFilter());
            alg.run(image);
        }
    }
}
