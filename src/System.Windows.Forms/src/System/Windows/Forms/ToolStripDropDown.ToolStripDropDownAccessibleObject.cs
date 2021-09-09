// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable disable

using static Interop;

namespace System.Windows.Forms
{
    public partial class ToolStripDropDown
    {
        public class ToolStripDropDownAccessibleObject : ToolStripAccessibleObject
        {
            private readonly ToolStripDropDown owner;

            public ToolStripDropDownAccessibleObject(ToolStripDropDown owner) : base(owner)
            {
                this.owner = owner;
            }

            internal override object GetPropertyValue(UiaCore.UIA propertyID)
            {
                switch (propertyID)
                {
                    case UiaCore.UIA.IsKeyboardFocusablePropertyId:
                        return (State & AccessibleStates.Focusable) == AccessibleStates.Focusable;
                }

                return base.GetPropertyValue(propertyID);
            }

            public override string Name
            {
                get
                {
                    // Special case: If an explicit name has been set in the AccessibleName property, use that.
                    // Note: Any non-null value in AccessibleName overrides the default accessible name logic,
                    // even an empty string (this is the only way to *force* the accessible name to be blank).
                    string name = owner.AccessibleName;
                    if (name is not null)
                    {
                        return name;
                    }

                    if (owner.OwnerItem is not null && owner.OwnerItem.AccessibilityObject.Name is not null)
                    {
                        name = owner.OwnerItem.AccessibilityObject.Name;
                    }

                    return name;
                }

                set
                {
                    // If anyone tries to set the accessible name, just cache the value in the control's
                    // AccessibleName property. This value will then end up overriding the normal accessible
                    // name logic, until such time as AccessibleName is set back to null.
                    owner.AccessibleName = value;
                }
            }

            public override AccessibleRole Role
            {
                get
                {
                    AccessibleRole role = Owner.AccessibleRole;
                    if (role != AccessibleRole.Default)
                    {
                        return role;
                    }

                    return AccessibleRole.MenuPopup;
                }
            }
        }
    }
}
