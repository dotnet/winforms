﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.InteropServices;
using static System.Windows.Forms.ComboBox.ObjectCollection;
using static Interop;

namespace System.Windows.Forms
{
    public partial class ComboBox
    {
        /// <summary>
        ///  Represents the ComboBox's child (inner) list native window control accessible object with UI Automation provider functionality.
        /// </summary>
        internal class ComboBoxChildListUiaProvider : ChildAccessibleObject
        {
            private const string COMBO_BOX_LIST_AUTOMATION_ID = "1000";

            private readonly ComboBox _owningComboBox;
            private readonly IntPtr _childListControlhandle;

            public ComboBoxChildListUiaProvider(ComboBox owningComboBox, IntPtr childListControlhandle) : base(owningComboBox, childListControlhandle)
            {
                _owningComboBox = owningComboBox;
                _childListControlhandle = childListControlhandle;
            }

            /// <summary>
            ///  Return the child object at the given screen coordinates.
            /// </summary>
            /// <param name="x">X coordinate.</param>
            /// <param name="y">Y coordinate.</param>
            /// <returns>The accessible object of corresponding element in the provided coordinates.</returns>
            internal override UiaCore.IRawElementProviderFragment? ElementProviderFromPoint(double x, double y)
            {
                var systemIAccessible = GetSystemIAccessibleInternal();
                if (systemIAccessible is not null)
                {
                    object result = systemIAccessible.accHitTest((int)x, (int)y);
                    if (result is int childId)
                    {
                        return GetChildFragment(childId - 1);
                    }
                    else
                    {
                        return null;
                    }
                }

                return base.ElementProviderFromPoint(x, y);
            }

            /// <summary>
            ///  Request to return the element in the specified direction.
            /// </summary>
            /// <param name="direction">Indicates the direction in which to navigate.</param>
            /// <returns>Returns the element in the specified direction.</returns>
            internal override UiaCore.IRawElementProviderFragment? FragmentNavigate(UiaCore.NavigateDirection direction)
            {
                if (!_owningComboBox.IsHandleCreated)
                {
                    return null;
                }

                switch (direction)
                {
                    case UiaCore.NavigateDirection.Parent:
                        return _owningComboBox.AccessibilityObject;
                    case UiaCore.NavigateDirection.FirstChild:
                        return GetChildFragment(0);
                    case UiaCore.NavigateDirection.LastChild:
                        var childFragmentCount = GetChildFragmentCount();
                        if (childFragmentCount > 0)
                        {
                            return GetChildFragment(childFragmentCount - 1);
                        }

                        return null;
                    case UiaCore.NavigateDirection.NextSibling:
                        return _owningComboBox.DropDownStyle == ComboBoxStyle.DropDownList
                            ? _owningComboBox.ChildTextAccessibleObject
                            : _owningComboBox.ChildEditAccessibleObject;
                    case UiaCore.NavigateDirection.PreviousSibling:
                        // A workaround for an issue with an Inspect not responding. It also simulates native control behavior.
                        return _owningComboBox.DropDownStyle == ComboBoxStyle.Simple
                            ? _owningComboBox.ChildListAccessibleObject
                            : null;
                    default:
                        return base.FragmentNavigate(direction);
                }
            }

            /// <summary>
            ///  Gets the top level element.
            /// </summary>
            internal override UiaCore.IRawElementProviderFragmentRoot FragmentRoot
            {
                get
                {
                    return _owningComboBox.AccessibilityObject;
                }
            }

            public AccessibleObject? GetChildFragment(int index)
            {
                if (index < 0 || index >= _owningComboBox.Items.Count)
                {
                    return null;
                }

                if (_owningComboBox.AccessibilityObject is not  ComboBoxAccessibleObject comboBoxAccessibleObject)
                {
                    return null;
                }

                Entry item = _owningComboBox.Entries[index];

                return item is null ? null : comboBoxAccessibleObject.ItemAccessibleObjects.GetComboBoxItemAccessibleObject(item);
            }

            public int GetChildFragmentCount()
            {
                return _owningComboBox.Items.Count;
            }

            /// <summary>
            ///  Gets the accessible property value.
            /// </summary>
            /// <param name="propertyID">The accessible property ID.</param>
            /// <returns>The accessible property value.</returns>
            internal override object? GetPropertyValue(UiaCore.UIA propertyID)
            {
                switch (propertyID)
                {
                    case UiaCore.UIA.RuntimeIdPropertyId:
                        return RuntimeId;
                    case UiaCore.UIA.BoundingRectanglePropertyId:
                        return Bounds;
                    case UiaCore.UIA.ControlTypePropertyId:
                        return UiaCore.UIA.ListControlTypeId;
                    case UiaCore.UIA.NamePropertyId:
                        return Name;
                    case UiaCore.UIA.AccessKeyPropertyId:
                        return string.Empty;
                    case UiaCore.UIA.HasKeyboardFocusPropertyId:
                        return false; // Narrator should keep the keyboard focus on th ComboBox itself but not on the DropDown.
                    case UiaCore.UIA.IsKeyboardFocusablePropertyId:
                        return (State & AccessibleStates.Focusable) == AccessibleStates.Focusable;
                    case UiaCore.UIA.IsEnabledPropertyId:
                        return _owningComboBox.Enabled;
                    case UiaCore.UIA.AutomationIdPropertyId:
                        return COMBO_BOX_LIST_AUTOMATION_ID;
                    case UiaCore.UIA.HelpTextPropertyId:
                        return Help ?? string.Empty;
                    case UiaCore.UIA.IsPasswordPropertyId:
                        return false;
                    case UiaCore.UIA.NativeWindowHandlePropertyId:
                        return _childListControlhandle;
                    case UiaCore.UIA.IsOffscreenPropertyId:
                        return false;
                    case UiaCore.UIA.IsSelectionPatternAvailablePropertyId:
                        return true;
                    case UiaCore.UIA.SelectionCanSelectMultiplePropertyId:
                        return CanSelectMultiple;
                    case UiaCore.UIA.SelectionIsSelectionRequiredPropertyId:
                        return IsSelectionRequired;

                    default:
                        return base.GetPropertyValue(propertyID);
                }
            }

            internal override UiaCore.IRawElementProviderFragment? GetFocus()
            {
                return GetFocused();
            }

            public override AccessibleObject? GetFocused()
            {
                if (!_owningComboBox.IsHandleCreated)
                {
                    return null;
                }

                int selectedIndex = _owningComboBox.SelectedIndex;
                return GetChildFragment(selectedIndex);
            }

            internal override UiaCore.IRawElementProviderSimple[] GetSelection()
            {
                if (!_owningComboBox.IsHandleCreated)
                {
                    return Array.Empty<UiaCore.IRawElementProviderSimple>();
                }

                int selectedIndex = _owningComboBox.SelectedIndex;

                AccessibleObject? itemAccessibleObject = GetChildFragment(selectedIndex);

                if (itemAccessibleObject is not null)
                {
                    return new UiaCore.IRawElementProviderSimple[] {
                        itemAccessibleObject
                    };
                }

                return Array.Empty<UiaCore.IRawElementProviderSimple>();
            }

            internal override bool CanSelectMultiple
            {
                get
                {
                    return false;
                }
            }

            internal override bool IsSelectionRequired
            {
                get
                {
                    return true;
                }
            }

            /// <summary>
            ///  Indicates whether specified pattern is supported.
            /// </summary>
            /// <param name="patternId">The pattern ID.</param>
            /// <returns>True if specified </returns>
            internal override bool IsPatternSupported(UiaCore.UIA patternId)
            {
                if (patternId == UiaCore.UIA.LegacyIAccessiblePatternId ||
                    patternId == UiaCore.UIA.SelectionPatternId)
                {
                    return true;
                }

                return base.IsPatternSupported(patternId);
            }

            internal override UiaCore.IRawElementProviderSimple HostRawElementProvider
            {
                get
                {
                    UiaCore.UiaHostProviderFromHwnd(new HandleRef(this, _childListControlhandle), out UiaCore.IRawElementProviderSimple provider);
                    return provider;
                }
            }

            /// <summary>
            ///  Gets the runtime ID.
            /// </summary>
            internal override int[] RuntimeId
            {
                get
                {
                    var runtimeId = new int[3];
                    runtimeId[0] = RuntimeIDFirstItem;
                    runtimeId[1] = (int)(long)_owningComboBox.InternalHandle;
                    runtimeId[2] = _owningComboBox.GetListNativeWindowRuntimeIdPart();

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
                    AccessibleStates state = AccessibleStates.Focusable;
                    if (_owningComboBox.Focused)
                    {
                        state |= AccessibleStates.Focused;
                    }

                    return state;
                }
            }
        }
    }
}
