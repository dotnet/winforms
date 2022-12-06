// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Xunit;
using static System.Windows.Forms.ToolStripMenuItem;
using static Interop;

namespace System.Windows.Forms.Tests
{
    public class ToolStripMenuItem_ToolStripMenuItemAccessibleObjectTests : IClassFixture<ThreadExceptionFixture>
    {
        [WinFormsFact]
        public void ToolStripMenuItemAccessibleObject_Ctor_Default()
        {
            using ToolStripMenuItem toolStripMenuItem = new ToolStripMenuItem();
            ToolStripMenuItemAccessibleObject accessibleObject = new ToolStripMenuItemAccessibleObject(toolStripMenuItem);

            Assert.Equal(toolStripMenuItem, accessibleObject.Owner);
        }

        [WinFormsFact]
        public void ToolStripMenuItemAccessibleObject_ControlType_IsMenuItem_IfAccessibleRoleIsDefault()
        {
            using ToolStripMenuItem toolStripMenuItem = new ToolStripMenuItem();
            // AccessibleRole is not set = Default

            object actual = toolStripMenuItem.AccessibilityObject.GetPropertyValue(UiaCore.UIA.ControlTypePropertyId);

            Assert.Equal(UiaCore.UIA.MenuItemControlTypeId, actual);
        }

        [WinFormsFact]
        public void ToolStripMenuItemAccessibleObject_Role_IsMenuItem_ByDefault()
        {
            using ToolStripMenuItem toolStripMenuItem = new ToolStripMenuItem();
            // AccessibleRole is not set = Default

            AccessibleRole actual = toolStripMenuItem.AccessibilityObject.Role;

            Assert.Equal(AccessibleRole.MenuItem, actual);
        }

        public static IEnumerable<object[]> ToolStripMenuItemAccessibleObject_GetPropertyValue_ControlType_IsExpected_ForCustomRole_TestData()
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
        [MemberData(nameof(ToolStripMenuItemAccessibleObject_GetPropertyValue_ControlType_IsExpected_ForCustomRole_TestData))]
        public void ToolStripMenuItemAccessibleObject_GetPropertyValue_ControlType_IsExpected_ForCustomRole(AccessibleRole role)
        {
            using ToolStripMenuItem toolStripMenuItem = new ToolStripMenuItem();
            toolStripMenuItem.AccessibleRole = role;

            object actual = toolStripMenuItem.AccessibilityObject.GetPropertyValue(UiaCore.UIA.ControlTypePropertyId);
            UiaCore.UIA expected = AccessibleRoleControlTypeMap.GetControlType(role);

            Assert.Equal(expected, actual);
        }

        [WinFormsTheory]
        [InlineData(true, CheckState.Checked, true)]
        [InlineData(true, CheckState.Unchecked, true)]
        [InlineData(true, CheckState.Indeterminate, true)]
        [InlineData(false, CheckState.Checked, true)]
        [InlineData(false, CheckState.Unchecked, false)]
        [InlineData(false, CheckState.Indeterminate, true)]
        public void ToolStripMenuItemAccessibleObject_IsTogglePatternSupported_ReturnExpected(bool checkOnClick, CheckState checkState, bool expected)
        {
            using ToolStripMenuItem toolStripMenuItem = new()
            {
                CheckOnClick = checkOnClick,
                CheckState = checkState
            };

            object actual = toolStripMenuItem.AccessibilityObject.IsPatternSupported(UiaCore.UIA.TogglePatternId);

            Assert.Equal(expected, actual);
        }

        [WinFormsTheory]
        [InlineData(CheckState.Checked, (int)UiaCore.ToggleState.On)]
        [InlineData(CheckState.Unchecked, (int)UiaCore.ToggleState.Off)]
        [InlineData(CheckState.Indeterminate, (int)UiaCore.ToggleState.Indeterminate)]
        public void ToolStripMenuItemAccessibleObject_ToggleState_ReturnsExpected(CheckState checkState, int expectedToggleState)
        {
            using ToolStripMenuItem toolStripMenuItem = new()
            {
                CheckState = checkState
            };

            object actual = toolStripMenuItem.AccessibilityObject.ToggleState;

            Assert.Equal((UiaCore.ToggleState)expectedToggleState, actual);
        }

        [WinFormsFact]
        public void ToolStripMenuItemAccessibleObject_Toggle_Invoke()
        {
            using ToolStripMenuItem toolStripMenuItem = new()
            {
                CheckOnClick = true
            };

            int clickCounter = 0;

            toolStripMenuItem.Click += (s, e) => { clickCounter++; };

            Assert.Equal(UiaCore.ToggleState.Off, toolStripMenuItem.AccessibilityObject.ToggleState);

            toolStripMenuItem.AccessibilityObject.Toggle();

            Assert.Equal(UiaCore.ToggleState.On, toolStripMenuItem.AccessibilityObject.ToggleState);

            toolStripMenuItem.AccessibilityObject.Toggle();

            Assert.Equal(UiaCore.ToggleState.Off, toolStripMenuItem.AccessibilityObject.ToggleState);

            Assert.Equal(0, clickCounter);
        }
    }
}
