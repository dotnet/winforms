// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Xunit;
using static Interop;

namespace System.Windows.Forms.Tests
{
    public class GroupBoxAccessibleObjectTests
    {
        [WinFormsFact]
        public void GroupBoxAccessibleObject_GetPropertyValue_Name_ReturnsExpected()
        {
            string testAccName = "Test group name";
            using var groupBox = new GroupBox();
            AccessibleObject groupBoxAccessibleObject = groupBox.AccessibilityObject;

            Assert.Null(groupBoxAccessibleObject.GetPropertyValue(Interop.UiaCore.UIA.NamePropertyId));
            Assert.Null(groupBoxAccessibleObject.GetPropertyValue(Interop.UiaCore.UIA.LegacyIAccessibleNamePropertyId));

            groupBox.Text = "Some test groupBox text";
            groupBox.Name = "Group1";
            groupBox.AccessibleName = testAccName;

            Assert.Equal(testAccName, groupBoxAccessibleObject.GetPropertyValue(Interop.UiaCore.UIA.NamePropertyId));
            Assert.Equal(testAccName, groupBoxAccessibleObject.GetPropertyValue(Interop.UiaCore.UIA.LegacyIAccessibleNamePropertyId));
            Assert.False(groupBox.IsHandleCreated);
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
        public void GroupBoxAccessibleObject_Role_IsGrouping_ByDefault()
        {
            using GroupBox groupBox = new GroupBox();
            // AccessibleRole is not set = Default

            AccessibleRole actual = groupBox.AccessibilityObject.Role;

            Assert.Equal(AccessibleRole.Grouping, actual);
            Assert.False(groupBox.IsHandleCreated);
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

        [WinFormsFact]
        public void GroupBoxAccessibleObject_ControlType_IsGroup_IfAccessibleRoleIsDefault()
        {
            using GroupBox groupBox = new GroupBox();
            // AccessibleRole is not set = Default
            object actual = groupBox.AccessibilityObject.GetPropertyValue(UiaCore.UIA.ControlTypePropertyId);
            Assert.Equal(UiaCore.UIA.GroupControlTypeId, actual);
            Assert.False(groupBox.IsHandleCreated);
        }

        public static IEnumerable<object[]> GroupBoxAccessibleObject_GetPropertyValue_ControlType_IsExpected_ForCustomRole_TestData()
        {
            Array roles = Enum.GetValues(typeof(AccessibleRole));

            foreach (AccessibleRole role in roles)
            {
                if (role == AccessibleRole.Default)
                {
                    continue; // The test checks custom roles
                }

                yield return new object[] { role };
            }
        }

        [WinFormsTheory]
        [MemberData(nameof(GroupBoxAccessibleObject_GetPropertyValue_ControlType_IsExpected_ForCustomRole_TestData))]
        public void GroupBoxAccessibleObject_GetPropertyValue_ControlType_IsExpected_ForCustomRole(AccessibleRole role)
        {
            using GroupBox groupBox = new GroupBox();
            groupBox.AccessibleRole = role;

            object actual = groupBox.AccessibilityObject.GetPropertyValue(UiaCore.UIA.ControlTypePropertyId);
            UiaCore.UIA expected = AccessibleRoleControlTypeMap.GetControlType(role);

            Assert.Equal(expected, actual);
            Assert.False(groupBox.IsHandleCreated);
        }

        [WinFormsFact]
        public void GroupBoxAccessibleObject_GetPropertyValue_AutomationId_ReturnsExpected()
        {
            using GroupBox ownerControl = new() { Name = "test name" };
            string expected = ownerControl.Name;
            object actual = ownerControl.AccessibilityObject.GetPropertyValue(UiaCore.UIA.AutomationIdPropertyId);

            Assert.Equal(expected, actual);
            Assert.False(ownerControl.IsHandleCreated);
        }
    }
}
