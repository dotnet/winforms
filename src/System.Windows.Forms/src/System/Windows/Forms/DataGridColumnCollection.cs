// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing.Design;
using System.Globalization;

namespace System.Windows.Forms
{
    /// <summary>
    ///  Represents a collection of System.Windows.Forms.DataGridColumnStyle objects in the <see cref='DataGrid'/>
    ///  control.
    /// </summary>
    [
    Editor("System.Windows.Forms.Design.DataGridColumnCollectionEditor, " + AssemblyRef.SystemDesign, typeof(UITypeEditor)),
    ListBindable(false)
    ]
    public class GridColumnStylesCollection : BaseCollection, IList
    {
        CollectionChangeEventHandler onCollectionChanged;
        readonly ArrayList items = new ArrayList();
        readonly DataGridTableStyle owner = null;
        private readonly bool isDefault = false;

        // we have to implement IList for the Collection editor to work
        //
        int IList.Add(object value)
        {
            return Add((DataGridColumnStyle)value);
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
            Remove((DataGridColumnStyle)value);
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

        internal GridColumnStylesCollection(DataGridTableStyle table)
        {
            owner = table;
        }

        internal GridColumnStylesCollection(DataGridTableStyle table, bool isDefault) : this(table)
        {
            this.isDefault = isDefault;
        }

        /// <summary>
        ///  Gets the list of items in the collection.
        /// </summary>
        protected override ArrayList List
        {
            get
            {
                return items;
            }
        }

        /* implemented in BaseCollection
        /// <summary>
        ///  Gets the number of System.Windows.Forms.DataGridColumnStyle objects in the collection.
        /// </summary>
        /// <value>
        ///  The number of System.Windows.Forms.DataGridColumnStyle objects in the System.Windows.Forms.GridColumnsStyleCollection .
        /// </value>
        /// <example>
        ///  The following example uses the <see cref='System.Windows.Forms.GridColumnsCollection.Count'/>
        ///  property to determine how many System.Windows.Forms.DataGridColumnStyle objects are in a System.Windows.Forms.GridColumnsStyleCollection, and uses that number to iterate through the
        ///  collection.
        ///  <code lang='VB'>
        ///  Private Sub PrintGridColumns()
        ///  Dim colsCount As Integer
        ///  colsCount = DataGrid1.GridColumns.Count
        ///  Dim i As Integer
        ///  For i = 0 to colsCount - 1
        ///  Debug.Print DataGrid1.GridColumns(i).GetType.ToString
        ///  Next i
        ///  End Sub
        ///  </code>
        /// </example>
        /// <seealso cref='System.Windows.Forms.GridColumnsCollection.Add'/>
        /// <seealso cref='System.Windows.Forms.GridColumnsCollection.Remove'/>
        public override int Count {
            get {
                return items.Count;
            }
        }
        */

        /// <summary>
        ///  Gets the System.Windows.Forms.DataGridColumnStyle at a specified index.
        /// </summary>
        public DataGridColumnStyle this[int index]
        {
            get
            {
                return (DataGridColumnStyle)items[index];
            }
        }

        /// <summary>
        ///  Gets the System.Windows.Forms.DataGridColumnStyle
        ///  with the specified name.
        /// </summary>
        public DataGridColumnStyle this[string columnName]
        {
            get
            {
                int itemCount = items.Count;
                for (int i = 0; i < itemCount; ++i)
                {
                    DataGridColumnStyle column = (DataGridColumnStyle)items[i];
                    // NOTE: case-insensitive
                    if (string.Equals(column.MappingName, columnName, StringComparison.OrdinalIgnoreCase))
                    {
                        return column;
                    }
                }
                return null;
            }
        }

        internal DataGridColumnStyle MapColumnStyleToPropertyName(string mappingName)
        {
            int itemCount = items.Count;
            for (int i = 0; i < itemCount; ++i)
            {
                DataGridColumnStyle column = (DataGridColumnStyle)items[i];
                // NOTE: case-insensitive
                if (string.Equals(column.MappingName, mappingName, StringComparison.OrdinalIgnoreCase))
                {
                    return column;
                }
            }
            return null;
        }

        /// <summary>
        ///  Gets the System.Windows.Forms.DataGridColumnStyle associated with the
        ///  specified <see cref='Data.DataColumn'/>.
        /// </summary>
        public DataGridColumnStyle this[PropertyDescriptor propertyDesciptor]
        {
            get
            {
                int itemCount = items.Count;
                for (int i = 0; i < itemCount; ++i)
                {
                    DataGridColumnStyle column = (DataGridColumnStyle)items[i];
                    if (propertyDesciptor.Equals(column.PropertyDescriptor))
                    {
                        return column;
                    }
                }
                return null;
            }
        }

        internal DataGridTableStyle DataGridTableStyle
        {
            get
            {
                return owner;
            }
        }

        /// <summary>
        ///  Adds a System.Windows.Forms.DataGridColumnStyle to the System.Windows.Forms.GridColumnStylesCollection
        /// </summary>
        internal void CheckForMappingNameDuplicates(DataGridColumnStyle column)
        {
            if (string.IsNullOrEmpty(column.MappingName))
            {
                return;
            }

            for (int i = 0; i < items.Count; i++)
            {
                if (((DataGridColumnStyle)items[i]).MappingName.Equals(column.MappingName) && column != items[i])
                {
                    throw new ArgumentException(SR.DataGridColumnStyleDuplicateMappingName, "column");
                }
            }
        }

        private void ColumnStyleMappingNameChanged(object sender, EventArgs pcea)
        {
            OnCollectionChanged(new CollectionChangeEventArgs(CollectionChangeAction.Refresh, null));
        }

        private void ColumnStylePropDescChanged(object sender, EventArgs pcea)
        {
            OnCollectionChanged(new CollectionChangeEventArgs(CollectionChangeAction.Refresh, (DataGridColumnStyle)sender));
        }

        public virtual int Add(DataGridColumnStyle column)
        {
            if (isDefault)
            {
                throw new ArgumentException(SR.DataGridDefaultColumnCollectionChanged);
            }

            CheckForMappingNameDuplicates(column);

            column.SetDataGridTableInColumn(owner, true);
            column.MappingNameChanged += new EventHandler(ColumnStyleMappingNameChanged);
            column.PropertyDescriptorChanged += new EventHandler(ColumnStylePropDescChanged);

            // columns which are not the default should have a default
            // width of DataGrid.PreferredColumnWidth
            if (DataGridTableStyle != null && column.Width == -1)
            {
                column._width = DataGridTableStyle.PreferredColumnWidth;
            }
#if false
            column.AddOnPropertyChanged(owner.OnColumnChanged);
#endif
            int index = items.Add(column);
            OnCollectionChanged(new CollectionChangeEventArgs(CollectionChangeAction.Add, column));
            return index;
        }

        public void AddRange(DataGridColumnStyle[] columns)
        {
            if (columns == null)
            {
                throw new ArgumentNullException(nameof(columns));
            }
            for (int i = 0; i < columns.Length; i++)
            {
                Add(columns[i]);
            }
        }

        // the dataGrid will need to add default columns to a default
        // table when there is no match for the listName in the tableStyle
        internal void AddDefaultColumn(DataGridColumnStyle column)
        {
#if DEBUG
            Debug.Assert(isDefault, "we should be calling this function only for default tables");
            Debug.Assert(column.IsDefault, "we should be a default column");
#endif // DEBUG
            column.SetDataGridTableInColumn(owner, true);
            items.Add(column);
        }

        internal void ResetDefaultColumnCollection()
        {
            Debug.Assert(isDefault, "we should be calling this function only for default tables");
            // unparent the edit controls
            for (int i = 0; i < Count; i++)
            {
                this[i].ReleaseHostedControl();
            }

            // get rid of the old list and get a new empty list
            items.Clear();
        }

        /// <summary>
        ///  Occurs when a change is made to the System.Windows.Forms.GridColumnStylesCollection.
        /// </summary>
        public event CollectionChangeEventHandler CollectionChanged
        {
            add => onCollectionChanged += value;
            remove => onCollectionChanged -= value;
        }

        public void Clear()
        {
            for (int i = 0; i < Count; i++)
            {
                this[i].ReleaseHostedControl();
            }
            items.Clear();
            OnCollectionChanged(new CollectionChangeEventArgs(CollectionChangeAction.Refresh, null));
        }

        /// <summary>
        ///  Gets a value indicating whether the System.Windows.Forms.GridColumnStylesCollection contains a System.Windows.Forms.DataGridColumnStyle associated with the
        ///  specified <see cref='Data.DataColumn'/>.
        /// </summary>
        public bool Contains(PropertyDescriptor propertyDescriptor)
        {
            return this[propertyDescriptor] != null;
        }

        /// <summary>
        ///  Gets a value indicating whether the System.Windows.Forms.GridColumnsStyleCollection contains the specified System.Windows.Forms.DataGridColumnStyle.
        /// </summary>
        public bool Contains(DataGridColumnStyle column)
        {
            int index = items.IndexOf(column);
            return index != -1;
        }

        /// <summary>
        ///  Gets a value indicating whether the System.Windows.Forms.GridColumnsStyleCollection contains the System.Windows.Forms.DataGridColumnStyle with the specified name.
        /// </summary>
        public bool Contains(string name)
        {
            IEnumerator e = items.GetEnumerator();
            while (e.MoveNext())
            {
                DataGridColumnStyle column = (DataGridColumnStyle)e.Current;
                // NOTE: case-insensitive
                if (string.Compare(column.MappingName, name, true, CultureInfo.InvariantCulture) == 0)
                {
                    return true;
                }
            }
            return false;
        }

        /* implemented at BaseCollection
        /// <overload>
        ///  Gets an enumerator for the System.Windows.Forms.GridColumnsStyleCollection.
        /// </overload>
        /// <summary>
        ///  Gets an enumerator for the System.Windows.Forms.GridColumnsStyleCollection.
        /// </summary>
        /// <returns>
        ///  An <see cref='System.Collections.IEnumerator'/>
        ///  that can be used to iterate through the collection.
        /// </returns>
        /// <example>
        ///  The following example gets an <see cref='System.Collections.IEnumerator'/> that iterates through the System.Windows.Forms.GridColumnsStyleCollection. and prints the
        ///  <see cref='System.Windows.Forms.GridColumnsCollection.Caption'/> of each <see cref='System.Data.DataColumn'/>
        ///  associated with the object.
            /// <code lang='VB'>
        ///  Private Sub EnumerateThroughGridColumns()
        ///  Dim ie As System.Collections.IEnumerator
        ///  Dim dgCol As DataGridColumn
        ///  Set ie = DataGrid1.GridColumns.GetEnumerator
        ///  Do While ie.GetNext = True
        ///  Set dgCol = ie.GetObject
        ///  Debug.Print dgCol.DataColumn.Caption
        ///  Loop
        ///  End Sub
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
        ///  Gets an enumerator for the System.Windows.Forms.GridColumnsStyleCollection
        ///  .
        /// </summary>
        /// <param name='allowRemove'>
        ///  A value that indicates if the enumerator can remove elements. <see langword='true'/>, if removals are allowed; otherwise, <see langword='false'/>. The default is <see langword='false'/>.
        /// </param>
        /// <returns>
        ///  An <see cref='System.Collections.IEnumerator'/> that can be used to iterate through the
        ///  collection.
        /// </returns>
        /// <exception cref='NotSupportedException'>
        ///  An attempt was made to remove the System.Windows.Forms.DataGridColumnStyle through the <see cref='System.Collections.Enumerator'/> object's <see cref='System.Windows.Forms.Enumerator.Remove'/> method. Use the System.Windows.Forms.GridColumnsStyleCollection object's <see cref='System.Windows.Forms.GridColumnsCollection.Remove'/> method instead.
        /// </exception>
        /// <remarks>
        ///  Because this implementation doesn't support the removal
        ///  of System.Windows.Forms.DataGridColumnStyle objects through the <see cref='System.Collections.Enumerator'/>
        ///  class's <see cref='System.Windows.Forms.Enumerator.Remove'/> method, you must use the <see cref='System.Windows.Forms.DataGridCollection'/> class's <see cref='System.Windows.Forms.GridColumnsCollection.Remove'/>
        ///  method instead.
        /// </remarks>
        /// <example>
        ///  The following example gets an <see cref='System.Collections.IEnumerator'/> for that iterates through the System.Windows.Forms.GridColumnsStyleCollection. If a column in the collection is of type <see cref='System.Windows.Forms.DataGridBoolColumn'/>, it is deleted.
        ///  <code lang='VB'>
        ///  Private Sub RemoveBoolColumns()
        ///  Dim ie As System.Collections.IEnumerator
        ///  Dim dgCol As DataGridColumn
        ///  Set ie = DataGrid1.GridColumns.GetEnumerator(true)
        ///  Do While ie.GetNext
        ///  Set dgCol = ie.GetObject
        ///
        ///  If dgCol.ToString = "DataGridBoolColumn" Then
        ///  DataGrid1.GridColumns.Remove dgCol
        ///  End If
        ///  Loop
        ///  End If
        ///  </code>
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

        /// <summary>
        ///  Gets the index of a specified System.Windows.Forms.DataGridColumnStyle.
        /// </summary>
        public int IndexOf(DataGridColumnStyle element)
        {
            int itemCount = items.Count;
            for (int i = 0; i < itemCount; ++i)
            {
                DataGridColumnStyle column = (DataGridColumnStyle)items[i];
                if (element == column)
                {
                    return i;
                }
            }
            return -1;
        }

        /// <summary>
        ///  Raises the System.Windows.Forms.GridColumnsCollection.CollectionChanged event.
        /// </summary>
        protected void OnCollectionChanged(CollectionChangeEventArgs e)
        {
            onCollectionChanged?.Invoke(this, e);

            DataGrid grid = owner.DataGrid;
            if (grid != null)
            {
                grid.checkHierarchy = true;
            }
        }

        /// <summary>
        ///  Removes the specified System.Windows.Forms.DataGridColumnStyle from the System.Windows.Forms.GridColumnsStyleCollection.
        /// </summary>
        public void Remove(DataGridColumnStyle column)
        {
            if (isDefault)
            {
                throw new ArgumentException(SR.DataGridDefaultColumnCollectionChanged);
            }

            int columnIndex = -1;
            int itemsCount = items.Count;
            for (int i = 0; i < itemsCount; ++i)
            {
                if (items[i] == column)
                {
                    columnIndex = i;
                    break;
                }
            }

            if (columnIndex == -1)
            {
                throw new InvalidOperationException(SR.DataGridColumnCollectionMissing);
            }
            else
            {
                RemoveAt(columnIndex);
            }
        }

        /// <summary>
        ///  Removes the System.Windows.Forms.DataGridColumnStyle with the specified index from the System.Windows.Forms.GridColumnsStyleCollection.
        /// </summary>
        public void RemoveAt(int index)
        {
            if (isDefault)
            {
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

        public void ResetPropertyDescriptors()
        {
            for (int i = 0; i < Count; i++)
            {
                this[i].PropertyDescriptor = null;
            }
        }
    }
}
