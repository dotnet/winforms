// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using Moq;
using WinForms.Common.Tests;
using Xunit;
using static Interop;

namespace System.Windows.Forms.Tests
{
    using Point = System.Drawing.Point;
    using Size = System.Drawing.Size;

    public class ListBoxTests
    {
        public ListBoxTests()
        {
            Application.ThreadException += (sender, e) => throw new Exception(e.Exception.StackTrace.ToString());
        }

        [WinFormsFact]
        public void ListBox_Ctor_Default()
        {
            using var control = new SubListBox();
            Assert.Null(control.AccessibleDefaultActionDescription);
            Assert.Null(control.AccessibleDescription);
            Assert.Null(control.AccessibleName);
            Assert.Equal(AccessibleRole.Default, control.AccessibleRole);
            Assert.False(control.AllowDrop);
            Assert.True(control.AllowSelection);
            Assert.Equal(AnchorStyles.Top | AnchorStyles.Left, control.Anchor);
            Assert.False(control.AutoSize);
            Assert.Equal(SystemColors.Window, control.BackColor);
            Assert.Null(control.BackgroundImage);
            Assert.Equal(ImageLayout.Tile, control.BackgroundImageLayout);
            Assert.Null(control.BindingContext);
            Assert.Equal(BorderStyle.Fixed3D, control.BorderStyle);
            Assert.Equal(96, control.Bottom);
            Assert.Equal(new Rectangle(0, 0, 120, 96), control.Bounds);
            Assert.True(control.CanEnableIme);
            Assert.False(control.CanFocus);
            Assert.True(control.CanRaiseEvents);
            Assert.True(control.CanSelect);
            Assert.False(control.Capture);
            Assert.True(control.CausesValidation);
            Assert.Equal(new Size(116, 92), control.ClientSize);
            Assert.Equal(new Rectangle(0, 0, 116, 92), control.ClientRectangle);
            Assert.Equal(0, control.ColumnWidth); 
            Assert.Null(control.Container);
            Assert.False(control.ContainsFocus);
            Assert.Null(control.ContextMenuStrip);
            Assert.Empty(control.Controls);
            Assert.Same(control.Controls, control.Controls);
            Assert.False(control.Created);
            Assert.Same(Cursors.Default, control.Cursor);
            Assert.Empty(control.CustomTabOffsets);
            Assert.Same(control.CustomTabOffsets, control.CustomTabOffsets);
            Assert.Null(control.DataManager);
            Assert.Null(control.DataSource);
            Assert.Same(Cursors.Default, control.DefaultCursor);
            Assert.Equal(ImeMode.Inherit, control.DefaultImeMode);
            Assert.Equal(new Padding(3), control.DefaultMargin);
            Assert.Equal(Size.Empty, control.DefaultMaximumSize);
            Assert.Equal(Size.Empty, control.DefaultMinimumSize);
            Assert.Equal(Padding.Empty, control.DefaultPadding);
            Assert.Equal(new Size(120, 96), control.DefaultSize);
            Assert.False(control.DesignMode);
            Assert.Empty(control.DisplayMember);
            Assert.Equal(new Rectangle(0, 0, 116, 92), control.DisplayRectangle);
            Assert.Equal(DockStyle.None, control.Dock);
            Assert.False(control.DoubleBuffered);
            Assert.Equal(DrawMode.Normal, control.DrawMode);
            Assert.True(control.Enabled);
            Assert.NotNull(control.Events);
            Assert.Same(control.Events, control.Events);
            Assert.False(control.Focused);
            Assert.Equal(Control.DefaultFont, control.Font);
            Assert.Equal(control.Font.Height, control.FontHeight);
            Assert.Equal(SystemColors.WindowText, control.ForeColor);
            Assert.Null(control.FormatInfo);
            Assert.Empty(control.FormatString);
            Assert.False(control.FormattingEnabled);
            Assert.False(control.HasChildren);
            Assert.Equal(96, control.Height);
            Assert.Equal(0, control.HorizontalExtent);
            Assert.False(control.HorizontalScrollbar);
            Assert.Equal(ImeMode.NoControl, control.ImeMode);
            Assert.Equal(ImeMode.NoControl, control.ImeModeBase);
            Assert.True(control.IntegralHeight);
            Assert.False(control.IsAccessible);
            Assert.False(control.IsMirrored);
            Assert.Equal(13, control.ItemHeight);
            Assert.Empty(control.Items);
            Assert.Same(control.Items, control.Items);
            Assert.NotNull(control.LayoutEngine);
            Assert.Same(control.LayoutEngine, control.LayoutEngine);
            Assert.Equal(0, control.Left);
            Assert.Equal(Point.Empty, control.Location);
            Assert.Equal(new Padding(3), control.Margin);
            Assert.Equal(Size.Empty, control.MaximumSize);
            Assert.Equal(Size.Empty, control.MinimumSize);
            Assert.False(control.MultiColumn);
            Assert.Equal(Padding.Empty, control.Padding);
            Assert.Null(control.Parent);
            Assert.Equal("Microsoft\u00AE .NET", control.ProductName);
            Assert.Equal(new Size(120, 96), control.PreferredSize);
            Assert.False(control.RecreatingHandle);
            Assert.Null(control.Region);
            Assert.False(control.ResizeRedraw);
            Assert.Equal(120, control.Right);
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
            Assert.True(control.ShowFocusCues);
            Assert.True(control.ShowKeyboardCues);
            Assert.Null(control.Site);
            Assert.Equal(new Size(120, 96), control.Size);
            Assert.False(control.Sorted);
            Assert.Equal(0, control.TabIndex);
            Assert.True(control.TabStop);
            Assert.Empty(control.Text);
            Assert.Equal(0, control.Top);
            Assert.Equal(0, control.TopIndex);
            Assert.Null(control.TopLevelControl);
            Assert.False(control.UseCustomTabOffsets);
            Assert.True(control.UseTabStops);
            Assert.False(control.UseWaitCursor);
            Assert.Empty(control.ValueMember);
            Assert.True(control.Visible);
            Assert.Equal(120, control.Width);

            Assert.False(control.IsHandleCreated);
        }

        [WinFormsFact]
        public void ListBox_CreateParams_GetDefault_ReturnsExpected()
        {
            using var control = new SubListBox();
            CreateParams createParams = control.CreateParams;
            Assert.Null(createParams.Caption);
            Assert.Equal("ListBox", createParams.ClassName);
            Assert.Equal(0x8, createParams.ClassStyle);
            Assert.Equal(0x200, createParams.ExStyle);
            Assert.Equal(96, createParams.Height);
            Assert.Equal(IntPtr.Zero, createParams.Parent);
            Assert.Null(createParams.Param);
            Assert.Equal(0x562100C1, createParams.Style);
            Assert.Equal(120, createParams.Width);
            Assert.Equal(0, createParams.X);
            Assert.Equal(0, createParams.Y);
            Assert.Same(createParams, control.CreateParams);
            Assert.False(control.IsHandleCreated);
        }

        public static IEnumerable<object[]> BackColor_Set_TestData()
        {
            yield return new object[] { Color.Empty, SystemColors.Window };
            yield return new object[] { Color.Red, Color.Red };
        }

        [Theory]
        [MemberData(nameof(BackColor_Set_TestData))]
        public void ListBox_BackColor_Set_GetReturnsExpected(Color value, Color expected)
        {
            var control = new ListBox
            {
                BackColor = value
            };
            Assert.Equal(expected, control.BackColor);
            Assert.False(control.IsHandleCreated);

            // Set same.
            control.BackColor = value;
            Assert.Equal(expected, control.BackColor);
            Assert.False(control.IsHandleCreated);
        }

        public static IEnumerable<object[]> BackColor_SetWithHandle_TestData()
        {
            yield return new object[] { Color.Empty, SystemColors.Window, 0 };
            yield return new object[] { Color.Red, Color.Red, 1 };
        }

        [Theory]
        [MemberData(nameof(BackColor_SetWithHandle_TestData))]
        public void ListBox_BackColor_SetWithHandle_GetReturnsExpected(Color value, Color expected, int expectedInvalidatedCallCount)
        {
            var control = new ListBox();
            Assert.NotEqual(IntPtr.Zero, control.Handle);
            int invalidatedCallCount = 0;
            control.Invalidated += (sender, e) => invalidatedCallCount++;
            int styleChangedCallCount = 0;
            control.StyleChanged += (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            control.HandleCreated += (sender, e) => createdCallCount++;

            control.BackColor = value;
            Assert.Equal(expected, control.BackColor);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(expectedInvalidatedCallCount, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Set same.
            control.BackColor = value;
            Assert.Equal(expected, control.BackColor);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(expectedInvalidatedCallCount, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
        }

        [Fact]
        public void ListBox_BackColor_SetWithHandler_CallsBackColorChanged()
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
            var control = new SubListBox
            {
                Font = value
            };
            Assert.Equal(value ?? Control.DefaultFont, control.Font);
            Assert.Equal(control.Font.Height, control.FontHeight);

            // Set same.
            control.Font = value;
            Assert.Equal(value ?? Control.DefaultFont, control.Font);
            Assert.Equal(control.Font.Height, control.FontHeight);
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
            Assert.Equal(Control.DefaultFont, control.Font);
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
            yield return new object[] { Color.FromArgb(254, 1, 2, 3), Color.FromArgb(254, 1, 2, 3) };
            yield return new object[] { Color.White, Color.White };
            yield return new object[] { Color.Black, Color.Black };
            yield return new object[] { Color.Red, Color.Red };
        }

        [Theory]
        [MemberData(nameof(ForeColor_Set_TestData))]
        public void ListBox_ForeColor_Set_GetReturnsExpected(Color value, Color expected)
        {
            var control = new ListBox
            {
                ForeColor = value
            };
            Assert.Equal(expected, control.ForeColor);
            Assert.False(control.IsHandleCreated);

            // Set same.
            control.ForeColor = value;
            Assert.Equal(expected, control.ForeColor);
            Assert.False(control.IsHandleCreated);
        }

        public static IEnumerable<object[]> ForeColor_SetWithHandle_TestData()
        {
            yield return new object[] { Color.Empty, SystemColors.WindowText, 0 };
            yield return new object[] { Color.FromArgb(254, 1, 2, 3), Color.FromArgb(254, 1, 2, 3), 1 };
            yield return new object[] { Color.White, Color.White, 1 };
            yield return new object[] { Color.Black, Color.Black, 1 };
            yield return new object[] { Color.Red, Color.Red, 1 };
        }

        [Theory]
        [MemberData(nameof(ForeColor_SetWithHandle_TestData))]
        public void ListBox_ForeColor_SetWithHandle_GetReturnsExpected(Color value, Color expected, int expectedInvalidatedCallCount)
        {
            var control = new ListBox();
            Assert.NotEqual(IntPtr.Zero, control.Handle);
            int invalidatedCallCount = 0;
            control.Invalidated += (sender, e) => invalidatedCallCount++;
            int styleChangedCallCount = 0;
            control.StyleChanged += (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            control.HandleCreated += (sender, e) => createdCallCount++;

            control.ForeColor = value;
            Assert.Equal(expected, control.ForeColor);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(expectedInvalidatedCallCount, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Set same.
            control.ForeColor = value;
            Assert.Equal(expected, control.ForeColor);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(expectedInvalidatedCallCount, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
        }

        [Fact]
        public void ListBox_ForeColor_SetWithHandler_CallsForeColorChanged()
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
        public void ListBox_Padding_Set_GetReturnsExpected(Padding value, Padding expected)
        {
            using var control = new ListBox
            {
                Padding = value
            };
            Assert.Equal(expected, control.Padding);
            Assert.False(control.IsHandleCreated);

            // Set same.
            control.Padding = value;
            Assert.Equal(expected, control.Padding);
            Assert.False(control.IsHandleCreated);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetPaddingNormalizedTheoryData))]
        public void ListBox_Padding_SetWithHandle_GetReturnsExpected(Padding value, Padding expected)
        {
            using var control = new ListBox();
            Assert.NotEqual(IntPtr.Zero, control.Handle);
            int invalidatedCallCount = 0;
            control.Invalidated += (sender, e) => invalidatedCallCount++;
            int styleChangedCallCount = 0;
            control.StyleChanged += (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            control.HandleCreated += (sender, e) => createdCallCount++;

            control.Padding = value;
            Assert.Equal(expected, control.Padding);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Set same.
            control.Padding = value;
            Assert.Equal(expected, control.Padding);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
        }

        [Fact]
        public void ListBox_Padding_SetWithHandler_CallsPaddingChanged()
        {
            using var control = new ListBox();
            int callCount = 0;
            EventHandler handler = (sender, e) =>
            {
                Assert.Equal(control, sender);
                Assert.Equal(EventArgs.Empty, e);
                callCount++;
            };
            control.PaddingChanged += handler;

            // Set different.
            var padding1 = new Padding(1);
            control.Padding = padding1;
            Assert.Equal(padding1, control.Padding);
            Assert.Equal(1, callCount);

            // Set same.
            control.Padding = padding1;
            Assert.Equal(padding1, control.Padding);
            Assert.Equal(1, callCount);

            // Set different.
            var padding2 = new Padding(2);
            control.Padding = padding2;
            Assert.Equal(padding2, control.Padding);
            Assert.Equal(2, callCount);

            // Remove handler.
            control.PaddingChanged -= handler;
            control.Padding = padding1;
            Assert.Equal(padding1, control.Padding);
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
        
        [WinFormsFact]
        public void ListBox_GetAutoSizeMode_Invoke_ReturnsExpected()
        {
            using var control = new SubListBox();
            Assert.Equal(AutoSizeMode.GrowOnly, control.GetAutoSizeMode());
        }

        [WinFormsTheory]
        [InlineData(ControlStyles.ContainerControl, false)]
        [InlineData(ControlStyles.UserPaint, false)]
        [InlineData(ControlStyles.Opaque, false)]
        [InlineData(ControlStyles.ResizeRedraw, false)]
        [InlineData(ControlStyles.FixedWidth, false)]
        [InlineData(ControlStyles.FixedHeight, false)]
        [InlineData(ControlStyles.StandardClick, false)]
        [InlineData(ControlStyles.Selectable, true)]
        [InlineData(ControlStyles.UserMouse, false)]
        [InlineData(ControlStyles.SupportsTransparentBackColor, false)]
        [InlineData(ControlStyles.StandardDoubleClick, true)]
        [InlineData(ControlStyles.AllPaintingInWmPaint, true)]
        [InlineData(ControlStyles.CacheText, false)]
        [InlineData(ControlStyles.EnableNotifyMessage, false)]
        [InlineData(ControlStyles.DoubleBuffer, false)]
        [InlineData(ControlStyles.OptimizedDoubleBuffer, false)]
        [InlineData(ControlStyles.UseTextForAccessibility, false)]
        [InlineData((ControlStyles)0, true)]
        [InlineData((ControlStyles)int.MaxValue, false)]
        [InlineData((ControlStyles)(-1), false)]
        public void ListBox_GetStyle_Invoke_ReturnsExpected(ControlStyles flag, bool expected)
        {
            using var control = new SubListBox();
            Assert.Equal(expected, control.GetStyle(flag));

            // Call again to test caching.
            Assert.Equal(expected, control.GetStyle(flag));
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

        [WinFormsFact]
        public void ListBox_GetItemRectangle_InvokeWithoutHandle_ReturnsExpectedAndCreatedHandle()
        {
            using var control = new ListBox();
            control.Items.Add("Item1");
            control.Items.Add("Item1");

            Rectangle rect1 = control.GetItemRectangle(0);
            Assert.True(rect1.X >= 0);
            Assert.True(rect1.Y >= 0);
            Assert.True(rect1.Width > 0);
            Assert.True(rect1.Height > 0);
            Assert.Equal(rect1, control.GetItemRectangle(0));
            Assert.True(control.IsHandleCreated);

            Rectangle rect2 = control.GetItemRectangle(1);
            Assert.Equal(rect2.X, rect1.X);
            Assert.True(rect2.Y >= rect1.Y + rect1.Height);
            Assert.True(rect2.Width > 0);
            Assert.True(rect2.Height > 0);
            Assert.True(control.IsHandleCreated);
        }

        [WinFormsFact]
        public void ListBox_GetItemRectangle_InvokeWithHandle_ReturnsExpected()
        {
            using var control = new ListBox();
            control.Items.Add("Item1");
            control.Items.Add("Item1");
            Assert.NotEqual(IntPtr.Zero, control.Handle);
            int invalidatedCallCount = 0;
            control.Invalidated += (sender, e) => invalidatedCallCount++;
            int styleChangedCallCount = 0;
            control.StyleChanged += (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            control.HandleCreated += (sender, e) => createdCallCount++;

            Rectangle rect1 = control.GetItemRectangle(0);
            Assert.True(rect1.X >= 0);
            Assert.True(rect1.Y >= 0);
            Assert.True(rect1.Width > 0);
            Assert.True(rect1.Height > 0);
            Assert.Equal(rect1, control.GetItemRectangle(0));
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            Rectangle rect2 = control.GetItemRectangle(1);
            Assert.Equal(rect2.X, rect1.X);
            Assert.True(rect2.Y >= rect1.Y + rect1.Height);
            Assert.True(rect2.Width > 0);
            Assert.True(rect2.Height > 0);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
        }

        public static IEnumerable<object[]> GetItemRectangle_CustomGetItemRect_TestData()
        {
            yield return new object[] { new RECT(), Rectangle.Empty };
            yield return new object[] { new RECT(1, 2, 3, 4), new Rectangle(1, 2, 2, 2) };
        }

        [WinFormsTheory]
        [MemberData(nameof(GetItemRectangle_CustomGetItemRect_TestData))]
        public void ListBox_GetItemRectangle_InvokeCustomGetItemRect_ReturnsExpected(object getItemRectResult, Rectangle expected)
        {
            using var control = new CustomGetItemRectListBox
            {
                GetItemRectResult = (RECT)getItemRectResult
            };
            control.Items.Add("Item");
            Assert.NotEqual(IntPtr.Zero, control.Handle);

            Assert.Equal(expected, control.GetItemRectangle(0));
        }

        private class CustomGetItemRectListBox : ListBox
        {
            public RECT GetItemRectResult { get; set; }

            protected unsafe override void WndProc(ref Message m)
            {
                if (m.Msg == (int)User32.LB.GETITEMRECT)
                {
                    RECT* pRect = (RECT*)m.LParam;
                    *pRect = GetItemRectResult;
                    m.Result = (IntPtr)1;
                    return;
                }

                base.WndProc(ref m);
            }
        }

        [WinFormsFact]
        public void ListBox_GetItemRectangle_InvokeInvalidGetItemRect_ReturnsExpected()
        {
            using var control = new InvalidGetItemRectListBox();
            control.Items.Add("Item");
            Assert.NotEqual(IntPtr.Zero, control.Handle);

            control.MakeInvalid = true;
            Assert.Equal(Rectangle.Empty, control.GetItemRectangle(0));
        }

        private class InvalidGetItemRectListBox : ListBox
        {
            public bool MakeInvalid { get; set; }

            protected unsafe override void WndProc(ref Message m)
            {
                if (MakeInvalid && m.Msg == (int)User32.LB.GETITEMRECT)
                {
                    RECT* pRect = (RECT*)m.LParam;
                    *pRect = new RECT(1, 2, 3, 4);
                    m.Result = IntPtr.Zero;
                    return;
                }

                base.WndProc(ref m);
            }
        }

        [WinFormsFact]
        public void ListBox_GetItemRectangle_InvalidIndexEmpty_ThrowsArgumentOutOfRangeException()
        {
            using var control = new ListBox();
            Assert.Throws<ArgumentOutOfRangeException>("index", () => control.GetItemRectangle(-1));
            Assert.Throws<ArgumentOutOfRangeException>("index", () => control.GetItemRectangle(0));
            Assert.Throws<ArgumentOutOfRangeException>("index", () => control.GetItemRectangle(1));
        }

        [WinFormsFact]
        public void ListBox_GetItemRectangle_InvalidIndexNotEmpty_ThrowsArgumentOutOfRangeException()
        {
            using var control = new ListBox();
            control.Items.Add("Item");

            Assert.Throws<ArgumentOutOfRangeException>("index", () => control.GetItemRectangle(-1));
            Assert.Throws<ArgumentOutOfRangeException>("index", () => control.GetItemRectangle(1));
            Assert.Throws<ArgumentOutOfRangeException>("index", () => control.GetItemRectangle(2));
        }

        [WinFormsFact]
        public void ListBox_GetItemRectangle_InvalidIndexWithHandleEmpty_ThrowsArgumentOutOfRangeException()
        {
            using var control = new ListBox();
            Assert.Throws<ArgumentOutOfRangeException>("index", () => control.GetItemRectangle(-1));
            Assert.Throws<ArgumentOutOfRangeException>("index", () => control.GetItemRectangle(0));
            Assert.Throws<ArgumentOutOfRangeException>("index", () => control.GetItemRectangle(1));
        }

        [WinFormsFact]
        public void ListBox_GetItemRectangle_InvalidIndexWithHandleNotEmpty_ThrowsArgumentOutOfRangeException()
        {
            using var control = new ListBox();
            control.Items.Add("Item");
            Assert.NotEqual(IntPtr.Zero, control.Handle);

            Assert.Throws<ArgumentOutOfRangeException>("index", () => control.GetItemRectangle(-1));
            Assert.Throws<ArgumentOutOfRangeException>("index", () => control.GetItemRectangle(1));
            Assert.Throws<ArgumentOutOfRangeException>("index", () => control.GetItemRectangle(2));
        }

        private class SubListBox : ListBox
        {
            public new bool AllowSelection => base.AllowSelection;

            public new bool CanEnableIme => base.CanEnableIme;

            public new bool CanRaiseEvents => base.CanRaiseEvents;

            public new CreateParams CreateParams => base.CreateParams;

            public new CurrencyManager DataManager => base.DataManager;

            public new Cursor DefaultCursor => base.DefaultCursor;

            public new ImeMode DefaultImeMode => base.DefaultImeMode;

            public new Padding DefaultMargin => base.DefaultMargin;

            public new Size DefaultMaximumSize => base.DefaultMaximumSize;

            public new Size DefaultMinimumSize => base.DefaultMinimumSize;

            public new Padding DefaultPadding => base.DefaultPadding;

            public new Size DefaultSize => base.DefaultSize;

            public new bool DesignMode => base.DesignMode;

            public new bool DoubleBuffered
            {
                get => base.DoubleBuffered;
                set => base.DoubleBuffered = value;
            }

            public new EventHandlerList Events => base.Events;

            public new int FontHeight
            {
                get => base.FontHeight;
                set => base.FontHeight = value;
            }

            public new ImeMode ImeModeBase
            {
                get => base.ImeModeBase;
                set => base.ImeModeBase = value;
            }

            public new bool ResizeRedraw
            {
                get => base.ResizeRedraw;
                set => base.ResizeRedraw = value;
            }

            public new bool ShowFocusCues => base.ShowFocusCues;

            public new bool ShowKeyboardCues => base.ShowKeyboardCues;

            public new AutoSizeMode GetAutoSizeMode() => base.GetAutoSizeMode();

            public new bool GetStyle(ControlStyles flag) => base.GetStyle(flag);
        }

        private class DataClass
        {
            public string Value { get; set; }
        }
    }
}
