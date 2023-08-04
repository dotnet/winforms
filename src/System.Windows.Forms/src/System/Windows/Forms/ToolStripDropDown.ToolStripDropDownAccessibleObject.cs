﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using static Interop;

namespace System.Windows.Forms;

public partial class ToolStripDropDown
{
    public class ToolStripDropDownAccessibleObject : ToolStripAccessibleObject
    {
        public ToolStripDropDownAccessibleObject(ToolStripDropDown owner) : base(owner)
        {
        }

        internal override object? GetPropertyValue(UiaCore.UIA propertyID) =>
            propertyID switch
            {
                UiaCore.UIA.IsKeyboardFocusablePropertyId => (State & AccessibleStates.Focusable) == AccessibleStates.Focusable,
                _ => base.GetPropertyValue(propertyID)
            };

        public override string? Name
        {
            get
            {
                // Special case: If an explicit name has been set in the AccessibleName property, use that.
                // Note: Any non-null value in AccessibleName overrides the default accessible name logic,
                // even an empty string (this is the only way to *force* the accessible name to be blank).
                return this.TryGetOwnerAs(out ToolStripDropDown? owner)
                    && owner.AccessibleName is { } name ? name : owner?.OwnerItem?.AccessibilityObject.Name;
            }
            set
            {
                // If anyone tries to set the accessible name, just cache the value in the control's
                // AccessibleName property. This value will then end up overriding the normal accessible
                // name logic, until such time as AccessibleName is set back to null.
                if (this.TryGetOwnerAs(out ToolStripDropDown? owner))
                {
                    owner.AccessibleName = value;
                }
            }
        }

        public override AccessibleRole Role => this.GetOwnerAccessibleRole(AccessibleRole.MenuPopup);
    }
}
