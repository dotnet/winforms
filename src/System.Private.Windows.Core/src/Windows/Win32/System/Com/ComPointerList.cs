// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace Windows.Win32.Foundation;

/// <summary>
///  Simple list for "typed" COM struct pointer storage. Prevents nulls.
/// </summary>
/// <remarks>
///  <para>
///   Doesn't implement generic interfaces as pointer types can't be used as generic arguments.
///  </para>
/// </remarks>
internal unsafe class ComPointerList<T> where T : unmanaged, IComIID
{
    private readonly List<nint> _pointers = [];

    public int Count => _pointers.Count;

    public T* this[int index] => (T*)_pointers[index];

    public void Add(T* item)
    {
        if (item is null)
            throw new ArgumentNullException(nameof(item));

        _pointers.Add((nint)item);
    }

    public bool Remove(T* item)
    {
        return item is null ? throw new ArgumentNullException(nameof(item)) : _pointers.Remove((nint)item);
    }

    public void RemoveAt(int index) => _pointers.RemoveAt(index);

    public void Clear() => _pointers.Clear();

    public int IndexOf(T* item) => _pointers.IndexOf((nint)item);
}
