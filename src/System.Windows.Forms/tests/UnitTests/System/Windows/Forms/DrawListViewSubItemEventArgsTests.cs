// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;

namespace System.Windows.Forms.Tests;

public class DrawListViewSubItemEventArgsTests
{
    public static IEnumerable<object[]> Ctor_Graphics_ListViewItem_Rectangle_Int_ListViewItemStates_TestData()
    {
        yield return new object[] { Rectangle.Empty, null, new ListViewItem.ListViewSubItem(), -2, -2, null, ListViewItemStates.Checked - 1 };
        yield return new object[] { Rectangle.Empty, new ListViewItem(), new ListViewItem.ListViewSubItem(), -2, -2, null, ListViewItemStates.Checked - 1 };
        yield return new object[] { new Rectangle(1, 2, 3, 4), new ListViewItem(), null, -1, -1, new ColumnHeader(), ListViewItemStates.Checked };
        yield return new object[] { new Rectangle(1, 2, 3, 4), new ListViewItem(), new ListViewItem.ListViewSubItem(), -1, -1, new ColumnHeader(), ListViewItemStates.Checked };
        yield return new object[] { new Rectangle(-1, 2, -3, -4), new ListViewItem(), new ListViewItem.ListViewSubItem(), 0, 0, new ColumnHeader(), ListViewItemStates.Focused };
        yield return new object[] { new Rectangle(1, 2, 3, 4), new ListViewItem(), new ListViewItem.ListViewSubItem(), 1, 2, new ColumnHeader(), ListViewItemStates.Checked };
    }

    [WinFormsTheory]
    [MemberData(nameof(Ctor_Graphics_ListViewItem_Rectangle_Int_ListViewItemStates_TestData))]
    public void DrawListViewSubItemEventArgs_Ctor_Graphics_ListViewItem_Rectangle_Int_ListViewItemStates(Rectangle bounds, ListViewItem item, ListViewItem.ListViewSubItem subItem, int itemIndex, int columnIndex, ColumnHeader header, ListViewItemStates itemState)
    {
        using Bitmap image = new(10, 10);
        using Graphics graphics = Graphics.FromImage(image);
        DrawListViewSubItemEventArgs e = new(graphics, bounds, item, subItem, itemIndex, columnIndex, header, itemState);
        Assert.Same(graphics, e.Graphics);
        Assert.Equal(bounds, e.Bounds);
        Assert.Same(item, e.Item);
        Assert.Same(subItem, e.SubItem);
        Assert.Equal(itemIndex, e.ItemIndex);
        Assert.Equal(columnIndex, e.ColumnIndex);
        Assert.Same(header, e.Header);
        Assert.Equal(itemState, e.ItemState);
        Assert.False(e.DrawDefault);
    }

    [WinFormsFact]
    public void DrawListViewSubItemEventArgs_Ctor_NullGraphics_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>("graphics", () => new DrawListViewSubItemEventArgs(null, new Rectangle(1, 2, 3, 4), new ListViewItem(), new ListViewItem.ListViewSubItem(), -1, 0, new ColumnHeader(), ListViewItemStates.Default));
    }

    [WinFormsFact]
    public void DrawListViewSubItemEventArgs_Ctor_NullItemIndexNegativeOne_ThrowsArgumentNullException()
    {
        using Bitmap image = new(10, 10);
        using Graphics graphics = Graphics.FromImage(image);
        Assert.Throws<ArgumentNullException>("item", () => new DrawListViewSubItemEventArgs(graphics, new Rectangle(1, 2, 3, 4), null, new ListViewItem.ListViewSubItem(), -1, 0, new ColumnHeader(), ListViewItemStates.Default));
    }

    [WinFormsTheory]
    [InlineData(-2)]
    [InlineData(0)]
    public void DrawListViewSubItemEventArgs_Ctor_NullSubItemIndexNotNegativeOne_ThrowsArgumentNullException(int itemIndex)
    {
        using Bitmap image = new(10, 10);
        using Graphics graphics = Graphics.FromImage(image);
        Assert.Throws<ArgumentNullException>("subItem", () => new DrawListViewSubItemEventArgs(graphics, new Rectangle(1, 2, 3, 4), new ListViewItem(), null, itemIndex, 0, new ColumnHeader(), ListViewItemStates.Default));
    }

    [WinFormsTheory]
    [BoolData]
    public void DrawListViewSubItemEventArgs_DrawDefault_Set_GetReturnsExpected(bool value)
    {
        using Bitmap image = new(10, 10);
        using Graphics graphics = Graphics.FromImage(image);
        DrawListViewSubItemEventArgs e = new(graphics, new Rectangle(1, 2, 3, 4), new ListViewItem(), null, -1, -1, new ColumnHeader(), ListViewItemStates.Checked)
        {
            DrawDefault = value
        };
        Assert.Equal(value, e.DrawDefault);

        // Set same.
        e.DrawDefault = value;
        Assert.Equal(value, e.DrawDefault);

        // Set different.
        e.DrawDefault = !value;
        Assert.Equal(!value, e.DrawDefault);
    }

    public static IEnumerable<object[]> Draw_TestData()
    {
        yield return new object[] { new Rectangle(-1, -2, -3, -4), new ListViewItem(), null, -1, null, ListViewItemStates.Default };
        yield return new object[] { new Rectangle(-1, -2, -3, -4), new ListViewItem(), null, -1, null, ListViewItemStates.Checked };
        yield return new object[] { new Rectangle(-1, -2, -3, -4), new ListViewItem(), null, -1, null, ListViewItemStates.Focused };
        yield return new object[] { new Rectangle(-1, -2, -3, -4), new ListViewItem(), null, -1, new ColumnHeader(), ListViewItemStates.Default };
        yield return new object[] { new Rectangle(-1, -2, -3, -4), new ListViewItem(), null, -1, new ColumnHeader(), ListViewItemStates.Checked };
        yield return new object[] { new Rectangle(-1, -2, -3, -4), new ListViewItem(), null, -1, new ColumnHeader(), ListViewItemStates.Focused };
        yield return new object[] { new Rectangle(1, 2, 3, 4), new ListViewItem(), null, -1, null, ListViewItemStates.Default };
        yield return new object[] { new Rectangle(1, 2, 3, 4), new ListViewItem(), null, -1, null, ListViewItemStates.Checked };
        yield return new object[] { new Rectangle(1, 2, 3, 4), new ListViewItem(), null, -1, null, ListViewItemStates.Focused };
        yield return new object[] { new Rectangle(1, 2, 3, 4), new ListViewItem(), null, -1, new ColumnHeader(), ListViewItemStates.Default };
        yield return new object[] { new Rectangle(1, 2, 3, 4), new ListViewItem(), null, -1, new ColumnHeader(), ListViewItemStates.Checked };
        yield return new object[] { new Rectangle(1, 2, 3, 4), new ListViewItem(), null, -1, new ColumnHeader(), ListViewItemStates.Focused };

        yield return new object[] { new Rectangle(-1, -2, -3, -4), null, new ListViewItem.ListViewSubItem(), 0, null, ListViewItemStates.Default };
        yield return new object[] { new Rectangle(-1, -2, -3, -4), null, new ListViewItem.ListViewSubItem(), 0, null, ListViewItemStates.Checked };
        yield return new object[] { new Rectangle(-1, -2, -3, -4), null, new ListViewItem.ListViewSubItem(), 0, null, ListViewItemStates.Focused };
        yield return new object[] { new Rectangle(-1, -2, -3, -4), null, new ListViewItem.ListViewSubItem(), 0, new ColumnHeader(), ListViewItemStates.Default };
        yield return new object[] { new Rectangle(-1, -2, -3, -4), null, new ListViewItem.ListViewSubItem(), 0, new ColumnHeader(), ListViewItemStates.Checked };
        yield return new object[] { new Rectangle(-1, -2, -3, -4), null, new ListViewItem.ListViewSubItem(), 0, new ColumnHeader(), ListViewItemStates.Focused };
        yield return new object[] { new Rectangle(1, 2, 3, 4), null, new ListViewItem.ListViewSubItem(), 0, null, ListViewItemStates.Default };
        yield return new object[] { new Rectangle(1, 2, 3, 4), null, new ListViewItem.ListViewSubItem(), 0, null, ListViewItemStates.Checked };
        yield return new object[] { new Rectangle(1, 2, 3, 4), null, new ListViewItem.ListViewSubItem(), 0, null, ListViewItemStates.Focused };
        yield return new object[] { new Rectangle(1, 2, 3, 4), null, new ListViewItem.ListViewSubItem(), 0, new ColumnHeader(), ListViewItemStates.Default };
        yield return new object[] { new Rectangle(1, 2, 3, 4), null, new ListViewItem.ListViewSubItem(), 0, new ColumnHeader(), ListViewItemStates.Checked };
        yield return new object[] { new Rectangle(1, 2, 3, 4), null, new ListViewItem.ListViewSubItem(), 0, new ColumnHeader(), ListViewItemStates.Focused };
    }

    [WinFormsTheory]
    [MemberData(nameof(Draw_TestData))]
    public void DrawListViewSubItemEventArgs_DrawBackground_HasGraphics_Success(Rectangle bounds, ListViewItem item, ListViewItem.ListViewSubItem subItem, int itemIndex, ColumnHeader header, ListViewItemStates itemState)
    {
        using Bitmap image = new(10, 10);
        using Graphics graphics = Graphics.FromImage(image);
        DrawListViewSubItemEventArgs e = new(graphics, bounds, item, subItem, itemIndex, 0, header, itemState);
        e.DrawBackground();
    }

    [WinFormsTheory]
    [MemberData(nameof(Draw_TestData))]
    public void DrawListViewSubItemEventArgs_DrawFocusRectangle_HasGraphics_Success(Rectangle bounds, ListViewItem item, ListViewItem.ListViewSubItem subItem, int itemIndex, ColumnHeader header, ListViewItemStates itemState)
    {
        using Bitmap image = new(10, 10);
        using Graphics graphics = Graphics.FromImage(image);
        DrawListViewSubItemEventArgs e = new(graphics, bounds, item, subItem, itemIndex, 0, header, itemState);
        e.DrawFocusRectangle(new Rectangle(1, 2, 3, 4));
    }
}
