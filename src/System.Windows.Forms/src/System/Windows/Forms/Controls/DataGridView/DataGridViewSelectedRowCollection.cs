// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Collections;
using System.ComponentModel;

namespace System.Windows.Forms;

/// <summary>
///  Represents a collection of selected <see cref="DataGridViewCell"/> objects in the <see cref="DataGridView"/>
///  control.
/// </summary>
[ListBindable(false)]
public class DataGridViewSelectedRowCollection : BaseCollection, IList
{
    private readonly List<DataGridViewRow> _items = [];

    int IList.Add(object? value) => throw new NotSupportedException(SR.DataGridView_ReadOnlyCollection);

    void IList.Clear() => throw new NotSupportedException(SR.DataGridView_ReadOnlyCollection);

    bool IList.Contains(object? value) => ((IList)_items).Contains(value);

    int IList.IndexOf(object? value) => ((IList)_items).IndexOf(value);

    void IList.Insert(int index, object? value) => throw new NotSupportedException(SR.DataGridView_ReadOnlyCollection);

    void IList.Remove(object? value) => throw new NotSupportedException(SR.DataGridView_ReadOnlyCollection);

    void IList.RemoveAt(int index) => throw new NotSupportedException(SR.DataGridView_ReadOnlyCollection);

    bool IList.IsFixedSize => true;

    bool IList.IsReadOnly => true;

    object? IList.this[int index]
    {
        get { return _items[index]; }
        set { throw new NotSupportedException(SR.DataGridView_ReadOnlyCollection); }
    }

    void ICollection.CopyTo(Array array, int index) => ((ICollection)_items).CopyTo(array, index);

    int ICollection.Count => _items.Count;

    bool ICollection.IsSynchronized => false;

    object ICollection.SyncRoot => this;

    IEnumerator IEnumerable.GetEnumerator() => _items.GetEnumerator();

    internal DataGridViewSelectedRowCollection()
    {
    }

    protected override ArrayList List => ArrayList.Adapter(_items);

    public DataGridViewRow this[int index] => _items[index];

    /// <summary>
    ///  Adds a <see cref="DataGridViewCell"/> to this collection.
    /// </summary>
    internal int Add(DataGridViewRow dataGridViewRow) => ((IList)_items).Add(dataGridViewRow);

    [EditorBrowsable(EditorBrowsableState.Never)]
    public void Clear() => throw new NotSupportedException(SR.DataGridView_ReadOnlyCollection);

    /// <summary>
    ///  Checks to see if a DataGridViewCell is contained in this collection.
    /// </summary>
    public bool Contains(DataGridViewRow dataGridViewRow) => ((IList)_items).Contains(dataGridViewRow);

    public void CopyTo(DataGridViewRow[] array, int index) => _items.CopyTo(array, index);

    [EditorBrowsable(EditorBrowsableState.Never)]
    public void Insert(int index, DataGridViewRow dataGridViewRow) => throw new NotSupportedException(SR.DataGridView_ReadOnlyCollection);
}
