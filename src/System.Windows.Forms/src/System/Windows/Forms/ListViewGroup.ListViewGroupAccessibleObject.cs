﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;
using static System.Windows.Forms.ListView;
using static Interop;

namespace System.Windows.Forms;

public partial class ListViewGroup
{
    internal class ListViewGroupAccessibleObject : AccessibleObject
    {
        private readonly ListView _owningListView;
        private readonly ListViewAccessibleObject _owningListViewAccessibilityObject;
        private readonly ListViewGroup _owningGroup;
        private readonly bool _owningGroupIsDefault;

        public ListViewGroupAccessibleObject(ListViewGroup owningGroup, bool owningGroupIsDefault)
        {
            _owningGroup = owningGroup.OrThrowIfNull();

            // Using item from group for getting of ListView is a workaround for https://github.com/dotnet/winforms/issues/4019
            _owningListView = owningGroup.ListView
                ?? (owningGroup.Items.Count > 0 && _owningGroup.Items[0].ListView is ListView listView
                    ? listView
                    : throw new InvalidOperationException(nameof(owningGroup.ListView)));

            _owningListViewAccessibilityObject = _owningListView.AccessibilityObject as ListView.ListViewAccessibleObject
                ?? throw new InvalidOperationException(nameof(_owningListView.AccessibilityObject));

            _owningGroupIsDefault = owningGroupIsDefault;
        }

        private protected override string AutomationId
            => $"{nameof(ListViewGroup)}-{CurrentIndex}";

        public override Rectangle Bounds
        {
            get
            {
                if (!_owningListView.IsHandleCreated || !_owningListView.GroupsDisplayed || IsEmpty)
                {
                    return Rectangle.Empty;
                }

                int nativeGroupId = GetNativeGroupId();
                if (nativeGroupId == -1)
                {
                    return Rectangle.Empty;
                }

                uint rectType = _owningGroup.CollapsedState == ListViewGroupCollapsedState.Collapsed
                    ? PInvoke.LVGGR_HEADER
                    : PInvoke.LVGGR_GROUP;

                // Get the native rectangle
                RECT groupRect = default;

                // Using the "top" property, we set which rectangle type of the group we want to get
                // This is described in more detail in https://docs.microsoft.com/windows/win32/controls/lvm-getgrouprect
                groupRect.top = (int)rectType;
                PInvoke.SendMessage(_owningListView, PInvoke.LVM_GETGROUPRECT, (WPARAM)nativeGroupId, ref groupRect);

                // Using the following code, we limit the size of the ListViewGroup rectangle
                // so that it does not go beyond the rectangle of the ListView
                Rectangle listViewBounds = _owningListView.AccessibilityObject.Bounds;
                groupRect = _owningListView.RectangleToScreen(groupRect);
                groupRect.top = Math.Max(listViewBounds.Top, groupRect.top);
                groupRect.bottom = Math.Min(listViewBounds.Bottom, groupRect.bottom);
                groupRect.left = Math.Max(listViewBounds.Left, groupRect.left);
                groupRect.right = Math.Min(listViewBounds.Right, groupRect.right);

                return groupRect;
            }
        }

        internal int CurrentIndex
            // The default group has 0 index, as it is always displayed first.
            => _owningGroupIsDefault
                ? 0
                // When calculating the index of other groups, we add a shift if the default group is displayed
                : _owningListViewAccessibilityObject.OwnerHasDefaultGroup
                    ? _owningListView.Groups.IndexOf(_owningGroup) + 1
                    : _owningListView.Groups.IndexOf(_owningGroup);

        public override string DefaultAction
            => SR.AccessibleActionDoubleClick;

        internal override UiaCore.ExpandCollapseState ExpandCollapseState
            => _owningGroup.CollapsedState == ListViewGroupCollapsedState.Collapsed
                ? UiaCore.ExpandCollapseState.Collapsed
                : UiaCore.ExpandCollapseState.Expanded;

        internal override UiaCore.IRawElementProviderFragmentRoot FragmentRoot
            => _owningListView.AccessibilityObject;

        public override string Name
            => !string.IsNullOrEmpty(_owningGroup.Subtitle)
                ? $"{_owningGroup.Header}. {_owningGroup.Subtitle}"
                : _owningGroup.Header;

        public override AccessibleRole Role
            => AccessibleRole.Grouping;

        internal override int[] RuntimeId
        {
            get
            {
                var owningListViewRuntimeId = _owningListViewAccessibilityObject.RuntimeId;

                Debug.Assert(owningListViewRuntimeId.Length >= 2);

                return new int[]
                {
                    owningListViewRuntimeId[0],
                    owningListViewRuntimeId[1],
                    4, // Win32-control specific RuntimeID constant, is used in similar Win32 controls and is used in WinForms controls for consistency.
                    GetHashCode()
                };
            }
        }

        public override AccessibleStates State
        {
            get
            {
                AccessibleStates state = AccessibleStates.Focusable;

                if (Focused)
                {
                    state |= AccessibleStates.Focused;
                }

                return state;
            }
        }

        private bool IsEmpty => GetVisibleItems().Count == 0;

        internal override void Collapse()
            => _owningGroup.CollapsedState = ListViewGroupCollapsedState.Collapsed;

        public override void DoDefaultAction() => SetFocus();

        internal override void Expand()
            => _owningGroup.CollapsedState = ListViewGroupCollapsedState.Expanded;

        private bool Focused
            => GetNativeFocus() || _owningGroup.Focused;

        private bool GetNativeFocus()
        {
            if (!_owningListView.IsHandleCreated || !_owningListView.GroupsDisplayed)
            {
                return false;
            }

            int nativeGroupId = GetNativeGroupId();
            if (nativeGroupId == -1)
            {
                return false;
            }

            return (LIST_VIEW_GROUP_STATE_FLAGS)(uint)PInvoke.SendMessage(
                _owningListView,
                PInvoke.LVM_GETGROUPSTATE,
                (WPARAM)nativeGroupId,
                (LPARAM)(uint)LIST_VIEW_GROUP_STATE_FLAGS.LVGS_FOCUSED) == LIST_VIEW_GROUP_STATE_FLAGS.LVGS_FOCUSED;
        }

        private int GetNativeGroupId()
        {
            if (PInvoke.SendMessage(_owningListView, PInvoke.LVM_HASGROUP, (WPARAM)_owningGroup.ID) == 0)
            {
                return -1;
            }

            return _owningGroup.ID;
        }

        internal override object? GetPropertyValue(UiaCore.UIA propertyID)
            => propertyID switch
            {
                UiaCore.UIA.ControlTypePropertyId => UiaCore.UIA.GroupControlTypeId,
                UiaCore.UIA.HasKeyboardFocusPropertyId => _owningListView.Focused && Focused,
                UiaCore.UIA.IsEnabledPropertyId => _owningListView.Enabled,
                UiaCore.UIA.IsKeyboardFocusablePropertyId => (State & AccessibleStates.Focusable) == AccessibleStates.Focusable,
                UiaCore.UIA.NativeWindowHandlePropertyId => _owningListView.IsHandleCreated ? _owningListView.Handle : IntPtr.Zero,
                _ => base.GetPropertyValue(propertyID)
            };

        internal IReadOnlyList<ListViewItem> GetVisibleItems()
        {
            List<ListViewItem> visibleItems = new();
            if (_owningGroupIsDefault)
            {
                foreach (ListViewItem? listViewItem in _owningListView.Items)
                {
                    if (listViewItem is not null && listViewItem.Group is null)
                    {
                        visibleItems.Add(listViewItem);
                    }
                }

                return visibleItems;
            }

            foreach (ListViewItem listViewItem in _owningGroup.Items)
            {
                if (listViewItem.ListView is not null)
                {
                    visibleItems.Add(listViewItem);
                }
            }

            return visibleItems;
        }

        internal override UiaCore.IRawElementProviderFragment? FragmentNavigate(UiaCore.NavigateDirection direction)
        {
            if (!_owningListView.IsHandleCreated || !_owningListView.GroupsDisplayed || IsEmpty)
            {
                return null;
            }

            switch (direction)
            {
                case UiaCore.NavigateDirection.Parent:
                    return _owningListViewAccessibilityObject;
                case UiaCore.NavigateDirection.NextSibling:
                    int childIndex = _owningListViewAccessibilityObject.GetChildIndex(this);
                    return childIndex == InvalidIndex
                        ? null
                        : _owningListViewAccessibilityObject.GetChild(childIndex + 1);
                case UiaCore.NavigateDirection.PreviousSibling:
                    return _owningListViewAccessibilityObject.GetChild(_owningListViewAccessibilityObject.GetChildIndex(this) - 1);
                case UiaCore.NavigateDirection.FirstChild:
                    return GetChild(0);
                case UiaCore.NavigateDirection.LastChild:
                    IReadOnlyList<ListViewItem> visibleItems = GetVisibleItems();
                    return visibleItems.Count > 0 ? visibleItems[visibleItems.Count - 1].AccessibilityObject : null;

                default:
                    return null;
            }
        }

        public override AccessibleObject? GetChild(int index)
        {
            if (!_owningListView.IsHandleCreated || !_owningListView.GroupsDisplayed || index < 0)
            {
                return null;
            }

            IReadOnlyList<ListViewItem> visibleItems = GetVisibleItems();
            if (index >= visibleItems.Count)
            {
                return null;
            }

            return visibleItems[index].AccessibilityObject;
        }

        internal override int GetChildIndex(AccessibleObject? child)
        {
            if (child is null || !_owningListView.IsHandleCreated || !_owningListView.GroupsDisplayed)
            {
                return InvalidIndex;
            }

            IReadOnlyList<ListViewItem> visibleItems = GetVisibleItems();
            for (int i = 0; i < visibleItems.Count; i++)
            {
                if (visibleItems[i].AccessibilityObject == child)
                {
                    return i;
                }
            }

            return InvalidIndex;
        }

        public override int GetChildCount()
        {
            if (!_owningListView.IsHandleCreated || !_owningListView.GroupsDisplayed)
            {
                return InvalidIndex;
            }

            return GetVisibleItems().Count;
        }

        internal override bool IsPatternSupported(UiaCore.UIA patternId)
            => patternId switch
            {
                UiaCore.UIA.LegacyIAccessiblePatternId => true,
                UiaCore.UIA.ExpandCollapsePatternId => _owningGroup.CollapsedState != ListViewGroupCollapsedState.Default,
                _ => base.IsPatternSupported(patternId),
            };

        internal override unsafe void SetFocus()
        {
            if (!_owningListView.IsHandleCreated || !_owningListView.GroupsDisplayed || IsEmpty)
            {
                return;
            }

            _owningListView.FocusedGroup = _owningGroup;

            RaiseAutomationEvent(UiaCore.UIA.AutomationFocusChangedEventId);
        }
    }
}
