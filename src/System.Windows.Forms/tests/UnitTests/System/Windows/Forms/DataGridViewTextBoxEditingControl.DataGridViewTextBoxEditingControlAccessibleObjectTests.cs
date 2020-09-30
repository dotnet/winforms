// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Xunit;
using static System.Windows.Forms.DataGridViewTextBoxEditingControl;
using static Interop;

namespace System.Windows.Forms.Tests
{
    public class DataGridViewTextBoxEditingControl_DataGridViewTextBoxEditingControlAccessibleObjectTests : IClassFixture<ThreadExceptionFixture>
    {
        [WinFormsFact]
        public void DataGridViewTextBoxEditingControlAccessibleObject_Ctor_Default()
        {
            using DataGridViewTextBoxEditingControl textCellControl = new DataGridViewTextBoxEditingControl();
            DataGridViewTextBoxEditingControlAccessibleObject accessibleObject = new DataGridViewTextBoxEditingControlAccessibleObject(textCellControl);
            Assert.Equal(textCellControl, accessibleObject.Owner);
        }

        [WinFormsFact]
        public void DataGridViewTextBoxEditingControlAccessibleObject_Ctor_ThrowsException_IfOwnerIsNull()
        {
            using DataGridViewTextBoxEditingControl textCellControl = new DataGridViewTextBoxEditingControl();
            Assert.Throws<ArgumentNullException>(() => new DataGridViewTextBoxEditingControlAccessibleObject(null));
        }

        [WinFormsTheory]
        [InlineData(true)]
        [InlineData(false)]
        public void ToolStripTextBoxControlAccessibleObject_IsReadOnly_IsExpected(bool readOnly)
        {
            using DataGridViewTextBoxEditingControl textCellControl = new DataGridViewTextBoxEditingControl();
            textCellControl.ReadOnly = readOnly;
            AccessibleObject accessibleObject = textCellControl.AccessibilityObject;
            Assert.Equal(readOnly, accessibleObject.IsReadOnly);
        }

        [WinFormsTheory]
        [InlineData((int)UiaCore.UIA.IsTextPatternAvailablePropertyId)]
        [InlineData((int)UiaCore.UIA.IsTextPattern2AvailablePropertyId)]
        [InlineData((int)UiaCore.UIA.IsValuePatternAvailablePropertyId)]
        public void ToolStripTextBoxControlAccessibleObject_GetPropertyValue_PatternsSuported(int propertyID)
        {
            using DataGridViewTextBoxEditingControl textCellControl = new DataGridViewTextBoxEditingControl();
            AccessibleObject accessibleObject = textCellControl.AccessibilityObject;
            Assert.True((bool)accessibleObject.GetPropertyValue((UiaCore.UIA)propertyID));
        }

        [WinFormsTheory]
        [InlineData((int)UiaCore.UIA.ValuePatternId)]
        [InlineData((int)UiaCore.UIA.TextPatternId)]
        [InlineData((int)UiaCore.UIA.TextPattern2Id)]
        public void ToolStripTextBoxControlAccessibleObject_IsPatternSupported_PatternsSuported(int patternId)
        {
            using DataGridViewTextBoxEditingControl textCellControl = new DataGridViewTextBoxEditingControl();
            AccessibleObject accessibleObject = textCellControl.AccessibilityObject;
            Assert.True(accessibleObject.IsPatternSupported((UiaCore.UIA)patternId));
        }
    }
}
