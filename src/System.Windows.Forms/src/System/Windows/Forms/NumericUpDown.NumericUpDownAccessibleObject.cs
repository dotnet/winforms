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
                    case UiaCore.UIA.RuntimeIdPropertyId:
                        return RuntimeId;
                    case UiaCore.UIA.NamePropertyId:
                        return Name;
                    case UiaCore.UIA.BoundingRectanglePropertyId:
                        return Bounds;
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

            internal override int[] RuntimeId
            {
                get
                {
                    if (_owningNumericUpDown is null)
                    {
                        return base.RuntimeId;
                    }

                    // we need to provide a unique ID
                    // others are implementing this in the same manner
                    // first item is static - 0x2a (RuntimeIDFirstItem)
                    // second item can be anything, but here it is a hash

                    var runtimeId = new int[3];
                    runtimeId[0] = RuntimeIDFirstItem;
                    runtimeId[1] = (int)(long)_owningNumericUpDown.InternalHandle;
                    runtimeId[2] = _owningNumericUpDown.GetHashCode();

                    return runtimeId;
                }
            }
        }
    }
}
