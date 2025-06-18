// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;

namespace System.Windows.Forms.Tests;

public class DataGridViewImageCellTests : IDisposable
{
    public DataGridViewImageCell dataGridViewImageCell => new();

    public void Dispose() => dataGridViewImageCell.Dispose();

    [Fact]
    public void Ctor_Default_SetsValueIsIconFalse() =>
        dataGridViewImageCell.ValueIsIcon.Should().BeFalse();

    [Theory]
    [BoolData]
    public void Ctor_ValueIsIcon_SetsValueIsIcon(bool valueIsIcon)
    {
        using DataGridViewImageCell cell = new(valueIsIcon);

        cell.ValueIsIcon.Should().Be(valueIsIcon);
    }

    [Fact]
    public void DefaultNewRowValue_ReturnsErrorBitmap_WhenValueTypeIsImage()
    {
        dataGridViewImageCell.ValueType = typeof(Image);
        var value = dataGridViewImageCell.DefaultNewRowValue;

        value.Should().BeSameAs(DataGridViewImageCell.ErrorBitmap);
    }

    [Fact]
    public void DefaultNewRowValue_ReturnsNull_WhenValueTypeIsOther()
    {
        dataGridViewImageCell.ValueType = typeof(string);
        dataGridViewImageCell.ValueIsIcon = false;
        var value = dataGridViewImageCell.DefaultNewRowValue;

        value.Should().BeSameAs(DataGridViewImageCell.ErrorBitmap);
    }

    [Fact]
    public void EditType_IsAlwaysNull()
    {
        dataGridViewImageCell.EditType.Should().BeNull();

        using DataGridViewImageCell cellIcon = new(true);
        cellIcon.EditType.Should().BeNull();
    }

    [Fact]
    public void FormattedValueType_ReturnsImage_WhenValueIsIconFalse()
    {
        dataGridViewImageCell.ValueIsIcon = false;
        dataGridViewImageCell.FormattedValueType.Should().Be(typeof(Image));
    }

    [Fact]
    public void ValueIsIcon_SetFalse_UpdatesFlag()
    {
        dataGridViewImageCell.ValueIsIcon = true;
        dataGridViewImageCell.ValueIsIcon = false;
        dataGridViewImageCell.ValueIsIcon.Should().BeFalse();
    }

    [Fact]
    public void ValueType_GetSet_RoundTrips()
    {
        dataGridViewImageCell.ValueType = typeof(Image);
        dataGridViewImageCell.ValueType.Should().Be(typeof(Image));

        dataGridViewImageCell.ValueType = null;
        dataGridViewImageCell.ValueType.Should().Be(typeof(Image));
    }

    [Fact]
    public void ValueType_SetToNull_ResetsValueIsIconToDefault()
    {
        dataGridViewImageCell.ValueType = typeof(Icon);
        dataGridViewImageCell.ValueIsIcon.Should().BeFalse();

        dataGridViewImageCell.ValueType = null;
        dataGridViewImageCell.ValueIsIcon.Should().BeFalse();
    }

    [Fact]
    public void GetContentBounds_ReturnsEmpty_WhenDataGridViewIsNull()
    {
        using Graphics g = Graphics.FromImage(new Bitmap(10, 10));
        DataGridViewCellStyle style = new();
        Rectangle bounds = (Rectangle)dataGridViewImageCell.TestAccessor().Dynamic.GetContentBounds(g, style, 0);

        bounds.Should().Be(Rectangle.Empty);
    }

    [Fact]
    public void GetContentBounds_ReturnsEmpty_WhenRowIndexNegative()
    {
        using DataGridView dataGridView = new();
        dataGridViewImageCell.DataGridView = dataGridView;
        using Graphics g = Graphics.FromImage(new Bitmap(10, 10));
        DataGridViewCellStyle style = new();
        Rectangle bounds = (Rectangle)dataGridViewImageCell.TestAccessor().Dynamic.GetContentBounds(g, style, -1);

        bounds.Should().Be(Rectangle.Empty);
    }

    [Fact]
    public void GetContentBounds_ReturnsEmpty_WhenOwningColumnIsNull()
    {
        using DataGridView dataGridView = new();
        dataGridView.Columns.Add(new DataGridViewImageColumn());
        dataGridView.Rows.Add();
        dataGridViewImageCell.DataGridView = dataGridView;
        using Graphics g = Graphics.FromImage(new Bitmap(10, 10));
        DataGridViewCellStyle style = new();
        Rectangle bounds = (Rectangle)dataGridViewImageCell.TestAccessor().Dynamic.GetContentBounds(g, style, 0);

        bounds.Should().Be(Rectangle.Empty);
    }

    [Fact]
    public void GetErrorIconBounds_ReturnsEmpty_WhenDataGridViewIsNull()
    {
        using Graphics g = Graphics.FromImage(new Bitmap(10, 10));
        DataGridViewCellStyle style = new();
        Rectangle bounds = (Rectangle)dataGridViewImageCell.TestAccessor().Dynamic.GetErrorIconBounds(g, style, 0);

        bounds.Should().Be(Rectangle.Empty);
    }

    [Fact]
    public void GetErrorIconBounds_ReturnsEmpty_WhenRowIndexNegative()
    {
        using DataGridView dataGridView = new();
        dataGridView.Columns.Add(new DataGridViewImageColumn());
        dataGridView.Rows.Add();
        dataGridViewImageCell.DataGridView = dataGridView;
        using Graphics g = Graphics.FromImage(new Bitmap(10, 10));
        DataGridViewCellStyle style = new();
        Rectangle bounds = (Rectangle)dataGridViewImageCell.TestAccessor().Dynamic.GetErrorIconBounds(g, style, -1);

        bounds.Should().Be(Rectangle.Empty);
    }

    [Fact]
    public void GetErrorIconBounds_ReturnsEmpty_WhenOwningColumnIsNull()
    {
        using DataGridView dataGridView = new();
        dataGridView.Columns.Add(new DataGridViewImageColumn());
        dataGridView.Rows.Add();
        dataGridViewImageCell.DataGridView = dataGridView;
        using Graphics g = Graphics.FromImage(new Bitmap(10, 10));
        DataGridViewCellStyle style = new();
        Rectangle bounds = (Rectangle)dataGridViewImageCell.TestAccessor().Dynamic.GetErrorIconBounds(g, style, 0);

        bounds.Should().Be(Rectangle.Empty);
    }

    [Fact]
    public void GetErrorIconBounds_ReturnsEmpty_WhenShowCellErrorsIsFalse()
    {
        using DataGridView dataGridView = new();
        DataGridViewImageColumn col = new();
        dataGridView.Columns.Add(col);
        dataGridView.Rows.Add();
        dataGridView.ShowCellErrors = false;
        dataGridViewImageCell.DataGridView = dataGridView;
        using Graphics g = Graphics.FromImage(new Bitmap(10, 10));
        DataGridViewCellStyle style = new();
        Rectangle bounds = (Rectangle)dataGridViewImageCell.TestAccessor().Dynamic.GetErrorIconBounds(g, style, 0);

        bounds.Should().Be(Rectangle.Empty);
    }

    [Fact]
    public void GetErrorIconBounds_ReturnsEmpty_WhenErrorTextIsNullOrEmpty()
    {
        using DataGridView dataGridView = new();
        DataGridViewImageColumn col = new();
        dataGridView.Columns.Add(col);
        dataGridView.Rows.Add();
        dataGridView.ShowCellErrors = true;
        dataGridViewImageCell.DataGridView = dataGridView;
        using Graphics g = Graphics.FromImage(new Bitmap(10, 10));
        DataGridViewCellStyle style = new();
        Rectangle bounds = (Rectangle)dataGridViewImageCell.TestAccessor().Dynamic.GetErrorIconBounds(g, style, 0);

        bounds.Should().Be(Rectangle.Empty);
    }

    [Fact]
    public void GetPreferredSize_ReturnsMinusOne_WhenDataGridViewIsNull()
    {
        DataGridViewImageCell cell = dataGridViewImageCell;
        using Graphics g = Graphics.FromImage(new Bitmap(10, 10));
        DataGridViewCellStyle style = new();
        Size size = (Size)cell.TestAccessor().Dynamic.GetPreferredSize(g, style, 0, new Size(100, 100));

        size.Should().Be(new Size(-1, -1));
    }

    [Fact]
    public void GetPreferredSize_ThrowsNullReferenceException_WhenCellStyleIsNull()
    {
        DataGridViewImageCell cell = dataGridViewImageCell;
        using Graphics g = Graphics.FromImage(new Bitmap(10, 10));
        Action action = () => cell.TestAccessor().Dynamic.GetPreferredSize(g, null, 0, new Size(100, 100));

        action.Should().Throw<NullReferenceException>();
    }

    [Fact]
    public void Paint_DoesNotThrow_WithValidArguments_UsingTestAccessor()
    {
        using DataGridView dataGridView = new();
        DataGridViewImageColumn col = new();
        dataGridView.Columns.Add(col);
        dataGridView.Rows.Add();
        DataGridViewImageCell cell = dataGridViewImageCell;
        dataGridView.Rows[0].Cells[0] = cell;
        using Graphics g = Graphics.FromImage(new Bitmap(10, 10));
        DataGridViewCellStyle style = new();
        DataGridViewAdvancedBorderStyle borderStyle = new();

        Action action = () => cell.TestAccessor().Dynamic.Paint(
        g,
        new Rectangle(0, 0, 10, 10),
        new Rectangle(0, 0, 10, 10),
        0,
        DataGridViewElementStates.None,
        null,
        null,
        null,
        style,
        borderStyle,
        DataGridViewPaintParts.All);

        action.Should().NotThrow();
    }

    [Fact]
    public void ToString_ReturnsExpectedFormat()
    {
        using DataGridView dataGridView = new();
        dataGridView.Columns.Add(new DataGridViewImageColumn());
        dataGridView.Rows.Add();
        DataGridViewImageCell cell = dataGridViewImageCell;
        dataGridView.Rows[0].Cells[0] = cell;

        cell.ColumnIndex.Should().Be(0);
    }
}
