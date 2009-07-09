using System;
using Gtk;
using Halftone;

namespace Gimp.HalftoneLab
{
    // TODO:
    // *- show ModuleAttribute.TypeName as ComboBox text
    // - show ModuleAttribute.TypeDescription as a ComboBox tool-tip
    //   or in a separate frame
    // *- null checkbox

    public class SubmoduleSelector<ModuleType> : Table
        where ModuleType : Module
    {
        private ComboBox typeComboBox;
        private ListStore typeStore;
        private Button editButton;
        private CheckButton nullCheckButton;

        private ModuleType module;
        public ModuleType Module {
            get { return (!AllowNull || !IsNull) ? module : null; }
            private set {
                module = value;
                if (editButton != null) {
                    editButton.Sensitive = ((module != null) &&
                    (ModuleRegistry.Instance.getDialogType(
                        module.GetType().Name) != null));
                }
                if (ModuleChanged != null) {
                    ModuleChanged(this, new EventArgs());
                }
            }
        }

        // Allow the module to be set to null?
        private bool allowNull;
        public bool AllowNull {
            get { return allowNull; }
            set {
                allowNull = value;
                if (nullCheckButton != null) {
                    nullCheckButton.Sensitive = allowNull;
                }
            }
        }

        private bool isNull;
        // Module is set to null
        private bool IsNull {
            get { return isNull; }
            set {
                if (AllowNull) {
                    bool changed = (isNull != value);
                    isNull = value;
                    if (changed && (ModuleChanged != null)) {
                        ModuleChanged(this, new EventArgs());
                    }
                }
            }
        }

        public event EventHandler ModuleChanged;

        public SubmoduleSelector() 
            : this(null){}

        public SubmoduleSelector(ModuleType existingModule)
            : base(1, 3, false)
        {
            ColumnSpacing = RowSpacing = 5;
            editButton = new Button("Edit");
            nullCheckButton = new CheckButton("Null");
            Module = existingModule;
            AllowNull = false;

            ModuleRegistry moduleRegistry = ModuleRegistry.Instance;

            nullCheckButton.Toggled += delegate
            {
                IsNull = nullCheckButton.Active;
            };

            typeStore = new ListStore(typeof(string), typeof(Type));
            string[] errorFilterSubtypes = moduleRegistry.getSubmodules(
                typeof(ModuleType).Name);
            foreach (string moduleTypeName in errorFilterSubtypes) {
                Type type = moduleRegistry.getModuleType(moduleTypeName);
                ModuleAttribute attribute =
                    moduleRegistry.getModuleAttribute(moduleTypeName);
                string text = ((attribute != null) && (attribute.TypeName != null))
                    ? attribute.TypeName : moduleTypeName;
                typeStore.AppendValues(text, type);
            }
            typeComboBox = new ComboBox(typeStore);
            CellRendererText renderer = new CellRendererText();
            typeComboBox.PackStart(renderer, true);
            typeComboBox.SetAttributes(renderer, "text", 0);
            
            nullCheckButton.Active = Module == null;
            // set existing module type as active
            if (Module != null) {
                string activeTypeName = Module.GetType().Name;
                typeComboBox.Active = Array.FindIndex(
                    errorFilterSubtypes, (string type) => activeTypeName == type);
            }
            typeComboBox.Changed += delegate
            {
                ModuleType selectedModule = ConfigDialog.instantiateModule(
                    ActiveModuleType) as ModuleType;
                if (selectedModule != null) {
                    Module = selectedModule;
                }
                nullCheckButton.Active = Module == null;
            };

            editButton.Clicked += delegate
            {
                if (Module != null) {
                    Type activeType = ActiveModuleType;
                    Module mod = (Module.GetType() == activeType)
                        ? Module : null;
                    ModuleType configuredModule = null;
                    if (activeType != null) {
                        configuredModule =
                            ConfigDialog.configureModule(activeType.Name, mod)
                                as ModuleType;
                    }
                    if (configuredModule != null) {
                        Module = configuredModule;
                    }
                }
            };

            Attach(typeComboBox, 0, 1, 0, 1,
                    AttachOptions.Fill | AttachOptions.Expand,
                    AttachOptions.Shrink, 0, 0);
            Attach(editButton, 1, 2, 0, 1,
                AttachOptions.Fill, AttachOptions.Shrink, 0, 0);
            Attach(nullCheckButton, 2, 3, 0, 1,
                AttachOptions.Fill, AttachOptions.Shrink, 0, 0);
            ShowAll();
        }

        private Type ActiveModuleType {
            get {
                Type type = null;
                if ((typeComboBox != null) && (typeStore != null)) {
                    TreeIter iter;
                    if (typeComboBox.GetActiveIter(out iter)) {
                        type = typeStore.GetValue(iter, 1) as Type;
                    }
                }
                return type;
            }
        }
    }
}
