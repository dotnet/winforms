// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using static Interop;

namespace System.Windows.Forms
{
    public partial class PrintPreviewControl
    {
        internal class PrintPreviewControlAccessibleObject : ControlAccessibleObject
        {
            private readonly PrintPreviewControl _owningPrintPreviewControl;

            public PrintPreviewControlAccessibleObject(PrintPreviewControl owner) : base(owner)
            {
                _owningPrintPreviewControl = owner;
            }

            internal override object? GetPropertyValue(UiaCore.UIA propertyID)
                => propertyID switch
                {
                    UiaCore.UIA.AutomationIdPropertyId
                        => Owner.Name,
                    UiaCore.UIA.HasKeyboardFocusPropertyId
                        => _owningPrintPreviewControl.Focused,
                    UiaCore.UIA.IsKeyboardFocusablePropertyId
                        => (State & AccessibleStates.Focusable) == AccessibleStates.Focusable,
                    _ => base.GetPropertyValue(propertyID)
                };
        }
    }
}
