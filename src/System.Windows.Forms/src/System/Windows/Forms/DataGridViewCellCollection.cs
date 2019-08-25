// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections;
using System.ComponentModel;
using System.Diagnostics;

namespace System.Windows.Forms
{
    /// <summary>
    ///  Represents a collection of <see cref='DataGridViewCell'/> objects in the <see cref='DataGridView'/>
    ///  control.
    /// </summary>
    [ListBindable(false)]
    public class DataGridViewCellCollection : BaseCollection, IList
    {
        CollectionChangeEventHandler onCollectionChanged;
        readonly ArrayList items = new ArrayList();
        readonly DataGridViewRow owner = null;

        int IList.Add(object value)
        {
            return Add((DataGridViewCell)value);
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
            Insert(index, (DataGridViewCell)value);
        }

        void IList.Remove(object value)
        {
            Remove((DataGridViewCell)value);
        }

        void IList.RemoveAt(int index)
        {
            RemoveAt(index);
        }

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
            set { this[index] = (DataGridViewCell)value; }
        }

        void ICollection.CopyTo(Array array, int index)
        {
            items.CopyTo(array, index);
        }

        int ICollection.Count
        {
            get { return items.Count; }
        }

        bool ICollection.IsSynchronized
        {
            get { return false; }
        }

        object ICollection.SyncRoot
        {
            get { return this; }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return items.GetEnumerator();
        }

        public DataGridViewCellCollection(DataGridViewRow dataGridViewRow)
        {
            Debug.Assert(dataGridViewRow != null);
            owner = dataGridViewRow;
        }

        protected override ArrayList List
        {
            get
            {
                return items;
            }
        }

        /// <summary>
        ///  Retrieves the DataGridViewCell with the specified index.
        /// </summary>
        public DataGridViewCell this[int index]
        {
            get
            {
                return (DataGridViewCell)items[index];
            }
            set
            {
                DataGridViewCell dataGridViewCell = value;
                if (dataGridViewCell == null)
                {
                    throw new ArgumentNullException(nameof(value));
                }
                if (dataGridViewCell.DataGridView != null)
                {
                    throw new InvalidOperationException(SR.DataGridViewCellCollection_CellAlreadyBelongsToDataGridView);
                }
                if (dataGridViewCell.OwningRow != null)
                {
                    throw new InvalidOperationException(SR.DataGridViewCellCollection_CellAlreadyBelongsToDataGridViewRow);
                }
                if (owner.DataGridView != null)
                {
                    owner.DataGridView.OnReplacingCell(owner, index);
                }

                DataGridViewCell oldDataGridViewCell = (DataGridViewCell)items[index];
                items[index] = dataGridViewCell;
                dataGridViewCell.OwningRow = owner;
                dataGridViewCell.State = oldDataGridViewCell.State;
                if (owner.DataGridView != null)
                {
                    dataGridViewCell.DataGridView = owner.DataGridView;
                    dataGridViewCell.OwningColumn = owner.DataGridView.Columns[index];
                    owner.DataGridView.OnReplacedCell(owner, index);
                }

                oldDataGridViewCell.DataGridView = null;
                oldDataGridViewCell.OwningRow = null;
                oldDataGridViewCell.OwningColumn = null;
                if (oldDataGridViewCell.ReadOnly)
                {
                    oldDataGridViewCell.ReadOnlyInternal = false;
                }
                if (oldDataGridViewCell.Selected)
                {
                    oldDataGridViewCell.SelectedInternal = false;
                }
            }
        }

        /// <summary>
        ///  Retrieves the DataGridViewCell with the specified column name.
        /// </summary>
        public DataGridViewCell this[string columnName]
        {
            get
            {
                DataGridViewColumn dataGridViewColumn = null;
                if (owner.DataGridView != null)
                {
                    dataGridViewColumn = owner.DataGridView.Columns[columnName];
                }
                if (dataGridViewColumn == null)
                {
                    throw new ArgumentException(string.Format(SR.DataGridViewColumnCollection_ColumnNotFound, columnName), "columnName");
                }
                return (DataGridViewCell)items[dataGridViewColumn.Index];
            }
            set
            {
                DataGridViewColumn dataGridViewColumn = null;
                if (owner.DataGridView != null)
                {
                    dataGridViewColumn = owner.DataGridView.Columns[columnName];
                }
                if (dataGridViewColumn == null)
                {
                    throw new ArgumentException(string.Format(SR.DataGridViewColumnCollection_ColumnNotFound, columnName), "columnName");
                }
                this[dataGridViewColumn.Index] = value;
            }
        }

        public event CollectionChangeEventHandler CollectionChanged
        {
            add => onCollectionChanged += value;
            remove => onCollectionChanged -= value;
        }

        /// <summary>
        ///  Adds a <see cref='DataGridViewCell'/> to this collection.
        /// </summary>
        public virtual int Add(DataGridViewCell dataGridViewCell)
        {
            if (owner.DataGridView != null)
            {
                throw new InvalidOperationException(SR.DataGridViewCellCollection_OwningRowAlreadyBelongsToDataGridView);
            }
            if (dataGridViewCell.OwningRow != null)
            {
                throw new InvalidOperationException(SR.DataGridViewCellCollection_CellAlreadyBelongsToDataGridViewRow);
            }
            Debug.Assert(!dataGridViewCell.ReadOnly);
            return AddInternal(dataGridViewCell);
        }

        internal int AddInternal(DataGridViewCell dataGridViewCell)
        {
            Debug.Assert(!dataGridViewCell.Selected);
            int index = items.Add(dataGridViewCell);
            dataGridViewCell.OwningRow = owner;
            DataGridView dataGridView = owner.DataGridView;
            if (dataGridView != null && dataGridView.Columns.Count > index)
            {
                dataGridViewCell.OwningColumn = dataGridView.Columns[index];
            }
            OnCollectionChanged(new CollectionChangeEventArgs(CollectionChangeAction.Add, dataGridViewCell));
            return index;
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public virtual void AddRange(params DataGridViewCell[] dataGridViewCells)
        {
            if (dataGridViewCells == null)
            {
                throw new ArgumentNullException(nameof(dataGridViewCells));
            }
            if (owner.DataGridView != null)
            {
                throw new InvalidOperationException(SR.DataGridViewCellCollection_OwningRowAlreadyBelongsToDataGridView);
            }
            foreach (DataGridViewCell dataGridViewCell in dataGridViewCells)
            {
                if (dataGridViewCell == null)
                {
                    throw new InvalidOperationException(SR.DataGridViewCellCollection_AtLeastOneCellIsNull);
                }

                if (dataGridViewCell.OwningRow != null)
                {
                    throw new InvalidOperationException(SR.DataGridViewCellCollection_CellAlreadyBelongsToDataGridViewRow);
                }
            }

            // Make sure no two cells are identical
            int cellCount = dataGridViewCells.Length;
            for (int cell1 = 0; cell1 < cellCount - 1; cell1++)
            {
                for (int cell2 = cell1 + 1; cell2 < cellCount; cell2++)
                {
                    if (dataGridViewCells[cell1] == dataGridViewCells[cell2])
                    {
                        throw new InvalidOperationException(SR.DataGridViewCellCollection_CannotAddIdenticalCells);
                    }
                }
            }

            items.AddRange(dataGridViewCells);
            foreach (DataGridViewCell dataGridViewCell in dataGridViewCells)
            {
                dataGridViewCell.OwningRow = owner;
                Debug.Assert(!dataGridViewCell.Selected);
            }
            OnCollectionChanged(new CollectionChangeEventArgs(CollectionChangeAction.Refresh, null));
        }

        public virtual void Clear()
        {
            if (owner.DataGridView != null)
            {
                throw new InvalidOperationException(SR.DataGridViewCellCollection_OwningRowAlreadyBelongsToDataGridView);
            }
            foreach (DataGridViewCell dataGridViewCell in items)
            {
                dataGridViewCell.OwningRow = null;
            }
            items.Clear();
            OnCollectionChanged(new CollectionChangeEventArgs(CollectionChangeAction.Refresh, null));
        }

        public void CopyTo(DataGridViewCell[] array, int index)
        {
            items.CopyTo(array, index);
        }

        /// <summary>
        ///  Checks to see if a DataGridViewCell is contained in this collection.
        /// </summary>
        public virtual bool Contains(DataGridViewCell dataGridViewCell)
        {
            int index = items.IndexOf(dataGridViewCell);
            return index != -1;
        }

        public int IndexOf(DataGridViewCell dataGridViewCell)
        {
            return items.IndexOf(dataGridViewCell);
        }

        public virtual void Insert(int index, DataGridViewCell dataGridViewCell)
        {
            if (owner.DataGridView != null)
            {
                throw new InvalidOperationException(SR.DataGridViewCellCollection_OwningRowAlreadyBelongsToDataGridView);
            }
            if (dataGridViewCell.OwningRow != null)
            {
                throw new InvalidOperationException(SR.DataGridViewCellCollection_CellAlreadyBelongsToDataGridViewRow);
            }
            Debug.Assert(!dataGridViewCell.ReadOnly);
            Debug.Assert(!dataGridViewCell.Selected);
            items.Insert(index, dataGridViewCell);
            dataGridViewCell.OwningRow = owner;
            OnCollectionChanged(new CollectionChangeEventArgs(CollectionChangeAction.Add, dataGridViewCell));
        }

        internal void InsertInternal(int index, DataGridViewCell dataGridViewCell)
        {
            Debug.Assert(!dataGridViewCell.Selected);
            items.Insert(index, dataGridViewCell);
            dataGridViewCell.OwningRow = owner;
            DataGridView dataGridView = owner.DataGridView;
            if (dataGridView != null && dataGridView.Columns.Count > index)
            {
                dataGridViewCell.OwningColumn = dataGridView.Columns[index];
            }
            OnCollectionChanged(new CollectionChangeEventArgs(CollectionChangeAction.Add, dataGridViewCell));
        }

        protected void OnCollectionChanged(CollectionChangeEventArgs e)
        {
            onCollectionChanged?.Invoke(this, e);
        }

        public virtual void Remove(DataGridViewCell cell)
        {
            if (owner.DataGridView != null)
            {
                throw new InvalidOperationException(SR.DataGridViewCellCollection_OwningRowAlreadyBelongsToDataGridView);
            }
            int cellIndex = -1;
            int itemsCount = items.Count;
            for (int i = 0; i < itemsCount; ++i)
            {
                if (items[i] == cell)
                {
                    cellIndex = i;
                    break;
                }
            }
            if (cellIndex == -1)
            {
                throw new ArgumentException(SR.DataGridViewCellCollection_CellNotFound);
            }
            else
            {
                RemoveAt(cellIndex);
            }
        }

        public virtual void RemoveAt(int index)
        {
            if (owner.DataGridView != null)
            {
                throw new InvalidOperationException(SR.DataGridViewCellCollection_OwningRowAlreadyBelongsToDataGridView);
            }
            RemoveAtInternal(index);
        }

        internal void RemoveAtInternal(int index)
        {
            DataGridViewCell dataGridViewCell = (DataGridViewCell)items[index];
            items.RemoveAt(index);
            dataGridViewCell.DataGridView = null;
            dataGridViewCell.OwningRow = null;
            if (dataGridViewCell.ReadOnly)
            {
                dataGridViewCell.ReadOnlyInternal = false;
            }
            if (dataGridViewCell.Selected)
            {
                dataGridViewCell.SelectedInternal = false;
            }
            OnCollectionChanged(new CollectionChangeEventArgs(CollectionChangeAction.Remove, dataGridViewCell));
        }
    }
}
