// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Xunit;

namespace System.Windows.Forms.Tests.AccessibleObjects
{
    public class StatusStripAccessibleObjectTests
    {
        [WinFormsFact]
        public void StatusStripAccessibleObject_GetPropertyValue_Name_ReturnsExpected()
        {
            using var statusStrip = new StatusStrip()
            {
                Name = "Name1",
                AccessibleName = "Test Name"
            };

            AccessibleObject statusStripAccessibleObject = statusStrip.AccessibilityObject;
            var accessibleName = statusStripAccessibleObject.GetPropertyValue(NativeMethods.UIA_NamePropertyId);

            Assert.Equal("Test Name", accessibleName);
        }

        [WinFormsFact]
        public void StatusStripAccessibleObject_IsPatternSupported_LegacyIAccessible_ReturnsTrue()
        {
            using var statusStrip = new StatusStrip();
            AccessibleObject statusStripAccessibleObject = statusStrip.AccessibilityObject;

            bool supportsLegacyIAccessiblePatternId = statusStripAccessibleObject.IsPatternSupported(NativeMethods.UIA_LegacyIAccessiblePatternId);

            Assert.True(supportsLegacyIAccessiblePatternId);
        }

        [WinFormsFact]
        public void StatusStripAccessibleObject_LegacyIAccessible_Role_ReturnsExpected()
        {
            using var statusStrip = new StatusStrip()
            {
                AccessibleRole = AccessibleRole.Link
            };

            AccessibleObject statusStripAccessibleObject = statusStrip.AccessibilityObject;
            var accessibleObjectRole = statusStripAccessibleObject.Role;

            Assert.Equal(AccessibleRole.Link, accessibleObjectRole);
        }

        [WinFormsFact]
        public void StatusStripAccessibleObject_LegacyIAccessible_Description_ReturnsExpected()
        {
            using var statusStrip = new StatusStrip()
            {
                AccessibleDescription = "Test Description"
            };

            AccessibleObject statusStripAccessibleObject = statusStrip.AccessibilityObject;
            var accessibleObjectDescription = statusStripAccessibleObject.Description;

            Assert.Equal("Test Description", accessibleObjectDescription);
        }
    }
}
