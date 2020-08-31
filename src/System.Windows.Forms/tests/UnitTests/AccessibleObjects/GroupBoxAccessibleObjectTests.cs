// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Xunit;

namespace System.Windows.Forms.Tests.AccessibleObjects
{
    public class GroupBoxAccessibleObjectTests
    {
        [WinFormsFact]
        public void GroupBoxAccessibleObject_GetPropertyValue_Name_ReturnsExpected()
        {
            string testAccName = "Test group name";
            using var groupBox = new GroupBox();
            groupBox.Text = "Some test groupBox text";
            groupBox.Name = "Group1";
            groupBox.AccessibleName = testAccName;
            AccessibleObject groupBoxAccessibleObject = groupBox.AccessibilityObject;

            Assert.False(groupBox.IsHandleCreated);

            var accessibleName = groupBoxAccessibleObject.GetPropertyValue(Interop.UiaCore.UIA.NamePropertyId);
            Assert.Equal(testAccName, accessibleName);
        }

        [WinFormsFact]
        public void GroupBoxAccessibleObject_IsPatternSupported_LegacyIAccessible_ReturnsTrue()
        {
            using var groupBox = new GroupBox();
            groupBox.Name = "Group1";
            groupBox.Text = "Some test groupBox text";
            var groupBoxAccessibleObject = new GroupBox.GroupBoxAccessibleObject(groupBox);

            Assert.False(groupBox.IsHandleCreated);

            bool supportsLegacyIAccessiblePatternId = groupBoxAccessibleObject.IsPatternSupported(Interop.UiaCore.UIA.LegacyIAccessiblePatternId);
            Assert.True(supportsLegacyIAccessiblePatternId);
        }

        [WinFormsFact]
        public void GroupBoxAccessibleObject_LegacyIAccessible_Role_ReturnsExpected()
        {
            using var groupBox = new GroupBox();
            groupBox.Name = "Group1";
            groupBox.Text = "Some test groupBox text";
            groupBox.AccessibleRole = AccessibleRole.Link;
            var groupBoxAccessibleObject = new GroupBox.GroupBoxAccessibleObject(groupBox);

            Assert.False(groupBox.IsHandleCreated);
            Assert.Equal(AccessibleRole.Link, groupBoxAccessibleObject.Role);
        }

        [WinFormsFact]
        public void GroupBoxAccessibleObject_LegacyIAccessible_Description_ReturnsExpected()
        {
            string testAccDescription = "Test description";
            using var groupBox = new GroupBox();
            groupBox.Name = "Group1";
            groupBox.Text = "Some test groupBox text";
            groupBox.AccessibleDescription = testAccDescription;
            var groupBoxAccessibleObject = new GroupBox.GroupBoxAccessibleObject(groupBox);

            Assert.False(groupBox.IsHandleCreated);
            Assert.Equal(testAccDescription, groupBoxAccessibleObject.Description);
        }
    }
}
