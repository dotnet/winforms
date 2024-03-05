// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Collections;
using System.ComponentModel;

namespace System.Windows.Forms;

#pragma warning disable RS0016
#nullable disable
[Obsolete("GridTableStylesCollection has been deprecated.")]
public class GridTableStylesCollection : BaseCollection, IList
{
    private DataGrid owner;

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
        return default;
    }

    int IList.IndexOf(object value)
    {
        return default;
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
        get { return default; }
        set { throw new NotSupportedException(); }
    }

    void ICollection.CopyTo(Array array, int index)
    {
    }

    int ICollection.Count
    {
        get { return default; }
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
        return default;
    }

    internal GridTableStylesCollection(DataGrid grid)
    {
        owner = grid;
    }

    protected override ArrayList List
    {
        get
        {
            return default;
        }
    }

    [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
    public DataGridTableStyle this[int index]
    {
        get
        {
            throw new PlatformNotSupportedException();
        }
    }

    [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
    public DataGridTableStyle this[string tableName]
    {
        get
        {
            throw new PlatformNotSupportedException();
        }
    }

    internal void CheckForMappingNameDuplicates(DataGridTableStyle table)
    {
        if (string.IsNullOrEmpty(table.MappingName))
            return;
    }

    public virtual int Add(DataGridTableStyle table)
    {
        throw new PlatformNotSupportedException();
    }

    private void TableStyleMappingNameChanged(object sender, EventArgs pcea)
    {
        OnCollectionChanged(new CollectionChangeEventArgs(CollectionChangeAction.Refresh, null));
    }

    public virtual void AddRange(DataGridTableStyle[] tables)
    {
        throw new PlatformNotSupportedException();
    }

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

    public bool Contains(DataGridTableStyle table)
    {
        throw new PlatformNotSupportedException();
    }

    public bool Contains(string name)
    {
        throw new PlatformNotSupportedException();
    }

    protected void OnCollectionChanged(CollectionChangeEventArgs e)
    {
        DataGrid grid = owner;
        if (grid is not null)
        {
            grid.checkHierarchy = true;
        }
    }

    public void Remove(DataGridTableStyle table)
    {
        throw new PlatformNotSupportedException();
    }

    public void RemoveAt(int index)
    {
        throw new PlatformNotSupportedException();
    }
}
