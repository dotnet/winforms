// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Xunit;
using static System.Windows.Forms.ToolStripOverflow;
using static Interop;

namespace System.Windows.Forms.Tests
{
    public class ToolStripOverflow_ToolStripOverflowAccessibleObjectTests : IClassFixture<ThreadExceptionFixture>
    {
        [WinFormsFact]
        public void ToolStripOverflowAccessibleObject_ctor_default()
        {
            using ToolStripButton toolStripItem = new ToolStripButton();
            using ToolStripOverflow toolStripOverflow = new ToolStripOverflow(toolStripItem);
            ToolStripOverflowAccessibleObject accessibleObject = new ToolStripOverflowAccessibleObject(toolStripOverflow);

            Assert.Equal(toolStripOverflow, accessibleObject.Owner);
            Assert.False(toolStripOverflow.IsHandleCreated);
        }

        [WinFormsFact]
        public void ToolStripOverflowAccessibleObject_ControlType_IsToolBar_IfAccessibleRoleIsDefault()
        {
            using ToolStripButton toolStripItem = new ToolStripButton();
            using ToolStripOverflow toolStripOverflow = new ToolStripOverflow(toolStripItem);
            // AccessibleRole is not set = Default

            AccessibleObject accessibleObject = toolStripOverflow.AccessibilityObject;
            object actual = accessibleObject.GetPropertyValue(UiaCore.UIA.ControlTypePropertyId);

            Assert.Equal(UiaCore.UIA.ToolBarControlTypeId, actual);
            Assert.False(toolStripOverflow.IsHandleCreated);
        }

        [WinFormsFact]
        public void ToolStripOverflowAccessibleObject_Role_IsToolBar_ByDefault()
        {
            using ToolStripButton toolStripItem = new ToolStripButton();
            using ToolStripOverflow toolStripOverflow = new ToolStripOverflow(toolStripItem);
            // AccessibleRole is not set = Default

            object actual = toolStripOverflow.AccessibilityObject.Role;

            Assert.Equal(AccessibleRole.ToolBar, actual);
            Assert.False(toolStripOverflow.IsHandleCreated);
        }
    }
}
