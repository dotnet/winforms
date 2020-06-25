// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using static Interop;

namespace System.Windows.Forms
{
    public partial class Splitter
    {
        internal class SplitterAccessibleObject : ControlAccessibleObject
        {
            internal SplitterAccessibleObject(Splitter owner) : base(owner)
            {
            }

            internal override object? GetPropertyValue(UiaCore.UIA propertyID)
                => propertyID switch
                {
                    UiaCore.UIA.NamePropertyId
                        => Name,
                    UiaCore.UIA.AutomationIdPropertyId
                        => Owner.Name,
                    UiaCore.UIA.ControlTypePropertyId
                        => UiaCore.UIA.PaneControlTypeId,
                    UiaCore.UIA.IsKeyboardFocusablePropertyId
                        // This is necessary for compatibility with MSAA proxy:
                        // IsKeyboardFocusable = true regardless the control is enabled/disabled.
                        => true,
                    _ => base.GetPropertyValue(propertyID)
                };
        }
    }
}
