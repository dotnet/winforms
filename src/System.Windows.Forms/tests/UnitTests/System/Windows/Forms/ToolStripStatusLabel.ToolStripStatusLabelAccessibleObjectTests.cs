// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Xunit;
using static System.Windows.Forms.ToolStripStatusLabel;
using static Interop;

namespace System.Windows.Forms.Tests
{
    public class ToolStripStatusLabel_ToolStripStatusLabelAccessibleObjectTests : IClassFixture<ThreadExceptionFixture>
    {
        [WinFormsFact]
        public void ToolStripStatusLabelAccessibleObect_Ctor_Default()
        {
            using ToolStripStatusLabel toolStripStatusLabel = new ToolStripStatusLabel();
            ToolStripStatusLabelAccessibleObject accessibleObject = new ToolStripStatusLabelAccessibleObject(toolStripStatusLabel);

            Assert.Equal(toolStripStatusLabel, accessibleObject.Owner);
        }

        [WinFormsFact]
        public void ToolStripStatusLabelAccessibleObect_ControlType_IsText_IfAccessibleRoleIsDefault()
        {
            using ToolStripStatusLabel toolStripStatusLabel = new ToolStripStatusLabel();
            // AccessibleRole is not set = Default

            object actual = toolStripStatusLabel.AccessibilityObject.GetPropertyValue(UiaCore.UIA.ControlTypePropertyId);
            Assert.Equal(UiaCore.UIA.TextControlTypeId, actual);
        }

        [WinFormsTheory]
        [InlineData(true, AccessibleRole.Link)]
        [InlineData(false, AccessibleRole.StaticText)]
        public void ToolStripStatusLabelAccessibleObect_Role_IsExpected_ByDefault(bool isLink, AccessibleRole expectedRole)
        {
            using ToolStripStatusLabel toolStripStatusLabel = new ToolStripStatusLabel();
            toolStripStatusLabel.IsLink = isLink;
            // AccessibleRole is not set = Default

            AccessibleRole actual = toolStripStatusLabel.AccessibilityObject.Role;

            Assert.Equal(expectedRole, actual);
        }
    }
}
