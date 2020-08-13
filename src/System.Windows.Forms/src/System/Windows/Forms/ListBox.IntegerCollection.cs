// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable disable

using System.Collections;
using System.ComponentModel;

namespace System.Windows.Forms
{
    public partial class ListBox
    {
        public class IntegerCollection : IList
        {
            private readonly ListBox owner;
            private int[] innerArray;
            private int count;

            public IntegerCollection(ListBox owner)
            {
                this.owner = owner ?? throw new ArgumentNullException(nameof(owner));
            }

            /// <summary>
            ///  Number of current selected items.
            /// </summary>
            [Browsable(false)]
            public int Count
            {
                get
                {
                    return count;
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

            bool IList.Contains(object item)
            {
                if (item is int)
                {
                    return Contains((int)item);
                }
                else
                {
                    return false;
                }
            }

            public void Clear()
            {
                count = 0;
                innerArray = null;
            }

            public int IndexOf(int item)
            {
                int index = -1;

                if (innerArray != null)
                {
                    index = Array.IndexOf(innerArray, item);

                    // We initialize innerArray with more elements than needed in the method EnsureSpace,
                    // and we don't actually remove element from innerArray in the method RemoveAt,
                    // so there maybe some elements which are not actually in innerArray will be found
                    // and we need to filter them out
                    if (index >= count)
                    {
                        index = -1;
                    }
                }

                return index;
            }

            int IList.IndexOf(object item)
            {
                if (item is int)
                {
                    return IndexOf((int)item);
                }
                else
                {
                    return -1;
                }
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
                    innerArray[count++] = item;
                    Array.Sort(innerArray, 0, count);
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
                owner.UpdateCustomTabOffsets();

                return index;
            }

            int IList.Add(object item)
            {
                if (!(item is int))
                {
                    throw new ArgumentException(nameof(item));
                }
                return Add((int)item);
            }

            public void AddRange(int[] items)
            {
                AddRangeInternal((ICollection)items);
            }

            public void AddRange(IntegerCollection value)
            {
                AddRangeInternal((ICollection)value);
            }

            /// <summary>
            ///  Add range that bypasses the data source check.
            /// </summary>
            private void AddRangeInternal(ICollection items)
            {
                if (items is null)
                {
                    throw new ArgumentNullException(nameof(items));
                }
                owner.BeginUpdate();
                try
                {
                    EnsureSpace(items.Count);
                    foreach (object item in items)
                    {
                        if (!(item is int))
                        {
                            throw new ArgumentException(nameof(item));
                        }
                        else
                        {
                            AddInternal((int)item);
                        }
                    }
                    owner.UpdateCustomTabOffsets();
                }
                finally
                {
                    owner.EndUpdate();
                }
            }

            /// <summary>
            ///  Ensures that our internal array has space for
            ///  the requested # of elements.
            /// </summary>
            private void EnsureSpace(int elements)
            {
                if (innerArray is null)
                {
                    innerArray = new int[Math.Max(elements, 4)];
                }
                else if (count + elements >= innerArray.Length)
                {
                    int newLength = Math.Max(innerArray.Length * 2, innerArray.Length + elements);
                    int[] newEntries = new int[newLength];
                    innerArray.CopyTo(newEntries, 0);
                    innerArray = newEntries;
                }
            }

            void IList.Clear()
            {
                Clear();
            }

            void IList.Insert(int index, object value)
            {
                throw new NotSupportedException(SR.ListBoxCantInsertIntoIntegerCollection);
            }

            void IList.Remove(object value)
            {
                if (!(value is int))
                {
                    throw new ArgumentException(nameof(value));
                }
                Remove((int)value);
            }

            void IList.RemoveAt(int index)
            {
                RemoveAt(index);
            }

            /// <summary>
            ///  Removes the given item from the array.  If
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
                if (index < 0 || index >= count)
                {
                    throw new ArgumentOutOfRangeException(nameof(index), index, string.Format(SR.InvalidArgument, nameof(index), index));
                }

                count--;
                for (int i = index; i < count; i++)
                {
                    innerArray[i] = innerArray[i + 1];
                }
            }

            /// <summary>
            ///  Retrieves the specified selected item.
            /// </summary>
            public int this[int index]
            {
                get
                {
                    if (index < 0 || index >= count)
                    {
                        throw new ArgumentOutOfRangeException(nameof(index), index, string.Format(SR.InvalidArgument, nameof(index), index));
                    }

                    return innerArray[index];
                }
                set
                {
                    if (index < 0 || index >= count)
                    {
                        throw new ArgumentOutOfRangeException(nameof(index), index, string.Format(SR.InvalidArgument, nameof(index), index));
                    }
                    innerArray[index] = (int)value;
                    owner.UpdateCustomTabOffsets();
                }
            }

            object IList.this[int index]
            {
                get
                {
                    return this[index];
                }
                set
                {
                    if (!(value is int))
                    {
                        throw new ArgumentException(nameof(value));
                    }
                    else
                    {
                        this[index] = (int)value;
                    }
                }
            }

            public void CopyTo(Array destination, int index)
            {
                if (destination is null)
                {
                    throw new ArgumentNullException(nameof(destination));
                }

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

            /// <summary>
            ///  EntryEnumerator is an enumerator that will enumerate over
            ///  a given state mask.
            /// </summary>
            private class CustomTabOffsetsEnumerator : IEnumerator
            {
                private readonly IntegerCollection items;
                private int current;

                /// <summary>
                ///  Creates a new enumerator that will enumerate over the given state.
                /// </summary>
                public CustomTabOffsetsEnumerator(IntegerCollection items)
                {
                    this.items = items;
                    current = -1;
                }

                /// <summary>
                ///  Moves to the next element, or returns false if at the end.
                /// </summary>
                bool IEnumerator.MoveNext()
                {
                    if (current < items.Count - 1)
                    {
                        current++;
                        return true;
                    }
                    else
                    {
                        current = items.Count;
                        return false;
                    }
                }

                /// <summary>
                ///  Resets the enumeration back to the beginning.
                /// </summary>
                void IEnumerator.Reset()
                {
                    current = -1;
                }

                /// <summary>
                ///  Retrieves the current value in the enumerator.
                /// </summary>
                object IEnumerator.Current
                {
                    get
                    {
                        if (current == -1 || current == items.Count)
                        {
                            throw new InvalidOperationException(SR.ListEnumCurrentOutOfRange);
                        }

                        return items[current];
                    }
                }
            }
        }
    }
}
