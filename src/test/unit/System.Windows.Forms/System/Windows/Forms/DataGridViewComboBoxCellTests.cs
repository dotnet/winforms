// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;
using System.Drawing;

namespace System.Windows.Forms.Tests;

public class DataGridViewComboBoxCellTests : IDisposable
{
    private readonly DataGridViewComboBoxCell _dataGridViewComboBoxCell;

    public void Dispose() => _dataGridViewComboBoxCell.Dispose();

    public DataGridViewComboBoxCellTests() =>
        _dataGridViewComboBoxCell = new();

    [Fact]
    public void AutoComplete_DefaultValue_IsTrue() =>
        _dataGridViewComboBoxCell.AutoComplete.Should().BeTrue();

    [Fact]
    public void AutoComplete_SetFalse_ChangesValue()
    {
        _dataGridViewComboBoxCell.AutoComplete = false;
        _dataGridViewComboBoxCell.AutoComplete.Should().BeFalse();
    }

    [WinFormsFact]
    public void DataSource_DefaultValue_IsNull() =>
        _dataGridViewComboBoxCell.DataSource.Should().BeNull();

    [WinFormsFact]
    public void DataSource_SetToIList_StoresReference()
    {
        IList<string> list = ["a", "b"];
        _dataGridViewComboBoxCell.DataSource = list;
        _dataGridViewComboBoxCell.DataSource.Should().BeSameAs(list);
    }

    [WinFormsFact]
    public void DataSource_SetToIListSource_StoresReference()
    {
        using BindingSource bindingSource = new();
        _dataGridViewComboBoxCell.DataSource = bindingSource;

        _dataGridViewComboBoxCell.DataSource.Should().BeSameAs(bindingSource);
    }

    [WinFormsFact]
    public void DataSource_SetToInvalidType_Throws()
    {
        Exception? invalidDataSourceException = Record.Exception(() => _dataGridViewComboBoxCell.DataSource = 123);

        invalidDataSourceException.Should().BeOfType<ArgumentException>();
    }

    [WinFormsFact]
    public void DataSource_SetToNull_ClearsDisplayAndValueMember()
    {
        _dataGridViewComboBoxCell.DataSource = new List<string> { "a" };
        _dataGridViewComboBoxCell.DisplayMember = "Name";
        _dataGridViewComboBoxCell.ValueMember = "Id";
        _dataGridViewComboBoxCell.DataSource = null;

        _dataGridViewComboBoxCell.DataSource.Should().BeNull();
        _dataGridViewComboBoxCell.DisplayMember.Should().BeEmpty();
        _dataGridViewComboBoxCell.ValueMember.Should().BeEmpty();
    }

    [WinFormsFact]
    public void DisplayStyleForCurrentCellOnly_DefaultValue_IsFalse() =>
        _dataGridViewComboBoxCell.DisplayStyleForCurrentCellOnly.Should().BeFalse();

    [Fact]
    public void DropDownWidth_DefaultValue_IsOne() =>
        _dataGridViewComboBoxCell.DropDownWidth.Should().Be(1);

    [Theory]
    [InlineData(5)]
    [InlineData(100)]
    public void DropDownWidth_SetValidValue_ChangesValue(int width)
    {
        _dataGridViewComboBoxCell.DropDownWidth = width;
        _dataGridViewComboBoxCell.DropDownWidth.Should().Be(width);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-5)]
    public void DropDownWidth_SetInvalidValue_Throws(int width)
    {
        Exception? dropDownWidthOutOfRangeException = Record.Exception(() => _dataGridViewComboBoxCell.DropDownWidth = width);

        dropDownWidthOutOfRangeException.Should().BeOfType<ArgumentOutOfRangeException>();
    }

    [Fact]
    public void FlatStyle_DefaultValue_IsStandard() =>
        _dataGridViewComboBoxCell.FlatStyle.Should().Be(FlatStyle.Standard);

    [WinFormsTheory]
    [InlineData(FlatStyle.Flat)]
    [InlineData(FlatStyle.Popup)]
    [InlineData(FlatStyle.System)]
    [InlineData(FlatStyle.Standard)]
    public void FlatStyle_SetValidValue_ChangesValue(FlatStyle style)
    {
        _dataGridViewComboBoxCell.FlatStyle = style;
        _dataGridViewComboBoxCell.FlatStyle.Should().Be(style);
    }

    [Fact]
    public void MaxDropDownItems_DefaultValue_IsEight() =>
        _dataGridViewComboBoxCell.MaxDropDownItems.Should().Be(DataGridViewComboBoxCell.DefaultMaxDropDownItems);

    [Theory]
    [InlineData(1)]
    [InlineData(50)]
    [InlineData(100)]
    public void MaxDropDownItems_SetValidValue_ChangesValue(int value)
    {
        _dataGridViewComboBoxCell.MaxDropDownItems = value;
        _dataGridViewComboBoxCell.MaxDropDownItems.Should().Be(value);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(101)]
    [InlineData(200)]
    public void MaxDropDownItems_SetInvalidValue_Throws(int value)
    {
        Exception? maxDropDownItemsOutOfRangeException = Record.Exception(() => _dataGridViewComboBoxCell.MaxDropDownItems = value);

        maxDropDownItemsOutOfRangeException.Should().BeOfType<ArgumentOutOfRangeException>();
    }

    [Fact]
    public void Sorted_DefaultValue_IsFalse() =>
        _dataGridViewComboBoxCell.Sorted.Should().BeFalse();

    [Fact]
    public void Sorted_SetTrue_WithoutDataSource_SetsFlag()
    {
        _dataGridViewComboBoxCell.Sorted = true;
        _dataGridViewComboBoxCell.Sorted.Should().BeTrue();
    }

    [WinFormsFact]
    public void Sorted_SetTrue_WithDataSource_Throws()
    {
        _dataGridViewComboBoxCell.DataSource = new List<string> { "a", "b" };

        Exception? sortedWithDataSourceException = Record.Exception(() => _dataGridViewComboBoxCell.Sorted = true);

        sortedWithDataSourceException.Should().BeOfType<ArgumentException>();
    }

    [Fact]
    public void ValueMember_DefaultValue_IsEmpty() =>
        _dataGridViewComboBoxCell.ValueMember.Should().BeEmpty();

    [WinFormsTheory]
    [InlineData("Id")]
    [InlineData("Name")]
    [InlineData("")]
    public void ValueMember_SetValue_ChangesValue(string value)
    {
        _dataGridViewComboBoxCell.ValueMember = value;
        _dataGridViewComboBoxCell.ValueMember.Should().Be(value);
    }

    [Fact]
    public void ValueType_DefaultValue_IsObject() =>
        _dataGridViewComboBoxCell.ValueType.Should().Be(typeof(object));

    private class StubPropertyDescriptor : PropertyDescriptor
    {
        private readonly Type _propertyType;

        public StubPropertyDescriptor(string name, Type propertyType)
            : base(name, null)
        {
            _propertyType = propertyType;
        }

        public override Type PropertyType => _propertyType;
        public override void SetValue(object? component, object? value) { }
        public override object? GetValue(object? component) => null!;
        public override bool CanResetValue(object? component) => false;
        public override void ResetValue(object? component) { }
        public override bool IsReadOnly => true;
        public override Type ComponentType => typeof(object);
        public override bool ShouldSerializeValue(object? component) => false;
    }

    [Fact]
    public void ValueType_WithDisplayMemberProperty_ReturnsPropertyType()
    {
        StubPropertyDescriptor prop = new("Name", typeof(string));

        _dataGridViewComboBoxCell.TestAccessor.Dynamic.DisplayMemberProperty = prop;
        _dataGridViewComboBoxCell.TestAccessor.Dynamic.ValueMemberProperty = null;

        _dataGridViewComboBoxCell.ValueType.Should().Be(typeof(string));
    }

    [Fact]
    public void GetContentBounds_ReturnsEmpty_WhenDataGridViewIsNull()
    {
        using Bitmap bmp = new(10, 10);
        using Graphics g = Graphics.FromImage(bmp);
        DataGridViewCellStyle style = new();

        Rectangle result = _dataGridViewComboBoxCell.TestAccessor.Dynamic.GetContentBounds(g, style, 0);

        result.Should().Be(Rectangle.Empty);
    }

    [Fact]
    public void GetContentBounds_ReturnsEmpty_WhenRowIndexIsNegative()
    {
        using DataGridView dataGridView = new();
        using DataGridViewComboBoxColumn column = new() { CellTemplate = _dataGridViewComboBoxCell };
        dataGridView.Columns.Add(column);
        using Bitmap bmp = new(10, 10);
        using Graphics g = Graphics.FromImage(bmp);
        DataGridViewCellStyle style = new();

        Rectangle result = _dataGridViewComboBoxCell.TestAccessor.Dynamic.GetContentBounds(g, style, -1);

        result.Should().Be(Rectangle.Empty);
    }

    [Fact]
    public void GetErrorIconBounds_ReturnsEmpty_WhenDataGridViewIsNull()
    {
        using Bitmap bmp = new(10, 10);
        using Graphics g = Graphics.FromImage(bmp);
        DataGridViewCellStyle style = new();

        Rectangle result = _dataGridViewComboBoxCell.TestAccessor.Dynamic.GetErrorIconBounds(g, style, 0);

        result.Should().Be(Rectangle.Empty);
    }

    [Fact]
    public void GetErrorIconBounds_UsingTestAccessor_ReturnsEmpty_WhenRowIndexIsNegative()
    {
        using DataGridView dataGridView = new();
        using DataGridViewComboBoxColumn column = new() { CellTemplate = _dataGridViewComboBoxCell };
        dataGridView.Columns.Add(column);
        using Bitmap bmp = new(10, 10);
        using Graphics g = Graphics.FromImage(bmp);
        DataGridViewCellStyle style = new();

        Rectangle result = _dataGridViewComboBoxCell.TestAccessor.Dynamic.GetErrorIconBounds(g, style, -1);

        result.Should().Be(Rectangle.Empty);
    }

    [Fact]
    public void GetPreferredSize_ReturnsMinusOne_WhenDataGridViewIsNull()
    {
        using Bitmap bmp = new(10, 10);
        using Graphics g = Graphics.FromImage(bmp);
        DataGridViewCellStyle style = new();

        Size result = _dataGridViewComboBoxCell.TestAccessor.Dynamic.GetPreferredSize(
            g,
            style,
            0,
            new Size(100, 20));

        result.Should().Be(new Size(-1, -1));
    }

    [Theory]
    [InlineData(Keys.A, false, false, false, true)]
    [InlineData(Keys.F4, false, false, false, true)]
    [InlineData(Keys.Space, false, false, false, true)]
    [InlineData(Keys.Space, true, false, false, false)]
    [InlineData(Keys.Down, false, true, false, true)]
    [InlineData(Keys.Up, false, true, false, true)]
    [InlineData(Keys.Down, false, false, false, false)]
    [InlineData(Keys.F1, false, false, false, false)]
    [InlineData(Keys.F24, false, false, false, false)]
    [InlineData(Keys.A, false, false, true, false)]
    public void KeyEntersEditMode_ReturnsExpected(Keys key, bool shift, bool alt, bool control, bool expected)
    {
        Keys keyData = key;
        if (shift)
        {
            keyData |= Keys.Shift;
        }

        if (alt)
        {
            keyData |= Keys.Alt;
        }

        if (control)
        {
            keyData |= Keys.Control;
        }

        KeyEventArgs e = new(keyData);
        bool result = _dataGridViewComboBoxCell.KeyEntersEditMode(e);
        result.Should().Be(expected);
    }

    [Fact]
    public void ParseFormattedValue_UsesValueTypeConverter_WhenValueTypeConverterIsProvided()
    {
        using DataGridView dataGridView = new();
        using DataGridViewComboBoxColumn column = new() { CellTemplate = _dataGridViewComboBoxCell };
        dataGridView.Columns.Add(column);
        DataGridViewCellStyle style = new();

        object? result = _dataGridViewComboBoxCell.ParseFormattedValue(
            "test",
            style,
            null,
            TypeDescriptor.GetConverter(typeof(string)));

        result.Should().Be("test");
    }

    [Fact]
    public void ParseFormattedValue_UsesValueMemberPropertyConverter_WhenValueTypeConverterIsNull()
    {
        using DataGridView dataGridView = new();
        using DataGridViewComboBoxColumn column = new() { CellTemplate = _dataGridViewComboBoxCell };
        dataGridView.Columns.Add(column);
        DataGridViewCellStyle style = new();

        StubPropertyDescriptor prop = new("Id", typeof(string));
        _dataGridViewComboBoxCell.TestAccessor.Dynamic.ValueMemberProperty = prop;

        object? result = _dataGridViewComboBoxCell.ParseFormattedValue("abc", style, null, null);

        result.Should().Be("abc");
    }

    [Fact]
    public void ParseFormattedValue_UsesParseFormattedValueInternal_WithValueType_WhenNoDataManagerOrMembers()
    {
        using DataGridView dataGridView = new();
        using DataGridViewComboBoxColumn column = new() { CellTemplate = _dataGridViewComboBoxCell };
        dataGridView.Columns.Add(column);
        DataGridViewCellStyle style = new();

        object? result = _dataGridViewComboBoxCell.ParseFormattedValue("test", style, null, null);

        result.Should().Be("test");
    }
}
