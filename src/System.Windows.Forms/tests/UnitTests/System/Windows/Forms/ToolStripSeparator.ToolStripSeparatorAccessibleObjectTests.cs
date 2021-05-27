// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using Xunit;
using static System.Windows.Forms.ToolStripSeparator;
using static Interop;

namespace System.Windows.Forms.Tests
{
    public class ToolStripSeparator_ToolStripSeparatorAccessibleObjectTests : IClassFixture<ThreadExceptionFixture>
    {
        [WinFormsFact]
        public void ToolStripSeparatorAccessibleObject_Ctor_Default()
        {
            using ToolStripSeparator toolStripSeparator = new ToolStripSeparator();
            ToolStripSeparatorAccessibleObject accessibleObject = new ToolStripSeparatorAccessibleObject(toolStripSeparator);

            Assert.Equal(toolStripSeparator, accessibleObject.Owner);
        }

        [WinFormsFact]
        public void ToolStripSeparatorAccessibleObject_ControlType_IsSeparator_IfAccessibleRoleIsDefault()
        {
            using ToolStripSeparator toolStripSeparator = new ToolStripSeparator();
            // AccessibleRole is not set = Default

            object actual = toolStripSeparator.AccessibilityObject.GetPropertyValue(UiaCore.UIA.ControlTypePropertyId);

            Assert.Equal(UiaCore.UIA.SeparatorControlTypeId, actual);
        }

        [WinFormsFact]
        public void ToolStripSeparatorAccessibleObject_Role_IsSeparator_ByDefault()
        {
            using ToolStripSeparator toolStripSeparator = new ToolStripSeparator();
            // AccessibleRole is not set = Default

            AccessibleRole actual = toolStripSeparator.AccessibilityObject.Role;

            Assert.Equal(AccessibleRole.Separator, actual);
        }

        public static IEnumerable<object[]> ToolStripSeparatorAccessibleObject_GetPropertyValue_ControlType_IsExpected_ForCustomRole_TestData()
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
        [MemberData(nameof(ToolStripSeparatorAccessibleObject_GetPropertyValue_ControlType_IsExpected_ForCustomRole_TestData))]
        public void ToolStripSeparatorAccessibleObject_GetPropertyValue_ControlType_IsExpected_ForCustomRole(AccessibleRole role)
        {
            using ToolStripSeparator toolStripSeparator = new ToolStripSeparator();
            toolStripSeparator.AccessibleRole = role;

            object actual = toolStripSeparator.AccessibilityObject.GetPropertyValue(UiaCore.UIA.ControlTypePropertyId);
            UiaCore.UIA expected = AccessibleRoleControlTypeMap.GetControlType(role);

            Assert.Equal(expected, actual);
        }
    }
}
