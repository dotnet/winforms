// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using Xunit;
using static System.Windows.Forms.ToolStripProgressBar;
using static Interop;

namespace System.Windows.Forms.Tests
{
    public class ToolStripProgressBar_ToolStripProgressBarControl_ToolStripProgressBarControlAccessibleObjectTests : IClassFixture<ThreadExceptionFixture>
    {
        [WinFormsFact]
        public void ToolStripProgressBarControlAccessibleObject_ctor_default()
        {
            using ToolStripProgressBarControl toolStripProgressBarControl = new ToolStripProgressBarControl();
            ToolStripProgressBarControlAccessibleObject accessibleObject = new ToolStripProgressBarControlAccessibleObject(toolStripProgressBarControl);

            Assert.Equal(toolStripProgressBarControl, accessibleObject.Owner);
            Assert.False(toolStripProgressBarControl.IsHandleCreated);
        }

        [WinFormsFact]
        public void ToolStripProgressBarControlAccessibleObject_ControlType_IsProgressBar_IfAccessibleRoleIsDefault()
        {
            using ToolStripProgressBarControl toolStripProgressBarControl = new ToolStripProgressBarControl();
            // AccessibleRole is not set = Default

            object actual = toolStripProgressBarControl.AccessibilityObject.GetPropertyValue(UiaCore.UIA.ControlTypePropertyId);

            Assert.Equal(UiaCore.UIA.ProgressBarControlTypeId, actual);
            Assert.False(toolStripProgressBarControl.IsHandleCreated);
        }

        [WinFormsTheory]
        [InlineData(true, AccessibleRole.ProgressBar)]
        [InlineData(false, AccessibleRole.None)]
        public void ToolStripProgressBarControlAccessibleObject_Role_IsExpected_ByDefault(bool createControl, AccessibleRole expectedRole)
        {
            using ToolStripProgressBarControl toolStripProgressBarControl = new ToolStripProgressBarControl();
            // AccessibleRole is not set = Default

            if (createControl)
            {
                toolStripProgressBarControl.CreateControl();
            }

            object actual = toolStripProgressBarControl.AccessibilityObject.Role;

            Assert.Equal(expectedRole, actual);
            Assert.Equal(createControl, toolStripProgressBarControl.IsHandleCreated);
        }
    }
}
