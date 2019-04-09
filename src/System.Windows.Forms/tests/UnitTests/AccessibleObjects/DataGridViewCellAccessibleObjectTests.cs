// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Xunit;

namespace System.Windows.Forms.Tests.AccessibleObjects
{
    public class DataGridViewCellsAccessibleObjectTests
    {
        [Fact]
        public void DataGridViewCellsAccessibleObject_Ctor_Default()
        {
            DataGridView dataGridView = new DataGridView();
            
            dataGridView.ColumnCount = 4;
            dataGridView.Width = 130;

            dataGridView.Columns[0].Width = 40;
            dataGridView.Columns[1].Width = 40;
            dataGridView.Columns[2].Width = 40;
            dataGridView.Columns[3].Width = 40;

            var accCellWidthSum = 0;
            for(int i = 0; i < 4; i++)
            {
                accCellWidthSum += dataGridView.Rows[0].Cells[i].AccessibilityObject.BoundingRectangle.Width;
            }
            var accRowWidth = dataGridView.Rows[0].AccessibilityObject.BoundingRectangle.Width;

            Assert.True(accCellWidthSum <= accRowWidth);
        }
    }
}
