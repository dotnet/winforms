// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Collections;
using System.ComponentModel;
using System.Drawing.Design;

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
[Editor($"System.Windows.Forms.Design.DataGridColumnCollectionEditor, {Assemblies.SystemDesign}", typeof(UITypeEditor))]
[ListBindable(false)]
public class GridColumnStylesCollection : BaseCollection, IList
{
    private GridColumnStylesCollection() { }

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

    public DataGridColumnStyle this[int index] => throw null;

    public DataGridColumnStyle this[string columnName] => throw null;

    public DataGridColumnStyle this[PropertyDescriptor propertyDesciptor] => throw null;

    public virtual int Add(DataGridColumnStyle column) => throw null;

    public void AddRange(DataGridColumnStyle[] columns) { }

    public event CollectionChangeEventHandler CollectionChanged
    {
        add { }
        remove { }
    }

    public void Clear() { }

    public bool Contains(PropertyDescriptor propertyDescriptor) => throw null;

    public bool Contains(DataGridColumnStyle column) => throw null;

    public bool Contains(string name) => throw null;

    public int IndexOf(DataGridColumnStyle element) => throw null;

    protected void OnCollectionChanged(CollectionChangeEventArgs e) { }

    public void Remove(DataGridColumnStyle column) { }

    public void RemoveAt(int index) { }

    public void ResetPropertyDescriptors() { }
}
