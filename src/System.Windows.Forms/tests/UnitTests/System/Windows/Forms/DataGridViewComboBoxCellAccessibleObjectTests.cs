// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Xunit;
using static Interop;

namespace System.Windows.Forms.Tests
{
    public class DataGridViewComboBoxCellAccessibleObjectTests : IClassFixture<ThreadExceptionFixture>
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
            using var dataGridView = new DataGridView();
            dataGridView.Columns.Add(new DataGridViewComboBoxColumn() { DisplayStyle = displayStyle });
            dataGridView.Rows.Add();
            dataGridView.CurrentCell = dataGridView.Rows[0].Cells[0];

            if (cellIsEdited)
            {
                dataGridView.BeginEdit(false);
            }

            object actualPropertyValue = dataGridView.Rows[0].Cells[0].AccessibilityObject.GetPropertyValue(UiaCore.UIA.IsExpandCollapsePatternAvailablePropertyId);

            Assert.Equal(expectedValue, actualPropertyValue);
        }

        public static IEnumerable<object[]> DataGridViewComboBoxCellAccessibleObject_ControlType_TestData()
        {
            foreach (DataGridViewComboBoxDisplayStyle displayStyle in Enum.GetValues(typeof(DataGridViewComboBoxDisplayStyle)))
            {
                foreach (bool cellIsEdited in new[] { true, false })
                {
                    int expectedControlType = displayStyle != DataGridViewComboBoxDisplayStyle.Nothing || cellIsEdited
                                        ? (int)UiaCore.UIA.ComboBoxControlTypeId
                                        : (int)UiaCore.UIA.DataItemControlTypeId;

                    yield return new object[] { displayStyle, cellIsEdited, expectedControlType };
                }
            }
        }

        [WinFormsTheory]
        [MemberData(nameof(DataGridViewComboBoxCellAccessibleObject_ControlType_TestData))]
        public void DataGridViewComboBoxCellAccessibleObject_ControlType_ReturnExpected(DataGridViewComboBoxDisplayStyle displayStyle, bool cellIsEdited, int expectedControlType)
        {
            using var dataGridView = new DataGridView();
            dataGridView.Columns.Add(new DataGridViewComboBoxColumn() { DisplayStyle = displayStyle });
            dataGridView.Rows.Add();
            dataGridView.CurrentCell = dataGridView.Rows[0].Cells[0];

            if (cellIsEdited)
            {
                dataGridView.BeginEdit(false);
            }

            int actualPropertyValue = (int)dataGridView.Rows[0].Cells[0].AccessibilityObject.GetPropertyValue(UiaCore.UIA.ControlTypePropertyId);

            Assert.Equal(expectedControlType, actualPropertyValue);
        }
    }
}
