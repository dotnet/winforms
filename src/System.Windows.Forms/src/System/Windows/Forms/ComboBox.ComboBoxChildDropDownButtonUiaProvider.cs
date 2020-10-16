// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable disable

using System.Drawing;
using static Interop;

namespace System.Windows.Forms
{
    public partial class ComboBox
    {
        /// <summary>
        ///  Represents the ComboBox child (inner) DropDown button accessible object with UI Automation functionality.
        /// </summary>
        internal class ComboBoxChildDropDownButtonUiaProvider : AccessibleObject
        {
            private const int COMBOBOX_DROPDOWN_BUTTON_ACC_ITEM_INDEX = 2;
            private readonly ComboBox _owner;

            /// <summary>
            ///  Initializes new instance of ComboBoxChildDropDownButtonUiaProvider.
            /// </summary>
            /// <param name="owner">The owning ComboBox control.</param>
            /// <param name="comboBoxControlhandle">The owning ComboBox control's handle.</param>
            public ComboBoxChildDropDownButtonUiaProvider(ComboBox owner, IntPtr comboBoxControlhandle)
            {
                _owner = owner;
                UseStdAccessibleObjects(comboBoxControlhandle);
            }

            /// <summary>
            ///  Gets or sets the accessible Name of ComboBox's child DropDown button. ("Open" or "Close" depending on stat of the DropDown)
            /// </summary>
            public override string Name
            {
                get
                {
                    return get_accNameInternal(COMBOBOX_DROPDOWN_BUTTON_ACC_ITEM_INDEX);
                }
                set
                {
                    var systemIAccessible = GetSystemIAccessibleInternal();
                    systemIAccessible?.set_accName(COMBOBOX_DROPDOWN_BUTTON_ACC_ITEM_INDEX, value);
                }
            }

            /// <summary>
            ///  Gets the DropDown button bounds.
            /// </summary>
            public override Rectangle Bounds
            {
                get
                {
                    int left = 0;
                    int top = 0;
                    int width = 0;
                    int height = 0;
                    var systemIAccessible = GetSystemIAccessibleInternal();
                    systemIAccessible?.accLocation(out left, out top, out width, out height, COMBOBOX_DROPDOWN_BUTTON_ACC_ITEM_INDEX);
                    return new Rectangle(left, top, width, height);
                }
            }

            /// <summary>
            ///  Gets the DropDown button default action.
            /// </summary>
            public override string DefaultAction
            {
                get
                {
                    var systemIAccessible = GetSystemIAccessibleInternal();
                    return systemIAccessible?.accDefaultAction[COMBOBOX_DROPDOWN_BUTTON_ACC_ITEM_INDEX];
                }
            }

            /// <summary>
            ///  Returns the element in the specified direction.
            /// </summary>
            /// <param name="direction">Indicates the direction in which to navigate.</param>
            /// <returns>Returns the element in the specified direction.</returns>
            internal override UiaCore.IRawElementProviderFragment FragmentNavigate(UiaCore.NavigateDirection direction)
            {
                if (direction == UiaCore.NavigateDirection.Parent)
                {
                    return _owner.AccessibilityObject;
                }
                else if (direction == UiaCore.NavigateDirection.PreviousSibling)
                {
                    if (_owner.AccessibilityObject is ComboBoxAccessibleObject comboBoxAccessibleObject)
                    {
                        int comboBoxChildFragmentCount = comboBoxAccessibleObject.GetChildFragmentCount();
                        if (comboBoxChildFragmentCount > 1)
                        { // Text or edit is previous;
                            return comboBoxAccessibleObject.GetChildFragment(comboBoxChildFragmentCount - 1);
                        }
                    }

                    return null;
                }

                return base.FragmentNavigate(direction);
            }

            /// <summary>
            ///  Gets the top level element.
            /// </summary>
            internal override UiaCore.IRawElementProviderFragmentRoot FragmentRoot
            {
                get
                {
                    return _owner.AccessibilityObject;
                }
            }

            /// <summary>
            ///  Gets the child accessible object ID.
            /// </summary>
            /// <returns>The child accessible object ID.</returns>
            internal override int GetChildId()
            {
                return COMBOBOX_DROPDOWN_BUTTON_ACC_ITEM_INDEX;
            }

            /// <summary>
            ///  Gets the accessible property value.
            /// </summary>
            /// <param name="propertyID">The accessible property ID.</param>
            /// <returns>The accessible property value.</returns>
            internal override object GetPropertyValue(UiaCore.UIA propertyID)
            {
                switch (propertyID)
                {
                    case UiaCore.UIA.RuntimeIdPropertyId:
                        return RuntimeId;
                    case UiaCore.UIA.BoundingRectanglePropertyId:
                        return BoundingRectangle;
                    case UiaCore.UIA.ControlTypePropertyId:
                        return UiaCore.UIA.ButtonControlTypeId;
                    case UiaCore.UIA.NamePropertyId:
                        return Name;
                    case UiaCore.UIA.AccessKeyPropertyId:
                        return KeyboardShortcut;
                    case UiaCore.UIA.HasKeyboardFocusPropertyId:
                        return _owner.Focused;
                    case UiaCore.UIA.IsKeyboardFocusablePropertyId:
                        return (State & AccessibleStates.Focusable) == AccessibleStates.Focusable;
                    case UiaCore.UIA.IsEnabledPropertyId:
                        return _owner.Enabled;
                    case UiaCore.UIA.HelpTextPropertyId:
                        return Help ?? string.Empty;
                    case UiaCore.UIA.IsPasswordPropertyId:
                        return false;
                    case UiaCore.UIA.IsOffscreenPropertyId:
                        return (State & AccessibleStates.Offscreen) == AccessibleStates.Offscreen;
                    default:
                        return base.GetPropertyValue(propertyID);
                }
            }

            /// <summary>
            ///  Gets the help text.
            /// </summary>
            public override string Help
            {
                get
                {
                    var systemIAccessible = GetSystemIAccessibleInternal();
                    return systemIAccessible?.accHelp[COMBOBOX_DROPDOWN_BUTTON_ACC_ITEM_INDEX];
                }
            }

            /// <summary>
            ///  Gets the keyboard shortcut.
            /// </summary>
            public override string KeyboardShortcut
            {
                get
                {
                    var systemIAccessible = GetSystemIAccessibleInternal();
                    return systemIAccessible?.get_accKeyboardShortcut(COMBOBOX_DROPDOWN_BUTTON_ACC_ITEM_INDEX);
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
                    patternId == UiaCore.UIA.InvokePatternId)
                {
                    return true;
                }

                return base.IsPatternSupported(patternId);
            }

            /// <summary>
            ///  Gets the accessible role.
            /// </summary>
            public override AccessibleRole Role
            {
                get
                {
                    var systemIAccessible = GetSystemIAccessibleInternal();
                    var accRole = systemIAccessible?.get_accRole(COMBOBOX_DROPDOWN_BUTTON_ACC_ITEM_INDEX);
                    return accRole != null
                        ? (AccessibleRole)accRole
                        : AccessibleRole.None;
                }
            }

            /// <summary>
            ///  Gets the runtime ID.
            /// </summary>
            internal override int[] RuntimeId
            {
                get
                {
                    var runtimeId = new int[5];
                    runtimeId[0] = RuntimeIDFirstItem;
                    runtimeId[1] = (int)(long)_owner.InternalHandle;
                    runtimeId[2] = _owner.GetHashCode();

                    // Made up constant from MSAA proxy. When MSAA proxy is used as an accessibility provider,
                    // the similar Runtime ID is returned (for consistency purpose)
                    const int generatedRuntimeId = 61453;
                    runtimeId[3] = generatedRuntimeId;
                    runtimeId[4] = COMBOBOX_DROPDOWN_BUTTON_ACC_ITEM_INDEX;
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
                    var systemIAccessible = GetSystemIAccessibleInternal();
                    var accState = systemIAccessible?.get_accState(COMBOBOX_DROPDOWN_BUTTON_ACC_ITEM_INDEX);
                    return accState != null
                        ? (AccessibleStates)accState
                        : AccessibleStates.None;
                }
            }
        }
    }
}
