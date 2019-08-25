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
    public class ControlTests
    {
        [Fact]
        public void Control_Ctor_Default()
        {
            var control = new SubControl();
            Assert.False(control.AllowDrop);
            Assert.Equal(AnchorStyles.Top | AnchorStyles.Left, control.Anchor);
            Assert.False(control.AutoSize);
            Assert.Equal(Control.DefaultBackColor, control.BackColor);
            Assert.Null(control.BackgroundImage);
            Assert.Equal(ImageLayout.Tile, control.BackgroundImageLayout);
            Assert.Null(control.BindingContext);
            Assert.Equal(0, control.Bottom);
            Assert.Equal(Rectangle.Empty, control.Bounds);
            Assert.True(control.CanEnableIme);
            Assert.True(control.CanRaiseEvents);
            Assert.True(control.CausesValidation);
            Assert.Equal(Size.Empty, control.ClientSize);
            Assert.Equal(Rectangle.Empty, control.ClientRectangle);
            Assert.Null(control.Container);
            Assert.False(control.Created);
            Assert.Same(Cursors.Default, control.Cursor);
            Assert.Same(Cursors.Default, control.DefaultCursor);
            Assert.Equal(ImeMode.Inherit, control.DefaultImeMode);
            Assert.Equal(new Padding(3), control.DefaultMargin);
            Assert.Equal(Size.Empty, control.DefaultMaximumSize);
            Assert.Equal(Size.Empty, control.DefaultMinimumSize);
            Assert.Equal(Padding.Empty, control.DefaultPadding);
            Assert.Equal(Size.Empty, control.DefaultSize);
            Assert.False(control.DesignMode);
            Assert.Equal(DpiHelper.DeviceDpi, control._deviceDpi);
            Assert.Equal(Rectangle.Empty, control.DisplayRectangle);
            Assert.Equal(DockStyle.None, control.Dock);
            Assert.True(control.Enabled);
            Assert.NotNull(control.Events);
            Assert.Same(control.Events, control.Events);
            Assert.Equal(Control.DefaultFont, control.Font);
            Assert.Equal(Control.DefaultForeColor, control.ForeColor);
            Assert.False(control.HasChildren);
            Assert.Equal(ImeMode.NoControl, control.ImeMode);
            Assert.Equal(ImeMode.NoControl, control.ImeModeBase);
            Assert.Equal(0, control.Left);
            Assert.Equal(Point.Empty, control.Location);
            Assert.Equal(new Padding(3), control.Margin);
            Assert.Equal(Padding.Empty, control.Padding);
            Assert.Null(control.Parent);
            Assert.Equal("Microsoft\u00AE .NET", control.ProductName);
            Assert.Equal(BoundsSpecified.All, control.RequiredScaling);
            Assert.True(control.RequiredScalingEnabled);
            Assert.Equal(0, control.Right);
            Assert.Equal(RightToLeft.No, control.RightToLeft);
            Assert.Null(control.Site);
            Assert.Equal(Size.Empty, control.Size);
            Assert.Equal(0, control.TabIndex);
            Assert.True(control.TabStop);
            Assert.Empty(control.Text);
            Assert.Equal(0, control.Top);
            Assert.True(control.Visible);
            Assert.Equal(0, control.Width);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetStringNormalizedTheoryData))]
        public void Control_Ctor_String(string text, string expectedText)
        {
            var control = new SubControl(text);
            Assert.False(control.AllowDrop);
            Assert.Equal(AnchorStyles.Top | AnchorStyles.Left, control.Anchor);
            Assert.False(control.AutoSize);
            Assert.Equal(Control.DefaultBackColor, control.BackColor);
            Assert.Null(control.BackgroundImage);
            Assert.Equal(ImageLayout.Tile, control.BackgroundImageLayout);
            Assert.Null(control.BindingContext);
            Assert.Equal(0, control.Bottom);
            Assert.Equal(Rectangle.Empty, control.Bounds);
            Assert.True(control.CanEnableIme);
            Assert.True(control.CanRaiseEvents);
            Assert.True(control.CausesValidation);
            Assert.Equal(Size.Empty, control.ClientSize);
            Assert.Equal(Rectangle.Empty, control.ClientRectangle);
            Assert.Null(control.Container);
            Assert.False(control.Created);
            Assert.Same(Cursors.Default, control.Cursor);
            Assert.Same(Cursors.Default, control.DefaultCursor);
            Assert.Equal(ImeMode.Inherit, control.DefaultImeMode);
            Assert.Equal(new Padding(3), control.DefaultMargin);
            Assert.Equal(Size.Empty, control.DefaultMaximumSize);
            Assert.Equal(Size.Empty, control.DefaultMinimumSize);
            Assert.Equal(Padding.Empty, control.DefaultPadding);
            Assert.Equal(Size.Empty, control.DefaultSize);
            Assert.False(control.DesignMode);
            Assert.Equal(DpiHelper.DeviceDpi, control._deviceDpi);
            Assert.Equal(Rectangle.Empty, control.DisplayRectangle);
            Assert.Equal(DockStyle.None, control.Dock);
            Assert.True(control.Enabled);
            Assert.NotNull(control.Events);
            Assert.Same(control.Events, control.Events);
            Assert.Equal(Control.DefaultFont, control.Font);
            Assert.Equal(Control.DefaultForeColor, control.ForeColor);
            Assert.False(control.HasChildren);
            Assert.Equal(0, control.Height);
            Assert.Equal(ImeMode.NoControl, control.ImeMode);
            Assert.Equal(ImeMode.NoControl, control.ImeModeBase);
            Assert.Equal(0, control.Left);
            Assert.Equal(Point.Empty, control.Location);
            Assert.Equal(new Padding(3), control.Margin);
            Assert.Equal(Padding.Empty, control.Padding);
            Assert.Null(control.Parent);
            Assert.Equal("Microsoft\u00AE .NET", control.ProductName);
            Assert.Equal(BoundsSpecified.All, control.RequiredScaling);
            Assert.True(control.RequiredScalingEnabled);
            Assert.Equal(0, control.Right);
            Assert.Equal(RightToLeft.No, control.RightToLeft);
            Assert.Null(control.Site);
            Assert.Equal(Size.Empty, control.Size);
            Assert.Equal(0, control.TabIndex);
            Assert.True(control.TabStop);
            Assert.Equal(expectedText, control.Text);
            Assert.Equal(0, control.Top);
            Assert.True(control.Visible);
            Assert.Equal(0, control.Width);
        }

        public static IEnumerable<object[]> Ctor_String_Int_Int_Int_Int_TestData()
        {
            yield return new object[] { null, -1, -2, -3, -4, string.Empty };
            yield return new object[] { string.Empty, 0, 0, 0, 0, string.Empty };
            yield return new object[] { "Text", 1, 2, 3, 4, "Text" };
        }

        [Theory]
        [MemberData(nameof(Ctor_String_Int_Int_Int_Int_TestData))]
        public void Ctor_String_Int_Int_Int_Int(string text, int left, int top, int width, int height, string expectedText)
        {
            var control = new SubControl(text, left, top, width, height);
            Assert.False(control.AllowDrop);
            Assert.Equal(AnchorStyles.Top | AnchorStyles.Left, control.Anchor);
            Assert.False(control.AutoSize);
            Assert.Equal(Control.DefaultBackColor, control.BackColor);
            Assert.Null(control.BackgroundImage);
            Assert.Equal(ImageLayout.Tile, control.BackgroundImageLayout);
            Assert.Null(control.BindingContext);
            Assert.Equal(top + height, control.Bottom);
            Assert.Equal(new Rectangle(left, top, width, height), control.Bounds);
            Assert.True(control.CanEnableIme);
            Assert.True(control.CanRaiseEvents);
            Assert.True(control.CausesValidation);
            Assert.Equal(new Rectangle(0, 0, width, height), control.ClientRectangle);
            Assert.Equal(new Size(width, height), control.ClientSize);
            Assert.Null(control.Container);
            Assert.False(control.Created);
            Assert.Same(Cursors.Default, control.Cursor);
            Assert.Same(Cursors.Default, control.DefaultCursor);
            Assert.Equal(ImeMode.Inherit, control.DefaultImeMode);
            Assert.Equal(new Padding(3), control.DefaultMargin);
            Assert.Equal(Size.Empty, control.DefaultMaximumSize);
            Assert.Equal(Size.Empty, control.DefaultMinimumSize);
            Assert.Equal(Padding.Empty, control.DefaultPadding);
            Assert.Equal(Size.Empty, control.DefaultSize);
            Assert.False(control.DesignMode);
            Assert.Equal(DpiHelper.DeviceDpi, control._deviceDpi);
            Assert.Equal(new Rectangle(0, 0, width, height), control.DisplayRectangle);
            Assert.Equal(DockStyle.None, control.Dock);
            Assert.True(control.Enabled);
            Assert.NotNull(control.Events);
            Assert.Same(control.Events, control.Events);
            Assert.Equal(Control.DefaultFont, control.Font);
            Assert.Equal(Control.DefaultForeColor, control.ForeColor);
            Assert.False(control.HasChildren);
            Assert.Equal(height, control.Height);
            Assert.Equal(ImeMode.NoControl, control.ImeMode);
            Assert.Equal(ImeMode.NoControl, control.ImeModeBase);
            Assert.Equal(left, control.Left);
            Assert.Equal(new Point(left, top), control.Location);
            Assert.Equal(new Padding(3), control.Margin);
            Assert.Equal(Padding.Empty, control.Padding);
            Assert.Null(control.Parent);
            Assert.Equal("Microsoft\u00AE .NET", control.ProductName);
            Assert.Equal(BoundsSpecified.All, control.RequiredScaling);
            Assert.True(control.RequiredScalingEnabled);
            Assert.Equal(left + width, control.Right);
            Assert.Equal(RightToLeft.No, control.RightToLeft);
            Assert.Null(control.Site);
            Assert.Equal(new Size(width, height), control.Size);
            Assert.Equal(0, control.TabIndex);
            Assert.True(control.TabStop);
            Assert.Equal(expectedText, control.Text);
            Assert.Equal(top, control.Top);
            Assert.True(control.Visible);
            Assert.Equal(width, control.Width);
        }

        public static IEnumerable<object[]> Ctor_Control_String_TestData()
        {
            yield return new object[] { null, null, string.Empty };
            yield return new object[] { new Control(), string.Empty, string.Empty };
            yield return new object[] { new Control(), "text", "text" };
        }

        [Theory]
        [MemberData(nameof(Ctor_Control_String_TestData))]
        public void Control_Ctor_Control_String(Control parent, string text, string expectedText)
        {
            var control = new SubControl(parent, text);
            Assert.False(control.AllowDrop);
            Assert.Equal(AnchorStyles.Top | AnchorStyles.Left, control.Anchor);
            Assert.False(control.AutoSize);
            Assert.Equal(Control.DefaultBackColor, control.BackColor);
            Assert.Null(control.BackgroundImage);
            Assert.Equal(ImageLayout.Tile, control.BackgroundImageLayout);
            Assert.Null(control.BindingContext);
            Assert.Equal(0, control.Bottom);
            Assert.Equal(Rectangle.Empty, control.Bounds);
            Assert.True(control.CanEnableIme);
            Assert.True(control.CanRaiseEvents);
            Assert.True(control.CausesValidation);
            Assert.Equal(Rectangle.Empty, control.ClientRectangle);
            Assert.Equal(Size.Empty, control.ClientSize);
            Assert.Null(control.Container);
            Assert.False(control.Created);
            Assert.Same(Cursors.Default, control.Cursor);
            Assert.Same(Cursors.Default, control.DefaultCursor);
            Assert.Equal(ImeMode.Inherit, control.DefaultImeMode);
            Assert.Equal(new Padding(3), control.DefaultMargin);
            Assert.Equal(Size.Empty, control.DefaultMaximumSize);
            Assert.Equal(Size.Empty, control.DefaultMinimumSize);
            Assert.Equal(Padding.Empty, control.DefaultPadding);
            Assert.Equal(Size.Empty, control.DefaultSize);
            Assert.False(control.DesignMode);
            Assert.Equal(DpiHelper.DeviceDpi, control._deviceDpi);
            Assert.Equal(Rectangle.Empty, control.DisplayRectangle);
            Assert.Equal(DockStyle.None, control.Dock);
            Assert.True(control.Enabled);
            Assert.NotNull(control.Events);
            Assert.Same(control.Events, control.Events);
            Assert.Equal(Control.DefaultFont, control.Font);
            Assert.Equal(Control.DefaultForeColor, control.ForeColor);
            Assert.False(control.HasChildren);
            Assert.Equal(0, control.Height);
            Assert.Equal(ImeMode.NoControl, control.ImeMode);
            Assert.Equal(ImeMode.NoControl, control.ImeModeBase);
            Assert.Equal(0, control.Left);
            Assert.Equal(Point.Empty, control.Location);
            Assert.Equal(new Padding(3), control.Margin);
            Assert.Equal(Padding.Empty, control.Padding);
            Assert.Same(parent, control.Parent);
            Assert.Equal("Microsoft\u00AE .NET", control.ProductName);
            Assert.Equal(BoundsSpecified.All, control.RequiredScaling);
            Assert.True(control.RequiredScalingEnabled);
            Assert.Equal(0, control.Right);
            Assert.Equal(RightToLeft.No, control.RightToLeft);
            Assert.Null(control.Site);
            Assert.Equal(Size.Empty, control.Size);
            Assert.Equal(0, control.TabIndex);
            Assert.True(control.TabStop);
            Assert.Same(expectedText, control.Text);
            Assert.Equal(0, control.Top);
            Assert.True(control.Visible);
            Assert.Equal(0, control.Width);
        }

        public static IEnumerable<object[]> Ctor_Control_String_Int_Int_Int_Int_TestData()
        {
            yield return new object[] { null, null, -1, -2, -3, -4, string.Empty };
            yield return new object[] { new Control(), string.Empty, 0, 0, 0, 0, string.Empty };
            yield return new object[] { new Control(), "text", 1, 2, 3, 4, "text" };
        }

        [Theory]
        [MemberData(nameof(Ctor_Control_String_Int_Int_Int_Int_TestData))]
        public void Control_Ctor_Control_String_Int_Int_Int_Int(Control parent, string text, int left, int top, int width, int height, string expectedText)
        {
            var control = new SubControl(parent, text, left, top, width, height);
            Assert.False(control.AllowDrop);
            Assert.Equal(AnchorStyles.Top | AnchorStyles.Left, control.Anchor);
            Assert.False(control.AutoSize);
            Assert.Equal(Control.DefaultBackColor, control.BackColor);
            Assert.Null(control.BackgroundImage);
            Assert.Equal(ImageLayout.Tile, control.BackgroundImageLayout);
            Assert.Null(control.BindingContext);
            Assert.Equal(top + height, control.Bottom);
            Assert.Equal(new Rectangle(left, top, width, height), control.Bounds);
            Assert.True(control.CanEnableIme);
            Assert.True(control.CanRaiseEvents);
            Assert.True(control.CausesValidation);
            Assert.Equal(new Rectangle(0, 0, width, height), control.ClientRectangle);
            Assert.Equal(new Size(width, height), control.ClientSize);
            Assert.Null(control.Container);
            Assert.Empty(control.Controls);
            Assert.Same(control.Controls, control.Controls);
            Assert.False(control.Created);
            Assert.Same(Cursors.Default, control.Cursor);
            Assert.Same(Cursors.Default, control.DefaultCursor);
            Assert.Equal(ImeMode.Inherit, control.DefaultImeMode);
            Assert.Equal(new Padding(3), control.DefaultMargin);
            Assert.Equal(Size.Empty, control.DefaultMaximumSize);
            Assert.Equal(Size.Empty, control.DefaultMinimumSize);
            Assert.Equal(Padding.Empty, control.DefaultPadding);
            Assert.Equal(Size.Empty, control.DefaultSize);
            Assert.False(control.DesignMode);
            Assert.Equal(DpiHelper.DeviceDpi, control._deviceDpi);
            Assert.Equal(new Rectangle(0, 0, width, height), control.DisplayRectangle);
            Assert.Equal(DockStyle.None, control.Dock);
            Assert.True(control.Enabled);
            Assert.NotNull(control.Events);
            Assert.Same(control.Events, control.Events);
            Assert.Equal(Control.DefaultFont, control.Font);
            Assert.Equal(Control.DefaultForeColor, control.ForeColor);
            Assert.False(control.HasChildren);
            Assert.Equal(height, control.Height);
            Assert.Equal(ImeMode.NoControl, control.ImeMode);
            Assert.Equal(ImeMode.NoControl, control.ImeModeBase);
            Assert.Equal(left, control.Left);
            Assert.Equal(new Point(left, top), control.Location);
            Assert.Equal(new Padding(3), control.Margin);
            Assert.Equal(Padding.Empty, control.Padding);
            Assert.Same(parent, control.Parent);
            Assert.Equal("Microsoft\u00AE .NET", control.ProductName);
            Assert.Equal(BoundsSpecified.All, control.RequiredScaling);
            Assert.True(control.RequiredScalingEnabled);
            Assert.Equal(left + width, control.Right);
            Assert.Equal(RightToLeft.No, control.RightToLeft);
            Assert.Null(control.Site);
            Assert.Equal(new Size(width, height), control.Size);
            Assert.Equal(0, control.TabIndex);
            Assert.True(control.TabStop);
            Assert.Same(expectedText, control.Text);
            Assert.Equal(top, control.Top);
            Assert.True(control.Visible);
            Assert.Equal(width, control.Width);
        }

        [Fact]
        public void Control_DefaultBackColor_Get_ReturnsExpected()
        {
            Assert.Equal(SystemColors.Control, Control.DefaultBackColor);
        }

        [Fact]
        public void Control_DefaultForeColor_Get_ReturnsExpected()
        {
            Assert.Equal(SystemColors.ControlText, Control.DefaultForeColor);
        }

        [Fact]
        public void Control_DefaultFont_Get_ReturnsExpected()
        {
            Font font = Control.DefaultFont;
            Assert.Equal(SystemFonts.MessageBoxFont, Control.DefaultFont);
            Assert.Same(font, Control.DefaultFont);
        }

        [Fact]
        public void Control_CreateParams_GetDefault_ReturnsExpected()
        {
            var control = new SubControl();
            CreateParams createParams = control.CreateParams;
            Assert.Null(createParams.Caption);
            Assert.Null(createParams.ClassName);
            Assert.Equal(0x8, createParams.ClassStyle);
            Assert.Equal(0, createParams.ExStyle);
            Assert.Equal(0, createParams.Height);
            Assert.Equal(IntPtr.Zero, createParams.Parent);
            Assert.Null(createParams.Param);
            Assert.Equal(0x56010000, createParams.Style);
            Assert.Equal(0, createParams.Width);
            Assert.Equal(0, createParams.X);
            Assert.Equal(0, createParams.Y);
            Assert.Same(createParams, control.CreateParams);
        }

        #region Control Creation

        [Fact]
        public void Control_CreateControl()
        {
            var cont = new Control();

            cont.CreateControl();

            Assert.True(cont.Created);
        }

        /// <summary>
        ///  Data for the CreateControlInternal test
        /// </summary>
        public static TheoryData<bool> CreateControlInternalData =>
            CommonTestHelper.GetBoolTheoryData();

        [Theory]
        [MemberData(nameof(CreateControlInternalData))]
        public void Control_CreateControlInternal(bool fIgnoreVisible)
        {
            var cont = new Control();

            cont.CreateControl(fIgnoreVisible);

            Assert.True(cont.Created);
        }

        #endregion

        #region Parenting

        #region Tabbing

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(2)]
        public void Control_TabIndex_Set_GetReturnsExpected(int value)
        {
            var control = new Control
            {
                TabIndex = value
            };
            Assert.Equal(value, control.TabIndex);

            // Set same.
            control.TabIndex = value;
            Assert.Equal(value, control.TabIndex);
        }

        [Fact]
        public void Control_TabIndex_SetWithHandler_CallsTabIndexChanged()
        {
            var control = new Control
            {
                TabIndex = 0
            };
            int callCount = 0;
            EventHandler handler = (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(EventArgs.Empty, e);
                callCount++;
            };
            control.TabIndexChanged += handler;

            // Set different.
            control.TabIndex = 1;
            Assert.Equal(1, control.TabIndex);
            Assert.Equal(1, callCount);

            // Set same.
            control.TabIndex = 1;
            Assert.Equal(1, control.TabIndex);
            Assert.Equal(1, callCount);

            // Set different.
            control.TabIndex = 2;
            Assert.Equal(2, control.TabIndex);
            Assert.Equal(2, callCount);

            // Remove handler.
            control.TabIndexChanged -= handler;
            control.TabIndex = 1;
            Assert.Equal(1, control.TabIndex);
            Assert.Equal(2, callCount);
        }

        [Fact]
        public void Control_TabIndex_SetNegative_CallsArgumentOutOfRangeException()
        {
            var control = new Control();
            Assert.Throws<ArgumentOutOfRangeException>("value", () => control.TabIndex = -1);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetBoolTheoryData))]
        public void Control_TabStop_Set_GetReturnsExpected(bool value)
        {
            var control = new Control
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
        public void Control_TabStop_SetWithHandle_GetReturnsExpected(bool value)
        {
            var control = new Control();
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
        public void Control_TabStop_SetWithHandler_CallsTabStopChanged()
        {
            var control = new Control
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
        [CommonMemberData(nameof(CommonTestHelper.GetBoolTheoryData))]
        public void Control_TabStopInternal_Set_GetReturnsExpected(bool value)
        {
            var control = new Control
            {
                TabStopInternal = value
            };
            Assert.Equal(value, control.TabStopInternal);

            // Set same.
            control.TabStopInternal = value;
            Assert.Equal(value, control.TabStopInternal);

            // Set different.
            control.TabStopInternal = value;
            Assert.Equal(value, control.TabStopInternal);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetBoolTheoryData))]
        public void Control_TabStopInternal_SetWithHandle_GetReturnsExpected(bool value)
        {
            var control = new Control();
            Assert.NotEqual(IntPtr.Zero, control.Handle);

            control.TabStopInternal = value;
            Assert.Equal(value, control.TabStopInternal);

            // Set same.
            control.TabStopInternal = value;
            Assert.Equal(value, control.TabStopInternal);

            // Set different.
            control.TabStopInternal = value;
            Assert.Equal(value, control.TabStopInternal);
        }

        [Fact]
        public void Control_TabStopInternal_SetWithHandler_DoesNotCallTabStopChanged()
        {
            var control = new Control
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
            control.TabStopInternal = false;
            Assert.False(control.TabStopInternal);
            Assert.Equal(0, callCount);

            // Set same.
            control.TabStopInternal = false;
            Assert.False(control.TabStopInternal);
            Assert.Equal(0, callCount);

            // Set different.
            control.TabStopInternal = true;
            Assert.True(control.TabStopInternal);
            Assert.Equal(0, callCount);

            // Remove handler.
            control.TabStopChanged -= handler;
            control.TabStopInternal = false;
            Assert.False(control.TabStopInternal);
            Assert.Equal(0, callCount);
        }

        [Fact]
        public void Control_GetChildControlsInTabOrder()
        {
            var cont = new Control();
            var first = new Control
            {
                TabIndex = 0
            };
            var second = new Control
            {
                TabIndex = 1
            };
            var third = new Control
            {
                TabIndex = 2
            };
            var ordered = new Control[]
            {
                first,
                second,
                third
            };
            var unordered = new Control[]
            {
                second,
                first,
                third
            };
            cont.Controls.AddRange(unordered);

            Control[] tabOrderedChildren = cont.GetChildControlsInTabOrder(false);

            Assert.Equal(ordered, tabOrderedChildren);
        }

        [Fact]
        public void Control_GetChildControlsInTabOrderHandlesOnly()
        {
            var cont = new Control();
            var first = new Control
            {
                TabIndex = 0
            };
            var second = new Control
            {
                TabIndex = 1
            };
            var third = new Control
            {
                TabIndex = 2
            };
            var unordered = new Control[]
            {
                second,
                first,
                third
            };
            cont.Controls.AddRange(unordered);

            Control[] tabOrderedChildrenWithhandlesOnly = cont.GetChildControlsInTabOrder(true);

            Assert.Empty(tabOrderedChildrenWithhandlesOnly);
        }

        [Fact]
        public void Control_GetFirstChildControlInTabOrder()
        {
            var cont = new Control();
            var first = new Control
            {
                TabIndex = 0
            };
            var second = new Control
            {
                TabIndex = 1
            };
            var third = new Control
            {
                TabIndex = 2
            };
            var tabOrder = new Control[]
            {
                second,
                first,
                third
            };
            cont.Controls.AddRange(tabOrder);

            // act and assert
            Assert.Equal(first, cont.GetFirstChildControlInTabOrder(true));
        }

        [Fact]
        public void Control_GetFirstChildControlInTabOrderReverse()
        {
            var cont = new Control();
            var first = new Control
            {
                TabIndex = 0
            };
            var second = new Control
            {
                TabIndex = 1
            };
            var third = new Control
            {
                TabIndex = 2
            };
            var tabOrder = new Control[]
            {
                second,
                first,
                third
            };
            cont.Controls.AddRange(tabOrder);

            // act and assert
            Assert.Equal(third, cont.GetFirstChildControlInTabOrder(false));
        }

        [Fact]
        public void Control_GetNextControl()
        {
            var cont = new Control();
            var first = new Control
            {
                TabIndex = 0
            };
            var second = new Control
            {
                TabIndex = 1
            };
            var third = new Control
            {
                TabIndex = 2
            };
            var tabOrder = new Control[]
            {
                second,
                first,
                third
            };
            cont.Controls.AddRange(tabOrder);

            // act and assert
            Assert.Equal(second, cont.GetNextControl(first, true));
        }

        [Fact]
        public void Control_GetNextControlReverse()
        {
            var cont = new Control();
            var first = new Control
            {
                TabIndex = 0
            };
            var second = new Control
            {
                TabIndex = 1
            };
            var third = new Control
            {
                TabIndex = 2
            };
            var tabOrder = new Control[]
            {
                second,
                first,
                third
            };
            cont.Controls.AddRange(tabOrder);

            // act and assert
            Assert.Equal(first, cont.GetNextControl(second, false));
        }

        [Fact]
        public void Control_GetNextControlNoNext()
        {
            var cont = new Control();
            var first = new Control
            {
                TabIndex = 0
            };
            var second = new Control
            {
                TabIndex = 1
            };
            var third = new Control
            {
                TabIndex = 2
            };
            var tabOrder = new Control[]
            {
                second,
                first,
                third
            };
            cont.Controls.AddRange(tabOrder);

            // act and assert
            Assert.Null(cont.GetNextControl(third, true));
        }

        [Fact]
        public void Control_GetNextControlNoNextReverse()
        {
            var cont = new Control();
            var first = new Control
            {
                TabIndex = 0
            };
            var second = new Control
            {
                TabIndex = 1
            };
            var third = new Control
            {
                TabIndex = 2
            };
            var tabOrder = new Control[]
            {
                second,
                first,
                third
            };
            cont.Controls.AddRange(tabOrder);

            // act and assert
            Assert.Null(cont.GetNextControl(first, false));
        }

        #endregion

        [Fact]
        public void Control_AssignParent()
        {
            var cont = new Control();
            var parent = new Control();

            cont.AssignParent(parent);

            Assert.Equal(parent, cont.Parent);
        }

        [Fact]
        public void Control_ParentChangedFromAssign()
        {
            bool wasChanged = false;
            var cont = new Control();
            cont.ParentChanged += (sender, args) => wasChanged = true;
            var parent = new Control();

            cont.AssignParent(parent);

            Assert.True(wasChanged);
        }

        public static IEnumerable<object[]> Parent_Set_TestData()
        {
            yield return new object[] { null };
            yield return new object[] { new Control() };
        }

        [Theory]
        [MemberData(nameof(Parent_Set_TestData))]
        public void Control_Parent_Set_GetReturnsExpected(Control value)
        {
            var control = new Control
            {
                Parent = value
            };
            Assert.Same(value, control.Parent);

            // Set same.
            control.Parent = value;
            Assert.Same(value, control.Parent);
        }

        [Theory]
        [MemberData(nameof(Parent_Set_TestData))]
        public void Control_Parent_SetWithNonNullOldParent_GetReturnsExpected(Control value)
        {
            var oldParent = new Control();
            var control = new Control
            {
                Parent = oldParent
            };
            
            control.Parent = value;
            Assert.Same(value, control.Parent);
            Assert.Empty(oldParent.Controls);

            // Set same.
            control.Parent = value;
            Assert.Same(value, control.Parent);
            Assert.Empty(oldParent.Controls);
        }

        [Fact]
        public void Control_Parent_SetNonNull_AddsToControls()
        {
            var parent = new Control();
            var control = new Control
            {
                Parent = parent
            };
            Assert.Same(parent, control.Parent);
            Assert.Same(control, Assert.Single(parent.Controls));

            // Set same.
            control.Parent = parent;
            Assert.Same(parent, control.Parent);
            Assert.Same(control, Assert.Single(parent.Controls));
        }

        [Fact]
        public void Control_Parent_SetWithHandler_CallsParentChanged()
        {
            var parent = new Control();
            var control = new Control();
            int callCount = 0;
            EventHandler handler = (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(EventArgs.Empty, e);
                callCount++;
            };
            control.ParentChanged += handler;

            // Set different.
            control.Parent = parent;
            Assert.Same(parent, control.Parent);
            Assert.Equal(1, callCount);

            // Set same.
            control.Parent = parent;
            Assert.Same(parent, control.Parent);
            Assert.Equal(1, callCount);

            // Set null.
            control.Parent = null;
            Assert.Null(control.Parent);
            Assert.Equal(2, callCount);

            // Remove handler.
            control.ParentChanged -= handler;
            control.Parent = parent;
            Assert.Same(parent, control.Parent);
            Assert.Equal(2, callCount);
        }

        [Fact]
        public void Control_Parent_SetSame_ThrowsArgumentException()
        {
            var control = new Control();
            Assert.Throws<ArgumentException>(null, () => control.Parent = control);
            Assert.Null(control.Parent);
        }

        [Theory]
        [MemberData(nameof(Parent_Set_TestData))]
        public void Control_ParentInternal_Set_GetReturnsExpected(Control value)
        {
            var control = new Control
            {
                ParentInternal = value
            };
            Assert.Same(value, control.ParentInternal);

            // Set same.
            control.ParentInternal = value;
            Assert.Same(value, control.ParentInternal);
        }

        [Theory]
        [MemberData(nameof(Parent_Set_TestData))]
        public void Control_ParentInternal_SetWithNonNullOldParent_GetReturnsExpected(Control value)
        {
            var oldParent = new Control();
            var control = new Control
            {
                ParentInternal = oldParent
            };
            
            control.ParentInternal = value;
            Assert.Same(value, control.ParentInternal);
            Assert.Empty(oldParent.Controls);

            // Set same.
            control.ParentInternal = value;
            Assert.Same(value, control.ParentInternal);
            Assert.Empty(oldParent.Controls);
        }

        [Fact]
        public void Control_ParentInternal_SetNonNull_AddsToControls()
        {
            var parent = new Control();
            var control = new Control
            {
                ParentInternal = parent
            };
            Assert.Same(parent, control.ParentInternal);
            Assert.Same(control, Assert.Single(parent.Controls));

            // Set same.
            control.ParentInternal = parent;
            Assert.Same(parent, control.ParentInternal);
            Assert.Same(control, Assert.Single(parent.Controls));
        }

        [Fact]
        public void Control_ParentInternal_SetWithHandler_CallsParentChanged()
        {
            var parent = new Control();
            var control = new Control();
            int callCount = 0;
            EventHandler handler = (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(EventArgs.Empty, e);
                callCount++;
            };
            control.ParentChanged += handler;

            // Set different.
            control.ParentInternal = parent;
            Assert.Same(parent, control.ParentInternal);
            Assert.Equal(1, callCount);

            // Set same.
            control.ParentInternal = parent;
            Assert.Same(parent, control.ParentInternal);
            Assert.Equal(1, callCount);

            // Set null.
            control.ParentInternal = null;
            Assert.Null(control.ParentInternal);
            Assert.Equal(2, callCount);

            // Remove handler.
            control.ParentChanged -= handler;
            control.ParentInternal = parent;
            Assert.Same(parent, control.ParentInternal);
            Assert.Equal(2, callCount);
        }

        [Fact]
        public void Control_ParentInternal_SetSame_ThrowsArgumentException()
        {
            var control = new Control();
            Assert.Throws<ArgumentException>(null, () => control.ParentInternal = control);
            Assert.Null(control.ParentInternal);
        }

        [Fact]
        public void Control_GetContainerControl()
        {
            var cont = new Control();

            IContainerControl ret = cont.GetContainerControl();

            Assert.Null(ret);
        }

        #region Contains

        [Fact]
        public void Control_Contains()
        {
            var cont = new Control();
            var child = new Control();
            cont.Controls.Add(child);

            // act and assert
            Assert.True(cont.Contains(child));
        }

        [Fact]
        public void Control_ContainsGrandchild()
        {
            var cont = new Control();
            var child = new Control();
            var grandchild = new Control();
            cont.Controls.Add(child);
            child.Controls.Add(grandchild);

            // act and assert
            Assert.True(cont.Contains(grandchild));
        }

        [Fact]
        public void Control_ContainsNot()
        {
            var cont = new Control();
            var child = new Control();

            // act and assert
            Assert.False(cont.Contains(child));
        }

        #endregion

        [Fact]
        public void Control_ParentingExcpetion()
        {
            var bootstrap = new Control();
            var paradox = new Control();
            bootstrap.Parent = paradox;

            // act and assert
            Assert.Throws<ArgumentException>(() => paradox.Parent = bootstrap);
        }

        [Fact]
        public void Control_ChildingExcpetion()
        {
            var bootstrap = new Control();
            var paradox = new Control();
            bootstrap.Controls.Add(paradox);

            // act and assert
            Assert.Throws<ArgumentException>(() => paradox.Controls.Add(bootstrap));
        }

        #endregion

        #region Accesability

        [Fact]
        public void Control_AccessibleNameGetSet()
        {
            var cont = new Control
            {
                AccessibleName = "Foo"
            };

            Assert.Equal("Foo", cont.AccessibleName);
        }

        /// <summary>
        ///  Data for the AccessibleRole test
        /// </summary>
        public static TheoryData<AccessibleRole> AccessibleRoleData =>
            CommonTestHelper.GetEnumTheoryData<AccessibleRole>();

        [Theory]
        [MemberData(nameof(AccessibleRoleData))]
        public void Control_SetItemCheckState(AccessibleRole expected)
        {
            var cont = new Control
            {
                AccessibleRole = expected
            };

            Assert.Equal(expected, cont.AccessibleRole);
        }

        /// <summary>
        ///  Data for the AccessibleRoleInvalid test
        /// </summary>
        public static TheoryData<CheckState> AccessibleRoleInvalidData =>
            CommonTestHelper.GetEnumTheoryDataInvalid<CheckState>();

        [Theory]
        [MemberData(nameof(AccessibleRoleInvalidData))]
        public void Control_AccessibleRoleInvalid(AccessibleRole expected)
        {
            var cont = new Control();

            InvalidEnumArgumentException ex = Assert.Throws<InvalidEnumArgumentException>(() => cont.AccessibleRole = expected);
            Assert.Equal("value", ex.ParamName);
        }

        /// <summary>
        ///  Data for the IsAccessibleGetSet test
        /// </summary>
        public static TheoryData<bool> IsAccessibleGetSetData =>
            CommonTestHelper.GetBoolTheoryData();

        [Theory]
        [MemberData(nameof(IsAccessibleGetSetData))]
        public void Control_IsAccessibleGetSet(bool expected)
        {
            var cont = new Control
            {
                IsAccessible = expected
            };

            Assert.Equal(expected, cont.IsAccessible);
        }

        #endregion

        #region Colors

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetBackColorTheoryData))]
        public void Control_BackColor_Set_GetReturnsExpected(Color value, Color expected)
        {
            var control = new Control
            {
                BackColor = value
            };
            Assert.Equal(expected, control.BackColor);

            // Set same.
            control.BackColor = value;
            Assert.Equal(expected, control.BackColor);
        }

        [Fact]
        public void Control_BackColor_SetWithHandler_CallsBackColorChanged()
        {
            var control = new Control();
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
        [CommonMemberData(nameof(CommonTestHelper.GetForeColorTheoryData))]
        public void Control_ForeColor_Set_GetReturnsExpected(Color value, Color expected)
        {
            var control = new Control
            {
                ForeColor = value
            };
            Assert.Equal(expected, control.ForeColor);

            // Set same.
            control.ForeColor = value;
            Assert.Equal(expected, control.ForeColor);
        }

        [Fact]
        public void Control_ForeColor_SetWithHandler_CallsForeColorChanged()
        {
            var control = new Control();
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

        #endregion

        #region ImageLayout

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetImageTheoryData))]
        public void Control_BackgroundImage_Set_GetReturnsExpected(Image value)
        {
            var control = new Control
            {
                BackgroundImage = value
            };
            Assert.Equal(value, control.BackgroundImage);

            // Set same.
            control.BackgroundImage = value;
            Assert.Equal(value, control.BackgroundImage);
        }

        [Fact]
        public void Control_BackgroundImage_SetWithHandler_CallsBackgroundImageChanged()
        {
            var control = new Control();
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
        public void Control_BackgroundImageLayout_Set_GetReturnsExpected(ImageLayout value)
        {
            var control = new Control
            {
                BackgroundImageLayout = value
            };
            Assert.Equal(value, control.BackgroundImageLayout);

            // Set same.
            control.BackgroundImageLayout = value;
            Assert.Equal(value, control.BackgroundImageLayout);
        }

        [Fact]
        public void Control_BackgroundImageLayout_SetWithHandler_CallsBackgroundImageLayoutChanged()
        {
            var control = new Control();
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
        public void Control_BackgroundImageLayout_SetInvalid_ThrowsInvalidEnumArgumentException(ImageLayout value)
        {
            var control = new Control();
            Assert.Throws<InvalidEnumArgumentException>("value", () => control.BackgroundImageLayout = value);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetEnumTypeTheoryData), typeof(ImageLayout))]
        public void Control_ImeMode_Set_GetReturnsExpected(ImeMode value)
        {
            var control = new Control
            {
                ImeMode = value
            };
            Assert.Equal(value, control.ImeMode);

            // Set same.
            control.ImeMode = value;
            Assert.Equal(value, control.ImeMode);
        }

        [Fact]
        public void Control_ImeMode_SetWithHandler_CallsImeModeChanged()
        {
            var control = new Control();
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
            Assert.Equal(1, callCount);

            // Set same.
            control.ImeMode = ImeMode.On;
            Assert.Equal(ImeMode.On, control.ImeMode);
            Assert.Equal(1, callCount);

            // Set different.
            control.ImeMode = ImeMode.Off;
            Assert.Equal(ImeMode.Off, control.ImeMode);
            Assert.Equal(2, callCount);

            // Remove handler.
            control.ImeModeChanged -= handler;
            control.ImeMode = ImeMode.Off;
            Assert.Equal(ImeMode.Off, control.ImeMode);
            Assert.Equal(2, callCount);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetEnumTypeTheoryDataInvalid), typeof(ImeMode))]
        public void Control_ImeMode_SetInvalid_ThrowsInvalidEnumArgumentException(ImeMode value)
        {
            var control = new Control();
            Assert.Throws<InvalidEnumArgumentException>("value", () => control.ImeMode = value);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetEnumTypeTheoryData), typeof(ImageLayout))]
        public void Control_ImeModeBase_Set_GetReturnsExpected(ImeMode value)
        {
            var control = new SubControl
            {
                ImeModeBase = value
            };
            Assert.Equal(value, control.ImeModeBase);

            // Set same.
            control.ImeModeBase = value;
            Assert.Equal(value, control.ImeModeBase);
        }

        [Fact]
        public void Control_ImeModeBase_SetWithHandler_CallsImeModeBaseChanged()
        {
            var control = new SubControl();
            int callCount = 0;
            EventHandler handler = (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(EventArgs.Empty, e);
                callCount++;
            };
            control.ImeModeChanged += handler;

            // Set different.
            control.ImeModeBase = ImeMode.On;
            Assert.Equal(ImeMode.On, control.ImeModeBase);
            Assert.Equal(1, callCount);

            // Set same.
            control.ImeModeBase = ImeMode.On;
            Assert.Equal(ImeMode.On, control.ImeModeBase);
            Assert.Equal(1, callCount);

            // Set different.
            control.ImeModeBase = ImeMode.Off;
            Assert.Equal(ImeMode.Off, control.ImeModeBase);
            Assert.Equal(2, callCount);

            // Remove handler.
            control.ImeModeChanged -= handler;
            control.ImeModeBase = ImeMode.Off;
            Assert.Equal(ImeMode.Off, control.ImeModeBase);
            Assert.Equal(2, callCount);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetEnumTypeTheoryDataInvalid), typeof(ImeMode))]
        public void Control_ImeModeBase_SetInvalid_ThrowsInvalidEnumArgumentException(ImeMode value)
        {
            var control = new SubControl();
            Assert.Throws<InvalidEnumArgumentException>("value", () => control.ImeModeBase = value);
        }

        #endregion

        #region Place and Shape

        [Fact]
        public void Control_RegionGetSet()
        {
            var cont = new Control();
            var expected = new Region(new Rectangle(1, 1, 20, 20));

            cont.Region = expected;

            Assert.Equal(expected, cont.Region);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetBoolTheoryData))]
        public void Control_AutoSize_Set_GetReturnsExpected(bool value)
        {
            var control = new Control
            {
                AutoSize = value
            };
            Assert.Equal(value, control.AutoSize);

            // Set same.
            control.AutoSize = value;
            Assert.Equal(value, control.AutoSize);

            // Set different.
            control.AutoSize = value;
            Assert.Equal(value, control.AutoSize);
        }

        [Fact]
        public void Control_AutoSize_SetWithHandler_CallsAutoSizeChanged()
        {
            var control = new Control
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

        [Fact]
        public void Control_PreferedSizeGet()
        {
            var cont = new Control();

            // act and assert
            Assert.Equal(Size.Empty, cont.PreferredSize);
        }

        #region ApplySizeConstraints

        /// <summary>
        ///  Data for the ApplySizeConstraints test
        /// </summary>
        public static TheoryData<int> ApplySizeConstraintsData =>
            CommonTestHelper.GetIntTheoryData();

        [Theory]
        [MemberData(nameof(ApplySizeConstraintsData))]
        public void Control_ApplySizeConstraints(int expected)
        {
            var cont = new Control();
            var expectedSize = new Size(expected, expected);

            Size actualSize = cont.ApplySizeConstraints(expected, expected);

            Assert.Equal(expectedSize, actualSize);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetSizeTheoryData))]
        public void Control_ApplySizeConstraintsSize_Invoke_ReturnsExpected(Size expectedSize)
        {
            var control = new Control();
            Size actualSize = control.ApplySizeConstraints(expectedSize);
            Assert.Equal(expectedSize, actualSize);
        }

        #endregion

        #region ApplyBoundsConstraints

        /// <summary>
        ///  Data for the ApplyBoundsConstraints test
        /// </summary>
        public static TheoryData<int> ApplyBoundsConstraintsData =>
            CommonTestHelper.GetIntTheoryData();

        [Theory]
        [MemberData(nameof(ApplySizeConstraintsData))]
        public void Control_ApplyBoundsConstraints(int expected)
        {
            var cont = new Control();
            var expectedBounds = new Rectangle(expected, expected, expected, expected);

            Rectangle actualBounds = cont.ApplyBoundsConstraints(expected, expected, expected, expected);

            Assert.Equal(expectedBounds, actualBounds);
        }

        #endregion

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetPaddingNormalizedTheoryData))]
        public void Control_Padding_Set_GetReturnsExpected(Padding value, Padding expected)
        {
            var control = new Control
            {
                Padding = value
            };
            Assert.Equal(expected, control.Padding);

            // Set same.
            control.Padding = value;
            Assert.Equal(expected, control.Padding);
        }

        public static IEnumerable<object[]> Anchor_Set_TestData()
        {
            yield return new object[] { AnchorStyles.Top, AnchorStyles.Top };
            yield return new object[] { AnchorStyles.Top | AnchorStyles.Bottom, AnchorStyles.Top | AnchorStyles.Bottom };
            yield return new object[] { AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left, AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left };
            yield return new object[] { AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Right, AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Right };
            yield return new object[] { AnchorStyles.Top | AnchorStyles.Left, AnchorStyles.Top | AnchorStyles.Left };
            yield return new object[] { AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right, AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right };
            yield return new object[] { AnchorStyles.Top | AnchorStyles.Right, AnchorStyles.Top | AnchorStyles.Right };
            yield return new object[] { AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right, AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right };

            yield return new object[] { AnchorStyles.Bottom, AnchorStyles.Bottom };
            yield return new object[] { AnchorStyles.Bottom | AnchorStyles.Left, AnchorStyles.Bottom | AnchorStyles.Left };
            yield return new object[] { AnchorStyles.Bottom | AnchorStyles.Right, AnchorStyles.Bottom | AnchorStyles.Right };
            yield return new object[] { AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right, AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right };

            yield return new object[] { AnchorStyles.Left, AnchorStyles.Left };
            yield return new object[] { AnchorStyles.Left | AnchorStyles.Right, AnchorStyles.Left | AnchorStyles.Right };

            yield return new object[] { AnchorStyles.Right, AnchorStyles.Right };

            yield return new object[] { AnchorStyles.None, AnchorStyles.None };
            yield return new object[] { (AnchorStyles)(-1), AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right };
            yield return new object[] { (AnchorStyles)int.MaxValue, AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right };
        }

        [Theory]
        [MemberData(nameof(Anchor_Set_TestData))]
        public void Control_Anchor_Set_GetReturnsExpected(AnchorStyles value, AnchorStyles expected)
        {
            var control = new Control
            {
                Anchor = value
            };
            Assert.Equal(expected, control.Anchor);
            Assert.Equal(DockStyle.None, control.Dock);

            // Set same.
            control.Anchor = value;
            Assert.Equal(expected, control.Anchor);
            Assert.Equal(DockStyle.None, control.Dock);
        }

        [Theory]
        [MemberData(nameof(Anchor_Set_TestData))]
        public void Control_Anchor_SetWithOldValue_GetReturnsExpected(AnchorStyles value, AnchorStyles expected)
        {
            var control = new Control
            {
                Anchor = AnchorStyles.Left
            };

            control.Anchor = value;
            Assert.Equal(expected, control.Anchor);
            Assert.Equal(DockStyle.None, control.Dock);

            // Set same.
            control.Anchor = value;
            Assert.Equal(expected, control.Anchor);
            Assert.Equal(DockStyle.None, control.Dock);
        }

        public static IEnumerable<object[]> Anchor_SetWithDock_TestData()
        {
            foreach (DockStyle dock in Enum.GetValues(typeof(DockStyle)))
            {
                yield return new object[] { dock, AnchorStyles.Top, AnchorStyles.Top, DockStyle.None };
                yield return new object[] { dock, AnchorStyles.Top | AnchorStyles.Bottom, AnchorStyles.Top | AnchorStyles.Bottom, DockStyle.None };
                yield return new object[] { dock, AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left, AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left, DockStyle.None };
                yield return new object[] { dock, AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Right, AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Right, DockStyle.None };
                yield return new object[] { dock, AnchorStyles.Top | AnchorStyles.Left, AnchorStyles.Top | AnchorStyles.Left, dock };
                yield return new object[] { dock, AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right, AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right, DockStyle.None };
                yield return new object[] { dock, AnchorStyles.Top | AnchorStyles.Right, AnchorStyles.Top | AnchorStyles.Right, DockStyle.None };
                yield return new object[] { dock, AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right, AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right, DockStyle.None };

                yield return new object[] { dock, AnchorStyles.Bottom, AnchorStyles.Bottom, DockStyle.None };
                yield return new object[] { dock, AnchorStyles.Bottom | AnchorStyles.Left, AnchorStyles.Bottom | AnchorStyles.Left, DockStyle.None };
                yield return new object[] { dock, AnchorStyles.Bottom | AnchorStyles.Right, AnchorStyles.Bottom | AnchorStyles.Right, DockStyle.None };
                yield return new object[] { dock, AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right, AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right, DockStyle.None };

                yield return new object[] { dock, AnchorStyles.Left, AnchorStyles.Left, DockStyle.None };
                yield return new object[] { dock, AnchorStyles.Left | AnchorStyles.Right, AnchorStyles.Left | AnchorStyles.Right, DockStyle.None };

                yield return new object[] { dock, AnchorStyles.Right, AnchorStyles.Right, DockStyle.None };

                yield return new object[] { dock, AnchorStyles.None, AnchorStyles.None, DockStyle.None };
                yield return new object[] { dock, (AnchorStyles)(-1), AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right, DockStyle.None };
                yield return new object[] { dock, (AnchorStyles)int.MaxValue, AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right, DockStyle.None };
            }
        }

        [Theory]
        [MemberData(nameof(Anchor_SetWithDock_TestData))]
        public void Control_Anchor_SetWithDock_GetReturnsExpected(DockStyle dock, AnchorStyles value, AnchorStyles expectedAnchor, DockStyle expectedDock)
        {
            var control = new Control
            {
                Dock = dock
            };

            control.Anchor = value;
            Assert.Equal(expectedAnchor, control.Anchor);
            Assert.Equal(expectedDock, control.Dock);

            // Set same.
            control.Anchor = value;
            Assert.Equal(expectedAnchor, control.Anchor);
            Assert.Equal(expectedDock, control.Dock);
        }

        [Fact]
        public void Control_BoundsGetSet()
        {
            var cont = new Control();
            var expected = new Rectangle(1, 1, 20, 20);

            cont.Bounds = expected;

            Assert.Equal(expected, cont.Bounds);
        }

        /// <summary>
        ///  Data for the HeightGetSet test
        /// </summary>
        public static TheoryData<int> HeightGetSetData =>
            CommonTestHelper.GetIntTheoryData();

        [Theory]
        [MemberData(nameof(HeightGetSetData))]
        public void Control_HeightGetSet(int expected)
        {
            var cont = new Control
            {
                Height = expected
            };

            Assert.Equal(expected, cont.Height);
        }

        /// <summary>
        ///  Data for the LeftGetSet test
        /// </summary>
        public static TheoryData<int> LeftGetSetData =>
            CommonTestHelper.GetIntTheoryData();

        [Theory]
        [MemberData(nameof(LeftGetSetData))]
        public void Control_LeftGetSet(int expected)
        {
            var cont = new Control
            {
                Left = expected
            };

            Assert.Equal(expected, cont.Left);
        }

        /// <summary>
        ///  Data for the TopGetSet test
        /// </summary>
        public static TheoryData<int> TopGetSetData =>
            CommonTestHelper.GetIntTheoryData();

        [Theory]
        [MemberData(nameof(TopGetSetData))]
        public void Control_TopGetSet(int expected)
        {
            var cont = new Control
            {
                Top = expected
            };

            Assert.Equal(expected, cont.Top);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetPointTheoryData))]
        public void Control_Location_Set_GetReturnsExpected(Point value)
        {
            var control = new Control
            {
                Location = value
            };
            Assert.Equal(value, control.Location);

            // Set same.
            control.Location = value;
            Assert.Equal(value, control.Location);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetPointTheoryData))]
        public void Control_Location_SetWithParent_GetReturnsExpected(Point value)
        {
            var parent = new Control();
            var control = new Control
            {
                Parent = parent,
                Location = value
            };
            Assert.Equal(value, control.Location);

            // Set same.
            control.Location = value;
            Assert.Equal(value, control.Location);
        }

        [Fact]
        public void Control_Location_SetWithHandle_DoesNotCallInvalidate()
        {
            var control = new Control();
            Assert.NotEqual(IntPtr.Zero, control.Handle);
            int invalidatedCallCount = 0;
            control.Invalidated += (sender, e) => invalidatedCallCount++;

            // Set different.
            control.Location = new Point(1, 2);
            Assert.Equal(new Point(1, 2), control.Location);
            Assert.Equal(0, invalidatedCallCount);

            // Set same.
            control.Location = new Point(1, 2);
            Assert.Equal(new Point(1, 2), control.Location);
            Assert.Equal(0, invalidatedCallCount);

            // Set different.
            control.Location = new Point(2, 3);
            Assert.Equal(new Point(2, 3), control.Location);
            Assert.Equal(0, invalidatedCallCount);
        }

        [Theory]
        [InlineData(true, 1)]
        [InlineData(false, 0)]
        public void Control_Location_SetWithHandleWithTransparentBackColor_DoesNotCallInvalidate(bool supportsTransparentBackgroundColor, int expectedInvalidatedCallCount)
        {
            var control = new SubControl();
            control.SetStyle(ControlStyles.SupportsTransparentBackColor, true);
            control.BackColor = Color.FromArgb(254, 255, 255, 255);
            control.SetStyle(ControlStyles.SupportsTransparentBackColor, supportsTransparentBackgroundColor);

            Assert.NotEqual(IntPtr.Zero, control.Handle);
            int invalidatedCallCount = 0;
            control.Invalidated += (sender, e) => invalidatedCallCount++;

            // Set different.
            control.Location = new Point(1, 2);
            Assert.Equal(new Point(1, 2), control.Location);
            Assert.Equal(expectedInvalidatedCallCount, invalidatedCallCount);

            // Set same.
            control.Location = new Point(1, 2);
            Assert.Equal(new Point(1, 2), control.Location);
            Assert.Equal(expectedInvalidatedCallCount, invalidatedCallCount);

            // Set different.
            control.Location = new Point(2, 3);
            Assert.Equal(new Point(2, 3), control.Location);
            Assert.Equal(expectedInvalidatedCallCount * 2, invalidatedCallCount);
        }

        [Fact]
        public void Control_Location_SetWithHandler_CallsLocationChanged()
        {
            var control = new Control();
            int locationChangedCallCount = 0;
            EventHandler locationChangedHandler = (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(EventArgs.Empty, e);
                locationChangedCallCount++;
            };
            control.LocationChanged += locationChangedHandler;
            int moveCallCount = 0;
            EventHandler moveHandler = (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(EventArgs.Empty, e);
                moveCallCount++;
            };
            control.Move += moveHandler;

            // Set different.
            control.Location = new Point(1, 2);
            Assert.Equal(new Point(1, 2), control.Location);
            Assert.Equal(1, locationChangedCallCount);
            Assert.Equal(1, moveCallCount);

            // Set same.
            control.Location = new Point(1, 2);
            Assert.Equal(new Point(1, 2), control.Location);
            Assert.Equal(1, locationChangedCallCount);
            Assert.Equal(1, moveCallCount);

            // Set different x.
            control.Location = new Point(2, 2);
            Assert.Equal(new Point(2, 2), control.Location);
            Assert.Equal(2, locationChangedCallCount);
            Assert.Equal(2, moveCallCount);

            // Set different y.
            control.Location = new Point(2, 3);
            Assert.Equal(new Point(2, 3), control.Location);
            Assert.Equal(3, locationChangedCallCount);
            Assert.Equal(3, moveCallCount);

            // Remove handler.
            control.LocationChanged -= locationChangedHandler;
            control.Move -= moveHandler;
            control.Location = new Point(1, 2);
            Assert.Equal(new Point(1, 2), control.Location);
            Assert.Equal(3, locationChangedCallCount);
            Assert.Equal(3, moveCallCount);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetPaddingNormalizedTheoryData))]
        public void Control_Margin_Set_GetReturnsExpected(Padding value, Padding expected)
        {
            var control = new Control
            {
                Margin = value
            };
            Assert.Equal(expected, control.Margin);

            // Set same.
            control.Margin = value;
            Assert.Equal(expected, control.Margin);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetSizeTheoryData))]
        public void Control_MaximumSize_Set_GetReturnsExpected(Size value)
        {
            var control = new Control
            {
                MaximumSize = value
            };
            Assert.Equal(value, control.MaximumSize);

            // Set same.
            control.MaximumSize = value;
            Assert.Equal(value, control.MaximumSize);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetSizeTheoryData))]
        public void Control_MinimumSize_Set_GetReturnsExpected(Size value)
        {
            var control = new Control
            {
                MinimumSize = value
            };
            Assert.Equal(value, control.MinimumSize);

            // Set same.
            control.MinimumSize = value;
            Assert.Equal(value, control.MinimumSize);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetEnumTypeTheoryData), typeof(BoundsSpecified))]
        public void Control_RequiredScaling_Set_GetReturnsExpected(BoundsSpecified value)
        {
            var control = new Control
            {
                RequiredScaling = value
            };
            Assert.Equal(value, control.RequiredScaling);

            // Set same.
            control.RequiredScaling = value;
            Assert.Equal(value, control.RequiredScaling);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetBoolTheoryData))]
        public void Control_RequiredScalingEnabled_Get_ReturnsExpected(bool value)
        {
            var control = new Control
            {
                RequiredScalingEnabled = value
            };
            Assert.Equal(value, control.RequiredScalingEnabled);

            // Set same.
            control.RequiredScalingEnabled = value;
            Assert.Equal(value, control.RequiredScalingEnabled);

            // Set different.
            control.RequiredScalingEnabled = !value;
            Assert.Equal(!value, control.RequiredScalingEnabled);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetRightToLeftTheoryData))]
        public void Control_RightToLeft_Set_GetReturnsExpected(RightToLeft value, RightToLeft expected)
        {
            var control = new Control
            {
                RightToLeft = value
            };
            Assert.Equal(expected, control.RightToLeft);

            // Set same.
            control.RightToLeft = value;
            Assert.Equal(expected, control.RightToLeft);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetRightToLeftTheoryData))]
        public void Control_RightToLeft_SetAsParent_GetReturnsExpected(RightToLeft value, RightToLeft expected)
        {
            var parent = new Control
            {
                RightToLeft = value
            };
            var control = new Control
            {
                Parent = parent
            };
            Assert.Equal(expected, control.RightToLeft);

            // Set same.
            control.RightToLeft = value;
            Assert.Equal(expected, control.RightToLeft);
        }

        [Fact]
        public void Control_RightToLeft_SetWithHandler_CallsRightToLeftChanged()
        {
            var control = new Control();
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
        public void Control_RightToLeft_SetInvalid_ThrowsInvalidEnumArgumentException(RightToLeft value)
        {
            var control = new Control();
            Assert.Throws<InvalidEnumArgumentException>("value", () => control.RightToLeft = value);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetSizeTheoryData))]
        public void Control_SizeGetSet(Size value)
        {
            var control = new Control
            {
                Size = value
            };
            Assert.Equal(value, control.Size);

            // Set same.
            control.Size = value;
            Assert.Equal(value, control.Size);
        }

        /// <summary>
        ///  Data for the WidthGetSet test
        /// </summary>
        public static TheoryData<int> WidthGetSetData =>
            CommonTestHelper.GetIntTheoryData();

        [Theory]
        [MemberData(nameof(WidthGetSetData))]
        public void Control_WidthGetSet(int expected)
        {
            var cont = new Control
            {
                Width = expected
            };

            Assert.Equal(expected, cont.Width);
        }

        #endregion

        #region Events

        [Fact]
        public void Control_Enter()
        {
            bool wasChanged = false;
            var cont = new Control();
            cont.Enter += (sender, args) => wasChanged = true;

            cont.NotifyEnter();

            Assert.True(wasChanged);
        }

        [Fact]
        public void Control_Leave()
        {
            bool wasChanged = false;
            var cont = new Control();
            cont.Leave += (sender, args) => wasChanged = true;

            cont.NotifyLeave();

            Assert.True(wasChanged);
        }

        [Fact]
        public void Control_PaddingChanged()
        {
            bool wasChanged = false;
            var cont = new Control();
            cont.PaddingChanged += (sender, args) => wasChanged = true;

            cont.Padding = new Padding(1);

            Assert.True(wasChanged);
        }

        [Fact]
        public void Control_ControlAddedAndRemoved()
        {
            bool wasAdded = false;
            bool wasRemoved = false;
            var cont = new Control();
            cont.ControlAdded += (sender, args) => wasAdded = true;
            cont.ControlRemoved += (sender, args) => wasRemoved = true;
            var child = new Control();

            cont.Controls.Add(child);
            cont.Controls.Remove(child);

            Assert.True(wasAdded);
            Assert.True(wasRemoved);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetFontTheoryData))]
        public void Control_Font_Set_GetReturnsExpected(Font value)
        {
            var control = new Control
            {
                Font = value
            };
            Assert.Equal(value ?? Control.DefaultFont, control.Font);

            // Set same.
            control.Font = value;
            Assert.Equal(value ?? Control.DefaultFont, control.Font);
        }

        [Fact]
        public void Control_Font_SetWithHandler_CallsFontChanged()
        {
            var control = new Control();
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

        [Fact]
        public void Control_MarginChanged()
        {
            bool wasChanged = false;
            var cont = new Control();
            cont.MarginChanged += (sender, args) => wasChanged = true;

            cont.Margin = new Padding(1);

            Assert.True(wasChanged);
        }

        [Fact]
        public void Control_RegionChanged()
        {
            bool wasChanged = false;
            var cont = new Control();
            cont.RegionChanged += (sender, args) => wasChanged = true;

            cont.Region = new Region(new Rectangle(1, 1, 20, 20));

            Assert.True(wasChanged);
        }

        [Fact]
        public void Control_SizeChanged()
        {
            bool wasChanged = false;
            var cont = new Control();
            cont.SizeChanged += (sender, args) => wasChanged = true;

            cont.Size = new Size(50, 50);

            Assert.True(wasChanged);
        }

        [Fact]
        public void Control_TabIndexChanged()
        {
            bool wasChanged = false;
            var cont = new Control();
            cont.TabIndexChanged += (sender, args) => wasChanged = true;

            cont.TabIndex = 1;

            Assert.True(wasChanged);
        }

        #endregion

        #region Enabled and Visible

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetBoolTheoryData))]
        public void Control_Enabled_Set_GetReturnsExpected(bool value)
        {
            var control = new Control
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
        public void Control_Enabled_SetWithHandle_GetReturnsExpected(bool value)
        {
            var control = new Control();
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
        public void Control_Enabled_SetWithHandler_CallsEnabledChanged()
        {
            var control = new Control
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
        [CommonMemberData(nameof(CommonTestHelper.GetBoolTheoryData))]
        public void Control_Visible_Set_GetReturnsExpected(bool value)
        {
            var control = new Control
            {
                Visible = value
            };
            Assert.Equal(value, control.Visible);

            // Set same.
            control.Visible = value;
            Assert.Equal(value, control.Visible);

            // Set different.
            control.Visible = value;
            Assert.Equal(value, control.Visible);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetBoolTheoryData))]
        public void Control_Visible_SetWithHandle_GetReturnsExpected(bool value)
        {
            var control = new Control();
            Assert.NotEqual(IntPtr.Zero, control.Handle);

            control.Visible = value;
            Assert.Equal(value, control.Visible);

            // Set same.
            control.Visible = value;
            Assert.Equal(value, control.Visible);

            // Set different.
            control.Visible = value;
            Assert.Equal(value, control.Visible);
        }

        [Fact]
        public void Control_Visible_SetWithHandler_CallsVisibleChanged()
        {
            var control = new Control
            {
                Visible = true
            };
            int callCount = 0;
            EventHandler handler = (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(EventArgs.Empty, e);
                callCount++;
            };
            control.VisibleChanged += handler;

            // Set different.
            control.Visible = false;
            Assert.False(control.Visible);
            Assert.Equal(1, callCount);

            // Set same.
            control.Visible = false;
            Assert.False(control.Visible);
            Assert.Equal(1, callCount);

            // Set different.
            control.Visible = true;
            Assert.True(control.Visible);
            Assert.Equal(2, callCount);

            // Remove handler.
            control.VisibleChanged -= handler;
            control.Visible = false;
            Assert.False(control.Visible);
            Assert.Equal(2, callCount);
        }

        [Fact]
        public void Control_GetVisibleCore_NoParent_ReturnsExpected()
        {
            var control = new Control();
            Assert.Null(control.Parent);
            Assert.True(control.GetVisibleCore());
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetBoolTheoryData))]
        public void Control_GetVisibleCoreVisible_Parent_ReturnsExpected(bool value)
        {
            var control = new Control();
            var parent = new Control
            {
                Visible = value
            };
            control.Parent = parent;

            Assert.Equal(value, control.GetVisibleCore());
        }

        [Fact]
        public void Control_Hide_Invoke_SetsInvisible()
        {
            var control = new Control
            {
                Visible = true
            };
            control.Hide();
            Assert.False(control.Visible);

            // Hide again.
            control.Hide();
            Assert.False(control.Visible);
        }

        [Fact]
        public void Control_Hide_InvokeWithHandler_CallsVisibleChanged()
        {
            var control = new Control
            {
                Visible = true
            };
            int callCount = 0;
            EventHandler handler = (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(EventArgs.Empty, e);
                callCount++;
            };
            control.VisibleChanged += handler;

            control.Hide();
            Assert.False(control.Visible);
            Assert.Equal(1, callCount);

            // Call again.
            control.Hide();
            Assert.False(control.Visible);
            Assert.Equal(1, callCount);

            // Remove handler.
            control.VisibleChanged -= handler;
            control.Visible = true;
            control.Hide();
            Assert.False(control.Visible);
            Assert.Equal(1, callCount);
        }

        #endregion

        #region Font

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetFontTheoryData))]
        public void Control_FontGetSet(Font value)
        {
            var control = new Control
            {
                Font = value
            };
            Assert.Equal(value ?? Control.DefaultFont, control.Font);

            // Set same.
            control.Font = value;
            Assert.Equal(value ?? Control.DefaultFont, control.Font);
        }

        [Theory]
        [InlineData(10.0f)]
        [InlineData(0.1f)]
        [InlineData(float.Epsilon)]
        public void Control_ScaleFont(float expected)
        {
            var cont = new Control
            {
                Font = new Font(new FontFamily(Drawing.Text.GenericFontFamilies.Serif), 1.0f)
            };

            cont.ScaleFont(expected);

            Assert.Equal(expected, cont.Font.Size);
        }

        #endregion

        #region Name and Text

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetStringWithNullTheoryData))]
        public void Control_WindowText_Set_GetReturnsExpected(string value)
        {
            var control = new Control
            {
                WindowText = value
            };
            Assert.Equal(value ?? string.Empty, control.WindowText);

            // Set same.
            control.WindowText = value;
            Assert.Equal(value ?? string.Empty, control.WindowText);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetStringWithNullTheoryData))]
        public void Control_WindowText_SetWithHandle_GetReturnsExpected(string value)
        {
            var control = new Control();
            Assert.NotEqual(IntPtr.Zero, control.Handle);

            control.WindowText = value;
            Assert.Equal(value ?? string.Empty, control.WindowText);

            // Set same.
            control.WindowText = value;
            Assert.Equal(value ?? string.Empty, control.WindowText);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetStringWithNullTheoryData))]
        public void Control_Name_Set_GetReturnsExpected(string value)
        {
            var control = new Control
            {
                Name = value
            };
            Assert.Equal(value ?? string.Empty, control.Name);

            // Set same.
            control.Name = value;
            Assert.Equal(value ?? string.Empty, control.Name);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetStringNormalizedTheoryData))]
        public void Control_Text_Set_GetReturnsExpected(string value, string expected)
        {
            var control = new Control
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
        public void Control_Text_SetWithHandle_GetReturnsExpected(string value, string expected)
        {
            var control = new Control();
            Assert.NotEqual(IntPtr.Zero, control.Handle);

            control.Text = value;
            Assert.Equal(expected, control.Text);

            // Set same.
            control.Text = value;
            Assert.Equal(expected, control.Text);
        }

        [Fact]
        public void Control_Text_SetWithHandler_CallsTextChanged()
        {
            var control = new Control();
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

        #endregion

        #region Capture

        /// <summary>
        ///  Data for the CaptureGetSet test
        /// </summary>
        public static TheoryData<bool> CaptureGetSetData =>
            CommonTestHelper.GetBoolTheoryData();

        [Theory]
        [MemberData(nameof(CaptureGetSetData))]
        public void Control_CaptureGetSet(bool expected)
        {
            var cont = new Control
            {
                Capture = expected
            };

            Assert.Equal(expected, cont.Capture);
        }

        /// <summary>
        ///  Data for the CaptureInternalGetSet test
        /// </summary>
        public static TheoryData<bool> CaptureInternalGetSetData =>
            CommonTestHelper.GetBoolTheoryData();

        [Theory]
        [MemberData(nameof(CaptureInternalGetSetData))]
        public void Control_CaptureInternalGetSet(bool expected)
        {
            var cont = new Control
            {
                CaptureInternal = expected
            };

            Assert.Equal(expected, cont.CaptureInternal);
        }

        #endregion

        #region CanProcessMnemonic

        [Fact]
        public void Control_CanProcessMnemonic()
        {
            var cont = new Control();

            // act and assert
            Assert.True(cont.CanProcessMnemonic());
        }

        [Fact]
        public void Control_CanProcessMnemonicNotEnabled()
        {
            var cont = new Control
            {
                Enabled = false
            };

            // act and assert
            Assert.False(cont.CanProcessMnemonic());
        }

        [Fact]
        public void Control_CanProcessMnemonicNotVisible()
        {
            var cont = new Control
            {
                Visible = false
            };

            // act and assert
            Assert.False(cont.CanProcessMnemonic());
        }

        [Fact]
        public void Control_CanProcessMnemonicParent()
        {
            var cont = new Control();
            var parent = new Control();
            cont.AssignParent(parent);

            // act and assert
            Assert.True(cont.CanProcessMnemonic());
        }

        #endregion

        #region CanSelectCore

        [Fact]
        public void Control_CanSelectCore()
        {
            var cont = new Control();

            // act and assert
            Assert.True(cont.CanSelectCore());
        }

        [Fact]
        public void Control_CanSelectCoreNotEnabled()
        {
            var cont = new Control
            {
                Enabled = false
            };

            // act and assert
            Assert.False(cont.CanSelectCore());
        }

        [Fact]
        public void Control_CanSelectCoreParentNotEnabled()
        {
            var cont = new Control
            {
                Enabled = true
            };
            var parent = new Control
            {
                Enabled = false
            };
            cont.AssignParent(parent);

            // act and assert
            Assert.False(cont.CanSelectCore());
        }

        #endregion

        [Fact]
        public void Control_FindFormWithParent_ReturnsForm()
        {
            var control = new Control();
            var form = new Form();
            control.Parent = form;
            Assert.Equal(form, control.FindForm());
        }

        [Fact]
        public void Control_FindFormWithoutParent_ReturnsNull()
        {
            var control = new Control();
            Assert.Null(control.FindForm());
        }

        #region GetChildAtPoint

        /// <summary>
        ///  Data for the GetChildAtPointNull test
        /// </summary>
        public static TheoryData<GetChildAtPointSkip> GetChildAtPointNullData =>
            CommonTestHelper.GetEnumTheoryData<GetChildAtPointSkip>();

        [Theory]
        [MemberData(nameof(GetChildAtPointNullData))]
        public void Control_GetChildAtPointNull(GetChildAtPointSkip skip)
        {
            var cont = new Control();

            Control ret = cont.GetChildAtPoint(new Point(5, 5), skip);

            Assert.Null(ret);
        }

        /// <summary>
        ///  Data for the GetChildAtPointInvalid test
        /// </summary>
        public static TheoryData<GetChildAtPointSkip> GetChildAtPointInvalidData =>
            CommonTestHelper.GetEnumTheoryDataInvalid<GetChildAtPointSkip>();

        [Theory]
        [MemberData(nameof(GetChildAtPointInvalidData))]
        public void Control_GetChildAtPointInvalid(GetChildAtPointSkip skip)
        {
            var cont = new Control();

            // act & assert
            InvalidEnumArgumentException ex = Assert.Throws<InvalidEnumArgumentException>(() => cont.GetChildAtPoint(new Point(5, 5), skip));
            Assert.Equal("skipValue", ex.ParamName);
        }

        #endregion

        [Fact]
        public void Control_GetHandle()
        {
            var cont = new Control();

            IntPtr intptr = cont.Handle;

            Assert.NotEqual(IntPtr.Zero, intptr);
        }

        [Fact]
        public void Control_GetHandleInternalShouldBeZero()
        {
            var cont = new Control();

            IntPtr intptr = cont.HandleInternal;

            Assert.Equal(IntPtr.Zero, intptr);
            Assert.False(cont.IsHandleCreated);
        }

        /// <summary>
        ///  Data for the DoDragDrop test
        /// </summary>
        public static TheoryData<DragDropEffects> DoDragDropData =>
            CommonTestHelper.GetEnumTheoryData<DragDropEffects>();

        [Theory]
        [MemberData(nameof(DoDragDropData))]
        public void Control_DoDragDrop(DragDropEffects expected)
        {
            var cont = new Control();
            var mock = new Mock<IDataObject>(MockBehavior.Strict);

            DragDropEffects ret = cont.DoDragDrop(mock.Object, expected);

            Assert.Equal(DragDropEffects.None, ret);
        }

        // TODO: create a focus test that returns true when a handle has been created
        [Fact]
        public void Control_FocusHandleNotCreated()
        {
            var cont = new Control();

            var ret = cont.Focus();

            Assert.False(ret);
        }

        #region Misc. GetSet

        public static IEnumerable<object[]> Site_Set_TestData()
        {
            yield return new object[] { null };

            var mockNullSite = new Mock<ISite>(MockBehavior.Strict);
            mockNullSite
                .Setup(s => s.GetService(typeof(AmbientProperties)))
                .Returns(null);
            yield return new object[] { mockNullSite.Object };

            var mockInvalidSite = new Mock<ISite>(MockBehavior.Strict);
            mockInvalidSite
                .Setup(s => s.GetService(typeof(AmbientProperties)))
                .Returns(new object());
            yield return new object[] { mockInvalidSite.Object };

            var mockSite = new Mock<ISite>(MockBehavior.Strict);
            mockSite
                .Setup(s => s.GetService(typeof(AmbientProperties)))
                .Returns(new AmbientProperties());
            yield return new object[] { mockSite.Object };
        }

        [Theory]
        [MemberData(nameof(Site_Set_TestData))]
        public void Control_Site_Set_GetReturnsExpected(ISite value)
        {
            var control = new Control
            {
                Site = value
            };
            Assert.Same(value, control.Site);

            // Set same.
            control.Site = value;
            Assert.Same(value, control.Site);
        }

        [Theory]
        [MemberData(nameof(Site_Set_TestData))]
        public void Control_Site_SetWithNonNullOldValue_GetReturnsExpected(ISite value)
        {
            var mockSite = new Mock<ISite>(MockBehavior.Strict);
            mockSite
                .Setup(s => s.GetService(typeof(AmbientProperties)))
                .Returns(new AmbientProperties());
            var control = new Control
            {
                Site = mockSite.Object
            };

            control.Site = value;
            Assert.Same(value, control.Site);

            // Set same.
            control.Site = value;
            Assert.Same(value, control.Site);
        }

        [Fact]
        public void Control_Site_SetWithoutAmbientPropertiesSet_UpdatesProperties()
        {
            Font font1 = SystemFonts.CaptionFont;
            Font font2 = SystemFonts.DialogFont;
            Cursor cursor1 = Cursors.AppStarting;
            Cursor cursor2 = Cursors.Arrow;
            var properties = new AmbientProperties
            {
                BackColor = Color.Blue,
                Cursor = cursor1,
                Font = font1,
                ForeColor = Color.Red
            };
            var mockSite1 = new Mock<ISite>(MockBehavior.Strict);
            mockSite1
                .Setup(s => s.GetService(typeof(AmbientProperties)))
                .Returns(properties)
                .Verifiable();

            var sameProperties = new AmbientProperties
            {
                BackColor = Color.Blue,
                Cursor = cursor1,
                Font = font1,
                ForeColor = Color.Red
            };
            var mockSite2 = new Mock<ISite>(MockBehavior.Strict);
            mockSite2
                .Setup(s => s.GetService(typeof(AmbientProperties)))
                .Returns(sameProperties)
                .Verifiable();

            var differentProperties = new AmbientProperties
            {
                BackColor = Color.Red,
                Cursor = cursor2,
                Font = font2,
                ForeColor = Color.Blue
            };
            var mockSite3 = new Mock<ISite>(MockBehavior.Strict);
            mockSite3
                .Setup(s => s.GetService(typeof(AmbientProperties)))
                .Returns(differentProperties)
                .Verifiable();

            var mockSite4 = new Mock<ISite>(MockBehavior.Strict);
            mockSite4
                .Setup(s => s.GetService(typeof(AmbientProperties)))
                .Returns(null)
                .Verifiable();

            var control = new Control();
            int backColorChangedCallCount = 0;
            control.BackColorChanged += (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(EventArgs.Empty, e);
                backColorChangedCallCount++;
            };
            int cursorChangedCallCount = 0;
            control.CursorChanged += (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(EventArgs.Empty, e);
                cursorChangedCallCount++;
            };
            int foreColorChangedCallCount = 0;
            control.ForeColorChanged += (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(EventArgs.Empty, e);
                foreColorChangedCallCount++;
            };
            int fontChangedCallCount = 0;
            control.FontChanged += (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(EventArgs.Empty, e);
                fontChangedCallCount++;
            };

            control.Site = mockSite1.Object;
            Assert.Same(mockSite1.Object, control.Site);
            Assert.Equal(Color.Blue, control.BackColor);
            Assert.Same(cursor1, control.Cursor);
            Assert.Equal(Color.Red, control.ForeColor);
            Assert.Same(font1, control.Font);
            mockSite1.Verify(s => s.GetService(typeof(AmbientProperties)), Times.Once());
            mockSite2.Verify(s => s.GetService(typeof(AmbientProperties)), Times.Never());
            mockSite3.Verify(s => s.GetService(typeof(AmbientProperties)), Times.Never());
            mockSite4.Verify(s => s.GetService(typeof(AmbientProperties)), Times.Never());
            Assert.Equal(1, backColorChangedCallCount);
            Assert.Equal(0, cursorChangedCallCount);
            Assert.Equal(1, foreColorChangedCallCount);
            Assert.Equal(0, fontChangedCallCount);

            // Set same.
            control.Site = mockSite1.Object;
            Assert.Same(mockSite1.Object, control.Site);
            Assert.Equal(Color.Blue, control.BackColor);
            Assert.Same(cursor1, control.Cursor);
            Assert.Equal(Color.Red, control.ForeColor);
            Assert.Same(font1, control.Font);
            mockSite1.Verify(s => s.GetService(typeof(AmbientProperties)), Times.Exactly(2));
            mockSite2.Verify(s => s.GetService(typeof(AmbientProperties)), Times.Never());
            mockSite3.Verify(s => s.GetService(typeof(AmbientProperties)), Times.Never());
            mockSite4.Verify(s => s.GetService(typeof(AmbientProperties)), Times.Never());
            Assert.Equal(1, backColorChangedCallCount);
            Assert.Equal(0, cursorChangedCallCount);
            Assert.Equal(1, foreColorChangedCallCount);
            Assert.Equal(0, fontChangedCallCount);

            // Set equal.
            control.Site = mockSite2.Object;
            Assert.Same(mockSite2.Object, control.Site);
            Assert.Equal(Color.Blue, control.BackColor);
            Assert.Same(cursor1, control.Cursor);
            Assert.Equal(Color.Red, control.ForeColor);
            Assert.Same(font1, control.Font);
            mockSite1.Verify(s => s.GetService(typeof(AmbientProperties)), Times.Exactly(2));
            mockSite2.Verify(s => s.GetService(typeof(AmbientProperties)), Times.Exactly(1));
            mockSite3.Verify(s => s.GetService(typeof(AmbientProperties)), Times.Never());
            mockSite4.Verify(s => s.GetService(typeof(AmbientProperties)), Times.Never());
            Assert.Equal(1, backColorChangedCallCount);
            Assert.Equal(1, cursorChangedCallCount);
            Assert.Equal(1, foreColorChangedCallCount);
            Assert.Equal(0, fontChangedCallCount);

            // Set different.
            control.Site = mockSite3.Object;
            Assert.Same(mockSite3.Object, control.Site);
            Assert.Equal(Color.Red, control.BackColor);
            Assert.Same(cursor2, control.Cursor);
            Assert.Equal(Color.Blue, control.ForeColor);
            Assert.Same(font2, control.Font);
            mockSite1.Verify(s => s.GetService(typeof(AmbientProperties)), Times.Exactly(2));
            mockSite2.Verify(s => s.GetService(typeof(AmbientProperties)), Times.Exactly(1));
            mockSite3.Verify(s => s.GetService(typeof(AmbientProperties)), Times.Exactly(1));
            mockSite4.Verify(s => s.GetService(typeof(AmbientProperties)), Times.Never());
            Assert.Equal(2, backColorChangedCallCount);
            Assert.Equal(1, cursorChangedCallCount);
            Assert.Equal(2, foreColorChangedCallCount);
            Assert.Equal(1, fontChangedCallCount);

            // Set null.
            control.Site = mockSite4.Object;
            Assert.Same(mockSite4.Object, control.Site);
            Assert.Equal(Control.DefaultBackColor, control.BackColor);
            Assert.Same(Cursors.Default, control.Cursor);
            Assert.Equal(Control.DefaultForeColor, control.ForeColor);
            Assert.Same(Control.DefaultFont, control.Font);
            mockSite1.Verify(s => s.GetService(typeof(AmbientProperties)), Times.Exactly(2));
            mockSite2.Verify(s => s.GetService(typeof(AmbientProperties)), Times.Exactly(1));
            mockSite3.Verify(s => s.GetService(typeof(AmbientProperties)), Times.Exactly(1));
            mockSite4.Verify(s => s.GetService(typeof(AmbientProperties)), Times.Exactly(1));
            Assert.Equal(3, backColorChangedCallCount);
            Assert.Equal(2, cursorChangedCallCount);
            Assert.Equal(3, foreColorChangedCallCount);
            Assert.Equal(2, fontChangedCallCount);
        }

        [Fact]
        public void Control_Site_SetWithAmbientPropertiesSet_DoesNotUpdate()
        {
            Font font1 = SystemFonts.MenuFont;
            Font font2 = SystemFonts.DialogFont;
            Cursor cursor1 = Cursors.AppStarting;
            Cursor cursor2 = Cursors.Arrow;
            var properties = new AmbientProperties
            {
                BackColor = Color.Blue,
                Cursor = cursor1,
                Font = font1,
                ForeColor = Color.Red
            };
            var mockSite1 = new Mock<ISite>(MockBehavior.Strict);
            mockSite1
                .Setup(s => s.GetService(typeof(AmbientProperties)))
                .Returns(properties)
                .Verifiable();

            var sameProperties = new AmbientProperties
            {
                BackColor = Color.Blue,
                Cursor = cursor1,
                Font = font1,
                ForeColor = Color.Red
            };
            var mockSite2 = new Mock<ISite>(MockBehavior.Strict);
            mockSite2
                .Setup(s => s.GetService(typeof(AmbientProperties)))
                .Returns(sameProperties)
                .Verifiable();

            var differentProperties = new AmbientProperties
            {
                BackColor = Color.Red,
                Cursor = cursor2,
                Font = font2,
                ForeColor = Color.Blue
            };
            var mockSite3 = new Mock<ISite>(MockBehavior.Strict);
            mockSite3
                .Setup(s => s.GetService(typeof(AmbientProperties)))
                .Returns(differentProperties)
                .Verifiable();

            var mockSite4 = new Mock<ISite>(MockBehavior.Strict);
            mockSite4
                .Setup(s => s.GetService(typeof(AmbientProperties)))
                .Returns(null)
                .Verifiable();

            var controlCursor = new Cursor((IntPtr)3);
            Font controlFont = SystemFonts.StatusFont;
            var control = new Control
            {
                BackColor = Color.Green,
                Cursor = controlCursor,
                ForeColor = Color.Yellow,
                Font = controlFont
            };
            int backColorChangedCallCount = 0;
            control.BackColorChanged += (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(EventArgs.Empty, e);
                backColorChangedCallCount++;
            };
            int cursorChangedCallCount = 0;
            control.CursorChanged += (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(EventArgs.Empty, e);
                cursorChangedCallCount++;
            };
            int foreColorChangedCallCount = 0;
            control.ForeColorChanged += (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(EventArgs.Empty, e);
                foreColorChangedCallCount++;
            };
            int fontChangedCallCount = 0;
            control.FontChanged += (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(EventArgs.Empty, e);
                fontChangedCallCount++;
            };

            control.Site = mockSite1.Object;
            Assert.Same(mockSite1.Object, control.Site);
            Assert.Equal(Color.Green, control.BackColor);
            Assert.Same(controlCursor, control.Cursor);
            Assert.Equal(Color.Yellow, control.ForeColor);
            Assert.Same(controlFont, control.Font);
            mockSite1.Verify(s => s.GetService(typeof(AmbientProperties)), Times.Once());
            mockSite2.Verify(s => s.GetService(typeof(AmbientProperties)), Times.Never());
            mockSite3.Verify(s => s.GetService(typeof(AmbientProperties)), Times.Never());
            mockSite4.Verify(s => s.GetService(typeof(AmbientProperties)), Times.Never());
            Assert.Equal(0, backColorChangedCallCount);
            Assert.Equal(0, cursorChangedCallCount);
            Assert.Equal(0, foreColorChangedCallCount);
            Assert.Equal(0, fontChangedCallCount);

            // Set same.
            control.Site = mockSite1.Object;
            Assert.Same(mockSite1.Object, control.Site);
            Assert.Equal(Color.Green, control.BackColor);
            Assert.Same(controlCursor, control.Cursor);
            Assert.Equal(Color.Yellow, control.ForeColor);
            Assert.Same(controlFont, control.Font);
            mockSite1.Verify(s => s.GetService(typeof(AmbientProperties)), Times.Exactly(2));
            mockSite2.Verify(s => s.GetService(typeof(AmbientProperties)), Times.Never());
            mockSite3.Verify(s => s.GetService(typeof(AmbientProperties)), Times.Never());
            mockSite4.Verify(s => s.GetService(typeof(AmbientProperties)), Times.Never());
            Assert.Equal(0, backColorChangedCallCount);
            Assert.Equal(0, cursorChangedCallCount);
            Assert.Equal(0, foreColorChangedCallCount);
            Assert.Equal(0, fontChangedCallCount);

            // Set equal.
            control.Site = mockSite2.Object;
            Assert.Same(mockSite2.Object, control.Site);
            Assert.Equal(Color.Green, control.BackColor);
            Assert.Same(controlCursor, control.Cursor);
            Assert.Equal(Color.Yellow, control.ForeColor);
            Assert.Same(controlFont, control.Font);
            mockSite1.Verify(s => s.GetService(typeof(AmbientProperties)), Times.Exactly(2));
            mockSite2.Verify(s => s.GetService(typeof(AmbientProperties)), Times.Exactly(1));
            mockSite3.Verify(s => s.GetService(typeof(AmbientProperties)), Times.Never());
            mockSite4.Verify(s => s.GetService(typeof(AmbientProperties)), Times.Never());
            Assert.Equal(0, backColorChangedCallCount);
            Assert.Equal(0, cursorChangedCallCount);
            Assert.Equal(0, foreColorChangedCallCount);
            Assert.Equal(0, fontChangedCallCount);

            // Set different.
            control.Site = mockSite3.Object;
            Assert.Same(mockSite3.Object, control.Site);
            Assert.Equal(Color.Green, control.BackColor);
            Assert.Same(controlCursor, control.Cursor);
            Assert.Equal(Color.Yellow, control.ForeColor);
            Assert.Same(controlFont, control.Font);
            mockSite1.Verify(s => s.GetService(typeof(AmbientProperties)), Times.Exactly(2));
            mockSite2.Verify(s => s.GetService(typeof(AmbientProperties)), Times.Exactly(1));
            mockSite3.Verify(s => s.GetService(typeof(AmbientProperties)), Times.Exactly(1));
            mockSite4.Verify(s => s.GetService(typeof(AmbientProperties)), Times.Never());
            Assert.Equal(0, backColorChangedCallCount);
            Assert.Equal(0, cursorChangedCallCount);
            Assert.Equal(0, foreColorChangedCallCount);
            Assert.Equal(0, fontChangedCallCount);
        }

        /// <summary>
        ///  Data for the UseWaitCursorGetSet test
        /// </summary>
        public static TheoryData<bool> UseWaitCursorGetSetData =>
            CommonTestHelper.GetBoolTheoryData();

        [Theory]
        [MemberData(nameof(UseWaitCursorGetSetData))]
        public void Control_UseWaitCursorGetSet(bool expected)
        {
            var cont = new Control
            {
                UseWaitCursor = expected
            };

            Assert.Equal(expected, cont.UseWaitCursor);
        }

        [Theory]
        [InlineData(true, true)]
        [InlineData(false, true)] // setting is impossible; default is false
        // SupportsUseCompatibleTextRendering is always false
        public void Control_UseCompatibleTextRenderingIntGetSet(bool given, bool expected)
        {
            var cont = new Control
            {
                UseCompatibleTextRenderingInt = given
            };

            Assert.Equal(expected, cont.UseCompatibleTextRenderingInt);
        }

        [Fact]
        public void Control_WindowTargetGetSet()
        {
            var cont = new Control();
            var mock = new Mock<IWindowTarget>(MockBehavior.Strict);

            cont.WindowTarget = mock.Object;

            Assert.Equal(mock.Object, cont.WindowTarget);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetBoolTheoryData))]
        public void Control_AllowDrop_Set_GetReturnsExpected(bool value)
        {
            var control = new Control
            {
                AllowDrop = value
            };
            Assert.Equal(value, control.AllowDrop);

            // Set same.
            control.AllowDrop = value;
            Assert.Equal(value, control.AllowDrop);

            // Set different.
            control.AllowDrop = value;
            Assert.Equal(value, control.AllowDrop);
        }

        [Fact]
        public void Control_AllowDrop_SetWithHandleNonSTAThread_ThrowsInvalidOperationException()
        {
            var control = new Control();
            Assert.NotEqual(IntPtr.Zero, control.Handle);

            Assert.Throws<InvalidOperationException>(() => control.AllowDrop = true);
            Assert.False(control.AllowDrop);

            // Can set to false.
            control.AllowDrop = false;
            Assert.False(control.AllowDrop);
        }

        [StaTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetBoolTheoryData))]
        public void Control_AllowDrop_SetWithHandleSTA_GetReturnsExpected(bool value)
        {
            var control = new Control();
            Assert.NotEqual(IntPtr.Zero, control.Handle);

            control.AllowDrop = value;
            Assert.Equal(value, control.AllowDrop);

            // Set same.
            control.AllowDrop = value;
            Assert.Equal(value, control.AllowDrop);

            // Set different.
            control.AllowDrop = value;
            Assert.Equal(value, control.AllowDrop);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetPointTheoryData))]
        public void Control_AutoScrollOffsetGetSet(Point value)
        {
            var control = new Control
            {
                AutoScrollOffset = value
            };
            Assert.Equal(value, control.AutoScrollOffset);

            // Set same.
            control.AutoScrollOffset = value;
            Assert.Equal(value, control.AutoScrollOffset);
        }

        public static IEnumerable<object[]> BindingContext_Set_TestData()
        {
            yield return new object[] { null };
            yield return new object[] { new BindingContext() };
        }

        [Theory]
        [MemberData(nameof(BindingContext_Set_TestData))]
        public void Control_BindingContext_Set_GetReturnsExpected(BindingContext value)
        {
            var control = new Control
            {
                BindingContext = value
            };
            Assert.Same(value, control.BindingContext);

            // Set same.
            control.BindingContext = value;
            Assert.Same(value, control.BindingContext);
        }

        [Theory]
        [MemberData(nameof(BindingContext_Set_TestData))]
        public void Control_BindingContext_SetWithNonNullBindingContext_GetReturnsExpected(BindingContext value)
        {
            var control = new Control
            {
                BindingContext = new BindingContext()
            };

            control.BindingContext = value;
            Assert.Same(value, control.BindingContext);

            // Set same.
            control.BindingContext = value;
            Assert.Same(value, control.BindingContext);
        }

        [Fact]
        public void Control_BindingContext_SetWithHandler_CallsBindingContextChanged()
        {
            var control = new Control();
            int callCount = 0;
            EventHandler handler = (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(EventArgs.Empty, e);
                callCount++;
            };
            control.BindingContextChanged += handler;

            // Set different.
            var context1 = new BindingContext();
            control.BindingContext = context1;
            Assert.Same(context1, control.BindingContext);
            Assert.Equal(1, callCount);

            // Set same.
            control.BindingContext = context1;
            Assert.Same(context1, control.BindingContext);
            Assert.Equal(1, callCount);

            // Set different.
            var context2 = new BindingContext();
            control.BindingContext = context2;
            Assert.Same(context2, control.BindingContext);
            Assert.Equal(2, callCount);

            // Set null.
            control.BindingContext = null;
            Assert.Null(control.BindingContext);
            Assert.Equal(3, callCount);

            // Remove handler.
            control.BindingContextChanged -= handler;
            control.BindingContext = context1;
            Assert.Same(context1, control.BindingContext);
            Assert.Equal(3, callCount);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetBoolTheoryData))]
        public void Control_CausesValidation_Set_GetReturnsExpected(bool value)
        {
            var control = new Control
            {
                CausesValidation = value
            };
            Assert.Equal(value, control.CausesValidation);
            
            // Set same
            control.CausesValidation = value;
            Assert.Equal(value, control.CausesValidation);
            
            // Set different
            control.CausesValidation = !value;
            Assert.Equal(!value, control.CausesValidation);
        }

        [Fact]
        public void Control_CausesValidation_SetWithHandler_CallsCausesValidationChanged()
        {
            var control = new Control
            {
                CausesValidation = true
            };
            int callCount = 0;
            EventHandler handler = (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(EventArgs.Empty, e);
                callCount++;
            };
            control.CausesValidationChanged += handler;

            // Set different.
            control.CausesValidation = false;
            Assert.False(control.CausesValidation);
            Assert.Equal(1, callCount);

            // Set same.
            control.CausesValidation = false;
            Assert.False(control.CausesValidation);
            Assert.Equal(1, callCount);

            // Set different.
            control.CausesValidation = true;
            Assert.True(control.CausesValidation);
            Assert.Equal(2, callCount);

            // Remove handler.
            control.CausesValidationChanged -= handler;
            control.CausesValidation = false;
            Assert.False(control.CausesValidation);
            Assert.Equal(2, callCount);
        }

        [Theory]
        [InlineData(false, false)]
        [InlineData(true, false)] // giving true cannot set to true
        public void Control_CacheTextInternalGetSet(bool given, bool expected)
        {
            var cont = new Control
            {
                CacheTextInternal = given
            };

            Assert.Equal(expected, cont.CacheTextInternal);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetSizeTheoryData))]
        public void Control_ClientSize_Set_GetReturnsExpected(Size value)
        {
            var control = new Control
            {
                ClientSize = value
            };
            Assert.Equal(value, control.ClientSize);

            // Set same.
            control.ClientSize = value;
            Assert.Equal(value, control.ClientSize);
        }

        [Fact]
        public void Control_ContextMenuGetSet()
        {
            var cont = new Control();
            var expected = new ContextMenu();

            cont.ContextMenu = expected;

            Assert.Equal(expected, cont.ContextMenu);
        }

        [Fact]
        public void Control_ContextMenuStripGetSet()
        {
            var cont = new Control();
            var expected = new ContextMenuStrip();

            cont.ContextMenuStrip = expected;

            Assert.Equal(expected, cont.ContextMenuStrip);
        }

        /// <summary>
        ///  Data for the ValidationCancelledGetSet test
        /// </summary>
        public static TheoryData<bool> ValidationCancelledGetSetData =>
            CommonTestHelper.GetBoolTheoryData();

        [Theory]
        [MemberData(nameof(ValidationCancelledGetSetData))]
        public void Control_ValidationCancelledGetSet(bool expected)
        {
            var cont = new Control
            {
                ValidationCancelled = expected
            };

            Assert.Equal(expected, cont.ValidationCancelled);
        }

        /// <summary>
        ///  Data for the IsTopMdiWindowClosingGetSet test
        /// </summary>
        public static TheoryData<bool> IsTopMdiWindowClosingGetSetData =>
            CommonTestHelper.GetBoolTheoryData();

        [Theory]
        [MemberData(nameof(IsTopMdiWindowClosingGetSetData))]
        public void Control_IsTopMdiWindowClosingGetSet(bool expected)
        {
            var cont = new Control
            {
                IsTopMdiWindowClosing = expected
            };

            Assert.Equal(expected, cont.IsTopMdiWindowClosing);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetCursorTheoryData))]
        public void Control_Cursor_Set_GetReturnsExpected(Cursor value)
        {
            var control = new Control
            {
                Cursor = value
            };
            Assert.Same(value ?? Cursors.Default, control.Cursor);

            // Set same.
            control.Cursor = value;
            Assert.Same(value ?? Cursors.Default, control.Cursor);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetEnumTypeTheoryData), typeof(DockStyle))]
        public void Control_Dock_Set_GetReturnsExpected(DockStyle value)
        {
            var control = new Control
            {
                Dock = value
            };
            Assert.Equal(value, control.Dock);
            Assert.Equal(AnchorStyles.Top | AnchorStyles.Left, control.Anchor);

            // Set same.
            control.Dock = value;
            Assert.Equal(value, control.Dock);
            Assert.Equal(AnchorStyles.Top | AnchorStyles.Left, control.Anchor);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetEnumTypeTheoryData), typeof(DockStyle))]
        public void Control_Dock_SetWithOldValue_GetReturnsExpected(DockStyle value)
        {
            var control = new Control
            {
                Dock = DockStyle.Top
            };
             
            control.Dock = value;
            Assert.Equal(value, control.Dock);
            Assert.Equal(AnchorStyles.Top | AnchorStyles.Left, control.Anchor);

            // Set same.
            control.Dock = value;
            Assert.Equal(value, control.Dock);
            Assert.Equal(AnchorStyles.Top | AnchorStyles.Left, control.Anchor);
        }

        public static IEnumerable<object[]> Dock_SetWithAnchor_TestData()
        {
            foreach (AnchorStyles anchor in Enum.GetValues(typeof(AnchorStyles)))
            {
                foreach (DockStyle value in Enum.GetValues(typeof(DockStyle)))
                {
                    yield return new object[] { anchor, value, value == DockStyle.None ? anchor : AnchorStyles.Top | AnchorStyles.Left };
                }
            }
        }

        [Theory]
        [MemberData(nameof(Dock_SetWithAnchor_TestData))]
        public void Control_Dock_SetWithAnchor_GetReturnsExpected(AnchorStyles anchor, DockStyle value, AnchorStyles expectedAnchor)
        {
            var control = new Control
            {
                Anchor = anchor
            };
             
            control.Dock = value;
            Assert.Equal(value, control.Dock);
            Assert.Equal(expectedAnchor, control.Anchor);

            // Set same.
            control.Dock = value;
            Assert.Equal(value, control.Dock);
            Assert.Equal(expectedAnchor, control.Anchor);
        }

        [Fact]
        public void Control_Dock_SetWithHandler_CallsDockChanged()
        {
            var control = new Control();
            int callCount = 0;
            EventHandler handler = (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(EventArgs.Empty, e);
                callCount++;
            };
            control.DockChanged += handler;

            // Set different.
            control.Dock = DockStyle.Top;
            Assert.Equal(DockStyle.Top, control.Dock);
            Assert.Equal(1, callCount);

            // Set same.
            control.Dock = DockStyle.Top;
            Assert.Equal(DockStyle.Top, control.Dock);
            Assert.Equal(1, callCount);

            // Set different.
            control.Dock = DockStyle.Left;
            Assert.Equal(DockStyle.Left, control.Dock);
            Assert.Equal(2, callCount);

            // Remove handler.
            control.DockChanged -= handler;
            control.Dock = DockStyle.Top;
            Assert.Equal(DockStyle.Top, control.Dock);
            Assert.Equal(2, callCount);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetEnumTypeTheoryDataInvalid), typeof(DockStyle))]
        public void Control_Dock_SetInvalid_ThrowsInvalidEnumArgumentException(DockStyle value)
        {
            var control = new Control();
            Assert.Throws<InvalidEnumArgumentException>("value", () => control.Dock = value);
        }

        #endregion

        [Theory]
        [InlineData(ControlStyles.ContainerControl, false)]
        [InlineData(ControlStyles.UserPaint, true)]
        [InlineData(ControlStyles.Opaque, false)]
        [InlineData(ControlStyles.ResizeRedraw, false)]
        [InlineData(ControlStyles.FixedWidth, false)]
        [InlineData(ControlStyles.FixedHeight, false)]
        [InlineData(ControlStyles.StandardClick, true)]
        [InlineData(ControlStyles.Selectable, true)]
        [InlineData(ControlStyles.UserMouse, false)]
        [InlineData(ControlStyles.SupportsTransparentBackColor, false)]
        [InlineData(ControlStyles.StandardDoubleClick, true)]
        [InlineData(ControlStyles.AllPaintingInWmPaint, true)]
        [InlineData(ControlStyles.CacheText, false)]
        [InlineData(ControlStyles.EnableNotifyMessage, false)]
        [InlineData(ControlStyles.DoubleBuffer, false)]
        [InlineData(ControlStyles.OptimizedDoubleBuffer, false)]
        [InlineData(ControlStyles.UseTextForAccessibility, true)]
        [InlineData((ControlStyles)int.MaxValue, false)]
        [InlineData((ControlStyles)(-1), false)]
        public void Control_GetStyle_Invoke_ReturnsExpected(ControlStyles flag, bool expected)
        {
            var control = new SubControl();
            Assert.Equal(expected, control.GetStyle(flag));
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetEventArgsTheoryData))]
        public void Control_OnCausesValidationChanged_Invoke_CallsCausesValidationChanged(EventArgs eventArgs)
        {
            var control = new SubControl();
            int callCount = 0;
            EventHandler handler = (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(eventArgs, e);
                callCount++;
            };

            // Call with handler.
            control.CausesValidationChanged += handler;
            control.OnCausesValidationChanged(eventArgs);
            Assert.Equal(1, callCount);

            // Remove handler.
            control.CausesValidationChanged -= handler;
            control.OnCausesValidationChanged(eventArgs);
            Assert.Equal(1, callCount);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetEventArgsTheoryData))]
        public void Control_OnClick_Invoke_CallsClick(EventArgs eventArgs)
        {
            var control = new SubControl();
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
        public void Control_OnDoubleClick_Invoke_CallsDoubleClick(EventArgs eventArgs)
        {
            var control = new SubControl();
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
        [CommonMemberData(nameof(CommonTestHelper.GetKeyEventArgsTheoryData))]
        public void Control_OnKeyDown_Invoke_CallsKeyDown(KeyEventArgs eventArgs)
        {
            var control = new SubControl();
            int callCount = 0;
            KeyEventHandler handler = (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(eventArgs, e);
                callCount++;
            };

            // Call with handler.
            control.KeyDown += handler;
            control.OnKeyDown(eventArgs);
            Assert.Equal(1, callCount);

            // Remove handler.
            control.KeyDown -= handler;
            control.OnKeyDown(eventArgs);
            Assert.Equal(1, callCount);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetKeyPressEventArgsTheoryData))]
        public void Control_OnKeyPress_Invoke_CallsKeyPress(KeyPressEventArgs eventArgs)
        {
            var control = new SubControl();
            int callCount = 0;
            KeyPressEventHandler handler = (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(eventArgs, e);
                callCount++;
            };

            // Call with handler.
            control.KeyPress += handler;
            control.OnKeyPress(eventArgs);
            Assert.Equal(1, callCount);

            // Remove handler.
            control.KeyPress -= handler;
            control.OnKeyPress(eventArgs);
            Assert.Equal(1, callCount);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetKeyEventArgsTheoryData))]
        public void Control_OnKeyUp_Invoke_CallsKeyUp(KeyEventArgs eventArgs)
        {
            var control = new SubControl();
            int callCount = 0;
            KeyEventHandler handler = (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(eventArgs, e);
                callCount++;
            };

            // Call with handler.
            control.KeyUp += handler;
            control.OnKeyUp(eventArgs);
            Assert.Equal(1, callCount);

            // Remove handler.
            control.KeyUp -= handler;
            control.OnKeyUp(eventArgs);
            Assert.Equal(1, callCount);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetEventArgsTheoryData))]
        public void Control_OnEnabledChanged_Invoke_CallsEnabledChanged(EventArgs eventArgs)
        {
            var control = new SubControl();
            int callCount = 0;
            EventHandler handler = (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(eventArgs, e);
                callCount++;
            };

            // Call with handler.
            control.EnabledChanged += handler;
            control.OnEnabledChanged(eventArgs);
            Assert.Equal(1, callCount);

            // Remove handler.
            control.EnabledChanged -= handler;
            control.OnEnabledChanged(eventArgs);
            Assert.Equal(1, callCount);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetEventArgsTheoryData))]
        public void Control_OnEnter_Invoke_CallsEnter(EventArgs eventArgs)
        {
            var control = new SubControl();
            int callCount = 0;
            EventHandler handler = (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(eventArgs, e);
                callCount++;
            };

            // Call with handler.
            control.Enter += handler;
            control.OnEnter(eventArgs);
            Assert.Equal(1, callCount);

            // Remove handler.
            control.Enter -= handler;
            control.OnEnter(eventArgs);
            Assert.Equal(1, callCount);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetEventArgsTheoryData))]
        public void Control_OnHandleCreated_Invoke_CallsHandleCreated(EventArgs eventArgs)
        {
            var control = new SubControl();
            int callCount = 0;
            EventHandler handler = (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(eventArgs, e);
                callCount++;
            };

            // Call with handler.
            control.HandleCreated += handler;
            control.OnHandleCreated(eventArgs);
            Assert.Equal(1, callCount);

            // Remove handler.
            control.HandleCreated -= handler;
            control.OnHandleCreated(eventArgs);
            Assert.Equal(1, callCount);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetEventArgsTheoryData))]
        public void Control_OnHandleDestroyed_Invoke_CallsHandleDestroyed(EventArgs eventArgs)
        {
            var control = new SubControl();
            int callCount = 0;
            EventHandler handler = (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(eventArgs, e);
                callCount++;
            };

            // Call with handler.
            control.HandleDestroyed += handler;
            control.OnHandleDestroyed(eventArgs);
            Assert.Equal(1, callCount);

            // Remove handler.
            control.HandleDestroyed -= handler;
            control.OnHandleDestroyed(eventArgs);
            Assert.Equal(1, callCount);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetLayoutEventArgsTheoryData))]
        public void Control_OnLayout_Invoke_CallsLayout(LayoutEventArgs eventArgs)
        {
            var control = new SubControl();
            int callCount = 0;
            LayoutEventHandler handler = (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(eventArgs, e);
                callCount++;
            };

            // Call with handler.
            control.Layout += handler;
            control.OnLayout(eventArgs);
            Assert.Equal(1, callCount);

            // Remove handler.
            control.Layout -= handler;
            control.OnLayout(eventArgs);
            Assert.Equal(1, callCount);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetEventArgsTheoryData))]
        public void Control_OnLocationChanged_Invoke_CallsLocationChangedAndMove(EventArgs eventArgs)
        {
            var control = new SubControl();
            int locationChangedCallCount = 0;
            EventHandler locationChangedHandler = (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(eventArgs, e);
                locationChangedCallCount++;
            };
            int moveCallCount = 0;
            EventHandler moveHandler = (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(eventArgs, e);
                moveCallCount++;
            };

            // Call with handler.
            control.LocationChanged += locationChangedHandler;
            control.Move += moveHandler;
            control.OnLocationChanged(eventArgs);
            Assert.Equal(1, locationChangedCallCount);
            Assert.Equal(1, moveCallCount);

            // Remove handler.
            control.LocationChanged -= locationChangedHandler;
            control.Move -= moveHandler;
            control.OnLocationChanged(eventArgs);
            Assert.Equal(1, locationChangedCallCount);
            Assert.Equal(1, moveCallCount);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetEventArgsTheoryData))]
        public void Control_OnLocationChanged_InvokeWithHandle_CallsLocationChangedAndMove(EventArgs eventArgs)
        {
            var control = new SubControl();
            Assert.NotEqual(IntPtr.Zero, control.Handle);

            int locationChangedCallCount = 0;
            EventHandler locationChangedHandler = (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(eventArgs, e);
                locationChangedCallCount++;
            };
            int moveCallCount = 0;
            EventHandler moveHandler = (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(eventArgs, e);
                moveCallCount++;
            };
            int invalidatedCallCount = 0;
            InvalidateEventHandler invalidatedHandler = (sender, e) => invalidatedCallCount++;

            // Call with handler.
            control.LocationChanged += locationChangedHandler;
            control.Move += moveHandler;
            control.Invalidated += invalidatedHandler;
            control.OnLocationChanged(eventArgs);
            Assert.Equal(1, locationChangedCallCount);
            Assert.Equal(1, moveCallCount);
            Assert.Equal(0, invalidatedCallCount);

            // Remove handler.
            control.LocationChanged -= locationChangedHandler;
            control.Move -= moveHandler;
            control.Invalidated -= invalidatedHandler;
            control.OnLocationChanged(eventArgs);
            Assert.Equal(1, locationChangedCallCount);
            Assert.Equal(1, moveCallCount);
            Assert.Equal(0, invalidatedCallCount);
        }

        public static IEnumerable<object[]> OnLocationChanged_HandleWithTransparentBackColor_TestData()
        {
            foreach (object[] testData in CommonTestHelper.GetEventArgsTheoryData())
            {
                yield return new object[] { true, testData[0], 1 };
                yield return new object[] { false, testData[0], 0 };
            }
        }

        [Theory]
        [MemberData(nameof(OnLocationChanged_HandleWithTransparentBackColor_TestData))]
        public void Control_OnLocationChanged_InvokeWithHandleWithTransparentBackColor_CallsLocationChangedAndMoveAndInvalidated(bool supportsTransparentBackgroundColor, EventArgs eventArgs, int expectedInvalidatedCallCount)
        {
            var control = new SubControl();
            control.SetStyle(ControlStyles.SupportsTransparentBackColor, true);
            control.BackColor = Color.FromArgb(254, 255, 255, 255);
            control.SetStyle(ControlStyles.SupportsTransparentBackColor, supportsTransparentBackgroundColor);
            Assert.NotEqual(IntPtr.Zero, control.Handle);

            int locationChangedCallCount = 0;
            EventHandler locationChangedHandler = (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(eventArgs, e);
                locationChangedCallCount++;
            };
            int moveCallCount = 0;
            EventHandler moveHandler = (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(eventArgs, e);
                moveCallCount++;
            };
            int invalidatedCallCount = 0;
            InvalidateEventHandler invalidatedHandler = (sender, e) => invalidatedCallCount++;

            // Call with handler.
            control.LocationChanged += locationChangedHandler;
            control.Move += moveHandler;
            control.Invalidated += invalidatedHandler;
            control.OnLocationChanged(eventArgs);
            Assert.Equal(1, locationChangedCallCount);
            Assert.Equal(1, moveCallCount);
            Assert.Equal(expectedInvalidatedCallCount, invalidatedCallCount);

            // Remove handler.
            control.LocationChanged -= locationChangedHandler;
            control.Move -= moveHandler;
            control.Invalidated -= invalidatedHandler;
            control.OnLocationChanged(eventArgs);
            Assert.Equal(1, locationChangedCallCount);
            Assert.Equal(1, moveCallCount);
            Assert.Equal(expectedInvalidatedCallCount, invalidatedCallCount);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetMouseEventArgsTheoryData))]
        public void Control_OnMouseClick_Invoke_CallsMouseClick(MouseEventArgs eventArgs)
        {
            var control = new SubControl();
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
        public void Control_OnMouseDoubleClick_Invoke_CallsMouseDoubleClick(MouseEventArgs eventArgs)
        {
            var control = new SubControl();
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
        public void Control_OnMouseDown_Invoke_CallsMouseDown(MouseEventArgs eventArgs)
        {
            var control = new SubControl();
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
        public void Control_OnMouseMove_Invoke_CallsMouseMove(MouseEventArgs eventArgs)
        {
            var control = new SubControl();
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
        public void Control_OnMouseUp_Invoke_CallsMouseUp(MouseEventArgs eventArgs)
        {
            var control = new SubControl();
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
        public void Control_OnMouseWheel_Invoke_CallsMouseWheel(MouseEventArgs eventArgs)
        {
            var control = new SubControl();
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
        public void Control_OnMouseWheel_InvokeHandledMouseEventArgs_DoesNotSetHandled()
        {
            var control = new SubControl();
            var eventArgs = new HandledMouseEventArgs(MouseButtons.Left, 1, 2, 3, 4);
            int callCount = 0;
            MouseEventHandler handler = (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(eventArgs, e);
                Assert.False(eventArgs.Handled);
                callCount++;
            };
            control.MouseWheel += handler;

            control.OnMouseWheel(eventArgs);
            Assert.Equal(1, callCount);
            Assert.False(eventArgs.Handled);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetEventArgsTheoryData))]
        public void Control_OnMove_Invoke_CallsMove(EventArgs eventArgs)
        {
            var control = new SubControl();
            int moveCallCount = 0;
            EventHandler moveHandler = (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(eventArgs, e);
                moveCallCount++;
            };

            // Call with handler.
            control.Move += moveHandler;
            control.OnMove(eventArgs);
            Assert.Equal(1, moveCallCount);

            // Remove handler.
            control.Move -= moveHandler;
            control.OnMove(eventArgs);
            Assert.Equal(1, moveCallCount);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetEventArgsTheoryData))]
        public void Control_OnMove_InvokeWithHandle_CallsMove(EventArgs eventArgs)
        {
            var control = new SubControl();
            Assert.NotEqual(IntPtr.Zero, control.Handle);

            int moveCallCount = 0;
            EventHandler moveHandler = (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(eventArgs, e);
                moveCallCount++;
            };
            int invalidatedCallCount = 0;
            InvalidateEventHandler invalidatedHandler = (sender, e) => invalidatedCallCount++;

            // Call with handler.
            control.Move += moveHandler;
            control.Invalidated += invalidatedHandler;
            control.OnMove(eventArgs);
            Assert.Equal(1, moveCallCount);
            Assert.Equal(0, invalidatedCallCount);

            // Remove handler.
            control.Move -= moveHandler;
            control.Invalidated -= invalidatedHandler;
            control.OnMove(eventArgs);
            Assert.Equal(1, moveCallCount);
            Assert.Equal(0, invalidatedCallCount);
        }

        [Theory]
        [MemberData(nameof(OnLocationChanged_HandleWithTransparentBackColor_TestData))]
        public void Control_OnMove_InvokeWithHandleWithTransparentBackColor_CallsMoveAndInvalidated(bool supportsTransparentBackgroundColor, EventArgs eventArgs, int expectedInvalidatedCallCount)
        {
            var control = new SubControl();
            control.SetStyle(ControlStyles.SupportsTransparentBackColor, true);
            control.BackColor = Color.FromArgb(254, 255, 255, 255);
            control.SetStyle(ControlStyles.SupportsTransparentBackColor, supportsTransparentBackgroundColor);
            Assert.NotEqual(IntPtr.Zero, control.Handle);

            int moveCallCount = 0;
            EventHandler moveHandler = (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(eventArgs, e);
                moveCallCount++;
            };
            int invalidatedCallCount = 0;
            InvalidateEventHandler invalidatedHandler = (sender, e) => invalidatedCallCount++;

            // Call with handler.
            control.Move += moveHandler;
            control.Invalidated += invalidatedHandler;
            control.OnMove(eventArgs);
            Assert.Equal(1, moveCallCount);
            Assert.Equal(expectedInvalidatedCallCount, invalidatedCallCount);

            // Remove handler.
            control.Move -= moveHandler;
            control.Invalidated -= invalidatedHandler;
            control.OnMove(eventArgs);
            Assert.Equal(1, moveCallCount);
            Assert.Equal(expectedInvalidatedCallCount, invalidatedCallCount);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetPaintEventArgsTheoryData))]
        public void Control_OnPaint_Invoke_CallsPaint(PaintEventArgs eventArgs)
        {
            var control = new SubControl();
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

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetEventArgsTheoryData))]
        public void Control_OnParentChanged_Invoke_CallsParentChanged(EventArgs eventArgs)
        {
            var control = new SubControl();
            int callCount = 0;
            EventHandler handler = (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(eventArgs, e);
                callCount++;
            };

            // Call with handler.
            control.ParentChanged += handler;
            control.OnParentChanged(eventArgs);
            Assert.Equal(1, callCount);

            // Remove handler.
            control.ParentChanged -= handler;
            control.OnParentChanged(eventArgs);
            Assert.Equal(1, callCount);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetEventArgsTheoryData))]
        public void Control_OnResize_Invoke_CallsResize(EventArgs eventArgs)
        {
            var control = new SubControl();
            Assert.NotEqual(IntPtr.Zero, control.Handle);
            int callCount = 0;
            EventHandler handler = (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(eventArgs, e);
                callCount++;
            };
            int invalidatedCallCount = 0;
            control.Invalidated += (sender, e) => invalidatedCallCount++;

            // Call with handler.
            control.Resize += handler;
            control.OnResize(eventArgs);
            Assert.Equal(1, callCount);
            Assert.Equal(0, invalidatedCallCount);

            // Remove handler.
            control.Resize -= handler;
            control.OnResize(eventArgs);
            Assert.Equal(1, callCount);
            Assert.Equal(0, invalidatedCallCount);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetEventArgsTheoryData))]
        public void Control_OnResize_InvokeWithResizeRedraw_CallsResizeAndInvalidate(EventArgs eventArgs)
        {
            var control = new SubControl();
            control.SetStyle(ControlStyles.ResizeRedraw, true);
            Assert.NotEqual(IntPtr.Zero, control.Handle);
            int callCount = 0;
            EventHandler handler = (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(eventArgs, e);
                callCount++;
            };
            int invalidatedCallCount = 0;
            InvalidateEventHandler invalidatedHandler = (sender, e) => invalidatedCallCount++;

            // Call with handler.
            control.Resize += handler;
            control.Invalidated += invalidatedHandler;
            control.OnResize(eventArgs);
            Assert.Equal(1, callCount);
            Assert.Equal(1, invalidatedCallCount);

            // Remove handler.
            control.Resize -= handler;
            control.Invalidated -= invalidatedHandler;
            control.OnResize(eventArgs);
            Assert.Equal(1, callCount);
            Assert.Equal(1, invalidatedCallCount);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetEventArgsTheoryData))]
        public void Control_OnVisibleChanged_Invoke_CallsVisibleChanged(EventArgs eventArgs)
        {
            var control = new SubControl();
            int callCount = 0;
            EventHandler handler = (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(eventArgs, e);
                callCount++;
            };

            // Call with handler.
            control.VisibleChanged += handler;
            control.OnVisibleChanged(eventArgs);
            Assert.Equal(1, callCount);

            // Remove handler.
            control.VisibleChanged -= handler;
            control.OnVisibleChanged(eventArgs);
            Assert.Equal(1, callCount);
        }

        [Theory]
        [InlineData(ControlStyles.UserPaint, true)]
        [InlineData(ControlStyles.UserPaint, false)]
        public void Control_SetStyle_Invoke_GetStyleReturnsExpected(ControlStyles flag, bool value)
        {
            var control = new SubControl();
            control.SetStyle(flag, value);
            Assert.Equal(value, control.GetStyle(flag));

            // Set same.
            control.SetStyle(flag, value);
            Assert.Equal(value, control.GetStyle(flag));
        }

        [Fact]
        public void Control_ToString_Invoke_ReturnsExpected()
        {
            var control = new Control();
            Assert.Equal("System.Windows.Forms.Control", control.ToString());
        }

        private class SubControl : Control
        {
            public SubControl() : base()
            {
            }

            public SubControl(string text) : base(text)
            {
            }

            public SubControl(string text, int left, int top, int width, int height) : base(text, left, top, width, height)
            {
            }

            public SubControl(Control parent, string text) : base(parent, text)
            {
            }

            public SubControl(Control parent, string text, int left, int top, int width, int height) : base(parent, text, left, top, width, height)
            {
            }

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

            public new ImeMode ImeModeBase
            {
                get => base.ImeModeBase;
                set => base.ImeModeBase = value;
            }

            public new bool GetStyle(ControlStyles flag) => base.GetStyle(flag);

            public new void OnCausesValidationChanged(EventArgs e) => base.OnCausesValidationChanged(e);

            public new void OnClick(EventArgs e) => base.OnClick(e);

            public new void OnDoubleClick(EventArgs e) => base.OnDoubleClick(e);

            public new void OnEnabledChanged(EventArgs e) => base.OnEnabledChanged(e);

            public new void OnEnter(EventArgs e) => base.OnEnter(e);

            public new void OnHandleCreated(EventArgs e) => base.OnHandleCreated(e);

            public new void OnHandleDestroyed(EventArgs e) => base.OnHandleDestroyed(e);

            public new void OnKeyDown(KeyEventArgs e) => base.OnKeyDown(e);

            public new void OnKeyPress(KeyPressEventArgs e) => base.OnKeyPress(e);

            public new void OnKeyUp(KeyEventArgs e) => base.OnKeyUp(e);

            public new void OnLayout(LayoutEventArgs e) => base.OnLayout(e);

            public new void OnLeave(EventArgs e) => base.OnLeave(e);

            public new void OnLocationChanged(EventArgs e) => base.OnLocationChanged(e);

            public new void OnMouseClick(MouseEventArgs e) => base.OnMouseClick(e);

            public new void OnMouseDoubleClick(MouseEventArgs e) => base.OnMouseDoubleClick(e);

            public new void OnMouseDown(MouseEventArgs e) => base.OnMouseDown(e);

            public new void OnMouseMove(MouseEventArgs e) => base.OnMouseMove(e);

            public new void OnMouseUp(MouseEventArgs e) => base.OnMouseUp(e);

            public new void OnMouseWheel(MouseEventArgs e) => base.OnMouseWheel(e);

            public new void OnMove(EventArgs e) => base.OnMove(e);

            public new void OnPaint(PaintEventArgs e) => base.OnPaint(e);

            public new void OnParentChanged(EventArgs e) => base.OnParentChanged(e);

            public new void OnResize(EventArgs e) => base.OnResize(e);

            public new void OnVisibleChanged(EventArgs e) => base.OnVisibleChanged(e);

            public new void SetStyle(ControlStyles flag, bool value) => base.SetStyle(flag, value);
        }
    }
}
