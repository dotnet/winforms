// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;

namespace System.Windows.Forms.Tests;

public class DataGridViewRowPostPaintEventArgsTests
{
    public static IEnumerable<object[]> Ctor_Rectangle_Rectangle_Int_DataGridViewElementStates_String_DataGridViewCellStyle_Bool_Bool_TestData()
    {
        yield return new object[] { Rectangle.Empty, Rectangle.Empty, -2, DataGridViewElementStates.Displayed, null, new DataGridViewCellStyle(), false, false };
        yield return new object[] { new Rectangle(-1, -2, -3, -4), new Rectangle(-1, -2, -3, -4), -1, DataGridViewElementStates.Displayed, "", new DataGridViewCellStyle(), true, false };
        yield return new object[] { new Rectangle(1, 2, 3, 4), new Rectangle(2, 3, 4, 5), 0, (DataGridViewElementStates)7, "errorText", new DataGridViewCellStyle(), true, true };
    }

    [WinFormsTheory]
    [MemberData(nameof(Ctor_Rectangle_Rectangle_Int_DataGridViewElementStates_String_DataGridViewCellStyle_Bool_Bool_TestData))]
    public void Ctor_DataGridView_Graphics_Rectangle_Rectangle_Int_DataGridViewElementStates_String_DataGridViewCellStyle_Bool_Bool(Rectangle clipBounds, Rectangle rowBounds, int rowIndex, DataGridViewElementStates rowState, string errorText, DataGridViewCellStyle inheritedRowStyle, bool isFirstDisplayedRow, bool isLastVisibleRow)
    {
        using Bitmap image = new(10, 10);
        using Graphics graphics = Graphics.FromImage(image);
        using DataGridView dataGridView = new();

        DataGridViewRowPostPaintEventArgs e = new(dataGridView, graphics, clipBounds, rowBounds, rowIndex, rowState, errorText, inheritedRowStyle, isFirstDisplayedRow, isLastVisibleRow);
        Assert.Equal(graphics, e.Graphics);
        Assert.Equal(clipBounds, e.ClipBounds);
        Assert.Equal(rowBounds, e.RowBounds);
        Assert.Equal(rowIndex, e.RowIndex);
        Assert.Equal(rowState, e.State);
        Assert.Equal(errorText, e.ErrorText);
        Assert.Equal(inheritedRowStyle, e.InheritedRowStyle);
        Assert.Equal(isFirstDisplayedRow, e.IsFirstDisplayedRow);
        Assert.Equal(isLastVisibleRow, e.IsLastVisibleRow);
    }

    [WinFormsFact]
    public void Ctor_NullDataGridView_ThrowsArgumentNullException()
    {
        using Bitmap image = new(10, 10);
        using Graphics graphics = Graphics.FromImage(image);
        Assert.Throws<ArgumentNullException>("dataGridView", () => new DataGridViewRowPostPaintEventArgs(null, graphics, Rectangle.Empty, Rectangle.Empty, -2, DataGridViewElementStates.Displayed, null, new DataGridViewCellStyle(), false, false));
    }

    [WinFormsFact]
    public void Ctor_NullGraphics_ThrowsArgumentNullException()
    {
        using DataGridView dataGridView = new();
        Assert.Throws<ArgumentNullException>("graphics", () => new DataGridViewRowPostPaintEventArgs(dataGridView, null, Rectangle.Empty, Rectangle.Empty, -2, DataGridViewElementStates.Displayed, null, new DataGridViewCellStyle(), false, false));
    }

    [WinFormsFact]
    public void Ctor_NullInheritedRowStyle_ThrowsArgumentNullException()
    {
        using Bitmap image = new(10, 10);
        using Graphics graphics = Graphics.FromImage(image);
        using DataGridView dataGridView = new();
        Assert.Throws<ArgumentNullException>("inheritedRowStyle", () => new DataGridViewRowPostPaintEventArgs(dataGridView, graphics, Rectangle.Empty, Rectangle.Empty, -2, DataGridViewElementStates.Displayed, null, null, false, false));
    }

    public static IEnumerable<object[]> ClipBounds_TestData()
    {
        yield return new object[] { Rectangle.Empty };
        yield return new object[] { new Rectangle(1, 2, 3, 4) };
        yield return new object[] { new Rectangle(-1, -2, -3, -4) };
    }

    [WinFormsTheory]
    [MemberData(nameof(ClipBounds_TestData))]
    public void ClipBounds_Set_GetReturnsExpected(Rectangle value)
    {
        using Bitmap image = new(10, 10);
        using Graphics graphics = Graphics.FromImage(image);
        using DataGridView dataGridView = new();
        DataGridViewRowPostPaintEventArgs e = new(dataGridView, graphics, Rectangle.Empty, Rectangle.Empty, -2, DataGridViewElementStates.Displayed, null, new DataGridViewCellStyle(), false, false)
        {
            ClipBounds = value
        };
        Assert.Equal(value, e.ClipBounds);
    }

    [WinFormsFact]
    public void DrawFocus_ValidRowIndex_Success()
    {
        using Bitmap image = new(10, 10);
        using Graphics graphics = Graphics.FromImage(image);
        using DataGridView dataGridView = new();
        dataGridView.Columns.Add("name", "text");
        DataGridViewRowPostPaintEventArgs e = new(dataGridView, graphics, Rectangle.Empty, Rectangle.Empty, 0, DataGridViewElementStates.Displayed, null, new DataGridViewCellStyle(), false, false);
        e.DrawFocus(new Rectangle(1, 2, 3, 4), true);
    }

    [WinFormsTheory]
    [InlineData(-1)]
    [InlineData(1)]
    public void DrawFocus_InvalidRowIndex_ThrowsInvalidOperationException(int rowIndex)
    {
        using Bitmap image = new(10, 10);
        using Graphics graphics = Graphics.FromImage(image);
        using DataGridView dataGridView = new();
        dataGridView.Columns.Add("name", "text");
        DataGridViewRowPostPaintEventArgs e = new(dataGridView, graphics, Rectangle.Empty, Rectangle.Empty, rowIndex, DataGridViewElementStates.Displayed, null, new DataGridViewCellStyle(), false, false);
        Assert.Throws<InvalidOperationException>(() => e.DrawFocus(new Rectangle(1, 2, 3, 4), true));
    }

    [WinFormsFact]
    public void PaintCells_ValidRowIndex_Success()
    {
        using Bitmap image = new(10, 10);
        using Graphics graphics = Graphics.FromImage(image);
        using DataGridView dataGridView = new();
        dataGridView.Columns.Add("name", "text");
        DataGridViewRowPostPaintEventArgs e = new(dataGridView, graphics, Rectangle.Empty, Rectangle.Empty, 0, DataGridViewElementStates.Displayed, null, new DataGridViewCellStyle(), false, false);
        e.PaintCells(new Rectangle(1, 2, 3, 4), DataGridViewPaintParts.None);
    }

    [WinFormsTheory]
    [InlineData(-1)]
    [InlineData(1)]
    public void PaintCells_InvalidRowIndex_ThrowsInvalidOperationException(int rowIndex)
    {
        using Bitmap image = new(10, 10);
        using Graphics graphics = Graphics.FromImage(image);
        using DataGridView dataGridView = new();
        dataGridView.Columns.Add("name", "text");
        DataGridViewRowPostPaintEventArgs e = new(dataGridView, graphics, Rectangle.Empty, Rectangle.Empty, rowIndex, DataGridViewElementStates.Displayed, null, new DataGridViewCellStyle(), false, false);
        Assert.Throws<InvalidOperationException>(() => e.PaintCells(new Rectangle(1, 2, 3, 4), DataGridViewPaintParts.None));
    }

    [WinFormsFact]
    public void PaintCellsBackground_ValidRowIndex_Success()
    {
        using Bitmap image = new(10, 10);
        using Graphics graphics = Graphics.FromImage(image);
        using DataGridView dataGridView = new();
        dataGridView.Columns.Add("name", "text");
        DataGridViewRowPostPaintEventArgs e = new(dataGridView, graphics, Rectangle.Empty, Rectangle.Empty, 0, DataGridViewElementStates.Displayed, null, new DataGridViewCellStyle(), false, false);
        e.PaintCellsBackground(new Rectangle(1, 2, 3, 4), true);
    }

    [WinFormsTheory]
    [InlineData(-1)]
    [InlineData(1)]
    public void PaintCellsBackground_InvalidRowIndex_ThrowsInvalidOperationException(int rowIndex)
    {
        using Bitmap image = new(10, 10);
        using Graphics graphics = Graphics.FromImage(image);
        using DataGridView dataGridView = new();
        dataGridView.Columns.Add("name", "text");
        DataGridViewRowPostPaintEventArgs e = new(dataGridView, graphics, Rectangle.Empty, Rectangle.Empty, rowIndex, DataGridViewElementStates.Displayed, null, new DataGridViewCellStyle(), false, false);
        Assert.Throws<InvalidOperationException>(() => e.PaintCellsBackground(new Rectangle(1, 2, 3, 4), true));
    }

    [WinFormsFact]
    public void PaintCellsContent_ValidRowIndex_Success()
    {
        using Bitmap image = new(10, 10);
        using Graphics graphics = Graphics.FromImage(image);
        using DataGridView dataGridView = new();
        dataGridView.Columns.Add("name", "text");
        DataGridViewRowPostPaintEventArgs e = new(dataGridView, graphics, Rectangle.Empty, Rectangle.Empty, 0, DataGridViewElementStates.Displayed, null, new DataGridViewCellStyle(), false, false);
        e.PaintCellsContent(new Rectangle(1, 2, 3, 4));
    }

    [WinFormsTheory]
    [InlineData(-1)]
    [InlineData(1)]
    public void PaintCellsContent_InvalidRowIndex_ThrowsInvalidOperationException(int rowIndex)
    {
        using Bitmap image = new(10, 10);
        using Graphics graphics = Graphics.FromImage(image);
        using DataGridView dataGridView = new();
        dataGridView.Columns.Add("name", "text");
        DataGridViewRowPostPaintEventArgs e = new(dataGridView, graphics, Rectangle.Empty, Rectangle.Empty, rowIndex, DataGridViewElementStates.Displayed, null, new DataGridViewCellStyle(), false, false);
        Assert.Throws<InvalidOperationException>(() => e.PaintCellsContent(new Rectangle(1, 2, 3, 4)));
    }

    [WinFormsFact]
    public void PaintHeader_ValidRowIndexDataGridViewPaintParts_Success()
    {
        using Bitmap image = new(10, 10);
        using Graphics graphics = Graphics.FromImage(image);
        using DataGridView dataGridView = new();
        dataGridView.Columns.Add("name", "text");
        DataGridViewRowPostPaintEventArgs e = new(dataGridView, graphics, Rectangle.Empty, Rectangle.Empty, 0, DataGridViewElementStates.Displayed, null, new DataGridViewCellStyle(), false, false);
        e.PaintHeader(DataGridViewPaintParts.None);
    }

    [WinFormsTheory]
    [InlineData(true)]
    [InlineData(false)]
    public void PaintHeader_ValidRowIndexBool_Success(bool paintSelectionBackground)
    {
        using Bitmap image = new(10, 10);
        using Graphics graphics = Graphics.FromImage(image);
        using DataGridView dataGridView = new();
        dataGridView.Columns.Add("name", "text");
        DataGridViewRowPostPaintEventArgs e = new(dataGridView, graphics, Rectangle.Empty, Rectangle.Empty, 0, DataGridViewElementStates.Displayed, null, new DataGridViewCellStyle(), false, false);
        e.PaintHeader(paintSelectionBackground);
    }

    [WinFormsTheory]
    [InlineData(-1)]
    [InlineData(1)]
    public void PaintHeader_InvalidRowIndex_ThrowsInvalidOperationException(int rowIndex)
    {
        using Bitmap image = new(10, 10);
        using Graphics graphics = Graphics.FromImage(image);
        using DataGridView dataGridView = new();
        dataGridView.Columns.Add("name", "text");
        DataGridViewRowPostPaintEventArgs e = new(dataGridView, graphics, Rectangle.Empty, Rectangle.Empty, rowIndex, DataGridViewElementStates.Displayed, null, new DataGridViewCellStyle(), false, false);
        Assert.Throws<InvalidOperationException>(() => e.PaintHeader(DataGridViewPaintParts.None));
        Assert.Throws<InvalidOperationException>(() => e.PaintHeader(true));
    }
}
