// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Drawing;
using Xunit;

namespace System.Windows.Forms.Tests
{
    public class DrawListViewItemEventArgsTests
    {
        public static IEnumerable<object[]> Ctor_Graphics_ListViewItem_Rectangle_Int_ListViewItemStates_TestData()
        {
            var image = new Bitmap(10, 10);
            Graphics graphics = Graphics.FromImage(image);

            yield return new object[] { null, null, Rectangle.Empty, -2, (ListViewItemStates)(ListViewItemStates.Checked - 1) };
            yield return new object[] { graphics, new ListViewItem(), new Rectangle(1, 2, 3, 4), -1, ListViewItemStates.Checked };
            yield return new object[] { graphics, new ListViewItem(), new Rectangle(-1, 2, -3, -4), 0, ListViewItemStates.Focused };
            yield return new object[] { graphics, new ListViewItem(), new Rectangle(1, 2, 3, 4), 1, ListViewItemStates.Checked };
        }

        [Theory]
        [MemberData(nameof(Ctor_Graphics_ListViewItem_Rectangle_Int_ListViewItemStates_TestData))]
        public void Ctor_Graphics_ListViewItem_Rectangle_Int_ListViewItemStates(Graphics graphics, ListViewItem item, Rectangle bounds, int itemIndex, ListViewItemStates state)
        {
            var e = new DrawListViewItemEventArgs(graphics, item, bounds, itemIndex, state);
            Assert.Equal(graphics, e.Graphics);
            Assert.Equal(item, e.Item);
            Assert.Equal(bounds, e.Bounds);
            Assert.Equal(itemIndex, e.ItemIndex);
            Assert.Equal(state, e.State);
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
                var e = new DrawListViewItemEventArgs(graphics, new ListViewItem(), new Rectangle(1, 2, 3, 4), -1, ListViewItemStates.Checked)
                {
                    DrawDefault = value
                };
                Assert.Equal(value, e.DrawDefault);
            }
        }

        [Fact]
        public void DrawBackground_HasGraphics_Success()
        {
            using (var image = new Bitmap(10, 10))
            using (Graphics graphics = Graphics.FromImage(image))
            {
                var e = new DrawListViewItemEventArgs(graphics, new ListViewItem(), new Rectangle(1, 2, 3, 4), -1, ListViewItemStates.Checked);
                e.DrawBackground();
            }
        }

        [Fact]
        public void DrawBackground_NullGraphics_ThrowsNullReferenceException()
        {
            var e = new DrawListViewItemEventArgs(null, new ListViewItem(), new Rectangle(1, 2, 3, 4), -1, ListViewItemStates.Checked);
            Assert.Throws<NullReferenceException>(() => e.DrawBackground());
        }

        [Fact]
        public void DrawBackground_NullItem_ThrowsNullReferenceException()
        {
            using (var image = new Bitmap(10, 10))
            using (Graphics graphics = Graphics.FromImage(image))
            {
                var e = new DrawListViewItemEventArgs(graphics, null, new Rectangle(1, 2, 3, 4), -1, ListViewItemStates.Focused);
                Assert.Throws<NullReferenceException>(() => e.DrawBackground());
            }
        }

        public static IEnumerable<object[]> ListViewItem_TestData()
        {
            foreach (View view in new View[] { View.Details, View.List })
            {
                var listView = new ListView { View = view };
                var listViewItem = new ListViewItem();
                listView.Items.Add(listViewItem);
                yield return new object[] { listViewItem };

                var subItemsItem = new ListViewItem();
                subItemsItem.SubItems.Add(new ListViewItem.ListViewSubItem());
                listView.Items.Add(subItemsItem);
                yield return new object[] { subItemsItem };

                var fullRowSelectListView = new ListView { View = view, FullRowSelect = true };
                var fullRowSelectListViewItem = new ListViewItem();
                fullRowSelectListViewItem.SubItems.Add(new ListViewItem.ListViewSubItem());
                fullRowSelectListView.Items.Add(fullRowSelectListViewItem);
                yield return new object[] { fullRowSelectListViewItem };
            }
        }

        [Theory]
        [MemberData(nameof(ListViewItem_TestData))]
        public void DrawFocusRectangle_HasGraphicsFocused_Success(ListViewItem listViewItem)
        {
            using (var image = new Bitmap(10, 10))
            using (Graphics graphics = Graphics.FromImage(image))
            {
                var e = new DrawListViewItemEventArgs(graphics, listViewItem, new Rectangle(1, 2, 3, 4), -1, ListViewItemStates.Focused);
                e.DrawFocusRectangle();
            }
        }

        [Fact]
        public void DrawFocusRectangle_NullGraphicsNotFocused_Nop()
        {
            var e = new DrawListViewItemEventArgs(null, new ListViewItem(), new Rectangle(1, 2, 3, 4), -1, ListViewItemStates.Checked);
            e.DrawFocusRectangle();
        }

        [Fact]
        public void DrawFocusRectangle_NullGraphicsFocused_ThrowsArgumentNullException()
        {
            var listView = new ListView();
            var listViewItem = new ListViewItem();
            listView.Items.Add(listViewItem);

            var e = new DrawListViewItemEventArgs(null, listViewItem, new Rectangle(1, 2, 3, 4), -1, ListViewItemStates.Focused);
            Assert.Throws<ArgumentNullException>("graphics", () => e.DrawFocusRectangle());
        }

        [Fact]
        public void DrawFocusRectangle_NullItemNotFocused_Nop()
        {
            using (var image = new Bitmap(10, 10))
            using (Graphics graphics = Graphics.FromImage(image))
            {
                var e = new DrawListViewItemEventArgs(graphics, null, new Rectangle(1, 2, 3, 4), -1, ListViewItemStates.Checked);
                e.DrawFocusRectangle();
            }
        }

        [Fact]
        public void DrawFocusRectangle_NullItemFocused_ThrowsNullReferenceException()
        {
            using (var image = new Bitmap(10, 10))
            using (Graphics graphics = Graphics.FromImage(image))
            {
                var e = new DrawListViewItemEventArgs(graphics, null, new Rectangle(1, 2, 3, 4), -1, ListViewItemStates.Focused);
                Assert.Throws<NullReferenceException>(() => e.DrawFocusRectangle());
            }
        }

        [Fact]
        public void DrawFocusRectangle_ItemHasNoListViewNotFocused_Nop()
        {
            using (var image = new Bitmap(10, 10))
            using (Graphics graphics = Graphics.FromImage(image))
            {
                var e = new DrawListViewItemEventArgs(graphics, new ListViewItem(), new Rectangle(1, 2, 3, 4), -1, ListViewItemStates.Checked);
                e.DrawFocusRectangle();
            }
        }

        [Fact]
        public void DrawFocusRectangle_ItemHasNoListViewFocused_ThrowsNullReferenceException()
        {
            using (var image = new Bitmap(10, 10))
            using (Graphics graphics = Graphics.FromImage(image))
            {
                var e = new DrawListViewItemEventArgs(graphics, new ListViewItem(), new Rectangle(1, 2, 3, 4), -1, ListViewItemStates.Focused);
                Assert.Throws<NullReferenceException>(() => e.DrawFocusRectangle());
            }
        }

        [Theory]
        [MemberData(nameof(ListViewItem_TestData))]
        public void DrawText_HasGraphicsWithoutFlags_Success(ListViewItem listViewItem)
        {
            using (var image = new Bitmap(10, 10))
            using (Graphics graphics = Graphics.FromImage(image))
            {
                var e = new DrawListViewItemEventArgs(graphics, listViewItem, new Rectangle(1, 2, 3, 4), -1, ListViewItemStates.Checked);
                e.DrawText();
            }
        }

        [Theory]
        [MemberData(nameof(ListViewItem_TestData))]
        public void DrawText_HasGraphicsWithFlags_Success(ListViewItem listViewItem)
        {
            using (var image = new Bitmap(10, 10))
            using (Graphics graphics = Graphics.FromImage(image))
            {
                var e = new DrawListViewItemEventArgs(graphics, listViewItem, new Rectangle(1, 2, 3, 4), -1, ListViewItemStates.Checked);
                e.DrawText(TextFormatFlags.Bottom);
            }
        }

        [Fact]
        public void DrawText_NullGraphics_ThrowsNullReferenceException()
        {
            var e = new DrawListViewItemEventArgs(null, new ListViewItem(), new Rectangle(1, 2, 3, 4), -1, ListViewItemStates.Checked);
            Assert.Throws<NullReferenceException>(() => e.DrawText());
            Assert.Throws<NullReferenceException>(() => e.DrawText(TextFormatFlags.Left));
        }

        [Fact]
        public void DrawText_NullItem_ThrowsNullReferenceException()
        {
            using (var image = new Bitmap(10, 10))
            using (Graphics graphics = Graphics.FromImage(image))
            {
                var e = new DrawListViewItemEventArgs(graphics, null, new Rectangle(1, 2, 3, 4), -1, ListViewItemStates.Checked);
                Assert.Throws<NullReferenceException>(() => e.DrawText());
                Assert.Throws<NullReferenceException>(() => e.DrawText(TextFormatFlags.Left));
            }
        }

        [Fact]
        public void DrawText_ItemHasNoListView_ThrowsNullReferenceException()
        {
            using (var image = new Bitmap(10, 10))
            using (Graphics graphics = Graphics.FromImage(image))
            {
                var e = new DrawListViewItemEventArgs(graphics, new ListViewItem(), new Rectangle(1, 2, 3, 4), -1, ListViewItemStates.Checked);
                Assert.Throws<NullReferenceException>(() => e.DrawText());
                Assert.Throws<NullReferenceException>(() => e.DrawText(TextFormatFlags.Left));
            }
        }
    }
}
