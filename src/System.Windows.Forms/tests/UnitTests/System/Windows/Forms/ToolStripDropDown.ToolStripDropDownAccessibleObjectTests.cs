// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Xunit;
using static System.Windows.Forms.ToolStripDropDown;
using static Interop;

namespace System.Windows.Forms.Tests
{
    public class ToolStripDropDown_ToolStripDropDownAccessibleObjectTests : IClassFixture<ThreadExceptionFixture>
    {
        [WinFormsFact]
        public void ToolStripDropDownAccessibleObject_ctor_default()
        {
            using ToolStripDropDown toolStripDropDown = new ToolStripDropDown();
            ToolStripDropDownAccessibleObject accessibleObject = new ToolStripDropDownAccessibleObject(toolStripDropDown);

            Assert.Equal(toolStripDropDown, accessibleObject.Owner);
            Assert.False(toolStripDropDown.IsHandleCreated);
        }

        [WinFormsFact]
        public void ToolStripDropDownAccessibleObject_ControlType_IsMenu_IfAccessibleRoleIsDefault()
        {
            using ToolStripDropDown toolStripDropDown = new ToolStripDropDown();
            // AccessibleRole is not set = Default

            AccessibleObject accessibleObject = toolStripDropDown.AccessibilityObject;
            object actual = accessibleObject.GetPropertyValue(UiaCore.UIA.ControlTypePropertyId);

            Assert.Equal(UiaCore.UIA.MenuControlTypeId, actual);
            Assert.False(toolStripDropDown.IsHandleCreated);
        }

        [WinFormsFact]
        public void ToolStripDropDownAccessibleObject_Role_IsMenuPopup_ByDefault()
        {
            using ToolStripDropDown toolStripDropDown = new ToolStripDropDown();
            // AccessibleRole is not set = Default

            AccessibleObject accessibleObject = toolStripDropDown.AccessibilityObject;
            AccessibleRole actual = accessibleObject.Role;

            Assert.Equal(AccessibleRole.MenuPopup, actual);
            Assert.False(toolStripDropDown.IsHandleCreated);
        }

        public static IEnumerable<object[]> ToolStripDropDownAccessibleObject_GetPropertyValue_ControlType_IsExpected_ForCustomRole_TestData()
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
        [MemberData(nameof(ToolStripDropDownAccessibleObject_GetPropertyValue_ControlType_IsExpected_ForCustomRole_TestData))]
        public void ToolStripDropDownAccessibleObject_GetPropertyValue_ControlType_IsExpected_ForCustomRole(AccessibleRole role)
        {
            using ToolStripDropDown toolStripDropDown = new ToolStripDropDown();
            toolStripDropDown.AccessibleRole = role;

            AccessibleObject accessibleObject = toolStripDropDown.AccessibilityObject;
            object actual = accessibleObject.GetPropertyValue(UiaCore.UIA.ControlTypePropertyId);
            UiaCore.UIA expected = AccessibleRoleControlTypeMap.GetControlType(role);

            Assert.Equal(role, accessibleObject.Role);
            Assert.Equal(expected, actual);
            Assert.False(toolStripDropDown.IsHandleCreated);
        }
    }
}
