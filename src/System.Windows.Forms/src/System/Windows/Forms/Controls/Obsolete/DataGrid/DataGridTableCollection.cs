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

    protected override ArrayList List => throw new PlatformNotSupportedException();

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public DataGridTableStyle this[int index] => throw new PlatformNotSupportedException();

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public DataGridTableStyle this[string tableName] => throw new PlatformNotSupportedException();

    public virtual int Add(DataGridTableStyle table) => throw new PlatformNotSupportedException();

    public virtual void AddRange(DataGridTableStyle[] tables) => throw new PlatformNotSupportedException();

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public event CollectionChangeEventHandler CollectionChanged
    {
        add => throw new PlatformNotSupportedException();
        remove => throw new PlatformNotSupportedException();
    }

    public void Clear() => throw new PlatformNotSupportedException();

    public bool Contains(DataGridTableStyle table) => throw new PlatformNotSupportedException();

    public bool Contains(string name) => throw new PlatformNotSupportedException();

    protected void OnCollectionChanged(CollectionChangeEventArgs e) => throw new PlatformNotSupportedException();

    public void Remove(DataGridTableStyle table) => throw new PlatformNotSupportedException();

    public void RemoveAt(int index) => throw new PlatformNotSupportedException();
}
