﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable disable

using System.Collections;
using System.Diagnostics;
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
            private readonly ListView _owner;

            public ListViewNativeItemCollection(ListView owner)
            {
                _owner = owner;
            }

            public int Count
            {
                get
                {
                    _owner.ApplyUpdateCachedItems();
                    if (_owner.VirtualMode)
                    {
                        return _owner.VirtualListSize;
                    }
                    else
                    {
                        return _owner._itemCount;
                    }
                }
            }

            public bool OwnerIsVirtualListView
            {
                get
                {
                    return _owner.VirtualMode;
                }
            }

            public bool OwnerIsDesignMode
            {
                get
                {
                    return _owner.DesignMode;
                }
            }

            public ListViewItem this[int displayIndex]
            {
                get
                {
                    _owner.ApplyUpdateCachedItems();

                    if (_owner.VirtualMode)
                    {
                        // if we are showing virtual items, we need to get the item from the user
                        RetrieveVirtualItemEventArgs rVI = new RetrieveVirtualItemEventArgs(displayIndex);
                        _owner.OnRetrieveVirtualItem(rVI);
                        rVI.Item!.SetItemIndex(_owner, displayIndex);
                        return rVI.Item;
                    }
                    else
                    {
                        if (displayIndex < 0 || displayIndex >= _owner._itemCount)
                        {
                            throw new ArgumentOutOfRangeException(nameof(displayIndex), displayIndex, string.Format(SR.InvalidArgument, nameof(displayIndex), displayIndex));
                        }

                        if (_owner.IsHandleCreated && !_owner.ListViewHandleDestroyed)
                        {
                            return (ListViewItem)_owner._listItemsTable[DisplayIndexToID(displayIndex)];
                        }
                        else
                        {
                            Debug.Assert(_owner._listViewItems is not null, "listItemsArray is null, but the handle isn't created");
                            return _owner._listViewItems[displayIndex];
                        }
                    }
                }
                set
                {
                    _owner.ApplyUpdateCachedItems();
                    if (_owner.VirtualMode)
                    {
                        throw new InvalidOperationException(SR.ListViewCantModifyTheItemCollInAVirtualListView);
                    }

                    if (displayIndex < 0 || displayIndex >= _owner._itemCount)
                    {
                        throw new ArgumentOutOfRangeException(nameof(displayIndex), displayIndex, string.Format(SR.InvalidArgument, nameof(displayIndex), displayIndex));
                    }

                    if (_owner.ExpectingMouseUp)
                    {
                        _owner.ItemCollectionChangedInMouseDown = true;
                    }

                    RemoveAt(displayIndex);
                    Insert(displayIndex, value);
                }
            }

            public ListViewItem Add(ListViewItem value)
            {
                if (_owner.VirtualMode)
                {
                    throw new InvalidOperationException(SR.ListViewCantAddItemsToAVirtualListView);
                }
                else
                {
                    Debug.Assert(!_owner.FlipViewToLargeIconAndSmallIcon || Count == 0, "the FlipView... bit is turned off after adding 1 item.");

                    // PERF.
                    // Get the Checked bit before adding it to the back end.
                    // This saves a call into NativeListView to retrieve the real index.
                    bool valueChecked = value.Checked;

                    _owner.InsertItems(_owner._itemCount, new ListViewItem[] { value }, true);

                    if (_owner.IsHandleCreated && !_owner.CheckBoxes && valueChecked)
                    {
                        _owner.UpdateSavedCheckedItems(value, true /*addItem*/);
                    }

                    if (_owner.ExpectingMouseUp)
                    {
                        _owner.ItemCollectionChangedInMouseDown = true;
                    }

                    return value;
                }
            }

            public void AddRange(ListViewItem[] values)
            {
                ArgumentNullException.ThrowIfNull(values);

                if (_owner.VirtualMode)
                {
                    throw new InvalidOperationException(SR.ListViewCantAddItemsToAVirtualListView);
                }

                IComparer comparer = _owner._listItemSorter;
                _owner._listItemSorter = null;

                Debug.Assert(!_owner.FlipViewToLargeIconAndSmallIcon || Count == 0, "the FlipView... bit is turned off after adding 1 item.");

                bool[] checkedValues = null;

                if (_owner.IsHandleCreated && !_owner.CheckBoxes)
                {
                    // PERF.
                    // Cache the Checked bit before adding the item to the list view.
                    // This saves a bunch of calls to native list view when we want to UpdateSavedCheckedItems.
                    checkedValues = new bool[values.Length];
                    for (int i = 0; i < values.Length; i++)
                    {
                        checkedValues[i] = values[i].Checked;
                    }
                }

                try
                {
                    _owner.BeginUpdate();
                    _owner.InsertItems(_owner._itemCount, values, true);

                    if (_owner.IsHandleCreated && !_owner.CheckBoxes)
                    {
                        for (int i = 0; i < values.Length; i++)
                        {
                            if (checkedValues![i])
                            {
                                _owner.UpdateSavedCheckedItems(values[i], true /*addItem*/);
                            }
                        }
                    }
                }
                finally
                {
                    _owner._listItemSorter = comparer;
                    _owner.EndUpdate();
                }

                if (_owner.ExpectingMouseUp)
                {
                    _owner.ItemCollectionChangedInMouseDown = true;
                }

                if (comparer is not null ||
                    ((_owner.Sorting != SortOrder.None) && !_owner.VirtualMode))
                {
                    _owner.Sort();
                }
            }

            private int DisplayIndexToID(int displayIndex)
            {
                Debug.Assert(!_owner.VirtualMode, "in virtual mode, this method does not make any sense");
                if (_owner.IsHandleCreated && !_owner.ListViewHandleDestroyed)
                {
                    // Obtain internal index of the item
                    var lvItem = new LVITEMW
                    {
                        mask = LVIF.PARAM,
                        iItem = displayIndex
                    };

                    User32.SendMessageW(_owner, (User32.WM)LVM.GETITEMW, 0, ref lvItem);
                    return PARAM.ToInt(lvItem.lParam);
                }
                else
                {
                    return this[displayIndex].ID;
                }
            }

            public void Clear()
            {
                if (_owner._itemCount <= 0)
                {
                    return;
                }

                _owner.ApplyUpdateCachedItems();

                if (_owner.IsHandleCreated && !_owner.ListViewHandleDestroyed)
                {
                    // Walk the items to see which ones are selected.
                    // We use the LVM_GETNEXTITEM message to see what the next selected item is
                    // so we can avoid checking selection for each one.
                    int count = _owner.Items.Count;
                    int nextSelected = (int)User32.SendMessageW(_owner, (User32.WM)LVM.GETNEXTITEM, -1, (nint)LVNI.SELECTED);
                    for (int i = 0; i < count; i++)
                    {
                        ListViewItem item = _owner.Items[i];
                        Debug.Assert(item is not null, $"Failed to get item at index {i}");
                        if (item is not null)
                        {
                            // If it's the one we're looking for, ask for the next one.
                            if (i == nextSelected)
                            {
                                item.StateSelected = true;
                                nextSelected = (int)User32.SendMessageW(_owner, (User32.WM)LVM.GETNEXTITEM, nextSelected, (nint)LVNI.SELECTED);
                            }
                            else
                            {
                                // Otherwise it's false.
                                item.StateSelected = false;
                            }

                            item.UnHost(i, false);
                        }
                    }

                    Debug.Assert(_owner._listViewItems is null, "listItemsArray not null, even though handle created");

                    User32.SendMessageW(_owner, (User32.WM)LVM.DELETEALLITEMS);

                    // There's a problem in the list view that if it's in small icon, it won't pick up the small icon
                    // sizes until it changes from large icon, so we flip it twice here...
                    if (_owner.View == View.SmallIcon)
                    {
                        if (Application.ComCtlSupportsVisualStyles)
                        {
                            _owner.FlipViewToLargeIconAndSmallIcon = true;
                        }
                        else
                        {
                            Debug.Assert(!_owner.FlipViewToLargeIconAndSmallIcon, "we only set this when comctl 6.0 is loaded");
                            _owner.View = View.LargeIcon;
                            _owner.View = View.SmallIcon;
                        }
                    }
                }
                else
                {
                    int count = _owner.Items.Count;

                    for (int i = 0; i < count; i++)
                    {
                        ListViewItem item = _owner.Items[i];
                        if (item is not null)
                        {
                            item.UnHost(i, true);
                        }
                    }

                    Debug.Assert(_owner._listViewItems is not null, "listItemsArray is null, but the handle isn't created");
                    _owner._listViewItems.Clear();
                }

                _owner._listItemsTable.Clear();
                if (_owner.IsHandleCreated && !_owner.CheckBoxes)
                {
                    _owner._savedCheckedItems = null;
                }

                _owner._itemCount = 0;

                if (_owner.ExpectingMouseUp)
                {
                    _owner.ItemCollectionChangedInMouseDown = true;
                }
            }

            public bool Contains(ListViewItem item)
            {
                _owner.ApplyUpdateCachedItems();
                if (_owner.IsHandleCreated && !_owner.ListViewHandleDestroyed)
                {
                    return _owner._listItemsTable[item.ID] == item;
                }
                else
                {
                    Debug.Assert(_owner._listViewItems is not null, "listItemsArray is null, but the handle isn't created");
                    return _owner._listViewItems.Contains(item);
                }
            }

            public ListViewItem Insert(int index, ListViewItem item)
            {
                int count = 0;
                if (_owner.VirtualMode)
                {
                    count = Count;
                }
                else
                {
                    count = _owner._itemCount;
                }

                if (index < 0 || index > count)
                {
                    throw new ArgumentOutOfRangeException(nameof(index), index, string.Format(SR.InvalidArgument, nameof(index), index));
                }

                if (_owner.VirtualMode)
                {
                    throw new InvalidOperationException(SR.ListViewCantAddItemsToAVirtualListView);
                }

                Debug.Assert(!_owner.FlipViewToLargeIconAndSmallIcon || Count == 0, "the FlipView... bit is turned off after adding 1 item.");

                if (index < count)
                {
                    // if we're not inserting at the end, force the add.
                    _owner.ApplyUpdateCachedItems();
                }

                _owner.InsertItems(index, new ListViewItem[] { item }, true);
                if (_owner.IsHandleCreated && !_owner.CheckBoxes && item.Checked)
                {
                    _owner.UpdateSavedCheckedItems(item, true /*addItem*/);
                }

                if (_owner.ExpectingMouseUp)
                {
                    _owner.ItemCollectionChangedInMouseDown = true;
                }

                return item;
            }

            public int IndexOf(ListViewItem item)
            {
                Debug.Assert(!_owner.VirtualMode, "in virtual mode, this function does not make any sense");
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
                int index = _owner.VirtualMode ? Count - 1 : IndexOf(item);

                Debug.Assert(!_owner.FlipViewToLargeIconAndSmallIcon || Count == 0, "the FlipView... bit is turned off after adding 1 item.");

                if (_owner.VirtualMode)
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
                if (_owner.VirtualMode)
                {
                    throw new InvalidOperationException(SR.ListViewCantRemoveItemsFromAVirtualListView);
                }

                if (index < 0 || index >= _owner._itemCount)
                {
                    throw new ArgumentOutOfRangeException(nameof(index), index, string.Format(SR.InvalidArgument, nameof(index), index));
                }

                Debug.Assert(!_owner.FlipViewToLargeIconAndSmallIcon || Count == 0, "the FlipView... bit is turned off after adding 1 item.");

                if (_owner.IsHandleCreated && !_owner.CheckBoxes && this[index].Checked)
                {
                    _owner.UpdateSavedCheckedItems(this[index], addItem: false);
                }

                _owner.ApplyUpdateCachedItems();
                int itemID = DisplayIndexToID(index);

                this[index].Focused = false;
                this[index].UnHost(true);

                if (_owner.IsHandleCreated)
                {
                    Debug.Assert(_owner._listViewItems is null, "listItemsArray not null, even though handle created");
                    if (User32.SendMessageW(_owner, (User32.WM)LVM.DELETEITEM, index) == 0)
                    {
                        throw new ArgumentOutOfRangeException(nameof(index), index, string.Format(SR.InvalidArgument, nameof(index), index));
                    }
                }
                else
                {
                    Debug.Assert(_owner._listViewItems is not null, "listItemsArray is null, but the handle isn't created");
                    _owner._listViewItems.RemoveAt(index);
                }

                _owner._itemCount--;
                _owner._listItemsTable.Remove(itemID);

                if (_owner.ExpectingMouseUp)
                {
                    _owner.ItemCollectionChangedInMouseDown = true;
                }
            }

            public void CopyTo(Array dest, int index)
            {
                if (_owner._itemCount > 0)
                {
                    for (int displayIndex = 0; displayIndex < Count; ++displayIndex)
                    {
                        dest.SetValue(this[displayIndex], index++);
                    }
                }
            }

            public IEnumerator GetEnumerator()
            {
                ListViewItem[] items = new ListViewItem[_owner._itemCount];
                CopyTo(items, 0);

                return items.GetEnumerator();
            }
        }
    }
}
