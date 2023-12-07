﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Collections;

namespace System.Windows.Forms.Design;
internal sealed class ListAdapter<T> : IList<T>, IWrapper<IList>
{
    private readonly IList _list;
    internal ListAdapter(IList list) => _list = list.OrThrowIfNull();

    T IList<T>.this[int index]
    {
        get => (T?)_list[index] ?? throw new InvalidOperationException();
        set => _list[index] = value.OrThrowIfNull();
    }

    int ICollection<T>.Count => _list.Count;

    bool ICollection<T>.IsReadOnly => _list.IsReadOnly;

    void ICollection<T>.Add(T item) => _list.Add(item.OrThrowIfNull());
    void ICollection<T>.Clear() => _list.Clear();
    bool ICollection<T>.Contains(T item) => _list.Contains(item);
    void ICollection<T>.CopyTo(T[] array, int arrayIndex) => _list.CopyTo(array, arrayIndex);
    IEnumerator IEnumerable.GetEnumerator() => _list.GetEnumerator();
    int IList<T>.IndexOf(T item) => _list.IndexOf(item);
    void IList<T>.Insert(int index, T item) => _list.Insert(index, item.OrThrowIfNull());
    void IList<T>.RemoveAt(int index) => _list.RemoveAt(index);

    bool ICollection<T>.Remove(T item)
    {
        if (_list.IsReadOnly || !_list.Contains(item))
        {
            return false;
        }

        _list.Remove(item);
        return true;
    }

    IEnumerator<T> IEnumerable<T>.GetEnumerator() => new Enumerator(_list.GetEnumerator());

    private sealed class Enumerator(IEnumerator _enumerator) : IEnumerator<T>
    {
        T IEnumerator<T>.Current => (T)_enumerator.Current;
        object IEnumerator.Current => _enumerator.Current;

        void IDisposable.Dispose() { }
        bool IEnumerator.MoveNext() => _enumerator.MoveNext();
        void IEnumerator.Reset() => _enumerator.Reset();
    }

    public IList Unwrap() => _list;
}

internal interface IWrapper<T>
{
    T Unwrap();
}

internal static class AdapterHelpers
{
    internal static IList Unwrap<T>(this IList<T> list) => list is IWrapper<IList> wrapper ? wrapper.Unwrap() : (IList)list;
    internal static IList<T> Adapt<T>(this IList list) => list is IList<T> iList ? iList : new ListAdapter<T>(list);
}
