// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using static Interop;

namespace System.Windows.Forms
{
    public partial class MenuStrip
    {
        internal class MenuStripAccessibleObject : ToolStripAccessibleObject
        {
            public MenuStripAccessibleObject(MenuStrip owner)
                : base(owner)
            {
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

                    return AccessibleRole.MenuBar;
                }
            }

            internal override object? GetPropertyValue(UiaCore.UIA propertyID)
            {
                switch (propertyID)
                {
                    case UiaCore.UIA.ControlTypePropertyId:
                        return UiaCore.UIA.MenuBarControlTypeId;
                    case UiaCore.UIA.NamePropertyId:
                        return Name;
                }

                return base.GetPropertyValue(propertyID);
            }
        }
    }
}
