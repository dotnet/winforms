// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Xunit;
using static Interop;

namespace System.Windows.Forms.Tests
{
    public class TableLayoutPanel_TableLayoutPanelAccessibilityObjectTests : IClassFixture<ThreadExceptionFixture>
    {
        [WinFormsFact]
        public void TableLayoutPanelAccessibilityObject_Ctor_Default()
        {
            using TableLayoutPanel tableLayoutPanel = new TableLayoutPanel();
            tableLayoutPanel.CreateControl();

            Assert.NotNull(tableLayoutPanel.AccessibilityObject);
            Assert.True(tableLayoutPanel.IsHandleCreated);
        }

        [WinFormsFact]
        public void TableLayoutPanelAccessibilityObject_ControlType_IsPane_IfAccessibleRoleIsDefault()
        {
            using TableLayoutPanel tableLayoutPanel = new TableLayoutPanel();
            tableLayoutPanel.CreateControl();
            // AccessibleRole is not set = Default

            object actual = tableLayoutPanel.AccessibilityObject.GetPropertyValue(UiaCore.UIA.ControlTypePropertyId);

            Assert.Equal(UiaCore.UIA.PaneControlTypeId, actual);
            Assert.True(tableLayoutPanel.IsHandleCreated);
        }

        [WinFormsFact]
        public void TableLayoutPanelAccessibilityObject_Role_IsClient_ByDefault()
        {
            using TableLayoutPanel tableLayoutPanel = new TableLayoutPanel();
            tableLayoutPanel.CreateControl();
            // AccessibleRole is not set = Default

            AccessibleRole actual = tableLayoutPanel.AccessibilityObject.Role;

            Assert.Equal(AccessibleRole.Client, actual);
            Assert.True(tableLayoutPanel.IsHandleCreated);
        }

        public static IEnumerable<object[]> TableLayoutPanelAccessibleObject_GetPropertyValue_ControlType_IsExpected_ForCustomRole_TestData()
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
        [MemberData(nameof(TableLayoutPanelAccessibleObject_GetPropertyValue_ControlType_IsExpected_ForCustomRole_TestData))]
        public void TableLayoutPanelAccessibleObject_GetPropertyValue_ControlType_IsExpected_ForCustomRole(AccessibleRole role)
        {
            using TableLayoutPanel tableLayoutPanel = new TableLayoutPanel();
            tableLayoutPanel.AccessibleRole = role;

            object actual = tableLayoutPanel.AccessibilityObject.GetPropertyValue(UiaCore.UIA.ControlTypePropertyId);
            UiaCore.UIA expected = AccessibleRoleControlTypeMap.GetControlType(role);

            Assert.Equal(expected, actual);
            Assert.False(tableLayoutPanel.IsHandleCreated);
        }
    }
}
