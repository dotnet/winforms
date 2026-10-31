// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms.Tests;

public class DataGridViewRowCollectionTests
{
    [Fact]
    public void DataGridViewRowCollection_GetVisibleIndex_LastRow_VisitsEachRowOnce()
    {
        const int RowCount = 1000;
        using SubDataGridView dataGridView = new()
        {
            AllowUserToAddRows = false,
            ColumnCount = 1,
            RowCount = RowCount
        };

        CountingDataGridViewRowCollection rows = dataGridView.CountingRows;
        DataGridViewRow lastRow = rows[RowCount - 1];
        rows.GetRowStateCallCount = 0;

        rows.GetVisibleIndex(lastRow).Should().Be(RowCount - 1);
        rows.GetRowStateCallCount.Should().Be(RowCount);
    }

    private sealed class SubDataGridView : DataGridView
    {
        public CountingDataGridViewRowCollection CountingRows => (CountingDataGridViewRowCollection)Rows;

        protected override DataGridViewRowCollection CreateRowsInstance()
            => new CountingDataGridViewRowCollection(this);
    }

    private sealed class CountingDataGridViewRowCollection(DataGridView dataGridView) : DataGridViewRowCollection(dataGridView)
    {
        public int GetRowStateCallCount { get; set; }

        public override DataGridViewElementStates GetRowState(int rowIndex)
        {
            GetRowStateCallCount++;
            return base.GetRowState(rowIndex);
        }
    }
}
