// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Xunit;

namespace System.Windows.Forms.Tests.AccessibleObjects
{
    public class LabelAccessibleObjectTests
    {
        [WinFormsFact]
        public void LabelAccessibleObject_GetPropertyValue_Name_ReturnsExpected()
        {
            using var label = new Label()
            {
                Name = "Name1",
                AccessibleName = "Test Name"
            };

            AccessibleObject labelAccessibleObject = label.AccessibilityObject;
            var accessibleName = labelAccessibleObject.GetPropertyValue(NativeMethods.UIA_NamePropertyId);

            Assert.Equal("Test Name", accessibleName);
        }

        [WinFormsFact]
        public void LabelAccessibleObject_IsPatternSupported_LegacyIAccessible_ReturnsTrue()
        {
            using var label = new Label();
            AccessibleObject labelAccessibleObject = label.AccessibilityObject;

            bool supportsLegacyIAccessiblePatternId = labelAccessibleObject.IsPatternSupported(NativeMethods.UIA_LegacyIAccessiblePatternId);

            Assert.True(supportsLegacyIAccessiblePatternId);
        }

        [WinFormsFact]
        public void LabelAccessibleObject_LegacyIAccessible_Role_ReturnsExpected()
        {
            using var label = new Label()
            {
                AccessibleRole = AccessibleRole.Link
            };

            AccessibleObject labelAccessibleObject = label.AccessibilityObject;
            var accessibleObjectRole = labelAccessibleObject.Role;

            Assert.Equal(AccessibleRole.Link, accessibleObjectRole);
        }

        [WinFormsFact]
        public void LabelAccessibleObject_LegacyIAccessible_Description_ReturnsExpected()
        {
            using var label = new Label()
            {
                AccessibleDescription = "Test Description"
            };

            AccessibleObject labelAccessibleObject = label.AccessibilityObject;
            var accessibleObjectDescription = labelAccessibleObject.Description;

            Assert.Equal("Test Description", accessibleObjectDescription);
        }
    }
}
