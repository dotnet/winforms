// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable disable

using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;

namespace System.Windows.Forms
{
    public partial class ListBox
    {
        /// <summary>
        ///  This is similar to ArrayList except that it also
        ///  mantains a bit-flag based state element for each item
        ///  in the array.
        ///
        ///  The methods to enumerate, count and get data support
        ///  virtualized indexes.  Indexes are virtualized according
        ///  to the state mask passed in.  This allows ItemArray
        ///  to be the backing store for one read-write "master"
        ///  collection and serveral read-only collections based
        ///  on masks.  ItemArray supports up to 31 masks.
        /// </summary>
        internal partial class ItemArray : IComparer
        {
            private static int s_lastMask = 1;

            private readonly ListControl _listControl;
            private Entry[] _entries;
            private int _count;

            public ItemArray(ListControl listControl)
            {
                _listControl = listControl;
            }

            internal IReadOnlyList<Entry> Entries => _entries;

            /// <summary>
            ///  The version of this array.  This number changes with each
            ///  change to the item list.
            /// </summary>
            public int Version { get; private set; }

            /// <summary>
            ///  Adds the given item to the array.  The state is initially
            ///  zero.
            /// </summary>
            public object Add(object item)
            {
                EnsureSpace(1);
                Version++;
                _entries[_count] = new Entry(item);
                return _entries[_count++];
            }

            /// <summary>
            ///  Adds the given collection of items to the array.
            /// </summary>
            public void AddRange(ICollection items)
            {
                if (items is null)
                {
                    throw new ArgumentNullException(nameof(items));
                }
                EnsureSpace(items.Count);
                foreach (object i in items)
                {
                    _entries[_count++] = new Entry(i);
                }
                Version++;
            }

            /// <summary>
            ///  Clears this array.
            /// </summary>
            public void Clear()
            {
                if (_count > 0)
                {
                    Array.Clear(_entries, 0, _count);
                }

                _count = 0;
                Version++;
            }

            /// <summary>
            ///  Allocates a new bitmask for use.
            /// </summary>
            public static int CreateMask()
            {
                int mask = s_lastMask;
                s_lastMask <<= 1;
                Debug.Assert(s_lastMask > mask, "We have overflowed our state mask.");
                return mask;
            }

            /// <summary>
            ///  Ensures that our internal array has space for
            ///  the requested # of elements.
            /// </summary>
            private void EnsureSpace(int elements)
            {
                if (_entries is null)
                {
                    _entries = new Entry[Math.Max(elements, 4)];
                }
                else if (_count + elements >= _entries.Length)
                {
                    int newLength = Math.Max(_entries.Length * 2, _entries.Length + elements);
                    Entry[] newEntries = new Entry[newLength];
                    _entries.CopyTo(newEntries, 0);
                    _entries = newEntries;
                }
            }

            /// <summary>
            ///  Turns a virtual index into an actual index.
            /// </summary>
            public int GetActualIndex(int virtualIndex, int stateMask)
            {
                if (stateMask == 0)
                {
                    return virtualIndex;
                }

                // More complex; we must compute this index.
                int calcIndex = -1;
                for (int i = 0; i < _count; i++)
                {
                    if ((_entries[i].state & stateMask) != 0)
                    {
                        calcIndex++;
                        if (calcIndex == virtualIndex)
                        {
                            return i;
                        }
                    }
                }

                return -1;
            }

            /// <summary>
            ///  Gets the count of items matching the given mask.
            /// </summary>
            public int GetCount(int stateMask)
            {
                // If mask is zero, then just give the main count
                if (stateMask == 0)
                {
                    return _count;
                }

                // more complex:  must provide a count of items
                // based on a mask.

                int filteredCount = 0;

                for (int i = 0; i < _count; i++)
                {
                    if ((_entries[i].state & stateMask) != 0)
                    {
                        filteredCount++;
                    }
                }

                return filteredCount;
            }

            /// <summary>
            ///  Retrieves an enumerator that will enumerate based on
            ///  the given mask.
            /// </summary>
            public IEnumerator GetEnumerator(int stateMask)
            {
                return GetEnumerator(stateMask, false);
            }

            /// <summary>
            ///  Retrieves an enumerator that will enumerate based on
            ///  the given mask.
            /// </summary>
            public IEnumerator GetEnumerator(int stateMask, bool anyBit)
            {
                return new EntryEnumerator(this, stateMask, anyBit);
            }

            /// <summary>
            ///  Gets the item at the given index.  The index is
            ///  virtualized against the given mask value.
            /// </summary>
            public object GetItem(int virtualIndex, int stateMask)
            {
                int actualIndex = GetActualIndex(virtualIndex, stateMask);

                if (actualIndex == -1)
                {
                    throw new IndexOutOfRangeException();
                }

                return _entries[actualIndex].item;
            }
            /// <summary>
            ///  Gets the item at the given index.  The index is
            ///  virtualized against the given mask value.
            /// </summary>
            internal object GetEntryObject(int virtualIndex, int stateMask)
            {
                int actualIndex = GetActualIndex(virtualIndex, stateMask);

                if (actualIndex == -1)
                {
                    throw new IndexOutOfRangeException();
                }

                return _entries[actualIndex];
            }
            /// <summary>
            ///  Returns true if the requested state mask is set.
            ///  The index is the actual index to the array.
            /// </summary>
            public bool GetState(int index, int stateMask)
            {
                return ((_entries[index].state & stateMask) == stateMask);
            }

            /// <summary>
            ///  Returns the virtual index of the item based on the
            ///  state mask.
            /// </summary>
            public int IndexOf(object item, int stateMask)
            {
                int virtualIndex = -1;

                for (int i = 0; i < _count; i++)
                {
                    if (stateMask == 0 || (_entries[i].state & stateMask) != 0)
                    {
                        virtualIndex++;
                        if (_entries[i].item.Equals(item))
                        {
                            return virtualIndex;
                        }
                    }
                }

                return -1;
            }

            /// <summary>
            ///  Returns the virtual index of the item based on the
            ///  state mask. Uses reference equality to identify the
            ///  given object in the list.
            /// </summary>
            public int IndexOfIdentifier(object identifier, int stateMask)
            {
                int virtualIndex = -1;

                for (int i = 0; i < _count; i++)
                {
                    if (stateMask == 0 || (_entries[i].state & stateMask) != 0)
                    {
                        virtualIndex++;
                        if (_entries[i] == identifier)
                        {
                            return virtualIndex;
                        }
                    }
                }

                return -1;
            }

            /// <summary>
            ///  Inserts item at the given index.  The index
            ///  is not virtualized.
            /// </summary>
            public void Insert(int index, object item)
            {
                EnsureSpace(1);

                if (index < _count)
                {
                    System.Array.Copy(_entries, index, _entries, index + 1, _count - index);
                }

                _entries[index] = new Entry(item);
                _count++;
                Version++;
            }

            /// <summary>
            ///  Removes the given item from the array.  If
            ///  the item is not in the array, this does nothing.
            /// </summary>
            public void Remove(object item)
            {
                int index = IndexOf(item, 0);

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
                _count--;
                for (int i = index; i < _count; i++)
                {
                    _entries[i] = _entries[i + 1];
                }
                _entries[_count] = null;
                Version++;
            }

            /// <summary>
            ///  Sets the item at the given index to a new value.
            /// </summary>
            public void SetItem(int index, object item)
            {
                _entries[index].item = item;
            }

            /// <summary>
            ///  Sets the state data for the given index.
            /// </summary>
            public void SetState(int index, int stateMask, bool value)
            {
                if (value)
                {
                    _entries[index].state |= stateMask;
                }
                else
                {
                    _entries[index].state &= ~stateMask;
                }
                Version++;
            }

            /// <summary>
            ///  Find element in sorted array. If element is not found returns a binary complement of index for inserting
            /// </summary>
            public int BinarySearch(object element)
            {
                return Array.BinarySearch(_entries, 0, _count, element, this);
            }

            /// <summary>
            ///  Sorts our array.
            /// </summary>
            public void Sort()
            {
                Array.Sort(_entries, 0, _count, this);
            }

            public void Sort(Array externalArray)
            {
                Array.Sort(externalArray, this);
            }

            int IComparer.Compare(object item1, object item2)
            {
                if (item1 is null)
                {
                    if (item2 is null)
                    {
                        return 0; //both null, then they are equal
                    }

                    return -1; //item1 is null, but item2 is valid (greater)
                }
                if (item2 is null)
                {
                    return 1; //item2 is null, so item 1 is greater
                }

                if (item1 is Entry entry1)
                {
                    item1 = entry1.item;
                }

                if (item2 is Entry entry2)
                {
                    item2 = entry2.item;
                }

                string itemName1 = _listControl.GetItemText(item1);
                string itemName2 = _listControl.GetItemText(item2);

                CompareInfo compInfo = Application.CurrentCulture.CompareInfo;
                return compInfo.Compare(itemName1, itemName2, CompareOptions.StringSort);
            }
        }
    }
}
