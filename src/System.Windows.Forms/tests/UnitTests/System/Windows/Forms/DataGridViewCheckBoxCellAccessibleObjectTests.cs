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
                // Make sure that toggle state is off as a default case.
                Assert.Equal(UiaCore.ToggleState.Off, accessibleObject.ToggleState);
                // Check it.
                accessibleObject.DoDefaultAction();
            }

            Assert.Equal((UiaCore.ToggleState)expected, accessibleObject.ToggleState);
            Assert.True(control.IsHandleCreated);
        }

        [WinFormsFact]
        public void DataGridViewCheckBoxCellAccessibleObject_GetChildCount_ReturnsExpected()
        {
            var accessibleObject = new DataGridViewCheckBoxCellAccessibleObject(null);
            Assert.Equal(0, accessibleObject.GetChildCount());
        }

        [WinFormsFact]
        public void DataGridViewCheckBoxCellAccessibleObject_ControlType_ReturnsExpected()
        {
            var accessibleObject = new DataGridViewCheckBoxCellAccessibleObject(null);
            Assert.Equal(UiaCore.UIA.CheckBoxControlTypeId, accessibleObject.GetPropertyValue(UiaCore.UIA.ControlTypePropertyId));
        }

        [WinFormsFact]
        public void DataGridViewCheckBoxCellAccessibleObject_IsTogglePatternAvailablePropertyId_ReturnsExpected()
        {
            var accessibleObject = new DataGridViewCheckBoxCellAccessibleObject(null);

            Assert.True((bool)accessibleObject.GetPropertyValue(UiaCore.UIA.IsTogglePatternAvailablePropertyId));
        }

        [WinFormsFact]
        public void DataGridViewCheckBoxCellAccessibleObject_GetPropertyValue_ValueValuePropertyId_ReturnsExpected()
        {
            using DataGridView control = new();
            control.Columns.Add(new DataGridViewCheckBoxColumn());
            var cell = control.Rows[0].Cells[0];
            control.CreateControl();
            var accessibleObject = (DataGridViewCheckBoxCellAccessibleObject)cell.AccessibilityObject;

            Assert.False(bool.Parse(accessibleObject.GetPropertyValue(UiaCore.UIA.ValueValuePropertyId).ToString()));

            accessibleObject.DoDefaultAction();

            Assert.True(bool.Parse(accessibleObject.GetPropertyValue(UiaCore.UIA.ValueValuePropertyId).ToString()));
            Assert.True(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [InlineData((int)UiaCore.UIA.TogglePatternId)]
        public void DataGridViewCheckBoxCellAccessibleObject_IsPatternSupported_ReturnsExpected(int patternId)
        {
            var accessibleObject = new DataGridViewCheckBoxCellAccessibleObject(null);
            Assert.True(accessibleObject.IsPatternSupported((UiaCore.UIA)patternId));
        }

        [WinFormsFact]
        public void DataGridViewCheckBoxCellAccessibleObject_IsIAccessibleExSupported_ReturnsExpected()
        {
            var accessibleObject = new DataGridViewCheckBoxCellAccessibleObject(null);
            Assert.True(accessibleObject.IsIAccessibleExSupported());
        }

        [WinFormsFact]
        public void DataGridViewCheckBoxCellAccessibleObject_DefaultAction_ThrowsException_IfOwnerIsNull()
        {
            Assert.Throws<InvalidOperationException>(()
              => new DataGridViewCheckBoxCellAccessibleObject(null).DefaultAction);
        }

        [WinFormsFact]
        public void DataGridViewCheckBoxCellAccessibleObject_DefaultAction_ReturnsExpected_IfOwnerIsReadOnly()
        {
            using DataGridViewCheckBoxCell cell = new();
            using DataGridViewRow row = new();
            row.Cells.Add(cell);
            cell.ReadOnly = true;

            Assert.True(cell.ReadOnly);
            Assert.Equal(string.Empty, cell.AccessibilityObject.DefaultAction);
        }

        public static IEnumerable<object[]> DataGridViewCheckBoxCellAccessibleObject_DefaultAction_TestData()
        {
            yield return new object[] { false, SR.DataGridView_AccCheckBoxCellDefaultActionCheck };
            yield return new object[] { true, SR.DataGridView_AccCheckBoxCellDefaultActionUncheck };
        }

        [WinFormsTheory]
        [MemberData(nameof(DataGridViewCheckBoxCellAccessibleObject_DefaultAction_TestData))]
        public void DataGridViewCheckBoxCellAccessibleObject_DefaultAction_ReturnsExpected(bool isChecked, string expected)
        {
            using DataGridView control = new();
            control.Columns.Add(new DataGridViewCheckBoxColumn());
            control.Rows.Add(new DataGridViewRow());
            var cell = control.Rows[0].Cells[0];
            // Create control to check cell if it is needed.
            control.CreateControl();

            var accessibleObject = (DataGridViewCheckBoxCellAccessibleObject)cell.AccessibilityObject;
            if (isChecked)
            {
                // Make sure that default action is check as a default case.
                Assert.Equal(SR.DataGridView_AccCheckBoxCellDefaultActionCheck, accessibleObject.DefaultAction);
                // Check it.
                accessibleObject.DoDefaultAction();
            }

            Assert.Equal(expected, accessibleObject.DefaultAction);
            Assert.True(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [MemberData(nameof(DataGridViewCheckBoxCellAccessibleObject_DefaultAction_TestData))]
        public void DataGridViewCheckBoxCellAccessibleObject_GetPropertyValue_LegacyIAccessibleDefaultActionPropertyId_ReturnsExpected(bool isChecked, string expected)
        {
            using DataGridView control = new();
            control.Columns.Add(new DataGridViewCheckBoxColumn());
            control.Rows.Add(new DataGridViewRow());
            var cell = control.Rows[0].Cells[0];
            // Create control to check cell if it is needed.
            control.CreateControl();

            var accessibleObject = (DataGridViewCheckBoxCellAccessibleObject)cell.AccessibilityObject;
            if (isChecked)
            {
                // Make sure that default action is check as a default case.
                Assert.Equal(SR.DataGridView_AccCheckBoxCellDefaultActionCheck, accessibleObject.GetPropertyValue(UiaCore.UIA.LegacyIAccessibleDefaultActionPropertyId));
                // Check it.
                accessibleObject.DoDefaultAction();
            }

            Assert.Equal(expected, accessibleObject.GetPropertyValue(UiaCore.UIA.LegacyIAccessibleDefaultActionPropertyId));
            Assert.True(control.IsHandleCreated);
        }

        [WinFormsFact]
        public void DataGridViewCheckBoxCellAccessibleObject_ToggleState_ThrowsException_IfOwnerIsNull()
        {
            Assert.Throws<InvalidOperationException>(()
              => new DataGridViewCheckBoxCellAccessibleObject(null).ToggleState);
        }

        public static IEnumerable<object[]> DataGridViewCheckBoxCellAccessibleObject_DoDefaultAction_TestData()
        {
            yield return new object[] { false, true, (int)UiaCore.ToggleState.Off };
            yield return new object[] { false, false, (int)UiaCore.ToggleState.Off };
            yield return new object[] { true, true, (int)UiaCore.ToggleState.Off };
            yield return new object[] { true, false, (int)UiaCore.ToggleState.On };
        }

        [WinFormsFact]
        public void DataGridViewCheckBoxCellAccessibleObject_DoDefaultAction_ThrowsException_IfOwnerIsNull()
        {
            Assert.Throws<InvalidOperationException>(()
              => new DataGridViewCheckBoxCellAccessibleObject(null).DoDefaultAction());
        }

        [WinFormsFact]
        public void DataGridViewCheckBoxCellAccessibleObject_DoDefaultAction_ThrowsException_IfRowIndexIsIncorrect()
        {
            using DataGridViewCheckBoxCell cell = new();

            Assert.Equal(-1, cell.RowIndex);
            Assert.Throws<InvalidOperationException>(() => cell.AccessibilityObject.DoDefaultAction());
        }

        [WinFormsTheory]
        [MemberData(nameof(DataGridViewCheckBoxCellAccessibleObject_DoDefaultAction_TestData))]
        public void DataGridViewCheckBoxCellAccessibleObject_DoDefaultAction_WorksExpected(bool createControl, bool isChecked, int expected)
        {
            using DataGridView control = new();
            control.Columns.Add(new DataGridViewCheckBoxColumn());
            control.Rows.Add(new DataGridViewRow());
            var cell = control.Rows[0].Cells[0];
            if (createControl)
            {
                control.CreateControl();
            }

            var accessibleObject = (DataGridViewCheckBoxCellAccessibleObject)cell.AccessibilityObject;
            if (isChecked)
            {
                // Make sure that toogle state is off as a default case.
                Assert.Equal(UiaCore.ToggleState.Off, accessibleObject.ToggleState);
                // Check it.
                accessibleObject.DoDefaultAction();

                if (createControl)
                {
                    // Make sure that toogle state has been changed.
                    Assert.Equal(UiaCore.ToggleState.On, accessibleObject.ToggleState);
                }
                else
                {
                    // Make sure that nothing changes.
                    Assert.Equal(UiaCore.ToggleState.Off, accessibleObject.ToggleState);
                }
            }

            accessibleObject.DoDefaultAction();

            Assert.Equal((UiaCore.ToggleState)expected, accessibleObject.ToggleState);
            Assert.Equal(createControl, control.IsHandleCreated);
        }
    }
}
