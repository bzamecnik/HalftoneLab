// Copyright (c) 2009 Bohumir Zamecnik <bohumir@zamecnik.org>
// License: The MIT License, see the LICENSE file

using System;
using HalftoneLab;

namespace HalftoneLab.SampleConfig
{
    /// <summary>
    /// A collection of sample module configurations.
    /// </summary>
    /// <remarks>
    /// It is intended to fill the ConfigManager with a default configuration
    /// if it is empty.
    /// </remarks>
    public class SampleConfig
    {
        /// <summary>
        /// Fill the config manager with a default configuration.
        /// </summary>
        /// <param name="config"></param>
        public static void makeSampleConfig(ConfigManager config) {
            if (config == null) {
                return;
            }

            int counter = 1; // number of the preset to preserve the order

            // ----------------------------------------------

            // Thresholding

            HalftoneAlgorithm halftoneAlgorithm = new HalftoneAlgorithm();
            ThresholdHalftoneMethod thresholdHalftoneMethod =
                new ThresholdHalftoneMethod();
            halftoneAlgorithm.Method = thresholdHalftoneMethod;
            halftoneAlgorithm.Name = String.Format("[{0:d2}] Thresholding", counter);
            halftoneAlgorithm.Description = "Simple threshold at 50% grey";
            config.saveModule(halftoneAlgorithm, false);
            counter++;

            // ----------------------------------------------

            // Random dither with white noise

            DynamicMatrixThresholdFilter dynamicThresholdFilter =
                new DynamicMatrixThresholdFilter();
            dynamicThresholdFilter.MatrixTable.addDefinitionRecord(
                new DynamicMatrixThresholdFilter.ThresholdRecord(
                    0, ThresholdMatrix.Samples.simpleThreshold, 1.0));
            thresholdHalftoneMethod.ThresholdFilter = dynamicThresholdFilter;
            halftoneAlgorithm.Name =
                String.Format("[{0:d2}] Random dither", counter);
            halftoneAlgorithm.Description = "Threshold perturbed with white noise";
            config.saveModule(halftoneAlgorithm, false);
            counter++;

            // ----------------------------------------------

            // Bayer matrix

            thresholdHalftoneMethod.ThresholdFilter = new MatrixThresholdFilter(
                ThresholdMatrix.Samples.createBayerDispersedDotMatrix(3));
            halftoneAlgorithm.Name =
                String.Format("[{0:d2}] Ordered dither with Bayer matrix", counter);
            halftoneAlgorithm.Description = "Recursive tesselation matrix 8x8";
            config.saveModule(halftoneAlgorithm, false);
            counter++;

            // -------------------- SCREENING METHODS --------------------

            // Clustered-dot hand coded threshold matrix

            thresholdHalftoneMethod.ThresholdFilter = new MatrixThresholdFilter(
                ThresholdMatrix.Samples.sampleScreenMatrix);
            halftoneAlgorithm.Name =
                String.Format("[{0:d2}] Screen - Clustered-dot threshold matrix", counter);
            halftoneAlgorithm.Description = "Hand-coded threshold matrix 8x8 with 45 angle";
            config.saveModule(halftoneAlgorithm, false);
            counter++;

            // ----------------------------------------------

            // Spot function - Eulid dot

            thresholdHalftoneMethod.ThresholdFilter = new SpotFunctionThresholdFilter(
                SpotFunction.Samples.euclidDot);
            halftoneAlgorithm.Name =
                String.Format("[{0:d2}] Screen - Spot function", counter);
            halftoneAlgorithm.Description = "Euclid-dot spot function - computed on-the-fly\nEuclid dot: angle = 45 deg, distance: 10 px";
            config.saveModule(halftoneAlgorithm, false);
            counter++;

            // ----------------------------------------------

            // Patterning

            thresholdHalftoneMethod.ThresholdFilter = new MatrixThresholdFilter(
                new ThresholdMatrix(new int[,] { { 1, 2 }, { 3, 4 } }));
            halftoneAlgorithm.PreResize = new HalftoneAlgorithm.Resize()
            {
                Factor = 2.0,
                Interpolation = HalftoneAlgorithm.Resize.InterpolationType.NearestNeighbour
            };
            halftoneAlgorithm.Name =
                String.Format("[{0:d2}] Screen - Patterning", counter);
            halftoneAlgorithm.Description = "Resize 2x via Nearest-neighbour interpolation\n2x2 matrix threshold";
            config.saveModule(halftoneAlgorithm, false);
            counter++;
            halftoneAlgorithm.PreResize = null;

            // -------------------- ERROR DIFFUSION METHODS --------------------

            // Floyd-Steinberg

            thresholdHalftoneMethod.ThresholdFilter = new MatrixThresholdFilter();

            thresholdHalftoneMethod.ErrorFilter = new MatrixErrorFilter(
                ErrorMatrix.Samples.floydSteinberg);
            halftoneAlgorithm.Name =
                String.Format("[{0:d2}] Error diffusion - Floyd-Steinberg", counter);
            halftoneAlgorithm.Description = "Floyd-Steinberg matrix, scanline order";
            config.saveModule(halftoneAlgorithm, false);
            counter++;

            // ----------------------------------------------

            // Jarvis-Judice-Ninke

            thresholdHalftoneMethod.ErrorFilter = new MatrixErrorFilter(
                ErrorMatrix.Samples.jarvisJudiceNinke);
            halftoneAlgorithm.Name =
                String.Format("[{0:d2}] Error diffusion - Jarvis-Judice-Ninke", counter);
            halftoneAlgorithm.Description = "Jarvis-Judice-Ninke matrix, scanline order";
            config.saveModule(halftoneAlgorithm, false);
            counter++;

            // ----------------------------------------------

            // Stucki

            thresholdHalftoneMethod.ErrorFilter = new MatrixErrorFilter(
                ErrorMatrix.Samples.stucki);
            halftoneAlgorithm.Name =
                String.Format("[{0:d2}] Error diffusion - Stucki", counter);
            halftoneAlgorithm.Description = "Stucki matrix, scanline order";
            config.saveModule(halftoneAlgorithm, false);
            counter++;

            // ----------------------------------------------

            // Floyd-Steinberg serpentine

            thresholdHalftoneMethod.ErrorFilter = new MatrixErrorFilter(
                ErrorMatrix.Samples.floydSteinberg);
            thresholdHalftoneMethod.ScanningOrder = new SerpentineScanningOrder();
            halftoneAlgorithm.Name =
                String.Format("[{0:d2}] Error diffusion - Floyd-Steinberg serpentine", counter);
            halftoneAlgorithm.Description = "Floyd-Steinberg matrix, serpentine order";
            config.saveModule(halftoneAlgorithm, false);
            counter++;
            thresholdHalftoneMethod.ScanningOrder = new ScanlineScanningOrder();

            // Error diffusion + sharpening

            thresholdHalftoneMethod.ErrorFilter = new MatrixErrorFilter(
                ErrorMatrix.Samples.floydSteinberg);
            halftoneAlgorithm.PreSharpen = new HalftoneAlgorithm.Sharpen() { Amount = 0.4 };
            halftoneAlgorithm.Name =
                String.Format("[{0:d2}] Error diffusion + sharpening", counter);
            halftoneAlgorithm.Description = "Pre-sharpened by 40%\nFloyd-Steinberg matrix, scanline order";
            config.saveModule(halftoneAlgorithm, false);
            counter++;
            halftoneAlgorithm.PreSharpen = null;

            // -------------------- SFC METHODS --------------------

            // SFC clustering - positioning & adaptive

            SFCClusteringMethod sfcClusteringMethod = new SFCClusteringMethod();
            halftoneAlgorithm.Method = sfcClusteringMethod;

            sfcClusteringMethod.UseClusterPositioning = true;
            sfcClusteringMethod.UseAdaptiveClustering = true;
            halftoneAlgorithm.Name =
                String.Format("[{0:d2}] SFC clustering - positioned & adaptive", counter);
            halftoneAlgorithm.Description = "SFC clustering, cell size: " +
                sfcClusteringMethod.MaxCellSize + ", positioning enabled, adaptive";
            config.saveModule(halftoneAlgorithm, false);
            counter++;

            // ----------------------------------------------

            // SFC clustering - bare

            sfcClusteringMethod.UseClusterPositioning = false;
            sfcClusteringMethod.UseAdaptiveClustering = false;
            halftoneAlgorithm.Name =
                String.Format("[{0:d2}] SFC clustering - bare", counter);
            halftoneAlgorithm.Description = "SFC clustering, cell size: " +
                sfcClusteringMethod.MaxCellSize + ", no positioning, non-adaptive";
            config.saveModule(halftoneAlgorithm, false);
            counter++;

            // ----------------------------------------------

            // SFC clustering - positioning

            sfcClusteringMethod.UseClusterPositioning = true;
            halftoneAlgorithm.Name =
                String.Format("[{0:d2}] SFC clustering - positioned", counter);
            halftoneAlgorithm.Description = "SFC clustering, cell size: " +
                sfcClusteringMethod.MaxCellSize + ", positioning enabled, non-adaptive";
            config.saveModule(halftoneAlgorithm, false);
            counter++;

            // ----------------------------------------------

            // SFC clustering - adaptive

            sfcClusteringMethod.UseClusterPositioning = false;
            sfcClusteringMethod.UseAdaptiveClustering = true;
            halftoneAlgorithm.Name =
                String.Format("[{0:d2}] SFC clustering - adaptive", counter);
            halftoneAlgorithm.Description = "SFC clustering, cell size: " +
                sfcClusteringMethod.MaxCellSize + ", no positioning, adaptive";
            config.saveModule(halftoneAlgorithm, false);
            counter++;

            // ----------------------------------------------

            // SFC clustering - positioning & adaptive - larger cell

            sfcClusteringMethod.UseClusterPositioning = true;
            sfcClusteringMethod.UseAdaptiveClustering = true;
            sfcClusteringMethod.MaxCellSize = 30;
            halftoneAlgorithm.Name =
                String.Format("[{0:d2}] SFC clustering - larger cell", counter);
            halftoneAlgorithm.Description = "SFC clustering, cell size: " +
                sfcClusteringMethod.MaxCellSize + ", positioning enabled, adaptive";
            config.saveModule(halftoneAlgorithm, false);
            counter++;

            // ----------------------------------------------

            halftoneAlgorithm.Method = thresholdHalftoneMethod;
            thresholdHalftoneMethod.ThresholdFilter = new MatrixThresholdFilter();
            thresholdHalftoneMethod.ScanningOrder = new HilbertScanningOrder();

            // Riermersma

            thresholdHalftoneMethod.ErrorFilter = new VectorErrorFilter(
                ErrorMatrix.Samples.riemersma16);
            halftoneAlgorithm.Name =
                String.Format("[{0:d2}] SFC - Riemersma", counter);
            halftoneAlgorithm.Description = "Error diffusion along Hilbert SFC, error coefficients decrease exponencially\nNumber of coefficients: 16, highest-to-lowest ratio: 16:1";
            config.saveModule(halftoneAlgorithm, false);
            counter++;

            // ----------------------------------------------

            // SFC + simple error diffusion

            thresholdHalftoneMethod.ErrorFilter = new VectorErrorFilter(
                ErrorMatrix.Samples.nextPixel);
            halftoneAlgorithm.Name =
                String.Format("[{0:d2}] SFC - Simple error diffusion along Hilbert curve", counter);
            halftoneAlgorithm.Description = "The whole error goes to the next pixel";
            config.saveModule(halftoneAlgorithm, false);
            counter++;

            // ----------------------------------------------

            // SFC + simple error diffusion + pertubed threshold

            dynamicThresholdFilter.MatrixTable.clearDefinitionRecords();
            dynamicThresholdFilter.MatrixTable.addDefinitionRecord(
                new DynamicMatrixThresholdFilter.ThresholdRecord(0,
                    ThresholdMatrix.Samples.simpleThreshold, 1.0));
            
            thresholdHalftoneMethod.ThresholdFilter = dynamicThresholdFilter;
            thresholdHalftoneMethod.ErrorFilter = new VectorErrorFilter(
                ErrorMatrix.Samples.nextPixel);
            halftoneAlgorithm.Name =
                String.Format("[{0:d2}] SFC - error diffusion, pertubed threshold", counter);
            halftoneAlgorithm.Description = "Hilbert SFC, error to the next pixel, threshold perturbation: 1.0\nQuite good blue-noise";
            config.saveModule(halftoneAlgorithm, false);
            counter++;

            // ----------------------------------------------

            // Riermersma + perturbed threshold

            dynamicThresholdFilter.MatrixTable.clearDefinitionRecords();
            dynamicThresholdFilter.MatrixTable.addDefinitionRecord(
                new DynamicMatrixThresholdFilter.ThresholdRecord(0,
                    ThresholdMatrix.Samples.simpleThreshold, 0.25));

            thresholdHalftoneMethod.ErrorFilter = new VectorErrorFilter(
                ErrorMatrix.Samples.riemersma16);
            halftoneAlgorithm.Name =
                String.Format("[{0:d2}] SFC - Riemersma, perturbed threshold", counter);
            halftoneAlgorithm.Description = "Error diffusion along Hilbert SFC, error coefficients decrease exponencially\nNumber of coefficients: 16, highest-to-lowest ratio: 16:1\nThreshold perturbation: 0.25";
            config.saveModule(halftoneAlgorithm, false);
            counter++;

            // -------------------- BLUE-NOISE METHODS --------------------

            // Randomized error matrix - preserved coefficient count

            thresholdHalftoneMethod.ThresholdFilter = new MatrixThresholdFilter();
            thresholdHalftoneMethod.ErrorFilter = new RandomizedMatrixErrorFilter(
                ErrorMatrix.Samples.floydSteinberg);
            thresholdHalftoneMethod.ScanningOrder = new SerpentineScanningOrder();
            halftoneAlgorithm.Name =
                String.Format("[{0:d2}] Randomized error matrix - preserved coefficient count", counter);
            halftoneAlgorithm.Description = "A template for coefficient positions as proposed by Ulichney";
            config.saveModule(halftoneAlgorithm, false);
            counter++;

            // ----------------------------------------------

            // Randomized error matrix - variable coefficient count

            thresholdHalftoneMethod.ErrorFilter = new RandomizedMatrixErrorFilter(
                ErrorMatrix.Samples.floydSteinberg) { RandomizeCoeffCount = true };
            halftoneAlgorithm.Name =
                String.Format("[{0:d2}] Randomized error matrix - variable coefficient count", counter);
            halftoneAlgorithm.Description = "Floyd-Steinberg matrix as a template for matrix size only\nThe number of coefficient is randomized up to the capacity of the matrix";
            config.saveModule(halftoneAlgorithm, false);
            counter++;

            // ----------------------------------------------

            // Perturbed error matrix

            thresholdHalftoneMethod.ErrorFilter = new PerturbedErrorFilter(
                new MatrixErrorFilter(ErrorMatrix.Samples.floydSteinberg))
                { PerturbationAmplitude = 0.3 };
            halftoneAlgorithm.Name =
                String.Format("[{0:d2}] Perturbed error matrix", counter);
            halftoneAlgorithm.Description = "Floyd-Steinberg matrix, perturbation amplitude: 0.3";
            config.saveModule(halftoneAlgorithm, false);
            counter++;

            // ----------------------------------------------

            // Perturbed error matrix + perturbed threshold

            dynamicThresholdFilter.MatrixTable.clearDefinitionRecords();
            dynamicThresholdFilter.MatrixTable.addDefinitionRecord(
                new DynamicMatrixThresholdFilter.ThresholdRecord(0,
                    ThresholdMatrix.Samples.simpleThreshold, 0.15));

            thresholdHalftoneMethod.ThresholdFilter = dynamicThresholdFilter;
            thresholdHalftoneMethod.ErrorFilter = new PerturbedErrorFilter(
                new MatrixErrorFilter(ErrorMatrix.Samples.floydSteinberg)) { PerturbationAmplitude = 0.3 };
            thresholdHalftoneMethod.ScanningOrder = new SerpentineScanningOrder();
            halftoneAlgorithm.Name =
                String.Format("[{0:d2}] Perturbed error matrix and threshold", counter);
            halftoneAlgorithm.Description = "A kind of blue-noise generator proposed by Ulichney\nFloyd-Steinberg matrix, perturbation amplitude: 0.3\nPerturbed threshold: 0.15\nSerpentine scanning";
            config.saveModule(halftoneAlgorithm, false);
            counter++;

            // ----------------------------------------------

            // Optimal weights - dynamic error matrix

            int[,] ostromoukhovOptimalCoeffs = new int[,] {
                {13, 0, 5}, {13, 0, 5}, {21, 0, 10}, {7, 0, 4},
                {8, 0, 5}, {47, 3, 28}, {23, 3, 13}, {15, 3, 8},
                {22, 6, 11}, {43, 15, 20}, {7, 3, 3}, {501, 224, 211}, 
                {249, 116, 103}, {165, 80, 67}, {123, 62, 49}, {489, 256, 191},
                {81, 44, 31}, {483, 272, 181}, {60, 35, 22}, {53, 32, 19},
                {237, 148, 83}, {471, 304, 161}, {3, 2, 1}, {481, 314, 185},
                {354, 226, 155}, {1389, 866, 685}, {227, 138, 125}, {267, 158, 163},
                {327, 188, 220}, {61, 34, 45}, {627, 338, 505}, {1227, 638, 1075},
                {20, 10, 19}, {1937, 1000, 1767}, {977, 520, 855}, {657, 360, 551},
                {71, 40, 57}, {2005, 1160, 1539}, {337, 200, 247}, {2039, 1240, 1425},
                {257, 160, 171}, {691, 440, 437}, {1045, 680, 627}, {301, 200, 171},
                {177, 120, 95}, {2141, 1480, 1083}, {1079, 760, 513}, {725, 520, 323},
                {137, 100, 57}, {2209, 1640, 855}, {53, 40, 19}, {2243, 1720, 741},
                {565, 440, 171}, {759, 600, 209}, {1147, 920, 285}, {2311, 1880, 513},
                {97, 80, 19}, {335, 280, 57}, {1181, 1000, 171}, {793, 680, 95},
                {599, 520, 57}, {2413, 2120, 171}, {405, 360, 19}, {2447, 2200, 57},
                {11, 10, 0}, {158, 151, 3}, {178, 179, 7}, {1030, 1091, 63},
                {248, 277, 21}, {318, 375, 35}, {458, 571, 63}, {878, 1159, 147},
                {5, 7, 1}, {172, 181, 37}, {97, 76, 22}, {72, 41, 17},
                {119, 47, 29}, {4, 1, 1}, {4, 1, 1}, {4, 1, 1},
                {4, 1, 1}, {4, 1, 1}, {4, 1, 1}, {4, 1, 1},
                {4, 1, 1}, {4, 1, 1}, {65, 18, 17}, {95, 29, 26},
                {185, 62, 53}, {30, 11, 9}, {35, 14, 11}, {85, 37, 28},
                {55, 26, 19}, {80, 41, 29}, {155, 86, 59}, {5, 3, 2},
                {5, 3, 2}, {5, 3, 2}, {5, 3, 2}, {5, 3, 2},
                {5, 3, 2}, {5, 3, 2}, {5, 3, 2}, {5, 3, 2},
                {5, 3, 2}, {5, 3, 2}, {5, 3, 2}, {5, 3, 2},
                {305, 176, 119}, {155, 86, 59}, {105, 56, 39}, {80, 41, 29},
                {65, 32, 23}, {55, 26, 19}, {335, 152, 113}, {85, 37, 28},
                {115, 48, 37}, {35, 14, 11}, {355, 136, 109}, {30, 11, 9},
                {365, 128, 107}, {185, 62, 53}, {25, 8, 7}, {95, 29, 26},
                {385, 112, 103}, {65, 18, 17}, {395, 104, 101}, {4, 1, 1}
            };

            thresholdHalftoneMethod.ThresholdFilter = new MatrixThresholdFilter();
            DynamicMatrixErrorFilter dynamicErrorFilter = new DynamicMatrixErrorFilter();
            int[,] dynamicErrorCoeffs = new int[2,3];
            ErrorMatrix dynamicErrorMatrix = new ErrorMatrix(dynamicErrorCoeffs, 1);
            for (int i = 0; i < ostromoukhovOptimalCoeffs.GetLength(0); i++) {
                dynamicErrorCoeffs[0, 2] = ostromoukhovOptimalCoeffs[i, 0];
                dynamicErrorCoeffs[1, 0] = ostromoukhovOptimalCoeffs[i, 1];
                dynamicErrorCoeffs[1, 1] = ostromoukhovOptimalCoeffs[i, 2];
                dynamicErrorMatrix.setDefinitionMatrix(dynamicErrorCoeffs);
                dynamicErrorFilter.MatrixTable.addDefinitionRecord(
                    new DynamicMatrixErrorFilter.ErrorRecord(i, dynamicErrorMatrix));
            }
            for (int i = ostromoukhovOptimalCoeffs.GetLength(0) - 1; i >= 0; i--) {
                dynamicErrorCoeffs[0, 2] = ostromoukhovOptimalCoeffs[i, 0];
                dynamicErrorCoeffs[1, 0] = ostromoukhovOptimalCoeffs[i, 1];
                dynamicErrorCoeffs[1, 1] = ostromoukhovOptimalCoeffs[i, 2];
                dynamicErrorMatrix.setDefinitionMatrix(dynamicErrorCoeffs);
                dynamicErrorFilter.MatrixTable.addDefinitionRecord(
                    new DynamicMatrixErrorFilter.ErrorRecord(255 - i, dynamicErrorMatrix));
            }
            
            thresholdHalftoneMethod.ErrorFilter = dynamicErrorFilter;
            thresholdHalftoneMethod.ScanningOrder = new SerpentineScanningOrder();
            halftoneAlgorithm.Name =
                String.Format("[{0:d2}] Blue noise - error diffusion with optimal coefficients", counter);
            halftoneAlgorithm.Description = "Ostromoukhov";
            config.saveModule(halftoneAlgorithm, false);
            counter++;

            // ----------------------------------------------

            // Optimal weights + perturbed threshold

            dynamicThresholdFilter.MatrixTable.clearDefinitionRecords();
            dynamicThresholdFilter.MatrixTable.addDefinitionRecord(
                new DynamicMatrixThresholdFilter.ThresholdRecord(0,
                    ThresholdMatrix.Samples.simpleThreshold, 0.35));
            thresholdHalftoneMethod.ThresholdFilter = dynamicThresholdFilter;
            halftoneAlgorithm.Name =
                String.Format("[{0:d2}] Blue noise - optimal coefficients, perturbed threshold", counter);
            halftoneAlgorithm.Description = "Ostromoukhov coefficients\n threshold perturbation: 0.35";
            config.saveModule(halftoneAlgorithm, false);
            counter++;

            // -------------------- ARTISTIC METHODS --------------------

            // Veryovka-Buchanan - no error filter

            thresholdHalftoneMethod.ThresholdFilter = new ImageThresholdFilter()
            {
                ImageGenerator = new ImageGenerator()
                {
                    SpotFunction = SpotFunction.Samples.nullSpot,
                    Effects = { ImageGenerator.Samples.patternEffect }
                }
            };
            thresholdHalftoneMethod.ErrorFilter = null;
            halftoneAlgorithm.Name =
                String.Format("[{0:d2}] Veryovka-Buchanan without error filter", counter);
            halftoneAlgorithm.Description =
                "Image filled with a GIMP pattern + histogram equalization";
            config.saveModule(halftoneAlgorithm, false);
            counter++;

            // ----------------------------------------------

            // Veryovka-Buchanan - Ja-Ju-Ni error filter

            thresholdHalftoneMethod.ErrorFilter = new MatrixErrorFilter(
                ErrorMatrix.Samples.jarvisJudiceNinke);
            halftoneAlgorithm.Name =
                String.Format("[{0:d2}] Veryovka-Buchanan with error filter", counter);
            halftoneAlgorithm.Description =
                "Image filled with a GIMP pattern + histogram equalization\nJarvis-Judice-Ninke error matrix";
            config.saveModule(halftoneAlgorithm, false);
            counter++;

            // ----------------------------------------------

            //  Supersampling - euclid dot spot function

            SpotFunction bigDot = SpotFunction.Samples.euclidDot;
            bigDot.Distance = 30;

            thresholdHalftoneMethod.ErrorFilter = null;
            thresholdHalftoneMethod.ThresholdFilter =
                new SpotFunctionThresholdFilter(bigDot);
            halftoneAlgorithm.PreResize = new HalftoneAlgorithm.Resize()
            {
                Factor = 2.0,
                Interpolation = HalftoneAlgorithm.Resize.InterpolationType.Bicubic
            };
            halftoneAlgorithm.PostResize = new HalftoneAlgorithm.Resize()
            {
                Factor = 2.0,
                Interpolation = HalftoneAlgorithm.Resize.InterpolationType.Bicubic,
                Forward = false
            };
            halftoneAlgorithm.Name =
                String.Format("[{0:d2}] Spot function with supersampling", counter);
            halftoneAlgorithm.Description = "Euclid-dot spot function\nSmoothened via supersampling";
            config.saveModule(halftoneAlgorithm, false);
            counter++;
            halftoneAlgorithm.PreResize = null;
            halftoneAlgorithm.PostResize = null;

            // ----------------------------------------------

            //  Smoothening - euclid dot spot function
            
            halftoneAlgorithm.PostSmoothen = new HalftoneAlgorithm.Smoothen()
            {
                Radius = 5
            };
            halftoneAlgorithm.Name =
                String.Format("[{0:d2}] Spot function with smoothening", counter);
            halftoneAlgorithm.Description = "Euclid-dot spot function\nSmoothened via Gaussian blur & Levels";
            config.saveModule(halftoneAlgorithm, false);
            counter++;
            halftoneAlgorithm.PostSmoothen = null;

            // ----------------------------------------------

            // Spot function + effects

            thresholdHalftoneMethod.ThresholdFilter = new ImageThresholdFilter()
            {
                ImageGenerator = new ImageGenerator()
                {
                    SpotFunction = SpotFunction.Samples.euclidDot,
                    Effects = {
                            ImageGenerator.Samples.pixelizeEffect,
                            ImageGenerator.Samples.rippleEffect
                        }
                }
            };
            halftoneAlgorithm.Name =
                String.Format("[{0:d2}] Spot function with effects", counter);
            halftoneAlgorithm.Description =
                "Euclid dot: angle = 45 deg, distance: 10 px\nEffects: Pixelize, Ripple";
            config.saveModule(halftoneAlgorithm, false);
            counter++;

            // ----------------------------------------------

            // Spot function + effects

            thresholdHalftoneMethod.ThresholdFilter = new ImageThresholdFilter()
            {
                ImageGenerator = new ImageGenerator()
                {
                    SpotFunction = bigDot,
                    Effects = {
                            ImageGenerator.Samples.canvasEffect
                        }
                }
            };
            halftoneAlgorithm.Name =
                String.Format("[{0:d2}] Spot function with effects", counter);
            halftoneAlgorithm.Description =
                "Euclid dot: angle = 45 deg, distance: 10 px\nEffects: Canvas";
            config.saveModule(halftoneAlgorithm, false);
            counter++;

            // -------------------- MISCELANEOUS METHODS --------------------

            // Floyd-Steinberg with dot gain compensation

            thresholdHalftoneMethod.ThresholdFilter = new MatrixThresholdFilter();
            thresholdHalftoneMethod.ErrorFilter = new MatrixErrorFilter(
                ErrorMatrix.Samples.floydSteinberg);
            halftoneAlgorithm.PreDotGain = new HalftoneAlgorithm.GammaCorrection()
                { Gamma = 2.2 };
            halftoneAlgorithm.Name =
                String.Format("[{0:d2}] Dot gain compensation", counter);
            halftoneAlgorithm.Description = "Floyd-Steinberg, dot gain compensation via gamma curve";
            config.saveModule(halftoneAlgorithm, false);
            counter++;
            halftoneAlgorithm.PreDotGain = null;

            // Dynamic threshold filter

            dynamicThresholdFilter.MatrixTable.clearDefinitionRecords();
            dynamicThresholdFilter.MatrixTable.addDefinitionRecord(
                new DynamicMatrixThresholdFilter.ThresholdRecord(0,
                    ThresholdMatrix.Samples.sampleScreenMatrix));
            dynamicThresholdFilter.MatrixTable.addDefinitionRecord(
                new DynamicMatrixThresholdFilter.ThresholdRecord(32,
                    ThresholdMatrix.Samples.createBayerDispersedDotMatrix(3)));
            dynamicThresholdFilter.MatrixTable.addDefinitionRecord(
                new DynamicMatrixThresholdFilter.ThresholdRecord(64,
                    ThresholdMatrix.Samples.createBayerDispersedDotMatrix(3), 1.0));
            dynamicThresholdFilter.MatrixTable.addDefinitionRecord(
                new DynamicMatrixThresholdFilter.ThresholdRecord(96,
                    ThresholdMatrix.Samples.createBayerDispersedDotMatrix(3), 0.5));
            dynamicThresholdFilter.MatrixTable.addDefinitionRecord(
                new DynamicMatrixThresholdFilter.ThresholdRecord(112,
                    ThresholdMatrix.Samples.simpleThreshold));
            dynamicThresholdFilter.MatrixTable.addDefinitionRecord(
                new DynamicMatrixThresholdFilter.ThresholdRecord(144,
                    ThresholdMatrix.Samples.createBayerDispersedDotMatrix(1)));
            dynamicThresholdFilter.MatrixTable.addDefinitionRecord(
                new DynamicMatrixThresholdFilter.ThresholdRecord(160,
                    ThresholdMatrix.Samples.createBayerDispersedDotMatrix(2)));
            dynamicThresholdFilter.MatrixTable.addDefinitionRecord(
                new DynamicMatrixThresholdFilter.ThresholdRecord(192,
                    ThresholdMatrix.Samples.simpleThreshold, 1.0));
            dynamicThresholdFilter.MatrixTable.addDefinitionRecord(
                new DynamicMatrixThresholdFilter.ThresholdRecord(224,
                    ThresholdMatrix.Samples.sampleScreenMatrix));
            thresholdHalftoneMethod.ThresholdFilter = dynamicThresholdFilter;
            thresholdHalftoneMethod.ErrorFilter = null;
            thresholdHalftoneMethod.ScanningOrder = new ScanlineScanningOrder();
            halftoneAlgorithm.Name =
                String.Format("[{0:d2}] Tone-dependent threshold filter", counter);
            halftoneAlgorithm.Description = "Screen 4x4 45deg angle, Bayer 8x8, Bayer 8x8 + perturbation 1.0, Bayer 8x8 + perturbation 0.5, threshold 0.5, Bayer 2x2, Bayer 4x4, white noise, screen";
            config.saveModule(halftoneAlgorithm, false);
            counter++;

            // ----------------------------------------------

            // Dynamic error filter

            thresholdHalftoneMethod.ThresholdFilter = new MatrixThresholdFilter();
            dynamicErrorFilter.MatrixTable.clearDefinitionRecords();
            dynamicErrorFilter.MatrixTable.addDefinitionRecord(
                    new DynamicMatrixErrorFilter.ErrorRecord(0,
                        ErrorMatrix.Samples.floydSteinberg));
            dynamicErrorFilter.MatrixTable.addDefinitionRecord(
                    new DynamicMatrixErrorFilter.ErrorRecord(64,
                        ErrorMatrix.Samples.jarvisJudiceNinke));
            dynamicErrorFilter.MatrixTable.addDefinitionRecord(
                    new DynamicMatrixErrorFilter.ErrorRecord(128,
                        ErrorMatrix.Samples.stucki));
            dynamicErrorFilter.MatrixTable.addDefinitionRecord(
                    new DynamicMatrixErrorFilter.ErrorRecord(192,
                        ErrorMatrix.Samples.sierra));
            thresholdHalftoneMethod.ErrorFilter = dynamicErrorFilter;
            thresholdHalftoneMethod.ScanningOrder = new ScanlineScanningOrder();
            halftoneAlgorithm.Name =
                String.Format("[{0:d2}] Tone-dependent error filter", counter);
            halftoneAlgorithm.Description = "Floyd-Steinberg, Jarvis-Judice-Ninke, Stucki, Sierra";
            config.saveModule(halftoneAlgorithm, false);
            counter++;

            // ----------------------------------------------

            config.save();

            // DEBUG:
            //foreach (Module module in config.listAllModules()) {
            //    Console.Out.WriteLine("type: {0}, name: {1}, desc: {2}",
            //        module.GetType(), module.Name, module.Description);
            //}
        }
    }
}