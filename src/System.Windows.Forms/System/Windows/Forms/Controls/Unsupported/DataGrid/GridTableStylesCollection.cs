// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Collections;
using System.ComponentModel;

namespace System.Windows.Forms;

#nullable disable

/// <summary>
///  This type is provided for binary compatibility with .NET Framework and is not intended to be used directly from your code.
/// </summary>
[Obsolete(
    Obsoletions.DataGridMessage,
    error: false,
    DiagnosticId = Obsoletions.UnsupportedControlsDiagnosticId,
    UrlFormat = Obsoletions.SharedUrlFormat)]
[EditorBrowsable(EditorBrowsableState.Never)]
[Browsable(false)]
[ListBindable(false)]
public class GridTableStylesCollection : BaseCollection, IList
{
    private GridTableStylesCollection() { }

    int IList.Add(object value) => throw null;

    void IList.Clear() { }

    bool IList.Contains(object value) => throw null;

    int IList.IndexOf(object value) => throw null;

    void IList.Insert(int index, object value) { }

    void IList.Remove(object value) { }

    void IList.RemoveAt(int index) { }

    bool IList.IsFixedSize => throw null;

    bool IList.IsReadOnly => throw null;

    object IList.this[int index]
    {
        get => throw null;
        set { }
    }

    void ICollection.CopyTo(Array array, int index) { }

    int ICollection.Count => throw null;

    bool ICollection.IsSynchronized => throw null;

    object ICollection.SyncRoot => throw null;

    IEnumerator IEnumerable.GetEnumerator() => throw null;

    public DataGridTableStyle this[int index] => throw null;

    public DataGridTableStyle this[string tableName] => throw null;

    public virtual int Add(DataGridTableStyle table) => throw null;

    public virtual void AddRange(DataGridTableStyle[] tables) { }

    public event CollectionChangeEventHandler CollectionChanged
    {
        add { }
        remove { }
    }

    public void Clear() { }

    public bool Contains(DataGridTableStyle table) => throw null;

    public bool Contains(string name) => throw null;

    protected void OnCollectionChanged(CollectionChangeEventArgs e) { }

    public void Remove(DataGridTableStyle table) { }

    public void RemoveAt(int index) { }
}
