using System;
using System.Collections.Generic;
using Halftone;

namespace Gimp.HalftoneLab
{
    public class ModuleRegistry
    {
        // TODO:
        // - put all concrete subclasses of given module class in a tree

        public class ModuleInfo {
            public Type moduleType;
            public Type dialogType;
            public string[] submodules;
            public ModuleAttribute moduleAttribute;

            public ModuleInfo(Type moduleType, Type dialogType,
                string[] submodules)
            {
                this.moduleType = moduleType;
                this.dialogType = dialogType;
                this.submodules = submodules;
                this.moduleAttribute = getModuleAttribute(moduleType);
            }
        }

        private Dictionary<string, ModuleInfo> registry;
        private static ModuleRegistry instance;

        public static ModuleRegistry Instance {
            get {
                if (instance == null) {
                    instance = new ModuleRegistry();
                }
                return instance;
            }
        }

        private ModuleRegistry() {
            registry = new Dictionary<string, ModuleInfo>();
            initialize();
        }

        private void initialize() {
            addModule(typeof(MatrixThresholdFilter),
                typeof(MatrixThresholdFilterDialog), null);
            addModule(typeof(DynamicMatrixThresholdFilter),
                typeof(DynamicMatrixThresholdFilterDialog), null);
            addModule(typeof(ImageThresholdFilter),
                typeof(ImageThresholdFilterDialog), null);
            addModule(typeof(SpotFunctionThresholdFilter),
                typeof(SpotFunctionThresholdFilterDialog), null);
            addModule(typeof(ThresholdFilter),
                null, new string[] {
                    "MatrixThresholdFilter",
                    "DynamicMatrixThresholdFilter",
                    "ImageThresholdFilter",
                    "SpotFunctionThresholdFilter"
                });

            addModule(typeof(DynamicMatrixErrorFilter),
                typeof(DynamicMatrixErrorFilterDialog),null);
            addModule(typeof(RandomizedMatrixErrorFilter),
                typeof(RandomizedMatrixErrorFilterDialog), null);
            addModule(typeof(PerturbedErrorFilter),
                typeof(PerturbedErrorFilterDialog), null);
            addModule(typeof(VectorErrorFilter),
                typeof(VectorErrorFilterDialog),
                new string[] { "VectorErrorFilter" }
                );
            addModule(typeof(MatrixErrorFilter),
                typeof(MatrixErrorFilterDialog), new string[] {
                    "MatrixErrorFilter",
                    "RandomizedMatrixErrorFilter",
                    "DynamicMatrixErrorFilter"
                });
            addModule(typeof(ErrorFilter),
                null, new string[] {
                    "MatrixErrorFilter",
                    "DynamicMatrixErrorFilter",
                    "RandomizedMatrixErrorFilter",
                    "PerturbedErrorFilter",
                    "VectorErrorFilter"
                });
            
            addModule(typeof(ThresholdHalftoneMethod),
                typeof(ThresholdHalftoneMethodDialog), null);
            addModule(typeof(SFCClusteringMethod),
                typeof(SFCClusteringMethodDialog), null);
            addModule(typeof(HalftoneMethod),
                null, new string[] {
                    "ThresholdHalftoneMethod",
                    "SFCClusteringMethod"
                });
            addModule(typeof(CellHalftoneMethod),
                null, new string[] {
                    "SFCClusteringMethod"
                });
            addModule(typeof(PointHalftoneMethod),
                null, new string[] {
                    "ThresholdHalftoneMethod"
                });

            addModule(typeof(SpotFunction),
                typeof(SpotFunctionDialog), null);

            addModule(typeof(ScanlineScanningOrder), null, null);
            addModule(typeof(SerpentineScanningOrder), null, null);
            addModule(typeof(HilbertScanningOrder), null, null);
            addModule(typeof(ScanningOrder),
                null, new string[] {
                    "ScanlineScanningOrder",
                    "SerpentineScanningOrder",
                    "HilbertScanningOrder"
                });
            addModule(typeof(SFCScanningOrder),
                null, new string[] {
                    "HilbertScanningOrder"
                });
        }

        public Type getModuleType(string moduleTypeName) {
            ModuleInfo record = null;
            registry.TryGetValue(moduleTypeName, out record);
            return (record != null) ? record.moduleType : null;
        }

        public Type getDialogType(string moduleTypeName) {
            ModuleInfo record = null;
            registry.TryGetValue(moduleTypeName, out record);
            return (record != null) ? record.dialogType : null;
        }

        public string[] getSubmodules(string moduleTypeName) {
            ModuleInfo record = null;
            registry.TryGetValue(moduleTypeName, out record);
            string[] submodules = (record != null) ? record.submodules : null;
            return (submodules != null) ? submodules : new string[0];
        }

        public ModuleAttribute getModuleAttribute(string moduleTypeName) {
            ModuleInfo record = null;
            registry.TryGetValue(moduleTypeName, out record);
            return (record != null) ? record.moduleAttribute : null;
        }

        private void addModule(Type moduleType, Type dialogType,
            string[] submodules)
        {
            registry.Add(moduleType.Name, new ModuleInfo(moduleType,
                dialogType, submodules));
        }

        public static ModuleAttribute getModuleAttribute(Type moduleType) {
            foreach (object attribute in moduleType.GetCustomAttributes(false)) {
                if (attribute is ModuleAttribute) {
                    return (ModuleAttribute)attribute;
                }
            }
            return null;
        }
    }
}
