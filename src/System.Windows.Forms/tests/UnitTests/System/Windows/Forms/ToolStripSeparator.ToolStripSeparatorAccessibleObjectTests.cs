// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Xunit;
using static System.Windows.Forms.ToolStripSeparator;
using static Interop;

namespace System.Windows.Forms.Tests
{
    public class ToolStripSeparator_ToolStripSeparatorAccessibleObjectTests : IClassFixture<ThreadExceptionFixture>
    {
        [WinFormsFact]
        public void ToolStripSeparatorAccessibleObect_Ctor_Default()
        {
            using ToolStripSeparator toolStripSeparator = new ToolStripSeparator();
            ToolStripSeparatorAccessibleObject accessibleObject = new ToolStripSeparatorAccessibleObject(toolStripSeparator);

            Assert.Equal(toolStripSeparator, accessibleObject.Owner);
        }

        [WinFormsFact]
        public void ToolStripSeparatorAccessibleObect_ControlType_IsSeparator_IfAccessibleRoleIsDefault()
        {
            using ToolStripSeparator toolStripSeparator = new ToolStripSeparator();
            // AccessibleRole is not set = Default

            object actual = toolStripSeparator.AccessibilityObject.GetPropertyValue(UiaCore.UIA.ControlTypePropertyId);

            Assert.Equal(UiaCore.UIA.SeparatorControlTypeId, actual);
        }

        [WinFormsFact]
        public void ToolStripSeparatorAccessibleObect_Role_IsSeparator_ByDefault()
        {
            using ToolStripSeparator toolStripSeparator = new ToolStripSeparator();
            // AccessibleRole is not set = Default

            AccessibleRole actual = toolStripSeparator.AccessibilityObject.Role;

            Assert.Equal(AccessibleRole.Separator, actual);
        }
    }
}
