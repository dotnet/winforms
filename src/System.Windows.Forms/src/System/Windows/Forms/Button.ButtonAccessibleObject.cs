// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Windows.Win32.UI.Accessibility;

namespace System.Windows.Forms;

public partial class Button
{
    internal class ButtonAccessibleObject : ButtonBaseAccessibleObject
    {
        public ButtonAccessibleObject(Button owner) : base(owner)
        {
        }

        internal override object? GetPropertyValue(UIA_PROPERTY_ID propertyID)
            => propertyID switch
            {
                UIA_PROPERTY_ID.UIA_ControlTypePropertyId
                    // If we don't set a default role for Button and ButtonBase accessible objects
                    // it will be retrieved from Windows.
                    // And we don't have a 100% guarantee it will be correct, hence set it ourselves.
                    => this.TryGetOwnerAs(out Control? owner) && owner.AccessibleRole == AccessibleRole.Default
                       ? UIA_CONTROLTYPE_ID.UIA_ButtonControlTypeId
                       : base.GetPropertyValue(propertyID),
                UIA_PROPERTY_ID.UIA_HasKeyboardFocusPropertyId => this.TryGetOwnerAs(out Control? owner) && owner.Focused,
                UIA_PROPERTY_ID.UIA_IsKeyboardFocusablePropertyId
                    =>
                    // This is necessary for compatibility with MSAA proxy:
                    // IsKeyboardFocusable = true regardless the control is enabled/disabled.
                    true,
                _ => base.GetPropertyValue(propertyID)
            };
    }
}
