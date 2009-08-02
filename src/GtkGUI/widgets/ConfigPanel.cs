// Copyright (c) 2009 Bohumir Zamecnik <bohumir@zamecnik.org>
// License: The MIT License, see the LICENSE file

using System;
using System.Collections.Generic;
using System.Linq;
using Gtk;
using HalftoneLab;

namespace HalftoneLab.GUI.Gtk
{
    /// <summary>
    /// A panel for managing configurations of a particular type.
    /// </summary>
    /// <typeparam name="ModuleType">Type of the module to be configured
    /// </typeparam>
    public class ConfigPanel<ModuleType> : Table
        where ModuleType : Module, new()
    {
        private ConfigManager configManager;
        private ComboBox configNameComboBox;
        private ListStore configNameListStore;
        private Button saveCurrentConfigButton;
        private Button deleteSelectedConfigButton;
        private TextView configDescTextView;
        private ScrolledWindow configDescScroll;

        /// <summary>
        /// Currently selected and configured module.
        /// </summary>
        public ModuleType CurrentModule { get; set; }

        /// <summary>
        /// %Module configured by this panel has changed. You can retrieve it
        /// via the CurrentModule property.
        /// </summary>
        public event EventHandler ModuleChanged;

        private List<string> configNames;
        private static string defaultModuleName = "_DEFAULT";

        /// <summary>
        /// Create a new config panel backed by an existing config manager.
        /// </summary>
        /// <param name="manager">Configuration manager</param>
        public ConfigPanel(ConfigManager manager)
            : base(2, 4, false)
        {
            if (manager == null) {
                throw new ArgumentNullException();
            }
            configManager = manager;

            ColumnSpacing = 3;
            BorderWidth = 3;

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
                if (CurrentModule != null) {
                    configDescTextView.Buffer.Text = CurrentModule.Description;
                }
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

            configDescTextView = new TextView()
            {
                WrapMode = WrapMode.Word,
                Sensitive = false
            };
            configDescScroll = new ScrolledWindow() { HeightRequest = 30 };
            configDescScroll.Add(configDescTextView);

            Attach(new Label("Config:") { Xalign = 0.0f }, 0, 1, 0, 1,
                AttachOptions.Fill, AttachOptions.Shrink, 0, 0);
            Attach(configNameComboBox, 1, 2, 0, 1, AttachOptions.Fill |
                AttachOptions.Expand, AttachOptions.Shrink, 0, 0);
            Attach(saveCurrentConfigButton, 2, 3, 0, 1, AttachOptions.Fill,
                AttachOptions.Shrink, 0, 0);
            Attach(deleteSelectedConfigButton, 3, 4, 0, 1, AttachOptions.Fill,
                AttachOptions.Shrink, 0, 0);
            Attach(configDescScroll, 0, 4, 1, 2, AttachOptions.Fill |
                AttachOptions.Expand, AttachOptions.Fill | AttachOptions.Expand,
                0, 0);

            ShowAll();
        }

        /// <summary>
        /// Save the current configuration.
        /// </summary>
        /// <remarks>
        /// Use a ConfigEditDialog to fill name and description of the
        /// configuration.
        /// </remarks>
        private void saveCurrentConfig() {
            string oldName = configNameComboBox.ActiveText;
            // open an edit dialog to fill config name and description
            if (CurrentModule != null) {
                ConfigEditDialog dialog = new ConfigEditDialog(
                    CurrentModule.Name, CurrentModule.Description);
                dialog.Response += new ResponseHandler(
                    (object obj, ResponseArgs respArgs) =>
                {
                    if ((respArgs.ResponseId == ResponseType.Ok) &&
                        (dialog.NameText != defaultModuleName)) {
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
                        // +2 is for _LAST and DEFAULT
                        configNameComboBox.Active =
                                configNames.IndexOf(CurrentModule.Name) + 2;
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

        /// <summary>
        /// Delete the currently selected configuration.
        /// </summary>
        private void deleteSelectedConfig() {
            string selectedName = configNameComboBox.ActiveText;
            if (selectedName != null) {
                int selectedIndex = configNameComboBox.Active;
                configManager.deleteModule<ModuleType>(selectedName);
                configNames.Remove(CurrentModule.Name);
                refreshConfigNameListStore();
            }
        }

        /// <summary>
        /// Set the module from the configuration currently selected in the
        /// list.
        /// </summary>
        private void selectConfig() {
            ModuleType selectedConfig = getSelectedConfig();
            if (selectedConfig != null) {
                CurrentModule = selectedConfig;
                if (ModuleChanged != null) {
                    ModuleChanged(this, new EventArgs());
                }
            }
        }

        /// <summary>
        /// Get the currently selected configuration.
        /// </summary>
        /// <returns></returns>
        private ModuleType getSelectedConfig() {
            string selectedName = configNameComboBox.ActiveText;
            return (selectedName != defaultModuleName) ?
                configManager.getModule<ModuleType>(selectedName) :
                new ModuleType();

        }

        /// <summary>
        /// Refresh the list of configurations.
        /// </summary>
        private void refreshConfigNameListStore() {
            configNames.Sort();
            configNameListStore.Clear();
            configNameListStore.AppendValues(defaultModuleName);
            foreach (string name in configNames) {
                configNameListStore.AppendValues(name);
            }
        }

        /// <summary>
        /// Dialog for editing configuration details (such as name or
        /// description).
        /// </summary>
        class ConfigEditDialog : Dialog {
            /// <summary>
            /// Name of the configuration.
            /// </summary>
            public string NameText { get; set; }
            /// Description of the configuration.
            public string DescriptionText { get; set; }

            private Entry nameEntry;
            private TextView descTextView;
            private TextBuffer descTextBuffer;
            private ScrolledWindow descScroll;
            private Table table;

            /// <summary>
            /// Create a new configuration details dialog.
            /// </summary>
            /// <param name="name">Existing configuration name</param>
            /// <param name="description">Existing configuration description
            /// </param>
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
