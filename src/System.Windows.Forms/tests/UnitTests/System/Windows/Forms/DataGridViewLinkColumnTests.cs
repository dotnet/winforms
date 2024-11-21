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
        TestPropertySetAndGet(
            color => _column.ActiveLinkColor = color,
            () => _column.ActiveLinkColor,
            Color.Red);
    }

    [WinFormsFact]
    public void ActiveLinkColor_SetWithDataGridView_GetReturnsExpected()
    {
        TestPropertySetWithDataGridView(
            color => _column.ActiveLinkColor = color,
            () => _column.ActiveLinkColor,
            () => ((DataGridViewLinkCell)_dataGridView.Rows[0].Cells[_column.Index]).ActiveLinkColor,
            Color.Red);
    }

    [WinFormsFact]
    public void ActiveLinkColor_SetSameValue_DoesNotInvalidate()
    {
        TestPropertySetSameValueDoesNotInvalidate(
            () => _column.ActiveLinkColor,
            color => _column.ActiveLinkColor = color);
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
        TestPropertySetAndGet(
            behavior => _column.LinkBehavior = behavior,
            () => _column.LinkBehavior,
            LinkBehavior.AlwaysUnderline);
    }

    [WinFormsFact]
    public void LinkBehavior_SetWithDataGridView_GetReturnsExpected()
    {
        TestPropertySetWithDataGridView(
            behavior => _column.LinkBehavior = behavior,
            () => _column.LinkBehavior,
            () => ((DataGridViewLinkCell)_dataGridView.Rows[0].Cells[_column.Index]).LinkBehavior,
            LinkBehavior.NeverUnderline);
    }

    [WinFormsFact]
    public void LinkBehavior_SetSameValue_DoesNotInvalidate()
    {
        TestPropertySetSameValueDoesNotInvalidate(
            () => _column.LinkBehavior,
            behavior => _column.LinkBehavior = behavior);
    }

    [WinFormsFact]
    public void LinkColor_GetSet()
    {
        TestPropertySetAndGet(
            color => _column.LinkColor = color,
            () => _column.LinkColor,
            Color.Blue);
    }

    [WinFormsFact]
    public void LinkColor_SetWithDataGridView_GetReturnsExpected()
    {
        TestPropertySetWithDataGridView(
            color => _column.LinkColor = color,
            () => _column.LinkColor,
            () => ((DataGridViewLinkCell)_dataGridView.Rows[0].Cells[_column.Index]).LinkColor,
            Color.Blue);
    }

    [WinFormsFact]
    public void LinkColor_SetSameValue_DoesNotInvalidate()
    {
        TestPropertySetSameValueDoesNotInvalidate(
            () => _column.LinkColor,
            color => _column.LinkColor = color);
    }

    [WinFormsFact]
    public void Text_GetSet()
    {
        TestPropertySetAndGet(
            text => _column.Text = text,
            () => _column.Text,
            "Test");
    }

    [WinFormsFact]
    public void Text_SetSameValue_DoesNotInvalidate()
    {
        TestPropertySetSameValueDoesNotInvalidate(
            () => _column.Text,
            text => _column.Text = text);
    }

    [WinFormsFact]
    public void TrackVisitedState_GetSet()
    {
        TestPropertySetAndGet(
            trackVisitedState => _column.TrackVisitedState = trackVisitedState,
            () => _column.TrackVisitedState,
            false);
    }

    [WinFormsFact]
    public void TrackVisitedState_SetWithDataGridView_GetReturnsExpected()
    {
        TestPropertySetWithDataGridView(
            trackVisitedState => _column.TrackVisitedState = trackVisitedState,
            () => _column.TrackVisitedState,
            () => ((DataGridViewLinkCell)_dataGridView.Rows[0].Cells[_column.Index]).TrackVisitedState,
            false);
    }

    [WinFormsFact]
    public void TrackVisitedState_SetSameValue_DoesNotInvalidate()
    {
        TestPropertySetSameValueDoesNotInvalidate(
            () => _column.TrackVisitedState,
            trackVisitedState => _column.TrackVisitedState = trackVisitedState);
    }

    [WinFormsFact]
    public void UseColumnTextForLinkValue_GetSet()
    {
        TestPropertySetAndGet(
            useColumnTextForLinkValue => _column.UseColumnTextForLinkValue = useColumnTextForLinkValue,
            () => _column.UseColumnTextForLinkValue,
            true);
    }

    [WinFormsFact]
    public void UseColumnTextForLinkValue_SetWithDataGridView_GetReturnsExpected()
    {
        TestPropertySetWithDataGridView(
            useColumnTextForLinkValue => _column.UseColumnTextForLinkValue = useColumnTextForLinkValue,
            () => _column.UseColumnTextForLinkValue,
            () => ((DataGridViewLinkCell)_dataGridView.Rows[0].Cells[_column.Index]).UseColumnTextForLinkValue,
            true);
    }

    [WinFormsFact]
    public void UseColumnTextForLinkValue_SetSameValue_DoesNotInvalidate()
    {
        TestPropertySetSameValueDoesNotInvalidate(
            () => _column.UseColumnTextForLinkValue,
            useColumnTextForLinkValue => _column.UseColumnTextForLinkValue = useColumnTextForLinkValue);
    }

    [WinFormsFact]
    public void VisitedLinkColor_GetSet()
    {
        TestPropertySetAndGet(
            color => _column.VisitedLinkColor = color,
            () => _column.VisitedLinkColor,
            Color.Green);
    }

    [WinFormsFact]
    public void VisitedLinkColor_SetWithDataGridView_GetReturnsExpected()
    {
        TestPropertySetWithDataGridView(
            color => _column.VisitedLinkColor = color,
            () => _column.VisitedLinkColor,
            () => ((DataGridViewLinkCell)_dataGridView.Rows[0].Cells[_column.Index]).VisitedLinkColor,
            Color.Green);
    }

    [WinFormsFact]
    public void VisitedLinkColor_SetSameValue_DoesNotInvalidate()
    {
        TestPropertySetSameValueDoesNotInvalidate(
            () => _column.VisitedLinkColor,
            color => _column.VisitedLinkColor = color);
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

        var clone = (DataGridViewLinkColumn)_column.Clone();
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

        var clone = (DataGridViewLinkColumn)_column.Clone();
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

    private void TestPropertySetAndGet<T>(Action<T> setProperty, Func<T> getProperty, T value)
    {
        setProperty(value);
        getProperty().Should().Be(value);
    }

    private void TestPropertySetWithDataGridView<T>(Action<T> setProperty, Func<T> getProperty, Func<T> getCellProperty, T value)
    {
        _dataGridView.Columns.Add(_column);
        _dataGridView.Rows.Add(2);

        setProperty(value);
        getProperty().Should().Be(value);
        foreach (DataGridViewRow row in _dataGridView.Rows)
        {
            getCellProperty().Should().Be(value);
        }
    }

    private void TestPropertySetSameValueDoesNotInvalidate<T>(Func<T> getProperty, Action<T> setProperty)
    {
        _dataGridView.Columns.Add(_column);
        _dataGridView.Rows.Add(2);

        var value = getProperty();
        int invalidatedCallCount = 0;
        _dataGridView.Invalidated += (sender, e) => invalidatedCallCount++;

        setProperty(value);
        getProperty().Should().Be(value);
        invalidatedCallCount.Should().Be(0);
    }
}
