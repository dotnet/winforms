﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Drawing;
using WinForms.Common.Tests;
using Xunit;

namespace System.Windows.Forms.Tests
{
    public class DrawListViewColumnHeaderEventArgsTests : IClassFixture<ThreadExceptionFixture>
    {
        public static IEnumerable<object[]> Ctor_Graphics_Rectangle_Int_ColumnHeader_ListViewItemStates_Color_Color_Font_TestData()
        {
            yield return new object[] { Rectangle.Empty, -2, null, (DrawItemState)(ListViewItemStates.Checked - 1), Color.Empty, Color.Empty, null };
            yield return new object[] { new Rectangle(1, 2, 3, 4), -1, new ColumnHeader(), ListViewItemStates.Checked, Color.Red, Color.Blue, SystemFonts.DefaultFont };
            yield return new object[] { new Rectangle(-1, 2, -3, -4), 0, new ColumnHeader(), ListViewItemStates.Checked, Color.Red, Color.Blue, SystemFonts.DefaultFont };
            yield return new object[] { new Rectangle(1, 2, 3, 4), 1, new ColumnHeader(), ListViewItemStates.Checked, Color.Red, Color.Blue, SystemFonts.DefaultFont };
        }

        [Theory]
        [MemberData(nameof(Ctor_Graphics_Rectangle_Int_ColumnHeader_ListViewItemStates_Color_Color_Font_TestData))]
        public void DrawListViewColumnHeaderEventArgs_Ctor_Graphics_Rectangle_Int_ColumnHeader_ListViewItemStates_Color_Color_Font(Rectangle bounds, int columnIndex, ColumnHeader header, ListViewItemStates state, Color foreColor, Color backColor, Font font)
        {
            using (var image = new Bitmap(10, 10))
            using (Graphics graphics = Graphics.FromImage(image))
            {
                var e = new DrawListViewColumnHeaderEventArgs(graphics, bounds, columnIndex, header, state, foreColor, backColor, font);
                Assert.Same(graphics, e.Graphics);
                Assert.Equal(bounds, e.Bounds);
                Assert.Equal(columnIndex, e.ColumnIndex);
                Assert.Same(header, e.Header);
                Assert.Equal(state, e.State);
                Assert.Equal(foreColor, e.ForeColor);
                Assert.Equal(backColor, e.BackColor);
                Assert.Equal(font, e.Font);
                Assert.False(e.DrawDefault);
            }
        }

        [Fact]
        public void DrawListViewColumnHeaderEventArgs_Ctor_NullGraphics_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>("graphics", () => new DrawListViewColumnHeaderEventArgs(null, new Rectangle(1, 2, 3, 4), 0, new ColumnHeader(), ListViewItemStates.Default, Color.Red, Color.Blue, SystemFonts.DefaultFont));
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetBoolTheoryData))]
        public void DrawListViewColumnHeaderEventArgs_DrawDefault_Set_GetReturnsExpected(bool value)
        {
            using (var image = new Bitmap(10, 10))
            using (Graphics graphics = Graphics.FromImage(image))
            {
                var e = new DrawListViewColumnHeaderEventArgs(graphics, new Rectangle(1, 2, 3, 4), -1, new ColumnHeader(), ListViewItemStates.Checked, Color.Red, Color.Blue, SystemFonts.DefaultFont)
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
        }

        public static IEnumerable<object[]> Draw_TestData()
        {
            yield return new object[] { new Rectangle(1, 2, 3, 4), new ColumnHeader(), ListViewItemStates.Default, Color.Red, Color.Blue, SystemFonts.DefaultFont };
            yield return new object[] { new Rectangle(1, 2, 3, 4), new ColumnHeader(), ListViewItemStates.Default, Color.Red, Color.Blue, SystemFonts.DefaultFont };
            yield return new object[] { new Rectangle(1, 2, 3, 4), new ColumnHeader(), ListViewItemStates.Default, Color.Red, Color.Blue, SystemFonts.DefaultFont };
        }

        [Theory]
        [MemberData(nameof(Draw_TestData))]
        public void DrawListViewColumnHeaderEventArgs_DrawBackground_Invoke_Success(Rectangle bounds, ColumnHeader header, ListViewItemStates state, Color foreColor, Color backColor, Font font)
        {
            using (var image = new Bitmap(10, 10))
            using (Graphics graphics = Graphics.FromImage(image))
            {
                var e = new DrawListViewColumnHeaderEventArgs(graphics, bounds, -1, header, state, foreColor, backColor, font);
                e.DrawBackground();
            }
        }

        [Theory]
        [InlineData(HorizontalAlignment.Left)]
        [InlineData(HorizontalAlignment.Center)]
        [InlineData(HorizontalAlignment.Right)]
        public void DrawListViewColumnHeaderEventArgs_DrawText_Invoke_Success(HorizontalAlignment textAlign)
        {
            using (var image = new Bitmap(10, 10))
            using (Graphics graphics = Graphics.FromImage(image))
            {
                var header = new ColumnHeader { TextAlign = textAlign };
                var e = new DrawListViewColumnHeaderEventArgs(graphics, new Rectangle(1, 2, 3, 4), -1, header, ListViewItemStates.Checked, Color.Red, Color.Blue, SystemFonts.DefaultFont);
                e.DrawText();
            }
        }

        [Fact]
        public void DrawListViewColumnHeaderEventArgs_DrawText_InvokeTextFormatFlags_Success()
        {
            using (var image = new Bitmap(10, 10))
            using (Graphics graphics = Graphics.FromImage(image))
            {
                var e = new DrawListViewColumnHeaderEventArgs(graphics, new Rectangle(1, 2, 3, 4), -1, new ColumnHeader(), ListViewItemStates.Checked, Color.Red, Color.Blue, SystemFonts.DefaultFont);
                e.DrawText(TextFormatFlags.Bottom);
            }
        }

        public static IEnumerable<object[]> NullHeader_TestData()
        {
            yield return new object[] { new Rectangle(-1, -2, -3, -4), 0, ListViewItemStates.Default, Color.Empty, Color.Empty, null };
            yield return new object[] { new Rectangle(1, 2, 3, 4), -1, ListViewItemStates.Checked, Color.Red, Color.Blue, SystemFonts.DefaultFont };
        }

        [Theory]
        [MemberData(nameof(NullHeader_TestData))]
        public void DrawListViewColumnHeaderEventArgs_DrawText_NullHeader_Success(Rectangle bounds, int columnIndex, ListViewItemStates state, Color foreColor, Color backColor, Font font)
        {
            using (var image = new Bitmap(10, 10))
            using (Graphics graphics = Graphics.FromImage(image))
            {
                var e = new DrawListViewColumnHeaderEventArgs(graphics, bounds, columnIndex, null, state, foreColor, backColor, font);
                e.DrawText();
                e.DrawText(TextFormatFlags.Left);
            }
        }
    }
}
