// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Collections;
using System.ComponentModel;

namespace System.Windows.Forms;

#pragma warning disable RS0016
#nullable disable
[Obsolete("GridColumnStylesCollection has been deprecated.")]
public class GridColumnStylesCollection : BaseCollection, IList
{
    private ArrayList items = new ArrayList();
    private DataGridTableStyle owner;
    private bool isDefault;

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

    protected override ArrayList List
    {
        get
        {
            return items;
        }
    }

    public DataGridColumnStyle this[int index]
    {
        get
        {
            throw new PlatformNotSupportedException();
        }
    }

    public DataGridColumnStyle this[string columnName]
    {
        get
        {
            throw new PlatformNotSupportedException();
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
                return column;
        }

        return null;
    }

    public DataGridColumnStyle this[PropertyDescriptor propertyDesciptor]
    {
        get
        {
            throw new PlatformNotSupportedException();
        }
    }

    internal DataGridTableStyle DataGridTableStyle
    {
        get
        {
            return owner;
        }
    }

    internal void CheckForMappingNameDuplicates(DataGridColumnStyle column)
    {
        if (string.IsNullOrEmpty(column.MappingName))
            return;
        for (int i = 0; i < items.Count; i++)
            if (((DataGridColumnStyle)items[i]).MappingName.Equals(column.MappingName) && column != items[i])
                throw new ArgumentException("Data grid column styles collection already contains a column style with the same mapping name.");
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
        throw new PlatformNotSupportedException();
    }

    public void AddRange(DataGridColumnStyle[] columns)
    {
        throw new PlatformNotSupportedException();
    }

    internal void AddDefaultColumn(DataGridColumnStyle column)
    {
        column.SetDataGridTableInColumn(owner, true);
        items.Add(column);
    }

    internal void ResetDefaultColumnCollection()
    {
        for (int i = 0; i < Count; i++)
        {
            this[i].ReleaseHostedControl();
        }

        items.Clear();
    }

    [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
    public event CollectionChangeEventHandler CollectionChanged
    {
        add
        {
            throw new PlatformNotSupportedException();
        }
        remove
        {
            throw new PlatformNotSupportedException();
        }
    }

    public void Clear()
    {
        throw new PlatformNotSupportedException();
    }

    public bool Contains(PropertyDescriptor propertyDescriptor)
    {
        throw new PlatformNotSupportedException();
    }

    public bool Contains(DataGridColumnStyle column)
    {
        throw new PlatformNotSupportedException();
    }

    public bool Contains(string name)
    {
        throw new PlatformNotSupportedException();
    }

    public int IndexOf(DataGridColumnStyle element)
    {
        throw new PlatformNotSupportedException();
    }

    protected void OnCollectionChanged(CollectionChangeEventArgs e)
    {
        DataGrid grid = owner.DataGrid;
        if (grid is not null)
        {
            grid.checkHierarchy = true;
        }
    }

    public void Remove(DataGridColumnStyle column)
    {
        throw new PlatformNotSupportedException();
    }

    public void RemoveAt(int index)
    {
        throw new PlatformNotSupportedException();
    }

    public void ResetPropertyDescriptors()
    {
        throw new PlatformNotSupportedException();
    }
}
