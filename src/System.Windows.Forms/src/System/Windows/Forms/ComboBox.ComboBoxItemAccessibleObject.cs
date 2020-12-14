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
                _owningComboBox = owningComboBox ?? throw new ArgumentNullException(nameof(owningComboBox));
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
                    ChildAccessibleObject listAccessibleObject = _owningComboBox.ChildListAccessibleObject;
                    int currentIndex = GetCurrentIndex();
                    if (_owningComboBox.IsHandleCreated)
                    {
                        int firstVisibleIndex = (int)(long)User32.SendMessageW(_owningComboBox, (User32.WM)User32.CB.GETTOPINDEX);

                        // Using the first visible index, we make an index shift, which helps to draw a rectangle with the correct position
                        currentIndex -= firstVisibleIndex;
                    }

                    Rectangle parentRect = listAccessibleObject.BoundingRectangle;
                    int left = parentRect.Left;
                    int top = parentRect.Top + _owningComboBox.ItemHeight * currentIndex;
                    int width = parentRect.Width;
                    int height = _owningComboBox.ItemHeight;

                    return new Rectangle(left, top, width, height);
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

            internal override object? GetPropertyValue(UiaCore.UIA propertyID)
            {
                switch (propertyID)
                {
                    case UiaCore.UIA.RuntimeIdPropertyId:
                        return RuntimeId;
                    case UiaCore.UIA.BoundingRectanglePropertyId:
                        return BoundingRectangle;
                    case UiaCore.UIA.ControlTypePropertyId:
                        return UiaCore.UIA.ListItemControlTypeId;
                    case UiaCore.UIA.NamePropertyId:
                        return Name;
                    case UiaCore.UIA.AccessKeyPropertyId:
                        return KeyboardShortcut ?? string.Empty;
                    case UiaCore.UIA.HasKeyboardFocusPropertyId:
                        return _owningComboBox.Focused && _owningComboBox.SelectedIndex == GetCurrentIndex();
                    case UiaCore.UIA.IsKeyboardFocusablePropertyId:
                        return (State & AccessibleStates.Focusable) == AccessibleStates.Focusable;
                    case UiaCore.UIA.IsEnabledPropertyId:
                        return _owningComboBox.Enabled;
                    case UiaCore.UIA.HelpTextPropertyId:
                        return Help ?? string.Empty;
                    case UiaCore.UIA.IsControlElementPropertyId:
                        return true;
                    case UiaCore.UIA.IsContentElementPropertyId:
                        return true;
                    case UiaCore.UIA.IsPasswordPropertyId:
                        return false;
                    case UiaCore.UIA.IsOffscreenPropertyId:
                        return (State & AccessibleStates.Offscreen) == AccessibleStates.Offscreen;
                    case UiaCore.UIA.IsSelectionItemPatternAvailablePropertyId:
                        return true;
                    case UiaCore.UIA.SelectionItemIsSelectedPropertyId:
                        return (State & AccessibleStates.Selected) != 0;
                    case UiaCore.UIA.SelectionItemSelectionContainerPropertyId:
                        return _owningComboBox.ChildListAccessibleObject;

                    default:
                        return base.GetPropertyValue(propertyID);
                }
            }

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
                    return accRole != null
                        ? (AccessibleRole)accRole
                        : AccessibleRole.None;
                }
            }

            /// <summary>
            ///  Gets the runtime ID.
            /// </summary>
            internal override int[]? RuntimeId
            {
                get
                {
                    if (!_owningComboBox.IsHandleCreated || !(_owningComboBox.AccessibilityObject is ComboBoxAccessibleObject comboBoxAccessibleObject))
                    {
                        return base.RuntimeId;
                    }

                    var runtimeId = new int[4];
                    runtimeId[0] = RuntimeIDFirstItem;
                    runtimeId[1] = (int)(long)_owningComboBox.InternalHandle;
                    runtimeId[2] = _owningComboBox.GetListNativeWindowRuntimeIdPart();
                    runtimeId[3] = _owningItem.GetHashCode();

                    return runtimeId;
                }
            }

            /// <summary>
            ///  Gets the accessible state.
            /// </summary>
            public override AccessibleStates State
            {
                get
                {
                    var accState = _systemIAccessible?.get_accState(GetChildId());
                    return accState != null
                        ? (AccessibleStates)accState
                        : AccessibleStates.None;
                }
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
                InvalidateRect(new HandleRef(this, _owningComboBox.GetListHandle()), null, BOOL.FALSE);
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
