// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Drawing;
using WinForms.Common.Tests;
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
        [CommonMemberData(nameof(CommonTestHelper.GetBoolTheoryData))]
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

                // Set same.
                e.DrawDefault = value;
                Assert.Equal(value, e.DrawDefault);

                // Set different.
                e.DrawDefault = !value;
                Assert.Equal(!value, e.DrawDefault);
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

        public static IEnumerable<object[]> NullGraphics_TestData()
        {
            yield return new object[] { null, new Rectangle(-1, -2, -3, 4), -1, ListViewItemStates.Default };
            yield return new object[] { null, new Rectangle(-1, -2, -3, 4), -1, ListViewItemStates.Checked };
            yield return new object[] { null, new Rectangle(-1, -2, -3, 4), -1, ListViewItemStates.Focused };
            yield return new object[] { new ListViewItem(), new Rectangle(1, 2, 3, 4), 0, ListViewItemStates.Default };
            yield return new object[] { new ListViewItem(), new Rectangle(1, 2, 3, 4), 0, ListViewItemStates.Checked };
            yield return new object[] { new ListViewItem(), new Rectangle(1, 2, 3, 4), 0, ListViewItemStates.Focused };
        }

        [Theory]
        [MemberData(nameof(NullGraphics_TestData))]
        public void DrawBackground_NullGraphics_Nop(ListViewItem item, Rectangle bounds, int itemIndex, ListViewItemStates state)
        {
            var e = new DrawListViewItemEventArgs(null, item, bounds, itemIndex, state);
            e.DrawBackground();
        }

        public static IEnumerable<object[]> NullItem_TestData()
        {
            yield return new object[] { new Rectangle(-1, -2, -3, -4), -1, ListViewItemStates.Default };
            yield return new object[] { new Rectangle(-1, -2, -3, -4), -1, ListViewItemStates.Checked };
            yield return new object[] { new Rectangle(-1, -2, -3, -4), -1, ListViewItemStates.Focused };
            yield return new object[] { new Rectangle(1, 2, 3, 4), -1, ListViewItemStates.Default };
            yield return new object[] { new Rectangle(1, 2, 3, 4), -1, ListViewItemStates.Checked };
            yield return new object[] { new Rectangle(1, 2, 3, 4), -1, ListViewItemStates.Focused };
        }

        [Theory]
        [MemberData(nameof(NullItem_TestData))]
        public void DrawBackground_NullItem_Nop(Rectangle bounds, int itemIndex, ListViewItemStates state)
        {
            using (var image = new Bitmap(10, 10))
            using (Graphics graphics = Graphics.FromImage(image))
            {
                var e = new DrawListViewItemEventArgs(graphics, null, bounds, itemIndex, state);
                e.DrawBackground();
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

        [Theory]
        [MemberData(nameof(NullGraphics_TestData))]
        public void DrawFocusRectangle_NullGraphics_Nop(ListViewItem item, Rectangle bounds, int itemIndex, ListViewItemStates state)
        {
            var e = new DrawListViewItemEventArgs(null, item, bounds, itemIndex, state);
            e.DrawFocusRectangle();
        }

        [Theory]
        [MemberData(nameof(NullItem_TestData))]
        public void DrawFocusRectangle_NullItem_Nop(Rectangle bounds, int itemIndex, ListViewItemStates state)
        {
            using (var image = new Bitmap(10, 10))
            using (Graphics graphics = Graphics.FromImage(image))
            {
                var e = new DrawListViewItemEventArgs(graphics, null, bounds, itemIndex, state);
                e.DrawFocusRectangle();
            }
        }

        public static IEnumerable<object[]> ItemWithoutListView_TestData()
        {
            yield return new object[] { new Rectangle(-1, -2, -3, -4), -1, ListViewItemStates.Default };
            yield return new object[] { new Rectangle(-1, -2, -3, -4), -1, ListViewItemStates.Checked };
            yield return new object[] { new Rectangle(-1, -2, -3, -4), -1, ListViewItemStates.Focused };
            yield return new object[] { new Rectangle(1, 2, 3, 4), 0, ListViewItemStates.Default };
            yield return new object[] { new Rectangle(1, 2, 3, 4), 0, ListViewItemStates.Checked };
            yield return new object[] { new Rectangle(1, 2, 3, 4), 0, ListViewItemStates.Focused };
        }

        [Theory]
        [MemberData(nameof(ItemWithoutListView_TestData))]
        public void DrawFocusRectangle_ItemHasNoListViewNotFocused_Nop(Rectangle bounds, int itemIndex, ListViewItemStates state)
        {
            using (var image = new Bitmap(10, 10))
            using (Graphics graphics = Graphics.FromImage(image))
            {
                var e = new DrawListViewItemEventArgs(graphics, new ListViewItem(), bounds, itemIndex, state);
                e.DrawFocusRectangle();
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

        [Theory]
        [MemberData(nameof(NullGraphics_TestData))]
        public void DrawText_NullGraphics_Nop(ListViewItem item, Rectangle bounds, int itemIndex, ListViewItemStates state)
        {
            using (var image = new Bitmap(10, 10))
            using (Graphics graphics = Graphics.FromImage(image))
            {
                var e = new DrawListViewItemEventArgs(graphics, item, bounds, itemIndex, state);
                e.DrawText();
                e.DrawText(TextFormatFlags.Left);
            }
        }

        [Theory]
        [MemberData(nameof(NullItem_TestData))]
        public void DrawText_NullItem_Nop(Rectangle bounds, int itemIndex, ListViewItemStates state)
        {
            using (var image = new Bitmap(10, 10))
            using (Graphics graphics = Graphics.FromImage(image))
            {
                var e = new DrawListViewItemEventArgs(graphics, null, bounds, itemIndex, state);
                e.DrawText();
                e.DrawText(TextFormatFlags.Left);
            }
        }

        [Theory]
        [MemberData(nameof(ItemWithoutListView_TestData))]
        public void DrawText_ItemHasNoListView_Nop(Rectangle bounds, int itemIndex, ListViewItemStates state)
        {
            using (var image = new Bitmap(10, 10))
            using (Graphics graphics = Graphics.FromImage(image))
            {
                var e = new DrawListViewItemEventArgs(graphics, new ListViewItem(), bounds, itemIndex, state);
                e.DrawText();
                e.DrawText(TextFormatFlags.Left);
            }
        }
    }
}
