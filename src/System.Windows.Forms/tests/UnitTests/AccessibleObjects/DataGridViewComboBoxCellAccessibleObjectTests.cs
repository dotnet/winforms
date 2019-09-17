// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Xunit;

namespace System.Windows.Forms.Tests.AccessibleObjects
{
    public class DataGridViewComboBoxCellAccessibleObjectTests
    {
        [Theory]
        [InlineData(NativeMethods.UIA_IsExpandCollapsePatternAvailablePropertyId, true)]
        public void GetPropertyValue_Returns_Correct_Value(int propertyID, object expectedPropertyValue)
        {
            DataGridView dataGridView = new DataGridView();
            DataGridViewComboBoxColumn column = new DataGridViewComboBoxColumn();
            dataGridView.Columns.Add(column);
            dataGridView.Rows.Add();

            object actualPropertyValue = dataGridView.Rows[0].Cells[0].AccessibilityObject.GetPropertyValue(propertyID);

            Assert.Equal(expectedPropertyValue, actualPropertyValue);
        }
    }
}
