// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;
using System.Windows.Forms.TestUtilities;

namespace System.Windows.Forms.Tests;

// NB: doesn't require thread affinity
public class ListViewItemTests
{
    [Fact]
    public void ListViewItem_Ctor_Default()
    {
        ListViewItem item = new();
        Assert.Equal(SystemColors.Window, item.BackColor);
        Assert.Equal(Rectangle.Empty, item.Bounds);
        Assert.False(item.Checked);
        Assert.False(item.Focused);
        Assert.Equal(Control.DefaultFont, item.Font);
        Assert.Equal(SystemColors.WindowText, item.ForeColor);
        Assert.Null(item.Group);
        Assert.Equal(-1, item.ImageIndex);
        Assert.Equal(string.Empty, item.ImageKey);
        Assert.Null(item.ImageList);
        Assert.Equal(0, item.IndentCount);
        Assert.Equal(-1, item.Index);
        Assert.Null(item.ListView);
        Assert.Empty(item.Name);
        Assert.Equal(new Point(-1, -1), item.Position);
        Assert.False(item.Selected);
        Assert.Equal(-1, item.StateImageIndex);
        ListViewItem.ListViewSubItem subItem = Assert.Single(item.SubItems.Cast<ListViewItem.ListViewSubItem>());
        Assert.Empty(subItem.Text);
        Assert.Same(item.SubItems, item.SubItems);
        Assert.Null(item.Tag);
        Assert.Empty(item.Text);
        Assert.Empty(item.ToolTipText);
        Assert.True(item.UseItemStyleForSubItems);
    }

    public static IEnumerable<object[]> Ctor_StringArray_String_Color_Color_Font_ListViewGroup_TestData()
    {
        yield return new object[] { null, null, Color.Empty, Color.Empty, null, null, string.Empty, SystemColors.WindowText, SystemColors.Window, string.Empty };
        yield return new object[] { Array.Empty<string>(), null, Color.Empty, Color.Empty, null, null, string.Empty, SystemColors.WindowText, SystemColors.Window, string.Empty };
        yield return new object[] { new string[] { null }, string.Empty, Color.Empty, Color.Empty, null, new ListViewGroup(), string.Empty, SystemColors.WindowText, SystemColors.Window, string.Empty };
        yield return new object[] { new string[] { "text" }, "imageKey", Color.Blue, Color.Red, SystemFonts.MenuFont, new ListViewGroup(), "imageKey", Color.Blue, Color.Red, "text" };
    }

    [Theory]
    [MemberData(nameof(Ctor_StringArray_String_Color_Color_Font_ListViewGroup_TestData))]
    public void ListViewItem_Ctor_StringArray_String_Color_Color_Font_ListViewGroup(string[] subItems, string imageKey, Color foreColor, Color backColor, Font font, ListViewGroup group, string expectedImageKey, Color expectedForeColor, Color expectedBackColor, string expectedText)
    {
        ListViewItem item = new(subItems, imageKey, foreColor, backColor, font, group);
        Assert.Equal(expectedBackColor, item.BackColor);
        Assert.Equal(Rectangle.Empty, item.Bounds);
        Assert.False(item.Checked);
        Assert.False(item.Focused);
        Assert.Equal(font ?? Control.DefaultFont, item.Font);
        Assert.Equal(expectedForeColor, item.ForeColor);
        Assert.Equal(group, item.Group);
        Assert.Equal(-1, item.ImageIndex);
        Assert.Same(expectedImageKey, item.ImageKey);
        Assert.Null(item.ImageList);
        Assert.Equal(0, item.IndentCount);
        Assert.Equal(-1, item.Index);
        Assert.Null(item.ListView);
        Assert.Empty(item.Name);
        Assert.Equal(new Point(-1, -1), item.Position);
        Assert.False(item.Selected);
        Assert.Equal(-1, item.StateImageIndex);
        AssertEqualListViewSubItem(subItems, item.SubItems.Cast<ListViewItem.ListViewSubItem>().ToArray());
        Assert.Same(item.SubItems, item.SubItems);
        Assert.Null(item.Tag);
        Assert.Same(expectedText, item.Text);
        Assert.Empty(item.ToolTipText);
        Assert.True(item.UseItemStyleForSubItems);
    }

    public static IEnumerable<object[]> Ctor_StringArray_Int_Color_Color_Font_ListViewGroup_TestData()
    {
        yield return new object[] { null, -1, Color.Empty, Color.Empty, null, null, SystemColors.WindowText, SystemColors.Window, string.Empty };
        yield return new object[] { Array.Empty<string>(), 0, Color.Empty, Color.Empty, null, null, SystemColors.WindowText, SystemColors.Window, string.Empty };
        yield return new object[] { new string[] { null }, 1, Color.Empty, Color.Empty, null, new ListViewGroup(), SystemColors.WindowText, SystemColors.Window, string.Empty };
        yield return new object[] { new string[] { "text" }, 2, Color.Blue, Color.Red, SystemFonts.MenuFont, new ListViewGroup(), Color.Blue, Color.Red, "text" };
    }

    [Theory]
    [MemberData(nameof(Ctor_StringArray_Int_Color_Color_Font_ListViewGroup_TestData))]
    public void ListViewItem_Ctor_StringArray_Int_Color_Color_Font_ListViewGroup(string[] subItems, int imageIndex, Color foreColor, Color backColor, Font font, ListViewGroup group, Color expectedForeColor, Color expectedBackColor, string expectedText)
    {
        ListViewItem item = new(subItems, imageIndex, foreColor, backColor, font, group);
        Assert.Equal(expectedBackColor, item.BackColor);
        Assert.Equal(Rectangle.Empty, item.Bounds);
        Assert.False(item.Checked);
        Assert.False(item.Focused);
        Assert.Equal(font ?? Control.DefaultFont, item.Font);
        Assert.Equal(expectedForeColor, item.ForeColor);
        Assert.Equal(group, item.Group);
        Assert.Equal(imageIndex, item.ImageIndex);
        Assert.Empty(item.ImageKey);
        Assert.Null(item.ImageList);
        Assert.Equal(0, item.IndentCount);
        Assert.Equal(-1, item.Index);
        Assert.Null(item.ListView);
        Assert.Empty(item.Name);
        Assert.Equal(new Point(-1, -1), item.Position);
        Assert.False(item.Selected);
        Assert.Equal(-1, item.StateImageIndex);
        AssertEqualListViewSubItem(subItems, item.SubItems.Cast<ListViewItem.ListViewSubItem>().ToArray());
        Assert.Same(item.SubItems, item.SubItems);
        Assert.Null(item.Tag);
        Assert.Same(expectedText, item.Text);
        Assert.Empty(item.ToolTipText);
        Assert.True(item.UseItemStyleForSubItems);
    }

    public static IEnumerable<object[]> Ctor_StringArray_String_Color_Color_Font_TestData()
    {
        yield return new object[] { null, null, Color.Empty, Color.Empty, null, string.Empty, SystemColors.WindowText, SystemColors.Window, string.Empty };
        yield return new object[] { Array.Empty<string>(), null, Color.Empty, Color.Empty, null, string.Empty, SystemColors.WindowText, SystemColors.Window, string.Empty };
        yield return new object[] { new string[] { null }, string.Empty, Color.Empty, Color.Empty, null, string.Empty, SystemColors.WindowText, SystemColors.Window, string.Empty };
        yield return new object[] { new string[] { "text" }, "imageKey", Color.Blue, Color.Red, SystemFonts.MenuFont, "imageKey", Color.Blue, Color.Red, "text" };
    }

    [Theory]
    [MemberData(nameof(Ctor_StringArray_String_Color_Color_Font_TestData))]
    public void ListViewItem_Ctor_StringArray_String_Color_Color_Font(string[] subItems, string imageKey, Color foreColor, Color backColor, Font font, string expectedImageKey, Color expectedForeColor, Color expectedBackColor, string expectedText)
    {
        ListViewItem item = new(subItems, imageKey, foreColor, backColor, font);
        Assert.Equal(expectedBackColor, item.BackColor);
        Assert.Equal(Rectangle.Empty, item.Bounds);
        Assert.False(item.Checked);
        Assert.False(item.Focused);
        Assert.Equal(font ?? Control.DefaultFont, item.Font);
        Assert.Equal(expectedForeColor, item.ForeColor);
        Assert.Null(item.Group);
        Assert.Equal(-1, item.ImageIndex);
        Assert.Same(expectedImageKey, item.ImageKey);
        Assert.Null(item.ImageList);
        Assert.Equal(0, item.IndentCount);
        Assert.Equal(-1, item.Index);
        Assert.Null(item.ListView);
        Assert.Empty(item.Name);
        Assert.Equal(new Point(-1, -1), item.Position);
        Assert.False(item.Selected);
        Assert.Equal(-1, item.StateImageIndex);
        AssertEqualListViewSubItem(subItems, item.SubItems.Cast<ListViewItem.ListViewSubItem>().ToArray());
        Assert.Same(item.SubItems, item.SubItems);
        Assert.Null(item.Tag);
        Assert.Same(expectedText, item.Text);
        Assert.Empty(item.ToolTipText);
        Assert.True(item.UseItemStyleForSubItems);
    }

    public static IEnumerable<object[]> Ctor_StringArray_Int_Color_Color_Font_TestData()
    {
        yield return new object[] { null, -1, Color.Empty, Color.Empty, null, SystemColors.WindowText, SystemColors.Window, string.Empty };
        yield return new object[] { Array.Empty<string>(), 0, Color.Empty, Color.Empty, null, SystemColors.WindowText, SystemColors.Window, string.Empty };
        yield return new object[] { new string[] { null }, 1, Color.Empty, Color.Empty, null, SystemColors.WindowText, SystemColors.Window, string.Empty };
        yield return new object[] { new string[] { "text" }, 2, Color.Blue, Color.Red, SystemFonts.MenuFont, Color.Blue, Color.Red, "text" };
    }

    [Theory]
    [MemberData(nameof(Ctor_StringArray_Int_Color_Color_Font_TestData))]
    public void ListViewItem_Ctor_StringArray_Int_Color_Color_Font(string[] subItems, int imageIndex, Color foreColor, Color backColor, Font font, Color expectedForeColor, Color expectedBackColor, string expectedText)
    {
        ListViewItem item = new(subItems, imageIndex, foreColor, backColor, font);
        Assert.Equal(expectedBackColor, item.BackColor);
        Assert.Equal(Rectangle.Empty, item.Bounds);
        Assert.False(item.Checked);
        Assert.False(item.Focused);
        Assert.Equal(font ?? Control.DefaultFont, item.Font);
        Assert.Equal(expectedForeColor, item.ForeColor);
        Assert.Null(item.Group);
        Assert.Equal(imageIndex, item.ImageIndex);
        Assert.Empty(item.ImageKey);
        Assert.Null(item.ImageList);
        Assert.Equal(0, item.IndentCount);
        Assert.Equal(-1, item.Index);
        Assert.Null(item.ListView);
        Assert.Empty(item.Name);
        Assert.Equal(new Point(-1, -1), item.Position);
        Assert.False(item.Selected);
        Assert.Equal(-1, item.StateImageIndex);
        AssertEqualListViewSubItem(subItems, item.SubItems.Cast<ListViewItem.ListViewSubItem>().ToArray());
        Assert.Same(item.SubItems, item.SubItems);
        Assert.Null(item.Tag);
        Assert.Same(expectedText, item.Text);
        Assert.Empty(item.ToolTipText);
        Assert.True(item.UseItemStyleForSubItems);
    }

    public static IEnumerable<object[]> Ctor_ListViewSubItemArray_String_ListViewGroup_TestData()
    {
        yield return new object[] { Array.Empty<ListViewItem.ListViewSubItem>(), null, null, string.Empty, string.Empty };
        yield return new object[] { new ListViewItem.ListViewSubItem[] { new(null, "text") }, string.Empty, null, string.Empty, "text" };
        yield return new object[] { new ListViewItem.ListViewSubItem[] { new(null, "text") }, "imageKey", new ListViewGroup(), "imageKey", "text" };
    }

    [Theory]
    [MemberData(nameof(Ctor_ListViewSubItemArray_String_ListViewGroup_TestData))]
    public void ListViewItem_Ctor_ListViewSubItemArray_String_ListViewGroup(ListViewItem.ListViewSubItem[] subItems, string imageKey, ListViewGroup group, string expectedImageKey, string expectedText)
    {
        ListViewItem item = new(subItems, imageKey, group);
        Assert.Equal(SystemColors.Window, item.BackColor);
        Assert.Equal(Rectangle.Empty, item.Bounds);
        Assert.False(item.Checked);
        Assert.False(item.Focused);
        Assert.Equal(Control.DefaultFont, item.Font);
        Assert.Equal(SystemColors.WindowText, item.ForeColor);
        Assert.Equal(group, item.Group);
        Assert.Equal(-1, item.ImageIndex);
        Assert.Same(expectedImageKey, item.ImageKey);
        Assert.Null(item.ImageList);
        Assert.Equal(0, item.IndentCount);
        Assert.Equal(-1, item.Index);
        Assert.Null(item.ListView);
        Assert.Empty(item.Name);
        Assert.Equal(new Point(-1, -1), item.Position);
        Assert.False(item.Selected);
        Assert.Equal(-1, item.StateImageIndex);
        AssertEqualListViewSubItem(subItems, item.SubItems.Cast<ListViewItem.ListViewSubItem>().ToArray());
        Assert.Same(item.SubItems, item.SubItems);
        Assert.Null(item.Tag);
        Assert.Same(expectedText, item.Text);
        Assert.Empty(item.ToolTipText);
        Assert.True(item.UseItemStyleForSubItems);
    }

    public static IEnumerable<object[]> Ctor_ListViewSubItemArray_Int_ListViewGroup_TestData()
    {
        yield return new object[] { Array.Empty<ListViewItem.ListViewSubItem>(), 0, null, string.Empty };
        yield return new object[] { new ListViewItem.ListViewSubItem[] { new(null, "text") }, 1, null, "text" };
        yield return new object[] { new ListViewItem.ListViewSubItem[] { new(null, "text") }, -1, new ListViewGroup(), "text" };
    }

    [Theory]
    [MemberData(nameof(Ctor_ListViewSubItemArray_Int_ListViewGroup_TestData))]
    public void ListViewItem_Ctor_ListViewSubItemArray_Int_ListViewGroup(ListViewItem.ListViewSubItem[] subItems, int imageIndex, ListViewGroup group, string expectedText)
    {
        ListViewItem item = new(subItems, imageIndex, group);
        Assert.Equal(SystemColors.Window, item.BackColor);
        Assert.Equal(Rectangle.Empty, item.Bounds);
        Assert.False(item.Checked);
        Assert.False(item.Focused);
        Assert.Equal(Control.DefaultFont, item.Font);
        Assert.Equal(SystemColors.WindowText, item.ForeColor);
        Assert.Equal(group, item.Group);
        Assert.Equal(imageIndex, item.ImageIndex);
        Assert.Equal(string.Empty, item.ImageKey);
        Assert.Null(item.ImageList);
        Assert.Equal(0, item.IndentCount);
        Assert.Equal(-1, item.Index);
        Assert.Null(item.ListView);
        Assert.Empty(item.Name);
        Assert.Equal(new Point(-1, -1), item.Position);
        Assert.False(item.Selected);
        Assert.Equal(-1, item.StateImageIndex);
        AssertEqualListViewSubItem(subItems, item.SubItems.Cast<ListViewItem.ListViewSubItem>().ToArray());
        Assert.Same(item.SubItems, item.SubItems);
        Assert.Null(item.Tag);
        Assert.Same(expectedText, item.Text);
        Assert.Empty(item.ToolTipText);
        Assert.True(item.UseItemStyleForSubItems);
    }

    public static IEnumerable<object[]> Ctor_StringArray_String_ListViewGroup_TestData()
    {
        yield return new object[] { null, null, null, string.Empty, string.Empty };
        yield return new object[] { Array.Empty<string>(), null, null, string.Empty, string.Empty };
        yield return new object[] { new string[] { null }, string.Empty, new ListViewGroup(), string.Empty, string.Empty };
        yield return new object[] { new string[] { "text" }, "imageKey", new ListViewGroup(), "imageKey", "text" };
    }

    [Theory]
    [MemberData(nameof(Ctor_StringArray_String_ListViewGroup_TestData))]
    public void ListViewItem_Ctor_StringArray_String_ListViewGroup(string[] subItems, string imageKey, ListViewGroup group, string expectedImageKey, string expectedText)
    {
        ListViewItem item = new(subItems, imageKey, group);
        Assert.Equal(SystemColors.Window, item.BackColor);
        Assert.Equal(Rectangle.Empty, item.Bounds);
        Assert.False(item.Checked);
        Assert.False(item.Focused);
        Assert.Equal(Control.DefaultFont, item.Font);
        Assert.Equal(SystemColors.WindowText, item.ForeColor);
        Assert.Equal(group, item.Group);
        Assert.Equal(-1, item.ImageIndex);
        Assert.Same(expectedImageKey, item.ImageKey);
        Assert.Null(item.ImageList);
        Assert.Equal(0, item.IndentCount);
        Assert.Equal(-1, item.Index);
        Assert.Null(item.ListView);
        Assert.Empty(item.Name);
        Assert.Equal(new Point(-1, -1), item.Position);
        Assert.False(item.Selected);
        Assert.Equal(-1, item.StateImageIndex);
        AssertEqualListViewSubItem(subItems, item.SubItems.Cast<ListViewItem.ListViewSubItem>().ToArray());
        Assert.Same(item.SubItems, item.SubItems);
        Assert.Null(item.Tag);
        Assert.Same(expectedText, item.Text);
        Assert.Empty(item.ToolTipText);
        Assert.True(item.UseItemStyleForSubItems);
    }

    public static IEnumerable<object[]> Ctor_StringArray_Int_ListViewGroup_TestData()
    {
        yield return new object[] { null, -1, null, string.Empty };
        yield return new object[] { Array.Empty<string>(), 0, null, string.Empty };
        yield return new object[] { new string[] { null }, 1, new ListViewGroup(), string.Empty };
        yield return new object[] { new string[] { "text" }, 2, new ListViewGroup(), "text" };
    }

    [Theory]
    [MemberData(nameof(Ctor_StringArray_Int_ListViewGroup_TestData))]
    public void ListViewItem_Ctor_StringArray_Int_ListViewGroup(string[] subItems, int imageIndex, ListViewGroup group, string expectedText)
    {
        ListViewItem item = new(subItems, imageIndex, group);
        Assert.Equal(SystemColors.Window, item.BackColor);
        Assert.Equal(Rectangle.Empty, item.Bounds);
        Assert.False(item.Checked);
        Assert.False(item.Focused);
        Assert.Equal(Control.DefaultFont, item.Font);
        Assert.Equal(SystemColors.WindowText, item.ForeColor);
        Assert.Equal(group, item.Group);
        Assert.Equal(imageIndex, item.ImageIndex);
        Assert.Empty(item.ImageKey);
        Assert.Null(item.ImageList);
        Assert.Equal(0, item.IndentCount);
        Assert.Equal(-1, item.Index);
        Assert.Null(item.ListView);
        Assert.Empty(item.Name);
        Assert.Equal(new Point(-1, -1), item.Position);
        Assert.False(item.Selected);
        Assert.Equal(-1, item.StateImageIndex);
        AssertEqualListViewSubItem(subItems, item.SubItems.Cast<ListViewItem.ListViewSubItem>().ToArray());
        Assert.Same(item.SubItems, item.SubItems);
        Assert.Null(item.Tag);
        Assert.Same(expectedText, item.Text);
        Assert.Empty(item.ToolTipText);
        Assert.True(item.UseItemStyleForSubItems);
    }

    public static IEnumerable<object[]> Ctor_String_String_ListViewGroup_TestData()
    {
        yield return new object[] { null, null, null, string.Empty, string.Empty };
        yield return new object[] { string.Empty, string.Empty, new ListViewGroup(), string.Empty, string.Empty };
        yield return new object[] { "text", "imageKey", new ListViewGroup(), "text", "imageKey" };
    }

    [Theory]
    [MemberData(nameof(Ctor_String_String_ListViewGroup_TestData))]
    public void ListViewItem_Ctor_String_String_ListViewGroup(string text, string imageKey, ListViewGroup group, string expectedText, string expectedImageKey)
    {
        ListViewItem item = new(text, imageKey, group);
        Assert.Equal(SystemColors.Window, item.BackColor);
        Assert.Equal(Rectangle.Empty, item.Bounds);
        Assert.False(item.Checked);
        Assert.False(item.Focused);
        Assert.Equal(Control.DefaultFont, item.Font);
        Assert.Equal(SystemColors.WindowText, item.ForeColor);
        Assert.Equal(group, item.Group);
        Assert.Equal(-1, item.ImageIndex);
        Assert.Same(expectedImageKey, item.ImageKey);
        Assert.Null(item.ImageList);
        Assert.Equal(0, item.IndentCount);
        Assert.Equal(-1, item.Index);
        Assert.Null(item.ListView);
        Assert.Empty(item.Name);
        Assert.Equal(new Point(-1, -1), item.Position);
        Assert.False(item.Selected);
        Assert.Equal(-1, item.StateImageIndex);
        ListViewItem.ListViewSubItem subItem = Assert.Single(item.SubItems.Cast<ListViewItem.ListViewSubItem>());
        Assert.Same(expectedText, subItem.Text);
        Assert.Same(item.SubItems, item.SubItems);
        Assert.Null(item.Tag);
        Assert.Same(expectedText, item.Text);
        Assert.Empty(item.ToolTipText);
        Assert.True(item.UseItemStyleForSubItems);
    }

    public static IEnumerable<object[]> Ctor_String_Int_ListViewGroup_TestData()
    {
        yield return new object[] { null, -1, null, string.Empty };
        yield return new object[] { string.Empty, 0, new ListViewGroup(), string.Empty };
        yield return new object[] { "text", 1, new ListViewGroup(), "text" };
    }

    [Theory]
    [MemberData(nameof(Ctor_String_Int_ListViewGroup_TestData))]
    public void ListViewItem_Ctor_String_Int_ListViewGroup(string text, int imageIndex, ListViewGroup group, string expectedText)
    {
        ListViewItem item = new(text, imageIndex, group);
        Assert.Equal(SystemColors.Window, item.BackColor);
        Assert.Equal(Rectangle.Empty, item.Bounds);
        Assert.False(item.Checked);
        Assert.False(item.Focused);
        Assert.Equal(Control.DefaultFont, item.Font);
        Assert.Equal(SystemColors.WindowText, item.ForeColor);
        Assert.Equal(group, item.Group);
        Assert.Equal(imageIndex, item.ImageIndex);
        Assert.Empty(item.ImageKey);
        Assert.Null(item.ImageList);
        Assert.Equal(0, item.IndentCount);
        Assert.Equal(-1, item.Index);
        Assert.Null(item.ListView);
        Assert.Empty(item.Name);
        Assert.Equal(new Point(-1, -1), item.Position);
        Assert.False(item.Selected);
        Assert.Equal(-1, item.StateImageIndex);
        ListViewItem.ListViewSubItem subItem = Assert.Single(item.SubItems.Cast<ListViewItem.ListViewSubItem>());
        Assert.Same(expectedText, subItem.Text);
        Assert.Same(item.SubItems, item.SubItems);
        Assert.Null(item.Tag);
        Assert.Same(expectedText, item.Text);
        Assert.Empty(item.ToolTipText);
        Assert.True(item.UseItemStyleForSubItems);
    }

    public static IEnumerable<object[]> Ctor_ListViewSubItemArray_String_TestData()
    {
        yield return new object[] { Array.Empty<ListViewItem.ListViewSubItem>(), null, string.Empty, string.Empty };
        yield return new object[] { new ListViewItem.ListViewSubItem[] { new(null, "text") }, string.Empty, string.Empty, "text" };
        yield return new object[] { new ListViewItem.ListViewSubItem[] { new(null, "text") }, "imageKey", "imageKey", "text" };
    }

    [Theory]
    [MemberData(nameof(Ctor_ListViewSubItemArray_String_TestData))]
    public void ListViewItem_Ctor_ListViewSubItemArray_String(ListViewItem.ListViewSubItem[] subItems, string imageKey, string expectedImageKey, string expectedText)
    {
        ListViewItem item = new(subItems, imageKey);
        Assert.Equal(SystemColors.Window, item.BackColor);
        Assert.Equal(Rectangle.Empty, item.Bounds);
        Assert.False(item.Checked);
        Assert.False(item.Focused);
        Assert.Equal(Control.DefaultFont, item.Font);
        Assert.Equal(SystemColors.WindowText, item.ForeColor);
        Assert.Null(item.Group);
        Assert.Equal(-1, item.ImageIndex);
        Assert.Same(expectedImageKey, item.ImageKey);
        Assert.Null(item.ImageList);
        Assert.Equal(0, item.IndentCount);
        Assert.Equal(-1, item.Index);
        Assert.Null(item.ListView);
        Assert.Empty(item.Name);
        Assert.Equal(new Point(-1, -1), item.Position);
        Assert.False(item.Selected);
        Assert.Equal(-1, item.StateImageIndex);
        AssertEqualListViewSubItem(subItems, item.SubItems.Cast<ListViewItem.ListViewSubItem>().ToArray());
        Assert.Same(item.SubItems, item.SubItems);
        Assert.Null(item.Tag);
        Assert.Same(expectedText, item.Text);
        Assert.Empty(item.ToolTipText);
        Assert.True(item.UseItemStyleForSubItems);
    }

    public static IEnumerable<object[]> Ctor_ListViewSubItemArray_Int_TestData()
    {
        yield return new object[] { Array.Empty<ListViewItem.ListViewSubItem>(), 0, string.Empty };
        yield return new object[] { new ListViewItem.ListViewSubItem[] { new(null, "text") }, 1, "text" };
        yield return new object[] { new ListViewItem.ListViewSubItem[] { new(null, "text") }, -1, "text" };
    }

    [Theory]
    [MemberData(nameof(Ctor_ListViewSubItemArray_Int_TestData))]
    public void ListViewItem_Ctor_ListViewSubItemArray_Int(ListViewItem.ListViewSubItem[] subItems, int imageIndex, string expectedText)
    {
        ListViewItem item = new(subItems, imageIndex);
        Assert.Equal(SystemColors.Window, item.BackColor);
        Assert.Equal(Rectangle.Empty, item.Bounds);
        Assert.False(item.Checked);
        Assert.False(item.Focused);
        Assert.Equal(Control.DefaultFont, item.Font);
        Assert.Equal(SystemColors.WindowText, item.ForeColor);
        Assert.Null(item.Group);
        Assert.Equal(imageIndex, item.ImageIndex);
        Assert.Equal(string.Empty, item.ImageKey);
        Assert.Null(item.ImageList);
        Assert.Equal(0, item.IndentCount);
        Assert.Equal(-1, item.Index);
        Assert.Null(item.ListView);
        Assert.Empty(item.Name);
        Assert.Equal(new Point(-1, -1), item.Position);
        Assert.False(item.Selected);
        Assert.Equal(-1, item.StateImageIndex);
        AssertEqualListViewSubItem(subItems, item.SubItems.Cast<ListViewItem.ListViewSubItem>().ToArray());
        Assert.Same(item.SubItems, item.SubItems);
        Assert.Null(item.Tag);
        Assert.Same(expectedText, item.Text);
        Assert.Empty(item.ToolTipText);
        Assert.True(item.UseItemStyleForSubItems);
    }

    public static IEnumerable<object[]> Ctor_StringArray_ListViewGroup_TestData()
    {
        yield return new object[] { null, null, string.Empty };
        yield return new object[] { Array.Empty<string>(), null, string.Empty };
        yield return new object[] { new string[] { null }, new ListViewGroup(), string.Empty };
        yield return new object[] { new string[] { "text" }, new ListViewGroup(), "text" };
    }

    [Theory]
    [MemberData(nameof(Ctor_StringArray_ListViewGroup_TestData))]
    public void ListViewItem_Ctor_StringArray_ListViewGroup(string[] subItems, ListViewGroup group, string expectedText)
    {
        ListViewItem item = new(subItems, group);
        Assert.Equal(SystemColors.Window, item.BackColor);
        Assert.Equal(Rectangle.Empty, item.Bounds);
        Assert.False(item.Checked);
        Assert.False(item.Focused);
        Assert.Equal(Control.DefaultFont, item.Font);
        Assert.Equal(SystemColors.WindowText, item.ForeColor);
        Assert.Equal(group, item.Group);
        Assert.Equal(-1, item.ImageIndex);
        Assert.Empty(item.ImageKey);
        Assert.Null(item.ImageList);
        Assert.Equal(0, item.IndentCount);
        Assert.Equal(-1, item.Index);
        Assert.Null(item.ListView);
        Assert.Empty(item.Name);
        Assert.Equal(new Point(-1, -1), item.Position);
        Assert.False(item.Selected);
        Assert.Equal(-1, item.StateImageIndex);
        AssertEqualListViewSubItem(subItems, item.SubItems.Cast<ListViewItem.ListViewSubItem>().ToArray());
        Assert.Same(item.SubItems, item.SubItems);
        Assert.Null(item.Tag);
        Assert.Same(expectedText, item.Text);
        Assert.Empty(item.ToolTipText);
        Assert.True(item.UseItemStyleForSubItems);
    }

    public static IEnumerable<object[]> Ctor_StringArray_String_TestData()
    {
        yield return new object[] { null, null, string.Empty, string.Empty };
        yield return new object[] { Array.Empty<string>(), null, string.Empty, string.Empty };
        yield return new object[] { new string[] { null }, string.Empty, string.Empty, string.Empty };
        yield return new object[] { new string[] { "text" }, "imageKey", "imageKey", "text" };
    }

    [Theory]
    [MemberData(nameof(Ctor_StringArray_String_TestData))]
    public void ListViewItem_Ctor_StringArray_String(string[] subItems, string imageKey, string expectedImageKey, string expectedText)
    {
        ListViewItem item = new(subItems, imageKey);
        Assert.Equal(SystemColors.Window, item.BackColor);
        Assert.Equal(Rectangle.Empty, item.Bounds);
        Assert.False(item.Checked);
        Assert.False(item.Focused);
        Assert.Equal(Control.DefaultFont, item.Font);
        Assert.Equal(SystemColors.WindowText, item.ForeColor);
        Assert.Null(item.Group);
        Assert.Equal(-1, item.ImageIndex);
        Assert.Equal(expectedImageKey, item.ImageKey);
        Assert.Null(item.ImageList);
        Assert.Equal(0, item.IndentCount);
        Assert.Equal(-1, item.Index);
        Assert.Null(item.ListView);
        Assert.Empty(item.Name);
        Assert.Equal(new Point(-1, -1), item.Position);
        Assert.False(item.Selected);
        Assert.Equal(-1, item.StateImageIndex);
        AssertEqualListViewSubItem(subItems, item.SubItems.Cast<ListViewItem.ListViewSubItem>().ToArray());
        Assert.Same(item.SubItems, item.SubItems);
        Assert.Null(item.Tag);
        Assert.Equal(expectedText, item.Text);
        Assert.Empty(item.ToolTipText);
        Assert.True(item.UseItemStyleForSubItems);
    }

    public static IEnumerable<object[]> Ctor_StringArray_Int_TestData()
    {
        yield return new object[] { null, -1, string.Empty };
        yield return new object[] { Array.Empty<string>(), 0, string.Empty };
        yield return new object[] { new string[] { null }, 1, string.Empty };
        yield return new object[] { new string[] { "text" }, 2, "text" };
    }

    [Theory]
    [MemberData(nameof(Ctor_StringArray_Int_TestData))]
    public void ListViewItem_Ctor_StringArray_Int(string[] subItems, int imageIndex, string expectedText)
    {
        ListViewItem item = new(subItems, imageIndex);
        Assert.Equal(SystemColors.Window, item.BackColor);
        Assert.Equal(Rectangle.Empty, item.Bounds);
        Assert.False(item.Checked);
        Assert.False(item.Focused);
        Assert.Equal(Control.DefaultFont, item.Font);
        Assert.Equal(SystemColors.WindowText, item.ForeColor);
        Assert.Null(item.Group);
        Assert.Equal(imageIndex, item.ImageIndex);
        Assert.Equal(string.Empty, item.ImageKey);
        Assert.Null(item.ImageList);
        Assert.Equal(0, item.IndentCount);
        Assert.Equal(-1, item.Index);
        Assert.Null(item.ListView);
        Assert.Empty(item.Name);
        Assert.Equal(new Point(-1, -1), item.Position);
        Assert.False(item.Selected);
        Assert.Equal(-1, item.StateImageIndex);
        AssertEqualListViewSubItem(subItems, item.SubItems.Cast<ListViewItem.ListViewSubItem>().ToArray());
        Assert.Same(item.SubItems, item.SubItems);
        Assert.Null(item.Tag);
        Assert.Equal(expectedText, item.Text);
        Assert.Empty(item.ToolTipText);
        Assert.True(item.UseItemStyleForSubItems);
    }

    public static IEnumerable<object[]> Ctor_String_ListViewGroup_TestData()
    {
        yield return new object[] { null, null, string.Empty };
        yield return new object[] { string.Empty, null, string.Empty };
        yield return new object[] { "text", new ListViewGroup(), "text" };
    }

    [Theory]
    [MemberData(nameof(Ctor_String_ListViewGroup_TestData))]
    public void ListViewItem_Ctor_String_ListViewGroup(string text, ListViewGroup group, string expectedText)
    {
        ListViewItem item = new(text, group);
        Assert.Equal(SystemColors.Window, item.BackColor);
        Assert.Equal(Rectangle.Empty, item.Bounds);
        Assert.False(item.Checked);
        Assert.False(item.Focused);
        Assert.Equal(Control.DefaultFont, item.Font);
        Assert.Equal(SystemColors.WindowText, item.ForeColor);
        Assert.Equal(group, item.Group);
        Assert.Equal(-1, item.ImageIndex);
        Assert.Equal(string.Empty, item.ImageKey);
        Assert.Null(item.ImageList);
        Assert.Equal(0, item.IndentCount);
        Assert.Equal(-1, item.Index);
        Assert.Null(item.ListView);
        Assert.Empty(item.Name);
        Assert.Equal(new Point(-1, -1), item.Position);
        Assert.False(item.Selected);
        Assert.Equal(-1, item.StateImageIndex);
        ListViewItem.ListViewSubItem subItem = Assert.Single(item.SubItems.Cast<ListViewItem.ListViewSubItem>());
        Assert.Same(expectedText, subItem.Text);
        Assert.Same(item.SubItems, item.SubItems);
        Assert.Null(item.Tag);
        Assert.Same(expectedText, item.Text);
        Assert.Empty(item.ToolTipText);
        Assert.True(item.UseItemStyleForSubItems);
    }

    public static IEnumerable<object[]> Ctor_String_String_TestData()
    {
        yield return new object[] { null, null, string.Empty, string.Empty };
        yield return new object[] { string.Empty, string.Empty, string.Empty, string.Empty };
        yield return new object[] { "text", "imageKey", "text", "imageKey" };
    }

    [Theory]
    [MemberData(nameof(Ctor_String_String_TestData))]
    public void ListViewItem_Ctor_String_String(string text, string imageKey, string expectedText, string expectedImageKey)
    {
        ListViewItem item = new(text, imageKey);
        Assert.Equal(SystemColors.Window, item.BackColor);
        Assert.Equal(Rectangle.Empty, item.Bounds);
        Assert.False(item.Checked);
        Assert.False(item.Focused);
        Assert.Equal(Control.DefaultFont, item.Font);
        Assert.Equal(SystemColors.WindowText, item.ForeColor);
        Assert.Null(item.Group);
        Assert.Equal(-1, item.ImageIndex);
        Assert.Equal(expectedImageKey, item.ImageKey);
        Assert.Null(item.ImageList);
        Assert.Equal(0, item.IndentCount);
        Assert.Equal(-1, item.Index);
        Assert.Null(item.ListView);
        Assert.Empty(item.Name);
        Assert.Equal(new Point(-1, -1), item.Position);
        Assert.False(item.Selected);
        Assert.Equal(-1, item.StateImageIndex);
        ListViewItem.ListViewSubItem subItem = Assert.Single(item.SubItems.Cast<ListViewItem.ListViewSubItem>());
        Assert.Equal(expectedText, subItem.Text);
        Assert.Same(item.SubItems, item.SubItems);
        Assert.Null(item.Tag);
        Assert.Equal(expectedText, item.Text);
        Assert.Empty(item.ToolTipText);
        Assert.True(item.UseItemStyleForSubItems);
    }

    public static IEnumerable<object[]> Ctor_String_Int_TestData()
    {
        yield return new object[] { null, -1, string.Empty };
        yield return new object[] { string.Empty, 0, string.Empty };
        yield return new object[] { "text", 1, "text" };
    }

    [Theory]
    [MemberData(nameof(Ctor_String_Int_TestData))]
    public void ListViewItem_Ctor_String_Int(string text, int imageIndex, string expectedText)
    {
        ListViewItem item = new(text, imageIndex);
        Assert.Equal(SystemColors.Window, item.BackColor);
        Assert.Equal(Rectangle.Empty, item.Bounds);
        Assert.False(item.Checked);
        Assert.False(item.Focused);
        Assert.Equal(Control.DefaultFont, item.Font);
        Assert.Equal(SystemColors.WindowText, item.ForeColor);
        Assert.Null(item.Group);
        Assert.Equal(imageIndex, item.ImageIndex);
        Assert.Equal(string.Empty, item.ImageKey);
        Assert.Null(item.ImageList);
        Assert.Equal(0, item.IndentCount);
        Assert.Equal(-1, item.Index);
        Assert.Null(item.ListView);
        Assert.Empty(item.Name);
        Assert.Equal(new Point(-1, -1), item.Position);
        Assert.False(item.Selected);
        Assert.Equal(-1, item.StateImageIndex);
        ListViewItem.ListViewSubItem subItem = Assert.Single(item.SubItems.Cast<ListViewItem.ListViewSubItem>());
        Assert.Equal(expectedText, subItem.Text);
        Assert.Same(item.SubItems, item.SubItems);
        Assert.Null(item.Tag);
        Assert.Equal(expectedText, item.Text);
        Assert.Empty(item.ToolTipText);
        Assert.True(item.UseItemStyleForSubItems);
    }

    public static IEnumerable<object[]> Ctor_ListViewGroup_TestData()
    {
        yield return new object[] { null };
        yield return new object[] { new ListViewGroup() };
    }

    [Theory]
    [MemberData(nameof(Ctor_ListViewGroup_TestData))]
    public void ListViewItem_Ctor_ListViewGroup(ListViewGroup group)
    {
        ListViewItem item = new(group);
        Assert.Equal(SystemColors.Window, item.BackColor);
        Assert.Equal(Rectangle.Empty, item.Bounds);
        Assert.False(item.Checked);
        Assert.False(item.Focused);
        Assert.Equal(Control.DefaultFont, item.Font);
        Assert.Equal(SystemColors.WindowText, item.ForeColor);
        Assert.Equal(group, item.Group);
        Assert.Equal(-1, item.ImageIndex);
        Assert.Equal(string.Empty, item.ImageKey);
        Assert.Null(item.ImageList);
        Assert.Equal(0, item.IndentCount);
        Assert.Equal(-1, item.Index);
        Assert.Null(item.ListView);
        Assert.Empty(item.Name);
        Assert.Equal(new Point(-1, -1), item.Position);
        Assert.False(item.Selected);
        Assert.Equal(-1, item.StateImageIndex);
        ListViewItem.ListViewSubItem subItem = Assert.Single(item.SubItems.Cast<ListViewItem.ListViewSubItem>());
        Assert.Empty(subItem.Text);
        Assert.Same(item.SubItems, item.SubItems);
        Assert.Null(item.Tag);
        Assert.Empty(item.Text);
        Assert.Empty(item.ToolTipText);
        Assert.True(item.UseItemStyleForSubItems);
    }

    public static IEnumerable<object[]> Ctor_StringArray_TestData()
    {
        yield return new object[] { null, string.Empty };
        yield return new object[] { Array.Empty<string>(), string.Empty };
        yield return new object[] { new string[] { null }, string.Empty };
        yield return new object[] { new string[] { "text" }, "text" };
    }

    [Theory]
    [MemberData(nameof(Ctor_StringArray_TestData))]
    public void ListViewItem_Ctor_StringArray(string[] subItems, string expectedText)
    {
        ListViewItem item = new(subItems);
        Assert.Equal(SystemColors.Window, item.BackColor);
        Assert.Equal(Rectangle.Empty, item.Bounds);
        Assert.False(item.Checked);
        Assert.False(item.Focused);
        Assert.Equal(Control.DefaultFont, item.Font);
        Assert.Equal(SystemColors.WindowText, item.ForeColor);
        Assert.Null(item.Group);
        Assert.Equal(-1, item.ImageIndex);
        Assert.Empty(item.ImageKey);
        Assert.Null(item.ImageList);
        Assert.Equal(0, item.IndentCount);
        Assert.Equal(-1, item.Index);
        Assert.Null(item.ListView);
        Assert.Empty(item.Name);
        Assert.Equal(new Point(-1, -1), item.Position);
        Assert.False(item.Selected);
        Assert.Equal(-1, item.StateImageIndex);
        AssertEqualListViewSubItem(subItems, item.SubItems.Cast<ListViewItem.ListViewSubItem>().ToArray());
        Assert.Same(item.SubItems, item.SubItems);
        Assert.Null(item.Tag);
        Assert.Equal(expectedText, item.Text);
        Assert.Empty(item.ToolTipText);
        Assert.True(item.UseItemStyleForSubItems);
    }

    [Theory]
    [NormalizedStringData]
    public void ListViewItem_Ctor_String(string text, string expectedText)
    {
        ListViewItem item = new(text);
        Assert.Equal(SystemColors.Window, item.BackColor);
        Assert.Equal(Rectangle.Empty, item.Bounds);
        Assert.False(item.Checked);
        Assert.False(item.Focused);
        Assert.Equal(Control.DefaultFont, item.Font);
        Assert.Equal(SystemColors.WindowText, item.ForeColor);
        Assert.Null(item.Group);
        Assert.Equal(-1, item.ImageIndex);
        Assert.Empty(item.ImageKey);
        Assert.Null(item.ImageList);
        Assert.Equal(0, item.IndentCount);
        Assert.Equal(-1, item.Index);
        Assert.Null(item.ListView);
        Assert.Empty(item.Name);
        Assert.Equal(new Point(-1, -1), item.Position);
        Assert.False(item.Selected);
        Assert.Equal(-1, item.StateImageIndex);
        ListViewItem.ListViewSubItem subItem = Assert.Single(item.SubItems.Cast<ListViewItem.ListViewSubItem>());
        Assert.Equal(expectedText, subItem.Text);
        Assert.Same(item.SubItems, item.SubItems);
        Assert.Null(item.Tag);
        Assert.Equal(expectedText, item.Text);
        Assert.Empty(item.ToolTipText);
        Assert.True(item.UseItemStyleForSubItems);
    }

    [Fact]
    public void ListViewItem_Ctor_NullSubItems_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>("subItems", () => new ListViewItem((ListViewItem.ListViewSubItem[])null, "imageKey"));
        Assert.Throws<ArgumentNullException>("subItems", () => new ListViewItem((ListViewItem.ListViewSubItem[])null, 1));
        Assert.Throws<ArgumentNullException>("subItems", () => new ListViewItem((ListViewItem.ListViewSubItem[])null, "imageKey", new ListViewGroup()));
        Assert.Throws<ArgumentNullException>("subItems", () => new ListViewItem((ListViewItem.ListViewSubItem[])null, 0, new ListViewGroup()));
    }

    [Fact]
    public void ListViewItem_Ctor_NullValueInSubItems_ThrowsArgumentNullException()
    {
        var subItems = new ListViewItem.ListViewSubItem[] { null };
        Assert.Throws<ArgumentNullException>("subItems", () => new ListViewItem(subItems, "imageKey"));
        Assert.Throws<ArgumentNullException>("subItems", () => new ListViewItem(subItems, 1));
        Assert.Throws<ArgumentNullException>("subItems", () => new ListViewItem(subItems, "imageKey", new ListViewGroup()));
        Assert.Throws<ArgumentNullException>("subItems", () => new ListViewItem(subItems, 1, new ListViewGroup()));
    }

    public static IEnumerable<object[]> BackColor_Set_TestData()
    {
        yield return new object[] { Color.Empty, SystemColors.Window };
        yield return new object[] { Color.Red, Color.Red };
    }

    [WinFormsTheory]
    [MemberData(nameof(BackColor_Set_TestData))]
    public void ListViewItem_BackColor_GetWithOwner_ReturnsExpected(Color value, Color expected)
    {
        using ListView listView = new()
        {
            BackColor = value
        };

        ListViewItem item = new();
        listView.Items.Add(item);
        Assert.Equal(expected, item.BackColor);

        // Remove item.
        listView.Items.Remove(item);
        Assert.Equal(SystemColors.Window, item.BackColor);
    }

    [Theory]
    [MemberData(nameof(BackColor_Set_TestData))]
    public void ListViewItem_BackColor_Set_GetReturnsExpected(Color value, Color expected)
    {
        ListViewItem item = new()
        {
            BackColor = value
        };
        Assert.Equal(expected, item.BackColor);

        // Set same.
        item.BackColor = value;
        Assert.Equal(expected, item.BackColor);
    }

    [WinFormsTheory]
    [MemberData(nameof(BackColor_Set_TestData))]
    public void ListViewItem_BackColor_SetWithOwner_GetReturnsExpected(Color value, Color expected)
    {
        using ListView listView = new();
        ListViewItem item = new();
        listView.Items.Add(item);

        item.BackColor = value;
        Assert.Equal(expected, item.BackColor);

        // Set same.
        item.BackColor = value;
        Assert.Equal(expected, item.BackColor);
    }

    public static IEnumerable<object[]> ForeColor_Set_TestData()
    {
        yield return new object[] { Color.Empty, SystemColors.WindowText };
        yield return new object[] { Color.White, Color.White };
        yield return new object[] { Color.Black, Color.Black };
        yield return new object[] { Color.Red, Color.Red };
    }

    [WinFormsTheory]
    [MemberData(nameof(ForeColor_Set_TestData))]
    public void ListViewItem_ForeColor_GetWithOwner_ReturnsExpected(Color value, Color expected)
    {
        using ListView listView = new()
        {
            ForeColor = value
        };

        ListViewItem item = new();
        listView.Items.Add(item);
        Assert.Equal(expected, item.ForeColor);

        // Remove item.
        listView.Items.Remove(item);
        Assert.Equal(SystemColors.WindowText, item.ForeColor);
    }

    [Theory]
    [MemberData(nameof(ForeColor_Set_TestData))]
    public void ListViewItem_ForeColor_Set_GetReturnsExpected(Color value, Color expected)
    {
        ListViewItem item = new()
        {
            ForeColor = value
        };

        Assert.Equal(expected, item.ForeColor);

        // Set same.
        item.ForeColor = value;
        Assert.Equal(expected, item.ForeColor);
    }

    [WinFormsTheory]
    [MemberData(nameof(ForeColor_Set_TestData))]
    public void ListViewItem_ForeColor_SetWithOwner_GetReturnsExpected(Color value, Color expected)
    {
        using ListView listView = new();
        ListViewItem item = new();
        listView.Items.Add(item);

        item.ForeColor = value;
        Assert.Equal(expected, item.ForeColor);

        // Set same.
        item.ForeColor = value;
        Assert.Equal(expected, item.ForeColor);
    }

    [WinFormsTheory]
    [CommonMemberData(typeof(CommonTestHelperEx), nameof(CommonTestHelperEx.GetFontTheoryData))]
    public void ListViewItem_Font_GetWithOwner_ReturnsExpected(Font value)
    {
        using ListView listView = new()
        {
            Font = value
        };

        ListViewItem item = new();
        listView.Items.Add(item);
        Assert.Equal(value ?? Control.DefaultFont, item.Font);

        // Remove item.
        listView.Items.Remove(item);
        Assert.Equal(Control.DefaultFont, item.Font);
    }

    [Theory]
    [CommonMemberData(typeof(CommonTestHelperEx), nameof(CommonTestHelperEx.GetFontTheoryData))]
    public void ListViewItem_Font_Set_GetReturnsExpected(Font value)
    {
        ListViewItem item = new()
        {
            Font = value
        };

        Assert.Equal(value ?? Control.DefaultFont, item.Font);

        // Set same.
        item.Font = value;
        Assert.Equal(value ?? Control.DefaultFont, item.Font);
    }

    [WinFormsTheory]
    [CommonMemberData(typeof(CommonTestHelperEx), nameof(CommonTestHelperEx.GetFontTheoryData))]
    public void ListViewItem_Font_SetWithOwner_GetReturnsExpected(Font value)
    {
        using ListView listView = new()
        {
            Font = SystemFonts.CaptionFont
        };

        ListViewItem item = new();
        listView.Items.Add(item);

        item.Font = value;
        Assert.Equal(value ?? SystemFonts.CaptionFont, item.Font);

        // Set same.
        item.Font = value;
        Assert.Equal(value ?? SystemFonts.CaptionFont, item.Font);
    }

    [WinFormsFact]
    public void ListViewItem_EnsureVisible_HasListViewWithoutHandle_Nop()
    {
        using ListView listView = new();
        ListViewItem item = new();
        listView.Items.Add(item);
        item.EnsureVisible();
    }

    [Fact]
    public void ListViewItem_EnsureVisible_NoListView_Nop()
    {
        ListViewItem item = new();
        item.EnsureVisible();
    }

    [WinFormsFact]
    public void ListViewItem_Remove_HasListView_Success()
    {
        using ListView listView = new();
        ListViewItem item = new();
        listView.Items.Add(item);
        item.Remove();
        Assert.Empty(listView.Items);
        Assert.Null(item.ListView);
    }

    [Fact]
    public void ListViewItem_Remove_NoListView_Nop()
    {
        ListViewItem item = new();
        item.Remove();
    }

    private static void AssertEqualListViewSubItem(string[] expected, ListViewItem.ListViewSubItem[] actual)
    {
        AssertEqualListViewSubItem(expected?.Select(t => new ListViewItem.ListViewSubItem(null, t)).ToArray(), actual);
    }

    private static void AssertEqualListViewSubItem(ListViewItem.ListViewSubItem[] expected, ListViewItem.ListViewSubItem[] actual)
    {
        if (expected is null || expected.Length == 0)
        {
            ListViewItem.ListViewSubItem subItem = Assert.Single(actual.Cast<ListViewItem.ListViewSubItem>());
            Assert.Empty(subItem.Text);
            return;
        }

        Assert.Equal(expected.Length, actual.Length);
        for (int i = 0; i < expected.Length; i++)
        {
            Assert.Equal(expected[i].Name, actual[i].Name);
            Assert.Equal(expected[i].Text, actual[i].Text);
            Assert.Equal(expected[i].Tag, actual[i].Tag);
        }
    }

    [Fact]
    public void ListViewItem_AccessibilityObject_ThrowsInvalidOperationException_IfListViewNotExists()
    {
        ListViewItem item = new();

        Assert.Throws<InvalidOperationException>(() => item.AccessibilityObject);
    }

    [WinFormsFact]
    public void ListViewItem_AccessibilityObject_ReturnsExpected_IfListViewExists()
    {
        using ListView listView = new();
        ListViewItem item = new();
        listView.Items.Add(item);

        Assert.NotNull(item.AccessibilityObject);
    }

    [WinFormsFact]
    public void ListViewItem_AccessibilityObject_ThrowsInvalidOperationException_IfRemoved()
    {
        using ListView listView = new();
        ListViewItem item = new();
        listView.Items.Add(item);

        Assert.NotNull(item.AccessibilityObject);

        listView.Items.RemoveAt(0);

        Assert.Throws<InvalidOperationException>(() => item.AccessibilityObject);
    }

    [WinFormsFact]
    public void ListViewItem_AccessibilityObject_ThrowsInvalidOperationException_IfCleared()
    {
        using ListView listView = new();
        ListViewItem item = new();
        listView.Items.Add(item);

        Assert.NotNull(item.AccessibilityObject);

        listView.Items.Clear();

        Assert.Throws<InvalidOperationException>(() => item.AccessibilityObject);
    }
}
