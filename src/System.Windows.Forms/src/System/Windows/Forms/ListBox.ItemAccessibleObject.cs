// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Accessibility;

using System.Drawing;
using System.Runtime.InteropServices;

namespace System.Windows.Forms
{
    public partial class ListBox
    {
        /// <summary>
        ///  ListBox control accessible object with UI Automation provider functionality.
        ///  This inherits from the base ListBoxExAccessibleObject and ListBoxAccessibleObject
        ///  to have all base functionality.
        /// </summary>
        [ComVisible(true)]
        internal class ListBoxItemAccessibleObject : AccessibleObject
        {
            private readonly ItemArray.Entry _itemEntry;
            private readonly ListBoxAccessibleObject _owningAccessibleObject;
            private readonly ListBox _owningListBox;
            private readonly IAccessible _systemIAccessible;

            public ListBoxItemAccessibleObject(ListBox owningListBox, object itemEntry, ListBoxAccessibleObject owningAccessibleObject)
            {
                _owningListBox = owningListBox;
                _itemEntry = (ItemArray.Entry)itemEntry;
                _owningAccessibleObject = owningAccessibleObject;
                _systemIAccessible = owningAccessibleObject.GetSystemIAccessibleInternal();
            }

            private int CurrentIndex => Array.IndexOf(_owningListBox.Items.InnerArray.Entries as Array, _itemEntry);

            internal override UnsafeNativeMethods.IRawElementProviderFragmentRoot FragmentRoot => _owningAccessibleObject;

            internal override bool IsItemSelected
            {
                get
                {
                    return (State & AccessibleStates.Selected) != 0;
                }
            }

            internal override UnsafeNativeMethods.IRawElementProviderSimple ItemSelectionContainer => _owningAccessibleObject;

            /// <summary>
            ///  Gets the runtime ID.
            /// </summary>
            internal override int[] RuntimeId
            {
                get
                {
                    var runtimeId = new int[4];

                    runtimeId[0] = _owningAccessibleObject.RuntimeId[0];
                    runtimeId[1] = _owningAccessibleObject.RuntimeId[1];
                    runtimeId[2] = _owningAccessibleObject.RuntimeId[2];
                    runtimeId[3] = _itemEntry.GetHashCode();

                    return runtimeId;
                }
            }

            /// <summary>
            ///  Gets the ListBox Item bounds.
            /// </summary>
            public override Rectangle Bounds
            {
                get
                {
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
            ///  Gets the ListBox item default action.
            /// </summary>
            public override string DefaultAction => _systemIAccessible.accDefaultAction[GetChildId()];

            /// <summary>
            ///  Gets the help text.
            /// </summary>
            public override string Help => _systemIAccessible.accHelp[GetChildId()];

            /// <summary>
            ///  Gets or sets the accessible name.
            /// </summary>
            public override string Name
            {
                get
                {
                    return _itemEntry.item.ToString();
                }
                set
                {
                    base.Name = value;
                }
            }

            /// <summary>
            ///  Gets the accessible role.
            /// </summary>
            public override AccessibleRole Role => (AccessibleRole)_systemIAccessible.get_accRole(GetChildId());

            /// <summary>
            ///  Gets the accessible state.
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

                    return state |= (AccessibleStates)(_systemIAccessible.get_accState(GetChildId()));
                }
            }

            internal override void AddToSelection()
            {
                SelectItem();
            }

            public override void DoDefaultAction()
            {
                SetFocus();
            }

            internal override UnsafeNativeMethods.IRawElementProviderFragment FragmentNavigate(UnsafeNativeMethods.NavigateDirection direction)
            {
                int firstItemIndex = 0;
                int lastItemIndex = _owningListBox.Items.Count - 1;
                int currentIndex = CurrentIndex;

                switch (direction)
                {
                    case UnsafeNativeMethods.NavigateDirection.Parent:
                        return _owningListBox.AccessibilityObject;
                    case UnsafeNativeMethods.NavigateDirection.PreviousSibling:
                        if (currentIndex > firstItemIndex && currentIndex <= lastItemIndex)
                        {
                            return _owningAccessibleObject.GetChild(currentIndex - 1);
                        }
                        return null;
                    case UnsafeNativeMethods.NavigateDirection.NextSibling:
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
                return CurrentIndex + 1; // Index is zero-based, Child ID is 1-based.
            }

            internal override object GetPropertyValue(int propertyID)
            {
                switch (propertyID)
                {
                    case NativeMethods.UIA_RuntimeIdPropertyId:
                        return RuntimeId;
                    case NativeMethods.UIA_BoundingRectanglePropertyId:
                        return Bounds;
                    case NativeMethods.UIA_ControlTypePropertyId:
                        return NativeMethods.UIA_ListItemControlTypeId;
                    case NativeMethods.UIA_NamePropertyId:
                        return Name;
                    case NativeMethods.UIA_AccessKeyPropertyId:
                        return string.Empty;
                    case NativeMethods.UIA_HasKeyboardFocusPropertyId:
                        return _owningListBox.Focused && _owningListBox.FocusedIndex == CurrentIndex;
                    case NativeMethods.UIA_IsKeyboardFocusablePropertyId:
                        return (State & AccessibleStates.Focusable) == AccessibleStates.Focusable;
                    case NativeMethods.UIA_IsEnabledPropertyId:
                        return _owningListBox.Enabled;
                    case NativeMethods.UIA_HelpTextPropertyId:
                        return Help ?? string.Empty;
                    case NativeMethods.UIA_IsPasswordPropertyId:
                        return false;
                    case NativeMethods.UIA_NativeWindowHandlePropertyId:
                        return _owningListBox.Handle;
                    case NativeMethods.UIA_IsOffscreenPropertyId:
                        return (State & AccessibleStates.Offscreen) == AccessibleStates.Offscreen;
                    case NativeMethods.UIA_IsSelectionItemPatternAvailablePropertyId:
                        return IsPatternSupported(NativeMethods.UIA_SelectionItemPatternId);
                    case NativeMethods.UIA_IsScrollItemPatternAvailablePropertyId:
                        return IsPatternSupported(NativeMethods.UIA_ScrollItemPatternId);
                    default:
                        return base.GetPropertyValue(propertyID);
                }
            }

            /// <summary>
            ///  Indicates whether specified pattern is supported.
            /// </summary>
            /// <param name="patternId">The pattern ID.</param>
            /// <returns>True if specified </returns>
            internal override bool IsPatternSupported(int patternId)
            {
                if (patternId == NativeMethods.UIA_ScrollItemPatternId ||
                    patternId == NativeMethods.UIA_LegacyIAccessiblePatternId ||
                    patternId == NativeMethods.UIA_SelectionItemPatternId)
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
                int currentIndex = CurrentIndex;

                if (_owningListBox.SelectedIndex == -1) //no item selected
                {
                    _owningListBox.SendMessage(NativeMethods.LB_SETCARETINDEX, currentIndex, 0);
                    return;
                }

                int firstVisibleIndex = _owningListBox.SendMessage(NativeMethods.LB_GETTOPINDEX, 0, 0).ToInt32();

                if (currentIndex < firstVisibleIndex)
                {
                    _owningListBox.SendMessage(NativeMethods.LB_SETTOPINDEX, currentIndex, 0);
                    return;
                }

                int itemsHeightSum = 0;
                int visibleItemsCount = 0;
                int listBoxHeight = _owningListBox.ClientRectangle.Height;
                int itemsCount = _owningListBox.Items.Count;

                for (int i = firstVisibleIndex; i < itemsCount; i++)
                {
                    int itemHeight = _owningListBox.SendMessage(NativeMethods.LB_GETITEMHEIGHT, i, 0).ToInt32();

                    if ((itemsHeightSum += itemHeight) <= listBoxHeight)
                    {
                        continue;
                    }

                    int lastVisibleIndex = i - 1; // - 1 because last "i" index is invisible
                    visibleItemsCount = lastVisibleIndex - firstVisibleIndex + 1; // + 1 because array indexes begin with 0

                    if (currentIndex > lastVisibleIndex)
                    {
                        _owningListBox.SendMessage(NativeMethods.LB_SETTOPINDEX, currentIndex - visibleItemsCount + 1, 0);
                    }

                    break;
                }
            }

            internal override void SelectItem()
            {
                _owningListBox.SelectedIndex = CurrentIndex;

                SafeNativeMethods.InvalidateRect(new HandleRef(this, _owningListBox.Handle), null, false);
                RaiseAutomationEvent(NativeMethods.UIA_AutomationFocusChangedEventId);
                RaiseAutomationEvent(NativeMethods.UIA_SelectionItem_ElementSelectedEventId);
            }

            internal override void SetFocus()
            {
                RaiseAutomationEvent(NativeMethods.UIA_AutomationFocusChangedEventId);
                SelectItem();
            }

            public override void Select(AccessibleSelection flags)
            {
                try
                {
                    _systemIAccessible.accSelect((int)flags, GetChildId());
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
        }
    }
}
