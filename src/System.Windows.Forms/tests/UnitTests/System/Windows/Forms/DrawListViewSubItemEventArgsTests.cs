// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Drawing;
using Xunit;

namespace System.Windows.Forms.Tests
{
    public class DrawListViewSubItemEventArgsTests
    {
        public static IEnumerable<object[]> Ctor_Graphics_ListViewItem_Rectangle_Int_ListViewItemStates_TestData()
        {
            var image = new Bitmap(10, 10);
            Graphics graphics = Graphics.FromImage(image);

            yield return new object[] { null, Rectangle.Empty, null, null, -2, -2, null, (ListViewItemStates)(ListViewItemStates.Checked - 1) };
            yield return new object[] { graphics, new Rectangle(1, 2, 3, 4), new ListViewItem(), null, -1, -1, new ColumnHeader(), ListViewItemStates.Checked };
            yield return new object[] { graphics, new Rectangle(-1, 2, -3, -4), new ListViewItem(), new ListViewItem.ListViewSubItem(), 0, 0, new ColumnHeader(), ListViewItemStates.Focused };
            yield return new object[] { graphics, new Rectangle(1, 2, 3, 4), new ListViewItem(), new ListViewItem.ListViewSubItem(), 1, 2, new ColumnHeader(), ListViewItemStates.Checked };
        }

        [Theory]
        [MemberData(nameof(Ctor_Graphics_ListViewItem_Rectangle_Int_ListViewItemStates_TestData))]
        public void Ctor_Graphics_ListViewItem_Rectangle_Int_ListViewItemStates(Graphics graphics, Rectangle bounds, ListViewItem item, ListViewItem.ListViewSubItem subItem, int itemIndex, int columnIndex, ColumnHeader header, ListViewItemStates itemState)
        {
            var e = new DrawListViewSubItemEventArgs(graphics, bounds, item, subItem, itemIndex, columnIndex, header, itemState);
            Assert.Equal(graphics, e.Graphics);
            Assert.Equal(bounds, e.Bounds);
            Assert.Equal(item, e.Item);
            Assert.Equal(subItem, e.SubItem);
            Assert.Equal(itemIndex, e.ItemIndex);
            Assert.Equal(columnIndex, e.ColumnIndex);
            Assert.Equal(header, e.Header);
            Assert.Equal(itemState, e.ItemState);
            Assert.False(e.DrawDefault);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void DrawDefault_Set_GetReturnsExpected(bool value)
        {
            using (var image = new Bitmap(10, 10))
            using (Graphics graphics = Graphics.FromImage(image))
            {
                var e = new DrawListViewSubItemEventArgs(graphics, new Rectangle(1, 2, 3, 4), new ListViewItem(), null, -1, -1, new ColumnHeader(), ListViewItemStates.Checked)
                {
                    DrawDefault = value
                };
                Assert.Equal(value, e.DrawDefault);
            }
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(1)]
        public void DrawBackground_HasGraphics_Success(int itemIndex)
        {
            using (var image = new Bitmap(10, 10))
            using (Graphics graphics = Graphics.FromImage(image))
            {
                var e = new DrawListViewSubItemEventArgs(graphics, new Rectangle(1, 2, 3, 4), new ListViewItem(), new ListViewItem.ListViewSubItem(), itemIndex, 0, new ColumnHeader(), ListViewItemStates.Checked);
                e.DrawBackground();
            }
        }

        [Fact]
        public void DrawBackground_NullGraphics_ThrowsNullReferenceException()
        {
            var e = new DrawListViewSubItemEventArgs(null, new Rectangle(1, 2, 3, 4), new ListViewItem(), new ListViewItem.ListViewSubItem(), 0, 0, new ColumnHeader(), ListViewItemStates.Checked);
            Assert.Throws<NullReferenceException>(() => e.DrawBackground());
        }

        [Theory]
        [InlineData(-2)]
        [InlineData(0)]
        [InlineData(1)]
        public void DrawBackground_NullItemNonMinusOneItemIndex_Nop(int itemIndex)
        {
            using (var image = new Bitmap(10, 10))
            using (Graphics graphics = Graphics.FromImage(image))
            {
                var e = new DrawListViewSubItemEventArgs(graphics, new Rectangle(1, 2, 3, 4), null, new ListViewItem.ListViewSubItem(), itemIndex, 0, new ColumnHeader(), ListViewItemStates.Checked);
                e.DrawBackground();
            }
        }

        [Fact]
        public void DrawBackground_NullItemMinusOneItemIndex_ThrowsNullReferenceException()
        {
            using (var image = new Bitmap(10, 10))
            using (Graphics graphics = Graphics.FromImage(image))
            {
                var e = new DrawListViewSubItemEventArgs(graphics, new Rectangle(1, 2, 3, 4), null, new ListViewItem.ListViewSubItem(), -1, 0, new ColumnHeader(), ListViewItemStates.Checked);
                Assert.Throws<NullReferenceException>(() => e.DrawBackground());
            }
        }

        [Theory]
        [InlineData(ListViewItemStates.Checked)]
        [InlineData(ListViewItemStates.Focused)]
        public void DrawFocusRectangle_HasGraphics_Success(ListViewItemStates itemState)
        {
            using (var image = new Bitmap(10, 10))
            using (Graphics graphics = Graphics.FromImage(image))
            {
                var e = new DrawListViewSubItemEventArgs(graphics, new Rectangle(1, 2, 3, 4), new ListViewItem(), new ListViewItem.ListViewSubItem(), 0, 0, new ColumnHeader(), itemState);
                e.DrawFocusRectangle(new Rectangle(1, 2, 3, 4));
            }
        }

        [Fact]
        public void DrawFocusRectangle_NullGraphicsNotFocused_Nop()
        {
            var e = new DrawListViewSubItemEventArgs(null, new Rectangle(1, 2, 3, 4), new ListViewItem(), new ListViewItem.ListViewSubItem(), 0, 0, new ColumnHeader(), ListViewItemStates.Checked);
            e.DrawFocusRectangle(new Rectangle(1, 2, 3, 4));
        }

        [Fact]
        public void DrawFocusRectangle_NullGraphicsFocused_ThrowsArgumentNullException()
        {
            var e = new DrawListViewSubItemEventArgs(null, new Rectangle(1, 2, 3, 4), new ListViewItem(), new ListViewItem.ListViewSubItem(), 0, 0, new ColumnHeader(), ListViewItemStates.Focused);
            Assert.Throws<ArgumentNullException>("graphics", () => e.DrawFocusRectangle(new Rectangle(1, 2, 3, 4)));
        }

        [Fact]
        public void DrawFocusRectangle_NullItemNotFocused_Nop()
        {
            using (var image = new Bitmap(10, 10))
            using (Graphics graphics = Graphics.FromImage(image))
            {
                var e = new DrawListViewSubItemEventArgs(graphics, new Rectangle(1, 2, 3, 4), null, new ListViewItem.ListViewSubItem(), 0, 0, new ColumnHeader(), ListViewItemStates.Checked);
                e.DrawFocusRectangle(new Rectangle(1, 2, 3, 4));
            }
        }

        [Fact]
        public void DrawFocusRectangle_NullItemFocused_ThrowsNullReferenceException()
        {
            using (var image = new Bitmap(10, 10))
            using (Graphics graphics = Graphics.FromImage(image))
            {
                var e = new DrawListViewSubItemEventArgs(graphics, new Rectangle(1, 2, 3, 4), null, new ListViewItem.ListViewSubItem(), 0, 0, new ColumnHeader(), ListViewItemStates.Focused);
                Assert.Throws<NullReferenceException>(() => e.DrawFocusRectangle(new Rectangle(1, 2, 3, 4)));
            }
        }

        public static IEnumerable<object[]> DrawText_HasGraphicsWithoutFlags_TestData()
        {
            yield return new object[] { -1, HorizontalAlignment.Left };
            yield return new object[] { -1, HorizontalAlignment.Center };
            yield return new object[] { -1, HorizontalAlignment.Right };
            yield return new object[] { 1, HorizontalAlignment.Left };
            yield return new object[] { 1, HorizontalAlignment.Center };
            yield return new object[] { 1, HorizontalAlignment.Right };
        }

        [Theory]
        [MemberData(nameof(DrawText_HasGraphicsWithoutFlags_TestData))]
        public void DrawText_HasGraphicsWithoutFlags_Success(int itemIndex, HorizontalAlignment textAlign)
        {
            using (var image = new Bitmap(10, 10))
            using (Graphics graphics = Graphics.FromImage(image))
            {
                var header = new ColumnHeader { TextAlign = textAlign };
                var e = new DrawListViewSubItemEventArgs(graphics, new Rectangle(1, 2, 3, 4), new ListViewItem(), new ListViewItem.ListViewSubItem(), itemIndex, 0, header, ListViewItemStates.Checked);
                e.DrawText();
            }
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(1)]
        public void DrawText_HasGraphicsWithFlags_Success(int itemIndex)
        {
            using (var image = new Bitmap(10, 10))
            using (Graphics graphics = Graphics.FromImage(image))
            {
                var e = new DrawListViewSubItemEventArgs(graphics, new Rectangle(1, 2, 3, 4), new ListViewItem(), new ListViewItem.ListViewSubItem(), itemIndex, 0, new ColumnHeader(), ListViewItemStates.Checked);
                e.DrawText(TextFormatFlags.Bottom);
            }
        }

        [Fact]
        public void DrawText_NullGraphics_ThrowsArgumentNullException()
        {
            var e = new DrawListViewSubItemEventArgs(null, new Rectangle(1, 2, 3, 4), new ListViewItem(), new ListViewItem.ListViewSubItem(), 0, 0, new ColumnHeader(), ListViewItemStates.Checked);
            Assert.Throws<ArgumentNullException>("dc", () => e.DrawText());
            Assert.Throws<ArgumentNullException>("dc", () => e.DrawText(TextFormatFlags.Left));
        }

        [Theory]
        [InlineData(-2)]
        [InlineData(0)]
        [InlineData(1)]
        public void DrawText_NullItemNonMinusOneItemIndex_Nop(int itemIndex)
        {
            using (var image = new Bitmap(10, 10))
            using (Graphics graphics = Graphics.FromImage(image))
            {
                var e = new DrawListViewSubItemEventArgs(graphics, new Rectangle(1, 2, 3, 4), null, new ListViewItem.ListViewSubItem(), itemIndex, 0, new ColumnHeader(), ListViewItemStates.Checked);
                e.DrawText();
                e.DrawText(TextFormatFlags.Left);
            }
        }

        [Fact]
        public void DrawText_NullItemMinusOneItemIndex_ThrowsNullReferenceException()
        {
            using (var image = new Bitmap(10, 10))
            using (Graphics graphics = Graphics.FromImage(image))
            {
                var e = new DrawListViewSubItemEventArgs(graphics, new Rectangle(1, 2, 3, 4), null, new ListViewItem.ListViewSubItem(), -1, 0, new ColumnHeader(), ListViewItemStates.Checked);
                Assert.Throws<NullReferenceException>(() => e.DrawText());
                Assert.Throws<NullReferenceException>(() => e.DrawText(TextFormatFlags.Left));
            }
        }

        [Theory]
        [InlineData(-2)]
        [InlineData(0)]
        [InlineData(1)]
        public void DrawText_NullSubItemNonMinusOneItemIndex_ThrowsNullReferenceException(int itemIndex)
        {
            using (var image = new Bitmap(10, 10))
            using (Graphics graphics = Graphics.FromImage(image))
            {
                var e = new DrawListViewSubItemEventArgs(graphics, new Rectangle(1, 2, 3, 4), new ListViewItem(), null, itemIndex, 0, new ColumnHeader(), ListViewItemStates.Checked);
                Assert.Throws<NullReferenceException>(() => e.DrawText());
                Assert.Throws<NullReferenceException>(() => e.DrawText(TextFormatFlags.Left));
            }
        }

        [Fact]
        public void DrawText_NullHeading_ThrowsNullReferenceException()
        {
            using (var image = new Bitmap(10, 10))
            using (Graphics graphics = Graphics.FromImage(image))
            {
                var e = new DrawListViewSubItemEventArgs(graphics, new Rectangle(1, 2, 3, 4), new ListViewItem(), new ListViewItem.ListViewSubItem(), 0, 0, null, ListViewItemStates.Checked);
                Assert.Throws<NullReferenceException>(() => e.DrawText());
                e.DrawText(TextFormatFlags.Left);
            }
        }
    }
}
