// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Xunit;
using static Interop;

namespace System.Windows.Forms.Tests
{
    public class DataGridViewCheckBoxCellAccessibleObjectTests : DataGridViewCheckBoxCell
    {
        [WinFormsFact]
        public void DataGridViewCheckBoxCellAccessibleObject_Ctor_Default()
        {
            var accessibleObject = new DataGridViewCheckBoxCellAccessibleObject(null);
            Assert.Null(accessibleObject.Owner);
            Assert.Equal(AccessibleRole.Cell, accessibleObject.Role);
        }

        public static IEnumerable<object[]> DataGridViewCheckBoxCellAccessibleObject_ToggleState_TestData()
        {
            yield return new object[] { false, (int)UiaCore.ToggleState.Off };
            yield return new object[] { true, (int)UiaCore.ToggleState.On };
        }

        [WinFormsTheory]
        [MemberData(nameof(DataGridViewCheckBoxCellAccessibleObject_ToggleState_TestData))]
        public void DataGridViewCheckBoxCellAccessibleObject_ToggleState_ReturnsExpected(bool isChecked, int expected)
        {
            using DataGridView control = new();
            control.Columns.Add(new DataGridViewCheckBoxColumn());
            var cell = control.Rows[0].Cells[0];
            // Create control to check cell if it is needed.
            control.CreateControl();

            var accessibleObject = (DataGridViewCheckBoxCellAccessibleObject)cell.AccessibilityObject;
            if (isChecked)
            {
                // Make sure that toggle state is on as a default case.
                Assert.Equal(UiaCore.ToggleState.Off, accessibleObject.ToggleState);
                // Check it.
                accessibleObject.DoDefaultAction();
            }

            Assert.Equal((UiaCore.ToggleState)expected, accessibleObject.ToggleState);
            Assert.True(control.IsHandleCreated);
        }
    }
}
