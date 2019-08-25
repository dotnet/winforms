// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Drawing;
using WinForms.Common.Tests;
using Xunit;

namespace System.Windows.Forms.Tests
{
    public class DrawListViewSubItemEventArgsTests
    {
        public static IEnumerable<object[]> Ctor_Graphics_ListViewItem_Rectangle_Int_ListViewItemStates_TestData()
        {
            yield return new object[] { Rectangle.Empty, null, new ListViewItem.ListViewSubItem(), -2, -2, null, (ListViewItemStates)(ListViewItemStates.Checked - 1) };
            yield return new object[] { Rectangle.Empty, new ListViewItem(), new ListViewItem.ListViewSubItem(), -2, -2, null, (ListViewItemStates)(ListViewItemStates.Checked - 1) };
            yield return new object[] { new Rectangle(1, 2, 3, 4), new ListViewItem(), null, -1, -1, new ColumnHeader(), ListViewItemStates.Checked };
            yield return new object[] { new Rectangle(1, 2, 3, 4), new ListViewItem(), new ListViewItem.ListViewSubItem(), -1, -1, new ColumnHeader(), ListViewItemStates.Checked };
            yield return new object[] { new Rectangle(-1, 2, -3, -4), new ListViewItem(), new ListViewItem.ListViewSubItem(), 0, 0, new ColumnHeader(), ListViewItemStates.Focused };
            yield return new object[] { new Rectangle(1, 2, 3, 4), new ListViewItem(), new ListViewItem.ListViewSubItem(), 1, 2, new ColumnHeader(), ListViewItemStates.Checked };
        }

        [Theory]
        [MemberData(nameof(Ctor_Graphics_ListViewItem_Rectangle_Int_ListViewItemStates_TestData))]
        public void DrawListViewSubItemEventArgs_Ctor_Graphics_ListViewItem_Rectangle_Int_ListViewItemStates(Rectangle bounds, ListViewItem item, ListViewItem.ListViewSubItem subItem, int itemIndex, int columnIndex, ColumnHeader header, ListViewItemStates itemState)
        {
            using (var image = new Bitmap(10, 10))
            using (Graphics graphics = Graphics.FromImage(image))
            {
                var e = new DrawListViewSubItemEventArgs(graphics, bounds, item, subItem, itemIndex, columnIndex, header, itemState);
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
        }

        [Fact]
        public void DrawListViewSubItemEventArgs_Ctor_NullGraphics_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>("graphics", () => new DrawListViewSubItemEventArgs(null, new Rectangle(1, 2, 3, 4), new ListViewItem(), new ListViewItem.ListViewSubItem(), -1, 0, new ColumnHeader(), ListViewItemStates.Default));
        }

        [Fact]
        public void DrawListViewSubItemEventArgs_Ctor_NullItemIndexNegativeOne_ThrowsArgumentNullException()
        {
            using (var image = new Bitmap(10, 10))
            using (Graphics graphics = Graphics.FromImage(image))
            {
                Assert.Throws<ArgumentNullException>("item", () => new DrawListViewSubItemEventArgs(graphics, new Rectangle(1, 2, 3, 4), null, new ListViewItem.ListViewSubItem(), -1, 0, new ColumnHeader(), ListViewItemStates.Default));
            }
        }

        [Theory]
        [InlineData(-2)]
        [InlineData(0)]
        public void DrawListViewSubItemEventArgs_Ctor_NullSubItemIndexNotNegativeOne_ThrowsArgumentNullException(int itemIndex)
        {
            using (var image = new Bitmap(10, 10))
            using (Graphics graphics = Graphics.FromImage(image))
            {
                Assert.Throws<ArgumentNullException>("subItem", () => new DrawListViewSubItemEventArgs(graphics, new Rectangle(1, 2, 3, 4), new ListViewItem(), null, itemIndex, 0, new ColumnHeader(), ListViewItemStates.Default));
            }
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetBoolTheoryData))]
        public void DrawListViewSubItemEventArgs_DrawDefault_Set_GetReturnsExpected(bool value)
        {
            using (var image = new Bitmap(10, 10))
            using (Graphics graphics = Graphics.FromImage(image))
            {
                var e = new DrawListViewSubItemEventArgs(graphics, new Rectangle(1, 2, 3, 4), new ListViewItem(), null, -1, -1, new ColumnHeader(), ListViewItemStates.Checked)
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

        [Theory]
        [MemberData(nameof(Draw_TestData))]
        public void DrawListViewSubItemEventArgs_DrawBackground_HasGraphics_Success(Rectangle bounds, ListViewItem item, ListViewItem.ListViewSubItem subItem, int itemIndex, ColumnHeader header, ListViewItemStates itemState)
        {
            using (var image = new Bitmap(10, 10))
            using (Graphics graphics = Graphics.FromImage(image))
            {
                var e = new DrawListViewSubItemEventArgs(graphics, bounds, item, subItem, itemIndex, 0, header, itemState);
                e.DrawBackground();
            }
        }

        [Theory]
        [MemberData(nameof(Draw_TestData))]
        public void DrawListViewSubItemEventArgs_DrawFocusRectangle_HasGraphics_Success(Rectangle bounds, ListViewItem item, ListViewItem.ListViewSubItem subItem, int itemIndex, ColumnHeader header, ListViewItemStates itemState)
        {
            using (var image = new Bitmap(10, 10))
            using (Graphics graphics = Graphics.FromImage(image))
            {
                var e = new DrawListViewSubItemEventArgs(graphics, bounds, item, subItem, itemIndex, 0, header, itemState);
                e.DrawFocusRectangle(new Rectangle(1, 2, 3, 4));
            }
        }

/*
        public static IEnumerable<object[]> _TestData()
        {
            
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
        public void DrawListViewSubItemEventArgs_DrawText_HasGraphicsWithoutFlags_Success(int itemIndex, HorizontalAlignment textAlign)
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
        public void DrawListViewSubItemEventArgs_DrawText_HasGraphicsWithFlags_Success(int itemIndex)
        {
            using (var image = new Bitmap(10, 10))
            using (Graphics graphics = Graphics.FromImage(image))
            {
                var e = new DrawListViewSubItemEventArgs(graphics, new Rectangle(1, 2, 3, 4), new ListViewItem(), new ListViewItem.ListViewSubItem(), itemIndex, 0, new ColumnHeader(), ListViewItemStates.Checked);
                e.DrawText(TextFormatFlags.Bottom);
            }
        }
 */
    }
}
