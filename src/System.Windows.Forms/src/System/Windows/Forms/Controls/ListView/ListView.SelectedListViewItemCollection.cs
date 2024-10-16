// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Collections;
using System.ComponentModel;

namespace System.Windows.Forms;

public partial class ListView
{
    [ListBindable(false)]
    public class SelectedListViewItemCollection : IList
    {
        private readonly ListView _owner;

        ///  A caching mechanism for key accessor
        ///  We use an index here rather than control so that we don't have lifetime
        ///  issues by holding on to extra references.
        private int _lastAccessedIndex = -1;

        /* C#r: protected */
        public SelectedListViewItemCollection(ListView owner)
        {
            _owner = owner.OrThrowIfNull();
        }

        private ListViewItem[] SelectedItemArray
        {
            get
            {
                if (_owner.IsHandleCreated)
                {
                    int cnt = (int)PInvokeCore.SendMessage(_owner, PInvoke.LVM_GETSELECTEDCOUNT);

                    ListViewItem[] lvitems = new ListViewItem[cnt];

                    int displayIndex = -1;

                    for (int i = 0; i < cnt; i++)
                    {
                        int fidx = (int)PInvokeCore.SendMessage(
                            _owner,
                            PInvoke.LVM_GETNEXTITEM,
                            (WPARAM)displayIndex,
                            (LPARAM)PInvoke.LVNI_SELECTED);

                        if (fidx > -1)
                        {
                            lvitems[i] = _owner.Items[fidx];
                            displayIndex = fidx;
                        }
                        else
                        {
                            throw new InvalidOperationException(SR.SelectedNotEqualActual);
                        }
                    }

                    return lvitems;
                }
                else
                {
                    if (_owner._savedSelectedItems is not null)
                    {
                        ListViewItem[] cloned = new ListViewItem[_owner._savedSelectedItems.Count];
                        for (int i = 0; i < _owner._savedSelectedItems.Count; i++)
                        {
                            cloned[i] = _owner._savedSelectedItems[i];
                        }

                        return cloned;
                    }
                    else
                    {
                        return [];
                    }
                }
            }
        }

        /// <summary>
        ///  Number of currently selected items.
        /// </summary>
        [Browsable(false)]
        public int Count
        {
            get
            {
                if (_owner.VirtualMode)
                {
                    throw new InvalidOperationException(SR.ListViewCantAccessSelectedItemsCollectionWhenInVirtualMode);
                }

                if (_owner.IsHandleCreated)
                {
                    return (int)PInvokeCore.SendMessage(_owner, PInvoke.LVM_GETSELECTEDCOUNT);
                }
                else
                {
                    if (_owner._savedSelectedItems is not null)
                    {
                        return _owner._savedSelectedItems.Count;
                    }

                    return 0;
                }
            }
        }

        /// <summary>
        ///  Selected item in the list.
        /// </summary>
        public ListViewItem this[int index]
        {
            get
            {
                if (_owner.VirtualMode)
                {
                    throw new InvalidOperationException(SR.ListViewCantAccessSelectedItemsCollectionWhenInVirtualMode);
                }

                ArgumentOutOfRangeException.ThrowIfNegative(index);
                ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(index, Count);

                if (_owner.IsHandleCreated)
                {
                    // Count through the selected items in the ListView, until we reach the 'index'th selected item.
                    int fidx = -1;
                    for (int count = 0; count <= index; count++)
                    {
                        fidx = (int)PInvokeCore.SendMessage(
                            _owner,
                            PInvoke.LVM_GETNEXTITEM,
                            (WPARAM)fidx,
                            (LPARAM)PInvoke.LVNI_SELECTED);

                        Debug.Assert(fidx != -1, "Invalid index returned from LVM_GETNEXTITEM");
                    }

                    return _owner.Items[fidx];
                }
                else
                {
                    Debug.Assert(_owner._savedSelectedItems is not null, "Null selected items collection");
                    return _owner._savedSelectedItems[index];
                }
            }
        }

        /// <summary>
        ///  Retrieves the child control with the specified key.
        /// </summary>
        public virtual ListViewItem? this[string? key]
        {
            get
            {
                if (_owner.VirtualMode)
                {
                    throw new InvalidOperationException(SR.ListViewCantAccessSelectedItemsCollectionWhenInVirtualMode);
                }

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
                else
                {
                    return null;
                }
            }
        }

        object? IList.this[int index]
        {
            get
            {
                if (_owner.VirtualMode)
                {
                    throw new InvalidOperationException(SR.ListViewCantAccessSelectedItemsCollectionWhenInVirtualMode);
                }

                return this[index];
            }
            set
            {
                // SelectedListViewItemCollection is read-only
                throw new NotSupportedException();
            }
        }

        bool IList.IsFixedSize
        {
            get
            {
                return true;
            }
        }

        public bool IsReadOnly
        {
            get
            {
                return true;
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
                return false;
            }
        }

        int IList.Add(object? value)
        {
            // SelectedListViewItemCollection is read-only
            throw new NotSupportedException();
        }

        void IList.Insert(int index, object? value)
        {
            // SelectedListViewItemCollection is read-only
            throw new NotSupportedException();
        }

        /// <summary>
        ///  Determines if the index is valid for the collection.
        /// </summary>
        private bool IsValidIndex(int index)
        {
            return (index >= 0) && (index < Count);
        }

        void IList.Remove(object? value)
        {
            // SelectedListViewItemCollection is read-only
            throw new NotSupportedException();
        }

        void IList.RemoveAt(int index)
        {
            // SelectedListViewItemCollection is read-only
            throw new NotSupportedException();
        }

        /// <summary>
        ///  Deselects all items.
        /// </summary>
        public void Clear()
        {
            if (_owner.VirtualMode)
            {
                throw new InvalidOperationException(SR.ListViewCantAccessSelectedItemsCollectionWhenInVirtualMode);
            }

            ListViewItem[] items = SelectedItemArray;
            for (int i = 0; i < items.Length; i++)
            {
                items[i].Selected = false;
            }
        }

        /// <summary>
        ///  Returns true if the collection contains an item with the specified key, false otherwise.
        /// </summary>
        public virtual bool ContainsKey(string? key)
        {
            if (_owner.VirtualMode)
            {
                throw new InvalidOperationException(SR.ListViewCantAccessSelectedItemsCollectionWhenInVirtualMode);
            }

            return IsValidIndex(IndexOfKey(key));
        }

        public bool Contains(ListViewItem? item)
        {
            if (_owner.VirtualMode)
            {
                throw new InvalidOperationException(SR.ListViewCantAccessSelectedItemsCollectionWhenInVirtualMode);
            }

            return IndexOf(item) != -1;
        }

        bool IList.Contains(object? item)
        {
            if (_owner.VirtualMode)
            {
                throw new InvalidOperationException(SR.ListViewCantAccessSelectedItemsCollectionWhenInVirtualMode);
            }

            if (item is ListViewItem listViewItem)
            {
                return Contains(listViewItem);
            }
            else
            {
                return false;
            }
        }

        public void CopyTo(Array dest, int index)
        {
            if (_owner.VirtualMode)
            {
                throw new InvalidOperationException(SR.ListViewCantAccessSelectedItemsCollectionWhenInVirtualMode);
            }

            if (Count > 0)
            {
                Array.Copy(SelectedItemArray, 0, dest, index, Count);
            }
        }

        public IEnumerator GetEnumerator()
        {
            if (_owner.VirtualMode)
            {
                throw new InvalidOperationException(SR.ListViewCantAccessSelectedItemsCollectionWhenInVirtualMode);
            }

            ListViewItem[] items = SelectedItemArray;
            if (items is not null)
            {
                return items.GetEnumerator();
            }
            else
            {
                return Array.Empty<ListViewItem>().GetEnumerator();
            }
        }

        public int IndexOf(ListViewItem? item)
        {
            if (_owner.VirtualMode)
            {
                throw new InvalidOperationException(SR.ListViewCantAccessSelectedItemsCollectionWhenInVirtualMode);
            }

            ListViewItem[] items = SelectedItemArray;
            for (int index = 0; index < items.Length; ++index)
            {
                if (items[index] == item)
                {
                    return index;
                }
            }

            return -1;
        }

        int IList.IndexOf(object? item)
        {
            if (_owner.VirtualMode)
            {
                throw new InvalidOperationException(SR.ListViewCantAccessSelectedItemsCollectionWhenInVirtualMode);
            }

            if (item is ListViewItem listViewItem)
            {
                return IndexOf(listViewItem);
            }
            else
            {
                return -1;
            }
        }

        /// <summary>
        ///  The zero-based index of the first occurrence of value within the entire CollectionBase, if found; otherwise, -1.
        /// </summary>
        public virtual int IndexOfKey(string? key)
        {
            if (_owner.VirtualMode)
            {
                throw new InvalidOperationException(SR.ListViewCantAccessSelectedItemsCollectionWhenInVirtualMode);
            }

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
    }
}
