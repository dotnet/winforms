// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Xunit;
using static Interop.UiaCore;

namespace System.Windows.Forms.Tests.AccessibleObjects
{
    public class ToolStripTextBox_ToolStripTextBoxAccessibleObjectTests
    {
        [WinFormsFact]
        public void ToolStripTextBoxAccessibleObject_GetPropertyValue_Custom_Name_ReturnsExpected()
        {
            using var toolStripTextBox = new ToolStripTextBox()
            {
                Name = "Name1",
                AccessibleName = "Test Name"
            };

            AccessibleObject toolStripTextBoxAccessibleObject = toolStripTextBox.AccessibilityObject;
            var accessibleName = toolStripTextBoxAccessibleObject.GetPropertyValue(UIA.NamePropertyId);

            Assert.Equal("Test Name", accessibleName);
        }

        [WinFormsFact]
        public void ToolStripTextBoxAccessibleObject_IsPatternSupported_LegacyIAccessible_ReturnsTrue()
        {
            using var toolStripTextBox = new ToolStripTextBox();
            AccessibleObject toolStripTextBoxAccessibleObject = toolStripTextBox.AccessibilityObject;

            bool supportsLegacyIAccessiblePatternId = toolStripTextBoxAccessibleObject.IsPatternSupported(UIA.LegacyIAccessiblePatternId);

            Assert.True(supportsLegacyIAccessiblePatternId);
        }

        [WinFormsFact]
        public void ToolStripTextBoxAccessibleObject_LegacyIAccessible_Custom_Role_ReturnsExpected()
        {
            using var toolStripTextBox = new ToolStripTextBox()
            {
                AccessibleRole = AccessibleRole.Link
            };

            AccessibleObject toolStripTextBoxAccessibleObject = toolStripTextBox.AccessibilityObject;
            var accessibleObjectRole = toolStripTextBoxAccessibleObject.Role;

            Assert.Equal(AccessibleRole.Link, accessibleObjectRole);
        }

        [WinFormsFact]
        public void ToolStripTextBoxAccessibleObject_LegacyIAccessible_Custom_Description_ReturnsExpected()
        {
            using var toolStripTextBox = new ToolStripTextBox()
            {
                AccessibleDescription = "Test Description"
            };

            AccessibleObject toolStripTextBoxAccessibleObject = toolStripTextBox.AccessibilityObject;
            var accessibleObjectDescription = toolStripTextBoxAccessibleObject.Description;

            Assert.Equal("Test Description", accessibleObjectDescription);
        }
    }
}
