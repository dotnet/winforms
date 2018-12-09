// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms {

    using System.Diagnostics;

    using System;
    using System.Collections;
    using System.Windows.Forms;
    using System.ComponentModel;
    using System.Globalization;
    
    /// <include file='doc\DataGridTableCollection.uex' path='docs/doc[@for="GridTableStylesCollection"]/*' />
    /// <devdoc>
    /// <para>Represents a collection of <see cref='System.Windows.Forms.DataGridTableStyle'/> objects in the <see cref='System.Windows.Forms.DataGrid'/> 
    /// control.</para>
    /// </devdoc>
    [ListBindable(false)]
    public class GridTableStylesCollection : BaseCollection ,IList {
        CollectionChangeEventHandler onCollectionChanged;
        ArrayList        items = new ArrayList();
        DataGrid         owner  = null;

        /// <include file='doc\DataGridTableCollection.uex' path='docs/doc[@for="GridTableStylesCollection.IList.Add"]/*' />
        /// <internalonly/>
        int IList.Add(object value) {
            return this.Add((DataGridTableStyle) value);            
        }

        /// <include file='doc\DataGridTableCollection.uex' path='docs/doc[@for="GridTableStylesCollection.IList.Clear"]/*' />
        /// <internalonly/>
        void IList.Clear() {
            this.Clear();
        }

        /// <include file='doc\DataGridTableCollection.uex' path='docs/doc[@for="GridTableStylesCollection.IList.Contains"]/*' />
        /// <internalonly/>
        bool IList.Contains(object value) {
            return items.Contains(value);
        }

        /// <include file='doc\DataGridTableCollection.uex' path='docs/doc[@for="GridTableStylesCollection.IList.IndexOf"]/*' />
        /// <internalonly/>
        int IList.IndexOf(object value) {
            return items.IndexOf(value);
        }

        /// <include file='doc\DataGridTableCollection.uex' path='docs/doc[@for="GridTableStylesCollection.IList.Insert"]/*' />
        /// <internalonly/>
        void IList.Insert(int index, object value) {
            throw new NotSupportedException();
        }

        /// <include file='doc\DataGridTableCollection.uex' path='docs/doc[@for="GridTableStylesCollection.IList.Remove"]/*' />
        /// <internalonly/>
        void IList.Remove(object value) {
            this.Remove((DataGridTableStyle)value);
        }

        /// <include file='doc\DataGridTableCollection.uex' path='docs/doc[@for="GridTableStylesCollection.IList.RemoveAt"]/*' />
        /// <internalonly/>
        void IList.RemoveAt(int index) {
            this.RemoveAt(index);
        }

        /// <include file='doc\DataGridTableCollection.uex' path='docs/doc[@for="GridTableStylesCollection.IList.IsFixedSize"]/*' />
        /// <internalonly/>
        bool IList.IsFixedSize {
            get {return false;}
        }

        /// <include file='doc\DataGridTableCollection.uex' path='docs/doc[@for="GridTableStylesCollection.IList.IsReadOnly"]/*' />
        /// <internalonly/>
        bool IList.IsReadOnly {
            get {return false;}
        }

        /// <include file='doc\DataGridTableCollection.uex' path='docs/doc[@for="GridTableStylesCollection.IList.this"]/*' />
        /// <internalonly/>
        object IList.this[int index] {
            get { return items[index]; }
            set { throw new NotSupportedException(); }
        }

        /// <include file='doc\DataGridTableCollection.uex' path='docs/doc[@for="GridTableStylesCollection.ICollection.CopyTo"]/*' />
        /// <internalonly/>
        void ICollection.CopyTo(Array array, int index) {
            this.items.CopyTo(array, index);
        }

        /// <include file='doc\DataGridTableCollection.uex' path='docs/doc[@for="GridTableStylesCollection.ICollection.Count"]/*' />
        /// <internalonly/>
        int ICollection.Count {
            get {return this.items.Count;}
        }

        /// <include file='doc\DataGridTableCollection.uex' path='docs/doc[@for="GridTableStylesCollection.ICollection.IsSynchronized"]/*' />
        /// <internalonly/>
        bool ICollection.IsSynchronized {
            get {return false;}
        }

        /// <include file='doc\DataGridTableCollection.uex' path='docs/doc[@for="GridTableStylesCollection.ICollection.SyncRoot"]/*' />
        /// <internalonly/>
        object ICollection.SyncRoot {
            get {return this;}
        }

        /// <include file='doc\DataGridTableCollection.uex' path='docs/doc[@for="GridTableStylesCollection.IEnumerable.GetEnumerator"]/*' />
        /// <internalonly/>
        IEnumerator IEnumerable.GetEnumerator() {
            return items.GetEnumerator();
        }

        internal GridTableStylesCollection(DataGrid grid) {
            owner = grid;
        }

        /// <include file='doc\DataGridTableCollection.uex' path='docs/doc[@for="GridTableStylesCollection.List"]/*' />
        protected override ArrayList List {
            get {
                return items;
            }
        }
        
        /* implemented in BaseCollection
        /// <summary>
        ///      Retrieves the number of GridTables in the collection.
        /// </summary>
        /// <returns>
        ///      The number of GridTables in the collection.
        /// </returns>
        public override int Count {
            get {
                return items.Count;
            }
        }
        */

        /// <include file='doc\DataGridTableCollection.uex' path='docs/doc[@for="GridTableStylesCollection.this"]/*' />
        /// <devdoc>
        ///      Retrieves the DataGridTable with the specified index.
        /// </devdoc>
        public DataGridTableStyle this[int index] {
            get {
                return (DataGridTableStyle)items[index];
            }
        }

        /// <include file='doc\DataGridTableCollection.uex' path='docs/doc[@for="GridTableStylesCollection.this1"]/*' />
        /// <devdoc>
        ///      Retrieves the DataGridTable with the name provided.
        /// </devdoc>
        public DataGridTableStyle this[string tableName] {
            get {
                if (tableName == null)
                    throw new ArgumentNullException(nameof(tableName));
                int itemCount = items.Count;
                for (int i = 0; i < itemCount; ++i) {
                    DataGridTableStyle table = (DataGridTableStyle)items[i];
                    // NOTE: case-insensitive
                    if (String.Equals(table.MappingName, tableName, StringComparison.OrdinalIgnoreCase))
                        return table;
                }
                return null;
            }
        }

        internal void CheckForMappingNameDuplicates(DataGridTableStyle table) {
            if (String.IsNullOrEmpty(table.MappingName))
                return;
            for (int i = 0; i < items.Count; i++)
                if ( ((DataGridTableStyle)items[i]).MappingName.Equals(table.MappingName) && table != items[i])
                    throw new ArgumentException(SR.DataGridTableStyleDuplicateMappingName, "table");
        }

        /// <include file='doc\DataGridTableCollection.uex' path='docs/doc[@for="GridTableStylesCollection.Add"]/*' />
        /// <devdoc>
        /// <para>Adds a <see cref='System.Windows.Forms.DataGridTableStyle'/> to this collection.</para>
        /// </devdoc>
        public virtual int Add(DataGridTableStyle table) {
            // set the rowHeaderWidth on the newly added table to at least the minimum value
            // on its owner
            if (this.owner != null && this.owner.MinimumRowHeaderWidth() > table.RowHeaderWidth)
                table.RowHeaderWidth = this.owner.MinimumRowHeaderWidth();

            if (table.DataGrid != owner && table.DataGrid != null)
                throw new ArgumentException(SR.DataGridTableStyleCollectionAddedParentedTableStyle, "table");
            table.DataGrid = owner;
            CheckForMappingNameDuplicates(table);
            table.MappingNameChanged += new EventHandler(TableStyleMappingNameChanged);
            int index = items.Add(table);
            OnCollectionChanged(new CollectionChangeEventArgs(CollectionChangeAction.Add, table));
            
            return index;
        }
        
        private void TableStyleMappingNameChanged(object sender, EventArgs pcea) {
            OnCollectionChanged(new CollectionChangeEventArgs(CollectionChangeAction.Refresh, null));
        }
        
        /// <include file='doc\DataGridTableCollection.uex' path='docs/doc[@for="GridTableStylesCollection.AddRange"]/*' />
        public virtual void AddRange(DataGridTableStyle[] tables) {
            if (tables == null) {
                throw new ArgumentNullException(nameof(tables));
            }
            foreach(DataGridTableStyle table in tables) {
                table.DataGrid = owner;
                table.MappingNameChanged += new EventHandler(TableStyleMappingNameChanged);
                items.Add(table);
            }
            OnCollectionChanged(new CollectionChangeEventArgs(CollectionChangeAction.Refresh, null));
        }

        /// <include file='doc\DataGridTableCollection.uex' path='docs/doc[@for="GridTableStylesCollection.CollectionChanged"]/*' />
        public event CollectionChangeEventHandler CollectionChanged {
            add {
                onCollectionChanged += value;
            }
            remove {
                onCollectionChanged -= value;
            }
        }
        
        /// <include file='doc\DataGridTableCollection.uex' path='docs/doc[@for="GridTableStylesCollection.Clear"]/*' />
        public void Clear() {
            for (int i = 0; i < items.Count; i++) {
                DataGridTableStyle element = (DataGridTableStyle)items[i];
                element.MappingNameChanged -= new EventHandler(TableStyleMappingNameChanged);
            }

            items.Clear();
            OnCollectionChanged(new CollectionChangeEventArgs(CollectionChangeAction.Refresh, null));
        }

        /// <include file='doc\DataGridTableCollection.uex' path='docs/doc[@for="GridTableStylesCollection.Contains"]/*' />
        /// <devdoc>
        ///      Checks to see if a DataGridTableStyle is contained in this collection.
        /// </devdoc>
        public bool Contains(DataGridTableStyle table) {
            int index = items.IndexOf(table);
            return index != -1;
        }

        /// <include file='doc\DataGridTableCollection.uex' path='docs/doc[@for="GridTableStylesCollection.Contains1"]/*' />
        /// <devdoc>
        /// <para>Checks to see if a <see cref='System.Windows.Forms.DataGridTableStyle'/> with the given name
        ///    is contained in this collection.</para>
        /// </devdoc>
        public bool Contains(string name) {
            int itemCount = items.Count;
            for (int i = 0; i < itemCount; ++i) {
                DataGridTableStyle table = (DataGridTableStyle)items[i];
                // NOTE: case-insensitive
                if (String.Compare(table.MappingName, name, true, CultureInfo.InvariantCulture) == 0)
                    return true;
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

        /// <include file='doc\DataGridTableCollection.uex' path='docs/doc[@for="GridTableStylesCollection.OnCollectionChanged"]/*' />
        protected void OnCollectionChanged(CollectionChangeEventArgs e) {
            if (onCollectionChanged != null)
                onCollectionChanged(this, e);

            DataGrid grid = owner;
            if (grid != null) {
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

        /// <include file='doc\DataGridTableCollection.uex' path='docs/doc[@for="GridTableStylesCollection.Remove"]/*' />
        public void Remove(DataGridTableStyle table) {
            int tableIndex = -1;
            int itemsCount = items.Count;
            for (int i = 0; i < itemsCount; ++i)
                if (items[i] == table) {
                    tableIndex = i;
                    break;
                }
            if (tableIndex == -1)
                throw new ArgumentException(SR.DataGridTableCollectionMissingTable, "table");
            else
                RemoveAt(tableIndex);
        }

        /// <include file='doc\DataGridTableCollection.uex' path='docs/doc[@for="GridTableStylesCollection.RemoveAt"]/*' />
        public void RemoveAt(int index) {
            DataGridTableStyle element = (DataGridTableStyle)items[index];
            element.MappingNameChanged -= new EventHandler(TableStyleMappingNameChanged);
            items.RemoveAt(index);
            OnCollectionChanged(new CollectionChangeEventArgs(CollectionChangeAction.Remove, element));
        }
    }
}
