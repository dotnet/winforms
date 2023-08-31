// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

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

        internal override object? GetPropertyValue(Interop.UiaCore.UIA propertyID) =>
            propertyID switch
            {
                Interop.UiaCore.UIA.IsControlElementPropertyId => true,
                Interop.UiaCore.UIA.IsContentElementPropertyId => false,
                _ => base.GetPropertyValue(propertyID)
            };
    }
}
