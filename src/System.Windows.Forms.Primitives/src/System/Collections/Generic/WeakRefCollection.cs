// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Collections.Generic;

/// <summary>
///  Weak reference collection of <typeparamref name="T"/>.
/// </summary>
internal sealed class WeakRefCollection<T>() : IEnumerable<T> where T : class
{
    private readonly List<WeakReference<T>> _list = [];

    /// <summary>
    ///  If set to true, triggers the execution of ScavengeReferences during the next
    ///  <see cref="Add(T)"/> call to clean up dead weak references.
    /// </summary>
    private bool _scavenge;

    public T? this[int index]
    {
        get
        {
            _scavenge = !_list[index].TryGetTarget(out T? target);
            return target;
        }
        set
        {
            ArgumentNullException.ThrowIfNull(value, nameof(value));
            _list[index] = new(value);
        }
    }

    /// <summary>
    ///  Cleans up dead weak references.
    /// </summary>
    public void ScavengeReferences()
    {
        for (int i = Count - 1; i >= 0; i--)
        {
            if (!_list[i].TryGetTarget(out T? _))
            {
                _list.RemoveAt(i);
            }
        }

        _scavenge = false;
    }

    public void RemoveAt(int index) => _list.RemoveAt(index);

    public void Remove(T value)
    {
        int index = IndexOf(value);
        if (index != -1)
        {
            _list.RemoveAt(index);
        }
    }

    public int IndexOf(T value)
    {
        for (int i = 0; i < _list.Count; i++)
        {
            if (!_list[i].TryGetTarget(out T? target))
            {
                _scavenge = true;
            }
            else if (target == value)
            {
                return i;
            }
        }

        return -1;
    }

    public bool Contains(T value) => IndexOf(value) != -1;

    public void Add(T value)
    {
        if (_scavenge)
        {
            ScavengeReferences();
        }

        _list.Add(new(value));
    }

    public int Count => _list.Count;

    public IEnumerator<T> GetEnumerator()
    {
        foreach (WeakReference<T> weakRef in _list)
        {
            // Safely try to get the target. If it's still alive, yield return it.
            if (weakRef.TryGetTarget(out T? target))
            {
                yield return target;
            }
            else
            {
                _scavenge = true;
            }
        }
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
