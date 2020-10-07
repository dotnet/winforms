// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Xunit;
using static System.Windows.Forms.ToolStripLabel;
using static Interop;

namespace System.Windows.Forms.Tests
{
    public class ToolStripLabel_ToolStripLabelAccessibleObectTests : IClassFixture<ThreadExceptionFixture>
    {
        [WinFormsFact]
        public void ToolStripLabelAccessibleObect_Ctor_Default()
        {
            using ToolStripLabel toolStripLabel = new ToolStripLabel();
            ToolStripLabelAccessibleObject accessibleObject = new ToolStripLabelAccessibleObject(toolStripLabel);

            Assert.Equal(toolStripLabel, accessibleObject.Owner);
        }

        [WinFormsFact]
        public void ToolStripLabelAccessibleObect_ControlType_IsText_IfAccessibleRoleIsDefault()
        {
            using ToolStripLabel toolStripLabel = new ToolStripLabel();
            // AccessibleRole is not set = Default

            object actual = toolStripLabel.AccessibilityObject.GetPropertyValue(UiaCore.UIA.ControlTypePropertyId);
            Assert.Equal(UiaCore.UIA.TextControlTypeId, actual);
        }

        [WinFormsTheory]
        [InlineData(true, AccessibleRole.Link)]
        [InlineData(false, AccessibleRole.StaticText)]
        public void ToolStripLabelAccessibleObect_Role_IsExpected_ByDefault(bool isLink, AccessibleRole expectedRole)
        {
            using ToolStripLabel toolStripLabel = new ToolStripLabel();
            toolStripLabel.IsLink = isLink;
            // AccessibleRole is not set = Default

            AccessibleRole actual = toolStripLabel.AccessibilityObject.Role;

            Assert.Equal(expectedRole, actual);
        }
    }
}
