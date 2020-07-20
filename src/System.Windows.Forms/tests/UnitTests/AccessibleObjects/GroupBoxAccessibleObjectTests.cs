// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Xunit;

namespace System.Windows.Forms.Tests.AccessibleObjects
{
    public class GroupBoxAccessibleObjectTests
    {
        [WinFormsFact]
        public void GroupBoxAccessibleObject_GetPropertyValue_Custom_Name_ReturnsExpected()
        {
            using var groupBox = new GroupBox()
            {
                Name = "Name1",
                AccessibleName = "Test Name"
            };

            AccessibleObject groupBoxAccessibleObject = groupBox.AccessibilityObject;
            var accessibleName = groupBoxAccessibleObject.GetPropertyValue(NativeMethods.UIA_NamePropertyId);

            Assert.Equal("Test Name", accessibleName);
        }

        [WinFormsFact]
        public void GroupBoxAccessibleObject_IsPatternSupported_LegacyIAccessible_ReturnsTrue()
        {
            using var groupBox = new GroupBox();
            AccessibleObject groupBoxAccessibleObject = groupBox.AccessibilityObject;

            bool supportsLegacyIAccessiblePatternId = groupBoxAccessibleObject.IsPatternSupported(NativeMethods.UIA_LegacyIAccessiblePatternId);

            Assert.True(supportsLegacyIAccessiblePatternId);
        }

        [WinFormsFact]
        public void GroupBoxAccessibleObject_LegacyIAccessible_Custom_Role_ReturnsExpected()
        {
            using var groupBox = new GroupBox()
            {
                AccessibleRole = AccessibleRole.Link
            };

            AccessibleObject groupBoxAccessibleObject = groupBox.AccessibilityObject;
            var accessibleObjectRole = groupBoxAccessibleObject.Role;

            Assert.Equal(AccessibleRole.Link, accessibleObjectRole);
        }

        [WinFormsFact]
        public void GroupBoxAccessibleObject_LegacyIAccessible_Custom_Description_ReturnsExpected()
        {
            using var groupBox = new GroupBox()
            {
                AccessibleDescription = "Test Description"
            };

            AccessibleObject groupBoxAccessibleObject = groupBox.AccessibilityObject;
            var accessibleObjectDescription = groupBoxAccessibleObject.Description;

            Assert.Equal("Test Description", accessibleObjectDescription);
        }
    }
}
