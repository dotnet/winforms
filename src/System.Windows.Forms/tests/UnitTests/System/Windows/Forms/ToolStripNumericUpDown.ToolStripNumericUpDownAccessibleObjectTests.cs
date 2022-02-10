// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Xunit;
using static System.Windows.Forms.ToolStripControlHost;
using static Interop;

namespace System.Windows.Forms.Tests
{
    public class ToolStripNumericUpDown_ToolStripNumericUpDownAccessibleObjectTests : IClassFixture<ThreadExceptionFixture>
    {
        [WinFormsFact]
        public void ToolStripNumericUpDownAccessibleObject_Ctor_Default()
        {
            using ToolStripNumericUpDown toolStripNumericUpDown = new ToolStripNumericUpDown();
            ToolStripHostedControlAccessibleObject accessibleObject = (ToolStripHostedControlAccessibleObject)toolStripNumericUpDown.Control.AccessibilityObject;

            ToolStripNumericUpDown actual = accessibleObject.TestAccessor().Dynamic._toolStripControlHost;

            Assert.Equal(toolStripNumericUpDown, actual);
        }

        [WinFormsFact]
        public void ToolStripNumericUpDownAccessibleObject_ControlType_IsSpinner_IfAccessibleRoleIsDefault()
        {
            using ToolStripNumericUpDown toolStripNumericUpDown = new ToolStripNumericUpDown();
            // AccessibleRole is not set = Default

            Assert.Equal(UiaCore.UIA.SpinnerControlTypeId, toolStripNumericUpDown.Control.AccessibilityObject.GetPropertyValue(UiaCore.UIA.ControlTypePropertyId));
            Assert.Null(toolStripNumericUpDown.Control.AccessibilityObject.GetPropertyValue(UiaCore.UIA.ValueValuePropertyId));
        }

        [WinFormsTheory]
        [InlineData(true, AccessibleRole.Client)]
        [InlineData(false, AccessibleRole.None)]
        public void ToolStripNumericUpDownAccessibleObject_Role_IsExpected_ByDefault(bool createControl, AccessibleRole expectedRole)
        {
            using ToolStripNumericUpDown toolStripNumericUpDown = new ToolStripNumericUpDown();
            // AccessibleRole is not set = Default
            Control control = toolStripNumericUpDown.Control;

            if (createControl)
            {
                control.CreateControl();
            }

            AccessibleRole actual = toolStripNumericUpDown.AccessibilityObject.Role;

            Assert.Equal(expectedRole, actual);
            Assert.Equal(createControl, control.IsHandleCreated);
        }

        public static IEnumerable<object[]> ToolStripNumericUpDownAccessibleObject_GetPropertyValue_ControlType_IsExpected_ForCustomRole_TestData()
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
        [MemberData(nameof(ToolStripNumericUpDownAccessibleObject_GetPropertyValue_ControlType_IsExpected_ForCustomRole_TestData))]
        public void ToolStripNumericUpDownAccessibleObject_GetPropertyValue_ControlType_IsExpected_ForCustomRole(AccessibleRole role)
        {
            using ToolStripNumericUpDown toolStripNumericUpDown = new ToolStripNumericUpDown();
            toolStripNumericUpDown.AccessibleRole = role;

            object actual = toolStripNumericUpDown.AccessibilityObject.GetPropertyValue(UiaCore.UIA.ControlTypePropertyId);
            UiaCore.UIA expected = AccessibleRoleControlTypeMap.GetControlType(role);

            Assert.Equal(expected, actual);
        }
    }
}
