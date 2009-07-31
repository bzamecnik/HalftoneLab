using System;
using Gtk;
using HalftoneLab;

namespace HalftoneLab.GUI.Gtk
{
    // TODO:
    // - show ModuleAttribute.TypeDescription as a ComboBox tool-tip
    //   or in a separate frame

    /// <summary>
    /// A panel for selecting and configuring a submodule.
    /// </summary>
    /// <typeparam name="ModuleType">Type of the submodule</typeparam>
    public class SubmoduleSelector<ModuleType> : Table
        where ModuleType : Module
    {
        private ComboBox typeComboBox;
        private ListStore typeStore;
        private Button editButton;
        private CheckButton nullCheckButton;

        private string[] subtypeNames;

        private ModuleType module;
        
        /// <summary>
        /// The configured submodule.
        /// </summary>
        public ModuleType Module {
            get { return (!AllowNull || !IsNull) ? module : null; }
            set {
                assignModule(value, true);
            }
        }

        private bool allowNull;

        /// <summary>
        /// Allow the module to be set to null, ie. disable it?
        /// </summary>
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
        /// <summary>
        /// Is %Module set to null?
        /// </summary>
        /// <remarks>
        /// This property is there instead of just setting the module to null
        /// in order to temporarily disable the module. In case the user sets
        /// this propery back to false, the module remains there, instead of
        /// being deleted.
        /// </remarks>
        private bool IsNull {
            get { return isNull; }
            set {
                if (AllowNull) {
                    bool changed = (isNull != value);
                    isNull = value;
                    if (changed) {
                        if (editButton != null) {
                            editButton.Sensitive = (module != null) && !isNull;
                        }
                        if (ModuleChanged != null) {
                            ModuleChanged(this, new EventArgs());
                        }
                    }
                }
            }
        }

        /// <summary>
        /// %Module configured by this panel has changed. You can retrieve it
        /// via the %Module property.
        /// </summary>
        public event EventHandler ModuleChanged;

        /// <summary>
        /// Create a new submodule selector (with no module - null).
        /// </summary>
        public SubmoduleSelector() 
            : this(null){}

        /// <summary>
        /// Create a new submodule selector with an existing module.
        /// </summary>
        /// <param name="existingModule"></param>
        public SubmoduleSelector(ModuleType existingModule)
            : base(1, 3, false)
        {
            ColumnSpacing = RowSpacing = 5;
            editButton = new Button("gtk-edit");
            nullCheckButton = new CheckButton("null");
            AllowNull = false;

            ModuleRegistry moduleRegistry = ModuleRegistry.Instance;

            typeStore = new ListStore(typeof(string), typeof(Type));
            subtypeNames = moduleRegistry.getSubmodules(
                typeof(ModuleType).Name);
            foreach (string moduleTypeName in subtypeNames) {
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

            nullCheckButton.Toggled += delegate
            {
                IsNull = nullCheckButton.Active;
            };
            
            assignModule(existingModule, false);

            typeComboBox.Changed += delegate
            {
                Type activeModuleType = ActiveModuleType;
                Type moduleType = (Module != null) ? Module.GetType() : null;
                if ((activeModuleType != null) &&
                    (activeModuleType != moduleType)) {
                    ModuleType selectedModule = ModuleDialog.instantiateModule(
                        activeModuleType) as ModuleType;
                    if (selectedModule != null) {
                        Module = selectedModule;
                    }
                    nullCheckButton.Active = Module == null;
                }
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
                            ModuleDialog.configureModule(activeType.Name, mod)
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

        /// <summary>
        /// Set the module to be configured and optionally fire the
        /// ModuleChanged event.
        /// </summary>
        /// <param name="module">Module to be configured</param>
        /// <param name="fireEvent">Fire the ModuleChanged event?</param>
        public void assignModule(ModuleType module, bool fireEvent) {
            this.module = module;
            editButton.Sensitive = ((module != null) &&
            (ModuleRegistry.Instance.getDialogType(
                module.GetType().Name) != null));
            // set existing module type as active
            if (module != null) {
                string activeTypeName = module.GetType().Name;
                typeComboBox.Active = Array.FindIndex(
                    subtypeNames, (string type) => activeTypeName == type);
            }
            nullCheckButton.Active = Module == null;
            if (fireEvent && (ModuleChanged != null)) {
                ModuleChanged(this, new EventArgs());
            }
        }

        /// <summary>
        /// Type of the currently active module.
        /// </summary>
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
