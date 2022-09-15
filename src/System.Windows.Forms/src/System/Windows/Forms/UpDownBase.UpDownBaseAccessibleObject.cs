// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms
{
    public abstract partial class UpDownBase
    {
        internal class UpDownBaseAccessibleObject : ControlAccessibleObject
        {
            public UpDownBaseAccessibleObject(UpDownBase owner) : base(owner)
            { }

            public override AccessibleObject? GetChild(int index)
            {
                if (Owner is not UpDownBase owner)
                {
                    return null;
                }

                return index switch
                {
                    // TextBox child
                    0 => owner.TextBox.AccessibilityObject.Parent,
                    // Up/down buttons
                    1 => owner.UpDownButtonsInternal.AccessibilityObject.Parent,
                    _ => null,
                };
            }

            public override int GetChildCount() => 2;

            public override AccessibleRole Role
            {
                get
                {
                    AccessibleRole role = Owner.AccessibleRole;

                    return role != AccessibleRole.Default
                        ? role
                        : AccessibleRole.SpinButton;
                }
            }
        }
    }
}
