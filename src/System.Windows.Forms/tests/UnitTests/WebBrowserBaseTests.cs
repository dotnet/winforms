// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using Moq;
using Xunit;

namespace System.Windows.Forms.Tests
{
    using Size = System.Drawing.Size;

    [Collection("Sequential")] // workaround for WebBrowser control corrupting memory when run on multiple UI threads (instantiated via GUID)
    public class WebBrowserBaseTests
    {
        public static IEnumerable<object[]> Bounds_Set_TestData()
        {
            yield return new object[] { 0, 0, 0, 0 };
            yield return new object[] { -1, -2, -3, -4 };
            yield return new object[] { 1, 0, 0, 0 };
            yield return new object[] { 0, 2, 0, 0 };
            yield return new object[] { 1, 2, 0, 0 };
            yield return new object[] { 0, 0, 1, 0 };
            yield return new object[] { 0, 0, 0, 2 };
            yield return new object[] { 0, 0, 1, 2 };
            yield return new object[] { 1, 2, 30, 40 };
        }

        [WinFormsTheory]
        [MemberData(nameof(Bounds_Set_TestData))]
        public void WebBrowserBase_Bounds_Set_GetReturnsExpected(int x, int y, int width, int height)
        {
            using var control = new SubWebBrowserBase("8856f961-340a-11d0-a96b-00c04fd705a2");
            int layoutCallCount = 0;
            int resizeCallCount = 0;
            int sizeChangedCallCount = 0;
            int clientSizeChangedCallCount = 0;
            ((Control)control).Layout += (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(control, e.AffectedControl);
                Assert.Equal("Bounds", e.AffectedProperty);
                Assert.Equal(resizeCallCount, layoutCallCount);
                Assert.Equal(sizeChangedCallCount, layoutCallCount);
                Assert.Equal(clientSizeChangedCallCount, layoutCallCount);
                layoutCallCount++;
            };
            control.Resize += (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(EventArgs.Empty, e);
                Assert.Equal(layoutCallCount - 1, resizeCallCount);
                Assert.Equal(sizeChangedCallCount, resizeCallCount);
                Assert.Equal(clientSizeChangedCallCount, resizeCallCount);
                resizeCallCount++;
            };
            control.SizeChanged += (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(EventArgs.Empty, e);
                Assert.Equal(resizeCallCount - 1, sizeChangedCallCount);
                Assert.Equal(layoutCallCount - 1, sizeChangedCallCount);
                Assert.Equal(clientSizeChangedCallCount, sizeChangedCallCount);
                sizeChangedCallCount++;
            };
            control.ClientSizeChanged += (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(EventArgs.Empty, e);
                Assert.Equal(resizeCallCount - 1, clientSizeChangedCallCount);
                Assert.Equal(layoutCallCount - 1, clientSizeChangedCallCount);
                Assert.Equal(sizeChangedCallCount - 1, clientSizeChangedCallCount);
                clientSizeChangedCallCount++;
            };

            control.Bounds = new Rectangle(x, y, width, height);
            Assert.Equal(new Size(width, height), control.ClientSize);
            Assert.Equal(new Rectangle(0, 0, width, height), control.ClientRectangle);
            Assert.Equal(new Rectangle(0, 0, width, height), control.DisplayRectangle);
            Assert.Equal(new Size(width, height), control.Size);
            Assert.Equal(x, control.Left);
            Assert.Equal(x + width, control.Right);
            Assert.Equal(y, control.Top);
            Assert.Equal(y + height, control.Bottom);
            Assert.Equal(width, control.Width);
            Assert.Equal(height, control.Height);
            Assert.Equal(new Rectangle(x, y, width, height), control.Bounds);
            Assert.Equal(1, layoutCallCount);
            Assert.Equal(1, resizeCallCount);
            Assert.Equal(1, sizeChangedCallCount);
            Assert.Equal(1, clientSizeChangedCallCount);
            Assert.False(control.IsHandleCreated);

            // Call again.
            control.Bounds = new Rectangle(x, y, width, height);
            Assert.Equal(new Size(width, height), control.ClientSize);
            Assert.Equal(new Rectangle(0, 0, width, height), control.ClientRectangle);
            Assert.Equal(new Rectangle(0, 0, width, height), control.DisplayRectangle);
            Assert.Equal(new Size(width, height), control.Size);
            Assert.Equal(x, control.Left);
            Assert.Equal(x + width, control.Right);
            Assert.Equal(y, control.Top);
            Assert.Equal(y + height, control.Bottom);
            Assert.Equal(width, control.Width);
            Assert.Equal(height, control.Height);
            Assert.Equal(new Rectangle(x, y, width, height), control.Bounds);
            Assert.Equal(1, layoutCallCount);
            Assert.Equal(1, resizeCallCount);
            Assert.Equal(1, sizeChangedCallCount);
            Assert.Equal(1, clientSizeChangedCallCount);
            Assert.False(control.IsHandleCreated);
        }

        public static IEnumerable<object[]> Bounds_SetWithHandle_TestData()
        {
            yield return new object[] { true, 0, 0, 0, 0, 0, 0, 1 };
            yield return new object[] { true, -1, -2, -3, -4, 0, 0, 1 };
            yield return new object[] { true, 1, 0, 0, 0, 0, 0, 1 };
            yield return new object[] { true, 0, 2, 0, 0, 0, 0, 1 };
            yield return new object[] { true, 1, 2, 0, 0, 0, 0, 1 };
            yield return new object[] { true, 0, 0, 1, 0, 1, 0, 1 };
            yield return new object[] { true, 0, 0, 0, 2, 0, 2, 1 };
            yield return new object[] { true, 0, 0, 1, 2, 1, 2, 1 };
            yield return new object[] { true, 1, 2, 30, 40, 30, 40, 1 };

            yield return new object[] { false, 0, 0, 0, 0, 0, 0, 0 };
            yield return new object[] { false, -1, -2, -3, -4, 0, 0, 0 };
            yield return new object[] { false, 1, 0, 0, 0, 0, 0, 0 };
            yield return new object[] { false, 0, 2, 0, 0, 0, 0, 0 };
            yield return new object[] { false, 1, 2, 0, 0, 0, 0, 0 };
            yield return new object[] { false, 0, 0, 1, 0, 1, 0, 0 };
            yield return new object[] { false, 0, 0, 0, 2, 0, 2, 0 };
            yield return new object[] { false, 0, 0, 1, 2, 1, 2, 0 };
            yield return new object[] { false, 1, 2, 30, 40, 30, 40, 0 };
        }

        [WinFormsTheory]
        [MemberData(nameof(Bounds_SetWithHandle_TestData))]
        public void WebBrowserBase_Bounds_SetWithHandle_GetReturnsExpected(bool resizeRedraw, int x, int y, int width, int height, int expectedWidth, int expectedHeight, int expectedInvalidatedCallCount)
        {
            using var control = new SubWebBrowserBase("8856f961-340a-11d0-a96b-00c04fd705a2");
            control.SetStyle(ControlStyles.ResizeRedraw, resizeRedraw);
            int layoutCallCount = 0;
            int resizeCallCount = 0;
            int sizeChangedCallCount = 0;
            int clientSizeChangedCallCount = 0;
            ((Control)control).Layout += (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(control, e.AffectedControl);
                Assert.Equal("Bounds", e.AffectedProperty);
                Assert.Equal(resizeCallCount, layoutCallCount);
                Assert.Equal(sizeChangedCallCount, layoutCallCount);
                Assert.Equal(clientSizeChangedCallCount, layoutCallCount);
                layoutCallCount++;
            };
            control.Resize += (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(EventArgs.Empty, e);
                Assert.Equal(layoutCallCount - 1, resizeCallCount);
                Assert.Equal(sizeChangedCallCount, resizeCallCount);
                Assert.Equal(clientSizeChangedCallCount, resizeCallCount);
                resizeCallCount++;
            };
            control.SizeChanged += (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(EventArgs.Empty, e);
                Assert.Equal(resizeCallCount - 1, sizeChangedCallCount);
                Assert.Equal(layoutCallCount - 1, sizeChangedCallCount);
                Assert.Equal(clientSizeChangedCallCount, sizeChangedCallCount);
                sizeChangedCallCount++;
            };
            control.ClientSizeChanged += (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(EventArgs.Empty, e);
                Assert.Equal(resizeCallCount - 1, clientSizeChangedCallCount);
                Assert.Equal(layoutCallCount - 1, clientSizeChangedCallCount);
                Assert.Equal(sizeChangedCallCount - 1, clientSizeChangedCallCount);
                clientSizeChangedCallCount++;
            };
            Assert.NotEqual(IntPtr.Zero, control.Handle);
            int invalidatedCallCount = 0;
            control.Invalidated += (sender, e) => invalidatedCallCount++;
            int styleChangedCallCount = 0;
            ((Control)control).StyleChanged += (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            control.HandleCreated += (sender, e) => createdCallCount++;

            control.Bounds = new Rectangle(x, y, width, height);
            Assert.Equal(new Size(expectedWidth, expectedHeight), control.ClientSize);
            Assert.Equal(new Rectangle(0, 0, expectedWidth, expectedHeight), control.ClientRectangle);
            Assert.Equal(new Rectangle(0, 0, expectedWidth, expectedHeight), control.DisplayRectangle);
            Assert.Equal(new Size(expectedWidth, expectedHeight), control.Size);
            Assert.Equal(x, control.Left);
            Assert.Equal(x + expectedWidth, control.Right);
            Assert.Equal(y, control.Top);
            Assert.Equal(y + expectedHeight, control.Bottom);
            Assert.Equal(expectedWidth, control.Width);
            Assert.Equal(expectedHeight, control.Height);
            Assert.Equal(new Rectangle(x, y, expectedWidth, expectedHeight), control.Bounds);
            Assert.Equal(1, layoutCallCount);
            Assert.Equal(1, resizeCallCount);
            Assert.Equal(1, sizeChangedCallCount);
            Assert.Equal(1, clientSizeChangedCallCount);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(expectedInvalidatedCallCount, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Call again.
            control.Bounds = new Rectangle(x, y, width, height);
            Assert.Equal(new Size(expectedWidth, expectedHeight), control.ClientSize);
            Assert.Equal(new Rectangle(0, 0, expectedWidth, expectedHeight), control.ClientRectangle);
            Assert.Equal(new Rectangle(0, 0, expectedWidth, expectedHeight), control.DisplayRectangle);
            Assert.Equal(new Size(expectedWidth, expectedHeight), control.Size);
            Assert.Equal(x, control.Left);
            Assert.Equal(x + expectedWidth, control.Right);
            Assert.Equal(y, control.Top);
            Assert.Equal(y + expectedHeight, control.Bottom);
            Assert.Equal(expectedWidth, control.Width);
            Assert.Equal(expectedHeight, control.Height);
            Assert.Equal(new Rectangle(x, y, expectedWidth, expectedHeight), control.Bounds);
            Assert.Equal(1, layoutCallCount);
            Assert.Equal(1, resizeCallCount);
            Assert.Equal(1, sizeChangedCallCount);
            Assert.Equal(1, clientSizeChangedCallCount);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(expectedInvalidatedCallCount, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
        }

        [WinFormsFact]
        public void WebBrowserBase_CanSelect_GetInPlaceActvie_ReturnsExpected()
        {
            using var parent = new Control();
            using var control = new SubWebBrowserBase("8856f961-340a-11d0-a96b-00c04fd705a2")
            {
                Parent = parent
            };
            Assert.True(control.CanSelect);
        }

        [WinFormsFact]
        public void WebBrowserBase_Site_Set_GetReturnsExpected()
        {
            var mockSite1 = new Mock<ISite>(MockBehavior.Strict);
            mockSite1
                .Setup(s => s.Container)
                .Returns((IContainer)null);
            mockSite1
                .Setup(s => s.GetService(typeof(AmbientProperties)))
                .Returns(null);
            using var control = new SubWebBrowserBase("8856f961-340a-11d0-a96b-00c04fd705a2")
            {
                Site = mockSite1.Object
            };
            Assert.Same(mockSite1.Object, control.Site);
            Assert.False(control.IsHandleCreated);

            // Set same.
            control.Site = mockSite1.Object;
            Assert.Same(mockSite1.Object, control.Site);
            Assert.False(control.IsHandleCreated);

            // Set another.
            var mockSite2 = new Mock<ISite>(MockBehavior.Strict);
            mockSite2
                .Setup(s => s.Container)
                .Returns((IContainer)null);
            mockSite2
                .Setup(s => s.GetService(typeof(AmbientProperties)))
                .Returns(null);
            control.Site = mockSite2.Object;
            Assert.Same(mockSite2.Object, control.Site);
            Assert.False(control.IsHandleCreated);

            // Set null.
            control.Site = null;
            Assert.Null(control.Site);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsFact]
        public void WebBrowserBase_BackColorChanged_AddRemove_ThrowsNotSupportedException()
        {
            using var control = new SubWebBrowserBase("8856f961-340a-11d0-a96b-00c04fd705a2");
            EventHandler handler = (sender, e) => { };
            Assert.Throws<NotSupportedException>(() => control.BackColorChanged += handler);
            control.BackColorChanged -= handler;
        }

        [WinFormsFact]
        public void WebBrowserBase_BackgroundImageChanged_AddRemove_ThrowsNotSupportedException()
        {
            using var control = new SubWebBrowserBase("8856f961-340a-11d0-a96b-00c04fd705a2");
            EventHandler handler = (sender, e) => { };
            Assert.Throws<NotSupportedException>(() => control.BackgroundImageChanged += handler);
            control.BackgroundImageChanged -= handler;
        }

        [WinFormsFact]
        public void WebBrowserBase_BackgroundImageLayoutChanged_AddRemove_ThrowsNotSupportedException()
        {
            using var control = new SubWebBrowserBase("8856f961-340a-11d0-a96b-00c04fd705a2");
            EventHandler handler = (sender, e) => { };
            Assert.Throws<NotSupportedException>(() => control.BackgroundImageLayoutChanged += handler);
            control.BackgroundImageLayoutChanged -= handler;
        }

        [WinFormsFact]
        public void WebBrowserBase_BindingContextChanged_AddRemove_ThrowsNotSupportedException()
        {
            using var control = new SubWebBrowserBase("8856f961-340a-11d0-a96b-00c04fd705a2");
            EventHandler handler = (sender, e) => { };
            Assert.Throws<NotSupportedException>(() => control.BindingContextChanged += handler);
            control.BindingContextChanged -= handler;
        }

        [WinFormsFact]
        public void WebBrowserBase_UICuesEventHandler_AddRemove_ThrowsNotSupportedException()
        {
            using var control = new SubWebBrowserBase("8856f961-340a-11d0-a96b-00c04fd705a2");
            UICuesEventHandler handler = (sender, e) => { };
            Assert.Throws<NotSupportedException>(() => control.ChangeUICues += handler);
            control.ChangeUICues -= handler;
        }

        [WinFormsFact]
        public void WebBrowserBase_Click_AddRemove_ThrowsNotSupportedException()
        {
            using var control = new SubWebBrowserBase("8856f961-340a-11d0-a96b-00c04fd705a2");
            EventHandler handler = (sender, e) => { };
            Assert.Throws<NotSupportedException>(() => control.Click += handler);
            control.Click -= handler;
        }

        [WinFormsFact]
        public void WebBrowserBase_CursorChanged_AddRemove_ThrowsNotSupportedException()
        {
            using var control = new SubWebBrowserBase("8856f961-340a-11d0-a96b-00c04fd705a2");
            EventHandler handler = (sender, e) => { };
            Assert.Throws<NotSupportedException>(() => control.CursorChanged += handler);
            control.CursorChanged -= handler;
        }

        [WinFormsFact]
        public void WebBrowserBase_DoubleClick_AddRemove_ThrowsNotSupportedException()
        {
            using var control = new SubWebBrowserBase("8856f961-340a-11d0-a96b-00c04fd705a2");
            EventHandler handler = (sender, e) => { };
            Assert.Throws<NotSupportedException>(() => control.DoubleClick += handler);
            control.DoubleClick -= handler;
        }

        [WinFormsFact]
        public void WebBrowserBase_DragDrop_AddRemove_ThrowsNotSupportedException()
        {
            using var control = new SubWebBrowserBase("8856f961-340a-11d0-a96b-00c04fd705a2");
            DragEventHandler handler = (sender, e) => { };
            Assert.Throws<NotSupportedException>(() => control.DragDrop += handler);
            control.DragDrop -= handler;
        }

        [WinFormsFact]
        public void WebBrowserBase_DragEnter_AddRemove_ThrowsNotSupportedException()
        {
            using var control = new SubWebBrowserBase("8856f961-340a-11d0-a96b-00c04fd705a2");
            DragEventHandler handler = (sender, e) => { };
            Assert.Throws<NotSupportedException>(() => control.DragEnter += handler);
            control.DragEnter -= handler;
        }

        [WinFormsFact]
        public void WebBrowserBase_DragLeave_AddRemove_ThrowsNotSupportedException()
        {
            using var control = new SubWebBrowserBase("8856f961-340a-11d0-a96b-00c04fd705a2");
            EventHandler handler = (sender, e) => { };
            Assert.Throws<NotSupportedException>(() => control.DragLeave += handler);
            control.DragLeave -= handler;
        }

        [WinFormsFact]
        public void WebBrowserBase_DragOver_AddRemove_ThrowsNotSupportedException()
        {
            using var control = new SubWebBrowserBase("8856f961-340a-11d0-a96b-00c04fd705a2");
            DragEventHandler handler = (sender, e) => { };
            Assert.Throws<NotSupportedException>(() => control.DragOver += handler);
            control.DragOver -= handler;
        }

        [WinFormsFact]
        public void WebBrowserBase_EnabledChanged_AddRemove_ThrowsNotSupportedException()
        {
            using var control = new SubWebBrowserBase("8856f961-340a-11d0-a96b-00c04fd705a2");
            EventHandler handler = (sender, e) => { };
            Assert.Throws<NotSupportedException>(() => control.EnabledChanged += handler);
            control.EnabledChanged -= handler;
        }

        [WinFormsFact]
        public void WebBrowserBase_Enter_AddRemove_ThrowsNotSupportedException()
        {
            using var control = new SubWebBrowserBase("8856f961-340a-11d0-a96b-00c04fd705a2");
            EventHandler handler = (sender, e) => { };
            Assert.Throws<NotSupportedException>(() => control.Enter += handler);
            control.Enter -= handler;
        }

        [WinFormsFact]
        public void WebBrowserBase_FontChanged_AddRemove_ThrowsNotSupportedException()
        {
            using var control = new SubWebBrowserBase("8856f961-340a-11d0-a96b-00c04fd705a2");
            EventHandler handler = (sender, e) => { };
            Assert.Throws<NotSupportedException>(() => control.FontChanged += handler);
            control.FontChanged -= handler;
        }

        [WinFormsFact]
        public void WebBrowserBase_ForeColorChanged_AddRemove_ThrowsNotSupportedException()
        {
            using var control = new SubWebBrowserBase("8856f961-340a-11d0-a96b-00c04fd705a2");
            EventHandler handler = (sender, e) => { };
            Assert.Throws<NotSupportedException>(() => control.ForeColorChanged += handler);
            control.ForeColorChanged -= handler;
        }

        [WinFormsFact]
        public void WebBrowserBase_GiveFeedback_AddRemove_ThrowsNotSupportedException()
        {
            using var control = new SubWebBrowserBase("8856f961-340a-11d0-a96b-00c04fd705a2");
            GiveFeedbackEventHandler handler = (sender, e) => { };
            Assert.Throws<NotSupportedException>(() => control.GiveFeedback += handler);
            control.GiveFeedback -= handler;
        }

        [WinFormsFact]
        public void WebBrowserBase_HelpRequested_AddRemove_ThrowsNotSupportedException()
        {
            using var control = new SubWebBrowserBase("8856f961-340a-11d0-a96b-00c04fd705a2");
            HelpEventHandler handler = (sender, e) => { };
            Assert.Throws<NotSupportedException>(() => control.HelpRequested += handler);
            control.HelpRequested -= handler;
        }

        [WinFormsFact]
        public void WebBrowserBase_ImeModeChanged_AddRemove_ThrowsNotSupportedException()
        {
            using var control = new SubWebBrowserBase("8856f961-340a-11d0-a96b-00c04fd705a2");
            EventHandler handler = (sender, e) => { };
            Assert.Throws<NotSupportedException>(() => control.ImeModeChanged += handler);
            control.ImeModeChanged -= handler;
        }

        [WinFormsFact]
        public void WebBrowserBase_KeyDown_AddRemove_ThrowsNotSupportedException()
        {
            using var control = new SubWebBrowserBase("8856f961-340a-11d0-a96b-00c04fd705a2");
            KeyEventHandler handler = (sender, e) => { };
            Assert.Throws<NotSupportedException>(() => control.KeyDown += handler);
            control.KeyDown -= handler;
        }

        [WinFormsFact]
        public void WebBrowserBase_KeyPress_AddRemove_ThrowsNotSupportedException()
        {
            using var control = new SubWebBrowserBase("8856f961-340a-11d0-a96b-00c04fd705a2");
            KeyPressEventHandler handler = (sender, e) => { };
            Assert.Throws<NotSupportedException>(() => control.KeyPress += handler);
            control.KeyPress -= handler;
        }

        [WinFormsFact]
        public void WebBrowserBase_KeyUp_AddRemove_ThrowsNotSupportedException()
        {
            using var control = new SubWebBrowserBase("8856f961-340a-11d0-a96b-00c04fd705a2");
            KeyEventHandler handler = (sender, e) => { };
            Assert.Throws<NotSupportedException>(() => control.KeyUp += handler);
            control.KeyUp -= handler;
        }

        [WinFormsFact]
        public void WebBrowserBase_Layout_AddRemove_ThrowsNotSupportedException()
        {
            using var control = new SubWebBrowserBase("8856f961-340a-11d0-a96b-00c04fd705a2");
            LayoutEventHandler handler = (sender, e) => { };
            Assert.Throws<NotSupportedException>(() => control.Layout += handler);
            ((Control)control).Layout -= handler;
        }

        [WinFormsFact]
        public void WebBrowserBase_Leave_AddRemove_ThrowsNotSupportedException()
        {
            using var control = new SubWebBrowserBase("8856f961-340a-11d0-a96b-00c04fd705a2");
            EventHandler handler = (sender, e) => { };
            Assert.Throws<NotSupportedException>(() => control.Leave += handler);
            control.Leave -= handler;
        }

        [WinFormsFact]
        public void WebBrowserBase_MouseCaptureChanged_AddRemove_ThrowsNotSupportedException()
        {
            using var control = new SubWebBrowserBase("8856f961-340a-11d0-a96b-00c04fd705a2");
            EventHandler handler = (sender, e) => { };
            Assert.Throws<NotSupportedException>(() => control.MouseCaptureChanged += handler);
            control.MouseCaptureChanged -= handler;
        }

        [WinFormsFact]
        public void WebBrowserBase_MouseClick_AddRemove_ThrowsNotSupportedException()
        {
            using var control = new SubWebBrowserBase("8856f961-340a-11d0-a96b-00c04fd705a2");
            MouseEventHandler handler = (sender, e) => { };
            Assert.Throws<NotSupportedException>(() => control.MouseClick += handler);
            control.MouseClick -= handler;
        }

        [WinFormsFact]
        public void WebBrowserBase_MouseDoubleClick_AddRemove_ThrowsNotSupportedException()
        {
            using var control = new SubWebBrowserBase("8856f961-340a-11d0-a96b-00c04fd705a2");
            MouseEventHandler handler = (sender, e) => { };
            Assert.Throws<NotSupportedException>(() => control.MouseDoubleClick += handler);
            control.MouseDoubleClick -= handler;
        }

        [WinFormsFact]
        public void WebBrowserBase_MouseDown_AddRemove_ThrowsNotSupportedException()
        {
            using var control = new SubWebBrowserBase("8856f961-340a-11d0-a96b-00c04fd705a2");
            MouseEventHandler handler = (sender, e) => { };
            Assert.Throws<NotSupportedException>(() => control.MouseDown += handler);
            control.MouseDown -= handler;
        }

        [WinFormsFact]
        public void WebBrowserBase_MouseEnter_AddRemove_ThrowsNotSupportedException()
        {
            using var control = new SubWebBrowserBase("8856f961-340a-11d0-a96b-00c04fd705a2");
            EventHandler handler = (sender, e) => { };
            Assert.Throws<NotSupportedException>(() => control.MouseEnter += handler);
            control.MouseEnter -= handler;
        }

        [WinFormsFact]
        public void WebBrowserBase_MouseHover_AddRemove_ThrowsNotSupportedException()
        {
            using var control = new SubWebBrowserBase("8856f961-340a-11d0-a96b-00c04fd705a2");
            EventHandler handler = (sender, e) => { };
            Assert.Throws<NotSupportedException>(() => control.MouseHover += handler);
            control.MouseHover -= handler;
        }

        [WinFormsFact]
        public void WebBrowserBase_MouseLeave_AddRemove_ThrowsNotSupportedException()
        {
            using var control = new SubWebBrowserBase("8856f961-340a-11d0-a96b-00c04fd705a2");
            EventHandler handler = (sender, e) => { };
            Assert.Throws<NotSupportedException>(() => control.MouseLeave += handler);
            control.MouseLeave -= handler;
        }

        [WinFormsFact]
        public void WebBrowserBase_MouseMove_AddRemove_ThrowsNotSupportedException()
        {
            using var control = new SubWebBrowserBase("8856f961-340a-11d0-a96b-00c04fd705a2");
            MouseEventHandler handler = (sender, e) => { };
            Assert.Throws<NotSupportedException>(() => control.MouseMove += handler);
            control.MouseMove -= handler;
        }

        [WinFormsFact]
        public void WebBrowserBase_MouseUp_AddRemove_ThrowsNotSupportedException()
        {
            using var control = new SubWebBrowserBase("8856f961-340a-11d0-a96b-00c04fd705a2");
            MouseEventHandler handler = (sender, e) => { };
            Assert.Throws<NotSupportedException>(() => control.MouseUp += handler);
            control.MouseUp -= handler;
        }

        [WinFormsFact]
        public void WebBrowserBase_MouseWheel_AddRemove_ThrowsNotSupportedException()
        {
            using var control = new SubWebBrowserBase("8856f961-340a-11d0-a96b-00c04fd705a2");
            MouseEventHandler handler = (sender, e) => { };
            Assert.Throws<NotSupportedException>(() => control.MouseWheel += handler);
            control.MouseWheel -= handler;
        }

        [WinFormsFact]
        public void WebBrowserBase_Paint_AddRemove_ThrowsNotSupportedException()
        {
            using var control = new SubWebBrowserBase("8856f961-340a-11d0-a96b-00c04fd705a2");
            PaintEventHandler handler = (sender, e) => { };
            Assert.Throws<NotSupportedException>(() => control.Paint += handler);
            control.Paint -= handler;
        }

        [WinFormsFact]
        public void WebBrowserBase_QueryAccessibilityHelp__AddRemove_ThrowsNotSupportedException()
        {
            using var control = new SubWebBrowserBase("8856f961-340a-11d0-a96b-00c04fd705a2");
            QueryAccessibilityHelpEventHandler handler = (sender, e) => { };
            Assert.Throws<NotSupportedException>(() => control.QueryAccessibilityHelp += handler);
            control.QueryAccessibilityHelp -= handler;
        }

        [WinFormsFact]
        public void WebBrowserBase_QueryContinueDrag__AddRemove_ThrowsNotSupportedException()
        {
            using var control = new SubWebBrowserBase("8856f961-340a-11d0-a96b-00c04fd705a2");
            QueryContinueDragEventHandler handler = (sender, e) => { };
            Assert.Throws<NotSupportedException>(() => control.QueryContinueDrag += handler);
            control.QueryContinueDrag -= handler;
        }

        [WinFormsFact]
        public void WebBrowserBase_RightToLeftChanged_AddRemove_ThrowsNotSupportedException()
        {
            using var control = new SubWebBrowserBase("8856f961-340a-11d0-a96b-00c04fd705a2");
            EventHandler handler = (sender, e) => { };
            Assert.Throws<NotSupportedException>(() => control.RightToLeftChanged += handler);
            control.RightToLeftChanged -= handler;
        }

        [WinFormsFact]
        public void WebBrowserBase_StyleChanged_AddRemove_ThrowsNotSupportedException()
        {
            using var control = new SubWebBrowserBase("8856f961-340a-11d0-a96b-00c04fd705a2");
            EventHandler handler = (sender, e) => { };
            Assert.Throws<NotSupportedException>(() => control.StyleChanged += handler);
            control.StyleChanged -= handler;
        }

        [WinFormsFact]
        public void WebBrowserBase_TextChanged_AddRemove_ThrowsNotSupportedException()
        {
            using var control = new SubWebBrowserBase("8856f961-340a-11d0-a96b-00c04fd705a2");
            EventHandler handler = (sender, e) => { };
            Assert.Throws<NotSupportedException>(() => control.TextChanged += handler);
            control.TextChanged -= handler;
        }

        [WinFormsFact]
        public void WebBrowserBase_CreateWebBrowserSiteBase_Invoke_ReturnsExpected()
        {
            using var control = new SubWebBrowserBase("8856f961-340a-11d0-a96b-00c04fd705a2");
            WebBrowserSiteBase siteBase = control.CreateWebBrowserSiteBase();
            Assert.NotNull(siteBase);
            Assert.NotSame(siteBase, control.CreateWebBrowserSiteBase());
        }
        public static IEnumerable<object[]> DrawToBitmap_TestData()
        {
            yield return new object[] { new Rectangle(0, 0, 1, 1) };
            yield return new object[] { new Rectangle(0, 0, 10, 10) };
            yield return new object[] { new Rectangle(2, 3, 10, 15) };
            yield return new object[] { new Rectangle(2, 3, 15, 10) };
            yield return new object[] { new Rectangle(0, 0, 100, 150) };
        }

        [WinFormsTheory]
        [MemberData(nameof(DrawToBitmap_TestData))]
        public void WebBrowserBase_DrawToBitmap_Invoke_Success(Rectangle targetBounds)
        {
            using var control = new SubWebBrowserBase("8856f961-340a-11d0-a96b-00c04fd705a2")
            {
                Width = 20,
                Height = 20,
            };
            using var bitmap = new Bitmap(20, 20);
            control.DrawToBitmap(bitmap, targetBounds);
            Assert.True(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [MemberData(nameof(DrawToBitmap_TestData))]
        public void WebBrowserBase_DrawToBitmap_InvokeWithHandle_Success(Rectangle rectangle)
        {
            using var control = new SubWebBrowserBase("8856f961-340a-11d0-a96b-00c04fd705a2")
            {
                Width = 20,
                Height = 20,
            };
            Assert.NotEqual(IntPtr.Zero, control.Handle);
            int invalidatedCallCount = 0;
            control.Invalidated += (sender, e) => invalidatedCallCount++;
            int styleChangedCallCount = 0;
            ((Control)control).StyleChanged += (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            control.HandleCreated += (sender, e) => createdCallCount++;

            using var bitmap = new Bitmap(20, 20);
            control.DrawToBitmap(bitmap, rectangle);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
        }

        [WinFormsFact]
        public void WebBrowserBase_DrawToBitmap_NullBitmap_ThrowsArgumentNullException()
        {
            using var control = new SubWebBrowserBase("8856f961-340a-11d0-a96b-00c04fd705a2");
            Assert.Throws<ArgumentNullException>("bitmap", () => control.DrawToBitmap(null, new Rectangle(1, 2, 3, 4)));
        }

        [WinFormsTheory]
        [InlineData(-1, 0, 1, 2)]
        [InlineData(0, -1, 1, 2)]
        [InlineData(0, 0, -1, 2)]
        [InlineData(0, 0, 0, 2)]
        [InlineData(0, 0, 1, -1)]
        [InlineData(0, 0, 1, 0)]
        public void WebBrowserBase_DrawToBitmap_InvalidTargetBounds_ThrowsArgumentException(int x, int y, int width, int height)
        {
            using var control = new SubWebBrowserBase("8856f961-340a-11d0-a96b-00c04fd705a2")
            {
                Width = 20,
                Height = 20
            };
            using var bitmap = new Bitmap(10, 10);
            Assert.Throws<ArgumentException>(null, () => control.DrawToBitmap(bitmap, new Rectangle(x, y, width, height)));
        }

        [WinFormsTheory]
        [InlineData(-1)]
        [InlineData(0)]
        public void WebBrowserBase_DrawToBitmap_InvokeZeroWidth_ThrowsArgumentException(int width)
        {
            using var control = new SubWebBrowserBase("8856f961-340a-11d0-a96b-00c04fd705a2")
            {
                Width = width,
                Height = 20
            };
            using var bitmap = new Bitmap(10, 10);
            Assert.Throws<ArgumentException>(null, () => control.DrawToBitmap(bitmap, new Rectangle(1, 2, 3, 4)));
        }

        [WinFormsTheory]
        [InlineData(-1)]
        [InlineData(0)]
        public void WebBrowserBase_DrawToBitmap_InvokeZeroHeight_ThrowsArgumentException(int height)
        {
            using var control = new SubWebBrowserBase("8856f961-340a-11d0-a96b-00c04fd705a2")
            {
                Width = 20,
                Height = height
            };
            using var bitmap = new Bitmap(10, 10);
            Assert.Throws<ArgumentException>(null, () => control.DrawToBitmap(bitmap, new Rectangle(1, 2, 3, 4)));
        }

        [WinFormsFact]
        public void WebBrowserBase_GetAutoSizeMode_Invoke_ReturnsExpected()
        {
            using var control = new SubWebBrowserBase("8856f961-340a-11d0-a96b-00c04fd705a2");
            Assert.Equal(AutoSizeMode.GrowOnly, control.GetAutoSizeMode());
        }

        [WinFormsTheory]
        [InlineData(ControlStyles.ContainerControl, false)]
        [InlineData(ControlStyles.UserPaint, false)]
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
        [InlineData((ControlStyles)0, true)]
        [InlineData((ControlStyles)int.MaxValue, false)]
        [InlineData((ControlStyles)(-1), false)]
        public void WebBrowserBase_GetStyle_Invoke_ReturnsExpected(ControlStyles flag, bool expected)
        {
            using var control = new SubWebBrowserBase("8856f961-340a-11d0-a96b-00c04fd705a2");
            Assert.Equal(expected, control.GetStyle(flag));

            // Call again to test caching.
            Assert.Equal(expected, control.GetStyle(flag));
        }

        [WinFormsFact]
        public void WebBrowserBase_GetTopLevel_Invoke_ReturnsExpected()
        {
            using var control = new SubWebBrowserBase("8856f961-340a-11d0-a96b-00c04fd705a2");
            Assert.False(control.GetTopLevel());
        }

        [WinFormsTheory]
        [InlineData(Keys.A)]
        public void WebBrowserBase_ProcessDialogKey_InvokeWithoutParent_ReturnsFalse(Keys keyData)
        {
            using var control = new SubWebBrowserBase("8856f961-340a-11d0-a96b-00c04fd705a2");
            Assert.False(control.ProcessDialogKey(keyData));
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [InlineData(Keys.A)]
        public void WebBrowserBase_ProcessDialogKey_InvokeWithParent_ReturnsFalse(Keys keyData)
        {
            using var parent = new Control
            {
                Visible = false
            };
            using var control = new SubWebBrowserBase("8856f961-340a-11d0-a96b-00c04fd705a2")
            {
                Parent = parent
            };
            Assert.False(control.ProcessDialogKey(keyData));
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [InlineData(Keys.A, true)]
        [InlineData(Keys.A, false)]
        public void WebBrowserBase_ProcessDialogKey_InvokeWithCustomParent_ReturnsExpected(Keys keyData, bool result)
        {
            int callCount = 0;
            bool action(Keys actualKeyData)
            {
                Assert.Equal(keyData, actualKeyData);
                callCount++;
                return result;
            }
            using var parent = new CustomProcessControl
            {
                ProcessDialogKeyAction = action,
                Visible = false
            };
            using var control = new SubWebBrowserBase("8856f961-340a-11d0-a96b-00c04fd705a2")
            {
                Parent = parent
            };
            Assert.Equal(result, control.ProcessDialogKey(keyData));
            Assert.Equal(1, callCount);
            Assert.False(control.IsHandleCreated);
        }

        private class CustomProcessControl : Control
        {
            public Func<Message, Keys, bool> ProcessCmdKeyAction { get; set; }

            protected override bool ProcessCmdKey(ref Message msg, Keys keyData) => ProcessCmdKeyAction(msg, keyData);

            public Func<char, bool> ProcessDialogCharAction { get; set; }

            protected override bool ProcessDialogChar(char charCode) => ProcessDialogCharAction(charCode);

            public Func<Keys, bool> ProcessDialogKeyAction { get; set; }

            protected override bool ProcessDialogKey(Keys keyData) => ProcessDialogKeyAction(keyData);

            public Func<Message, bool> ProcessKeyPreviewAction { get; set; }

            protected override bool ProcessKeyPreview(ref Message m) => ProcessKeyPreviewAction(m);
        }

        private class SubWebBrowserBase : WebBrowserBase
        {
            public SubWebBrowserBase(string clsidString) : base(clsidString)
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

            public new void AttachInterfaces(object nativeActiveXObject) => base.AttachInterfaces(nativeActiveXObject);

            public new void CreateSink() => base.CreateSink();

            public new void DetachInterfaces() => base.DetachInterfaces();

            public new void DetachSink() => base.DetachSink();

            public new WebBrowserSiteBase CreateWebBrowserSiteBase() => base.CreateWebBrowserSiteBase();

            public new AutoSizeMode GetAutoSizeMode() => base.GetAutoSizeMode();

            public new bool GetStyle(ControlStyles flag) => base.GetStyle(flag);

            public new bool GetTopLevel() => base.GetTopLevel();

            public new bool ProcessDialogKey(Keys keyData) => base.ProcessDialogKey(keyData);

            public new void SetStyle(ControlStyles flag, bool value) => base.SetStyle(flag, value);
        }
    }
}
