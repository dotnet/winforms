// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Collections;
using System.ComponentModel;

namespace System.Windows.Forms;

#nullable disable
#pragma warning disable RS0016 // Add public types and members to the declared API to simplify porting of applications from .NET Framework to .NET.
// These types will not work, but if they are not accessed, other features in the application will work.
[Obsolete(
    Obsoletions.GridColumnStylesCollectionMessage,
    error: false,
    DiagnosticId = Obsoletions.GridColumnStylesCollectionDiagnosticId,
    UrlFormat = Obsoletions.SharedUrlFormat)]
public class GridColumnStylesCollection : BaseCollection, IList
{
    int IList.Add(object value)
        => Add((DataGridColumnStyle)value);

    void IList.Clear()
        => Clear();

    bool IList.Contains(object value)
        => throw new PlatformNotSupportedException();

    int IList.IndexOf(object value)
        => throw new PlatformNotSupportedException();

    void IList.Insert(int index, object value)
        => throw new PlatformNotSupportedException();

    void IList.Remove(object value)
        => Remove((DataGridColumnStyle)value);

    void IList.RemoveAt(int index)
        => RemoveAt(index);

    bool IList.IsFixedSize
    {
        get => false;
    }

    bool IList.IsReadOnly
    {
        get => false;
    }

    object IList.this[int index]
    {
        get => throw new NotSupportedException();
        set => throw new NotSupportedException();
    }

    void ICollection.CopyTo(Array array, int index)
        => throw new PlatformNotSupportedException();

    int ICollection.Count
    {
        get => throw new PlatformNotSupportedException();
    }

    bool ICollection.IsSynchronized
    {
        get => false;
    }

    object ICollection.SyncRoot
    {
        get => this;
    }

    IEnumerator IEnumerable.GetEnumerator()
        => throw new PlatformNotSupportedException();

    protected override ArrayList List
    {
        get => throw new PlatformNotSupportedException();
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public DataGridColumnStyle this[int index]
    {
        get => throw new PlatformNotSupportedException();
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public DataGridColumnStyle this[string columnName]
    {
        get => throw new PlatformNotSupportedException();
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public DataGridColumnStyle this[PropertyDescriptor propertyDesciptor]
    {
        get => throw new PlatformNotSupportedException();
    }

    public virtual int Add(DataGridColumnStyle column)
        => throw new PlatformNotSupportedException();

    public void AddRange(DataGridColumnStyle[] columns)
        => throw new PlatformNotSupportedException();

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public event CollectionChangeEventHandler CollectionChanged
    {
        add => throw new PlatformNotSupportedException();
        remove => throw new PlatformNotSupportedException();
    }

    public void Clear()
        => throw new PlatformNotSupportedException();

    public bool Contains(PropertyDescriptor propertyDescriptor)
        => throw new PlatformNotSupportedException();

    public bool Contains(DataGridColumnStyle column)
        => throw new PlatformNotSupportedException();

    public bool Contains(string name)
        => throw new PlatformNotSupportedException();

    public int IndexOf(DataGridColumnStyle element)
        => throw new PlatformNotSupportedException();

    protected void OnCollectionChanged(CollectionChangeEventArgs e)
        => throw new PlatformNotSupportedException();

    public void Remove(DataGridColumnStyle column)
        => throw new PlatformNotSupportedException();

    public void RemoveAt(int index)
        => throw new PlatformNotSupportedException();

    public void ResetPropertyDescriptors()
        => throw new PlatformNotSupportedException();
}
