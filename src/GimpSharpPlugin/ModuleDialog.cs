using System;
using Gtk;
using Halftone;

namespace Gimp.HalftoneLab
{
    public abstract class ModuleDialog : Dialog
    {
        protected Module originalModule;
        protected Module modifiedModule;

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

        public static Module instantiateModule(Type type) {
            System.Reflection.ConstructorInfo ci =
                type.GetConstructor(new Type[0]);
            return (ci != null) ? ci.Invoke(null) as Module : null;
        }

        // save form contents to the configured module
        // called on response OK
        protected virtual void save() { }
    }
}
