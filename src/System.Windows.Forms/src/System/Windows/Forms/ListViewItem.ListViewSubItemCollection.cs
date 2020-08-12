// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable disable

using System.Collections;
using System.ComponentModel;
using System.Drawing;

namespace System.Windows.Forms
{
    public partial class ListViewItem
    {
        public class ListViewSubItemCollection : IList
        {
            private readonly ListViewItem _owner;

            // A caching mechanism for key accessor
            // We use an index here rather than control so that we don't have lifetime
            // issues by holding on to extra references.
            private int _lastAccessedIndex = -1;

            public ListViewSubItemCollection(ListViewItem owner)
            {
                _owner = owner ?? throw new ArgumentNullException(nameof(owner));
            }

            /// <summary>
            ///  Returns the total number of items within the list view.
            /// </summary>
            [Browsable(false)]
            public int Count => _owner.SubItemCount;

            object ICollection.SyncRoot => this;

            bool ICollection.IsSynchronized => true;

            bool IList.IsFixedSize => false;

            public bool IsReadOnly => false;

            /// <summary>
            ///  Returns a ListViewSubItem given it's zero based index into the ListViewSubItemCollection.
            /// </summary>
            public ListViewSubItem this[int index]
            {
                get
                {
                    if (index < 0 || index >= Count)
                    {
                        throw new ArgumentOutOfRangeException(nameof(index), index, string.Format(SR.InvalidArgument, nameof(index), index));
                    }

                    return _owner.subItems[index];
                }
                set
                {
                    if (index < 0 || index >= Count)
                    {
                        throw new ArgumentOutOfRangeException(nameof(index), index, string.Format(SR.InvalidArgument, nameof(index), index));
                    }

                    _owner.subItems[index] = value ?? throw new ArgumentNullException(nameof(value));
                    _owner.UpdateSubItems(index);
                }
            }

            object IList.this[int index]
            {
                get => this[index];
                set
                {
                    if (!(value is ListViewSubItem item))
                    {
                        throw new ArgumentException(SR.ListViewBadListViewSubItem, nameof(value));
                    }

                    this[index] = item;
                }
            }
            /// <summary>
            ///  Retrieves the child control with the specified key.
            /// </summary>
            public virtual ListViewSubItem this[string key]
            {
                get
                {
                    // We do not support null and empty string as valid keys.
                    if (string.IsNullOrEmpty(key))
                    {
                        return null;
                    }

                    // Search for the key in our collection
                    int index = IndexOfKey(key);
                    if (!IsValidIndex(index))
                    {
                        return null;
                    }

                    return this[index];
                }
            }

            public ListViewSubItem Add(ListViewSubItem item)
            {
                if (item is null)
                {
                    throw new ArgumentNullException(nameof(item));
                }

                EnsureSubItemSpace(1, -1);
                item.owner = _owner;
                _owner.subItems[_owner.SubItemCount] = item;
                _owner.UpdateSubItems(_owner.SubItemCount++);
                return item;
            }

            public ListViewSubItem Add(string text)
            {
                ListViewSubItem item = new ListViewSubItem(_owner, text);
                Add(item);
                return item;
            }

            public ListViewSubItem Add(string text, Color foreColor, Color backColor, Font font)
            {
                ListViewSubItem item = new ListViewSubItem(_owner, text, foreColor, backColor, font);
                Add(item);
                return item;
            }

            public void AddRange(ListViewSubItem[] items)
            {
                if (items is null)
                {
                    throw new ArgumentNullException(nameof(items));
                }

                EnsureSubItemSpace(items.Length, -1);
                foreach (ListViewSubItem item in items)
                {
                    if (item != null)
                    {
                        _owner.subItems[_owner.SubItemCount++] = item;
                    }
                }

                _owner.UpdateSubItems(-1);
            }

            public void AddRange(string[] items)
            {
                if (items is null)
                {
                    throw new ArgumentNullException(nameof(items));
                }

                EnsureSubItemSpace(items.Length, -1);
                foreach (string item in items)
                {
                    if (item != null)
                    {
                        _owner.subItems[_owner.SubItemCount++] = new ListViewSubItem(_owner, item);
                    }
                }

                _owner.UpdateSubItems(-1);
            }

            public void AddRange(string[] items, Color foreColor, Color backColor, Font font)
            {
                if (items is null)
                {
                    throw new ArgumentNullException(nameof(items));
                }

                EnsureSubItemSpace(items.Length, -1);
                foreach (string item in items)
                {
                    if (item != null)
                    {
                        _owner.subItems[_owner.SubItemCount++] = new ListViewSubItem(_owner, item, foreColor, backColor, font);
                    }
                }

                _owner.UpdateSubItems(-1);
            }

            int IList.Add(object item)
            {
                if (!(item is ListViewSubItem itemValue))
                {
                    throw new ArgumentException(SR.ListViewSubItemCollectionInvalidArgument, nameof(item));
                }

                return IndexOf(Add(itemValue));
            }

            public void Clear()
            {
                int oldCount = _owner.SubItemCount;
                if (oldCount > 0)
                {
                    _owner.SubItemCount = 0;
                    _owner.UpdateSubItems(-1, oldCount);
                }
            }

            public bool Contains(ListViewSubItem subItem) => IndexOf(subItem) != -1;

            bool IList.Contains(object item)
            {
                if (!(item is ListViewSubItem itemValue))
                {
                    return false;
                }

                return Contains(itemValue);
            }

            /// <summary>
            ///  Returns true if the collection contains an item with the specified key, false otherwise.
            /// </summary>
            public virtual bool ContainsKey(string key) => IsValidIndex(IndexOfKey(key));

            /// <summary>
            ///  Ensures that the sub item array has the given
            ///  capacity. If it doesn't, it enlarges the
            ///  array until it does. If index is -1, additional
            ///  space is tacked onto the end. If it is a valid
            ///  insertion index into the array, this will move
            ///  the array data to accomodate the space.
            /// </summary>
            private void EnsureSubItemSpace(int size, int index)
            {
                if (_owner.SubItemCount == MaxSubItems)
                {
                    throw new InvalidOperationException(SR.ErrorCollectionFull);
                }

                if (_owner.subItems is null || _owner.SubItemCount + size > _owner.subItems.Length)
                {
                    // Must grow array. Don't do it just by size, though;
                    // chunk it for efficiency.
                    if (_owner.subItems is null)
                    {
                        int newSize = (size > 4) ? size : 4;
                        _owner.subItems = new ListViewSubItem[newSize];
                    }
                    else
                    {
                        int newSize = _owner.subItems.Length * 2;
                        while (newSize - _owner.SubItemCount < size)
                        {
                            newSize *= 2;
                        }

                        ListViewSubItem[] newItems = new ListViewSubItem[newSize];

                        // Now, when copying to the member variable, use index
                        // if it was provided.
                        if (index != -1)
                        {
                            Array.Copy(_owner.subItems, 0, newItems, 0, index);
                            Array.Copy(_owner.subItems, index, newItems, index + size, _owner.SubItemCount - index);
                        }
                        else
                        {
                            Array.Copy(_owner.subItems, newItems, _owner.SubItemCount);
                        }
                        _owner.subItems = newItems;
                    }
                }
                else
                {
                    // We had plenty of room. Just move the items if we need to
                    if (index != -1)
                    {
                        for (int i = _owner.SubItemCount - 1; i >= index; i--)
                        {
                            _owner.subItems[i + size] = _owner.subItems[i];
                        }
                    }
                }
            }

            public int IndexOf(ListViewSubItem subItem)
            {
                for (int index = 0; index < Count; ++index)
                {
                    if (_owner.subItems[index] == subItem)
                    {
                        return index;
                    }
                }

                return -1;
            }

            int IList.IndexOf(object subItem)
            {
                if (!(subItem is ListViewSubItem subItemValue))
                {
                    return -1;
                }

                return IndexOf(subItemValue);
            }

            /// <summary>
            ///  The zero-based index of the first occurrence of value within the entire CollectionBase, if found; otherwise, -1.
            /// </summary>
            public virtual int IndexOfKey(string key)
            {
                if (string.IsNullOrEmpty(key))
                {
                    return -1;
                }

                if (IsValidIndex(_lastAccessedIndex))
                {
                    if (WindowsFormsUtils.SafeCompareStrings(this[_lastAccessedIndex].Name, key, ignoreCase: true))
                    {
                        return _lastAccessedIndex;
                    }
                }

                for (int i = 0; i < Count; i++)
                {
                    if (WindowsFormsUtils.SafeCompareStrings(this[i].Name, key, ignoreCase: true))
                    {
                        _lastAccessedIndex = i;
                        return i;
                    }
                }

                _lastAccessedIndex = -1;
                return -1;
            }

            /// <summary>
            ///  Determines if the index is valid for the collection.
            /// </summary>
            private bool IsValidIndex(int index) => ((index >= 0) && (index < Count));

            public void Insert(int index, ListViewSubItem item)
            {
                if (index < 0 || index > Count)
                {
                    throw new ArgumentOutOfRangeException(nameof(index));
                }
                if (item is null)
                {
                    throw new ArgumentNullException(nameof(item));
                }

                item.owner = _owner;

                EnsureSubItemSpace(1, index);

                // Insert new item
                _owner.subItems[index] = item;
                _owner.SubItemCount++;
                _owner.UpdateSubItems(-1);
            }

            void IList.Insert(int index, object item)
            {
                if (!(item is ListViewSubItem itemValue))
                {
                    throw new ArgumentException(SR.ListViewBadListViewSubItem, nameof(item));
                }

                Insert(index, (ListViewSubItem)item);
            }

            public void Remove(ListViewSubItem item)
            {
                int index = IndexOf(item);
                if (index != -1)
                {
                    RemoveAt(index);
                }
            }

            void IList.Remove(object item)
            {
                if (item is ListViewSubItem itemValue)
                {
                    Remove(itemValue);
                }
            }

            public void RemoveAt(int index)
            {
                if (index < 0 || index >= Count)
                {
                    throw new ArgumentOutOfRangeException(nameof(index));
                }

                // Remove ourselves as the owner.
                _owner.subItems[index].owner = null;

                // Collapse the items
                for (int i = index + 1; i < _owner.SubItemCount; i++)
                {
                    _owner.subItems[i - 1] = _owner.subItems[i];
                }

                int oldCount = _owner.SubItemCount;
                _owner.SubItemCount--;
                _owner.subItems[_owner.SubItemCount] = null;
                _owner.UpdateSubItems(-1, oldCount);
            }

            /// <summary>
            ///  Removes the child control with the specified key.
            /// </summary>
            public virtual void RemoveByKey(string key)
            {
                int index = IndexOfKey(key);
                if (IsValidIndex(index))
                {
                    RemoveAt(index);
                }
            }

            void ICollection.CopyTo(Array dest, int index)
            {
                if (Count > 0)
                {
                    Array.Copy(_owner.subItems, 0, dest, index, Count);
                }
            }

            public IEnumerator GetEnumerator()
            {
                if (_owner.subItems != null)
                {
                    return new ArraySubsetEnumerator(_owner.subItems, _owner.SubItemCount);
                }
                else
                {
                    return Array.Empty<ListViewSubItem>().GetEnumerator();
                }
            }
        }
    }
}
