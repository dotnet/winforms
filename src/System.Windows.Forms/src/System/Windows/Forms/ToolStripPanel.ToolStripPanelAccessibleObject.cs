// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using static Interop;

namespace System.Windows.Forms;

public partial class ToolStripPanel
{
    internal class ToolStripPanelAccessibleObject : ControlAccessibleObject
    {
        public ToolStripPanelAccessibleObject(ToolStripPanel owner) : base(owner)
        {
        }

        public override AccessibleRole Role => this.GetOwnerAccessibleRole(AccessibleRole.Client);

        internal override object? GetPropertyValue(UiaCore.UIA propertyId) => propertyId switch
        {
            UiaCore.UIA.IsKeyboardFocusablePropertyId => this.TryGetOwnerAs(out ToolStripPanel? owner) && owner.CanFocus,
            _ => base.GetPropertyValue(propertyId)
        };
    }
}
