// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable disable

using static Interop;

namespace System.Windows.Forms
{
    public partial class NumericUpDown
    {
        internal class NumericUpDownAccessibleObject : ControlAccessibleObject
        {
            private readonly UpDownBase _owningNumericUpDown;

            public NumericUpDownAccessibleObject(NumericUpDown owner) : base(owner)
            {
                _owningNumericUpDown = owner;
            }

            public override AccessibleObject GetChild(int index)
            {
                // TextBox child
                if (index == 0)
                {
                    return _owningNumericUpDown.TextBox.AccessibilityObject.Parent;
                }

                // Up/down buttons
                if (index == 1)
                {
                    return _owningNumericUpDown.UpDownButtonsInternal.AccessibilityObject.Parent;
                }

                return null;
            }

            public override int GetChildCount()
            {
                return 2;
            }

            internal override object GetPropertyValue(UiaCore.UIA propertyID)
            {
                switch (propertyID)
                {
                    case UiaCore.UIA.LegacyIAccessibleStatePropertyId:
                        return State;
                    case UiaCore.UIA.LegacyIAccessibleRolePropertyId:
                        return Role;
                    default:
                        return base.GetPropertyValue(propertyID);
                }
            }

            public override AccessibleRole Role
            {
                get
                {
                    AccessibleRole role = Owner.AccessibleRole;

                    if (role != AccessibleRole.Default)
                    {
                        return role;
                    }

                    return AccessibleRole.SpinButton;
                }
            }

            // We need to provide a unique ID. Others are implementing this in the same manner. First item is static - 0x2a (RuntimeIDFirstItem).
            // Second item can be anything, but it's good to supply HWND.
            internal override int[] RuntimeId
                => new int[]
                {
                    RuntimeIDFirstItem,
                    PARAM.ToInt(_owningNumericUpDown.InternalHandle),
                    _owningNumericUpDown.GetHashCode()
                };
        }
    }
}
