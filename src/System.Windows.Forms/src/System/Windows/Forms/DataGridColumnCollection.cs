// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms {
    using System.Runtime.Remoting;

    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;

    using System;
    using System.Collections;
    using System.Drawing.Design;
    using System.Windows.Forms;
    using System.ComponentModel;
    using System.Globalization;

    /// <include file='doc\DataGridColumnCollection.uex' path='docs/doc[@for="GridColumnStylesCollection"]/*' />
    /// <devdoc>
    /// <para>Represents a collection of System.Windows.Forms.DataGridColumnStyle objects in the <see cref='System.Windows.Forms.DataGrid'/>
    /// control.</para>
    /// </devdoc>
    [
    Editor("System.Windows.Forms.Design.DataGridColumnCollectionEditor, " + AssemblyRef.SystemDesign, typeof(UITypeEditor)),
    ListBindable(false)
    ]
    public class GridColumnStylesCollection : BaseCollection, IList {
        CollectionChangeEventHandler onCollectionChanged;
        ArrayList        items = new ArrayList();
        DataGridTableStyle    owner = null;
        private     bool isDefault = false;

        // we have to implement IList for the Collection editor to work
        //
        /// <include file='doc\DataGridColumnCollection.uex' path='docs/doc[@for="GridColumnStylesCollection.IList.Add"]/*' />
        /// <internalonly/>
        int IList.Add(object value) {
            return this.Add((DataGridColumnStyle) value);            
        }

        /// <include file='doc\DataGridColumnCollection.uex' path='docs/doc[@for="GridColumnStylesCollection.IList.Clear"]/*' />
        /// <internalonly/>
        void IList.Clear() {
            this.Clear();
        }

        /// <include file='doc\DataGridColumnCollection.uex' path='docs/doc[@for="GridColumnStylesCollection.IList.Contains"]/*' />
        /// <internalonly/>
        bool IList.Contains(object value) {
            return items.Contains(value);
        }

        /// <include file='doc\DataGridColumnCollection.uex' path='docs/doc[@for="GridColumnStylesCollection.IList.IndexOf"]/*' />
        /// <internalonly/>
        int IList.IndexOf(object value) {
            return items.IndexOf(value);
        }

        /// <include file='doc\DataGridColumnCollection.uex' path='docs/doc[@for="GridColumnStylesCollection.IList.Insert"]/*' />
        /// <internalonly/>
        void IList.Insert(int index, object value) {
            throw new NotSupportedException();
        }

        /// <include file='doc\DataGridColumnCollection.uex' path='docs/doc[@for="GridColumnStylesCollection.IList.Remove"]/*' />
        /// <internalonly/>
        void IList.Remove(object value) {
            this.Remove((DataGridColumnStyle)value);
        }

        /// <include file='doc\DataGridColumnCollection.uex' path='docs/doc[@for="GridColumnStylesCollection.IList.RemoveAt"]/*' />
        /// <internalonly/>
        void IList.RemoveAt(int index) {
            this.RemoveAt(index);
        }

        /// <include file='doc\DataGridColumnCollection.uex' path='docs/doc[@for="GridColumnStylesCollection.IList.IsFixedSize"]/*' />
        /// <internalonly/>
        bool IList.IsFixedSize {
            get {return false;}
        }

        /// <include file='doc\DataGridColumnCollection.uex' path='docs/doc[@for="GridColumnStylesCollection.IList.IsReadOnly"]/*' />
        /// <internalonly/>
        bool IList.IsReadOnly {
            get {return false;}
        }

        /// <include file='doc\DataGridColumnCollection.uex' path='docs/doc[@for="GridColumnStylesCollection.IList.this"]/*' />
        /// <internalonly/>
        object IList.this[int index] {
            get { return items[index]; }
            set { throw new NotSupportedException(); }
        }

        /// <include file='doc\DataGridColumnCollection.uex' path='docs/doc[@for="GridColumnStylesCollection.ICollection.CopyTo"]/*' />
        /// <internalonly/>
        void ICollection.CopyTo(Array array, int index) {
            this.items.CopyTo(array, index);
        }

        /// <include file='doc\DataGridColumnCollection.uex' path='docs/doc[@for="GridColumnStylesCollection.ICollection.Count"]/*' />
        /// <internalonly/>
        int ICollection.Count {
            get {return this.items.Count;}
        }

        /// <include file='doc\DataGridColumnCollection.uex' path='docs/doc[@for="GridColumnStylesCollection.ICollection.IsSynchronized"]/*' />
        /// <internalonly/>
        bool ICollection.IsSynchronized {
            get {return false;}
        }

        /// <include file='doc\DataGridColumnCollection.uex' path='docs/doc[@for="GridColumnStylesCollection.ICollection.SyncRoot"]/*' />
        /// <internalonly/>
        object ICollection.SyncRoot {
            get {return this;}
        }

        /// <include file='doc\DataGridColumnCollection.uex' path='docs/doc[@for="GridColumnStylesCollection.IEnumerable.GetEnumerator"]/*' />
        /// <internalonly/>
        IEnumerator IEnumerable.GetEnumerator() {
            return items.GetEnumerator();
        }

        internal GridColumnStylesCollection(DataGridTableStyle table) {
            owner = table;
        }

        internal GridColumnStylesCollection(DataGridTableStyle table, bool isDefault) : this(table) {
            this.isDefault = isDefault;
        }

        /// <include file='doc\DataGridColumnCollection.uex' path='docs/doc[@for="GridColumnStylesCollection.List"]/*' />
        /// <devdoc>
        ///    <para>Gets the list of items in the collection.</para>
        /// </devdoc>
        protected override ArrayList List {
            get {
                return items;
            }
        }
        
        /* implemented in BaseCollection
        /// <summary>
        ///    <para>
        ///       Gets the number of System.Windows.Forms.DataGridColumnStyle objects in the collection.
        ///    </para>
        /// </summary>
        /// <value>
        ///    <para>
        ///       The number of System.Windows.Forms.DataGridColumnStyle objects in the System.Windows.Forms.GridColumnsStyleCollection .
        ///    </para>
        /// </value>
        /// <example>
        ///    <para>
        ///       The following example uses the <see cref='System.Windows.Forms.GridColumnsCollection.Count'/>
        ///       property to determine how many System.Windows.Forms.DataGridColumnStyle objects are in a System.Windows.Forms.GridColumnsStyleCollection, and uses that number to iterate through the
        ///       collection.
        ///    </para>
        ///    <code lang='VB'>
        /// Private Sub PrintGridColumns()
        ///    Dim colsCount As Integer
        ///    colsCount = DataGrid1.GridColumns.Count
        ///    Dim i As Integer
        ///    For i = 0 to colsCount - 1
        ///       Debug.Print DataGrid1.GridColumns(i).GetType.ToString
        ///    Next i
        /// End Sub
        ///    </code>
        /// </example>
        /// <seealso cref='System.Windows.Forms.GridColumnsCollection.Add'/>
        /// <seealso cref='System.Windows.Forms.GridColumnsCollection.Remove'/>
        public override int Count {
            get {
                return items.Count;
            }
        }
        */

        /// <include file='doc\DataGridColumnCollection.uex' path='docs/doc[@for="GridColumnStylesCollection.this"]/*' />
        /// <devdoc>
        /// <para>Gets the System.Windows.Forms.DataGridColumnStyle at a specified index.</para>
        /// </devdoc>
        public DataGridColumnStyle this[int index] {
            get {
                return (DataGridColumnStyle)items[index];
            }
        }

        /// <include file='doc\DataGridColumnCollection.uex' path='docs/doc[@for="GridColumnStylesCollection.this1"]/*' />
        /// <devdoc>
        /// <para>Gets the System.Windows.Forms.DataGridColumnStyle
        /// with the specified name.</para>
        /// </devdoc>
        public DataGridColumnStyle this[string columnName] {
            // PM team has reviewed and decided on naming changes already
            [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly")]
            get {
                int itemCount = items.Count;
                for (int i = 0; i < itemCount; ++i) {
                    DataGridColumnStyle column = (DataGridColumnStyle)items[i];
                    // NOTE: case-insensitive
                    if (String.Equals(column.MappingName, columnName, StringComparison.OrdinalIgnoreCase))
                        return column;
                }
                return null;
            }
        }

        internal DataGridColumnStyle MapColumnStyleToPropertyName(string mappingName) {
            int itemCount = items.Count;
            for (int i = 0; i < itemCount; ++i) {
                DataGridColumnStyle column = (DataGridColumnStyle)items[i];
                // NOTE: case-insensitive
                if (String.Equals(column.MappingName, mappingName, StringComparison.OrdinalIgnoreCase))
                    return column;
            }
            return null;
        }

        /// <include file='doc\DataGridColumnCollection.uex' path='docs/doc[@for="GridColumnStylesCollection.this2"]/*' />
        /// <devdoc>
        /// <para>Gets the System.Windows.Forms.DataGridColumnStyle associated with the
        ///    specified <see cref='System.Data.DataColumn'/>.</para>
        /// </devdoc>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1043:UseIntegralOrStringArgumentForIndexers")]
        public DataGridColumnStyle this[PropertyDescriptor propertyDesciptor] {
            [
                System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly")   // already shipped
            ]
            get {
                int itemCount = items.Count;
                for (int i = 0; i < itemCount; ++i) {
                    DataGridColumnStyle column = (DataGridColumnStyle)items[i];
                    if (propertyDesciptor.Equals(column.PropertyDescriptor))
                        return column;
                }
                return null;
            }
        }

        internal DataGridTableStyle DataGridTableStyle {
            get {
                return this.owner;
            }
        }

        /// <devdoc>
        /// <para>Adds a System.Windows.Forms.DataGridColumnStyle to the System.Windows.Forms.GridColumnStylesCollection</para>
        /// </devdoc>

        internal void CheckForMappingNameDuplicates(DataGridColumnStyle column) {
            if (String.IsNullOrEmpty(column.MappingName))
                return;
            for (int i = 0; i < items.Count; i++)
                if ( ((DataGridColumnStyle)items[i]).MappingName.Equals(column.MappingName) && column != items[i])
                    throw new ArgumentException(SR.DataGridColumnStyleDuplicateMappingName, "column");
        }

        private void ColumnStyleMappingNameChanged(object sender, EventArgs pcea) {
            OnCollectionChanged(new CollectionChangeEventArgs(CollectionChangeAction.Refresh, null));
        }

        private void ColumnStylePropDescChanged(object sender, EventArgs pcea) {
            OnCollectionChanged(new CollectionChangeEventArgs(CollectionChangeAction.Refresh, (DataGridColumnStyle) sender));
        }

        /// <include file='doc\DataGridColumnCollection.uex' path='docs/doc[@for="GridColumnStylesCollection.Add"]/*' />
        public virtual int Add(DataGridColumnStyle column) {
            if (this.isDefault) {
                throw new ArgumentException(SR.DataGridDefaultColumnCollectionChanged);
            }

            CheckForMappingNameDuplicates(column);

            column.SetDataGridTableInColumn(owner, true);
            column.MappingNameChanged += new EventHandler(ColumnStyleMappingNameChanged);
            column.PropertyDescriptorChanged += new EventHandler(ColumnStylePropDescChanged);

            // columns which are not the default should have a default
            // width of DataGrid.PreferredColumnWidth
            if (this.DataGridTableStyle != null && column.Width == -1)
                column.width = this.DataGridTableStyle.PreferredColumnWidth;
#if false
            column.AddOnPropertyChanged(owner.OnColumnChanged);
#endif
            int index = items.Add(column);
            OnCollectionChanged(new CollectionChangeEventArgs(CollectionChangeAction.Add, column));
            return index;
        }
        
        /// <include file='doc\DataGridColumnCollection.uex' path='docs/doc[@for="GridColumnStylesCollection.AddRange"]/*' />
        public void AddRange(DataGridColumnStyle[] columns) {
            if (columns == null) {
                throw new ArgumentNullException(nameof(columns));
            }
            for (int i = 0; i < columns.Length; i++) {
                Add(columns[i]);            
            }
        }

        // the dataGrid will need to add default columns to a default
        // table when there is no match for the listName in the tableStyle
        internal void AddDefaultColumn(DataGridColumnStyle column) {
#if DEBUG
            Debug.Assert(this.isDefault, "we should be calling this function only for default tables");
            Debug.Assert(column.IsDefault, "we should be a default column");
#endif // DEBUG
            column.SetDataGridTableInColumn(owner, true);
            this.items.Add(column);
        }

        internal void ResetDefaultColumnCollection() {
            Debug.Assert(this.isDefault, "we should be calling this function only for default tables");
            // unparent the edit controls
            for (int i = 0; i < Count; i++) {
                this[i].ReleaseHostedControl();
            }

            // get rid of the old list and get a new empty list
            items.Clear();
        }

        /// <include file='doc\DataGridColumnCollection.uex' path='docs/doc[@for="GridColumnStylesCollection.CollectionChanged"]/*' />
        /// <devdoc>
        /// <para>Occurs when a change is made to the System.Windows.Forms.GridColumnStylesCollection.</para>
        /// </devdoc>
        public event CollectionChangeEventHandler CollectionChanged {
            add {
                onCollectionChanged += value;
            }
            remove {
                onCollectionChanged -= value;
            }
        }
        
        /// <include file='doc\DataGridColumnCollection.uex' path='docs/doc[@for="GridColumnStylesCollection.Clear"]/*' />
        public void Clear() {
            for (int i = 0; i < Count; i ++) {
                this[i].ReleaseHostedControl();
            }
            items.Clear();
            OnCollectionChanged(new CollectionChangeEventArgs(CollectionChangeAction.Refresh, null));
        }

        /// <include file='doc\DataGridColumnCollection.uex' path='docs/doc[@for="GridColumnStylesCollection.Contains"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Gets a value indicating whether the System.Windows.Forms.GridColumnStylesCollection contains a System.Windows.Forms.DataGridColumnStyle associated with the
        ///       specified <see cref='System.Data.DataColumn'/>.
        ///    </para>
        /// </devdoc>
        public bool Contains(PropertyDescriptor propertyDescriptor) {
            return this[propertyDescriptor] != null;
        }

        /// <include file='doc\DataGridColumnCollection.uex' path='docs/doc[@for="GridColumnStylesCollection.Contains1"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Gets a value indicating whether the System.Windows.Forms.GridColumnsStyleCollection contains the specified System.Windows.Forms.DataGridColumnStyle.
        ///    </para>
        /// </devdoc>
        public bool Contains(DataGridColumnStyle column) {
            int index = items.IndexOf(column);
            return index != -1;
        }

        /// <include file='doc\DataGridColumnCollection.uex' path='docs/doc[@for="GridColumnStylesCollection.Contains2"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Gets a value indicating whether the System.Windows.Forms.GridColumnsStyleCollection contains the System.Windows.Forms.DataGridColumnStyle with the specified name.
        ///    </para>
        /// </devdoc>
        public bool Contains(string name) {
            IEnumerator e = items.GetEnumerator();
            while (e.MoveNext()) {
                DataGridColumnStyle column = (DataGridColumnStyle)e.Current;
                // NOTE: case-insensitive
                if (String.Compare(column.MappingName, name, true, CultureInfo.InvariantCulture) == 0)
                    return true;
            }
            return false;
        }

        /* implemented at BaseCollection
        /// <overload>
        ///    <para>
        ///       Gets an enumerator for the System.Windows.Forms.GridColumnsStyleCollection.
        ///    </para>
        /// </overload>
        /// <summary>
        ///    <para>
        ///       Gets an enumerator for the System.Windows.Forms.GridColumnsStyleCollection.
        ///    </para>
        /// </summary>
        /// <returns>
        ///    <para>
        ///       An <see cref='System.Collections.IEnumerator'/>
        ///       that can be used to iterate through the collection.
        ///    </para>
        /// </returns>
        /// <example>
        ///    <para>
        ///       The following example gets an <see cref='System.Collections.IEnumerator'/> that iterates through the System.Windows.Forms.GridColumnsStyleCollection. and prints the
        ///    <see cref='System.Windows.Forms.GridColumnsCollection.Caption'/> of each <see cref='System.Data.DataColumn'/> 
        ///    associated with the object.
        /// </para>
        /// <code lang='VB'>
        /// Private Sub EnumerateThroughGridColumns()
        ///    Dim ie As System.Collections.IEnumerator
        ///    Dim dgCol As DataGridColumn
        ///    Set ie = DataGrid1.GridColumns.GetEnumerator
        ///    Do While ie.GetNext = True
        ///       Set dgCol = ie.GetObject
        ///       Debug.Print dgCol.DataColumn.Caption
        ///    Loop
        /// End Sub
        /// </code>
        /// </example>
        /// <seealso cref='System.Data.DataColumn'/>
        /// <seealso cref='System.Collections.IEnumerator'/>
        /// <seealso cref='System.Windows.Forms.IEnumerator.GetNext'/>
        /// <seealso cref='System.Windows.Forms.IEnumerator.GetObject'/>
        public override IEnumerator GetEnumerator() {
            return items.GetEnumerator();
        }

        /// <summary>
        ///    <para>
        ///       Gets an enumerator for the System.Windows.Forms.GridColumnsStyleCollection
        ///       .
        ///    </para>
        /// </summary>
        /// <param name='allowRemove'>
        /// <para>A value that indicates if the enumerator can remove elements. <see langword='true'/>, if removals are allowed; otherwise, <see langword='false'/>. The default is <see langword='false'/>.</para>
        /// </param>
        /// <returns>
        ///    <para>
        ///       An <see cref='System.Collections.IEnumerator'/> that can be used to iterate through the
        ///       collection.
        ///    </para>
        /// </returns>
        /// <exception cref='NotSupportedException'>
        ///    An attempt was made to remove the System.Windows.Forms.DataGridColumnStyle through the <see cref='System.Collections.Enumerator'/> object's <see cref='System.Windows.Forms.Enumerator.Remove'/> method. Use the System.Windows.Forms.GridColumnsStyleCollection object's <see cref='System.Windows.Forms.GridColumnsCollection.Remove'/> method instead.
        /// </exception>
        /// <remarks>
        ///    <para>
        ///       Because this implementation doesn't support the removal
        ///       of System.Windows.Forms.DataGridColumnStyle objects through the <see cref='System.Collections.Enumerator'/>
        ///       class's <see cref='System.Windows.Forms.Enumerator.Remove'/> method, you must use the <see cref='System.Windows.Forms.DataGridCollection'/> class's <see cref='System.Windows.Forms.GridColumnsCollection.Remove'/>
        ///       method instead.
        ///    </para>
        /// </remarks>
        /// <example>
        ///    <para>
        ///       The following example gets an <see cref='System.Collections.IEnumerator'/> for that iterates through the System.Windows.Forms.GridColumnsStyleCollection. If a column in the collection is of type <see cref='System.Windows.Forms.DataGridBoolColumn'/>, it is deleted.
        ///    </para>
        ///    <code lang='VB'>
        /// Private Sub RemoveBoolColumns()
        ///    Dim ie As System.Collections.IEnumerator
        ///    Dim dgCol As DataGridColumn
        ///    Set ie = DataGrid1.GridColumns.GetEnumerator(true)
        ///    Do While ie.GetNext
        ///       Set dgCol = ie.GetObject
        ///       
        ///       If dgCol.ToString = "DataGridBoolColumn" Then
        ///          DataGrid1.GridColumns.Remove dgCol
        ///       End If
        ///    Loop
        /// End If
        ///    </code>
        /// </example>
        /// <seealso cref='System.Collections.IEnumerator'/>
        /// <seealso cref='System.Windows.Forms.IEnumerator.GetNext'/>
        /// <seealso cref='System.Windows.Forms.IEnumerator.GetObject'/>
        /// <seealso cref='System.Windows.Forms.GridColumnsCollection.Remove'/>
        public override IEnumerator GetEnumerator(bool allowRemove) {
            if (!allowRemove)
                return GetEnumerator();
            else
                throw new NotSupportedException(SR.DataGridColumnCollectionGetEnumerator);
        }
        */

        /// <include file='doc\DataGridColumnCollection.uex' path='docs/doc[@for="GridColumnStylesCollection.IndexOf"]/*' />
        /// <devdoc>
        /// <para>Gets the index of a specified System.Windows.Forms.DataGridColumnStyle.</para>
        /// </devdoc>
        public int IndexOf(DataGridColumnStyle element) {
            int itemCount = items.Count;
            for (int i = 0; i < itemCount; ++i) {
                DataGridColumnStyle column = (DataGridColumnStyle)items[i];
                if (element == column)
                    return i;
            }
            return -1;
        }

        /// <include file='doc\DataGridColumnCollection.uex' path='docs/doc[@for="GridColumnStylesCollection.OnCollectionChanged"]/*' />
        /// <devdoc>
        /// <para>Raises the System.Windows.Forms.GridColumnsCollection.CollectionChanged event.</para>
        /// </devdoc>
        protected void OnCollectionChanged(CollectionChangeEventArgs e) {
            if (onCollectionChanged != null)
                onCollectionChanged(this, e);

            DataGrid grid = owner.DataGrid;
            if (grid != null) {
                grid.checkHierarchy = true;
            }
        }

        /// <include file='doc\DataGridColumnCollection.uex' path='docs/doc[@for="GridColumnStylesCollection.Remove"]/*' />
        /// <devdoc>
        /// <para>Removes the specified System.Windows.Forms.DataGridColumnStyle from the System.Windows.Forms.GridColumnsStyleCollection.</para>
        /// </devdoc>
        public void Remove(DataGridColumnStyle column) {
            if (this.isDefault) {
                throw new ArgumentException(SR.DataGridDefaultColumnCollectionChanged);
            }

            int columnIndex = -1;
            int itemsCount = items.Count;
            for (int i = 0; i < itemsCount; ++i)
                if (items[i] == column) {
                    columnIndex = i;
                    break;
                }
            if (columnIndex == -1)
                throw new InvalidOperationException(SR.DataGridColumnCollectionMissing);
            else
                RemoveAt(columnIndex);
        }

        /// <include file='doc\DataGridColumnCollection.uex' path='docs/doc[@for="GridColumnStylesCollection.RemoveAt"]/*' />
        /// <devdoc>
        /// <para>Removes the System.Windows.Forms.DataGridColumnStyle with the specified index from the System.Windows.Forms.GridColumnsStyleCollection.</para>
        /// </devdoc>
        public void RemoveAt(int index) {
            if (this.isDefault) {
                throw new ArgumentException(SR.DataGridDefaultColumnCollectionChanged);
            }

            DataGridColumnStyle toRemove = (DataGridColumnStyle)items[index];
            toRemove.SetDataGridTableInColumn(null, true);
            toRemove.MappingNameChanged -= new EventHandler(ColumnStyleMappingNameChanged);
            toRemove.PropertyDescriptorChanged -= new EventHandler(ColumnStylePropDescChanged);
#if false
            toRemove.RemoveOnPropertyChange(owner.OnColumnChanged);
#endif
            items.RemoveAt(index);
            OnCollectionChanged(new CollectionChangeEventArgs(CollectionChangeAction.Remove, toRemove));
        }

        /// <include file='doc\DataGridColumnCollection.uex' path='docs/doc[@for="GridColumnStylesCollection.ResetPropertyDescriptors"]/*' />
        public void ResetPropertyDescriptors() {
            for (int i = 0; i < this.Count; i++) {
                this[i].PropertyDescriptor = null;
            }
        }
    }
}
