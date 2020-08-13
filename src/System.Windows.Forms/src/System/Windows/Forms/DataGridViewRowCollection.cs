// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable disable

using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design.Serialization;
using System.Diagnostics;
using System.Drawing;

namespace System.Windows.Forms
{
    /// <summary>
    ///  Represents a collection of <see cref='DataGridViewRow'/> objects in the
    ///  <see cref='DataGridView'/> control.
    /// </summary>
    [ListBindable(false)]
    [DesignerSerializer("System.Windows.Forms.Design.DataGridViewRowCollectionCodeDomSerializer, " + AssemblyRef.SystemDesign,
                        "System.ComponentModel.Design.Serialization.CodeDomSerializer, " + AssemblyRef.SystemDesign)]
    public class DataGridViewRowCollection : ICollection, IList
    {
#if DEBUG
        // set to false when the cached row heights are dirty and should not be accessed.
        private bool cachedRowHeightsAccessAllowed = true;

        // set to false when the cached row counts are dirty and should not be accessed.
        private bool cachedRowCountsAccessAllowed = true;
#endif

        private CollectionChangeEventHandler onCollectionChanged;
        private readonly RowArrayList items;
        private readonly List<DataGridViewElementStates> rowStates;
        private int rowCountsVisible, rowCountsVisibleFrozen, rowCountsVisibleSelected;
        private int rowsHeightVisible, rowsHeightVisibleFrozen;
        private readonly DataGridView dataGridView;

        /* IList interface implementation */

        int IList.Add(object value)
        {
            return Add((DataGridViewRow)value);
        }

        void IList.Clear()
        {
            Clear();
        }

        bool IList.Contains(object value)
        {
            return items.Contains(value);
        }

        int IList.IndexOf(object value)
        {
            return items.IndexOf(value);
        }

        void IList.Insert(int index, object value)
        {
            Insert(index, (DataGridViewRow)value);
        }

        void IList.Remove(object value)
        {
            Remove((DataGridViewRow)value);
        }

        void IList.RemoveAt(int index)
        {
            RemoveAt(index);
        }

        bool IList.IsFixedSize
        {
            get
            {
                return false;
            }
        }

        bool IList.IsReadOnly
        {
            get
            {
                return false;
            }
        }

        object IList.this[int index]
        {
            get
            {
                return this[index];
            }
            set
            {
                throw new NotSupportedException();
            }
        }

        /* ICollection interface implementation */

        void ICollection.CopyTo(Array array, int index)
        {
            items.CopyTo(array, index);
        }

        int ICollection.Count
        {
            get
            {
                return Count;
            }
        }

        bool ICollection.IsSynchronized
        {
            get
            {
                return false;
            }
        }

        object ICollection.SyncRoot
        {
            get
            {
                return this;
            }
        }

        /* IEnumerator interface implementation */

        IEnumerator IEnumerable.GetEnumerator()
        {
            return new UnsharingRowEnumerator(this);
        }

        public DataGridViewRowCollection(DataGridView dataGridView)
        {
            InvalidateCachedRowCounts();
            InvalidateCachedRowsHeights();
            this.dataGridView = dataGridView;
            rowStates = new List<DataGridViewElementStates>();
            items = new RowArrayList(this);
        }

        public int Count
        {
            get
            {
                return items.Count;
            }
        }

        internal bool IsCollectionChangedListenedTo
        {
            get
            {
                return (onCollectionChanged != null);
            }
        }

        protected ArrayList List
        {
            get
            {
                // All rows need to be unshared
                // Accessing List property should be avoided.
                int rowCount = Count;
                for (int rowIndex = 0; rowIndex < rowCount; rowIndex++)
                {
                    DataGridViewRow dataGridViewRow = this[rowIndex];
                }
                return items;
            }
        }

        internal ArrayList SharedList
        {
            get
            {
                return items;
            }
        }

        public DataGridViewRow SharedRow(int rowIndex)
        {
            return (DataGridViewRow)SharedList[rowIndex];
        }

        protected DataGridView DataGridView
        {
            get
            {
                return dataGridView;
            }
        }

        /// <summary>
        ///  Retrieves the DataGridViewRow with the specified index.
        /// </summary>
        public DataGridViewRow this[int index]
        {
            get
            {
                DataGridViewRow dataGridViewRow = SharedRow(index);
                if (dataGridViewRow.Index == -1)
                {
                    if (index == 0 && items.Count == 1)
                    {
                        // The only row present in the grid gets unshared.
                        // Simply update the index and return the current row without cloning it.
                        dataGridViewRow.Index = 0;
                        dataGridViewRow.State = SharedRowState(0);
                        if (DataGridView != null)
                        {
                            DataGridView.OnRowUnshared(dataGridViewRow);
                        }
                        return dataGridViewRow;
                    }

                    // unshare row
                    DataGridViewRow newDataGridViewRow = (DataGridViewRow)dataGridViewRow.Clone();
                    newDataGridViewRow.Index = index;
                    newDataGridViewRow.DataGridView = dataGridViewRow.DataGridView;
                    newDataGridViewRow.State = SharedRowState(index);
                    SharedList[index] = newDataGridViewRow;
                    int columnIndex = 0;
                    foreach (DataGridViewCell dataGridViewCell in newDataGridViewRow.Cells)
                    {
                        dataGridViewCell.DataGridView = dataGridViewRow.DataGridView;
                        dataGridViewCell.OwningRow = newDataGridViewRow;
                        dataGridViewCell.OwningColumn = DataGridView.Columns[columnIndex];
                        columnIndex++;
                    }
                    if (newDataGridViewRow.HasHeaderCell)
                    {
                        newDataGridViewRow.HeaderCell.DataGridView = dataGridViewRow.DataGridView;
                        newDataGridViewRow.HeaderCell.OwningRow = newDataGridViewRow;
                    }
                    if (DataGridView != null)
                    {
                        DataGridView.OnRowUnshared(newDataGridViewRow);
                    }
                    return newDataGridViewRow;
                }
                else
                {
                    return dataGridViewRow;
                }
            }
        }

        public event CollectionChangeEventHandler CollectionChanged
        {
            add => onCollectionChanged += value;
            remove => onCollectionChanged -= value;
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public virtual int Add()
        {
            if (DataGridView.DataSource != null)
            {
                throw new InvalidOperationException(SR.DataGridViewRowCollection_AddUnboundRow);
            }

            if (DataGridView.NoDimensionChangeAllowed)
            {
                throw new InvalidOperationException(SR.DataGridView_ForbiddenOperationInEventHandler);
            }

            return AddInternal(false /*newRow*/, null);
        }

        internal int AddInternal(bool newRow, object[] values)
        {
            Debug.Assert(DataGridView != null);

            if (DataGridView.Columns.Count == 0)
            {
                throw new InvalidOperationException(SR.DataGridViewRowCollection_NoColumns);
            }

            if (DataGridView.RowTemplate.Cells.Count > DataGridView.Columns.Count)
            {
                throw new InvalidOperationException(SR.DataGridViewRowCollection_RowTemplateTooManyCells);
            }

            DataGridViewRow dataGridViewRow = DataGridView.RowTemplateClone;
            Debug.Assert(dataGridViewRow.Cells.Count == DataGridView.Columns.Count);
            if (newRow)
            {
                Debug.Assert(values is null);
                // Note that we allow the 'new' row to be frozen.
                Debug.Assert((dataGridViewRow.State & (DataGridViewElementStates.Selected | DataGridViewElementStates.Displayed)) == 0);
                // Make sure the 'new row' is visible even when the row template isn't
                dataGridViewRow.State = dataGridViewRow.State | DataGridViewElementStates.Visible;
                foreach (DataGridViewCell dataGridViewCell in dataGridViewRow.Cells)
                {
                    dataGridViewCell.Value = dataGridViewCell.DefaultNewRowValue;
                }
            }

            if (values != null)
            {
                dataGridViewRow.SetValuesInternal(values);
            }

            if (DataGridView.NewRowIndex != -1)
            {
                Debug.Assert(DataGridView.AllowUserToAddRowsInternal);
                Debug.Assert(DataGridView.NewRowIndex == Count - 1);
                int insertionIndex = Count - 1;
                Insert(insertionIndex, dataGridViewRow);
                return insertionIndex;
            }

            DataGridViewElementStates rowState = dataGridViewRow.State;
            DataGridView.OnAddingRow(dataGridViewRow, rowState, true /*checkFrozenState*/);   // will throw an exception if the addition is illegal

            dataGridViewRow.DataGridView = dataGridView;
            int columnIndex = 0;
            foreach (DataGridViewCell dataGridViewCell in dataGridViewRow.Cells)
            {
                dataGridViewCell.DataGridView = dataGridView;
                Debug.Assert(dataGridViewCell.OwningRow == dataGridViewRow);
                dataGridViewCell.OwningColumn = DataGridView.Columns[columnIndex];
                columnIndex++;
            }

            if (dataGridViewRow.HasHeaderCell)
            {
                dataGridViewRow.HeaderCell.DataGridView = DataGridView;
                dataGridViewRow.HeaderCell.OwningRow = dataGridViewRow;
            }

            int index = SharedList.Add(dataGridViewRow);
            Debug.Assert((rowState & (DataGridViewElementStates.Selected | DataGridViewElementStates.Displayed)) == 0);
            rowStates.Add(rowState);
#if DEBUG
            DataGridView._dataStoreAccessAllowed = false;
            cachedRowHeightsAccessAllowed = false;
            cachedRowCountsAccessAllowed = false;
#endif
            if (values != null || !RowIsSharable(index) || RowHasValueOrToolTipText(dataGridViewRow) || IsCollectionChangedListenedTo)
            {
                dataGridViewRow.Index = index;
                Debug.Assert(dataGridViewRow.State == SharedRowState(index));
            }
            OnCollectionChanged(new CollectionChangeEventArgs(CollectionChangeAction.Add, dataGridViewRow), index, 1);
            return index;
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public virtual int Add(params object[] values)
        {
            Debug.Assert(DataGridView != null);
            if (values is null)
            {
                throw new ArgumentNullException(nameof(values));
            }

            if (DataGridView.VirtualMode)
            {
                throw new InvalidOperationException(SR.DataGridView_InvalidOperationInVirtualMode);
            }

            if (DataGridView.DataSource != null)
            {
                throw new InvalidOperationException(SR.DataGridViewRowCollection_AddUnboundRow);
            }

            if (DataGridView.NoDimensionChangeAllowed)
            {
                throw new InvalidOperationException(SR.DataGridView_ForbiddenOperationInEventHandler);
            }

            return AddInternal(false /*newRow*/, values);
        }

        /// <summary>
        ///  Adds a <see cref='DataGridViewRow'/> to this collection.
        /// </summary>
        public virtual int Add(DataGridViewRow dataGridViewRow)
        {
            if (DataGridView.Columns.Count == 0)
            {
                throw new InvalidOperationException(SR.DataGridViewRowCollection_NoColumns);
            }

            if (DataGridView.DataSource != null)
            {
                throw new InvalidOperationException(SR.DataGridViewRowCollection_AddUnboundRow);
            }

            if (DataGridView.NoDimensionChangeAllowed)
            {
                throw new InvalidOperationException(SR.DataGridView_ForbiddenOperationInEventHandler);
            }

            return AddInternal(dataGridViewRow);
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public virtual int Add(int count)
        {
            Debug.Assert(DataGridView != null);

            if (count <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(count), SR.DataGridViewRowCollection_CountOutOfRange);
            }

            if (DataGridView.Columns.Count == 0)
            {
                throw new InvalidOperationException(SR.DataGridViewRowCollection_NoColumns);
            }

            if (DataGridView.DataSource != null)
            {
                throw new InvalidOperationException(SR.DataGridViewRowCollection_AddUnboundRow);
            }

            if (DataGridView.NoDimensionChangeAllowed)
            {
                throw new InvalidOperationException(SR.DataGridView_ForbiddenOperationInEventHandler);
            }

            if (DataGridView.RowTemplate.Cells.Count > DataGridView.Columns.Count)
            {
                throw new InvalidOperationException(SR.DataGridViewRowCollection_RowTemplateTooManyCells);
            }

            DataGridViewRow rowTemplate = DataGridView.RowTemplateClone;
            Debug.Assert(rowTemplate.Cells.Count == DataGridView.Columns.Count);
            DataGridViewElementStates rowTemplateState = rowTemplate.State;
            Debug.Assert((rowTemplateState & (DataGridViewElementStates.Selected | DataGridViewElementStates.Displayed)) == 0);
            rowTemplate.DataGridView = dataGridView;
            int columnIndex = 0;
            foreach (DataGridViewCell dataGridViewCell in rowTemplate.Cells)
            {
                dataGridViewCell.DataGridView = dataGridView;
                Debug.Assert(dataGridViewCell.OwningRow == rowTemplate);
                dataGridViewCell.OwningColumn = DataGridView.Columns[columnIndex];
                columnIndex++;
            }
            if (rowTemplate.HasHeaderCell)
            {
                rowTemplate.HeaderCell.DataGridView = dataGridView;
                rowTemplate.HeaderCell.OwningRow = rowTemplate;
            }

            if (DataGridView.NewRowIndex != -1)
            {
                Debug.Assert(DataGridView.AllowUserToAddRowsInternal);
                Debug.Assert(DataGridView.NewRowIndex == Count - 1);
                int insertionIndex = Count - 1;
                InsertCopiesPrivate(rowTemplate, rowTemplateState, insertionIndex, count);
                return insertionIndex + count - 1;
            }

            return AddCopiesPrivate(rowTemplate, rowTemplateState, count);
        }

        internal int AddInternal(DataGridViewRow dataGridViewRow)
        {
            Debug.Assert(DataGridView != null);

            if (dataGridViewRow is null)
            {
                throw new ArgumentNullException(nameof(dataGridViewRow));
            }
            if (dataGridViewRow.DataGridView != null)
            {
                throw new InvalidOperationException(SR.DataGridView_RowAlreadyBelongsToDataGridView);
            }
            if (DataGridView.Columns.Count == 0)
            {
                throw new InvalidOperationException(SR.DataGridViewRowCollection_NoColumns);
            }
            if (dataGridViewRow.Cells.Count > DataGridView.Columns.Count)
            {
                throw new ArgumentException(SR.DataGridViewRowCollection_TooManyCells, nameof(dataGridViewRow));
            }

            if (dataGridViewRow.Selected)
            {
                throw new InvalidOperationException(SR.DataGridViewRowCollection_CannotAddOrInsertSelectedRow);
            }

            if (DataGridView.NewRowIndex != -1)
            {
                Debug.Assert(DataGridView.AllowUserToAddRowsInternal);
                Debug.Assert(DataGridView.NewRowIndex == Count - 1);
                int insertionIndex = Count - 1;
                InsertInternal(insertionIndex, dataGridViewRow);
                return insertionIndex;
            }

            DataGridView.CompleteCellsCollection(dataGridViewRow);
            Debug.Assert(dataGridViewRow.Cells.Count == DataGridView.Columns.Count);
            DataGridView.OnAddingRow(dataGridViewRow, dataGridViewRow.State, true /*checkFrozenState*/);   // will throw an exception if the addition is illegal

            int columnIndex = 0;
            foreach (DataGridViewCell dataGridViewCell in dataGridViewRow.Cells)
            {
                dataGridViewCell.DataGridView = dataGridView;
                Debug.Assert(dataGridViewCell.OwningRow == dataGridViewRow);
                if (dataGridViewCell.ColumnIndex == -1)
                {
                    dataGridViewCell.OwningColumn = DataGridView.Columns[columnIndex];
                }
                columnIndex++;
            }

            if (dataGridViewRow.HasHeaderCell)
            {
                dataGridViewRow.HeaderCell.DataGridView = DataGridView;
                dataGridViewRow.HeaderCell.OwningRow = dataGridViewRow;
            }

            int index = SharedList.Add(dataGridViewRow);
            Debug.Assert((dataGridViewRow.State & (DataGridViewElementStates.Selected | DataGridViewElementStates.Displayed)) == 0);
            rowStates.Add(dataGridViewRow.State);
            Debug.Assert(rowStates.Count == SharedList.Count);
#if DEBUG
            DataGridView._dataStoreAccessAllowed = false;
            cachedRowHeightsAccessAllowed = false;
            cachedRowCountsAccessAllowed = false;
#endif

            dataGridViewRow.DataGridView = dataGridView;
            if (!RowIsSharable(index) || RowHasValueOrToolTipText(dataGridViewRow) || IsCollectionChangedListenedTo)
            {
                dataGridViewRow.Index = index;
                Debug.Assert(dataGridViewRow.State == SharedRowState(index));
            }
            OnCollectionChanged(new CollectionChangeEventArgs(CollectionChangeAction.Add, dataGridViewRow), index, 1);
            return index;
        }

        public virtual int AddCopy(int indexSource)
        {
            if (DataGridView.DataSource != null)
            {
                throw new InvalidOperationException(SR.DataGridViewRowCollection_AddUnboundRow);
            }

            if (DataGridView.NoDimensionChangeAllowed)
            {
                throw new InvalidOperationException(SR.DataGridView_ForbiddenOperationInEventHandler);
            }

            return AddCopyInternal(indexSource, DataGridViewElementStates.None, DataGridViewElementStates.Selected | DataGridViewElementStates.Displayed, false /*newRow*/);
        }

        internal int AddCopyInternal(int indexSource, DataGridViewElementStates dgvesAdd, DataGridViewElementStates dgvesRemove, bool newRow)
        {
            Debug.Assert(DataGridView != null);

            if (DataGridView.NewRowIndex != -1)
            {
                Debug.Assert(DataGridView.AllowUserToAddRowsInternal);
                Debug.Assert(DataGridView.NewRowIndex == Count - 1);
                Debug.Assert(!newRow);
                int insertionIndex = Count - 1;
                InsertCopy(indexSource, insertionIndex);
                return insertionIndex;
            }

            if (indexSource < 0 || indexSource >= Count)
            {
                throw new ArgumentOutOfRangeException(nameof(indexSource), SR.DataGridViewRowCollection_IndexSourceOutOfRange);
            }

            int index;
            DataGridViewRow rowTemplate = SharedRow(indexSource);
            if (rowTemplate.Index == -1 && !IsCollectionChangedListenedTo && !newRow)
            {
                Debug.Assert(DataGridView != null);
                DataGridViewElementStates rowState = rowStates[indexSource] & ~dgvesRemove;
                rowState |= dgvesAdd;
                DataGridView.OnAddingRow(rowTemplate, rowState, true /*checkFrozenState*/);   // will throw an exception if the addition is illegal

                index = SharedList.Add(rowTemplate);
                rowStates.Add(rowState);
#if DEBUG
                DataGridView._dataStoreAccessAllowed = false;
                cachedRowHeightsAccessAllowed = false;
                cachedRowCountsAccessAllowed = false;
#endif
                OnCollectionChanged(new CollectionChangeEventArgs(CollectionChangeAction.Add, rowTemplate), index, 1);
                return index;
            }
            else
            {
                index = AddDuplicateRow(rowTemplate, newRow);
                if (!RowIsSharable(index) || RowHasValueOrToolTipText(SharedRow(index)) || IsCollectionChangedListenedTo)
                {
                    UnshareRow(index);
                }
                OnCollectionChanged(new CollectionChangeEventArgs(CollectionChangeAction.Add, SharedRow(index)), index, 1);
                return index;
            }
        }

        public virtual int AddCopies(int indexSource, int count)
        {
            if (DataGridView.DataSource != null)
            {
                throw new InvalidOperationException(SR.DataGridViewRowCollection_AddUnboundRow);
            }

            if (DataGridView.NoDimensionChangeAllowed)
            {
                throw new InvalidOperationException(SR.DataGridView_ForbiddenOperationInEventHandler);
            }

            return AddCopiesInternal(indexSource, count);
        }

        internal int AddCopiesInternal(int indexSource, int count)
        {
            if (DataGridView.NewRowIndex != -1)
            {
                Debug.Assert(DataGridView.AllowUserToAddRowsInternal);
                Debug.Assert(DataGridView.NewRowIndex == Count - 1);
                int insertionIndex = Count - 1;
                InsertCopiesPrivate(indexSource, insertionIndex, count);
                return insertionIndex + count - 1;
            }

            return AddCopiesInternal(indexSource, count, DataGridViewElementStates.None, DataGridViewElementStates.Selected | DataGridViewElementStates.Displayed);
        }

        internal int AddCopiesInternal(int indexSource, int count, DataGridViewElementStates dgvesAdd, DataGridViewElementStates dgvesRemove)
        {
            if (indexSource < 0 || Count <= indexSource)
            {
                throw new ArgumentOutOfRangeException(nameof(indexSource), SR.DataGridViewRowCollection_IndexSourceOutOfRange);
            }

            if (count <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(count), SR.DataGridViewRowCollection_CountOutOfRange);
            }

            DataGridViewElementStates rowTemplateState = rowStates[indexSource] & ~dgvesRemove;
            rowTemplateState |= dgvesAdd;

            return AddCopiesPrivate(SharedRow(indexSource), rowTemplateState, count);
        }

        private int AddCopiesPrivate(DataGridViewRow rowTemplate, DataGridViewElementStates rowTemplateState, int count)
        {
            int index, indexStart = items.Count;
            if (rowTemplate.Index == -1)
            {
                DataGridView.OnAddingRow(rowTemplate, rowTemplateState, true /*checkFrozenState*/);   // Done once only, continue to check if this is OK - will throw an exception if the addition is illegal.
                for (int i = 0; i < count - 1; i++)
                {
                    SharedList.Add(rowTemplate);
                    rowStates.Add(rowTemplateState);
                }
                index = SharedList.Add(rowTemplate);
                rowStates.Add(rowTemplateState);
#if DEBUG
                DataGridView._dataStoreAccessAllowed = false;
                cachedRowHeightsAccessAllowed = false;
                cachedRowCountsAccessAllowed = false;
#endif
                DataGridView.OnAddedRow_PreNotification(index);   // Only calling this once instead of 'count' times. Continue to check if this is OK.
                OnCollectionChanged(new CollectionChangeEventArgs(CollectionChangeAction.Refresh, null), indexStart, count);
                for (int i = 0; i < count; i++)
                {
                    DataGridView.OnAddedRow_PostNotification(index - (count - 1) + i);
                }
                return index;
            }
            else
            {
                index = AddDuplicateRow(rowTemplate, false /*newRow*/);
                if (count > 1)
                {
                    DataGridView.OnAddedRow_PreNotification(index);
                    if (RowIsSharable(index))
                    {
                        DataGridViewRow rowTemplate2 = SharedRow(index);
                        DataGridView.OnAddingRow(rowTemplate2, rowTemplateState, true /*checkFrozenState*/);   // done only once, continue to check if this is OK - will throw an exception if the addition is illegal
                        for (int i = 1; i < count - 1; i++)
                        {
                            SharedList.Add(rowTemplate2);
                            rowStates.Add(rowTemplateState);
                        }
                        index = SharedList.Add(rowTemplate2);
                        rowStates.Add(rowTemplateState);
#if DEBUG
                        DataGridView._dataStoreAccessAllowed = false;
                        cachedRowHeightsAccessAllowed = false;
                        cachedRowCountsAccessAllowed = false;
#endif
                        DataGridView.OnAddedRow_PreNotification(index);   // Only calling this once instead of 'count-1' times. Continue to check if this is OK.
                    }
                    else
                    {
                        UnshareRow(index);
                        for (int i = 1; i < count; i++)
                        {
                            index = AddDuplicateRow(rowTemplate, false /*newRow*/);
                            UnshareRow(index);
                            DataGridView.OnAddedRow_PreNotification(index);
                        }
                    }
                    OnCollectionChanged(new CollectionChangeEventArgs(CollectionChangeAction.Refresh, null), indexStart, count);
                    for (int i = 0; i < count; i++)
                    {
                        DataGridView.OnAddedRow_PostNotification(index - (count - 1) + i);
                    }
                    return index;
                }
                else
                {
                    if (IsCollectionChangedListenedTo)
                    {
                        UnshareRow(index);
                    }
                    OnCollectionChanged(new CollectionChangeEventArgs(CollectionChangeAction.Add, SharedRow(index)), index, 1);
                    return index;
                }
            }
        }

        private int AddDuplicateRow(DataGridViewRow rowTemplate, bool newRow)
        {
            Debug.Assert(DataGridView != null);

            DataGridViewRow dataGridViewRow = (DataGridViewRow)rowTemplate.Clone();
            dataGridViewRow.State = DataGridViewElementStates.None;
            dataGridViewRow.DataGridView = dataGridView;
            DataGridViewCellCollection dgvcc = dataGridViewRow.Cells;
            int columnIndex = 0;
            foreach (DataGridViewCell dataGridViewCell in dgvcc)
            {
                if (newRow)
                {
                    dataGridViewCell.Value = dataGridViewCell.DefaultNewRowValue;
                }
                dataGridViewCell.DataGridView = dataGridView;
                dataGridViewCell.OwningColumn = DataGridView.Columns[columnIndex];
                columnIndex++;
            }
            DataGridViewElementStates rowState = rowTemplate.State & ~(DataGridViewElementStates.Selected | DataGridViewElementStates.Displayed);
            if (dataGridViewRow.HasHeaderCell)
            {
                dataGridViewRow.HeaderCell.DataGridView = dataGridView;
                dataGridViewRow.HeaderCell.OwningRow = dataGridViewRow;
            }

            DataGridView.OnAddingRow(dataGridViewRow, rowState, true /*checkFrozenState*/);   // will throw an exception if the addition is illegal

#if DEBUG
            DataGridView._dataStoreAccessAllowed = false;
            cachedRowHeightsAccessAllowed = false;
            cachedRowCountsAccessAllowed = false;
#endif
            Debug.Assert(dataGridViewRow.Index == -1);
            rowStates.Add(rowState);
            return SharedList.Add(dataGridViewRow);
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public virtual void AddRange(params DataGridViewRow[] dataGridViewRows)
        {
            if (dataGridViewRows is null)
            {
                throw new ArgumentNullException(nameof(dataGridViewRows));
            }

            Debug.Assert(DataGridView != null);

            if (DataGridView.NoDimensionChangeAllowed)
            {
                throw new InvalidOperationException(SR.DataGridView_ForbiddenOperationInEventHandler);
            }

            if (DataGridView.DataSource != null)
            {
                throw new InvalidOperationException(SR.DataGridViewRowCollection_AddUnboundRow);
            }

            if (DataGridView.NewRowIndex != -1)
            {
                Debug.Assert(DataGridView.AllowUserToAddRowsInternal);
                Debug.Assert(DataGridView.NewRowIndex == Count - 1);
                InsertRange(Count - 1, dataGridViewRows);
                return;
            }

            if (DataGridView.Columns.Count == 0)
            {
                throw new InvalidOperationException(SR.DataGridViewRowCollection_NoColumns);
            }

            int indexStart = items.Count;

            // OnAddingRows checks for Selected flag of each row and their dimension.
            DataGridView.OnAddingRows(dataGridViewRows, true /*checkFrozenStates*/);   // will throw an exception if the addition is illegal

            foreach (DataGridViewRow dataGridViewRow in dataGridViewRows)
            {
                Debug.Assert(dataGridViewRow.Cells.Count == DataGridView.Columns.Count);
                int columnIndex = 0;
                foreach (DataGridViewCell dataGridViewCell in dataGridViewRow.Cells)
                {
                    dataGridViewCell.DataGridView = dataGridView;
                    Debug.Assert(dataGridViewCell.OwningRow == dataGridViewRow);
                    dataGridViewCell.OwningColumn = DataGridView.Columns[columnIndex];
                    columnIndex++;
                }

                if (dataGridViewRow.HasHeaderCell)
                {
                    dataGridViewRow.HeaderCell.DataGridView = dataGridView;
                    dataGridViewRow.HeaderCell.OwningRow = dataGridViewRow;
                }

                int index = SharedList.Add(dataGridViewRow);
                Debug.Assert((dataGridViewRow.State & (DataGridViewElementStates.Selected | DataGridViewElementStates.Displayed)) == 0);
                rowStates.Add(dataGridViewRow.State);
#if DEBUG
                DataGridView._dataStoreAccessAllowed = false;
                cachedRowHeightsAccessAllowed = false;
                cachedRowCountsAccessAllowed = false;
#endif
                dataGridViewRow.Index = index;
                Debug.Assert(dataGridViewRow.State == SharedRowState(index));
                dataGridViewRow.DataGridView = dataGridView;
            }
            Debug.Assert(rowStates.Count == SharedList.Count);

            DataGridView.OnAddedRows_PreNotification(dataGridViewRows);
            OnCollectionChanged(new CollectionChangeEventArgs(CollectionChangeAction.Refresh, null), indexStart, dataGridViewRows.Length);
            DataGridView.OnAddedRows_PostNotification(dataGridViewRows);
        }

        public virtual void Clear()
        {
            if (DataGridView.NoDimensionChangeAllowed)
            {
                throw new InvalidOperationException(SR.DataGridView_ForbiddenOperationInEventHandler);
            }
            if (DataGridView.DataSource != null)
            {
                if (DataGridView.DataConnection.List is IBindingList list && list.AllowRemove && list.SupportsChangeNotification)
                {
                    ((IList)list).Clear();
                }
                else
                {
                    throw new InvalidOperationException(SR.DataGridViewRowCollection_CantClearRowCollectionWithWrongSource);
                }
            }
            else
            {
                ClearInternal(true);
            }
        }

        internal void ClearInternal(bool recreateNewRow)
        {
            int rowCount = items.Count;
            if (rowCount > 0)
            {
                DataGridView.OnClearingRows();

                for (int rowIndex = 0; rowIndex < rowCount; rowIndex++)
                {
                    SharedRow(rowIndex).DetachFromDataGridView();
                }

                SharedList.Clear();
                rowStates.Clear();
#if DEBUG
                DataGridView._dataStoreAccessAllowed = false;
                cachedRowHeightsAccessAllowed = false;
                cachedRowCountsAccessAllowed = false;
#endif
                OnCollectionChanged(new CollectionChangeEventArgs(CollectionChangeAction.Refresh, null), 0, rowCount, true, false, recreateNewRow, new Point(-1, -1));
            }
            else if (recreateNewRow &&
                     DataGridView.Columns.Count != 0 &&
                     DataGridView.AllowUserToAddRowsInternal &&
                     items.Count == 0) // accessing AllowUserToAddRowsInternal can trigger a nested call to ClearInternal. Rows count needs to be checked again.
            {
                DataGridView.AddNewRow(false);
            }
        }

        /// <summary>
        ///  Checks to see if a DataGridViewRow is contained in this collection.
        /// </summary>
        public virtual bool Contains(DataGridViewRow dataGridViewRow)
        {
            return items.IndexOf(dataGridViewRow) != -1;
        }

        public void CopyTo(DataGridViewRow[] array, int index)
        {
            items.CopyTo(array, index);
        }

        // returns the row collection index for the n'th visible row
        internal int DisplayIndexToRowIndex(int visibleRowIndex)
        {
            Debug.Assert(visibleRowIndex < GetRowCount(DataGridViewElementStates.Visible));

            // go row by row
            // the alternative would be to do a binary search using DataGridViewRowCollection::GetRowCount(...)
            // but that method also iterates thru each row so we would not gain much
            int indexOfCurrentVisibleRow = -1;
            for (int i = 0; i < Count; i++)
            {
                if ((GetRowState(i) & DataGridViewElementStates.Visible) == DataGridViewElementStates.Visible)
                {
                    indexOfCurrentVisibleRow++;
                }
                if (indexOfCurrentVisibleRow == visibleRowIndex)
                {
                    return i;
                }
            }
            Debug.Assert(false, "we should have found the row already");
            return -1;
        }

        public int GetFirstRow(DataGridViewElementStates includeFilter)
        {
            if ((includeFilter & ~(DataGridViewElementStates.Displayed | DataGridViewElementStates.Frozen | DataGridViewElementStates.Resizable |
                DataGridViewElementStates.ReadOnly | DataGridViewElementStates.Selected | DataGridViewElementStates.Visible)) != 0)
            {
                throw new ArgumentException(string.Format(SR.DataGridView_InvalidDataGridViewElementStateCombination, nameof(includeFilter)));
            }
#if DEBUG
            Debug.Assert(cachedRowCountsAccessAllowed);
#endif
            switch (includeFilter)
            {
                case DataGridViewElementStates.Visible:
                    if (rowCountsVisible == 0)
                    {
                        return -1;
                    }
                    break;
                case DataGridViewElementStates.Visible | DataGridViewElementStates.Frozen:
                    if (rowCountsVisibleFrozen == 0)
                    {
                        return -1;
                    }
                    break;
                case DataGridViewElementStates.Visible | DataGridViewElementStates.Selected:
                    if (rowCountsVisibleSelected == 0)
                    {
                        return -1;
                    }
                    break;
            }

            int index = 0;
            while (index < items.Count && !((GetRowState(index) & includeFilter) == includeFilter))
            {
                index++;
            }
            return (index < items.Count) ? index : -1;
        }

        public int GetFirstRow(DataGridViewElementStates includeFilter,
                               DataGridViewElementStates excludeFilter)
        {
            if (excludeFilter == DataGridViewElementStates.None)
            {
                return GetFirstRow(includeFilter);
            }
            if ((includeFilter & ~(DataGridViewElementStates.Displayed | DataGridViewElementStates.Frozen | DataGridViewElementStates.Resizable |
                DataGridViewElementStates.ReadOnly | DataGridViewElementStates.Selected | DataGridViewElementStates.Visible)) != 0)
            {
                throw new ArgumentException(string.Format(SR.DataGridView_InvalidDataGridViewElementStateCombination, nameof(includeFilter)));
            }
            if ((excludeFilter & ~(DataGridViewElementStates.Displayed | DataGridViewElementStates.Frozen | DataGridViewElementStates.Resizable |
                DataGridViewElementStates.ReadOnly | DataGridViewElementStates.Selected | DataGridViewElementStates.Visible)) != 0)
            {
                throw new ArgumentException(string.Format(SR.DataGridView_InvalidDataGridViewElementStateCombination, nameof(excludeFilter)));
            }
#if DEBUG
            Debug.Assert(cachedRowCountsAccessAllowed);
#endif
            switch (includeFilter)
            {
                case DataGridViewElementStates.Visible:
                    if (rowCountsVisible == 0)
                    {
                        return -1;
                    }
                    break;
                case DataGridViewElementStates.Visible | DataGridViewElementStates.Frozen:
                    if (rowCountsVisibleFrozen == 0)
                    {
                        return -1;
                    }
                    break;
                case DataGridViewElementStates.Visible | DataGridViewElementStates.Selected:
                    if (rowCountsVisibleSelected == 0)
                    {
                        return -1;
                    }
                    break;
            }

            int index = 0;
            while (index < items.Count && (!((GetRowState(index) & includeFilter) == includeFilter) || !((GetRowState(index) & excludeFilter) == 0)))
            {
                index++;
            }
            return (index < items.Count) ? index : -1;
        }

        public int GetLastRow(DataGridViewElementStates includeFilter)
        {
            if ((includeFilter & ~(DataGridViewElementStates.Displayed | DataGridViewElementStates.Frozen | DataGridViewElementStates.Resizable |
                DataGridViewElementStates.ReadOnly | DataGridViewElementStates.Selected | DataGridViewElementStates.Visible)) != 0)
            {
                throw new ArgumentException(string.Format(SR.DataGridView_InvalidDataGridViewElementStateCombination, nameof(includeFilter)));
            }
#if DEBUG
            Debug.Assert(cachedRowCountsAccessAllowed);
#endif
            switch (includeFilter)
            {
                case DataGridViewElementStates.Visible:
                    if (rowCountsVisible == 0)
                    {
                        return -1;
                    }
                    break;
                case DataGridViewElementStates.Visible | DataGridViewElementStates.Frozen:
                    if (rowCountsVisibleFrozen == 0)
                    {
                        return -1;
                    }
                    break;
                case DataGridViewElementStates.Visible | DataGridViewElementStates.Selected:
                    if (rowCountsVisibleSelected == 0)
                    {
                        return -1;
                    }
                    break;
            }

            int index = items.Count - 1;
            while (index >= 0 && !((GetRowState(index) & includeFilter) == includeFilter))
            {
                index--;
            }
            return (index >= 0) ? index : -1;
        }

        internal int GetNextRow(int indexStart, DataGridViewElementStates includeFilter, int skipRows)
        {
            Debug.Assert(skipRows >= 0);

            int rowIndex = indexStart;
            do
            {
                rowIndex = GetNextRow(rowIndex, includeFilter);
                skipRows--;
            }
            while (skipRows >= 0 && rowIndex != -1);

            return rowIndex;
        }

        public int GetNextRow(int indexStart, DataGridViewElementStates includeFilter)
        {
            if ((includeFilter & ~(DataGridViewElementStates.Displayed | DataGridViewElementStates.Frozen | DataGridViewElementStates.Resizable |
                DataGridViewElementStates.ReadOnly | DataGridViewElementStates.Selected | DataGridViewElementStates.Visible)) != 0)
            {
                throw new ArgumentException(string.Format(SR.DataGridView_InvalidDataGridViewElementStateCombination, nameof(includeFilter)));
            }
            if (indexStart < -1)
            {
                throw new ArgumentOutOfRangeException(nameof(indexStart), indexStart, string.Format(SR.InvalidLowBoundArgumentEx, nameof(indexStart), indexStart, -1));
            }

            int index = indexStart + 1;
            while (index < items.Count && !((GetRowState(index) & includeFilter) == includeFilter))
            {
                index++;
            }
            return (index < items.Count) ? index : -1;
        }

        public int GetNextRow(int indexStart,
                              DataGridViewElementStates includeFilter,
                              DataGridViewElementStates excludeFilter)
        {
            if (excludeFilter == DataGridViewElementStates.None)
            {
                return GetNextRow(indexStart, includeFilter);
            }
            if ((includeFilter & ~(DataGridViewElementStates.Displayed | DataGridViewElementStates.Frozen | DataGridViewElementStates.Resizable |
                DataGridViewElementStates.ReadOnly | DataGridViewElementStates.Selected | DataGridViewElementStates.Visible)) != 0)
            {
                throw new ArgumentException(string.Format(SR.DataGridView_InvalidDataGridViewElementStateCombination, nameof(includeFilter)));
            }
            if ((excludeFilter & ~(DataGridViewElementStates.Displayed | DataGridViewElementStates.Frozen | DataGridViewElementStates.Resizable |
                DataGridViewElementStates.ReadOnly | DataGridViewElementStates.Selected | DataGridViewElementStates.Visible)) != 0)
            {
                throw new ArgumentException(string.Format(SR.DataGridView_InvalidDataGridViewElementStateCombination, nameof(excludeFilter)));
            }
            if (indexStart < -1)
            {
                throw new ArgumentOutOfRangeException(nameof(indexStart), indexStart, string.Format(SR.InvalidLowBoundArgumentEx, nameof(indexStart), indexStart, -1));
            }

            int index = indexStart + 1;
            while (index < items.Count && (!((GetRowState(index) & includeFilter) == includeFilter) || !((GetRowState(index) & excludeFilter) == 0)))
            {
                index++;
            }
            return (index < items.Count) ? index : -1;
        }

        public int GetPreviousRow(int indexStart, DataGridViewElementStates includeFilter)
        {
            if ((includeFilter & ~(DataGridViewElementStates.Displayed | DataGridViewElementStates.Frozen | DataGridViewElementStates.Resizable |
                DataGridViewElementStates.ReadOnly | DataGridViewElementStates.Selected | DataGridViewElementStates.Visible)) != 0)
            {
                throw new ArgumentException(string.Format(SR.DataGridView_InvalidDataGridViewElementStateCombination, nameof(includeFilter)));
            }
            if (indexStart > items.Count)
            {
                throw new ArgumentOutOfRangeException(nameof(indexStart), indexStart, string.Format(SR.InvalidHighBoundArgumentEx, nameof(indexStart), indexStart, items.Count));
            }

            int index = indexStart - 1;
            while (index >= 0 && !((GetRowState(index) & includeFilter) == includeFilter))
            {
                index--;
            }
            return (index >= 0) ? index : -1;
        }

        public int GetPreviousRow(int indexStart,
                                  DataGridViewElementStates includeFilter,
                                  DataGridViewElementStates excludeFilter)
        {
            if (excludeFilter == DataGridViewElementStates.None)
            {
                return GetPreviousRow(indexStart, includeFilter);
            }
            if ((includeFilter & ~(DataGridViewElementStates.Displayed | DataGridViewElementStates.Frozen | DataGridViewElementStates.Resizable |
                DataGridViewElementStates.ReadOnly | DataGridViewElementStates.Selected | DataGridViewElementStates.Visible)) != 0)
            {
                throw new ArgumentException(string.Format(SR.DataGridView_InvalidDataGridViewElementStateCombination, nameof(includeFilter)));
            }
            if ((excludeFilter & ~(DataGridViewElementStates.Displayed | DataGridViewElementStates.Frozen | DataGridViewElementStates.Resizable |
                DataGridViewElementStates.ReadOnly | DataGridViewElementStates.Selected | DataGridViewElementStates.Visible)) != 0)
            {
                throw new ArgumentException(string.Format(SR.DataGridView_InvalidDataGridViewElementStateCombination, nameof(excludeFilter)));
            }
            if (indexStart > items.Count)
            {
                throw new ArgumentOutOfRangeException(nameof(indexStart), indexStart, string.Format(SR.InvalidHighBoundArgumentEx, nameof(indexStart), indexStart, items.Count));
            }

            int index = indexStart - 1;
            while (index >= 0 && (!((GetRowState(index) & includeFilter) == includeFilter) || !((GetRowState(index) & excludeFilter) == 0)))
            {
                index--;
            }
            return (index >= 0) ? index : -1;
        }

        public int GetRowCount(DataGridViewElementStates includeFilter)
        {
            if ((includeFilter & ~(DataGridViewElementStates.Displayed | DataGridViewElementStates.Frozen | DataGridViewElementStates.Resizable |
                DataGridViewElementStates.ReadOnly | DataGridViewElementStates.Selected | DataGridViewElementStates.Visible)) != 0)
            {
                throw new ArgumentException(string.Format(SR.DataGridView_InvalidDataGridViewElementStateCombination, nameof(includeFilter)));
            }
#if DEBUG
            Debug.Assert(cachedRowCountsAccessAllowed);
#endif
            // cache returned value and reuse it as long as none
            // of the row's state has changed.
            switch (includeFilter)
            {
                case DataGridViewElementStates.Visible:
                    if (rowCountsVisible != -1)
                    {
                        return rowCountsVisible;
                    }
                    break;
                case DataGridViewElementStates.Visible | DataGridViewElementStates.Frozen:
                    if (rowCountsVisibleFrozen != -1)
                    {
                        return rowCountsVisibleFrozen;
                    }
                    break;
                case DataGridViewElementStates.Visible | DataGridViewElementStates.Selected:
                    if (rowCountsVisibleSelected != -1)
                    {
                        return rowCountsVisibleSelected;
                    }
                    break;
            }

            int rowCount = 0;
            for (int rowIndex = 0; rowIndex < items.Count; rowIndex++)
            {
                if ((GetRowState(rowIndex) & includeFilter) == includeFilter)
                {
                    rowCount++;
                }
            }

            switch (includeFilter)
            {
                case DataGridViewElementStates.Visible:
                    rowCountsVisible = rowCount;
                    break;
                case DataGridViewElementStates.Visible | DataGridViewElementStates.Frozen:
                    rowCountsVisibleFrozen = rowCount;
                    break;
                case DataGridViewElementStates.Visible | DataGridViewElementStates.Selected:
                    rowCountsVisibleSelected = rowCount;
                    break;
            }
            return rowCount;
        }

        internal int GetRowCount(DataGridViewElementStates includeFilter, int fromRowIndex, int toRowIndex)
        {
            Debug.Assert(toRowIndex >= fromRowIndex);
            Debug.Assert((GetRowState(toRowIndex) & includeFilter) == includeFilter);

            int jumpRows = 0;
            for (int rowIndex = fromRowIndex + 1; rowIndex <= toRowIndex; rowIndex++)
            {
                if ((GetRowState(rowIndex) & includeFilter) == includeFilter)
                {
                    jumpRows++;
                }
            }

            return jumpRows;
        }

        public int GetRowsHeight(DataGridViewElementStates includeFilter)
        {
            if ((includeFilter & ~(DataGridViewElementStates.Displayed | DataGridViewElementStates.Frozen | DataGridViewElementStates.Resizable |
                DataGridViewElementStates.ReadOnly | DataGridViewElementStates.Selected | DataGridViewElementStates.Visible)) != 0)
            {
                throw new ArgumentException(string.Format(SR.DataGridView_InvalidDataGridViewElementStateCombination, nameof(includeFilter)));
            }
#if DEBUG
            Debug.Assert(cachedRowHeightsAccessAllowed);
#endif
            // cache returned value and reuse it as long as none
            // of the row's state/thickness has changed.
            switch (includeFilter)
            {
                case DataGridViewElementStates.Visible:
                    if (rowsHeightVisible != -1)
                    {
                        return rowsHeightVisible;
                    }
                    break;
                case DataGridViewElementStates.Visible | DataGridViewElementStates.Frozen:
                    if (rowsHeightVisibleFrozen != -1)
                    {
                        return rowsHeightVisibleFrozen;
                    }
                    break;
            }

            int rowsHeight = 0;
            for (int rowIndex = 0; rowIndex < items.Count; rowIndex++)
            {
                if ((GetRowState(rowIndex) & includeFilter) == includeFilter)
                {
                    rowsHeight += ((DataGridViewRow)items[rowIndex]).GetHeight(rowIndex);
                }
            }

            switch (includeFilter)
            {
                case DataGridViewElementStates.Visible:
                    rowsHeightVisible = rowsHeight;
                    break;
                case DataGridViewElementStates.Visible | DataGridViewElementStates.Frozen:
                    rowsHeightVisibleFrozen = rowsHeight;
                    break;
            }
            return rowsHeight;
        }

        // Cumulates the height of the rows from fromRowIndex to toRowIndex-1.
        internal int GetRowsHeight(DataGridViewElementStates includeFilter, int fromRowIndex, int toRowIndex)
        {
            Debug.Assert(toRowIndex >= fromRowIndex);
            Debug.Assert((GetRowState(toRowIndex) & includeFilter) == includeFilter);

            int rowsHeight = 0;
            for (int rowIndex = fromRowIndex; rowIndex < toRowIndex; rowIndex++)
            {
                if ((GetRowState(rowIndex) & includeFilter) == includeFilter)
                {
                    rowsHeight += ((DataGridViewRow)items[rowIndex]).GetHeight(rowIndex);
                }
            }
            return rowsHeight;
        }

        // Checks if the cumulated row heights from fromRowIndex to toRowIndex-1 exceed heightLimit.
        private bool GetRowsHeightExceedLimit(DataGridViewElementStates includeFilter, int fromRowIndex, int toRowIndex, int heightLimit)
        {
            Debug.Assert(toRowIndex >= fromRowIndex);
            Debug.Assert(toRowIndex == items.Count || (GetRowState(toRowIndex) & includeFilter) == includeFilter);

            int rowsHeight = 0;
            for (int rowIndex = fromRowIndex; rowIndex < toRowIndex; rowIndex++)
            {
                if ((GetRowState(rowIndex) & includeFilter) == includeFilter)
                {
                    rowsHeight += ((DataGridViewRow)items[rowIndex]).GetHeight(rowIndex);
                    if (rowsHeight > heightLimit)
                    {
                        return true;
                    }
                }
            }
            return rowsHeight > heightLimit;
        }

        public virtual DataGridViewElementStates GetRowState(int rowIndex)
        {
            if (rowIndex < 0 || rowIndex >= items.Count)
            {
                throw new ArgumentOutOfRangeException(nameof(rowIndex), SR.DataGridViewRowCollection_RowIndexOutOfRange);
            }
            DataGridViewRow dataGridViewRow = SharedRow(rowIndex);
            if (dataGridViewRow.Index == -1)
            {
                return SharedRowState(rowIndex);
            }
            else
            {
                Debug.Assert(dataGridViewRow.Index == rowIndex);
                return dataGridViewRow.GetState(rowIndex);
            }
        }

        public int IndexOf(DataGridViewRow dataGridViewRow)
        {
            return items.IndexOf(dataGridViewRow);
        }

        public virtual void Insert(int rowIndex, params object[] values)
        {
            Debug.Assert(DataGridView != null);

            if (values is null)
            {
                throw new ArgumentNullException(nameof(values));
            }

            if (DataGridView.VirtualMode)
            {
                throw new InvalidOperationException(SR.DataGridView_InvalidOperationInVirtualMode);
            }

            if (DataGridView.DataSource != null)
            {
                throw new InvalidOperationException(SR.DataGridViewRowCollection_AddUnboundRow);
            }

            DataGridViewRow dataGridViewRow = DataGridView.RowTemplateClone;
            dataGridViewRow.SetValuesInternal(values);
            Insert(rowIndex, dataGridViewRow);
        }

        /// <summary>
        ///  Inserts a <see cref='DataGridViewRow'/> to this collection.
        /// </summary>
        public virtual void Insert(int rowIndex, DataGridViewRow dataGridViewRow)
        {
            if (DataGridView.DataSource != null)
            {
                throw new InvalidOperationException(SR.DataGridViewRowCollection_AddUnboundRow);
            }

            if (DataGridView.NoDimensionChangeAllowed)
            {
                throw new InvalidOperationException(SR.DataGridView_ForbiddenOperationInEventHandler);
            }

            InsertInternal(rowIndex, dataGridViewRow);
        }

        public virtual void Insert(int rowIndex, int count)
        {
            if (DataGridView.DataSource != null)
            {
                throw new InvalidOperationException(SR.DataGridViewRowCollection_AddUnboundRow);
            }

            if (rowIndex < 0 || Count < rowIndex)
            {
                throw new ArgumentOutOfRangeException(nameof(rowIndex), SR.DataGridViewRowCollection_IndexDestinationOutOfRange);
            }

            if (count <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(count), SR.DataGridViewRowCollection_CountOutOfRange);
            }

            if (DataGridView.NoDimensionChangeAllowed)
            {
                throw new InvalidOperationException(SR.DataGridView_ForbiddenOperationInEventHandler);
            }

            if (DataGridView.Columns.Count == 0)
            {
                throw new InvalidOperationException(SR.DataGridViewRowCollection_NoColumns);
            }

            if (DataGridView.RowTemplate.Cells.Count > DataGridView.Columns.Count)
            {
                throw new InvalidOperationException(SR.DataGridViewRowCollection_RowTemplateTooManyCells);
            }

            if (DataGridView.NewRowIndex != -1 && rowIndex == Count)
            {
                // Trying to insert after the 'new' row.
                Debug.Assert(DataGridView.AllowUserToAddRowsInternal);
                throw new InvalidOperationException(SR.DataGridViewRowCollection_NoInsertionAfterNewRow);
            }

            DataGridViewRow rowTemplate = DataGridView.RowTemplateClone;
            Debug.Assert(rowTemplate.Cells.Count == DataGridView.Columns.Count);
            DataGridViewElementStates rowTemplateState = rowTemplate.State;
            Debug.Assert((rowTemplateState & (DataGridViewElementStates.Selected | DataGridViewElementStates.Displayed)) == 0);
            rowTemplate.DataGridView = dataGridView;
            int columnIndex = 0;
            foreach (DataGridViewCell dataGridViewCell in rowTemplate.Cells)
            {
                dataGridViewCell.DataGridView = dataGridView;
                Debug.Assert(dataGridViewCell.OwningRow == rowTemplate);
                dataGridViewCell.OwningColumn = DataGridView.Columns[columnIndex];
                columnIndex++;
            }
            if (rowTemplate.HasHeaderCell)
            {
                rowTemplate.HeaderCell.DataGridView = dataGridView;
                rowTemplate.HeaderCell.OwningRow = rowTemplate;
            }

            InsertCopiesPrivate(rowTemplate, rowTemplateState, rowIndex, count);
        }

        internal void InsertInternal(int rowIndex, DataGridViewRow dataGridViewRow)
        {
            Debug.Assert(DataGridView != null);
            if (rowIndex < 0 || Count < rowIndex)
            {
                throw new ArgumentOutOfRangeException(nameof(rowIndex), SR.DataGridViewRowCollection_RowIndexOutOfRange);
            }

            if (dataGridViewRow is null)
            {
                throw new ArgumentNullException(nameof(dataGridViewRow));
            }

            if (dataGridViewRow.DataGridView != null)
            {
                throw new InvalidOperationException(SR.DataGridView_RowAlreadyBelongsToDataGridView);
            }

            if (DataGridView.NewRowIndex != -1 && rowIndex == Count)
            {
                // Trying to insert after the 'new' row.
                Debug.Assert(DataGridView.AllowUserToAddRowsInternal);
                throw new InvalidOperationException(SR.DataGridViewRowCollection_NoInsertionAfterNewRow);
            }

            if (DataGridView.Columns.Count == 0)
            {
                throw new InvalidOperationException(SR.DataGridViewRowCollection_NoColumns);
            }

            if (dataGridViewRow.Cells.Count > DataGridView.Columns.Count)
            {
                throw new ArgumentException(SR.DataGridViewRowCollection_TooManyCells, nameof(dataGridViewRow));
            }

            if (dataGridViewRow.Selected)
            {
                throw new InvalidOperationException(SR.DataGridViewRowCollection_CannotAddOrInsertSelectedRow);
            }

            InsertInternal(rowIndex, dataGridViewRow, false);
        }

        internal void InsertInternal(int rowIndex, DataGridViewRow dataGridViewRow, bool force)
        {
            Debug.Assert(DataGridView != null);
            Debug.Assert(rowIndex >= 0 && rowIndex <= Count);
            Debug.Assert(dataGridViewRow != null);
            Debug.Assert(dataGridViewRow.DataGridView is null);
            Debug.Assert(!DataGridView.NoDimensionChangeAllowed);
            Debug.Assert(DataGridView.NewRowIndex == -1 || rowIndex != Count);
            Debug.Assert(!dataGridViewRow.Selected);

            Point newCurrentCell = new Point(-1, -1);

            if (force)
            {
                if (DataGridView.Columns.Count == 0)
                {
                    throw new InvalidOperationException(SR.DataGridViewRowCollection_NoColumns);
                }
                if (dataGridViewRow.Cells.Count > DataGridView.Columns.Count)
                {
                    throw new ArgumentException(SR.DataGridViewRowCollection_TooManyCells, nameof(dataGridViewRow));
                }
            }
            DataGridView.CompleteCellsCollection(dataGridViewRow);
            Debug.Assert(dataGridViewRow.Cells.Count == DataGridView.Columns.Count);
            DataGridView.OnInsertingRow(rowIndex, dataGridViewRow, dataGridViewRow.State, ref newCurrentCell, true, 1, force);   // will throw an exception if the insertion is illegal

            int columnIndex = 0;
            foreach (DataGridViewCell dataGridViewCell in dataGridViewRow.Cells)
            {
                dataGridViewCell.DataGridView = dataGridView;
                Debug.Assert(dataGridViewCell.OwningRow == dataGridViewRow);
                if (dataGridViewCell.ColumnIndex == -1)
                {
                    dataGridViewCell.OwningColumn = DataGridView.Columns[columnIndex];
                }
                columnIndex++;
            }

            if (dataGridViewRow.HasHeaderCell)
            {
                dataGridViewRow.HeaderCell.DataGridView = DataGridView;
                dataGridViewRow.HeaderCell.OwningRow = dataGridViewRow;
            }

            SharedList.Insert(rowIndex, dataGridViewRow);
            Debug.Assert((dataGridViewRow.State & (DataGridViewElementStates.Selected | DataGridViewElementStates.Displayed)) == 0);
            rowStates.Insert(rowIndex, dataGridViewRow.State);
            Debug.Assert(rowStates.Count == SharedList.Count);
#if DEBUG
            DataGridView._dataStoreAccessAllowed = false;
            cachedRowHeightsAccessAllowed = false;
            cachedRowCountsAccessAllowed = false;
#endif

            dataGridViewRow.DataGridView = dataGridView;
            if (!RowIsSharable(rowIndex) || RowHasValueOrToolTipText(dataGridViewRow) || IsCollectionChangedListenedTo)
            {
                dataGridViewRow.Index = rowIndex;
                Debug.Assert(dataGridViewRow.State == SharedRowState(rowIndex));
            }
            OnCollectionChanged(new CollectionChangeEventArgs(CollectionChangeAction.Add, dataGridViewRow), rowIndex, 1, false, true, false, newCurrentCell);
        }

        public virtual void InsertCopy(int indexSource, int indexDestination)
        {
            InsertCopies(indexSource, indexDestination, 1);
        }

        public virtual void InsertCopies(int indexSource, int indexDestination, int count)
        {
            if (DataGridView.DataSource != null)
            {
                throw new InvalidOperationException(SR.DataGridViewRowCollection_AddUnboundRow);
            }

            if (DataGridView.NoDimensionChangeAllowed)
            {
                throw new InvalidOperationException(SR.DataGridView_ForbiddenOperationInEventHandler);
            }

            InsertCopiesPrivate(indexSource, indexDestination, count);
        }

        private void InsertCopiesPrivate(int indexSource, int indexDestination, int count)
        {
            Debug.Assert(DataGridView != null);

            if (indexSource < 0 || Count <= indexSource)
            {
                throw new ArgumentOutOfRangeException(nameof(indexSource), SR.DataGridViewRowCollection_IndexSourceOutOfRange);
            }

            if (indexDestination < 0 || Count < indexDestination)
            {
                throw new ArgumentOutOfRangeException(nameof(indexDestination), SR.DataGridViewRowCollection_IndexDestinationOutOfRange);
            }

            if (count <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(count), SR.DataGridViewRowCollection_CountOutOfRange);
            }

            if (DataGridView.NewRowIndex != -1 && indexDestination == Count)
            {
                // Trying to insert after the 'new' row.
                Debug.Assert(DataGridView.AllowUserToAddRowsInternal);
                throw new InvalidOperationException(SR.DataGridViewRowCollection_NoInsertionAfterNewRow);
            }

            DataGridViewElementStates rowTemplateState = GetRowState(indexSource) & ~(DataGridViewElementStates.Selected | DataGridViewElementStates.Displayed);
            InsertCopiesPrivate(SharedRow(indexSource), rowTemplateState, indexDestination, count);
        }

        private void InsertCopiesPrivate(DataGridViewRow rowTemplate, DataGridViewElementStates rowTemplateState, int indexDestination, int count)
        {
            Point newCurrentCell = new Point(-1, -1);
            if (rowTemplate.Index == -1)
            {
                if (count > 1)
                {
                    // Done once only, continue to check if this is OK - will throw an exception if the insertion is illegal.
                    DataGridView.OnInsertingRow(indexDestination, rowTemplate, rowTemplateState, ref newCurrentCell, true, count, false /*force*/);
                    for (int i = 0; i < count; i++)
                    {
                        SharedList.Insert(indexDestination + i, rowTemplate);
                        rowStates.Insert(indexDestination + i, rowTemplateState);
                    }
#if DEBUG
                    DataGridView._dataStoreAccessAllowed = false;
                    cachedRowHeightsAccessAllowed = false;
                    cachedRowCountsAccessAllowed = false;
#endif
                    // Only calling this once instead of 'count' times. Continue to check if this is OK.
                    DataGridView.OnInsertedRow_PreNotification(indexDestination, count);
                    OnCollectionChanged(new CollectionChangeEventArgs(CollectionChangeAction.Refresh, null), indexDestination, count, false, true, false, newCurrentCell);
                    for (int i = 0; i < count; i++)
                    {
                        DataGridView.OnInsertedRow_PostNotification(indexDestination + i, newCurrentCell, i == count - 1);
                    }
                }
                else
                {
                    DataGridView.OnInsertingRow(indexDestination, rowTemplate, rowTemplateState, ref newCurrentCell, true, 1, false /*force*/); // will throw an exception if the insertion is illegal
                    SharedList.Insert(indexDestination, rowTemplate);
                    rowStates.Insert(indexDestination, rowTemplateState);
#if DEBUG
                    DataGridView._dataStoreAccessAllowed = false;
                    cachedRowHeightsAccessAllowed = false;
                    cachedRowCountsAccessAllowed = false;
#endif
                    OnCollectionChanged(new CollectionChangeEventArgs(CollectionChangeAction.Add, SharedRow(indexDestination)), indexDestination, count, false, true, false, newCurrentCell);
                }
            }
            else
            {
                // Sets this.DataGridView.dataStoreAccessAllowed to false
                InsertDuplicateRow(indexDestination, rowTemplate, true, ref newCurrentCell);
                Debug.Assert(rowStates.Count == SharedList.Count);
                if (count > 1)
                {
                    DataGridView.OnInsertedRow_PreNotification(indexDestination, 1);
                    if (RowIsSharable(indexDestination))
                    {
                        DataGridViewRow rowTemplate2 = SharedRow(indexDestination);
                        // Done once only, continue to check if this is OK - will throw an exception if the insertion is illegal.
                        DataGridView.OnInsertingRow(indexDestination + 1, rowTemplate2, rowTemplateState, ref newCurrentCell, false, count - 1, false /*force*/);
                        for (int i = 1; i < count; i++)
                        {
                            SharedList.Insert(indexDestination + i, rowTemplate2);
                            rowStates.Insert(indexDestination + i, rowTemplateState);
                        }
#if DEBUG
                        DataGridView._dataStoreAccessAllowed = false;
                        cachedRowHeightsAccessAllowed = false;
                        cachedRowCountsAccessAllowed = false;
#endif
                        // Only calling this once instead of 'count-1' times. Continue to check if this is OK.
                        DataGridView.OnInsertedRow_PreNotification(indexDestination + 1, count - 1);
                        OnCollectionChanged(new CollectionChangeEventArgs(CollectionChangeAction.Refresh, null), indexDestination, count, false, true, false, newCurrentCell);
                    }
                    else
                    {
                        UnshareRow(indexDestination);
                        for (int i = 1; i < count; i++)
                        {
                            InsertDuplicateRow(indexDestination + i, rowTemplate, false, ref newCurrentCell);
                            Debug.Assert(rowStates.Count == SharedList.Count);
                            UnshareRow(indexDestination + i);
                            DataGridView.OnInsertedRow_PreNotification(indexDestination + i, 1);
                        }
                        OnCollectionChanged(new CollectionChangeEventArgs(CollectionChangeAction.Refresh, null), indexDestination, count, false, true, false, newCurrentCell);
                    }
                    for (int i = 0; i < count; i++)
                    {
                        DataGridView.OnInsertedRow_PostNotification(indexDestination + i, newCurrentCell, i == count - 1);
                    }
                }
                else
                {
                    if (IsCollectionChangedListenedTo)
                    {
                        UnshareRow(indexDestination);
                    }
                    OnCollectionChanged(new CollectionChangeEventArgs(CollectionChangeAction.Add, SharedRow(indexDestination)), indexDestination, 1, false, true, false, newCurrentCell);
                }
            }
        }

        private void InsertDuplicateRow(int indexDestination, DataGridViewRow rowTemplate, bool firstInsertion, ref Point newCurrentCell)
        {
            Debug.Assert(DataGridView != null);

            DataGridViewRow dataGridViewRow = (DataGridViewRow)rowTemplate.Clone();
            dataGridViewRow.State = DataGridViewElementStates.None;
            dataGridViewRow.DataGridView = dataGridView;
            DataGridViewCellCollection dgvcc = dataGridViewRow.Cells;
            int columnIndex = 0;
            foreach (DataGridViewCell dataGridViewCell in dgvcc)
            {
                dataGridViewCell.DataGridView = dataGridView;
                dataGridViewCell.OwningColumn = DataGridView.Columns[columnIndex];
                columnIndex++;
            }
            DataGridViewElementStates rowState = rowTemplate.State & ~(DataGridViewElementStates.Selected | DataGridViewElementStates.Displayed);
            if (dataGridViewRow.HasHeaderCell)
            {
                dataGridViewRow.HeaderCell.DataGridView = dataGridView;
                dataGridViewRow.HeaderCell.OwningRow = dataGridViewRow;
            }

            DataGridView.OnInsertingRow(indexDestination, dataGridViewRow, rowState, ref newCurrentCell, firstInsertion, 1, false /*force*/);   // will throw an exception if the insertion is illegal

            Debug.Assert(dataGridViewRow.Index == -1);
            SharedList.Insert(indexDestination, dataGridViewRow);
            rowStates.Insert(indexDestination, rowState);
            Debug.Assert(rowStates.Count == SharedList.Count);
#if DEBUG
            DataGridView._dataStoreAccessAllowed = false;
            cachedRowHeightsAccessAllowed = false;
            cachedRowCountsAccessAllowed = false;
#endif
        }

        /// <summary>
        ///  Inserts a range of <see cref='DataGridViewRow'/> to this collection.
        /// </summary>
        public virtual void InsertRange(int rowIndex, params DataGridViewRow[] dataGridViewRows)
        {
            Debug.Assert(DataGridView != null);

            if (dataGridViewRows is null)
            {
                throw new ArgumentNullException(nameof(dataGridViewRows));
            }

            if (dataGridViewRows.Length == 1)
            {
                Insert(rowIndex, dataGridViewRows[0]);
                return;
            }

            if (rowIndex < 0 || rowIndex > Count)
            {
                throw new ArgumentOutOfRangeException(nameof(rowIndex), SR.DataGridViewRowCollection_IndexDestinationOutOfRange);
            }

            if (DataGridView.NoDimensionChangeAllowed)
            {
                throw new InvalidOperationException(SR.DataGridView_ForbiddenOperationInEventHandler);
            }

            if (DataGridView.NewRowIndex != -1 && rowIndex == Count)
            {
                // Trying to insert after the 'new' row.
                Debug.Assert(DataGridView.AllowUserToAddRowsInternal);
                throw new InvalidOperationException(SR.DataGridViewRowCollection_NoInsertionAfterNewRow);
            }

            if (DataGridView.DataSource != null)
            {
                throw new InvalidOperationException(SR.DataGridViewRowCollection_AddUnboundRow);
            }

            if (DataGridView.Columns.Count == 0)
            {
                throw new InvalidOperationException(SR.DataGridViewRowCollection_NoColumns);
            }

            Point newCurrentCell = new Point(-1, -1);

            // OnInsertingRows checks for Selected flag of each row, among other things.
            DataGridView.OnInsertingRows(rowIndex, dataGridViewRows, ref newCurrentCell);   // will throw an exception if the insertion is illegal

            int rowIndexInserted = rowIndex;
            foreach (DataGridViewRow dataGridViewRow in dataGridViewRows)
            {
                Debug.Assert(dataGridViewRow.Cells.Count == DataGridView.Columns.Count);
                int columnIndex = 0;
                foreach (DataGridViewCell dataGridViewCell in dataGridViewRow.Cells)
                {
                    dataGridViewCell.DataGridView = dataGridView;
                    Debug.Assert(dataGridViewCell.OwningRow == dataGridViewRow);
                    if (dataGridViewCell.ColumnIndex == -1)
                    {
                        dataGridViewCell.OwningColumn = DataGridView.Columns[columnIndex];
                    }
                    columnIndex++;
                }

                if (dataGridViewRow.HasHeaderCell)
                {
                    dataGridViewRow.HeaderCell.DataGridView = DataGridView;
                    dataGridViewRow.HeaderCell.OwningRow = dataGridViewRow;
                }

                SharedList.Insert(rowIndexInserted, dataGridViewRow);
                Debug.Assert((dataGridViewRow.State & (DataGridViewElementStates.Selected | DataGridViewElementStates.Displayed)) == 0);
                rowStates.Insert(rowIndexInserted, dataGridViewRow.State);
                Debug.Assert(rowStates.Count == SharedList.Count);
#if DEBUG
                DataGridView._dataStoreAccessAllowed = false;
                cachedRowHeightsAccessAllowed = false;
                cachedRowCountsAccessAllowed = false;
#endif

                dataGridViewRow.Index = rowIndexInserted;
                Debug.Assert(dataGridViewRow.State == SharedRowState(rowIndexInserted));
                dataGridViewRow.DataGridView = dataGridView;
                rowIndexInserted++;
            }

            DataGridView.OnInsertedRows_PreNotification(rowIndex, dataGridViewRows);
            OnCollectionChanged(new CollectionChangeEventArgs(CollectionChangeAction.Refresh, null), rowIndex, dataGridViewRows.Length, false, true, false, newCurrentCell);
            DataGridView.OnInsertedRows_PostNotification(dataGridViewRows, newCurrentCell);
        }

        internal void InvalidateCachedRowCount(DataGridViewElementStates includeFilter)
        {
            Debug.Assert(includeFilter == DataGridViewElementStates.Displayed ||
                         includeFilter == DataGridViewElementStates.Selected ||
                         includeFilter == DataGridViewElementStates.ReadOnly ||
                         includeFilter == DataGridViewElementStates.Resizable ||
                         includeFilter == DataGridViewElementStates.Frozen ||
                         includeFilter == DataGridViewElementStates.Visible);

            if (includeFilter == DataGridViewElementStates.Visible)
            {
                InvalidateCachedRowCounts();
            }
            else if (includeFilter == DataGridViewElementStates.Frozen)
            {
                rowCountsVisibleFrozen = -1;
            }
            else if (includeFilter == DataGridViewElementStates.Selected)
            {
                rowCountsVisibleSelected = -1;
            }

#if DEBUG
            cachedRowCountsAccessAllowed = true;
#endif
        }

        internal void InvalidateCachedRowCounts()
        {
            rowCountsVisible = rowCountsVisibleFrozen = rowCountsVisibleSelected = -1;
#if DEBUG
            cachedRowCountsAccessAllowed = true;
#endif
        }

        internal void InvalidateCachedRowsHeight(DataGridViewElementStates includeFilter)
        {
            Debug.Assert(includeFilter == DataGridViewElementStates.Displayed ||
                         includeFilter == DataGridViewElementStates.Selected ||
                         includeFilter == DataGridViewElementStates.ReadOnly ||
                         includeFilter == DataGridViewElementStates.Resizable ||
                         includeFilter == DataGridViewElementStates.Frozen ||
                         includeFilter == DataGridViewElementStates.Visible);

            if (includeFilter == DataGridViewElementStates.Visible)
            {
                InvalidateCachedRowsHeights();
            }
            else if (includeFilter == DataGridViewElementStates.Frozen)
            {
                rowsHeightVisibleFrozen = -1;
            }

#if DEBUG
            cachedRowHeightsAccessAllowed = true;
#endif
        }

        internal void InvalidateCachedRowsHeights()
        {
            rowsHeightVisible = rowsHeightVisibleFrozen = -1;
#if DEBUG
            cachedRowHeightsAccessAllowed = true;
#endif
        }

        protected virtual void OnCollectionChanged(CollectionChangeEventArgs e)
        {
            onCollectionChanged?.Invoke(this, e);
        }

        private void OnCollectionChanged(CollectionChangeEventArgs e,
                                         int rowIndex,
                                         int rowCount)
        {
            Debug.Assert(e.Action != CollectionChangeAction.Remove);
            Point newCurrentCell = new Point(-1, -1);
            DataGridViewRow dataGridViewRow = (DataGridViewRow)e.Element;
            int originalIndex = 0;
            if (dataGridViewRow != null && e.Action == CollectionChangeAction.Add)
            {
                originalIndex = SharedRow(rowIndex).Index;
            }
            OnCollectionChanged_PreNotification(e.Action, rowIndex, rowCount, ref dataGridViewRow, false);
            if (originalIndex == -1 && SharedRow(rowIndex).Index != -1)
            {
                // row got unshared inside OnCollectionChanged_PreNotification
                e = new CollectionChangeEventArgs(e.Action, dataGridViewRow);
            }
            OnCollectionChanged(e);
            OnCollectionChanged_PostNotification(e.Action, rowIndex, rowCount, dataGridViewRow, false, false, false, newCurrentCell);
        }

        private void OnCollectionChanged(CollectionChangeEventArgs e,
                                         int rowIndex,
                                         int rowCount,
                                         bool changeIsDeletion,
                                         bool changeIsInsertion,
                                         bool recreateNewRow,
                                         Point newCurrentCell)
        {
            DataGridViewRow dataGridViewRow = (DataGridViewRow)e.Element;
            int originalIndex = 0;
            if (dataGridViewRow != null && e.Action == CollectionChangeAction.Add)
            {
                originalIndex = SharedRow(rowIndex).Index;
            }
            OnCollectionChanged_PreNotification(e.Action, rowIndex, rowCount, ref dataGridViewRow, changeIsInsertion);
            if (originalIndex == -1 && SharedRow(rowIndex).Index != -1)
            {
                // row got unshared inside OnCollectionChanged_PreNotification
                e = new CollectionChangeEventArgs(e.Action, dataGridViewRow);
            }
            OnCollectionChanged(e);
            OnCollectionChanged_PostNotification(e.Action, rowIndex, rowCount, dataGridViewRow, changeIsDeletion, changeIsInsertion, recreateNewRow, newCurrentCell);
        }

        private void OnCollectionChanged_PreNotification(CollectionChangeAction cca,
                                                         int rowIndex,
                                                         int rowCount,
                                                         ref DataGridViewRow dataGridViewRow,
                                                         bool changeIsInsertion)
        {
            Debug.Assert(DataGridView != null);
            bool useRowShortcut = false;
            bool computeVisibleRows = false;
            switch (cca)
            {
                case CollectionChangeAction.Add:
                    {
                        int firstDisplayedRowHeight = 0;
                        UpdateRowCaches(rowIndex, ref dataGridViewRow, true);
                        if ((GetRowState(rowIndex) & DataGridViewElementStates.Visible) == 0)
                        {
                            // Adding an invisible row - no need for repaint
                            useRowShortcut = true;
                            computeVisibleRows = changeIsInsertion;
                        }
                        else
                        {
                            int firstDisplayedRowIndex = DataGridView.FirstDisplayedRowIndex;
                            if (firstDisplayedRowIndex != -1)
                            {
                                firstDisplayedRowHeight = SharedRow(firstDisplayedRowIndex).GetHeight(firstDisplayedRowIndex);
                            }
                        }
                        if (changeIsInsertion)
                        {
                            DataGridView.OnInsertedRow_PreNotification(rowIndex, 1);
                            if (!useRowShortcut)
                            {
                                if ((GetRowState(rowIndex) & DataGridViewElementStates.Frozen) != 0)
                                {
                                    // Inserted row is frozen
                                    useRowShortcut = DataGridView.FirstDisplayedScrollingRowIndex == -1 &&
                                                     GetRowsHeightExceedLimit(DataGridViewElementStates.Visible, 0, rowIndex, DataGridView.LayoutInfo.Data.Height);
                                }
                                else if (DataGridView.FirstDisplayedScrollingRowIndex != -1 &&
                                         rowIndex > DataGridView.FirstDisplayedScrollingRowIndex)
                                {
                                    useRowShortcut = GetRowsHeightExceedLimit(DataGridViewElementStates.Visible, 0, rowIndex, DataGridView.LayoutInfo.Data.Height + DataGridView.VerticalScrollingOffset) &&
                                                     firstDisplayedRowHeight <= DataGridView.LayoutInfo.Data.Height;
                                }
                            }
                        }
                        else
                        {
                            DataGridView.OnAddedRow_PreNotification(rowIndex);
                            if (!useRowShortcut)
                            {
                                int displayedRowsHeightBeforeAddition = GetRowsHeight(DataGridViewElementStates.Visible) - DataGridView.VerticalScrollingOffset - dataGridViewRow.GetHeight(rowIndex);
                                dataGridViewRow = SharedRow(rowIndex);
                                useRowShortcut = DataGridView.LayoutInfo.Data.Height < displayedRowsHeightBeforeAddition &&
                                                 firstDisplayedRowHeight <= DataGridView.LayoutInfo.Data.Height;
                            }
                        }
                        break;
                    }

                case CollectionChangeAction.Remove:
                    {
                        Debug.Assert(rowCount == 1);
                        DataGridViewElementStates rowStates = GetRowState(rowIndex);
                        bool deletedRowVisible = (rowStates & DataGridViewElementStates.Visible) != 0;
                        bool deletedRowFrozen = (rowStates & DataGridViewElementStates.Frozen) != 0;

                        // Can't do this earlier since it would break UpdateRowCaches
                        this.rowStates.RemoveAt(rowIndex);
                        SharedList.RemoveAt(rowIndex);
#if DEBUG
                        DataGridView._dataStoreAccessAllowed = false;
#endif
                        DataGridView.OnRemovedRow_PreNotification(rowIndex);
                        if (deletedRowVisible)
                        {
                            if (deletedRowFrozen)
                            {
                                // Delete row is frozen
                                useRowShortcut = DataGridView.FirstDisplayedScrollingRowIndex == -1 &&
                                                 GetRowsHeightExceedLimit(DataGridViewElementStates.Visible, 0, rowIndex, DataGridView.LayoutInfo.Data.Height + SystemInformation.HorizontalScrollBarHeight);
                            }
                            else if (DataGridView.FirstDisplayedScrollingRowIndex != -1 &&
                                     rowIndex > DataGridView.FirstDisplayedScrollingRowIndex)
                            {
                                int firstDisplayedRowHeight = 0;
                                int firstDisplayedRowIndex = DataGridView.FirstDisplayedRowIndex;
                                if (firstDisplayedRowIndex != -1)
                                {
                                    firstDisplayedRowHeight = SharedRow(firstDisplayedRowIndex).GetHeight(firstDisplayedRowIndex);
                                }
                                useRowShortcut = GetRowsHeightExceedLimit(DataGridViewElementStates.Visible, 0, rowIndex, DataGridView.LayoutInfo.Data.Height + DataGridView.VerticalScrollingOffset + SystemInformation.HorizontalScrollBarHeight) &&
                                                 firstDisplayedRowHeight <= DataGridView.LayoutInfo.Data.Height;
                            }
                        }
                        else
                        {
                            // Deleting an invisible row - no need for repaint
                            useRowShortcut = true;
                        }
                        break;
                    }

                case CollectionChangeAction.Refresh:
                    {
                        InvalidateCachedRowCounts();
                        InvalidateCachedRowsHeights();
                        break;
                    }

                default:
                    {
                        Debug.Fail("Unexpected cca value in DataGridViewRowCollecttion.OnCollectionChanged");
                        break;
                    }
            }
            DataGridView.ResetUIState(useRowShortcut, computeVisibleRows);
        }

        private void OnCollectionChanged_PostNotification(CollectionChangeAction cca,
                                                          int rowIndex,
                                                          int rowCount,
                                                          DataGridViewRow dataGridViewRow,
                                                          bool changeIsDeletion,
                                                          bool changeIsInsertion,
                                                          bool recreateNewRow,
                                                          Point newCurrentCell)
        {
            Debug.Assert(DataGridView != null);
            if (changeIsDeletion)
            {
                DataGridView.OnRowsRemovedInternal(rowIndex, rowCount);
            }
            else
            {
                DataGridView.OnRowsAddedInternal(rowIndex, rowCount);
            }

#if DEBUG
            DataGridView._dataStoreAccessAllowed = true;
#endif
            switch (cca)
            {
                case CollectionChangeAction.Add:
                    {
                        if (changeIsInsertion)
                        {
                            DataGridView.OnInsertedRow_PostNotification(rowIndex, newCurrentCell, true);
                        }
                        else
                        {
                            DataGridView.OnAddedRow_PostNotification(rowIndex);
                        }
                        break;
                    }

                case CollectionChangeAction.Remove:
                    {
                        DataGridView.OnRemovedRow_PostNotification(dataGridViewRow, newCurrentCell);
                        break;
                    }

                case CollectionChangeAction.Refresh:
                    {
                        if (changeIsDeletion)
                        {
                            DataGridView.OnClearedRows();
                        }
                        break;
                    }
            }

            DataGridView.OnRowCollectionChanged_PostNotification(recreateNewRow, newCurrentCell.X == -1, cca, dataGridViewRow, rowIndex);
        }

        public virtual void Remove(DataGridViewRow dataGridViewRow)
        {
            if (dataGridViewRow is null)
            {
                throw new ArgumentNullException(nameof(dataGridViewRow));
            }

            if (dataGridViewRow.DataGridView != DataGridView)
            {
                throw new ArgumentException(SR.DataGridView_RowDoesNotBelongToDataGridView, nameof(dataGridViewRow));
            }

            if (dataGridViewRow.Index == -1)
            {
                throw new ArgumentException(SR.DataGridView_RowMustBeUnshared, nameof(dataGridViewRow));
            }
            else
            {
                RemoveAt(dataGridViewRow.Index);
            }
        }

        public virtual void RemoveAt(int index)
        {
            if (index < 0 || index >= Count)
            {
                throw new ArgumentOutOfRangeException(nameof(index), SR.DataGridViewRowCollection_RowIndexOutOfRange);
            }

            if (DataGridView.NewRowIndex == index)
            {
                Debug.Assert(DataGridView.AllowUserToAddRowsInternal);
                throw new InvalidOperationException(SR.DataGridViewRowCollection_CannotDeleteNewRow);
            }

            if (DataGridView.NoDimensionChangeAllowed)
            {
                throw new InvalidOperationException(SR.DataGridView_ForbiddenOperationInEventHandler);
            }

            if (DataGridView.DataSource != null)
            {
                if (DataGridView.DataConnection.List is IBindingList list && list.AllowRemove && list.SupportsChangeNotification)
                {
                    ((IList)list).RemoveAt(index);
                }
                else
                {
                    throw new InvalidOperationException(SR.DataGridViewRowCollection_CantRemoveRowsWithWrongSource);
                }
            }
            else
            {
                RemoveAtInternal(index, false /*force*/);
            }
        }

        internal void RemoveAtInternal(int index, bool force)
        {
            // If force is true, the underlying data is gone and can't be accessed anymore.

            Debug.Assert(index >= 0 && index < Count);
            Debug.Assert(DataGridView != null);
            Debug.Assert(!DataGridView.NoDimensionChangeAllowed);

            DataGridViewRow dataGridViewRow = SharedRow(index);
            Point newCurrentCell = new Point(-1, -1);

            if (IsCollectionChangedListenedTo || dataGridViewRow.GetDisplayed(index))
            {
                dataGridViewRow = this[index]; // need to unshare row because dev is listening to OnCollectionChanged event or the row is displayed
            }

            dataGridViewRow = SharedRow(index);
            Debug.Assert(DataGridView != null);
            DataGridView.OnRemovingRow(index, out newCurrentCell, force);
            UpdateRowCaches(index, ref dataGridViewRow, false /*adding*/);
            if (dataGridViewRow.Index != -1)
            {
                rowStates[index] = dataGridViewRow.State;
                // Only detach unshared rows, since a shared row has never been accessed by the user
                dataGridViewRow.DetachFromDataGridView();
            }
            // Note: cannot set dataGridViewRow.DataGridView to null because this row may be shared and still be used.
            // Note that OnCollectionChanged takes care of calling this.items.RemoveAt(index) &
            // this.rowStates.RemoveAt(index). Can't do it here since OnCollectionChanged uses the arrays.
            OnCollectionChanged(new CollectionChangeEventArgs(CollectionChangeAction.Remove, dataGridViewRow), index, 1, true, false, false, newCurrentCell);
        }

        private static bool RowHasValueOrToolTipText(DataGridViewRow dataGridViewRow)
        {
            Debug.Assert(dataGridViewRow != null);
            Debug.Assert(dataGridViewRow.Index == -1);

            DataGridViewCellCollection cells = dataGridViewRow.Cells;
            foreach (DataGridViewCell dataGridViewCell in cells)
            {
                if (dataGridViewCell.HasValue || dataGridViewCell.HasToolTipText)
                {
                    return true;
                }
            }
            return false;
        }

        internal bool RowIsSharable(int index)
        {
            Debug.Assert(index >= 0);
            Debug.Assert(index < Count);

            DataGridViewRow dataGridViewRow = SharedRow(index);
            if (dataGridViewRow.Index != -1)
            {
                return false;
            }

            // A row is sharable if all of its cells' states can be deduced from
            // the column and row states.
            DataGridViewCellCollection cells = dataGridViewRow.Cells;
            foreach (DataGridViewCell dataGridViewCell in cells)
            {
                if ((dataGridViewCell.State & ~(dataGridViewCell.CellStateFromColumnRowStates(rowStates[index]))) != 0)
                {
                    return false;
                }
            }
            return true;
        }

        internal void SetRowState(int rowIndex, DataGridViewElementStates state, bool value)
        {
            Debug.Assert(state == DataGridViewElementStates.Displayed || state == DataGridViewElementStates.Selected || state == DataGridViewElementStates.ReadOnly || state == DataGridViewElementStates.Resizable || state == DataGridViewElementStates.Frozen || state == DataGridViewElementStates.Visible);

            DataGridViewRow dataGridViewRow = SharedRow(rowIndex);
            if (dataGridViewRow.Index == -1)
            {
                // row is shared
                if (((rowStates[rowIndex] & state) != 0) != value)
                {
                    if (state == DataGridViewElementStates.Frozen ||
                        state == DataGridViewElementStates.Visible ||
                        state == DataGridViewElementStates.ReadOnly)
                    {
                        dataGridViewRow.OnSharedStateChanging(rowIndex, state);
                    }
                    if (value)
                    {
                        rowStates[rowIndex] = rowStates[rowIndex] | state;
                    }
                    else
                    {
                        rowStates[rowIndex] = rowStates[rowIndex] & ~state;
                    }
                    dataGridViewRow.OnSharedStateChanged(rowIndex, state);
                }
            }
            else
            {
                // row is unshared
                switch (state)
                {
                    case DataGridViewElementStates.Displayed:
                        {
                            dataGridViewRow.Displayed = value;
                            break;
                        }
                    case DataGridViewElementStates.Selected:
                        {
                            dataGridViewRow.SelectedInternal = value;
                            break;
                        }
                    case DataGridViewElementStates.Visible:
                        {
                            dataGridViewRow.Visible = value;
                            break;
                        }
                    case DataGridViewElementStates.Frozen:
                        {
                            dataGridViewRow.Frozen = value;
                            break;
                        }
                    case DataGridViewElementStates.ReadOnly:
                        {
                            dataGridViewRow.ReadOnlyInternal = value;
                            break;
                        }
                    case DataGridViewElementStates.Resizable:
                        {
                            dataGridViewRow.Resizable = value ? DataGridViewTriState.True : DataGridViewTriState.False;
                            break;
                        }
                    default:
                        {
                            Debug.Fail("Unexpected DataGridViewElementStates parameter in DataGridViewRowCollection.SetRowState.");
                            break;
                        }
                }
            }
        }

        internal DataGridViewElementStates SharedRowState(int rowIndex)
        {
            return rowStates[rowIndex];
        }

        internal void Sort(IComparer customComparer, bool ascending)
        {
            if (items.Count > 0)
            {
                RowComparer rowComparer = new RowComparer(this, customComparer, ascending);
                items.CustomSort(rowComparer);
                // Caller takes care of the dataGridView invalidation
            }
        }

        internal void SwapSortedRows(int rowIndex1, int rowIndex2)
        {
            // Deal with the current cell address updates +
            // selected rows updates.
            DataGridView.SwapSortedRows(rowIndex1, rowIndex2);

            DataGridViewRow dataGridViewRow1 = SharedRow(rowIndex1);
            DataGridViewRow dataGridViewRow2 = SharedRow(rowIndex2);

            if (dataGridViewRow1.Index != -1)
            {
                dataGridViewRow1.Index = rowIndex2;
            }
            if (dataGridViewRow2.Index != -1)
            {
                dataGridViewRow2.Index = rowIndex1;
            }

            if (DataGridView.VirtualMode)
            {
                // All cell contents on the involved rows need to be swapped
                int columnCount = DataGridView.Columns.Count;

                for (int columnIndex = 0; columnIndex < columnCount; columnIndex++)
                {
                    DataGridViewCell dataGridViewCell1 = dataGridViewRow1.Cells[columnIndex];
                    DataGridViewCell dataGridViewCell2 = dataGridViewRow2.Cells[columnIndex];
                    object value1 = dataGridViewCell1.GetValueInternal(rowIndex1);
                    object value2 = dataGridViewCell2.GetValueInternal(rowIndex2);
                    dataGridViewCell1.SetValueInternal(rowIndex1, value2);
                    dataGridViewCell2.SetValueInternal(rowIndex2, value1);
                }
            }

            object item = items[rowIndex1];
            items[rowIndex1] = items[rowIndex2];
            items[rowIndex2] = item;

            DataGridViewElementStates rowStates = this.rowStates[rowIndex1];
            this.rowStates[rowIndex1] = this.rowStates[rowIndex2];
            this.rowStates[rowIndex2] = rowStates;
        }

        // This function only adjusts the row's RowIndex and State properties - no more.
        private void UnshareRow(int rowIndex)
        {
            SharedRow(rowIndex).Index = rowIndex;
            SharedRow(rowIndex).State = SharedRowState(rowIndex);
        }

        private void UpdateRowCaches(int rowIndex, ref DataGridViewRow dataGridViewRow, bool adding)
        {
            if (rowCountsVisible != -1 || rowCountsVisibleFrozen != -1 || rowCountsVisibleSelected != -1 ||
                rowsHeightVisible != -1 || rowsHeightVisibleFrozen != -1)
            {
                DataGridViewElementStates rowStates = GetRowState(rowIndex);
                if ((rowStates & DataGridViewElementStates.Visible) != 0)
                {
                    int rowCountIncrement = adding ? 1 : -1;
                    int rowHeightIncrement = 0;
                    if (rowsHeightVisible != -1 ||
                        (rowsHeightVisibleFrozen != -1 &&
                         ((rowStates & (DataGridViewElementStates.Visible | DataGridViewElementStates.Frozen)) == (DataGridViewElementStates.Visible | DataGridViewElementStates.Frozen))))
                    {
                        // dataGridViewRow may become unshared in GetHeight call
                        rowHeightIncrement = adding ? dataGridViewRow.GetHeight(rowIndex) : -dataGridViewRow.GetHeight(rowIndex);
                        dataGridViewRow = SharedRow(rowIndex);
                    }

                    if (rowCountsVisible != -1)
                    {
                        rowCountsVisible += rowCountIncrement;
                    }
                    if (rowsHeightVisible != -1)
                    {
                        Debug.Assert(rowHeightIncrement != 0);
                        rowsHeightVisible += rowHeightIncrement;
                    }

                    if ((rowStates & (DataGridViewElementStates.Visible | DataGridViewElementStates.Frozen)) == (DataGridViewElementStates.Visible | DataGridViewElementStates.Frozen))
                    {
                        if (rowCountsVisibleFrozen != -1)
                        {
                            rowCountsVisibleFrozen += rowCountIncrement;
                        }
                        if (rowsHeightVisibleFrozen != -1)
                        {
                            Debug.Assert(rowHeightIncrement != 0);
                            rowsHeightVisibleFrozen += rowHeightIncrement;
                        }
                    }

                    if ((rowStates & (DataGridViewElementStates.Visible | DataGridViewElementStates.Selected)) == (DataGridViewElementStates.Visible | DataGridViewElementStates.Selected))
                    {
                        if (rowCountsVisibleSelected != -1)
                        {
                            rowCountsVisibleSelected += rowCountIncrement;
                        }
                    }
                }
            }

#if DEBUG
            cachedRowCountsAccessAllowed = true;
            cachedRowHeightsAccessAllowed = true;
#endif
        }

        private class RowArrayList : ArrayList
        {
            private readonly DataGridViewRowCollection owner;
            private RowComparer rowComparer;

            public RowArrayList(DataGridViewRowCollection owner)
            {
                this.owner = owner;
            }

            public void CustomSort(RowComparer rowComparer)
            {
                Debug.Assert(rowComparer != null);
                Debug.Assert(Count > 0);

                this.rowComparer = rowComparer;
                CustomQuickSort(0, Count - 1);
            }

            private void CustomQuickSort(int left, int right)
            {
                // Custom recursive QuickSort needed because of the notion of shared rows.
                // The indexes of the compared rows are required to do the comparisons.
                // For a study comparing the iterative and recursive versions of the QuickSort
                // see http://www.mathcs.carleton.edu/courses/course_resources/cs227_w96/wightmaj/data.html
                // Is the recursive version going to cause trouble with large dataGridViews?
                do
                {
                    if (right - left < 2) // sort subarray of two elements
                    {
                        if (right - left > 0 && rowComparer.CompareObjects(rowComparer.GetComparedObject(left), rowComparer.GetComparedObject(right), left, right) > 0)
                        {
                            owner.SwapSortedRows(left, right);
                        }
                        return;
                    }

                    int k = (left + right) >> 1;
                    object x = Pivot(left, k, right);
                    int i = left + 1;
                    int j = right - 1;
                    do
                    {
                        while (k != i && rowComparer.CompareObjects(rowComparer.GetComparedObject(i), x, i, k) < 0)
                        {
                            i++;
                        }
                        while (k != j && rowComparer.CompareObjects(x, rowComparer.GetComparedObject(j), k, j) < 0)
                        {
                            j--;
                        }
                        Debug.Assert(i >= left && j <= right, "(i>=left && j<=right)  Sort failed - Is your IComparer bogus?");
                        if (i > j)
                        {
                            break;
                        }
                        if (i < j)
                        {
                            owner.SwapSortedRows(i, j);
                            if (i == k)
                            {
                                k = j;
                            }
                            else if (j == k)
                            {
                                k = i;
                            }
                        }
                        i++;
                        j--;
                    }
                    while (i <= j);

                    if (j - left <= right - i)
                    {
                        if (left < j)
                        {
                            CustomQuickSort(left, j);
                        }
                        left = i;
                    }
                    else
                    {
                        if (i < right)
                        {
                            CustomQuickSort(i, right);
                        }
                        right = j;
                    }
                }
                while (left < right);
            }

            private object Pivot(int left, int center, int right)
            {
                // find median-of-3 (left, center and right) and sort these 3 elements
                if (rowComparer.CompareObjects(rowComparer.GetComparedObject(left), rowComparer.GetComparedObject(center), left, center) > 0)
                {
                    owner.SwapSortedRows(left, center);
                }
                if (rowComparer.CompareObjects(rowComparer.GetComparedObject(left), rowComparer.GetComparedObject(right), left, right) > 0)
                {
                    owner.SwapSortedRows(left, right);
                }
                if (rowComparer.CompareObjects(rowComparer.GetComparedObject(center), rowComparer.GetComparedObject(right), center, right) > 0)
                {
                    owner.SwapSortedRows(center, right);
                }
                return rowComparer.GetComparedObject(center);
            }
        }

        private class RowComparer
        {
            private readonly DataGridView dataGridView;
            private readonly DataGridViewRowCollection dataGridViewRows;
            private readonly DataGridViewColumn dataGridViewSortedColumn;
            private readonly int sortedColumnIndex;
            private readonly IComparer customComparer;
            private readonly bool ascending;
            private static readonly ComparedObjectMax max = new ComparedObjectMax();

            public RowComparer(DataGridViewRowCollection dataGridViewRows, IComparer customComparer, bool ascending)
            {
                dataGridView = dataGridViewRows.DataGridView;
                this.dataGridViewRows = dataGridViewRows;
                dataGridViewSortedColumn = dataGridView.SortedColumn;
                if (dataGridViewSortedColumn is null)
                {
                    Debug.Assert(customComparer != null);
                    sortedColumnIndex = -1;
                }
                else
                {
                    sortedColumnIndex = dataGridViewSortedColumn.Index;
                }
                this.customComparer = customComparer;
                this.ascending = ascending;
            }

            internal object GetComparedObject(int rowIndex)
            {
                if (dataGridView.NewRowIndex != -1)
                {
                    Debug.Assert(dataGridView.AllowUserToAddRowsInternal);
                    if (rowIndex == dataGridView.NewRowIndex)
                    {
                        return max;
                    }
                }
                if (customComparer is null)
                {
                    DataGridViewRow dataGridViewRow = dataGridViewRows.SharedRow(rowIndex);
                    Debug.Assert(dataGridViewRow != null);
                    Debug.Assert(sortedColumnIndex >= 0);
                    return dataGridViewRow.Cells[sortedColumnIndex].GetValueInternal(rowIndex);
                }
                else
                {
                    return dataGridViewRows[rowIndex]; // Unsharing compared rows!
                }
            }

            internal int CompareObjects(object value1, object value2, int rowIndex1, int rowIndex2)
            {
                if (value1 is ComparedObjectMax)
                {
                    return 1;
                }
                else if (value2 is ComparedObjectMax)
                {
                    return -1;
                }
                int result = 0;
                if (customComparer is null)
                {
                    if (!dataGridView.OnSortCompare(dataGridViewSortedColumn, value1, value2, rowIndex1, rowIndex2, out result))
                    {
                        if (!(value1 is IComparable) && !(value2 is IComparable))
                        {
                            if (value1 is null)
                            {
                                if (value2 is null)
                                {
                                    result = 0;
                                }
                                else
                                {
                                    result = 1;
                                }
                            }
                            else if (value2 is null)
                            {
                                result = -1;
                            }
                            else
                            {
                                result = Comparer.Default.Compare(value1.ToString(), value2.ToString());
                            }
                        }
                        else
                        {
                            result = Comparer.Default.Compare(value1, value2);
                        }
                        if (result == 0)
                        {
                            if (ascending)
                            {
                                result = rowIndex1 - rowIndex2;
                            }
                            else
                            {
                                result = rowIndex2 - rowIndex1;
                            }
                        }
                    }
                }
                else
                {
                    Debug.Assert(value1 is DataGridViewRow);
                    Debug.Assert(value2 is DataGridViewRow);
                    Debug.Assert(value1 != null);
                    Debug.Assert(value2 != null);
                    //

                    result = customComparer.Compare(value1, value2);
                }

                if (ascending)
                {
                    return result;
                }
                else
                {
                    return -result;
                }
            }

            class ComparedObjectMax
            {
                public ComparedObjectMax() { }
            }
        }

        private class UnsharingRowEnumerator : IEnumerator
        {
            private readonly DataGridViewRowCollection owner;
            private int current;

            /// <summary>
            ///  Creates a new enumerator that will enumerate over the rows and unshare the accessed rows if needed.
            /// </summary>
            public UnsharingRowEnumerator(DataGridViewRowCollection owner)
            {
                this.owner = owner;
                current = -1;
            }

            /// <summary>
            ///  Moves to the next element, or returns false if at the end.
            /// </summary>
            bool IEnumerator.MoveNext()
            {
                if (current < owner.Count - 1)
                {
                    current++;
                    return true;
                }
                else
                {
                    current = owner.Count;
                    return false;
                }
            }

            /// <summary>
            ///  Resets the enumeration back to the beginning.
            /// </summary>
            void IEnumerator.Reset()
            {
                current = -1;
            }

            /// <summary>
            ///  Retrieves the current value in the enumerator.
            /// </summary>
            object IEnumerator.Current
            {
                get
                {
                    if (current == -1)
                    {
                        throw new InvalidOperationException(SR.DataGridViewRowCollection_EnumNotStarted);
                    }
                    if (current == owner.Count)
                    {
                        throw new InvalidOperationException(SR.DataGridViewRowCollection_EnumFinished);
                    }
                    return owner[current];
                }
            }
        }
    }
}
