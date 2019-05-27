// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Xunit;

namespace System.Windows.Forms.Tests.AccessibleObjects
{
    public class DataGridViewRowsAccessibleObjectTests
    {
        [Fact]
        public void DataGridViewRowsAccessibleObject_Ctor_Default()
        {
            DataGridView dataGridView = new DataGridView
            {
                RowCount = 5,
                Height = 87
            };

            dataGridView.Rows[0].Height = 20;
            dataGridView.Rows[1].Height = 20;
            dataGridView.Rows[2].Height = 20;
            dataGridView.Rows[3].Height = 20;
            dataGridView.Rows[4].Height = 20;

            AccessibleObject accObject = dataGridView.AccessibilityObject; //it is necessary to be in time to initialize elements

            int accRowHeightSum = 0;
            for (int i = 0; i < 5; i++)
            {
                accRowHeightSum += dataGridView.Rows[i].AccessibilityObject.BoundingRectangle.Height;
            }
            int accDataGridViewHeight = dataGridView.AccessibilityObject.BoundingRectangle.Height;
            int borders = 2 * dataGridView.BorderWidth; //top border and bottom border

            Assert.True(accRowHeightSum == accDataGridViewHeight - borders - dataGridView.ColumnHeadersHeight);
        }
    }
}
