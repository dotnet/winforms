// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;
using Windows.Win32.UI.Accessibility;

namespace System.Windows.Forms;

public partial class ListViewItem
{
    internal sealed class ListViewItemTileAccessibleObject : ListViewItemBaseAccessibleObject
    {
        private ListViewLabelEditAccessibleObject? _labelEditAccessibleObject;
        public ListViewItemTileAccessibleObject(ListViewItem owningItem) : base(owningItem)
        {
        }

        protected override View View => View.Tile;

        private ListViewLabelEditAccessibleObject? LabelEditAccessibleObject
            => _labelEditAccessibleObject ??= _owningListView._labelEdit is null
                ? null
                : new(_owningListView, _owningListView._labelEdit);

        internal override IRawElementProviderFragment.Interface? FragmentNavigate(NavigateDirection direction)
            => direction switch
            {
                NavigateDirection.NavigateDirection_FirstChild
                    => _owningListView._labelEdit is not null
                        ? LabelEditAccessibleObject
                        : GetChildInternal(1),
                NavigateDirection.NavigateDirection_LastChild => GetChildInternal(GetLastChildIndex()),
                _ => base.FragmentNavigate(direction),
            };

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
                return InvalidIndex;
            }

            if (_owningItem.SubItems.Count == 1)
            {
                return _owningListView._labelEdit is not null ? 1 : 0;
            }

            return _owningListView._labelEdit is not null ? GetLastChildIndex() + 1 : GetLastChildIndex();
        }

        internal override int GetChildIndex(AccessibleObject? child)
        {
            if (child is null
                || !_owningListView.SupportsListViewSubItems
                || child == _owningItem.SubItems[0].AccessibilityObject
                || child is not ListViewSubItem.ListViewSubItemAccessibleObject subItemAccessibleObject
                || subItemAccessibleObject.OwningSubItem is null)
            {
                return InvalidIndex;
            }

            int index = _owningItem.SubItems.IndexOf(subItemAccessibleObject.OwningSubItem);
            return index == -1 || index > GetLastChildIndex() ? InvalidIndex : index;
        }

        private int GetLastChildIndex()
        {
            // Data about the first ListViewSubItem is displayed in the ListViewItem.
            // Therefore, it is not displayed in the ListViewSubItems list
            if (_owningItem.SubItems.Count == 1)
            {
                return InvalidIndex;
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
