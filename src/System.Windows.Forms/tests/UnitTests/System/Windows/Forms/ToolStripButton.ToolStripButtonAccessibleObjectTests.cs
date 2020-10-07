// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Xunit;
using static System.Windows.Forms.ToolStripButton;
using static Interop;

namespace System.Windows.Forms.Tests
{
    public class ToolStripButton_ToolStripButtonAccessibleObjectTests : IClassFixture<ThreadExceptionFixture>
    {
        [WinFormsFact]
        public void ToolStripButtonAccessibleObject_Ctor_Default()
        {
            using ToolStripButton toolStripButton = new ToolStripButton();
            ToolStripButtonAccessibleObject accessibleObject = new ToolStripButtonAccessibleObject(toolStripButton);

            Assert.Equal(toolStripButton, accessibleObject.Owner);
        }

        [WinFormsFact]
        public void ToolStripButtonAccessibleObject_ControlType_IsButton_IfAccessibleRoleIsDefault()
        {
            using ToolStripButton toolStripButton = new ToolStripButton();
            // AccessibleRole is not set = Default

            object actual = toolStripButton.AccessibilityObject.GetPropertyValue(UiaCore.UIA.ControlTypePropertyId);

            Assert.Equal(UiaCore.UIA.ButtonControlTypeId, actual);
        }

        [WinFormsFact]
        public void ToolStripButtonAccessibleObject_Role_IsPushButton_ByDefault()
        {
            using ToolStripButton toolStripButton = new ToolStripButton();
            // AccessibleRole is not set = Default

            AccessibleRole actual = toolStripButton.AccessibilityObject.Role;

            Assert.Equal(AccessibleRole.PushButton, actual);
        }
    }
}
