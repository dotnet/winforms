// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Windows.Win32.System.Variant;
using Windows.Win32.UI.Accessibility;

namespace System.Windows.Forms;

public partial class ToolStripPanel
{
    internal class ToolStripPanelAccessibleObject : ControlAccessibleObject
    {
        public ToolStripPanelAccessibleObject(ToolStripPanel owner) : base(owner)
        {
        }

        public override AccessibleRole Role => this.GetOwnerAccessibleRole(AccessibleRole.Client);

        internal override VARIANT GetPropertyValue(UIA_PROPERTY_ID propertyId) => propertyId switch
        {
            UIA_PROPERTY_ID.UIA_IsKeyboardFocusablePropertyId => (VARIANT)(this.TryGetOwnerAs(out ToolStripPanel? owner) && owner.CanFocus),
            _ => base.GetPropertyValue(propertyId)
        };
    }
}
