using System;
using System.Collections.Generic;
using HalftoneLab;
using System.Linq;

namespace HalftoneLab.GUI.Gtk
{
    /// <summary>
    /// Registry of modules that can be configured via %GUI.
    /// </summary>
    /// <remarks>
    /// <para>
    /// In this registry is stored information about all available modules
    /// (descendants of class HalftoneLab.Module), their submodules, attributes.
    /// Configuration dialog types are assigned to module types when available.
    /// </para>
    /// <para>
    /// ModuleRegistry is available in form of Singleton design pattern.
    /// </para>
    /// </remarks>
    public class ModuleRegistry
    {
        // TODO:
        // - put all concrete subclasses of given module class in a tree

        /// <summary>
        /// Information about a module. A basic record of the registry.
        /// </summary>
        /// <remarks>
        /// It includes module type, assigned dialog type, submodule types and
        /// module attributes.
        /// </remarks>
        public class ModuleInfo {
            public Type moduleType;
            public Type dialogType;
            public string[] submodules;
            public ModuleAttribute moduleAttribute;

            /// <summary>
            /// Create a new module information record without assigning
            /// a dialog.
            /// </summary>
            /// <param name="moduleType">Type of module</param>
            /// <param name="submodules">List of submodule type names
            /// (without namespace)</param>
            public ModuleInfo(Type moduleType, string[] submodules)
                : this(moduleType, null, submodules) {}

            /// <summary>
            /// Create a new module information record with a dialog assigned.
            /// </summary>
            /// <param name="moduleType">Type of module</param>
            /// <param name="dialogType">Type of module configuration dialog
            /// </param>
            /// <param name="submodules">List of submodule type names
            /// (without namespace)</param>
            public ModuleInfo(Type moduleType, Type dialogType,
                string[] submodules)
            {
                this.moduleType = moduleType;
                this.dialogType = dialogType;
                this.submodules = submodules;
                this.moduleAttribute = getModuleAttribute(moduleType);
            }
        }

        /// <summary>
        /// Registry of module information records.
        /// </summary>
        /// <remarks>
        /// Key is module type name (without namespace).
        /// </remarks>
        private Dictionary<string, ModuleInfo> registry;

        private static ModuleRegistry instance;

        /// <summary>
        /// Singleton instance. Created on demand.
        /// </summary>
        public static ModuleRegistry Instance {
            get {
                if (instance == null) {
                    instance = new ModuleRegistry();
                }
                return instance;
            }
        }

        /// <summary>
        /// Create a new module registry and initialize it.
        /// </summary>
        private ModuleRegistry() {
            registry = new Dictionary<string, ModuleInfo>();
            initialize();
        }

        /// <summary>
        /// Initialize module registry.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Find modules and their submodules using reflection.
        /// A module is regarded as a submodule of a module if it
        /// is its descendant and is not abstract.
        /// </para>
        /// <para>
        /// Possible problem: Namespace is stripped from module class names,
        /// so there might be a problem in future. There could be for example
        /// another libraries with an additional modules sitting in a different
        /// namespaces, but with the same class names. A name conflict then
        /// arises...
        /// </para>
        /// <para>
        /// TODO: Assign dialog types to module types should be done in static
        /// constructors or particular dialogs. But their instances need
        /// to be referenced from somewhere...
        /// </para>
        /// </remarks>
        private void initialize() {
            System.Reflection.Assembly assembly =
                System.Reflection.Assembly.GetAssembly(
                typeof(HalftoneLab.Module));
            List<Type> moduleTypes = assembly.GetTypes().Where(
                    (type) => typeof(HalftoneLab.Module).IsAssignableFrom(type)
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

        /// <summary>
        /// Get module type given its type name.
        /// </summary>
        /// <param name="moduleTypeName">Module type name (without namespaces)
        /// </param>
        /// <returns>Module type or null if there is no such a module</returns>
        public Type getModuleType(string moduleTypeName) {
            ModuleInfo record = null;
            registry.TryGetValue(moduleTypeName, out record);
            return (record != null) ? record.moduleType : null;
        }

        /// <summary>
        /// Get the type of dialog assigned to a module with given type name.
        /// </summary>
        /// <param name="moduleTypeName">Module type name (without namespaces)
        /// </param>
        /// <returns>Dialog type or null if there is no such a module</returns>
        public Type getDialogType(string moduleTypeName) {
            ModuleInfo record = null;
            registry.TryGetValue(moduleTypeName, out record);
            return (record != null) ? record.dialogType : null;
        }

        /// <summary>
        /// List submodules of a module given its type name.
        /// </summary>
        /// <param name="moduleTypeName">Module type name (without namespaces)
        /// </param>
        /// <returns>List of submodule type names (without namespaces),
        /// or an empty array if there is no such a module or it has no
        /// submodules</returns>
        public string[] getSubmodules(string moduleTypeName) {
            ModuleInfo record = null;
            registry.TryGetValue(moduleTypeName, out record);
            string[] submodules = (record != null) ? record.submodules : null;
            return (submodules != null) ? submodules : new string[0];
        }

        /// <summary>
        /// Get ModuleAttribute information of a module given its type name.
        /// </summary>
        /// <param name="moduleTypeName">Module type name (without namespaces)
        /// </param>
        /// <returns>ModuleAttribute or null if there is no such a module
        /// </returns>
        public ModuleAttribute getModuleAttribute(string moduleTypeName) {
            ModuleInfo record = null;
            registry.TryGetValue(moduleTypeName, out record);
            return (record != null) ? record.moduleAttribute : null;
        }

        /// <summary>
        /// Get ModuleAttribute information of a module given its type.
        /// </summary>
        /// <param name="moduleType">Module type</param>
        /// <returns>ModuleAttribute or null if there is no such a module
        /// </returns>
        public static ModuleAttribute getModuleAttribute(Type moduleType) {
            foreach (object attribute in moduleType.GetCustomAttributes(false))
            {
                if (attribute is ModuleAttribute) {
                    return (ModuleAttribute)attribute;
                }
            }
            return null;
        }

        /// <summary>
        /// Add a module to the registry.
        /// </summary>
        /// <param name="moduleType">Type of module being added</param>
        /// <param name="dialogType">Dialog type</param>
        /// <param name="submodules">List of submodule type names
        /// (without namespaces)</param>
        private void addModule(Type moduleType, Type dialogType,
            string[] submodules)
        {
            registry.Add(moduleType.Name, new ModuleInfo(moduleType,
                dialogType, submodules));
        }

        /// <summary>
        /// Assign a dialog type to a module given its type name.
        /// </summary>
        /// <param name="moduleTypeName">Module type name (without namespaces)
        /// </param>
        /// <param name="dialogType">Dialog type</param>
        public void assignDialog(string moduleTypeName, Type dialogType) {
            ModuleInfo record = null;
            registry.TryGetValue(moduleTypeName, out record);
            if (record != null) {
                record.dialogType = dialogType;
            }
        }
    }
}
