// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Windows.Win32.System.Variant;
using Windows.Win32.UI.Accessibility;

namespace System.Windows.Forms;

public partial class PictureBox
{
    internal class PictureBoxAccessibleObject : ControlAccessibleObject
    {
        public PictureBoxAccessibleObject(PictureBox owner) : base(owner)
        {
        }

        internal override VARIANT GetPropertyValue(UIA_PROPERTY_ID propertyID)
            => propertyID switch
            {
                UIA_PROPERTY_ID.UIA_ControlTypePropertyId when this.GetOwnerAccessibleRole() == AccessibleRole.Default
                    // If we don't set a default role for the accessible object
                    // it will be retrieved from Windows.
                    // And we don't have a 100% guarantee it will be correct, hence set it ourselves.
                    => (VARIANT)(int)UIA_CONTROLTYPE_ID.UIA_PaneControlTypeId,
                UIA_PROPERTY_ID.UIA_IsKeyboardFocusablePropertyId => VARIANT.True,
                _ => base.GetPropertyValue(propertyID)
            };
    }
}
