// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Drawing;
using System.Runtime.InteropServices;
using Accessibility;
using static System.Windows.Forms.ComboBox.ObjectCollection;
using static Interop;
using static Interop.User32;

namespace System.Windows.Forms
{
    public partial class ComboBox
    {
        /// <summary>
        ///  Represents the ComboBox item accessible object.
        /// </summary>
        internal class ComboBoxItemAccessibleObject : AccessibleObject
        {
            private readonly ComboBox _owningComboBox;
            private readonly Entry _owningItem;
            private IAccessible? _systemIAccessible;

            /// <summary>
            ///  Initializes new instance of ComboBox item accessible object.
            /// </summary>
            /// <param name="owningComboBox">The owning ComboBox.</param>
            /// <param name="owningItem">The owning ComboBox item.</param>
            public ComboBoxItemAccessibleObject(ComboBox owningComboBox, Entry owningItem)
            {
                _owningComboBox = owningComboBox.OrThrowIfNull();
                _owningItem = owningItem;

                _systemIAccessible = _owningComboBox.ChildListAccessibleObject.GetSystemIAccessibleInternal();
            }

            /// <summary>
            ///  Gets the ComboBox Item bounds.
            /// </summary>
            public override Rectangle Bounds
            {
                get
                {
                    int currentIndex = GetCurrentIndex();
                    HWND listHandle = (HWND)_owningComboBox.GetListHandle();
                    RECT itemRect = new();

                    int result = (int)User32.SendMessageW(
                        listHandle,
                        (User32.WM)User32.LB.GETITEMRECT,
                        currentIndex,
                        ref itemRect);

                    if (result == User32.LB_ERR)
                    {
                        return Rectangle.Empty;
                    }

                    // Translate the item rect to screen coordinates
                    RECT translated = itemRect;
                    PInvoke.MapWindowPoints((HWND)listHandle, HWND.Null, ref translated);
                    return translated;
                }
            }

            /// <summary>
            ///  Gets the ComboBox item default action.
            /// </summary>
            public override string? DefaultAction => _systemIAccessible?.accDefaultAction[GetChildId()];

            internal override UiaCore.IRawElementProviderFragment? FragmentNavigate(UiaCore.NavigateDirection direction)
            {
                if (direction == UiaCore.NavigateDirection.Parent)
                {
                    return _owningComboBox.ChildListAccessibleObject;
                }

                if (!(_owningComboBox.ChildListAccessibleObject is ComboBoxChildListUiaProvider comboBoxChildListUiaProvider))
                {
                    return base.FragmentNavigate(direction);
                }

                switch (direction)
                {
                    case UiaCore.NavigateDirection.NextSibling:
                        int currentIndex = GetCurrentIndex();
                        int itemsCount = comboBoxChildListUiaProvider.GetChildFragmentCount();
                        int nextItemIndex = currentIndex + 1;
                        if (itemsCount > nextItemIndex)
                        {
                            return comboBoxChildListUiaProvider.GetChildFragment(nextItemIndex);
                        }

                        break;
                    case UiaCore.NavigateDirection.PreviousSibling:
                        currentIndex = GetCurrentIndex();
                        int previousItemIndex = currentIndex - 1;
                        if (previousItemIndex >= 0)
                        {
                            return comboBoxChildListUiaProvider.GetChildFragment(previousItemIndex);
                        }

                        break;
                }

                return base.FragmentNavigate(direction);
            }

            internal override UiaCore.IRawElementProviderFragmentRoot FragmentRoot => _owningComboBox.AccessibilityObject;

            private int GetCurrentIndex() => _owningComboBox.Items.InnerList.IndexOf(_owningItem);

            // Index is zero-based, Child ID is 1-based.
            internal override int GetChildId() => GetCurrentIndex() + 1;

            internal override object? GetPropertyValue(UiaCore.UIA propertyID) =>
                propertyID switch
                {
                    UiaCore.UIA.ControlTypePropertyId => UiaCore.UIA.ListItemControlTypeId,
                    UiaCore.UIA.HasKeyboardFocusPropertyId => _owningComboBox.Focused && _owningComboBox.SelectedIndex == GetCurrentIndex(),
                    UiaCore.UIA.IsContentElementPropertyId => true,
                    UiaCore.UIA.IsControlElementPropertyId => true,
                    UiaCore.UIA.IsEnabledPropertyId => _owningComboBox.Enabled,
                    UiaCore.UIA.IsKeyboardFocusablePropertyId => (State & AccessibleStates.Focusable) == AccessibleStates.Focusable,
                    UiaCore.UIA.SelectionItemIsSelectedPropertyId => (State & AccessibleStates.Selected) != 0,
                    UiaCore.UIA.SelectionItemSelectionContainerPropertyId => _owningComboBox.ChildListAccessibleObject,
                    _ => base.GetPropertyValue(propertyID)
                };

            /// <summary>
            ///  Gets the help text.
            /// </summary>
            public override string? Help => _systemIAccessible?.accHelp[GetChildId()];

            /// <summary>
            ///  Indicates whether specified pattern is supported.
            /// </summary>
            /// <param name="patternId">The pattern ID.</param>
            /// <returns>True if specified </returns>
            internal override bool IsPatternSupported(UiaCore.UIA patternId)
            {
                if (patternId == UiaCore.UIA.LegacyIAccessiblePatternId ||
                    patternId == UiaCore.UIA.InvokePatternId ||
                    patternId == UiaCore.UIA.ScrollItemPatternId ||
                    patternId == UiaCore.UIA.SelectionItemPatternId)
                {
                    return true;
                }

                return base.IsPatternSupported(patternId);
            }

            /// <summary>
            ///  Gets or sets the accessible name.
            /// </summary>
            public override string? Name
            {
                get => _owningComboBox is null ? base.Name : _owningComboBox.GetItemText(_owningItem.Item);
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
                    return accRole is not null
                        ? (AccessibleRole)accRole
                        : AccessibleRole.None;
                }
            }

            /// <summary>
            ///  Gets the runtime ID.
            /// </summary>
            internal override int[] RuntimeId
                => new int[]
                {
                    RuntimeIDFirstItem,
                    PARAM.ToInt(_owningComboBox.InternalHandle),
                    _owningComboBox.GetListNativeWindowRuntimeIdPart(),
                    _owningItem.GetHashCode()
                };

            /// <summary>
            ///  Gets the accessible state.
            /// </summary>
            public override AccessibleStates State
            {
                get
                {
                    var accState = _systemIAccessible?.get_accState(GetChildId());
                    if (accState is null)
                    {
                        return AccessibleStates.None;
                    }

                    AccessibleStates accessibleStates = (AccessibleStates)accState;

                    if (!_owningComboBox.DroppedDown || !_owningComboBox.ChildListAccessibleObject.Bounds.IntersectsWith(Bounds))
                    {
                        accessibleStates |= AccessibleStates.Offscreen;
                    }

                    return accessibleStates;
                }
            }

            internal override void ScrollIntoView()
            {
                if (!_owningComboBox.IsHandleCreated || !_owningComboBox.Enabled)
                {
                    return;
                }

                Rectangle listBounds = _owningComboBox.ChildListAccessibleObject.Bounds;
                if (listBounds.IntersectsWith(Bounds))
                {
                    // Do nothing because the item is already visible
                    return;
                }

                User32.SendMessageW(_owningComboBox, (User32.WM)User32.CB.SETTOPINDEX, GetCurrentIndex());
            }

            internal override void SetFocus()
            {
                RaiseAutomationEvent(UiaCore.UIA.AutomationFocusChangedEventId);

                base.SetFocus();
            }

            internal unsafe override void SelectItem()
            {
                if (!_owningComboBox.IsHandleCreated)
                {
                    return;
                }

                _owningComboBox.SelectedIndex = GetCurrentIndex();
                InvalidateRect(new HandleRef(this, _owningComboBox.GetListHandle()), null, false);
            }

            internal override void AddToSelection()
            {
                if (!_owningComboBox.IsHandleCreated)
                {
                    return;
                }

                SelectItem();
            }

            internal override void RemoveFromSelection()
            {
                // Do nothing, C++ implementation returns UIA_E_INVALIDOPERATION 0x80131509
            }

            internal override bool IsItemSelected => (State & AccessibleStates.Selected) != 0;

            internal override UiaCore.IRawElementProviderSimple ItemSelectionContainer => _owningComboBox.ChildListAccessibleObject;
        }
    }
}
