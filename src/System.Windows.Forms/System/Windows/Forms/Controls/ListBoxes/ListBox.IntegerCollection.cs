// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Collections;
using System.ComponentModel;

namespace System.Windows.Forms;

public partial class ListBox
{
    public partial class IntegerCollection : IList
    {
        private readonly ListBox _owner;
        private int[]? _innerArray;
        private int _count;

        public IntegerCollection(ListBox owner)
        {
            _owner = owner.OrThrowIfNull();
        }

        /// <summary>
        ///  Number of current selected items.
        /// </summary>
        [Browsable(false)]
        public int Count
        {
            get
            {
                return _count;
            }
        }

        object ICollection.SyncRoot
        {
            get
            {
                return this;
            }
        }

        bool ICollection.IsSynchronized
        {
            get
            {
                return true;
            }
        }

        bool IList.IsFixedSize
        {
            get
            {
                return false;
            }
        }

        bool IList.IsReadOnly
        {
            get
            {
                return false;
            }
        }

        public bool Contains(int item)
        {
            return IndexOf(item) != -1;
        }

        bool IList.Contains(object? item)
        {
            if (item is int itemAsInt)
            {
                return Contains(itemAsInt);
            }

            return false;
        }

        public void Clear()
        {
            _count = 0;
            _innerArray = null;
        }

        public int IndexOf(int item)
        {
            int index = -1;

            if (_innerArray is not null)
            {
                index = Array.IndexOf(_innerArray, item);

                // We initialize innerArray with more elements than needed in the method EnsureSpace,
                // and we don't actually remove element from innerArray in the method RemoveAt,
                // so there maybe some elements which are not actually in innerArray will be found
                // and we need to filter them out
                if (index >= _count)
                {
                    index = -1;
                }
            }

            return index;
        }

        int IList.IndexOf(object? item)
        {
            if (item is int itemAsInt)
            {
                return IndexOf(itemAsInt);
            }

            return -1;
        }

        /// <summary>
        ///  Add a unique integer to the collection in sorted order.
        ///  A SystemException occurs if there is insufficient space available to
        ///  store the new item.
        /// </summary>
        private int AddInternal(int item)
        {
            EnsureSpace(1);

            int index = IndexOf(item);
            if (index == -1)
            {
                _innerArray![_count++] = item;
                Array.Sort(_innerArray, 0, _count);
                index = IndexOf(item);
            }

            return index;
        }

        /// <summary>
        ///  Adds a unique integer to the collection in sorted order.
        ///  A SystemException occurs if there is insufficient space available to
        ///  store the new item.
        /// </summary>
        public int Add(int item)
        {
            int index = AddInternal(item);
            _owner.UpdateCustomTabOffsets();

            return index;
        }

        int IList.Add(object? item)
        {
            if (item is not int)
            {
                throw new ArgumentException(null, nameof(item));
            }

            return Add((int)item);
        }

        public void AddRange(params int[] items)
        {
            AddRangeInternal(items);
        }

        public void AddRange(IntegerCollection value)
        {
            AddRangeInternal(value);
        }

        /// <summary>
        ///  Add range that bypasses the data source check.
        /// </summary>
        private void AddRangeInternal(ICollection items)
        {
            ArgumentNullException.ThrowIfNull(items);

            _owner.BeginUpdate();
            try
            {
                EnsureSpace(items.Count);
                foreach (object item in items)
                {
                    if (item is not int)
                    {
                        throw new ArgumentException(nameof(item));
                    }
                    else
                    {
                        AddInternal((int)item);
                    }
                }

                _owner.UpdateCustomTabOffsets();
            }
            finally
            {
                _owner.EndUpdate();
            }
        }

        /// <summary>
        ///  Ensures that our internal array has space for
        ///  the requested # of elements.
        /// </summary>
        private void EnsureSpace(int elements)
        {
            if (_innerArray is null)
            {
                _innerArray = new int[Math.Max(elements, 4)];
            }
            else if (_count + elements >= _innerArray.Length)
            {
                int newLength = Math.Max(_innerArray.Length * 2, _innerArray.Length + elements);
                int[] newEntries = new int[newLength];
                _innerArray.CopyTo(newEntries, 0);
                _innerArray = newEntries;
            }
        }

        void IList.Clear()
        {
            Clear();
        }

        void IList.Insert(int index, object? value)
        {
            throw new NotSupportedException(SR.ListBoxCantInsertIntoIntegerCollection);
        }

        void IList.Remove(object? value)
        {
            if (value is not int)
            {
                throw new ArgumentException(null, nameof(value));
            }

            Remove((int)value);
        }

        void IList.RemoveAt(int index)
        {
            RemoveAt(index);
        }

        /// <summary>
        ///  Removes the given item from the array. If
        ///  the item is not in the array, this does nothing.
        /// </summary>
        public void Remove(int item)
        {
            int index = IndexOf(item);

            if (index != -1)
            {
                RemoveAt(index);
            }
        }

        /// <summary>
        ///  Removes the item at the given index.
        /// </summary>
        public void RemoveAt(int index)
        {
            ArgumentOutOfRangeException.ThrowIfNegative(index);
            ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(index, _count);

            _count--;
            for (int i = index; i < _count; i++)
            {
                _innerArray![i] = _innerArray[i + 1];
            }
        }

        /// <summary>
        ///  Retrieves the specified selected item.
        /// </summary>
        public int this[int index]
        {
            get
            {
                ArgumentOutOfRangeException.ThrowIfNegative(index);
                ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(index, _count);

                return _innerArray![index];
            }

            set
            {
                ArgumentOutOfRangeException.ThrowIfNegative(index);
                ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(index, _count);

                _innerArray![index] = value;
                _owner.UpdateCustomTabOffsets();
            }
        }

        object? IList.this[int index]
        {
            get
            {
                return this[index];
            }
            set
            {
                if (value is not int)
                {
                    throw new ArgumentException(null, nameof(value));
                }
                else
                {
                    this[index] = (int)value;
                }
            }
        }

        public void CopyTo(Array destination, int index)
        {
            ArgumentNullException.ThrowIfNull(destination);

            int cnt = Count;
            for (int i = 0; i < cnt; i++)
            {
                destination.SetValue(this[i], i + index);
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return new CustomTabOffsetsEnumerator(this);
        }
    }
}
