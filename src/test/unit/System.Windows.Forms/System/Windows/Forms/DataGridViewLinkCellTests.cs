// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;
using System.Reflection;

namespace System.Windows.Forms.Tests;

public class DataGridViewLinkCellTests : IDisposable
{
    private readonly DataGridViewLinkCell _cell;

    public DataGridViewLinkCellTests() => _cell = new();

    public void Dispose() => _cell.Dispose();

    private static DataGridView CreateGridWithColumn()
    {
        DataGridView dataGridView = new();
        dataGridView.Columns.Add(new DataGridViewTextBoxColumn());
        dataGridView.Rows.Add();
        return dataGridView;
    }

    [Fact]
    public void Ctor_Default_Properties()
    {
        _cell.Should().BeOfType<DataGridViewLinkCell>();
        _cell.Should().BeAssignableTo<DataGridViewCell>();
    }

    [WinFormsFact]
    public void ActiveLinkColor_Default_ReturnsExpected()
    {
        using DataGridView dataGridView = CreateGridWithColumn();
        dataGridView.Rows[0].Cells[0] = _cell;

        _cell.Selected = false;
        _cell.ActiveLinkColor.Should().Be(LinkUtilities.IEActiveLinkColor);

        _cell.Selected = true;
        _cell.ActiveLinkColor.Should().Be(SystemColors.HighlightText);
    }

    [WinFormsFact]
    public void ActiveLinkColor_SetAndGet_ReturnsSetValue()
    {
        Color color = Color.Red;
        _cell.ActiveLinkColor = color;

        _cell.ActiveLinkColor.Should().Be(color);
    }

    [WinFormsFact]
    public void ActiveLinkColor_Set_TriggersInvalidateCell_WhenInDataGridViewAndRowIndexSet()
    {
        using DataGridView dataGridView = CreateGridWithColumn();
        dataGridView.Rows[0].Cells[0] = _cell;

        _cell.Selected = false;

        _cell.ActiveLinkColor = Color.Blue;
        _cell.ActiveLinkColor.Should().Be(Color.Blue);

        _cell.Selected = true;
        _cell.ActiveLinkColor = Color.Blue;

        _cell.ActiveLinkColor.Should().Be(Color.Blue);
    }

    [WinFormsFact]
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
        using DataGridViewLinkCell cell = new();

        cell.EditType.Should().BeNull();
    }

    [Fact]
    public void FormattedValueType_Always_ReturnsStringType() =>
        _cell.FormattedValueType.Should().Be(typeof(string));

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
    }

    [WinFormsFact]
    public void LinkColor_Default_ReturnsExpected()
    {
        using DataGridView dataGridView = CreateGridWithColumn();
        dataGridView.Rows[0].Cells[0] = _cell;

        _cell.Selected = false;
        _cell.LinkColor.Should().Be(LinkUtilities.IELinkColor);

        _cell.Selected = true;
        _cell.LinkColor.Should().Be(SystemColors.HighlightText);
    }

    [WinFormsFact]
    public void LinkColor_SetAndGet_RoundTrips()
    {
        Color color = Color.Purple;
        _cell.LinkColor = color;

        _cell.LinkColor.Should().Be(color);
    }

    [WinFormsFact]
    public void LinkColorInternal_SetsValueWithoutInvalidation()
    {
        _cell.LinkColorInternal = Color.Brown;

        _cell.LinkColor.Should().Be(Color.Brown);
    }

    [WinFormsFact]
    public void LinkVisited_Default_IsFalse() =>
        _cell.LinkVisited.Should().BeFalse();

    [WinFormsFact]
    public void LinkVisited_SetTrueAndFalse_RoundTrips()
    {
        _cell.LinkVisited = true;
        _cell.LinkVisited.Should().BeTrue();

        _cell.LinkVisited = false;
        _cell.LinkVisited.Should().BeFalse();
    }

    [WinFormsFact]
    public void LinkVisited_SetSameValue_DoesNotChange()
    {
        _cell.LinkVisited = false;
        _cell.LinkVisited.Should().BeFalse();
    }

    [WinFormsFact]
    public void TrackVisitedState_Default_IsTrue() =>
        _cell.TrackVisitedState.Should().BeTrue();

    [WinFormsFact]
    public void TrackVisitedState_SetAndGet_RoundTrips()
    {
        _cell.TrackVisitedState = false;
        _cell.TrackVisitedState.Should().BeFalse();

        _cell.TrackVisitedState = true;
        _cell.TrackVisitedState.Should().BeTrue();
    }

    [WinFormsFact]
    public void TrackVisitedState_SetSameValue_DoesNotChange()
    {
        _cell.TrackVisitedState = true;
        _cell.TrackVisitedState.Should().BeTrue();
    }

    [WinFormsFact]
    public void UseColumnTextForLinkValue_Default_IsFalse() =>
        _cell.UseColumnTextForLinkValue.Should().BeFalse();

    [WinFormsFact]
    public void UseColumnTextForLinkValue_SetAndGet_RoundTrips()
    {
        _cell.UseColumnTextForLinkValue = true;
        _cell.UseColumnTextForLinkValue.Should().BeTrue();

        _cell.UseColumnTextForLinkValue = false;
        _cell.UseColumnTextForLinkValue.Should().BeFalse();
    }

    [WinFormsFact]
    public void UseColumnTextForLinkValue_SetSameValue_DoesNotChange()
    {
        _cell.UseColumnTextForLinkValue = false;
        _cell.UseColumnTextForLinkValue.Should().BeFalse();
    }

    [WinFormsFact]
    public void ToString_ReturnsExpectedFormat()
    {
        _cell.ToString().Should().Be("DataGridViewLinkCell { ColumnIndex=-1, RowIndex=-1 }");

        using DataGridView dataGridView = CreateGridWithColumn();
        dataGridView.Rows[0].Cells[0] = _cell;
        string result = dataGridView.Rows[0].Cells[0].ToString();

        result.Should().Contain("DataGridViewLinkCell { ColumnIndex=0, RowIndex=0 }");
    }

    [WinFormsFact]
    public void Clone_Default_CopiesAllRelevantProperties()
    {
        _cell.ActiveLinkColor = Color.Red;
        _cell.LinkBehavior = LinkBehavior.HoverUnderline;
        _cell.LinkColor = Color.Green;
        _cell.TrackVisitedState = false;
        _cell.VisitedLinkColor = Color.Blue;
        _cell.UseColumnTextForLinkValue = true;
        _cell.LinkVisited = true;

        DataGridViewLinkCell clonedLinkCell = (DataGridViewLinkCell)_cell.Clone();

        clonedLinkCell.Should().NotBeSameAs(_cell);
        clonedLinkCell.ActiveLinkColor.Should().Be(_cell.ActiveLinkColor);
        clonedLinkCell.LinkBehavior.Should().Be(_cell.LinkBehavior);
        clonedLinkCell.LinkColor.Should().Be(_cell.LinkColor);
        clonedLinkCell.TrackVisitedState.Should().Be(_cell.TrackVisitedState);
        clonedLinkCell.VisitedLinkColor.Should().Be(_cell.VisitedLinkColor);
        clonedLinkCell.UseColumnTextForLinkValue.Should().Be(_cell.UseColumnTextForLinkValue);
        clonedLinkCell.LinkVisited.Should().Be(_cell.LinkVisited);
    }

    [WinFormsFact]
    public void Clone_BaseType_CreatesSameType()
    {
        _cell.LinkColor = Color.Pink;
        DataGridViewLinkCell clonedLinkCell = (DataGridViewLinkCell)_cell.Clone();

        clonedLinkCell.Should().BeOfType<DataGridViewLinkCell>();
        clonedLinkCell.LinkColor.Should().Be(_cell.LinkColor);
    }

    [WinFormsFact]
    public void Clone_CopiesSetProperties_LeavesOthersDefault()
    {
        _cell.LinkColor = Color.Orange;
        DataGridViewLinkCell clonedLinkCell = (DataGridViewLinkCell)_cell.Clone();

        clonedLinkCell.LinkColor.Should().Be(Color.Orange);
        clonedLinkCell.LinkBehavior.Should().Be(LinkBehavior.SystemDefault);
        clonedLinkCell.TrackVisitedState.Should().BeTrue();
        clonedLinkCell.UseColumnTextForLinkValue.Should().BeFalse();
        clonedLinkCell.LinkVisited.Should().BeFalse();
    }

    [WinFormsFact]
    public void GetContentBounds_ReturnsEmpty_WhenNotInDataGridView() =>
        _cell.GetContentBounds(0).Should().Be(Rectangle.Empty);

    [WinFormsFact]
    public void GetContentBounds_ThrowsArgumentOutOfRangeException_WhenRowIndexNegative()
    {
        using DataGridView dataGridView = CreateGridWithColumn();
        dataGridView.Rows[0].Cells[0] = _cell;

        Action action = () => dataGridView.Rows[0].Cells[0].GetContentBounds(-1);

        action.Should().Throw<ArgumentOutOfRangeException>();
    }

    [WinFormsFact]
    public void GetContentBounds_ReturnsEmpty_WhenOwningColumnIsNull() =>
        _cell.GetContentBounds(0).Should().Be(Rectangle.Empty);

    [WinFormsFact]
    public void GetErrorIconBounds_ThrowsInvalidOperationException_WhenNotInDataGridView() =>
        ((Action)(() => _cell.GetErrorIconBounds(0))).Should().Throw<InvalidOperationException>();

    [WinFormsFact]
    public void GetErrorIconBounds_ThrowsArgumentOutOfRangeException_WhenRowIndexNegative()
    {
        using DataGridView dataGridView = CreateGridWithColumn();
        dataGridView.Rows[0].Cells[0] = _cell;

        Action action = () => _cell.GetErrorIconBounds(-1);

        action.Should().Throw<ArgumentOutOfRangeException>();
        ReferenceEquals(_cell, dataGridView.Rows[0].Cells[0]).Should().BeTrue();
    }

    [WinFormsFact]
    public void GetErrorIconBounds_ThrowsInvalidOperationException_WhenOwningColumnIsNull() =>
        ((Action)(() => _cell.GetErrorIconBounds(0))).Should().Throw<InvalidOperationException>();

    [WinFormsFact]
    public void GetErrorIconBounds_ReturnsEmpty_WhenShowCellErrorsIsFalse()
    {
        using DataGridView dataGridView = CreateGridWithColumn();
        dataGridView.Rows[0].Cells[0] = _cell;
        dataGridView.ShowCellErrors = false;
        Rectangle result = _cell.GetErrorIconBounds(0);

        result.Should().Be(Rectangle.Empty);
    }

    [WinFormsFact]
    public void GetErrorIconBounds_ReturnsEmpty_WhenErrorTextIsNullOrEmpty()
    {
        using DataGridView dataGridView = CreateGridWithColumn();
        dataGridView.Rows[0].Cells[0] = _cell;

        string? errorText = _cell.TestAccessor().Dynamic.GetErrorText(0);
        errorText.Should().BeNullOrEmpty();
        Rectangle result = _cell.GetErrorIconBounds(0);

        result.Should().Be(Rectangle.Empty);
    }

    [WinFormsFact]
    public void GetValue_ReturnsBaseValue_WhenUseColumnTextForLinkValueIsFalse()
    {
        using DataGridView dataGridView = CreateGridWithColumn();
        dataGridView.Columns.Clear();
        dataGridView.Columns.Add(new DataGridViewLinkColumn());
        dataGridView.Rows.Add();
        dataGridView.Rows[0].Cells[0] = _cell;
        object testValue = "TestValue";
        _cell.Value = testValue;

        object? result = _cell.TestAccessor().Dynamic.GetValue(0);

        result.Should().Be(testValue);
    }

    [WinFormsFact]
    public void GetValue_ReturnsColumnText_WhenUseColumnTextForLinkValueIsTrue_AndOwningColumnIsLinkColumn()
    {
        using DataGridView dataGridView = CreateGridWithColumn();
        dataGridView.Columns.Clear();
        dataGridView.Columns.AddRange([new DataGridViewLinkColumn { Text = "ColumnText" }]);
        dataGridView.Rows.Add();
        _cell.UseColumnTextForLinkValue = true;
        dataGridView.Rows[0].Cells[0] = _cell;

        object? result = _cell.TestAccessor().Dynamic.GetValue(0);

        result.Should().Be("ColumnText");
    }

    [WinFormsFact]
    public void GetValue_ReturnsBaseValue_WhenUseColumnTextForLinkValueIsTrue_ButOwningColumnIsNotLinkColumn()
    {
        using DataGridView dataGridView = CreateGridWithColumn();
        _cell.UseColumnTextForLinkValue = true;
        dataGridView.Rows[0].Cells[0] = _cell;
        object testValue = "TestValue";
        _cell.Value = testValue;

        object? result = _cell.TestAccessor().Dynamic.GetValue(0);

        result.Should().Be(testValue);
    }

    [WinFormsFact]
    public void GetValue_ReturnsBaseValue_WhenUseColumnTextForLinkValueIsTrue_AndRowIsNewRow()
    {
        using DataGridView dataGridView = CreateGridWithColumn();
        dataGridView.Columns.Clear();
        dataGridView.Columns.AddRange([new DataGridViewLinkColumn { Text = "ColumnText" }]);
        dataGridView.Rows.Add();
        _cell.UseColumnTextForLinkValue = true;
        dataGridView.Rows[0].Cells[0] = _cell;
        int newRowIndex = dataGridView.NewRowIndex;
        object testValue = "TestValue";
        _cell.Value = testValue;

        object? result = _cell.TestAccessor().Dynamic.GetValue(newRowIndex);

        result.Should().Be(testValue);
    }

    [WinFormsTheory]
    [InlineData(Keys.Space, false, false, false, true, false, true)]
    [InlineData(Keys.Space, false, false, false, false, false, false)]
    [InlineData(Keys.Space, true, false, false, true, false, true)]
    [InlineData(Keys.Space, false, true, false, true, false, true)]
    [InlineData(Keys.Space, false, false, true, true, false, true)]
    [InlineData(Keys.Enter, false, false, false, true, false, true)]
    public void KeyUpUnsharesRow_VariousScenarios_ReturnsExpected(Keys key, bool alt, bool control, bool shift, bool trackVisitedState, bool linkVisited, bool expected)
    {
        KeyEventArgs keyEvent = new(key | (alt ? Keys.Alt : 0) | (control ? Keys.Control : 0) | (shift ? Keys.Shift : 0));
        _cell.TrackVisitedState = trackVisitedState;
        _cell.LinkVisited = linkVisited;

        bool result = _cell.TestAccessor().Dynamic.KeyUpUnsharesRow(keyEvent, 0);

        result.Should().Be(expected);
    }

    [WinFormsFact]
    public void MouseDownUnsharesRow_ReturnsFalse_WhenPointInLinkBounds()
    {
        using DataGridView dataGridView = CreateGridWithColumn();
        dataGridView.Rows[0].Cells[0] = _cell;
        MouseEventArgs mouseEventArgs = new(MouseButtons.None, 0, 1, 1, 0);
        DataGridViewCellMouseEventArgs args = new(0, 0, 1, 1, mouseEventArgs);

        bool result = _cell.TestAccessor().Dynamic.MouseDownUnsharesRow(args);

        result.Should().BeFalse();
    }

    [WinFormsFact]
    public void MouseDownUnsharesRow_ReturnsFalse_WhenPointNotInLinkBounds()
    {
        using DataGridView dataGridView = CreateGridWithColumn();
        dataGridView.Rows[0].Cells[0] = _cell;
        MouseEventArgs mouseEventArgs = new(MouseButtons.None, 0, 100, 100, 0);
        DataGridViewCellMouseEventArgs args = new(0, 0, 100, 100, mouseEventArgs);

        bool result = _cell.TestAccessor().Dynamic.MouseDownUnsharesRow(args);

        result.Should().BeFalse();
    }

    [WinFormsFact]
    public void MouseMoveUnsharesRow_ReturnsTrue_WhenPointInLinkBounds_AndHoverSet()
    {
        _cell.TestAccessor().Dynamic.LinkState = LinkState.Hover;

        MouseEventArgs mouseEventArgs = new(MouseButtons.None, 0, 1, 1, 0);
        DataGridViewCellMouseEventArgs args = new(0, 0, 1, 1, mouseEventArgs);

        bool result = _cell.TestAccessor().Dynamic.MouseMoveUnsharesRow(args);

        result.Should().BeTrue();
    }

    [WinFormsFact]
    public void MouseMoveUnsharesRow_CanSetLinkBehaviorInternal()
    {
        _cell.TestAccessor().Dynamic.LinkBehaviorInternal = LinkBehavior.AlwaysUnderline;

        _cell.LinkBehavior.Should().Be(LinkBehavior.AlwaysUnderline);
    }

    [WinFormsFact]
    public void MouseMoveUnsharesRow_ReturnsTrue_WhenPointNotInLinkBounds_AndHoverSet()
    {
        _cell.TestAccessor().Dynamic.LinkState = LinkState.Hover;

        MouseEventArgs mouseEventArgs = new(MouseButtons.None, 0, 100, 100, 0);
        DataGridViewCellMouseEventArgs args = new(0, 0, 100, 100, mouseEventArgs);

        bool result = _cell.TestAccessor().Dynamic.MouseMoveUnsharesRow(args);

        result.Should().BeTrue();
    }

    [WinFormsFact]
    public void MouseMoveUnsharesRow_ReturnsFalse_WhenPointNotInLinkBounds_AndNotHover()
    {
        MouseEventArgs mouseEventArgs = new(MouseButtons.None, 0, 100, 100, 0);
        DataGridViewCellMouseEventArgs args = new(0, 0, 100, 100, mouseEventArgs);

        bool result = _cell.TestAccessor().Dynamic.MouseMoveUnsharesRow(args);

        result.Should().BeFalse();
    }

    [WinFormsFact]
    public void MouseUpUnsharesRow_ReturnsFalse_WhenTrackVisitedStateFalse_AndPointInLinkBounds()
    {
        _cell.TrackVisitedState = false;

        MouseEventArgs mouseEventArgs = new(MouseButtons.None, 0, 1, 1, 0);
        DataGridViewCellMouseEventArgs args = new(0, 0, 1, 1, mouseEventArgs);

        bool result = _cell.TestAccessor().Dynamic.MouseUpUnsharesRow(args);

        result.Should().BeFalse();
    }

    [WinFormsFact]
    public void MouseUpUnsharesRow_ReturnsFalse_WhenTrackVisitedStateFalse_AndPointNotInLinkBounds()
    {
        _cell.TrackVisitedState = false;

        MouseEventArgs mouseEventArgs = new(MouseButtons.None, 0, 100, 100, 0);
        DataGridViewCellMouseEventArgs args = new(0, 0, 100, 100, mouseEventArgs);

        bool result = _cell.TestAccessor().Dynamic.MouseUpUnsharesRow(args);

        result.Should().BeFalse();
    }

    [WinFormsFact]
    public void OnKeyUp_DoesNothing_WhenDataGridViewIsNull()
    {
        KeyEventArgs keyEvent = new(Keys.Space);

        _cell.Invoking(c => c.TestAccessor().Dynamic.OnKeyUp(keyEvent, 0)).Should().NotThrow();
        keyEvent.Handled.Should().BeFalse();
    }

    [WinFormsFact]
    public void OnKeyUp_RaisesCellClickAndContentClick_AndSetsHandled_AndLinkVisited_WhenSpacePressed()
    {
        using DataGridView dataGridView = CreateGridWithColumn();
        dataGridView.Rows[0].Cells[0] = _cell;
        _cell.TrackVisitedState = true;
        bool cellClickRaised = false;
        bool contentClickRaised = false;
        dataGridView.CellClick += (s, e) =>
        {
            if (e.ColumnIndex == 0 && e.RowIndex == 0)
                cellClickRaised = true;
        };
        dataGridView.CellContentClick += (s, e) =>
        {
            if (e.ColumnIndex == 0 && e.RowIndex == 0)
                contentClickRaised = true;
        };
        KeyEventArgs keyEvent = new(Keys.Space);

        _cell.TestAccessor().Dynamic.OnKeyUp(keyEvent, 0);

        cellClickRaised.Should().BeTrue();
        contentClickRaised.Should().BeTrue();
        keyEvent.Handled.Should().BeTrue();
        _cell.LinkVisited.Should().BeTrue();
    }

    [WinFormsFact]
    public void OnKeyUp_RaisesCellClickAndContentClick_ButNotLinkVisited_WhenTrackVisitedStateIsFalse()
    {
        using DataGridView dataGridView = CreateGridWithColumn();
        dataGridView.Rows[0].Cells[0] = _cell;
        _cell.TrackVisitedState = false;
        bool cellClickRaised = false;
        bool contentClickRaised = false;
        dataGridView.CellClick += (s, e) =>
        {
            if (e.ColumnIndex == 0 && e.RowIndex == 0)
                cellClickRaised = true;
        };
        dataGridView.CellContentClick += (s, e) =>
        {
            if (e.ColumnIndex == 0 && e.RowIndex == 0)
                contentClickRaised = true;
        };
        KeyEventArgs keyEvent = new(Keys.Space);

        _cell.TestAccessor().Dynamic.OnKeyUp(keyEvent, 0);

        cellClickRaised.Should().BeTrue();
        contentClickRaised.Should().BeTrue();
        keyEvent.Handled.Should().BeTrue();
        _cell.LinkVisited.Should().BeFalse();
    }

    [WinFormsFact]
    public void OnKeyUp_DoesNotRaiseEvents_WhenKeyIsNotSpace()
    {
        using DataGridView dataGridView = CreateGridWithColumn();
        dataGridView.Rows[0].Cells[0] = _cell;
        bool cellClickRaised = false;
        bool contentClickRaised = false;
        dataGridView.CellClick += (s, e) => cellClickRaised = true;
        dataGridView.CellContentClick += (s, e) => contentClickRaised = true;
        KeyEventArgs keyEvent = new(Keys.Enter);

        _cell.TestAccessor().Dynamic.OnKeyUp(keyEvent, 0);

        cellClickRaised.Should().BeFalse();
        contentClickRaised.Should().BeFalse();
        keyEvent.Handled.Should().BeFalse();
    }

    [WinFormsFact]
    public void OnMouseDown_DoesNothing_WhenDataGridViewIsNull()
    {
        MouseEventArgs mouseEventArgs = new(MouseButtons.None, 0, 1, 1, 0);
        DataGridViewCellMouseEventArgs args = new(0, 0, 1, 1, mouseEventArgs);

        _cell.Invoking(c => c.TestAccessor().Dynamic.OnMouseDown(args)).Should().NotThrow();
    }

    [WinFormsFact]
    public void OnMouseLeave_DoesNothing_WhenDataGridViewIsNull() =>
        _cell.Invoking(c => c.TestAccessor().Dynamic.OnMouseLeave(0)).Should().NotThrow();

    [WinFormsFact]
    public void OnMouseMove_DoesNothing_WhenDataGridViewIsNull()
    {
        MouseEventArgs mouseEventArgs = new(MouseButtons.None, 0, 1, 1, 0);
        DataGridViewCellMouseEventArgs args = new(0, 0, 1, 1, mouseEventArgs);

        _cell.Invoking(c => c.TestAccessor().Dynamic.OnMouseMove(args)).Should().NotThrow();
    }

    [WinFormsFact]
    public void OnMouseUp_DoesNothing_WhenDataGridViewIsNull()
    {
        MouseEventArgs mouseEventArgs = new(MouseButtons.None, 0, 1, 1, 0);
        DataGridViewCellMouseEventArgs args = new(0, 0, 1, 1, mouseEventArgs);

        _cell.Invoking(c => c.TestAccessor().Dynamic.OnMouseUp(args)).Should().NotThrow();
    }

    [WinFormsFact]
    public void OnMouseUp_DoesNotSetLinkVisited_WhenPointNotInLinkBounds()
    {
        using DataGridView dataGridView = CreateGridWithColumn();
        dataGridView.Rows[0].Cells[0] = _cell;
        _cell.TrackVisitedState = true;
        _cell.LinkVisited = false;
        MouseEventArgs mouseEventArgs = new(MouseButtons.None, 0, 100, 100, 0);
        DataGridViewCellMouseEventArgs args = new(0, 0, 100, 100, mouseEventArgs);

        _cell.TestAccessor().Dynamic.OnMouseUp(args);

        _cell.LinkVisited.Should().BeFalse();
    }

    [WinFormsFact]
    public void OnMouseUp_DoesNotSetLinkVisited_WhenTrackVisitedStateFalse()
    {
        using DataGridView dataGridView = CreateGridWithColumn();
        dataGridView.Rows[0].Cells[0] = _cell;
        _cell.TrackVisitedState = false;
        _cell.LinkVisited = false;
        MouseEventArgs mouseEventArgs = new(MouseButtons.None, 0, 1, 1, 0);
        DataGridViewCellMouseEventArgs args = new(0, 0, 1, 1, mouseEventArgs);

        _cell.TestAccessor().Dynamic.OnMouseUp(args);

        _cell.LinkVisited.Should().BeFalse();
    }

    [WinFormsFact]
    public void Paint_ThrowsArgumentNullException_WhenCellStyleIsNull()
    {
        using Bitmap bitmap = new(10, 10);
        using Graphics graphics = Graphics.FromImage(bitmap);

        TargetInvocationException ex = ((Action)(() =>
            _cell.TestAccessor().Dynamic.Paint(
                graphics,
                new Rectangle(0, 0, 10, 10),
                new Rectangle(0, 0, 10, 10),
                0,
                DataGridViewElementStates.None,
                null,
                null,
                null,
                null,
                null,
                DataGridViewPaintParts.All
            )
        )).Should().Throw<TargetInvocationException>().Subject.First();

        ex.InnerException.Should().BeOfType<ArgumentNullException>();
        ex.InnerException!.Message.Should().Contain("cellStyle");
    }
}
