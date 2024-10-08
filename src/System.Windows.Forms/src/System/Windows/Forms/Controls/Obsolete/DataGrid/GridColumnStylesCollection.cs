// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Collections;
using System.ComponentModel;

namespace System.Windows.Forms;

#nullable disable

[Obsolete(
    Obsoletions.DataGridMessage,
    error: false,
    DiagnosticId = Obsoletions.DataGridDiagnosticId,
    UrlFormat = Obsoletions.SharedUrlFormat)]
[EditorBrowsable(EditorBrowsableState.Never)]
public class GridColumnStylesCollection : BaseCollection, IList
{
    public GridColumnStylesCollection() => throw new PlatformNotSupportedException();

    int IList.Add(object value) => throw new PlatformNotSupportedException();

    void IList.Clear() => throw new PlatformNotSupportedException();

    bool IList.Contains(object value) => throw new PlatformNotSupportedException();

    int IList.IndexOf(object value) => throw new PlatformNotSupportedException();

    void IList.Insert(int index, object value) => throw new PlatformNotSupportedException();

    void IList.Remove(object value) => throw new PlatformNotSupportedException();

    void IList.RemoveAt(int index) => throw new PlatformNotSupportedException();

    bool IList.IsFixedSize => throw new PlatformNotSupportedException();

    bool IList.IsReadOnly => throw new PlatformNotSupportedException();

    object IList.this[int index]
    {
        get => throw new PlatformNotSupportedException();
        set => throw new PlatformNotSupportedException();
    }

    void ICollection.CopyTo(Array array, int index) => throw new PlatformNotSupportedException();

    int ICollection.Count => throw new PlatformNotSupportedException();

    bool ICollection.IsSynchronized => throw new PlatformNotSupportedException();

    object ICollection.SyncRoot => throw new PlatformNotSupportedException();

    IEnumerator IEnumerable.GetEnumerator() => throw new PlatformNotSupportedException();

    public DataGridColumnStyle this[int index] => throw new PlatformNotSupportedException();

    public DataGridColumnStyle this[string columnName] => throw new PlatformNotSupportedException();

    public DataGridColumnStyle this[PropertyDescriptor propertyDesciptor] => throw new PlatformNotSupportedException();

    public virtual int Add(DataGridColumnStyle column) => throw new PlatformNotSupportedException();

    public void AddRange(DataGridColumnStyle[] columns) => throw new PlatformNotSupportedException();

    public event CollectionChangeEventHandler CollectionChanged
    {
        add => throw new PlatformNotSupportedException();
        remove => throw new PlatformNotSupportedException();
    }

    public void Clear() => throw new PlatformNotSupportedException();

    public bool Contains(PropertyDescriptor propertyDescriptor) => throw new PlatformNotSupportedException();

    public bool Contains(DataGridColumnStyle column) => throw new PlatformNotSupportedException();

    public bool Contains(string name) => throw new PlatformNotSupportedException();

    public int IndexOf(DataGridColumnStyle element) => throw new PlatformNotSupportedException();

    protected void OnCollectionChanged(CollectionChangeEventArgs e) => throw new PlatformNotSupportedException();

    public void Remove(DataGridColumnStyle column) => throw new PlatformNotSupportedException();

    public void RemoveAt(int index) => throw new PlatformNotSupportedException();

    public void ResetPropertyDescriptors() => throw new PlatformNotSupportedException();
}
