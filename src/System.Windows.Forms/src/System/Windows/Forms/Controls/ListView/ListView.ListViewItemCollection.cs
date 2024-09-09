// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Collections;
using System.ComponentModel;

namespace System.Windows.Forms;

public partial class ListView
{
    /// <summary>
    ///  Represents the collection of items in a ListView or ListViewGroup
    /// </summary>
    [ListBindable(false)]
    public partial class ListViewItemCollection : IList
    {
        ///  A caching mechanism for key accessor
        ///  We use an index here rather than control so that we don't have lifetime
        ///  issues by holding on to extra references.
        private int _lastAccessedIndex = -1;

        private readonly IInnerList _innerList;

        public ListViewItemCollection(ListView owner)
        {
            // Kept for APPCOMPAT reasons.
            // In Whidbey this constructor is a no-op.

            // initialize the inner list w/ a dummy list.
            _innerList = new ListViewNativeItemCollection(owner);
        }

        internal ListViewItemCollection(IInnerList innerList)
        {
            Debug.Assert(innerList is not null, "Can't pass in null innerList");
            _innerList = innerList;
        }

        private IInnerList InnerList => _innerList;

        /// <summary>
        ///  Returns the total number of items within the list view.
        /// </summary>
        [Browsable(false)]
        public int Count => InnerList.Count;

        object ICollection.SyncRoot => this;

        bool ICollection.IsSynchronized => true;

        bool IList.IsFixedSize => false;

        public bool IsReadOnly => false;

        /// <summary>
        ///  Returns the ListViewItem at the given index.
        /// </summary>
        public virtual ListViewItem this[int index]
        {
            get
            {
                ArgumentOutOfRangeException.ThrowIfNegative(index);
                ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(index, InnerList.Count);

                return InnerList[index];
            }
            set
            {
                ArgumentOutOfRangeException.ThrowIfNegative(index);
                ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(index, InnerList.Count);

                InnerList[index] = value;
            }
        }

        internal ListViewItem? GetItemByIndex(int index)
        {
            ArgumentOutOfRangeException.ThrowIfNegative(index);
            ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(index, InnerList.Count);

            return InnerList.GetItemByIndex(index);
        }

        object? IList.this[int index]
        {
            get
            {
                return this[index];
            }
            set
            {
                this[index] = value is ListViewItem item
                    ? item
                    : new ListViewItem(value!.ToString(), -1);
            }
        }

        /// <summary>
        ///  Retrieves the child control with the specified key.
        /// </summary>
        public virtual ListViewItem? this[string key]
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
                if (IsValidIndex(index))
                {
                    return this[index];
                }

                return null;
            }
        }

        /// <summary>
        ///  Add an item to the ListView. The item will be inserted either in
        ///  the correct sorted position, or, if no sorting is set, at the end
        ///  of the list.
        /// </summary>
        public virtual ListViewItem Add(string? text)
        {
            return Add(text, -1);
        }

        int IList.Add(object? item)
        {
            if (item is ListViewItem listViewItem)
            {
                return IndexOf(Add(listViewItem));
            }

            if (item is { } obj)
            {
                return IndexOf(Add(obj.ToString()));
            }

            return -1;
        }

        /// <summary>
        ///  Add an item to the ListView. The item will be inserted either in
        ///  the correct sorted position, or, if no sorting is set, at the end
        ///  of the list.
        /// </summary>
        public virtual ListViewItem Add(string? text, int imageIndex)
        {
            ListViewItem item = new(text, imageIndex);
            Add(item);
            return item;
        }

        /// <summary>
        ///  Add an item to the ListView. The item will be inserted either in
        ///  the correct sorted position, or, if no sorting is set, at the end
        ///  of the list.
        /// </summary>
        public virtual ListViewItem Add(ListViewItem value)
        {
            InnerList.Add(value);
            return value;
        }

        // <-- NEW ADD OVERLOADS IN WHIDBEY

        /// <summary>
        ///  Add an item to the ListView. The item will be inserted either in
        ///  the correct sorted position, or, if no sorting is set, at the end
        ///  of the list.
        /// </summary>
        public virtual ListViewItem Add(string? text, string? imageKey)
        {
            ListViewItem item = new(text, imageKey);
            Add(item);
            return item;
        }

        /// <summary>
        ///  Add an item to the ListView. The item will be inserted either in
        ///  the correct sorted position, or, if no sorting is set, at the end
        ///  of the list.
        /// </summary>
        public virtual ListViewItem Add(string? key, string? text, string? imageKey)
        {
            ListViewItem item = new(text, imageKey)
            {
                Name = key
            };
            Add(item);
            return item;
        }

        /// <summary>
        ///  Add an item to the ListView. The item will be inserted either in
        ///  the correct sorted position, or, if no sorting is set, at the end
        ///  of the list.
        /// </summary>
        public virtual ListViewItem Add(string? key, string? text, int imageIndex)
        {
            ListViewItem item = new(text, imageIndex)
            {
                Name = key
            };
            Add(item);
            return item;
        }

        // END - NEW ADD OVERLOADS IN WHIDBEY  -->

        public void AddRange(params ListViewItem[] items)
        {
            ArgumentNullException.ThrowIfNull(items);

            InnerList.AddRange(items);
        }

        public void AddRange(ListViewItemCollection items)
        {
            ArgumentNullException.ThrowIfNull(items);

            var itemArray = new ListViewItem[items.Count];
            items.CopyTo(itemArray, 0);
            InnerList.AddRange(itemArray);
        }

        /// <summary>
        ///  Removes all items from the list view.
        /// </summary>
        public virtual void Clear()
        {
            InnerList.Clear();
        }

        public bool Contains(ListViewItem item)
        {
            return InnerList.Contains(item);
        }

        bool IList.Contains(object? item)
            => item is ListViewItem listViewItem && Contains(listViewItem);

        /// <summary>
        ///  Returns true if the collection contains an item with the specified key, false otherwise.
        /// </summary>
        public virtual bool ContainsKey(string? key)
        {
            return IsValidIndex(IndexOfKey(key));
        }

        public void CopyTo(Array dest, int index)
        {
            InnerList.CopyTo(dest, index);
        }

        /// <summary>
        ///  Searches for Controls by their Name property, builds up an array
        ///  of all the controls that match.
        /// </summary>
        public ListViewItem[] Find(string key, bool searchAllSubItems)
        {
            key.ThrowIfNullOrEmptyWithMessage(SR.FindKeyMayNotBeEmptyOrNull);

            List<ListViewItem> foundItems = [];
            FindInternal(key, searchAllSubItems, this, foundItems);

            return [.. foundItems];
        }

        /// <summary>
        ///  Searches for Controls by their Name property, builds up a list
        ///  of all the controls that match.
        /// </summary>
        private static void FindInternal(string? key, bool searchAllSubItems, ListViewItemCollection listViewItems, List<ListViewItem> foundItems)
        {
            for (int i = 0; i < listViewItems.Count; i++)
            {
                if (WindowsFormsUtils.SafeCompareStrings(listViewItems[i].Name, key, ignoreCase: true))
                {
                    foundItems.Add(listViewItems[i]);
                }
                else
                {
                    if (searchAllSubItems)
                    {
                        // start from 1, as we've already compared subitems[0]
                        for (int j = 1; j < listViewItems[i].SubItems.Count; j++)
                        {
                            if (WindowsFormsUtils.SafeCompareStrings(listViewItems[i].SubItems[j].Name, key, ignoreCase: true))
                            {
                                foundItems.Add(listViewItems[i]);
                                break;
                            }
                        }
                    }
                }
            }
        }

        public IEnumerator GetEnumerator()
        {
            if (InnerList.OwnerIsVirtualListView && !InnerList.OwnerIsDesignMode)
            {
                // Throw the exception only at runtime.
                throw new InvalidOperationException(SR.ListViewCantGetEnumeratorInVirtualMode);
            }

            return InnerList.GetEnumerator();
        }

        public int IndexOf(ListViewItem item)
        {
            for (int index = 0; index < Count; ++index)
            {
                if (this[index] == item)
                {
                    return index;
                }
            }

            return -1;
        }

        int IList.IndexOf(object? item)
            => item is ListViewItem listViewItem ? IndexOf(listViewItem) : -1;

        /// <summary>
        ///  The zero-based index of the first occurrence of value within the entire CollectionBase, if found; otherwise, -1.
        /// </summary>
        public virtual int IndexOfKey(string? key)
        {
            // Step 0 - Arg validation
            if (string.IsNullOrEmpty(key))
            {
                return -1; // we don't support empty or null keys.
            }

            // step 1 - check the last cached item
            if (IsValidIndex(_lastAccessedIndex))
            {
                if (WindowsFormsUtils.SafeCompareStrings(this[_lastAccessedIndex].Name, key, /* ignoreCase = */ true))
                {
                    return _lastAccessedIndex;
                }
            }

            // step 2 - search for the item
            for (int i = 0; i < Count; i++)
            {
                if (WindowsFormsUtils.SafeCompareStrings(this[i].Name, key, /* ignoreCase = */ true))
                {
                    _lastAccessedIndex = i;
                    return i;
                }
            }

            // step 3 - we didn't find it. Invalidate the last accessed index and return -1.
            _lastAccessedIndex = -1;
            return -1;
        }

        /// <summary>
        ///  Determines if the index is valid for the collection.
        /// </summary>
        private bool IsValidIndex(int index)
        {
            return index >= 0 && index < Count;
        }

        public ListViewItem Insert(int index, ListViewItem item)
        {
            ArgumentOutOfRangeException.ThrowIfNegative(index);
            ArgumentOutOfRangeException.ThrowIfGreaterThan(index, Count);

            InnerList.Insert(index, item);
            return item;
        }

        public ListViewItem Insert(int index, string? text)
        {
            return Insert(index, new ListViewItem(text));
        }

        public ListViewItem Insert(int index, string? text, int imageIndex)
        {
            return Insert(index, new ListViewItem(text, imageIndex));
        }

        void IList.Insert(int index, object? item)
        {
            if (item is ListViewItem listViewItem)
            {
                Insert(index, listViewItem);
            }
            else if (item is { } obj)
            {
                Insert(index, obj.ToString());
            }
        }

        // <-- NEW INSERT OVERLOADS IN WHIDBEY

        public ListViewItem Insert(int index, string? text, string? imageKey)
            => Insert(index, new ListViewItem(text, imageKey));

        public virtual ListViewItem Insert(int index, string? key, string? text, string? imageKey)
            => Insert(index, new ListViewItem(text, imageKey)
            {
                Name = key
            });

        public virtual ListViewItem Insert(int index, string? key, string? text, int imageIndex)
            => Insert(index, new ListViewItem(text, imageIndex)
            {
                Name = key
            });

        // END - NEW INSERT OVERLOADS IN WHIDBEY -->

        /// <summary>
        ///  Removes an item from the ListView
        /// </summary>
        public virtual void Remove(ListViewItem item)
        {
            InnerList.Remove(item);
        }

        /// <summary>
        ///  Removes an item from the ListView
        /// </summary>
        public virtual void RemoveAt(int index)
        {
            ArgumentOutOfRangeException.ThrowIfNegative(index);
            ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(index, Count);

            InnerList.RemoveAt(index);
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

        void IList.Remove(object? item)
        {
            if (item is ListViewItem listViewItem)
            {
                Remove(listViewItem);
            }
        }
    }
}
