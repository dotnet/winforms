// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;
using Windows.Win32.System.Variant;
using Windows.Win32.UI.Accessibility;

namespace System.Windows.Forms;

public partial class ListView
{
    internal class ListViewAccessibleObject : ControlAccessibleObject
    {
        private static readonly int[] s_enumViewValues = (int[])Enum.GetValues(typeof(View));

        internal ListViewAccessibleObject(ListView owningListView) : base(owningListView)
        {
        }

        private protected override bool IsInternal => true;

        internal override Rectangle BoundingRectangle
        {
            get
            {
                if (this.IsOwnerHandleCreated(out ListView? owningListView))
                {
                    PInvokeCore.GetWindowRect(owningListView, out var rect);
                    return rect;
                }

                return Rectangle.Empty;
            }
        }

        internal override bool CanSelectMultiple
            => this.IsOwnerHandleCreated(out ListView? listView) && listView.MultiSelect;

        internal override int ColumnCount
            => this.TryGetOwnerAs(out ListView? owningListView) ? owningListView.Columns.Count : base.ColumnCount;

        internal bool OwnerHasDefaultGroup
        {
            get
            {
                if (!this.TryGetOwnerAs(out ListView? owningListView) || !owningListView.GroupsDisplayed)
                {
                    return false;
                }

                foreach (ListViewItem? item in owningListView.Items)
                {
                    // If there are groups in the ListView, then the items which do not belong
                    // to any of the group and have null as the item.Group value, so these items
                    // are put into the default group and thereby the ListView itself starts
                    // containing Default group.
                    if (item is not null && item.Group is null)
                    {
                        return true;
                    }
                }

                return false;
            }
        }

        internal override int RowCount
            => this.TryGetOwnerAs(out ListView? owningListView) ? owningListView.Items.Count : base.RowCount;

        internal override RowOrColumnMajor RowOrColumnMajor
            => RowOrColumnMajor.RowOrColumnMajor_RowMajor;

        internal override IRawElementProviderFragment.Interface? ElementProviderFromPoint(double x, double y)
        {
            AccessibleObject? element = HitTest((int)x, (int)y);

            return element ?? base.ElementProviderFromPoint(x, y);
        }

        internal override IRawElementProviderFragment.Interface? FragmentNavigate(NavigateDirection direction) =>
            !this.IsOwnerHandleCreated(out ListView? _)
                ? null
                : direction switch
                {
                    NavigateDirection.NavigateDirection_FirstChild => GetChild(0),
                    NavigateDirection.NavigateDirection_LastChild => GetLastChild(),
                    _ => base.FragmentNavigate(direction)
                };

        public override AccessibleObject? GetChild(int index)
        {
            if (!this.IsOwnerHandleCreated(out ListView? owningListView) || index < 0)
            {
                return null;
            }

            if (owningListView.GroupsDisplayed)
            {
                IReadOnlyList<ListViewGroup> visibleGroups = GetVisibleGroups();
                return index < visibleGroups.Count ? visibleGroups[index].AccessibilityObject : null;
            }

            return index < owningListView.Items.Count ? owningListView.Items[index].AccessibilityObject : null;
        }

        public override int GetChildCount()
        {
            if (!this.IsOwnerHandleCreated(out ListView? owningListView))
            {
                return InvalidIndex;
            }

            int count = owningListView.GroupsDisplayed ? GetVisibleGroups().Count : owningListView.Items.Count;

            return count;
        }

        private int GetItemIndex(AccessibleObject? child)
        {
            if (!this.TryGetOwnerAs(out ListView? owningListView) || child is null)
            {
                return InvalidIndex;
            }

            if (child is ListViewItem.ListViewItemBaseAccessibleObject itemAccessibleObject)
            {
                int index = itemAccessibleObject.CurrentIndex;
                return index < owningListView.Items.Count ? index : InvalidIndex;
            }

            return InvalidIndex;
        }

        private int GetGroupIndex(AccessibleObject? child)
        {
            if (child is null)
            {
                return InvalidIndex;
            }

            IReadOnlyList<ListViewGroup> visibleGroups = GetVisibleGroups();
            for (int i = 0; i < visibleGroups.Count; i++)
            {
                if (visibleGroups[i].AccessibilityObject == child)
                {
                    return i;
                }
            }

            return InvalidIndex;
        }

        internal override int GetChildIndex(AccessibleObject? child) => this.TryGetOwnerAs(out ListView? owningListView)
            ? owningListView.GroupsDisplayed ? GetGroupIndex(child) : GetItemIndex(child)
            : base.GetChildIndex(child);

        private string GetItemStatus() => this.TryGetOwnerAs(out ListView? owningListView)
            ? owningListView.Sorting switch
            {
                SortOrder.Ascending => SR.SortedAscendingAccessibleStatus,
                SortOrder.Descending => SR.SortedDescendingAccessibleStatus,
                _ => SR.NotSortedAccessibleStatus
            }
            : string.Empty;

        internal override IRawElementProviderSimple.Interface[]? GetColumnHeaders()
        {
            if (!this.TryGetOwnerAs(out ListView? owningListView))
            {
                return base.GetColumnHeaders();
            }

            var columnHeaders = new IRawElementProviderSimple.Interface[owningListView.Columns.Count];
            for (int i = 0; i < columnHeaders.Length; i++)
            {
                columnHeaders[i] = owningListView.Columns[i].AccessibilityObject;
            }

            return columnHeaders;
        }

        internal override IRawElementProviderFragment.Interface? GetFocus()
            => !this.IsOwnerHandleCreated(out ListView? owningListView)
                ? null
                : (owningListView.FocusedItem?.AccessibilityObject ?? owningListView.FocusedGroup?.AccessibilityObject);

        internal override int GetMultiViewProviderCurrentView()
            => this.TryGetOwnerAs(out ListView? owningListView) ? (int)owningListView.View : base.GetMultiViewProviderCurrentView();

        internal override int[] GetMultiViewProviderSupportedViews()
            => [(int)View.Details];

        internal override string GetMultiViewProviderViewName(int viewId)
        {
            foreach (int view in s_enumViewValues)
            {
                if (view == viewId)
                {
                    return view.ToString();
                }
            }

            return string.Empty;
        }

        private AccessibleObject? GetLastChild()
        {
            if (!this.TryGetOwnerAs(out ListView? owningListView))
            {
                return null;
            }

            if (owningListView.GroupsDisplayed)
            {
                IReadOnlyList<ListViewGroup> visibleGroups = GetVisibleGroups();
                return visibleGroups.Count == 0 ? null : visibleGroups[visibleGroups.Count - 1].AccessibilityObject;
            }

            return owningListView.Items.Count == 0 ? null : owningListView.Items[^1].AccessibilityObject;
        }

        internal override VARIANT GetPropertyValue(UIA_PROPERTY_ID propertyID)
            => propertyID switch
            {
                // If we don't set a default role for the accessible object
                // it will be retrieved from Windows.
                // And we don't have a 100% guarantee it will be correct, hence set it ourselves.
                UIA_PROPERTY_ID.UIA_ControlTypePropertyId when
                    this.GetOwnerAccessibleRole() == AccessibleRole.Default
                    => (VARIANT)(int)((this.TryGetOwnerAs(out ListView? owningListView) && owningListView.View == View.Details)
                        ? UIA_CONTROLTYPE_ID.UIA_TableControlTypeId
                        : UIA_CONTROLTYPE_ID.UIA_ListControlTypeId),
                UIA_PROPERTY_ID.UIA_HasKeyboardFocusPropertyId => VARIANT.False,
                UIA_PROPERTY_ID.UIA_IsKeyboardFocusablePropertyId => (VARIANT)State.HasFlag(AccessibleStates.Focusable),
                UIA_PROPERTY_ID.UIA_ItemStatusPropertyId => (VARIANT)GetItemStatus(),
                _ => base.GetPropertyValue(propertyID)
            };

        internal override IRawElementProviderSimple.Interface[]? GetRowHeaders()
            => null;

        internal override IRawElementProviderSimple.Interface[] GetSelection()
        {
            if (!this.IsOwnerHandleCreated(out ListView? owningListView))
            {
                return [];
            }

            var selectedItemProviders = new IRawElementProviderSimple.Interface[owningListView.SelectedIndices.Count];
            for (int i = 0; i < selectedItemProviders.Length; i++)
            {
                selectedItemProviders[i] = owningListView.Items[owningListView.SelectedIndices[i]].AccessibilityObject;
            }

            return selectedItemProviders;
        }

        internal IReadOnlyList<ListViewGroup> GetVisibleGroups()
        {
            List<ListViewGroup> list = [];

            if (!this.TryGetOwnerAs(out ListView? owningListView))
            {
                return list;
            }

            if (OwnerHasDefaultGroup)
            {
                list.Add(owningListView.DefaultGroup);
            }

            foreach (ListViewGroup listViewGroup in owningListView.Groups)
            {
                if (listViewGroup.AccessibilityObject is ListViewGroup.ListViewGroupAccessibleObject listViewGroupAccessibleObject
                    && listViewGroupAccessibleObject.GetVisibleItems().Count > 0)
                {
                    list.Add(listViewGroup);
                }
            }

            return list;
        }

        public override AccessibleObject? HitTest(int x, int y)
        {
            if (!this.IsOwnerHandleCreated(out ListView? owningListView))
            {
                return null;
            }

            Point hitTestPoint = new(x, y);
            Point point = owningListView.PointToClient(hitTestPoint);
            ListViewHitTestInfo hitTestInfo = owningListView.HitTest(point.X, point.Y);
            if (hitTestInfo.Item is null && owningListView.GroupsDisplayed)
            {
                IReadOnlyList<ListViewGroup> visibleGroups = GetVisibleGroups();
                for (int i = 0; i < visibleGroups.Count; i++)
                {
                    if (visibleGroups[i].AccessibilityObject.Bounds.Contains(hitTestPoint))
                    {
                        return visibleGroups[i].AccessibilityObject;
                    }
                }

                return null;
            }

            if (hitTestInfo.Item is not null)
            {
                AccessibleObject itemAccessibleObject = hitTestInfo.Item.AccessibilityObject;

                if (hitTestInfo.SubItem is not null)
                {
                    return owningListView.View switch
                    {
                        View.Details => ((ListViewItem.ListViewItemDetailsAccessibleObject)itemAccessibleObject)
                            .GetChild(hitTestInfo.SubItem.Index, point),

                        // Only additional ListViewSubItem are displayed in the accessibility tree if the ListView
                        // in the "Tile" view (the first ListViewSubItem is responsible for the ListViewItem)
                        View.Tile => hitTestInfo.SubItem.Index > 0 ? hitTestInfo.SubItem.AccessibilityObject : itemAccessibleObject,
                        _ => itemAccessibleObject
                    };
                }

                if (itemAccessibleObject is ListViewItem.ListViewItemDetailsAccessibleObject itemDetailsAccessibleObject)
                {
                    for (int i = 1; i < owningListView.Columns.Count; i++)
                    {
                        if (itemDetailsAccessibleObject.GetSubItemBounds(i).Contains(point))
                        {
                            return itemDetailsAccessibleObject.GetDetailsSubItemOrFake(i);
                        }
                    }
                }
                else if (itemAccessibleObject is ListViewItem.ListViewItemWithImageAccessibleObject itemIconAccessibleObject)
                {
                    return itemIconAccessibleObject.GetAccessibleObject(point);
                }

                return itemAccessibleObject;
            }

            return null;
        }

        internal override bool IsPatternSupported(UIA_PATTERN_ID patternId) =>
            this.TryGetOwnerAs(out ListView? owningListView)
                && (patternId == UIA_PATTERN_ID.UIA_SelectionPatternId
                    || patternId == UIA_PATTERN_ID.UIA_MultipleViewPatternId
                    || (patternId == UIA_PATTERN_ID.UIA_GridPatternId && owningListView.View == View.Details)
                    || (patternId == UIA_PATTERN_ID.UIA_TablePatternId && owningListView.View == View.Details)
                    || base.IsPatternSupported(patternId));

        internal override void SetMultiViewProviderCurrentView(int viewId)
        {
            if (!this.TryGetOwnerAs(out ListView? owningListView))
            {
                return;
            }

            foreach (int view in s_enumViewValues)
            {
                if (view == viewId)
                {
                    owningListView.View = (View)view;
                }
            }
        }
    }
}
