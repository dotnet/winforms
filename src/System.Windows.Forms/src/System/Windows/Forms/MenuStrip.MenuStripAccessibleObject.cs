// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

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
        }
    }
}
