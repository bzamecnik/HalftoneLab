using System;
using Halftone;
using Gtk;
using Gimp.HalftoneLab;

namespace testing
{
    class Program
    {
        static void Main(string[] args) {
           ConfigManagerTest.run();
           //ConfigGUI.run();
        }
    }

    class ConfigGUI {
        public static void run() {
            Application.Init();

            Window window = new Window("Halftone Laboratory");
            window.DeleteEvent += new DeleteEventHandler(
                delegate { Application.Quit(); });

            HalftoneMethod module;
            SubmoduleSelector<HalftoneMethod>
                halftoneAlgorithmSelector =
                new SubmoduleSelector<HalftoneMethod>();
            halftoneAlgorithmSelector.ModuleChanged += delegate
            {
                module = halftoneAlgorithmSelector.Module;
                Console.WriteLine(module);
            };

            Table table = new Table(1, 2, false)
                { ColumnSpacing = 5, RowSpacing = 5, BorderWidth = 5 };
            table.Attach(new Label("Halftone algorithm"), 0, 1, 0, 1,
                AttachOptions.Fill, AttachOptions.Shrink, 0, 0);
            table.Attach(halftoneAlgorithmSelector, 1, 2, 0, 1,
                AttachOptions.Fill | AttachOptions.Expand,
                AttachOptions.Shrink, 0, 0);
            table.ShowAll();

            window.Add(table);
            window.Show();

            Application.Run();
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
            dynamicMatrixErrorFilter.MatrixTable.addRecord(new DynamicMatrixErrorFilter.ErrorRecord(0, ErrorMatrix.Samples.floydSteinberg));
            dynamicMatrixErrorFilter.MatrixTable.addRecord(new DynamicMatrixErrorFilter.ErrorRecord(64, ErrorMatrix.Samples.jarvisJudiceNinke));
            dynamicMatrixErrorFilter.MatrixTable.addRecord(new DynamicMatrixErrorFilter.ErrorRecord(128, ErrorMatrix.Samples.stucki));
            dynamicMatrixErrorFilter.MatrixTable.addRecord(new DynamicMatrixErrorFilter.ErrorRecord(192, ErrorMatrix.Samples.nextPixel));
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
            dynamicThresholdFilter.MatrixTable.addRecord(new DynamicMatrixThresholdFilter.ThresholdRecord(0, ThresholdMatrix.Generator.sampleMatrix, 0.0));
            dynamicThresholdFilter.MatrixTable.addRecord(new DynamicMatrixThresholdFilter.ThresholdRecord(32, ThresholdMatrix.Generator.sampleMatrix, 0.125));
            dynamicThresholdFilter.MatrixTable.addRecord(new DynamicMatrixThresholdFilter.ThresholdRecord(64, ThresholdMatrix.Generator.sampleMatrix, 0.25));
            dynamicThresholdFilter.MatrixTable.addRecord(new DynamicMatrixThresholdFilter.ThresholdRecord(96, ThresholdMatrix.Generator.sampleMatrix, 0.375));
            dynamicThresholdFilter.MatrixTable.addRecord(new DynamicMatrixThresholdFilter.ThresholdRecord(128, ThresholdMatrix.Generator.sampleMatrix, 0.5));
            dynamicThresholdFilter.MatrixTable.addRecord(new DynamicMatrixThresholdFilter.ThresholdRecord(160, ThresholdMatrix.Generator.sampleMatrix, 0.625));
            dynamicThresholdFilter.MatrixTable.addRecord(new DynamicMatrixThresholdFilter.ThresholdRecord(192, ThresholdMatrix.Generator.sampleMatrix, 0.75));
            dynamicThresholdFilter.MatrixTable.addRecord(new DynamicMatrixThresholdFilter.ThresholdRecord(224, ThresholdMatrix.Generator.sampleMatrix, 0.875));

            dynamicThresholdFilter.MatrixTable.addRecord(new DynamicMatrixThresholdFilter.ThresholdRecord(100, ThresholdMatrix.Generator.simpleThreshold));
            dynamicThresholdFilter.MatrixTable.addRecord(new DynamicMatrixThresholdFilter.ThresholdRecord(150, ThresholdMatrix.Generator.createBayerDispersedDotMatrix(3)));

            //config.saveModule(dynamicThresholdFilter, false);

            ThresholdHalftoneMethod thresholdHalftoneAlgorithm = new ThresholdHalftoneMethod()
            {
                Name = "Threshold halftone algorithm"
            };
            config.saveModule(thresholdHalftoneAlgorithm, false);

            thresholdHalftoneAlgorithm.ErrorFilter = matrixErrorFilter;
            thresholdHalftoneAlgorithm.ScanningOrder = scanlineScanOrder;
            thresholdHalftoneAlgorithm.Name = "Floyd-Steinberg, scanline";
            thresholdHalftoneAlgorithm.Description = "Floyd-Steinberg error, Scanline order";
            config.saveModule(thresholdHalftoneAlgorithm, false);

            thresholdHalftoneAlgorithm.ScanningOrder = serpentineScanOrder;
            thresholdHalftoneAlgorithm.Name = "Floyd-Steinberg, serpentine";
            thresholdHalftoneAlgorithm.Description = "Floyd-Steinberg error, Serpentine order";
            config.saveModule(thresholdHalftoneAlgorithm, false);

            thresholdHalftoneAlgorithm.ScanningOrder = scanlineScanOrder;
            thresholdHalftoneAlgorithm.ErrorFilter = dynamicMatrixErrorFilter;
            thresholdHalftoneAlgorithm.Name = "Dynamic error filter";
            thresholdHalftoneAlgorithm.Description = "floydSteinberg, jarvisJudiceNinke, stucki, nextPixel, Scanline order";
            config.saveModule(thresholdHalftoneAlgorithm, false);

            thresholdHalftoneAlgorithm.ScanningOrder = scanlineScanOrder;
            thresholdHalftoneAlgorithm.ErrorFilter = null;
            thresholdHalftoneAlgorithm.ThresholdFilter = matrixThresholdFilter;
            thresholdHalftoneAlgorithm.Name = "Bayer dispersed halftone";
            config.saveModule(thresholdHalftoneAlgorithm, false);

            thresholdHalftoneAlgorithm.ThresholdFilter = new MatrixThresholdFilter(ThresholdMatrix.Generator.sampleMatrix);
            thresholdHalftoneAlgorithm.Name = "Clustered dot halftone";
            config.saveModule(thresholdHalftoneAlgorithm, false);

            thresholdHalftoneAlgorithm.ThresholdFilter = dynamicThresholdFilter;
            thresholdHalftoneAlgorithm.Name = "Dynamic threshold halftone";
            config.saveModule(thresholdHalftoneAlgorithm, false);

            dynamicThresholdFilter.MatrixTable.clearRecords();
            dynamicThresholdFilter.MatrixTable.addRecord(new DynamicMatrixThresholdFilter.ThresholdRecord(0, ThresholdMatrix.Generator.simpleThreshold, 1.0));
            thresholdHalftoneAlgorithm.ThresholdFilter = dynamicThresholdFilter;
            thresholdHalftoneAlgorithm.Name = "Simpe blue-noise";
            config.saveModule(thresholdHalftoneAlgorithm, false);

            thresholdHalftoneAlgorithm.ErrorFilter = matrixErrorFilter;
            thresholdHalftoneAlgorithm.Name = "Blue-noise Floyd-Steinberg, scanline";
            config.saveModule(thresholdHalftoneAlgorithm, false);

            thresholdHalftoneAlgorithm.ScanningOrder = serpentineScanOrder;
            thresholdHalftoneAlgorithm.Name = "Blue-noise, Floyd-Steinberg, serpentine";
            config.saveModule(thresholdHalftoneAlgorithm, false);

            thresholdHalftoneAlgorithm.ScanningOrder = scanlineScanOrder;
            thresholdHalftoneAlgorithm.ThresholdFilter = new MatrixThresholdFilter();

            //thresholdHalftoneAlgorithm.ErrorFilter = dynamicMatrixErrorFilter;
            //thresholdHalftoneAlgorithm.Name = "Dynamic error filter";
            //config.saveModule(thresholdHalftoneAlgorithm, false);

            //thresholdHalftoneAlgorithm.ErrorFilter = perturbedErrorFilter;
            //thresholdHalftoneAlgorithm.Name = "Perturbed error filter";
            //config.saveModule(thresholdHalftoneAlgorithm, false);

            thresholdHalftoneAlgorithm.ErrorFilter = randomizedMatrixErrorFilter;
            thresholdHalftoneAlgorithm.Name = "Randomized error filter";
            config.saveModule(thresholdHalftoneAlgorithm, false);

            thresholdHalftoneAlgorithm.ScanningOrder = hilbertScanOrder;
            thresholdHalftoneAlgorithm.ErrorFilter = vectorErrorFilter;
            thresholdHalftoneAlgorithm.Name = "SFC Threshold halftone algorithm";
            thresholdHalftoneAlgorithm.Description = "Hilbert SFC order, next pixel vector error filter";
            config.saveModule(thresholdHalftoneAlgorithm, false);

            SFCClusteringMethod sfcAdaptiveClustering = new SFCClusteringMethod()
            {
                Name = "SFC Clustering",
                ScanningOrder = hilbertScanOrder,
                ErrorFilter = new VectorErrorFilter(ErrorMatrix.Samples.nextTwoPixels)
            };
            config.saveModule(sfcAdaptiveClustering, false);

            SpotFunction euclidDotSpotFunction = new SpotFunction(
                        SpotFunction.Samples.euclidDot, Math.PI * 0.25, 8);

            thresholdHalftoneAlgorithm.ThresholdFilter =
                new SpotFunctionThresholdFilter(euclidDotSpotFunction);
            thresholdHalftoneAlgorithm.ScanningOrder = scanlineScanOrder;
            thresholdHalftoneAlgorithm.ErrorFilter = null;
            thresholdHalftoneAlgorithm.Name = "Halftoning with Euclid dot, direct";
            thresholdHalftoneAlgorithm.Description = "";
            config.saveModule(thresholdHalftoneAlgorithm, false);

            euclidDotSpotFunction.Distance = 32 ;
            euclidDotSpotFunction.Angle = Math.PI * 0.25;
            thresholdHalftoneAlgorithm.ThresholdFilter =
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
            thresholdHalftoneAlgorithm.Name = "Halftoning with Euclid dot, using Image, with effects";
            thresholdHalftoneAlgorithm.Description = "";
            config.saveModule(thresholdHalftoneAlgorithm, false);

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
