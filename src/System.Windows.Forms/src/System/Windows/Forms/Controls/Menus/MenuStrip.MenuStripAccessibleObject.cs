// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Windows.Win32.System.Variant;
using Windows.Win32.UI.Accessibility;

namespace System.Windows.Forms;

public partial class MenuStrip
{
    internal class MenuStripAccessibleObject : ToolStripAccessibleObject
    {
        public MenuStripAccessibleObject(MenuStrip owner)
            : base(owner)
        {
        }

        public override AccessibleRole Role => this.GetOwnerAccessibleRole(AccessibleRole.MenuBar);

        internal override VARIANT GetPropertyValue(UIA_PROPERTY_ID propertyID) =>
            propertyID switch
            {
                UIA_PROPERTY_ID.UIA_IsControlElementPropertyId => VARIANT.True,
                UIA_PROPERTY_ID.UIA_IsContentElementPropertyId => VARIANT.False,
                _ => base.GetPropertyValue(propertyID)
            };
    }
}
