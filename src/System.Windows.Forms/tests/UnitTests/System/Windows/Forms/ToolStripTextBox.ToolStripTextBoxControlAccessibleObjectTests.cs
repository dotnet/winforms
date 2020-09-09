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
            ControlAccessibleObject accessibleObject = (ControlAccessibleObject)Activator.CreateInstance(type, textBox, toolStripTextBox);
            Assert.Equal(textBox, accessibleObject.Owner);
        }

        [WinFormsFact]
        public void ToolStripTextBoxControlAccessibleObject_ctor_ThrowsException_IfOwnerIsNull()
        {
            using ToolStripTextBox toolStripTextBox = new ToolStripTextBox();
            TextBox textBox = toolStripTextBox.TextBox;
            Type type = toolStripTextBox.GetType().GetNestedType("ToolStripTextBoxControlAccessibleObject", BindingFlags.NonPublic);
            Assert.NotNull(type);
            Assert.Throws<TargetInvocationException>(() => Activator.CreateInstance(type, (Control)null, toolStripTextBox));
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
    }
}
