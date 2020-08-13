// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Xunit;
using static Interop;

namespace System.Windows.Forms.Tests.AccessibleObjects
{
    public class DataGridViewComboBoxCellAccessibleObjectTests : IClassFixture<ThreadExceptionFixture>
    {
        [WinFormsTheory]
        [InlineData((int)UiaCore.UIA.IsExpandCollapsePatternAvailablePropertyId, true)]
        public void GetPropertyValue_Returns_Correct_Value(int propertyID, object expectedPropertyValue)
        {
            using var dataGridView = new DataGridView();
            DataGridViewComboBoxColumn column = new DataGridViewComboBoxColumn();
            dataGridView.Columns.Add(column);
            dataGridView.Rows.Add();

            object actualPropertyValue = dataGridView.Rows[0].Cells[0].AccessibilityObject.GetPropertyValue((UiaCore.UIA)propertyID);

            Assert.Equal(expectedPropertyValue, actualPropertyValue);
        }
    }
}
