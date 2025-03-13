// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Windows.Win32.System.Variant;
using Windows.Win32.UI.Accessibility;

namespace System.Windows.Forms;

public partial class GroupBox
{
    internal class GroupBoxAccessibleObject : ControlAccessibleObject
    {
        internal GroupBoxAccessibleObject(GroupBox owner) : base(owner)
        {
        }

        public override AccessibleRole Role => this.GetOwnerAccessibleRole(AccessibleRole.Grouping);

        internal override VARIANT GetPropertyValue(UIA_PROPERTY_ID propertyID)
            => propertyID switch
            {
                UIA_PROPERTY_ID.UIA_IsKeyboardFocusablePropertyId => VARIANT.True,
                _ => base.GetPropertyValue(propertyID)
            };
    }
}
