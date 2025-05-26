// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;

namespace System.Windows.Forms.Tests;

public class DataGridViewLinkCellTests : IDisposable
{
    private readonly DataGridViewLinkCell _cell;

    public DataGridViewLinkCellTests()
    {
        _cell = new();
    }

    public void Dispose() => _cell.Dispose();

    private static DataGridView CreateGridWithColumn()
    {
        DataGridView dgv = new();
        dgv.Columns.Add(new DataGridViewTextBoxColumn());
        dgv.Rows.Add();
        return dgv;
    }

    [Fact]
    public void Ctor_Default_Properties()
    {
        _cell.Should().BeOfType<DataGridViewLinkCell>();
        _cell.Should().BeAssignableTo<DataGridViewCell>();
    }

    [Fact]
    public void ActiveLinkColor_Default_ReturnsExpected()
    {
        _cell.Selected = false;
        _cell.ActiveLinkColor.Should().Be(LinkUtilities.IEActiveLinkColor);
    }

    [Fact]
    public void ActiveLinkColor_SetAndGet_ReturnsSetValue()
    {
        Color color = Color.Red;
        _cell.ActiveLinkColor = color;
        _cell.ActiveLinkColor.Should().Be(color);
    }

    [Fact]
    public void ActiveLinkColor_Set_TriggersInvalidateCell_WhenInDataGridViewAndRowIndexSet()
    {
        DataGridView dgv = CreateGridWithColumn();
        dgv.Rows[0].Cells[0] = _cell;

        _cell.Selected = false;
        _cell.ActiveLinkColor = Color.Blue;
        _cell.ActiveLinkColor.Should().Be(Color.Blue);
    }

    [Fact]
    public void ActiveLinkColorInternal_SetsValueWithoutInvalidation()
    {
        _cell.ActiveLinkColorInternal = Color.Yellow;
        _cell.ActiveLinkColor.Should().Be(Color.Yellow);
    }

    [Fact]
    public void EditType_Always_ReturnsNull() =>
        _cell.EditType.Should().BeNull();

    [Fact]
    public void EditType_Override_ReturnsNull_ForDerivedType()
    {
        DerivedDataGridViewLinkCell cell = new();
        cell.EditType.Should().BeNull();
    }

    private class DerivedDataGridViewLinkCell : DataGridViewLinkCell
    {
    }

    [Fact]
    public void FormattedValueType_Always_ReturnsStringType() =>
        _cell.FormattedValueType.Should().Be(typeof(string));

    [Fact]
    public void FormattedValueType_Override_ReturnsStringType_ForDerivedType()
    {
        DerivedDataGridViewLinkCell cell = new();
        cell.FormattedValueType.Should().Be(typeof(string));
    }

    [Fact]
    public void LinkBehavior_Default_IsSystemDefault() =>
        _cell.LinkBehavior.Should().Be(LinkBehavior.SystemDefault);

    [Fact]
    public void LinkBehavior_SetAndGet_RoundTrips()
    {
        _cell.LinkBehavior = LinkBehavior.HoverUnderline;
        _cell.LinkBehavior.Should().Be(LinkBehavior.HoverUnderline);

        _cell.LinkBehavior = LinkBehavior.NeverUnderline;
        _cell.LinkBehavior.Should().Be(LinkBehavior.NeverUnderline);
    }

    [Fact]
    public void LinkBehavior_SetSameValue_DoesNotChange()
    {
        _cell.LinkBehavior = LinkBehavior.SystemDefault;
        _cell.LinkBehavior.Should().Be(LinkBehavior.SystemDefault);
        _cell.LinkBehavior = LinkBehavior.SystemDefault;
        _cell.LinkBehavior.Should().Be(LinkBehavior.SystemDefault);
    }

    [Fact]
    public void LinkBehavior_Set_TriggersInvalidateCell_WhenInDataGridViewAndRowIndexSet()
    {
        DataGridView dgv = CreateGridWithColumn();
        dgv.Rows[0].Cells[0] = _cell;

        _cell.LinkBehavior = LinkBehavior.AlwaysUnderline;
        _cell.LinkBehavior.Should().Be(LinkBehavior.AlwaysUnderline);
    }

    [Fact]
    public void LinkColor_Default_ReturnsExpected()
    {
        _cell.Selected = false;
        _cell.LinkColor.Should().Be(LinkUtilities.IELinkColor);
    }

    [Fact]
    public void LinkColor_Selected_ReturnsHighlightText()
    {
        DataGridView dgv = CreateGridWithColumn();
        dgv.Rows[0].Cells[0] = _cell;

        _cell.Selected = true;
        _cell.LinkColor.Should().Be(SystemColors.HighlightText);
    }

    [Fact]
    public void LinkColor_SetAndGet_RoundTrips()
    {
        Color color = Color.Purple;
        _cell.LinkColor = color;
        _cell.LinkColor.Should().Be(color);
    }

    [Fact]
    public void LinkColor_Set_TriggersInvalidateCell_WhenInDataGridViewAndRowIndexSet()
    {
        DataGridView dgv = CreateGridWithColumn();
        dgv.Rows[0].Cells[0] = _cell;

        _cell.LinkColor = Color.Orange;
        _cell.LinkColor.Should().Be(Color.Orange);
    }

    [Fact]
    public void LinkColorInternal_SetsValueWithoutInvalidation()
    {
        _cell.LinkColorInternal = Color.Brown;
        _cell.LinkColor.Should().Be(Color.Brown);
    }

    [Fact]
    public void LinkVisited_Default_IsFalse() =>
        _cell.LinkVisited.Should().BeFalse();

    [Fact]
    public void LinkVisited_SetTrueAndFalse_RoundTrips()
    {
        _cell.LinkVisited = true;
        _cell.LinkVisited.Should().BeTrue();

        _cell.LinkVisited = false;
        _cell.LinkVisited.Should().BeFalse();
    }

    [Fact]
    public void LinkVisited_SetSameValue_DoesNotChange()
    {
        _cell.LinkVisited = false;
        _cell.LinkVisited.Should().BeFalse();
        _cell.LinkVisited = false;
        _cell.LinkVisited.Should().BeFalse();
    }

    [Fact]
    public void LinkVisited_Set_TriggersInvalidateCell_WhenInDataGridViewAndRowIndexSet()
    {
        DataGridView dgv = CreateGridWithColumn();
        dgv.Rows[0].Cells[0] = _cell;

        _cell.LinkVisited = true;
        _cell.LinkVisited.Should().BeTrue();
    }

    [Fact]
    public void TrackVisitedState_Default_IsTrue() =>
        _cell.TrackVisitedState.Should().BeTrue();

    [Fact]
    public void TrackVisitedState_SetAndGet_RoundTrips()
    {
        _cell.TrackVisitedState = false;
        _cell.TrackVisitedState.Should().BeFalse();

        _cell.TrackVisitedState = true;
        _cell.TrackVisitedState.Should().BeTrue();
    }

    [Fact]
    public void TrackVisitedState_SetSameValue_DoesNotChange()
    {
        _cell.TrackVisitedState = true;
        _cell.TrackVisitedState.Should().BeTrue();
        _cell.TrackVisitedState = true;
        _cell.TrackVisitedState.Should().BeTrue();
    }

    [Fact]
    public void TrackVisitedState_Set_TriggersInvalidateCell_WhenInDataGridViewAndRowIndexSet()
    {
        DataGridView dgv = CreateGridWithColumn();
        dgv.Rows[0].Cells[0] = _cell;

        _cell.TrackVisitedState = false;
        _cell.TrackVisitedState.Should().BeFalse();
    }

    [Fact]
    public void UseColumnTextForLinkValue_Default_IsFalse() =>
        _cell.UseColumnTextForLinkValue.Should().BeFalse();

    [Fact]
    public void UseColumnTextForLinkValue_SetAndGet_RoundTrips()
    {
        _cell.UseColumnTextForLinkValue = true;
        _cell.UseColumnTextForLinkValue.Should().BeTrue();

        _cell.UseColumnTextForLinkValue = false;
        _cell.UseColumnTextForLinkValue.Should().BeFalse();
    }

    [Fact]
    public void UseColumnTextForLinkValue_SetSameValue_DoesNotChange()
    {
        _cell.UseColumnTextForLinkValue = false;
        _cell.UseColumnTextForLinkValue.Should().BeFalse();
        _cell.UseColumnTextForLinkValue = false;
        _cell.UseColumnTextForLinkValue.Should().BeFalse();
    }

    [Fact]
    public void ToString_ReturnsExpectedFormat()
    {
        _cell.ToString().Should().Be("DataGridViewLinkCell { ColumnIndex=-1, RowIndex=-1 }");

        DataGridView dgv = CreateGridWithColumn();
        dgv.Rows[0].Cells[0] = _cell;
        string result = _cell.ToString();
        result.Should().Contain("DataGridViewLinkCell { ColumnIndex=0, RowIndex=0 }");
    }

    [Fact]
    public void Clone_Default_CopiesAllRelevantProperties()
    {
        _cell.ActiveLinkColor = Color.Red;
        _cell.LinkBehavior = LinkBehavior.HoverUnderline;
        _cell.LinkColor = Color.Green;
        _cell.TrackVisitedState = false;
        _cell.VisitedLinkColor = Color.Blue;
        _cell.UseColumnTextForLinkValue = true;
        _cell.LinkVisited = true;

        DataGridViewLinkCell clone = (DataGridViewLinkCell)_cell.Clone();

        clone.Should().NotBeSameAs(_cell);
        clone.ActiveLinkColor.Should().Be(_cell.ActiveLinkColor);
        clone.LinkBehavior.Should().Be(_cell.LinkBehavior);
        clone.LinkColor.Should().Be(_cell.LinkColor);
        clone.TrackVisitedState.Should().Be(_cell.TrackVisitedState);
        clone.VisitedLinkColor.Should().Be(_cell.VisitedLinkColor);
        clone.UseColumnTextForLinkValue.Should().Be(_cell.UseColumnTextForLinkValue);
        clone.LinkVisited.Should().Be(_cell.LinkVisited);
    }

    [Fact]
    public void Clone_DerivedType_CreatesSameType()
    {
        DerivedDataGridViewLinkCell cell = new() { LinkColor = Color.Pink };
        var clone = cell.Clone();
        clone.Should().BeOfType<DerivedDataGridViewLinkCell>();
        ((DerivedDataGridViewLinkCell)clone).LinkColor.Should().Be(cell.LinkColor);
    }

    [Fact]
    public void Clone_OnlyCopiesSetProperties()
    {
        _cell.LinkColor = Color.Orange;
        DataGridViewLinkCell clone = (DataGridViewLinkCell)_cell.Clone();
        clone.LinkColor.Should().Be(Color.Orange);
        clone.LinkBehavior.Should().Be(LinkBehavior.SystemDefault);
        clone.TrackVisitedState.Should().BeTrue();
        clone.UseColumnTextForLinkValue.Should().BeFalse();
        clone.LinkVisited.Should().BeFalse();
    }
}
