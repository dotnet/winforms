// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Xunit;
using static System.Windows.Forms.ToolStripProgressBar;
using static Interop;

namespace System.Windows.Forms.Tests
{
    public class ToolStripProgressBar_ToolStripProgressBarControl_ToolStripProgressBarControlAccessibleObjectTests : IClassFixture<ThreadExceptionFixture>
    {
        [WinFormsFact]
        public void ToolStripProgressBarControlAccessibleObject_ctor_default()
        {
            using ToolStripProgressBarControl toolStripProgressBarControl = new ToolStripProgressBarControl();
            ToolStripProgressBarControlAccessibleObject accessibleObject = new ToolStripProgressBarControlAccessibleObject(toolStripProgressBarControl);

            Assert.Equal(toolStripProgressBarControl, accessibleObject.Owner);
            Assert.False(toolStripProgressBarControl.IsHandleCreated);
        }

        [WinFormsFact]
        public void ToolStripProgressBarControlAccessibleObject_ControlType_IsProgressBar_IfAccessibleRoleIsDefault()
        {
            using ToolStripProgressBarControl toolStripProgressBarControl = new ToolStripProgressBarControl();
            // AccessibleRole is not set = Default

            object actual = toolStripProgressBarControl.AccessibilityObject.GetPropertyValue(UiaCore.UIA.ControlTypePropertyId);

            Assert.Equal(UiaCore.UIA.ProgressBarControlTypeId, actual);
            Assert.False(toolStripProgressBarControl.IsHandleCreated);
        }

        [WinFormsTheory]
        [InlineData(true, AccessibleRole.ProgressBar)]
        [InlineData(false, AccessibleRole.None)]
        public void ToolStripProgressBarControlAccessibleObject_Role_IsExpected_ByDefault(bool createControl, AccessibleRole expectedRole)
        {
            using ToolStripProgressBarControl toolStripProgressBarControl = new ToolStripProgressBarControl();
            // AccessibleRole is not set = Default

            if (createControl)
            {
                toolStripProgressBarControl.CreateControl();
            }

            object actual = toolStripProgressBarControl.AccessibilityObject.Role;

            Assert.Equal(expectedRole, actual);
            Assert.Equal(createControl, toolStripProgressBarControl.IsHandleCreated);
        }

        public static IEnumerable<object[]> ToolStripProgressBarControlAccessibleObject_GetPropertyValue_ControlType_IsExpected_ForCustomRole_TestData()
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
        [MemberData(nameof(ToolStripProgressBarControlAccessibleObject_GetPropertyValue_ControlType_IsExpected_ForCustomRole_TestData))]
        public void ToolStripProgressBarControlAccessibleObject_GetPropertyValue_ControlType_IsExpected_ForCustomRole(AccessibleRole role)
        {
            using ToolStripProgressBarControl toolStripProgressBarControl = new ToolStripProgressBarControl();
            toolStripProgressBarControl.AccessibleRole = role;

            AccessibleObject accessibleObject = toolStripProgressBarControl.AccessibilityObject;
            object actual = accessibleObject.GetPropertyValue(UiaCore.UIA.ControlTypePropertyId);
            UiaCore.UIA expected = AccessibleRoleControlTypeMap.GetControlType(role);

            Assert.Equal(role, accessibleObject.Role);
            Assert.Equal(expected, actual);
            Assert.False(toolStripProgressBarControl.IsHandleCreated);
        }

        [WinFormsTheory]
        [InlineData((int)UiaCore.NavigateDirection.FirstChild)]
        [InlineData((int)UiaCore.NavigateDirection.LastChild)]
        public void ToolStripProgressBarControlAccessibleObject_FragmentNavigate_ChildrenAreNull(int direction)
        {
            using ToolStripProgressBarControl control = new();

            object actual = control.AccessibilityObject.FragmentNavigate((UiaCore.NavigateDirection)direction);

            Assert.Null(actual);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsFact]
        public void ToolStripProgressBarControlAccessibleObject_FragmentNavigate_ParentIsToolStrip()
        {
            using NoAssertContext noAssertContext = new();
            using ToolStripProgressBarControl control = new();
            using ToolStripProgressBar item = new();
            using ToolStrip toolStrip = new();
            control.Owner = item;
            item.Parent = toolStrip;

            object actual = control.AccessibilityObject.FragmentNavigate(UiaCore.NavigateDirection.Parent);

            Assert.Equal(toolStrip.AccessibilityObject, actual);
            Assert.False(control.IsHandleCreated);
            Assert.False(toolStrip.IsHandleCreated);
        }

        [WinFormsFact]
        public void ToolStripProgressBarControlAccessibleObject_FragmentNavigate_SiblingsAreExpected()
        {
            using NoAssertContext noAssertContext = new();
            using ToolStripProgressBarControl control = new();
            using ToolStripProgressBar item = new();
            using ToolStrip toolStrip = new() { Size = new Drawing.Size(500, 25) };
            using ToolStripLabel label = new() { Text = "Label"};
            using ToolStripButton button = new() { Text = "Button"};

            toolStrip.CreateControl();
            control.Owner = item;
            item.Parent = toolStrip;
            toolStrip.Items.Add(label);
            toolStrip.Items.Add(item);
            toolStrip.Items.Add(button);

            object nextSibling = control.AccessibilityObject.FragmentNavigate(UiaCore.NavigateDirection.NextSibling);
            object previousSibling = control.AccessibilityObject.FragmentNavigate(UiaCore.NavigateDirection.PreviousSibling);

            Assert.Equal(button.AccessibilityObject, nextSibling);
            Assert.Equal(label.AccessibilityObject, previousSibling);
            Assert.False(control.IsHandleCreated);
            Assert.True(toolStrip.IsHandleCreated);
        }
    }
}
