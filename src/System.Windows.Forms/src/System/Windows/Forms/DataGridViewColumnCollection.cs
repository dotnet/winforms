// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable disable

using System.Collections;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;

namespace System.Windows.Forms
{
    /// <summary>
    ///  Represents a collection of <see cref='DataGridViewColumn'/> objects in the
    ///  <see cref='DataGridView'/> control.
    /// </summary>
    [ListBindable(false)]
    public class DataGridViewColumnCollection : BaseCollection, IList
    {
        private CollectionChangeEventHandler _onCollectionChanged;
        private readonly ArrayList _items = new ArrayList();
        private ArrayList _itemsSorted;
        private int _lastAccessedSortedIndex = -1;
        private int _columnCountsVisible, _columnCountsVisibleSelected;
        private int _columnsWidthVisible, _columnsWidthVisibleFrozen;
        private static readonly ColumnOrderComparer s_columnOrderComparer = new ColumnOrderComparer();

        /* IList interface implementation */

        bool IList.IsFixedSize
        {
            get { return false; }
        }

        bool IList.IsReadOnly
        {
            get { return false; }
        }

        object IList.this[int index]
        {
            get { return this[index]; }
            set { throw new NotSupportedException(); }
        }

        int IList.Add(object value)
        {
            return Add((DataGridViewColumn)value);
        }

        void IList.Clear()
        {
            Clear();
        }

        bool IList.Contains(object value)
        {
            return _items.Contains(value);
        }

        int IList.IndexOf(object value)
        {
            return _items.IndexOf(value);
        }

        void IList.Insert(int index, object value)
        {
            Insert(index, (DataGridViewColumn)value);
        }

        void IList.Remove(object value)
        {
            Remove((DataGridViewColumn)value);
        }

        void IList.RemoveAt(int index)
        {
            RemoveAt(index);
        }

        /* ICollection interface implementation */

        int ICollection.Count
        {
            get
            {
                return _items.Count;
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

        void ICollection.CopyTo(Array array, int index)
        {
            _items.CopyTo(array, index);
        }

        /* IEnumerable interface implementation */

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _items.GetEnumerator();
        }

        public DataGridViewColumnCollection(DataGridView dataGridView)
        {
            InvalidateCachedColumnCounts();
            InvalidateCachedColumnsWidths();
            this.DataGridView = dataGridView;
        }

        internal static IComparer ColumnCollectionOrderComparer
        {
            get
            {
                return System.Windows.Forms.DataGridViewColumnCollection.s_columnOrderComparer;
            }
        }

        protected override ArrayList List
        {
            get
            {
                return _items;
            }
        }

        protected DataGridView DataGridView { get; }

        /// <summary>
        ///  Retrieves the DataGridViewColumn with the specified index.
        /// </summary>
        public DataGridViewColumn this[int index]
        {
            get
            {
                return (DataGridViewColumn)_items[index];
            }
        }

        /// <summary>
        ///  Retrieves the DataGridViewColumn with the Name provided.
        /// </summary>
        public DataGridViewColumn this[string columnName]
        {
            get
            {
                if (columnName is null)
                {
                    throw new ArgumentNullException(nameof(columnName));
                }
                int itemCount = _items.Count;
                for (int i = 0; i < itemCount; ++i)
                {
                    DataGridViewColumn dataGridViewColumn = (DataGridViewColumn)_items[i];
                    // NOTE: case-insensitive
                    if (string.Equals(dataGridViewColumn.Name, columnName, StringComparison.OrdinalIgnoreCase))
                    {
                        return dataGridViewColumn;
                    }
                }
                return null;
            }
        }

        public event CollectionChangeEventHandler CollectionChanged
        {
            add => _onCollectionChanged += value;
            remove => _onCollectionChanged -= value;
        }

        internal int ActualDisplayIndexToColumnIndex(int actualDisplayIndex, DataGridViewElementStates includeFilter)
        {
            // Microsoft: is there a faster way to get the column index?
            DataGridViewColumn dataGridViewColumn = GetFirstColumn(includeFilter);
            for (int i = 0; i < actualDisplayIndex; i++)
            {
                dataGridViewColumn = GetNextColumn(dataGridViewColumn, includeFilter, DataGridViewElementStates.None);
            }
            return dataGridViewColumn.Index;
        }

        /// <summary>
        ///  Adds a <see cref='DataGridViewColumn'/> to this collection.
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public virtual int Add(string columnName, string headerText)
        {
            DataGridViewTextBoxColumn dataGridViewTextBoxColumn = new DataGridViewTextBoxColumn
            {
                Name = columnName,
                HeaderText = headerText
            };

            return Add(dataGridViewTextBoxColumn);
        }

        /// <summary>
        ///  Adds a <see cref='DataGridViewColumn'/> to this collection.
        /// </summary>
        public virtual int Add(DataGridViewColumn dataGridViewColumn)
        {
            Debug.Assert(DataGridView != null);
            if (DataGridView.NoDimensionChangeAllowed)
            {
                throw new InvalidOperationException(SR.DataGridView_ForbiddenOperationInEventHandler);
            }
            if (DataGridView.InDisplayIndexAdjustments)
            {
                // We are within columns display indexes adjustments. We do not allow changing the column collection while adjusting display indexes.
                throw new InvalidOperationException(SR.DataGridView_CannotAlterDisplayIndexWithinAdjustments);
            }

            DataGridView.OnAddingColumn(dataGridViewColumn);   // will throw an exception if the addition is illegal

            InvalidateCachedColumnsOrder();
            int index = _items.Add(dataGridViewColumn);
            dataGridViewColumn.Index = index;
            dataGridViewColumn.DataGridView = DataGridView;
            UpdateColumnCaches(dataGridViewColumn, true);
            DataGridView.OnAddedColumn(dataGridViewColumn);
            OnCollectionChanged(new CollectionChangeEventArgs(CollectionChangeAction.Add, dataGridViewColumn), false /*changeIsInsertion*/, new Point(-1, -1));
#if DEBUG
            Debug.Assert(_itemsSorted is null || VerifyColumnOrderCache());
#endif
            return index;
        }

        public virtual void AddRange(params DataGridViewColumn[] dataGridViewColumns)
        {
            if (dataGridViewColumns is null)
            {
                throw new ArgumentNullException(nameof(dataGridViewColumns));
            }

            Debug.Assert(DataGridView != null);
            if (DataGridView.NoDimensionChangeAllowed)
            {
                throw new InvalidOperationException(SR.DataGridView_ForbiddenOperationInEventHandler);
            }
            if (DataGridView.InDisplayIndexAdjustments)
            {
                // We are within columns display indexes adjustments. We do not allow changing the column collection while adjusting display indexes.
                throw new InvalidOperationException(SR.DataGridView_CannotAlterDisplayIndexWithinAdjustments);
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
                smallestDisplayIndex = int.MaxValue;
                smallestIndex = -1;
                for (index = 0; index < initialColumns.Count; index++)
                {
                    DataGridViewColumn dataGridViewColumn = (DataGridViewColumn)initialColumns[index];
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

            DataGridView.OnAddingColumns(dataGridViewColumns);   // will throw an exception if the addition is illegal

            foreach (DataGridViewColumn dataGridViewColumn in dataGridViewColumns)
            {
                InvalidateCachedColumnsOrder();
                index = _items.Add(dataGridViewColumn);
                dataGridViewColumn.Index = index;
                dataGridViewColumn.DataGridView = DataGridView;
                UpdateColumnCaches(dataGridViewColumn, true);
                DataGridView.OnAddedColumn(dataGridViewColumn);
            }

            OnCollectionChanged(new CollectionChangeEventArgs(CollectionChangeAction.Refresh, null), false /*changeIsInsertion*/, new Point(-1, -1));
#if DEBUG
            Debug.Assert(_itemsSorted is null || VerifyColumnOrderCache());
#endif
        }

        public virtual void Clear()
        {
            if (Count > 0)
            {
                if (DataGridView.NoDimensionChangeAllowed)
                {
                    throw new InvalidOperationException(SR.DataGridView_ForbiddenOperationInEventHandler);
                }
                if (DataGridView.InDisplayIndexAdjustments)
                {
                    // We are within columns display indexes adjustments. We do not allow changing the column collection while adjusting display indexes.
                    throw new InvalidOperationException(SR.DataGridView_CannotAlterDisplayIndexWithinAdjustments);
                }

                for (int columnIndex = 0; columnIndex < Count; columnIndex++)
                {
                    DataGridViewColumn dataGridViewColumn = this[columnIndex];
                    // Detach the column...
                    dataGridViewColumn.DataGridView = null;
                    // ...and its potential header cell
                    if (dataGridViewColumn.HasHeaderCell)
                    {
                        dataGridViewColumn.HeaderCell.DataGridView = null;
                    }
                }

                DataGridViewColumn[] aColumns = new DataGridViewColumn[_items.Count];
                CopyTo(aColumns, 0);

                DataGridView.OnClearingColumns();
                InvalidateCachedColumnsOrder();
                _items.Clear();
                InvalidateCachedColumnCounts();
                InvalidateCachedColumnsWidths();
                foreach (DataGridViewColumn dataGridViewColumn in aColumns)
                {
                    DataGridView.OnColumnRemoved(dataGridViewColumn);
                    DataGridView.OnColumnHidden(dataGridViewColumn);
                }
                OnCollectionChanged(new CollectionChangeEventArgs(CollectionChangeAction.Refresh, null), false /*changeIsInsertion*/, new Point(-1, -1));
#if DEBUG
                Debug.Assert(_itemsSorted is null || VerifyColumnOrderCache());
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
                actualDisplayIndex++;
            }
            return actualDisplayIndex;
        }

        /// <summary>
        ///  Checks to see if a DataGridViewColumn is contained in this collection.
        /// </summary>
        public virtual bool Contains(DataGridViewColumn dataGridViewColumn)
        {
            return _items.IndexOf(dataGridViewColumn) != -1;
        }

        public virtual bool Contains(string columnName)
        {
            if (columnName is null)
            {
                throw new ArgumentNullException(nameof(columnName));
            }
            int itemCount = _items.Count;
            for (int i = 0; i < itemCount; ++i)
            {
                DataGridViewColumn dataGridViewColumn = (DataGridViewColumn)_items[i];
                // NOTE: case-insensitive
                if (0 == string.Compare(dataGridViewColumn.Name, columnName, true, CultureInfo.InvariantCulture))
                {
                    return true;
                }
            }
            return false;
        }

        public void CopyTo(DataGridViewColumn[] array, int index)
        {
            _items.CopyTo(array, index);
        }

        internal bool DisplayInOrder(int columnIndex1, int columnIndex2)
        {
            int displayIndex1 = ((DataGridViewColumn)_items[columnIndex1]).DisplayIndex;
            int displayIndex2 = ((DataGridViewColumn)_items[columnIndex2]).DisplayIndex;
            return displayIndex1 < displayIndex2;
        }

        internal DataGridViewColumn GetColumnAtDisplayIndex(int displayIndex)
        {
            if (displayIndex < 0 || displayIndex >= _items.Count)
            {
                return null;
            }
            DataGridViewColumn dataGridViewColumn = ((DataGridViewColumn)_items[displayIndex]);
            if (dataGridViewColumn.DisplayIndex == displayIndex)
            {
                // Performance gain if display indexes coincide with indexes.
                return dataGridViewColumn;
            }
            for (int columnIndex = 0; columnIndex < _items.Count; columnIndex++)
            {
                dataGridViewColumn = ((DataGridViewColumn)_items[columnIndex]);
                if (dataGridViewColumn.DisplayIndex == displayIndex)
                {
                    return dataGridViewColumn;
                }
            }
            Debug.Fail("no column found in GetColumnAtDisplayIndex");
            return null;
        }

        public int GetColumnCount(DataGridViewElementStates includeFilter)
        {
            if ((includeFilter & ~(DataGridViewElementStates.Displayed | DataGridViewElementStates.Frozen | DataGridViewElementStates.Resizable |
                DataGridViewElementStates.ReadOnly | DataGridViewElementStates.Selected | DataGridViewElementStates.Visible)) != 0)
            {
                throw new ArgumentException(string.Format(SR.DataGridView_InvalidDataGridViewElementStateCombination, nameof(includeFilter)));
            }

            // cache returned value and reuse it as long as none
            // of the column's state has changed.
            switch (includeFilter)
            {
                case DataGridViewElementStates.Visible:
                    if (_columnCountsVisible != -1)
                    {
                        return _columnCountsVisible;
                    }
                    break;
                case DataGridViewElementStates.Visible | DataGridViewElementStates.Selected:
                    if (_columnCountsVisibleSelected != -1)
                    {
                        return _columnCountsVisibleSelected;
                    }
                    break;
            }

            int columnCount = 0;
            if ((includeFilter & DataGridViewElementStates.Resizable) == 0)
            {
                for (int columnIndex = 0; columnIndex < _items.Count; columnIndex++)
                {
                    if (((DataGridViewColumn)_items[columnIndex]).StateIncludes(includeFilter))
                    {
                        columnCount++;
                    }
                }
                switch (includeFilter)
                {
                    case DataGridViewElementStates.Visible:
                        _columnCountsVisible = columnCount;
                        break;
                    case DataGridViewElementStates.Visible | DataGridViewElementStates.Selected:
                        _columnCountsVisibleSelected = columnCount;
                        break;
                }
            }
            else
            {
                DataGridViewElementStates correctedIncludeFilter = includeFilter & ~DataGridViewElementStates.Resizable;
                for (int columnIndex = 0; columnIndex < _items.Count; columnIndex++)
                {
                    if (((DataGridViewColumn)_items[columnIndex]).StateIncludes(correctedIncludeFilter) &&
                        ((DataGridViewColumn)_items[columnIndex]).Resizable == DataGridViewTriState.True)
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
            Debug.Assert(((DataGridViewColumn)_items[toColumnIndex]).StateIncludes(includeFilter));

            int jumpColumns = 0;
            DataGridViewColumn dataGridViewColumn = (DataGridViewColumn)_items[fromColumnIndex];

            while (dataGridViewColumn != (DataGridViewColumn)_items[toColumnIndex])
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
            Debug.Assert(_itemsSorted != null);
            Debug.Assert(_lastAccessedSortedIndex == -1 ||
                _lastAccessedSortedIndex < Count);

#if DEBUG
            Debug.Assert(VerifyColumnOrderCache());
#endif
            if (_lastAccessedSortedIndex != -1 &&
                _itemsSorted[_lastAccessedSortedIndex] == dataGridViewColumn)
            {
                return _lastAccessedSortedIndex;
            }

            int index = 0;
            while (index < _itemsSorted.Count)
            {
                if (dataGridViewColumn.Index == ((DataGridViewColumn)_itemsSorted[index]).Index)
                {
                    _lastAccessedSortedIndex = index;
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
            for (int columnIndex = 0; columnIndex < _items.Count; columnIndex++)
            {
                if (((DataGridViewColumn)_items[columnIndex]).StateIncludes(includeFilter))
                {
                    weightSum += ((DataGridViewColumn)_items[columnIndex]).FillWeight;
                }
            }
            return weightSum;
        }

        public int GetColumnsWidth(DataGridViewElementStates includeFilter)
        {
            if ((includeFilter & ~(DataGridViewElementStates.Displayed | DataGridViewElementStates.Frozen | DataGridViewElementStates.Resizable |
                DataGridViewElementStates.ReadOnly | DataGridViewElementStates.Selected | DataGridViewElementStates.Visible)) != 0)
            {
                throw new ArgumentException(string.Format(SR.DataGridView_InvalidDataGridViewElementStateCombination, nameof(includeFilter)));
            }

            // cache returned value and reuse it as long as none
            // of the column's state/thickness has changed.
            switch (includeFilter)
            {
                case DataGridViewElementStates.Visible:
                    if (_columnsWidthVisible != -1)
                    {
                        return _columnsWidthVisible;
                    }
                    break;
                case DataGridViewElementStates.Visible | DataGridViewElementStates.Frozen:
                    if (_columnsWidthVisibleFrozen != -1)
                    {
                        return _columnsWidthVisibleFrozen;
                    }
                    break;
            }

            int columnsWidth = 0;
            for (int columnIndex = 0; columnIndex < _items.Count; columnIndex++)
            {
                if (((DataGridViewColumn)_items[columnIndex]).StateIncludes(includeFilter))
                {
                    columnsWidth += ((DataGridViewColumn)_items[columnIndex]).Thickness;
                }
            }

            switch (includeFilter)
            {
                case DataGridViewElementStates.Visible:
                    _columnsWidthVisible = columnsWidth;
                    break;
                case DataGridViewElementStates.Visible | DataGridViewElementStates.Frozen:
                    _columnsWidthVisibleFrozen = columnsWidth;
                    break;
            }
            return columnsWidth;
        }

        public DataGridViewColumn GetFirstColumn(DataGridViewElementStates includeFilter)
        {
            if ((includeFilter & ~(DataGridViewElementStates.Displayed | DataGridViewElementStates.Frozen | DataGridViewElementStates.Resizable |
                DataGridViewElementStates.ReadOnly | DataGridViewElementStates.Selected | DataGridViewElementStates.Visible)) != 0)
            {
                throw new ArgumentException(string.Format(SR.DataGridView_InvalidDataGridViewElementStateCombination, nameof(includeFilter)));
            }

            if (_itemsSorted is null)
            {
                UpdateColumnOrderCache();
            }
#if DEBUG
            Debug.Assert(VerifyColumnOrderCache());
#endif
            int index = 0;
            while (index < _itemsSorted.Count)
            {
                DataGridViewColumn dataGridViewColumn = (DataGridViewColumn)_itemsSorted[index];
                if (dataGridViewColumn.StateIncludes(includeFilter))
                {
                    _lastAccessedSortedIndex = index;
                    return dataGridViewColumn;
                }
                index++;
            }
            return null;
        }

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
                throw new ArgumentException(string.Format(SR.DataGridView_InvalidDataGridViewElementStateCombination, nameof(includeFilter)));
            }
            if ((excludeFilter & ~(DataGridViewElementStates.Displayed | DataGridViewElementStates.Frozen | DataGridViewElementStates.Resizable |
                DataGridViewElementStates.ReadOnly | DataGridViewElementStates.Selected | DataGridViewElementStates.Visible)) != 0)
            {
                throw new ArgumentException(string.Format(SR.DataGridView_InvalidDataGridViewElementStateCombination, nameof(excludeFilter)));
            }

            if (_itemsSorted is null)
            {
                UpdateColumnOrderCache();
            }
#if DEBUG
            Debug.Assert(VerifyColumnOrderCache());
#endif
            int index = 0;
            while (index < _itemsSorted.Count)
            {
                DataGridViewColumn dataGridViewColumn = (DataGridViewColumn)_itemsSorted[index];
                if (dataGridViewColumn.StateIncludes(includeFilter) &&
                    dataGridViewColumn.StateExcludes(excludeFilter))
                {
                    _lastAccessedSortedIndex = index;
                    return dataGridViewColumn;
                }
                index++;
            }
            return null;
        }

        public DataGridViewColumn GetLastColumn(DataGridViewElementStates includeFilter,
                                                DataGridViewElementStates excludeFilter)
        {
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

            if (_itemsSorted is null)
            {
                UpdateColumnOrderCache();
            }
#if DEBUG
            Debug.Assert(VerifyColumnOrderCache());
#endif
            int index = _itemsSorted.Count - 1;
            while (index >= 0)
            {
                DataGridViewColumn dataGridViewColumn = (DataGridViewColumn)_itemsSorted[index];
                if (dataGridViewColumn.StateIncludes(includeFilter) &&
                    dataGridViewColumn.StateExcludes(excludeFilter))
                {
                    _lastAccessedSortedIndex = index;
                    return dataGridViewColumn;
                }
                index--;
            }
            return null;
        }

        public DataGridViewColumn GetNextColumn(DataGridViewColumn dataGridViewColumnStart,
                                                DataGridViewElementStates includeFilter,
                                                DataGridViewElementStates excludeFilter)
        {
            if (dataGridViewColumnStart is null)
            {
                throw new ArgumentNullException(nameof(dataGridViewColumnStart));
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

            if (_itemsSorted is null)
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
                int indexMin = int.MaxValue, displayIndexMin = int.MaxValue;
                for (index = 0; index < _items.Count; index++)
                {
                    DataGridViewColumn dataGridViewColumn = (DataGridViewColumn)_items[index];
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
                return columnFound ? ((DataGridViewColumn)_items[indexMin]) : null;
            }
            else
            {
                index++;
                while (index < _itemsSorted.Count)
                {
                    DataGridViewColumn dataGridViewColumn = (DataGridViewColumn)_itemsSorted[index];

                    if (dataGridViewColumn.StateIncludes(includeFilter) && dataGridViewColumn.StateExcludes(excludeFilter))
                    {
                        _lastAccessedSortedIndex = index;
                        return dataGridViewColumn;
                    }

                    index++;
                }
            }
            return null;
        }

        public DataGridViewColumn GetPreviousColumn(DataGridViewColumn dataGridViewColumnStart,
                                                             DataGridViewElementStates includeFilter,
                                                             DataGridViewElementStates excludeFilter)
        {
            if (dataGridViewColumnStart is null)
            {
                throw new ArgumentNullException(nameof(dataGridViewColumnStart));
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

            if (_itemsSorted is null)
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
                for (index = 0; index < _items.Count; index++)
                {
                    DataGridViewColumn dataGridViewColumn = (DataGridViewColumn)_items[index];
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
                return columnFound ? ((DataGridViewColumn)_items[indexMax]) : null;
            }
            else
            {
                index--;
                while (index >= 0)
                {
                    DataGridViewColumn dataGridViewColumn = (DataGridViewColumn)_itemsSorted[index];
                    if (dataGridViewColumn.StateIncludes(includeFilter) &&
                        dataGridViewColumn.StateExcludes(excludeFilter))
                    {
                        _lastAccessedSortedIndex = index;
                        return dataGridViewColumn;
                    }
                    index--;
                }
            }
            return null;
        }

        public int IndexOf(DataGridViewColumn dataGridViewColumn)
        {
            return _items.IndexOf(dataGridViewColumn);
        }

        /// <summary>
        ///  Inserts a <see cref='DataGridViewColumn'/> in this collection.
        /// </summary>
        public virtual void Insert(int columnIndex, DataGridViewColumn dataGridViewColumn)
        {
            Debug.Assert(DataGridView != null);
            if (DataGridView.NoDimensionChangeAllowed)
            {
                throw new InvalidOperationException(SR.DataGridView_ForbiddenOperationInEventHandler);
            }
            if (DataGridView.InDisplayIndexAdjustments)
            {
                // We are within columns display indexes adjustments. We do not allow changing the column collection while adjusting display indexes.
                throw new InvalidOperationException(SR.DataGridView_CannotAlterDisplayIndexWithinAdjustments);
            }
            if (dataGridViewColumn is null)
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
                DataGridView.OnInsertingColumn(columnIndex, dataGridViewColumn, out newCurrentCell);   // will throw an exception if the insertion is illegal
            }
            finally
            {
                dataGridViewColumn.DisplayIndexInternal = originalDisplayIndex;
            }
            InvalidateCachedColumnsOrder();
            _items.Insert(columnIndex, dataGridViewColumn);
            dataGridViewColumn.Index = columnIndex;
            dataGridViewColumn.DataGridView = DataGridView;
            UpdateColumnCaches(dataGridViewColumn, true);
            DataGridView.OnInsertedColumn_PreNotification(dataGridViewColumn);
            OnCollectionChanged(new CollectionChangeEventArgs(CollectionChangeAction.Add, dataGridViewColumn), true /*changeIsInsertion*/, newCurrentCell);
#if DEBUG
            Debug.Assert(_itemsSorted is null || VerifyColumnOrderCache());
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
                _columnCountsVisibleSelected = -1;
            }
        }

        internal void InvalidateCachedColumnCounts()
        {
            _columnCountsVisible = _columnCountsVisibleSelected = -1;
        }

        internal void InvalidateCachedColumnsOrder()
        {
            _itemsSorted = null;
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
                _columnsWidthVisibleFrozen = -1;
            }
        }

        internal void InvalidateCachedColumnsWidths()
        {
            _columnsWidthVisible = _columnsWidthVisibleFrozen = -1;
        }

        protected virtual void OnCollectionChanged(CollectionChangeEventArgs e)
        {
            _onCollectionChanged?.Invoke(this, e);
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
            Debug.Assert(DataGridView != null);
            DataGridView.OnColumnCollectionChanged_PreNotification(ccea);
        }

        private void OnCollectionChanged_PostNotification(CollectionChangeEventArgs ccea, bool changeIsInsertion, Point newCurrentCell)
        {
            Debug.Assert(DataGridView != null);
            DataGridViewColumn dataGridViewColumn = (DataGridViewColumn)ccea.Element;
            if (ccea.Action == CollectionChangeAction.Add && changeIsInsertion)
            {
                DataGridView.OnInsertedColumn_PostNotification(newCurrentCell);
            }
            else if (ccea.Action == CollectionChangeAction.Remove)
            {
                DataGridView.OnRemovedColumn_PostNotification(dataGridViewColumn, newCurrentCell);
            }

            DataGridView.OnColumnCollectionChanged_PostNotification(dataGridViewColumn);
        }

        public virtual void Remove(DataGridViewColumn dataGridViewColumn)
        {
            if (dataGridViewColumn is null)
            {
                throw new ArgumentNullException(nameof(dataGridViewColumn));
            }

            if (dataGridViewColumn.DataGridView != DataGridView)
            {
                throw new ArgumentException(SR.DataGridView_ColumnDoesNotBelongToDataGridView, nameof(dataGridViewColumn));
            }

            int itemsCount = _items.Count;
            for (int i = 0; i < itemsCount; ++i)
            {
                if (_items[i] == dataGridViewColumn)
                {
                    RemoveAt(i);
#if DEBUG
                    Debug.Assert(_itemsSorted is null || VerifyColumnOrderCache());
#endif
                    return;
                }
            }

            Debug.Fail("Column should have been found in DataGridViewColumnCollection.Remove");
        }

        public virtual void Remove(string columnName)
        {
            if (columnName is null)
            {
                throw new ArgumentNullException(nameof(columnName));
            }

            int itemsCount = _items.Count;
            for (int i = 0; i < itemsCount; ++i)
            {
                DataGridViewColumn dataGridViewColumn = (DataGridViewColumn)_items[i];
                // NOTE: case-insensitive
                if (0 == string.Compare(dataGridViewColumn.Name, columnName, true, CultureInfo.InvariantCulture))
                {
                    RemoveAt(i);
                    return;
                }
            }

            throw new ArgumentException(string.Format(SR.DataGridViewColumnCollection_ColumnNotFound, columnName), nameof(columnName));
        }

        public virtual void RemoveAt(int index)
        {
            if (index < 0 || index >= Count)
            {
                throw new ArgumentOutOfRangeException(nameof(index), index, string.Format(SR.InvalidArgument, nameof(index), index));
            }

            if (DataGridView.NoDimensionChangeAllowed)
            {
                throw new InvalidOperationException(SR.DataGridView_ForbiddenOperationInEventHandler);
            }

            if (DataGridView.InDisplayIndexAdjustments)
            {
                // We are within columns display indexes adjustments. We do not allow changing the column collection while adjusting display indexes.
                throw new InvalidOperationException(SR.DataGridView_CannotAlterDisplayIndexWithinAdjustments);
            }

            RemoveAtInternal(index, false /*force*/);
#if DEBUG
            Debug.Assert(_itemsSorted is null || VerifyColumnOrderCache());
#endif
        }

        internal void RemoveAtInternal(int index, bool force)
        {
            // If force is true, the underlying data is gone and can't be accessed anymore.

            Debug.Assert(index >= 0 && index < Count);
            Debug.Assert(DataGridView != null);
            Debug.Assert(!DataGridView.NoDimensionChangeAllowed);
            Debug.Assert(!DataGridView.InDisplayIndexAdjustments);

            DataGridViewColumn dataGridViewColumn = (DataGridViewColumn)_items[index];
            DataGridView.OnRemovingColumn(dataGridViewColumn, out Point newCurrentCell, force);
            InvalidateCachedColumnsOrder();
            _items.RemoveAt(index);
            dataGridViewColumn.DataGridView = null;
            UpdateColumnCaches(dataGridViewColumn, false);
            DataGridView.OnRemovedColumn_PreNotification(dataGridViewColumn);
            OnCollectionChanged(new CollectionChangeEventArgs(CollectionChangeAction.Remove, dataGridViewColumn), false /*changeIsInsertion*/, newCurrentCell);
        }

        private void UpdateColumnCaches(DataGridViewColumn dataGridViewColumn, bool adding)
        {
            if (_columnCountsVisible != -1 || _columnCountsVisibleSelected != -1 ||
                _columnsWidthVisible != -1 || _columnsWidthVisibleFrozen != -1)
            {
                DataGridViewElementStates columnStates = dataGridViewColumn.State;
                if ((columnStates & DataGridViewElementStates.Visible) != 0)
                {
                    int columnCountIncrement = adding ? 1 : -1;
                    int columnWidthIncrement = 0;
                    if (_columnsWidthVisible != -1 ||
                        (_columnsWidthVisibleFrozen != -1 &&
                         ((columnStates & (DataGridViewElementStates.Visible | DataGridViewElementStates.Frozen)) == (DataGridViewElementStates.Visible | DataGridViewElementStates.Frozen))))
                    {
                        columnWidthIncrement = adding ? dataGridViewColumn.Width : -dataGridViewColumn.Width;
                    }

                    if (_columnCountsVisible != -1)
                    {
                        _columnCountsVisible += columnCountIncrement;
                    }
                    if (_columnsWidthVisible != -1)
                    {
                        Debug.Assert(columnWidthIncrement != 0);
                        _columnsWidthVisible += columnWidthIncrement;
                    }

                    if ((columnStates & (DataGridViewElementStates.Visible | DataGridViewElementStates.Frozen)) == (DataGridViewElementStates.Visible | DataGridViewElementStates.Frozen))
                    {
                        if (_columnsWidthVisibleFrozen != -1)
                        {
                            Debug.Assert(columnWidthIncrement != 0);
                            _columnsWidthVisibleFrozen += columnWidthIncrement;
                        }
                    }

                    if ((columnStates & (DataGridViewElementStates.Visible | DataGridViewElementStates.Selected)) == (DataGridViewElementStates.Visible | DataGridViewElementStates.Selected))
                    {
                        if (_columnCountsVisibleSelected != -1)
                        {
                            _columnCountsVisibleSelected += columnCountIncrement;
                        }
                    }
                }
            }
        }

        private void UpdateColumnOrderCache()
        {
            _itemsSorted = (ArrayList)_items.Clone();
            _itemsSorted.Sort(s_columnOrderComparer);
            _lastAccessedSortedIndex = -1;
        }

#if DEBUG
        internal bool VerifyColumnDisplayIndexes()
        {
            for (int columnDisplayIndex = 0; columnDisplayIndex < _items.Count; columnDisplayIndex++)
            {
                if (GetColumnAtDisplayIndex(columnDisplayIndex) is null)
                {
                    return false;
                }
            }
            return true;
        }

        private bool VerifyColumnOrderCache()
        {
            if (_itemsSorted is null)
            {
                return false;
            }

            if (_itemsSorted.Count != _items.Count)
            {
                return false;
            }

            int index = 0;
            while (index < _itemsSorted.Count - 1)
            {
                if (((DataGridViewColumn)_itemsSorted[index + 1]).DisplayIndex !=
                    ((DataGridViewColumn)_itemsSorted[index]).DisplayIndex + 1)
                {
                    return false;
                }

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

            public int Compare(object x, object y)
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
