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
        internal class ListViewItemAccessibleObject : AccessibleObject
        {
            private readonly ListView _owningListView;
            private readonly ListViewItem _owningItem;
            private readonly IAccessible? _systemIAccessible;
            private readonly Dictionary<int, AccessibleObject> _listViewSubItemAccessibleObjects;

            public ListViewItemAccessibleObject(ListViewItem owningItem)
            {
                _owningItem = owningItem ?? throw new ArgumentNullException(nameof(owningItem));
                _owningListView = owningItem.ListView ?? owningItem.Group?.ListView ?? throw new InvalidOperationException(nameof(owningItem.ListView));
                _systemIAccessible = _owningListView.AccessibilityObject.GetSystemIAccessibleInternal();
                _listViewSubItemAccessibleObjects = new Dictionary<int, AccessibleObject>();
            }

            private ListViewGroup? OwningGroup => _owningListView.GroupsDisplayed
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
                        if (_owningItem.SubItems.Count > 0)
                        {
                            return GetChild(0);
                        }

                        break;
                    case UiaCore.NavigateDirection.LastChild:
                        return GetChildInternal(LastChildIndex);
                }

                return base.FragmentNavigate(direction);
            }

            public override AccessibleObject? GetChild(int index)
                // Only additional ListViewSubItem are displayed in the accessibility tree if the ListView
                // in the "Tile" view (the first ListViewSubItem is responsible for the ListViewItem)
                => GetChildInternal(_owningListView.View == View.Tile ? index + 1 : index);

            internal AccessibleObject? GetChildInternal(int index)
            {
                // If the ListView does not support ListViewSubItems, the index is greater than the number of columns
                // or the index is negative, then we return null
                if (!_owningListView.SupportsListViewSubItems || _owningListView.Columns.Count <= index || index < 0)
                {
                    return null;
                }

                if (_owningListView.View == View.Details)
                {
                    return GetDetailsSubItemOrFake(index);
                }

                // The ListViewSubItem for "Tile" mode is only displayed if the following conditions are met:
                // 1. The ListViewSubItem is not the first (data about the first ListViewSubItem is displayed in the ListViewtem itself)
                // 2. The ListViewSubItem has non-empty bounds(otherwise it means that it is not displayed)
                if (index > 0
                    && _owningItem.SubItems.Count > index
                    && !GetSubItemBounds(index).IsEmpty)
                {
                    return _owningItem.SubItems[index].AccessibilityObject;
                }

                return null;
            }

            public override int GetChildCount()
            {
                if (!_owningListView.IsHandleCreated || !_owningListView.SupportsListViewSubItems)
                {
                    return -1;
                }

                return _owningListView.View switch
                {
                    View.Details => _owningListView.Columns.Count,
                    View.Tile => GetChildCountTileView(),
                    _ => -1
                };
            }

            private int GetChildCountTileView()
            {
                Debug.Assert(_owningListView.View == View.Tile, $"{nameof(GetChildCountTileView)} method should only be called for 'Tile' view");

                // Data about the first ListViewSubItem is displayed in the ListViewItem.
                // Therefore, it is not displayed in the ListViewSubItems list
                if (_owningItem.SubItems.Count == 1)
                {
                    return 0;
                }

                return GetLastTileChildIndex();
            }

            internal override int GetChildIndex(AccessibleObject? child)
            {
                if (child is null
                    || !_owningListView.SupportsListViewSubItems
                    || child is not ListViewSubItem.ListViewSubItemAccessibleObject subItemAccessibleObject)
                {
                    return -1;
                }

                // Data about the first ListViewSubItem is displayed in the ListViewItem.
                // Therefore, it is not displayed in the ListViewSubItems list
                if (_owningListView.View == View.Tile && _owningItem.SubItems[0].AccessibilityObject == child)
                {
                    return -1;
                }

                if (subItemAccessibleObject.OwningSubItem is null)
                {
                    return _owningListView.View == View.Details
                        ? GetFakeSubItemIndex(subItemAccessibleObject)
                        : -1;
                }

                int index = _owningItem.SubItems.IndexOf(subItemAccessibleObject.OwningSubItem);
                if (index == -1
                    || (_owningListView.View == View.Details && index > _owningListView.Columns.Count - 1)
                    || (_owningListView.View == View.Tile && index > GetLastTileChildIndex()))
                {
                    return -1;
                }

                return index;
            }

            private int LastChildIndex
                => _owningListView.View switch
                {
                    View.Details => _owningListView.Columns.Count - 1,
                    View.Tile => GetLastTileChildIndex(),
                    _ => -1
                };

            // This method returns an accessibility object for an existing ListViewSubItem, or creates a fake one
            // if the ListViewSubItem does not exist. This is necessary for the "Details" view,
            // when there is no ListViewSubItem, but the cell for it is displayed and the user can interact with it.
            internal AccessibleObject? GetDetailsSubItemOrFake(int subItemIndex)
            {
                if (subItemIndex < _owningItem.SubItems.Count)
                {
                    _listViewSubItemAccessibleObjects.Remove(subItemIndex);

                    return _owningItem.SubItems[subItemIndex].AccessibilityObject;
                }

                if (_listViewSubItemAccessibleObjects.ContainsKey(subItemIndex))
                {
                    return _listViewSubItemAccessibleObjects[subItemIndex];
                }

                ListViewSubItem.ListViewSubItemAccessibleObject fakeAccessbileObject = new(owningSubItem: null, _owningItem);
                _listViewSubItemAccessibleObjects.Add(subItemIndex, fakeAccessbileObject);
                return fakeAccessbileObject;
            }

            // This method is required to get the index of the fake accessibility object. Since the fake accessibility object
            // has no ListViewSubItem from which we could get an index, we have to get its index from the dictionary
            private int GetFakeSubItemIndex(ListViewSubItem.ListViewSubItemAccessibleObject fakeAccessbileObject)
            {
                foreach (KeyValuePair<int, AccessibleObject> keyValuePair in _listViewSubItemAccessibleObjects)
                {
                    if (keyValuePair.Value == fakeAccessbileObject)
                    {
                        return keyValuePair.Key;
                    }
                }

                return -1;
            }

            private int GetLastTileChildIndex()
            {
                Debug.Assert(_owningListView.View == View.Tile, $"{nameof(GetLastTileChildIndex)} method should only be called for 'Tile' view");

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

            internal Rectangle GetSubItemBounds(int subItemIndex)
                => _owningListView.IsHandleCreated
                    ? _owningListView.GetSubItemRect(_owningItem.Index, subItemIndex)
                    : Rectangle.Empty;

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
