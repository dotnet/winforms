// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using Moq;
using WinForms.Common.Tests;
using Xunit;

namespace System.Windows.Forms.Tests
{
    public class ListBoxTests
    {
        [Fact]
        public void Ctor_Default()
        {
            var control = new SubListBox();
            Assert.True(control.AllowSelection);
            Assert.Equal(SystemColors.Window, control.BackColor);
            Assert.Null(control.BackgroundImage);
            Assert.Equal(ImageLayout.Tile, control.BackgroundImageLayout);
            Assert.Equal(BorderStyle.Fixed3D, control.BorderStyle);
            Assert.Equal(0, control.Bounds.X);
            Assert.Equal(0, control.Bounds.Y);
            Assert.Equal(120, control.Bounds.Width);
            Assert.True(control.Bounds.Height > 0);
            Assert.True(control.ClientSize.Width > 0);
            Assert.True(control.ClientSize.Height > 0);
            Assert.Equal(0, control.ClientRectangle.X);
            Assert.Equal(0, control.ClientRectangle.Y);
            Assert.True(control.ClientRectangle.Width > 0);
            Assert.True(control.ClientRectangle.Height > 0);
            Assert.Equal(0, control.ColumnWidth);
            Assert.Empty(control.CustomTabOffsets);
            Assert.Same(control.CustomTabOffsets, control.CustomTabOffsets);
            Assert.Equal(0, control.DisplayRectangle.X);
            Assert.Equal(0, control.DisplayRectangle.Y);
            Assert.True(control.DisplayRectangle.Width > 0);
            Assert.True(control.DisplayRectangle.Height > 0);
            Assert.Null(control.DataManager);
            Assert.Null(control.DataSource);
            Assert.Equal(Size.Empty, control.DefaultMaximumSize);
            Assert.Equal(Size.Empty, control.DefaultMinimumSize);
            Assert.Equal(Padding.Empty, control.DefaultPadding);
            Assert.Equal(120, control.DefaultSize.Width);
            Assert.True(control.DefaultSize.Height > 0);
            Assert.Empty(control.DisplayMember);
            Assert.Equal(DrawMode.Normal, control.DrawMode);
            Assert.Null(control.FormatInfo);
            Assert.Empty(control.FormatString);
            Assert.False(control.FormattingEnabled);
            Assert.Same(Control.DefaultFont, control.Font);
            Assert.Equal(SystemColors.WindowText, control.ForeColor);
            Assert.True(control.Height > 0);
            Assert.Equal(0, control.HorizontalExtent);
            Assert.False(control.HorizontalScrollbar);
            Assert.True(control.IntegralHeight);
            Assert.Equal(13, control.ItemHeight);
            Assert.Empty(control.Items);
            Assert.Same(control.Items, control.Items);
            Assert.Equal(Point.Empty, control.Location);
            Assert.Equal(Size.Empty, control.MaximumSize);
            Assert.Equal(Size.Empty, control.MinimumSize);
            Assert.False(control.MultiColumn);
            Assert.Equal(Padding.Empty, control.Padding);
            Assert.Equal(120, control.PreferredSize.Width);
            Assert.True(control.PreferredSize.Height > 0);
            Assert.Equal(RightToLeft.No, control.RightToLeft);
            Assert.False(control.ScrollAlwaysVisible);
            Assert.Null(control.SelectedValue);
            Assert.Equal(-1, control.SelectedIndex);
            Assert.Empty(control.SelectedIndices);
            Assert.Same(control.SelectedIndices, control.SelectedIndices);
            Assert.Null(control.SelectedItem);
            Assert.Empty(control.SelectedItems);
            Assert.Same(control.SelectedItems, control.SelectedItems);
            Assert.Equal(SelectionMode.One, control.SelectionMode);
            Assert.Equal(120, control.Size.Width);
            Assert.True(control.Size.Height > 0);
            Assert.False(control.Sorted);
            Assert.Empty(control.Text);
            Assert.Equal(0, control.TopIndex);
            Assert.False(control.UseCustomTabOffsets);
            Assert.True(control.UseTabStops);
            Assert.Empty(control.ValueMember);
            Assert.Equal(120, control.Width);
        }

        public static IEnumerable<object[]> BackColor_Set_TestData()
        {
            yield return new object[] { Color.Empty, SystemColors.Window };
            yield return new object[] { Color.Red, Color.Red };
        }

        [Theory]
        [MemberData(nameof(BackColor_Set_TestData))]
        public void BackColor_Set_GetReturnsExpected(Color value, Color expected)
        {
            var control = new ListBox
            {
                BackColor = value
            };
            Assert.Equal(expected, control.BackColor);

            // Set same.
            control.BackColor = value;
            Assert.Equal(expected, control.BackColor);
        }

        [Fact]
        public void BackColor_SetWithHandler_CallsBackColorChanged()
        {
            var control = new ListBox();
            int callCount = 0;
            EventHandler handler = (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(EventArgs.Empty, e);
                callCount++;
            };
            control.BackColorChanged += handler;

            // Set different.
            control.BackColor = Color.Red;
            Assert.Equal(Color.Red, control.BackColor);
            Assert.Equal(1, callCount);

            // Set same.
            control.BackColor = Color.Red;
            Assert.Equal(Color.Red, control.BackColor);
            Assert.Equal(1, callCount);

            // Set different.
            control.BackColor = Color.Empty;
            Assert.Equal(SystemColors.Window, control.BackColor);
            Assert.Equal(2, callCount);

            // Remove handler.
            control.BackColorChanged -= handler;
            control.BackColor = Color.Red;
            Assert.Equal(Color.Red, control.BackColor);
            Assert.Equal(2, callCount);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetImageTheoryData))]
        public void BackgroundImage_Set_GetReturnsExpected(Image value)
        {
            var control = new ListBox
            {
                BackgroundImage = value
            };
            Assert.Equal(value, control.BackgroundImage);

            // Set same.
            control.BackgroundImage = value;
            Assert.Equal(value, control.BackgroundImage);
        }

        [Fact]
        public void BackgroundImage_SetWithHandler_CallsBackgroundImageChanged()
        {
            var control = new ListBox();
            int callCount = 0;
            EventHandler handler = (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(EventArgs.Empty, e);
                callCount++;
            };
            control.BackgroundImageChanged += handler;

            // Set different.
            var image1 = new Bitmap(10, 10);
            control.BackgroundImage = image1;
            Assert.Same(image1, control.BackgroundImage);
            Assert.Equal(1, callCount);

            // Set same.
            control.BackgroundImage = image1;
            Assert.Same(image1, control.BackgroundImage);
            Assert.Equal(1, callCount);

            // Set different.
            var image2 = new Bitmap(10, 10);
            control.BackgroundImage = image2;
            Assert.Same(image2, control.BackgroundImage);
            Assert.Equal(2, callCount);

            // Set null.
            control.BackgroundImage = null;
            Assert.Null(control.BackgroundImage);
            Assert.Equal(3, callCount);

            // Remove handler.
            control.BackgroundImageChanged -= handler;
            control.BackgroundImage = image1;
            Assert.Same(image1, control.BackgroundImage);
            Assert.Equal(3, callCount);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetEnumTypeTheoryData), typeof(ImageLayout))]
        public void BackgroundImageLayout_Set_GetReturnsExpected(ImageLayout value)
        {
            var control = new ListBox
            {
                BackgroundImageLayout = value
            };
            Assert.Equal(value, control.BackgroundImageLayout);

            // Set same.
            control.BackgroundImageLayout = value;
            Assert.Equal(value, control.BackgroundImageLayout);
        }

        [Fact]
        public void BackgroundImageLayout_SetWithHandler_CallsBackgroundImageLayoutChanged()
        {
            var control = new ListBox();
            int callCount = 0;
            EventHandler handler = (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(EventArgs.Empty, e);
                callCount++;
            };
            control.BackgroundImageLayoutChanged += handler;

            // Set different.
            control.BackgroundImageLayout = ImageLayout.Center;
            Assert.Equal(ImageLayout.Center, control.BackgroundImageLayout);
            Assert.Equal(1, callCount);

            // Set same.
            control.BackgroundImageLayout = ImageLayout.Center;
            Assert.Equal(ImageLayout.Center, control.BackgroundImageLayout);
            Assert.Equal(1, callCount);

            // Set different.
            control.BackgroundImageLayout = ImageLayout.Stretch;
            Assert.Equal(ImageLayout.Stretch, control.BackgroundImageLayout);
            Assert.Equal(2, callCount);

            // Remove handler.
            control.BackgroundImageLayoutChanged -= handler;
            control.BackgroundImageLayout = ImageLayout.Center;
            Assert.Equal(ImageLayout.Center, control.BackgroundImageLayout);
            Assert.Equal(2, callCount);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetEnumTypeTheoryDataInvalid), typeof(ImageLayout))]
        public void BackgroundImageLayout_SetInvalid_ThrowsInvalidEnumArgumentException(ImageLayout value)
        {
            var control = new ListBox();
            Assert.Throws<InvalidEnumArgumentException>("value", () => control.BackgroundImageLayout = value);
        }

        public static IEnumerable<object[]> DataSource_Set_TestData()
        {
            yield return new object[] { null };
            yield return new object[] { new List<int>() };
            yield return new object[] { Array.Empty<int>() };

            var mockSource = new Mock<IListSource>(MockBehavior.Strict);
            mockSource
                .Setup(s => s.GetList())
                .Returns(new int[] { 1 });
            yield return new object[] { mockSource.Object };
        }

        [Theory]
        [MemberData(nameof(DataSource_Set_TestData))]
        public void DataSource_Set_GetReturnsExpected(object value)
        {
            var control = new SubListBox
            {
                DataSource = value
            };
            Assert.Same(value, control.DataSource);
            Assert.Empty(control.DisplayMember);
            Assert.Null(control.DataManager);

            // Set same.
            control.DataSource = value;
            Assert.Same(value, control.DataSource);
            Assert.Empty(control.DisplayMember);
            Assert.Null(control.DataManager);
        }

        [Fact]
        public void DataSource_SetWithHandler_CallsDataSourceChanged()
        {
            var control = new ListBox();
            int dataSourceCallCount = 0;
            int displayMemberCallCount = 0;
            EventHandler dataSourceHandler = (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(EventArgs.Empty, e);
                dataSourceCallCount++;
            };
            EventHandler displayMemberHandler = (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(EventArgs.Empty, e);
                displayMemberCallCount++;
            };
            control.DataSourceChanged += dataSourceHandler;
            control.DisplayMemberChanged += displayMemberHandler;

            // Set different.
            var dataSource1 = new List<int>();
            control.DataSource = dataSource1;
            Assert.Same(dataSource1, control.DataSource);
            Assert.Equal(1, dataSourceCallCount);
            Assert.Equal(0, displayMemberCallCount);

            // Set same.
            control.DataSource = dataSource1;
            Assert.Same(dataSource1, control.DataSource);
            Assert.Equal(1, dataSourceCallCount);
            Assert.Equal(0, displayMemberCallCount);

            // Set different.
            var dataSource2 = new List<int>();
            control.DataSource = dataSource2;
            Assert.Same(dataSource2, control.DataSource);
            Assert.Equal(2, dataSourceCallCount);
            Assert.Equal(0, displayMemberCallCount);

            // Set null.
            control.DataSource = null;
            Assert.Null(control.DataSource);
            Assert.Equal(3, dataSourceCallCount);
            Assert.Equal(0, displayMemberCallCount);

            // Remove handler.
            control.DataSourceChanged -= dataSourceHandler;
            control.DisplayMemberChanged -= displayMemberHandler;
            control.DataSource = dataSource1;
            Assert.Same(dataSource1, control.DataSource);
            Assert.Equal(3, dataSourceCallCount);
            Assert.Equal(0, displayMemberCallCount);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetFontTheoryData))]
        public void Font_Set_GetReturnsExpected(Font value)
        {
            var control = new ListBox
            {
                Font = value
            };
            Assert.Equal(value ?? Control.DefaultFont, control.Font);

            // Set same.
            control.Font = value;
            Assert.Equal(value ?? Control.DefaultFont, control.Font);
        }

        [Fact]
        public void Font_SetWithHandler_CallsFontChanged()
        {
            var control = new ListBox();
            int callCount = 0;
            EventHandler handler = (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(EventArgs.Empty, e);
                callCount++;
            };
            control.FontChanged += handler;

            // Set different.
            Font font1 = new Font("Arial", 8.25f);
            control.Font = font1;
            Assert.Same(font1, control.Font);
            Assert.Equal(1, callCount);

            // Set same.
            control.Font = font1;
            Assert.Same(font1, control.Font);
            Assert.Equal(1, callCount);

            // Set different.
            Font font2 = SystemFonts.DialogFont;
            control.Font = font2;
            Assert.Same(font2, control.Font);
            Assert.Equal(2, callCount);

            // Set null.
            control.Font = null;
            Assert.Same(Control.DefaultFont, control.Font);
            Assert.Equal(3, callCount);

            // Remove handler.
            control.FontChanged -= handler;
            control.Font = font1;
            Assert.Same(font1, control.Font);
            Assert.Equal(3, callCount);
        }

        public static IEnumerable<object[]> ForeColor_Set_TestData()
        {
            yield return new object[] { Color.Empty, SystemColors.WindowText };
            yield return new object[] { Color.Red, Color.Red };
        }

        [Theory]
        [MemberData(nameof(ForeColor_Set_TestData))]
        public void ForeColor_Set_GetReturnsExpected(Color value, Color expected)
        {
            var control = new ListBox
            {
                ForeColor = value
            };
            Assert.Equal(expected, control.ForeColor);

            // Set same.
            control.ForeColor = value;
            Assert.Equal(expected, control.ForeColor);
        }

        [Fact]
        public void ForeColor_SetWithHandler_CallsForeColorChanged()
        {
            var control = new ListBox();
            int callCount = 0;
            EventHandler handler = (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(EventArgs.Empty, e);
                callCount++;
            };
            control.ForeColorChanged += handler;

            // Set different.
            control.ForeColor = Color.Red;
            Assert.Equal(Color.Red, control.ForeColor);
            Assert.Equal(1, callCount);

            // Set same.
            control.ForeColor = Color.Red;
            Assert.Equal(Color.Red, control.ForeColor);
            Assert.Equal(1, callCount);

            // Set different.
            control.ForeColor = Color.Empty;
            Assert.Equal(SystemColors.WindowText, control.ForeColor);
            Assert.Equal(2, callCount);

            // Remove handler.
            control.ForeColorChanged -= handler;
            control.ForeColor = Color.Red;
            Assert.Equal(Color.Red, control.ForeColor);
            Assert.Equal(2, callCount);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetPaddingNormalizedTheoryData))]
        public void Padding_Set_GetReturnsExpected(Padding value, Padding expected)
        {
            var control = new ListBox
            {
                Padding = value
            };
            Assert.Equal(expected, control.Padding);

            // Set same.
            control.Padding = value;
            Assert.Equal(expected, control.Padding);
        }

        [Fact]
        public void Padding_SetWithHandler_CallsPaddingChanged()
        {
            var control = new ListBox();
            int callCount = 0;
            EventHandler handler = (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(EventArgs.Empty, e);
                callCount++;
            };
            control.PaddingChanged += handler;

            // Set different.
            control.Padding = new Padding(1);
            Assert.Equal(new Padding(1), control.Padding);
            Assert.Equal(1, callCount);

            // Set same.
            control.Padding = new Padding(1);
            Assert.Equal(new Padding(1), control.Padding);
            Assert.Equal(1, callCount);

            // Set different.
            control.Padding = new Padding(2);
            Assert.Equal(new Padding(2), control.Padding);
            Assert.Equal(2, callCount);

            // Remove handler.
            control.PaddingChanged -= handler;
            control.Padding = new Padding(1);
            Assert.Equal(new Padding(1), control.Padding);
            Assert.Equal(2, callCount);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetRightToLeftTheoryData))]
        public void RightToLeft_Set_GetReturnsExpected(RightToLeft value, RightToLeft expected)
        {
            var control = new ListBox
            {
                RightToLeft = value
            };
            Assert.Equal(expected, control.RightToLeft);

            // Set same.
            control.RightToLeft = value;
            Assert.Equal(expected, control.RightToLeft);
        }

        [Fact]
        public void RightToLeft_SetWithHandler_CallsRightToLeftChanged()
        {
            var control = new ListBox();
            int callCount = 0;
            EventHandler handler = (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(EventArgs.Empty, e);
                callCount++;
            };
            control.RightToLeftChanged += handler;

            // Set different.
            control.RightToLeft = RightToLeft.Yes;
            Assert.Equal(RightToLeft.Yes, control.RightToLeft);
            Assert.Equal(1, callCount);

            // Set same.
            control.RightToLeft = RightToLeft.Yes;
            Assert.Equal(RightToLeft.Yes, control.RightToLeft);
            Assert.Equal(1, callCount);

            // Set different.
            control.RightToLeft = RightToLeft.Inherit;
            Assert.Equal(RightToLeft.No, control.RightToLeft);
            Assert.Equal(2, callCount);

            // Remove handler.
            control.RightToLeftChanged -= handler;
            control.RightToLeft = RightToLeft.Yes;
            Assert.Equal(RightToLeft.Yes, control.RightToLeft);
            Assert.Equal(2, callCount);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetEnumTypeTheoryDataInvalid), typeof(RightToLeft))]
        public void RightToLeft_SetInvalid_ThrowsInvalidEnumArgumentException(RightToLeft value)
        {
            var control = new ListBox();
            Assert.Throws<InvalidEnumArgumentException>("value", () => control.RightToLeft = value);
        }

        public static IEnumerable<object[]> FindString_TestData()
        {
            foreach (int startIndex in new int[] { -2, -1, 0, 1 })
            {
                yield return new object[] { new ListBox(), null, startIndex, -1 };
                yield return new object[] { new ListBox(), string.Empty, startIndex, -1 };
                yield return new object[] { new ListBox(), "s", startIndex, -1 };

                var controlWithNoItems = new ListBox();
                Assert.Empty(controlWithNoItems.Items);
                yield return new object[] { new ListBox(), null, startIndex, -1 };
                yield return new object[] { new ListBox(), string.Empty, startIndex, -1 };
                yield return new object[] { new ListBox(), "s", startIndex, -1 };
            }

            var controlWithItems = new ListBox
            {
                DisplayMember = "Value"
            };
            controlWithItems.Items.Add(new DataClass { Value = "abc" });
            controlWithItems.Items.Add(new DataClass { Value = "abc" });
            controlWithItems.Items.Add(new DataClass { Value = "ABC" });
            controlWithItems.Items.Add(new DataClass { Value = "def" });
            controlWithItems.Items.Add(new DataClass { Value = "" });
            controlWithItems.Items.Add(new DataClass { Value = null });

            yield return new object[] { controlWithItems, "abc", -1, 0 };
            yield return new object[] { controlWithItems, "abc", 0, 1 };
            yield return new object[] { controlWithItems, "abc", 1, 2 };
            yield return new object[] { controlWithItems, "abc", 2, 0 };
            yield return new object[] { controlWithItems, "abc", 5, 0 };

            yield return new object[] { controlWithItems, "ABC", -1, 0 };
            yield return new object[] { controlWithItems, "ABC", 0, 1 };
            yield return new object[] { controlWithItems, "ABC", 1, 2 };
            yield return new object[] { controlWithItems, "ABC", 2, 0 };
            yield return new object[] { controlWithItems, "ABC", 5, 0 };

            yield return new object[] { controlWithItems, "a", -1, 0 };
            yield return new object[] { controlWithItems, "a", 0, 1 };
            yield return new object[] { controlWithItems, "a", 1, 2 };
            yield return new object[] { controlWithItems, "a", 2, 0 };
            yield return new object[] { controlWithItems, "a", 5, 0 };

            yield return new object[] { controlWithItems, "A", -1, 0 };
            yield return new object[] { controlWithItems, "A", 0, 1 };
            yield return new object[] { controlWithItems, "A", 1, 2 };
            yield return new object[] { controlWithItems, "A", 2, 0 };
            yield return new object[] { controlWithItems, "A", 5, 0 };

            yield return new object[] { controlWithItems, "abcd", -1, -1 };
            yield return new object[] { controlWithItems, "abcd", 0, -1 };
            yield return new object[] { controlWithItems, "abcd", 1, -1 };
            yield return new object[] { controlWithItems, "abcd", 2, -1 };
            yield return new object[] { controlWithItems, "abcd", 5, -1 };

            yield return new object[] { controlWithItems, "def", -1, 3 };
            yield return new object[] { controlWithItems, "def", 0, 3 };
            yield return new object[] { controlWithItems, "def", 1, 3 };
            yield return new object[] { controlWithItems, "def", 2, 3 };
            yield return new object[] { controlWithItems, "def", 5, 3 };

            yield return new object[] { controlWithItems, null, -1, -1 };
            yield return new object[] { controlWithItems, null, 0, -1 };
            yield return new object[] { controlWithItems, null, 1, -1 };
            yield return new object[] { controlWithItems, null, 2, -1 };
            yield return new object[] { controlWithItems, null, 5, -1 };

            yield return new object[] { controlWithItems, string.Empty, -1, 0 };
            yield return new object[] { controlWithItems, string.Empty, 0, 1 };
            yield return new object[] { controlWithItems, string.Empty, 1, 2 };
            yield return new object[] { controlWithItems, string.Empty, 2, 3 };
            yield return new object[] { controlWithItems, string.Empty, 5, 0 };

            yield return new object[] { controlWithItems, "NoSuchItem", -1, -1 };
            yield return new object[] { controlWithItems, "NoSuchItem", 0, -1 };
            yield return new object[] { controlWithItems, "NoSuchItem", 1, -1 };
            yield return new object[] { controlWithItems, "NoSuchItem", 2, -1 };
            yield return new object[] { controlWithItems, "NoSuchItem", 5, -1 };
        }

        [Theory]
        [MemberData(nameof(FindString_TestData))]
        public void FindString_Invoke_ReturnsExpected(ListBox control, string s, int startIndex, int expected)
        {
            if (startIndex == -1)
            {
                Assert.Equal(expected, control.FindString(s));
            }

            Assert.Equal(expected, control.FindString(s, startIndex));
        }

        [Theory]
        [InlineData(-2)]
        [InlineData(1)]
        [InlineData(2)]
        public void FindString_InvalidStartIndex_ThrowsArgumentOutOfRangeException(int startIndex)
        {
            var control = new ListBox();
            control.Items.Add("item");
            Assert.Throws<ArgumentOutOfRangeException>("startIndex", () => control.FindString("s", startIndex));
        }

        public static IEnumerable<object[]> FindStringExact_TestData()
        {
            foreach (int startIndex in new int[] { -2, -1, 0, 1 })
            {
                yield return new object[] { new ListBox(), null, startIndex, -1 };
                yield return new object[] { new ListBox(), string.Empty, startIndex, -1 };
                yield return new object[] { new ListBox(), "s", startIndex, -1 };

                var controlWithNoItems = new ListBox();
                Assert.Empty(controlWithNoItems.Items);
                yield return new object[] { new ListBox(), null, startIndex, -1 };
                yield return new object[] { new ListBox(), string.Empty, startIndex, -1 };
                yield return new object[] { new ListBox(), "s", startIndex, -1 };
            }

            var controlWithItems = new ListBox
            {
                DisplayMember = "Value"
            };
            controlWithItems.Items.Add(new DataClass { Value = "abc" });
            controlWithItems.Items.Add(new DataClass { Value = "abc" });
            controlWithItems.Items.Add(new DataClass { Value = "ABC" });
            controlWithItems.Items.Add(new DataClass { Value = "def" });
            controlWithItems.Items.Add(new DataClass { Value = "" });
            controlWithItems.Items.Add(new DataClass { Value = null });

            yield return new object[] { controlWithItems, "abc", -1, 0 };
            yield return new object[] { controlWithItems, "abc", 0, 1 };
            yield return new object[] { controlWithItems, "abc", 1, 2 };
            yield return new object[] { controlWithItems, "abc", 2, 0 };
            yield return new object[] { controlWithItems, "abc", 5, 0 };

            yield return new object[] { controlWithItems, "ABC", -1, 0 };
            yield return new object[] { controlWithItems, "ABC", 0, 1 };
            yield return new object[] { controlWithItems, "ABC", 1, 2 };
            yield return new object[] { controlWithItems, "ABC", 2, 0 };
            yield return new object[] { controlWithItems, "ABC", 5, 0 };

            yield return new object[] { controlWithItems, "a", -1, -1 };
            yield return new object[] { controlWithItems, "a", 0, -1 };
            yield return new object[] { controlWithItems, "a", 1, -1 };
            yield return new object[] { controlWithItems, "a", 2, -1 };
            yield return new object[] { controlWithItems, "a", 5, -1 };

            yield return new object[] { controlWithItems, "A", -1, -1 };
            yield return new object[] { controlWithItems, "A", 0, -1 };
            yield return new object[] { controlWithItems, "A", 1, -1 };
            yield return new object[] { controlWithItems, "A", 2, -1 };
            yield return new object[] { controlWithItems, "A", 5, -1 };

            yield return new object[] { controlWithItems, "abcd", -1, -1 };
            yield return new object[] { controlWithItems, "abcd", 0, -1 };
            yield return new object[] { controlWithItems, "abcd", 1, -1 };
            yield return new object[] { controlWithItems, "abcd", 2, -1 };
            yield return new object[] { controlWithItems, "abcd", 5, -1 };

            yield return new object[] { controlWithItems, "def", -1, 3 };
            yield return new object[] { controlWithItems, "def", 0, 3 };
            yield return new object[] { controlWithItems, "def", 1, 3 };
            yield return new object[] { controlWithItems, "def", 2, 3 };
            yield return new object[] { controlWithItems, "def", 5, 3 };

            yield return new object[] { controlWithItems, null, -1, -1 };
            yield return new object[] { controlWithItems, null, 0, -1 };
            yield return new object[] { controlWithItems, null, 1, -1 };
            yield return new object[] { controlWithItems, null, 2, -1 };
            yield return new object[] { controlWithItems, null, 5, -1 };

            yield return new object[] { controlWithItems, string.Empty, -1, 4 };
            yield return new object[] { controlWithItems, string.Empty, 0, 4 };
            yield return new object[] { controlWithItems, string.Empty, 1, 4 };
            yield return new object[] { controlWithItems, string.Empty, 2, 4 };
            yield return new object[] { controlWithItems, string.Empty, 5, 4 };

            yield return new object[] { controlWithItems, "NoSuchItem", -1, -1 };
            yield return new object[] { controlWithItems, "NoSuchItem", 0, -1 };
            yield return new object[] { controlWithItems, "NoSuchItem", 1, -1 };
            yield return new object[] { controlWithItems, "NoSuchItem", 2, -1 };
            yield return new object[] { controlWithItems, "NoSuchItem", 5, -1 };
        }

        [Theory]
        [MemberData(nameof(FindStringExact_TestData))]
        public void FindStringExact_Invoke_ReturnsExpected(ListBox control, string s, int startIndex, int expected)
        {
            if (startIndex == -1)
            {
                Assert.Equal(expected, control.FindStringExact(s));
            }

            Assert.Equal(expected, control.FindStringExact(s, startIndex));
        }

        [Theory]
        [InlineData(-2)]
        [InlineData(1)]
        [InlineData(2)]
        public void FindStringExact_InvalidStartIndex_ThrowsArgumentOutOfRangeException(int startIndex)
        {
            var control = new ListBox();
            control.Items.Add("item");
            Assert.Throws<ArgumentOutOfRangeException>("startIndex", () => control.FindStringExact("s", startIndex));
        }

        private class SubListBox : ListBox
        {
            public new bool AllowSelection => base.AllowSelection;

            public new CurrencyManager DataManager => base.DataManager;

            public new Size DefaultMaximumSize => base.DefaultMaximumSize;

            public new Size DefaultMinimumSize => base.DefaultMinimumSize;

            public new Padding DefaultPadding => base.DefaultPadding;

            public new Size DefaultSize => base.DefaultSize;
        }

        private class DataClass
        {
            public string Value { get; set; }
        }
    }
}
