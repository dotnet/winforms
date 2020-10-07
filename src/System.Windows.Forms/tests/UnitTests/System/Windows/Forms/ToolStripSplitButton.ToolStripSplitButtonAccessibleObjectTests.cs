// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Xunit;
using static System.Windows.Forms.ToolStripSplitButton;
using static Interop;

namespace System.Windows.Forms.Tests
{
    public class ToolStripSplitButton_ToolStripSplitButtonAccessibleObjectTests : IClassFixture<ThreadExceptionFixture>
    {
        [WinFormsFact]
        public void ToolStripSplitButtonAccessibleObect_Ctor_Default()
        {
            using ToolStripSplitButton toolStripSplitButton = new ToolStripSplitButton();
            ToolStripSplitButtonAccessibleObject accessibleObject = new ToolStripSplitButtonAccessibleObject(toolStripSplitButton);

            Assert.Equal(toolStripSplitButton, accessibleObject.Owner);
        }

        [WinFormsFact]
        public void ToolStripSplitButtonAccessibleObect_ControlType_IsButton_IfAccessibleRoleIsDefault()
        {
            using ToolStripSplitButton toolStripSplitButton = new ToolStripSplitButton();
            // AccessibleRole is not set = Default

            object actual = toolStripSplitButton.AccessibilityObject.GetPropertyValue(UiaCore.UIA.ControlTypePropertyId);

            Assert.Equal(UiaCore.UIA.ButtonControlTypeId, actual);
        }

        [WinFormsFact]
        public void ToolStripSplitButtonAccessibleObect_Role_IsMenuItem_ByDefault()
        {
            using ToolStripSplitButton toolStripSplitButton = new ToolStripSplitButton();
            // AccessibleRole is not set = Default

            AccessibleRole actual = toolStripSplitButton.AccessibilityObject.Role;

            Assert.Equal(AccessibleRole.MenuItem, actual);
        }
    }
}
