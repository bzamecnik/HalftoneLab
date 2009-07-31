using System;
using Gtk;
using HalftoneLab;

namespace HalftoneLab.GUI.Gtk
{
    /// <summary>
    /// A skeleton for module configuration dialogs.
    /// </summary>
    public abstract class ModuleDialog : Dialog
    {
        /// <summary>
        /// Original module is stored in case the dialog is canceled.
        /// </summary>
        protected Module originalModule;
        /// <summary>
        /// Modified module (deep copied from the original one).
        /// </summary>
        protected Module modifiedModule;

        /// <summary>
        /// Create a new module dialog with an existing module.
        /// </summary>
        /// <param name="module"></param>
        protected ModuleDialog(Module module) {
            originalModule = module;
            modifiedModule = (Module)module.deepCopy();
            string moduleName = module.GetType().Name;
            ModuleAttribute attribute =
                ModuleRegistry.Instance.getModuleAttribute(moduleName);
            Title = (attribute != null) ? attribute.TypeName : moduleName;
            Modal = true;

            AddButton("OK", ResponseType.Ok);
            AddButton("Cancel", ResponseType.Cancel);
        }

        /// <summary>
        /// Run the dialog and return the configured dialog.
        /// </summary>
        /// <returns>Modified dialog if the dialog was confirmed, or the
        /// original one if the dialog was canceled.</returns>
        public Module runConfiguration() {
            Module configuredModule = null;
            Response += new ResponseHandler(
                (object obj, ResponseArgs respArgs) =>
                {
                    if (respArgs.ResponseId == ResponseType.Ok) {
                        save();
                        configuredModule = modifiedModule;
                    } else {
                        configuredModule = originalModule;
                    }
                }
              );
            Run();
            Destroy();
            return configuredModule;
        }

        /// <summary>
        /// Make a dialog from the module type name and run it to configure an
        /// existing module.
        /// </summary>
        /// <param name="moduleTypeName">Module type name to find the proper
        /// dialog type for it</param>
        /// <param name="existingModule">Existing module to be configured
        /// </param>
        /// <returns>Configured module</returns>
        public static Module configureModule(
            string moduleTypeName,
            Module existingModule) {
            if (moduleTypeName == null) {
                return null;
            }
            ModuleRegistry moduleRegistry = ModuleRegistry.Instance;
            Type moduleType = moduleRegistry.getModuleType(moduleTypeName);
            Type dialogType = moduleRegistry.getDialogType(moduleTypeName);
            if ((moduleType == null) || (dialogType == null)) {
                return null;
            }
            ModuleDialog dialog = null;
            System.Reflection.ConstructorInfo ci;
            if (existingModule != null) {
                ci = dialogType.GetConstructor(new Type[] { moduleType });
                if (ci != null) {
                    dialog = ci.Invoke(new object[] { existingModule })
                        as ModuleDialog;
                }
            } else {
                ci = dialogType.GetConstructor(new Type[0]);
                if (ci != null) {
                    dialog = ci.Invoke(null) as ModuleDialog;
                }
            }
            if (dialog != null) {
                return dialog.runConfiguration();
            }
            return null;
        }

        /// <summary>
        /// Make an instance of given module type.
        /// </summary>
        /// <remarks>
        /// The module need to have a default constructor.
        /// </remarks>
        /// <param name="type">Module type</param>
        /// <returns>Instance of that module type or null is something went
        /// wrong</returns>
        public static Module instantiateModule(Type type) {
            System.Reflection.ConstructorInfo ci =
                type.GetConstructor(new Type[0]);
            return (ci != null) ? ci.Invoke(null) as Module : null;
        }

        /// <summary>
        /// Save the contents of the dialog to the module being configured.
        /// </summary>
        /// <remarks>
        /// It is called on an OK response.
        /// </remarks>
        protected virtual void save() { }
    }
}
