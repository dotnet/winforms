// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Collections;

namespace System.Windows.Forms;

/// <summary>
///  Collection that protects against inserting null objects.
/// </summary>
[DebuggerDisplay("{DebuggerDisplay,nq}")]
internal abstract class NonNullCollection<T>
    : IList<T>, ICollection<T>, IEnumerable<T>, IEnumerable, IList, ICollection, IReadOnlyList<T>, IReadOnlyCollection<T>
    where T : class
{
    private readonly List<T> _list = [];

    public NonNullCollection() { }

    public NonNullCollection(IEnumerable<T> items) => AddRange(items);

    public T this[int index]
    {
        get => _list[index];
        set
        {
            _list[index] = value ?? ThrowArgumentNull(nameof(value));
            ItemAdded(value);
        }
    }

    public int Count => _list.Count;

    public bool IsReadOnly => ((ICollection<T>)_list).IsReadOnly;

    public void Add(T item)
    {
        _list.Add(item ?? ThrowArgumentNull(nameof(item)));
        ItemAdded(item);
    }

    public void Clear() => _list.Clear();

    public bool Contains(T item) => _list.Contains(item);

    public void CopyTo(T[] array, int arrayIndex) => _list.CopyTo(array, arrayIndex);

    public IEnumerator<T> GetEnumerator() => _list.GetEnumerator();

    public int IndexOf(T item) => _list.IndexOf(item);

    public void Insert(int index, T item)
    {
        _list.Insert(index, item ?? ThrowArgumentNull(nameof(item)));
        ItemAdded(item);
    }

    public bool Remove(T item) => _list.Remove(item);

    public void RemoveAt(int index) => _list.RemoveAt(index);

    IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)_list).GetEnumerator();

    /// <summary>
    ///  Adds the items in <paramref name="items"/> to the collection.
    /// </summary>
    public void AddRange(IEnumerable<T> items)
    {
        // We don't want to put in partial items, so we need to enumerate twice to throw appropriately.
#pragma warning disable CA1851 // Possible multiple enumerations of 'IEnumerable' collection
        if (items is null || items.Contains(default))
        {
            ThrowArgumentNull(nameof(items));
        }

        _list.AddRange(items);

        foreach (T item in items)
        {
            ItemAdded(item);
        }
#pragma warning restore CA1851
    }

    int IList.Add(object? value)
    {
        int index = ((IList)_list).Add(value ?? ThrowArgumentNull(nameof(value)));
        ItemAdded((T)value);
        return index;
    }

    bool IList.Contains(object? value) => ((IList)_list).Contains(value);

    int IList.IndexOf(object? value) => ((IList)_list).IndexOf(value);

    void IList.Insert(int index, object? value)
    {
        ((IList)_list).Insert(index, value ?? ThrowArgumentNull(nameof(value)));
        ItemAdded((T)value);
    }

    void IList.Remove(object? value) => ((IList)_list).Remove(value);

    object? IList.this[int index]
    {
        get => ((IList)_list)[index];
        set
        {
            ((IList)_list)[index] = value ?? ThrowArgumentNull(nameof(value));
            ItemAdded((T)value);
        }
    }

    void ICollection.CopyTo(Array array, int index) => ((IList)_list).CopyTo(array, index);

    private string DebuggerDisplay => $"Count: {Count}";

    bool IList.IsFixedSize => ((IList)_list).IsFixedSize;

    object ICollection.SyncRoot => ((ICollection)_list).SyncRoot;

    bool ICollection.IsSynchronized => ((ICollection)_list).IsSynchronized;

    /// <summary>
    ///  Called for each item added to the collection.
    /// </summary>
    protected virtual void ItemAdded(T item)
    {
    }

    [DoesNotReturn]
    private static T ThrowArgumentNull(string name) => throw new ArgumentNullException(name);
}
