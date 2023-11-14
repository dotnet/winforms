// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Collections.Generic;

/// <summary>
///  A collection that holds onto weak references.
///
///  Essentially you pass in the object as it is, and under the covers
///  we only hold a weak reference to the object.
///
///  -----------------------------------------------------------------
///  !!!IMPORTANT USAGE NOTE!!!
///  Users of this class should set the RefCheckThreshold property
///  explicitly or call ScavengeReferences every once in a while to
///  remove dead references.
///  Also avoid calling Remove(item).  Instead call RemoveByHashCode(item)
///  to make sure dead refs are removed.
///  -----------------------------------------------------------------
/// </summary>
internal sealed class WeakRefCollection<T> : IList<T?> where T : class
{
    public WeakRefCollection() : this(4)
    {
    }

    public WeakRefCollection(int size)
    {
        InnerList = new List<WeakRefObject?>(size);
    }

    public List<WeakRefObject?> InnerList { get; }

    /// <summary>
    ///  Indicates the value where the collection should check its items to remove dead weakref left over.
    ///  Note: When GC collects weak refs from this collection the WeakRefObject identity changes since its
    ///  Target becomes null. This makes the item unrecognizable by the collection and cannot be
    ///  removed - Remove(item) and Contains(item) will not find it anymore.
    /// </summary>
    public int RefCheckThreshold { get; set; } = int.MaxValue; // this means this is disabled by default.

    public T? this[int index]
    {
        get
        {
            if (InnerList[index] is not WeakRefObject weakRef)
            {
                return null;
            }

            weakRef.TryGetTarget(out T? target);
            return target;
        }
        set => InnerList[index] = CreateWeakRefObject(value);
    }

    public void ScavengeReferences()
    {
        int currentIndex = 0;
        int currentCount = Count;
        for (int i = 0; i < currentCount; i++)
        {
            WeakRefObject? item = InnerList[currentIndex];

            if (item is null)
            {
                InnerList.RemoveAt(currentIndex);
            }
            else
            {
                // Only increment if we have not removed the item
                currentIndex++;
            }
        }
    }

    public override bool Equals(object? obj)
    {
        WeakRefCollection<T>? other = obj as WeakRefCollection<T>;
        if (other == this)
        {
            return true;
        }

        if (other is null || Count != other.Count)
        {
            return false;
        }

        for (int i = 0; i < Count; i++)
        {
            WeakRefObject? item = InnerList[i];
            if (item != other.InnerList[i])
            {
                if (item is null || !item.Equals(other.InnerList[i]))
                {
                    return false;
                }
            }
        }

        return true;
    }

    public override int GetHashCode()
    {
        HashCode hash = default;
        foreach (WeakRefObject? o in InnerList)
        {
            hash.Add(o);
        }

        return hash.ToHashCode();
    }

    /// <summary>
    /// Removes the value using its hash code as its identity.
    /// This is needed because the underlying item in the collection may have already been collected changing
    /// the identity of the WeakRefObject making it impossible for the collection to identify it.
    /// See <see cref="WeakRefObject"/> for more info.
    /// </summary>
    public void RemoveByHashCode(T? value)
    {
        if (value is null)
        {
            return;
        }

        int hash = value.GetHashCode();

        for (int idx = 0; idx < InnerList.Count; idx++)
        {
            if (InnerList[idx] is not null && InnerList[idx]!.GetHashCode() == hash)
            {
                RemoveAt(idx);
                return;
            }
        }
    }

    private static WeakRefObject? CreateWeakRefObject(T? value)
        => value is null ? (WeakRefCollection<T>.WeakRefObject?)null : new WeakRefObject(value);

    public void Clear() => InnerList.Clear();

    public bool Contains(T? value) => InnerList.Contains(CreateWeakRefObject(value));

    public void RemoveAt(int index) => InnerList.RemoveAt(index);

    public bool Remove(T? value) => InnerList.Remove(CreateWeakRefObject(value));

    public int IndexOf(T? value) => InnerList.IndexOf(CreateWeakRefObject(value));

    public void Insert(int index, T? value) => InnerList.Insert(index, CreateWeakRefObject(value));

    public void Add(T? value)
    {
        if (Count > RefCheckThreshold)
        {
            ScavengeReferences();
        }

        var weakRefObject = CreateWeakRefObject(value);
        InnerList.Add(weakRefObject);
    }

    public int Count => InnerList.Count;

    public bool IsReadOnly => false;

    public void CopyTo(T?[] array, int index) => throw new NotImplementedException();
    public void CopyTo(Array array, int index) => InnerList.CopyTo((WeakRefObject?[])array, index);

    public IEnumerator<T> GetEnumerator()
    {
        foreach (WeakRefObject? weakRef in InnerList)
        {
            if (weakRef is not null)
            {
                // Safely try to get the target. If it's still alive, yield return it.
                if (weakRef.TryGetTarget(out T? target) && target is not null)
                {
                    yield return target;
                }
            }
        }
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    /// <summary>
    ///  Wraps a weak ref object. WARNING: Use this class carefully!
    ///  When the weak ref is collected, this object looses its identity. This is bad when the object
    ///  has been added to a collection since Contains(WeakRef(item)) and Remove(WeakRef(item)) would
    ///  not be able to identify the item.
    /// </summary>
    internal sealed class WeakRefObject
    {
        private readonly int _hash;
        private readonly WeakReference<T> _weakHolder;

        internal WeakRefObject(T obj)
        {
            Debug.Assert(obj is not null, "Unexpected null object!");
            _weakHolder = new(obj);
            _hash = obj.GetHashCode();
        }

        internal bool TryGetTarget(out T? target) => _weakHolder.TryGetTarget(out target);

        public override int GetHashCode() => _hash;

        public override bool Equals(object? obj)
        {
            // Cast obj to WeakRefObject; if it fails, return false.
            if (obj is not WeakRefObject other)
            {
                return false;
            }

            // Check for reference equality.
            if (ReferenceEquals(this, other))
            {
                return true;
            }

            if (other is null)
            {
                return false;
            }

            // Try to get the targets of both weak references.
            bool thisAlive = _weakHolder.TryGetTarget(out T? thisTarget);
            bool otherAlive = other._weakHolder.TryGetTarget(out T? otherTarget);

            // If both are alive, compare the targets.
            if (thisAlive && otherAlive)
            {
                return Equals(thisTarget, otherTarget);
            }

            // If both are not alive, they are considered equal.
            // (Both refer to collected objects.)
            return !thisAlive && !otherAlive;
        }
    }
}
