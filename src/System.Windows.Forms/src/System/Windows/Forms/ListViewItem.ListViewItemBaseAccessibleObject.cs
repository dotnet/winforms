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
        /// <summary>
        ///  This class contains the base implementation of properties and methods for ListViewItem accessibility objects.
        /// </summary>
        /// <remarks>
        ///  The implementation of this class fully corresponds to the behavior of the ListViewItem accessibility object
        ///  when the ListView is in "LargeIcon" or "SmallIcon" view.
        /// </remarks>
        internal class ListViewItemBaseAccessibleObject : AccessibleObject
        {
            private protected readonly ListView _owningListView;
            private protected readonly ListViewItem _owningItem;
            private protected readonly IAccessible? _systemIAccessible;

            public ListViewItemBaseAccessibleObject(ListViewItem owningItem)
            {
                _owningItem = owningItem ?? throw new ArgumentNullException(nameof(owningItem));
                _owningListView = owningItem.ListView ?? owningItem.Group?.ListView ?? throw new InvalidOperationException(nameof(owningItem.ListView));
                _systemIAccessible = _owningListView.AccessibilityObject.GetSystemIAccessibleInternal();
            }

            private protected ListViewGroup? OwningGroup => _owningListView.GroupsDisplayed
                ? _owningItem.Group ?? _owningListView.DefaultGroup
                : null;

            private string AutomationId
                => string.Format("{0}-{1}", typeof(ListViewItem).Name, CurrentIndex);

            public override Rectangle Bounds
                => !_owningListView.IsHandleCreated || OwningGroup?.CollapsedState == ListViewGroupCollapsedState.Collapsed
                    ? Rectangle.Empty
                    : new Rectangle(
                        _owningListView.AccessibilityObject.Bounds.X + _owningItem.Bounds.X,
                        _owningListView.AccessibilityObject.Bounds.Y + _owningItem.Bounds.Y,
                        _owningItem.Bounds.Width,
                        _owningItem.Bounds.Height);

            private protected int CurrentIndex
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
                    if (systemIAccessibleState is not null)
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
                AccessibleObject _parentInternal = OwningGroup?.AccessibilityObject ?? _owningListView.AccessibilityObject;

                switch (direction)
                {
                    case UiaCore.NavigateDirection.Parent:
                        return _parentInternal;
                    case UiaCore.NavigateDirection.NextSibling:
                        int childIndex = _parentInternal.GetChildIndex(this);
                        return childIndex == -1 ? null : _parentInternal.GetChild(childIndex + 1);
                    case UiaCore.NavigateDirection.PreviousSibling:
                        return _parentInternal.GetChild(_parentInternal.GetChildIndex(this) - 1);
                    case UiaCore.NavigateDirection.FirstChild:
                    case UiaCore.NavigateDirection.LastChild:
                        return null;
                }

                return base.FragmentNavigate(direction);
            }

            public override AccessibleObject? GetChild(int index)
            {
                if (_owningListView.View == View.Details || _owningListView.View == View.Tile)
                {
                    throw new InvalidOperationException(string.Format(SR.ListViewItemAccessibilityObjectInvalidViewsException,
                        nameof(View.LargeIcon),
                        nameof(View.List),
                        nameof(View.SmallIcon)));
                }

                return null;
            }

            internal virtual AccessibleObject? GetChildInternal(int index) => GetChild(index);

            public override int GetChildCount()
            {
                if (_owningListView.View == View.Details || _owningListView.View == View.Tile)
                {
                    throw new InvalidOperationException(string.Format(SR.ListViewItemAccessibilityObjectInvalidViewsException,
                        nameof(View.LargeIcon),
                        nameof(View.List),
                        nameof(View.SmallIcon)));
                }

                return -1;
            }

            internal override int GetChildIndex(AccessibleObject? child) => -1;

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

            internal virtual Rectangle GetSubItemBounds(int subItemIndex) => Rectangle.Empty;

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

                    if (OwningGroup is not null)
                    {
                        runtimeId = new int[5];
                        runtimeId[0] = owningListViewRuntimeId[0];
                        runtimeId[1] = owningListViewRuntimeId[1];
                        runtimeId[2] = 4; // Win32-control specific RuntimeID constant, is used in similar Win32 controls and is used in WinForms controls for consistency.
                        runtimeId[3] = OwningGroup.AccessibilityObject is ListViewGroupAccessibleObject listViewGroupAccessibleObject
                                        ? listViewGroupAccessibleObject.CurrentIndex
                                        : -1;

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
