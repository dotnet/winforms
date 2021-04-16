// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Drawing;
using System.Runtime.InteropServices;
using static System.Windows.Forms.ListView;
using static Interop;
using static Interop.ComCtl32;

namespace System.Windows.Forms
{
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
                _owningGroup = owningGroup ?? throw new ArgumentNullException(nameof(owningGroup));

                // Using item from group for getting of ListView is a workaround for https://github.com/dotnet/winforms/issues/4019
                _owningListView = owningGroup.ListView
                    ?? (owningGroup.Items.Count > 0 && _owningGroup.Items[0].ListView is not null
                        ? _owningGroup.Items[0].ListView
                        : throw new InvalidOperationException(nameof(owningGroup.ListView)));

                _owningListViewAccessibilityObject = _owningListView.AccessibilityObject as ListView.ListViewAccessibleObject
                    ?? throw new InvalidOperationException(nameof(_owningListView.AccessibilityObject));

                _owningGroupIsDefault = owningGroupIsDefault;
            }

            private string AutomationId
                => string.Format("{0}-{1}", typeof(ListViewGroup).Name, CurrentIndex);

            public override Rectangle Bounds
            {
                get
                {
                    if (!_owningListView.IsHandleCreated || !_owningListViewAccessibilityObject.ShowGroupAccessibleObject)
                    {
                        return Rectangle.Empty;
                    }

                    if (GetVisibleItems().Count == 0)
                    {
                        return Rectangle.Empty;
                    }

                    int nativeGroupId = GetNativeGroupId();
                    if (nativeGroupId == -1)
                    {
                        return Rectangle.Empty;
                    }

                    LVGGR rectType = _owningGroup.CollapsedState == ListViewGroupCollapsedState.Collapsed
                        ? LVGGR.HEADER
                        : LVGGR.GROUP;

                    // Get the native rectangle
                    RECT groupRect = new();

                    // Using the "top" property, we set which rectangle type of the group we want to get
                    // This is described in more detail in https://docs.microsoft.com/windows/win32/controls/lvm-getgrouprect
                    groupRect.top = (int)rectType;
                    User32.SendMessageW(_owningListView, (User32.WM)LVM.GETGROUPRECT, (IntPtr)nativeGroupId, ref groupRect);

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

            internal override UiaCore.IRawElementProviderFragmentRoot FragmentRoot => _owningListView.AccessibilityObject;

            public override string Name
                => _owningGroup.Header;

            public override AccessibleRole Role
                => AccessibleRole.Grouping;

            internal override int[]? RuntimeId
            {
                get
                {
                    var owningListViewRuntimeId = _owningListViewAccessibilityObject.RuntimeId;
                    if (owningListViewRuntimeId is null)
                    {
                        return base.RuntimeId;
                    }

                    var runtimeId = new int[4];
                    runtimeId[0] = owningListViewRuntimeId[0];
                    runtimeId[1] = owningListViewRuntimeId[1];
                    runtimeId[2] = 4; // Win32-control specific RuntimeID constant, is used in similar Win32 controls and is used in WinForms controls for consistency.
                    runtimeId[3] = CurrentIndex;
                    return runtimeId;
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

            internal override void Collapse()
                => _owningGroup.CollapsedState = ListViewGroupCollapsedState.Collapsed;

            public override void DoDefaultAction() => SetFocus();

            internal override void Expand()
                => _owningGroup.CollapsedState = ListViewGroupCollapsedState.Expanded;

            private bool Focused
                => GetNativeFocus() || _owningGroup.Focused;

            private bool GetNativeFocus()
            {
                if (!_owningListView.IsHandleCreated || _owningListView.VirtualMode)
                {
                    return false;
                }

                int nativeGroupId = GetNativeGroupId();
                if (nativeGroupId == -1)
                {
                    return false;
                }

                return LVGS.FOCUSED == unchecked((LVGS)(long)User32.SendMessageW(_owningListView, (User32.WM)LVM.GETGROUPSTATE, (IntPtr)nativeGroupId, (IntPtr)LVGS.FOCUSED));
            }

            private unsafe int GetNativeGroupId()
            {
                LVGROUPW lvgroup = new LVGROUPW
                {
                    cbSize = (uint)sizeof(LVGROUPW),
                    mask = LVGF.GROUPID,
                };

                return User32.SendMessageW(_owningListView, (User32.WM)LVM.GETGROUPINFOBYINDEX, (IntPtr)CurrentIndex, ref lvgroup) == IntPtr.Zero
                    ? -1
                    : lvgroup.iGroupId;
            }

            internal override object? GetPropertyValue(UiaCore.UIA propertyID)
                => propertyID switch
                {
                    UiaCore.UIA.RuntimeIdPropertyId => RuntimeId,
                    UiaCore.UIA.AutomationIdPropertyId => AutomationId,
                    UiaCore.UIA.BoundingRectanglePropertyId => Bounds,
                    UiaCore.UIA.LegacyIAccessibleRolePropertyId => Role,
                    UiaCore.UIA.LegacyIAccessibleNamePropertyId => Name,
                    UiaCore.UIA.FrameworkIdPropertyId => NativeMethods.WinFormFrameworkId,
                    UiaCore.UIA.ControlTypePropertyId => UiaCore.UIA.GroupControlTypeId,
                    UiaCore.UIA.NamePropertyId => Name,
                    UiaCore.UIA.HasKeyboardFocusPropertyId => _owningListView.Focused && Focused,
                    UiaCore.UIA.IsKeyboardFocusablePropertyId => (State & AccessibleStates.Focusable) == AccessibleStates.Focusable,
                    UiaCore.UIA.IsEnabledPropertyId => _owningListView.Enabled,
                    UiaCore.UIA.IsOffscreenPropertyId => (State & AccessibleStates.Offscreen) == AccessibleStates.Offscreen,
                    UiaCore.UIA.NativeWindowHandlePropertyId => _owningListView.IsHandleCreated ? _owningListView.Handle : IntPtr.Zero,
                    UiaCore.UIA.IsLegacyIAccessiblePatternAvailablePropertyId => IsPatternSupported(UiaCore.UIA.LegacyIAccessiblePatternId),
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
                if (!_owningListView.IsHandleCreated || _owningListView.VirtualMode)
                {
                    return null;
                }

                switch (direction)
                {
                    case UiaCore.NavigateDirection.Parent:
                        return _owningListViewAccessibilityObject;
                    case UiaCore.NavigateDirection.NextSibling:
                        return _owningListViewAccessibilityObject.GetNextChild(this);
                    case UiaCore.NavigateDirection.PreviousSibling:
                        return _owningListViewAccessibilityObject.GetPreviousChild(this);
                    case UiaCore.NavigateDirection.FirstChild:
                        int childCount = GetChildCount();
                        if (childCount > 0)
                        {
                            return GetChild(0);
                        }

                        return null;

                    case UiaCore.NavigateDirection.LastChild:
                        childCount = GetChildCount();
                        if (childCount > 0)
                        {
                            return GetChild(childCount - 1);
                        }

                        return null;

                    default:
                        return null;
                }
            }

            public override AccessibleObject? GetChild(int index)
            {
                if (!_owningListView.IsHandleCreated || _owningListView.VirtualMode)
                {
                    return null;
                }

                IReadOnlyList<ListViewItem> visibleItems = GetVisibleItems();
                if (index < 0 || index >= visibleItems.Count)
                {
                    return null;
                }

                return visibleItems[index].AccessibilityObject;
            }

            private int GetChildIndex(AccessibleObject child)
            {
                if (!_owningListView.IsHandleCreated || _owningListView.VirtualMode)
                {
                    return -1;
                }

                int childCount = GetChildCount();
                for (int i = 0; i < childCount; i++)
                {
                    var currentChild = GetChild(i);
                    if (child == currentChild)
                    {
                        return i;
                    }
                }

                return -1;
            }

            internal AccessibleObject? GetNextChild(AccessibleObject currentChild)
            {
                if (!_owningListView.IsHandleCreated || _owningListView.VirtualMode)
                {
                    return null;
                }

                int currentChildIndex = GetChildIndex(currentChild);
                if (currentChildIndex == -1)
                {
                    return null;
                }

                int childCount = GetChildCount();
                if (currentChildIndex > childCount - 2) // Is more than pre-last element.
                {
                    return null;
                }

                return GetChild(currentChildIndex + 1);
            }

            internal AccessibleObject? GetPreviousChild(AccessibleObject currentChild)
            {
                if (!_owningListView.IsHandleCreated || _owningListView.VirtualMode)
                {
                    return null;
                }

                int currentChildIndex = GetChildIndex(currentChild);
                if (currentChildIndex <= 0)
                {
                    return null;
                }

                return GetChild(currentChildIndex - 1);
            }

            public override int GetChildCount()
            {
                if (!_owningListView.IsHandleCreated || _owningListView.VirtualMode)
                {
                    return -1;
                }

                return GetVisibleItems().Count;
            }

            internal override bool IsPatternSupported(UiaCore.UIA patternId)
            {
                if (patternId == UiaCore.UIA.LegacyIAccessiblePatternId ||
                    patternId == UiaCore.UIA.ExpandCollapsePatternId)
                {
                    return true;
                }

                return base.IsPatternSupported(patternId);
            }

            internal override unsafe void SetFocus()
            {
                if (!_owningListView.IsHandleCreated || _owningListView.VirtualMode)
                {
                    return;
                }

                _owningListView.FocusedGroup = _owningGroup;

                RaiseAutomationEvent(UiaCore.UIA.AutomationFocusChangedEventId);
            }
        }
    }
}
