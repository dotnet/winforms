﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable disable

using System.Collections;
using System.Diagnostics;
using System.Globalization;
using static Interop;
using static Interop.ComCtl32;

namespace System.Windows.Forms
{
    public partial class ListView
    {
        // Overrides ListViewItemCollection methods and properties to automatically update
        // the native listview.
        //
        internal class ListViewNativeItemCollection : ListViewItemCollection.IInnerList
        {
            private readonly ListView owner;

            public ListViewNativeItemCollection(ListView owner)
            {
                this.owner = owner;
            }

            public int Count
            {
                get
                {
                    owner.ApplyUpdateCachedItems();
                    if (owner.VirtualMode)
                    {
                        return owner.VirtualListSize;
                    }
                    else
                    {
                        return owner.itemCount;
                    }
                }
            }

            public bool OwnerIsVirtualListView
            {
                get
                {
                    return owner.VirtualMode;
                }
            }

            public bool OwnerIsDesignMode
            {
                get
                {
                    return owner.DesignMode;
                }
            }

            public ListViewItem this[int displayIndex]
            {
                get
                {
                    owner.ApplyUpdateCachedItems();

                    if (owner.VirtualMode)
                    {
                        // if we are showing virtual items, we need to get the item from the user
                        RetrieveVirtualItemEventArgs rVI = new RetrieveVirtualItemEventArgs(displayIndex);
                        owner.OnRetrieveVirtualItem(rVI);
                        rVI.Item.SetItemIndex(owner, displayIndex);
                        return rVI.Item;
                    }
                    else
                    {
                        if (displayIndex < 0 || displayIndex >= owner.itemCount)
                        {
                            throw new ArgumentOutOfRangeException(nameof(displayIndex), displayIndex, string.Format(SR.InvalidArgument, nameof(displayIndex), displayIndex));
                        }

                        if (owner.IsHandleCreated && !owner.ListViewHandleDestroyed)
                        {
                            return (ListViewItem)owner.listItemsTable[DisplayIndexToID(displayIndex)];
                        }
                        else
                        {
                            Debug.Assert(owner.listItemsArray != null, "listItemsArray is null, but the handle isn't created");
                            return (ListViewItem)owner.listItemsArray[displayIndex];
                        }
                    }
                }
                set
                {
                    owner.ApplyUpdateCachedItems();
                    if (owner.VirtualMode)
                    {
                        throw new InvalidOperationException(SR.ListViewCantModifyTheItemCollInAVirtualListView);
                    }

                    if (displayIndex < 0 || displayIndex >= owner.itemCount)
                    {
                        throw new ArgumentOutOfRangeException(nameof(displayIndex), displayIndex, string.Format(SR.InvalidArgument, nameof(displayIndex), displayIndex));
                    }

                    if (owner.ExpectingMouseUp)
                    {
                        owner.ItemCollectionChangedInMouseDown = true;
                    }

                    RemoveAt(displayIndex);
                    Insert(displayIndex, value);
                }
            }

            public ListViewItem Add(ListViewItem value)
            {
                if (owner.VirtualMode)
                {
                    throw new InvalidOperationException(SR.ListViewCantAddItemsToAVirtualListView);
                }
                else
                {
                    Debug.Assert(!owner.FlipViewToLargeIconAndSmallIcon || Count == 0, "the FlipView... bit is turned off after adding 1 item.");

                    // PERF.
                    // Get the Checked bit before adding it to the back end.
                    // This saves a call into NativeListView to retrieve the real index.
                    bool valueChecked = value.Checked;

                    owner.InsertItems(owner.itemCount, new ListViewItem[] { value }, true);

                    if (owner.IsHandleCreated && !owner.CheckBoxes && valueChecked)
                    {
                        owner.UpdateSavedCheckedItems(value, true /*addItem*/);
                    }

                    if (owner.ExpectingMouseUp)
                    {
                        owner.ItemCollectionChangedInMouseDown = true;
                    }

                    return value;
                }
            }

            public void AddRange(ListViewItem[] values)
            {
                if (values is null)
                {
                    throw new ArgumentNullException(nameof(values));
                }

                if (owner.VirtualMode)
                {
                    throw new InvalidOperationException(SR.ListViewCantAddItemsToAVirtualListView);
                }

                IComparer comparer = owner.listItemSorter;
                owner.listItemSorter = null;

                Debug.Assert(!owner.FlipViewToLargeIconAndSmallIcon || Count == 0, "the FlipView... bit is turned off after adding 1 item.");

                bool[] checkedValues = null;

                if (owner.IsHandleCreated && !owner.CheckBoxes)
                {
                    // PERF.
                    // Cache the Checked bit before adding the item to the list view.
                    // This saves a bunch of calls to native list view when we want to UpdateSavedCheckedItems.
                    //
                    checkedValues = new bool[values.Length];
                    for (int i = 0; i < values.Length; i++)
                    {
                        checkedValues[i] = values[i].Checked;
                    }
                }

                try
                {
                    owner.BeginUpdate();
                    owner.InsertItems(owner.itemCount, values, true);

                    if (owner.IsHandleCreated && !owner.CheckBoxes)
                    {
                        for (int i = 0; i < values.Length; i++)
                        {
                            if (checkedValues[i])
                            {
                                owner.UpdateSavedCheckedItems(values[i], true /*addItem*/);
                            }
                        }
                    }
                }
                finally
                {
                    owner.listItemSorter = comparer;
                    owner.EndUpdate();
                }

                if (owner.ExpectingMouseUp)
                {
                    owner.ItemCollectionChangedInMouseDown = true;
                }

#pragma warning disable SA1408 // Conditional expressions should declare precedence
                if (comparer != null || (owner.Sorting != SortOrder.None) && !owner.VirtualMode)
#pragma warning restore SA1408 // Conditional expressions should declare precedence
                {
                    owner.Sort();
                }
            }

            private int DisplayIndexToID(int displayIndex)
            {
                Debug.Assert(!owner.VirtualMode, "in virtual mode, this method does not make any sense");
                if (owner.IsHandleCreated && !owner.ListViewHandleDestroyed)
                {
                    // Obtain internal index of the item
                    var lvItem = new LVITEMW
                    {
                        mask = LVIF.PARAM,
                        iItem = displayIndex
                    };
                    User32.SendMessageW(owner, (User32.WM)LVM.GETITEMW, IntPtr.Zero, ref lvItem);
                    return (int)lvItem.lParam;
                }
                else
                {
                    return this[displayIndex].ID;
                }
            }

            public void Clear()
            {
                if (owner.itemCount > 0)
                {
                    owner.ApplyUpdateCachedItems();

                    if (owner.IsHandleCreated && !owner.ListViewHandleDestroyed)
                    {
                        // walk the items to see which ones are selected.
                        // we use the LVM_GETNEXTITEM message to see what the next selected item is
                        // so we can avoid checking selection for each one.
                        //
                        int count = owner.Items.Count;
                        int nextSelected = (int)User32.SendMessageW(owner, (User32.WM)LVM.GETNEXTITEM, (IntPtr)(-1), (IntPtr)LVNI.SELECTED);
                        for (int i = 0; i < count; i++)
                        {
                            ListViewItem item = owner.Items[i];
                            Debug.Assert(item != null, "Failed to get item at index " + i.ToString(CultureInfo.InvariantCulture));
                            if (item != null)
                            {
                                // if it's the one we're looking for, ask for the next one
                                //
                                if (i == nextSelected)
                                {
                                    item.StateSelected = true;
                                    nextSelected = (int)User32.SendMessageW(owner, (User32.WM)LVM.GETNEXTITEM, (IntPtr)nextSelected, (IntPtr)LVNI.SELECTED);
                                }
                                else
                                {
                                    // otherwise it's false
                                    //
                                    item.StateSelected = false;
                                }
                                item.UnHost(i, false);
                            }
                        }
                        Debug.Assert(owner.listItemsArray is null, "listItemsArray not null, even though handle created");

                        User32.SendMessageW(owner, (User32.WM)LVM.DELETEALLITEMS);

                        // There's a problem in the list view that if it's in small icon, it won't pick upo the small icon
                        // sizes until it changes from large icon, so we flip it twice here...
                        //
                        if (owner.View == View.SmallIcon)
                        {
                            if (Application.ComCtlSupportsVisualStyles)
                            {
                                owner.FlipViewToLargeIconAndSmallIcon = true;
                            }
                            else
                            {
                                Debug.Assert(!owner.FlipViewToLargeIconAndSmallIcon, "we only set this when comctl 6.0 is loaded");
                                owner.View = View.LargeIcon;
                                owner.View = View.SmallIcon;
                            }
                        }
                    }
                    else
                    {
                        int count = owner.Items.Count;

                        for (int i = 0; i < count; i++)
                        {
                            ListViewItem item = owner.Items[i];
                            if (item != null)
                            {
                                item.UnHost(i, true);
                            }
                        }

                        Debug.Assert(owner.listItemsArray != null, "listItemsArray is null, but the handle isn't created");
                        owner.listItemsArray.Clear();
                    }

                    owner.listItemsTable.Clear();
                    if (owner.IsHandleCreated && !owner.CheckBoxes)
                    {
                        owner.savedCheckedItems = null;
                    }
                    owner.itemCount = 0;

                    if (owner.ExpectingMouseUp)
                    {
                        owner.ItemCollectionChangedInMouseDown = true;
                    }
                }
            }

            public bool Contains(ListViewItem item)
            {
                owner.ApplyUpdateCachedItems();
                if (owner.IsHandleCreated && !owner.ListViewHandleDestroyed)
                {
                    return owner.listItemsTable[item.ID] == item;
                }
                else
                {
                    Debug.Assert(owner.listItemsArray != null, "listItemsArray is null, but the handle isn't created");
                    return owner.listItemsArray.Contains(item);
                }
            }

            public ListViewItem Insert(int index, ListViewItem item)
            {
                int count = 0;
                if (owner.VirtualMode)
                {
                    count = Count;
                }
                else
                {
                    count = owner.itemCount;
                }

                if (index < 0 || index > count)
                {
                    throw new ArgumentOutOfRangeException(nameof(index), index, string.Format(SR.InvalidArgument, nameof(index), index));
                }

                if (owner.VirtualMode)
                {
                    throw new InvalidOperationException(SR.ListViewCantAddItemsToAVirtualListView);
                }

                Debug.Assert(!owner.FlipViewToLargeIconAndSmallIcon || Count == 0, "the FlipView... bit is turned off after adding 1 item.");

                if (index < count)
                {
                    // if we're not inserting at the end, force the add.
                    //
                    owner.ApplyUpdateCachedItems();
                }

                owner.InsertItems(index, new ListViewItem[] { item }, true);
                if (owner.IsHandleCreated && !owner.CheckBoxes && item.Checked)
                {
                    owner.UpdateSavedCheckedItems(item, true /*addItem*/);
                }

                if (owner.ExpectingMouseUp)
                {
                    owner.ItemCollectionChangedInMouseDown = true;
                }

                return item;
            }

            public int IndexOf(ListViewItem item)
            {
                Debug.Assert(!owner.VirtualMode, "in virtual mode, this function does not make any sense");
                for (int i = 0; i < Count; i++)
                {
                    if (item == this[i])
                    {
                        return i;
                    }
                }
                return -1;
            }

            public void Remove(ListViewItem item)
            {
                int index = owner.VirtualMode ? Count - 1 : IndexOf(item);

                Debug.Assert(!owner.FlipViewToLargeIconAndSmallIcon || Count == 0, "the FlipView... bit is turned off after adding 1 item.");

                if (owner.VirtualMode)
                {
                    throw new InvalidOperationException(SR.ListViewCantRemoveItemsFromAVirtualListView);
                }

                if (index != -1)
                {
                    RemoveAt(index);
                }
            }

            public void RemoveAt(int index)
            {
                if (owner.VirtualMode)
                {
                    throw new InvalidOperationException(SR.ListViewCantRemoveItemsFromAVirtualListView);
                }

                if (index < 0 || index >= owner.itemCount)
                {
                    throw new ArgumentOutOfRangeException(nameof(index), index, string.Format(SR.InvalidArgument, nameof(index), index));
                }

                Debug.Assert(!owner.FlipViewToLargeIconAndSmallIcon || Count == 0, "the FlipView... bit is turned off after adding 1 item.");

                if (owner.IsHandleCreated && !owner.CheckBoxes && this[index].Checked)
                {
                    owner.UpdateSavedCheckedItems(this[index], false /*addItem*/);
                }

                owner.ApplyUpdateCachedItems();
                int itemID = DisplayIndexToID(index);
                this[index].UnHost(true);

                if (owner.IsHandleCreated)
                {
                    Debug.Assert(owner.listItemsArray is null, "listItemsArray not null, even though handle created");
                    int retval = unchecked((int)(long)User32.SendMessageW(owner, (User32.WM)LVM.DELETEITEM, (IntPtr)index));

                    if (0 == retval)
                    {
                        throw new ArgumentOutOfRangeException(nameof(index), index, string.Format(SR.InvalidArgument, nameof(index), index));
                    }
                }
                else
                {
                    Debug.Assert(owner.listItemsArray != null, "listItemsArray is null, but the handle isn't created");
                    owner.listItemsArray.RemoveAt(index);
                }

                owner.itemCount--;
                owner.listItemsTable.Remove(itemID);

                if (owner.ExpectingMouseUp)
                {
                    owner.ItemCollectionChangedInMouseDown = true;
                }
            }

            public void CopyTo(Array dest, int index)
            {
                if (owner.itemCount > 0)
                {
                    for (int displayIndex = 0; displayIndex < Count; ++displayIndex)
                    {
                        dest.SetValue(this[displayIndex], index++);
                    }
                }
            }

            public IEnumerator GetEnumerator()
            {
                ListViewItem[] items = new ListViewItem[owner.itemCount];
                CopyTo(items, 0);

                return items.GetEnumerator();
            }
        }
    }
}
