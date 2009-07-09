using System;
using Gtk;
using Halftone;

namespace Gimp.HalftoneLab
{
    public abstract class ConfigDialog : Dialog
    {
        protected Module originalModule;
        protected Module modifiedModule;

        protected ConfigDialog(Module module) {
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

        public Module runConfiguration() {
            Module configuredModule = null;
            Response += new ResponseHandler(
                (object obj, ResponseArgs respArgs) =>
                {
                    configuredModule =
                        (respArgs.ResponseId == ResponseType.Ok) ?
                        modifiedModule : originalModule;
                }
              );
            Run();
            Destroy();
            return configuredModule;
        }

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
            ConfigDialog dialog = null;
            System.Reflection.ConstructorInfo ci;
            if (existingModule != null) {
                ci = dialogType.GetConstructor(new Type[] { moduleType });
                if (ci != null) {
                    dialog = ci.Invoke(new object[] { existingModule })
                        as ConfigDialog;
                }
            } else {
                ci = dialogType.GetConstructor(new Type[0]);
                if (ci != null) {
                    dialog = ci.Invoke(null) as ConfigDialog;
                }
            }
            if (dialog != null) {
                return dialog.runConfiguration();
            }
            return null;
        }

        public static Module instantiateModule(Type type) {
            System.Reflection.ConstructorInfo ci =
                type.GetConstructor(new Type[0]);
            return (ci != null) ? ci.Invoke(null) as Module : null;
        }
    }
}
