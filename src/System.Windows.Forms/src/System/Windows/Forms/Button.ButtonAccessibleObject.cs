// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using static Interop.UiaCore;

namespace System.Windows.Forms
{
    public partial class Button
    {
        internal class ButtonAccessibleObject : ButtonBaseAccessibleObject
        {
            public ButtonAccessibleObject(Button owner) : base(owner)
            {
            }

            internal override object? GetPropertyValue(UIA propertyID)
                => propertyID switch
                {
                    UIA.NamePropertyId
                        => Name,
                    UIA.AutomationIdPropertyId
                        => Owner.Name,
                    UIA.ControlTypePropertyId
                        => UIA.ButtonControlTypeId,
                    UIA.IsKeyboardFocusablePropertyId
                        =>
                        // This is necessary for compatibility with MSAA proxy:
                        // IsKeyboardFocusable = true regardless the control is enabled/disabled.
                        true,
                    UIA.HasKeyboardFocusPropertyId
                        => Owner.Focused,
                    _ => base.GetPropertyValue(propertyID)
                };
        }
    }
}
