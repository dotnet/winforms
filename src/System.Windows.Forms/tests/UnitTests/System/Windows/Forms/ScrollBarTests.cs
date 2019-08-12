// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using WinForms.Common.Tests;
using Xunit;
using static Interop;

namespace System.Windows.Forms.Tests
{
    public class ScrollBarTests
    {
        [Fact]
        public void ScrollBar_Ctor_Default()
        {
            var scrollBar = new SubScrollBar();
            Assert.False(scrollBar.AllowDrop);
            Assert.Equal(AnchorStyles.Top | AnchorStyles.Left, scrollBar.Anchor);
            Assert.False(scrollBar.AutoSize);
            Assert.Equal(Control.DefaultBackColor, scrollBar.BackColor);
            Assert.Null(scrollBar.BackgroundImage);
            Assert.Equal(ImageLayout.Tile, scrollBar.BackgroundImageLayout);
            Assert.Null(scrollBar.BindingContext);
            Assert.Equal(0, scrollBar.Bottom);
            Assert.Equal(Rectangle.Empty, scrollBar.Bounds);
            Assert.False(scrollBar.CanEnableIme);
            Assert.True(scrollBar.CanRaiseEvents);
            Assert.True(scrollBar.CausesValidation);
            Assert.Equal(Rectangle.Empty, scrollBar.ClientRectangle);
            Assert.Equal(Size.Empty, scrollBar.ClientSize);
            Assert.Null(scrollBar.Container);
            Assert.Empty(scrollBar.Controls);
            Assert.Same(scrollBar.Controls, scrollBar.Controls);
            Assert.False(scrollBar.Created);
            Assert.Same(Cursors.Default, scrollBar.Cursor);
            Assert.Same(Cursors.Default, scrollBar.DefaultCursor);
            Assert.Equal(ImeMode.Disable, scrollBar.DefaultImeMode);
            Assert.Equal(Padding.Empty, scrollBar.DefaultMargin);
            Assert.Equal(Size.Empty, scrollBar.DefaultMaximumSize);
            Assert.Equal(Size.Empty, scrollBar.DefaultMinimumSize);
            Assert.Equal(Padding.Empty, scrollBar.DefaultPadding);
            Assert.Equal(Size.Empty, scrollBar.DefaultSize);
            Assert.False(scrollBar.DesignMode);
            Assert.Equal(Rectangle.Empty, scrollBar.DisplayRectangle);
            Assert.Equal(DockStyle.None, scrollBar.Dock);
            Assert.True(scrollBar.Enabled);
            Assert.NotNull(scrollBar.Events);
            Assert.Same(scrollBar.Events, scrollBar.Events);
            Assert.Equal(Control.DefaultFont, scrollBar.Font);
            Assert.Equal(Control.DefaultForeColor, scrollBar.ForeColor);
            Assert.False(scrollBar.HasChildren);
            Assert.Equal(0, scrollBar.Height);
            Assert.Equal(ImeMode.Disable, scrollBar.ImeMode);
            Assert.Equal(ImeMode.Disable, scrollBar.ImeModeBase);
            Assert.Equal(10, scrollBar.LargeChange);
            Assert.Equal(0, scrollBar.Left);
            Assert.Equal(Point.Empty, scrollBar.Location);
            Assert.Equal(100, scrollBar.Maximum);
            Assert.Equal(Padding.Empty, scrollBar.Margin);
            Assert.Equal(0, scrollBar.Minimum);
            Assert.Equal(Padding.Empty, scrollBar.Padding);
            Assert.Equal(0, scrollBar.Right);
            Assert.Equal(RightToLeft.No, scrollBar.RightToLeft);
            Assert.True(scrollBar.ScaleScrollBarForDpiChange);
            Assert.Null(scrollBar.Site);
            Assert.Equal(Size.Empty, scrollBar.Size);
            Assert.Equal(1, scrollBar.SmallChange);
            Assert.Equal(0, scrollBar.TabIndex);
            Assert.False(scrollBar.TabStop);
            Assert.Empty(scrollBar.Text);
            Assert.Equal(0, scrollBar.Top);
            Assert.Equal(0, scrollBar.Value);
            Assert.True(scrollBar.Visible);
            Assert.Equal(0, scrollBar.Width);
        }

        [Fact]
        public void ScrollBar_CreateParams_GetDefault_ReturnsExpected()
        {
            var control = new SubScrollBar();
            CreateParams createParams = control.CreateParams;
            Assert.Null(createParams.Caption);
            Assert.Equal("SCROLLBAR", createParams.ClassName);
            Assert.Equal(0x8, createParams.ClassStyle);
            Assert.Equal(0, createParams.ExStyle);
            Assert.Equal(0, createParams.Height);
            Assert.Equal(IntPtr.Zero, createParams.Parent);
            Assert.Null(createParams.Param);
            Assert.Equal(0x56000000, createParams.Style);
            Assert.Equal(0, createParams.Width);
            Assert.Equal(0, createParams.X);
            Assert.Equal(0, createParams.Y);
            Assert.Same(createParams, control.CreateParams);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetBoolTheoryData))]
        public void ScrollBar_AutoSize_Set_GetReturnsExpected(bool value)
        {
            var control = new SubScrollBar
            {
                AutoSize = value
            };
            Assert.Equal(value, control.AutoSize);

            // Set same.
            control.AutoSize = value;
            Assert.Equal(value, control.AutoSize);

            // Set different.
            control.AutoSize = !value;
            Assert.Equal(!value, control.AutoSize);
        }

        [Fact]
        public void ScrollBar_AutoSize_SetWithHandler_CallsAutoSizeChanged()
        {
            var control = new SubScrollBar
            {
                AutoSize = true
            };
            int callCount = 0;
            EventHandler handler = (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(EventArgs.Empty, e);
                callCount++;
            };
            control.AutoSizeChanged += handler;

            // Set different.
            control.AutoSize = false;
            Assert.False(control.AutoSize);
            Assert.Equal(1, callCount);

            // Set same.
            control.AutoSize = false;
            Assert.False(control.AutoSize);
            Assert.Equal(1, callCount);

            // Set different.
            control.AutoSize = true;
            Assert.True(control.AutoSize);
            Assert.Equal(2, callCount);

            // Remove handler.
            control.AutoSizeChanged -= handler;
            control.AutoSize = false;
            Assert.False(control.AutoSize);
            Assert.Equal(2, callCount);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetBackColorTheoryData))]
        public void ScrollBar_BackColor_Set_GetReturnsExpected(Color value, Color expected)
        {
            var control = new SubScrollBar
            {
                BackColor = value
            };
            Assert.Equal(expected, control.BackColor);

            // Set same.
            control.BackColor = value;
            Assert.Equal(expected, control.BackColor);
        }

        [Fact]
        public void ScrollBar_BackColor_SetWithHandler_CallsBackColorChanged()
        {
            var control = new SubScrollBar();
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
            Assert.Equal(Control.DefaultBackColor, control.BackColor);
            Assert.Equal(2, callCount);

            // Remove handler.
            control.BackColorChanged -= handler;
            control.BackColor = Color.Red;
            Assert.Equal(Color.Red, control.BackColor);
            Assert.Equal(2, callCount);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetImageTheoryData))]
        public void ScrollBar_BackgroundImage_Set_GetReturnsExpected(Image value)
        {
            var control = new SubScrollBar
            {
                BackgroundImage = value
            };
            Assert.Equal(value, control.BackgroundImage);

            // Set same.
            control.BackgroundImage = value;
            Assert.Equal(value, control.BackgroundImage);
        }

        [Fact]
        public void ScrollBar_BackgroundImage_SetWithHandler_CallsBackgroundImageChanged()
        {
            var control = new SubScrollBar();
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
        public void ScrollBar_BackgroundImageLayout_Set_GetReturnsExpected(ImageLayout value)
        {
            var control = new SubScrollBar
            {
                BackgroundImageLayout = value
            };
            Assert.Equal(value, control.BackgroundImageLayout);

            // Set same.
            control.BackgroundImageLayout = value;
            Assert.Equal(value, control.BackgroundImageLayout);
        }

        [Fact]
        public void ScrollBar_BackgroundImageLayout_SetWithHandler_CallsBackgroundImageLayoutChanged()
        {
            var control = new SubScrollBar();
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
        public void ScrollBar_BackgroundImageLayout_SetInvalid_ThrowsInvalidEnumArgumentException(ImageLayout value)
        {
            var control = new SubScrollBar();
            Assert.Throws<InvalidEnumArgumentException>("value", () => control.BackgroundImageLayout = value);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetBoolTheoryData))]
        public void ScrollBar_Enabled_Set_GetReturnsExpected(bool value)
        {
            var control = new SubScrollBar
            {
                Enabled = value
            };
            Assert.Equal(value, control.Enabled);

            // Set same.
            control.Enabled = value;
            Assert.Equal(value, control.Enabled);

            // Set different.
            control.Enabled = value;
            Assert.Equal(value, control.Enabled);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetBoolTheoryData))]
        public void ScrollBar_Enabled_SetWithHandle_GetReturnsExpected(bool value)
        {
            var control = new SubScrollBar();
            Assert.NotEqual(IntPtr.Zero, control.Handle);

            control.Enabled = value;
            Assert.Equal(value, control.Enabled);

            // Set same.
            control.Enabled = value;
            Assert.Equal(value, control.Enabled);

            // Set different.
            control.Enabled = value;
            Assert.Equal(value, control.Enabled);
        }

        [Fact]
        public void ScrollBar_Enabled_SetWithHandler_CallsEnabledChanged()
        {
            var control = new SubScrollBar
            {
                Enabled = true
            };
            int callCount = 0;
            EventHandler handler = (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(EventArgs.Empty, e);
                callCount++;
            };
            control.EnabledChanged += handler;

            // Set different.
            control.Enabled = false;
            Assert.False(control.Enabled);
            Assert.Equal(1, callCount);

            // Set same.
            control.Enabled = false;
            Assert.False(control.Enabled);
            Assert.Equal(1, callCount);

            // Set different.
            control.Enabled = true;
            Assert.True(control.Enabled);
            Assert.Equal(2, callCount);

            // Remove handler.
            control.EnabledChanged -= handler;
            control.Enabled = false;
            Assert.False(control.Enabled);
            Assert.Equal(2, callCount);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetFontTheoryData))]
        public void ScrollBar_Font_Set_GetReturnsExpected(Font value)
        {
            var control = new SubScrollBar
            {
                Font = value
            };
            Assert.Equal(value ?? Control.DefaultFont, control.Font);

            // Set same.
            control.Font = value;
            Assert.Equal(value ?? Control.DefaultFont, control.Font);
        }

        [Fact]
        public void ScrollBar_Font_SetWithHandler_CallsFontChanged()
        {
            var control = new SubScrollBar();
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

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetForeColorTheoryData))]
        public void ScrollBar_ForeColor_Set_GetReturnsExpected(Color value, Color expected)
        {
            var control = new SubScrollBar
            {
                ForeColor = value
            };
            Assert.Equal(expected, control.ForeColor);

            // Set same.
            control.ForeColor = value;
            Assert.Equal(expected, control.ForeColor);
        }

        [Fact]
        public void ScrollBar_ForeColor_SetWithHandler_CallsForeColorChanged()
        {
            var control = new SubScrollBar();
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
            Assert.Equal(Control.DefaultForeColor, control.ForeColor);
            Assert.Equal(2, callCount);

            // Remove handler.
            control.ForeColorChanged -= handler;
            control.ForeColor = Color.Red;
            Assert.Equal(Color.Red, control.ForeColor);
            Assert.Equal(2, callCount);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetEnumTypeTheoryData), typeof(ImageLayout))]
        public void ScrollBar_ImeMode_Set_GetReturnsExpected(ImeMode value)
        {
            var control = new SubScrollBar
            {
                ImeMode = value
            };
            Assert.Equal(value, control.ImeMode);

            // Set same.
            control.ImeMode = value;
            Assert.Equal(value, control.ImeMode);
        }

        [Fact]
        public void ScrollBar_ImeMode_SetWithHandler_CallsImeModeChanged()
        {
            var control = new SubScrollBar();
            int callCount = 0;
            EventHandler handler = (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(EventArgs.Empty, e);
                callCount++;
            };
            control.ImeModeChanged += handler;

            // Set different.
            control.ImeMode = ImeMode.On;
            Assert.Equal(ImeMode.On, control.ImeMode);
            Assert.Equal(0, callCount);

            // Set same.
            control.ImeMode = ImeMode.On;
            Assert.Equal(ImeMode.On, control.ImeMode);
            Assert.Equal(0, callCount);

            // Set different.
            control.ImeMode = ImeMode.Off;
            Assert.Equal(ImeMode.Off, control.ImeMode);
            Assert.Equal(0, callCount);

            // Remove handler.
            control.ImeModeChanged -= handler;
            control.ImeMode = ImeMode.Off;
            Assert.Equal(ImeMode.Off, control.ImeMode);
            Assert.Equal(0, callCount);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetEnumTypeTheoryDataInvalid), typeof(ImeMode))]
        public void ScrollBar_ImeMode_SetInvalid_ThrowsInvalidEnumArgumentException(ImeMode value)
        {
            var control = new SubScrollBar();
            Assert.Throws<InvalidEnumArgumentException>("value", () => control.ImeMode = value);
        }

        public static IEnumerable<object[]> LargeChange_TestData()
        {
            yield return new object[] { 10 };
            yield return new object[] { 12 };
        }

        [Theory]
        [MemberData(nameof(LargeChange_TestData))]
        public void ScrollBar_LargeChange_Set_GetReturnsExpected(int value)
        {
            var scrollBar = new SubScrollBar
            {
                LargeChange = value
            };
            Assert.Equal(value, scrollBar.LargeChange);

            // Set same.
            scrollBar.LargeChange = value;
            Assert.Equal(value, scrollBar.LargeChange);
        }

        [Theory]
        [MemberData(nameof(LargeChange_TestData))]
        public void ScrollBar_LargeChange_SetWithHandle_GetReturnsExpected(int value)
        {
            var scrollBar = new SubScrollBar();
            Assert.NotEqual(IntPtr.Zero, scrollBar.Handle);

            scrollBar.LargeChange = value;
            Assert.Equal(value, scrollBar.LargeChange);

            // Set same.
            scrollBar.LargeChange = value;
            Assert.Equal(value, scrollBar.LargeChange);
        }

        [Theory]
        [MemberData(nameof(LargeChange_TestData))]
        public void ScrollBar_LargeChange_SetWithHandleDisabled_GetReturnsExpected(int value)
        {
            var scrollBar = new SubScrollBar
            {
                Enabled = false
            };
            Assert.NotEqual(IntPtr.Zero, scrollBar.Handle);

            scrollBar.LargeChange = value;
            Assert.Equal(value, scrollBar.LargeChange);

            // Set same.
            scrollBar.LargeChange = value;
            Assert.Equal(value, scrollBar.LargeChange);
        }

        [Fact]
        public void ScrollBar_LargeChange_SetNegative_ThrowsArgumentOutOfRangeException()
        {
            var scrollBar = new SubScrollBar();
            Assert.Throws<ArgumentOutOfRangeException>("value", () => scrollBar.LargeChange = -1);
            Assert.Equal(10, scrollBar.LargeChange);
        }

        public static IEnumerable<object[]> Maximum_Set_TestData()
        {
            yield return new object[] { 0, 1 };
            yield return new object[] { 8, 9 };
            yield return new object[] { 10, 10 };
            yield return new object[] { 50, 10 };
        }

        [Theory]
        [MemberData(nameof(Maximum_Set_TestData))]
        public void ScrollBar_Maximum_Set_GetReturnsExpected(int value, int expectedLargeChange)
        {
            var scrollBar = new SubScrollBar
            {
                Maximum = value
            };
            Assert.Equal(value, scrollBar.Maximum);
            Assert.Equal(0, scrollBar.Minimum);
            Assert.Equal(0, scrollBar.Value);
            Assert.Equal(expectedLargeChange, scrollBar.LargeChange);
            Assert.Equal(1, scrollBar.SmallChange);

            // Set same.
            scrollBar.Maximum = value;
            Assert.Equal(value, scrollBar.Maximum);
            Assert.Equal(0, scrollBar.Minimum);
            Assert.Equal(0, scrollBar.Value);
            Assert.Equal(expectedLargeChange, scrollBar.LargeChange);
            Assert.Equal(1, scrollBar.SmallChange);
        }

        [Theory]
        [MemberData(nameof(Maximum_Set_TestData))]
        public void ScrollBar_Maximum_SetWithHandle_GetReturnsExpected(int value, int expectedLargeChange)
        {
            var scrollBar = new SubScrollBar();
            Assert.NotEqual(IntPtr.Zero, scrollBar.Handle);

            scrollBar.Maximum = value;
            Assert.Equal(value, scrollBar.Maximum);
            Assert.Equal(0, scrollBar.Minimum);
            Assert.Equal(0, scrollBar.Value);
            Assert.Equal(expectedLargeChange, scrollBar.LargeChange);
            Assert.Equal(1, scrollBar.SmallChange);

            // Set same.
            scrollBar.Maximum = value;
            Assert.Equal(value, scrollBar.Maximum);
            Assert.Equal(0, scrollBar.Minimum);
            Assert.Equal(0, scrollBar.Value);
            Assert.Equal(expectedLargeChange, scrollBar.LargeChange);
            Assert.Equal(1, scrollBar.SmallChange);
        }

        [Theory]
        [MemberData(nameof(Maximum_Set_TestData))]
        public void ScrollBar_Maximum_SetWithHandleDisabled_GetReturnsExpected(int value, int expectedLargeChange)
        {
            var scrollBar = new SubScrollBar
            {
                Enabled = false
            };
            Assert.NotEqual(IntPtr.Zero, scrollBar.Handle);

            scrollBar.Maximum = value;
            Assert.Equal(value, scrollBar.Maximum);
            Assert.Equal(0, scrollBar.Minimum);
            Assert.Equal(0, scrollBar.Value);
            Assert.Equal(expectedLargeChange, scrollBar.LargeChange);
            Assert.Equal(1, scrollBar.SmallChange);

            // Set same.
            scrollBar.Maximum = value;
            Assert.Equal(value, scrollBar.Maximum);
            Assert.Equal(0, scrollBar.Minimum);
            Assert.Equal(0, scrollBar.Value);
            Assert.Equal(expectedLargeChange, scrollBar.LargeChange);
            Assert.Equal(1, scrollBar.SmallChange);
        }

        [Fact]
        public void ScrollBar_Maximum_SetLessThanValueAndMinimum_SetsValueAndMinimum()
        {
            var scrollBar = new SubScrollBar
            {
                Value = 10,
                Minimum = 8,
                Maximum = 5
            };
            Assert.Equal(5, scrollBar.Maximum);
            Assert.Equal(5, scrollBar.Minimum);
            Assert.Equal(5, scrollBar.Value);
            Assert.Equal(1, scrollBar.LargeChange);
            Assert.Equal(1, scrollBar.SmallChange);
        }

        [Fact]
        public void ScrollBar_Maximum_SetNegative_SetsValueAndMinimum()
        {
            var scrollBar = new SubScrollBar
            {
                Maximum = -1
            };
            Assert.Equal(-1, scrollBar.Maximum);
            Assert.Equal(-1, scrollBar.Minimum);
            Assert.Equal(-1, scrollBar.Value);
            Assert.Equal(1, scrollBar.LargeChange);
            Assert.Equal(1, scrollBar.SmallChange);
        }

        public static IEnumerable<object[]> Minimum_TestData()
        {
            yield return new object[] { -1 };
            yield return new object[] { 0 };
            yield return new object[] { 5 };
        }

        [Theory]
        [MemberData(nameof(Minimum_TestData))]
        public void ScrollBar_Minimum_Set_GetReturnsExpected(int value)
        {
            var scrollBar = new SubScrollBar
            {
                Value = 5,
                Minimum = value
            };
            Assert.Equal(100, scrollBar.Maximum);
            Assert.Equal(value, scrollBar.Minimum);
            Assert.Equal(5, scrollBar.Value);
            Assert.Equal(10, scrollBar.LargeChange);
            Assert.Equal(1, scrollBar.SmallChange);

            // Set same.
            scrollBar.Minimum = value;
            Assert.Equal(100, scrollBar.Maximum);
            Assert.Equal(value, scrollBar.Minimum);
            Assert.Equal(5, scrollBar.Value);
            Assert.Equal(10, scrollBar.LargeChange);
            Assert.Equal(1, scrollBar.SmallChange);
        }

        [Theory]
        [MemberData(nameof(Minimum_TestData))]
        public void ScrollBar_Minimum_SetWithHandle_GetReturnsExpected(int value)
        {
            var scrollBar = new SubScrollBar
            {
                Value = 5
            };
            Assert.NotEqual(IntPtr.Zero, scrollBar.Handle);

            scrollBar.Minimum = value;
            Assert.Equal(100, scrollBar.Maximum);
            Assert.Equal(value, scrollBar.Minimum);
            Assert.Equal(5, scrollBar.Value);
            Assert.Equal(10, scrollBar.LargeChange);
            Assert.Equal(1, scrollBar.SmallChange);

            // Set same.
            scrollBar.Minimum = value;
            Assert.Equal(100, scrollBar.Maximum);
            Assert.Equal(value, scrollBar.Minimum);
            Assert.Equal(5, scrollBar.Value);
            Assert.Equal(10, scrollBar.LargeChange);
            Assert.Equal(1, scrollBar.SmallChange);
        }

        [Theory]
        [MemberData(nameof(Minimum_TestData))]
        public void ScrollBar_Minimum_SetWithHandleDisabled_GetReturnsExpected(int value)
        {
            var scrollBar = new SubScrollBar
            {
                Value = 5,
                Enabled = false
            };
            Assert.NotEqual(IntPtr.Zero, scrollBar.Handle);

            scrollBar.Minimum = value;
            Assert.Equal(100, scrollBar.Maximum);
            Assert.Equal(value, scrollBar.Minimum);
            Assert.Equal(5, scrollBar.Value);
            Assert.Equal(10, scrollBar.LargeChange);
            Assert.Equal(1, scrollBar.SmallChange);

            // Set same.
            scrollBar.Minimum = value;
            Assert.Equal(100, scrollBar.Maximum);
            Assert.Equal(value, scrollBar.Minimum);
            Assert.Equal(5, scrollBar.Value);
            Assert.Equal(10, scrollBar.LargeChange);
            Assert.Equal(1, scrollBar.SmallChange);
        }

        [Fact]
        public void ScrollBar_Minimum_SetGreaterThanValueAndMaximum_SetsValueAndMinimum()
        {
            var scrollBar = new SubScrollBar
            {
                Value = 10,
                Maximum = 8,
                Minimum = 12
            };
            Assert.Equal(12, scrollBar.Maximum);
            Assert.Equal(12, scrollBar.Minimum);
            Assert.Equal(12, scrollBar.Value);
            Assert.Equal(1, scrollBar.LargeChange);
            Assert.Equal(1, scrollBar.SmallChange);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetRightToLeftTheoryData))]
        public void ScrollBar_RightToLeft_Set_GetReturnsExpected(RightToLeft value, RightToLeft expected)
        {
            var control = new SubScrollBar
            {
                RightToLeft = value
            };
            Assert.Equal(expected, control.RightToLeft);

            // Set same.
            control.RightToLeft = value;
            Assert.Equal(expected, control.RightToLeft);
        }

        [Fact]
        public void ScrollBar_RightToLeft_SetWithHandler_CallsRightToLeftChanged()
        {
            var control = new SubScrollBar();
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
        public void ScrollBar_RightToLeft_SetInvalid_ThrowsInvalidEnumArgumentException(RightToLeft value)
        {
            var control = new SubScrollBar();
            Assert.Throws<InvalidEnumArgumentException>("value", () => control.RightToLeft = value);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetBoolTheoryData))]
        public void ScrollBar_ScaleScrollBarForDpiChange_Set_GetReturnsExpected(bool value)
        {
            var control = new SubScrollBar
            {
                ScaleScrollBarForDpiChange = value
            };
            Assert.Equal(value, control.ScaleScrollBarForDpiChange);

            // Set same.
            control.ScaleScrollBarForDpiChange = value;
            Assert.Equal(value, control.ScaleScrollBarForDpiChange);

            // Set different.
            control.ScaleScrollBarForDpiChange = !value;
            Assert.Equal(!value, control.ScaleScrollBarForDpiChange);
        }

        public static IEnumerable<object[]> SmallChange_TestData()
        {
            yield return new object[] { 1, 1 };
            yield return new object[] { 8, 8 };
            yield return new object[] { 10, 10 };
            yield return new object[] { 12, 10 };
        }

        [Theory]
        [MemberData(nameof(SmallChange_TestData))]
        public void ScrollBar_SmallChange_Set_GetReturnsExpected(int value, int expectedValue)
        {
            var scrollBar = new SubScrollBar
            {
                SmallChange = value
            };
            Assert.Equal(expectedValue, scrollBar.SmallChange);

            // Set same.
            scrollBar.SmallChange = value;
            Assert.Equal(expectedValue, scrollBar.SmallChange);
        }

        [Theory]
        [MemberData(nameof(SmallChange_TestData))]
        public void ScrollBar_SmallChange_SetWithHandle_GetReturnsExpected(int value, int expectedValue)
        {
            var scrollBar = new SubScrollBar();
            Assert.NotEqual(IntPtr.Zero, scrollBar.Handle);

            scrollBar.SmallChange = value;
            Assert.Equal(expectedValue, scrollBar.SmallChange);

            // Set same.
            scrollBar.SmallChange = value;
            Assert.Equal(expectedValue, scrollBar.SmallChange);
        }

        [Theory]
        [MemberData(nameof(SmallChange_TestData))]
        public void ScrollBar_SmallChange_SetWithHandleDisabled_GetReturnsExpected(int value, int expectedValue)
        {
            var scrollBar = new SubScrollBar
            {
                Enabled = false
            };
            Assert.NotEqual(IntPtr.Zero, scrollBar.Handle);

            scrollBar.SmallChange = value;
            Assert.Equal(expectedValue, scrollBar.SmallChange);

            // Set same.
            scrollBar.SmallChange = value;
            Assert.Equal(expectedValue, scrollBar.SmallChange);
        }

        [Fact]
        public void ScrollBar_SmallChange_SetNegative_ThrowsArgumentOutOfRangeException()
        {
            var scrollBar = new SubScrollBar();
            Assert.Throws<ArgumentOutOfRangeException>("value", () => scrollBar.SmallChange = -1);
            Assert.Equal(1, scrollBar.SmallChange);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetBoolTheoryData))]
        public void ScrollBar_TabStop_Set_GetReturnsExpected(bool value)
        {
            var control = new SubScrollBar
            {
                TabStop = value
            };
            Assert.Equal(value, control.TabStop);

            // Set same.
            control.TabStop = value;
            Assert.Equal(value, control.TabStop);

            // Set different.
            control.TabStop = value;
            Assert.Equal(value, control.TabStop);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetBoolTheoryData))]
        public void ScrollBar_TabStop_SetWithHandle_GetReturnsExpected(bool value)
        {
            var control = new SubScrollBar();
            Assert.NotEqual(IntPtr.Zero, control.Handle);

            control.TabStop = value;
            Assert.Equal(value, control.TabStop);

            // Set same.
            control.TabStop = value;
            Assert.Equal(value, control.TabStop);

            // Set different.
            control.TabStop = value;
            Assert.Equal(value, control.TabStop);
        }

        [Fact]
        public void ScrollBar_TabStop_SetWithHandler_CallsTabStopChanged()
        {
            var control = new SubScrollBar
            {
                TabStop = true
            };
            int callCount = 0;
            EventHandler handler = (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(EventArgs.Empty, e);
                callCount++;
            };
            control.TabStopChanged += handler;

            // Set different.
            control.TabStop = false;
            Assert.False(control.TabStop);
            Assert.Equal(1, callCount);

            // Set same.
            control.TabStop = false;
            Assert.False(control.TabStop);
            Assert.Equal(1, callCount);

            // Set different.
            control.TabStop = true;
            Assert.True(control.TabStop);
            Assert.Equal(2, callCount);

            // Remove handler.
            control.TabStopChanged -= handler;
            control.TabStop = false;
            Assert.False(control.TabStop);
            Assert.Equal(2, callCount);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetStringNormalizedTheoryData))]
        public void ScrollBar_Text_Set_GetReturnsExpected(string value, string expected)
        {
            var control = new SubScrollBar
            {
                Text = value
            };
            Assert.Equal(expected, control.Text);

            // Set same.
            control.Text = value;
            Assert.Equal(expected, control.Text);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetStringNormalizedTheoryData))]
        public void ScrollBar_Text_SetWithHandle_GetReturnsExpected(string value, string expected)
        {
            var control = new SubScrollBar();
            Assert.NotEqual(IntPtr.Zero, control.Handle);

            control.Text = value;
            Assert.Equal(expected, control.Text);

            // Set same.
            control.Text = value;
            Assert.Equal(expected, control.Text);
        }

        [Fact]
        public void ScrollBar_Text_SetWithHandler_CallsTextChanged()
        {
            var control = new SubScrollBar();
            int callCount = 0;
            EventHandler handler = (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Equal(EventArgs.Empty, e);
                callCount++;
            };
            control.TextChanged += handler;

            // Set different.
            control.Text = "text";
            Assert.Same("text", control.Text);
            Assert.Equal(1, callCount);

            // Set same.
            control.Text = "text";
            Assert.Same("text", control.Text);
            Assert.Equal(1, callCount);

            // Set different.
            control.Text = null;
            Assert.Empty(control.Text);
            Assert.Equal(2, callCount);

            // Remove handler.
            control.TextChanged -= handler;
            control.Text = "text";
            Assert.Same("text", control.Text);
            Assert.Equal(2, callCount);
        }

        public static IEnumerable<object[]> Value_TestData()
        {
            yield return new object[] { 0 };
            yield return new object[] { 5 };
            yield return new object[] { 100 };
        }

        [Theory]
        [MemberData(nameof(Value_TestData))]
        public void ScrollBar_Value_Set_GetReturnsExpected(int value)
        {
            var scrollBar = new SubScrollBar
            {
                Value = value
            };
            Assert.Equal(100, scrollBar.Maximum);
            Assert.Equal(0, scrollBar.Minimum);
            Assert.Equal(value, scrollBar.Value);
            Assert.Equal(10, scrollBar.LargeChange);
            Assert.Equal(1, scrollBar.SmallChange);

            // Set same.
            scrollBar.Value = value;
            Assert.Equal(100, scrollBar.Maximum);
            Assert.Equal(0, scrollBar.Minimum);
            Assert.Equal(value, scrollBar.Value);
            Assert.Equal(10, scrollBar.LargeChange);
            Assert.Equal(1, scrollBar.SmallChange);
        }

        [Theory]
        [MemberData(nameof(Value_TestData))]
        public void ScrollBar_Value_SetWithHandle_GetReturnsExpected(int value)
        {
            var scrollBar = new SubScrollBar();
            Assert.NotEqual(IntPtr.Zero, scrollBar.Handle);

            scrollBar.Value = value;
            Assert.Equal(100, scrollBar.Maximum);
            Assert.Equal(0, scrollBar.Minimum);
            Assert.Equal(value, scrollBar.Value);
            Assert.Equal(10, scrollBar.LargeChange);
            Assert.Equal(1, scrollBar.SmallChange);

            // Set same.
            scrollBar.Value = value;
            Assert.Equal(100, scrollBar.Maximum);
            Assert.Equal(0, scrollBar.Minimum);
            Assert.Equal(value, scrollBar.Value);
            Assert.Equal(10, scrollBar.LargeChange);
            Assert.Equal(1, scrollBar.SmallChange);
        }

        [Theory]
        [MemberData(nameof(Value_TestData))]
        public void ScrollBar_Value_SetWithHandleDisabled_GetReturnsExpected(int value)
        {
            var scrollBar = new SubScrollBar
            {
                Enabled = false
            };
            Assert.NotEqual(IntPtr.Zero, scrollBar.Handle);

            scrollBar.Value = value;
            Assert.Equal(100, scrollBar.Maximum);
            Assert.Equal(0, scrollBar.Minimum);
            Assert.Equal(value, scrollBar.Value);
            Assert.Equal(10, scrollBar.LargeChange);
            Assert.Equal(1, scrollBar.SmallChange);

            // Set same.
            scrollBar.Value = value;
            Assert.Equal(100, scrollBar.Maximum);
            Assert.Equal(0, scrollBar.Minimum);
            Assert.Equal(value, scrollBar.Value);
            Assert.Equal(10, scrollBar.LargeChange);
            Assert.Equal(1, scrollBar.SmallChange);
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(101)]
        public void ScrollBar_Value_SetOutOfRange_ThrowsArgumentOutOfRangeException(int value)
        {
            var scrollBar = new SubScrollBar();
            Assert.Throws<ArgumentOutOfRangeException>("value", () => scrollBar.Value = value);
            Assert.Equal(0, scrollBar.Value);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetEventArgsTheoryData))]
        public void ScrollBar_OnClick_Invoke_CallsClick(EventArgs eventArgs)
        {
            var control = new SubScrollBar();
            int callCount = 0;
            EventHandler handler = (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(eventArgs, e);
                callCount++;
            };

            // Call with handler.
            control.Click += handler;
            control.OnClick(eventArgs);
            Assert.Equal(1, callCount);

            // Remove handler.
            control.Click -= handler;
            control.OnClick(eventArgs);
            Assert.Equal(1, callCount);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetEventArgsTheoryData))]
        public void ScrollBar_OnDoubleClick_Invoke_CallsDoubleClick(EventArgs eventArgs)
        {
            var control = new SubScrollBar();
            int callCount = 0;
            EventHandler handler = (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(eventArgs, e);
                callCount++;
            };

            // Call with handler.
            control.DoubleClick += handler;
            control.OnDoubleClick(eventArgs);
            Assert.Equal(1, callCount);

            // Remove handler.
            control.DoubleClick -= handler;
            control.OnDoubleClick(eventArgs);
            Assert.Equal(1, callCount);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetMouseEventArgsTheoryData))]
        public void ScrollBar_OnMouseClick_Invoke_CallsMouseClick(MouseEventArgs eventArgs)
        {
            var control = new SubScrollBar();
            int callCount = 0;
            MouseEventHandler handler = (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(eventArgs, e);
                callCount++;
            };

            // Call with handler.
            control.MouseClick += handler;
            control.OnMouseClick(eventArgs);
            Assert.Equal(1, callCount);

            // Remove handler.
            control.MouseClick -= handler;
            control.OnMouseClick(eventArgs);
            Assert.Equal(1, callCount);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetMouseEventArgsTheoryData))]
        public void ScrollBar_OnMouseDoubleClick_Invoke_CallsMouseDoubleClick(MouseEventArgs eventArgs)
        {
            var control = new SubScrollBar();
            int callCount = 0;
            MouseEventHandler handler = (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(eventArgs, e);
                callCount++;
            };

            // Call with handler.
            control.MouseDoubleClick += handler;
            control.OnMouseDoubleClick(eventArgs);
            Assert.Equal(1, callCount);

            // Remove handler.
            control.MouseDoubleClick -= handler;
            control.OnMouseDoubleClick(eventArgs);
            Assert.Equal(1, callCount);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetMouseEventArgsTheoryData))]
        public void ScrollBar_OnMouseDown_Invoke_CallsMouseDown(MouseEventArgs eventArgs)
        {
            var control = new SubScrollBar();
            int callCount = 0;
            MouseEventHandler handler = (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(eventArgs, e);
                callCount++;
            };

            // Call with handler.
            control.MouseDown += handler;
            control.OnMouseDown(eventArgs);
            Assert.Equal(1, callCount);

            // Remove handler.
            control.MouseDown -= handler;
            control.OnMouseDown(eventArgs);
            Assert.Equal(1, callCount);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetMouseEventArgsTheoryData))]
        public void ScrollBar_OnMouseMove_Invoke_CallsMouseMove(MouseEventArgs eventArgs)
        {
            var control = new SubScrollBar();
            int callCount = 0;
            MouseEventHandler handler = (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(eventArgs, e);
                callCount++;
            };

            // Call with handler.
            control.MouseMove += handler;
            control.OnMouseMove(eventArgs);
            Assert.Equal(1, callCount);

            // Remove handler.
            control.MouseMove -= handler;
            control.OnMouseMove(eventArgs);
            Assert.Equal(1, callCount);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetMouseEventArgsTheoryData))]
        public void ScrollBar_OnMouseUp_Invoke_CallsMouseUp(MouseEventArgs eventArgs)
        {
            var control = new SubScrollBar();
            int callCount = 0;
            MouseEventHandler handler = (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(eventArgs, e);
                callCount++;
            };

            // Call with handler.
            control.MouseUp += handler;
            control.OnMouseUp(eventArgs);
            Assert.Equal(1, callCount);

            // Remove handler.
            control.MouseUp -= handler;
            control.OnMouseUp(eventArgs);
            Assert.Equal(1, callCount);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetMouseEventArgsTheoryData))]
        public void ScrollBar_OnMouseWheel_Invoke_CallsMouseWheel(MouseEventArgs eventArgs)
        {
            var control = new SubScrollBar();
            int callCount = 0;
            MouseEventHandler handler = (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(eventArgs, e);
                callCount++;
            };

            // Call with handler.
            control.MouseWheel += handler;
            control.OnMouseWheel(eventArgs);
            Assert.Equal(1, callCount);

            // Remove handler.
            control.MouseWheel -= handler;
            control.OnMouseWheel(eventArgs);
            Assert.Equal(1, callCount);
        }

        [Fact]
        public void ScrollBar_OnMouseWheel_InvokeHandledMouseEventArgs_SetsHandled()
        {
            var control = new SubScrollBar();
            var eventArgs = new HandledMouseEventArgs(MouseButtons.Left, 1, 2, 3, 4);
            int callCount = 0;
            MouseEventHandler handler = (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(eventArgs, e);
                Assert.True(eventArgs.Handled);
                callCount++;
            };
            control.MouseWheel += handler;

            control.OnMouseWheel(eventArgs);
            Assert.Equal(1, callCount);
            Assert.True(eventArgs.Handled);
        }

        public static IEnumerable<object[]> OnMouseWheel_TestData()
        {
            yield return new object[] { RightToLeft.No, 10, -119, new List<ScrollEventArgs>(), 10 };
            yield return new object[] { RightToLeft.No, 10, 0, new List<ScrollEventArgs>(), 10 };
            yield return new object[] { RightToLeft.No, 10, 119, new List<ScrollEventArgs>(), 10 };
            yield return new object[] { RightToLeft.Yes, 10, -119, new List<ScrollEventArgs>(), 10 };
            yield return new object[] { RightToLeft.Yes, 10, 0, new List<ScrollEventArgs>(), 10 };
            yield return new object[] { RightToLeft.Yes, 10, 119, new List<ScrollEventArgs>(), 10 };

            // Decrement.
            yield return new object[]
            {
                RightToLeft.No, 10, 120,
                new List<ScrollEventArgs>
                {
                    new ScrollEventArgs(ScrollEventType.SmallDecrement, 10, 9),
                    new ScrollEventArgs(ScrollEventType.EndScroll, 9, 9),
                }, 9
            };
            yield return new object[]
            {
                RightToLeft.No, 10, 121,
                new List<ScrollEventArgs>
                {
                    new ScrollEventArgs(ScrollEventType.SmallDecrement, 10, 9),
                    new ScrollEventArgs(ScrollEventType.EndScroll, 9, 9),
                }, 9
            };
            yield return new object[]
            {
                RightToLeft.No, 10, 240,
                new List<ScrollEventArgs>
                {
                    new ScrollEventArgs(ScrollEventType.SmallDecrement, 10, 9),
                    new ScrollEventArgs(ScrollEventType.SmallDecrement, 9, 8),
                    new ScrollEventArgs(ScrollEventType.EndScroll, 8, 8),
                }, 8
            };
            yield return new object[]
            {
                RightToLeft.No, 1, 120,
                new List<ScrollEventArgs>
                {
                    new ScrollEventArgs(ScrollEventType.SmallDecrement, 1, 0),
                    new ScrollEventArgs(ScrollEventType.EndScroll, 0, 0),
                }, 0
            };
            yield return new object[]
            {
                RightToLeft.No, 1, 240,
                new List<ScrollEventArgs>
                {
                    new ScrollEventArgs(ScrollEventType.SmallDecrement, 1, 0),
                    new ScrollEventArgs(ScrollEventType.SmallDecrement, 0, 0),
                    new ScrollEventArgs(ScrollEventType.EndScroll, 0, 0),
                }, 0
            };
            yield return new object[]
            {
                RightToLeft.No, 100, 240,
                new List<ScrollEventArgs>
                {
                    new ScrollEventArgs(ScrollEventType.SmallDecrement, 100, 99),
                    new ScrollEventArgs(ScrollEventType.SmallDecrement, 99, 98),
                    new ScrollEventArgs(ScrollEventType.EndScroll, 98, 98),
                }, 98
            };

            yield return new object[]
            {
                RightToLeft.Yes, 10, -120,
                new List<ScrollEventArgs>
                {
                    new ScrollEventArgs(ScrollEventType.SmallDecrement, 10, 9),
                    new ScrollEventArgs(ScrollEventType.EndScroll, 9, 9),
                }, 9
            };
            yield return new object[]
            {
                RightToLeft.Yes, 10, -121,
                new List<ScrollEventArgs>
                {
                    new ScrollEventArgs(ScrollEventType.SmallDecrement, 10, 9),
                    new ScrollEventArgs(ScrollEventType.EndScroll, 9, 9),
                }, 9
            };
            yield return new object[]
            {
                RightToLeft.Yes, 10, -240,
                new List<ScrollEventArgs>
                {
                    new ScrollEventArgs(ScrollEventType.SmallDecrement, 10, 9),
                    new ScrollEventArgs(ScrollEventType.SmallDecrement, 9, 8),
                    new ScrollEventArgs(ScrollEventType.EndScroll, 8, 8),
                }, 8
            };
            yield return new object[]
            {
                RightToLeft.Yes, 1, -120,
                new List<ScrollEventArgs>
                {
                    new ScrollEventArgs(ScrollEventType.SmallDecrement, 1, 0),
                    new ScrollEventArgs(ScrollEventType.EndScroll, 0, 0),
                }, 0
            };
            yield return new object[]
            {
                RightToLeft.Yes, 1, -240,
                new List<ScrollEventArgs>
                {
                    new ScrollEventArgs(ScrollEventType.SmallDecrement, 1, 0),
                    new ScrollEventArgs(ScrollEventType.SmallDecrement, 0, 0),
                    new ScrollEventArgs(ScrollEventType.EndScroll, 0, 0),
                }, 0
            };
            yield return new object[]
            {
                RightToLeft.Yes, 100, -240,
                new List<ScrollEventArgs>
                {
                    new ScrollEventArgs(ScrollEventType.SmallDecrement, 100, 99),
                    new ScrollEventArgs(ScrollEventType.SmallDecrement, 99, 98),
                    new ScrollEventArgs(ScrollEventType.EndScroll, 98, 98),
                }, 98
            };

            // Increment.
            yield return new object[]
            {
                RightToLeft.No, 10, -120,
                new List<ScrollEventArgs>
                {
                    new ScrollEventArgs(ScrollEventType.SmallIncrement, 10, 11),
                    new ScrollEventArgs(ScrollEventType.EndScroll, 11, 11),
                }, 11
            };
            yield return new object[]
            {
                RightToLeft.No, 10, -121,
                new List<ScrollEventArgs>
                {
                    new ScrollEventArgs(ScrollEventType.SmallIncrement, 10, 11),
                    new ScrollEventArgs(ScrollEventType.EndScroll, 11, 11),
                }, 11
            };
            yield return new object[]
            {
                RightToLeft.No, 10, -240,
                new List<ScrollEventArgs>
                {
                    new ScrollEventArgs(ScrollEventType.SmallIncrement, 10, 11),
                    new ScrollEventArgs(ScrollEventType.SmallIncrement, 11, 12),
                    new ScrollEventArgs(ScrollEventType.EndScroll, 12, 12),
                }, 12
            };
            yield return new object[]
            {
                RightToLeft.No, 90, -120,
                new List<ScrollEventArgs>
                {
                    new ScrollEventArgs(ScrollEventType.SmallIncrement, 90, 91),
                    new ScrollEventArgs(ScrollEventType.EndScroll, 91, 91),
                }, 91
            };
            yield return new object[]
            {
                RightToLeft.No, 99, -120,
                new List<ScrollEventArgs>
                {
                    new ScrollEventArgs(ScrollEventType.SmallIncrement, 99, 91),
                    new ScrollEventArgs(ScrollEventType.EndScroll, 91, 91),
                }, 91
            };
            yield return new object[]
            {
                RightToLeft.No, 99, -240,
                new List<ScrollEventArgs>
                {
                    new ScrollEventArgs(ScrollEventType.SmallIncrement, 99, 91),
                    new ScrollEventArgs(ScrollEventType.SmallIncrement, 91, 91),
                    new ScrollEventArgs(ScrollEventType.EndScroll, 91, 91),
                }, 91
            };

            yield return new object[]
            {
                RightToLeft.Yes, 10, 120,
                new List<ScrollEventArgs>
                {
                    new ScrollEventArgs(ScrollEventType.SmallIncrement, 10, 11),
                    new ScrollEventArgs(ScrollEventType.EndScroll, 11, 11),
                }, 11
            };
            yield return new object[]
            {
                RightToLeft.Yes, 10, 121,
                new List<ScrollEventArgs>
                {
                    new ScrollEventArgs(ScrollEventType.SmallIncrement, 10, 11),
                    new ScrollEventArgs(ScrollEventType.EndScroll, 11, 11),
                }, 11
            };
            yield return new object[]
            {
                RightToLeft.Yes, 10, 240,
                new List<ScrollEventArgs>
                {
                    new ScrollEventArgs(ScrollEventType.SmallIncrement, 10, 11),
                    new ScrollEventArgs(ScrollEventType.SmallIncrement, 11, 12),
                    new ScrollEventArgs(ScrollEventType.EndScroll, 12, 12),
                }, 12
            };
            yield return new object[]
            {
                RightToLeft.Yes, 90, 120,
                new List<ScrollEventArgs>
                {
                    new ScrollEventArgs(ScrollEventType.SmallIncrement, 90, 91),
                    new ScrollEventArgs(ScrollEventType.EndScroll, 91, 91),
                }, 91
            };
            yield return new object[]
            {
                RightToLeft.Yes, 99, 120,
                new List<ScrollEventArgs>
                {
                    new ScrollEventArgs(ScrollEventType.SmallIncrement, 99, 91),
                    new ScrollEventArgs(ScrollEventType.EndScroll, 91, 91),
                }, 91
            };
            yield return new object[]
            {
                RightToLeft.Yes, 99, 240,
                new List<ScrollEventArgs>
                {
                    new ScrollEventArgs(ScrollEventType.SmallIncrement, 99, 91),
                    new ScrollEventArgs(ScrollEventType.SmallIncrement, 91, 91),
                    new ScrollEventArgs(ScrollEventType.EndScroll, 91, 91),
                }, 91
            };
        }

        [Theory]
        [MemberData(nameof(OnMouseWheel_TestData))]
        public void ScrollBar_OnMouseWheel_InvokeWithScroll_CallsScroll(RightToLeft rightToLeft, int originalValue, int delta, IList<ScrollEventArgs> expected, int expectedValue)
        {
            var scrollBar = new SubScrollBar
            {
                RightToLeft = rightToLeft,
                Value = originalValue
            };
            var eventArgs = new MouseEventArgs(MouseButtons.Left, 0, 0, 0, delta);
            int callCount = 0;
            ScrollEventHandler handler = (sender, e) =>
            {
                Assert.Same(scrollBar, sender);
                Assert.Equal(expected[callCount].Type, e.Type);
                Assert.Equal(expected[callCount].NewValue, e.NewValue);
                Assert.Equal(expected[callCount].OldValue, e.OldValue);
                Assert.Equal(ScrollOrientation.HorizontalScroll, e.ScrollOrientation);
                callCount++;
            };
            scrollBar.Scroll += handler;

            scrollBar.OnMouseWheel(eventArgs);
            Assert.Equal(expectedValue, scrollBar.Value);
            Assert.Equal(expected.Count, callCount);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetPaintEventArgsTheoryData))]
        public void ScrollBar_OnPaint_Invoke_CallsPaint(PaintEventArgs eventArgs)
        {
            var control = new SubScrollBar();
            int callCount = 0;
            PaintEventHandler handler = (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(eventArgs, e);
                callCount++;
            };

            // Call with handler.
            control.Paint += handler;
            control.OnPaint(eventArgs);
            Assert.Equal(1, callCount);

            // Remove handler.
            control.Paint -= handler;
            control.OnPaint(eventArgs);
            Assert.Equal(1, callCount);
        }

        public static IEnumerable<object[]> OnScroll_TestData()
        {
            yield return new object[] { null };
            yield return new object[] { new ScrollEventArgs(ScrollEventType.SmallDecrement, 2) };
        }

        [Theory]
        [MemberData(nameof(OnScroll_TestData))]
        public void ScrollBar_OnScroll_Invoke_CallsScroll(ScrollEventArgs eventArgs)
        {
            var control = new SubScrollBar();
            int callCount = 0;
            ScrollEventHandler handler = (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(eventArgs, e);
                callCount++;
            };

            // Call with handler.
            control.Scroll += handler;
            control.OnScroll(eventArgs);
            Assert.Equal(1, callCount);

            // Remove handler.
            control.Scroll -= handler;
            control.OnScroll(eventArgs);
            Assert.Equal(1, callCount);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetEventArgsTheoryData))]
        public void ScrollBar_OnValueChanged_Invoke_CallsValueChanged(EventArgs eventArgs)
        {
            var control = new SubScrollBar();
            int callCount = 0;
            EventHandler handler = (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(eventArgs, e);
                callCount++;
            };

            // Call with handler.
            control.ValueChanged += handler;
            control.OnValueChanged(eventArgs);
            Assert.Equal(1, callCount);

            // Remove handler.
            control.ValueChanged -= handler;
            control.OnValueChanged(eventArgs);
            Assert.Equal(1, callCount);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetBoolTheoryData))]
        public void ScrollBar_UpdateScrollInfo_NoHandle_Success(bool enabled)
        {
            var scrollBar = new SubScrollBar
            {
                Enabled = enabled
            };
            scrollBar.UpdateScrollInfo();
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetBoolTheoryData))]
        public void ScrollBar_UpdateScrollInfo_WithHandle_Success(bool enabled)
        {
            var scrollBar = new SubScrollBar
            {
                Enabled = enabled
            };
            Assert.NotEqual(IntPtr.Zero, scrollBar.Handle);
            scrollBar.UpdateScrollInfo();
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetBoolTheoryData))]
        public void ScrollBar_UpdateScrollInfo_WithHandleRightToLeft_Success(bool enabled)
        {
            var scrollBar = new SubScrollBar
            {
                Enabled = enabled,
                RightToLeft = RightToLeft.Yes
            };
            Assert.NotEqual(IntPtr.Zero, scrollBar.Handle);
            scrollBar.UpdateScrollInfo();
        }

        [Fact]
        public void ScrollBar_ToString_Invoke_ReturnsExpected()
        {
            var control = new SubScrollBar();
            Assert.Equal("System.Windows.Forms.Tests.ScrollBarTests+SubScrollBar, Minimum: 0, Maximum: 100, Value: 0", control.ToString());
        }

        public static IEnumerable<object[]> WndProc_Scroll_TestData()
        {
            foreach (int msg in new int[] { WindowMessages.WM_REFLECT + WindowMessages.WM_HSCROLL /*, WindowMessages.WM_REFLECT + WindowMessages.WM_VSCROLL  */ })
            {
                yield return new object[] { msg, RightToLeft.No, 100, ScrollEventType.SmallIncrement, 91, ScrollEventType.SmallIncrement };
                yield return new object[] { msg, RightToLeft.No, 99, ScrollEventType.SmallIncrement, 91, ScrollEventType.SmallIncrement };
                yield return new object[] { msg, RightToLeft.No, 91, ScrollEventType.SmallIncrement, 91, ScrollEventType.SmallIncrement };
                yield return new object[] { msg, RightToLeft.No, 85, ScrollEventType.SmallIncrement, 86, ScrollEventType.SmallIncrement };
                yield return new object[] { msg, RightToLeft.No, 15, ScrollEventType.SmallIncrement, 16, ScrollEventType.SmallIncrement };
                yield return new object[] { msg, RightToLeft.No, 10, ScrollEventType.SmallIncrement, 11, ScrollEventType.SmallIncrement };
                yield return new object[] { msg, RightToLeft.No, 1, ScrollEventType.SmallIncrement, 2, ScrollEventType.SmallIncrement };
                yield return new object[] { msg, RightToLeft.No, 0, ScrollEventType.SmallIncrement, 1, ScrollEventType.SmallIncrement };
                yield return new object[] { msg, RightToLeft.Yes, 100, ScrollEventType.SmallDecrement, 91, ScrollEventType.SmallIncrement };
                yield return new object[] { msg, RightToLeft.Yes, 99, ScrollEventType.SmallDecrement, 91, ScrollEventType.SmallIncrement };
                yield return new object[] { msg, RightToLeft.Yes, 91, ScrollEventType.SmallDecrement, 91, ScrollEventType.SmallIncrement };
                yield return new object[] { msg, RightToLeft.Yes, 85, ScrollEventType.SmallDecrement, 86, ScrollEventType.SmallIncrement };
                yield return new object[] { msg, RightToLeft.Yes, 15, ScrollEventType.SmallDecrement, 16, ScrollEventType.SmallIncrement };
                yield return new object[] { msg, RightToLeft.Yes, 10, ScrollEventType.SmallDecrement, 11, ScrollEventType.SmallIncrement };
                yield return new object[] { msg, RightToLeft.Yes, 1, ScrollEventType.SmallDecrement, 2, ScrollEventType.SmallIncrement };
                yield return new object[] { msg, RightToLeft.Yes, 0, ScrollEventType.SmallDecrement, 1, ScrollEventType.SmallIncrement };

                yield return new object[] { msg, RightToLeft.No, 100, ScrollEventType.LargeIncrement, 91, ScrollEventType.LargeIncrement };
                yield return new object[] { msg, RightToLeft.No, 99, ScrollEventType.LargeIncrement, 91, ScrollEventType.LargeIncrement };
                yield return new object[] { msg, RightToLeft.No, 91, ScrollEventType.LargeIncrement, 91, ScrollEventType.LargeIncrement };
                yield return new object[] { msg, RightToLeft.No, 85, ScrollEventType.LargeIncrement, 91, ScrollEventType.LargeIncrement };
                yield return new object[] { msg, RightToLeft.No, 15, ScrollEventType.LargeIncrement, 25, ScrollEventType.LargeIncrement };
                yield return new object[] { msg, RightToLeft.No, 10, ScrollEventType.LargeIncrement, 20, ScrollEventType.LargeIncrement };
                yield return new object[] { msg, RightToLeft.No, 1, ScrollEventType.LargeIncrement, 11, ScrollEventType.LargeIncrement };
                yield return new object[] { msg, RightToLeft.No, 0, ScrollEventType.LargeIncrement, 10, ScrollEventType.LargeIncrement };
                yield return new object[] { msg, RightToLeft.Yes, 100, ScrollEventType.LargeDecrement, 91, ScrollEventType.LargeIncrement };
                yield return new object[] { msg, RightToLeft.Yes, 99, ScrollEventType.LargeDecrement, 91, ScrollEventType.LargeIncrement };
                yield return new object[] { msg, RightToLeft.Yes, 91, ScrollEventType.LargeDecrement, 91, ScrollEventType.LargeIncrement };
                yield return new object[] { msg, RightToLeft.Yes, 85, ScrollEventType.LargeDecrement, 91, ScrollEventType.LargeIncrement };
                yield return new object[] { msg, RightToLeft.Yes, 15, ScrollEventType.LargeDecrement, 25, ScrollEventType.LargeIncrement };
                yield return new object[] { msg, RightToLeft.Yes, 10, ScrollEventType.LargeDecrement, 20, ScrollEventType.LargeIncrement };
                yield return new object[] { msg, RightToLeft.Yes, 1, ScrollEventType.LargeDecrement, 11, ScrollEventType.LargeIncrement };
                yield return new object[] { msg, RightToLeft.Yes, 0, ScrollEventType.LargeDecrement, 10, ScrollEventType.LargeIncrement };

                yield return new object[] { msg, RightToLeft.No, 100, ScrollEventType.SmallDecrement, 99, ScrollEventType.SmallDecrement };
                yield return new object[] { msg, RightToLeft.No, 99, ScrollEventType.SmallDecrement, 98, ScrollEventType.SmallDecrement };
                yield return new object[] { msg, RightToLeft.No, 91, ScrollEventType.SmallDecrement, 90, ScrollEventType.SmallDecrement };
                yield return new object[] { msg, RightToLeft.No, 85, ScrollEventType.SmallDecrement, 84, ScrollEventType.SmallDecrement };
                yield return new object[] { msg, RightToLeft.No, 15, ScrollEventType.SmallDecrement, 14, ScrollEventType.SmallDecrement };
                yield return new object[] { msg, RightToLeft.No, 10, ScrollEventType.SmallDecrement, 9, ScrollEventType.SmallDecrement };
                yield return new object[] { msg, RightToLeft.No, 1, ScrollEventType.SmallDecrement, 0, ScrollEventType.SmallDecrement };
                yield return new object[] { msg, RightToLeft.No, 0, ScrollEventType.SmallDecrement, 0, ScrollEventType.SmallDecrement };
                yield return new object[] { msg, RightToLeft.Yes, 100, ScrollEventType.SmallIncrement, 99, ScrollEventType.SmallDecrement };
                yield return new object[] { msg, RightToLeft.Yes, 99, ScrollEventType.SmallIncrement, 98, ScrollEventType.SmallDecrement };
                yield return new object[] { msg, RightToLeft.Yes, 91, ScrollEventType.SmallIncrement, 90, ScrollEventType.SmallDecrement };
                yield return new object[] { msg, RightToLeft.Yes, 85, ScrollEventType.SmallIncrement, 84, ScrollEventType.SmallDecrement };
                yield return new object[] { msg, RightToLeft.Yes, 15, ScrollEventType.SmallIncrement, 14, ScrollEventType.SmallDecrement };
                yield return new object[] { msg, RightToLeft.Yes, 10, ScrollEventType.SmallIncrement, 9, ScrollEventType.SmallDecrement };
                yield return new object[] { msg, RightToLeft.Yes, 1, ScrollEventType.SmallIncrement, 0, ScrollEventType.SmallDecrement };
                yield return new object[] { msg, RightToLeft.Yes, 0, ScrollEventType.SmallIncrement, 0, ScrollEventType.SmallDecrement };

                yield return new object[] { msg, RightToLeft.No, 100, ScrollEventType.LargeDecrement, 90, ScrollEventType.LargeDecrement };
                yield return new object[] { msg, RightToLeft.No, 99, ScrollEventType.LargeDecrement, 89, ScrollEventType.LargeDecrement };
                yield return new object[] { msg, RightToLeft.No, 91, ScrollEventType.LargeDecrement, 81, ScrollEventType.LargeDecrement };
                yield return new object[] { msg, RightToLeft.No, 85, ScrollEventType.LargeDecrement, 75, ScrollEventType.LargeDecrement };
                yield return new object[] { msg, RightToLeft.No, 15, ScrollEventType.LargeDecrement, 5, ScrollEventType.LargeDecrement };
                yield return new object[] { msg, RightToLeft.No, 10, ScrollEventType.LargeDecrement, 0, ScrollEventType.LargeDecrement };
                yield return new object[] { msg, RightToLeft.No, 1, ScrollEventType.LargeDecrement, 0, ScrollEventType.LargeDecrement };
                yield return new object[] { msg, RightToLeft.No, 0, ScrollEventType.LargeDecrement, 0, ScrollEventType.LargeDecrement };
                yield return new object[] { msg, RightToLeft.Yes, 100, ScrollEventType.LargeIncrement, 90, ScrollEventType.LargeDecrement };
                yield return new object[] { msg, RightToLeft.Yes, 99, ScrollEventType.LargeIncrement, 89, ScrollEventType.LargeDecrement };
                yield return new object[] { msg, RightToLeft.Yes, 91, ScrollEventType.LargeIncrement, 81, ScrollEventType.LargeDecrement };
                yield return new object[] { msg, RightToLeft.Yes, 85, ScrollEventType.LargeIncrement, 75, ScrollEventType.LargeDecrement };
                yield return new object[] { msg, RightToLeft.Yes, 15, ScrollEventType.LargeIncrement, 5, ScrollEventType.LargeDecrement };
                yield return new object[] { msg, RightToLeft.Yes, 10, ScrollEventType.LargeIncrement, 0, ScrollEventType.LargeDecrement };
                yield return new object[] { msg, RightToLeft.Yes, 1, ScrollEventType.LargeIncrement, 0, ScrollEventType.LargeDecrement };
                yield return new object[] { msg, RightToLeft.Yes, 0, ScrollEventType.LargeIncrement, 0, ScrollEventType.LargeDecrement };

                yield return new object[] { msg, RightToLeft.No, 100, ScrollEventType.First, 0, ScrollEventType.First };
                yield return new object[] { msg, RightToLeft.No, 99, ScrollEventType.First, 0, ScrollEventType.First };
                yield return new object[] { msg, RightToLeft.No, 91, ScrollEventType.First, 0, ScrollEventType.First };
                yield return new object[] { msg, RightToLeft.No, 85, ScrollEventType.First, 0, ScrollEventType.First };
                yield return new object[] { msg, RightToLeft.No, 15, ScrollEventType.First, 0, ScrollEventType.First };
                yield return new object[] { msg, RightToLeft.No, 10, ScrollEventType.First, 0, ScrollEventType.First };
                yield return new object[] { msg, RightToLeft.No, 1, ScrollEventType.First, 0, ScrollEventType.First };
                yield return new object[] { msg, RightToLeft.No, 0, ScrollEventType.First, 0, ScrollEventType.First };
                yield return new object[] { msg, RightToLeft.Yes, 100, ScrollEventType.Last, 0, ScrollEventType.First };
                yield return new object[] { msg, RightToLeft.Yes, 99, ScrollEventType.Last, 0, ScrollEventType.First };
                yield return new object[] { msg, RightToLeft.Yes, 91, ScrollEventType.Last, 0, ScrollEventType.First };
                yield return new object[] { msg, RightToLeft.Yes, 85, ScrollEventType.Last, 0, ScrollEventType.First };
                yield return new object[] { msg, RightToLeft.Yes, 15, ScrollEventType.Last, 0, ScrollEventType.First };
                yield return new object[] { msg, RightToLeft.Yes, 10, ScrollEventType.Last, 0, ScrollEventType.First };
                yield return new object[] { msg, RightToLeft.Yes, 1, ScrollEventType.Last, 0, ScrollEventType.First };
                yield return new object[] { msg, RightToLeft.Yes, 0, ScrollEventType.Last, 0, ScrollEventType.First };

                yield return new object[] { msg, RightToLeft.No, 100, ScrollEventType.Last, 91, ScrollEventType.Last };
                yield return new object[] { msg, RightToLeft.No, 99, ScrollEventType.Last, 91, ScrollEventType.Last };
                yield return new object[] { msg, RightToLeft.No, 91, ScrollEventType.Last, 91, ScrollEventType.Last };
                yield return new object[] { msg, RightToLeft.No, 85, ScrollEventType.Last, 91, ScrollEventType.Last };
                yield return new object[] { msg, RightToLeft.No, 15, ScrollEventType.Last, 91, ScrollEventType.Last };
                yield return new object[] { msg, RightToLeft.No, 10, ScrollEventType.Last, 91, ScrollEventType.Last };
                yield return new object[] { msg, RightToLeft.No, 1, ScrollEventType.Last, 91, ScrollEventType.Last };
                yield return new object[] { msg, RightToLeft.No, 0, ScrollEventType.Last, 91, ScrollEventType.Last };
                yield return new object[] { msg, RightToLeft.Yes, 100, ScrollEventType.First, 91, ScrollEventType.Last };
                yield return new object[] { msg, RightToLeft.Yes, 99, ScrollEventType.First, 91, ScrollEventType.Last };
                yield return new object[] { msg, RightToLeft.Yes, 91, ScrollEventType.First, 91, ScrollEventType.Last };
                yield return new object[] { msg, RightToLeft.Yes, 85, ScrollEventType.First, 91, ScrollEventType.Last };
                yield return new object[] { msg, RightToLeft.Yes, 15, ScrollEventType.First, 91, ScrollEventType.Last };
                yield return new object[] { msg, RightToLeft.Yes, 10, ScrollEventType.First, 91, ScrollEventType.Last };
                yield return new object[] { msg, RightToLeft.Yes, 1, ScrollEventType.First, 91, ScrollEventType.Last };
                yield return new object[] { msg, RightToLeft.Yes, 0, ScrollEventType.First, 91, ScrollEventType.Last };

                yield return new object[] { msg, RightToLeft.No, 10, ScrollEventType.EndScroll, 10, ScrollEventType.EndScroll };
                yield return new object[] { msg, RightToLeft.Yes, 10, ScrollEventType.EndScroll, 10, ScrollEventType.EndScroll };
                yield return new object[] { msg, RightToLeft.No, 10, ScrollEventType.ThumbPosition, 10, ScrollEventType.ThumbPosition };
                yield return new object[] { msg, RightToLeft.Yes, 10, ScrollEventType.ThumbPosition, 10, ScrollEventType.ThumbPosition };
                yield return new object[] { msg, RightToLeft.No, 10, ScrollEventType.ThumbTrack, 10, ScrollEventType.ThumbTrack };
                yield return new object[] { msg, RightToLeft.Yes, 10, ScrollEventType.ThumbTrack, 10, ScrollEventType.ThumbTrack };
                yield return new object[] { msg, RightToLeft.No, 10, (ScrollEventType)(ScrollEventType.SmallDecrement - 1), 10, (ScrollEventType)ushort.MaxValue };
                yield return new object[] { msg, RightToLeft.Yes, 10, (ScrollEventType)(ScrollEventType.SmallDecrement - 1), 10, (ScrollEventType)ushort.MaxValue };
                yield return new object[] { msg, RightToLeft.No, 10, (ScrollEventType)(ScrollEventType.EndScroll + 1), 10, (ScrollEventType)(ScrollEventType.EndScroll + 1) };
                yield return new object[] { msg, RightToLeft.Yes, 10, (ScrollEventType)(ScrollEventType.EndScroll + 1), 10, (ScrollEventType)(ScrollEventType.EndScroll + 1) };
            }
        }

        [Theory]
        [MemberData(nameof(WndProc_Scroll_TestData))]
        public void ScrollBar_WndProc_InvokeScroll_Success(int msg, RightToLeft rightToLeft, int originalValue, ScrollEventType eventType, int expectedValue, ScrollEventType expectedEventType)
        {
            var scrollBar = new SubScrollBar
            {
                RightToLeft = rightToLeft,
                Value = originalValue
            };
            int callCount = 0;
            ScrollEventHandler handler = (sender, e) =>
            {
                Assert.Same(scrollBar, sender);
                Assert.Equal(expectedEventType, e.Type);
                Assert.Equal(expectedValue, e.NewValue);
                Assert.Equal(originalValue, e.OldValue);
                Assert.Equal(ScrollOrientation.HorizontalScroll, e.ScrollOrientation);
                callCount++;
            };
            scrollBar.Scroll += handler;

            var message = new Message
            {
                Msg = msg,
                WParam = (IntPtr)eventType
            };
            scrollBar.WndProc(ref message);
            Assert.Equal(1, callCount);
            Assert.Equal(expectedValue, scrollBar.Value);
        }

        private class SubScrollBar : ScrollBar
        {
            public new bool CanEnableIme => base.CanEnableIme;

            public new bool CanRaiseEvents => base.CanRaiseEvents;

            public new CreateParams CreateParams => base.CreateParams;

            public new Cursor DefaultCursor => base.DefaultCursor;

            public new ImeMode DefaultImeMode => base.DefaultImeMode;

            public new Padding DefaultMargin => base.DefaultMargin;

            public new Size DefaultMaximumSize => base.DefaultMaximumSize;

            public new Size DefaultMinimumSize => base.DefaultMinimumSize;

            public new Padding DefaultPadding => base.DefaultPadding;

            public new Size DefaultSize => base.DefaultSize;

            public new bool DesignMode => base.DesignMode;

            public new EventHandlerList Events => base.Events;

            public new ImeMode ImeModeBase => base.ImeModeBase;

            public new void OnClick(EventArgs e) => base.OnClick(e);

            public new void OnDoubleClick(EventArgs e) => base.OnDoubleClick(e);

            public new void OnMouseClick(MouseEventArgs e) => base.OnMouseClick(e);

            public new void OnMouseDoubleClick(MouseEventArgs e) => base.OnMouseDoubleClick(e);

            public new void OnMouseDown(MouseEventArgs e) => base.OnMouseDown(e);

            public new void OnMouseMove(MouseEventArgs e) => base.OnMouseMove(e);

            public new void OnMouseUp(MouseEventArgs e) => base.OnMouseUp(e);

            public new void OnMouseWheel(MouseEventArgs e) => base.OnMouseWheel(e);

            public new void OnPaint(PaintEventArgs e) => base.OnPaint(e);

            public new void OnScroll(ScrollEventArgs se) => base.OnScroll(se);

            public new void OnValueChanged(EventArgs e) => base.OnValueChanged(e);

            public new void UpdateScrollInfo() => base.UpdateScrollInfo();

            public new void WndProc(ref Message m) => base.WndProc(ref m);
        }
    }
}
