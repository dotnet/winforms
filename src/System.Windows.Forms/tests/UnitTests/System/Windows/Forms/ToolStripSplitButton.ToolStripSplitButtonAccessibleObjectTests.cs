// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using Xunit;
using static System.Windows.Forms.ToolStripSplitButton;
using static Interop;

namespace System.Windows.Forms.Tests
{
    public class ToolStripSplitButton_ToolStripSplitButtonAccessibleObjectTests : IClassFixture<ThreadExceptionFixture>
    {
        [WinFormsFact]
        public void ToolStripSplitButtonAccessibleObject_Ctor_Default()
        {
            using ToolStripSplitButton toolStripSplitButton = new ToolStripSplitButton();
            ToolStripSplitButtonAccessibleObject accessibleObject = new ToolStripSplitButtonAccessibleObject(toolStripSplitButton);

            Assert.Equal(toolStripSplitButton, accessibleObject.Owner);
        }

        [WinFormsFact]
        public void ToolStripSplitButtonAccessibleObject_ControlType_IsButton_IfAccessibleRoleIsDefault()
        {
            using ToolStripSplitButton toolStripSplitButton = new ToolStripSplitButton();
            // AccessibleRole is not set = Default

            object actual = toolStripSplitButton.AccessibilityObject.GetPropertyValue(UiaCore.UIA.ControlTypePropertyId);

            Assert.Equal(UiaCore.UIA.ButtonControlTypeId, actual);
        }

        [WinFormsFact]
        public void ToolStripSplitButtonAccessibleObject_Role_IsMenuItem_ByDefault()
        {
            using ToolStripSplitButton toolStripSplitButton = new ToolStripSplitButton();
            // AccessibleRole is not set = Default

            AccessibleRole actual = toolStripSplitButton.AccessibilityObject.Role;

            Assert.Equal(AccessibleRole.MenuItem, actual);
        }

        public static IEnumerable<object[]> ToolStripSplitButtonAccessibleObject_GetPropertyValue_ControlType_IsExpected_ForCustomRole_TestData()
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
        [MemberData(nameof(ToolStripSplitButtonAccessibleObject_GetPropertyValue_ControlType_IsExpected_ForCustomRole_TestData))]
        public void ToolStripSplitButtonAccessibleObject_GetPropertyValue_ControlType_IsExpected_ForCustomRole(AccessibleRole role)
        {
            using ToolStripSplitButton toolStripSplitButton = new ToolStripSplitButton();
            toolStripSplitButton.AccessibleRole = role;

            object actual = toolStripSplitButton.AccessibilityObject.GetPropertyValue(UiaCore.UIA.ControlTypePropertyId);
            UiaCore.UIA expected = AccessibleRoleControlTypeMap.GetControlType(role);

            Assert.Equal(expected, actual);
        }
    }
}
