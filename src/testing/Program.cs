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
            //config.saveModule(scanlineScanOrder, false);

            ScanningOrder serpentineScanOrder = new SerpentineScanningOrder()
            {
                Name = "Serpentine",
                Description = "Go in rows - zig-zag, top down"
            };
            //config.saveModule(serpentineScanOrder, false);

            ScanningOrder hilbertScanOrder = new HilbertScanningOrder()
            {
                Name = "Hilbert",
                Description = "Hilbert Space-Filling Curve"
            };
            //config.saveModule(hilbertScanOrder, false);


            ErrorMatrix floydSteinbergErrorMatrix = ErrorMatrix.Samples.floydSteinberg;
            floydSteinbergErrorMatrix.Name = "Floyd-Steinberg";
            //config.saveModule(floydSteinbergErrorMatrix, false);

            ThresholdMatrix bayerThresholdMatrix =
                ThresholdMatrix.Generator.createBayerDispersedDotMatrix(3);
            bayerThresholdMatrix.Name = "Bayer matrix 8x8";
            //config.saveModule(bayerThresholdMatrix, false);

            MatrixErrorFilter matrixErrorFilter = new MatrixErrorFilter(floydSteinbergErrorMatrix)
            {
                Name = "Floyd-Steinberg filter"
            };
            //config.saveModule(matrixErrorFilter, false);

            PerturbedErrorFilter perturbedErrorFilter = new PerturbedErrorFilter(matrixErrorFilter)
            {
                PerturbationAmplitude = 0.5,
                Name = "Perturbed Floyd-Steinberg"
            };
            //config.saveModule(perturbedErrorFilter, false);

            RandomizedMatrixErrorFilter randomizedMatrixErrorFilter =
                new RandomizedMatrixErrorFilter(floydSteinbergErrorMatrix)
            {
                Name = "Randomized error filter"
            };
            //config.saveModule(randomizedMatrixErrorFilter, false);

            VectorErrorFilter vectorErrorFilter =
                new VectorErrorFilter(ErrorMatrix.Samples.nextPixel)
            {
                Name = "Vector error filter",
                Description = "Whole error goes to next pixel"
            };
            //config.saveModule(vectorErrorFilter, false);

            DynamicMatrixErrorFilter dynamicMatrixErrorFilter = new DynamicMatrixErrorFilter()
            {
                Name = "Dynamic error filter"
            };
            dynamicMatrixErrorFilter.addRecord(0, ErrorMatrix.Samples.floydSteinberg);
            dynamicMatrixErrorFilter.addRecord(64, ErrorMatrix.Samples.jarvisJudiceNinke);
            dynamicMatrixErrorFilter.addRecord(128, ErrorMatrix.Samples.stucki);
            dynamicMatrixErrorFilter.addRecord(192, ErrorMatrix.Samples.nextPixel);
            //config.saveModule(dynamicMatrixErrorFilter, false);

            MatrixThresholdFilter matrixThresholdFilter = new MatrixThresholdFilter(bayerThresholdMatrix)
            {
                Name = "Bayer filter"
            };
            //config.saveModule(matrixThresholdFilter, false);

            DynamicThresholdFilter dynamicThresholdFilter = new DynamicThresholdFilter()
            {
                NoiseEnabled = true,
                Name = "Dynamic threshold filter"
            };
            dynamicThresholdFilter.addThresholdRecord(0, ThresholdMatrix.Generator.sampleMatrix, 0.0);
            dynamicThresholdFilter.addThresholdRecord(32, ThresholdMatrix.Generator.sampleMatrix, 0.125);
            dynamicThresholdFilter.addThresholdRecord(64, ThresholdMatrix.Generator.sampleMatrix, 0.25);
            dynamicThresholdFilter.addThresholdRecord(96, ThresholdMatrix.Generator.sampleMatrix, 0.375);
            dynamicThresholdFilter.addThresholdRecord(128, ThresholdMatrix.Generator.sampleMatrix, 0.5);
            dynamicThresholdFilter.addThresholdRecord(160, ThresholdMatrix.Generator.sampleMatrix, 0.625);
            dynamicThresholdFilter.addThresholdRecord(192, ThresholdMatrix.Generator.sampleMatrix, 0.75);
            dynamicThresholdFilter.addThresholdRecord(224, ThresholdMatrix.Generator.sampleMatrix, 0.875);

            dynamicThresholdFilter.addThresholdRecord(100, ThresholdMatrix.Generator.simpleThreshold);
            dynamicThresholdFilter.addThresholdRecord(150, ThresholdMatrix.Generator.createBayerDispersedDotMatrix(3));

            //config.saveModule(dynamicThresholdFilter, false);

            ThresholdDitherAlgorithm thresholdDitherAlgorithm = new ThresholdDitherAlgorithm()
            {
                Name = "Threshold dither algorithm"
            };
            config.saveModule(thresholdDitherAlgorithm, false);

            thresholdDitherAlgorithm.ErrorFilter = matrixErrorFilter;
            thresholdDitherAlgorithm.ScanningOrder = scanlineScanOrder;
            thresholdDitherAlgorithm.Name = "Floyd-Steinberg, scanline";
            thresholdDitherAlgorithm.Description = "Floyd-Steinberg error, Scanline order";
            config.saveModule(thresholdDitherAlgorithm, false);

            thresholdDitherAlgorithm.ScanningOrder = serpentineScanOrder;
            thresholdDitherAlgorithm.Name = "Floyd-Steinberg, serpentine";
            thresholdDitherAlgorithm.Description = "Floyd-Steinberg error, Serpentine order";
            config.saveModule(thresholdDitherAlgorithm, false);

            thresholdDitherAlgorithm.ScanningOrder = scanlineScanOrder;
            thresholdDitherAlgorithm.ErrorFilter = null;
            thresholdDitherAlgorithm.ThresholdFilter = matrixThresholdFilter;
            thresholdDitherAlgorithm.Name = "Bayer dispersed dither";
            config.saveModule(thresholdDitherAlgorithm, false);

            thresholdDitherAlgorithm.ThresholdFilter = new MatrixThresholdFilter(ThresholdMatrix.Generator.sampleMatrix);
            thresholdDitherAlgorithm.Name = "Clustered dot dither";
            config.saveModule(thresholdDitherAlgorithm, false);

            thresholdDitherAlgorithm.ThresholdFilter = dynamicThresholdFilter;
            thresholdDitherAlgorithm.Name = "Dynamic threshold dither";
            config.saveModule(thresholdDitherAlgorithm, false);

            dynamicThresholdFilter.clearThresholdRecords();
            dynamicThresholdFilter.addThresholdRecord(0, ThresholdMatrix.Generator.simpleThreshold, 1.0);
            thresholdDitherAlgorithm.ThresholdFilter = dynamicThresholdFilter;
            thresholdDitherAlgorithm.Name = "Simpe blue-noise";
            config.saveModule(thresholdDitherAlgorithm, false);

            thresholdDitherAlgorithm.ErrorFilter = matrixErrorFilter;
            thresholdDitherAlgorithm.Name = "Blue-noise Floyd-Steinberg, scanline";
            config.saveModule(thresholdDitherAlgorithm, false);

            thresholdDitherAlgorithm.ScanningOrder = serpentineScanOrder;
            thresholdDitherAlgorithm.Name = "Blue-noise, Floyd-Steinberg, serpentine";
            config.saveModule(thresholdDitherAlgorithm, false);

            thresholdDitherAlgorithm.ScanningOrder = scanlineScanOrder;
            thresholdDitherAlgorithm.ThresholdFilter = new MatrixThresholdFilter();

            //thresholdDitherAlgorithm.ErrorFilter = dynamicMatrixErrorFilter;
            //thresholdDitherAlgorithm.Name = "Dynamic error filter";
            //config.saveModule(thresholdDitherAlgorithm, false);

            //thresholdDitherAlgorithm.ErrorFilter = perturbedErrorFilter;
            //thresholdDitherAlgorithm.Name = "Perturbed error filter";
            //config.saveModule(thresholdDitherAlgorithm, false);

            thresholdDitherAlgorithm.ErrorFilter = randomizedMatrixErrorFilter;
            thresholdDitherAlgorithm.Name = "Randomized error filter";
            config.saveModule(thresholdDitherAlgorithm, false);

            thresholdDitherAlgorithm.ScanningOrder = hilbertScanOrder;
            thresholdDitherAlgorithm.ErrorFilter = vectorErrorFilter;
            thresholdDitherAlgorithm.Name = "SFC Threshold dither algorithm";
            thresholdDitherAlgorithm.Description = "Hilbert SFC order, next pixel vector error filter";
            config.saveModule(thresholdDitherAlgorithm, false);

            SFCAdaptiveClustering sfcAdaptiveClustering = new SFCAdaptiveClustering()
            {
                Name = "SFC Clustering",
                ScanningOrder = hilbertScanOrder,
                ErrorFilter = new VectorErrorFilter(ErrorMatrix.Samples.nextTwoPixels)
            };
            config.saveModule(sfcAdaptiveClustering, false);

            SpotFunction euclidDotSpotFunction = new SpotFunction(
                        SpotFunction.Samples.euclidDot, Math.PI * 0.25, 8);

            thresholdDitherAlgorithm.ThresholdFilter =
                new SpotFunctionThresholdFilter(euclidDotSpotFunction);
            thresholdDitherAlgorithm.ScanningOrder = scanlineScanOrder;
            thresholdDitherAlgorithm.ErrorFilter = null;
            thresholdDitherAlgorithm.Name = "Halftoning with Euclid dot, direct";
            thresholdDitherAlgorithm.Description = "";
            config.saveModule(thresholdDitherAlgorithm, false);

            euclidDotSpotFunction.Distance = 32;
            thresholdDitherAlgorithm.ThresholdFilter =
                new ImageThresholdFilter()
                {
                    ImageGenerator = new ImageThresholdFilter.Generator()
                    {
                        SpotFunction = euclidDotSpotFunction,
                        Effects = ImageThresholdFilter.Generator.pixelizeEffect
                    }
                };
            thresholdDitherAlgorithm.Name = "Halftoning with Euclid dot, using Image";
            thresholdDitherAlgorithm.Description = "";
            config.saveModule(thresholdDitherAlgorithm, false);

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
