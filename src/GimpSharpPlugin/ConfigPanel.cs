using System;
using System.Collections.Generic;
using System.Linq;
using Gtk;
using Halftone;

namespace Gimp.HalftoneLab
{
    public class ConfigPanel<ModuleType> : Table where ModuleType : Module
    {
        private ConfigManager configManager;
        private ComboBox configNameComboBox;
        private ListStore configNameListStore;
        private Button saveCurrentConfigButton;
        private Button deleteSelectedConfigButton;

        public ModuleType CurrentModule { get; set; }

        public event EventHandler ModuleChanged;

        List<string> configNames;

        public ConfigPanel(ConfigManager manager)
            : base(1, 3, false)
        {
            if (manager == null) {
                throw new ArgumentNullException();
            }
            configManager = manager;

            configNames = (from module in
                configManager.findAllModules<ModuleType>((module) => true)
                select module.Name).ToList();
            configNameListStore = new ListStore(typeof(string));
            refreshConfigNameListStore();
            configNameComboBox = new ComboBox(configNameListStore);
            CellRendererText renderer = new CellRendererText();
            configNameComboBox.PackStart(renderer, true);
            configNameComboBox.SetAttributes(renderer, "text", 0);

            configNameComboBox.Changed += delegate
            {
                selectConfig();
            };

            saveCurrentConfigButton = new Button("gtk-save");
            saveCurrentConfigButton.Clicked += delegate
            {
                saveCurrentConfig();
            };

            deleteSelectedConfigButton = new Button("gtk-delete");
            deleteSelectedConfigButton.Clicked += delegate
            {
                deleteSelectedConfig();
            };

            Attach(configNameComboBox, 0, 1, 0, 1, AttachOptions.Fill |
                AttachOptions.Expand, AttachOptions.Shrink, 0, 0);
            Attach(saveCurrentConfigButton, 1, 2, 0, 1, AttachOptions.Fill,
                AttachOptions.Shrink, 0, 0);
            Attach(deleteSelectedConfigButton, 3, 4, 0, 1, AttachOptions.Fill,
                AttachOptions.Shrink, 0, 0);

            ShowAll();
        }

        private void saveCurrentConfig() {
            string oldName = configNameComboBox.ActiveText;
            // open an edit dialog to fill config name and description
            if (CurrentModule != null) {
                ConfigEditDialog dialog = new ConfigEditDialog(
                    CurrentModule.Name, CurrentModule.Description);
                dialog.Response += new ResponseHandler(
                    (object obj, ResponseArgs respArgs) =>
                {
                    if (respArgs.ResponseId == ResponseType.Ok) {
                        CurrentModule.Name = dialog.NameText;
                        CurrentModule.Description = dialog.DescriptionText;
                        // TODO: ask whether to replace a module if such a name
                        // already exists in the config manager
                        configManager.saveModule(CurrentModule);
                        // add the new config name the combo box
                        // or replace existing
                        if (!configNames.Contains(CurrentModule.Name)) {
                            configNames.Add(CurrentModule.Name);
                            refreshConfigNameListStore();
                        }
                        // select it
                        configNameComboBox.Active =
                                configNames.IndexOf(CurrentModule.Name);
                        if (ModuleChanged != null) {
                            ModuleChanged(this, new EventArgs());
                        }
                    }
                }
              );
              dialog.Run();
              dialog.Destroy();
            }
        }

        private void deleteSelectedConfig() {
            string selectedName = configNameComboBox.ActiveText;
            if (selectedName != null) {
                int selectedIndex = configNameComboBox.Active;
                configManager.deleteModule<ModuleType>(selectedName);
                configNames.Remove(CurrentModule.Name);
                refreshConfigNameListStore();
            }
        }

        private void selectConfig() {
            ModuleType selectedConfig = getSelectedConfig();
            if (selectedConfig != null) {
                CurrentModule = selectedConfig;
                if (ModuleChanged != null) {
                    ModuleChanged(this, new EventArgs());
                }
            }
        }

        private ModuleType getSelectedConfig() {
            string selectedName = configNameComboBox.ActiveText;
            return configManager.getModule<ModuleType>(selectedName);
        }

        private void refreshConfigNameListStore() {
            configNames.Sort();
            configNameListStore.Clear();
            foreach (string name in configNames) {
                configNameListStore.AppendValues(name);
            }
        }

        class ConfigEditDialog : Dialog {
            public string NameText { get; set; }
            public string DescriptionText { get; set; }

            private Entry nameEntry;
            private TextView descTextView;
            private TextBuffer descTextBuffer;
            private ScrolledWindow descScroll;
            private Table table;

            public ConfigEditDialog(string name, string description)
            {
                Title = "Configuration details";
                Modal = true;
                AddButton("OK", ResponseType.Ok);
                AddButton("Cancel", ResponseType.Cancel);

                NameText = name;
                DescriptionText = description;
                nameEntry = new Entry(NameText);
                descTextBuffer = new TextBuffer(new TextTagTable());
                descTextBuffer.Text = DescriptionText;
                descTextView = new TextView(descTextBuffer);

                descScroll = new ScrolledWindow();
                descScroll.Add(descTextView);

                table = new Table(4, 1, false)
                    { ColumnSpacing = 5, RowSpacing = 5, BorderWidth = 5 };
                table.Attach(new Label("Configuration name:") { Xalign = 0.0f },
                    0, 1, 0, 1, AttachOptions.Fill, AttachOptions.Shrink, 0, 0);
                table.Attach(nameEntry,
                    0, 1, 1, 2, AttachOptions.Fill, AttachOptions.Shrink, 0, 0);
                table.Attach(new Label("Description:") { Xalign = 0.0f },
                    0, 1, 2, 3, AttachOptions.Fill, AttachOptions.Shrink, 0, 0);
                table.Attach(descScroll,
                    0, 1, 3, 4, AttachOptions.Fill | AttachOptions.Expand,
                    AttachOptions.Fill | AttachOptions.Expand, 0, 0);
                table.ShowAll();
                VBox.Add(table);
            }

            protected override void OnResponse(ResponseType response_id) {
                NameText = nameEntry.Text;
                DescriptionText = descTextBuffer.Text;
            }
        }
    }
}
