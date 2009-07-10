using System;
using Gtk;
using Halftone;

namespace Gimp.HalftoneLab
{
    // brainstorming:
    // - list of intensity range records
    //   - intensity range record
    //     - start intensity
    //     - error matrix
    //   - impl: treeview + liststore
    // - operations:
    //   - add new record
    //     -> popup dialog, append to liststore, add to module
    //   - delete selected record
    //     -> delete from liststore, delete from module
    //   - edit existing record
    //     - get selected, popup dialog, store
    //       - the same as add op., except for getting the original one

    public class DynamicMatrixErrorFilterDialog : ConfigDialog
    {
        private DynamicMatrixErrorFilter module;
        private Table table;
        private TreeView recordTreeView;
        private ListStore recordStore;
        private Button addRecordButton;
        private Button editRecordButton;
        private Button deleteRecordButton;
        private Button clearRecordsButton;

        public DynamicMatrixErrorFilterDialog()
            : this(new DynamicMatrixErrorFilter()) { }

        public DynamicMatrixErrorFilterDialog(
            DynamicMatrixErrorFilter existingModule)
            : base(existingModule)
        {
            module = modifiedModule as DynamicMatrixErrorFilter;
            if (module == null) {
                modifiedModule = new DynamicMatrixErrorFilter();
                module = modifiedModule as DynamicMatrixErrorFilter;
            }

            initRecordTable(module);

            addRecordButton = new Button("gtk-new");
            addRecordButton.Clicked += delegate
            {
                ErrorTableRecordDialog dialog = new ErrorTableRecordDialog();
                DynamicMatrixErrorFilter.ErrorTableRecord record =
                    dialog.runConfiguration();
                addRecord(record);
            };

            editRecordButton = new Button("gtk-edit");
            editRecordButton.Clicked += delegate
            {
                TreeIter selectedIter = getSelectedRowIter();
                int selectedIntensity = getIntensityFromRow(selectedIter);
                if (selectedIntensity >= 0) {
                    DynamicMatrixErrorFilter.ErrorTableRecord record =
                        module.getRecord(selectedIntensity, false);
                    ErrorTableRecordDialog dialog =
                        new ErrorTableRecordDialog(record);
                    record = dialog.runConfiguration();
                    if (record != null) {
                        if (record.intensityRangeStart == selectedIntensity) {
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
                module.clearRecords();
                recordStore.Clear();
            };

            table = new Table(5, 2, false)
                { ColumnSpacing = 5, RowSpacing = 5, BorderWidth = 5 };
            
            table.Attach(new Label("Intensity range table:")
                { Xalign = 0.0f }, 0, 2, 0, 1, AttachOptions.Fill,
                AttachOptions.Shrink, 0, 0);

            ScrolledWindow recordTreeViewScroll = new ScrolledWindow();
            recordTreeViewScroll.AddWithViewport(recordTreeView);
            table.Attach(recordTreeViewScroll, 1, 2, 1, 5,
                AttachOptions.Fill | AttachOptions.Expand,
                AttachOptions.Fill | AttachOptions.Expand, 0, 0);

            table.Attach(addRecordButton, 0, 1, 1, 2,
                AttachOptions.Fill,
                AttachOptions.Shrink, 0, 0);

            table.Attach(editRecordButton, 0, 1, 2, 3,
                AttachOptions.Fill,
                AttachOptions.Shrink, 0, 0);

            table.Attach(deleteRecordButton, 0, 1, 3, 4,
                AttachOptions.Fill,
                AttachOptions.Shrink, 0, 0);

            table.Attach(clearRecordsButton, 0, 1, 4, 5,
                AttachOptions.Fill,
                AttachOptions.Shrink, 0, 0);

            table.ShowAll();
            VBox.PackStart(table);
        }

        public void initRecordTable(DynamicMatrixErrorFilter module) {
            // TODO: possibly add a string column for ErrorMatrix name
            recordStore = new ListStore(
                typeof(DynamicMatrixErrorFilter.ErrorTableRecord));
            // make the list store records sorted
            recordStore.SetSortFunc(0,
                    (TreeModel model, TreeIter iter1, TreeIter iter2) =>
                    ((DynamicMatrixErrorFilter.ErrorTableRecord)
                    recordStore.GetValue(iter1, 0)).CompareTo(
                    ((DynamicMatrixErrorFilter.ErrorTableRecord)
                    recordStore.GetValue(iter2, 0)))
                );
            recordStore.SetSortColumnId(0, SortType.Ascending);
            foreach (DynamicMatrixErrorFilter.ErrorTableRecord record in
                module.listRecords())
            {
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
                        DynamicMatrixErrorFilter.ErrorTableRecord record =
                            (DynamicMatrixErrorFilter.ErrorTableRecord)
                                recordStore.GetValue(iter, 0);
                        (cell as CellRendererText).Text =
                            record.intensityRangeStart.ToString();
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
            DynamicMatrixErrorFilter.ErrorTableRecord record = null;
            do {
                record = recordStore.GetValue(iter, 0)
                    as DynamicMatrixErrorFilter.ErrorTableRecord;
                if ((record != null) &&
                    (record.intensityRangeStart == intensity))
                {
                    return iter;
                }
            } while (recordStore.IterNext(ref iter));
            return iter;
        }

        int getIntensityFromRow(TreeIter iter) {
            if (recordStore.IterIsValid(iter)) {
                return ((DynamicMatrixErrorFilter.ErrorTableRecord)
                    recordStore.GetValue(iter, 0)).intensityRangeStart;
            }
            return -1;
        }

        void addRecord(DynamicMatrixErrorFilter.ErrorTableRecord record) {
            if (record != null) {
                module.addRecord(record.intensityRangeStart,
                    record.matrix);
                // if there is another record with the same intensity
                // delete it from the list store
                TreeIter iter =
                    getIterByIntensity(record.intensityRangeStart);
                if (recordStore.IterIsValid(iter)) {
                    recordStore.SetValue(iter, 0, record);
                } else {
                    recordStore.AppendValues(record);
                }
            }
        }

        void deleteRecord(ref TreeIter iter, int intensity) {
            recordStore.Remove(ref iter);
            module.deleteRecord(intensity);
        }

        public class ErrorTableRecordDialog : Dialog {
            private Table table;
            private SpinButton intensitySpinButton;
            private Button errorMatrixEditButton;
            ErrorMatrixPanel matrixPanel;
            private DynamicMatrixErrorFilter.ErrorTableRecord record;

            public ErrorTableRecordDialog()
                : this(null) { }

            public ErrorTableRecordDialog(
                DynamicMatrixErrorFilter.ErrorTableRecord editedRecord)
            {
                Title = "Intensity record editing";
                Modal = true;
                AddButton("OK", ResponseType.Ok);
                AddButton("Cancel", ResponseType.Cancel);

                record = editedRecord;
                if (record == null) {
                    record = new DynamicMatrixErrorFilter.ErrorTableRecord();
                }

                intensitySpinButton = new SpinButton(0, 255, 1);
                intensitySpinButton.Value = record.intensityRangeStart;

                matrixPanel = new ErrorMatrixPanel((uint)record.matrix.Height,
                (uint)record.matrix.Width);
                matrixPanel.Matrix = record.matrix.DefinitionMatrix;
                matrixPanel.Divisor = record.matrix.Divisor;
                matrixPanel.SourceOffsetX = record.matrix.SourceOffsetX;

                table = new Table(3, 2, false)
                    { ColumnSpacing = 5, RowSpacing = 5, BorderWidth = 5 };

                table.Attach(new Label("Intensity range start:")
                    { Xalign = 0.0f }, 0, 1, 0, 1, AttachOptions.Fill,
                    AttachOptions.Shrink, 0, 0);

                table.Attach(intensitySpinButton, 1, 2, 0, 1,
                    AttachOptions.Fill, AttachOptions.Shrink, 0, 0);

                table.Attach(new Label("Error matrix:") { Xalign = 0.0f },
                    0, 1, 1, 2, AttachOptions.Fill,
                    AttachOptions.Shrink, 0, 0);

                table.Attach(matrixPanel, 0, 2, 2, 3,
                    AttachOptions.Fill | AttachOptions.Expand,
                    AttachOptions.Shrink, 0, 0);

                table.ShowAll();
                VBox.PackStart(table);
            }

            void save() {
                record.intensityRangeStart = intensitySpinButton.ValueAsInt;
                record.matrix = new ErrorMatrix(matrixPanel.Matrix,
                        matrixPanel.SourceOffsetX, matrixPanel.Divisor);
            }

            public DynamicMatrixErrorFilter.ErrorTableRecord runConfiguration()
            {
                DynamicMatrixErrorFilter.ErrorTableRecord configuredRecord =
                    null;
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
