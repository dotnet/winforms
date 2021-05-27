// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable disable

using System.Collections;
using System.ComponentModel;
using System.Diagnostics;
using static Interop;
using static Interop.ComCtl32;

namespace System.Windows.Forms
{
    public partial class ListView
    {
        [ListBindable(false)]
        public class SelectedListViewItemCollection : IList
        {
            private readonly ListView owner;

            ///  A caching mechanism for key accessor
            ///  We use an index here rather than control so that we don't have lifetime
            ///  issues by holding on to extra references.
            private int lastAccessedIndex = -1;

            /* C#r: protected */
            public SelectedListViewItemCollection(ListView owner)
            {
                this.owner = owner;
            }

            private ListViewItem[] SelectedItemArray
            {
                get
                {
                    if (owner.IsHandleCreated)
                    {
                        int cnt = unchecked((int)(long)User32.SendMessageW(owner, (User32.WM)LVM.GETSELECTEDCOUNT));

                        ListViewItem[] lvitems = new ListViewItem[cnt];

                        int displayIndex = -1;

                        for (int i = 0; i < cnt; i++)
                        {
                            int fidx = unchecked((int)(long)User32.SendMessageW(owner, (User32.WM)LVM.GETNEXTITEM, (IntPtr)displayIndex, (IntPtr)LVNI.SELECTED));
                            if (fidx > -1)
                            {
                                lvitems[i] = owner.Items[fidx];
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
                        if (owner.savedSelectedItems is not null)
                        {
                            ListViewItem[] cloned = new ListViewItem[owner.savedSelectedItems.Count];
                            for (int i = 0; i < owner.savedSelectedItems.Count; i++)
                            {
                                cloned[i] = owner.savedSelectedItems[i];
                            }

                            return cloned;
                        }
                        else
                        {
                            return Array.Empty<ListViewItem>();
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
                    if (owner.VirtualMode)
                    {
                        throw new InvalidOperationException(SR.ListViewCantAccessSelectedItemsCollectionWhenInVirtualMode);
                    }

                    if (owner.IsHandleCreated)
                    {
                        return unchecked((int)(long)User32.SendMessageW(owner, (User32.WM)LVM.GETSELECTEDCOUNT));
                    }
                    else
                    {
                        if (owner.savedSelectedItems is not null)
                        {
                            return owner.savedSelectedItems.Count;
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
                    if (owner.VirtualMode)
                    {
                        throw new InvalidOperationException(SR.ListViewCantAccessSelectedItemsCollectionWhenInVirtualMode);
                    }

                    if (index < 0 || index >= Count)
                    {
                        throw new ArgumentOutOfRangeException(nameof(index), index, string.Format(SR.InvalidArgument, nameof(index), index));
                    }

                    if (owner.IsHandleCreated)
                    {
                        // Count through the selected items in the ListView, until
                        // we reach the 'index'th selected item.
                        //
                        int fidx = -1;
                        for (int count = 0; count <= index; count++)
                        {
                            fidx = unchecked((int)(long)User32.SendMessageW(owner, (User32.WM)LVM.GETNEXTITEM, (IntPtr)fidx, (IntPtr)LVNI.SELECTED));
                            Debug.Assert(fidx != -1, "Invalid index returned from LVM_GETNEXTITEM");
                        }

                        return owner.Items[fidx];
                    }
                    else
                    {
                        Debug.Assert(owner.savedSelectedItems is not null, "Null selected items collection");
                        return owner.savedSelectedItems[index];
                    }
                }
            }

            /// <summary>
            ///  Retrieves the child control with the specified key.
            /// </summary>
            public virtual ListViewItem this[string key]
            {
                get
                {
                    if (owner.VirtualMode)
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

            object IList.this[int index]
            {
                get
                {
                    if (owner.VirtualMode)
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

            int IList.Add(object value)
            {
                // SelectedListViewItemCollection is read-only
                throw new NotSupportedException();
            }

            void IList.Insert(int index, object value)
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

            void IList.Remove(object value)
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
                if (owner.VirtualMode)
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
            public virtual bool ContainsKey(string key)
            {
                if (owner.VirtualMode)
                {
                    throw new InvalidOperationException(SR.ListViewCantAccessSelectedItemsCollectionWhenInVirtualMode);
                }

                return IsValidIndex(IndexOfKey(key));
            }

            public bool Contains(ListViewItem item)
            {
                if (owner.VirtualMode)
                {
                    throw new InvalidOperationException(SR.ListViewCantAccessSelectedItemsCollectionWhenInVirtualMode);
                }

                return IndexOf(item) != -1;
            }

            bool IList.Contains(object item)
            {
                if (owner.VirtualMode)
                {
                    throw new InvalidOperationException(SR.ListViewCantAccessSelectedItemsCollectionWhenInVirtualMode);
                }

                if (item is ListViewItem)
                {
                    return Contains((ListViewItem)item);
                }
                else
                {
                    return false;
                }
            }

            public void CopyTo(Array dest, int index)
            {
                if (owner.VirtualMode)
                {
                    throw new InvalidOperationException(SR.ListViewCantAccessSelectedItemsCollectionWhenInVirtualMode);
                }

                if (Count > 0)
                {
                    System.Array.Copy(SelectedItemArray, 0, dest, index, Count);
                }
            }

            public IEnumerator GetEnumerator()
            {
                if (owner.VirtualMode)
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

            public int IndexOf(ListViewItem item)
            {
                if (owner.VirtualMode)
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

            int IList.IndexOf(object item)
            {
                if (owner.VirtualMode)
                {
                    throw new InvalidOperationException(SR.ListViewCantAccessSelectedItemsCollectionWhenInVirtualMode);
                }

                if (item is ListViewItem)
                {
                    return IndexOf((ListViewItem)item);
                }
                else
                {
                    return -1;
                }
            }

            /// <summary>
            ///  The zero-based index of the first occurrence of value within the entire CollectionBase, if found; otherwise, -1.
            /// </summary>
            public virtual int IndexOfKey(string key)
            {
                if (owner.VirtualMode)
                {
                    throw new InvalidOperationException(SR.ListViewCantAccessSelectedItemsCollectionWhenInVirtualMode);
                }

                // Step 0 - Arg validation
                if (string.IsNullOrEmpty(key))
                {
                    return -1; // we don't support empty or null keys.
                }

                // step 1 - check the last cached item
                if (IsValidIndex(lastAccessedIndex))
                {
                    if (WindowsFormsUtils.SafeCompareStrings(this[lastAccessedIndex].Name, key, /* ignoreCase = */ true))
                    {
                        return lastAccessedIndex;
                    }
                }

                // step 2 - search for the item
                for (int i = 0; i < Count; i++)
                {
                    if (WindowsFormsUtils.SafeCompareStrings(this[i].Name, key, /* ignoreCase = */ true))
                    {
                        lastAccessedIndex = i;
                        return i;
                    }
                }

                // step 3 - we didn't find it.  Invalidate the last accessed index and return -1.
                lastAccessedIndex = -1;
                return -1;
            }
        }
    }
}
