// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Xunit;
using static Interop;

namespace System.Windows.Forms.Tests
{
    public class LabelAccessibleObjectTests
    {
        [WinFormsFact]
        public void LabelAccessibleObject_GetPropertyValue_Name_ReturnsExpected()
        {
            string testAccName = "Address";
            using var label = new Label();
            label.Text = "Some test label text";
            label.Name = "Label1";
            label.AccessibleName = testAccName;
            AccessibleObject labelAccessibleObject = label.AccessibilityObject;

            Assert.False(label.IsHandleCreated);

            var accessibleName = labelAccessibleObject.GetPropertyValue(Interop.UiaCore.UIA.NamePropertyId);
            Assert.Equal(testAccName, accessibleName);
        }

        [WinFormsFact]
        public void LabelAccessibleObject_IsPatternSupported_LegacyIAccessible_ReturnsTrue()
        {
            using var label = new Label();
            label.Name = "Label1";
            label.Text = "Some test label text";
            var labelAccessibleObject = new Label.LabelAccessibleObject(label);

            Assert.False(label.IsHandleCreated);

            bool supportsLegacyIAccessiblePatternId = labelAccessibleObject.IsPatternSupported(Interop.UiaCore.UIA.LegacyIAccessiblePatternId);
            Assert.True(supportsLegacyIAccessiblePatternId);
        }

        [WinFormsFact]
        public void LabelAccessibleObject_LegacyIAccessible_Role_ReturnsExpected()
        {
            using var label = new Label();
            label.Name = "Label1";
            label.Text = "Some test label text";
            label.AccessibleRole = AccessibleRole.Link;
            var labelAccessibleObject = new Label.LabelAccessibleObject(label);

            Assert.False(label.IsHandleCreated);
            Assert.Equal(AccessibleRole.Link, labelAccessibleObject.Role);
        }

        [WinFormsFact]
        public void LabelAccessibleObject_Role_IsStaticText_ByDefault()
        {
            using Label label = new Label();
            // AccessibleRole is not set = Default

            AccessibleRole actual = label.AccessibilityObject.Role;

            Assert.Equal(AccessibleRole.StaticText, actual);
            Assert.False(label.IsHandleCreated);
        }

        [WinFormsFact]
        public void LabelAccessibleObject_LegacyIAccessible_Description_ReturnsExpected()
        {
            string testAccDescription = "Test description";
            using var label = new Label();
            label.Name = "Label1";
            label.Text = "Some test label text";
            label.AccessibleDescription = testAccDescription;
            var labelAccessibleObject = new Label.LabelAccessibleObject(label);

            Assert.False(label.IsHandleCreated);
            Assert.Equal(testAccDescription, labelAccessibleObject.Description);
        }

        [WinFormsFact]
        public void LabelAccessibleObject_ControlType_IsText_IfAccessibleRoleIsDefault()
        {
            using Label label = new Label();
            // AccessibleRole is not set = Default

            object actual = label.AccessibilityObject.GetPropertyValue(UiaCore.UIA.ControlTypePropertyId);

            Assert.Equal(UiaCore.UIA.TextControlTypeId, actual);
            Assert.False(label.IsHandleCreated);
        }

        public static IEnumerable<object[]> LabelAccessibleObject_GetPropertyValue_ControlType_IsExpected_ForCustomRole_TestData()
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
        [MemberData(nameof(LabelAccessibleObject_GetPropertyValue_ControlType_IsExpected_ForCustomRole_TestData))]
        public void LabelAccessibleObject_GetPropertyValue_ControlType_IsExpected_ForCustomRole(AccessibleRole role)
        {
            using Label label = new Label();
            label.AccessibleRole = role;

            object actual = label.AccessibilityObject.GetPropertyValue(UiaCore.UIA.ControlTypePropertyId);
            UiaCore.UIA expected = AccessibleRoleControlTypeMap.GetControlType(role);

            Assert.Equal(expected, actual);
            Assert.False(label.IsHandleCreated);
        }
    }
}
