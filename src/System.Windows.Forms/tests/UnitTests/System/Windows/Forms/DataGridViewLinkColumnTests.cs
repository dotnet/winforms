// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#nullable enable

using System.Drawing;

namespace System.Windows.Forms.Tests;

public class DataGridViewLinkColumnTests : IDisposable
{
    private readonly DataGridViewLinkColumn _column;
    private readonly DataGridView _dataGridView;

    public DataGridViewLinkColumnTests()
    {
        _column = new();
        _dataGridView = new();
    }

    public void Dispose()
    {
        _column.Dispose();
        _dataGridView.Dispose();
    }

    [WinFormsFact]
    public void Ctor_Default()
    {
        _column.Should().NotBeNull();
        _column.LinkBehavior.Should().Be(LinkBehavior.SystemDefault);
        _column.TrackVisitedState.Should().BeTrue();
        _column.UseColumnTextForLinkValue.Should().BeFalse();
        _column.Text.Should().BeNull();
        _column.ToString().Should().Be($"DataGridViewLinkColumn {{ Name=, Index=-1 }}");
        _column.ActiveLinkColor.Should().Be(LinkUtilities.IEActiveLinkColor);

        _column.CellTemplate = null;
        _column.CellTemplate.Should().BeNull();
    }

    [WinFormsFact]
    public void ActiveLinkColor_GetSet()
    {
        var color = Color.Red;
        _column.ActiveLinkColor = color;
        _column.ActiveLinkColor.Should().Be(color);
    }

    [WinFormsFact]
    public void ActiveLinkColor_SetWithDataGridView_GetReturnsExpected()
    {
        _dataGridView.Columns.Add(_column);
        _dataGridView.Rows.Add(2);

        var color = Color.Red;
        _column.ActiveLinkColor = color;
        _column.ActiveLinkColor.Should().Be(color);
        foreach (DataGridViewRow row in _dataGridView.Rows)
        {
            ((DataGridViewLinkCell)row.Cells[_column.Index]).ActiveLinkColor.Should().Be(color);
        }
    }

    [WinFormsFact]
    public void ActiveLinkColor_SetSameValue_DoesNotInvalidate()
    {
        _dataGridView.Columns.Add(_column);
        _dataGridView.Rows.Add(2);

        var color = _column.ActiveLinkColor;
        int invalidatedCallCount = 0;
        _dataGridView.Invalidated += (sender, e) => invalidatedCallCount++;

        _column.ActiveLinkColor = color;
        _column.ActiveLinkColor.Should().Be(color);
        invalidatedCallCount.Should().Be(0);
    }

    [WinFormsFact]
    public void CellTemplate_SetInvalidValue_ThrowsInvalidCastException()
    {
        using DataGridViewTextBoxCell cell = new();
        Action action = () => _column.CellTemplate = cell;
        action.Should().Throw<InvalidCastException>()
            .WithMessage(string.Format(SR.DataGridViewTypeColumn_WrongCellTemplateType, "System.Windows.Forms.DataGridViewLinkCell"));
    }

    [WinFormsFact]
    public void CellTemplate_SetNullValue_GetReturnsExpected()
    {
        _column.CellTemplate = null;
        _column.CellTemplate.Should().BeNull();
    }

    [WinFormsFact]
    public void LinkBehavior_GetSet()
    {
        var behavior = LinkBehavior.AlwaysUnderline;
        _column.LinkBehavior = behavior;
        _column.LinkBehavior.Should().Be(behavior);
    }

    [WinFormsFact]
    public void LinkBehavior_SetWithDataGridView_GetReturnsExpected()
    {
        _dataGridView.Columns.Add(_column);
        _dataGridView.Rows.Add(2);

        var behavior = LinkBehavior.NeverUnderline;
        _column.LinkBehavior = behavior;
        _column.LinkBehavior.Should().Be(behavior);
        foreach (DataGridViewRow row in _dataGridView.Rows)
        {
            ((DataGridViewLinkCell)row.Cells[_column.Index]).LinkBehavior.Should().Be(behavior);
        }
    }

    [WinFormsFact]
    public void LinkBehavior_SetSameValue_DoesNotInvalidate()
    {
        _dataGridView.Columns.Add(_column);
        _dataGridView.Rows.Add(2);

        var behavior = _column.LinkBehavior;
        int invalidatedCallCount = 0;
        _dataGridView.Invalidated += (sender, e) => invalidatedCallCount++;

        _column.LinkBehavior = behavior;
        _column.LinkBehavior.Should().Be(behavior);
        invalidatedCallCount.Should().Be(0);
    }

    [WinFormsFact]
    public void LinkColor_GetSet()
    {
        var color = Color.Blue;
        _column.LinkColor = color;
        _column.LinkColor.Should().Be(color);
    }

    [WinFormsFact]
    public void LinkColor_SetWithDataGridView_GetReturnsExpected()
    {
        _dataGridView.Columns.Add(_column);
        _dataGridView.Rows.Add(2);

        var color = Color.Blue;
        _column.LinkColor = color;
        _column.LinkColor.Should().Be(color);
        foreach (DataGridViewRow row in _dataGridView.Rows)
        {
            ((DataGridViewLinkCell)row.Cells[_column.Index]).LinkColor.Should().Be(color);
        }
    }

    [WinFormsFact]
    public void LinkColor_SetSameValue_DoesNotInvalidate()
    {
        _dataGridView.Columns.Add(_column);
        _dataGridView.Rows.Add(2);

        var color = _column.LinkColor;
        int invalidatedCallCount = 0;
        _dataGridView.Invalidated += (sender, e) => invalidatedCallCount++;

        _column.LinkColor = color;
        _column.LinkColor.Should().Be(color);
        invalidatedCallCount.Should().Be(0);
    }

    [WinFormsFact]
    public void Text_GetSet()
    {
        var text = "Test";
        _column.Text = text;
        _column.Text.Should().Be(text);
    }

    [WinFormsFact]
    public void Text_SetSameValue_DoesNotInvalidate()
    {
        _dataGridView.Columns.Add(_column);
        _dataGridView.Rows.Add(2);

        var text = _column.Text;
        int invalidatedCallCount = 0;
        _dataGridView.Invalidated += (sender, e) => invalidatedCallCount++;

        _column.Text = text;
        _column.Text.Should().Be(text);
        invalidatedCallCount.Should().Be(0);
    }

    [WinFormsFact]
    public void TrackVisitedState_GetSet()
    {
        var trackVisitedState = false;
        _column.TrackVisitedState = trackVisitedState;
        _column.TrackVisitedState.Should().Be(trackVisitedState);
    }

    [WinFormsFact]
    public void TrackVisitedState_SetWithDataGridView_GetReturnsExpected()
    {
        _dataGridView.Columns.Add(_column);
        _dataGridView.Rows.Add(2);

        var trackVisitedState = false;
        _column.TrackVisitedState = trackVisitedState;
        _column.TrackVisitedState.Should().Be(trackVisitedState);
        foreach (DataGridViewRow row in _dataGridView.Rows)
        {
            ((DataGridViewLinkCell)row.Cells[_column.Index]).TrackVisitedState.Should().Be(trackVisitedState);
        }
    }

    [WinFormsFact]
    public void TrackVisitedState_SetSameValue_DoesNotInvalidate()
    {
        _dataGridView.Columns.Add(_column);
        _dataGridView.Rows.Add(2);

        var trackVisitedState = _column.TrackVisitedState;
        int invalidatedCallCount = 0;
        _dataGridView.Invalidated += (sender, e) => invalidatedCallCount++;

        _column.TrackVisitedState = trackVisitedState;
        _column.TrackVisitedState.Should().Be(trackVisitedState);
        invalidatedCallCount.Should().Be(0);
    }

    [WinFormsFact]
    public void UseColumnTextForLinkValue_GetSet()
    {
        var useColumnTextForLinkValue = true;
        _column.UseColumnTextForLinkValue = useColumnTextForLinkValue;
        _column.UseColumnTextForLinkValue.Should().Be(useColumnTextForLinkValue);
    }

    [WinFormsFact]
    public void UseColumnTextForLinkValue_SetWithDataGridView_GetReturnsExpected()
    {
        _dataGridView.Columns.Add(_column);
        _dataGridView.Rows.Add(2);

        var useColumnTextForLinkValue = true;
        _column.UseColumnTextForLinkValue = useColumnTextForLinkValue;
        _column.UseColumnTextForLinkValue.Should().Be(useColumnTextForLinkValue);
        foreach (DataGridViewRow row in _dataGridView.Rows)
        {
            ((DataGridViewLinkCell)row.Cells[_column.Index]).UseColumnTextForLinkValue.Should().Be(useColumnTextForLinkValue);
        }
    }

    [WinFormsFact]
    public void UseColumnTextForLinkValue_SetSameValue_DoesNotInvalidate()
    {
        _dataGridView.Columns.Add(_column);
        _dataGridView.Rows.Add(2);

        var useColumnTextForLinkValue = _column.UseColumnTextForLinkValue;
        int invalidatedCallCount = 0;
        _dataGridView.Invalidated += (sender, e) => invalidatedCallCount++;

        _column.UseColumnTextForLinkValue = useColumnTextForLinkValue;
        _column.UseColumnTextForLinkValue.Should().Be(useColumnTextForLinkValue);
        invalidatedCallCount.Should().Be(0);
    }

    [WinFormsFact]
    public void VisitedLinkColor_GetSet()
    {
        var color = Color.Green;
        _column.VisitedLinkColor = color;
        _column.VisitedLinkColor.Should().Be(color);
    }

    [WinFormsFact]
    public void VisitedLinkColor_SetWithDataGridView_GetReturnsExpected()
    {
        _dataGridView.Columns.Add(_column);
        _dataGridView.Rows.Add(2);

        var color = Color.Green;
        _column.VisitedLinkColor = color;
        _column.VisitedLinkColor.Should().Be(color);
        foreach (DataGridViewRow row in _dataGridView.Rows)
        {
            ((DataGridViewLinkCell)row.Cells[_column.Index]).VisitedLinkColor.Should().Be(color);
        }
    }

    [WinFormsFact]
    public void VisitedLinkColor_SetSameValue_DoesNotInvalidate()
    {
        _dataGridView.Columns.Add(_column);
        _dataGridView.Rows.Add(2);

        var color = _column.VisitedLinkColor;
        int invalidatedCallCount = 0;
        _dataGridView.Invalidated += (sender, e) => invalidatedCallCount++;

        _column.VisitedLinkColor = color;
        _column.VisitedLinkColor.Should().Be(color);
        invalidatedCallCount.Should().Be(0);
    }

    [WinFormsFact]
    public void Clone_CreatesCopy()
    {
        _column.ActiveLinkColor = Color.Red;
        _column.LinkBehavior = LinkBehavior.AlwaysUnderline;
        _column.LinkColor = Color.Blue;
        _column.Text = "Test";
        _column.TrackVisitedState = false;
        _column.UseColumnTextForLinkValue = true;
        _column.VisitedLinkColor = Color.Green;

        using var clone = (DataGridViewLinkColumn)_column.Clone();
        clone.ActiveLinkColor.Should().Be(_column.ActiveLinkColor);
        clone.LinkBehavior.Should().Be(_column.LinkBehavior);
        clone.LinkColor.Should().Be(_column.LinkColor);
        clone.Text.Should().Be(_column.Text);
        clone.TrackVisitedState.Should().Be(_column.TrackVisitedState);
        clone.UseColumnTextForLinkValue.Should().Be(_column.UseColumnTextForLinkValue);
        clone.VisitedLinkColor.Should().Be(_column.VisitedLinkColor);
    }

    [WinFormsFact]
    public void Clone_CreatesCopyWithDifferentInstance()
    {
        _column.ActiveLinkColor = Color.Red;
        _column.LinkBehavior = LinkBehavior.AlwaysUnderline;
        _column.LinkColor = Color.Blue;
        _column.Text = "Test";
        _column.TrackVisitedState = false;
        _column.UseColumnTextForLinkValue = true;
        _column.VisitedLinkColor = Color.Green;

        using var clone = (DataGridViewLinkColumn)_column.Clone();
        clone.Should().NotBeSameAs(_column);
        clone.ActiveLinkColor.Should().Be(_column.ActiveLinkColor);
        clone.LinkBehavior.Should().Be(_column.LinkBehavior);
        clone.LinkColor.Should().Be(_column.LinkColor);
        clone.Text.Should().Be(_column.Text);
        clone.TrackVisitedState.Should().Be(_column.TrackVisitedState);
        clone.UseColumnTextForLinkValue.Should().Be(_column.UseColumnTextForLinkValue);
        clone.VisitedLinkColor.Should().Be(_column.VisitedLinkColor);
    }

    [WinFormsTheory]
    [InlineData("", -1, "DataGridViewLinkColumn { Name=, Index=-1 }")]
    [InlineData("TestColumn", -1, "DataGridViewLinkColumn { Name=TestColumn, Index=-1 }")]
    [InlineData("", 0, "DataGridViewLinkColumn { Name=, Index=0 }")]
    [InlineData("TestColumn", 0, "DataGridViewLinkColumn { Name=TestColumn, Index=0 }")]
    public void ToString_VariousScenarios(string name, int index, string expected)
    {
        _column.Name = name;

        if (index >= 0)
        {
            using DataGridView dataGridView = new();
            dataGridView.Columns.Add(_column);
        }

        _column.ToString().Should().Be(expected);
    }
}
