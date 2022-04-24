// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Reflection;
using Xunit;
using static System.Windows.Forms.Control;
using static Interop;

namespace System.Windows.Forms.Tests
{
    public class ToolStripTextBox_ToolStripTextBoxControlAccessibleObjectTests : IClassFixture<ThreadExceptionFixture>
    {
        [WinFormsFact]
        public void ToolStripTextBoxControlAccessibleObject_ctor_default()
        {
            using ToolStripTextBox toolStripTextBox = new ToolStripTextBox();
            TextBox textBox = toolStripTextBox.TextBox;
            Type type = toolStripTextBox.GetType().GetNestedType("ToolStripTextBoxControlAccessibleObject", BindingFlags.NonPublic);
            Assert.NotNull(type);
            ControlAccessibleObject accessibleObject = (ControlAccessibleObject)Activator.CreateInstance(type, textBox);
            Assert.Equal(textBox, accessibleObject.Owner);
        }

        [WinFormsFact]
        public void ToolStripTextBoxControlAccessibleObject_ctor_ThrowsException_IfOwnerIsNull()
        {
            using ToolStripTextBox toolStripTextBox = new ToolStripTextBox();
            TextBox textBox = toolStripTextBox.TextBox;
            Type type = toolStripTextBox.GetType().GetNestedType("ToolStripTextBoxControlAccessibleObject", BindingFlags.NonPublic);
            Assert.NotNull(type);
            Assert.Throws<TargetInvocationException>(() => Activator.CreateInstance(type, (Control)null));
        }

        [WinFormsTheory]
        [InlineData(true)]
        [InlineData(false)]
        public void ToolStripTextBoxControlAccessibleObject_IsReadOnly_IsExpected(bool readOnly)
        {
            using ToolStripTextBox toolStripTextBox = new ToolStripTextBox();
            TextBox textBox = toolStripTextBox.TextBox;
            textBox.ReadOnly = readOnly;
            AccessibleObject accessibleObject = textBox.AccessibilityObject;
            Assert.Equal(readOnly, accessibleObject.IsReadOnly);
        }

        [WinFormsTheory]
        [InlineData((int)UiaCore.UIA.IsTextPatternAvailablePropertyId)]
        [InlineData((int)UiaCore.UIA.IsTextPattern2AvailablePropertyId)]
        [InlineData((int)UiaCore.UIA.IsValuePatternAvailablePropertyId)]
        public void ToolStripTextBoxControlAccessibleObject_GetPropertyValue_PatternsSuported(int propertyID)
        {
            using ToolStripTextBox toolStripTextBox = new ToolStripTextBox();
            TextBox textBox = toolStripTextBox.TextBox;
            AccessibleObject accessibleObject = textBox.AccessibilityObject;
            Assert.True((bool)accessibleObject.GetPropertyValue((UiaCore.UIA)propertyID));
        }

        [WinFormsTheory]
        [InlineData((int)UiaCore.UIA.ValuePatternId)]
        [InlineData((int)UiaCore.UIA.TextPatternId)]
        [InlineData((int)UiaCore.UIA.TextPattern2Id)]
        public void ToolStripTextBoxControlAccessibleObject_IsPatternSupported_PatternsSuported(int patternId)
        {
            using ToolStripTextBox toolStripTextBox = new ToolStripTextBox();
            TextBox textBox = toolStripTextBox.TextBox;
            AccessibleObject accessibleObject = textBox.AccessibilityObject;
            Assert.True(accessibleObject.IsPatternSupported((UiaCore.UIA)patternId));
        }

        [WinFormsTheory]
        [InlineData(true, (int)UiaCore.UIA.EditControlTypeId)]
        [InlineData(false, (int)UiaCore.UIA.PaneControlTypeId)]
        public void ToolStripTextBoxControlAccessibleObject_ControlType_IsExpected_IfAccessibleRoleIsDefault(bool createControl, int expectedType)
        {
            using ToolStripTextBox toolStripTextBox = new ToolStripTextBox();
            // AccessibleRole is not set = Default
            TextBox toolStripTextBoxControl = toolStripTextBox.TextBox;

            if (createControl)
            {
                toolStripTextBoxControl.CreateControl();
            }

            object actual = toolStripTextBox.AccessibilityObject.GetPropertyValue(UiaCore.UIA.ControlTypePropertyId);

            Assert.Equal((UiaCore.UIA)expectedType, actual);
            Assert.Equal(createControl, toolStripTextBoxControl.IsHandleCreated);
        }

        [WinFormsTheory]
        [InlineData(true, AccessibleRole.Text)]
        [InlineData(false, AccessibleRole.None)]
        public void ToolStripTextBoxControlAccessibleObject_Default_Role_IsExpected(bool createControl, AccessibleRole expectedRole)
        {
            using ToolStripTextBox toolStripTextBox = new ToolStripTextBox();
            // AccessibleRole is not set = Default
            TextBox toolStripTextBoxControl = toolStripTextBox.TextBox;

            if (createControl)
            {
                toolStripTextBoxControl.CreateControl();
            }

            object actual = toolStripTextBox.AccessibilityObject.Role;

            Assert.Equal(expectedRole, actual);
            Assert.Equal(createControl, toolStripTextBoxControl.IsHandleCreated);
        }

        public static IEnumerable<object[]> ToolStripTextBoxControlAccessibleObject_GetPropertyValue_ControlType_IsExpected_ForCustomRole_TestData()
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
        [MemberData(nameof(ToolStripTextBoxControlAccessibleObject_GetPropertyValue_ControlType_IsExpected_ForCustomRole_TestData))]
        public void ToolStripTextBoxControlAccessibleObject_GetPropertyValue_ControlType_IsExpected_ForCustomRole(AccessibleRole role)
        {
            using ToolStripTextBox toolStripTextBox = new ToolStripTextBox();
            toolStripTextBox.AccessibleRole = role;

            TextBox toolStripTextBoxControl = toolStripTextBox.TextBox;
            AccessibleObject accessibleObject = toolStripTextBox.AccessibilityObject;
            object actual = accessibleObject.GetPropertyValue(UiaCore.UIA.ControlTypePropertyId);
            UiaCore.UIA expected = AccessibleRoleControlTypeMap.GetControlType(role);

            Assert.Equal(role, accessibleObject.Role);
            Assert.Equal(expected, actual);
            Assert.False(toolStripTextBoxControl.IsHandleCreated);
        }
    }
}
