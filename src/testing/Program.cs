using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Halftone;

namespace testing
{
    class Program
    {
        static void Main(string[] args) {
            ConfigManagerTest.run();
        }
    }

    class ConfigManagerTest {
        public static void run() {
            ConfigManager config = new ConfigManager()
            {
                ConfigFileName = "halftonelab.cfg"
            };
            config.load();
            printConfig(config);

            ScanningOrder scanlineScanOrder = new ScanlineScanningOrder()
            {
                Name = "Scanline",
                Description = "Go in rows - left to right, top down"
            };
            config.saveModule(scanlineScanOrder, false);

            ScanningOrder serpentineScanOrder = new SerpentineScanningOrder()
            {
                Name = "Serpentine",
                Description = "Go in rows - zig-zag, top down"
            };
            config.saveModule(serpentineScanOrder, false);

            ScanningOrder hilbertScanOrder = new HilbertScanningOrder()
            {
                Name = "Hilbert",
                Description = "Hilbert Space-Filling Curve"
            };
            config.saveModule(hilbertScanOrder, false);


            ErrorMatrix floydSteinbergErrorMatrix = ErrorMatrix.Samples.floydSteinberg;
            floydSteinbergErrorMatrix.Name = "Floyd-Steinberg";
            config.saveModule(floydSteinbergErrorMatrix, false);

            TresholdMatrix bayerTresholdMatrix =
                TresholdMatrix.Generator.createBayerDispersedDotMatrix(2);
            bayerTresholdMatrix.Name = "Bayer matrix 4x4";
            config.saveModule(bayerTresholdMatrix, false);

            MatrixErrorFilter matrixErrorFilter = new MatrixErrorFilter(floydSteinbergErrorMatrix)
            {
                Name = "Floyd-Steinberg filter"
            };
            config.saveModule(matrixErrorFilter, false);

            PerturbedErrorFilter perturbedErrorFilter = new PerturbedErrorFilter(matrixErrorFilter)
            {
                PerturbationAmplitude = 0.5,
                Name = "Perturbed Floyd-Steinberg"
            };
            config.saveModule(perturbedErrorFilter, false);

            RandomizedMatrixErrorFilter randomizedMatrixErrorFilter =
                new RandomizedMatrixErrorFilter(floydSteinbergErrorMatrix)
            {
                Name = "Randomized error filter"
            };
            config.saveModule(randomizedMatrixErrorFilter, false);

            VectorErrorFilter vectorErrorFilter =
                new VectorErrorFilter(ErrorMatrix.Samples.nextPixel)
            {
                Name = "Vector error filter",
                Description = "Whole error goes to next pixel"
            };
            config.saveModule(vectorErrorFilter, false);

            DynamicMatrixErrorFilter dynamicMatrixErrorFilter = new DynamicMatrixErrorFilter()
            {
                Name = "Dynamic error filter"
            };
            dynamicMatrixErrorFilter.addRecord(0, ErrorMatrix.Samples.floydSteinberg);
            dynamicMatrixErrorFilter.addRecord(64, ErrorMatrix.Samples.jarvisJudiceNinke);
            dynamicMatrixErrorFilter.addRecord(128, ErrorMatrix.Samples.stucki);
            dynamicMatrixErrorFilter.addRecord(192, ErrorMatrix.Samples.nextPixel);
            config.saveModule(dynamicMatrixErrorFilter, false);

            MatrixTresholdFilter matrixTresholdFilter = new MatrixTresholdFilter(bayerTresholdMatrix)
            {
                Name = "Bayer filter"
            };
            config.saveModule(matrixTresholdFilter, false);

            DynamicTresholdFilter dynamicTresholdFilter = new DynamicTresholdFilter()
            {
                NoiseEnabled = true,
                Name = "Dynamic treshold filter"
            };
            dynamicTresholdFilter.addTresholdRecord(0, TresholdMatrix.Generator.sampleMatrix, 0.0);
            dynamicTresholdFilter.addTresholdRecord(32, TresholdMatrix.Generator.sampleMatrix, 0.125);
            dynamicTresholdFilter.addTresholdRecord(64, TresholdMatrix.Generator.sampleMatrix, 0.25);
            dynamicTresholdFilter.addTresholdRecord(96, TresholdMatrix.Generator.sampleMatrix, 0.375);
            dynamicTresholdFilter.addTresholdRecord(128, TresholdMatrix.Generator.sampleMatrix, 0.5);
            dynamicTresholdFilter.addTresholdRecord(160, TresholdMatrix.Generator.sampleMatrix, 0.625);
            dynamicTresholdFilter.addTresholdRecord(192, TresholdMatrix.Generator.sampleMatrix, 0.75);
            dynamicTresholdFilter.addTresholdRecord(224, TresholdMatrix.Generator.sampleMatrix, 0.875);

            //dynamicTresholdFilter.addTresholdRecord(100, TresholdMatrix.Generator.simpleTreshold);
            //dynamicTresholdFilter.addTresholdRecord(150, TresholdMatrix.Generator.createBayerDispersedDotMatrix(3));
            config.saveModule(dynamicTresholdFilter, false);

            TresholdDitherAlgorithm tresholdDitherAlgorithm = new TresholdDitherAlgorithm(
                matrixTresholdFilter, matrixErrorFilter, scanlineScanOrder)
            {
                Name = "Treshold dither algorithm",
                Description = "Floyd-Steinberg error, Bayer treshold, Scanline order"
            };
            config.saveModule(tresholdDitherAlgorithm, false);

            tresholdDitherAlgorithm.ScanningOrder = hilbertScanOrder;
            tresholdDitherAlgorithm.ErrorFilter = vectorErrorFilter;
            tresholdDitherAlgorithm.Name = "SFC Treshold dither algorithm";
            tresholdDitherAlgorithm.Description = "Hilbert SFC order, next pixel vector error filter";
            config.saveModule(tresholdDitherAlgorithm, false);

            config.save();

            printConfig(config);

            //config.deleteModule(typeof(HilbertScanningOrder), "Hilbert");
            
        }

        public static void printConfig(ConfigManager config) {
            foreach (Module module in config.listAllModules()) {
                Console.Out.WriteLine("type: {0}, name: {1}, desc: {2}",
                    module.GetType(), module.Name, module.Description);
            }
        }
    }
}
