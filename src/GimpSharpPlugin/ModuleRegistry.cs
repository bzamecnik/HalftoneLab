using System;
using System.Collections.Generic;
using Halftone;
using System.Linq;

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

            public ModuleInfo(Type moduleType, string[] submodules)
                : this(moduleType, null, submodules) {}

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
            // Find modules and their submodules using reflection.
            // A module is regarded as a submodule of a module if it
            // is its descendant and is not abstract.

            System.Reflection.Assembly assembly =
                System.Reflection.Assembly.GetAssembly(
                typeof(Halftone.Module));
            List<Type> moduleTypes = assembly.GetTypes().Where(
                    (type) => typeof(Halftone.Module).IsAssignableFrom(type)
                ).ToList();
            moduleTypes.OrderBy(type => type.Name);
            //moduleTypes.Sort((t1, t2) => t1.Name.CompareTo(t2.Name));
            foreach (Type module in moduleTypes) {
                List<string> submoduleNames = new List<string>();
                // TODO: this should be optimized!
                foreach (Type otherType in moduleTypes) {
                    if (module.IsAssignableFrom(otherType) &&
                        !otherType.IsAbstract) {
                        submoduleNames.Add(otherType.Name);
                    }
                }
                addModule(module, null, submoduleNames.ToArray());
            }

            // assign dialogs to modules

            // TODO: This could be done in static constructors
            // or particular dialogs, but still their instances need
            // to be referenced somewhere.

            assignDialog("MatrixThresholdFilter",
                typeof(MatrixThresholdFilterDialog));
            assignDialog("DynamicMatrixThresholdFilter",
                typeof(DynamicMatrixThresholdFilterDialog));
            assignDialog("ImageThresholdFilter",
                typeof(ImageThresholdFilterDialog));
            assignDialog("SpotFunctionThresholdFilter",
                typeof(SpotFunctionThresholdFilterDialog));

            assignDialog("DynamicMatrixErrorFilter",
                typeof(DynamicMatrixErrorFilterDialog));
            assignDialog("RandomizedMatrixErrorFilter",
                typeof(RandomizedMatrixErrorFilterDialog));
            assignDialog("PerturbedErrorFilter",
                typeof(PerturbedErrorFilterDialog));
            assignDialog("VectorErrorFilter",
                typeof(VectorErrorFilterDialog));
            assignDialog("MatrixErrorFilter",
                typeof(MatrixErrorFilterDialog));
            
            assignDialog("ThresholdHalftoneMethod",
                typeof(ThresholdHalftoneMethodDialog));
            assignDialog("SFCClusteringMethod",
                typeof(SFCClusteringMethodDialog));
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

        public void assignDialog(string moduleTypeName, Type dialogType) {
            ModuleInfo record = null;
            registry.TryGetValue(moduleTypeName, out record);
            if (record != null) {
                record.dialogType = dialogType;
            }
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
