// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms
{
    using System.Diagnostics;

    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Windows.Forms;
    using System.ComponentModel;
    using System.ComponentModel.Design.Serialization;
    using System.Globalization;
    using System.Diagnostics.CodeAnalysis;
    
    /// <include file='doc\DataGridViewRowCollection.uex' path='docs/doc[@for="DataGridViewRowCollection"]/*' />
    /// <devdoc>
    /// <para>Represents a collection of <see cref='System.Windows.Forms.DataGridViewRow'/> objects in the <see cref='System.Windows.Forms.DataGrid'/> 
    /// control.</para>
    /// </devdoc>
    [
        ListBindable(false),
        DesignerSerializerAttribute("System.Windows.Forms.Design.DataGridViewRowCollectionCodeDomSerializer, " + AssemblyRef.SystemDesign,
                                    "System.ComponentModel.Design.Serialization.CodeDomSerializer, " + AssemblyRef.SystemDesign),
        SuppressMessage("Microsoft.Design", "CA1010:CollectionsShouldImplementGenericInterface") // Consider adding an IList<DataGridViewRowCollection> implementation
    ]
    public class DataGridViewRowCollection : ICollection ,IList
    {
#if DEBUG
        // set to false when the cached row heights are dirty and should not be accessed.
        private bool cachedRowHeightsAccessAllowed = true;

        // set to false when the cached row counts are dirty and should not be accessed.
        private bool cachedRowCountsAccessAllowed = true;
#endif

        private CollectionChangeEventHandler onCollectionChanged;
        private RowArrayList items;
        private List<DataGridViewElementStates> rowStates;
        private int rowCountsVisible, rowCountsVisibleFrozen, rowCountsVisibleSelected;
        private int rowsHeightVisible, rowsHeightVisibleFrozen;
        private DataGridView dataGridView;

        /* IList interface implementation */

        /// <include file='doc\DataGridViewRowCollection.uex' path='docs/doc[@for="DataGridViewRowCollection.IList.Add"]/*' />
        /// <internalonly/>
        int IList.Add(object value)
        {
            return this.Add((DataGridViewRow) value);            
        }

        /// <include file='doc\DataGridViewRowCollection.uex' path='docs/doc[@for="DataGridViewRowCollection.IList.Clear"]/*' />
        /// <internalonly/>
        void IList.Clear()
        {
            this.Clear();
        }

        /// <include file='doc\DataGridViewRowCollection.uex' path='docs/doc[@for="DataGridViewRowCollection.IList.Contains"]/*' />
        /// <internalonly/>
        bool IList.Contains(object value)
        {
            return this.items.Contains(value);
        }

        /// <include file='doc\DataGridViewRowCollection.uex' path='docs/doc[@for="DataGridViewRowCollection.IList.IndexOf"]/*' />
        /// <internalonly/>
        int IList.IndexOf(object value)
        {
            return this.items.IndexOf(value);
        }

        /// <include file='doc\DataGridViewRowCollection.uex' path='docs/doc[@for="DataGridViewRowCollection.IList.Insert"]/*' />
        /// <internalonly/>
        void IList.Insert(int index, object value)
        {
            this.Insert(index, (DataGridViewRow) value);
        }

        /// <include file='doc\DataGridViewRowCollection.uex' path='docs/doc[@for="DataGridViewRowCollection.IList.Remove"]/*' />
        /// <internalonly/>
        void IList.Remove(object value)
        {
            this.Remove((DataGridViewRow) value);
        }

        /// <include file='doc\DataGridViewRowCollection.uex' path='docs/doc[@for="DataGridViewRowCollection.IList.RemoveAt"]/*' />
        /// <internalonly/>
        void IList.RemoveAt(int index)
        {
            this.RemoveAt(index);
        }

        /// <include file='doc\DataGridViewRowCollection.uex' path='docs/doc[@for="DataGridViewRowCollection.IList.IsFixedSize"]/*' />
        /// <internalonly/>
        bool IList.IsFixedSize
        {
            get
            {
                return false;
            }
        }

        /// <include file='doc\DataGridViewRowCollection.uex' path='docs/doc[@for="DataGridViewRowCollection.IList.IsReadOnly"]/*' />
        /// <internalonly/>
        bool IList.IsReadOnly
        {
            get
            {
                return false;
            }
        }

        /// <include file='doc\DataGridViewRowCollection.uex' path='docs/doc[@for="DataGridViewRowCollection.IList.this"]/*' />
        /// <internalonly/>
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

        /// <include file='doc\DataGridViewRowCollection.uex' path='docs/doc[@for="DataGridViewRowCollection.ICollection.CopyTo"]/*' />
        /// <internalonly/>
        void ICollection.CopyTo(Array array, int index)
        {
            this.items.CopyTo(array, index);
        }

        /// <include file='doc\DataGridViewRowCollection.uex' path='docs/doc[@for="DataGridViewRowCollection.ICollection.Count"]/*' />
        /// <internalonly/>
        int ICollection.Count 
        {
            get 
            {
                return this.Count;
            }
        }

        /// <include file='doc\DataGridViewRowCollection.uex' path='docs/doc[@for="DataGridViewRowCollection.ICollection.IsSynchronized"]/*' />
        /// <internalonly/>
        bool ICollection.IsSynchronized
        {
            get
            {
                return false;
            }
        }

        /// <include file='doc\DataGridViewRowCollection.uex' path='docs/doc[@for="DataGridViewRowCollection.ICollection.SyncRoot"]/*' />
        /// <internalonly/>
        object ICollection.SyncRoot
        {
            get
            {
                return this;
            }
        }

        /* IEnumerator interface implementation */

        /// <include file='doc\DataGridViewRowCollection.uex' path='docs/doc[@for="DataGridViewRowCollection.IEnumerable.GetEnumerator"]/*' />
        /// <internalonly/>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return new UnsharingRowEnumerator(this);
        }


        /// <include file='doc\DataGridViewRowCollection.uex' path='docs/doc[@for="DataGridViewRowCollection.DataGridViewRowCollection"]/*' />
        public DataGridViewRowCollection(DataGridView dataGridView)
        {
            InvalidateCachedRowCounts();
            InvalidateCachedRowsHeights();
            this.dataGridView = dataGridView;
            this.rowStates = new List<DataGridViewElementStates>();
            this.items = new RowArrayList(this);
        }

        /// <include file='doc\DataGridViewRowCollection.uex' path='docs/doc[@for="DataGridViewRowCollection.Count"]/*' />
        public int Count
        {
            get
            {
                return this.items.Count;
            }
        }

        internal bool IsCollectionChangedListenedTo
        {
            get
            {
                return (this.onCollectionChanged != null);
            }
        }

        /// <include file='doc\DataGridViewRowCollection.uex' path='docs/doc[@for="DataGridViewRowCollection.List"]/*' />
        protected ArrayList List
        {
            [
                SuppressMessage("Microsoft.Performance", "CA1817:DoNotCallPropertiesThatCloneValuesInLoops") // Illegitimate report.
            ]
            get
            {
                // All rows need to be unshared
                // Accessing List property should be avoided.
                int rowCount = this.Count;
                for (int rowIndex = 0; rowIndex < rowCount; rowIndex++)
                {
                    DataGridViewRow dataGridViewRow = this[rowIndex];
                }
                return this.items;
            }
        }

        internal ArrayList SharedList
        {
            get
            {
                return this.items;
            }
        }

        /// <include file='doc\DataGridViewRowCollection.uex' path='docs/doc[@for="DataGridViewRowCollection.SharedRow"]/*' />
        public DataGridViewRow SharedRow(int rowIndex)
        {
            return (DataGridViewRow) this.SharedList[rowIndex];
        }

        /// <include file='doc\DataGridViewRowCollection.uex' path='docs/doc[@for="DataGridViewRowCollection.DataGridView"]/*' />
        protected DataGridView DataGridView
        {
            get
            {
                return this.dataGridView;
            }
        }

        /// <include file='doc\DataGridViewRowCollection.uex' path='docs/doc[@for="DataGridViewRowCollection.this"]/*' />
        /// <devdoc>
        ///      Retrieves the DataGridViewRow with the specified index.
        /// </devdoc>
        public DataGridViewRow this[int index] 
        {
            get 
            {
                DataGridViewRow dataGridViewRow = SharedRow(index);
                if (dataGridViewRow.Index == -1)
                {
                    if (index == 0 && this.items.Count == 1)
                    {
                        // The only row present in the grid gets unshared.
                        // Simply update the index and return the current row without cloning it.
                        dataGridViewRow.IndexInternal = 0;
                        dataGridViewRow.StateInternal = SharedRowState(0);
                        if (this.DataGridView != null)
                        {
                            this.DataGridView.OnRowUnshared(dataGridViewRow);
                        }
                        return dataGridViewRow;
                    }

                    // unshare row
                    DataGridViewRow newDataGridViewRow = (DataGridViewRow) dataGridViewRow.Clone();
                    newDataGridViewRow.IndexInternal = index;
                    newDataGridViewRow.DataGridViewInternal = dataGridViewRow.DataGridView;
                    newDataGridViewRow.StateInternal = SharedRowState(index);
                    this.SharedList[index] = newDataGridViewRow;
                    int columnIndex = 0;
                    foreach (DataGridViewCell dataGridViewCell in newDataGridViewRow.Cells)
                    {
                        dataGridViewCell.DataGridViewInternal = dataGridViewRow.DataGridView;
                        dataGridViewCell.OwningRowInternal = newDataGridViewRow;
                        dataGridViewCell.OwningColumnInternal = this.DataGridView.Columns[columnIndex];
                        columnIndex++;
                    }
                    if (newDataGridViewRow.HasHeaderCell)
                    {
                        newDataGridViewRow.HeaderCell.DataGridViewInternal = dataGridViewRow.DataGridView;
                        newDataGridViewRow.HeaderCell.OwningRowInternal = newDataGridViewRow;
                    }
                    if (this.DataGridView != null)
                    {
                        this.DataGridView.OnRowUnshared(newDataGridViewRow);
                    }
                    return newDataGridViewRow;
                }
                else
                {
                    return dataGridViewRow;
                }
            }
        }

        /// <include file='doc\DataGridViewRowCollection.uex' path='docs/doc[@for="DataGridViewRowCollection.CollectionChanged"]/*' />
        public event CollectionChangeEventHandler CollectionChanged
        {
            add
            {
                this.onCollectionChanged += value;
            }
            remove
            {
                this.onCollectionChanged -= value;
            }
        }

        /// <include file='doc\DataGridViewRowCollection.uex' path='docs/doc[@for="DataGridViewRowCollection.Add"]/*' />
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public virtual int Add()
        {
            if (this.DataGridView.DataSource != null)
            {
                throw new InvalidOperationException(string.Format(SR.DataGridViewRowCollection_AddUnboundRow));
            }

            if (this.DataGridView.NoDimensionChangeAllowed)
            {
                throw new InvalidOperationException(string.Format(SR.DataGridView_ForbiddenOperationInEventHandler));
            }

            return AddInternal(false /*newRow*/, null);
        }

        internal int AddInternal(bool newRow, object[] values)
        {
            Debug.Assert(this.DataGridView != null);

            if (this.DataGridView.Columns.Count == 0)
            {
                throw new InvalidOperationException(string.Format(SR.DataGridViewRowCollection_NoColumns));
            }

            if (this.DataGridView.RowTemplate.Cells.Count > this.DataGridView.Columns.Count)
            {
                throw new InvalidOperationException(string.Format(SR.DataGridViewRowCollection_RowTemplateTooManyCells));
            }

            DataGridViewRow dataGridViewRow = this.DataGridView.RowTemplateClone;
            Debug.Assert(dataGridViewRow.Cells.Count == this.DataGridView.Columns.Count);
            if (newRow)
            {
                Debug.Assert(values == null);
                // Note that we allow the 'new' row to be frozen.
                Debug.Assert((dataGridViewRow.State & (DataGridViewElementStates.Selected | DataGridViewElementStates.Displayed)) == 0);
                // Make sure the 'new row' is visible even when the row template isn't
                dataGridViewRow.StateInternal = dataGridViewRow.State | DataGridViewElementStates.Visible;
                foreach (DataGridViewCell dataGridViewCell in dataGridViewRow.Cells)
                {
                    dataGridViewCell.Value = dataGridViewCell.DefaultNewRowValue;
                }
            }

            if (values != null)
            {
                dataGridViewRow.SetValuesInternal(values);
            }

            if (this.DataGridView.NewRowIndex != -1)
            {
                Debug.Assert(this.DataGridView.AllowUserToAddRowsInternal);
                Debug.Assert(this.DataGridView.NewRowIndex == this.Count - 1);
                int insertionIndex = this.Count - 1;
                Insert(insertionIndex, dataGridViewRow);
                return insertionIndex;
            }

            DataGridViewElementStates rowState = dataGridViewRow.State;
            this.DataGridView.OnAddingRow(dataGridViewRow, rowState, true /*checkFrozenState*/);   // will throw an exception if the addition is illegal

            dataGridViewRow.DataGridViewInternal = this.dataGridView;
            int columnIndex = 0;
            foreach (DataGridViewCell dataGridViewCell in dataGridViewRow.Cells)
            {
                dataGridViewCell.DataGridViewInternal = this.dataGridView;
                Debug.Assert(dataGridViewCell.OwningRow == dataGridViewRow);
                dataGridViewCell.OwningColumnInternal = this.DataGridView.Columns[columnIndex];
                columnIndex++;
            }

            if (dataGridViewRow.HasHeaderCell)
            {
                dataGridViewRow.HeaderCell.DataGridViewInternal = this.DataGridView;
                dataGridViewRow.HeaderCell.OwningRowInternal = dataGridViewRow;
            }

            int index = this.SharedList.Add(dataGridViewRow);
            Debug.Assert((rowState & (DataGridViewElementStates.Selected | DataGridViewElementStates.Displayed)) == 0);
            this.rowStates.Add(rowState);
#if DEBUG
            this.DataGridView.dataStoreAccessAllowed = false;
            this.cachedRowHeightsAccessAllowed = false;
            this.cachedRowCountsAccessAllowed = false;
#endif
            if (values != null || !RowIsSharable(index) || RowHasValueOrToolTipText(dataGridViewRow) || this.IsCollectionChangedListenedTo)
            {
                dataGridViewRow.IndexInternal = index;
                Debug.Assert(dataGridViewRow.State == SharedRowState(index));
            }
            OnCollectionChanged(new CollectionChangeEventArgs(CollectionChangeAction.Add, dataGridViewRow), index, 1);
            return index;
        }

        /// <include file='doc\DataGridViewRowCollection.uex' path='docs/doc[@for="DataGridViewRowCollection.Add1"]/*' />
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public virtual int Add(params object[] values)
        {
            Debug.Assert(this.DataGridView != null);
            if (values == null)
            {
                throw new ArgumentNullException(nameof(values));
            }

            /* Intentionally not being strict about this. We just take what we get.
            if (values.Length != this.DataGridView.Columns.Count)
            {
                // DataGridView_WrongValueCount=The array of cell values provided does not contain as many items as there are columns.
                throw new ArgumentException(string.Format(SR.DataGridView_WrongValueCount), "values");
            }*/

            if (this.DataGridView.VirtualMode)
            {
                throw new InvalidOperationException(string.Format(SR.DataGridView_InvalidOperationInVirtualMode));
            }

            if (this.DataGridView.DataSource != null)
            {
                throw new InvalidOperationException(string.Format(SR.DataGridViewRowCollection_AddUnboundRow));
            }

            if (this.DataGridView.NoDimensionChangeAllowed)
            {
                throw new InvalidOperationException(string.Format(SR.DataGridView_ForbiddenOperationInEventHandler));
            }

            /* Microsoft: Add once databinding is implemented
            foreach (DataGridViewColumn dataGridViewColumn in this.DataGridView.Columns)
            {
                if (dataGridViewColumn.DataBound)
                {
                    throw new InvalidOperationException(string.Format(SR.DataGridView_InvalidOperationInDataBoundMode));
                }
            }*/
            return AddInternal(false /*newRow*/, values);
        }

        /// <include file='doc\DataGridViewRowCollection.uex' path='docs/doc[@for="DataGridViewRowCollection.Add2"]/*' />
        /// <devdoc>
        /// <para>Adds a <see cref='System.Windows.Forms.DataGridViewRow'/> to this collection.</para>
        /// </devdoc>
        public virtual int Add(DataGridViewRow dataGridViewRow)
        {
            if (this.DataGridView.Columns.Count == 0)
            {
                throw new InvalidOperationException(string.Format(SR.DataGridViewRowCollection_NoColumns));
            }

            if (this.DataGridView.DataSource != null)
            {
                throw new InvalidOperationException(string.Format(SR.DataGridViewRowCollection_AddUnboundRow));
            }

            if (this.DataGridView.NoDimensionChangeAllowed)
            {
                throw new InvalidOperationException(string.Format(SR.DataGridView_ForbiddenOperationInEventHandler));
            }

            return AddInternal(dataGridViewRow);
        }

        /// <include file='doc\DataGridViewRowCollection.uex' path='docs/doc[@for="DataGridViewRowCollection.Add3"]/*' />
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public virtual int Add(int count)
        {
            Debug.Assert(this.DataGridView != null);

            if (count <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(count), string.Format(SR.DataGridViewRowCollection_CountOutOfRange));
            }

            if (this.DataGridView.Columns.Count == 0)
            {
                throw new InvalidOperationException(string.Format(SR.DataGridViewRowCollection_NoColumns));
            }

            if (this.DataGridView.DataSource != null)
            {
                throw new InvalidOperationException(string.Format(SR.DataGridViewRowCollection_AddUnboundRow));
            }

            if (this.DataGridView.NoDimensionChangeAllowed)
            {
                throw new InvalidOperationException(string.Format(SR.DataGridView_ForbiddenOperationInEventHandler));
            }

            if (this.DataGridView.RowTemplate.Cells.Count > this.DataGridView.Columns.Count)
            {
                throw new InvalidOperationException(string.Format(SR.DataGridViewRowCollection_RowTemplateTooManyCells));
            }

            DataGridViewRow rowTemplate = this.DataGridView.RowTemplateClone;
            Debug.Assert(rowTemplate.Cells.Count == this.DataGridView.Columns.Count);
            DataGridViewElementStates rowTemplateState = rowTemplate.State;
            Debug.Assert((rowTemplateState & (DataGridViewElementStates.Selected | DataGridViewElementStates.Displayed)) == 0);
            rowTemplate.DataGridViewInternal = this.dataGridView;
            int columnIndex = 0;
            foreach (DataGridViewCell dataGridViewCell in rowTemplate.Cells)
            {
                dataGridViewCell.DataGridViewInternal = this.dataGridView;
                Debug.Assert(dataGridViewCell.OwningRow == rowTemplate);
                dataGridViewCell.OwningColumnInternal = this.DataGridView.Columns[columnIndex];
                columnIndex++;
            }
            if (rowTemplate.HasHeaderCell)
            {
                rowTemplate.HeaderCell.DataGridViewInternal = this.dataGridView;
                rowTemplate.HeaderCell.OwningRowInternal = rowTemplate;
            }

            if (this.DataGridView.NewRowIndex != -1)
            {
                Debug.Assert(this.DataGridView.AllowUserToAddRowsInternal);
                Debug.Assert(this.DataGridView.NewRowIndex == this.Count - 1);
                int insertionIndex = this.Count - 1;
                InsertCopiesPrivate(rowTemplate, rowTemplateState, insertionIndex, count);
                return insertionIndex + count - 1;
            }

            return AddCopiesPrivate(rowTemplate, rowTemplateState, count);
        }

        internal int AddInternal(DataGridViewRow dataGridViewRow)
        {
            Debug.Assert(this.DataGridView != null);

            if (dataGridViewRow == null)
            {
                throw new ArgumentNullException(nameof(dataGridViewRow));
            }
            if (dataGridViewRow.DataGridView != null)
            {
                throw new InvalidOperationException(string.Format(SR.DataGridView_RowAlreadyBelongsToDataGridView));
            }
            if (this.DataGridView.Columns.Count == 0)
            {
                throw new InvalidOperationException(string.Format(SR.DataGridViewRowCollection_NoColumns));
            }
            if (dataGridViewRow.Cells.Count > this.DataGridView.Columns.Count)
            {
                throw new ArgumentException(string.Format(SR.DataGridViewRowCollection_TooManyCells), "dataGridViewRow");
            }

            if (dataGridViewRow.Selected)
            {
                throw new InvalidOperationException(string.Format(SR.DataGridViewRowCollection_CannotAddOrInsertSelectedRow));
            }

            if (this.DataGridView.NewRowIndex != -1)
            {
                Debug.Assert(this.DataGridView.AllowUserToAddRowsInternal);
                Debug.Assert(this.DataGridView.NewRowIndex == this.Count - 1);
                int insertionIndex = this.Count - 1;
                InsertInternal(insertionIndex, dataGridViewRow);
                return insertionIndex;
            }

            this.DataGridView.CompleteCellsCollection(dataGridViewRow);
            Debug.Assert(dataGridViewRow.Cells.Count == this.DataGridView.Columns.Count);
            this.DataGridView.OnAddingRow(dataGridViewRow, dataGridViewRow.State, true /*checkFrozenState*/);   // will throw an exception if the addition is illegal

            int columnIndex = 0;
            foreach (DataGridViewCell dataGridViewCell in dataGridViewRow.Cells)
            {
                dataGridViewCell.DataGridViewInternal = this.dataGridView;
                Debug.Assert(dataGridViewCell.OwningRow == dataGridViewRow);
                if (dataGridViewCell.ColumnIndex == -1)
                {
                    dataGridViewCell.OwningColumnInternal = this.DataGridView.Columns[columnIndex];
                }
                columnIndex++;
            }

            if (dataGridViewRow.HasHeaderCell)
            {
                dataGridViewRow.HeaderCell.DataGridViewInternal = this.DataGridView;
                dataGridViewRow.HeaderCell.OwningRowInternal = dataGridViewRow;
            }

            int index = this.SharedList.Add(dataGridViewRow);
            Debug.Assert((dataGridViewRow.State & (DataGridViewElementStates.Selected | DataGridViewElementStates.Displayed)) == 0);
            this.rowStates.Add(dataGridViewRow.State);
            Debug.Assert(this.rowStates.Count == this.SharedList.Count);
#if DEBUG
            this.DataGridView.dataStoreAccessAllowed = false;
            this.cachedRowHeightsAccessAllowed = false;
            this.cachedRowCountsAccessAllowed = false;
#endif

            dataGridViewRow.DataGridViewInternal = this.dataGridView;
            if (!RowIsSharable(index) || RowHasValueOrToolTipText(dataGridViewRow) || this.IsCollectionChangedListenedTo)
            {
                dataGridViewRow.IndexInternal = index;
                Debug.Assert(dataGridViewRow.State == SharedRowState(index));
            }
            OnCollectionChanged(new CollectionChangeEventArgs(CollectionChangeAction.Add, dataGridViewRow), index, 1);
            return index;
        }

        /// <include file='doc\DataGridViewRowCollection.uex' path='docs/doc[@for="DataGridViewRowCollection.AddCopy2"]/*' />
        public virtual int AddCopy(int indexSource)
        {
            if (this.DataGridView.DataSource != null)
            {
                throw new InvalidOperationException(string.Format(SR.DataGridViewRowCollection_AddUnboundRow));
            }

            if (this.DataGridView.NoDimensionChangeAllowed)
            {
                throw new InvalidOperationException(string.Format(SR.DataGridView_ForbiddenOperationInEventHandler));
            }

            return AddCopyInternal(indexSource, DataGridViewElementStates.None, DataGridViewElementStates.Selected | DataGridViewElementStates.Displayed, false /*newRow*/);
        }

        internal int AddCopyInternal(int indexSource, DataGridViewElementStates dgvesAdd, DataGridViewElementStates dgvesRemove, bool newRow)
        {
            Debug.Assert(this.DataGridView != null);

            if (this.DataGridView.NewRowIndex != -1)
            {
                Debug.Assert(this.DataGridView.AllowUserToAddRowsInternal);
                Debug.Assert(this.DataGridView.NewRowIndex == this.Count - 1);
                Debug.Assert(!newRow);
                int insertionIndex = this.Count - 1;
                InsertCopy(indexSource, insertionIndex);
                return insertionIndex;
            }

            if (indexSource < 0 || indexSource >= this.Count)
            {
                throw new ArgumentOutOfRangeException(nameof(indexSource), string.Format(SR.DataGridViewRowCollection_IndexSourceOutOfRange));
            }

            int index;
            DataGridViewRow rowTemplate = SharedRow(indexSource);
            if (rowTemplate.Index == -1 && !this.IsCollectionChangedListenedTo && !newRow)
            {
                Debug.Assert(this.DataGridView != null);
                DataGridViewElementStates rowState = this.rowStates[indexSource] & ~dgvesRemove;
                rowState |= dgvesAdd;
                this.DataGridView.OnAddingRow(rowTemplate, rowState, true /*checkFrozenState*/);   // will throw an exception if the addition is illegal

                index = this.SharedList.Add(rowTemplate);
                this.rowStates.Add(rowState);
#if DEBUG
                this.DataGridView.dataStoreAccessAllowed = false;
                this.cachedRowHeightsAccessAllowed = false;
                this.cachedRowCountsAccessAllowed = false;
#endif
                OnCollectionChanged(new CollectionChangeEventArgs(CollectionChangeAction.Add, rowTemplate), index, 1);
                return index;
            }
            else
            {
                index = AddDuplicateRow(rowTemplate, newRow);
                if (!RowIsSharable(index) || RowHasValueOrToolTipText(SharedRow(index)) || this.IsCollectionChangedListenedTo)
                {
                    UnshareRow(index);
                }
                OnCollectionChanged(new CollectionChangeEventArgs(CollectionChangeAction.Add, SharedRow(index)), index, 1);
                return index;
            }
        }

        /// <include file='doc\DataGridViewRowCollection.uex' path='docs/doc[@for="DataGridViewRowCollection.AddCopies2"]/*' />
        public virtual int AddCopies(int indexSource, int count)
        {
            if (this.DataGridView.DataSource != null)
            {
                throw new InvalidOperationException(string.Format(SR.DataGridViewRowCollection_AddUnboundRow));
            }

            if (this.DataGridView.NoDimensionChangeAllowed)
            {
                throw new InvalidOperationException(string.Format(SR.DataGridView_ForbiddenOperationInEventHandler));
            }

            return AddCopiesInternal(indexSource, count);
        }

        internal int AddCopiesInternal(int indexSource, int count)
        {
            if (this.DataGridView.NewRowIndex != -1)
            {
                Debug.Assert(this.DataGridView.AllowUserToAddRowsInternal);
                Debug.Assert(this.DataGridView.NewRowIndex == this.Count - 1);
                int insertionIndex = this.Count - 1;
                InsertCopiesPrivate(indexSource, insertionIndex, count);
                return insertionIndex + count - 1;
            }

            return AddCopiesInternal(indexSource, count, DataGridViewElementStates.None, DataGridViewElementStates.Selected | DataGridViewElementStates.Displayed);
        }

        internal int AddCopiesInternal(int indexSource, int count, DataGridViewElementStates dgvesAdd, DataGridViewElementStates dgvesRemove)
        {
            if (indexSource < 0 || this.Count <= indexSource)
            {
                throw new ArgumentOutOfRangeException(nameof(indexSource), string.Format(SR.DataGridViewRowCollection_IndexSourceOutOfRange));
            }

            if (count <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(count), string.Format(SR.DataGridViewRowCollection_CountOutOfRange));
            }

            DataGridViewElementStates rowTemplateState = this.rowStates[indexSource] & ~dgvesRemove;
            rowTemplateState |= dgvesAdd;

            return AddCopiesPrivate(SharedRow(indexSource), rowTemplateState, count);
        }

        private int AddCopiesPrivate(DataGridViewRow rowTemplate, DataGridViewElementStates rowTemplateState, int count)
        {
            int index, indexStart = this.items.Count;
            if (rowTemplate.Index == -1)
            {
                this.DataGridView.OnAddingRow(rowTemplate, rowTemplateState, true /*checkFrozenState*/);   // Done once only, continue to check if this is OK - will throw an exception if the addition is illegal.
                for (int i = 0; i < count - 1; i++)
                {
                    this.SharedList.Add(rowTemplate);
                    this.rowStates.Add(rowTemplateState);
                }
                index = this.SharedList.Add(rowTemplate);
                this.rowStates.Add(rowTemplateState);
#if DEBUG
                this.DataGridView.dataStoreAccessAllowed = false;
                this.cachedRowHeightsAccessAllowed = false;
                this.cachedRowCountsAccessAllowed = false;
#endif
                this.DataGridView.OnAddedRow_PreNotification(index);   // Only calling this once instead of 'count' times. Continue to check if this is OK.
                OnCollectionChanged(new CollectionChangeEventArgs(CollectionChangeAction.Refresh, null), indexStart, count);
                for (int i = 0; i < count; i++)
                {
                    this.DataGridView.OnAddedRow_PostNotification(index - (count - 1) + i);
                }
                return index;
            }
            else
            {
                index = AddDuplicateRow(rowTemplate, false /*newRow*/);
                if (count > 1)
                {
                    this.DataGridView.OnAddedRow_PreNotification(index);
                    if (RowIsSharable(index))
                    {
                        DataGridViewRow rowTemplate2 = SharedRow(index);
                        this.DataGridView.OnAddingRow(rowTemplate2, rowTemplateState, true /*checkFrozenState*/);   // done only once, continue to check if this is OK - will throw an exception if the addition is illegal
                        for (int i = 1; i < count - 1; i++)
                        {
                            this.SharedList.Add(rowTemplate2);
                            this.rowStates.Add(rowTemplateState);
                        }
                        index = this.SharedList.Add(rowTemplate2);
                        this.rowStates.Add(rowTemplateState);
#if DEBUG
                        this.DataGridView.dataStoreAccessAllowed = false;
                        this.cachedRowHeightsAccessAllowed = false;
                        this.cachedRowCountsAccessAllowed = false;
#endif
                        this.DataGridView.OnAddedRow_PreNotification(index);   // Only calling this once instead of 'count-1' times. Continue to check if this is OK.
                    }
                    else
                    {
                        UnshareRow(index);
                        for (int i = 1; i < count; i++)
                        {
                            index = AddDuplicateRow(rowTemplate, false /*newRow*/);
                            UnshareRow(index);
                            this.DataGridView.OnAddedRow_PreNotification(index);
                        }
                    }
                    OnCollectionChanged(new CollectionChangeEventArgs(CollectionChangeAction.Refresh, null), indexStart, count);
                    for (int i = 0; i < count; i++)
                    {
                        this.DataGridView.OnAddedRow_PostNotification(index - (count - 1) + i);
                    }
                    return index;
                }
                else
                {
                    if (this.IsCollectionChangedListenedTo)
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
            Debug.Assert(this.DataGridView != null);

            DataGridViewRow dataGridViewRow = (DataGridViewRow) rowTemplate.Clone();
            dataGridViewRow.StateInternal = DataGridViewElementStates.None;
            dataGridViewRow.DataGridViewInternal = this.dataGridView;
            DataGridViewCellCollection dgvcc = dataGridViewRow.Cells;
            int columnIndex = 0;
            foreach (DataGridViewCell dataGridViewCell in dgvcc)
            {
                if (newRow)
                {
                    dataGridViewCell.Value = dataGridViewCell.DefaultNewRowValue;
                }
                dataGridViewCell.DataGridViewInternal = this.dataGridView;
                dataGridViewCell.OwningColumnInternal = this.DataGridView.Columns[columnIndex];
                columnIndex++;
            }
            DataGridViewElementStates rowState = rowTemplate.State & ~(DataGridViewElementStates.Selected | DataGridViewElementStates.Displayed);
            if (dataGridViewRow.HasHeaderCell)
            {
                dataGridViewRow.HeaderCell.DataGridViewInternal = this.dataGridView;
                dataGridViewRow.HeaderCell.OwningRowInternal = dataGridViewRow;
            }

            this.DataGridView.OnAddingRow(dataGridViewRow, rowState, true /*checkFrozenState*/);   // will throw an exception if the addition is illegal

#if DEBUG
            this.DataGridView.dataStoreAccessAllowed = false;
            this.cachedRowHeightsAccessAllowed = false;
            this.cachedRowCountsAccessAllowed = false;
#endif
            Debug.Assert(dataGridViewRow.Index == -1);
            this.rowStates.Add(rowState);
            return this.SharedList.Add(dataGridViewRow);
        }
        
        /// <include file='doc\DataGridViewRowCollection.uex' path='docs/doc[@for="DataGridViewRowCollection.AddRange"]/*' />
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public virtual void AddRange(params DataGridViewRow[] dataGridViewRows)
        {
            if (dataGridViewRows == null)
            {
                throw new ArgumentNullException(nameof(dataGridViewRows));
            }

            Debug.Assert(this.DataGridView != null);

            if (this.DataGridView.NoDimensionChangeAllowed)
            {
                throw new InvalidOperationException(string.Format(SR.DataGridView_ForbiddenOperationInEventHandler));
            }

            if (this.DataGridView.DataSource != null)
            {
                throw new InvalidOperationException(string.Format(SR.DataGridViewRowCollection_AddUnboundRow));
            }

            if (this.DataGridView.NewRowIndex != -1)
            {
                Debug.Assert(this.DataGridView.AllowUserToAddRowsInternal);
                Debug.Assert(this.DataGridView.NewRowIndex == this.Count - 1);
                InsertRange(this.Count - 1, dataGridViewRows);
                return;
            }

            if (this.DataGridView.Columns.Count == 0)
            {
                throw new InvalidOperationException(string.Format(SR.DataGridViewRowCollection_NoColumns));
            }

            int indexStart = this.items.Count;

            // OnAddingRows checks for Selected flag of each row and their dimension.
            this.DataGridView.OnAddingRows(dataGridViewRows, true /*checkFrozenStates*/);   // will throw an exception if the addition is illegal

            foreach(DataGridViewRow dataGridViewRow in dataGridViewRows) 
            {
                Debug.Assert(dataGridViewRow.Cells.Count == this.DataGridView.Columns.Count);
                int columnIndex = 0;
                foreach (DataGridViewCell dataGridViewCell in dataGridViewRow.Cells)
                {
                    dataGridViewCell.DataGridViewInternal = this.dataGridView;
                    Debug.Assert(dataGridViewCell.OwningRow == dataGridViewRow);
                    dataGridViewCell.OwningColumnInternal = this.DataGridView.Columns[columnIndex];
                    columnIndex++;
                }

                if (dataGridViewRow.HasHeaderCell)
                {
                    dataGridViewRow.HeaderCell.DataGridViewInternal = this.dataGridView;
                    dataGridViewRow.HeaderCell.OwningRowInternal = dataGridViewRow;
                }

                int index = this.SharedList.Add(dataGridViewRow);
                Debug.Assert((dataGridViewRow.State & (DataGridViewElementStates.Selected | DataGridViewElementStates.Displayed)) == 0);
                this.rowStates.Add(dataGridViewRow.State);
#if DEBUG
                this.DataGridView.dataStoreAccessAllowed = false;
                this.cachedRowHeightsAccessAllowed = false;
                this.cachedRowCountsAccessAllowed = false;
#endif
                dataGridViewRow.IndexInternal = index;
                Debug.Assert(dataGridViewRow.State == SharedRowState(index));
                dataGridViewRow.DataGridViewInternal = this.dataGridView;
            }
            Debug.Assert(this.rowStates.Count == this.SharedList.Count);

            this.DataGridView.OnAddedRows_PreNotification(dataGridViewRows);
            OnCollectionChanged(new CollectionChangeEventArgs(CollectionChangeAction.Refresh, null), indexStart, dataGridViewRows.Length);
            this.DataGridView.OnAddedRows_PostNotification(dataGridViewRows);
        }

        /// <include file='doc\DataGridViewRowCollection.uex' path='docs/doc[@for="DataGridViewRowCollection.Clear"]/*' />
        public virtual void Clear()
        {
            if (this.DataGridView.NoDimensionChangeAllowed)
            {
                throw new InvalidOperationException(string.Format(SR.DataGridView_ForbiddenOperationInEventHandler));
            }
            if (this.DataGridView.DataSource != null)
            {
                IBindingList list = this.DataGridView.DataConnection.List as IBindingList;
                if (list != null && list.AllowRemove && list.SupportsChangeNotification)
                {
                    ((IList)list).Clear();
                }
                else
                {
                    throw new InvalidOperationException(string.Format(SR.DataGridViewRowCollection_CantClearRowCollectionWithWrongSource));
                }
            }
            else
            {
                ClearInternal(true);
            }
        }

        internal void ClearInternal(bool recreateNewRow)
        {
            int rowCount = this.items.Count;
            if (rowCount > 0)
            {
                this.DataGridView.OnClearingRows();
                
                for (int rowIndex = 0; rowIndex < rowCount; rowIndex++)
                {
                    SharedRow(rowIndex).DetachFromDataGridView();
                }

                this.SharedList.Clear();
                this.rowStates.Clear();
#if DEBUG
                this.DataGridView.dataStoreAccessAllowed = false;
                this.cachedRowHeightsAccessAllowed = false;
                this.cachedRowCountsAccessAllowed = false;
#endif
                OnCollectionChanged(new CollectionChangeEventArgs(CollectionChangeAction.Refresh, null), 0, rowCount, true, false, recreateNewRow, new Point(-1, -1));
            }
            else if (recreateNewRow && 
                     this.DataGridView.Columns.Count != 0 &&
                     this.DataGridView.AllowUserToAddRowsInternal &&
                     this.items.Count == 0) // accessing AllowUserToAddRowsInternal can trigger a nested call to ClearInternal. Rows count needs to be checked again.
            {
                this.DataGridView.AddNewRow(false);
            }
        }

        /// <include file='doc\DataGridViewRowCollection.uex' path='docs/doc[@for="DataGridViewRowCollection.Contains"]/*' />
        /// <devdoc>
        ///      Checks to see if a DataGridViewRow is contained in this collection.
        /// </devdoc>
        public virtual bool Contains(DataGridViewRow dataGridViewRow)
        {
            return this.items.IndexOf(dataGridViewRow) != -1;
        }

        /// <include file='doc\DataGridViewRowCollection.uex' path='docs/doc[@for="DataGridViewRowCollection.CopyTo"]/*' />
        public void CopyTo(DataGridViewRow[] array, int index)
        {
            this.items.CopyTo(array, index);
        }

        // returns the row collection index for the n'th visible row
        internal int DisplayIndexToRowIndex(int visibleRowIndex)
        {
            Debug.Assert(visibleRowIndex < GetRowCount(DataGridViewElementStates.Visible));

            // go row by row
            // the alternative would be to do a binary search using DataGridViewRowCollection::GetRowCount(...)
            // but that method also iterates thru each row so we would not gain much
            int indexOfCurrentVisibleRow = -1;
            for (int i = 0; i < this.Count; i++)
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

        /// <include file='doc\DataGridViewRowCollection.uex' path='docs/doc[@for="DataGridViewRowCollection.GetFirstRow"]/*' />
        public int GetFirstRow(DataGridViewElementStates includeFilter)
        {
            if ((includeFilter & ~(DataGridViewElementStates.Displayed | DataGridViewElementStates.Frozen | DataGridViewElementStates.Resizable |
                DataGridViewElementStates.ReadOnly | DataGridViewElementStates.Selected | DataGridViewElementStates.Visible)) != 0)
            {
                throw new ArgumentException(string.Format(SR.DataGridView_InvalidDataGridViewElementStateCombination, "includeFilter"));
            }
#if DEBUG
            Debug.Assert(this.cachedRowCountsAccessAllowed);
#endif
            switch (includeFilter)
            {
                case DataGridViewElementStates.Visible:
                    if (this.rowCountsVisible == 0)
                    {
                        return -1;
                    }
                    break;
                case DataGridViewElementStates.Visible | DataGridViewElementStates.Frozen:
                    if (this.rowCountsVisibleFrozen == 0)
                    {
                        return -1;
                    }
                    break;
                case DataGridViewElementStates.Visible | DataGridViewElementStates.Selected:
                    if (this.rowCountsVisibleSelected == 0)
                    {
                        return -1;
                    }
                    break;
            }

            int index = 0;
            while (index < this.items.Count && !((GetRowState(index) & includeFilter) == includeFilter))
            {
                index++;
            }
            return (index < this.items.Count) ? index : -1;
        }

        /// <include file='doc\DataGridViewRowCollection.uex' path='docs/doc[@for="DataGridViewRowCollection.GetFirstRow2"]/*' />
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
                throw new ArgumentException(string.Format(SR.DataGridView_InvalidDataGridViewElementStateCombination, "includeFilter"));
            }
            if ((excludeFilter & ~(DataGridViewElementStates.Displayed | DataGridViewElementStates.Frozen | DataGridViewElementStates.Resizable |
                DataGridViewElementStates.ReadOnly | DataGridViewElementStates.Selected | DataGridViewElementStates.Visible)) != 0)
            {
                throw new ArgumentException(string.Format(SR.DataGridView_InvalidDataGridViewElementStateCombination, "excludeFilter"));
            }
#if DEBUG
            Debug.Assert(this.cachedRowCountsAccessAllowed);
#endif
            switch (includeFilter)
            {
                case DataGridViewElementStates.Visible:
                    if (this.rowCountsVisible == 0)
                    {
                        return -1;
                    }
                    break;
                case DataGridViewElementStates.Visible | DataGridViewElementStates.Frozen:
                    if (this.rowCountsVisibleFrozen == 0)
                    {
                        return -1;
                    }
                    break;
                case DataGridViewElementStates.Visible | DataGridViewElementStates.Selected:
                    if (this.rowCountsVisibleSelected == 0)
                    {
                        return -1;
                    }
                    break;
            }

            int index = 0;
            while (index < this.items.Count && (!((GetRowState(index) & includeFilter) == includeFilter) || !((GetRowState(index) & excludeFilter) == 0)))
            {
                index++;
            }
            return (index < this.items.Count) ? index : -1;
        }

        /// <include file='doc\DataGridViewRowCollection.uex' path='docs/doc[@for="DataGridViewRowCollection.GetLastRow"]/*' />
        public int GetLastRow(DataGridViewElementStates includeFilter)
        {
            if ((includeFilter & ~(DataGridViewElementStates.Displayed | DataGridViewElementStates.Frozen | DataGridViewElementStates.Resizable |
                DataGridViewElementStates.ReadOnly | DataGridViewElementStates.Selected | DataGridViewElementStates.Visible)) != 0)
            {
                throw new ArgumentException(string.Format(SR.DataGridView_InvalidDataGridViewElementStateCombination, "includeFilter"));
            }
#if DEBUG
            Debug.Assert(this.cachedRowCountsAccessAllowed);
#endif
            switch (includeFilter)
            {
                case DataGridViewElementStates.Visible:
                    if (this.rowCountsVisible == 0)
                    {
                        return -1;
                    }
                    break;
                case DataGridViewElementStates.Visible | DataGridViewElementStates.Frozen:
                    if (this.rowCountsVisibleFrozen == 0)
                    {
                        return -1;
                    }
                    break;
                case DataGridViewElementStates.Visible | DataGridViewElementStates.Selected:
                    if (this.rowCountsVisibleSelected == 0)
                    {
                        return -1;
                    }
                    break;
            }

            int index = this.items.Count - 1;
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

        /// <include file='doc\DataGridViewRowCollection.uex' path='docs/doc[@for="DataGridViewRowCollection.GetNextRow"]/*' />
        public int GetNextRow(int indexStart, DataGridViewElementStates includeFilter)
        {
            if ((includeFilter & ~(DataGridViewElementStates.Displayed | DataGridViewElementStates.Frozen | DataGridViewElementStates.Resizable |
                DataGridViewElementStates.ReadOnly | DataGridViewElementStates.Selected | DataGridViewElementStates.Visible)) != 0)
            {
                throw new ArgumentException(string.Format(SR.DataGridView_InvalidDataGridViewElementStateCombination, "includeFilter"));
            }
            if (indexStart < -1)
            {
                throw new ArgumentOutOfRangeException(nameof(indexStart), string.Format(SR.InvalidLowBoundArgumentEx, "indexStart", (indexStart).ToString(CultureInfo.CurrentCulture), (-1).ToString(CultureInfo.CurrentCulture)));
            }

            int index = indexStart + 1;
            while (index < this.items.Count && !((GetRowState(index) & includeFilter) == includeFilter))
            {
                index++;
            }
            return (index < this.items.Count) ? index : -1;
        }

        /// <include file='doc\DataGridViewRowCollection.uex' path='docs/doc[@for="DataGridViewRowCollection.GetNextRow2"]/*' />
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
                throw new ArgumentException(string.Format(SR.DataGridView_InvalidDataGridViewElementStateCombination, "includeFilter"));
            }
            if ((excludeFilter & ~(DataGridViewElementStates.Displayed | DataGridViewElementStates.Frozen | DataGridViewElementStates.Resizable |
                DataGridViewElementStates.ReadOnly | DataGridViewElementStates.Selected | DataGridViewElementStates.Visible)) != 0)
            {
                throw new ArgumentException(string.Format(SR.DataGridView_InvalidDataGridViewElementStateCombination, "excludeFilter"));
            }
            if (indexStart < -1)
            {
                throw new ArgumentOutOfRangeException(nameof(indexStart), string.Format(SR.InvalidLowBoundArgumentEx, "indexStart", (indexStart).ToString(CultureInfo.CurrentCulture), (-1).ToString(CultureInfo.CurrentCulture)));
            }

            int index = indexStart + 1;
            while (index < this.items.Count && (!((GetRowState(index) & includeFilter) == includeFilter) || !((GetRowState(index) & excludeFilter) == 0)))
            {
                index++;
            }
            return (index < this.items.Count) ? index : -1;
        }

        /// <include file='doc\DataGridViewRowCollection.uex' path='docs/doc[@for="DataGridViewRowCollection.GetPreviousRow"]/*' />
        public int GetPreviousRow(int indexStart, DataGridViewElementStates includeFilter)
        {
            if ((includeFilter & ~(DataGridViewElementStates.Displayed | DataGridViewElementStates.Frozen | DataGridViewElementStates.Resizable |
                DataGridViewElementStates.ReadOnly | DataGridViewElementStates.Selected | DataGridViewElementStates.Visible)) != 0)
            {
                throw new ArgumentException(string.Format(SR.DataGridView_InvalidDataGridViewElementStateCombination, "includeFilter"));
            }
            if (indexStart > this.items.Count)
            {
                throw new ArgumentOutOfRangeException(nameof(indexStart), string.Format(SR.InvalidHighBoundArgumentEx, "indexStart", (indexStart).ToString(CultureInfo.CurrentCulture), (this.items.Count).ToString(CultureInfo.CurrentCulture)));
            }

            int index = indexStart - 1;
            while (index >= 0 && !((GetRowState(index) & includeFilter) == includeFilter))
            {
                index--;
            }
            return (index >= 0) ? index : -1;
        }

        /// <include file='doc\DataGridViewRowCollection.uex' path='docs/doc[@for="DataGridViewRowCollection.GetPreviousRow2"]/*' />
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
                throw new ArgumentException(string.Format(SR.DataGridView_InvalidDataGridViewElementStateCombination, "includeFilter"));
            }
            if ((excludeFilter & ~(DataGridViewElementStates.Displayed | DataGridViewElementStates.Frozen | DataGridViewElementStates.Resizable |
                DataGridViewElementStates.ReadOnly | DataGridViewElementStates.Selected | DataGridViewElementStates.Visible)) != 0)
            {
                throw new ArgumentException(string.Format(SR.DataGridView_InvalidDataGridViewElementStateCombination, "excludeFilter"));
            }
            if (indexStart > this.items.Count)
            {
                throw new ArgumentOutOfRangeException(nameof(indexStart), string.Format(SR.InvalidHighBoundArgumentEx, "indexStart", (indexStart).ToString(CultureInfo.CurrentCulture), (this.items.Count).ToString(CultureInfo.CurrentCulture)));
            }

            int index = indexStart - 1;
            while (index >= 0 && (!((GetRowState(index) & includeFilter) == includeFilter) || !((GetRowState(index) & excludeFilter) == 0)))
            {
                index--;
            }
            return (index >= 0) ? index : -1;
        }

        /// <include file='doc\DataGridViewRowCollection.uex' path='docs/doc[@for="DataGridViewRowCollection.GetRowCount"]/*' />
        public int GetRowCount(DataGridViewElementStates includeFilter)
        {
            if ((includeFilter & ~(DataGridViewElementStates.Displayed | DataGridViewElementStates.Frozen | DataGridViewElementStates.Resizable |
                DataGridViewElementStates.ReadOnly | DataGridViewElementStates.Selected | DataGridViewElementStates.Visible)) != 0)
            {
                throw new ArgumentException(string.Format(SR.DataGridView_InvalidDataGridViewElementStateCombination, "includeFilter"));
            }
#if DEBUG
            Debug.Assert(this.cachedRowCountsAccessAllowed);
#endif
            // cache returned value and reuse it as long as none
            // of the row's state has changed.
            switch (includeFilter)
            {
                case DataGridViewElementStates.Visible:
                    if (this.rowCountsVisible != -1)
                    {
                        return this.rowCountsVisible;
                    }
                    break;
                case DataGridViewElementStates.Visible | DataGridViewElementStates.Frozen:
                    if (this.rowCountsVisibleFrozen != -1)
                    {
                        return this.rowCountsVisibleFrozen;
                    }
                    break;
                case DataGridViewElementStates.Visible | DataGridViewElementStates.Selected:
                    if (this.rowCountsVisibleSelected != -1)
                    {
                        return this.rowCountsVisibleSelected;
                    }
                    break;
            }

            int rowCount = 0;
            for (int rowIndex = 0; rowIndex < this.items.Count; rowIndex++)
            {
                if ((GetRowState(rowIndex) & includeFilter) == includeFilter)
                {
                    rowCount++;
                }
            }

            switch (includeFilter)
            {
                case DataGridViewElementStates.Visible:
                    this.rowCountsVisible = rowCount;
                    break;
                case DataGridViewElementStates.Visible | DataGridViewElementStates.Frozen:
                    this.rowCountsVisibleFrozen = rowCount;
                    break;
                case DataGridViewElementStates.Visible | DataGridViewElementStates.Selected:
                    this.rowCountsVisibleSelected = rowCount;
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

        /// <include file='doc\DataGridViewRowCollection.uex' path='docs/doc[@for="DataGridViewRowCollection.GetRowsHeight"]/*' />
        public int GetRowsHeight(DataGridViewElementStates includeFilter)
        {
            if ((includeFilter & ~(DataGridViewElementStates.Displayed | DataGridViewElementStates.Frozen | DataGridViewElementStates.Resizable |
                DataGridViewElementStates.ReadOnly | DataGridViewElementStates.Selected | DataGridViewElementStates.Visible)) != 0)
            {
                throw new ArgumentException(string.Format(SR.DataGridView_InvalidDataGridViewElementStateCombination, "includeFilter"));
            }
#if DEBUG
            Debug.Assert(this.cachedRowHeightsAccessAllowed);
#endif
            // cache returned value and reuse it as long as none
            // of the row's state/thickness has changed.
            switch (includeFilter)
            {
                case DataGridViewElementStates.Visible:
                    if (this.rowsHeightVisible != -1)
                    {
                        return this.rowsHeightVisible;
                    }
                    break;
                case DataGridViewElementStates.Visible | DataGridViewElementStates.Frozen:
                    if (this.rowsHeightVisibleFrozen != -1)
                    {
                        return this.rowsHeightVisibleFrozen;
                    }
                    break;
            }
            
            int rowsHeight = 0;
            for (int rowIndex = 0; rowIndex < this.items.Count; rowIndex++)
            {
                if ((GetRowState(rowIndex) & includeFilter) == includeFilter)
                {
                    rowsHeight += ((DataGridViewRow)this.items[rowIndex]).GetHeight(rowIndex);
                }
            }

            switch (includeFilter)
            {
                case DataGridViewElementStates.Visible:
                    this.rowsHeightVisible = rowsHeight;
                    break;
                case DataGridViewElementStates.Visible | DataGridViewElementStates.Frozen:
                    this.rowsHeightVisibleFrozen = rowsHeight;
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
                    rowsHeight += ((DataGridViewRow)this.items[rowIndex]).GetHeight(rowIndex);
                }
            }
            return rowsHeight;
        }

        // Checks if the cumulated row heights from fromRowIndex to toRowIndex-1 exceed heightLimit.
        private bool GetRowsHeightExceedLimit(DataGridViewElementStates includeFilter, int fromRowIndex, int toRowIndex, int heightLimit)
        {
            Debug.Assert(toRowIndex >= fromRowIndex);
            Debug.Assert(toRowIndex == this.items.Count || (GetRowState(toRowIndex) & includeFilter) == includeFilter);

            int rowsHeight = 0;
            for (int rowIndex = fromRowIndex; rowIndex < toRowIndex; rowIndex++)
            {
                if ((GetRowState(rowIndex) & includeFilter) == includeFilter)
                {
                    rowsHeight += ((DataGridViewRow)this.items[rowIndex]).GetHeight(rowIndex);
                    if (rowsHeight > heightLimit)
                    {
                        return true;
                    }
                }
            }
            return rowsHeight > heightLimit;
        }

        /// <include file='doc\DataGridViewRowCollection.uex' path='docs/doc[@for="DataGridViewRowCollection.GetRowState"]/*' />
        public virtual DataGridViewElementStates GetRowState(int rowIndex)
        {
            if (rowIndex < 0 || rowIndex >= this.items.Count)
            {
                throw new ArgumentOutOfRangeException(nameof(rowIndex), string.Format(SR.DataGridViewRowCollection_RowIndexOutOfRange));
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

        /// <include file='doc\DataGridViewRowCollection.uex' path='docs/doc[@for="DataGridViewRowCollection.IndexOf"]/*' />
        public int IndexOf(DataGridViewRow dataGridViewRow)
        {
            return this.items.IndexOf(dataGridViewRow);
        }

        /// <include file='doc\DataGridViewRowCollection.uex' path='docs/doc[@for="DataGridViewRowCollection.Insert1"]/*' />
        public virtual void Insert(int rowIndex, params object[] values)
        {
            Debug.Assert(this.DataGridView != null);
            
            if (values == null)
            {
                throw new ArgumentNullException(nameof(values));
            }

            /* Intentionally not being strict about this. We just take what we get.
            if (values.Length != this.DataGridView.Columns.Count)
            {
                // DataGridView_WrongValueCount=The array of cell values provided does not contain as many items as there are columns.
                throw new ArgumentException(string.Format(SR.DataGridView_WrongValueCount), "values");
            }*/

            if (this.DataGridView.VirtualMode)
            {
                throw new InvalidOperationException(string.Format(SR.DataGridView_InvalidOperationInVirtualMode));
            }

            if (this.DataGridView.DataSource != null)
            {
                throw new InvalidOperationException(string.Format(SR.DataGridViewRowCollection_AddUnboundRow));
            }

            DataGridViewRow dataGridViewRow = this.DataGridView.RowTemplateClone;
            dataGridViewRow.SetValuesInternal(values);
            Insert(rowIndex, dataGridViewRow);
        }

        /// <include file='doc\DataGridViewRowCollection.uex' path='docs/doc[@for="DataGridViewRowCollection.Insert2"]/*' />
        /// <devdoc>
        /// <para>Inserts a <see cref='System.Windows.Forms.DataGridViewRow'/> to this collection.</para>
        /// </devdoc>
        public virtual void Insert(int rowIndex, DataGridViewRow dataGridViewRow)
        {
            if (this.DataGridView.DataSource != null)
            {
                throw new InvalidOperationException(string.Format(SR.DataGridViewRowCollection_AddUnboundRow));
            }

            if (this.DataGridView.NoDimensionChangeAllowed)
            {
                throw new InvalidOperationException(string.Format(SR.DataGridView_ForbiddenOperationInEventHandler));
            }
            
            InsertInternal(rowIndex, dataGridViewRow);
        }

        /// <include file='doc\DataGridViewRowCollection.uex' path='docs/doc[@for="DataGridViewRowCollection.Insert3"]/*' />
        public virtual void Insert(int rowIndex, int count)
        {
            if (this.DataGridView.DataSource != null)
            {
                throw new InvalidOperationException(string.Format(SR.DataGridViewRowCollection_AddUnboundRow));
            }

            if (rowIndex < 0 || this.Count < rowIndex)
            {
                throw new ArgumentOutOfRangeException(nameof(rowIndex), string.Format(SR.DataGridViewRowCollection_IndexDestinationOutOfRange));
            }

            if (count <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(count), string.Format(SR.DataGridViewRowCollection_CountOutOfRange));
            }

            if (this.DataGridView.NoDimensionChangeAllowed)
            {
                throw new InvalidOperationException(string.Format(SR.DataGridView_ForbiddenOperationInEventHandler));
            }

            if (this.DataGridView.Columns.Count == 0)
            {
                throw new InvalidOperationException(string.Format(SR.DataGridViewRowCollection_NoColumns));
            }

            if (this.DataGridView.RowTemplate.Cells.Count > this.DataGridView.Columns.Count)
            {
                throw new InvalidOperationException(string.Format(SR.DataGridViewRowCollection_RowTemplateTooManyCells));
            }

            if (this.DataGridView.NewRowIndex != -1 && rowIndex == this.Count)
            {
                // Trying to insert after the 'new' row.
                Debug.Assert(this.DataGridView.AllowUserToAddRowsInternal);
                throw new InvalidOperationException(string.Format(SR.DataGridViewRowCollection_NoInsertionAfterNewRow));
            }

            DataGridViewRow rowTemplate = this.DataGridView.RowTemplateClone;
            Debug.Assert(rowTemplate.Cells.Count == this.DataGridView.Columns.Count);
            DataGridViewElementStates rowTemplateState = rowTemplate.State;
            Debug.Assert((rowTemplateState & (DataGridViewElementStates.Selected | DataGridViewElementStates.Displayed)) == 0);
            rowTemplate.DataGridViewInternal = this.dataGridView;
            int columnIndex = 0;
            foreach (DataGridViewCell dataGridViewCell in rowTemplate.Cells)
            {
                dataGridViewCell.DataGridViewInternal = this.dataGridView;
                Debug.Assert(dataGridViewCell.OwningRow == rowTemplate);
                dataGridViewCell.OwningColumnInternal = this.DataGridView.Columns[columnIndex];
                columnIndex++;
            }
            if (rowTemplate.HasHeaderCell)
            {
                rowTemplate.HeaderCell.DataGridViewInternal = this.dataGridView;
                rowTemplate.HeaderCell.OwningRowInternal = rowTemplate;
            }

            InsertCopiesPrivate(rowTemplate, rowTemplateState, rowIndex, count);
        }

        internal void InsertInternal(int rowIndex, DataGridViewRow dataGridViewRow)
        {
            Debug.Assert(this.DataGridView != null);
            if (rowIndex < 0 || this.Count < rowIndex)
            {
                throw new ArgumentOutOfRangeException(nameof(rowIndex), string.Format(SR.DataGridViewRowCollection_RowIndexOutOfRange));
            }

            if (dataGridViewRow == null)
            {
                throw new ArgumentNullException(nameof(dataGridViewRow));
            }

            if (dataGridViewRow.DataGridView != null)
            {
                throw new InvalidOperationException(string.Format(SR.DataGridView_RowAlreadyBelongsToDataGridView));
            }

            if (this.DataGridView.NewRowIndex != -1 && rowIndex == this.Count)
            {
                // Trying to insert after the 'new' row.
                Debug.Assert(this.DataGridView.AllowUserToAddRowsInternal);
                throw new InvalidOperationException(string.Format(SR.DataGridViewRowCollection_NoInsertionAfterNewRow));
            }

            if (this.DataGridView.Columns.Count == 0)
            {
                throw new InvalidOperationException(string.Format(SR.DataGridViewRowCollection_NoColumns));
            }

            if (dataGridViewRow.Cells.Count > this.DataGridView.Columns.Count)
            {
                throw new ArgumentException(string.Format(SR.DataGridViewRowCollection_TooManyCells), "dataGridViewRow");
            }

            if (dataGridViewRow.Selected)
            {
                throw new InvalidOperationException(string.Format(SR.DataGridViewRowCollection_CannotAddOrInsertSelectedRow));
            }

            InsertInternal(rowIndex, dataGridViewRow, false);
        }

        internal void InsertInternal(int rowIndex, DataGridViewRow dataGridViewRow, bool force)
        {
            Debug.Assert(this.DataGridView != null);
            Debug.Assert(rowIndex >= 0 && rowIndex <= this.Count);
            Debug.Assert(dataGridViewRow != null);
            Debug.Assert(dataGridViewRow.DataGridView == null);
            Debug.Assert(!this.DataGridView.NoDimensionChangeAllowed);
            Debug.Assert(this.DataGridView.NewRowIndex == -1 || rowIndex != this.Count);
            Debug.Assert(!dataGridViewRow.Selected);

            Point newCurrentCell = new Point(-1, -1);

            if (force)
            {
                if (this.DataGridView.Columns.Count == 0)
                {
                    throw new InvalidOperationException(string.Format(SR.DataGridViewRowCollection_NoColumns));
                }
                if (dataGridViewRow.Cells.Count > this.DataGridView.Columns.Count)
                {
                    throw new ArgumentException(string.Format(SR.DataGridViewRowCollection_TooManyCells), "dataGridViewRow");
                }
            }
            this.DataGridView.CompleteCellsCollection(dataGridViewRow);
            Debug.Assert(dataGridViewRow.Cells.Count == this.DataGridView.Columns.Count);
            this.DataGridView.OnInsertingRow(rowIndex, dataGridViewRow, dataGridViewRow.State, ref newCurrentCell, true, 1, force);   // will throw an exception if the insertion is illegal

            int columnIndex = 0;
            foreach (DataGridViewCell dataGridViewCell in dataGridViewRow.Cells)
            {
                dataGridViewCell.DataGridViewInternal = this.dataGridView;
                Debug.Assert(dataGridViewCell.OwningRow == dataGridViewRow);
                if (dataGridViewCell.ColumnIndex == -1)
                {
                    dataGridViewCell.OwningColumnInternal = this.DataGridView.Columns[columnIndex];
                }
                columnIndex++;
            }

            if (dataGridViewRow.HasHeaderCell)
            {
                dataGridViewRow.HeaderCell.DataGridViewInternal = this.DataGridView;
                dataGridViewRow.HeaderCell.OwningRowInternal = dataGridViewRow;
            }

            this.SharedList.Insert(rowIndex, dataGridViewRow);
            Debug.Assert((dataGridViewRow.State & (DataGridViewElementStates.Selected | DataGridViewElementStates.Displayed)) == 0);
            this.rowStates.Insert(rowIndex, dataGridViewRow.State);
            Debug.Assert(this.rowStates.Count == this.SharedList.Count);
#if DEBUG
            this.DataGridView.dataStoreAccessAllowed = false;
            this.cachedRowHeightsAccessAllowed = false;
            this.cachedRowCountsAccessAllowed = false;
#endif

            dataGridViewRow.DataGridViewInternal = this.dataGridView;
            if (!RowIsSharable(rowIndex) || RowHasValueOrToolTipText(dataGridViewRow) || this.IsCollectionChangedListenedTo)
            {
                dataGridViewRow.IndexInternal = rowIndex;
                Debug.Assert(dataGridViewRow.State == SharedRowState(rowIndex));
            }
            OnCollectionChanged(new CollectionChangeEventArgs(CollectionChangeAction.Add, dataGridViewRow), rowIndex, 1, false, true, false, newCurrentCell);
        }

        /// <include file='doc\DataGridViewRowCollection.uex' path='docs/doc[@for="DataGridViewRowCollection.InsertCopy"]/*' />
        public virtual void InsertCopy(int indexSource, int indexDestination)
        {
            InsertCopies(indexSource, indexDestination, 1);
        }

        /// <include file='doc\DataGridViewRowCollection.uex' path='docs/doc[@for="DataGridViewRowCollection.InsertCopies"]/*' />
        public virtual void InsertCopies(int indexSource, int indexDestination, int count)
        {
            if (this.DataGridView.DataSource != null)
            {
                throw new InvalidOperationException(string.Format(SR.DataGridViewRowCollection_AddUnboundRow));
            }

            if (this.DataGridView.NoDimensionChangeAllowed)
            {
                throw new InvalidOperationException(string.Format(SR.DataGridView_ForbiddenOperationInEventHandler));
            }

            InsertCopiesPrivate(indexSource, indexDestination, count);
        }

        private void InsertCopiesPrivate(int indexSource, int indexDestination, int count)
        {
            Debug.Assert(this.DataGridView != null);

            if (indexSource < 0 || this.Count <= indexSource)
            {
                throw new ArgumentOutOfRangeException(nameof(indexSource), string.Format(SR.DataGridViewRowCollection_IndexSourceOutOfRange));
            }

            if (indexDestination < 0 || this.Count < indexDestination)
            {
                throw new ArgumentOutOfRangeException(nameof(indexDestination), string.Format(SR.DataGridViewRowCollection_IndexDestinationOutOfRange));
            }

            if (count <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(count), string.Format(SR.DataGridViewRowCollection_CountOutOfRange));
            }

            if (this.DataGridView.NewRowIndex != -1 && indexDestination == this.Count)
            {
                // Trying to insert after the 'new' row.
                Debug.Assert(this.DataGridView.AllowUserToAddRowsInternal);
                throw new InvalidOperationException(string.Format(SR.DataGridViewRowCollection_NoInsertionAfterNewRow));
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
                    this.DataGridView.OnInsertingRow(indexDestination, rowTemplate, rowTemplateState, ref newCurrentCell, true, count, false /*force*/);
                    for (int i = 0; i < count; i++)
                    {
                        this.SharedList.Insert(indexDestination + i, rowTemplate);
                        this.rowStates.Insert(indexDestination + i, rowTemplateState);
                    }
#if DEBUG
                    this.DataGridView.dataStoreAccessAllowed = false;
                    this.cachedRowHeightsAccessAllowed = false;
                    this.cachedRowCountsAccessAllowed = false;
#endif
                    // Only calling this once instead of 'count' times. Continue to check if this is OK.
                    this.DataGridView.OnInsertedRow_PreNotification(indexDestination, count);
                    OnCollectionChanged(new CollectionChangeEventArgs(CollectionChangeAction.Refresh, null), indexDestination, count, false, true, false, newCurrentCell);
                    for (int i = 0; i < count; i++)
                    {
                        this.DataGridView.OnInsertedRow_PostNotification(indexDestination + i, newCurrentCell, i == count - 1);
                    }
                }
                else
                {
                    this.DataGridView.OnInsertingRow(indexDestination, rowTemplate, rowTemplateState, ref newCurrentCell, true, 1, false /*force*/); // will throw an exception if the insertion is illegal
                    this.SharedList.Insert(indexDestination, rowTemplate);
                    this.rowStates.Insert(indexDestination, rowTemplateState);
#if DEBUG
                    this.DataGridView.dataStoreAccessAllowed = false;
                    this.cachedRowHeightsAccessAllowed = false;
                    this.cachedRowCountsAccessAllowed = false;
#endif
                    OnCollectionChanged(new CollectionChangeEventArgs(CollectionChangeAction.Add, SharedRow(indexDestination)), indexDestination, count, false, true, false, newCurrentCell);
                }
            }
            else
            {
                // Sets this.DataGridView.dataStoreAccessAllowed to false
                InsertDuplicateRow(indexDestination, rowTemplate, true, ref newCurrentCell);
                Debug.Assert(this.rowStates.Count == this.SharedList.Count);
                if (count > 1)
                {
                    this.DataGridView.OnInsertedRow_PreNotification(indexDestination,  1);
                    if (RowIsSharable(indexDestination))
                    {
                        DataGridViewRow rowTemplate2 = SharedRow(indexDestination);
                        // Done once only, continue to check if this is OK - will throw an exception if the insertion is illegal.
                        this.DataGridView.OnInsertingRow(indexDestination + 1, rowTemplate2, rowTemplateState, ref newCurrentCell, false, count-1, false /*force*/);
                        for (int i = 1; i < count; i++)
                        {
                            this.SharedList.Insert(indexDestination + i, rowTemplate2);
                            this.rowStates.Insert(indexDestination + i, rowTemplateState);
                        }
#if DEBUG
                        this.DataGridView.dataStoreAccessAllowed = false;
                        this.cachedRowHeightsAccessAllowed = false;
                        this.cachedRowCountsAccessAllowed = false;
#endif
                        // Only calling this once instead of 'count-1' times. Continue to check if this is OK.
                        this.DataGridView.OnInsertedRow_PreNotification(indexDestination+1, count-1);
                        OnCollectionChanged(new CollectionChangeEventArgs(CollectionChangeAction.Refresh, null), indexDestination, count, false, true, false, newCurrentCell);
                    }
                    else
                    {
                        UnshareRow(indexDestination);
                        for (int i = 1; i < count; i++)
                        {
                            InsertDuplicateRow(indexDestination + i, rowTemplate, false, ref newCurrentCell);
                            Debug.Assert(this.rowStates.Count == this.SharedList.Count);
                            UnshareRow(indexDestination + i);
                            this.DataGridView.OnInsertedRow_PreNotification(indexDestination + i, 1);
                        }
                        OnCollectionChanged(new CollectionChangeEventArgs(CollectionChangeAction.Refresh, null), indexDestination, count, false, true, false, newCurrentCell);
                    }
                    for (int i = 0; i < count; i++)
                    {
                        this.DataGridView.OnInsertedRow_PostNotification(indexDestination + i, newCurrentCell, i == count - 1);
                    }
                }
                else
                {
                    if (this.IsCollectionChangedListenedTo)
                    {
                        UnshareRow(indexDestination);
                    }
                    OnCollectionChanged(new CollectionChangeEventArgs(CollectionChangeAction.Add, SharedRow(indexDestination)), indexDestination, 1, false, true, false, newCurrentCell);
                }
            }
        }

        private void InsertDuplicateRow(int indexDestination, DataGridViewRow rowTemplate, bool firstInsertion, ref Point newCurrentCell)
        {
            Debug.Assert(this.DataGridView != null);

            DataGridViewRow dataGridViewRow = (DataGridViewRow) rowTemplate.Clone();
            dataGridViewRow.StateInternal = DataGridViewElementStates.None;
            dataGridViewRow.DataGridViewInternal = this.dataGridView;
            DataGridViewCellCollection dgvcc = dataGridViewRow.Cells;
            int columnIndex = 0;
            foreach (DataGridViewCell dataGridViewCell in dgvcc)
            {
                dataGridViewCell.DataGridViewInternal = this.dataGridView;
                dataGridViewCell.OwningColumnInternal = this.DataGridView.Columns[columnIndex];
                columnIndex++;
            }
            DataGridViewElementStates rowState = rowTemplate.State & ~(DataGridViewElementStates.Selected | DataGridViewElementStates.Displayed);
            if (dataGridViewRow.HasHeaderCell)
            {
                dataGridViewRow.HeaderCell.DataGridViewInternal = this.dataGridView;
                dataGridViewRow.HeaderCell.OwningRowInternal = dataGridViewRow;
            }

            this.DataGridView.OnInsertingRow(indexDestination, dataGridViewRow, rowState, ref newCurrentCell, firstInsertion, 1, false /*force*/);   // will throw an exception if the insertion is illegal

            Debug.Assert(dataGridViewRow.Index == -1);
            this.SharedList.Insert(indexDestination, dataGridViewRow);
            this.rowStates.Insert(indexDestination, rowState);
            Debug.Assert(this.rowStates.Count == this.SharedList.Count);
#if DEBUG
            this.DataGridView.dataStoreAccessAllowed = false;
            this.cachedRowHeightsAccessAllowed = false;
            this.cachedRowCountsAccessAllowed = false;
#endif
        }

        /// <include file='doc\DataGridViewRowCollection.uex' path='docs/doc[@for="DataGridViewRowCollection.InsertRange"]/*' />
        /// <devdoc>
        /// <para>Inserts a range of <see cref='System.Windows.Forms.DataGridViewRow'/> to this collection.</para>
        /// </devdoc>
        public virtual void InsertRange(int rowIndex, params DataGridViewRow[] dataGridViewRows)
        {
            Debug.Assert(this.DataGridView != null);

            if (dataGridViewRows == null)
            {
                throw new ArgumentNullException(nameof(dataGridViewRows));
            }

            if (dataGridViewRows.Length == 1)
            {
                Insert(rowIndex, dataGridViewRows[0]);
                return;
            }

            if (rowIndex < 0 || rowIndex > this.Count)
            {
                throw new ArgumentOutOfRangeException(nameof(rowIndex), string.Format(SR.DataGridViewRowCollection_IndexDestinationOutOfRange));
            }

            if (this.DataGridView.NoDimensionChangeAllowed)
            {
                throw new InvalidOperationException(string.Format(SR.DataGridView_ForbiddenOperationInEventHandler));
            }

            if (this.DataGridView.NewRowIndex != -1 && rowIndex == this.Count)
            {
                // Trying to insert after the 'new' row.
                Debug.Assert(this.DataGridView.AllowUserToAddRowsInternal);
                throw new InvalidOperationException(string.Format(SR.DataGridViewRowCollection_NoInsertionAfterNewRow));
            }

            if (this.DataGridView.DataSource != null)
            {
                throw new InvalidOperationException(string.Format(SR.DataGridViewRowCollection_AddUnboundRow));
            }

            if (this.DataGridView.Columns.Count == 0)
            {
                throw new InvalidOperationException(string.Format(SR.DataGridViewRowCollection_NoColumns));
            }

            Point newCurrentCell = new Point(-1, -1);

            // OnInsertingRows checks for Selected flag of each row, among other things.
            this.DataGridView.OnInsertingRows(rowIndex, dataGridViewRows, ref newCurrentCell);   // will throw an exception if the insertion is illegal

            int rowIndexInserted = rowIndex;
            foreach (DataGridViewRow dataGridViewRow in dataGridViewRows)
            {
                Debug.Assert(dataGridViewRow.Cells.Count == this.DataGridView.Columns.Count);
                int columnIndex = 0;
                foreach (DataGridViewCell dataGridViewCell in dataGridViewRow.Cells)
                {
                    dataGridViewCell.DataGridViewInternal = this.dataGridView;
                    Debug.Assert(dataGridViewCell.OwningRow == dataGridViewRow);
                    if (dataGridViewCell.ColumnIndex == -1)
                    {
                        dataGridViewCell.OwningColumnInternal = this.DataGridView.Columns[columnIndex];
                    }
                    columnIndex++;
                }

                if (dataGridViewRow.HasHeaderCell)
                {
                    dataGridViewRow.HeaderCell.DataGridViewInternal = this.DataGridView;
                    dataGridViewRow.HeaderCell.OwningRowInternal = dataGridViewRow;
                }

                this.SharedList.Insert(rowIndexInserted, dataGridViewRow);
                Debug.Assert((dataGridViewRow.State & (DataGridViewElementStates.Selected | DataGridViewElementStates.Displayed)) == 0);
                this.rowStates.Insert(rowIndexInserted, dataGridViewRow.State);
                Debug.Assert(this.rowStates.Count == this.SharedList.Count);
#if DEBUG
                this.DataGridView.dataStoreAccessAllowed = false;
                this.cachedRowHeightsAccessAllowed = false;
                this.cachedRowCountsAccessAllowed = false;
#endif

                dataGridViewRow.IndexInternal = rowIndexInserted;
                Debug.Assert(dataGridViewRow.State == SharedRowState(rowIndexInserted));
                dataGridViewRow.DataGridViewInternal = this.dataGridView;
                rowIndexInserted++;
            }

            this.DataGridView.OnInsertedRows_PreNotification(rowIndex, dataGridViewRows);
            OnCollectionChanged(new CollectionChangeEventArgs(CollectionChangeAction.Refresh, null), rowIndex, dataGridViewRows.Length, false, true, false, newCurrentCell);
            this.DataGridView.OnInsertedRows_PostNotification(dataGridViewRows, newCurrentCell);
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
                this.rowCountsVisibleFrozen = -1;
            }
            else if (includeFilter == DataGridViewElementStates.Selected)
            {
                this.rowCountsVisibleSelected = -1;
            }

#if DEBUG
            this.cachedRowCountsAccessAllowed = true;
#endif
        }

        internal void InvalidateCachedRowCounts()
        {
            this.rowCountsVisible = this.rowCountsVisibleFrozen = this.rowCountsVisibleSelected = -1;            
#if DEBUG
            this.cachedRowCountsAccessAllowed = true;
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
                this.rowsHeightVisibleFrozen = -1;
            }

#if DEBUG
            this.cachedRowHeightsAccessAllowed = true;
#endif
        }

        internal void InvalidateCachedRowsHeights()
        {
            this.rowsHeightVisible = this.rowsHeightVisibleFrozen = -1;
#if DEBUG
            this.cachedRowHeightsAccessAllowed = true;
#endif
        }

        /// <include file='doc\DataGridViewRowCollection.uex' path='docs/doc[@for="DataGridViewRowCollection.OnCollectionChanged"]/*' />
        protected virtual void OnCollectionChanged(CollectionChangeEventArgs e)
        {
            if (this.onCollectionChanged != null)
            {
                this.onCollectionChanged(this, e);
            }
        }

        private void OnCollectionChanged(CollectionChangeEventArgs e, 
                                         int rowIndex, 
                                         int rowCount)
        {
            Debug.Assert(e.Action != CollectionChangeAction.Remove);
            Point newCurrentCell = new Point(-1, -1);
            DataGridViewRow dataGridViewRow = (DataGridViewRow) e.Element;
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
            Debug.Assert(this.DataGridView != null);
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
                        int firstDisplayedRowIndex = this.DataGridView.FirstDisplayedRowIndex;
                        if (firstDisplayedRowIndex != -1)
                        {
                            firstDisplayedRowHeight = SharedRow(firstDisplayedRowIndex).GetHeight(firstDisplayedRowIndex);
                        }
                    }
                    if (changeIsInsertion)
                    {
                        this.DataGridView.OnInsertedRow_PreNotification(rowIndex, 1);
                        if (!useRowShortcut)
                        {
                            if ((GetRowState(rowIndex) & DataGridViewElementStates.Frozen) != 0)
                            {
                                // Inserted row is frozen
                                useRowShortcut = this.DataGridView.FirstDisplayedScrollingRowIndex == -1 && 
                                                 GetRowsHeightExceedLimit(DataGridViewElementStates.Visible, 0, rowIndex, this.DataGridView.LayoutInfo.Data.Height);
                            }
                            else if (this.DataGridView.FirstDisplayedScrollingRowIndex != -1 &&
                                     rowIndex > this.DataGridView.FirstDisplayedScrollingRowIndex)
                            {
                                useRowShortcut = GetRowsHeightExceedLimit(DataGridViewElementStates.Visible, 0, rowIndex, this.DataGridView.LayoutInfo.Data.Height + this.DataGridView.VerticalScrollingOffset) &&
                                                 firstDisplayedRowHeight <= this.DataGridView.LayoutInfo.Data.Height;
                            }
                        }
                    }
                    else
                    {
                        this.DataGridView.OnAddedRow_PreNotification(rowIndex);
                        if (!useRowShortcut)
                        {
                            int displayedRowsHeightBeforeAddition = GetRowsHeight(DataGridViewElementStates.Visible) - this.DataGridView.VerticalScrollingOffset - dataGridViewRow.GetHeight(rowIndex);
                            dataGridViewRow = SharedRow(rowIndex);
                            useRowShortcut = this.DataGridView.LayoutInfo.Data.Height < displayedRowsHeightBeforeAddition &&
                                             firstDisplayedRowHeight <= this.DataGridView.LayoutInfo.Data.Height;
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
                    this.SharedList.RemoveAt(rowIndex);
#if DEBUG
                    this.DataGridView.dataStoreAccessAllowed = false;
#endif
                    this.DataGridView.OnRemovedRow_PreNotification(rowIndex);
                    if (deletedRowVisible)
                    {
                        if (deletedRowFrozen)
                        {
                            // Delete row is frozen
                            useRowShortcut = this.DataGridView.FirstDisplayedScrollingRowIndex == -1 &&
                                             GetRowsHeightExceedLimit(DataGridViewElementStates.Visible, 0, rowIndex, this.DataGridView.LayoutInfo.Data.Height + SystemInformation.HorizontalScrollBarHeight);
                        }
                        else if (this.DataGridView.FirstDisplayedScrollingRowIndex != -1 &&
                                 rowIndex > this.DataGridView.FirstDisplayedScrollingRowIndex)
                        {
                            int firstDisplayedRowHeight = 0;
                            int firstDisplayedRowIndex = this.DataGridView.FirstDisplayedRowIndex;
                            if (firstDisplayedRowIndex != -1)
                            {
                                firstDisplayedRowHeight = SharedRow(firstDisplayedRowIndex).GetHeight(firstDisplayedRowIndex);
                            }
                            useRowShortcut = GetRowsHeightExceedLimit(DataGridViewElementStates.Visible, 0, rowIndex, this.DataGridView.LayoutInfo.Data.Height + this.DataGridView.VerticalScrollingOffset + SystemInformation.HorizontalScrollBarHeight) &&
                                             firstDisplayedRowHeight <= this.DataGridView.LayoutInfo.Data.Height;
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
            this.DataGridView.ResetUIState(useRowShortcut, computeVisibleRows);
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
            Debug.Assert(this.DataGridView != null);
            if (changeIsDeletion)
            {
                this.DataGridView.OnRowsRemovedInternal(rowIndex, rowCount);
            }
            else
            {
                this.DataGridView.OnRowsAddedInternal(rowIndex, rowCount);
            }

#if DEBUG
            this.DataGridView.dataStoreAccessAllowed = true;
#endif
            switch (cca)
            {
                case CollectionChangeAction.Add:
                {
                    if (changeIsInsertion)
                    {
                        this.DataGridView.OnInsertedRow_PostNotification(rowIndex, newCurrentCell, true);
                    }
                    else
                    {
                        this.DataGridView.OnAddedRow_PostNotification(rowIndex);
                    }
                    break;
                }

                case CollectionChangeAction.Remove:
                {
                    this.DataGridView.OnRemovedRow_PostNotification(dataGridViewRow, newCurrentCell);
                    break;
                }

                case CollectionChangeAction.Refresh:
                {
                    if (changeIsDeletion)
                    {
                        this.DataGridView.OnClearedRows();
                    }
                    break;
                }
            }

            this.DataGridView.OnRowCollectionChanged_PostNotification(recreateNewRow, newCurrentCell.X == -1, cca, dataGridViewRow, rowIndex);
        }

        /// <include file='doc\DataGridViewRowCollection.uex' path='docs/doc[@for="DataGridViewRowCollection.Remove"]/*' />
        [
            SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters") // We don't want to use DataGridViewBand here.
        ]
        public virtual void Remove(DataGridViewRow dataGridViewRow)
        {
            if (dataGridViewRow == null)
            {
                throw new ArgumentNullException(nameof(dataGridViewRow));
            }

            if (dataGridViewRow.DataGridView != this.DataGridView)
            {
                throw new ArgumentException(string.Format(SR.DataGridView_RowDoesNotBelongToDataGridView), "dataGridViewRow");
            }

            if (dataGridViewRow.Index == -1)
            {
                throw new ArgumentException(string.Format(SR.DataGridView_RowMustBeUnshared), "dataGridViewRow");
            }
            else
            {
                RemoveAt(dataGridViewRow.Index);
            }
        }

        /// <include file='doc\DataGridViewRowCollection.uex' path='docs/doc[@for="DataGridViewRowCollection.RemoveAt"]/*' />
        public virtual void RemoveAt(int index)
        {
            if (index < 0 || index >= this.Count)
            {
                throw new ArgumentOutOfRangeException(nameof(index), string.Format(SR.DataGridViewRowCollection_RowIndexOutOfRange));
            }

            if (this.DataGridView.NewRowIndex == index)
            {
                Debug.Assert(this.DataGridView.AllowUserToAddRowsInternal);
                throw new InvalidOperationException(string.Format(SR.DataGridViewRowCollection_CannotDeleteNewRow)); 
            }

            if (this.DataGridView.NoDimensionChangeAllowed)
            {
                throw new InvalidOperationException(string.Format(SR.DataGridView_ForbiddenOperationInEventHandler));
            }

            if (this.DataGridView.DataSource != null)
            {
                IBindingList list = this.DataGridView.DataConnection.List as IBindingList;
                if (list != null && list.AllowRemove && list.SupportsChangeNotification)
                {
                    ((IList)list).RemoveAt(index);
                }
                else
                {
                    throw new InvalidOperationException(string.Format(SR.DataGridViewRowCollection_CantRemoveRowsWithWrongSource));
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

            Debug.Assert(index >= 0 && index < this.Count);
            Debug.Assert(this.DataGridView != null);
            Debug.Assert(!this.DataGridView.NoDimensionChangeAllowed);

            DataGridViewRow dataGridViewRow = SharedRow(index);
            Point newCurrentCell = new Point(-1, -1);

            if (this.IsCollectionChangedListenedTo || dataGridViewRow.GetDisplayed(index))
            {
                dataGridViewRow = this[index]; // need to unshare row because dev is listening to OnCollectionChanged event or the row is displayed
            }

            dataGridViewRow = SharedRow(index);
            Debug.Assert(this.DataGridView != null);
            this.DataGridView.OnRemovingRow(index, out newCurrentCell, force);
            UpdateRowCaches(index, ref dataGridViewRow, false /*adding*/); 
            if (dataGridViewRow.Index != -1)
            {
                this.rowStates[index] = dataGridViewRow.State;
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
            Debug.Assert(index < this.Count);

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
                if ((dataGridViewCell.State & ~(dataGridViewCell.CellStateFromColumnRowStates(this.rowStates[index]))) != 0)
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
                if (((this.rowStates[rowIndex] & state) != 0) != value)
                {
                    if (state == DataGridViewElementStates.Frozen || 
                        state == DataGridViewElementStates.Visible ||
                        state == DataGridViewElementStates.ReadOnly)
                    {
                        dataGridViewRow.OnSharedStateChanging(rowIndex, state);
                    }
                    if (value)
                    {
                        this.rowStates[rowIndex] = this.rowStates[rowIndex] | state;
                    }
                    else
                    {
                        this.rowStates[rowIndex] = this.rowStates[rowIndex] & ~state;
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
                        dataGridViewRow.DisplayedInternal = value;
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
            return this.rowStates[rowIndex];
        }

        internal void Sort(IComparer customComparer, bool ascending)
        {
            if (this.items.Count > 0)
            {
                RowComparer rowComparer = new RowComparer(this, customComparer, ascending);
                this.items.CustomSort(rowComparer);
                // Caller takes care of the dataGridView invalidation
            }
        }

        internal void SwapSortedRows(int rowIndex1, int rowIndex2)
        {
            // Deal with the current cell address updates +
            // selected rows updates.
            this.DataGridView.SwapSortedRows(rowIndex1, rowIndex2);

            DataGridViewRow dataGridViewRow1 = SharedRow(rowIndex1);
            DataGridViewRow dataGridViewRow2 = SharedRow(rowIndex2);

            if (dataGridViewRow1.Index != -1)
            {
                dataGridViewRow1.IndexInternal = rowIndex2;
            }
            if (dataGridViewRow2.Index != -1)
            {
                dataGridViewRow2.IndexInternal = rowIndex1;
            }

            if (this.DataGridView.VirtualMode)
            {
                // All cell contents on the involved rows need to be swapped
                int columnCount = this.DataGridView.Columns.Count;

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

            object item = this.items[rowIndex1];
            this.items[rowIndex1] = this.items[rowIndex2];
            this.items[rowIndex2] = item;

            DataGridViewElementStates rowStates = this.rowStates[rowIndex1];
            this.rowStates[rowIndex1] = this.rowStates[rowIndex2];
            this.rowStates[rowIndex2] = rowStates;
        }

        // This function only adjusts the row's RowIndex and State properties - no more.
        private void UnshareRow(int rowIndex)
        {
            SharedRow(rowIndex).IndexInternal = rowIndex;
            SharedRow(rowIndex).StateInternal = SharedRowState(rowIndex);
        }

        private void UpdateRowCaches(int rowIndex, ref DataGridViewRow dataGridViewRow, bool adding)
        {
            if (this.rowCountsVisible != -1 || this.rowCountsVisibleFrozen != -1 || this.rowCountsVisibleSelected != -1 ||
                this.rowsHeightVisible != -1 || this.rowsHeightVisibleFrozen != -1)
            {
                DataGridViewElementStates rowStates = GetRowState(rowIndex);
                if ((rowStates & DataGridViewElementStates.Visible) != 0)
                {
                    int rowCountIncrement = adding ? 1 : -1;
                    int rowHeightIncrement = 0;
                    if (this.rowsHeightVisible != -1 || 
                        (this.rowsHeightVisibleFrozen != -1 && 
                         ((rowStates & (DataGridViewElementStates.Visible | DataGridViewElementStates.Frozen)) == (DataGridViewElementStates.Visible | DataGridViewElementStates.Frozen))))
                    {
                        // dataGridViewRow may become unshared in GetHeight call
                        rowHeightIncrement = adding ? dataGridViewRow.GetHeight(rowIndex) : -dataGridViewRow.GetHeight(rowIndex);
                        dataGridViewRow = SharedRow(rowIndex);
                    }
                    
                    if (this.rowCountsVisible != -1)
                    {
                        this.rowCountsVisible += rowCountIncrement;
                    }
                    if (this.rowsHeightVisible != -1)
                    {
                        Debug.Assert(rowHeightIncrement != 0);
                        this.rowsHeightVisible += rowHeightIncrement;
                    }
                    
                    if ((rowStates & (DataGridViewElementStates.Visible | DataGridViewElementStates.Frozen)) == (DataGridViewElementStates.Visible | DataGridViewElementStates.Frozen))
                    {
                        if (this.rowCountsVisibleFrozen != -1)
                        {
                            this.rowCountsVisibleFrozen += rowCountIncrement;
                        }
                        if (this.rowsHeightVisibleFrozen != -1)
                        {
                            Debug.Assert(rowHeightIncrement != 0);
                            this.rowsHeightVisibleFrozen += rowHeightIncrement;
                        }
                    }

                    if ((rowStates & (DataGridViewElementStates.Visible | DataGridViewElementStates.Selected)) == (DataGridViewElementStates.Visible | DataGridViewElementStates.Selected))
                    {
                        if (this.rowCountsVisibleSelected != -1)
                        {
                            this.rowCountsVisibleSelected += rowCountIncrement;
                        }
                    }
                }
            }

#if DEBUG
            this.cachedRowCountsAccessAllowed = true;
            this.cachedRowHeightsAccessAllowed = true;
#endif
        }

/*#if DEBUG
        private bool inVerifyRowFrozenStates = false;
        public void VerifyRowFrozenStates()
        {
            if (inVerifyRowFrozenStates) return;

            inVerifyRowFrozenStates = true;
            try 
            {
                bool previousVisibleRowFrozen = true;
                for (int rowIndex = 0; rowIndex < this.items.Count; rowIndex++)
                {
                    DataGridViewElementStates rowStates = GetRowState(rowIndex);
                    if (!previousVisibleRowFrozen &&
                        (rowStates & DataGridViewElementStates.Visible) != 0 &&
                        (rowStates & DataGridViewElementStates.Frozen) != 0)
                    {
                        Debug.Fail("VerifyRowFrozenStates - wrong frozen state");
                    }
                    if ((rowStates & DataGridViewElementStates.Visible) != 0)
                    {
                        previousVisibleRowFrozen = (rowStates & DataGridViewElementStates.Frozen) != 0;
                    }
                }
            }
            finally
            {
                inVerifyRowFrozenStates = false;
            }
        }
#endif*/

        /* Private classes */

        /*private class DefaultRowComparer : IComparer
        {
            private DataGridView dataGridView;
            private DataGridViewRowCollection dataGridViewRows;
            private DataGridViewColumn dataGridViewSortedColumn;
            private int sortedColumnIndex;

            public DefaultRowComparer(DataGridViewRowCollection dataGridViewRows)
            {
                this.DataGridViewInternal = dataGridViewRows.DataGridView;
                this.dataGridViewRows = dataGridViewRows;
                this.dataGridViewSortedColumn = this.dataGridView.SortedColumn;
                this.sortedColumnIndex = this.dataGridViewSortedColumn.Index;
            }

            int IComparer.Compare(object x, object y)
            {
                DataGridViewRow dataGridViewRow1 = this.dataGridViewRows.SharedRow((int)x);
                DataGridViewRow dataGridViewRow2 = this.dataGridViewRows.SharedRow((int)y);
                Debug.Assert(dataGridViewRow1 != null);
                Debug.Assert(dataGridViewRow2 != null);
                object value1 = dataGridViewRow1.Cells[this.sortedColumnIndex].GetValueInternal((int)x);
                object value2 = dataGridViewRow2.Cells[this.sortedColumnIndex].GetValueInternal((int)y);
                DataGridViewSortEventArgs tsea = new DataGridViewSortEventArgs(this.dataGridViewSortedColumn, value1, value2);
                this.dataGridView.OnSorting(tsea);
                if (tsea.Handled)
                {
                    return tsea.SortResult;
                }
                else
                {
                    return Comparer.Default.Compare(value1, value2);
                }
            }
        }*/

        private class RowArrayList : ArrayList
        {
            private DataGridViewRowCollection owner;
            private RowComparer rowComparer;

            public RowArrayList(DataGridViewRowCollection owner)
            {
                this.owner = owner;
            }

            public void CustomSort(RowComparer rowComparer)
            {
                Debug.Assert(rowComparer != null);
                Debug.Assert(this.Count > 0);

                this.rowComparer = rowComparer;
                CustomQuickSort(0, this.Count - 1);
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
                        if (right - left > 0 && this.rowComparer.CompareObjects(this.rowComparer.GetComparedObject(left), this.rowComparer.GetComparedObject(right), left, right) > 0)
                        {
                            this.owner.SwapSortedRows(left, right);
                        }
                        return;
                    }

                    int k = (left + right) >> 1;
                    object x = Pivot(left, k, right);                
                    int i = left+1;
                    int j = right-1;
                    do
                    {
                        while (k != i && this.rowComparer.CompareObjects(this.rowComparer.GetComparedObject(i), x, i, k) < 0)
                        {
                            i++;
                        }
                        while (k != j && this.rowComparer.CompareObjects(x, this.rowComparer.GetComparedObject(j), k, j) < 0)
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
                            this.owner.SwapSortedRows(i, j);
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
                if (this.rowComparer.CompareObjects(this.rowComparer.GetComparedObject(left), this.rowComparer.GetComparedObject(center), left, center) > 0)
                {
                    this.owner.SwapSortedRows(left, center);
                }
                if (this.rowComparer.CompareObjects(this.rowComparer.GetComparedObject(left), this.rowComparer.GetComparedObject(right), left, right) > 0)
                {
                    this.owner.SwapSortedRows(left, right);
                }
                if (this.rowComparer.CompareObjects(this.rowComparer.GetComparedObject(center), this.rowComparer.GetComparedObject(right), center, right) > 0)
                {
                    this.owner.SwapSortedRows(center, right);
                }
                return this.rowComparer.GetComparedObject(center);
            }
        }

        private class RowComparer
        {
            private DataGridView dataGridView;
            private DataGridViewRowCollection dataGridViewRows;
            private DataGridViewColumn dataGridViewSortedColumn;
            private int sortedColumnIndex;
            private IComparer customComparer;
            private bool ascending;
            private static ComparedObjectMax max = new ComparedObjectMax();

            public RowComparer(DataGridViewRowCollection dataGridViewRows, IComparer customComparer, bool ascending)
            {
                this.dataGridView = dataGridViewRows.DataGridView;
                this.dataGridViewRows = dataGridViewRows;
                this.dataGridViewSortedColumn = this.dataGridView.SortedColumn;
                if (this.dataGridViewSortedColumn == null)
                {
                    Debug.Assert(customComparer != null);
                    this.sortedColumnIndex = -1;
                }
                else
                {
                    this.sortedColumnIndex = this.dataGridViewSortedColumn.Index;
                }
                this.customComparer = customComparer;
                this.ascending = ascending;
            }

            internal object GetComparedObject(int rowIndex)
            {
                if (this.dataGridView.NewRowIndex != -1)
                {
                    Debug.Assert(this.dataGridView.AllowUserToAddRowsInternal);
                    if (rowIndex == this.dataGridView.NewRowIndex)
                    {
                        return max;
                    }
                }
                if (this.customComparer == null)
                {
                    DataGridViewRow dataGridViewRow = this.dataGridViewRows.SharedRow(rowIndex);
                    Debug.Assert(dataGridViewRow != null);
                    Debug.Assert(this.sortedColumnIndex >= 0);
                    return dataGridViewRow.Cells[this.sortedColumnIndex].GetValueInternal(rowIndex);
                }
                else
                {
                    return this.dataGridViewRows[rowIndex]; // Unsharing compared rows!
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
                if (this.customComparer == null)
                {
                    if (!this.dataGridView.OnSortCompare(this.dataGridViewSortedColumn, value1, value2, rowIndex1, rowIndex2, out result))
                    {
                        if (!(value1 is IComparable) && !(value2 is IComparable))
                        {
                            if (value1 == null)
                            {
                                if (value2 == null)
                                {
                                    result = 0;
                                }
                                else
                                {
                                    result = 1;
                                }
                            }
                            else if (value2 == null)
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
                            if (this.ascending)
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


                    result = this.customComparer.Compare(value1, value2);
                }

                if (this.ascending)
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
            private DataGridViewRowCollection owner;
            private int current;
                
            /// <devdoc>
            ///     Creates a new enumerator that will enumerate over the rows and unshare the accessed rows if needed.
            /// </devdoc>
            public UnsharingRowEnumerator(DataGridViewRowCollection owner) 
            {
                this.owner = owner;
                this.current = -1;
            }
                
            /// <devdoc>
            ///     Moves to the next element, or returns false if at the end.
            /// </devdoc>
            bool IEnumerator.MoveNext() 
            {

                if (this.current < this.owner.Count - 1) 
                {
                    this.current++;
                    return true;
                }
                else 
                {
                    this.current = this.owner.Count;
                    return false;
                }
            }
    
            /// <devdoc>
            ///     Resets the enumeration back to the beginning.
            /// </devdoc>
            void IEnumerator.Reset() 
            {
                this.current = -1;
            }
    
            /// <devdoc>
            ///     Retrieves the current value in the enumerator.
            /// </devdoc>
            object IEnumerator.Current 
            {
                get 
                {
                    if (this.current == -1) 
                    {
                        throw new InvalidOperationException(string.Format(SR.DataGridViewRowCollection_EnumNotStarted));
                    }
                    if (this.current == this.owner.Count)
                    {
                        throw new InvalidOperationException(string.Format(SR.DataGridViewRowCollection_EnumFinished));
                    }
                    return this.owner[this.current];
                }
            }
        }
    }
}
