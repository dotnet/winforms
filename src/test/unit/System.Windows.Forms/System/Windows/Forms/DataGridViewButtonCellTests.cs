// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;

namespace System.Windows.Forms.Tests;

public class DataGridViewButtonCellTests : IDisposable
{
    private readonly DataGridViewButtonCell _dataGridViewButtonCell;

    public void Dispose() => _dataGridViewButtonCell?.Dispose();

    public DataGridViewButtonCellTests() => _dataGridViewButtonCell = new();

    [Fact]
    public void FormattedValueType_ReturnsStringType()
    {
        Type result = _dataGridViewButtonCell.FormattedValueType;

        result.Should().Be(typeof(string));
    }

    [WinFormsFact]
    public void UseColumnTextForButtonValue_DefaultValue_IsFalse() =>
        _dataGridViewButtonCell.UseColumnTextForButtonValue.Should().BeFalse();

    [WinFormsFact]
    public void UseColumnTextForButtonValue_SetTrue_ReflectsValue()
    {
        _dataGridViewButtonCell.UseColumnTextForButtonValue = true;
        _dataGridViewButtonCell.UseColumnTextForButtonValue.Should().BeTrue();
    }

    [Fact]
    public void ValueType_DefaultValue_IsObject() =>
        _dataGridViewButtonCell.ValueType.Should().Be(typeof(object));

    [Fact]
    public void ValueType_WhenBaseValueTypeIsSet_ReturnsBaseValueType()
    {
        Type customType = typeof(int);
        _dataGridViewButtonCell.GetType().BaseType!
            .GetProperty("ValueType")!
            .SetValue(_dataGridViewButtonCell, customType);

        _dataGridViewButtonCell.ValueType.Should().Be(customType);
    }

    [WinFormsFact]
    public void GetContentBounds_ReturnsEmpty_WhenDataGridViewIsNull()
    {
        DataGridViewCellStyle dataGridViewCellStyle = new();
        using Graphics g = Graphics.FromImage(new Bitmap(10, 10));
        var result = _dataGridViewButtonCell.TestAccessor.Dynamic.GetContentBounds(g, dataGridViewCellStyle, 0);

        ((Rectangle)result).Should().Be(Rectangle.Empty);
    }

    [WinFormsFact]
    public void GetContentBounds_ReturnsEmpty_WhenRowIndexIsNegative()
    {
        DataGridViewCellStyle dataGridViewCellStyle = new();
        using Graphics g = Graphics.FromImage(new Bitmap(10, 10));
        using DataGridView dataGridView = new();
        dataGridView.Columns.Add(new DataGridViewButtonColumn());
        dataGridView.Rows.Add();
        dataGridView[0, 0] = _dataGridViewButtonCell;
        var result = _dataGridViewButtonCell.TestAccessor.Dynamic.GetContentBounds(g, dataGridViewCellStyle, -1);

        ((Rectangle)result).Should().Be(Rectangle.Empty);
    }

    [WinFormsFact]
    public void GetContentBounds_ReturnsEmpty_WhenOwningColumnIsNull()
    {
        DataGridViewCellStyle dataGridViewCellStyle = new();
        using Graphics g = Graphics.FromImage(new Bitmap(10, 10));
        using DataGridView dataGridView = new();
        dataGridView.Columns.Add(new DataGridViewTextBoxColumn());
        dataGridView.Rows.Add();
        dataGridView[0, 0] = _dataGridViewButtonCell;
        dataGridView.Columns.Clear();
        var result = _dataGridViewButtonCell.TestAccessor.Dynamic.GetContentBounds(g, dataGridViewCellStyle, 0);

        ((Rectangle)result).Should().Be(Rectangle.Empty);
    }

    [WinFormsFact]
    public void GetErrorIconBounds_ReturnsEmpty_WhenDataGridViewIsNull()
    {
        DataGridViewCellStyle dataGridViewCellStyle = new();
        using Graphics g = Graphics.FromImage(new Bitmap(10, 10));
        var result = _dataGridViewButtonCell.TestAccessor.Dynamic.GetErrorIconBounds(g, dataGridViewCellStyle, 0);

        ((Rectangle)result).Should().Be(Rectangle.Empty);
    }

    [WinFormsFact]
    public void GetErrorIconBounds_ReturnsEmpty_WhenRowIndexIsNegative()
    {
        DataGridViewCellStyle dataGridViewCellStyle = new();
        using Graphics g = Graphics.FromImage(new Bitmap(10, 10));
        using DataGridView dataGridView = new();
        dataGridView.Columns.Add(new DataGridViewButtonColumn());
        dataGridView.Rows.Add();
        dataGridView[0, 0] = _dataGridViewButtonCell;
        var result = _dataGridViewButtonCell.TestAccessor.Dynamic.GetErrorIconBounds(g, dataGridViewCellStyle, -1);

        ((Rectangle)result).Should().Be(Rectangle.Empty);
    }

    [WinFormsFact]
    public void GetErrorIconBounds_ReturnsEmpty_WhenOwningColumnIsNull()
    {
        DataGridViewCellStyle dataGridViewCellStyle = new();
        using Graphics g = Graphics.FromImage(new Bitmap(10, 10));
        using DataGridView dataGridView = new();
        dataGridView.Columns.Add(new DataGridViewTextBoxColumn());
        dataGridView.Rows.Add();
        dataGridView[0, 0] = _dataGridViewButtonCell;
        dataGridView.Columns.Clear();
        var result = _dataGridViewButtonCell.TestAccessor.Dynamic.GetErrorIconBounds(g, dataGridViewCellStyle, 0);

        ((Rectangle)result).Should().Be(Rectangle.Empty);
    }

    [WinFormsFact]
    public void GetErrorIconBounds_ReturnsEmpty_WhenShowCellErrorsIsFalse()
    {
        DataGridViewCellStyle dataGridViewCellStyle = new();
        using Graphics g = Graphics.FromImage(new Bitmap(10, 10));
        using DataGridView dataGridView = new();
        dataGridView.Columns.Add(new DataGridViewButtonColumn());
        dataGridView.Rows.Add();
        dataGridView.ShowCellErrors = false;
        dataGridView[0, 0] = _dataGridViewButtonCell;
        var result = _dataGridViewButtonCell.TestAccessor.Dynamic.GetErrorIconBounds(g, dataGridViewCellStyle, 0);

        ((Rectangle)result).Should().Be(Rectangle.Empty);
    }

    [WinFormsFact]
    public void GetErrorIconBounds_ReturnsEmpty_WhenErrorTextIsEmpty()
    {
        DataGridViewCellStyle dataGridViewCellStyle = new();
        using Graphics g = Graphics.FromImage(new Bitmap(10, 10));
        using DataGridView dataGridView = new();
        dataGridView.Columns.Add(new DataGridViewButtonColumn());
        dataGridView.Rows.Add();
        dataGridView[0, 0] = _dataGridViewButtonCell;
        var result = _dataGridViewButtonCell.TestAccessor.Dynamic.GetErrorIconBounds(g, dataGridViewCellStyle, 0);

        ((Rectangle)result).Should().Be(Rectangle.Empty);
    }

    [WinFormsFact]
    public void GetErrorIconBounds_ReturnsNonEmpty_WhenErrorTextIsSet()
    {
        DataGridViewCellStyle dataGridViewCellStyle = new();
        using Graphics g = Graphics.FromImage(new Bitmap(10, 10));
        using DataGridView dataGridView = new();
        dataGridView.Columns.Add(new DataGridViewButtonColumn());
        dataGridView.Rows.Add();
        dataGridView[0, 0] = _dataGridViewButtonCell;
        _dataGridViewButtonCell.ErrorText = "Error!";

        var result = _dataGridViewButtonCell.TestAccessor.Dynamic.GetErrorIconBounds(g, dataGridViewCellStyle, 0);

        ((Rectangle)result).Should().NotBe(Rectangle.Empty);
    }

    [WinFormsFact]
    public void GetPreferredSize_ReturnsMinusOne_WhenDataGridViewIsNull()
    {
        DataGridViewCellStyle dataGridViewCellStyle = new()
        {
            Font = SystemFonts.DefaultFont
        };
        using Graphics g = Graphics.FromImage(new Bitmap(10, 10));
        var result = _dataGridViewButtonCell.TestAccessor.Dynamic.GetPreferredSize(g, dataGridViewCellStyle, 0, new Size(100, 100));

        ((Size)result).Should().Be(new Size(-1, -1));
    }

    [WinFormsFact]
    public void GetPreferredSize_ThrowsNullReferenceException_WhenCellStyleIsNull()
    {
        using Graphics g = Graphics.FromImage(new Bitmap(10, 10));
        using DataGridView dataGridView = new();
        dataGridView.Columns.Add(new DataGridViewButtonColumn());
        dataGridView.Rows.Add();
        dataGridView[0, 0] = _dataGridViewButtonCell;

        Action action = () => _dataGridViewButtonCell.TestAccessor.Dynamic.GetPreferredSize(g, null, 0, new Size(100, 100));
        action.Should().Throw<NullReferenceException>();
    }

    [WinFormsFact]
    public void GetPreferredSize_UsesThemeMargins_WhenVisualStylesApplied()
    {
        using Graphics g = Graphics.FromImage(new Bitmap(100, 100));
        using DataGridView dataGridView = new();
        dataGridView.Columns.Add(new DataGridViewButtonColumn());
        dataGridView.Rows.Add();
        dataGridView[0, 0] = _dataGridViewButtonCell;
        dataGridView.Rows[0].Cells[0].Value = "Test";
        dataGridView.Rows[0].Cells[0].Style.Font = SystemFonts.DefaultFont;
        dataGridView.Rows[0].Cells[0].Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
        dataGridView.Rows[0].Cells[0].Style.Padding = Padding.Empty;
        dataGridView.Rows[0].Cells[0].Style.WrapMode = DataGridViewTriState.False;

        if (!DataGridView.ApplyVisualStylesToInnerCells)
            return;

        var result = _dataGridViewButtonCell.TestAccessor.Dynamic.GetPreferredSize(
            g, dataGridView.Rows[0].Cells[0].Style, 0, Size.Empty);

        ((Size)result).Width.Should().BeGreaterThan(0);
        ((Size)result).Height.Should().BeGreaterThan(0);
    }

    [WinFormsFact]
    public void GetValue_ReturnsColumnText_WhenUseColumnTextForButtonValueIsTrue()
    {
        using DataGridView dataGridView = new();
        using DataGridViewButtonColumn dataGridViewButtonColumn = new() { Text = "ColumnText" };
        dataGridView.Columns.Add(dataGridViewButtonColumn);
        dataGridView.Rows.Add();
        _dataGridViewButtonCell.UseColumnTextForButtonValue = true;
        dataGridView[0, 0] = _dataGridViewButtonCell;

        var value = _dataGridViewButtonCell.TestAccessor.Dynamic.GetValue(0);

        ((string)value).Should().Be("ColumnText");
    }

    [WinFormsFact]
    public void GetValue_ReturnsBaseValue_WhenUseColumnTextForButtonValueIsFalse()
    {
        using DataGridView dataGridView = new();
        using DataGridViewButtonColumn dataGridViewButtonColumn = new();
        dataGridView.Columns.Add(dataGridViewButtonColumn);
        dataGridView.Rows.Add();
        _dataGridViewButtonCell.UseColumnTextForButtonValue = false;
        dataGridView[0, 0] = _dataGridViewButtonCell;
        _dataGridViewButtonCell.Value = "CellValue";

        var value = _dataGridViewButtonCell.TestAccessor.Dynamic.GetValue(0);

        ((string)value).Should().Be("CellValue");
    }

    [WinFormsFact]
    public void GetValue_ReturnsBaseValue_WhenOwningColumnIsNotButtonColumn()
    {
        using DataGridView dataGridView = new();
        using DataGridViewTextBoxColumn dataGridViewTextBoxColumn = new();
        dataGridView.Columns.Add(dataGridViewTextBoxColumn);
        dataGridView.Rows.Add();
        using DataGridViewButtonColumn dataGridViewButtonColumn = new() { Text = "CellValue" };
        dataGridView.Columns.Add(dataGridViewButtonColumn);
        dataGridView[0, 0] = _dataGridViewButtonCell;
        _dataGridViewButtonCell.Value = "CellValue";

        var value = _dataGridViewButtonCell.TestAccessor.Dynamic.GetValue(0);

        ((string)value).Should().Be("CellValue");
    }

    [Theory]
    [InlineData(Keys.Space, false, false, false, true)]
    [InlineData(Keys.Space, true, false, false, false)]
    [InlineData(Keys.Space, false, true, false, false)]
    [InlineData(Keys.Space, false, false, true, false)]
    [InlineData(Keys.A, false, false, false, false)]
    public void KeyDownUnsharesRow_ReturnsExpected(Keys key, bool alt, bool control, bool shift, bool expected)
    {
        Keys keyData = key;
        if (alt)
            keyData |= Keys.Alt;
        if (control)
            keyData |= Keys.Control;
        if (shift)
            keyData |= Keys.Shift;
        KeyEventArgs e = new(keyData);

        bool result = _dataGridViewButtonCell.TestAccessor.Dynamic.KeyDownUnsharesRow(e, 0);

        result.Should().Be(expected);
    }

    [Theory]
    [InlineData(Keys.Space, true)]
    [InlineData(Keys.A, false)]
    public void KeyUpUnsharesRow_ReturnsExpected(Keys key, bool expected)
    {
        KeyEventArgs e = new(key);

        bool result = _dataGridViewButtonCell.TestAccessor.Dynamic.KeyUpUnsharesRow(e, 0);

        result.Should().Be(expected);
    }

    [Theory]
    [InlineData(MouseButtons.Left, true)]
    [InlineData(MouseButtons.Right, false)]
    [InlineData(MouseButtons.Middle, false)]
    [InlineData(MouseButtons.None, false)]
    public void MouseDownUnsharesRow_ReturnsExpected(MouseButtons button, bool expected)
    {
        DataGridViewCellMouseEventArgs e = new(0, 0, 0, 0, new MouseEventArgs(button, 1, 0, 0, 0));

        bool result = _dataGridViewButtonCell.TestAccessor.Dynamic.MouseDownUnsharesRow(e);
        result.Should().Be(expected);
    }

    [WinFormsTheory]
    [InlineData(ButtonState.Pushed, true)]
    [InlineData(ButtonState.Normal, false)]
    [InlineData(ButtonState.Checked, false)]
    [InlineData(ButtonState.Pushed | ButtonState.Checked, true)]
    public void MouseLeaveUnsharesRow_ReturnsExpected(ButtonState buttonState, bool expected)
    {
        using DataGridView dataGridView = new();
        using DataGridViewButtonColumn dataGridViewButtonColumn = new();
        dataGridView.Columns.Add(dataGridViewButtonColumn);
        dataGridView.Rows.Add();
        dataGridView[0, 0] = _dataGridViewButtonCell;

        _dataGridViewButtonCell.TestAccessor.Dynamic.ButtonState = buttonState;

        bool result = _dataGridViewButtonCell.TestAccessor.Dynamic.MouseLeaveUnsharesRow(0);
        result.Should().Be(expected);
    }

    [WinFormsTheory]
    [InlineData(MouseButtons.Left, true)]
    [InlineData(MouseButtons.Right, false)]
    [InlineData(MouseButtons.Middle, false)]
    [InlineData(MouseButtons.None, false)]
    public void MouseUpUnsharesRow_ReturnsExpected(MouseButtons button, bool expected)
    {
        using DataGridView dataGridView = new();
        using DataGridViewButtonColumn dataGridViewButtonColumn = new();
        dataGridView.Columns.Add(dataGridViewButtonColumn);
        dataGridView.Rows.Add();
        dataGridView[0, 0] = _dataGridViewButtonCell;
        DataGridViewCellMouseEventArgs dataGridViewCellMouseEventArgs = new(0, 0, 0, 0, new MouseEventArgs(button, 1, 0, 0, 0));

        bool result = _dataGridViewButtonCell.TestAccessor.Dynamic.MouseUpUnsharesRow(dataGridViewCellMouseEventArgs);
        result.Should().Be(expected);
    }

    [WinFormsFact]
    public void OnKeyDown_SetsCheckedStateAndHandled_WhenSpacePressedWithoutModifiers()
    {
        using DataGridView dataGridView = new();
        using DataGridViewButtonColumn dataGridViewButtonColumn = new();
        dataGridView.Columns.Add(dataGridViewButtonColumn);
        dataGridView.Rows.Add();
        dataGridView[0, 0] = _dataGridViewButtonCell;

        KeyEventArgs keyEventArgs = new(Keys.Space);

        ButtonState buttonState = _dataGridViewButtonCell.TestAccessor.Dynamic.ButtonState;
        buttonState.Should().Be(ButtonState.Normal);

        _dataGridViewButtonCell.TestAccessor.Dynamic.OnKeyDown(keyEventArgs, 0);
        buttonState = _dataGridViewButtonCell.TestAccessor.Dynamic.ButtonState;

        buttonState.HasFlag(ButtonState.Checked).Should().BeTrue();
        keyEventArgs.Handled.Should().BeTrue();
    }

    [WinFormsTheory]
    [InlineData(Keys.Space | Keys.Alt)]
    [InlineData(Keys.Space | Keys.Control)]
    [InlineData(Keys.Space | Keys.Shift)]
    [InlineData(Keys.A)]
    public void OnKeyDown_DoesNotSetCheckedStateOrHandled_WhenModifiersOrOtherKey(Keys keyData)
    {
        using DataGridView dataGridView = new();
        using DataGridViewButtonColumn dataGridViewButtonColumn = new();
        dataGridView.Columns.Add(dataGridViewButtonColumn);
        dataGridView.Rows.Add();
        dataGridView[0, 0] = _dataGridViewButtonCell;

        KeyEventArgs keyEventArgs = new(keyData);

        ButtonState buttonState = _dataGridViewButtonCell.TestAccessor.Dynamic.ButtonState;
        buttonState.Should().Be(ButtonState.Normal);

        _dataGridViewButtonCell.TestAccessor.Dynamic.OnKeyDown(keyEventArgs, 0);
        buttonState = _dataGridViewButtonCell.TestAccessor.Dynamic.ButtonState;

        (buttonState & ButtonState.Checked).Should().Be(0);
        keyEventArgs.Handled.Should().BeFalse();
    }

    [WinFormsTheory]
    [InlineData(Keys.Space, true)]
    [InlineData(Keys.Space | Keys.Alt, false)]
    [InlineData(Keys.Space | Keys.Control, false)]
    [InlineData(Keys.Space | Keys.Shift, false)]
    public void OnKeyUp_RemovesCheckedState_AndSetsHandledAsExpected(Keys keyData, bool expectedHandled)
    {
        using DataGridView dataGridView = new();
        using DataGridViewButtonColumn dataGridViewButtonColumn = new();
        dataGridView.Columns.Add(dataGridViewButtonColumn);
        dataGridView.Rows.Add();
        dataGridView[0, 0] = _dataGridViewButtonCell;

        _dataGridViewButtonCell.TestAccessor.Dynamic.ButtonState = ButtonState.Checked;

        KeyEventArgs keyEventArgs = new(keyData);
        _dataGridViewButtonCell.TestAccessor.Dynamic.OnKeyUp(keyEventArgs, 0);

        ButtonState buttonState = _dataGridViewButtonCell.TestAccessor.Dynamic.ButtonState;
        buttonState.HasFlag(ButtonState.Checked).Should().BeFalse();
        keyEventArgs.Handled.Should().Be(expectedHandled);
    }

    [WinFormsFact]
    public void OnLeave_ResetsButtonState_WhenNotNormal()
    {
        using DataGridView dataGridView = new();
        using DataGridViewButtonColumn dataGridViewButtonColumn = new();
        dataGridView.Columns.Add(dataGridViewButtonColumn);
        dataGridView.Rows.Add();
        dataGridView[0, 0] = _dataGridViewButtonCell;

        _dataGridViewButtonCell.TestAccessor.Dynamic.ButtonState = ButtonState.Pushed | ButtonState.Checked;

        _dataGridViewButtonCell.TestAccessor.Dynamic.OnLeave(0, false);

        ButtonState buttonState = _dataGridViewButtonCell.TestAccessor.Dynamic.ButtonState;
        buttonState.Should().Be(ButtonState.Normal);
    }

    [WinFormsFact]
    public void OnMouseUp_UpdatesButtonState_WhenLeftButton()
    {
        using DataGridView dataGridView = new();
        using DataGridViewButtonColumn dataGridViewButtonColumn = new();
        dataGridView.Columns.Add(dataGridViewButtonColumn);
        dataGridView.Rows.Add();
        dataGridView[0, 0] = _dataGridViewButtonCell;
        _dataGridViewButtonCell.TestAccessor.Dynamic.ButtonState = ButtonState.Pushed;

        DataGridViewCellMouseEventArgs dataGridViewCellMouseEventArgs = new(0, 0, 0, 0, new MouseEventArgs(MouseButtons.Left, 1, 0, 0, 0));
        _dataGridViewButtonCell.TestAccessor.Dynamic.OnMouseUp(dataGridViewCellMouseEventArgs);

        ((ButtonState)_dataGridViewButtonCell.TestAccessor.Dynamic.ButtonState).HasFlag(ButtonState.Pushed).Should().BeFalse();
    }

    [WinFormsFact]
    public void Paint_CallsPaintPrivate_WithExpectedParameters()
    {
        using var g = Graphics.FromImage(new Bitmap(10, 10));
        using DataGridView dataGridView = new();
        dataGridView.Columns.Add(new DataGridViewButtonColumn());
        dataGridView.Rows.Add();
        dataGridView[0, 0] = _dataGridViewButtonCell;
        DataGridViewCellStyle dataGridViewCellStyle = new() { Font = SystemFonts.DefaultFont };
        DataGridViewAdvancedBorderStyle advancedBorderStyle = new();
        _dataGridViewButtonCell.TestAccessor.Dynamic.Paint(
            g,
            new Rectangle(0, 0, 10, 10),
            new Rectangle(0, 0, 10, 10),
            0,
            DataGridViewElementStates.Selected,
            "value",
            "formatted",
            "error",
            dataGridViewCellStyle,
            advancedBorderStyle,
            DataGridViewPaintParts.All);

        _dataGridViewButtonCell.DataGridView.Should().BeSameAs(dataGridView);
        dataGridViewCellStyle.Font.Should().Be(SystemFonts.DefaultFont);
    }

    [Fact]
    public void ToString_ReturnsExpectedFormat_WithDefaultIndices()
    {
        string result = _dataGridViewButtonCell.ToString();
        result.Should().Be("DataGridViewButtonCell { ColumnIndex=-1, RowIndex=-1 }");
    }

    [WinFormsFact]
    public void ToString_ReturnsExpectedFormat_WhenAddedToDataGridView()
    {
        using DataGridView dataGridView = new();
        dataGridView.Columns.Add(new DataGridViewButtonColumn());
        dataGridView.Rows.Add();
        dataGridView[0, 0] = _dataGridViewButtonCell;

        _dataGridViewButtonCell.ToString().Should().Be("DataGridViewButtonCell { ColumnIndex=0, RowIndex=0 }");
    }
}
