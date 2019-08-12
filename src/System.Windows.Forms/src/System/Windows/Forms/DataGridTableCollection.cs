// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections;
using System.ComponentModel;
using System.Globalization;

namespace System.Windows.Forms
{
    /// <summary>
    ///  Represents a collection of <see cref='DataGridTableStyle'/> objects in the <see cref='DataGrid'/>
    ///  control.
    /// </summary>
    [ListBindable(false)]
    public class GridTableStylesCollection : BaseCollection, IList
    {
        CollectionChangeEventHandler onCollectionChanged;
        readonly ArrayList items = new ArrayList();
        readonly DataGrid owner = null;

        int IList.Add(object value)
        {
            return Add((DataGridTableStyle)value);
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
            throw new NotSupportedException();
        }

        void IList.Remove(object value)
        {
            Remove((DataGridTableStyle)value);
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
            get { return items[index]; }
            set { throw new NotSupportedException(); }
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

        internal GridTableStylesCollection(DataGrid grid)
        {
            owner = grid;
        }

        protected override ArrayList List
        {
            get
            {
                return items;
            }
        }

        /* implemented in BaseCollection
        /// <summary>
        ///  Retrieves the number of GridTables in the collection.
        /// </summary>
        /// <returns>
        ///  The number of GridTables in the collection.
        /// </returns>
        public override int Count {
            get {
                return items.Count;
            }
        }
        */

        /// <summary>
        ///  Retrieves the DataGridTable with the specified index.
        /// </summary>
        public DataGridTableStyle this[int index]
        {
            get
            {
                return (DataGridTableStyle)items[index];
            }
        }

        /// <summary>
        ///  Retrieves the DataGridTable with the name provided.
        /// </summary>
        public DataGridTableStyle this[string tableName]
        {
            get
            {
                if (tableName == null)
                {
                    throw new ArgumentNullException(nameof(tableName));
                }

                int itemCount = items.Count;
                for (int i = 0; i < itemCount; ++i)
                {
                    DataGridTableStyle table = (DataGridTableStyle)items[i];
                    // NOTE: case-insensitive
                    if (string.Equals(table.MappingName, tableName, StringComparison.OrdinalIgnoreCase))
                    {
                        return table;
                    }
                }
                return null;
            }
        }

        internal void CheckForMappingNameDuplicates(DataGridTableStyle table)
        {
            if (string.IsNullOrEmpty(table.MappingName))
            {
                return;
            }

            for (int i = 0; i < items.Count; i++)
            {
                if (((DataGridTableStyle)items[i]).MappingName.Equals(table.MappingName) && table != items[i])
                {
                    throw new ArgumentException(SR.DataGridTableStyleDuplicateMappingName, "table");
                }
            }
        }

        /// <summary>
        ///  Adds a <see cref='DataGridTableStyle'/> to this collection.
        /// </summary>
        public virtual int Add(DataGridTableStyle table)
        {
            // set the rowHeaderWidth on the newly added table to at least the minimum value
            // on its owner
            if (owner != null && owner.MinimumRowHeaderWidth() > table.RowHeaderWidth)
            {
                table.RowHeaderWidth = owner.MinimumRowHeaderWidth();
            }

            if (table.DataGrid != owner && table.DataGrid != null)
            {
                throw new ArgumentException(SR.DataGridTableStyleCollectionAddedParentedTableStyle, "table");
            }

            table.DataGrid = owner;
            CheckForMappingNameDuplicates(table);
            table.MappingNameChanged += new EventHandler(TableStyleMappingNameChanged);
            int index = items.Add(table);
            OnCollectionChanged(new CollectionChangeEventArgs(CollectionChangeAction.Add, table));

            return index;
        }

        private void TableStyleMappingNameChanged(object sender, EventArgs pcea)
        {
            OnCollectionChanged(new CollectionChangeEventArgs(CollectionChangeAction.Refresh, null));
        }

        public virtual void AddRange(DataGridTableStyle[] tables)
        {
            if (tables == null)
            {
                throw new ArgumentNullException(nameof(tables));
            }
            foreach (DataGridTableStyle table in tables)
            {
                table.DataGrid = owner;
                table.MappingNameChanged += new EventHandler(TableStyleMappingNameChanged);
                items.Add(table);
            }
            OnCollectionChanged(new CollectionChangeEventArgs(CollectionChangeAction.Refresh, null));
        }

        public event CollectionChangeEventHandler CollectionChanged
        {
            add => onCollectionChanged += value;
            remove => onCollectionChanged -= value;
        }

        public void Clear()
        {
            for (int i = 0; i < items.Count; i++)
            {
                DataGridTableStyle element = (DataGridTableStyle)items[i];
                element.MappingNameChanged -= new EventHandler(TableStyleMappingNameChanged);
            }

            items.Clear();
            OnCollectionChanged(new CollectionChangeEventArgs(CollectionChangeAction.Refresh, null));
        }

        /// <summary>
        ///  Checks to see if a DataGridTableStyle is contained in this collection.
        /// </summary>
        public bool Contains(DataGridTableStyle table)
        {
            int index = items.IndexOf(table);
            return index != -1;
        }

        /// <summary>
        ///  Checks to see if a <see cref='DataGridTableStyle'/> with the given name
        ///  is contained in this collection.
        /// </summary>
        public bool Contains(string name)
        {
            int itemCount = items.Count;
            for (int i = 0; i < itemCount; ++i)
            {
                DataGridTableStyle table = (DataGridTableStyle)items[i];
                // NOTE: case-insensitive
                if (string.Compare(table.MappingName, name, true, CultureInfo.InvariantCulture) == 0)
                {
                    return true;
                }
            }
            return false;
        }

        /*
        public override IEnumerator GetEnumerator() {
            return items.GetEnumerator();
        }

        public override IEnumerator GetEnumerator(bool allowRemove) {
            if (!allowRemove)
                return GetEnumerator();
            else
                throw new NotSupportedException(SR.DataGridTableCollectionGetEnumerator);
        }
        */

        protected void OnCollectionChanged(CollectionChangeEventArgs e)
        {
            onCollectionChanged?.Invoke(this, e);

            DataGrid grid = owner;
            if (grid != null)
            {
                /* FOR DEMO: Microsoft: TableStylesCollection::OnCollectionChanged: set the datagridtble
                DataView dataView = ((DataView) grid.DataSource);
                if (dataView != null) {
                    DataTable dataTable = dataView.Table;
                    if (dataTable != null) {
                        if (Contains(dataTable)) {
                            grid.SetDataGridTable(this[dataTable]);
                        }
                    }
                }
                */
                grid.checkHierarchy = true;
            }
        }

        public void Remove(DataGridTableStyle table)
        {
            int tableIndex = -1;
            int itemsCount = items.Count;
            for (int i = 0; i < itemsCount; ++i)
            {
                if (items[i] == table)
                {
                    tableIndex = i;
                    break;
                }
            }

            if (tableIndex == -1)
            {
                throw new ArgumentException(SR.DataGridTableCollectionMissingTable, "table");
            }
            else
            {
                RemoveAt(tableIndex);
            }
        }

        public void RemoveAt(int index)
        {
            DataGridTableStyle element = (DataGridTableStyle)items[index];
            element.MappingNameChanged -= new EventHandler(TableStyleMappingNameChanged);
            items.RemoveAt(index);
            OnCollectionChanged(new CollectionChangeEventArgs(CollectionChangeAction.Remove, element));
        }
    }
}
