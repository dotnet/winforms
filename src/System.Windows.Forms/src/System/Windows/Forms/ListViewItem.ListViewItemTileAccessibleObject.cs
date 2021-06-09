// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.InteropServices;
using Accessibility;
using static System.Windows.Forms.ListView;
using static System.Windows.Forms.ListViewGroup;
using static Interop;

namespace System.Windows.Forms
{
    public partial class ListViewItem
    {
        internal class ListViewItemTileAccessibleObject : ListViewItemBaseAccessibleObject
        {
            public ListViewItemTileAccessibleObject(ListViewItem owningItem) : base(owningItem)
            {
            }

            internal override UiaCore.IRawElementProviderFragment? FragmentNavigate(UiaCore.NavigateDirection direction)
            {
                switch (direction)
                {
                    case UiaCore.NavigateDirection.FirstChild:
                        return GetChildInternal(1);
                    case UiaCore.NavigateDirection.LastChild:
                        return GetChildInternal(GetLastChildIndex());
                }

                return base.FragmentNavigate(direction);
            }

            // Only additional ListViewSubItem are displayed in the accessibility tree if the ListView
            // in the "Tile" view (the first ListViewSubItem is responsible for the ListViewItem)
            public override AccessibleObject? GetChild(int index)
            {
                if (_owningListView.View != View.Tile)
                {
                    throw new InvalidOperationException(string.Format(SR.ListViewItemAccessibilityObjectInvalidViewException, nameof(View.Tile)));
                }

                return GetChildInternal(index + 1);
            }

            internal override AccessibleObject? GetChildInternal(int index)
            {
                // If the ListView does not support ListViewSubItems, the index is greater than the number of columns
                // or the index is negative, then we return null
                if (!_owningListView.SupportsListViewSubItems
                    || index <= 0
                    || _owningListView.Columns.Count <= index
                    || _owningItem.SubItems.Count <= index
                    || GetSubItemBounds(index).IsEmpty)
                {
                    return null;
                }

                return _owningItem.SubItems[index].AccessibilityObject;
            }

            public override int GetChildCount()
            {
                if (_owningListView.View != View.Tile)
                {
                    throw new InvalidOperationException(string.Format(SR.ListViewItemAccessibilityObjectInvalidViewException, nameof(View.Tile)));
                }

                if (!_owningListView.IsHandleCreated || !_owningListView.SupportsListViewSubItems)
                {
                    return -1;
                }

                if (_owningItem.SubItems.Count == 1)
                {
                    return 0;
                }

                return GetLastChildIndex();
            }

            internal override int GetChildIndex(AccessibleObject? child)
            {
                if (child is null
                    || !_owningListView.SupportsListViewSubItems
                    || child == _owningItem.SubItems[0].AccessibilityObject
                    || child is not ListViewSubItem.ListViewSubItemAccessibleObject subItemAccessibleObject
                    || subItemAccessibleObject.OwningSubItem is null)
                {
                    return -1;
                }

                int index = _owningItem.SubItems.IndexOf(subItemAccessibleObject.OwningSubItem);
                return index == -1 || index > GetLastChildIndex() ? -1 : index;
            }

            private int GetLastChildIndex()
            {
                // Data about the first ListViewSubItem is displayed in the ListViewItem.
                // Therefore, it is not displayed in the ListViewSubItems list
                if (_owningItem.SubItems.Count == 1)
                {
                    return -1;
                }

                // Only ListViewSubItems with the corresponding columns are displayed in the ListView
                int subItemCount = Math.Min(_owningListView.Columns.Count, _owningItem.SubItems.Count);

                // The ListView can be of limited TileSize, so some of the ListViewSubItems can be hidden.
                // sListViewSubItems that do not have enough space to display have an empty bounds
                for (int i = 1; i < subItemCount; i++)
                {
                    if (GetSubItemBounds(i).IsEmpty)
                    {
                        return i - 1;
                    }
                }

                return subItemCount - 1;
            }

            internal override Rectangle GetSubItemBounds(int subItemIndex)
                => _owningListView.IsHandleCreated
                    ? _owningListView.GetSubItemRect(_owningItem.Index, subItemIndex)
                    : Rectangle.Empty;
        }
    }
}
