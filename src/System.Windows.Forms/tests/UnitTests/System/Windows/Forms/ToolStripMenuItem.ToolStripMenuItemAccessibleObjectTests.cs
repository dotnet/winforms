// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Xunit;
using static System.Windows.Forms.ToolStripMenuItem;
using static Interop;

namespace System.Windows.Forms.Tests
{
    public class ToolStripMenuItem_ToolStripMenuItemAccessibleObjectTests : IClassFixture<ThreadExceptionFixture>
    {
        [WinFormsFact]
        public void ToolStripMenuItemAccessibleObject_Ctor_Default()
        {
            using ToolStripMenuItem toolStripMenuItem = new ToolStripMenuItem();
            ToolStripMenuItemAccessibleObject accessibleObject = new ToolStripMenuItemAccessibleObject(toolStripMenuItem);

            Assert.Equal(toolStripMenuItem, accessibleObject.Owner);
        }

        [WinFormsFact]
        public void ToolStripMenuItemAccessibleObject_ControlType_IsMenuItem_IfAccessibleRoleIsDefault()
        {
            using ToolStripMenuItem toolStripMenuItem = new ToolStripMenuItem();
            // AccessibleRole is not set = Default

            object actual = toolStripMenuItem.AccessibilityObject.GetPropertyValue(UiaCore.UIA.ControlTypePropertyId);

            Assert.Equal(UiaCore.UIA.MenuItemControlTypeId, actual);
        }

        [WinFormsFact]
        public void ToolStripMenuItemAccessibleObject_Role_IsMenuItem_ByDefault()
        {
            using ToolStripMenuItem toolStripMenuItem = new ToolStripMenuItem();
            // AccessibleRole is not set = Default

            AccessibleRole actual = toolStripMenuItem.AccessibilityObject.Role;

            Assert.Equal(AccessibleRole.MenuItem, actual);
        }
    }
}
