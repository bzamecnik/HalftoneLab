﻿// Copyright (c) 2009 Bohumir Zamecnik <bohumir@zamecnik.org>
// License: The MIT License, see the LICENSE file

using System;
using Gtk;
using HalftoneLab;

namespace HalftoneLab.GUI.Gtk
{
    /// <summary>
    /// Dynamic matrix error filter configuration dialog.
    /// </summary>
    /// <see cref="HalftoneLab.DynamicMatrixErrorFilter"/>
    public class DynamicMatrixErrorFilterDialog : ModuleDialog
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
                DynamicMatrixErrorFilter.ErrorRecord record =
                    dialog.runConfiguration();
                addRecord(record);
            };

            editRecordButton = new Button("gtk-edit");
            editRecordButton.Clicked += delegate
            {
                TreeIter selectedIter = getSelectedRowIter();
                int selectedIntensity = getIntensityFromRow(selectedIter);
                if (selectedIntensity >= 0) {
                    DynamicMatrixErrorFilter.ErrorRecord record = module.MatrixTable.getDefinitionRecord(
                        selectedIntensity, false);
                    ErrorTableRecordDialog dialog =
                        new ErrorTableRecordDialog(record);
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

        private void initRecordTable(DynamicMatrixErrorFilter module) {
            // TODO: possibly add a string column for Matrix name
            recordStore = new ListStore(
                typeof(DynamicMatrixErrorFilter.ErrorRecord));
            // make the list store records sorted
            recordStore.SetSortFunc(0,
                    (TreeModel model, TreeIter iter1, TreeIter iter2) =>
                    ((DynamicMatrixErrorFilter.ErrorRecord)
                    recordStore.GetValue(iter1, 0)).CompareTo(
                    ((DynamicMatrixErrorFilter.ErrorRecord)
                    recordStore.GetValue(iter2, 0)))
                );
            recordStore.SetSortColumnId(0, SortType.Ascending);
            foreach (DynamicMatrixErrorFilter.ErrorRecord record in
                module.MatrixTable.listDefinitionRecords())
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
                        DynamicMatrixErrorFilter.ErrorRecord record =
                            (DynamicMatrixErrorFilter.ErrorRecord)
                                recordStore.GetValue(iter, 0);
                        (cell as CellRendererText).Text =
                            record.keyRangeStart.ToString();
                    }
                );

            recordTreeView.AppendColumn(intensityColumn);
        }

        private TreeIter getSelectedRowIter() {
            TreeIter iter;
            recordTreeView.Selection.GetSelected(out iter);
            return iter;
        }

        private TreeIter getIterByIntensity(int intensity) {
            TreeIter iter;
            recordStore.GetIterFirst(out iter);
            if (!recordStore.IterIsValid(iter)) { return iter; }
            DynamicMatrixErrorFilter.ErrorRecord record = null;
            do {
                record = recordStore.GetValue(iter, 0)
                    as DynamicMatrixErrorFilter.ErrorRecord;
                if ((record != null) &&
                    (record.keyRangeStart == intensity))
                {
                    return iter;
                }
            } while (recordStore.IterNext(ref iter));
            return iter;
        }

        private int getIntensityFromRow(TreeIter iter) {
            if (recordStore.IterIsValid(iter)) {
                return ((DynamicMatrixErrorFilter.ErrorRecord)
                    recordStore.GetValue(iter, 0)).keyRangeStart;
            }
            return -1;
        }

        private void addRecord(DynamicMatrixErrorFilter.ErrorRecord record) {
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

        private void deleteRecord(ref TreeIter iter, int intensity) {
            recordStore.Remove(ref iter);
            module.MatrixTable.deleteDefinitionRecord(intensity);
        }

        public class ErrorTableRecordDialog : Dialog {
            private Table table;
            private SpinButton intensitySpinButton;
            private Button errorMatrixEditButton;
            ErrorMatrixPanel matrixPanel;
            private DynamicMatrixErrorFilter.ErrorRecord record;

            public ErrorTableRecordDialog()
                : this(null) { }

            public ErrorTableRecordDialog(DynamicMatrixErrorFilter.ErrorRecord
                editedRecord)
            {
                Title = "Intensity record editing";
                Modal = true;
                AddButton("OK", ResponseType.Ok);
                AddButton("Cancel", ResponseType.Cancel);

                record = editedRecord;
                if (record == null) {
                    record = new DynamicMatrixErrorFilter.ErrorRecord();
                }

                intensitySpinButton = new SpinButton(0, 255, 1);
                intensitySpinButton.Value = record.keyRangeStart;

                matrixPanel = new ErrorMatrixPanel((uint)record.matrix.Height,
                (uint)record.matrix.Width);
                matrixPanel.Matrix = record.matrix;


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

            // TODO: use OnResponse() instead

            void save() {
                record.keyRangeStart = intensitySpinButton.ValueAsInt;
                record.matrix = matrixPanel.Matrix;
            }

            public DynamicMatrixErrorFilter.ErrorRecord runConfiguration()
            {
                DynamicMatrixErrorFilter.ErrorRecord
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
