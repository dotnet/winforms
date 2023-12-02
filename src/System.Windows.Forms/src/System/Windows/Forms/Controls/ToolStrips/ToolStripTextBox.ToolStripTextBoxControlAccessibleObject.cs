// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Windows.Win32.System.Variant;
using Windows.Win32.UI.Accessibility;
using static System.Windows.Forms.TextBoxBase;

namespace System.Windows.Forms;

public partial class ToolStripTextBox
{
    private class ToolStripTextBoxControlAccessibleObject : TextBoxBaseAccessibleObject
    {
        public ToolStripTextBoxControlAccessibleObject(TextBox toolStripHostedControl) : base(toolStripHostedControl)
        { }

        internal override VARIANT GetPropertyValue(UIA_PROPERTY_ID propertyID)
            => propertyID switch
            {
                // If we don't set a default role for the accessible object it will be retrieved from Windows.
                // And we don't have a 100% guarantee it will be correct, hence set it ourselves.
                UIA_PROPERTY_ID.UIA_ControlTypePropertyId when this.GetOwnerAccessibleRole() == AccessibleRole.Default
                    => (VARIANT)(int)UIA_CONTROLTYPE_ID.UIA_EditControlTypeId,
                UIA_PROPERTY_ID.UIA_HasKeyboardFocusPropertyId => (VARIANT)State.HasFlag(AccessibleStates.Focused),
                UIA_PROPERTY_ID.UIA_IsOffscreenPropertyId when this.TryGetOwnerAs(out TextBox? owner) => (VARIANT)GetIsOffscreenPropertyValue(owner.ToolStripControlHost?.Placement, Bounds),
                _ => base.GetPropertyValue(propertyID)
            };
    }
}
