// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel;
using System.Drawing;
using Moq;
using WinForms.Common.Tests;
using Xunit;

namespace System.Windows.Forms.Tests
{
    public class PanelTests
    {
        [Fact]
        public void Panel_Ctor_Default()
        {
            var panel = new SubPanel();
            Assert.False(panel.AllowDrop);
            Assert.Equal(AnchorStyles.Top | AnchorStyles.Left, panel.Anchor);
            Assert.False(panel.AutoScroll);
            Assert.Equal(Size.Empty, panel.AutoScrollMargin);
            Assert.Equal(Size.Empty, panel.AutoScrollMinSize);
            Assert.Equal(Point.Empty, panel.AutoScrollPosition);
            Assert.False(panel.AutoSize);
            Assert.Equal(AutoSizeMode.GrowOnly, panel.AutoSizeMode);
            Assert.Equal(Control.DefaultBackColor, panel.BackColor);
            Assert.Null(panel.BackgroundImage);
            Assert.Equal(ImageLayout.Tile, panel.BackgroundImageLayout);
            Assert.Null(panel.BindingContext);
            Assert.Equal(BorderStyle.None, panel.BorderStyle);
            Assert.Equal(100, panel.Bottom);
            Assert.Equal(new Rectangle(0, 0, 200, 100), panel.Bounds);
            Assert.True(panel.CanEnableIme);
            Assert.True(panel.CanRaiseEvents);
            Assert.Equal(new Rectangle(0, 0, 200, 100), panel.ClientRectangle);
            Assert.Equal(new Size(200, 100), panel.ClientSize);
            Assert.Null(panel.Container);
            Assert.True(panel.CausesValidation);
            Assert.Empty(panel.Controls);
            Assert.Same(panel.Controls, panel.Controls);
            Assert.False(panel.Created);
            Assert.Equal(Cursors.Default, panel.Cursor);
            Assert.Equal(Cursors.Default, panel.DefaultCursor);
            Assert.Equal(ImeMode.Inherit, panel.DefaultImeMode);
            Assert.Equal(new Padding(3), panel.DefaultMargin);
            Assert.Equal(Size.Empty, panel.DefaultMaximumSize);
            Assert.Equal(Size.Empty, panel.DefaultMinimumSize);
            Assert.Equal(Padding.Empty, panel.DefaultPadding);
            Assert.Equal(new Size(200, 100), panel.DefaultSize);
            Assert.False(panel.DesignMode);
            Assert.Equal(new Rectangle(0, 0, 200, 100), panel.DisplayRectangle);
            Assert.Equal(DockStyle.None, panel.Dock);
            Assert.NotNull(panel.DockPadding);
            Assert.Same(panel.DockPadding, panel.DockPadding);
            Assert.Equal(0, panel.DockPadding.Top);
            Assert.Equal(0, panel.DockPadding.Bottom);
            Assert.Equal(0, panel.DockPadding.Left);
            Assert.Equal(0, panel.DockPadding.Right);
            Assert.True(panel.Enabled);
            Assert.NotNull(panel.Events);
            Assert.Same(panel.Events, panel.Events);
            Assert.Equal(Control.DefaultFont, panel.Font);
            Assert.Equal(Control.DefaultForeColor, panel.ForeColor);
            Assert.False(panel.HasChildren);
            Assert.Equal(100, panel.Height);
            Assert.NotNull(panel.HorizontalScroll);
            Assert.Same(panel.HorizontalScroll, panel.HorizontalScroll);
            Assert.False(panel.HScroll);
            Assert.Equal(ImeMode.NoControl, panel.ImeMode);
            Assert.Equal(ImeMode.NoControl, panel.ImeModeBase);
            Assert.Equal(0, panel.Left);
            Assert.Equal(Point.Empty, panel.Location);
            Assert.Equal(Padding.Empty, panel.Padding);
            Assert.Equal(200, panel.Right);
            Assert.Equal(RightToLeft.No, panel.RightToLeft);
            Assert.Equal(new Size(200, 100), panel.Size);
            Assert.Equal(0, panel.TabIndex);
            Assert.False(panel.TabStop);
            Assert.Empty(panel.Text);
            Assert.Equal(0, panel.Top);
            Assert.True(panel.Visible);
            Assert.NotNull(panel.VerticalScroll);
            Assert.Same(panel.VerticalScroll, panel.VerticalScroll);
            Assert.False(panel.VScroll);
            Assert.Equal(200, panel.Width);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetBoolTheoryData))]
        public void Control_AutoSize_Set_GetReturnsExpected(bool value)
        {
            var control = new Panel
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
        public void Panel_AutoSize_SetWithHandler_CallsAutoSizeChanged()
        {
            var control = new Panel
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
        [CommonMemberData(nameof(CommonTestHelper.GetEnumTypeTheoryData), typeof(AutoSizeMode))]
        public void Panel_AutoSizeMode_Set_GetReturnsExpected(AutoSizeMode value)
        {
            var panel = new Panel
            {
                AutoSizeMode = value
            };
            Assert.Equal(value, panel.AutoSizeMode);

            // Set same.
            panel.AutoSizeMode = value;
            Assert.Equal(value, panel.AutoSizeMode);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetEnumTypeTheoryData), typeof(AutoSizeMode))]
        public void Panel_AutoSizeMode_SetWithParent_GetReturnsExpected(AutoSizeMode value)
        {
            var parent = new Control();
            var panel = new Panel
            {
                Parent = parent,
                AutoSizeMode = value
            };
            Assert.Equal(value, panel.AutoSizeMode);

            // Set same.
            panel.AutoSizeMode = value;
            Assert.Equal(value, panel.AutoSizeMode);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetEnumTypeTheoryDataInvalid), typeof(AutoSizeMode))]
        public void Panel_AutoSizeMode_SetInvalid_ThrowsInvalidEnumArgumentException(AutoSizeMode value)
        {
            var panel = new Panel();
            Assert.Throws<InvalidEnumArgumentException>("value", () => panel.AutoSizeMode = value);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetEnumTypeTheoryData), typeof(BorderStyle))]
        public void Panel_BorderStyle_Set_GetReturnsExpected(BorderStyle value)
        {
            var panel = new Panel
            {
                BorderStyle = value
            };
            Assert.Equal(value, panel.BorderStyle);

            // Set same.
            panel.BorderStyle = value;
            Assert.Equal(value, panel.BorderStyle);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetEnumTypeTheoryDataInvalid), typeof(BorderStyle))]
        public void Panel_BorderStyle_SetInvalid_ThrowsInvalidEnumArgumentException(BorderStyle value)
        {
            var panel = new Panel();
            Assert.Throws<InvalidEnumArgumentException>("value", () => panel.BorderStyle = value);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetBoolTheoryData))]
        public void Control_TabStop_Set_GetReturnsExpected(bool value)
        {
            var control = new Panel
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
            var control = new Panel();
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
        public void Panel_TabStop_SetWithHandler_CallsTabStopChanged()
        {
            var control = new Panel
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
        public void Control_Text_Set_GetReturnsExpected(string value, string expected)
        {
            var control = new Panel
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
            var control = new Panel();
            Assert.NotEqual(IntPtr.Zero, control.Handle);

            control.Text = value;
            Assert.Equal(expected, control.Text);

            // Set same.
            control.Text = value;
            Assert.Equal(expected, control.Text);
        }

        [Fact]
        public void Panel_Text_SetWithHandler_CallsTextChanged()
        {
            var control = new Panel();
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

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetKeyEventArgsTheoryData))]
        public void Panel_OnKeyDown_Invoke_CallsKeyDown(KeyEventArgs eventArgs)
        {
            var control = new SubPanel();
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
        public void Panel_OnKeyPress_Invoke_CallsKeyPress(KeyPressEventArgs eventArgs)
        {
            var control = new SubPanel();
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
        public void Panel_OnKeyUp_Invoke_CallsKeyUp(KeyEventArgs eventArgs)
        {
            var control = new SubPanel();
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
        public void Panel_OnResize_Invoke_CallsResize(EventArgs eventArgs)
        {
            var control = new SubPanel();
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
        public void Panel_OnResize_InvokeWithResizeRedraw_CallsResizeAndInvalidate(EventArgs eventArgs)
        {
            var control = new SubPanel();
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
        [InlineData(BorderStyle.None, 1)]
        [InlineData(BorderStyle.Fixed3D, 0)]
        [InlineData(BorderStyle.FixedSingle, 0)]
        public void Panel_OnResize_InvokeWithDesignMode_CallsResizeAndInvalidate(BorderStyle borderStyle, int expectedCallCount)
        {
            var mockSite = new Mock<ISite>(MockBehavior.Strict);
            mockSite
                .Setup(s => s.DesignMode)
                .Returns(true);
            mockSite
                .Setup(s => s.GetService(typeof(AmbientProperties)))
                .Returns(null);
            var control = new SubPanel
            {
                Site = mockSite.Object,
                BorderStyle = borderStyle
            };
            Assert.NotEqual(IntPtr.Zero, control.Handle);
            int callCount = 0;
            EventHandler handler = (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(EventArgs.Empty, e);
                callCount++;
            };
            int invalidatedCallCount = 0;
            InvalidateEventHandler invalidatedHandler = (sender, e) => invalidatedCallCount++;

            // Call with handler.
            control.Resize += handler;
            control.Invalidated += invalidatedHandler;
            control.OnResize(EventArgs.Empty);
            Assert.Equal(1, callCount);
            Assert.Equal(expectedCallCount, invalidatedCallCount);

            // Remove handler.
            control.Resize -= handler;
            control.Invalidated -= invalidatedHandler;
            control.OnResize(EventArgs.Empty);
            Assert.Equal(1, callCount);
            Assert.Equal(expectedCallCount, invalidatedCallCount);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetEventArgsTheoryData))]
        public void Panel_OnResize_InvokeWithDesignModeAndResizeRedraw_CallsResizeAndInvalidate(EventArgs eventArgs)
        {
            var mockSite = new Mock<ISite>(MockBehavior.Strict);
            mockSite
                .Setup(s => s.DesignMode)
                .Returns(true);
            mockSite
                .Setup(s => s.GetService(typeof(AmbientProperties)))
                .Returns(null);
            var control = new SubPanel
            {
                Site = mockSite.Object
            };
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
            Assert.Equal(2, invalidatedCallCount);

            // Remove handler.
            control.Resize -= handler;
            control.Invalidated -= invalidatedHandler;
            control.OnResize(eventArgs);
            Assert.Equal(1, callCount);
            Assert.Equal(2, invalidatedCallCount);
        }

        [Fact]
        public void Panel_ToString_Invoke_ReturnsExpected()
        {
            var panel = new Panel { BorderStyle = BorderStyle.Fixed3D };
            Assert.Equal("System.Windows.Forms.Panel, BorderStyle: System.Windows.Forms.BorderStyle.Fixed3D", panel.ToString());
        }

        private class SubPanel : Panel
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

            public new bool HScroll
            {
                get => base.HScroll;
                set => base.HScroll = value;
            }

            public new bool VScroll
            {
                get => base.VScroll;
                set => base.VScroll = value;
            }

            public new void OnKeyDown(KeyEventArgs e) => base.OnKeyDown(e);

            public new void OnKeyPress(KeyPressEventArgs e) => base.OnKeyPress(e);

            public new void OnKeyUp(KeyEventArgs e) => base.OnKeyUp(e);

            public new void OnResize(EventArgs e) => base.OnResize(e);

            public new void SetStyle(ControlStyles flag, bool value) => base.SetStyle(flag, value);
        }
    }
}
