// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Xunit;
using static Interop.UiaCore;

namespace System.Windows.Forms.Tests.AccessibleObjects
{
    public class MenuStrip_MenuStripAccessibleObjectTests
    {
        [WinFormsFact]
        public void MenuStripAccessibleObject_GetPropertyValue_Custom_Name_ReturnsExpected()
        {
            using var menuStrip = new MenuStrip()
            {
                Name = "Name1",
                AccessibleName = "Test Name"
            };

            AccessibleObject menuStripAccessibleObject = menuStrip.AccessibilityObject;
            var accessibleName = menuStripAccessibleObject.GetPropertyValue(UIA.NamePropertyId);

            Assert.Equal("Test Name", accessibleName);
        }

        [WinFormsFact]
        public void MenuStripAccessibleObject_IsPatternSupported_LegacyIAccessible_ReturnsTrue()
        {
            using var menuStrip = new MenuStrip();
            AccessibleObject menuStripAccessibleObject = menuStrip.AccessibilityObject;

            bool supportsLegacyIAccessiblePatternId = menuStripAccessibleObject.IsPatternSupported(UIA.LegacyIAccessiblePatternId);

            Assert.True(supportsLegacyIAccessiblePatternId);
        }

        [WinFormsFact]
        public void MenuStripAccessibleObject_LegacyIAccessible_Custom_Role_ReturnsExpected()
        {
            using var menuStrip = new MenuStrip()
            {
                AccessibleRole = AccessibleRole.Link
            };

            AccessibleObject menuStripAccessibleObject = menuStrip.AccessibilityObject;
            var accessibleObjectRole = menuStripAccessibleObject.Role;

            Assert.Equal(AccessibleRole.Link, accessibleObjectRole);
        }

        [WinFormsFact]
        public void MenuStripAccessibleObject_LegacyIAccessible_Custom_Description_ReturnsExpected()
        {
            using var menuStrip = new MenuStrip()
            {
                AccessibleDescription = "Test Description"
            };

            AccessibleObject menuStripAccessibleObject = menuStrip.AccessibilityObject;
            var accessibleObjectDescription = menuStripAccessibleObject.Description;

            Assert.Equal("Test Description", accessibleObjectDescription);
        }
    }
}
