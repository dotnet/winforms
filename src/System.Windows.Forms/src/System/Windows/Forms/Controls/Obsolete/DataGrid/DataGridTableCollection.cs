// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Collections;
using System.ComponentModel;

namespace System.Windows.Forms;

#nullable disable
#pragma warning disable RS0016 // Add public types and members to the declared API to simplify porting of applications from .NET Framework to .NET.
// These types will not work, but if they are not accessed, other features in the application will work.
[Obsolete(
    Obsoletions.GridTableStylesCollectionMessage,
    error: false,
    DiagnosticId = Obsoletions.GridTableStylesCollectionDiagnosticId,
    UrlFormat = Obsoletions.SharedUrlFormat)]
public class GridTableStylesCollection : BaseCollection, IList
{
    int IList.Add(object value)
        => Add((DataGridTableStyle)value);

    void IList.Clear()
        => Clear();

    bool IList.Contains(object value)
        => default;

    int IList.IndexOf(object value)
        => default;

    void IList.Insert(int index, object value)
        => throw new NotSupportedException();

    void IList.Remove(object value)
        => Remove((DataGridTableStyle)value);

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
        get => default;
        set => throw new NotSupportedException();
    }

    void ICollection.CopyTo(Array array, int index)
    {
    }

    int ICollection.Count
    {
        get => default;
    }

    bool ICollection.IsSynchronized
    {
        get => false;
    }

    object ICollection.SyncRoot
    {
        get => this;
    }

    IEnumerator IEnumerable.GetEnumerator() => default;

    protected override ArrayList List
    {
        get => default;
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public DataGridTableStyle this[int index]
    {
        get => throw new PlatformNotSupportedException();
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public DataGridTableStyle this[string tableName]
    {
        get => throw new PlatformNotSupportedException();
    }

    public virtual int Add(DataGridTableStyle table)
        => throw new PlatformNotSupportedException();

    public virtual void AddRange(DataGridTableStyle[] tables)
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

    public bool Contains(DataGridTableStyle table)
        => throw new PlatformNotSupportedException();

    public bool Contains(string name)
        => throw new PlatformNotSupportedException();

    protected void OnCollectionChanged(CollectionChangeEventArgs e)
        => throw new PlatformNotSupportedException();

    public void Remove(DataGridTableStyle table)
        => throw new PlatformNotSupportedException();

    public void RemoveAt(int index)
        => throw new PlatformNotSupportedException();
}
