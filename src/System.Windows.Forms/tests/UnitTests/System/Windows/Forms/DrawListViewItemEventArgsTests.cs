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
            yield return new object[] { Rectangle.Empty, -2, (ListViewItemStates)(ListViewItemStates.Checked - 1) };
            yield return new object[] { new Rectangle(1, 2, 3, 4), -1, ListViewItemStates.Checked };
            yield return new object[] { new Rectangle(-1, 2, -3, -4), 0, ListViewItemStates.Focused };
            yield return new object[] { new Rectangle(1, 2, 3, 4), 1, ListViewItemStates.Checked };
        }

        [Theory]
        [MemberData(nameof(Ctor_Graphics_ListViewItem_Rectangle_Int_ListViewItemStates_TestData))]
        public void DrawListViewItemEventArgs_Ctor_Graphics_ListViewItem_Rectangle_Int_ListViewItemStates(Rectangle bounds, int itemIndex, ListViewItemStates state)
        {
            using (var image = new Bitmap(10, 10))
            using (Graphics graphics = Graphics.FromImage(image))
            {
                var item = new ListViewItem();
                var e = new DrawListViewItemEventArgs(graphics, item, bounds, itemIndex, state);
                Assert.Same(graphics, e.Graphics);
                Assert.Same(item, e.Item);
                Assert.Equal(bounds, e.Bounds);
                Assert.Equal(itemIndex, e.ItemIndex);
                Assert.Equal(state, e.State);
                Assert.False(e.DrawDefault);
            }
        }

        [Fact]
        public void DrawListViewItemEventArgs_Ctor_NullGraphics_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>("graphics", () => new DrawListViewItemEventArgs(null, new ListViewItem(), new Rectangle(1, 2, 3, 4), 0, ListViewItemStates.Default));
        }

        [Fact]
        public void DrawListViewItemEventArgs_Ctor_NullItem_ThrowsArgumentNullException()
        {
            using (var image = new Bitmap(10, 10))
            using (Graphics graphics = Graphics.FromImage(image))
            {
                Assert.Throws<ArgumentNullException>("item", () => new DrawListViewItemEventArgs(graphics, null, new Rectangle(1, 2, 3, 4), 0, ListViewItemStates.Default));
            }
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetBoolTheoryData))]
        public void DrawListViewItemEventArgs_DrawDefault_Set_GetReturnsExpected(bool value)
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
                var listView = new ListView { View = view };
                var listViewItem = new ListViewItem();
                listView.Items.Add(listViewItem);
                yield return new object[] { listViewItem, new Rectangle(1, 2, 3, 4), ListViewItemStates.Default };
                yield return new object[] { listViewItem, new Rectangle(1, 2, 3, 4), ListViewItemStates.Focused };
                yield return new object[] { listViewItem, new Rectangle(1, 2, 3, 4), ListViewItemStates.Checked };

                var subItemsItem = new ListViewItem();
                subItemsItem.SubItems.Add(new ListViewItem.ListViewSubItem());
                listView.Items.Add(subItemsItem);
                yield return new object[] { subItemsItem, new Rectangle(1, 2, 3, 4), ListViewItemStates.Default };
                yield return new object[] { subItemsItem, new Rectangle(1, 2, 3, 4), ListViewItemStates.Focused };
                yield return new object[] { subItemsItem, new Rectangle(1, 2, 3, 4), ListViewItemStates.Checked };

                var fullRowSelectListView = new ListView { View = view, FullRowSelect = true };
                var fullRowSelectListViewItem = new ListViewItem();
                fullRowSelectListViewItem.SubItems.Add(new ListViewItem.ListViewSubItem());
                fullRowSelectListView.Items.Add(fullRowSelectListViewItem);
                yield return new object[] { fullRowSelectListViewItem, new Rectangle(1, 2, 3, 4), ListViewItemStates.Default };
                yield return new object[] { fullRowSelectListViewItem, new Rectangle(1, 2, 3, 4), ListViewItemStates.Focused };
                yield return new object[] { fullRowSelectListViewItem, new Rectangle(1, 2, 3, 4), ListViewItemStates.Checked };
            }
        }

        [Theory]
        [MemberData(nameof(Draw_TestData))]
        public void DrawListViewItemEventArgs_DrawBackground_Invoke_Success(ListViewItem item, Rectangle bounds, ListViewItemStates state)
        {
            using (var image = new Bitmap(10, 10))
            using (Graphics graphics = Graphics.FromImage(image))
            {
                var e = new DrawListViewItemEventArgs(graphics, item, bounds, -1, state);
                e.DrawBackground();
            }
        }

        [Theory]
        [MemberData(nameof(Draw_TestData))]
        public void DrawListViewItemEventArgs_DrawFocusRectangle_HasGraphicsFocused_Success(ListViewItem item, Rectangle bounds, ListViewItemStates state)
        {
            using (var image = new Bitmap(10, 10))
            using (Graphics graphics = Graphics.FromImage(image))
            {
                var e = new DrawListViewItemEventArgs(graphics, item, bounds, -1, state);
                e.DrawFocusRectangle();
            }
        }

        [Theory]
        [MemberData(nameof(Draw_TestData))]
        public void DrawListViewItemEventArgs_DrawText_Invoke_Success(ListViewItem item, Rectangle bounds, ListViewItemStates state)
        {
            using (var image = new Bitmap(10, 10))
            using (Graphics graphics = Graphics.FromImage(image))
            {
                var e = new DrawListViewItemEventArgs(graphics, item, bounds, -1, state);
                e.DrawText();
            }
        }

        [Theory]
        [MemberData(nameof(Draw_TestData))]
        public void DrawListViewItemEventArgs_DrawText_InvokeTextFormatFlags(ListViewItem item, Rectangle bounds, ListViewItemStates state)
        {
            using (var image = new Bitmap(10, 10))
            using (Graphics graphics = Graphics.FromImage(image))
            {
                var e = new DrawListViewItemEventArgs(graphics, item, bounds, -1, state);
                e.DrawText(TextFormatFlags.Bottom);
            }
        }
    }
}
