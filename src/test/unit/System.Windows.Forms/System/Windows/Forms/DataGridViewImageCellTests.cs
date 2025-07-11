// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;

namespace System.Windows.Forms.Tests;

public class DataGridViewImageCellTests : IDisposable
{
    private readonly DataGridViewImageCell _dataGridViewImageCell;

    public DataGridViewImageCellTests() => _dataGridViewImageCell = new();

    public void Dispose() => _dataGridViewImageCell.Dispose();

    [Fact]
    public void Ctor_Default_SetsValueIsIconFalse() =>
        _dataGridViewImageCell.ValueIsIcon.Should().BeFalse();

    [WinFormsTheory]
    [BoolData]
    public void Ctor_ValueIsIcon_SetsValueIsIcon(bool valueIsIcon)
    {
        using DataGridViewImageCell dataGridViewImageCell = new(valueIsIcon);

        dataGridViewImageCell.ValueIsIcon.Should().Be(valueIsIcon);
    }

    [Fact]
    public void DefaultNewRowValue_ReturnsErrorBitmap_WhenValueTypeIsImage()
    {
        _dataGridViewImageCell.ValueType = typeof(Image);
        var value = _dataGridViewImageCell.DefaultNewRowValue;

        value.Should().BeSameAs(DataGridViewImageCell.ErrorBitmap);
    }

    [WinFormsFact]
    public void DefaultNewRowValue_ReturnsNull_WhenValueTypeIsOther()
    {
        _dataGridViewImageCell.ValueType = typeof(string);
        _dataGridViewImageCell.ValueIsIcon = false;
        object? value = _dataGridViewImageCell.DefaultNewRowValue;

        value.Should().BeNull();
    }

    [Fact]
    public void EditType_IsAlwaysNull()
    {
        _dataGridViewImageCell.EditType.Should().BeNull();

        using DataGridViewImageCell dataGridViewImageCellIcon = new(true);
        dataGridViewImageCellIcon.EditType.Should().BeNull();
    }

    [WinFormsFact]
    public void FormattedValueType_ReturnsImage_WhenValueIsIconFalse()
    {
        _dataGridViewImageCell.ValueIsIcon = false;
        _dataGridViewImageCell.FormattedValueType.Should().Be(typeof(Image));
    }

    [WinFormsFact]
    public void ValueIsIcon_SetFalse_UpdatesFlag()
    {
        _dataGridViewImageCell.ValueIsIcon = true;
        _dataGridViewImageCell.ValueIsIcon = false;
        _dataGridViewImageCell.ValueIsIcon.Should().BeFalse();
    }

    [Fact]
    public void ValueType_GetSet_RoundTrips()
    {
        _dataGridViewImageCell.ValueType = typeof(Image);
        _dataGridViewImageCell.ValueType.Should().Be(typeof(Image));

        _dataGridViewImageCell.ValueType = null;
        _dataGridViewImageCell.ValueType.Should().Be(typeof(Image));

        _dataGridViewImageCell.ValueIsIcon = true;
        _dataGridViewImageCell.ValueType.Should().Be(typeof(Icon));
    }

    [Fact]
    public void ValueType_SetToNull_ResetsValueIsIconToDefault()
    {
        _dataGridViewImageCell.ValueType = typeof(Icon);
        _dataGridViewImageCell.ValueIsIcon.Should().BeTrue();

        _dataGridViewImageCell.ValueType = null;
        _dataGridViewImageCell.ValueIsIcon.Should().BeFalse();
    }

    [Fact]
    public void GetContentBounds_ReturnsEmpty_WhenDataGridViewIsNull()
    {
        using Graphics g = Graphics.FromImage(new Bitmap(10, 10));
        DataGridViewCellStyle dataGridViewCellStyle = new();
        _dataGridViewImageCell.DataGridView = null;
        Rectangle bounds = (Rectangle)_dataGridViewImageCell.TestAccessor().Dynamic.GetContentBounds(g, dataGridViewCellStyle, 0);

        bounds.Should().Be(Rectangle.Empty);
    }

    [WinFormsFact]
    public void GetContentBounds_ReturnsEmpty_WhenRowIndexNegative()
    {
        using DataGridView dataGridView = new();
        _dataGridViewImageCell.DataGridView = dataGridView;
        using Graphics g = Graphics.FromImage(new Bitmap(10, 10));
        DataGridViewCellStyle dataGridViewCellStyle = new();
        Rectangle bounds = (Rectangle)_dataGridViewImageCell.TestAccessor().Dynamic.GetContentBounds(g, dataGridViewCellStyle, -1);

        bounds.Should().Be(Rectangle.Empty);
    }

    [WinFormsFact]
    public void GetContentBounds_ReturnsEmpty_WhenOwningColumnIsNull()
    {
        using DataGridView dataGridView = new();
        dataGridView.Columns.Add(new DataGridViewImageColumn());
        dataGridView.Rows.Add();
        _dataGridViewImageCell.DataGridView = dataGridView;
        _dataGridViewImageCell.OwningColumn = null;
        using Graphics g = Graphics.FromImage(new Bitmap(10, 10));
        DataGridViewCellStyle dataGridViewCellStyle = new();
        Rectangle bounds = (Rectangle)_dataGridViewImageCell.TestAccessor().Dynamic.GetContentBounds(g, dataGridViewCellStyle, 0);

        bounds.Should().Be(Rectangle.Empty);
    }

    [Fact]
    public void GetErrorIconBounds_ReturnsEmpty_WhenDataGridViewIsNull()
    {
        using Graphics g = Graphics.FromImage(new Bitmap(10, 10));
        DataGridViewCellStyle dataGridViewCellStyle = new();
        Rectangle bounds = (Rectangle)_dataGridViewImageCell.TestAccessor().Dynamic.GetErrorIconBounds(g, dataGridViewCellStyle, 0);

        bounds.Should().Be(Rectangle.Empty);
    }

    [WinFormsFact]
    public void GetErrorIconBounds_ReturnsEmpty_WhenRowIndexNegative()
    {
        using DataGridView dataGridView = new();
        dataGridView.Columns.Add(new DataGridViewImageColumn());
        dataGridView.Rows.Add();
        _dataGridViewImageCell.DataGridView = dataGridView;
        using Graphics g = Graphics.FromImage(new Bitmap(10, 10));
        DataGridViewCellStyle dataGridViewCellStyle = new();
        Rectangle bounds = (Rectangle)_dataGridViewImageCell.TestAccessor().Dynamic.GetErrorIconBounds(g, dataGridViewCellStyle, -1);

        bounds.Should().Be(Rectangle.Empty);
    }

    [WinFormsFact]
    public void GetErrorIconBounds_ReturnsEmpty_WhenOwningColumnIsNull()
    {
        using DataGridView dataGridView = new();
        dataGridView.Columns.Add(new DataGridViewImageColumn());
        dataGridView.Rows.Add();
        _dataGridViewImageCell.DataGridView = dataGridView;
        _dataGridViewImageCell.OwningColumn = null;
        using Graphics g = Graphics.FromImage(new Bitmap(10, 10));
        DataGridViewCellStyle dataGridViewCellStyle = new();
        Rectangle bounds = (Rectangle)_dataGridViewImageCell.TestAccessor().Dynamic.GetErrorIconBounds(g, dataGridViewCellStyle, 0);

        bounds.Should().Be(Rectangle.Empty);
    }

    [WinFormsFact]
    public void GetErrorIconBounds_ReturnsEmpty_WhenShowCellErrorsIsFalse()
    {
        using DataGridView dataGridView = new();
        using DataGridViewImageColumn dataGridViewImageColumn = new();
        dataGridView.Columns.Add(dataGridViewImageColumn);
        dataGridView.Rows.Add();
        dataGridView.ShowCellErrors = false;
        _dataGridViewImageCell.DataGridView = dataGridView;
        using Graphics g = Graphics.FromImage(new Bitmap(10, 10));
        DataGridViewCellStyle dataGridViewCellStyle = new();
        Rectangle bounds = (Rectangle)_dataGridViewImageCell.TestAccessor().Dynamic.GetErrorIconBounds(g, dataGridViewCellStyle, 0);

        bounds.Should().Be(Rectangle.Empty);
    }

    [WinFormsFact]
    public void GetErrorIconBounds_ReturnsEmpty_WhenErrorTextIsNullOrEmpty()
    {
        using DataGridView dataGridView = new();
        using DataGridViewImageColumn dataGridViewImageColumn = new();
        dataGridView.Columns.Add(dataGridViewImageColumn);
        dataGridView.Rows.Add();
        dataGridView.ShowCellErrors = true;
        _dataGridViewImageCell.DataGridView = dataGridView;
        _dataGridViewImageCell.ErrorText = null;
        _dataGridViewImageCell.OwningColumn = null;
        using Graphics g = Graphics.FromImage(new Bitmap(10, 10));
        DataGridViewCellStyle dataGridViewCellStyle = new();
        Rectangle bounds = (Rectangle)_dataGridViewImageCell.TestAccessor().Dynamic.GetErrorIconBounds(g, dataGridViewCellStyle, 0);

        bounds.Should().Be(Rectangle.Empty);
    }

    [Fact]
    public void GetPreferredSize_ReturnsMinusOne_WhenDataGridViewIsNull()
    {
        using DataGridViewImageCell dataGridViewImageCell = _dataGridViewImageCell;
        using Graphics g = Graphics.FromImage(new Bitmap(10, 10));
        DataGridViewCellStyle dataGridViewCellStyle = new();
        Size size = (Size)dataGridViewImageCell.TestAccessor().Dynamic.GetPreferredSize(g, dataGridViewCellStyle, 0, new Size(100, 100));

        size.Should().Be(new Size(-1, -1));
    }

    [Fact]
    public void GetPreferredSize_ThrowsNullReferenceException_WhenCellStyleIsNull()
    {
        using DataGridViewImageCell dataGridViewImageCell = _dataGridViewImageCell;
        using Graphics g = Graphics.FromImage(new Bitmap(10, 10));
        Action action = () => dataGridViewImageCell.TestAccessor().Dynamic.GetPreferredSize(g, null, 0, new Size(100, 100));

        action.Should().Throw<NullReferenceException>();
    }

    [WinFormsFact]
    public void Paint_DoesNotThrow_WithValidArguments_UsingTestAccessor()
    {
        using DataGridView dataGridView = new();
        using DataGridViewImageColumn dataGridViewImageColumn = new();
        dataGridView.Columns.Add(dataGridViewImageColumn);
        dataGridView.Rows.Add();
        using DataGridViewImageCell dataGridViewImageCell = _dataGridViewImageCell;
        dataGridView.Rows[0].Cells[0] = dataGridViewImageCell;
        using Graphics g = Graphics.FromImage(new Bitmap(10, 10));
        DataGridViewCellStyle dataGridViewCellStyle = new();
        DataGridViewAdvancedBorderStyle borderStyle = new();

        Action action = () => dataGridViewImageCell.TestAccessor().Dynamic.Paint(
        g,
        new Rectangle(0, 0, 10, 10),
        new Rectangle(0, 0, 10, 10),
        0,
        DataGridViewElementStates.None,
        null,
        null,
        null,
        dataGridViewCellStyle,
        borderStyle,
        DataGridViewPaintParts.All);

        action.Should().NotThrow();
    }

    [WinFormsFact]
    public void ColumnIndex_ReturnsExpectedFormat()
    {
        using DataGridView dataGridView = new();
        dataGridView.Columns.Add(new DataGridViewImageColumn());
        dataGridView.Rows.Add();
        using DataGridViewImageCell dataGridViewImageCell = _dataGridViewImageCell;
        dataGridView.Rows[0].Cells[0] = dataGridViewImageCell;

        dataGridViewImageCell.ColumnIndex.Should().Be(0);
    }
}
