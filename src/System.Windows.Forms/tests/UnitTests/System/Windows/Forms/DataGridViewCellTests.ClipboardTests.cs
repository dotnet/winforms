// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#nullable enable

namespace System.Windows.Forms.Tests;

public partial class DataGridViewCellTests
{
    [Collection("Sequential")]
    [UISettings(MaxAttempts = 3)] // Try up to 3 times before failing.
    public class ClipboardTests
    {
        public static TheoryData<int, bool, bool, bool, bool, string?, object?> GetClipboardContent_TheoryData() => new()
        {
            { -2, true, true, true, true, "format", null },
            { -2, true, true, true, true, null, null },
            { -1, true, true, true, true, "format", null },
            { -1, true, true, true, true, null, null },
            { 0, true, true, true, true, "format", null },
            { 0, true, true, true, true, null, null },
        };

        [WinFormsTheory]
        [MemberData(nameof(GetClipboardContent_TheoryData))]
        public void DataGridViewCell_GetClipboardContent_Invoke_ReturnsExpected(int rowIndex, bool firstCell, bool lastCell, bool inFirstRow, bool inLastRow, string? format, object? expected)
        {
            using SubDataGridViewCell cell = new();
            cell.GetClipboardContent(rowIndex, firstCell, lastCell, inFirstRow, inLastRow, format).Should().Be(expected);
        }

        [WinFormsTheory]
        [MemberData(nameof(GetClipboardContent_TheoryData))]
        public void DataGridViewCell_GetClipboardContent_InvokeWithRow_ReturnsExpected(int rowIndex, bool firstCell, bool lastCell, bool inFirstRow, bool inLastRow, string? format, object? expected)
        {
            using DataGridViewRow row = new();
            using SubDataGridViewCell cell = new();
            row.Cells.Add(cell);
            cell.GetClipboardContent(rowIndex, firstCell, lastCell, inFirstRow, inLastRow, format).Should().Be(expected);
        }

        public static TheoryData<bool, bool, bool, bool, string?, object?> GetClipboardContent_WithColumn_TheoryData() => new()
        {
            { true, true, true, true, "format", null },
            { true, true, true, true, null, null }
        };

        [WinFormsTheory]
        [MemberData(nameof(GetClipboardContent_WithColumn_TheoryData))]
        public void DataGridViewCell_GetClipboardContent_InvokeWithColumn_ReturnsExpected(bool firstCell, bool lastCell, bool inFirstRow, bool inLastRow, string? format, object? expected)
        {
            using DataGridViewColumn column = new();
            using SubDataGridViewColumnHeaderCell cell = new();
            column.HeaderCell = cell;
            cell.GetClipboardContent(-1, firstCell, lastCell, inFirstRow, inLastRow, format).Should().Be(expected);
        }

        [WinFormsTheory]
        [MemberData(nameof(GetClipboardContent_WithColumn_TheoryData))]
        public void DataGridViewCell_GetClipboardContent_InvokeWithDataGridView_ReturnsExpected(bool firstCell, bool lastCell, bool inFirstRow, bool inLastRow, string? format, object? expected)
        {
            using SubDataGridViewCell cellTemplate = new();
            using DataGridViewColumn column = new()
            {
                CellTemplate = cellTemplate
            };
            using DataGridView control = new();
            control.Columns.Add(column);
            SubDataGridViewCell cell = (SubDataGridViewCell)control.Rows[0].Cells[0];
            cell.GetClipboardContent(0, firstCell, lastCell, inFirstRow, inLastRow, format).Should().Be(expected);
        }

        [WinFormsTheory]
        [MemberData(nameof(GetClipboardContent_WithColumn_TheoryData))]
        public void DataGridViewCell_GetClipboardContent_InvokeShared_ReturnsExpected(bool firstCell, bool lastCell, bool inFirstRow, bool inLastRow, string? format, object? expected)
        {
            using SubDataGridViewCell cellTemplate = new();
            using DataGridViewColumn column = new()
            {
                CellTemplate = cellTemplate
            };
            using DataGridView control = new();
            control.Columns.Add(column);
            SubDataGridViewCell cell = (SubDataGridViewCell)control.Rows.SharedRow(0).Cells[0];
            cell.GetClipboardContent(0, firstCell, lastCell, inFirstRow, inLastRow, format).Should().Be(expected);
        }

        [WinFormsTheory]
        [InlineData(-2)]
        [InlineData(0)]
        public void DataGridViewCell_GetClipboardContent_InvalidRowIndexWithColumn_ThrowsArgumentOutOfRangeException(int rowIndex)
        {
            using DataGridViewColumn column = new();
            using SubDataGridViewColumnHeaderCell cell = new();
            column.HeaderCell = cell;
            Action action = () => cell.GetClipboardContent(rowIndex, true, true, true, true, "format");
            action.Should().Throw<ArgumentOutOfRangeException>().WithParameterName("rowIndex");
        }

        [WinFormsTheory]
        [InlineData(-2)]
        [InlineData(-1)]
        [InlineData(1)]
        public void DataGridViewCell_GetClipboardContent_InvalidRowIndexWithDataGridView_ThrowsArgumentOutOfRangeException(int rowIndex)
        {
            using SubDataGridViewCell cellTemplate = new();
            using DataGridViewColumn column = new()
            {
                CellTemplate = cellTemplate
            };
            using DataGridView control = new();
            control.Columns.Add(column);
            SubDataGridViewCell cell = (SubDataGridViewCell)control.Rows[0].Cells[0];
            Action action = () => cell.GetClipboardContent(rowIndex, true, true, true, true, "format");
            action.Should().Throw<ArgumentOutOfRangeException>().WithParameterName("rowIndex");
        }

        [WinFormsTheory]
        [InlineData(-2)]
        [InlineData(-1)]
        [InlineData(1)]
        public void DataGridViewCell_GetClipboardContent_InvalidRowIndexShared_ThrowsArgumentOutOfRangeException(int rowIndex)
        {
            using SubDataGridViewCell cellTemplate = new();
            using DataGridViewColumn column = new()
            {
                CellTemplate = cellTemplate
            };
            using DataGridView control = new();
            control.Columns.Add(column);
            SubDataGridViewCell cell = (SubDataGridViewCell)control.Rows.SharedRow(0).Cells[0];
            Action action = () => cell.GetClipboardContent(rowIndex, true, true, true, true, "format");
            action.Should().Throw<ArgumentOutOfRangeException>().WithParameterName("rowIndex");
        }
    }
}
