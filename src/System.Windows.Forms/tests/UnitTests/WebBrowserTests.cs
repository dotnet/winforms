// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel;
using System.Drawing;
using WinForms.Common.Tests;
using Xunit;

namespace System.Windows.Forms.Tests
{
    using Point = System.Drawing.Point;
    using Size = System.Drawing.Size;

    public class WebBrowserTests
    {
        [WinFormsFact]
        public void WebBrowser_Ctor()
        {
            using var control = new SubWebBrowser();
            Assert.Null(control.ActiveXInstance);
            Assert.False(control.AllowDrop);
            Assert.True(control.AllowNavigation);
            Assert.Equal(AnchorStyles.Top | AnchorStyles.Left, control.Anchor);
            Assert.False(control.AutoSize);
            Assert.Equal(Control.DefaultBackColor, control.BackColor);
            Assert.Null(control.BackgroundImage);
            Assert.Equal(ImageLayout.Tile, control.BackgroundImageLayout);
            Assert.Null(control.BindingContext);
            Assert.Equal(250, control.Bottom);
            Assert.Equal(new Rectangle(0, 0, 250, 250), control.Bounds);
            Assert.True(control.CanEnableIme);
            Assert.True(control.CanRaiseEvents);
            Assert.False(control.CanSelect);
            Assert.True(control.CausesValidation);
            Assert.Equal(new Size(250, 250), control.ClientSize);
            Assert.Equal(new Rectangle(0, 0, 250, 250), control.ClientRectangle);
            Assert.Null(control.Container);
            Assert.Null(control.ContextMenuStrip);
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
            Assert.Equal(new Size(250, 250), control.DefaultSize);
            Assert.False(control.DesignMode);
            Assert.Equal(new Rectangle(0, 0, 250, 250), control.DisplayRectangle);
            Assert.Equal(DockStyle.None, control.Dock);
            Assert.False(control.DoubleBuffered);
            Assert.True(control.Enabled);
            Assert.NotNull(control.Events);
            Assert.Same(control.Events, control.Events);
            Assert.Equal(Control.DefaultFont, control.Font);
            Assert.Equal(control.Font.Height, control.FontHeight);
            Assert.Equal(Control.DefaultForeColor, control.ForeColor);
            Assert.False(control.HasChildren);
            Assert.Equal(250, control.Height);
            Assert.Equal(ImeMode.NoControl, control.ImeMode);
            Assert.Equal(ImeMode.NoControl, control.ImeModeBase);
            Assert.NotNull(control.LayoutEngine);
            Assert.Same(control.LayoutEngine, control.LayoutEngine);
            Assert.Equal(0, control.Left);
            Assert.Equal(Point.Empty, control.Location);
            Assert.Equal(new Padding(3), control.Margin);
            Assert.Equal(Size.Empty, control.MaximumSize);
            Assert.Equal(Size.Empty, control.MinimumSize);
            Assert.Equal(Padding.Empty, control.Padding);
            Assert.Null(control.Parent);
            Assert.Equal("Microsoft\u00AE .NET", control.ProductName);
            Assert.Equal(new Size(250, 250), control.PreferredSize);
            Assert.False(control.RecreatingHandle);
            Assert.Null(control.Region);
            Assert.False(control.ResizeRedraw);
            Assert.Equal(250, control.Right);
            Assert.Equal(RightToLeft.No, control.RightToLeft);
            Assert.Null(control.Site);
            Assert.Equal(new Size(250, 250), control.Size);
            Assert.Equal(0, control.TabIndex);
            Assert.True(control.TabStop);
            Assert.Empty(control.Text);
            Assert.Equal(0, control.Top);
            Assert.Null(control.TopLevelControl);
            Assert.True(control.Visible);
            Assert.Equal(250, control.Width);

            Assert.False(control.IsHandleCreated);
        }

        [WinFormsFact]
        public void WebBrowser_ActiveXInstance_GetWithHandle_ReturnsNull()
        {
            using var control = new WebBrowser();
            Assert.NotEqual(IntPtr.Zero, control.Handle);
            Assert.Null(control.ActiveXInstance);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetBoolTheoryData))]
        public void WebBrowser_AllowNavigation_Set_GetReturnsExpected(bool value)
        {
            using var control = new WebBrowser
            {
                AllowNavigation = value
            };
            Assert.Equal(value, control.AllowNavigation);
            Assert.Null(control.ActiveXInstance);
            Assert.False(control.IsHandleCreated);

            // Set same.
            control.AllowNavigation = value;
            Assert.Equal(value, control.AllowNavigation);
            Assert.Null(control.ActiveXInstance);
            Assert.False(control.IsHandleCreated);

            // Set different.
            control.AllowNavigation = !value;
            Assert.Equal(!value, control.AllowNavigation);
            Assert.Null(control.ActiveXInstance);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetBoolTheoryData))]
        public void WebBrowser_AllowNavigation_SetWithInstance_GetReturnsExpected(bool value)
        {
            var parent = new Control();
            using var control = new WebBrowser
            {
                Parent = parent
            };

            control.AllowNavigation = value;
            Assert.Equal(value, control.AllowNavigation);
            Assert.NotNull(control.ActiveXInstance);
            Assert.True(control.IsHandleCreated);

            // Set same.
            control.AllowNavigation = value;
            Assert.Equal(value, control.AllowNavigation);
            Assert.NotNull(control.ActiveXInstance);
            Assert.True(control.IsHandleCreated);

            // Set different.
            control.AllowNavigation = !value;
            Assert.Equal(!value, control.AllowNavigation);
            Assert.NotNull(control.ActiveXInstance);
            Assert.True(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetBoolTheoryData))]
        public void WebBrowser_AllowNavigation_SetWithSink_GetReturnsExpected(bool value)
        {
            var parent = new Control();
            using var control = new SubWebBrowser
            {
                Parent = parent
            };
            control.CreateSink();

            control.AllowNavigation = value;
            Assert.Equal(value, control.AllowNavigation);
            Assert.NotNull(control.ActiveXInstance);
            Assert.True(control.IsHandleCreated);

            // Set same.
            control.AllowNavigation = value;
            Assert.Equal(value, control.AllowNavigation);
            Assert.NotNull(control.ActiveXInstance);
            Assert.True(control.IsHandleCreated);

            // Set different.
            control.AllowNavigation = !value;
            Assert.Equal(!value, control.AllowNavigation);
            Assert.NotNull(control.ActiveXInstance);
            Assert.True(control.IsHandleCreated);
        }

        [WinFormsFact]
        public void WebBrowser_Parent_SetNonNull_AddsToControls()
        {
            var parent = new WebBrowser();
            using var control = new WebBrowser
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

        [WinFormsFact]
        public void WebBrowser_Parent_SetParent_GetReturnsExpected()
        {
            var parent = new Control();
            using var control = new WebBrowser
            {
                Parent = parent
            };
            Assert.Same(parent, control.Parent);
            Assert.NotNull(control.ActiveXInstance);
            Assert.True(control.IsHandleCreated);

            // Set null.
            control.Parent = null;
            Assert.Null(control.Parent);
            Assert.NotNull(control.ActiveXInstance);
            Assert.True(control.IsHandleCreated);
        }

        [WinFormsFact]
        public void WebBrowser_Parent_SetParentNotVisible_GetReturnsExpected()
        {
            var parent = new Control
            {
                Visible = false
            };
            using var control = new WebBrowser
            {
                Parent = parent
            };
            Assert.Same(parent, control.Parent);
            Assert.Null(control.ActiveXInstance);
            Assert.False(control.IsHandleCreated);

            // Set null.
            control.Parent = null;
            Assert.Null(control.Parent);
            Assert.Null(control.ActiveXInstance);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsFact]
        public void WebBrowser_Parent_SetChildNotVisible_GetReturnsExpected()
        {
            var parent = new Control();
            using var control = new WebBrowser
            {
                Visible = false,
                Parent = parent
            };
            Assert.Same(parent, control.Parent);
            Assert.Null(control.ActiveXInstance);
            Assert.False(control.IsHandleCreated);

            // Set null.
            control.Parent = null;
            Assert.Null(control.Parent);
            Assert.Null(control.ActiveXInstance);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsFact]
        public void WebBrowser_Parent_SetWithHandle_GetReturnsExpected()
        {
            var parent = new Control();
            using var control = new WebBrowser();
            Assert.NotEqual(IntPtr.Zero, control.Handle);

            control.Parent = parent;
            Assert.Same(parent, control.Parent);
            Assert.NotNull(control.ActiveXInstance);
            Assert.True(control.IsHandleCreated);

            // Set null.
            control.Parent = null;
            Assert.Null(control.Parent);
            Assert.NotNull(control.ActiveXInstance);
            Assert.True(control.IsHandleCreated);
        }

        [WinFormsFact]
        public void WebBrowser_Parent_SetWithHandleParentNotVisible_GetReturnsExpected()
        {
            var parent = new Control
            {
                Visible = false
            };
            using var control = new WebBrowser();
            Assert.NotEqual(IntPtr.Zero, control.Handle);

            control.Parent = parent;
            Assert.Same(parent, control.Parent);
            Assert.NotNull(control.ActiveXInstance);
            Assert.True(control.IsHandleCreated);

            // Set null.
            control.Parent = null;
            Assert.Null(control.Parent);
            Assert.NotNull(control.ActiveXInstance);
            Assert.True(control.IsHandleCreated);
        }

        [WinFormsFact]
        public void WebBrowser_Parent_SetWithHandleChildNotVisible_GetReturnsExpected()
        {
            var parent = new Control();
            Assert.NotEqual(IntPtr.Zero, parent.Handle);

            using var control = new WebBrowser
            {
                Visible = false,
                Parent = parent
            };
            Assert.Same(parent, control.Parent);
            Assert.Null(control.ActiveXInstance);
            Assert.False(control.IsHandleCreated);

            // Set null.
            control.Parent = null;
            Assert.Null(control.Parent);
            Assert.Null(control.ActiveXInstance);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsFact]
        public void WebBrowser_Parent_SetWithHandler_CallsParentChanged()
        {
            var parent = new WebBrowser();
            using var control = new WebBrowser();
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

        [WinFormsFact]
        public void WebBrowser_Parent_SetSame_ThrowsArgumentException()
        {
            using var control = new WebBrowser();
            Assert.Throws<ArgumentException>(null, () => control.Parent = control);
            Assert.Null(control.Parent);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetBoolTheoryData))]
        public void WebBrowser_Visible_Set_GetReturnsExpected(bool value)
        {
            using var control = new WebBrowser
            {
                Visible = value
            };
            Assert.Equal(value, control.Visible);

            // Set same.
            control.Visible = value;
            Assert.Equal(value, control.Visible);

            // Set different.
            control.Visible = !value;
            Assert.Equal(!value, control.Visible);
        }

        [WinFormsFact]
        public void WebBrowser_Visible_SetTrue_GetReturnsExpected()
        {
            using var control = new WebBrowser
            {
                Visible = false
            };
            Assert.False(control.Visible);
            Assert.Null(control.ActiveXInstance);
            Assert.False(control.IsHandleCreated);

            control.Visible = true;
            Assert.True(control.Visible);
            Assert.NotNull(control.ActiveXInstance);
            Assert.True(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetBoolTheoryData))]
        public void WebBrowser_Visible_SetWithHandle_GetReturnsExpected(bool value)
        {
            using var control = new WebBrowser();
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

        [WinFormsFact]
        public void WebBrowser_Visible_SetTrueWithHandle_GetReturnsExpected()
        {
            using var control = new WebBrowser();
            Assert.NotEqual(IntPtr.Zero, control.Handle);

            control.Visible = false;
            Assert.False(control.Visible);
            Assert.Null(control.ActiveXInstance);
            Assert.True(control.IsHandleCreated);

            // Set same.
            control.Visible = true;
            Assert.True(control.Visible);
            Assert.NotNull(control.ActiveXInstance);
            Assert.True(control.IsHandleCreated);
        }

        [WinFormsFact]
        public void WebBrowser_Visible_SetWithHandler_CallsVisibleChanged()
        {
            using var control = new WebBrowser
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

        [WinFormsFact]
        public void WebBrowser_AttachInterfaces_Invoke_Success()
        {
            Type t = Type.GetTypeFromCLSID(new Guid("0002DF01-0000-0000-C000-000000000046"));
            var nativeActiveXObject = Activator.CreateInstance(t);
            using var control = new SubWebBrowser();
            control.AttachInterfaces(nativeActiveXObject);

            // Attach again.
            control.AttachInterfaces(nativeActiveXObject);
            control.DetachInterfaces();

            // Attach null.
            control.AttachInterfaces(null);
        }

        [WinFormsFact]
        public void WebBrowser_AttachInterfaces_InvalidNativeActiveXObject_ThrowsInvalidCastException()
        {
            using var control = new SubWebBrowser();
            Assert.Throws<InvalidCastException>(() => control.AttachInterfaces(new object()));
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetBoolTheoryData))]
        public void WebBrowser_CreateSink_InvokeWithInstance_Success(bool allowNavigation)
        {
            using var parent = new Control();
            using var control = new SubWebBrowser
            {
                AllowNavigation = allowNavigation,
                Parent = parent
            };
            control.CreateSink();
            Assert.NotNull(control.ActiveXInstance);
            Assert.True(control.IsHandleCreated);

            control.CreateSink();
            Assert.NotNull(control.ActiveXInstance);
            Assert.True(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetBoolTheoryData))]
        public void WebBrowser_CreateSink_InvokeWithoutInstance_Nop(bool allowNavigation)
        {
            using var control = new SubWebBrowser
            {
                AllowNavigation = allowNavigation
            };
            control.CreateSink();
            Assert.Null(control.ActiveXInstance);
            Assert.False(control.IsHandleCreated);

            control.CreateSink();
            Assert.Null(control.ActiveXInstance);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsFact]
        public void WebBrowser_CreateWebBrowserSiteBase_Invoke_ReturnsExpected()
        {
            using var control = new SubWebBrowser();
            WebBrowserSiteBase siteBase = control.CreateWebBrowserSiteBase();
            Assert.NotNull(siteBase);
            Assert.NotSame(siteBase, control.CreateWebBrowserSiteBase());
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetBoolTheoryData))]
        public void WebBrowser_DetachSink_InvokeWithCreatedSink_Success(bool allowNavigation)
        {
            var parent = new Control();
            using var control = new SubWebBrowser
            {
                AllowNavigation = allowNavigation,
                Parent = parent
            };
            control.CreateSink();
            Assert.NotNull(control.ActiveXInstance);
            Assert.True(control.IsHandleCreated);

            control.DetachSink();
            Assert.NotNull(control.ActiveXInstance);
            Assert.True(control.IsHandleCreated);

            // Call again.
            control.DetachSink();
            Assert.NotNull(control.ActiveXInstance);
            Assert.True(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetBoolTheoryData))]
        public void WebBrowser_DetachSink_InvokeWithInstance_Success(bool allowNavigation)
        {
            var parent = new Control();
            using var control = new SubWebBrowser
            {
                AllowNavigation = allowNavigation,
                Parent = parent
            };

            control.DetachSink();
            Assert.NotNull(control.ActiveXInstance);
            Assert.True(control.IsHandleCreated);

            // Call again.
            control.DetachSink();
            Assert.NotNull(control.ActiveXInstance);
            Assert.True(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetBoolTheoryData))]
        public void WebBrowser_DetachSink_InvokeWithoutInstance_Nop(bool allowNavigation)
        {
            using var control = new SubWebBrowser
            {
                AllowNavigation = allowNavigation
            };
            control.DetachSink();
            Assert.Null(control.ActiveXInstance);
            Assert.False(control.IsHandleCreated);

            // Call again.
            control.DetachSink();
            Assert.Null(control.ActiveXInstance);
            Assert.False(control.IsHandleCreated);
        }

        private class SubWebBrowser : WebBrowser
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

            public new WebBrowserSiteBase CreateWebBrowserSiteBase() => base.CreateWebBrowserSiteBase();

            public new void DetachInterfaces() => base.DetachInterfaces();

            public new void DetachSink() => base.DetachSink();

            public new AutoSizeMode GetAutoSizeMode() => base.GetAutoSizeMode();

            public new bool GetStyle(ControlStyles flag) => base.GetStyle(flag);
        }
    }
}
