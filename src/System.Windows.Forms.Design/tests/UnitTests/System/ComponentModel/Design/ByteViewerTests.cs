// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.IO;
using System.Windows.Forms;
using WinForms.Common.Tests;
using Xunit;

namespace System.ComponentModel.Design.Tests
{
    using Point = System.Drawing.Point;
    using Size = System.Drawing.Size;

    public class ByteViewerTests : IClassFixture<ThreadExceptionFixture>
    {
        [WinFormsFact]
        public void ByteViewer_Ctor_Default()
        {
            using var control = new SubByteViewer();
            Assert.Null(control.AccessibleDefaultActionDescription);
            Assert.Null(control.AccessibleDescription);
            Assert.Null(control.AccessibleName);
            Assert.Equal(AccessibleRole.Default, control.AccessibleRole);
            Assert.False(control.AllowDrop);
            Assert.Equal(AnchorStyles.Top | AnchorStyles.Left, control.Anchor);
            Assert.False(control.AutoScroll);
            Assert.Equal(Size.Empty, control.AutoScrollMargin);
            Assert.Equal(Size.Empty, control.AutoScrollMinSize);
            Assert.Equal(Point.Empty, control.AutoScrollPosition);
            Assert.False(control.AutoSize);
            Assert.Equal(AutoSizeMode.GrowOnly, control.AutoSizeMode);
            Assert.Equal(Control.DefaultBackColor, control.BackColor);
            Assert.Null(control.BackgroundImage);
            Assert.Equal(ImageLayout.Tile, control.BackgroundImageLayout);
            Assert.Null(control.BindingContext);
            Assert.Equal(BorderStyle.None, control.BorderStyle);
            Assert.True(control.Bottom > 0);
            Assert.Equal(new Rectangle(0, 0, control.Width, control.Height), control.Bounds);
            Assert.True(control.CanEnableIme);
            Assert.False(control.CanFocus);
            Assert.True(control.CanRaiseEvents);
            Assert.False(control.CanSelect);
            Assert.False(control.Capture);
            Assert.True(control.CausesValidation);
            Assert.Equal(TableLayoutPanelCellBorderStyle.Inset, control.CellBorderStyle);
            Assert.Equal(new Rectangle(0, 0, control.Width, control.Height), control.ClientRectangle);
            Assert.Equal(new Size(control.Width, control.Height), control.ClientSize);
            Assert.Equal(1, control.ColumnCount);
            ColumnStyle columnStyle = Assert.IsType<ColumnStyle>(Assert.Single(control.ColumnStyles));
            Assert.Equal(SizeType.Percent, columnStyle.SizeType);
            Assert.Equal(100F, columnStyle.Width);
            Assert.Same(control.LayoutSettings.ColumnStyles, control.ColumnStyles);
            Assert.Null(control.Container);
            Assert.False(control.ContainsFocus);
            Assert.Null(control.ContextMenuStrip);
            Assert.NotEmpty(control.Controls);
            Assert.Same(control.Controls, control.Controls);
            Assert.False(control.Created);
            Assert.Equal(Cursors.Default, control.Cursor);
            Assert.Equal(Cursors.Default, control.DefaultCursor);
            Assert.Equal(ImeMode.Inherit, control.DefaultImeMode);
            Assert.Equal(new Padding(3), control.DefaultMargin);
            Assert.Equal(Size.Empty, control.DefaultMaximumSize);
            Assert.Equal(Size.Empty, control.DefaultMinimumSize);
            Assert.Equal(Padding.Empty, control.DefaultPadding);
            Assert.Equal(new Size(200, 100), control.DefaultSize);
            Assert.False(control.DesignMode);
            Assert.Equal(new Rectangle(0, 0, control.Width, control.Height), control.DisplayRectangle);
            Assert.Equal(DockStyle.None, control.Dock);
            Assert.NotNull(control.DockPadding);
            Assert.Same(control.DockPadding, control.DockPadding);
            Assert.Equal(0, control.DockPadding.Top);
            Assert.Equal(0, control.DockPadding.Bottom);
            Assert.Equal(0, control.DockPadding.Left);
            Assert.Equal(0, control.DockPadding.Right);
            Assert.True(control.DoubleBuffered);
            Assert.True(control.Enabled);
            Assert.NotNull(control.Events);
            Assert.Same(control.Events, control.Events);
            Assert.False(control.Focused);
            Assert.Equal(Control.DefaultFont, control.Font);
            Assert.Equal(control.Font.Height, control.FontHeight);
            Assert.Equal(Control.DefaultForeColor, control.ForeColor);
            Assert.Equal(TableLayoutPanelGrowStyle.AddRows, control.GrowStyle);
            Assert.True(control.HasChildren);
            Assert.True(control.Height > 0);
            Assert.NotNull(control.HorizontalScroll);
            Assert.Same(control.HorizontalScroll, control.HorizontalScroll);
            Assert.False(control.HScroll);
            Assert.Equal(ImeMode.NoControl, control.ImeMode);
            Assert.Equal(ImeMode.NoControl, control.ImeModeBase);
            Assert.False(control.IsAccessible);
            Assert.False(control.IsMirrored);
            Assert.NotNull(control.LayoutEngine);
            Assert.Same(control.LayoutEngine, control.LayoutEngine);
            Assert.NotNull(control.LayoutSettings);
            Assert.Same(control.LayoutSettings, control.LayoutSettings);
            Assert.Equal(0, control.Left);
            Assert.Equal(Point.Empty, control.Location);
            Assert.Equal(new Padding(3), control.Margin);
            Assert.Equal(Size.Empty, control.MaximumSize);
            Assert.Equal(Size.Empty, control.MinimumSize);
            Assert.Equal(Padding.Empty, control.Padding);
            Assert.Null(control.Parent);
            Assert.Equal(new Size(4, 4), control.PreferredSize);
            Assert.Equal("Microsoft\u00AE .NET", control.ProductName);
            Assert.False(control.RecreatingHandle);
            Assert.Null(control.Region);
            Assert.True(control.ResizeRedraw);
            Assert.True(control.Right > 0);
            Assert.Equal(RightToLeft.No, control.RightToLeft);
            Assert.Equal(1, control.RowCount);
            RowStyle rowStyle = Assert.IsType<RowStyle>(Assert.Single(control.RowStyles));
            Assert.Equal(SizeType.Percent, rowStyle.SizeType);
            Assert.Equal(100F, rowStyle.Height);
            Assert.Same(control.LayoutSettings.RowStyles, control.RowStyles);
            Assert.True(control.ShowFocusCues);
            Assert.True(control.ShowKeyboardCues);
            Assert.Equal(new Size(control.Width, control.Height), control.Size);
            Assert.Equal(0, control.TabIndex);
            Assert.False(control.TabStop);
            Assert.Empty(control.Text);
            Assert.Equal(0, control.Top);
            Assert.Null(control.TopLevelControl);
            Assert.False(control.UseWaitCursor);
            Assert.True(control.Visible);
            Assert.NotNull(control.VerticalScroll);
            Assert.Same(control.VerticalScroll, control.VerticalScroll);
            Assert.False(control.VScroll);
            Assert.True(control.Width > 0);

            Assert.False(control.IsHandleCreated);
        }

        [WinFormsFact]
        public void ByteViewer_CreateParams_GetDefault_ReturnsExpected()
        {
            using var control = new SubByteViewer();
            CreateParams createParams = control.CreateParams;
            Assert.Null(createParams.Caption);
            Assert.Null(createParams.ClassName);
            Assert.Equal(0x8, createParams.ClassStyle);
            Assert.Equal(0x10000, createParams.ExStyle);
            Assert.Equal(control.Height, createParams.Height);
            Assert.Equal(IntPtr.Zero, createParams.Parent);
            Assert.Null(createParams.Param);
            Assert.Equal(0x56000000, createParams.Style);
            Assert.Equal(control.Width, createParams.Width);
            Assert.Equal(0, createParams.X);
            Assert.Equal(0, createParams.Y);
            Assert.Same(createParams, control.CreateParams);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsFact]
        public void ByteViewer_GetAutoSizeMode_Invoke_ReturnsExpected()
        {
            using var control = new SubByteViewer();
            Assert.Equal(AutoSizeMode.GrowOnly, control.GetAutoSizeMode());
        }

        [WinFormsFact]
        public void ByteViewer_GetBytes_Invoke_ReturnsExpected()
        {
            using var control = new ByteViewer();
            Assert.Null(control.GetBytes());
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsFact]
        public void ByteViewer_GetDisplayMode_Invoke_ReturnsExpected()
        {
            using var control = new ByteViewer();
            Assert.Equal(DisplayMode.Hexdump, control.GetDisplayMode());
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [InlineData(0, true)]
        [InlineData(SubByteViewer.ScrollStateAutoScrolling, false)]
        [InlineData(SubByteViewer.ScrollStateFullDrag, false)]
        [InlineData(SubByteViewer.ScrollStateHScrollVisible, false)]
        [InlineData(SubByteViewer.ScrollStateUserHasScrolled, false)]
        [InlineData(SubByteViewer.ScrollStateVScrollVisible, false)]
        [InlineData(int.MaxValue, false)]
        [InlineData((-1), false)]
        public void ByteViewer_GetScrollState_Invoke_ReturnsExpected(int bit, bool expected)
        {
            using var control = new SubByteViewer();
            Assert.Equal(expected, control.GetScrollState(bit));
        }

        [WinFormsTheory]
        [InlineData(ControlStyles.ContainerControl, true)]
        [InlineData(ControlStyles.UserPaint, true)]
        [InlineData(ControlStyles.Opaque, false)]
        [InlineData(ControlStyles.ResizeRedraw, true)]
        [InlineData(ControlStyles.FixedWidth, false)]
        [InlineData(ControlStyles.FixedHeight, false)]
        [InlineData(ControlStyles.StandardClick, true)]
        [InlineData(ControlStyles.Selectable, false)]
        [InlineData(ControlStyles.UserMouse, false)]
        [InlineData(ControlStyles.SupportsTransparentBackColor, true)]
        [InlineData(ControlStyles.StandardDoubleClick, true)]
        [InlineData(ControlStyles.AllPaintingInWmPaint, true)]
        [InlineData(ControlStyles.CacheText, false)]
        [InlineData(ControlStyles.EnableNotifyMessage, false)]
        [InlineData(ControlStyles.DoubleBuffer, false)]
        [InlineData(ControlStyles.OptimizedDoubleBuffer, true)]
        [InlineData(ControlStyles.UseTextForAccessibility, true)]
        [InlineData((ControlStyles)0, true)]
        [InlineData((ControlStyles)int.MaxValue, false)]
        [InlineData((ControlStyles)(-1), false)]
        public void ByteViewer_GetStyle_Invoke_ReturnsExpected(ControlStyles flag, bool expected)
        {
            using var control = new SubByteViewer();
            Assert.Equal(expected, control.GetStyle(flag));

            // Call again to test caching.
            Assert.Equal(expected, control.GetStyle(flag));
        }

        [WinFormsFact]
        public void ByteViewer_GetTopLevel_Invoke_ReturnsExpected()
        {
            using var control = new SubByteViewer();
            Assert.False(control.GetTopLevel());
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetEventArgsTheoryData))]
        public void ByteViewer_OnHandleCreated_InvokeWithHandle_CallsHandleCreated(EventArgs eventArgs)
        {
            using var control = new SubByteViewer();
            Assert.NotEqual(IntPtr.Zero, control.Handle);
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
            Assert.True(control.IsHandleCreated);

            // Remove handler.
            control.HandleCreated -= handler;
            control.OnHandleCreated(eventArgs);
            Assert.Equal(1, callCount);
            Assert.True(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetEventArgsTheoryData))]
        public void ByteViewer_OnHandleDestroyed_Invoke_CallsHandleDestroyed(EventArgs eventArgs)
        {
            using var control = new SubByteViewer();
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
            Assert.False(control.IsHandleCreated);

            // Remove handler.
            control.HandleDestroyed -= handler;
            control.OnHandleDestroyed(eventArgs);
            Assert.Equal(1, callCount);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetEventArgsTheoryData))]
        public void ByteViewer_OnHandleDestroyed_InvokeWithHandle_CallsHandleDestroyed(EventArgs eventArgs)
        {
            using var control = new SubByteViewer();
            Assert.NotEqual(IntPtr.Zero, control.Handle);
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
            Assert.True(control.IsHandleCreated);

            // Remove handler.
            control.HandleDestroyed -= handler;
            control.OnHandleDestroyed(eventArgs);
            Assert.Equal(1, callCount);
            Assert.True(control.IsHandleCreated);
        }

        public static IEnumerable<object[]> OnLayout_TestData()
        {
            yield return new object[] { new LayoutEventArgs(null, null) };
            yield return new object[] { new LayoutEventArgs(new Control(), null) };
            yield return new object[] { new LayoutEventArgs(new Control(), string.Empty) };
            yield return new object[] { new LayoutEventArgs(new Control(), "ChildIndex") };
            yield return new object[] { new LayoutEventArgs(new Control(), "Visible") };
            yield return new object[] { new LayoutEventArgs(new Control(), "Items") };
            yield return new object[] { new LayoutEventArgs(new Control(), "Rows") };
            yield return new object[] { new LayoutEventArgs(new Control(), "Columns") };
            yield return new object[] { new LayoutEventArgs(new Control(), "RowStyles") };
            yield return new object[] { new LayoutEventArgs(new Control(), "ColumnStyles") };
            yield return new object[] { new LayoutEventArgs(new Control(), "TableIndex") };
            yield return new object[] { new LayoutEventArgs(new Control(), "GrowStyle") };
            yield return new object[] { new LayoutEventArgs(new Control(), "CellBorderStyle") };
            yield return new object[] { new LayoutEventArgs(new Control(), "LayoutSettings") };
            yield return new object[] { new LayoutEventArgs(new Control(), "NoSuchProperty") };
        }

        [WinFormsTheory]
        [MemberData(nameof(OnLayout_TestData))]
        public void ByteViewer_OnLayout_Invoke_CallsLayout(LayoutEventArgs eventArgs)
        {
            using var control = new SubByteViewer();
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
            Assert.False(control.IsHandleCreated);

            // Remove handler.
            control.Layout -= handler;
            control.OnLayout(eventArgs);
            Assert.Equal(1, callCount);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [MemberData(nameof(OnLayout_TestData))]
        public void ByteViewer_OnLayout_InvokeWithHandle_CallsLayout(LayoutEventArgs eventArgs)
        {
            using var control = new SubByteViewer();
            Assert.NotEqual(IntPtr.Zero, control.Handle);
            int invalidatedCallCount = 0;
            control.Invalidated += (sender, e) => invalidatedCallCount++;
            int styleChangedCallCount = 0;
            control.StyleChanged += (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            control.HandleCreated += (sender, e) => createdCallCount++;
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
            Assert.True(control.IsHandleCreated);
            Assert.Equal(1, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Remove handler.
            control.Layout -= handler;
            control.OnLayout(eventArgs);
            Assert.Equal(1, callCount);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(2, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
        }

        [WinFormsFact]
        public void ByteViewer_OnLayout_InvokeNullE_ThrowsNullReferenceException()
        {
            using var control = new SubByteViewer();
            Assert.Throws<NullReferenceException>(() => control.OnLayout(null));
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetKeyEventArgsTheoryData))]
        public void ByteViewer_OnKeyDown_Invoke_CallsKeyDown(KeyEventArgs eventArgs)
        {
            using var control = new SubByteViewer();
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
            Assert.Equal(0, callCount);

            // Remove handler.
            control.KeyDown -= handler;
            control.OnKeyDown(eventArgs);
            Assert.Equal(0, callCount);
        }

        [WinFormsFact]
        public void ByteViewer_OnPaint_Invoke_CallsPaint()
        {
            using var image = new Bitmap(10, 10);
            using Graphics graphics = Graphics.FromImage(image);
            var eventArgs = new PaintEventArgs(graphics, Rectangle.Empty);

            using var control = new SubByteViewer();
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
            Assert.False(control.IsHandleCreated);

            // Remove handler.
            control.Paint -= handler;
            control.OnPaint(eventArgs);
            Assert.Equal(1, callCount);
            Assert.False(control.IsHandleCreated);
        }

        public static IEnumerable<object[]> OnPaint_WithBytes_TestData()
        {
            foreach (DisplayMode displayMode in Enum.GetValues(typeof(DisplayMode)))
            {
                yield return new object[] { Array.Empty<byte>(), displayMode };
                yield return new object[] { new byte[] { 1, 2, 3 }, displayMode };
                yield return new object[] { new byte[] { 1, 2, 3, 4, 5, 6, 7, 8 }, displayMode };
                yield return new object[] { new byte[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 }, displayMode };
            }
        }

        [WinFormsTheory]
        [MemberData(nameof(OnPaint_WithBytes_TestData))]
        public void ByteViewer_OnPaint_InvokeWithBytes_CallsPaint(byte[] bytes, DisplayMode displayMode)
        {
            using var image = new Bitmap(10, 10);
            using Graphics graphics = Graphics.FromImage(image);
            var eventArgs = new PaintEventArgs(graphics, Rectangle.Empty);

            using var control = new SubByteViewer();
            control.SetBytes(bytes);
            control.SetDisplayMode(displayMode);
            Assert.NotEqual(IntPtr.Zero, control.Handle);
            int invalidatedCallCount = 0;
            control.Invalidated += (sender, e) => invalidatedCallCount++;
            int styleChangedCallCount = 0;
            control.StyleChanged += (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            control.HandleCreated += (sender, e) => createdCallCount++;

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
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Remove handler.
            control.Paint -= handler;
            control.OnPaint(eventArgs);
            Assert.Equal(1, callCount);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
        }

        [WinFormsFact]
        public void ByteViewer_OnPaint_InvokeWithHandle_CallsPaint()
        {
            using var image = new Bitmap(10, 10);
            using Graphics graphics = Graphics.FromImage(image);
            var eventArgs = new PaintEventArgs(graphics, Rectangle.Empty);

            using var control = new SubByteViewer();
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
            Assert.False(control.IsHandleCreated);

            // Remove handler.
            control.Paint -= handler;
            control.OnPaint(eventArgs);
            Assert.Equal(1, callCount);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [MemberData(nameof(OnPaint_WithBytes_TestData))]
        public void ByteViewer_OnPaint_InvokeWithBytesWithHandle_CallsPaint(byte[] bytes, DisplayMode displayMode)
        {
            using var image = new Bitmap(10, 10);
            using Graphics graphics = Graphics.FromImage(image);
            var eventArgs = new PaintEventArgs(graphics, Rectangle.Empty);

            using var control = new SubByteViewer();
            control.SetBytes(bytes);
            control.SetDisplayMode(displayMode);
            Assert.NotEqual(IntPtr.Zero, control.Handle);
            int invalidatedCallCount = 0;
            control.Invalidated += (sender, e) => invalidatedCallCount++;
            int styleChangedCallCount = 0;
            control.StyleChanged += (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            control.HandleCreated += (sender, e) => createdCallCount++;

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
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Remove handler.
            control.Paint -= handler;
            control.OnPaint(eventArgs);
            Assert.Equal(1, callCount);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
        }

        [WinFormsFact]
        public void ByteViewer_OnPaint_NullE_ThrowsNullReferenceException()
        {
            using var control = new SubByteViewer();
            Assert.Throws<NullReferenceException>(() => control.OnPaint(null));
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetStringWithNullTheoryData))]
        public void ByteViewer_SaveToFile_InvokeNoBytes_Nop(string path)
        {
            using var control = new ByteViewer();
            control.SaveToFile(path);
        }

        [WinFormsFact]
        public void ByteViewer_SaveToFile_InvokeWithBytes_Success()
        {
            using var control = new ByteViewer();
            control.SetBytes(new byte[] { 1, 2, 3 });
            string path = "ByteViewerContent";
            try
            {
                control.SaveToFile(path);
                Assert.Equal(new byte[] { 1, 2, 3 }, File.ReadAllBytes(path));
            }
            finally
            {
                File.Delete(path);
            }
        }

        [WinFormsFact]
        public void ByteViewer_SaveToFile_InvokeNullPath_ThrowsArgumentNullException()
        {
            using var control = new ByteViewer();
            control.SetBytes(Array.Empty<byte>());
            Assert.Throws<ArgumentNullException>("path", () => control.SaveToFile(null));
        }

        [WinFormsTheory]
        [InlineData("")]
        [InlineData("\0")]
        public void ByteViewer_SaveToFile_InvokeInvalidPath_ThrowsArgumentException(string path)
        {
            using var control = new ByteViewer();
            control.SetBytes(Array.Empty<byte>());
            Assert.Throws<ArgumentException>("path", () => control.SaveToFile(path));
        }

        public static IEnumerable<object[]> ScrollChanged_TestData()
        {
            yield return new object[] { null, null };
            yield return new object[] { null, new EventArgs() };
            yield return new object[] { new object(), null };
            yield return new object[] { new object(), new EventArgs() };
        }

        [WinFormsTheory]
        [MemberData(nameof(ScrollChanged_TestData))]
        public void ByteViewer_ScrollChanged_Invoke_Success(object source, EventArgs e)
        {
            using var control = new SubByteViewer();
            control.ScrollChanged(source, e);
            Assert.False(control.IsHandleCreated);

            // Call again.
            control.ScrollChanged(source, e);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [MemberData(nameof(ScrollChanged_TestData))]
        public void ByteViewer_ScrollChanged_InvokeWithHandle_Success(object source, EventArgs e)
        {
            using var control = new SubByteViewer();
            Assert.NotEqual(IntPtr.Zero, control.Handle);
            int invalidatedCallCount = 0;
            control.Invalidated += (sender, e) => invalidatedCallCount++;
            int styleChangedCallCount = 0;
            control.StyleChanged += (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            control.HandleCreated += (sender, e) => createdCallCount++;

            control.ScrollChanged(source, e);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(1, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Call again.
            control.ScrollChanged(source, e);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(2, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
        }

        public static IEnumerable<object[]> SetBytes_TestData()
        {
            yield return new object[] { Array.Empty<byte>() };
            yield return new object[] { new byte[] { 1, 2, 3 } };
            yield return new object[] { new byte[] { 1, 2, 3, 4, 5, 6, 7, 8 } };
            yield return new object[] { new byte[] { 1, 2, 3, 4, 5, 6, 7, 8, 9 } };
        }

        [WinFormsTheory]
        [MemberData(nameof(SetBytes_TestData))]
        public void ByteViewer_SetBytes_Invoke_GetReturnExpected(byte[] bytes)
        {
            using var control = new ByteViewer();
            control.SetBytes(bytes);
            Assert.Same(bytes, control.GetBytes());
            Assert.False(control.IsHandleCreated);

            // Set same.
            control.SetBytes(bytes);
            Assert.Same(bytes, control.GetBytes());
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [MemberData(nameof(SetBytes_TestData))]
        public void ByteViewer_SetBytes_InvokeWithBytes_GetReturnExpected(byte[] bytes)
        {
            using var control = new ByteViewer();
            control.SetBytes(new byte[] { 1 });

            control.SetBytes(bytes);
            Assert.Same(bytes, control.GetBytes());
            Assert.False(control.IsHandleCreated);

            // Set same.
            control.SetBytes(bytes);
            Assert.Same(bytes, control.GetBytes());
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsFact]
        public void ByteViewer_SetBytes_NullBytes_ThrowsArgumentNullException()
        {
            using var control = new ByteViewer();
            Assert.Throws<ArgumentNullException>("bytes", () => control.SetBytes(null));
        }

        [WinFormsTheory]
        [InlineData(DisplayMode.Auto)]
        [InlineData(DisplayMode.Hexdump)]
        public void ByteViewer_SetDisplayMode_InvokeNoBytes_GetReturnsExpected(DisplayMode value)
        {
            using var control = new ByteViewer();
            control.SetDisplayMode(value);
            Assert.Equal(value, control.GetDisplayMode());
            Assert.False(control.IsHandleCreated);

            // Set same.
            control.SetDisplayMode(value);
            Assert.Equal(value, control.GetDisplayMode());
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetEnumTypeTheoryData), typeof(DisplayMode))]
        public void ByteViewer_SetDisplayMode_InvokeWithBytes_GetReturnsExpected(DisplayMode value)
        {
            using var control = new ByteViewer();
            control.SetBytes(new byte[] { 1, 2, 3 });

            control.SetDisplayMode(value);
            Assert.Equal(value, control.GetDisplayMode());
            Assert.False(control.IsHandleCreated);

            // Set same.
            control.SetDisplayMode(value);
            Assert.Equal(value, control.GetDisplayMode());
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [InlineData(DisplayMode.Ansi, 2, 3)]
        [InlineData(DisplayMode.Auto, 0, 0)]
        [InlineData(DisplayMode.Hexdump, 0, 0)]
        [InlineData(DisplayMode.Unicode, 2, 3)]
        public void ByteViewer_SetDisplayMode_InvokeWithBytesWithHandle_GetReturnsExpected(DisplayMode value, int expectedInvalidatedCallCount1, int expectedInvalidatedCallCount2)
        {
            using var control = new ByteViewer();
            control.SetBytes(new byte[] { 1, 2, 3 });
            Assert.NotEqual(IntPtr.Zero, control.Handle);
            int invalidatedCallCount = 0;
            control.Invalidated += (sender, e) => invalidatedCallCount++;
            int styleChangedCallCount = 0;
            control.StyleChanged += (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            control.HandleCreated += (sender, e) => createdCallCount++;

            control.SetDisplayMode(value);
            Assert.Equal(value, control.GetDisplayMode());
            Assert.True(control.IsHandleCreated);
            Assert.Equal(expectedInvalidatedCallCount1, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Set same.
            control.SetDisplayMode(value);
            Assert.Equal(value, control.GetDisplayMode());
            Assert.True(control.IsHandleCreated);
            Assert.Equal(expectedInvalidatedCallCount2, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
        }

        public static IEnumerable<object[]> SetDisplayMode_AnsiWithBytes_TestData()
        {
            yield return new object[] { new byte[] { 1, 2, 3 }, "\u0001\u0002\u0003" };
            yield return new object[] { new byte[] { (byte)'a', (byte)'b', (byte)'c' }, "abc" };
            yield return new object[] { new byte[] { (byte)'a', (byte)'b', (byte)'c', (byte)'\0', (byte)'d', (byte)'e', (byte)'f' }, $"abc\u000Bdef" };
        }

        [WinFormsTheory]
        [MemberData(nameof(SetDisplayMode_AnsiWithBytes_TestData))]
        public void ByteViewer_SetDisplayMode_AnsiWithBytes_EditReturnsExpected(byte[] bytes, string expected)
        {
            using var control = new ByteViewer();
            control.SetBytes(bytes);
            control.SetDisplayMode(DisplayMode.Ansi);

            TextBox textBox = control.Controls.OfType<TextBox>().Single();
            ScrollBar scrollBar = control.Controls.OfType<ScrollBar>().Single();
            Assert.Equal(expected, textBox.Text);
            Assert.True(textBox.Visible);
            Assert.False(scrollBar.Visible);
            Assert.False(control.IsHandleCreated);

            // Set different.
            control.SetBytes(new byte[] { (byte)'1', (byte)'2', (byte)'3' });
            Assert.Equal("123", textBox.Text);
            Assert.True(textBox.Visible);
            Assert.False(scrollBar.Visible);
            Assert.False(control.IsHandleCreated);
        }

        public static IEnumerable<object[]> SetDisplayMode_UnicodeWithBytes_TestData()
        {
            yield return new object[] { new byte[] { 1, 0, 2, 0, 3, 0 }, "\u0001\u0002\u0003" };
            yield return new object[] { new byte[] { (byte)'a', 0, (byte)'b', 0, (byte)'c', 0 }, "abc" };
            yield return new object[] { new byte[] { (byte)'a', 0, (byte)'b', 0, (byte)'c', 0, (byte)'\0', 0, (byte)'d', 0, (byte)'e', 0, (byte)'f', 0 }, $"abc\u000Bdef" };
        }

        [WinFormsTheory]
        [MemberData(nameof(SetDisplayMode_UnicodeWithBytes_TestData))]
        public void ByteViewer_SetDisplayMode_UnicodeWithBytes_EditReturnsExpected(byte[] bytes, string expected)
        {
            using var control = new ByteViewer();
            control.SetBytes(bytes);
            control.SetDisplayMode(DisplayMode.Unicode);

            TextBox textBox = control.Controls.OfType<TextBox>().Single();
            ScrollBar scrollBar = control.Controls.OfType<ScrollBar>().Single();
            Assert.Equal(expected, textBox.Text);
            Assert.True(textBox.Visible);
            Assert.False(scrollBar.Visible);
            Assert.False(control.IsHandleCreated);

            // Set different.
            control.SetBytes(new byte[] { (byte)'1', 0, (byte)'2', 0, (byte)'3', 0 });
            Assert.Equal("123", textBox.Text);
            Assert.True(textBox.Visible);
            Assert.False(scrollBar.Visible);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [InlineData(DisplayMode.Ansi)]
        [InlineData(DisplayMode.Unicode)]
        public void ByteViewer_SetDisplayMode_InvokeNoBytes_ThrowsNullReferenceException(DisplayMode value)
        {
            using var control = new ByteViewer();
            Assert.Throws<NullReferenceException>(() => control.SetDisplayMode(value));
            Assert.Equal(value, control.GetDisplayMode());
            Assert.False(control.IsHandleCreated);

            // Set same.
            Assert.Throws<NullReferenceException>(() => control.SetDisplayMode(value));
            Assert.Equal(value, control.GetDisplayMode());
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetEnumTypeTheoryDataInvalid), typeof(DisplayMode))]
        public void ByteViewer_SetDisplayMode_InvokeInvalidMode_ThrowsInvalidEnumArgumentException(DisplayMode value)
        {
            using var control = new ByteViewer();
            Assert.Throws<InvalidEnumArgumentException>("mode", () => control.SetDisplayMode(value));
        }

        [WinFormsFact]
        public void ByteViewer_SetFile_InvokeNoBytes_Success()
        {
            using var control = new ByteViewer();
            using TempFile file = TempFile.Create(new byte[] { 1, 2, 3 });
            control.SetFile(file.Path);
            Assert.Equal(new byte[] { 1, 2, 3, 0 }, control.GetBytes());
        }

        [WinFormsFact]
        public void ByteViewer_SetFile_InvokeWithBytes_Success()
        {
            using var control = new ByteViewer();
            control.SetBytes(new byte[] { 4, 5, 6 });

            using TempFile file = TempFile.Create(new byte[] { 1, 2, 3 });
            control.SetFile(file.Path);
            Assert.Equal(new byte[] { 1, 2, 3, 0 }, control.GetBytes());
        }

        [WinFormsFact]
        public void ByteViewer_SetFile_InvokeNullPath_ThrowsArgumentNullException()
        {
            using var control = new ByteViewer();
            control.SetBytes(Array.Empty<byte>());
            Assert.Throws<ArgumentNullException>("path", () => control.SetFile(null));
        }

        [WinFormsTheory]
        [InlineData("")]
        [InlineData("\0")]
        public void ByteViewer_SetFile_InvokeInvalidPath_ThrowsArgumentException(string path)
        {
            using var control = new ByteViewer();
            control.SetBytes(Array.Empty<byte>());
            Assert.Throws<ArgumentException>("path", () => control.SetFile(path));
        }

        [WinFormsTheory]
        [InlineData(-1)]
        [InlineData(0)]
        [InlineData(1)]
        public void ByteViewer_SetStartLine_InvokeNoBytes_Success(int line)
        {
            using var control = new ByteViewer();
            control.SetStartLine(line);
            Assert.False(control.IsHandleCreated);

            // Call again.
            control.SetStartLine(line);
            Assert.False(control.IsHandleCreated);
        }

        public static IEnumerable<object[]> SetStartLine_WithBytes_TestData()
        {
            yield return new object[] { Array.Empty<byte>(), -1 };
            yield return new object[] { Array.Empty<byte>(), 0 };
            yield return new object[] { Array.Empty<byte>(), 1 };
            yield return new object[] { Array.Empty<byte>(), int.MaxValue };
            yield return new object[] { new byte[] { 1, 2, 3 }, -1 };
            yield return new object[] { new byte[] { 1, 2, 3 }, 0 };
            yield return new object[] { new byte[] { 1, 2, 3 }, 1 };
            yield return new object[] { new byte[] { 1, 2, 3 }, int.MaxValue };
            yield return new object[] { new byte[] { 1, 2, 3, 4, 5, 6, 7, 8, 9 }, -1 };
            yield return new object[] { new byte[] { 1, 2, 3, 4, 5, 6, 7, 8, 9 }, 0 };
            yield return new object[] { new byte[] { 1, 2, 3, 4, 5, 6, 7, 8, 9 }, 1 };
            yield return new object[] { new byte[] { 1, 2, 3, 4, 5, 6, 7, 8, 9 }, int.MaxValue };
        }

        [WinFormsTheory]
        [MemberData(nameof(SetStartLine_WithBytes_TestData))]
        public void ByteViewer_SetStartLine_InvokeWithBytes_Success(byte[] bytes, int line)
        {
            using var control = new ByteViewer();
            control.SetBytes(bytes);

            control.SetStartLine(line);
            Assert.False(control.IsHandleCreated);

            // Call again.
            control.SetStartLine(line);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [InlineData(-1)]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(int.MaxValue)]
        public void ByteViewer_SetStartLine_InvokeNoBytesWithHandle_Success(int line)
        {
            using var control = new ByteViewer();
            Assert.NotEqual(IntPtr.Zero, control.Handle);
            int invalidatedCallCount = 0;
            control.Invalidated += (sender, e) => invalidatedCallCount++;
            int styleChangedCallCount = 0;
            control.StyleChanged += (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            control.HandleCreated += (sender, e) => createdCallCount++;

            control.SetStartLine(line);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Call again.
            control.SetStartLine(line);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
        }

        [WinFormsTheory]
        [MemberData(nameof(SetStartLine_WithBytes_TestData))]
        public void ByteViewer_SetStartLine_InvokeWithBytesWithHandle_Success(byte[] bytes, int line)
        {
            using var control = new ByteViewer();
            control.SetBytes(bytes);
            Assert.NotEqual(IntPtr.Zero, control.Handle);
            int invalidatedCallCount = 0;
            control.Invalidated += (sender, e) => invalidatedCallCount++;
            int styleChangedCallCount = 0;
            control.StyleChanged += (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            control.HandleCreated += (sender, e) => createdCallCount++;

            control.SetStartLine(line);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Call again.
            control.SetStartLine(line);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
        }

        private class SubByteViewer : ByteViewer
        {
            public new const int ScrollStateAutoScrolling = ByteViewer.ScrollStateAutoScrolling;

            public new const int ScrollStateHScrollVisible = ByteViewer.ScrollStateHScrollVisible;

            public new const int ScrollStateVScrollVisible = ByteViewer.ScrollStateVScrollVisible;

            public new const int ScrollStateUserHasScrolled = ByteViewer.ScrollStateUserHasScrolled;

            public new const int ScrollStateFullDrag = ByteViewer.ScrollStateFullDrag;

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

            public new bool HScroll
            {
                get => base.HScroll;
                set => base.HScroll = value;
            }

            public new bool ResizeRedraw
            {
                get => base.ResizeRedraw;
                set => base.ResizeRedraw = value;
            }

            public new bool ShowFocusCues => base.ShowFocusCues;

            public new bool ShowKeyboardCues => base.ShowKeyboardCues;

            public new bool VScroll
            {
                get => base.VScroll;
                set => base.VScroll = value;
            }

            public new AutoSizeMode GetAutoSizeMode() => base.GetAutoSizeMode();

            public new bool GetScrollState(int bit) => base.GetScrollState(bit);

            public new bool GetStyle(ControlStyles flag) => base.GetStyle(flag);

            public new bool GetTopLevel() => base.GetTopLevel();

            public new void OnHandleCreated(EventArgs e) => base.OnHandleCreated(e);

            public new void OnHandleDestroyed(EventArgs e) => base.OnHandleDestroyed(e);

            public new void OnLayout(LayoutEventArgs levent) => base.OnLayout(levent);

            public new void OnKeyDown(KeyEventArgs e) => base.OnKeyDown(e);

            public new void OnPaint(PaintEventArgs e) => base.OnPaint(e);

            public new void ScrollChanged(object source, EventArgs e) => base.ScrollChanged(source, e);
        }
    }
}
