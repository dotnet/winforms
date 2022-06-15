// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Xunit;
using static Interop;
using static Interop.UiaCore;

namespace System.Windows.Forms.Tests
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

        [WinFormsFact]
        public void MenuStripAccessibleObject_ControlType_IsMenuBar_IfAccessibleRoleIsDefault()
        {
            using MenuStrip menuStrip = new MenuStrip();
            // AccessibleRole is not set = Default

            object actual = menuStrip.AccessibilityObject.GetPropertyValue(UIA.ControlTypePropertyId);

            Assert.Equal(UIA.MenuBarControlTypeId, actual);
            Assert.False(menuStrip.IsHandleCreated);
        }

        [WinFormsFact]
        public void MenuStripAccessibleObject_Role_IsMenuBar_ByDefault()
        {
            using MenuStrip menuStrip = new MenuStrip();
            // AccessibleRole is not set = Default

            AccessibleRole actual = menuStrip.AccessibilityObject.Role;

            Assert.Equal(AccessibleRole.MenuBar, actual);
            Assert.False(menuStrip.IsHandleCreated);
        }

        public static IEnumerable<object[]> MenuStripAccessibleObject_GetPropertyValue_ControlType_IsExpected_ForCustomRole_TestData()
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
        [MemberData(nameof(MenuStripAccessibleObject_GetPropertyValue_ControlType_IsExpected_ForCustomRole_TestData))]
        public void MenuStripAccessibleObject_GetPropertyValue_ControlType_IsExpected_ForCustomRole(AccessibleRole role)
        {
            using MenuStrip menuStrip = new MenuStrip();
            menuStrip.AccessibleRole = role;

            object actual = menuStrip.AccessibilityObject.GetPropertyValue(UiaCore.UIA.ControlTypePropertyId);
            UiaCore.UIA expected = AccessibleRoleControlTypeMap.GetControlType(role);

            Assert.Equal(expected, actual);
            Assert.False(menuStrip.IsHandleCreated);
        }
    }
}
