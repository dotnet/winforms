// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Xunit;
using static System.Windows.Forms.ToolStripDropDownButton;
using static Interop;

namespace System.Windows.Forms.Tests
{
    public class ToolStripDropDownButton_ToolStripDropDownButtonAccessibleObjectTests : IClassFixture<ThreadExceptionFixture>
    {
        [WinFormsFact]
        public void ToolStripDropDownButtonAccessibleObject_Ctor_Default()
        {
            using ToolStripDropDownButton toolStripDropDownButton = new ToolStripDropDownButton();
            ToolStripDropDownButtonAccessibleObject accessibleObject = new ToolStripDropDownButtonAccessibleObject(toolStripDropDownButton);

            Assert.Equal(toolStripDropDownButton, accessibleObject.Owner);
        }

        [WinFormsFact]
        public void ToolStripDropDownButtonAccessibleObject_ControlType_IsButton_IfAccessibleRoleIsDefault()
        {
            using ToolStripDropDownButton toolStripDropDownButton = new ToolStripDropDownButton();
            // AccessibleRole is not set = Default

            object actual = toolStripDropDownButton.AccessibilityObject.GetPropertyValue(UiaCore.UIA.ControlTypePropertyId);

            Assert.Equal(UiaCore.UIA.ButtonControlTypeId, actual);
        }

        [WinFormsFact]
        public void ToolStripDropDownButtonAccessibleObject_Role_IsMenuItem_ByDefault()
        {
            using ToolStripDropDownButton toolStripDropDownButton = new ToolStripDropDownButton();
            // AccessibleRole is not set = Default

            AccessibleRole actual = toolStripDropDownButton.AccessibilityObject.Role;

            Assert.Equal(AccessibleRole.MenuItem, actual);
        }
    }
}
