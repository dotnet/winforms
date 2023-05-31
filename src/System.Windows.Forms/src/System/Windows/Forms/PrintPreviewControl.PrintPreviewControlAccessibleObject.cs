// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Drawing;
using static Interop;

namespace System.Windows.Forms;

public partial class PrintPreviewControl
{
    internal class PrintPreviewControlAccessibleObject : ControlAccessibleObject
    {
        public PrintPreviewControlAccessibleObject(PrintPreviewControl owner) : base(owner)
        {
        }

        internal override object? GetPropertyValue(UiaCore.UIA propertyID)
            => !this.TryGetOwnerAs(out PrintPreviewControl? owner) ? null : propertyID switch
            {
                UiaCore.UIA.AutomationIdPropertyId => owner.Name,
                UiaCore.UIA.HasKeyboardFocusPropertyId => owner.Focused,
                UiaCore.UIA.IsKeyboardFocusablePropertyId => (State & AccessibleStates.Focusable) == AccessibleStates.Focusable,
                _ => base.GetPropertyValue(propertyID)
            };

        internal override Rectangle BoundingRectangle
            => this.TryGetOwnerAs(out PrintPreviewControl? owner) && owner.IsHandleCreated && owner.Parent is not null
                ? owner.GetToolNativeScreenRectangle()
                : Rectangle.Empty;
    }
}
