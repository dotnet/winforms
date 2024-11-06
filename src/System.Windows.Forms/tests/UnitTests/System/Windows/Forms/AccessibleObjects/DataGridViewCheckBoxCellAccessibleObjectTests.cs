// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Windows.Win32.UI.Accessibility;

namespace System.Windows.Forms.Tests.AccessibleObjects;

public class DataGridViewCheckBoxCellAccessibleObjectTests : DataGridViewCheckBoxCell
{
    [WinFormsFact]
    public void DataGridViewCheckBoxCellAccessibleObject_Ctor_Default()
    {
        DataGridViewCheckBoxCellAccessibleObject accessibleObject = new(null);
        Assert.Null(accessibleObject.Owner);
        Assert.Equal(AccessibleRole.Cell, accessibleObject.Role);
    }

    public static IEnumerable<object[]> DataGridViewCheckBoxCellAccessibleObject_ToggleState_TestData()
    {
        yield return new object[] { false, (int)ToggleState.ToggleState_Off };
        yield return new object[] { true, (int)ToggleState.ToggleState_On };
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
            Assert.Equal(ToggleState.ToggleState_Off, accessibleObject.ToggleState);
            // Check it.
            accessibleObject.DoDefaultAction();
        }

        Assert.Equal((ToggleState)expected, accessibleObject.ToggleState);
        Assert.True(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void DataGridViewCheckBoxCellAccessibleObject_GetChildCount_ReturnsExpected()
    {
        DataGridViewCheckBoxCellAccessibleObject accessibleObject = new(null);
        Assert.Equal(0, accessibleObject.GetChildCount());
    }

    [WinFormsFact]
    public void DataGridViewCheckBoxCellAccessibleObject_ControlType_ReturnsExpected()
    {
        DataGridViewCheckBoxCellAccessibleObject accessibleObject = new(null);
        Assert.Equal(UIA_CONTROLTYPE_ID.UIA_CheckBoxControlTypeId, (UIA_CONTROLTYPE_ID)(int)accessibleObject.GetPropertyValue(UIA_PROPERTY_ID.UIA_ControlTypePropertyId));
    }

    [WinFormsFact]
    public void DataGridViewCheckBoxCellAccessibleObject_IsTogglePatternAvailablePropertyId_ReturnsExpected()
    {
        DataGridViewCheckBoxCellAccessibleObject accessibleObject = new(null);

        Assert.True((bool)accessibleObject.GetPropertyValue(UIA_PROPERTY_ID.UIA_IsTogglePatternAvailablePropertyId));
    }

    [WinFormsFact]
    public void DataGridViewCheckBoxCellAccessibleObject_GetPropertyValue_ValueValuePropertyId_ReturnsExpected()
    {
        using DataGridView control = new();
        control.Columns.Add(new DataGridViewCheckBoxColumn());
        var cell = control.Rows[0].Cells[0];
        control.CreateControl();
        var accessibleObject = (DataGridViewCheckBoxCellAccessibleObject)cell.AccessibilityObject;

        Assert.False(bool.Parse(((BSTR)accessibleObject.GetPropertyValue(UIA_PROPERTY_ID.UIA_ValueValuePropertyId)).ToStringAndFree()));

        accessibleObject.DoDefaultAction();

        Assert.True(bool.Parse(((BSTR)accessibleObject.GetPropertyValue(UIA_PROPERTY_ID.UIA_ValueValuePropertyId)).ToStringAndFree()));
        Assert.True(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [InlineData((int)UIA_PATTERN_ID.UIA_TogglePatternId)]
    public void DataGridViewCheckBoxCellAccessibleObject_IsPatternSupported_ReturnsExpected(int patternId)
    {
        DataGridViewCheckBoxCellAccessibleObject accessibleObject = new(null);
        Assert.True(accessibleObject.IsPatternSupported((UIA_PATTERN_ID)patternId));
    }

    [WinFormsFact]
    public void DataGridViewCheckBoxCellAccessibleObject_IsIAccessibleExSupported_ReturnsExpected()
    {
        DataGridViewCheckBoxCellAccessibleObject accessibleObject = new(null);
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
            Assert.Equal(SR.DataGridView_AccCheckBoxCellDefaultActionCheck, ((BSTR)accessibleObject.GetPropertyValue(UIA_PROPERTY_ID.UIA_LegacyIAccessibleDefaultActionPropertyId)).ToStringAndFree());
            // Check it.
            accessibleObject.DoDefaultAction();
        }

        Assert.Equal(expected, ((BSTR)accessibleObject.GetPropertyValue(UIA_PROPERTY_ID.UIA_LegacyIAccessibleDefaultActionPropertyId)).ToStringAndFree());
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
        yield return new object[] { false, true, (int)ToggleState.ToggleState_Off };
        yield return new object[] { false, false, (int)ToggleState.ToggleState_Off };
        yield return new object[] { true, true, (int)ToggleState.ToggleState_Off };
        yield return new object[] { true, false, (int)ToggleState.ToggleState_On };
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
        Assert.Throws<InvalidOperationException>(cell.AccessibilityObject.DoDefaultAction);
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
            Assert.Equal(ToggleState.ToggleState_Off, accessibleObject.ToggleState);
            // Check it.
            accessibleObject.DoDefaultAction();

            if (createControl)
            {
                // Make sure that toogle state has been changed.
                Assert.Equal(ToggleState.ToggleState_On, accessibleObject.ToggleState);
            }
            else
            {
                // Make sure that nothing changes.
                Assert.Equal(ToggleState.ToggleState_Off, accessibleObject.ToggleState);
            }
        }

        accessibleObject.DoDefaultAction();

        Assert.Equal((ToggleState)expected, accessibleObject.ToggleState);
        Assert.Equal(createControl, control.IsHandleCreated);
    }

    // Unit test for https://github.com/dotnet/winforms/issues/9256
    [WinFormsFact]
    public void DataGridViewCheckBoxCellAccessibleObject_ToggleStateIndeterminate_ReturnsExpected()
    {
        using DataGridView dataGridView = new();
        using DataGridViewCheckBoxColumn checkBoxColumn = new();
        checkBoxColumn.ThreeState = true;
        dataGridView.Columns.Add(checkBoxColumn);
        using DataGridViewRow row = new();
        dataGridView.Rows.Add(row);
        dataGridView.CreateControl();

        DataGridViewCell cell = dataGridView.Rows[0].Cells[0];

        var checkBoxAccessibleObject = cell.AccessibilityObject;

        Assert.Equal(ToggleState.ToggleState_Indeterminate, checkBoxAccessibleObject.ToggleState);
        checkBoxAccessibleObject.DoDefaultAction();
        Assert.Equal(ToggleState.ToggleState_Off, checkBoxAccessibleObject.ToggleState);
        checkBoxAccessibleObject.DoDefaultAction();
        Assert.Equal(ToggleState.ToggleState_On, checkBoxAccessibleObject.ToggleState);
        checkBoxAccessibleObject.DoDefaultAction();
        Assert.Equal(ToggleState.ToggleState_Indeterminate, checkBoxAccessibleObject.ToggleState);
        checkBoxAccessibleObject.DoDefaultAction();
        Assert.Equal(ToggleState.ToggleState_Off, checkBoxAccessibleObject.ToggleState);
        checkBoxAccessibleObject.DoDefaultAction();
        Assert.Equal(ToggleState.ToggleState_On, checkBoxAccessibleObject.ToggleState);

        dataGridView.Rows.Add(true);
        Assert.Equal(ToggleState.ToggleState_On, dataGridView.Rows[1].Cells[0].AccessibilityObject.ToggleState);

        dataGridView.Rows.Add(false);
        Assert.Equal(ToggleState.ToggleState_Off, dataGridView.Rows[2].Cells[0].AccessibilityObject.ToggleState);

        dataGridView.Rows.Add(CheckState.Indeterminate);
        Assert.Equal(ToggleState.ToggleState_Indeterminate, dataGridView.Rows[3].Cells[0].AccessibilityObject.ToggleState);

        dataGridView.Rows.Add(CheckState.Checked);
        Assert.Equal(ToggleState.ToggleState_On, dataGridView.Rows[4].Cells[0].AccessibilityObject.ToggleState);

        dataGridView.Rows.Add(CheckState.Unchecked);
        Assert.Equal(ToggleState.ToggleState_Off, dataGridView.Rows[5].Cells[0].AccessibilityObject.ToggleState);

        checkBoxColumn.TrueValue = "on";
        checkBoxColumn.FalseValue = "off";
        checkBoxColumn.IndeterminateValue = "not set";

        dataGridView.Rows.Add("on");
        Assert.Equal(ToggleState.ToggleState_On, dataGridView.Rows[4].Cells[0].AccessibilityObject.ToggleState);

        dataGridView.Rows.Add("off");
        Assert.Equal(ToggleState.ToggleState_Off, dataGridView.Rows[5].Cells[0].AccessibilityObject.ToggleState);

        dataGridView.Rows.Add("not set");
        Assert.Equal(ToggleState.ToggleState_Indeterminate, dataGridView.Rows[3].Cells[0].AccessibilityObject.ToggleState);
    }
}
