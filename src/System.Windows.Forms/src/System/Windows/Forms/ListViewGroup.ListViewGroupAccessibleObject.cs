// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Drawing;
using static Interop;
using static Interop.ComCtl32;

namespace System.Windows.Forms
{
    public partial class ListViewGroup
    {
        internal class ListViewGroupAccessibleObject : AccessibleObject
        {
            private readonly ListView _owningListView;
            private readonly ListViewGroup _owningGroup;
            private readonly bool _owningGroupIsDefault;

            public ListViewGroupAccessibleObject(ListViewGroup owningGroup, bool owningGroupIsDefault)
            {
                _owningGroup = owningGroup ?? throw new ArgumentNullException(nameof(owningGroup));

                // Using item from group for getting of ListView is a workaround for https://github.com/dotnet/winforms/issues/4019
                _owningListView = owningGroup.ListView
                    ?? (owningGroup.Items.Count > 0 && _owningGroup.Items[0].ListView != null
                        ? _owningGroup.Items[0].ListView
                        : throw new InvalidOperationException(nameof(owningGroup.ListView)));

                _owningGroupIsDefault = owningGroupIsDefault;
            }

            private string AutomationId
                => string.Format("{0}-{1}", typeof(ListViewGroup).Name, CurrentIndex);

            public override Rectangle Bounds
            {
                get
                {
                    if (!_owningListView.IsHandleCreated || _owningListView.VirtualMode)
                    {
                        return Rectangle.Empty;
                    }

                    RECT groupRect = new RECT();
                    User32.SendMessageW(_owningListView, (User32.WM)ComCtl32.LVM.GETGROUPRECT, (IntPtr)CurrentIndex, ref groupRect);

                    return new Rectangle(
                        _owningListView.AccessibilityObject.Bounds.X + groupRect.left,
                        _owningListView.AccessibilityObject.Bounds.Y + groupRect.top,
                        groupRect.right - groupRect.left,
                        groupRect.bottom - groupRect.top);
                }
            }

            private int CurrentIndex
                => _owningGroupIsDefault
                    // Default group has the last index out of the Groups.Count
                    // upper bound: so the DefaultGroup.Index == Groups.Count.
                    ? _owningListView.Groups.Count
                    : _owningListView.Groups.IndexOf(_owningGroup);

            public override string DefaultAction
                => SR.AccessibleActionDoubleClick;

            internal override UiaCore.ExpandCollapseState ExpandCollapseState
                => _owningGroup.CollapsedState == ListViewGroupCollapsedState.Collapsed
                    ? UiaCore.ExpandCollapseState.Collapsed
                    : UiaCore.ExpandCollapseState.Expanded;

            public override string Name
                => _owningGroup.Header;

            public override AccessibleRole Role
                => AccessibleRole.Grouping;

            internal override int[]? RuntimeId
            {
                get
                {
                    var owningListViewRuntimeId = _owningListView.AccessibilityObject.RuntimeId;
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

                return LVGS.FOCUSED == unchecked((LVGS)(long)User32.SendMessageW(_owningListView, (User32.WM)LVM.GETGROUPSTATE, (IntPtr)CurrentIndex, (IntPtr)LVGS.FOCUSED));
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

            internal override UiaCore.IRawElementProviderFragment? FragmentNavigate(UiaCore.NavigateDirection direction)
            {
                if (!_owningListView.IsHandleCreated || _owningListView.VirtualMode)
                {
                    return null;
                }

                switch (direction)
                {
                    case UiaCore.NavigateDirection.Parent:
                        return _owningListView.AccessibilityObject;
                    case UiaCore.NavigateDirection.NextSibling:
                        return (_owningListView.AccessibilityObject as ListView.ListViewAccessibleObject)?.GetNextChild(this);
                    case UiaCore.NavigateDirection.PreviousSibling:
                        return (_owningListView.AccessibilityObject as ListView.ListViewAccessibleObject)?.GetPreviousChild(this);
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

                if (!_owningGroupIsDefault)
                {
                    if (index < 0 || index >= _owningGroup.Items.Count)
                    {
                        return null;
                    }

                    return _owningGroup.Items[index].AccessibilityObject;
                }

                foreach (ListViewItem? item in _owningListView.Items)
                {
                    if (item != null && item.Group is null && index-- == 0)
                    {
                        return item.AccessibilityObject;
                    }
                }

                return null;
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

                if (_owningGroupIsDefault)
                {
                    int count = 0;
                    foreach (ListViewItem? item in _owningListView.Items)
                    {
                        if (item != null && item.Group is null)
                        {
                            count++;
                        }
                    }

                    return count;
                }
                else
                {
                    return _owningGroup.Items.Count;
                }
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
