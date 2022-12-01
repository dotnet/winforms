// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Xunit;
using static System.Windows.Forms.ToolStripOverflowButton;
using static Interop;

namespace System.Windows.Forms.Tests
{
    public class ToolStripOverflowButton_ToolStripOverflowButtonAccessibleObjectTests : IClassFixture<ThreadExceptionFixture>
    {
        [WinFormsFact]
        public void ToolStripOverflowButtonAccessibleObject_Ctor_Default()
        {
            using ToolStrip toolStrip = new ToolStrip();
            ToolStripOverflowButton toolStripOverflowButton = new ToolStripOverflowButton(toolStrip);
            ToolStripOverflowButtonAccessibleObject accessibleObject = new ToolStripOverflowButtonAccessibleObject(toolStripOverflowButton);

            Assert.Equal(toolStripOverflowButton, accessibleObject.Owner);
            Assert.False(toolStrip.IsHandleCreated);
        }

        [WinFormsFact]
        public void ToolStripOverflowButtonAccessibleObject_ControlType_IsMenuItem_IfAccessibleRoleIsDefault()
        {
            using ToolStrip toolStrip = new ToolStrip();
            ToolStripOverflowButton toolStripOverflowButton = new ToolStripOverflowButton(toolStrip);
            // AccessibleRole is not set = Default

            object actual = toolStripOverflowButton.AccessibilityObject.GetPropertyValue(UiaCore.UIA.ControlTypePropertyId);

            Assert.Equal(UiaCore.UIA.MenuItemControlTypeId, actual);
            Assert.False(toolStrip.IsHandleCreated);
        }

        [WinFormsFact]
        public void ToolStripOverflowButtonAccessibleObject_Role_IsMenuItem_ByDefault()
        {
            using ToolStrip toolStrip = new ToolStrip();
            ToolStripOverflowButton toolStripOverflowButton = new ToolStripOverflowButton(toolStrip);
            // AccessibleRole is not set = Default

            AccessibleRole actual = toolStripOverflowButton.AccessibilityObject.Role;

            Assert.Equal(AccessibleRole.MenuItem, actual);
            Assert.False(toolStrip.IsHandleCreated);
        }

        public static IEnumerable<object[]> ToolStripOverflowButtonAccessibleObject_GetPropertyValue_ControlType_IsExpected_ForCustomRole_TestData()
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
        [MemberData(nameof(ToolStripOverflowButtonAccessibleObject_GetPropertyValue_ControlType_IsExpected_ForCustomRole_TestData))]
        public void ToolStripOverflowButtonAccessibleObject_GetPropertyValue_ControlType_IsExpected_ForCustomRole(AccessibleRole role)
        {
            using ToolStrip toolStrip = new ToolStrip();
            ToolStripOverflowButton toolStripOverflowButton = new ToolStripOverflowButton(toolStrip);
            toolStripOverflowButton.AccessibleRole = role;

            object actual = toolStripOverflowButton.AccessibilityObject.GetPropertyValue(UiaCore.UIA.ControlTypePropertyId);
            UiaCore.UIA expected = AccessibleRoleControlTypeMap.GetControlType(role);

            Assert.Equal(expected, actual);
        }

        [WinFormsFact]
        public void ToolStripOverflowButtonAccessibleObject_FragmentNavigate_Child_ReturnExpected()
        {
            using ToolStrip toolStrip = new() { AutoSize = false, Size = new(30, 30) };

            toolStrip.Items.Add(string.Empty);
            toolStrip.Items.Add(string.Empty);
            toolStrip.Items.Add(string.Empty);

            toolStrip.CreateControl();
            toolStrip.PerformLayout();

            AccessibleObject accessibleObject = toolStrip.OverflowButton.AccessibilityObject;

            Assert.Null(accessibleObject.FragmentNavigate(UiaCore.NavigateDirection.FirstChild));
            Assert.Null(accessibleObject.FragmentNavigate(UiaCore.NavigateDirection.LastChild));

            toolStrip.OverflowButton.DropDown.Show();

            AccessibleObject expected = toolStrip.OverflowButton.DropDown.AccessibilityObject;

            Assert.Equal(expected, accessibleObject.FragmentNavigate(UiaCore.NavigateDirection.FirstChild));
            Assert.Equal(expected, accessibleObject.FragmentNavigate(UiaCore.NavigateDirection.LastChild));
        }

        [WinFormsFact]
        public void ToolStripOverflowButtonAccessibleObject_FragmentNavigate_Siblings_ReturnExpected()
        {
            using ToolStrip toolStrip = new() { AutoSize = false, Size = new(60, 30) };

            // 1 item displayed and 2 overflown
            toolStrip.Items.Add(string.Empty);
            toolStrip.Items.Add(string.Empty);
            toolStrip.Items.Add(string.Empty);

            toolStrip.PerformLayout();

            AccessibleObject overflowButton = toolStrip.OverflowButton.AccessibilityObject;
            AccessibleObject item1 = toolStrip.Items[0].AccessibilityObject;

            Assert.Null(overflowButton.FragmentNavigate(UiaCore.NavigateDirection.NextSibling));
            Assert.Equal(item1, overflowButton.FragmentNavigate(UiaCore.NavigateDirection.PreviousSibling));

            Assert.False(toolStrip.IsHandleCreated);
        }
    }
}
