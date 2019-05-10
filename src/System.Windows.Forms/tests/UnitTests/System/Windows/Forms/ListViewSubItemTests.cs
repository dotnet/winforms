﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using WinForms.Common.Tests;
using Xunit;

namespace System.Windows.Forms.Tests
{
    public class ListViewSubItemTests
    {
        [Fact]
        public void ListViewSubItem_Ctor_Default()
        {
            var subItem = new ListViewItem.ListViewSubItem();
            Assert.Equal(SystemColors.Window, subItem.BackColor);
            Assert.Equal(Rectangle.Empty, subItem.Bounds);
            Assert.Equal(Control.DefaultFont, subItem.Font);
            Assert.Equal(SystemColors.WindowText, subItem.ForeColor);
            Assert.Empty(subItem.Name);
            Assert.Null(subItem.Tag);
            Assert.Empty(subItem.Text);
        }

        public static IEnumerable<object[]> Ctor_ListViewItem_String_TestData()
        {
            yield return new object[] { null, null, string.Empty };
            yield return new object[] { new ListViewItem(), string.Empty, string.Empty };
            yield return new object[] { new ListViewItem(), "reasonable", "reasonable" };
            yield return new object[] { new ListViewItem() { BackColor = Color.Yellow, ForeColor = Color.Yellow, Font = SystemFonts.StatusFont }, "reasonable", "reasonable" };

            var listView = new ListView();
            var item = new ListViewItem();
            Assert.Null(item.ListView);
            yield return new object[] { item, "reasonable", "reasonable" };
        }

        [Theory]
        [MemberData(nameof(Ctor_ListViewItem_String_TestData))]
        public void ListViewSubItem_Ctor_ListViewItem_String(ListViewItem owner, string text, string expectedText)
        {
            var subItem = new ListViewItem.ListViewSubItem(owner, text);
            Assert.Equal(SystemColors.Window, subItem.BackColor);
            Assert.Equal(Rectangle.Empty, subItem.Bounds);
            Assert.Equal(Control.DefaultFont, subItem.Font);
            Assert.Equal(SystemColors.WindowText, subItem.ForeColor);
            Assert.Empty(subItem.Name);
            Assert.Null(subItem.Tag);
            Assert.Equal(expectedText, subItem.Text);
        }

        public static IEnumerable<object[]> Ctor_ListViewItem_String_Color_Color_Font_TestData()
        {
            yield return new object[] { null, null, Color.Empty, Color.Empty, null, SystemColors.WindowText, SystemColors.Window, string.Empty };
            yield return new object[] { new ListViewItem(), string.Empty, Color.Red, Color.Blue, SystemFonts.MenuFont, Color.Red, Color.Blue, string.Empty };
            yield return new object[] { new ListViewItem(), "reasonable", Color.Red, Color.Blue, SystemFonts.MenuFont, Color.Red, Color.Blue, "reasonable" };
            yield return new object[] { new ListViewItem() { BackColor = Color.Yellow, ForeColor = Color.Yellow, Font = SystemFonts.StatusFont }, string.Empty, Color.Red, Color.Blue, SystemFonts.MenuFont, Color.Red, Color.Blue, string.Empty };

            var listView = new ListView();
            var item = new ListViewItem();
            Assert.Null(item.ListView);
            yield return new object[] { item, "reasonable", Color.Red, Color.Blue, SystemFonts.MenuFont, Color.Red, Color.Blue, "reasonable" };
        }

        [Theory]
        [MemberData(nameof(Ctor_ListViewItem_String_Color_Color_Font_TestData))]
        public void ListViewSubItem_Ctor_ListViewItem_String_Color_Color_Font(ListViewItem owner, string text, Color foreColor, Color backColor, Font font, Color expectedForeColor, Color expectedBackColor, string expectedText)
        {
            var subItem = new ListViewItem.ListViewSubItem(owner, text, foreColor, backColor, font);
            Assert.Equal(expectedBackColor, subItem.BackColor);
            Assert.Equal(Rectangle.Empty, subItem.Bounds);
            Assert.Equal(font ?? Control.DefaultFont, subItem.Font);
            Assert.Equal(expectedForeColor, subItem.ForeColor);
            Assert.Empty(subItem.Name);
            Assert.Null(subItem.Tag);
            Assert.Equal(expectedText, subItem.Text);
        }

        [Fact]
        public void ListViewSubItem_BackColor_GetWithListView_ReturnsEqual()
        {
            var listView = new ListView
            {
                BackColor = Color.Red
            };
            var item = new ListViewItem();
            listView.Items.Add(item);

            var subItem = new ListViewItem.ListViewSubItem(item, "text");
            Assert.Equal(Color.Red, subItem.BackColor);
        }

        public static IEnumerable<object[]> BackColor_Set_TestData()
        {
            yield return new object[] { Color.Red, Color.Red };
            yield return new object[] { Color.Empty, SystemColors.Window };
        }

        [Theory]
        [MemberData(nameof(BackColor_Set_TestData))]
        public void ListViewSubItem_BackColor_Set_GetReturnsExpected(Color value, Color expected)
        {
            var subItem = new ListViewItem.ListViewSubItem
            {
                BackColor = value
            };
            Assert.Equal(expected, subItem.BackColor);
            
            // Set same.
            subItem.BackColor = value;
            Assert.Equal(expected, subItem.BackColor);
        }

        [Fact]
        public void ListViewSubItem_Bounds_GetWithListViewHandle_ReturnsEqual()
        {
            var listView = new ListView();
            var item = new ListViewItem();
            listView.Items.Add(item);
            Assert.NotEqual(IntPtr.Zero, listView.Handle);

            var subItem = new ListViewItem.ListViewSubItem(item, "text");
            Assert.Equal(subItem.Bounds, subItem.Bounds);
        }

        [Fact]
        public void ListViewSubItem_Font_GetWithListView_ReturnsEqual()
        {
            var listView = new ListView
            {
                Font = SystemFonts.MenuFont
            };
            var item = new ListViewItem();
            listView.Items.Add(item);

            var subItem = new ListViewItem.ListViewSubItem(item, "text");
            Assert.Equal(SystemFonts.MenuFont, subItem.Font);
        }

        public static IEnumerable<object[]> Font_Set_TestData()
        {
            yield return new object[] { SystemFonts.MenuFont };
            yield return new object[] { null };
        }

        [Theory]
        [MemberData(nameof(Font_Set_TestData))]
        public void ListViewSubItem_Font_Set_GetReturnsExpected(Font value)
        {
            var subItem = new ListViewItem.ListViewSubItem
            {
                Font = value
            };
            Assert.Equal(value ?? Control.DefaultFont, subItem.Font);
            
            // Set same.
            subItem.Font = value;
            Assert.Equal(value ?? Control.DefaultFont, subItem.Font);
        }

        [Fact]
        public void ListViewSubItem_ForeColor_GetWithListView_ReturnsEqual()
        {
            var listView = new ListView
            {
                ForeColor = Color.Red
            };
            var item = new ListViewItem();
            listView.Items.Add(item);

            var subItem = new ListViewItem.ListViewSubItem(item, "text");
            Assert.Equal(Color.Red, subItem.ForeColor);
        }

        public static IEnumerable<object[]> ForeColor_Set_TestData()
        {
            yield return new object[] { Color.Red, Color.Red };
            yield return new object[] { Color.Empty, SystemColors.WindowText };
        }

        [Theory]
        [MemberData(nameof(ForeColor_Set_TestData))]
        public void ListViewSubItem_ForeColor_Set_GetReturnsExpected(Color value, Color expected)
        {
            var subItem = new ListViewItem.ListViewSubItem
            {
                ForeColor = value
            };
            Assert.Equal(expected, subItem.ForeColor);
            
            // Set same.
            subItem.ForeColor = value;
            Assert.Equal(expected, subItem.ForeColor);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetStringNormalizedTheoryData))]
        public void ListViewSubItem_Name_SetWithOwner_GetReturnsExpected(string value, string expected)
        {
            var item = new ListViewItem();
            var subItem = new ListViewItem.ListViewSubItem(item, "text")
            {
                Name = value
            };
            Assert.Same(expected, subItem.Name);

            // Set same.
            subItem.Name = value;
            Assert.Same(expected, subItem.Name);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetStringNormalizedTheoryData))]
        public void ListViewSubItem_Name_SetWithoutOwner_GetReturnsExpected(string value, string expected)
        {
            var subItem = new ListViewItem.ListViewSubItem
            {
                Name = value
            };
            Assert.Same(expected, subItem.Name);

            // Set same.
            subItem.Name = value;
            Assert.Same(expected, subItem.Name);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetStringWithNullTheoryData))]
        public void ListViewSubItem_Tag_Set_GetReturnsExpected(string value)
        {
            var subItem = new ListViewItem.ListViewSubItem
            {
                Tag = value
            };
            Assert.Same(value, subItem.Tag);

            // Set same.
            subItem.Tag = value;
            Assert.Same(value, subItem.Tag);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetStringNormalizedTheoryData))]
        public void ListViewSubItem_Text_SetWithOwner_GetReturnsExpected(string value, string expected)
        {
            var item = new ListViewItem();
            var subItem = new ListViewItem.ListViewSubItem(item, "text")
            {
                Text = value
            };
            Assert.Same(expected, subItem.Text);

            // Set same.
            subItem.Text = value;
            Assert.Same(expected, subItem.Text);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetStringNormalizedTheoryData))]
        public void ListViewSubItem_Text_SetWithoutOwner_GetReturnsExpected(string value, string expected)
        {
            var subItem = new ListViewItem.ListViewSubItem
            {
                Text = value
            };
            Assert.Same(expected, subItem.Text);

            // Set same.
            subItem.Text = value;
            Assert.Same(expected, subItem.Text);
        }

        public static IEnumerable<object[]> ResetStyle_Owner_TestData()
        {
            yield return new object[] { null };
            yield return new object[] { new ListViewItem() };
        }

        [Theory]
        [MemberData(nameof(ResetStyle_Owner_TestData))]
        public void ListViewSubItem_ResetStyle_NoStyle_Nop(ListViewItem owner)
        {
            var subItem = new ListViewItem.ListViewSubItem(owner, "text");
            subItem.ResetStyle();
            Assert.Equal(SystemColors.Window, subItem.BackColor);
            Assert.Equal(Control.DefaultFont, subItem.Font);
            Assert.Equal(SystemColors.WindowText, subItem.ForeColor);

            subItem.ResetStyle();
            Assert.Equal(SystemColors.Window, subItem.BackColor);
            Assert.Equal(Control.DefaultFont, subItem.Font);
            Assert.Equal(SystemColors.WindowText, subItem.ForeColor);
        }

        [Theory]
        [MemberData(nameof(ResetStyle_Owner_TestData))]
        public void ListViewSubItem_ResetStyle_HasStyleWithOwner_Success(ListViewItem owner)
        {
            var subItem = new ListViewItem.ListViewSubItem(owner, "text", Color.Red, Color.Blue, SystemFonts.MenuFont);
            
            subItem.ResetStyle();
            Assert.Equal(SystemColors.Window, subItem.BackColor);
            Assert.Equal(Control.DefaultFont, subItem.Font);
            Assert.Equal(SystemColors.WindowText, subItem.ForeColor);

            subItem.ResetStyle();
            Assert.Equal(SystemColors.Window, subItem.BackColor);
            Assert.Equal(Control.DefaultFont, subItem.Font);
            Assert.Equal(SystemColors.WindowText, subItem.ForeColor);
        }

        public static IEnumerable<object[]> Serialize_Deserialize_TestData()
        {
            yield return new object[] { new ListViewItem.ListViewSubItem() };
            yield return new object[] { new ListViewItem.ListViewSubItem(null, "header", Color.Red, Color.Blue, SystemFonts.MenuFont) { Name = "name", Tag = "tag" } };
        }

        [Theory]
        [MemberData(nameof(Serialize_Deserialize_TestData))]
        public void ListViewSubItem_Serialize_Deserialize_Success(ListViewItem.ListViewSubItem subItem)
        {
            using (var stream = new MemoryStream())
            {
                var formatter = new BinaryFormatter();
                formatter.Serialize(stream, subItem);
                stream.Seek(0, SeekOrigin.Begin);
                
                ListViewItem.ListViewSubItem result = Assert.IsType<ListViewItem.ListViewSubItem>(formatter.Deserialize(stream));
                Assert.Equal(subItem.BackColor, result.BackColor);
                Assert.Equal(subItem.Font, result.Font);
                Assert.Equal(subItem.ForeColor, result.ForeColor);
                Assert.Empty(result.Name);
                Assert.Null(result.Tag);
                Assert.Equal(subItem.Text, result.Text);
            }
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetStringWithNullTheoryData))]
        public void ListViewSubItem_ToString_Invoke_ReturnsExpected(string text)
        {
            var subItem = new ListViewItem.ListViewSubItem(null, text);
            Assert.Equal($"ListViewSubItem: {{{text}}}", subItem.ToString());
        }
    }
}
