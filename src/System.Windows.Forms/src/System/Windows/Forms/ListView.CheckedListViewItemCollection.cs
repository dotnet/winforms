// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable disable

using System.Collections;
using System.ComponentModel;

namespace System.Windows.Forms
{
    public partial class ListView
    {
        [ListBindable(false)]
        public class CheckedListViewItemCollection : IList
        {
            private readonly ListView owner;

            ///  A caching mechanism for key accessor
            ///  We use an index here rather than control so that we don't have lifetime
            ///  issues by holding on to extra references.
            private int lastAccessedIndex = -1;

            /* C#r: protected */
            public CheckedListViewItemCollection(ListView owner)
            {
                this.owner = owner;
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
                        throw new InvalidOperationException(SR.ListViewCantAccessCheckedItemsCollectionWhenInVirtualMode);
                    }

                    return owner.CheckedIndices.Count;
                }
            }

            private ListViewItem[] ItemArray
            {
                get
                {
                    ListViewItem[] items = new ListViewItem[Count];
                    int index = 0;
                    for (int i = 0; i < owner.Items.Count && index < items.Length; ++i)
                    {
                        if (owner.Items[i].Checked)
                        {
                            items[index++] = owner.Items[i];
                        }
                    }

                    return items;
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
                        throw new InvalidOperationException(SR.ListViewCantAccessCheckedItemsCollectionWhenInVirtualMode);
                    }

                    int itemIndex = owner.CheckedIndices[index];
                    return owner.Items[itemIndex];
                }
            }

            object IList.this[int index]
            {
                get
                {
                    if (owner.VirtualMode)
                    {
                        throw new InvalidOperationException(SR.ListViewCantAccessCheckedItemsCollectionWhenInVirtualMode);
                    }

                    return this[index];
                }
                set
                {
                    throw new NotSupportedException();
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
                        throw new InvalidOperationException(SR.ListViewCantAccessCheckedItemsCollectionWhenInVirtualMode);
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

            public bool Contains(ListViewItem item)
            {
                if (owner.VirtualMode)
                {
                    throw new InvalidOperationException(SR.ListViewCantAccessCheckedItemsCollectionWhenInVirtualMode);
                }

                if (item is not null && item.ListView == owner && item.Checked)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }

            bool IList.Contains(object item)
            {
                if (owner.VirtualMode)
                {
                    throw new InvalidOperationException(SR.ListViewCantAccessCheckedItemsCollectionWhenInVirtualMode);
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

            /// <summary>
            ///  Returns true if the collection contains an item with the specified key, false otherwise.
            /// </summary>
            public virtual bool ContainsKey(string key)
            {
                if (owner.VirtualMode)
                {
                    throw new InvalidOperationException(SR.ListViewCantAccessCheckedItemsCollectionWhenInVirtualMode);
                }

                return IsValidIndex(IndexOfKey(key));
            }

            public int IndexOf(ListViewItem item)
            {
                if (owner.VirtualMode)
                {
                    throw new InvalidOperationException(SR.ListViewCantAccessCheckedItemsCollectionWhenInVirtualMode);
                }

                ListViewItem[] items = ItemArray;
                for (int index = 0; index < items.Length; ++index)
                {
                    if (items[index] == item)
                    {
                        return index;
                    }
                }

                return -1;
            }

            /// <summary>
            ///  The zero-based index of the first occurrence of value within the entire CollectionBase, if found; otherwise, -1.
            /// </summary>
            public virtual int IndexOfKey(string key)
            {
                if (owner.VirtualMode)
                {
                    throw new InvalidOperationException(SR.ListViewCantAccessCheckedItemsCollectionWhenInVirtualMode);
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

            /// <summary>
            ///  Determines if the index is valid for the collection.
            /// </summary>
            private bool IsValidIndex(int index)
            {
                return (index >= 0) && (index < Count);
            }

            int IList.IndexOf(object item)
            {
                if (owner.VirtualMode)
                {
                    throw new InvalidOperationException(SR.ListViewCantAccessCheckedItemsCollectionWhenInVirtualMode);
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

            int IList.Add(object value)
            {
                throw new NotSupportedException();
            }

            void IList.Clear()
            {
                throw new NotSupportedException();
            }

            void IList.Insert(int index, object value)
            {
                throw new NotSupportedException();
            }

            void IList.Remove(object value)
            {
                throw new NotSupportedException();
            }

            void IList.RemoveAt(int index)
            {
                throw new NotSupportedException();
            }

            public void CopyTo(Array dest, int index)
            {
                if (owner.VirtualMode)
                {
                    throw new InvalidOperationException(SR.ListViewCantAccessCheckedItemsCollectionWhenInVirtualMode);
                }

                if (Count > 0)
                {
                    System.Array.Copy(ItemArray, 0, dest, index, Count);
                }
            }

            public IEnumerator GetEnumerator()
            {
                if (owner.VirtualMode)
                {
                    throw new InvalidOperationException(SR.ListViewCantAccessCheckedItemsCollectionWhenInVirtualMode);
                }

                ListViewItem[] items = ItemArray;
                if (items is not null)
                {
                    return items.GetEnumerator();
                }
                else
                {
                    return Array.Empty<ListViewItem>().GetEnumerator();
                }
            }
        }
    }
}
