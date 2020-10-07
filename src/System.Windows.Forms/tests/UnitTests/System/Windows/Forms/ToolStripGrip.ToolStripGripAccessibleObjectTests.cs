// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Xunit;
using static System.Windows.Forms.ToolStripGrip;
using static Interop;

namespace System.Windows.Forms
{
    public class ToolStripGrip_ToolStripGripAccessibleObjectTests : IClassFixture<ThreadExceptionFixture>
    {
        [WinFormsFact]
        public void ToolStripGripAccessibleObject_Ctor_Default()
        {
            using ToolStripGrip toolStripGrip = new ToolStripGrip();
            ToolStripGripAccessibleObject accessibleObject = new ToolStripGripAccessibleObject(toolStripGrip);

            Assert.Equal(toolStripGrip, accessibleObject.Owner);
        }

        [WinFormsFact]
        public void ToolStripGripAccessibleObject_ControlType_IsThumb_IfAccessibleRoleIsDefault()
        {
            using ToolStripGrip toolStripGrip = new ToolStripGrip();
            // AccessibleRole is not set = Default

            object actual = toolStripGrip.AccessibilityObject.GetPropertyValue(UiaCore.UIA.ControlTypePropertyId);

            Assert.Equal(UiaCore.UIA.ThumbControlTypeId, actual);
        }

        [WinFormsFact]
        public void ToolStripGripAccessibleObject_Role_IsGrip_ByDefault()
        {
            using ToolStripGrip toolStripGrip = new ToolStripGrip();
            // AccessibleRole is not set = Default

            AccessibleRole actual = toolStripGrip.AccessibilityObject.Role;

            Assert.Equal(AccessibleRole.Grip, actual);
        }
    }
}
