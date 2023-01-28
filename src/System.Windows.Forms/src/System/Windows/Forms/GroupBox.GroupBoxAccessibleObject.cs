// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using static Interop;

namespace System.Windows.Forms
{
    public partial class GroupBox
    {
        internal class GroupBoxAccessibleObject : ControlAccessibleObject
        {
            internal GroupBoxAccessibleObject(GroupBox owner) : base(owner)
            {
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

                    return AccessibleRole.Grouping;
                }
            }

            internal override object? GetPropertyValue(UiaCore.UIA propertyID)
                => propertyID switch
                {
                    UiaCore.UIA.IsKeyboardFocusablePropertyId => true,
                    _ => base.GetPropertyValue(propertyID)
                };
        }
    }
}
