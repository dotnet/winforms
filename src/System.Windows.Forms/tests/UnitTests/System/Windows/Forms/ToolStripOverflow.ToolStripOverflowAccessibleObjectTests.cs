// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Xunit;
using static System.Windows.Forms.ToolStripOverflow;
using static Interop;

namespace System.Windows.Forms.Tests
{
    public class ToolStripOverflow_ToolStripOverflowAccessibleObjectTests : IClassFixture<ThreadExceptionFixture>
    {
        [WinFormsFact]
        public void ToolStripOverflowAccessibleObject_ctor_default()
        {
            using ToolStripButton toolStripItem = new();
            using ToolStripOverflow toolStripOverflow = new(toolStripItem);
            ToolStripOverflowAccessibleObject accessibleObject = new(toolStripOverflow);

            Assert.Equal(toolStripOverflow, accessibleObject.Owner);
            Assert.False(toolStripOverflow.IsHandleCreated);
        }

        [WinFormsFact]
        public void ToolStripOverflowAccessibleObject_ControlType_IsToolBar_IfAccessibleRoleIsDefault()
        {
            using ToolStripButton toolStripItem = new();
            using ToolStripOverflow toolStripOverflow = new(toolStripItem);
            // AccessibleRole is not set = Default

            AccessibleObject accessibleObject = toolStripOverflow.AccessibilityObject;
            object actual = accessibleObject.GetPropertyValue(UiaCore.UIA.ControlTypePropertyId);

            Assert.Equal(UiaCore.UIA.MenuControlTypeId, actual);
            Assert.False(toolStripOverflow.IsHandleCreated);
        }

        [WinFormsFact]
        public void ToolStripOverflowAccessibleObject_Role_IsToolBar_ByDefault()
        {
            using ToolStripButton toolStripItem = new();
            using ToolStripOverflow toolStripOverflow = new(toolStripItem);
            // AccessibleRole is not set = Default

            object actual = toolStripOverflow.AccessibilityObject.Role;

            Assert.Equal(AccessibleRole.MenuPopup, actual);
            Assert.False(toolStripOverflow.IsHandleCreated);
        }

        public static IEnumerable<object[]> ToolStripOverflowAccessibleObject_GetPropertyValue_ControlType_IsExpected_ForCustomRole_TestData()
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
        [MemberData(nameof(ToolStripOverflowAccessibleObject_GetPropertyValue_ControlType_IsExpected_ForCustomRole_TestData))]
        public void ToolStripOverflowAccessibleObject_GetPropertyValue_ControlType_IsExpected_ForCustomRole(AccessibleRole role)
        {
            using ToolStripButton toolStripItem = new();
            using ToolStripOverflow toolStripOverflow = new(toolStripItem);
            toolStripOverflow.AccessibleRole = role;

            AccessibleObject accessibleObject = toolStripOverflow.AccessibilityObject;
            object actual = accessibleObject.GetPropertyValue(UiaCore.UIA.ControlTypePropertyId);
            UiaCore.UIA expected = AccessibleRoleControlTypeMap.GetControlType(role);

            Assert.Equal(role, accessibleObject.Role);
            Assert.Equal(expected, actual);
            Assert.False(toolStripOverflow.IsHandleCreated);
        }

        [WinFormsTheory]
        [InlineData((int)UiaCore.NavigateDirection.NextSibling)]
        [InlineData((int)UiaCore.NavigateDirection.PreviousSibling)]
        [InlineData((int)UiaCore.NavigateDirection.FirstChild)]
        [InlineData((int)UiaCore.NavigateDirection.LastChild)]
        public void ToolStripOverflowAccessibleObject_FragmentNavigate_ReturnsNull_IfHandleNotCreated(int navigateDirection)
        {
            using ToolStripButton toolStripItem = new();
            using ToolStripOverflow toolStripOverflow = new(toolStripItem);

            AccessibleObject accessibleObject = toolStripOverflow.AccessibilityObject;
            AccessibleObject expected = toolStripItem.AccessibilityObject;

            Assert.Null(accessibleObject.FragmentNavigate((UiaCore.NavigateDirection)navigateDirection));
            Assert.False(toolStripOverflow.IsHandleCreated);
        }

        [WinFormsFact]
        public void ToolStripOverflowAccessibleObject_FragmentNavigate_Parent_ReturnsExpected()
        {
            using ToolStripButton toolStripItem = new();
            using ToolStripOverflow toolStripOverflow = new(toolStripItem);

            AccessibleObject accessibleObject = toolStripOverflow.AccessibilityObject;
            AccessibleObject expected = toolStripItem.AccessibilityObject;

            Assert.Equal(expected, accessibleObject.FragmentNavigate(UiaCore.NavigateDirection.Parent));
        }

        [WinFormsFact]
        public void ToolStripOverflowAccessibleObject_FragmentNavigate_Sibling_ReturnsExpected()
        {
            using ToolStripButton toolStripItem = new();
            using ToolStripOverflow toolStripOverflow = new(toolStripItem);

            AccessibleObject accessibleObject = toolStripOverflow.AccessibilityObject;

            Assert.Null(accessibleObject.FragmentNavigate(UiaCore.NavigateDirection.NextSibling));
            Assert.Null(accessibleObject.FragmentNavigate(UiaCore.NavigateDirection.PreviousSibling));
        }

        [WinFormsFact]
        public void ToolStripOverflowAccessibleObject_FragmentNavigate_FirstChild_ReturnsExpected()
        {
            using ToolStrip toolStrip = CreateToolStripWithOverflow(5);

            toolStrip.OverflowButton.DropDown.CreateControl(ignoreVisible: true);

            AccessibleObject accessibleObject = toolStrip.OverflowButton.DropDown.AccessibilityObject;
            AccessibleObject expected = toolStrip.Items[0].AccessibilityObject;

            Assert.Equal(expected, accessibleObject.FragmentNavigate(UiaCore.NavigateDirection.FirstChild));
        }

        [WinFormsFact]
        public void ToolStripOverflowAccessibleObject_FragmentNavigate_FirstChild_ReturnsExpected_IfItemAligned()
        {
            using ToolStrip toolStrip = CreateToolStripWithOverflow(5);

            toolStrip.Items[0].Alignment = ToolStripItemAlignment.Right;

            toolStrip.PerformLayout();

            toolStrip.OverflowButton.DropDown.CreateControl(ignoreVisible: true);

            AccessibleObject accessibleObject = toolStrip.OverflowButton.DropDown.AccessibilityObject;
            AccessibleObject expected = toolStrip.Items[0].AccessibilityObject;

            Assert.Equal(expected, accessibleObject.FragmentNavigate(UiaCore.NavigateDirection.FirstChild));
        }

        [WinFormsFact]
        public void ToolStripOverflowAccessibleObject_FragmentNavigate_LastChild_ReturnsExpected()
        {
            using ToolStrip toolStrip = CreateToolStripWithOverflow(5);

            toolStrip.OverflowButton.DropDown.CreateControl(ignoreVisible: true);

            AccessibleObject accessibleObject = toolStrip.OverflowButton.DropDown.AccessibilityObject;
            AccessibleObject expected = toolStrip.Items[4].AccessibilityObject;

            Assert.Equal(expected, accessibleObject.FragmentNavigate(UiaCore.NavigateDirection.LastChild));
        }

        [WinFormsFact]
        public void ToolStripOverflowAccessibleObject_FragmentNavigate_LastChild_ReturnsExpected_IfItemAligned()
        {
            using ToolStrip toolStrip = CreateToolStripWithOverflow(5);

            toolStrip.Items[0].Alignment = ToolStripItemAlignment.Right;

            toolStrip.PerformLayout();

            toolStrip.OverflowButton.DropDown.CreateControl(ignoreVisible: true);

            AccessibleObject accessibleObject = toolStrip.OverflowButton.DropDown.AccessibilityObject;
            AccessibleObject expected = toolStrip.Items[4].AccessibilityObject;

            Assert.Equal(expected, accessibleObject.FragmentNavigate(UiaCore.NavigateDirection.LastChild));
        }

        [WinFormsFact]
        public void ToolStripOverflowAccessibleObject_FragmentNavigate_Child_ReturnsNullIfOverflowEmpty()
        {
            using ToolStripButton toolStripItem = new();
            using ToolStripOverflow toolStripOverflow = new(toolStripItem);

            toolStripOverflow.CreateControl(ignoreVisible: true);

            AccessibleObject accessibleObject = toolStripOverflow.AccessibilityObject;

            Assert.Null(accessibleObject.FragmentNavigate(UiaCore.NavigateDirection.FirstChild));
            Assert.Null(accessibleObject.FragmentNavigate(UiaCore.NavigateDirection.LastChild));
        }

        private static ToolStrip CreateToolStripWithOverflow(int itemCount)
        {
            ToolStrip toolStrip = new()
            {
                AutoSize = false,
                Size = new(20, 30),
                CanOverflow = true,
            };

            for (int i = 0; i < itemCount; i++)
            {
                toolStrip.Items.Add(CreateItem(i, ToolStripItemAlignment.Left));
            }

            return toolStrip;

            ToolStripItem CreateItem(int index, ToolStripItemAlignment align)
            {
                return new ToolStripButton
                {
                    AutoSize = false,
                    Size = new(25, 25),
                    Alignment = align
                };
            }
        }
    }
}
