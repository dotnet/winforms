// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

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
        internal class ListViewItemAccessibleObject : AccessibleObject
        {
            private readonly ListView _owningListView;
            private readonly ListViewItem _owningItem;
            private readonly ListViewGroup? _owningGroup;
            private readonly IAccessible? _systemIAccessible;

            public ListViewItemAccessibleObject(ListViewItem owningItem, ListViewGroup? owningGroup)
            {
                _owningItem = owningItem ?? throw new ArgumentNullException(nameof(owningItem));
                _owningListView = owningItem.ListView ?? owningGroup?.ListView ?? throw new InvalidOperationException(nameof(owningItem.ListView));
                _owningGroup = owningGroup;
                _systemIAccessible = _owningListView.AccessibilityObject.GetSystemIAccessibleInternal();
            }

            private string AutomationId
                => string.Format("{0}-{1}", typeof(ListViewItem).Name, CurrentIndex);

            public override Rectangle Bounds
                => _owningGroup?.CollapsedState == ListViewGroupCollapsedState.Collapsed
                    ? Rectangle.Empty
                    : new Rectangle(
                        _owningListView.AccessibilityObject.Bounds.X + _owningItem.Bounds.X,
                        _owningListView.AccessibilityObject.Bounds.Y + _owningItem.Bounds.Y,
                        _owningItem.Bounds.Width,
                        _owningItem.Bounds.Height);

            private int CurrentIndex
                => _owningListView.Items.IndexOf(_owningItem);

            internal override UiaCore.IRawElementProviderFragmentRoot FragmentRoot
                => _owningListView.AccessibilityObject;

            internal override bool IsItemSelected
                => (State & AccessibleStates.Selected) != 0;

            public override string? Name
            {
                get => _owningItem.Text;
                set => base.Name = value;
            }

            private bool OwningListItemFocused
            {
                get
                {
                    bool owningListViewFocused = _owningListView.Focused;
                    bool owningListItemFocused = _owningListView.FocusedItem == _owningItem;
                    return owningListViewFocused && owningListItemFocused;
                }
            }

            /// <summary>
            ///  Gets the accessible role.
            /// </summary>
            public override AccessibleRole Role
                => AccessibleRole.ListItem;

            /// <summary>
            ///  Gets the accessible state.
            /// </summary>
            public override AccessibleStates State
            {
                get
                {
                    AccessibleStates state = AccessibleStates.Selectable | AccessibleStates.Focusable | AccessibleStates.MultiSelectable;

                    if (_owningListView.SelectedIndices.Contains(_owningItem.Index))
                    {
                        return state |= AccessibleStates.Selected | AccessibleStates.Focused;
                    }

                    object? systemIAccessibleState = _systemIAccessible?.get_accState(GetChildId());
                    if (systemIAccessibleState != null)
                    {
                        return state |= (AccessibleStates)systemIAccessibleState;
                    }

                    return state;
                }
            }

            internal override void AddToSelection()
                => SelectItem();

            public override string DefaultAction
                => SR.AccessibleActionDoubleClick;

            public override void DoDefaultAction()
                => SetFocus();

            internal override UiaCore.IRawElementProviderFragment? FragmentNavigate(UiaCore.NavigateDirection direction)
            {
                ListViewGroupAccessibleObject? owningGroupAccessibleObject = (ListViewGroupAccessibleObject?)_owningGroup?.AccessibilityObject;
                switch (direction)
                {
                    case UiaCore.NavigateDirection.Parent:
                        return owningGroupAccessibleObject ?? _owningListView.AccessibilityObject;
                    case UiaCore.NavigateDirection.NextSibling:
                        if (owningGroupAccessibleObject is null)
                        {
                            ListViewAccessibleObject listViewAccessibilityObject = (ListViewAccessibleObject)_owningListView.AccessibilityObject;
                            return listViewAccessibilityObject.GetNextChild(this);
                        }

                        return owningGroupAccessibleObject.GetNextChild(this);
                    case UiaCore.NavigateDirection.PreviousSibling:
                        if (owningGroupAccessibleObject is null)
                        {
                            ListViewAccessibleObject listViewAccessibilityObject = (ListViewAccessibleObject)_owningListView.AccessibilityObject;
                            return listViewAccessibilityObject.GetPreviousChild(this);
                        }

                        return owningGroupAccessibleObject.GetPreviousChild(this);
                    case UiaCore.NavigateDirection.FirstChild:
                        if (_owningItem.SubItems.Count > 0)
                        {
                            return GetChild(0);
                        }
                        break;
                    case UiaCore.NavigateDirection.LastChild:
                        int subItemsCount = _owningItem.SubItems.Count;
                        if (subItemsCount > 0)
                        {
                            return GetChild(subItemsCount - 1);
                        }
                        break;
                }

                return base.FragmentNavigate(direction);
            }

            public override AccessibleObject? GetChild(int index)
            {
                if (index < 0 || index >= _owningItem.SubItems.Count || _owningGroup != null)
                {
                    return null;
                }

                return _owningItem.SubItems[index].AccessibilityObject;
            }

            internal override int[]? RuntimeId
            {
                get
                {
                    int[] runtimeId;
                    var owningListViewRuntimeId = _owningListView.AccessibilityObject.RuntimeId;
                    if (owningListViewRuntimeId is null)
                    {
                        return base.RuntimeId;
                    }

                    if (_owningGroup != null)
                    {
                        runtimeId = new int[5];
                        runtimeId[0] = owningListViewRuntimeId[0];
                        runtimeId[1] = owningListViewRuntimeId[1];
                        runtimeId[2] = 4; // Win32-control specific RuntimeID constant, is used in similar Win32 controls and is used in WinForms controls for consistency.
                        runtimeId[3] = _owningListView.GetGroupIndex(_owningGroup);
                        runtimeId[4] = CurrentIndex;

                        return runtimeId;
                    }

                    runtimeId = new int[4];
                    runtimeId[0] = owningListViewRuntimeId[0];
                    runtimeId[1] = owningListViewRuntimeId[1];
                    runtimeId[2] = 4; // Win32-control specific RuntimeID constant.
                    runtimeId[3] = CurrentIndex;

                    return runtimeId;
                }
            }

            internal override UiaCore.ToggleState ToggleState
                => _owningItem.Checked
                    ? UiaCore.ToggleState.On
                    : UiaCore.ToggleState.Off;

            internal override object? GetPropertyValue(UiaCore.UIA propertyID)
                => propertyID switch
                {
                    UiaCore.UIA.RuntimeIdPropertyId => RuntimeId,
                    UiaCore.UIA.AutomationIdPropertyId => AutomationId,
                    UiaCore.UIA.BoundingRectanglePropertyId => Bounds,
                    UiaCore.UIA.FrameworkIdPropertyId => NativeMethods.WinFormFrameworkId,
                    UiaCore.UIA.ControlTypePropertyId => UiaCore.UIA.ListItemControlTypeId,
                    UiaCore.UIA.NamePropertyId => Name,
                    UiaCore.UIA.HasKeyboardFocusPropertyId => OwningListItemFocused,
                    UiaCore.UIA.IsKeyboardFocusablePropertyId => (State & AccessibleStates.Focusable) == AccessibleStates.Focusable,
                    UiaCore.UIA.IsEnabledPropertyId => _owningListView.Enabled,
                    UiaCore.UIA.IsOffscreenPropertyId => (State & AccessibleStates.Offscreen) == AccessibleStates.Offscreen,
                    UiaCore.UIA.NativeWindowHandlePropertyId => _owningListView.IsHandleCreated ? _owningListView.Handle : IntPtr.Zero,
                    UiaCore.UIA.IsSelectionItemPatternAvailablePropertyId => IsPatternSupported(UiaCore.UIA.SelectionItemPatternId),
                    UiaCore.UIA.IsScrollItemPatternAvailablePropertyId => IsPatternSupported(UiaCore.UIA.ScrollItemPatternId),
                    UiaCore.UIA.IsInvokePatternAvailablePropertyId => IsPatternSupported(UiaCore.UIA.InvokePatternId),
                    _ => base.GetPropertyValue(propertyID)
                };

            /// <summary>
            ///  Indicates whether specified pattern is supported.
            /// </summary>
            /// <param name="patternId">The pattern ID.</param>
            /// <returns>True if specified </returns>
            internal override bool IsPatternSupported(UiaCore.UIA patternId)
            {
                if (patternId == UiaCore.UIA.ScrollItemPatternId ||
                    patternId == UiaCore.UIA.LegacyIAccessiblePatternId ||
                    patternId == UiaCore.UIA.SelectionItemPatternId ||
                    patternId == UiaCore.UIA.InvokePatternId ||
                    (patternId == UiaCore.UIA.TogglePatternId && _owningListView.CheckBoxes))
                {
                    return true;
                }

                return base.IsPatternSupported(patternId);
            }

            internal override void RemoveFromSelection()
            {
                // Do nothing, C++ implementation returns UIA_E_INVALIDOPERATION 0x80131509
            }

            internal override UiaCore.IRawElementProviderSimple ItemSelectionContainer
                => _owningListView.AccessibilityObject;

            internal override void ScrollIntoView() => _owningItem.EnsureVisible();

            internal unsafe override void SelectItem()
            {
                if (_owningListView.IsHandleCreated)
                {
                    _owningListView.SelectedIndices.Add(CurrentIndex);
                    User32.InvalidateRect(new HandleRef(this, _owningListView.Handle), null, BOOL.FALSE);
                }

                RaiseAutomationEvent(UiaCore.UIA.AutomationFocusChangedEventId);
                RaiseAutomationEvent(UiaCore.UIA.SelectionItem_ElementSelectedEventId);
            }

            internal override void SetFocus()
            {
                RaiseAutomationEvent(UiaCore.UIA.AutomationFocusChangedEventId);
                SelectItem();
            }

            public override void Select(AccessibleSelection flags)
            {
                if (!_owningListView.IsHandleCreated)
                {
                    return;
                }

                try
                {
                    _systemIAccessible?.accSelect((int)flags, GetChildId());
                }
                catch (ArgumentException)
                {
                    // In Everett, the ListBox accessible children did not have any selection capability.
                    // In Whidbey, they delegate the selection capability to OLEACC.
                    // However, OLEACC does not deal w/ several Selection flags: ExtendSelection, AddSelection, RemoveSelection.
                    // OLEACC instead throws an ArgumentException.
                    // Since Whidbey API's should not throw an exception in places where Everett API's did not, we catch
                    // the ArgumentException and fail silently.
                }
            }

            internal override void Toggle()
            {
                _owningItem.Checked = !_owningItem.Checked;
            }
        }
    }
}
