// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Xunit;

namespace System.Windows.Forms.Tests.AccessibleObjects
{
    public class ProgressBarAccessibleObjectTests
    {
        [WinFormsFact]
        public void ProgressBarAccessibleObject_GetPropertyValue_Name_ReturnsExpected()
        {
            using var progressBar = new ProgressBar()
            {
                Name = "Name1",
                AccessibleName = "Test Name"
            };

            AccessibleObject progressBarAccessibleObject = progressBar.AccessibilityObject;
            var accessibleName = progressBarAccessibleObject.GetPropertyValue(NativeMethods.UIA_NamePropertyId);

            Assert.Equal("Test Name", accessibleName);
        }

        [WinFormsFact]
        public void ProgressBarAccessibleObject_IsPatternSupported_LegacyIAccessible_ReturnsTrue()
        {
            using var progressBar = new ProgressBar();
            AccessibleObject progressBarAccessibleObject = progressBar.AccessibilityObject;

            bool supportsLegacyIAccessiblePatternId = progressBarAccessibleObject.IsPatternSupported(NativeMethods.UIA_LegacyIAccessiblePatternId);

            Assert.True(supportsLegacyIAccessiblePatternId);
        }

        [WinFormsFact]
        public void ProgressBarAccessibleObject_LegacyIAccessible_Role_ReturnsExpected()
        {
            using var progressBar = new ProgressBar()
            {
                AccessibleRole = AccessibleRole.Link
            };

            AccessibleObject progressBarAccessibleObject = progressBar.AccessibilityObject;
            var accessibleObjectRole = progressBarAccessibleObject.Role;

            Assert.Equal(AccessibleRole.Link, accessibleObjectRole);
        }

        [WinFormsFact]
        public void ProgressBarAccessibleObject_LegacyIAccessible_Description_ReturnsExpected()
        {
            using var progressBar = new ProgressBar()
            {
                AccessibleDescription = "Test Description"
            };

            AccessibleObject progressBarAccessibleObject = progressBar.AccessibilityObject;
            var accessibleObjectDescription = progressBarAccessibleObject.Description;

            Assert.Equal("Test Description", accessibleObjectDescription);
        }
    }
}
