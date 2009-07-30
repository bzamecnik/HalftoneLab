using System;
using HalftoneLab;

namespace HalftoneLab.SampleConfig
{
    class SampleConfig
    {
        // TODO: preprare sample configurations

        public static void makeSampleConfig(ConfigManager config) {
            if (config == null) {
                return;
            }
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

            SFCScanningOrder hilbertScanOrder = new HilbertScanningOrder()
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
            dynamicMatrixErrorFilter.MatrixTable.addDefinitionRecord(new DynamicMatrixErrorFilter.ErrorRecord(0, ErrorMatrix.Samples.floydSteinberg));
            dynamicMatrixErrorFilter.MatrixTable.addDefinitionRecord(new DynamicMatrixErrorFilter.ErrorRecord(64, ErrorMatrix.Samples.jarvisJudiceNinke));
            dynamicMatrixErrorFilter.MatrixTable.addDefinitionRecord(new DynamicMatrixErrorFilter.ErrorRecord(128, ErrorMatrix.Samples.stucki));
            dynamicMatrixErrorFilter.MatrixTable.addDefinitionRecord(new DynamicMatrixErrorFilter.ErrorRecord(192, ErrorMatrix.Samples.nextPixel));
            //config.saveModule(dynamicMatrixErrorFilter, false);

            MatrixThresholdFilter matrixThresholdFilter = new MatrixThresholdFilter(bayerThresholdMatrix)
            {
                Name = "Bayer filter"
            };
            //config.saveModule(matrixThresholdFilter, false);

            DynamicMatrixThresholdFilter dynamicThresholdFilter = new DynamicMatrixThresholdFilter()
            {
                NoiseEnabled = true,
                Name = "Dynamic threshold filter"
            };
            dynamicThresholdFilter.MatrixTable.addDefinitionRecord(new DynamicMatrixThresholdFilter.ThresholdRecord(0, ThresholdMatrix.Generator.sampleMatrix, 0.0));
            dynamicThresholdFilter.MatrixTable.addDefinitionRecord(new DynamicMatrixThresholdFilter.ThresholdRecord(32, ThresholdMatrix.Generator.sampleMatrix, 0.125));
            dynamicThresholdFilter.MatrixTable.addDefinitionRecord(new DynamicMatrixThresholdFilter.ThresholdRecord(64, ThresholdMatrix.Generator.sampleMatrix, 0.25));
            dynamicThresholdFilter.MatrixTable.addDefinitionRecord(new DynamicMatrixThresholdFilter.ThresholdRecord(96, ThresholdMatrix.Generator.sampleMatrix, 0.375));
            dynamicThresholdFilter.MatrixTable.addDefinitionRecord(new DynamicMatrixThresholdFilter.ThresholdRecord(128, ThresholdMatrix.Generator.sampleMatrix, 0.5));
            dynamicThresholdFilter.MatrixTable.addDefinitionRecord(new DynamicMatrixThresholdFilter.ThresholdRecord(160, ThresholdMatrix.Generator.sampleMatrix, 0.625));
            dynamicThresholdFilter.MatrixTable.addDefinitionRecord(new DynamicMatrixThresholdFilter.ThresholdRecord(192, ThresholdMatrix.Generator.sampleMatrix, 0.75));
            dynamicThresholdFilter.MatrixTable.addDefinitionRecord(new DynamicMatrixThresholdFilter.ThresholdRecord(224, ThresholdMatrix.Generator.sampleMatrix, 0.875));

            dynamicThresholdFilter.MatrixTable.addDefinitionRecord(new DynamicMatrixThresholdFilter.ThresholdRecord(100, ThresholdMatrix.Generator.simpleThreshold));
            dynamicThresholdFilter.MatrixTable.addDefinitionRecord(new DynamicMatrixThresholdFilter.ThresholdRecord(150, ThresholdMatrix.Generator.createBayerDispersedDotMatrix(3)));

            //config.saveModule(dynamicThresholdFilter, false);

            ThresholdHalftoneMethod thresholdHalftoneMethod =
                new ThresholdHalftoneMethod()
            {
                Name = "Threshold halftone algorithm"
            };
            HalftoneAlgorithm halftoneAlgorithm =
                new HalftoneAlgorithm()
            {
                Method = thresholdHalftoneMethod
            };
            config.saveModule(thresholdHalftoneMethod, false);

            thresholdHalftoneMethod.ErrorFilter = matrixErrorFilter;
            thresholdHalftoneMethod.ScanningOrder = scanlineScanOrder;
            halftoneAlgorithm.Name = "Floyd-Steinberg, scanline";
            halftoneAlgorithm.Description = "Floyd-Steinberg error, Scanline order";
            config.saveModule(halftoneAlgorithm, false);

            thresholdHalftoneMethod.ScanningOrder = serpentineScanOrder;
            halftoneAlgorithm.Name = "Floyd-Steinberg, serpentine";
            halftoneAlgorithm.Description = "Floyd-Steinberg error, Serpentine order";
            config.saveModule(halftoneAlgorithm, false);

            thresholdHalftoneMethod.ScanningOrder = scanlineScanOrder;
            thresholdHalftoneMethod.ErrorFilter = dynamicMatrixErrorFilter;
            halftoneAlgorithm.Name = "Dynamic error filter";
            halftoneAlgorithm.Description = "floydSteinberg, jarvisJudiceNinke, stucki, nextPixel, Scanline order";
            config.saveModule(halftoneAlgorithm, false);

            thresholdHalftoneMethod.ScanningOrder = scanlineScanOrder;
            thresholdHalftoneMethod.ErrorFilter = null;
            thresholdHalftoneMethod.ThresholdFilter = matrixThresholdFilter;
            halftoneAlgorithm.Name = "Bayer dispersed halftone";
            config.saveModule(halftoneAlgorithm, false);

            thresholdHalftoneMethod.ThresholdFilter = new MatrixThresholdFilter(ThresholdMatrix.Generator.sampleMatrix);
            halftoneAlgorithm.Name = "Clustered dot halftone";
            config.saveModule(halftoneAlgorithm, false);

            thresholdHalftoneMethod.ThresholdFilter = dynamicThresholdFilter;
            halftoneAlgorithm.Name = "Dynamic threshold halftone";
            config.saveModule(halftoneAlgorithm, false);

            dynamicThresholdFilter.MatrixTable.clearDefinitionRecords();
            dynamicThresholdFilter.MatrixTable.addDefinitionRecord(new DynamicMatrixThresholdFilter.ThresholdRecord(0, ThresholdMatrix.Generator.simpleThreshold, 1.0));
            thresholdHalftoneMethod.ThresholdFilter = dynamicThresholdFilter;
            halftoneAlgorithm.Name = "White noise";
            config.saveModule(halftoneAlgorithm, false);

            thresholdHalftoneMethod.ErrorFilter = matrixErrorFilter;
            halftoneAlgorithm.Name = "Blue-noise Floyd-Steinberg, scanline";
            config.saveModule(halftoneAlgorithm, false);

            thresholdHalftoneMethod.ScanningOrder = serpentineScanOrder;
            halftoneAlgorithm.Name = "Blue-noise, Floyd-Steinberg, serpentine";
            config.saveModule(halftoneAlgorithm, false);

            thresholdHalftoneMethod.ScanningOrder = scanlineScanOrder;
            thresholdHalftoneMethod.ThresholdFilter = new MatrixThresholdFilter();

            //thresholdHalftoneMethod.ErrorFilter = dynamicMatrixErrorFilter;
            //thresholdHalftoneMethod.Name = "Dynamic error filter";
            //config.saveModule(thresholdHalftoneMethod, false);

            //thresholdHalftoneMethod.ErrorFilter = perturbedErrorFilter;
            //thresholdHalftoneMethod.Name = "Perturbed error filter";
            //config.saveModule(thresholdHalftoneMethod, false);

            thresholdHalftoneMethod.ErrorFilter = randomizedMatrixErrorFilter;
            halftoneAlgorithm.Name = "Randomized error filter";
            config.saveModule(halftoneAlgorithm, false);

            thresholdHalftoneMethod.ScanningOrder = hilbertScanOrder;
            thresholdHalftoneMethod.ErrorFilter = vectorErrorFilter;
            halftoneAlgorithm.Name = "SFC Threshold halftone algorithm";
            halftoneAlgorithm.Description = "Hilbert SFC order, next pixel vector error filter";
            config.saveModule(halftoneAlgorithm, false);

            SFCClusteringMethod sfcAdaptiveClustering = new SFCClusteringMethod()
            {
                ScanningOrder = hilbertScanOrder,
                ErrorFilter = new VectorErrorFilter(ErrorMatrix.Samples.nextTwoPixels)
            };
            halftoneAlgorithm.Method = sfcAdaptiveClustering;
            halftoneAlgorithm.Name = "SFC Clustering";
            config.saveModule(halftoneAlgorithm, false);

            SpotFunction euclidDotSpotFunction = new SpotFunction();

            halftoneAlgorithm.Method = thresholdHalftoneMethod;
            thresholdHalftoneMethod.ThresholdFilter =
                new SpotFunctionThresholdFilter(euclidDotSpotFunction);
            thresholdHalftoneMethod.ScanningOrder = scanlineScanOrder;
            thresholdHalftoneMethod.ErrorFilter = null;
            halftoneAlgorithm.Name = "Halftoning with Euclid dot, direct";
            halftoneAlgorithm.Description = "";
            config.saveModule(halftoneAlgorithm, false);

            euclidDotSpotFunction.Distance = 32;
            euclidDotSpotFunction.Angle = Math.PI * 0.25;
            thresholdHalftoneMethod.ThresholdFilter =
                new ImageThresholdFilter()
                {
                    ImageGenerator = new ImageGenerator()
                    {
                        SpotFunction = euclidDotSpotFunction,
                        Effects = {
                            ImageGenerator.Samples.pixelizeEffect,
                            ImageGenerator.Samples.rippleEffect
                            //ImageThresholdFilter.ImageGenerator.canvasEffect
                        }
                    }
                };
            halftoneAlgorithm.Name = "Halftoning with Euclid dot, using Image, with effects";
            halftoneAlgorithm.Description = "";
            config.saveModule(halftoneAlgorithm, false);

            config.save();

            // DEBUG:
            //foreach (Module module in config.listAllModules()) {
            //    Console.Out.WriteLine("type: {0}, name: {1}, desc: {2}",
            //        module.GetType(), module.Name, module.Description);
            //}
        }
    }
}