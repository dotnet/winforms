// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using static Interop;

namespace System.Windows.Forms
{
    public partial class ComboBox
    {
        /// <summary>
        ///  Represents the ComboBox's child text (is used instead of inner Edit when style is DropDownList but not DropDown) accessible object.
        /// </summary>
        internal class ComboBoxChildTextUiaProvider : AccessibleObject
        {
            private const int COMBOBOX_TEXT_ACC_ITEM_INDEX = 1;

            private readonly ComboBox _owner;

            /// <summary>
            ///  Initializes new instance of ComboBoxChildTextUiaProvider.
            /// </summary>
            /// <param name="owner">The owning ComboBox control.</param>
            public ComboBoxChildTextUiaProvider(ComboBox owner)
            {
                _owner = owner;
            }

            /// <summary>
            ///  Gets the bounds.
            /// </summary>
            public override Rectangle Bounds
            {
                get
                {
                    return _owner.AccessibilityObject.Bounds;
                }
            }

            /// <summary>
            ///  Gets the child ID.
            /// </summary>
            /// <returns>The child ID.</returns>
            internal override int GetChildId()
            {
                return COMBOBOX_TEXT_ACC_ITEM_INDEX;
            }

            /// <summary>
            ///  Gets or sets the accessible Name of ComboBox's child text element.
            /// </summary>
            [AllowNull]
            public override string Name
            {
                get
                {
                    return _owner.AccessibilityObject.Name ?? string.Empty;
                }
                set
                {
                    // Do nothing.
                }
            }

            /// <summary>
            ///  Returns the element in the specified direction.
            /// </summary>
            /// <param name="direction">Indicates the direction in which to navigate.</param>
            /// <returns>Returns the element in the specified direction.</returns>
            internal override UiaCore.IRawElementProviderFragment? FragmentNavigate(UiaCore.NavigateDirection direction)
            {
                if (!_owner.IsHandleCreated)
                {
                    return null;
                }

                switch (direction)
                {
                    case UiaCore.NavigateDirection.Parent:
                        return _owner.AccessibilityObject;
                    case UiaCore.NavigateDirection.NextSibling:
                        return _owner.AccessibilityObject is ComboBoxAccessibleObject comboBoxAccessibleObject
                            ? comboBoxAccessibleObject.DropDownButtonUiaProvider
                            : null;
                    case UiaCore.NavigateDirection.PreviousSibling:
                        return _owner.DroppedDown
                            ? _owner.ChildListAccessibleObject
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
                    return _owner.AccessibilityObject;
                }
            }

            /// <summary>
            ///  Gets the accessible property value.
            /// </summary>
            /// <param name="propertyID">The accessible property ID.</param>
            /// <returns>The accessible property value.</returns>
            internal override object? GetPropertyValue(UiaCore.UIA propertyID) =>
                propertyID switch
                {
                    UiaCore.UIA.ControlTypePropertyId => UiaCore.UIA.TextControlTypeId,
                    UiaCore.UIA.HasKeyboardFocusPropertyId => _owner.Focused,
                    UiaCore.UIA.IsEnabledPropertyId => _owner.Enabled,
                    UiaCore.UIA.IsKeyboardFocusablePropertyId => (State & AccessibleStates.Focusable) == AccessibleStates.Focusable,
                    UiaCore.UIA.IsOffscreenPropertyId => false,
                    _ => base.GetPropertyValue(propertyID)
                };

            /// <summary>
            ///  Gets the runtime ID.
            /// </summary>
            internal override int[] RuntimeId
                => new int[]
                {
                    RuntimeIDFirstItem,
                    PARAM.ToInt(_owner.InternalHandle),
                    _owner.GetHashCode(),
                    GetHashCode(),
                    GetChildId()
                };

            /// <summary>
            ///  Gets the accessible state.
            /// </summary>
            public override AccessibleStates State
            {
                get
                {
                    AccessibleStates state = AccessibleStates.Focusable;
                    if (_owner.Focused)
                    {
                        state |= AccessibleStates.Focused;
                    }

                    return state;
                }
            }
        }
    }
}
