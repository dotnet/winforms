// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Xunit;
using static Interop;
using static Interop.UiaCore;

namespace System.Windows.Forms.Tests
{
    public class StatusStrip_StatusStripAccessibleObjectTests
    {
        [WinFormsFact]
        public void StatusStripAccessibleObject_GetPropertyValue_Custom_Name_ReturnsExpected()
        {
            using var statusStrip = new StatusStrip()
            {
                Name = "Name1",
                AccessibleName = "Test Name"
            };

            AccessibleObject statusStripAccessibleObject = statusStrip.AccessibilityObject;
            var accessibleName = statusStripAccessibleObject.GetPropertyValue(UIA.NamePropertyId);

            Assert.Equal("Test Name", accessibleName);
        }

        [WinFormsFact]
        public void StatusStripAccessibleObject_IsPatternSupported_LegacyIAccessible_ReturnsTrue()
        {
            using var statusStrip = new StatusStrip();
            AccessibleObject statusStripAccessibleObject = statusStrip.AccessibilityObject;

            bool supportsLegacyIAccessiblePatternId = statusStripAccessibleObject.IsPatternSupported(UIA.LegacyIAccessiblePatternId);

            Assert.True(supportsLegacyIAccessiblePatternId);
        }

        [WinFormsFact]
        public void StatusStripAccessibleObject_LegacyIAccessible_Custom_Role_ReturnsExpected()
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
        public void StatusStripAccessibleObject_LegacyIAccessible_Custom_Description_ReturnsExpected()
        {
            using var statusStrip = new StatusStrip()
            {
                AccessibleDescription = "Test Description"
            };

            AccessibleObject statusStripAccessibleObject = statusStrip.AccessibilityObject;
            var accessibleObjectDescription = statusStripAccessibleObject.Description;

            Assert.Equal("Test Description", accessibleObjectDescription);
        }

        [WinFormsFact]
        public void StatusStripAccessibleObject_ControlType_IsStatusBar_IfAccessibleRoleIsDefault()
        {
            using StatusStrip statusStrip = new StatusStrip();
            // AccessibleRole is not set = Default

            object actual = statusStrip.AccessibilityObject.GetPropertyValue(UIA.ControlTypePropertyId);

            Assert.Equal(UIA.StatusBarControlTypeId, actual);
            Assert.False(statusStrip.IsHandleCreated);
        }

        [WinFormsFact]
        public void StatusStripAccessibleObject_Role_IsStatusBar_ByDefault()
        {
            using StatusStrip statusStrip = new StatusStrip();
            // AccessibleRole is not set = Default

            AccessibleRole actual = statusStrip.AccessibilityObject.Role;

            Assert.Equal(AccessibleRole.StatusBar, actual);
            Assert.False(statusStrip.IsHandleCreated);
        }

        public static IEnumerable<object[]> StatusStripAccessibleObject_GetPropertyValue_ControlType_IsExpected_ForCustomRole_TestData()
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
        [MemberData(nameof(StatusStripAccessibleObject_GetPropertyValue_ControlType_IsExpected_ForCustomRole_TestData))]
        public void StatusStripAccessibleObject_GetPropertyValue_ControlType_IsExpected_ForCustomRole(AccessibleRole role)
        {
            using StatusStrip statusStrip = new StatusStrip();
            statusStrip.AccessibleRole = role;

            object actual = statusStrip.AccessibilityObject.GetPropertyValue(UiaCore.UIA.ControlTypePropertyId);
            UiaCore.UIA expected = AccessibleRoleControlTypeMap.GetControlType(role);

            Assert.Equal(expected, actual);
            Assert.False(statusStrip.IsHandleCreated);
        }
    }
}
