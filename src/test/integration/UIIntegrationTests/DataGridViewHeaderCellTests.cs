// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Windows.Forms.VisualStyles;

namespace System.Windows.Forms.UITests;

// Migrated from unit tests; see issue #4500. Remaining skipped tests in this file deferred to a follow-up PR.
public class DataGridViewHeaderCellTests : ControlTestBase
{
    public DataGridViewHeaderCellTests(ITestOutputHelper testOutputHelper)
        : base(testOutputHelper)
    {
    }

    public static IEnumerable<object[]> MouseLeaveUnsharesRow_WithDataGridViewMouseDown_TestData()
    {
        yield return new object[] { true, -2 };
        yield return new object[] { true, -1 };
        yield return new object[] { true, 0 };
        yield return new object[] { true, 1 };
        yield return new object[] { false, -2 };
        yield return new object[] { false, -1 };
        yield return new object[] { false, 0 };
        yield return new object[] { false, 1 };
    }

    [WinFormsTheory]
    [MemberData(nameof(MouseLeaveUnsharesRow_WithDataGridViewMouseDown_TestData))]
    public void DataGridViewHeaderCell_MouseLeaveUnsharesRow_InvokeWithDataGridViewMouseDown_ReturnsExpected(bool enableHeadersVisualStyles, int rowIndex)
    {
        using SubDataGridViewHeaderCell cellTemplate = new();
        using DataGridViewColumn column = new()
        {
            CellTemplate = cellTemplate
        };
        using DataGridView control = new()
        {
            EnableHeadersVisualStyles = enableHeadersVisualStyles
        };
        control.Columns.Add(column);
        SubDataGridViewHeaderCell cell = (SubDataGridViewHeaderCell)control.Rows[0].Cells[0];
        cell.OnMouseDown(new DataGridViewCellMouseEventArgs(-1, -1, 0, 0, new MouseEventArgs(MouseButtons.Left, 0, 0, 0, 0)));

        // Compute expected after the base ctor enabled visual styles; reading IsSupported during
        // MemberData enumeration can run too early and silently degrade the visual-styles cases.
        bool expectPushed = enableHeadersVisualStyles && VisualStyleRenderer.IsSupported;
        Assert.Equal(expectPushed, cell.MouseLeaveUnsharesRow(rowIndex));
        Assert.Equal(expectPushed ? ButtonState.Pushed : ButtonState.Normal, cell.ButtonState);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void DataGridViewHeaderCell_OnMouseDown_InvalidRowIndexVisualStyles_ThrowsArgumentOutOfRangeException()
    {
        using SubDataGridViewHeaderCell cellTemplate = new();
        using DataGridViewColumn column = new()
        {
            CellTemplate = cellTemplate
        };
        using DataGridView control = new()
        {
            EnableHeadersVisualStyles = true
        };
        control.Columns.Add(column);
        SubDataGridViewHeaderCell cell = (SubDataGridViewHeaderCell)control.Rows[0].Cells[0];
        DataGridViewCellMouseEventArgs e = new(0, 1, 0, 0, new MouseEventArgs(MouseButtons.Left, 0, 0, 0, 0));
        Assert.Throws<ArgumentOutOfRangeException>("rowIndex", () => cell.OnMouseDown(e));
        Assert.Equal(VisualStyleRenderer.IsSupported ? ButtonState.Pushed : ButtonState.Normal, cell.ButtonState);
    }

    [WinFormsFact]
    public void DataGridViewHeaderCell_OnMouseUp_InvalidRowIndexVisualStyles_ThrowsArgumentOutOfRangeException()
    {
        using SubDataGridViewHeaderCell cellTemplate = new();
        using DataGridViewColumn column = new()
        {
            CellTemplate = cellTemplate
        };
        using DataGridView control = new()
        {
            EnableHeadersVisualStyles = true
        };
        control.Columns.Add(column);
        SubDataGridViewHeaderCell cell = (SubDataGridViewHeaderCell)control.Rows[0].Cells[0];
        DataGridViewCellMouseEventArgs e = new(0, 1, 0, 0, new MouseEventArgs(MouseButtons.Left, 0, 0, 0, 0));
        Assert.Throws<ArgumentOutOfRangeException>("rowIndex", () => cell.OnMouseUp(e));
        Assert.Equal(ButtonState.Normal, cell.ButtonState);
    }

    public class SubDataGridViewHeaderCell : DataGridViewHeaderCell
    {
        public new ButtonState ButtonState => base.ButtonState;

        public new bool MouseLeaveUnsharesRow(int rowIndex) => base.MouseLeaveUnsharesRow(rowIndex);

        public new void OnMouseDown(DataGridViewCellMouseEventArgs e) => base.OnMouseDown(e);

        public new void OnMouseUp(DataGridViewCellMouseEventArgs e) => base.OnMouseUp(e);
    }
}
