// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Runtime.CompilerServices;

namespace System.Collections.Generic;

/// <summary>
///  Array based collection that tries to avoid copying the internal array and caps the maximum capacity.
/// </summary>
/// <remarks>
///  <para>
///   To mitigate corrupted length attacks, the backing array has an initial allocation size cap.
///  </para>
/// </remarks>
internal class CappedArray<T> : IReadOnlyList<T>, ICollection<T>
{
    private const int DefaultCapacity = 4;

    private const int MaxInitialArrayLength = 1024 * 10;

    private T[] _items;
    private readonly int _maxCount;

    /// <param name="expectedCount">
    ///  The <see cref="Count"/> cannot grow past this value and is expected to be this value
    ///  when the collection is "finished".
    /// </param>
    internal CappedArray(int expectedCount)
    {
        _items = new T[Math.Min(expectedCount, MaxInitialArrayLength)];
        _maxCount = expectedCount;
    }

    public T this[int index]
    {
        get
        {
            ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(index, Count);
            return _items[index];
        }
        set
        {
            ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(index, Count);
            _items[index] = value;
        }
    }

    public int Count { get; private set; }

    public bool IsReadOnly => throw new NotImplementedException();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Add(T item)
    {
        T[] array = _items;
        int count = Count;
        if ((uint)count < (uint)_items.Length)
        {
            Count = count + 1;
            array[count] = item;
        }
        else
        {
            AddWithResize(item);
        }
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private void AddWithResize(T item)
    {
        if (_items.Length == _maxCount)
        {
            throw new ArgumentOutOfRangeException(nameof(item), "Collection is at max capacity.");
        }

        Debug.Assert(Count == _items.Length);
        int size = Count;

        int newCapacity = _items.Length == 0 ? DefaultCapacity : 2 * _items.Length;

        if ((uint)newCapacity > _maxCount)
        {
            newCapacity = _maxCount;
        }

        Array.Resize(ref _items, newCapacity);
        Count = size + 1;
        _items[size] = item;
    }

    public IEnumerator<T> GetEnumerator()
    {
        Debug.Assert(Count == _maxCount);
        return new ArraySegment<T>(_items, 0, Count).GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public T[] ToArray()
    {
        Debug.Assert(Count == _maxCount);

        if (Count == _items.Length)
        {
            return _items;
        }

        ArraySegment<T> segment = new(_items, 0, Count);
        return [.. segment];
    }

    public void Clear() => Count = 0;
    public bool Contains(T item) => Array.IndexOf(_items, item, 0, Count) != -1;
    public void CopyTo(T[] array, int arrayIndex) => Array.Copy(_items, 0, array, arrayIndex, Count);
    bool ICollection<T>.Remove(T item) => throw new NotSupportedException();

#pragma warning disable IDE0305 // Simplify collection initialization
    // We deliberately don't want to use a collection expression here because it would allocate a new array.
    public static implicit operator T[](CappedArray<T> list) => list.ToArray();
#pragma warning restore IDE0305

    public static explicit operator ArraySegment<T>(CappedArray<T> list) => new(list._items, 0, list.Count);
}
