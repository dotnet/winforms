// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Windows.Win32.UI.Accessibility;

namespace System.Windows.Forms.Tests.AccessibleObjects;

public class DataGridViewComboBoxCellAccessibleObjectTests
{
    public static IEnumerable<object[]> DataGridViewComboBoxCellAccessibleObject_SupportExpandCollapsePattern_TestData()
    {
        foreach (DataGridViewComboBoxDisplayStyle displayStyle in Enum.GetValues(typeof(DataGridViewComboBoxDisplayStyle)))
        {
            foreach (bool cellIsEdited in new[] { true, false })
            {
                bool expectedValue = displayStyle != DataGridViewComboBoxDisplayStyle.Nothing || cellIsEdited;
                yield return new object[] { displayStyle, cellIsEdited, expectedValue };
            }
        }
    }

    [WinFormsTheory]
    [MemberData(nameof(DataGridViewComboBoxCellAccessibleObject_SupportExpandCollapsePattern_TestData))]
    public void DataGridViewComboBoxCellAccessibleObject_SupportExpandCollapsePattern_ReturnExpected(DataGridViewComboBoxDisplayStyle displayStyle, bool cellIsEdited, bool expectedValue)
    {
        using DataGridView dataGridView = new();
        dataGridView.Columns.Add(new DataGridViewComboBoxColumn() { DisplayStyle = displayStyle });
        dataGridView.Rows.Add();
        dataGridView.CurrentCell = dataGridView.Rows[0].Cells[0];

        if (cellIsEdited)
        {
            dataGridView.BeginEdit(false);
        }

        bool actualPropertyValue = (bool)dataGridView.Rows[0].Cells[0].AccessibilityObject.GetPropertyValue(UIA_PROPERTY_ID.UIA_IsExpandCollapsePatternAvailablePropertyId);

        Assert.Equal(expectedValue, actualPropertyValue);
    }

    public static IEnumerable<object[]> DataGridViewComboBoxCellAccessibleObject_ControlType_TestData()
    {
        foreach (DataGridViewComboBoxDisplayStyle displayStyle in Enum.GetValues(typeof(DataGridViewComboBoxDisplayStyle)))
        {
            foreach (bool cellIsEdited in new[] { true, false })
            {
                int expectedControlType = displayStyle != DataGridViewComboBoxDisplayStyle.Nothing || cellIsEdited
                                    ? (int)UIA_CONTROLTYPE_ID.UIA_ComboBoxControlTypeId
                                    : (int)UIA_CONTROLTYPE_ID.UIA_DataItemControlTypeId;

                yield return new object[] { displayStyle, cellIsEdited, expectedControlType };
            }
        }
    }

    [WinFormsTheory]
    [MemberData(nameof(DataGridViewComboBoxCellAccessibleObject_ControlType_TestData))]
    public void DataGridViewComboBoxCellAccessibleObject_ControlType_ReturnExpected(DataGridViewComboBoxDisplayStyle displayStyle, bool cellIsEdited, int expectedControlType)
    {
        using DataGridView dataGridView = new();
        dataGridView.Columns.Add(new DataGridViewComboBoxColumn() { DisplayStyle = displayStyle });
        dataGridView.Rows.Add();
        dataGridView.CurrentCell = dataGridView.Rows[0].Cells[0];

        if (cellIsEdited)
        {
            dataGridView.BeginEdit(false);
        }

        int actualPropertyValue = (int)dataGridView.Rows[0].Cells[0].AccessibilityObject.GetPropertyValue(UIA_PROPERTY_ID.UIA_ControlTypePropertyId);

        Assert.Equal(expectedControlType, actualPropertyValue);
    }
}
