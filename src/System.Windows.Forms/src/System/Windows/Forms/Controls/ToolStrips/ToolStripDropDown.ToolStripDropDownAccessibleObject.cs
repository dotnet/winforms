// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Windows.Win32.System.Variant;
using Windows.Win32.UI.Accessibility;

namespace System.Windows.Forms;

public partial class ToolStripDropDown
{
    public class ToolStripDropDownAccessibleObject : ToolStripAccessibleObject
    {
        public ToolStripDropDownAccessibleObject(ToolStripDropDown owner) : base(owner)
        {
        }

        internal override VARIANT GetPropertyValue(UIA_PROPERTY_ID propertyID) =>
            propertyID switch
            {
                UIA_PROPERTY_ID.UIA_IsKeyboardFocusablePropertyId => (VARIANT)State.HasFlag(AccessibleStates.Focusable),
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

        internal override bool CanGetNameInternal =>
            IsInternal
            && this.TryGetOwnerAs(out ToolStripDropDown? owner)
            && owner.AccessibleName is null
            && (owner?.OwnerItem?.AccessibilityObject.CanGetNameInternal ?? true);

        internal override BSTR GetNameInternal()
        {
            this.TryGetOwnerAs(out ToolStripDropDown? owner);
            Debug.Assert(owner is not null);
            return owner?.OwnerItem?.AccessibilityObject.GetNameInternal() ?? default;
        }

        public override AccessibleRole Role => this.GetOwnerAccessibleRole(AccessibleRole.MenuPopup);
    }
}
