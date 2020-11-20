// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable disable

using System.Diagnostics;
using System.Runtime.InteropServices;
using static Interop;

namespace System.Windows.Forms
{
    public partial class ComboBox
    {
        /// <summary>
        ///  Represents the ComboBox's child (inner) edit native window control accessible object with UI Automation provider functionality.
        /// </summary>
        internal class ComboBoxChildEditUiaProvider : ChildAccessibleObject
        {
            private const string COMBO_BOX_EDIT_AUTOMATION_ID = "1001";

            private readonly ComboBox _owner;
            private readonly IntPtr _handle;

            /// <summary>
            ///  Initializes new instance of ComboBoxChildEditUiaProvider.
            /// </summary>
            /// <param name="owner">The ComboBox owning control.</param>
            /// <param name="childEditControlhandle">The child edit native window handle.</param>
            public ComboBoxChildEditUiaProvider(ComboBox owner, IntPtr childEditControlhandle) : base(owner, childEditControlhandle)
            {
                _owner = owner;
                _handle = childEditControlhandle;
            }

            /// <summary>
            ///  Returns the element in the specified direction.
            /// </summary>
            /// <param name="direction">Indicates the direction in which to navigate.</param>
            /// <returns>Returns the element in the specified direction.</returns>
            internal override UiaCore.IRawElementProviderFragment FragmentNavigate(UiaCore.NavigateDirection direction)
            {
                if (!_owner.IsHandleCreated)
                {
                    return null;
                }

                switch (direction)
                {
                    case UiaCore.NavigateDirection.Parent:
                        Debug.WriteLine("Edit parent " + _owner.AccessibilityObject.GetPropertyValue(UiaCore.UIA.ControlTypePropertyId));
                        return _owner.AccessibilityObject;
                    case UiaCore.NavigateDirection.PreviousSibling:
                        return _owner.DroppedDown
                            ? _owner.ChildListAccessibleObject
                            : null;
                    case UiaCore.NavigateDirection.NextSibling:
                        return _owner.DropDownStyle != ComboBoxStyle.Simple
                            && _owner.AccessibilityObject is ComboBoxAccessibleObject comboBoxAccessibleObject
                                ? comboBoxAccessibleObject.DropDownButtonUiaProvider
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
            internal override object GetPropertyValue(UiaCore.UIA propertyID)
            {
                switch (propertyID)
                {
                    case UiaCore.UIA.RuntimeIdPropertyId:
                        return RuntimeId;
                    case UiaCore.UIA.BoundingRectanglePropertyId:
                        return Bounds;
                    case UiaCore.UIA.ControlTypePropertyId:
                        return UiaCore.UIA.EditControlTypeId;
                    case UiaCore.UIA.NamePropertyId:
                        return Name ?? SR.ComboBoxEditDefaultAccessibleName;
                    case UiaCore.UIA.AccessKeyPropertyId:
                        return string.Empty;
                    case UiaCore.UIA.HasKeyboardFocusPropertyId:
                        return _owner.Focused;
                    case UiaCore.UIA.IsKeyboardFocusablePropertyId:
                        return (State & AccessibleStates.Focusable) == AccessibleStates.Focusable;
                    case UiaCore.UIA.IsEnabledPropertyId:
                        return _owner.Enabled;
                    case UiaCore.UIA.AutomationIdPropertyId:
                        return COMBO_BOX_EDIT_AUTOMATION_ID;
                    case UiaCore.UIA.HelpTextPropertyId:
                        return Help ?? string.Empty;
                    case UiaCore.UIA.IsPasswordPropertyId:
                        return false;
                    case UiaCore.UIA.NativeWindowHandlePropertyId:
                        return _handle;
                    case UiaCore.UIA.IsOffscreenPropertyId:
                        return false;

                    default:
                        return base.GetPropertyValue(propertyID);
                }
            }

            internal override UiaCore.IRawElementProviderSimple HostRawElementProvider
            {
                get
                {
                    UiaCore.UiaHostProviderFromHwnd(new HandleRef(this, _handle), out UiaCore.IRawElementProviderSimple provider);
                    return provider;
                }
            }

            internal override bool IsIAccessibleExSupported() => true;

            /// <summary>
            ///  Gets the runtime ID.
            /// </summary>
            internal override int[] RuntimeId
            {
                get
                {
                    var runtimeId = new int[2];
                    runtimeId[0] = RuntimeIDFirstItem;
                    runtimeId[1] = GetHashCode();

                    return runtimeId;
                }
            }
        }
    }
}
