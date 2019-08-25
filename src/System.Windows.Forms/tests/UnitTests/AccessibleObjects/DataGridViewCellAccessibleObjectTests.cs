// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics;
using System.Threading;
using Xunit;

namespace System.Windows.Forms.Tests.AccessibleObjects
{
    public class DataGridViewCellsAccessibleObjectTests
    {
        [Theory]
        [InlineData(RightToLeft.No)]
        [InlineData(RightToLeft.Yes)]
        public void DataGridViewCellsAccessibleObject_Ctor_Default(RightToLeft rightToLeft)
        {
            DataGridView dataGridView = new DataGridView
            {
                RightToLeft = rightToLeft,
                ColumnCount = 4,
                Width = 85
            };

            dataGridView.Columns[0].Width = 40;
            dataGridView.Columns[1].Width = 40;
            dataGridView.Columns[2].Width = 40;
            dataGridView.Columns[3].Width = 40;

            AccessibleObject rr = dataGridView.AccessibilityObject; //it is necessary to be in time to initialize elements

            var accCellWidthSum = 0;
            for (int i = 0; i < 4; i++)
            {
                accCellWidthSum += dataGridView.Rows[0].Cells[i].AccessibilityObject.BoundingRectangle.Width;
            }
            var accRowWidth = dataGridView.Rows[0].AccessibilityObject.BoundingRectangle.Width;

            Assert.True(accCellWidthSum == accRowWidth - dataGridView.RowHeadersWidth);
        }

        [StaFact]
        public void DataGridViewCellsAccessibleObject_IsReadOnly_property()
        {
            DataGridView dataGridView = new DataGridView();
            dataGridView.Columns.Add(new DataGridViewTextBoxColumn());
            dataGridView.Columns.Add(new DataGridViewTextBoxColumn());
            dataGridView.Rows.Add(new DataGridViewRow()); 
            dataGridView.Rows.Add(new DataGridViewRow()); 

            dataGridView.Rows[0].Cells[0].ReadOnly = true;
            dataGridView.Rows[1].ReadOnly = true;
            dataGridView.Rows[2].ReadOnly = false;

            foreach (DataGridViewRow row in dataGridView.Rows)
            {
                foreach (DataGridViewCell cell in row.Cells)
                {
                    bool value = cell.AccessibilityObject.IsReadOnly;

                    Assert.Equal(cell.ReadOnly, value);
                }
            }
        }
    }
}
