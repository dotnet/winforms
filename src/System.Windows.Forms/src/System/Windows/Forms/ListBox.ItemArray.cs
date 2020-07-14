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
            private static int lastMask = 1;

            private readonly ListControl listControl;
            private Entry[] entries;
            private int count;
            private int version;

            public ItemArray(ListControl listControl)
            {
                this.listControl = listControl;
            }

            internal IReadOnlyList<Entry> Entries => entries;

            /// <summary>
            ///  The version of this array.  This number changes with each
            ///  change to the item list.
            /// </summary>
            public int Version
            {
                get
                {
                    return version;
                }
            }

            /// <summary>
            ///  Adds the given item to the array.  The state is initially
            ///  zero.
            /// </summary>
            public object Add(object item)
            {
                EnsureSpace(1);
                version++;
                entries[count] = new Entry(item);
                return entries[count++];
            }

            /// <summary>
            ///  Adds the given collection of items to the array.
            /// </summary>
            public void AddRange(ICollection items)
            {
                if (items == null)
                {
                    throw new ArgumentNullException(nameof(items));
                }
                EnsureSpace(items.Count);
                foreach (object i in items)
                {
                    entries[count++] = new Entry(i);
                }
                version++;
            }

            /// <summary>
            ///  Clears this array.
            /// </summary>
            public void Clear()
            {
                if (count > 0)
                {
                    Array.Clear(entries, 0, count);
                }

                count = 0;
                version++;
            }

            /// <summary>
            ///  Allocates a new bitmask for use.
            /// </summary>
            public static int CreateMask()
            {
                int mask = lastMask;
                lastMask <<= 1;
                Debug.Assert(lastMask > mask, "We have overflowed our state mask.");
                return mask;
            }

            /// <summary>
            ///  Ensures that our internal array has space for
            ///  the requested # of elements.
            /// </summary>
            private void EnsureSpace(int elements)
            {
                if (entries == null)
                {
                    entries = new Entry[Math.Max(elements, 4)];
                }
                else if (count + elements >= entries.Length)
                {
                    int newLength = Math.Max(entries.Length * 2, entries.Length + elements);
                    Entry[] newEntries = new Entry[newLength];
                    entries.CopyTo(newEntries, 0);
                    entries = newEntries;
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
                for (int i = 0; i < count; i++)
                {
                    if ((entries[i].state & stateMask) != 0)
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
                    return count;
                }

                // more complex:  must provide a count of items
                // based on a mask.

                int filteredCount = 0;

                for (int i = 0; i < count; i++)
                {
                    if ((entries[i].state & stateMask) != 0)
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

                return entries[actualIndex].item;
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

                return entries[actualIndex];
            }
            /// <summary>
            ///  Returns true if the requested state mask is set.
            ///  The index is the actual index to the array.
            /// </summary>
            public bool GetState(int index, int stateMask)
            {
                return ((entries[index].state & stateMask) == stateMask);
            }

            /// <summary>
            ///  Returns the virtual index of the item based on the
            ///  state mask.
            /// </summary>
            public int IndexOf(object item, int stateMask)
            {
                int virtualIndex = -1;

                for (int i = 0; i < count; i++)
                {
                    if (stateMask == 0 || (entries[i].state & stateMask) != 0)
                    {
                        virtualIndex++;
                        if (entries[i].item.Equals(item))
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

                for (int i = 0; i < count; i++)
                {
                    if (stateMask == 0 || (entries[i].state & stateMask) != 0)
                    {
                        virtualIndex++;
                        if (entries[i] == identifier)
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

                if (index < count)
                {
                    System.Array.Copy(entries, index, entries, index + 1, count - index);
                }

                entries[index] = new Entry(item);
                count++;
                version++;
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
                count--;
                for (int i = index; i < count; i++)
                {
                    entries[i] = entries[i + 1];
                }
                entries[count] = null;
                version++;
            }

            /// <summary>
            ///  Sets the item at the given index to a new value.
            /// </summary>
            public void SetItem(int index, object item)
            {
                entries[index].item = item;
            }

            /// <summary>
            ///  Sets the state data for the given index.
            /// </summary>
            public void SetState(int index, int stateMask, bool value)
            {
                if (value)
                {
                    entries[index].state |= stateMask;
                }
                else
                {
                    entries[index].state &= ~stateMask;
                }
                version++;
            }

            /// <summary>
            ///  Find element in sorted array. If element is not found returns a binary complement of index for inserting
            /// </summary>
            public int BinarySearch(object element)
            {
                return Array.BinarySearch(entries, 0, count, element, this);
            }

            /// <summary>
            ///  Sorts our array.
            /// </summary>
            public void Sort()
            {
                Array.Sort(entries, 0, count, this);
            }

            public void Sort(Array externalArray)
            {
                Array.Sort(externalArray, this);
            }

            int IComparer.Compare(object item1, object item2)
            {
                if (item1 == null)
                {
                    if (item2 == null)
                    {
                        return 0; //both null, then they are equal
                    }

                    return -1; //item1 is null, but item2 is valid (greater)
                }
                if (item2 == null)
                {
                    return 1; //item2 is null, so item 1 is greater
                }

                if (item1 is Entry)
                {
                    item1 = ((Entry)item1).item;
                }

                if (item2 is Entry)
                {
                    item2 = ((Entry)item2).item;
                }

                string itemName1 = listControl.GetItemText(item1);
                string itemName2 = listControl.GetItemText(item2);

                CompareInfo compInfo = (Application.CurrentCulture).CompareInfo;
                return compInfo.Compare(itemName1, itemName2, CompareOptions.StringSort);
            }
        }
    }
}
