// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms
{
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System;
    using System.Drawing;
    using System.Collections;
    using System.Windows.Forms;
    using System.ComponentModel;
    using System.ComponentModel.Design.Serialization;
    using System.Globalization;
    
    /// <include file='doc\DataGridViewColumnCollection.uex' path='docs/doc[@for="DataGridViewColumnCollection"]/*' />
    /// <devdoc>
    /// <para>Represents a collection of <see cref='System.Windows.Forms.DataGridViewColumn'/> objects in the <see cref='System.Windows.Forms.DataGrid'/> 
    /// control.</para>
    /// </devdoc>
    [
        ListBindable(false),
        SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable"), // Columns are only disposed in the designer.
        SuppressMessage("Microsoft.Design", "CA1010:CollectionsShouldImplementGenericInterface") // Consider adding an IList<DataGridViewColumnCollection> implementation
    ]
    public class DataGridViewColumnCollection : BaseCollection, IList
    {
        private CollectionChangeEventHandler onCollectionChanged;
        private ArrayList items = new ArrayList();
        private ArrayList itemsSorted;
        private int lastAccessedSortedIndex = -1;
        private int columnCountsVisible, columnCountsVisibleSelected;
        private int columnsWidthVisible, columnsWidthVisibleFrozen;
        private static ColumnOrderComparer columnOrderComparer = new ColumnOrderComparer();
        private DataGridView dataGridView;

        /* IList interface implementation */

        /// <include file='doc\DataGridViewColumnCollection.uex' path='docs/doc[@for="DataGridViewColumnCollection.IList.IsFixedSize"]/*' />
        /// <internalonly/>
        bool IList.IsFixedSize
        {
            get {return false;}
        }

        /// <include file='doc\DataGridViewColumnCollection.uex' path='docs/doc[@for="DataGridViewColumnCollection.IList.IsReadOnly"]/*' />
        /// <internalonly/>
        bool IList.IsReadOnly
        {
            get {return false;}
        }

        /// <include file='doc\DataGridViewColumnCollection.uex' path='docs/doc[@for="DataGridViewColumnCollection.IList.this"]/*' />
        /// <internalonly/>
        object IList.this[int index]
        {
            get { return this[index]; }
            set { throw new NotSupportedException(); }
        }

        /// <include file='doc\DataGridViewColumnCollection.uex' path='docs/doc[@for="DataGridViewColumnCollection.IList.Add"]/*' />
        /// <internalonly/>
        int IList.Add(object value)
        {
            return this.Add((DataGridViewColumn) value);            
        }

        /// <include file='doc\DataGridViewColumnCollection.uex' path='docs/doc[@for="DataGridViewColumnCollection.IList.Clear"]/*' />
        /// <internalonly/>
        void IList.Clear()
        {
            this.Clear();
        }

        /// <include file='doc\DataGridViewColumnCollection.uex' path='docs/doc[@for="DataGridViewColumnCollection.IList.Contains"]/*' />
        /// <internalonly/>
        bool IList.Contains(object value)
        {
            return this.items.Contains(value);
        }

        /// <include file='doc\DataGridViewColumnCollection.uex' path='docs/doc[@for="DataGridViewColumnCollection.IList.IndexOf"]/*' />
        /// <internalonly/>
        int IList.IndexOf(object value)
        {
            return this.items.IndexOf(value);
        }

        /// <include file='doc\DataGridViewColumnCollection.uex' path='docs/doc[@for="DataGridViewColumnCollection.IList.Insert"]/*' />
        /// <internalonly/>
        void IList.Insert(int index, object value)
        {
            this.Insert(index, (DataGridViewColumn) value);
        }

        /// <include file='doc\DataGridViewColumnCollection.uex' path='docs/doc[@for="DataGridViewColumnCollection.IList.Remove"]/*' />
        /// <internalonly/>
        void IList.Remove(object value)
        {
            this.Remove((DataGridViewColumn) value);
        }

        /// <include file='doc\DataGridViewColumnCollection.uex' path='docs/doc[@for="DataGridViewColumnCollection.IList.RemoveAt"]/*' />
        /// <internalonly/>
        void IList.RemoveAt(int index)
        {
            this.RemoveAt(index);
        }


        /* ICollection interface implementation */

        /// <include file='doc\DataGridViewColumnCollection.uex' path='docs/doc[@for="DataGridViewColumnCollection.ICollection.Count"]/*' />
        /// <internalonly/>
        int ICollection.Count
        {
            get 
            {
                return this.items.Count;
            }
        }

        /// <include file='doc\DataGridViewColumnCollection.uex' path='docs/doc[@for="DataGridViewColumnCollection.ICollection.IsSynchronized"]/*' />
        /// <internalonly/>
        bool ICollection.IsSynchronized
        {
            get 
            {
                return false;
            }
        }

        /// <include file='doc\DataGridViewColumnCollection.uex' path='docs/doc[@for="DataGridViewColumnCollection.ICollection.SyncRoot"]/*' />
        /// <internalonly/>
        object ICollection.SyncRoot
        {
            get 
            {
                return this;
            }
        }

        /// <include file='doc\DataGridViewColumnCollection.uex' path='docs/doc[@for="DataGridViewColumnCollection.ICollection.CopyTo"]/*' />
        /// <internalonly/>
        void ICollection.CopyTo(Array array, int index)
        {
            this.items.CopyTo(array, index);
        }


        /* IEnumerable interface implementation */

        /// <include file='doc\DataGridViewColumnCollection.uex' path='docs/doc[@for="DataGridViewColumnCollection.IEnumerable.GetEnumerator"]/*' />
        /// <internalonly/>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.items.GetEnumerator();
        }


        /// <include file='doc\DataGridViewColumnCollection.uex' path='docs/doc[@for="DataGridViewColumnCollection.DataGridViewColumnCollection"]/*' />
        public DataGridViewColumnCollection(DataGridView dataGridView)
        {
            InvalidateCachedColumnCounts();
            InvalidateCachedColumnsWidths();
            this.dataGridView = dataGridView;
        }

        internal static IComparer ColumnCollectionOrderComparer
        {
            get
            {
                return System.Windows.Forms.DataGridViewColumnCollection.columnOrderComparer;
            }
        }

        /// <include file='doc\DataGridViewColumnCollection.uex' path='docs/doc[@for="DataGridViewColumnCollection.List"]/*' />
        protected override ArrayList List
        {
            get
            {
                return this.items;
            }
        }

        /// <include file='doc\DataGridViewColumnCollection.uex' path='docs/doc[@for="DataGridViewColumnCollection.DataGridView"]/*' />
        protected DataGridView DataGridView
        {
            get
            {
                return this.dataGridView;
            }
        }

        /// <include file='doc\DataGridViewColumnCollection.uex' path='docs/doc[@for="DataGridViewColumnCollection.this"]/*' />
        /// <devdoc>
        ///      Retrieves the DataGridViewColumn with the specified index.
        /// </devdoc>
        public DataGridViewColumn this[int index]
        {
            get
            {
                return (DataGridViewColumn) this.items[index];
            }
        }

        /// <include file='doc\DataGridViewColumnCollection.uex' path='docs/doc[@for="DataGridViewColumnCollection.this1"]/*' />
        /// <devdoc>
        ///      Retrieves the DataGridViewColumn with the Name provided.
        /// </devdoc>
        public DataGridViewColumn this[string columnName]
        {
            get
            {
                if (columnName == null)
                {
                    throw new ArgumentNullException(nameof(columnName));
                }
                int itemCount = this.items.Count;
                for (int i = 0; i < itemCount; ++i)
                {
                    DataGridViewColumn dataGridViewColumn = (DataGridViewColumn) this.items[i];
                    // NOTE: case-insensitive
                    if (String.Equals(dataGridViewColumn.Name, columnName, StringComparison.OrdinalIgnoreCase))
                    {
                        return dataGridViewColumn;
                    }
                }
                return null;
            }
        }

        /// <include file='doc\DataGridViewColumnCollection.uex' path='docs/doc[@for="DataGridViewColumnCollection.CollectionChanged"]/*' />
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

        internal int ActualDisplayIndexToColumnIndex(int actualDisplayIndex, DataGridViewElementStates includeFilter)
        {
            // Microsoft: is there a faster way to get the column index?
            DataGridViewColumn dataGridViewColumn = GetFirstColumn(includeFilter);
            for (int i = 0; i < actualDisplayIndex; i ++)
            {
                dataGridViewColumn = GetNextColumn(dataGridViewColumn, includeFilter, DataGridViewElementStates.None);
            }
            return dataGridViewColumn.Index;
        }

        /// <include file='doc\DataGridViewColumnCollection.uex' path='docs/doc[@for="DataGridViewColumnCollection.Add1"]/*' />
        /// <devdoc>
        /// <para>Adds a <see cref='System.Windows.Forms.DataGridViewColumn'/> to this collection.</para>
        /// </devdoc>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public virtual int Add(string columnName, string headerText)
        {
            DataGridViewTextBoxColumn dataGridViewTextBoxColumn = new DataGridViewTextBoxColumn();
            dataGridViewTextBoxColumn.Name = columnName;
            dataGridViewTextBoxColumn.HeaderText = headerText;

            return Add(dataGridViewTextBoxColumn);
        }

        /// <include file='doc\DataGridViewColumnCollection.uex' path='docs/doc[@for="DataGridViewColumnCollection.Add2"]/*' />
        /// <devdoc>
        /// <para>Adds a <see cref='System.Windows.Forms.DataGridViewColumn'/> to this collection.</para>
        /// </devdoc>
        public virtual int Add(DataGridViewColumn dataGridViewColumn)
        {
            Debug.Assert(this.DataGridView != null);
            if (this.DataGridView.NoDimensionChangeAllowed)
            {
                throw new InvalidOperationException(string.Format(SR.DataGridView_ForbiddenOperationInEventHandler));
            }
            if (this.DataGridView.InDisplayIndexAdjustments)
            {
                // We are within columns display indexes adjustments. We do not allow changing the column collection while adjusting display indexes.
                throw new InvalidOperationException(string.Format(SR.DataGridView_CannotAlterDisplayIndexWithinAdjustments));
            }

            this.DataGridView.OnAddingColumn(dataGridViewColumn);   // will throw an exception if the addition is illegal

            InvalidateCachedColumnsOrder();
            int index = this.items.Add(dataGridViewColumn);
            dataGridViewColumn.IndexInternal = index;
            dataGridViewColumn.DataGridViewInternal = dataGridView;
            UpdateColumnCaches(dataGridViewColumn, true);
            this.DataGridView.OnAddedColumn(dataGridViewColumn);
            OnCollectionChanged(new CollectionChangeEventArgs(CollectionChangeAction.Add, dataGridViewColumn), false /*changeIsInsertion*/, new Point(-1, -1));
#if DEBUG
            Debug.Assert(this.itemsSorted == null || VerifyColumnOrderCache());
#endif
            return index;
        }
        
        /// <include file='doc\DataGridViewColumnCollection.uex' path='docs/doc[@for="DataGridViewColumnCollection.AddRange"]/*' />
        public virtual void AddRange(params DataGridViewColumn[] dataGridViewColumns)
        {
            if (dataGridViewColumns == null)
            {
                throw new ArgumentNullException(nameof(dataGridViewColumns));
            }

            Debug.Assert(this.DataGridView != null);
            if (this.DataGridView.NoDimensionChangeAllowed)
            {
                throw new InvalidOperationException(string.Format(SR.DataGridView_ForbiddenOperationInEventHandler));
            }
            if (this.DataGridView.InDisplayIndexAdjustments)
            {
                // We are within columns display indexes adjustments. We do not allow changing the column collection while adjusting display indexes.
                throw new InvalidOperationException(string.Format(SR.DataGridView_CannotAlterDisplayIndexWithinAdjustments));
            }

            // Order the columns by ascending DisplayIndex so that their display indexes are not altered by the operation.
            // The columns with DisplayIndex == -1 are left untouched relative to each other and put at the end of the array.
            ArrayList initialColumns = new ArrayList(dataGridViewColumns.Length);
            ArrayList sortedColumns = new ArrayList(dataGridViewColumns.Length);

            // All columns with DisplayIndex != -1 are put into the initialColumns array
            foreach (DataGridViewColumn dataGridViewColumn in dataGridViewColumns) 
            {
                if (dataGridViewColumn.DisplayIndex != -1)
                {
                    initialColumns.Add(dataGridViewColumn);
                }
            }

            // Those columns are copied into the sortedColumns array in an N^2 sort algo that
            // does not disrupt the order of columns with identical DisplayIndex values.
            int smallestDisplayIndex, smallestIndex, index;

            while (initialColumns.Count > 0)
            {
                smallestDisplayIndex = Int32.MaxValue;
                smallestIndex = -1;
                for (index = 0; index < initialColumns.Count; index++) 
                {
                    DataGridViewColumn dataGridViewColumn = (DataGridViewColumn) initialColumns[index];
                    if (dataGridViewColumn.DisplayIndex < smallestDisplayIndex)
                    {
                        smallestDisplayIndex = dataGridViewColumn.DisplayIndex;
                        smallestIndex = index;
                    }
                }
                Debug.Assert(smallestIndex >= 0);
                sortedColumns.Add(initialColumns[smallestIndex]);
                initialColumns.RemoveAt(smallestIndex);
            }

            // The columns with DisplayIndex == -1 are append at the end of sortedColumns
            // without disrupting their relative order.
            foreach (DataGridViewColumn dataGridViewColumn in dataGridViewColumns) 
            {
                if (dataGridViewColumn.DisplayIndex == -1)
                {
                    sortedColumns.Add(dataGridViewColumn);
                }
            }

            // Finally the dataGridViewColumns is reconstructed using the sortedColumns.
            index = 0;
            foreach (DataGridViewColumn dataGridViewColumn in sortedColumns) 
            {
                dataGridViewColumns[index] = dataGridViewColumn;
                index++;
            }

            this.DataGridView.OnAddingColumns(dataGridViewColumns);   // will throw an exception if the addition is illegal

            foreach (DataGridViewColumn dataGridViewColumn in dataGridViewColumns) 
            {
                InvalidateCachedColumnsOrder();
                index = this.items.Add(dataGridViewColumn);
                dataGridViewColumn.IndexInternal = index;
                dataGridViewColumn.DataGridViewInternal = dataGridView;
                UpdateColumnCaches(dataGridViewColumn, true);
                this.DataGridView.OnAddedColumn(dataGridViewColumn);
            }

            OnCollectionChanged(new CollectionChangeEventArgs(CollectionChangeAction.Refresh, null), false /*changeIsInsertion*/, new Point(-1, -1));
#if DEBUG
            Debug.Assert(this.itemsSorted == null || VerifyColumnOrderCache());
#endif
        }

        /// <include file='doc\DataGridViewColumnCollection.uex' path='docs/doc[@for="DataGridViewColumnCollection.Clear"]/*' />
        public virtual void Clear()
        {
            if (this.Count > 0)
            {
                if (this.DataGridView.NoDimensionChangeAllowed)
                {
                    throw new InvalidOperationException(string.Format(SR.DataGridView_ForbiddenOperationInEventHandler));
                }
                if (this.DataGridView.InDisplayIndexAdjustments)
                {
                    // We are within columns display indexes adjustments. We do not allow changing the column collection while adjusting display indexes.
                    throw new InvalidOperationException(string.Format(SR.DataGridView_CannotAlterDisplayIndexWithinAdjustments));
                }

                for (int columnIndex = 0; columnIndex < this.Count; columnIndex++)
                {
                    DataGridViewColumn dataGridViewColumn = this[columnIndex];
                    // Detach the column...
                    dataGridViewColumn.DataGridViewInternal = null;
                    // ...and its potential header cell
                    if (dataGridViewColumn.HasHeaderCell)
                    {
                        dataGridViewColumn.HeaderCell.DataGridViewInternal = null;
                    }
                }

                DataGridViewColumn[] aColumns = new DataGridViewColumn[this.items.Count];
                CopyTo(aColumns, 0);

                this.DataGridView.OnClearingColumns();
                InvalidateCachedColumnsOrder();
                this.items.Clear();
                InvalidateCachedColumnCounts();
                InvalidateCachedColumnsWidths();
                foreach (DataGridViewColumn dataGridViewColumn in aColumns)
                {
                    this.DataGridView.OnColumnRemoved(dataGridViewColumn);
                    this.DataGridView.OnColumnHidden(dataGridViewColumn);
                }
                OnCollectionChanged(new CollectionChangeEventArgs(CollectionChangeAction.Refresh, null), false /*changeIsInsertion*/, new Point(-1, -1));
#if DEBUG
                Debug.Assert(this.itemsSorted == null || VerifyColumnOrderCache());
#endif
            }
        }

        internal int ColumnIndexToActualDisplayIndex(int columnIndex, DataGridViewElementStates includeFilter)
        {
            // map the column index to the actual display index
            DataGridViewColumn dataGridViewColumn = GetFirstColumn(includeFilter);
            int actualDisplayIndex = 0;
            while (dataGridViewColumn != null && dataGridViewColumn.Index != columnIndex)
            {
                dataGridViewColumn = GetNextColumn(dataGridViewColumn, includeFilter, DataGridViewElementStates.None);
                actualDisplayIndex ++;
            }
            return actualDisplayIndex;
        }

        /// <include file='doc\DataGridViewColumnCollection.uex' path='docs/doc[@for="DataGridViewColumnCollection.Contains"]/*' />
        /// <devdoc>
        ///      Checks to see if a DataGridViewColumn is contained in this collection.
        /// </devdoc>
        public virtual bool Contains(DataGridViewColumn dataGridViewColumn)
        {
            return this.items.IndexOf(dataGridViewColumn) != -1;
        }

        /// <include file='doc\DataGridViewColumnCollection.uex' path='docs/doc[@for="DataGridViewColumnCollection.Contains2"]/*' />
        public virtual bool Contains(string columnName)
        {
            if (columnName == null)
            {
                throw new ArgumentNullException(nameof(columnName));
            }
            int itemCount = this.items.Count;
            for (int i = 0; i < itemCount; ++i)
            {
                DataGridViewColumn dataGridViewColumn = (DataGridViewColumn) this.items[i];
                // NOTE: case-insensitive
                if (0 == String.Compare(dataGridViewColumn.Name, columnName, true, CultureInfo.InvariantCulture))
                {
                    return true;
                }
            }
            return false;
        }

        /// <include file='doc\DataGridViewColumnCollection.uex' path='docs/doc[@for="DataGridViewColumnCollection.CopyTo"]/*' />
        public void CopyTo(DataGridViewColumn[] array, int index)
        {
            this.items.CopyTo(array, index);
        }

        internal bool DisplayInOrder(int columnIndex1, int columnIndex2)
        {
            int displayIndex1 = ((DataGridViewColumn) this.items[columnIndex1]).DisplayIndex;
            int displayIndex2 = ((DataGridViewColumn) this.items[columnIndex2]).DisplayIndex;
            return displayIndex1 < displayIndex2;
        }

        internal DataGridViewColumn GetColumnAtDisplayIndex(int displayIndex)
        {
            if (displayIndex < 0 || displayIndex >= this.items.Count)
            {
                return null;
            }
            DataGridViewColumn dataGridViewColumn = ((DataGridViewColumn) this.items[displayIndex]);
            if (dataGridViewColumn.DisplayIndex == displayIndex)
            {
                // Performance gain if display indexes coincide with indexes.
                return dataGridViewColumn;
            }
            for (int columnIndex = 0; columnIndex < this.items.Count; columnIndex++)
            {
                dataGridViewColumn = ((DataGridViewColumn) this.items[columnIndex]);
                if (dataGridViewColumn.DisplayIndex == displayIndex)
                {
                    return dataGridViewColumn;
                }
            }
            Debug.Fail("no column found in GetColumnAtDisplayIndex");
            return null;
        }

        /// <include file='doc\DataGridViewColumnCollection.uex' path='docs/doc[@for="DataGridViewColumnCollection.GetColumnCount"]/*' />
        public int GetColumnCount(DataGridViewElementStates includeFilter)
        {
            if ((includeFilter & ~(DataGridViewElementStates.Displayed | DataGridViewElementStates.Frozen | DataGridViewElementStates.Resizable |
                DataGridViewElementStates.ReadOnly | DataGridViewElementStates.Selected | DataGridViewElementStates.Visible)) != 0)
            {
                throw new ArgumentException(string.Format(SR.DataGridView_InvalidDataGridViewElementStateCombination, "includeFilter"));
            }

            // cache returned value and reuse it as long as none
            // of the column's state has changed.
            switch (includeFilter)
            {
                case DataGridViewElementStates.Visible:
                    if (this.columnCountsVisible != -1)
                    {
                        return this.columnCountsVisible;
                    }
                    break;
                case DataGridViewElementStates.Visible | DataGridViewElementStates.Selected:
                    if (this.columnCountsVisibleSelected != -1)
                    {
                        return this.columnCountsVisibleSelected;
                    }
                    break;
            }

            int columnCount = 0;
            if ((includeFilter & DataGridViewElementStates.Resizable) == 0)
            {
                for (int columnIndex = 0; columnIndex < this.items.Count; columnIndex++)
                {
                    if (((DataGridViewColumn)this.items[columnIndex]).StateIncludes(includeFilter))
                    {
                        columnCount++;
                    }
                }
                switch (includeFilter)
                {
                    case DataGridViewElementStates.Visible:
                        this.columnCountsVisible = columnCount;
                        break;
                    case DataGridViewElementStates.Visible | DataGridViewElementStates.Selected:
                        this.columnCountsVisibleSelected = columnCount;
                        break;
                }
            }
            else
            {
                DataGridViewElementStates correctedIncludeFilter = includeFilter & ~DataGridViewElementStates.Resizable;
                for (int columnIndex = 0; columnIndex < this.items.Count; columnIndex++)
                {
                    if (((DataGridViewColumn)this.items[columnIndex]).StateIncludes(correctedIncludeFilter) &&
                        ((DataGridViewColumn)this.items[columnIndex]).Resizable == DataGridViewTriState.True)
                    {
                        columnCount++;
                    }
                }
            }
            return columnCount;
        }

        internal int GetColumnCount(DataGridViewElementStates includeFilter, int fromColumnIndex, int toColumnIndex)
        {
            Debug.Assert((includeFilter & ~(DataGridViewElementStates.Displayed | DataGridViewElementStates.Frozen | DataGridViewElementStates.Resizable | 
                         DataGridViewElementStates.ReadOnly | DataGridViewElementStates.Selected | DataGridViewElementStates.Visible)) == 0);
            Debug.Assert((includeFilter & DataGridViewElementStates.Resizable) == 0);
            Debug.Assert(DisplayInOrder(fromColumnIndex, toColumnIndex));
            Debug.Assert(((DataGridViewColumn) this.items[toColumnIndex]).StateIncludes(includeFilter));

            int jumpColumns = 0;
            DataGridViewColumn dataGridViewColumn = (DataGridViewColumn) this.items[fromColumnIndex];
            
            while (dataGridViewColumn != (DataGridViewColumn) this.items[toColumnIndex])
            {
                dataGridViewColumn = GetNextColumn(dataGridViewColumn, includeFilter,
                    DataGridViewElementStates.None);
                Debug.Assert(dataGridViewColumn != null);
                if (dataGridViewColumn.StateIncludes(includeFilter))
                {
                    jumpColumns++;
                }
            }
            return jumpColumns;
        }

        private int GetColumnSortedIndex(DataGridViewColumn dataGridViewColumn)
        {
            Debug.Assert(dataGridViewColumn != null);
            Debug.Assert(this.itemsSorted != null);
            Debug.Assert(this.lastAccessedSortedIndex == -1 ||
                this.lastAccessedSortedIndex < this.Count);

#if DEBUG
            Debug.Assert(VerifyColumnOrderCache());
#endif
            if (this.lastAccessedSortedIndex != -1 && 
                this.itemsSorted[this.lastAccessedSortedIndex] == dataGridViewColumn)
            {
                return this.lastAccessedSortedIndex;
            }

            int index = 0;
            while (index < this.itemsSorted.Count)
            {
                if (dataGridViewColumn.Index == ((DataGridViewColumn) this.itemsSorted[index]).Index)
                {
                    this.lastAccessedSortedIndex = index;
                    return index;
                }
                index++;
            }
            return -1;
        }

        internal float GetColumnsFillWeight(DataGridViewElementStates includeFilter)
        {
            Debug.Assert((includeFilter & ~(DataGridViewElementStates.Displayed | DataGridViewElementStates.Frozen | DataGridViewElementStates.Resizable |
                         DataGridViewElementStates.ReadOnly | DataGridViewElementStates.Selected | DataGridViewElementStates.Visible)) == 0);

            float weightSum = 0F;
            for (int columnIndex = 0; columnIndex < this.items.Count; columnIndex++)
            {
                if (((DataGridViewColumn)this.items[columnIndex]).StateIncludes(includeFilter))
                {
                    weightSum += ((DataGridViewColumn)this.items[columnIndex]).FillWeight;
                }
            }
            return weightSum;
        }

        /// <include file='doc\DataGridViewColumnCollection.uex' path='docs/doc[@for="DataGridViewColumnCollection.GetColumnsWidth"]/*' />
        public int GetColumnsWidth(DataGridViewElementStates includeFilter)
        {
            if ((includeFilter & ~(DataGridViewElementStates.Displayed | DataGridViewElementStates.Frozen | DataGridViewElementStates.Resizable |
                DataGridViewElementStates.ReadOnly | DataGridViewElementStates.Selected | DataGridViewElementStates.Visible)) != 0)
            {
                throw new ArgumentException(string.Format(SR.DataGridView_InvalidDataGridViewElementStateCombination, "includeFilter"));
            }

            // cache returned value and reuse it as long as none
            // of the column's state/thickness has changed.
            switch (includeFilter)
            {
                case DataGridViewElementStates.Visible:
                    if (this.columnsWidthVisible != -1)
                    {
                        return this.columnsWidthVisible;
                    }
                    break;
                case DataGridViewElementStates.Visible | DataGridViewElementStates.Frozen:
                    if (this.columnsWidthVisibleFrozen != -1)
                    {
                        return this.columnsWidthVisibleFrozen;
                    }
                    break;
            }

            int columnsWidth = 0;
            for(int columnIndex = 0; columnIndex < this.items.Count; columnIndex++)
            {
                if (((DataGridViewColumn) this.items[columnIndex]).StateIncludes(includeFilter))
                {
                    columnsWidth += ((DataGridViewColumn) this.items[columnIndex]).Thickness;
                }
            }

            switch (includeFilter)
            {
                case DataGridViewElementStates.Visible:
                    this.columnsWidthVisible = columnsWidth;
                    break;
                case DataGridViewElementStates.Visible | DataGridViewElementStates.Frozen:
                    this.columnsWidthVisibleFrozen = columnsWidth;
                    break;
            }
            return columnsWidth;
        }

        /// <include file='doc\DataGridViewColumnCollection.uex' path='docs/doc[@for="DataGridViewColumnCollection.GetFirstColumn"]/*' />
        public DataGridViewColumn GetFirstColumn(DataGridViewElementStates includeFilter)
        {
            if ((includeFilter & ~(DataGridViewElementStates.Displayed | DataGridViewElementStates.Frozen | DataGridViewElementStates.Resizable |
                DataGridViewElementStates.ReadOnly | DataGridViewElementStates.Selected | DataGridViewElementStates.Visible)) != 0)
            {
                throw new ArgumentException(string.Format(SR.DataGridView_InvalidDataGridViewElementStateCombination, "includeFilter"));
            }

            if (this.itemsSorted == null)
            {
                UpdateColumnOrderCache();
            }
#if DEBUG
            Debug.Assert(VerifyColumnOrderCache());
#endif
            int index = 0;
            while (index < this.itemsSorted.Count)
            {
                DataGridViewColumn dataGridViewColumn = (DataGridViewColumn)this.itemsSorted[index];
                if (dataGridViewColumn.StateIncludes(includeFilter))
                {
                    this.lastAccessedSortedIndex = index;
                    return dataGridViewColumn;
                }
                index++;
            }
            return null;
        }

        /// <include file='doc\DataGridViewColumnCollection.uex' path='docs/doc[@for="DataGridViewColumnCollection.GetFirstColumn2"]/*' />
        public DataGridViewColumn GetFirstColumn(DataGridViewElementStates includeFilter,
                                                 DataGridViewElementStates excludeFilter)
        {
            if (excludeFilter == DataGridViewElementStates.None)
            {
                return GetFirstColumn(includeFilter);
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

            if (this.itemsSorted == null)
            {
                UpdateColumnOrderCache();
            }
#if DEBUG
            Debug.Assert(VerifyColumnOrderCache());
#endif
            int index = 0;
            while (index < this.itemsSorted.Count)
            {
                DataGridViewColumn dataGridViewColumn = (DataGridViewColumn)this.itemsSorted[index];
                if (dataGridViewColumn.StateIncludes(includeFilter) &&
                    dataGridViewColumn.StateExcludes(excludeFilter))
                {
                    this.lastAccessedSortedIndex = index;
                    return dataGridViewColumn;
                }
                index++;
            }
            return null;
        }

        /// <include file='doc\DataGridViewColumnCollection.uex' path='docs/doc[@for="DataGridViewColumnCollection.GetLastColumn"]/*' />
        public DataGridViewColumn GetLastColumn(DataGridViewElementStates includeFilter,
                                                DataGridViewElementStates excludeFilter)
        {
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

            if (this.itemsSorted == null)
            {
                UpdateColumnOrderCache();
            }
#if DEBUG
            Debug.Assert(VerifyColumnOrderCache());
#endif
            int index = this.itemsSorted.Count - 1;
            while (index >= 0)
            {
                DataGridViewColumn dataGridViewColumn = (DataGridViewColumn) this.itemsSorted[index];
                if (dataGridViewColumn.StateIncludes(includeFilter) && 
                    dataGridViewColumn.StateExcludes(excludeFilter))
                {
                    this.lastAccessedSortedIndex = index;
                    return dataGridViewColumn;
                }
                index--;
            }
            return null;
        }

        /// <include file='doc\DataGridViewColumnCollection.uex' path='docs/doc[@for="DataGridViewColumnCollection.GetNextColumn"]/*' />
        public DataGridViewColumn GetNextColumn(DataGridViewColumn dataGridViewColumnStart,
                                                DataGridViewElementStates includeFilter,
                                                DataGridViewElementStates excludeFilter)
        {
            if (dataGridViewColumnStart == null)
            {
                throw new ArgumentNullException(nameof(dataGridViewColumnStart));
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

            if (this.itemsSorted == null)
            {
                UpdateColumnOrderCache();
            }
#if DEBUG
            Debug.Assert(VerifyColumnOrderCache());
#endif
            int index = GetColumnSortedIndex(dataGridViewColumnStart);
            if (index == -1)
            {
                bool columnFound = false;
                int indexMin = Int32.MaxValue, displayIndexMin = Int32.MaxValue;
                for (index = 0; index < this.items.Count; index++)
                {
                    DataGridViewColumn dataGridViewColumn = (DataGridViewColumn) this.items[index];
                    if (dataGridViewColumn.StateIncludes(includeFilter) && 
                        dataGridViewColumn.StateExcludes(excludeFilter) && 
                        (dataGridViewColumn.DisplayIndex > dataGridViewColumnStart.DisplayIndex || 
                         (dataGridViewColumn.DisplayIndex == dataGridViewColumnStart.DisplayIndex && 
                          dataGridViewColumn.Index > dataGridViewColumnStart.Index)))
                    {
                        if (dataGridViewColumn.DisplayIndex < displayIndexMin || 
                            (dataGridViewColumn.DisplayIndex == displayIndexMin && 
                             dataGridViewColumn.Index < indexMin))
                        {
                            indexMin = index;
                            displayIndexMin = dataGridViewColumn.DisplayIndex;
                            columnFound = true;
                        }
                    }
                }
                return columnFound ? ((DataGridViewColumn) this.items[indexMin]) : null;
            }
            else
            {
                index++;
                while (index < this.itemsSorted.Count)
                {
                    DataGridViewColumn dataGridViewColumn = (DataGridViewColumn)this.itemsSorted[index];

                    if (dataGridViewColumn.StateIncludes (includeFilter) && dataGridViewColumn.StateExcludes (excludeFilter))
                    {
                        this.lastAccessedSortedIndex = index;
                        return dataGridViewColumn;
                    }

                    index++;
                }
            }
            return null;
        }

        /// <include file='doc\DataGridViewColumnCollection.uex' path='docs/doc[@for="DataGridViewColumnCollection.GetPreviousColumn"]/*' />
        public DataGridViewColumn GetPreviousColumn(DataGridViewColumn dataGridViewColumnStart, 
                                                             DataGridViewElementStates includeFilter,
                                                             DataGridViewElementStates excludeFilter)
        {
            if (dataGridViewColumnStart == null)
            {
                throw new ArgumentNullException(nameof(dataGridViewColumnStart));
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

            if (this.itemsSorted == null)
            {
                UpdateColumnOrderCache();
            }
#if DEBUG
            Debug.Assert(VerifyColumnOrderCache());
#endif
            int index = GetColumnSortedIndex(dataGridViewColumnStart);
            if (index == -1)
            {
                bool columnFound = false;
                int indexMax = -1, displayIndexMax = -1;
                for (index = 0; index < this.items.Count; index++)
                {
                    DataGridViewColumn dataGridViewColumn = (DataGridViewColumn)this.items[index];
                    if (dataGridViewColumn.StateIncludes(includeFilter) && 
                        dataGridViewColumn.StateExcludes(excludeFilter) && 
                        (dataGridViewColumn.DisplayIndex < dataGridViewColumnStart.DisplayIndex || 
                         (dataGridViewColumn.DisplayIndex == dataGridViewColumnStart.DisplayIndex && 
                          dataGridViewColumn.Index < dataGridViewColumnStart.Index)))
                    {
                        if (dataGridViewColumn.DisplayIndex > displayIndexMax || 
                            (dataGridViewColumn.DisplayIndex == displayIndexMax && 
                             dataGridViewColumn.Index > indexMax))
                        {
                            indexMax = index;
                            displayIndexMax = dataGridViewColumn.DisplayIndex;
                            columnFound = true;
                        }
                    }
                }
                return columnFound ? ((DataGridViewColumn) this.items[indexMax]) : null;
            }
            else
            {
                index--;
                while (index >= 0)
                {
                    DataGridViewColumn dataGridViewColumn = (DataGridViewColumn)this.itemsSorted[index];
                    if (dataGridViewColumn.StateIncludes(includeFilter) && 
                        dataGridViewColumn.StateExcludes(excludeFilter))
                    {
                        this.lastAccessedSortedIndex = index;
                        return dataGridViewColumn;
                    }
                    index--;
                }
            }
            return null;
        }

        /// <include file='doc\DataGridViewColumnCollection.uex' path='docs/doc[@for="DataGridViewColumnCollection.IndexOf"]/*' />
        public int IndexOf(DataGridViewColumn dataGridViewColumn)
        {
            return this.items.IndexOf(dataGridViewColumn);
        }

        /// <include file='doc\DataGridViewColumnCollection.uex' path='docs/doc[@for="DataGridViewColumnCollection.Insert"]/*' />
        /// <devdoc>
        /// <para>Inserts a <see cref='System.Windows.Forms.DataGridViewColumn'/> in this collection.</para>
        /// </devdoc>
        public virtual void Insert(int columnIndex, DataGridViewColumn dataGridViewColumn)
        {
            Debug.Assert(this.DataGridView != null);
            if (this.DataGridView.NoDimensionChangeAllowed)
            {
                throw new InvalidOperationException(string.Format(SR.DataGridView_ForbiddenOperationInEventHandler));
            }
            if (this.DataGridView.InDisplayIndexAdjustments)
            {
                // We are within columns display indexes adjustments. We do not allow changing the column collection while adjusting display indexes.
                throw new InvalidOperationException(string.Format(SR.DataGridView_CannotAlterDisplayIndexWithinAdjustments));
            }
            if (dataGridViewColumn == null)
            {
                throw new ArgumentNullException(nameof(dataGridViewColumn));
            }
            int originalDisplayIndex = dataGridViewColumn.DisplayIndex;
            if (originalDisplayIndex == -1)
            {
                dataGridViewColumn.DisplayIndex = columnIndex;
            }
            Point newCurrentCell;
            try
            {
                this.DataGridView.OnInsertingColumn(columnIndex, dataGridViewColumn, out newCurrentCell);   // will throw an exception if the insertion is illegal
            }
            finally
            {
                dataGridViewColumn.DisplayIndexInternal = originalDisplayIndex;
            }
            InvalidateCachedColumnsOrder();
            this.items.Insert(columnIndex, dataGridViewColumn);
            dataGridViewColumn.IndexInternal = columnIndex;
            dataGridViewColumn.DataGridViewInternal = dataGridView;
            UpdateColumnCaches(dataGridViewColumn, true);
            this.DataGridView.OnInsertedColumn_PreNotification(dataGridViewColumn);
            OnCollectionChanged(new CollectionChangeEventArgs(CollectionChangeAction.Add, dataGridViewColumn), true /*changeIsInsertion*/, newCurrentCell);
#if DEBUG
            Debug.Assert(this.itemsSorted == null || VerifyColumnOrderCache());
#endif
        }

        internal void InvalidateCachedColumnCount(DataGridViewElementStates includeFilter)
        {
            Debug.Assert(includeFilter == DataGridViewElementStates.Displayed ||
                         includeFilter == DataGridViewElementStates.Selected ||
                         includeFilter == DataGridViewElementStates.ReadOnly ||
                         includeFilter == DataGridViewElementStates.Resizable ||
                         includeFilter == DataGridViewElementStates.Frozen ||
                         includeFilter == DataGridViewElementStates.Visible);

            if (includeFilter == DataGridViewElementStates.Visible)
            {
                InvalidateCachedColumnCounts();
            }
            else if (includeFilter == DataGridViewElementStates.Selected)
            {
                this.columnCountsVisibleSelected = -1;
            }
        }

        internal void InvalidateCachedColumnCounts()
        {
            this.columnCountsVisible = this.columnCountsVisibleSelected = -1;
        }

        internal void InvalidateCachedColumnsOrder()
        {
            this.itemsSorted = null;
        }

        internal void InvalidateCachedColumnsWidth(DataGridViewElementStates includeFilter)
        {
            Debug.Assert(includeFilter == DataGridViewElementStates.Displayed ||
                         includeFilter == DataGridViewElementStates.Selected ||
                         includeFilter == DataGridViewElementStates.ReadOnly ||
                         includeFilter == DataGridViewElementStates.Resizable ||
                         includeFilter == DataGridViewElementStates.Frozen ||
                         includeFilter == DataGridViewElementStates.Visible);

            if (includeFilter == DataGridViewElementStates.Visible)
            {
                InvalidateCachedColumnsWidths();
            }
            else if (includeFilter == DataGridViewElementStates.Frozen)
            {
                this.columnsWidthVisibleFrozen = -1;
            }
        }

        internal void InvalidateCachedColumnsWidths()
        {
            this.columnsWidthVisible = this.columnsWidthVisibleFrozen = -1;
        }

        /// <include file='doc\DataGridViewColumnCollection.uex' path='docs/doc[@for="DataGridViewColumnCollection.OnCollectionChanged"]/*' />
        protected virtual void OnCollectionChanged(CollectionChangeEventArgs e)
        {
            if (this.onCollectionChanged != null)
            {
                this.onCollectionChanged(this, e);
            }
        }

        private void OnCollectionChanged(CollectionChangeEventArgs ccea, bool changeIsInsertion, Point newCurrentCell)
        {
#if DEBUG
            Debug.Assert(VerifyColumnDisplayIndexes());
#endif
            OnCollectionChanged_PreNotification(ccea);
            OnCollectionChanged(ccea);
            OnCollectionChanged_PostNotification(ccea, changeIsInsertion, newCurrentCell);
        }

        private void OnCollectionChanged_PreNotification(CollectionChangeEventArgs ccea)
        {
            Debug.Assert(this.DataGridView != null);
            this.DataGridView.OnColumnCollectionChanged_PreNotification(ccea);
        }

        private void OnCollectionChanged_PostNotification(CollectionChangeEventArgs ccea, bool changeIsInsertion, Point newCurrentCell)
        {
            Debug.Assert(this.DataGridView != null);
            DataGridViewColumn dataGridViewColumn = (DataGridViewColumn)ccea.Element;
            if (ccea.Action == CollectionChangeAction.Add && changeIsInsertion)
            {
                this.DataGridView.OnInsertedColumn_PostNotification(newCurrentCell);
            }
            else if (ccea.Action == CollectionChangeAction.Remove)
            {
                this.DataGridView.OnRemovedColumn_PostNotification(dataGridViewColumn, newCurrentCell);
            }

            this.DataGridView.OnColumnCollectionChanged_PostNotification(dataGridViewColumn);
        }

        /// <include file='doc\DataGridViewColumnCollection.uex' path='docs/doc[@for="DataGridViewColumnCollection.Remove"]/*' />
        [
            SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters") // We don't want to use DataGridViewElement here.
        ]
        public virtual void Remove(DataGridViewColumn dataGridViewColumn)
        {
            if (dataGridViewColumn == null)
            {
                throw new ArgumentNullException(nameof(dataGridViewColumn));
            }

            if (dataGridViewColumn.DataGridView != this.DataGridView)
            {
                throw new ArgumentException(string.Format(SR.DataGridView_ColumnDoesNotBelongToDataGridView), "dataGridViewColumn");
            }

            int itemsCount = this.items.Count;
            for (int i = 0; i < itemsCount; ++i)
            {
                if (this.items[i] == dataGridViewColumn)
                {
                    RemoveAt(i);
#if DEBUG
                    Debug.Assert(this.itemsSorted == null || VerifyColumnOrderCache());
#endif
                    return;
                }
            }

            Debug.Fail("Column should have been found in DataGridViewColumnCollection.Remove");
        }

        /// <include file='doc\DataGridViewColumnCollection.uex' path='docs/doc[@for="DataGridViewColumnCollection.Remove2"]/*' />
        public virtual void Remove(string columnName)
        {
            if (columnName == null)
            {
                throw new ArgumentNullException(nameof(columnName));
            }

            int itemsCount = this.items.Count;
            for (int i = 0; i < itemsCount; ++i)
            {
                DataGridViewColumn dataGridViewColumn = (DataGridViewColumn) this.items[i];
                // NOTE: case-insensitive
                if (0 == String.Compare(dataGridViewColumn.Name, columnName, true, CultureInfo.InvariantCulture))
                {
                    RemoveAt(i);
                    return;
                }
            }

            throw new ArgumentException(string.Format(SR.DataGridViewColumnCollection_ColumnNotFound, columnName), "columnName");
        }

        /// <include file='doc\DataGridViewColumnCollection.uex' path='docs/doc[@for="DataGridViewColumnCollection.RemoveAt"]/*' />
        public virtual void RemoveAt(int index)
        {
            if (index < 0 || index >= this.Count)
            {
                throw new ArgumentOutOfRangeException(nameof(index), string.Format(SR.InvalidArgument, "index", (index).ToString(CultureInfo.CurrentCulture)));
            }

            if (this.DataGridView.NoDimensionChangeAllowed)
            {
                throw new InvalidOperationException(string.Format(SR.DataGridView_ForbiddenOperationInEventHandler));
            }

            if (this.DataGridView.InDisplayIndexAdjustments)
            {
                // We are within columns display indexes adjustments. We do not allow changing the column collection while adjusting display indexes.
                throw new InvalidOperationException(string.Format(SR.DataGridView_CannotAlterDisplayIndexWithinAdjustments));
            }

            RemoveAtInternal(index, false /*force*/);
#if DEBUG
            Debug.Assert(this.itemsSorted == null || VerifyColumnOrderCache());
#endif
        }

        internal void RemoveAtInternal(int index, bool force)
        {
            // If force is true, the underlying data is gone and can't be accessed anymore.

            Debug.Assert(index >= 0 && index < this.Count);
            Debug.Assert(this.DataGridView != null);
            Debug.Assert(!this.DataGridView.NoDimensionChangeAllowed);
            Debug.Assert(!this.DataGridView.InDisplayIndexAdjustments);

            DataGridViewColumn dataGridViewColumn = (DataGridViewColumn)this.items[index];
            Point newCurrentCell; 
            this.DataGridView.OnRemovingColumn(dataGridViewColumn, out newCurrentCell, force);
            InvalidateCachedColumnsOrder();
            this.items.RemoveAt(index);
            dataGridViewColumn.DataGridViewInternal = null;
            UpdateColumnCaches(dataGridViewColumn, false);
            this.DataGridView.OnRemovedColumn_PreNotification(dataGridViewColumn);
            OnCollectionChanged(new CollectionChangeEventArgs(CollectionChangeAction.Remove, dataGridViewColumn), false /*changeIsInsertion*/, newCurrentCell);
        }

        private void UpdateColumnCaches(DataGridViewColumn dataGridViewColumn, bool adding)
        {
            if (this.columnCountsVisible != -1 || this.columnCountsVisibleSelected != -1 ||
                this.columnsWidthVisible != -1 || this.columnsWidthVisibleFrozen != -1)
            {
                DataGridViewElementStates columnStates = dataGridViewColumn.State;
                if ((columnStates & DataGridViewElementStates.Visible) != 0)
                {
                    int columnCountIncrement = adding ? 1 : -1;
                    int columnWidthIncrement = 0;
                    if (this.columnsWidthVisible != -1 ||
                        (this.columnsWidthVisibleFrozen != -1 &&
                         ((columnStates & (DataGridViewElementStates.Visible | DataGridViewElementStates.Frozen)) == (DataGridViewElementStates.Visible | DataGridViewElementStates.Frozen))))
                    {
                        columnWidthIncrement = adding ? dataGridViewColumn.Width : -dataGridViewColumn.Width;
                    }

                    if (this.columnCountsVisible != -1)
                    {
                        this.columnCountsVisible += columnCountIncrement;
                    }
                    if (this.columnsWidthVisible != -1)
                    {
                        Debug.Assert(columnWidthIncrement != 0);
                        this.columnsWidthVisible += columnWidthIncrement;
                    }

                    if ((columnStates & (DataGridViewElementStates.Visible | DataGridViewElementStates.Frozen)) == (DataGridViewElementStates.Visible | DataGridViewElementStates.Frozen))
                    {
                        if (this.columnsWidthVisibleFrozen != -1)
                        {
                            Debug.Assert(columnWidthIncrement != 0);
                            this.columnsWidthVisibleFrozen += columnWidthIncrement;
                        }
                    }

                    if ((columnStates & (DataGridViewElementStates.Visible | DataGridViewElementStates.Selected)) == (DataGridViewElementStates.Visible | DataGridViewElementStates.Selected))
                    {
                        if (this.columnCountsVisibleSelected != -1)
                        {
                            this.columnCountsVisibleSelected += columnCountIncrement;
                        }
                    }
                }
            }
        }

        private void UpdateColumnOrderCache()
        {
            this.itemsSorted = (ArrayList) this.items.Clone();
            this.itemsSorted.Sort(columnOrderComparer);
            this.lastAccessedSortedIndex = -1;
        }

#if DEBUG
        internal bool VerifyColumnDisplayIndexes()
        {
            for (int columnDisplayIndex = 0; columnDisplayIndex < this.items.Count; columnDisplayIndex++)
            {
                if (GetColumnAtDisplayIndex(columnDisplayIndex) == null)
                {
                    return false;
                }
            }
            return true;
        }

        private bool VerifyColumnOrderCache()
        {
            if (this.itemsSorted == null) return false;
            if (this.itemsSorted.Count != this.items.Count) return false;

            int index = 0;
            while (index < this.itemsSorted.Count-1)
            {
                if (((DataGridViewColumn) this.itemsSorted[index+1]).DisplayIndex != 
                    ((DataGridViewColumn) this.itemsSorted[index]).DisplayIndex+1) return false;
                index++;
            }
            return true;
        }
#endif

        private class ColumnOrderComparer : IComparer
        {
            public ColumnOrderComparer()
            {
            }

            public Int32 Compare(Object x, Object y)
            {
                Debug.Assert(x != null);
                Debug.Assert(y != null);

                DataGridViewColumn dataGridViewColumn1 = x as DataGridViewColumn;
                DataGridViewColumn dataGridViewColumn2 = y as DataGridViewColumn;

                Debug.Assert(dataGridViewColumn1 != null);
                Debug.Assert(dataGridViewColumn2 != null);

                return dataGridViewColumn1.DisplayIndex - dataGridViewColumn2.DisplayIndex;
            }
        }
    }
}
