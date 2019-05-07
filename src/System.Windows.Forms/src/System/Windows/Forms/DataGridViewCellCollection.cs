﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms
{
    using System;
    using System.Diagnostics;
    using System.Collections;
    using System.Windows.Forms;
    using System.ComponentModel;
    using System.Globalization;
    using System.Diagnostics.CodeAnalysis;

    /// <devdoc>
    /// <para>Represents a collection of <see cref='System.Windows.Forms.DataGridViewCell'/> objects in the <see cref='System.Windows.Forms.DataGridView'/> 
    /// control.</para>
    /// </devdoc>
    [
        ListBindable(false),
        SuppressMessage("Microsoft.Design", "CA1010:CollectionsShouldImplementGenericInterface") // Consider adding an IList<DataGridViewCellCollection> implementation
    ]
    public class DataGridViewCellCollection : BaseCollection, IList
    {
        CollectionChangeEventHandler onCollectionChanged;
        ArrayList items = new ArrayList();
        DataGridViewRow owner = null;

        int IList.Add(object value)
        {
            return this.Add((DataGridViewCell) value);            
        }

        void IList.Clear()
        {
            this.Clear();
        }

        bool IList.Contains(object value)
        {
            return this.items.Contains(value);
        }

        int IList.IndexOf(object value)
        {
            return this.items.IndexOf(value);
        }

        void IList.Insert(int index, object value)
        {
            this.Insert(index, (DataGridViewCell) value);
        }

        void IList.Remove(object value)
        {
            this.Remove((DataGridViewCell) value);
        }

        void IList.RemoveAt(int index)
        {
            this.RemoveAt(index);
        }

        bool IList.IsFixedSize
        {
            get {return false;}
        }

        bool IList.IsReadOnly
        {
            get {return false;}
        }

        object IList.this[int index]
        {
            get { return this[index]; }
            set { this[index] = (DataGridViewCell) value; }
        }

        void ICollection.CopyTo(Array array, int index)
        {
            this.items.CopyTo(array, index);
        }

        int ICollection.Count
        {
            get {return this.items.Count;}
        }

        bool ICollection.IsSynchronized
        {
            get {return false;}
        }

        object ICollection.SyncRoot
        {
            get {return this;}
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.items.GetEnumerator();
        }

        public DataGridViewCellCollection(DataGridViewRow dataGridViewRow)
        {
            Debug.Assert(dataGridViewRow != null);
            this.owner = dataGridViewRow;
        }

        protected override ArrayList List
        {
            get
            {
                return this.items;
            }
        }

        /// <devdoc>
        ///      Retrieves the DataGridViewCell with the specified index.
        /// </devdoc>
        public DataGridViewCell this[int index]
        {
            get
            {
                return (DataGridViewCell) this.items[index];
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
                    throw new InvalidOperationException(string.Format(SR.DataGridViewCellCollection_CellAlreadyBelongsToDataGridView));
                }
                if (dataGridViewCell.OwningRow != null)
                {
                    throw new InvalidOperationException(string.Format(SR.DataGridViewCellCollection_CellAlreadyBelongsToDataGridViewRow));
                }
                if (this.owner.DataGridView != null)
                {
                    this.owner.DataGridView.OnReplacingCell(this.owner, index);
                }

                DataGridViewCell oldDataGridViewCell = (DataGridViewCell) this.items[index];
                this.items[index] = dataGridViewCell;
                dataGridViewCell.OwningRowInternal = this.owner;
                dataGridViewCell.StateInternal = oldDataGridViewCell.State;
                if (this.owner.DataGridView != null)
                {
                    dataGridViewCell.DataGridViewInternal = this.owner.DataGridView;
                    dataGridViewCell.OwningColumnInternal = this.owner.DataGridView.Columns[index];
                    this.owner.DataGridView.OnReplacedCell(this.owner, index);
                }

                oldDataGridViewCell.DataGridViewInternal = null;
                oldDataGridViewCell.OwningRowInternal = null;
                oldDataGridViewCell.OwningColumnInternal = null;
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

        /// <devdoc>
        ///      Retrieves the DataGridViewCell with the specified column name.
        /// </devdoc>
        public DataGridViewCell this[string columnName]
        {
            get
            {
                DataGridViewColumn dataGridViewColumn = null;
                if (this.owner.DataGridView != null)
                {
                    dataGridViewColumn = this.owner.DataGridView.Columns[columnName];
                }
                if (dataGridViewColumn == null)
                {
                    throw new ArgumentException(string.Format(SR.DataGridViewColumnCollection_ColumnNotFound, columnName), "columnName");
                }
                return (DataGridViewCell) this.items[dataGridViewColumn.Index];
            }
            set
            {
                DataGridViewColumn dataGridViewColumn = null;
                if (this.owner.DataGridView != null)
                {
                    dataGridViewColumn = this.owner.DataGridView.Columns[columnName];
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
            add => this.onCollectionChanged += value;
            remove => this.onCollectionChanged -= value;
        }

        /// <devdoc>
        /// <para>Adds a <see cref='System.Windows.Forms.DataGridViewCell'/> to this collection.</para>
        /// </devdoc>
        public virtual int Add(DataGridViewCell dataGridViewCell)
        {
            if (this.owner.DataGridView != null)
            {
                throw new InvalidOperationException(string.Format(SR.DataGridViewCellCollection_OwningRowAlreadyBelongsToDataGridView));
            }
            if (dataGridViewCell.OwningRow != null)
            {
                throw new InvalidOperationException(string.Format(SR.DataGridViewCellCollection_CellAlreadyBelongsToDataGridViewRow));
            }
            Debug.Assert(!dataGridViewCell.ReadOnly);
            return AddInternal(dataGridViewCell);
        }

        internal int AddInternal(DataGridViewCell dataGridViewCell)
        {
            Debug.Assert(!dataGridViewCell.Selected);
            int index = this.items.Add(dataGridViewCell);
            dataGridViewCell.OwningRowInternal = this.owner;
            DataGridView dataGridView = this.owner.DataGridView;
            if (dataGridView != null && dataGridView.Columns.Count > index)
            {
                dataGridViewCell.OwningColumnInternal = dataGridView.Columns[index];
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
            if (this.owner.DataGridView != null)
            {
                throw new InvalidOperationException(string.Format(SR.DataGridViewCellCollection_OwningRowAlreadyBelongsToDataGridView));
            }
            foreach (DataGridViewCell dataGridViewCell in dataGridViewCells)
            {
                if (dataGridViewCell == null)
                {
                    throw new InvalidOperationException(string.Format(SR.DataGridViewCellCollection_AtLeastOneCellIsNull));
                }

                if (dataGridViewCell.OwningRow != null)
                {
                    throw new InvalidOperationException(string.Format(SR.DataGridViewCellCollection_CellAlreadyBelongsToDataGridViewRow));
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
                        throw new InvalidOperationException(string.Format(SR.DataGridViewCellCollection_CannotAddIdenticalCells));
                    }
                }
            }

            this.items.AddRange(dataGridViewCells);
            foreach (DataGridViewCell dataGridViewCell in dataGridViewCells)
            {
                dataGridViewCell.OwningRowInternal = this.owner;
                Debug.Assert(!dataGridViewCell.Selected);
            }
            OnCollectionChanged(new CollectionChangeEventArgs(CollectionChangeAction.Refresh, null));
        }
        
        public virtual void Clear()
        {
            if (this.owner.DataGridView != null)
            {
                throw new InvalidOperationException(string.Format(SR.DataGridViewCellCollection_OwningRowAlreadyBelongsToDataGridView));
            }
            foreach (DataGridViewCell dataGridViewCell in this.items)
            {
                dataGridViewCell.OwningRowInternal = null;
            }
            this.items.Clear();
            OnCollectionChanged(new CollectionChangeEventArgs(CollectionChangeAction.Refresh, null));
        }

        public void CopyTo(DataGridViewCell[] array, int index)
        {
            this.items.CopyTo(array, index);
        }

        /// <devdoc>
        ///      Checks to see if a DataGridViewCell is contained in this collection.
        /// </devdoc>
        public virtual bool Contains(DataGridViewCell dataGridViewCell)
        {
            int index = this.items.IndexOf(dataGridViewCell);
            return index != -1;
        }

        public int IndexOf(DataGridViewCell dataGridViewCell)
        {
            return this.items.IndexOf(dataGridViewCell);
        }

        public virtual void Insert(int index, DataGridViewCell dataGridViewCell)
        {
            if (this.owner.DataGridView != null)
            {
                throw new InvalidOperationException(string.Format(SR.DataGridViewCellCollection_OwningRowAlreadyBelongsToDataGridView));
            }
            if (dataGridViewCell.OwningRow != null)
            {
                throw new InvalidOperationException(string.Format(SR.DataGridViewCellCollection_CellAlreadyBelongsToDataGridViewRow));
            }
            Debug.Assert(!dataGridViewCell.ReadOnly);
            Debug.Assert(!dataGridViewCell.Selected);
            this.items.Insert(index, dataGridViewCell);
            dataGridViewCell.OwningRowInternal = this.owner;
            OnCollectionChanged(new CollectionChangeEventArgs(CollectionChangeAction.Add, dataGridViewCell));
        }

        internal void InsertInternal(int index, DataGridViewCell dataGridViewCell)
        {
            Debug.Assert(!dataGridViewCell.Selected);
            this.items.Insert(index, dataGridViewCell);
            dataGridViewCell.OwningRowInternal = this.owner;
            DataGridView dataGridView = this.owner.DataGridView;
            if (dataGridView != null && dataGridView.Columns.Count > index)
            {
                dataGridViewCell.OwningColumnInternal = dataGridView.Columns[index];
            }
            OnCollectionChanged(new CollectionChangeEventArgs(CollectionChangeAction.Add, dataGridViewCell));
        }

        protected void OnCollectionChanged(CollectionChangeEventArgs e)
        {
            if (this.onCollectionChanged != null)
            {
                this.onCollectionChanged(this, e);
            }
        }

        public virtual void Remove(DataGridViewCell cell)
        {
            if (this.owner.DataGridView != null)
            {
                throw new InvalidOperationException(string.Format(SR.DataGridViewCellCollection_OwningRowAlreadyBelongsToDataGridView));
            }
            int cellIndex = -1;
            int itemsCount = this.items.Count;
            for (int i = 0; i < itemsCount; ++i)
            {
                if (this.items[i] == cell) 
                {
                    cellIndex = i;
                    break;
                }
            }
            if (cellIndex == -1)
            {
                throw new ArgumentException(string.Format(SR.DataGridViewCellCollection_CellNotFound));
            }
            else
            {
                RemoveAt(cellIndex);
            }
        }

        public virtual void RemoveAt(int index)
        {
            if (this.owner.DataGridView != null)
            {
                throw new InvalidOperationException(string.Format(SR.DataGridViewCellCollection_OwningRowAlreadyBelongsToDataGridView));
            }
            RemoveAtInternal(index);
        }

        internal void RemoveAtInternal(int index)
        {
            DataGridViewCell dataGridViewCell = (DataGridViewCell) this.items[index];
            this.items.RemoveAt(index);
            dataGridViewCell.DataGridViewInternal = null;
            dataGridViewCell.OwningRowInternal = null;
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
