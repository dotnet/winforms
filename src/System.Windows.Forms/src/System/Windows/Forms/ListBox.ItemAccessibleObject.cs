// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Drawing;
using System.Runtime.InteropServices;
using Accessibility;
using static Interop;

namespace System.Windows.Forms
{
    public partial class ListBox
    {
        /// <summary>
        ///  ListBox item control accessible object with UI Automation provider functionality.
        /// </summary>
        internal class ListBoxItemAccessibleObject : AccessibleObject
        {
            private readonly ItemArray.Entry _itemEntry;
            private readonly ListBoxAccessibleObject _owningAccessibleObject;
            private readonly ListBox _owningListBox;
            private readonly IAccessible? _systemIAccessible;

            public ListBoxItemAccessibleObject(ListBox owningListBox, ItemArray.Entry itemEntry, ListBoxAccessibleObject owningAccessibleObject)
            {
                _owningListBox = owningListBox ?? throw new ArgumentNullException(nameof(owningListBox));
                _itemEntry = itemEntry ?? throw new ArgumentNullException(nameof(itemEntry));
                _owningAccessibleObject = owningAccessibleObject ?? throw new ArgumentNullException(nameof(owningAccessibleObject));
                _systemIAccessible = owningAccessibleObject.GetSystemIAccessibleInternal();
            }

            private int CurrentIndex
                => Array.IndexOf((Array)_owningListBox.Items.InnerArray.Entries, _itemEntry);

            internal override UiaCore.IRawElementProviderFragmentRoot FragmentRoot => _owningAccessibleObject;

            internal override bool IsItemSelected
            {
                get
                {
                    return (State & AccessibleStates.Selected) != 0;
                }
            }

            internal override UiaCore.IRawElementProviderSimple ItemSelectionContainer
                => _owningAccessibleObject;

            /// <summary>
            ///  Gets the runtime ID.
            /// </summary>
            internal override int[]? RuntimeId
            {
                get
                {
                    if (_owningAccessibleObject.RuntimeId is null)
                    {
                        return base.RuntimeId;
                    }

                    var runtimeId = new int[4];

                    runtimeId[0] = _owningAccessibleObject.RuntimeId[0];
                    runtimeId[1] = _owningAccessibleObject.RuntimeId[1];
                    runtimeId[2] = _owningAccessibleObject.RuntimeId[2];
                    runtimeId[3] = _itemEntry.GetHashCode();

                    return runtimeId;
                }
            }

            /// <summary>
            ///  Gets the <see cref="ListBox"/> item bounds.
            /// </summary>
            public override Rectangle Bounds
            {
                get
                {
                    if (!_owningListBox.IsHandleCreated)
                    {
                        return Rectangle.Empty;
                    }

                    Rectangle bounds = _owningListBox.GetItemRectangle(CurrentIndex);

                    if (bounds.IsEmpty)
                    {
                        return bounds;
                    }

                    bounds = _owningListBox.RectangleToScreen(bounds);
                    Rectangle visibleArea = _owningListBox.RectangleToScreen(_owningListBox.ClientRectangle);

                    if (visibleArea.Bottom < bounds.Bottom)
                    {
                        bounds.Height = visibleArea.Bottom - bounds.Top;
                    }

                    bounds.Width = visibleArea.Width;

                    return bounds;
                }
            }

            /// <summary>
            ///  Gets the <see cref="ListBox"/> item default action.
            /// </summary>
            public override string? DefaultAction
                => _systemIAccessible?.accDefaultAction[GetChildId()];

            /// <summary>
            ///  Gets the help text.
            /// </summary>
            public override string? Help
                => _systemIAccessible?.accHelp[GetChildId()];

            /// <summary>
            ///  Gets or sets the item accessible name.
            /// </summary>
            public override string? Name
            {
                get
                {
                    return _owningListBox.GetItemText(_itemEntry.item);
                }
                set => base.Name = value;
            }

            /// <summary>
            ///  Gets the accessible role.
            /// </summary>
            public override AccessibleRole Role
            {
                get
                {
                    var accRole = _systemIAccessible?.get_accRole(GetChildId());
                    return accRole != null
                        ? (AccessibleRole)accRole
                        : AccessibleRole.None;
                }
            }

            /// <summary>
            ///  Gets the item accessible state.
            /// </summary>
            public override AccessibleStates State
            {
                get
                {
                    AccessibleStates state = AccessibleStates.Selectable | AccessibleStates.Focusable;

                    if (_owningListBox.SelectedIndex == CurrentIndex)
                    {
                        return state |= AccessibleStates.Selected | AccessibleStates.Focused;
                    }

                    var systemIAccessibleState = _systemIAccessible?.get_accState(GetChildId());
                    if (systemIAccessibleState != null)
                    {
                        return state |= (AccessibleStates)systemIAccessibleState;
                    }

                    return state;
                }
            }

            internal override void AddToSelection()
            {
                if (_owningListBox.IsHandleCreated)
                {
                    SelectItem();
                }
            }

            public override void DoDefaultAction()
            {
                if (_owningListBox.IsHandleCreated)
                {
                    SetFocus();
                }
            }

            internal override UiaCore.IRawElementProviderFragment? FragmentNavigate(UiaCore.NavigateDirection direction)
            {
                int firstItemIndex = 0;
                int lastItemIndex = _owningListBox.Items.Count - 1;
                int currentIndex = CurrentIndex;

                switch (direction)
                {
                    case UiaCore.NavigateDirection.Parent:
                        return _owningListBox.AccessibilityObject;
                    case UiaCore.NavigateDirection.PreviousSibling:
                        if (currentIndex > firstItemIndex && currentIndex <= lastItemIndex)
                        {
                            return _owningAccessibleObject.GetChild(currentIndex - 1);
                        }
                        return null;
                    case UiaCore.NavigateDirection.NextSibling:
                        if (currentIndex >= firstItemIndex && currentIndex < lastItemIndex)
                        {
                            return _owningAccessibleObject.GetChild(currentIndex + 1);
                        }
                        return null;
                }

                return base.FragmentNavigate(direction);
            }

            internal override int GetChildId()
            {
                // Index is zero-based, Child ID is 1-based.
                return CurrentIndex + 1;
            }

            internal override object? GetPropertyValue(UiaCore.UIA propertyID)
                 => propertyID switch
                 {
                     UiaCore.UIA.RuntimeIdPropertyId => RuntimeId,
                     UiaCore.UIA.BoundingRectanglePropertyId => Bounds,
                     UiaCore.UIA.ControlTypePropertyId => UiaCore.UIA.ListItemControlTypeId,
                     UiaCore.UIA.NamePropertyId => Name,
                     UiaCore.UIA.AccessKeyPropertyId => string.Empty,
                     UiaCore.UIA.HasKeyboardFocusPropertyId => _owningListBox.Focused && _owningListBox.FocusedIndex == CurrentIndex,
                     UiaCore.UIA.IsKeyboardFocusablePropertyId => (State & AccessibleStates.Focusable) == AccessibleStates.Focusable,
                     UiaCore.UIA.IsEnabledPropertyId => _owningListBox.Enabled,
                     UiaCore.UIA.HelpTextPropertyId => Help ?? string.Empty,
                     UiaCore.UIA.IsPasswordPropertyId => false,
                     UiaCore.UIA.NativeWindowHandlePropertyId => _owningListBox.IsHandleCreated ? _owningListBox.Handle : IntPtr.Zero,
                     UiaCore.UIA.IsOffscreenPropertyId => (State & AccessibleStates.Offscreen) == AccessibleStates.Offscreen,
                     UiaCore.UIA.IsSelectionItemPatternAvailablePropertyId => IsPatternSupported(UiaCore.UIA.SelectionItemPatternId),
                     UiaCore.UIA.IsScrollItemPatternAvailablePropertyId => IsPatternSupported(UiaCore.UIA.ScrollItemPatternId),
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
                    patternId == UiaCore.UIA.SelectionItemPatternId)
                {
                    return true;
                }

                return base.IsPatternSupported(patternId);
            }

            internal override void RemoveFromSelection()
            {
                // Do nothing, C++ implementation returns UIA_E_INVALIDOPERATION 0x80131509
            }

            internal override void ScrollIntoView()
            {
                if (!_owningListBox.IsHandleCreated)
                {
                    return;
                }

                int currentIndex = CurrentIndex;

                if (_owningListBox.SelectedIndex == -1) //no item selected
                {
                    User32.SendMessageW(_owningListBox, (User32.WM)User32.LB.SETCARETINDEX, (IntPtr)currentIndex);
                    return;
                }

                int firstVisibleIndex = (int)(long)User32.SendMessageW(_owningListBox, (User32.WM)User32.LB.GETTOPINDEX);
                if (currentIndex < firstVisibleIndex)
                {
                    User32.SendMessageW(_owningListBox, (User32.WM)User32.LB.SETTOPINDEX, (IntPtr)currentIndex);
                    return;
                }

                int itemsHeightSum = 0;
                int listBoxHeight = _owningListBox.ClientRectangle.Height;
                int itemsCount = _owningListBox.Items.Count;

                for (int i = firstVisibleIndex; i < itemsCount; i++)
                {
                    int itemHeight = (int)(long)User32.SendMessageW(_owningListBox, (User32.WM)User32.LB.GETITEMHEIGHT, (IntPtr)i);

                    if ((itemsHeightSum += itemHeight) <= listBoxHeight)
                    {
                        continue;
                    }

                    int lastVisibleIndex = i - 1; // - 1 because last "i" index is invisible
                    int visibleItemsCount = lastVisibleIndex - firstVisibleIndex + 1; // + 1 because array indexes begin with 0

                    if (currentIndex > lastVisibleIndex)
                    {
                        User32.SendMessageW(_owningListBox, (User32.WM)User32.LB.SETTOPINDEX, (IntPtr)(currentIndex - visibleItemsCount + 1));
                    }

                    break;
                }
            }

            internal unsafe override void SelectItem()
            {
                if (!_owningListBox.IsHandleCreated)
                {
                    return;
                }

                _owningListBox.SelectedIndex = CurrentIndex;

                User32.InvalidateRect(new HandleRef(this, _owningListBox.Handle), null, BOOL.FALSE);
                RaiseAutomationEvent(UiaCore.UIA.AutomationFocusChangedEventId);
                RaiseAutomationEvent(UiaCore.UIA.SelectionItem_ElementSelectedEventId);
            }

            internal override void SetFocus()
            {
                if (!_owningListBox.IsHandleCreated)
                {
                    return;
                }

                RaiseAutomationEvent(UiaCore.UIA.AutomationFocusChangedEventId);
                SelectItem();
            }

            public override void Select(AccessibleSelection flags)
            {
                try
                {
                    _systemIAccessible?.accSelect((int)flags, GetChildId());
                }
                catch (ArgumentException)
                {
                    // In .NET Framework 1.1, the ListBox accessible children did not have any selection capability.
                    // In .NET Framework 2.0, they delegate the selection capability to OLEACC.
                    // However, OLEACC does not deal with several selection flags:
                    // ExtendSelection, AddSelection, RemoveSelection.
                    // OLEACC instead throws an ArgumentException.
                    // Since .NET Framework 2.0 API's should not throw an exception in places where
                    // .NET Framework 1.1 API's did not, we catch the ArgumentException and fail silently.
                }
            }
        }
    }
}
