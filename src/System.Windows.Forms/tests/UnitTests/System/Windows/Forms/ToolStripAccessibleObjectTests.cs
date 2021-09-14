// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Xunit;
using static Interop;

namespace System.Windows.Forms.Tests
{
    public class ToolStripAccessibleObjectTests : IClassFixture<ThreadExceptionFixture>
    {
        [WinFormsFact]
        public void ToolStripAccessibleObject_Ctor_Default()
        {
            using ToolStrip toolStrip = new ToolStrip();

            var accessibleObject = new ToolStrip.ToolStripAccessibleObject(toolStrip);
            Assert.NotNull(accessibleObject.Owner);
            Assert.Equal(AccessibleRole.ToolBar, accessibleObject.Role);
        }

        [WinFormsFact]
        public void ToolStripAccessibleObject_FragmentNavigate_FirstChild_ThumbButton()
        {
            using ToolStrip toolStrip = new ToolStrip();
            var accessibleObject = toolStrip.AccessibilityObject;

            UiaCore.IRawElementProviderFragment firstChild = accessibleObject.FragmentNavigate(UiaCore.NavigateDirection.FirstChild);
            Assert.NotNull(firstChild);
            Assert.Equal(UiaCore.UIA.ThumbControlTypeId, firstChild.GetPropertyValue(UiaCore.UIA.ControlTypePropertyId));
        }

        [WinFormsFact]
        public void ToolStripAccessibleObject_GetPropertyValue_Custom_Name_ReturnsExpected()
        {
            using var toolStrip = new ToolStrip()
            {
                Name = "Name1",
                AccessibleName = "Test Name"
            };

            AccessibleObject toolStripAccessibleObject = toolStrip.AccessibilityObject;
            var accessibleName = toolStripAccessibleObject.GetPropertyValue(UiaCore.UIA.NamePropertyId);

            Assert.Equal("Test Name", accessibleName);
        }

        [WinFormsFact]
        public void ToolStripAccessibleObject_IsPatternSupported_LegacyIAccessible_ReturnsTrue()
        {
            using var toolStrip = new ToolStrip();
            AccessibleObject toolStripAccessibleObject = toolStrip.AccessibilityObject;

            bool supportsLegacyIAccessiblePatternId = toolStripAccessibleObject.IsPatternSupported(UiaCore.UIA.LegacyIAccessiblePatternId);

            Assert.True(supportsLegacyIAccessiblePatternId);
        }

        [WinFormsFact]
        public void ToolStripAccessibleObject_LegacyIAccessible_Custom_Role_ReturnsExpected()
        {
            using var toolStrip = new ToolStrip()
            {
                AccessibleRole = AccessibleRole.Link
            };

            AccessibleObject toolStripAccessibleObject = toolStrip.AccessibilityObject;
            var accessibleObjectRole = toolStripAccessibleObject.Role;

            Assert.Equal(AccessibleRole.Link, accessibleObjectRole);
        }

        [WinFormsFact]
        public void ToolStripAccessibleObject_LegacyIAccessible_Custom_Description_ReturnsExpected()
        {
            using var toolStrip = new ToolStrip()
            {
                AccessibleDescription = "Test Description"
            };

            AccessibleObject toolStripAccessibleObject = toolStrip.AccessibilityObject;
            var accessibleObjectDescription = toolStripAccessibleObject.Description;

            Assert.Equal("Test Description", accessibleObjectDescription);
        }

        [WinFormsFact]
        public void ToolStripAccessibleObject_ControlType_IsToolBar_IfAccessibleRoleIsDefault()
        {
            using ToolStrip toolStrip = new ToolStrip();
            // AccessibleRole is not set = Default

            object actual = toolStrip.AccessibilityObject.GetPropertyValue(UiaCore.UIA.ControlTypePropertyId);

            Assert.Equal(UiaCore.UIA.ToolBarControlTypeId, actual);
            Assert.False(toolStrip.IsHandleCreated);
        }

        public static IEnumerable<object[]> ToolStripAccessibleObject_GetPropertyValue_ControlType_IsExpected_ForCustomRole_TestData()
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
        [MemberData(nameof(ToolStripAccessibleObject_GetPropertyValue_ControlType_IsExpected_ForCustomRole_TestData))]
        public void ToolStripAccessibleObject_GetPropertyValue_ControlType_IsExpected_ForCustomRole(AccessibleRole role)
        {
            using ToolStrip toolStrip = new ToolStrip();
            toolStrip.AccessibleRole = role;

            object actual = toolStrip.AccessibilityObject.GetPropertyValue(UiaCore.UIA.ControlTypePropertyId);
            UiaCore.UIA expected = AccessibleRoleControlTypeMap.GetControlType(role);

            Assert.Equal(expected, actual);
            Assert.False(toolStrip.IsHandleCreated);
        }
    }
}
