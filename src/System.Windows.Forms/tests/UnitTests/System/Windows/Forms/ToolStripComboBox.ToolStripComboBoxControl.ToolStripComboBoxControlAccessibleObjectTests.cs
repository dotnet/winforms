// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Xunit;
using static System.Windows.Forms.ToolStripComboBox;
using static System.Windows.Forms.ToolStripComboBox.ToolStripComboBoxControl;
using static Interop;

namespace System.Windows.Forms.Tests
{
    public class ToolStripComboBox_ToolStripComboBoxControl_ToolStripComboBoxControlAccessibleObjectTests : IClassFixture<ThreadExceptionFixture>
    {
        [WinFormsFact]
        public void ToolStripComboBoxControlAccessibleObject_ctor_default()
        {
            using ToolStripComboBox toolStripComboBox = new ToolStripComboBox();
            ToolStripComboBoxControl control = (ToolStripComboBoxControl)toolStripComboBox.ComboBox;
            control.CreateControl();
            ToolStripComboBoxControlAccessibleObject accessibleObject = new ToolStripComboBoxControlAccessibleObject(control);

            Assert.Equal(control, accessibleObject.Owner);
            Assert.True(control.IsHandleCreated);
        }

        [WinFormsFact]
        public void ToolStripComboBoxControlAccessibleObject_ControlType_IsComboBox_IfAccessibleRoleIsDefault()
        {
            using ToolStripComboBox toolStripComboBox = new ToolStripComboBox();
            // AccessibleRole is not set = Default
            ToolStripComboBoxControl control = (ToolStripComboBoxControl)toolStripComboBox.ComboBox;
            control.CreateControl();

            AccessibleObject accessibleObject = toolStripComboBox.AccessibilityObject;
            object actual = accessibleObject.GetPropertyValue(UiaCore.UIA.ControlTypePropertyId);

            Assert.Equal(AccessibleRole.ComboBox, accessibleObject.Role);
            Assert.Equal(UiaCore.UIA.ComboBoxControlTypeId, actual);
            Assert.True(control.IsHandleCreated);
        }

        public static IEnumerable<object[]> ToolStripComboBoxControlAccessibleObject_GetPropertyValue_ControlType_IsExpected_ForCustomRole_TestData()
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
        [MemberData(nameof(ToolStripComboBoxControlAccessibleObject_GetPropertyValue_ControlType_IsExpected_ForCustomRole_TestData))]
        public void ToolStripComboBoxControlAccessibleObject_GetPropertyValue_ControlType_IsExpected_ForCustomRole(AccessibleRole role)
        {
            using ToolStripComboBox toolStripComboBox = new ToolStripComboBox();
            toolStripComboBox.AccessibleRole = role;
            ToolStripComboBoxControl control = (ToolStripComboBoxControl)toolStripComboBox.ComboBox;
            control.CreateControl();

            AccessibleObject accessibleObject = toolStripComboBox.AccessibilityObject;
            object actual = accessibleObject.GetPropertyValue(UiaCore.UIA.ControlTypePropertyId);

            Assert.Equal(role, accessibleObject.Role);
            UiaCore.UIA expected = AccessibleRoleControlTypeMap.GetControlType(role);
            Assert.Equal(expected, actual);
        }

        [WinFormsFact]
        public void ToolStripComboBoxControlAccessibleObject_FragmentNavigate_ChildrenAreExpected()
        {
            using ToolStripComboBoxControl control = new();
            control.CreateControl();

            object firstChild = control.AccessibilityObject.FragmentNavigate(UiaCore.NavigateDirection.FirstChild);
            object lastChild = control.AccessibilityObject.FragmentNavigate(UiaCore.NavigateDirection.LastChild);

            Assert.Equal(control.ChildEditAccessibleObject, firstChild);
            Assert.Equal(((ToolStripComboBoxControlAccessibleObject)control.AccessibilityObject).DropDownButtonUiaProvider, lastChild);
            Assert.True(control.IsHandleCreated);
        }

        [WinFormsFact]
        public void ToolStripComboBoxControlAccessibleObject_FragmentNavigate_ParentIsToolStrip()
        {
            using NoAssertContext noAssertContext = new();
            using ToolStripComboBoxControl control = new();
            using ToolStripComboBox item = new();
            using ToolStrip toolStrip = new();
            control.Owner = item;
            item.Parent = toolStrip;

            object actual = control.AccessibilityObject.FragmentNavigate(UiaCore.NavigateDirection.Parent);

            Assert.Equal(toolStrip.AccessibilityObject, actual);
            Assert.False(control.IsHandleCreated);
            Assert.False(toolStrip.IsHandleCreated);
        }
    }
}
