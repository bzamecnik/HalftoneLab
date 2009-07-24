using System;
using Gtk;
using HalftoneLab;

namespace HalftoneLab.GUI.Gtk
{
    public class DynamicMatrixThresholdFilterDialog : ModuleDialog
    {
        private DynamicMatrixThresholdFilter module;
        private Table table;
        private TreeView recordTreeView;
        private ListStore recordStore;
        private Button addRecordButton;
        private Button editRecordButton;
        private Button deleteRecordButton;
        private Button clearRecordsButton;
        private CheckButton noiseEnabledCheckButton;

        public DynamicMatrixThresholdFilterDialog()
            : this(new DynamicMatrixThresholdFilter()) { }

        public DynamicMatrixThresholdFilterDialog(
            DynamicMatrixThresholdFilter existingModule)
            : base(existingModule) {
            module = modifiedModule as DynamicMatrixThresholdFilter;
            if (module == null) {
                modifiedModule = new DynamicMatrixThresholdFilter();
                module = modifiedModule as DynamicMatrixThresholdFilter;
            }

            initRecordTable(module);

            noiseEnabledCheckButton = new CheckButton("Noise enabled?");
            noiseEnabledCheckButton.Active = module.NoiseEnabled;
            noiseEnabledCheckButton.Toggled += delegate
            {
                module.NoiseEnabled = noiseEnabledCheckButton.Active;
            };

            addRecordButton = new Button("gtk-new");
            addRecordButton.Clicked += delegate
            {
                ThresholdTableRecordDialog dialog = new ThresholdTableRecordDialog();
                DynamicMatrixThresholdFilter.ThresholdRecord record =
                    dialog.runConfiguration();
                addRecord(record);
            };

            editRecordButton = new Button("gtk-edit");
            editRecordButton.Clicked += delegate
            {
                TreeIter selectedIter = getSelectedRowIter();
                int selectedIntensity = getIntensityFromRow(selectedIter);
                if (selectedIntensity >= 0) {
                    DynamicMatrixThresholdFilter.ThresholdRecord record = module.MatrixTable.getDefinitionRecord(
                        selectedIntensity, false);
                    ThresholdTableRecordDialog dialog =
                        new ThresholdTableRecordDialog(record);
                    record = dialog.runConfiguration();
                    if (record != null) {
                        if (record.keyRangeStart == selectedIntensity) {
                            recordStore.SetValue(selectedIter, 0, record);
                        } else {
                            deleteRecord(ref selectedIter, selectedIntensity);
                            addRecord(record);
                        }
                    }
                }
            };

            deleteRecordButton = new Button("gtk-delete");
            deleteRecordButton.Clicked += delegate
            {
                TreeIter selectedIter = getSelectedRowIter();
                int selectedIntensity = getIntensityFromRow(selectedIter);
                if (selectedIntensity >= 0) {
                    deleteRecord(ref selectedIter, selectedIntensity);
                }
            };

            clearRecordsButton = new Button("Clear all");
            clearRecordsButton.Clicked += delegate
            {
                module.MatrixTable.clearDefinitionRecords();
                recordStore.Clear();
            };

            table = new Table(6, 2, false)
                { ColumnSpacing = 5, RowSpacing = 5, BorderWidth = 5 };

            table.Attach(new Label("Intensity range table:") { Xalign = 0.0f },
                0, 2, 0, 1, AttachOptions.Fill, AttachOptions.Shrink, 0, 0);

            table.Attach(noiseEnabledCheckButton, 0, 2, 1, 2,
                AttachOptions.Fill | AttachOptions.Expand,
                AttachOptions.Shrink, 0, 0);

            ScrolledWindow recordTreeViewScroll = new ScrolledWindow();
            recordTreeViewScroll.AddWithViewport(recordTreeView);
            table.Attach(recordTreeViewScroll, 1, 2, 2, 6,
                AttachOptions.Fill | AttachOptions.Expand,
                AttachOptions.Fill | AttachOptions.Expand, 0, 0);

            table.Attach(addRecordButton, 0, 1, 2, 3,
                AttachOptions.Fill,
                AttachOptions.Shrink, 0, 0);

            table.Attach(editRecordButton, 0, 1, 3, 4,
                AttachOptions.Fill,
                AttachOptions.Shrink, 0, 0);

            table.Attach(deleteRecordButton, 0, 1, 4, 5,
                AttachOptions.Fill,
                AttachOptions.Shrink, 0, 0);

            table.Attach(clearRecordsButton, 0, 1, 5, 6,
                AttachOptions.Fill,
                AttachOptions.Shrink, 0, 0);

            table.ShowAll();
            VBox.PackStart(table);
        }

        public void initRecordTable(DynamicMatrixThresholdFilter module) {
            // TODO: possibly add:
            // - string column for ThresholdMatrix name
            // - double column for noise amplitude
            recordStore = new ListStore(
                typeof(DynamicMatrixThresholdFilter.ThresholdRecord));
            // make the list store records sorted
            recordStore.SetSortFunc(0,
                    (TreeModel model, TreeIter iter1, TreeIter iter2) =>
                    ((DynamicMatrixThresholdFilter.ThresholdRecord)
                    recordStore.GetValue(iter1, 0)).CompareTo(
                    ((DynamicMatrixThresholdFilter.ThresholdRecord)
                    recordStore.GetValue(iter2, 0)))
                );
            recordStore.SetSortColumnId(0, SortType.Ascending);
            foreach (DynamicMatrixThresholdFilter.ThresholdRecord record in
                module.MatrixTable.listDefinitionRecords()) {
                recordStore.AppendValues(record);
            }
            recordTreeView = new TreeView(recordStore);
            TreeViewColumn intensityColumn = new TreeViewColumn();
            intensityColumn.Title = "Start intensity";
            CellRendererText intensityRenderer = new CellRendererText();
            intensityColumn.PackStart(intensityRenderer, true);
            intensityColumn.SetCellDataFunc(intensityRenderer,
                (TreeViewColumn column, CellRenderer cell,
                    TreeModel model, TreeIter iter) =>
                {
                    DynamicMatrixThresholdFilter.ThresholdRecord record =
                        (DynamicMatrixThresholdFilter.ThresholdRecord)
                            recordStore.GetValue(iter, 0);
                    (cell as CellRendererText).Text =
                        record.keyRangeStart.ToString();
                }
                );

            recordTreeView.AppendColumn(intensityColumn);
        }

        TreeIter getSelectedRowIter() {
            TreeIter iter;
            recordTreeView.Selection.GetSelected(out iter);
            return iter;
        }

        TreeIter getIterByIntensity(int intensity) {
            TreeIter iter;
            recordStore.GetIterFirst(out iter);
            if (!recordStore.IterIsValid(iter)) { return iter; }
            DynamicMatrixThresholdFilter.ThresholdRecord record = null;
            do {
                record = recordStore.GetValue(iter, 0)
                    as DynamicMatrixThresholdFilter.ThresholdRecord;
                if ((record != null) &&
                    (record.keyRangeStart == intensity)) {
                    return iter;
                }
            } while (recordStore.IterNext(ref iter));
            return iter;
        }

        int getIntensityFromRow(TreeIter iter) {
            if (recordStore.IterIsValid(iter)) {
                return ((DynamicMatrixThresholdFilter.ThresholdRecord)
                    recordStore.GetValue(iter, 0)).keyRangeStart;
            }
            return -1;
        }

        void addRecord(DynamicMatrixThresholdFilter.ThresholdRecord record) {
            if (record != null) {
                module.MatrixTable.addDefinitionRecord(record);
                // if there is another record with the same intensity
                // delete it from the list store
                TreeIter iter =
                    getIterByIntensity(record.keyRangeStart);
                if (recordStore.IterIsValid(iter)) {
                    recordStore.SetValue(iter, 0, record);
                } else {
                    recordStore.AppendValues(record);
                }
            }
        }

        void deleteRecord(ref TreeIter iter, int intensity) {
            recordStore.Remove(ref iter);
            module.MatrixTable.deleteDefinitionRecord(intensity);
        }

        public class ThresholdTableRecordDialog : Dialog
        {
            private Table table;
            private SpinButton intensitySpinButton;
            ThresholdMatrixPanel matrixPanel;
            private HScale noiseHScale;
            private DynamicMatrixThresholdFilter.ThresholdRecord record;

            public ThresholdTableRecordDialog()
                : this(null) { }

            public ThresholdTableRecordDialog(DynamicMatrixThresholdFilter.ThresholdRecord
                editedRecord) {
                Title = "Threshold record editing";
                Modal = true;
                AddButton("OK", ResponseType.Ok);
                AddButton("Cancel", ResponseType.Cancel);

                record = editedRecord;
                if (record == null) {
                    record = new DynamicMatrixThresholdFilter.ThresholdRecord();
                }

                intensitySpinButton = new SpinButton(0, 255, 1);
                intensitySpinButton.Value = record.keyRangeStart;

                matrixPanel = new ThresholdMatrixPanel((uint)record.matrix.Height,
                (uint)record.matrix.Width);
                matrixPanel.Matrix = record.matrix.DefinitionMatrix;
                matrixPanel.Scaled = !record.matrix.Iterative;

                noiseHScale = new HScale(0, 1, 0.01);
                noiseHScale.Value = record.noiseAmplitude;

                table = new Table(4, 2, false)
                    { ColumnSpacing = 5, RowSpacing = 5, BorderWidth = 5 };

                table.Attach(new Label("Intensity range start:")
                    { Xalign = 0.0f }, 0, 1, 0, 1, AttachOptions.Fill,
                    AttachOptions.Shrink, 0, 0);
                table.Attach(intensitySpinButton, 1, 2, 0, 1,
                    AttachOptions.Fill, AttachOptions.Shrink, 0, 0);

                table.Attach(new Label("Threshold matrix:") { Xalign = 0.0f },
                    0, 1, 1, 2, AttachOptions.Fill, AttachOptions.Shrink, 0, 0);

                table.Attach(matrixPanel, 0, 2, 2, 3,
                    AttachOptions.Fill | AttachOptions.Expand,
                    AttachOptions.Fill | AttachOptions.Expand, 0, 0);

                table.Attach(new Label("Noise amplitude:") { Xalign = 0.0f },
                    0, 1, 3, 4, AttachOptions.Fill, AttachOptions.Shrink, 0, 0);
                table.Attach(noiseHScale, 1, 2, 3, 4,
                    AttachOptions.Fill, AttachOptions.Shrink, 0, 0);

                table.ShowAll();
                VBox.PackStart(table);
            }

            void save() {
                record.keyRangeStart = intensitySpinButton.ValueAsInt;
                record.matrix = new ThresholdMatrix(matrixPanel.Matrix,
                        !matrixPanel.Scaled);
                record.noiseAmplitude = noiseHScale.Value;
            }

            public DynamicMatrixThresholdFilter.ThresholdRecord runConfiguration() {
                DynamicMatrixThresholdFilter.ThresholdRecord
                    configuredRecord = null;
                Response += new ResponseHandler(
                    (object obj, ResponseArgs respArgs) =>
                    {
                        configuredRecord = null;
                        if (respArgs.ResponseId == ResponseType.Ok) {
                            save();
                            configuredRecord = record;
                        }
                    }
                  );
                Run();
                Destroy();
                return configuredRecord;
            }
        }
    }
}
