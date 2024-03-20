// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;

namespace System.Windows.Forms.Tests;

// NB: doesn't require thread affinity
public class DrawListViewItemEventArgsTests
{
    public static IEnumerable<object[]> Ctor_Graphics_ListViewItem_Rectangle_Int_ListViewItemStates_TestData()
    {
        yield return new object[] { Rectangle.Empty, -2, ListViewItemStates.Checked - 1 };
        yield return new object[] { new Rectangle(1, 2, 3, 4), -1, ListViewItemStates.Checked };
        yield return new object[] { new Rectangle(-1, 2, -3, -4), 0, ListViewItemStates.Focused };
        yield return new object[] { new Rectangle(1, 2, 3, 4), 1, ListViewItemStates.Checked };
    }

    [Theory]
    [MemberData(nameof(Ctor_Graphics_ListViewItem_Rectangle_Int_ListViewItemStates_TestData))]
    public void DrawListViewItemEventArgs_Ctor_Graphics_ListViewItem_Rectangle_Int_ListViewItemStates(Rectangle bounds, int itemIndex, ListViewItemStates state)
    {
        using Bitmap image = new(10, 10);
        using Graphics graphics = Graphics.FromImage(image);
        ListViewItem item = new();
        DrawListViewItemEventArgs e = new(graphics, item, bounds, itemIndex, state);
        Assert.Same(graphics, e.Graphics);
        Assert.Same(item, e.Item);
        Assert.Equal(bounds, e.Bounds);
        Assert.Equal(itemIndex, e.ItemIndex);
        Assert.Equal(state, e.State);
        Assert.False(e.DrawDefault);
    }

    [Fact]
    public void DrawListViewItemEventArgs_Ctor_NullGraphics_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>("graphics", () => new DrawListViewItemEventArgs(null, new ListViewItem(), new Rectangle(1, 2, 3, 4), 0, ListViewItemStates.Default));
    }

    [Fact]
    public void DrawListViewItemEventArgs_Ctor_NullItem_ThrowsArgumentNullException()
    {
        using Bitmap image = new(10, 10);
        using Graphics graphics = Graphics.FromImage(image);
        Assert.Throws<ArgumentNullException>("item", () => new DrawListViewItemEventArgs(graphics, null, new Rectangle(1, 2, 3, 4), 0, ListViewItemStates.Default));
    }

    [Theory]
    [BoolData]
    public void DrawListViewItemEventArgs_DrawDefault_Set_GetReturnsExpected(bool value)
    {
        using Bitmap image = new(10, 10);
        using Graphics graphics = Graphics.FromImage(image);
        DrawListViewItemEventArgs e = new(graphics, new ListViewItem(), new Rectangle(1, 2, 3, 4), -1, ListViewItemStates.Checked)
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
        yield return new object[] { new ListViewItem(), new Rectangle(1, 2, 3, 4), ListViewItemStates.Default };
        yield return new object[] { new ListViewItem(), new Rectangle(1, 2, 3, 4), ListViewItemStates.Focused };
        yield return new object[] { new ListViewItem(), new Rectangle(1, 2, 3, 4), ListViewItemStates.Checked };
        yield return new object[] { new ListViewItem(), Rectangle.Empty, ListViewItemStates.Default };
        yield return new object[] { new ListViewItem(), Rectangle.Empty, ListViewItemStates.Focused };
        yield return new object[] { new ListViewItem(), Rectangle.Empty, ListViewItemStates.Checked };
        yield return new object[] { new ListViewItem(), new Rectangle(-1, -2, -3, -4), ListViewItemStates.Default };
        yield return new object[] { new ListViewItem(), new Rectangle(-1, -2, -3, -4), ListViewItemStates.Focused };
        yield return new object[] { new ListViewItem(), new Rectangle(-1, -2, -3, -4), ListViewItemStates.Checked };

        foreach (View view in new View[] { View.Details, View.List })
        {
            ListView listView = new() { View = view };
            ListViewItem listViewItem = new();
            listView.Items.Add(listViewItem);
            yield return new object[] { listViewItem, new Rectangle(1, 2, 3, 4), ListViewItemStates.Default };
            yield return new object[] { listViewItem, new Rectangle(1, 2, 3, 4), ListViewItemStates.Focused };
            yield return new object[] { listViewItem, new Rectangle(1, 2, 3, 4), ListViewItemStates.Checked };

            ListViewItem subItemsItem = new();
            subItemsItem.SubItems.Add(new ListViewItem.ListViewSubItem());
            listView.Items.Add(subItemsItem);
            yield return new object[] { subItemsItem, new Rectangle(1, 2, 3, 4), ListViewItemStates.Default };
            yield return new object[] { subItemsItem, new Rectangle(1, 2, 3, 4), ListViewItemStates.Focused };
            yield return new object[] { subItemsItem, new Rectangle(1, 2, 3, 4), ListViewItemStates.Checked };

            ListView fullRowSelectListView = new() { View = view, FullRowSelect = true };
            ListViewItem fullRowSelectListViewItem = new();
            fullRowSelectListViewItem.SubItems.Add(new ListViewItem.ListViewSubItem());
            fullRowSelectListView.Items.Add(fullRowSelectListViewItem);
            yield return new object[] { fullRowSelectListViewItem, new Rectangle(1, 2, 3, 4), ListViewItemStates.Default };
            yield return new object[] { fullRowSelectListViewItem, new Rectangle(1, 2, 3, 4), ListViewItemStates.Focused };
            yield return new object[] { fullRowSelectListViewItem, new Rectangle(1, 2, 3, 4), ListViewItemStates.Checked };
        }
    }

    [WinFormsTheory]
    [MemberData(nameof(Draw_TestData))]
    public void DrawListViewItemEventArgs_DrawBackground_Invoke_Success(ListViewItem item, Rectangle bounds, ListViewItemStates state)
    {
        using Bitmap image = new(10, 10);
        using Graphics graphics = Graphics.FromImage(image);
        DrawListViewItemEventArgs e = new(graphics, item, bounds, -1, state);
        e.DrawBackground();
    }

    [WinFormsTheory]
    [MemberData(nameof(Draw_TestData))]
    public void DrawListViewItemEventArgs_DrawFocusRectangle_HasGraphicsFocused_Success(ListViewItem item, Rectangle bounds, ListViewItemStates state)
    {
        using Bitmap image = new(10, 10);
        using Graphics graphics = Graphics.FromImage(image);
        DrawListViewItemEventArgs e = new(graphics, item, bounds, -1, state);
        e.DrawFocusRectangle();
    }

    [WinFormsTheory]
    [MemberData(nameof(Draw_TestData))]
    public void DrawListViewItemEventArgs_DrawText_Invoke_Success(ListViewItem item, Rectangle bounds, ListViewItemStates state)
    {
        using Bitmap image = new(10, 10);
        using Graphics graphics = Graphics.FromImage(image);
        DrawListViewItemEventArgs e = new(graphics, item, bounds, -1, state);
        e.DrawText();
    }

    [WinFormsTheory]
    [MemberData(nameof(Draw_TestData))]
    public void DrawListViewItemEventArgs_DrawText_InvokeTextFormatFlags(ListViewItem item, Rectangle bounds, ListViewItemStates state)
    {
        using Bitmap image = new(10, 10);
        using Graphics graphics = Graphics.FromImage(image);
        DrawListViewItemEventArgs e = new(graphics, item, bounds, -1, state);
        e.DrawText(TextFormatFlags.Bottom);
    }
}
