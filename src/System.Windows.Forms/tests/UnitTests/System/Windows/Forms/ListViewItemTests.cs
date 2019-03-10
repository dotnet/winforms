
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using WinForms.Common.Tests;
using Xunit;

namespace System.Windows.Forms.Tests
{
    public class ListViewItemTests
    {
        [Fact]
        public void ListViewItem_Ctor_Default()
        {
            var item = new ListViewItem();
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
            yield return new object[] { null, null, Color.Empty, Color.Empty, null, null, SystemColors.WindowText, SystemColors.Window, "" };
            yield return new object[] { new string[0], null, Color.Empty, Color.Empty, null, null, SystemColors.WindowText, SystemColors.Window, "" };
            yield return new object[] { new string[] { null }, "", Color.Empty, Color.Empty, null, new ListViewGroup(), SystemColors.WindowText, SystemColors.Window, "" };
            yield return new object[] { new string[] { "text" }, "imageKey", Color.Blue, Color.Red, SystemFonts.MenuFont, new ListViewGroup(), Color.Blue, Color.Red, "text" };
        }

        [Theory]
        [MemberData(nameof(Ctor_StringArray_String_Color_Color_Font_ListViewGroup_TestData))]
        public void ListViewItem_Ctor_StringArray_String_Color_Color_Font_ListViewGroup(string[] subItems, string imageKey, Color foreColor, Color backColor, Font font, ListViewGroup group, Color expectedForeColor, Color expectedBackColor, string expectedText)
        {
            var item = new ListViewItem(subItems, imageKey, foreColor, backColor, font, group);
            Assert.Equal(expectedBackColor, item.BackColor);
            Assert.Equal(Rectangle.Empty, item.Bounds);
            Assert.False(item.Checked);
            Assert.False(item.Focused);
            Assert.Equal(font ?? Control.DefaultFont, item.Font);
            Assert.Equal(expectedForeColor, item.ForeColor);
            Assert.Equal(group, item.Group);
            Assert.Equal(-1, item.ImageIndex);
            Assert.Equal(imageKey ?? string.Empty, item.ImageKey);
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

        public static IEnumerable<object[]> Ctor_StringArray_Int_Color_Color_Font_ListViewGroup_TestData()
        {
            yield return new object[] { null, -1, Color.Empty, Color.Empty, null, null, SystemColors.WindowText, SystemColors.Window, "" };
            yield return new object[] { new string[0], 0, Color.Empty, Color.Empty, null, null, SystemColors.WindowText, SystemColors.Window, "" };
            yield return new object[] { new string[] { null }, 1, Color.Empty, Color.Empty, null, new ListViewGroup(), SystemColors.WindowText, SystemColors.Window, "" };
            yield return new object[] { new string[] { "text" }, 2, Color.Blue, Color.Red, SystemFonts.MenuFont, new ListViewGroup(), Color.Blue, Color.Red, "text" };
        }

        [Theory]
        [MemberData(nameof(Ctor_StringArray_Int_Color_Color_Font_ListViewGroup_TestData))]
        public void ListViewItem_Ctor_StringArray_Int_Color_Color_Font_ListViewGroup(string[] subItems, int imageIndex, Color foreColor, Color backColor, Font font, ListViewGroup group, Color expectedForeColor, Color expectedBackColor, string expectedText)
        {
            var item = new ListViewItem(subItems, imageIndex, foreColor, backColor, font, group);
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
            Assert.Equal(expectedText, item.Text);
            Assert.Empty(item.ToolTipText);
            Assert.True(item.UseItemStyleForSubItems);
        }

        public static IEnumerable<object[]> Ctor_StringArray_String_Color_Color_Font_TestData()
        {
            yield return new object[] { null, null, Color.Empty, Color.Empty, null, SystemColors.WindowText, SystemColors.Window, "" };
            yield return new object[] { new string[0], null, Color.Empty, Color.Empty, null, SystemColors.WindowText, SystemColors.Window, "" };
            yield return new object[] { new string[] { null }, "", Color.Empty, Color.Empty, null, SystemColors.WindowText, SystemColors.Window, "" };
            yield return new object[] { new string[] { "text" }, "imageKey", Color.Blue, Color.Red, SystemFonts.MenuFont, Color.Blue, Color.Red, "text" };
        }

        [Theory]
        [MemberData(nameof(Ctor_StringArray_String_Color_Color_Font_TestData))]
        public void ListViewItem_Ctor_StringArray_String_Color_Color_Font(string[] subItems, string imageKey, Color foreColor, Color backColor, Font font, Color expectedForeColor, Color expectedBackColor, string expectedText)
        {
            var item = new ListViewItem(subItems, imageKey, foreColor, backColor, font);
            Assert.Equal(expectedBackColor, item.BackColor);
            Assert.Equal(Rectangle.Empty, item.Bounds);
            Assert.False(item.Checked);
            Assert.False(item.Focused);
            Assert.Equal(font ?? Control.DefaultFont, item.Font);
            Assert.Equal(expectedForeColor, item.ForeColor);
            Assert.Null(item.Group);
            Assert.Equal(-1, item.ImageIndex);
            Assert.Equal(imageKey ?? string.Empty, item.ImageKey);
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

        public static IEnumerable<object[]> Ctor_StringArray_Int_Color_Color_Font_TestData()
        {
            yield return new object[] { null, -1, Color.Empty, Color.Empty, null, SystemColors.WindowText, SystemColors.Window, "" };
            yield return new object[] { new string[0], 0, Color.Empty, Color.Empty, null, SystemColors.WindowText, SystemColors.Window, "" };
            yield return new object[] { new string[] { null }, 1, Color.Empty, Color.Empty, null, SystemColors.WindowText, SystemColors.Window, "" };
            yield return new object[] { new string[] { "text" }, 2, Color.Blue, Color.Red, SystemFonts.MenuFont, Color.Blue, Color.Red, "text" };
        }

        [Theory]
        [MemberData(nameof(Ctor_StringArray_Int_Color_Color_Font_TestData))]
        public void ListViewItem_Ctor_StringArray_Int_Color_Color_Font(string[] subItems, int imageIndex, Color foreColor, Color backColor, Font font, Color expectedForeColor, Color expectedBackColor, string expectedText)
        {
            var item = new ListViewItem(subItems, imageIndex, foreColor, backColor, font);
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
            Assert.Equal(expectedText, item.Text);
            Assert.Empty(item.ToolTipText);
            Assert.True(item.UseItemStyleForSubItems);
        }

        public static IEnumerable<object[]> Ctor_ListViewSubItemArray_String_ListViewGroup_TestData()
        {
            yield return new object[] { new ListViewItem.ListViewSubItem[0], null, null, string.Empty };
            yield return new object[] { new ListViewItem.ListViewSubItem[] { new ListViewItem.ListViewSubItem(null, "text") }, "", null, "text" };
            yield return new object[] { new ListViewItem.ListViewSubItem[] { new ListViewItem.ListViewSubItem(null, "text") }, "imageKey", new ListViewGroup(), "text" };
        }

        [Theory]
        [MemberData(nameof(Ctor_ListViewSubItemArray_String_ListViewGroup_TestData))]
        public void ListViewItem_Ctor_ListViewSubItemArray_String_ListViewGroup(ListViewItem.ListViewSubItem[] subItems, string imageKey, ListViewGroup group, string expectedText)
        {
            var item = new ListViewItem(subItems, imageKey, group);
            Assert.Equal(SystemColors.Window, item.BackColor);
            Assert.Equal(Rectangle.Empty, item.Bounds);
            Assert.False(item.Checked);
            Assert.False(item.Focused);
            Assert.Equal(Control.DefaultFont, item.Font);
            Assert.Equal(SystemColors.WindowText, item.ForeColor);
            Assert.Equal(group, item.Group);
            Assert.Equal(-1, item.ImageIndex);
            Assert.Equal(imageKey ?? string.Empty, item.ImageKey);
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

        public static IEnumerable<object[]> Ctor_ListViewSubItemArray_Int_ListViewGroup_TestData()
        {
            yield return new object[] { new ListViewItem.ListViewSubItem[0], 0, null, string.Empty };
            yield return new object[] { new ListViewItem.ListViewSubItem[] { new ListViewItem.ListViewSubItem(null, "text") }, 1, null, "text" };
            yield return new object[] { new ListViewItem.ListViewSubItem[] { new ListViewItem.ListViewSubItem(null, "text") }, -1, new ListViewGroup(), "text" };
        }

        [Theory]
        [MemberData(nameof(Ctor_ListViewSubItemArray_Int_ListViewGroup_TestData))]
        public void ListViewItem_Ctor_ListViewSubItemArray_Int_ListViewGroup(ListViewItem.ListViewSubItem[] subItems, int imageIndex, ListViewGroup group, string expectedText)
        {
            var item = new ListViewItem(subItems, imageIndex, group);
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
            Assert.Equal(expectedText, item.Text);
            Assert.Empty(item.ToolTipText);
            Assert.True(item.UseItemStyleForSubItems);
        }

        public static IEnumerable<object[]> Ctor_StringArray_String_ListViewGroup_TestData()
        {
            yield return new object[] { null, null, null, "" };
            yield return new object[] { new string[0], null, null, "" };
            yield return new object[] { new string[] { null }, "", new ListViewGroup(), "" };
            yield return new object[] { new string[] { "text" }, "imageKey", new ListViewGroup(), "text" };
        }

        [Theory]
        [MemberData(nameof(Ctor_StringArray_String_ListViewGroup_TestData))]
        public void ListViewItem_Ctor_StringArray_String_ListViewGroup(string[] subItems, string imageKey, ListViewGroup group, string expectedText)
        {
            var item = new ListViewItem(subItems, imageKey, group);
            Assert.Equal(SystemColors.Window, item.BackColor);
            Assert.Equal(Rectangle.Empty, item.Bounds);
            Assert.False(item.Checked);
            Assert.False(item.Focused);
            Assert.Equal(Control.DefaultFont, item.Font);
            Assert.Equal(SystemColors.WindowText, item.ForeColor);
            Assert.Equal(group, item.Group);
            Assert.Equal(-1, item.ImageIndex);
            Assert.Equal(imageKey ?? string.Empty, item.ImageKey);
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

        public static IEnumerable<object[]> Ctor_StringArray_Int_ListViewGroup_TestData()
        {
            yield return new object[] { null, -1, null, "" };
            yield return new object[] { new string[0], 0, null, "" };
            yield return new object[] { new string[] { null }, 1, new ListViewGroup(), "" };
            yield return new object[] { new string[] { "text" }, 2, new ListViewGroup(), "text" };
        }

        [Theory]
        [MemberData(nameof(Ctor_StringArray_Int_ListViewGroup_TestData))]
        public void ListViewItem_Ctor_StringArray_Int_ListViewGroup(string[] subItems, int imageIndex, ListViewGroup group, string expectedText)
        {
            var item = new ListViewItem(subItems, imageIndex, group);
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
            Assert.Equal(expectedText, item.Text);
            Assert.Empty(item.ToolTipText);
            Assert.True(item.UseItemStyleForSubItems);
        }

        public static IEnumerable<object[]> Ctor_String_String_ListViewGroup_TestData()
        {
            yield return new object[] { null, null, null };
            yield return new object[] { "", "", new ListViewGroup() };
            yield return new object[] { "text", "imageKey", new ListViewGroup() };
        }

        [Theory]
        [MemberData(nameof(Ctor_String_String_ListViewGroup_TestData))]
        public void ListViewItem_Ctor_String_String_ListViewGroup(string text, string imageKey, ListViewGroup group)
        {
            var item = new ListViewItem(text, imageKey, group);
            Assert.Equal(SystemColors.Window, item.BackColor);
            Assert.Equal(Rectangle.Empty, item.Bounds);
            Assert.False(item.Checked);
            Assert.False(item.Focused);
            Assert.Equal(Control.DefaultFont, item.Font);
            Assert.Equal(SystemColors.WindowText, item.ForeColor);
            Assert.Equal(group, item.Group);
            Assert.Equal(-1, item.ImageIndex);
            Assert.Equal(imageKey ?? string.Empty, item.ImageKey);
            Assert.Null(item.ImageList);
            Assert.Equal(0, item.IndentCount);
            Assert.Equal(-1, item.Index);
            Assert.Null(item.ListView);
            Assert.Empty(item.Name);
            Assert.Equal(new Point(-1, -1), item.Position);
            Assert.False(item.Selected);
            Assert.Equal(-1, item.StateImageIndex);
            ListViewItem.ListViewSubItem subItem = Assert.Single(item.SubItems.Cast<ListViewItem.ListViewSubItem>());
            Assert.Equal(text ?? string.Empty, subItem.Text);
            Assert.Same(item.SubItems, item.SubItems);
            Assert.Null(item.Tag);
            Assert.Equal(text ?? string.Empty, item.Text);
            Assert.Empty(item.ToolTipText);
            Assert.True(item.UseItemStyleForSubItems);
        }

        public static IEnumerable<object[]> Ctor_String_Int_ListViewGroup_TestData()
        {
            yield return new object[] { null, -1, null };
            yield return new object[] { "", 0, new ListViewGroup() };
            yield return new object[] { "text", 1, new ListViewGroup() };
        }

        [Theory]
        [MemberData(nameof(Ctor_String_Int_ListViewGroup_TestData))]
        public void ListViewItem_Ctor_String_Int_ListViewGroup(string text, int imageIndex, ListViewGroup group)
        {
            var item = new ListViewItem(text, imageIndex, group);
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
            Assert.Equal(text ?? string.Empty, subItem.Text);
            Assert.Same(item.SubItems, item.SubItems);
            Assert.Null(item.Tag);
            Assert.Equal(text ?? string.Empty, item.Text);
            Assert.Empty(item.ToolTipText);
            Assert.True(item.UseItemStyleForSubItems);
        }

        public static IEnumerable<object[]> Ctor_ListViewSubItemArray_String_TestData()
        {
            yield return new object[] { new ListViewItem.ListViewSubItem[0], null, string.Empty };
            yield return new object[] { new ListViewItem.ListViewSubItem[] { new ListViewItem.ListViewSubItem(null, "text") }, "", "text" };
            yield return new object[] { new ListViewItem.ListViewSubItem[] { new ListViewItem.ListViewSubItem(null, "text") }, "imageKey", "text" };
        }

        [Theory]
        [MemberData(nameof(Ctor_ListViewSubItemArray_String_TestData))]
        public void ListViewItem_Ctor_ListViewSubItemArray_String(ListViewItem.ListViewSubItem[] subItems, string imageKey, string expectedText)
        {
            var item = new ListViewItem(subItems, imageKey);
            Assert.Equal(SystemColors.Window, item.BackColor);
            Assert.Equal(Rectangle.Empty, item.Bounds);
            Assert.False(item.Checked);
            Assert.False(item.Focused);
            Assert.Equal(Control.DefaultFont, item.Font);
            Assert.Equal(SystemColors.WindowText, item.ForeColor);
            Assert.Null(item.Group);
            Assert.Equal(-1, item.ImageIndex);
            Assert.Equal(imageKey ?? string.Empty, item.ImageKey);
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

        public static IEnumerable<object[]> Ctor_ListViewSubItemArray_Int_TestData()
        {
            yield return new object[] { new ListViewItem.ListViewSubItem[0], 0, string.Empty };
            yield return new object[] { new ListViewItem.ListViewSubItem[] { new ListViewItem.ListViewSubItem(null, "text") }, 1, "text" };
            yield return new object[] { new ListViewItem.ListViewSubItem[] { new ListViewItem.ListViewSubItem(null, "text") }, -1, "text" };
        }

        [Theory]
        [MemberData(nameof(Ctor_ListViewSubItemArray_Int_TestData))]
        public void ListViewItem_Ctor_ListViewSubItemArray_Int(ListViewItem.ListViewSubItem[] subItems, int imageIndex, string expectedText)
        {
            var item = new ListViewItem(subItems, imageIndex);
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

        public static IEnumerable<object[]> Ctor_StringArray_ListViewGroup_TestData()
        {
            yield return new object[] { null, null, "" };
            yield return new object[] { new string[0], null, "" };
            yield return new object[] { new string[] { null }, new ListViewGroup(), "" };
            yield return new object[] { new string[] { "text" }, new ListViewGroup(), "text" };
        }

        [Theory]
        [MemberData(nameof(Ctor_StringArray_ListViewGroup_TestData))]
        public void ListViewItem_Ctor_StringArray_ListViewGroup(string[] subItems, ListViewGroup group, string expectedText)
        {
            var item = new ListViewItem(subItems, group);
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
            Assert.Equal(expectedText, item.Text);
            Assert.Empty(item.ToolTipText);
            Assert.True(item.UseItemStyleForSubItems);
        }

        public static IEnumerable<object[]> Ctor_StringArray_String_TestData()
        {
            yield return new object[] { null, null, "" };
            yield return new object[] { new string[0], null, "" };
            yield return new object[] { new string[] { null }, "", "" };
            yield return new object[] { new string[] { "text" }, "imageKey",  "text" };
        }

        [Theory]
        [MemberData(nameof(Ctor_StringArray_String_TestData))]
        public void ListViewItem_Ctor_StringArray_String(string[] subItems, string imageKey, string expectedText)
        {
            var item = new ListViewItem(subItems, imageKey);
            Assert.Equal(SystemColors.Window, item.BackColor);
            Assert.Equal(Rectangle.Empty, item.Bounds);
            Assert.False(item.Checked);
            Assert.False(item.Focused);
            Assert.Equal(Control.DefaultFont, item.Font);
            Assert.Equal(SystemColors.WindowText, item.ForeColor);
            Assert.Null(item.Group);
            Assert.Equal(-1, item.ImageIndex);
            Assert.Equal(imageKey ?? string.Empty, item.ImageKey);
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
            yield return new object[] { null, -1, "" };
            yield return new object[] { new string[0], 0, "" };
            yield return new object[] { new string[] { null }, 1, "" };
            yield return new object[] { new string[] { "text" }, 2,  "text" };
        }

        [Theory]
        [MemberData(nameof(Ctor_StringArray_Int_TestData))]
        public void ListViewItem_Ctor_StringArray_Int(string[] subItems, int imageIndex, string expectedText)
        {
            var item = new ListViewItem(subItems, imageIndex);
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
            yield return new object[] { null, null };
            yield return new object[] { "", null };
            yield return new object[] { "text", new ListViewGroup() };
        }

        [Theory]
        [MemberData(nameof(Ctor_String_ListViewGroup_TestData))]
        public void ListViewItem_Ctor_String_ListViewGroup(string text, ListViewGroup group)
        {
            var item = new ListViewItem(text, group);
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
            Assert.Equal(text ?? string.Empty, subItem.Text);
            Assert.Same(item.SubItems, item.SubItems);
            Assert.Null(item.Tag);
            Assert.Equal(text ?? string.Empty, item.Text);
            Assert.Empty(item.ToolTipText);
            Assert.True(item.UseItemStyleForSubItems);
        }

        public static IEnumerable<object[]> Ctor_String_String_TestData()
        {
            yield return new object[] { null, null };
            yield return new object[] { "", "" };
            yield return new object[] { "text", "imageKey" };
        }

        [Theory]
        [MemberData(nameof(Ctor_String_String_TestData))]
        public void ListViewItem_Ctor_String_String(string text, string imageKey)
        {
            var item = new ListViewItem(text, imageKey);
            Assert.Equal(SystemColors.Window, item.BackColor);
            Assert.Equal(Rectangle.Empty, item.Bounds);
            Assert.False(item.Checked);
            Assert.False(item.Focused);
            Assert.Equal(Control.DefaultFont, item.Font);
            Assert.Equal(SystemColors.WindowText, item.ForeColor);
            Assert.Null(item.Group);
            Assert.Equal(-1, item.ImageIndex);
            Assert.Equal(imageKey ?? string.Empty, item.ImageKey);
            Assert.Null(item.ImageList);
            Assert.Equal(0, item.IndentCount);
            Assert.Equal(-1, item.Index);
            Assert.Null(item.ListView);
            Assert.Empty(item.Name);
            Assert.Equal(new Point(-1, -1), item.Position);
            Assert.False(item.Selected);
            Assert.Equal(-1, item.StateImageIndex);
            ListViewItem.ListViewSubItem subItem = Assert.Single(item.SubItems.Cast<ListViewItem.ListViewSubItem>());
            Assert.Equal(text ?? string.Empty, subItem.Text);
            Assert.Same(item.SubItems, item.SubItems);
            Assert.Null(item.Tag);
            Assert.Equal(text ?? string.Empty, item.Text);
            Assert.Empty(item.ToolTipText);
            Assert.True(item.UseItemStyleForSubItems);
        }

        public static IEnumerable<object[]> Ctor_String_Int_TestData()
        {
            yield return new object[] { null, -1 };
            yield return new object[] { "", 0 };
            yield return new object[] { "text", 1 };
        }

        [Theory]
        [MemberData(nameof(Ctor_String_Int_TestData))]
        public void ListViewItem_Ctor_String_Int(string text, int imageIndex)
        {
            var item = new ListViewItem(text, imageIndex);
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
            Assert.Equal(text ?? string.Empty, subItem.Text);
            Assert.Same(item.SubItems, item.SubItems);
            Assert.Null(item.Tag);
            Assert.Equal(text ?? string.Empty, item.Text);
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
            var item = new ListViewItem(group);
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
            yield return new object[] { null, "" };
            yield return new object[] { new string[0], "" };
            yield return new object[] { new string[] { null }, "" };
            yield return new object[] { new string[] { "text" }, "text" };
        }

        [Theory]
        [MemberData(nameof(Ctor_StringArray_TestData))]
        public void ListViewItem_Ctor_StringArray(string[] subItems, string expectedText)
        {
            var item = new ListViewItem(subItems);
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
        [CommonMemberData(nameof(CommonTestHelper.GetStringWithNullTheoryData))]
        public void ListViewItem_Ctor_String(string text)
        {
            var item = new ListViewItem(text);
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
            Assert.Equal(text ?? string.Empty, subItem.Text);
            Assert.Same(item.SubItems, item.SubItems);
            Assert.Null(item.Tag);
            Assert.Equal(text ?? string.Empty, item.Text);
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

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetColorTheoryData))]
        public void ListViewItem_BackColor_Set_GetReturnsExpected(Color value)
        {
            var item = new ListViewItem
            {
                BackColor = value
            };
            Assert.Equal(value, item.BackColor);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetColorTheoryData))]
        public void ListViewItem_ForeColor_Set_GetReturnsExpected(Color value)
        {
            var item = new ListViewItem
            {
                ForeColor = value
            };
            Assert.Equal(value, item.ForeColor);
        }

        [Fact]
        public void ListViewItem_EnsureVisible_HasListViewWithoutHandle_Nop()
        {
            var listView = new ListView();
            var item = new ListViewItem();
            listView.Items.Add(item);
            item.EnsureVisible();
        }

        [Fact]
        public void ListViewItem_EnsureVisible_NoListView_Nop()
        {
            var item = new ListViewItem();
            item.EnsureVisible();
        }

        [Fact]
        public void ListViewItem_Remove_HasListView_Success()
        {
            var listView = new ListView();
            var item = new ListViewItem();
            listView.Items.Add(item);
            item.Remove();
            Assert.Empty(listView.Items);
            Assert.Null(item.ListView);
        }

        [Fact]
        public void ListViewItem_Remove_NoListView_Nop()
        {
            var item = new ListViewItem();
            item.Remove();
        }

        private static void AssertEqualListViewSubItem(string[] expected, ListViewItem.ListViewSubItem[] actual)
        {
            AssertEqualListViewSubItem(expected?.Select(t => new ListViewItem.ListViewSubItem(null, t)).ToArray(), actual);
        }

        private static void AssertEqualListViewSubItem(ListViewItem.ListViewSubItem[] expected, ListViewItem.ListViewSubItem[] actual)
        {
            if (expected == null || expected.Length == 0)
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
    }
}
