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
internal class ArrayBuilder<T>
{
    private const int MaxInitialArrayLength = 1024 * 10;

    private T[] _items;
    private readonly int _maxCount;
    private int _count;

    /// <param name="expectedCount">
    ///  The <see cref="_count"/> cannot grow past this value and is expected to be this value
    ///  when the collection is "finished".
    /// </param>
    internal ArrayBuilder(int expectedCount)
    {
        _items = new T[Math.Min(expectedCount, MaxInitialArrayLength)];
        _maxCount = expectedCount;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Add(T item)
    {
        T[] array = _items;
        int count = _count;
        if ((uint)count < (uint)_items.Length)
        {
            _count = count + 1;
            array[count] = item;
        }
        else
        {
            AddWithResize(item);
        }
    }

    private void AddWithResize(T item)
    {
        if (_items.Length == _maxCount)
        {
            throw new ArgumentOutOfRangeException(nameof(item), "Collection is at max capacity.");
        }

        Debug.Assert(_count == _items.Length);
        int size = _count;

        Debug.Assert(_items.Length > 0);
        int newCapacity = Math.Min(_maxCount, 2 * _items.Length);

        Array.Resize(ref _items, newCapacity);
        _count = size + 1;
        _items[size] = item;
    }

    public T[] ToArray()
    {
        Debug.Assert(_count == _maxCount);
        Debug.Assert(_count == _items.Length);

        return _items;
    }
}
