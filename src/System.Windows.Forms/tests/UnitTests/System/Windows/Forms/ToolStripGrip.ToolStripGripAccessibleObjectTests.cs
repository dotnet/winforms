// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Xunit;
using static System.Windows.Forms.ToolStripGrip;
using static Interop;

namespace System.Windows.Forms
{
    public class ToolStripGrip_ToolStripGripAccessibleObjectTests : IClassFixture<ThreadExceptionFixture>
    {
        [WinFormsFact]
        public void ToolStripGripAccessibleObject_Ctor_Default()
        {
            using ToolStripGrip toolStripGrip = new ToolStripGrip();
            ToolStripGripAccessibleObject accessibleObject = new ToolStripGripAccessibleObject(toolStripGrip);

            Assert.Equal(toolStripGrip, accessibleObject.Owner);
        }

        [WinFormsFact]
        public void ToolStripGripAccessibleObject_ControlType_IsThumb_IfAccessibleRoleIsDefault()
        {
            using ToolStripGrip toolStripGrip = new ToolStripGrip();
            // AccessibleRole is not set = Default

            object actual = toolStripGrip.AccessibilityObject.GetPropertyValue(UiaCore.UIA.ControlTypePropertyId);

            Assert.Equal(UiaCore.UIA.ThumbControlTypeId, actual);
        }

        [WinFormsFact]
        public void ToolStripGripAccessibleObject_Role_IsGrip_ByDefault()
        {
            using ToolStripGrip toolStripGrip = new ToolStripGrip();
            // AccessibleRole is not set = Default

            AccessibleRole actual = toolStripGrip.AccessibilityObject.Role;

            Assert.Equal(AccessibleRole.Grip, actual);
        }

        public static IEnumerable<object[]> ToolStripGripAccessibleObject_GetPropertyValue_ControlType_IsExpected_ForCustomRole_TestData()
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
        [MemberData(nameof(ToolStripGripAccessibleObject_GetPropertyValue_ControlType_IsExpected_ForCustomRole_TestData))]
        public void ToolStripGripAccessibleObject_GetPropertyValue_ControlType_IsExpected_ForCustomRole(AccessibleRole role)
        {
            using ToolStripGrip toolStripGrip = new ToolStripGrip();
            toolStripGrip.AccessibleRole = role;

            object actual = toolStripGrip.AccessibilityObject.GetPropertyValue(UiaCore.UIA.ControlTypePropertyId);
            UiaCore.UIA expected = AccessibleRoleControlTypeMap.GetControlType(role);

            Assert.Equal(expected, actual);
        }
    }
}
