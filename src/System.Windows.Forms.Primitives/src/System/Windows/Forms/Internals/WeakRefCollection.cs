// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections;
using System.Diagnostics;

namespace System.Windows.Forms
{
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
    internal class WeakRefCollection : IList
    {
        public WeakRefCollection() : this(4)
        {
        }

        public WeakRefCollection(int size)
        {
            InnerList = new ArrayList(size);
        }

        public ArrayList InnerList { get; }

        /// <summary>
        ///  Indicates the value where the collection should check its items to remove dead weakref left over.
        ///  Note: When GC collects weak refs from this collection the WeakRefObject identity changes since its
        ///  Target becomes null. This makes the item unrecognizable by the collection and cannot be
        ///  removed - Remove(item) and Contains(item) will not find it anymore.
        /// </summary>
        public int RefCheckThreshold { get; set; } = int.MaxValue; // this means this is disabled by default.

        public object? this[int index]
        {
            get
            {
                if ((InnerList[index] is WeakRefObject weakRef) && (weakRef.IsAlive))
                {
                    return weakRef.Target;
                }

                return null;
            }
            set => InnerList[index] = CreateWeakRefObject(value);
        }

        public void ScavengeReferences()
        {
            int currentIndex = 0;
            int currentCount = Count;
            for (int i = 0; i < currentCount; i++)
            {
                object? item = this[currentIndex];

                if (item is null)
                {
                    InnerList.RemoveAt(currentIndex);
                }
                else
                {
                    // Only incriment if we have not removed the item
                    currentIndex++;
                }
            }
        }

        public override bool Equals(object? obj)
        {
            WeakRefCollection? other = obj as WeakRefCollection;
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
                WeakRefObject? item = (WeakRefObject?)InnerList[i];
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
            var hash = new HashCode();
            foreach (WeakRefObject? o in InnerList)
            {
                hash.Add(o);
            }

            return hash.ToHashCode();
        }

        private WeakRefObject? CreateWeakRefObject(object? value)
        {
            if (value is null)
            {
                return null;
            }

            return new WeakRefObject(value);
        }

        public void Clear() => InnerList.Clear();

        public bool IsFixedSize => InnerList.IsFixedSize;

        public bool Contains(object? value) => InnerList.Contains(CreateWeakRefObject(value));

        public void RemoveAt(int index) => InnerList.RemoveAt(index);

        public void Remove(object? value) => InnerList.Remove(CreateWeakRefObject(value));

        public int IndexOf(object? value) => InnerList.IndexOf(CreateWeakRefObject(value));

        public void Insert(int index, object? value) => InnerList.Insert(index, CreateWeakRefObject(value));

        public int Add(object? value)
        {
            if (Count > RefCheckThreshold)
            {
                ScavengeReferences();
            }

            return InnerList.Add(CreateWeakRefObject(value));
        }

        public int Count => InnerList.Count;

        object ICollection.SyncRoot => InnerList.SyncRoot;

        public bool IsReadOnly => InnerList.IsReadOnly;

        public void CopyTo(Array array, int index) => InnerList.CopyTo(array, index);

        bool ICollection.IsSynchronized => InnerList.IsSynchronized;

        public IEnumerator GetEnumerator() => InnerList.GetEnumerator();

        /// <summary>
        ///  Wraps a weak ref object. WARNING: Use this class carefully!
        ///  When the weak ref is collected, this object looses its identity. This is bad when the object
        ///  has been added to a collection since Contains(WeakRef(item)) and Remove(WeakRef(item)) would
        ///  not be able to identify the item.
        /// </summary>
        internal class WeakRefObject
        {
            private readonly int _hash;
            private readonly WeakReference weakHolder;

            internal WeakRefObject(object obj)
            {
                Debug.Assert(obj != null, "Unexpected null object!");
                weakHolder = new WeakReference(obj);
                _hash = obj.GetHashCode();
            }

            internal bool IsAlive => weakHolder.IsAlive;

            internal object? Target => weakHolder.Target;

            public override int GetHashCode() => _hash;

            public override bool Equals(object? obj)
            {
                WeakRefObject? other = obj as WeakRefObject;

                if (other == this)
                {
                    return true;
                }

                if (other is null)
                {
                    return false;
                }

                if (other.Target != Target)
                {
                    if (Target is null || !Target.Equals(other.Target))
                    {
                        return false;
                    }
                }

                return true;
            }
        }
    }
}
