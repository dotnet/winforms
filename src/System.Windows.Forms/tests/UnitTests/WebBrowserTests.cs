// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using WinForms.Common.Tests;
using Xunit;

namespace System.Windows.Forms.Tests
{
    public class WebBrowserTests
    {
        [StaFact]
        public void WebBrowser_Constructor()
        {
            var browser = new WebBrowser();
            Assert.Null(browser.ActiveXInstance);
            Assert.True(browser.AllowNavigation);

            Assert.False(browser.IsHandleCreated);
        }

        [StaFact]
        public void WebBrowser_ActiveXInstance_GetWithHandle_ReturnsNull()
        {
            var browser = new WebBrowser();
            Assert.NotEqual(IntPtr.Zero, browser.Handle);
            Assert.Null(browser.ActiveXInstance);
        }

        [StaTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetBoolTheoryData))]
        public void WebBrowser_AllowNavigation_Set_GetReturnsExpected(bool value)
        {
            var browser = new WebBrowser
            {
                AllowNavigation = value
            };
            Assert.Equal(value, browser.AllowNavigation);
            Assert.Null(browser.ActiveXInstance);
            Assert.False(browser.IsHandleCreated);

            // Set same.
            browser.AllowNavigation = value;
            Assert.Equal(value, browser.AllowNavigation);
            Assert.Null(browser.ActiveXInstance);
            Assert.False(browser.IsHandleCreated);

            // Set different.
            browser.AllowNavigation = !value;
            Assert.Equal(!value, browser.AllowNavigation);
            Assert.Null(browser.ActiveXInstance);
            Assert.False(browser.IsHandleCreated);
        }

        [StaTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetBoolTheoryData))]
        public void WebBrowser_AllowNavigation_SetWithInstance_GetReturnsExpected(bool value)
        {
            var parent = new Control();
            var browser = new WebBrowser
            {
                Parent = parent
            };

            browser.AllowNavigation = value;
            Assert.Equal(value, browser.AllowNavigation);
            Assert.NotNull(browser.ActiveXInstance);
            Assert.True(browser.IsHandleCreated);

            // Set same.
            browser.AllowNavigation = value;
            Assert.Equal(value, browser.AllowNavigation);
            Assert.NotNull(browser.ActiveXInstance);
            Assert.True(browser.IsHandleCreated);

            // Set different.
            browser.AllowNavigation = !value;
            Assert.Equal(!value, browser.AllowNavigation);
            Assert.NotNull(browser.ActiveXInstance);
            Assert.True(browser.IsHandleCreated);
        }

        [StaTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetBoolTheoryData))]
        public void WebBrowser_AllowNavigation_SetWithSink_GetReturnsExpected(bool value)
        {
            var parent = new Control();
            var browser = new SubWebBrowser
            {
                Parent = parent
            };
            browser.CreateSink();

            browser.AllowNavigation = value;
            Assert.Equal(value, browser.AllowNavigation);
            Assert.NotNull(browser.ActiveXInstance);
            Assert.True(browser.IsHandleCreated);

            // Set same.
            browser.AllowNavigation = value;
            Assert.Equal(value, browser.AllowNavigation);
            Assert.NotNull(browser.ActiveXInstance);
            Assert.True(browser.IsHandleCreated);

            // Set different.
            browser.AllowNavigation = !value;
            Assert.Equal(!value, browser.AllowNavigation);
            Assert.NotNull(browser.ActiveXInstance);
            Assert.True(browser.IsHandleCreated);
        }

        [StaFact]
        public void WebBrowser_Parent_SetNonNull_AddsToControls()
        {
            var parent = new WebBrowser();
            var control = new WebBrowser
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

        [StaFact]
        public void WebBrowser_Parent_SetParent_GetReturnsExpected()
        {
            var parent = new Control();
            var browser = new WebBrowser
            {
                Parent = parent
            };
            Assert.Same(parent, browser.Parent);
            Assert.NotNull(browser.ActiveXInstance);
            Assert.True(browser.IsHandleCreated);

            // Set null.
            browser.Parent = null;
            Assert.Null(browser.Parent);
            Assert.NotNull(browser.ActiveXInstance);
            Assert.True(browser.IsHandleCreated);
        }

        [StaFact]
        public void WebBrowser_Parent_SetParentNotVisible_GetReturnsExpected()
        {
            var parent = new Control
            {
                Visible = false
            };
            var browser = new WebBrowser
            {
                Parent = parent
            };
            Assert.Same(parent, browser.Parent);
            Assert.Null(browser.ActiveXInstance);
            Assert.False(browser.IsHandleCreated);

            // Set null.
            browser.Parent = null;
            Assert.Null(browser.Parent);
            Assert.Null(browser.ActiveXInstance);
            Assert.False(browser.IsHandleCreated);
        }

        [StaFact]
        public void WebBrowser_Parent_SetChildNotVisible_GetReturnsExpected()
        {
            var parent = new Control();
            var browser = new WebBrowser
            {
                Visible = false,
                Parent = parent
            };
            Assert.Same(parent, browser.Parent);
            Assert.Null(browser.ActiveXInstance);
            Assert.False(browser.IsHandleCreated);

            // Set null.
            browser.Parent = null;
            Assert.Null(browser.Parent);
            Assert.Null(browser.ActiveXInstance);
            Assert.False(browser.IsHandleCreated);
        }

        [StaFact]
        public void WebBrowser_Parent_SetWithHandle_GetReturnsExpected()
        {
            var parent = new Control();
            var browser = new WebBrowser();
            Assert.NotEqual(IntPtr.Zero, browser.Handle);

            browser.Parent = parent;
            Assert.Same(parent, browser.Parent);
            Assert.NotNull(browser.ActiveXInstance);
            Assert.True(browser.IsHandleCreated);

            // Set null.
            browser.Parent = null;
            Assert.Null(browser.Parent);
            Assert.NotNull(browser.ActiveXInstance);
            Assert.True(browser.IsHandleCreated);
        }

        [StaFact]
        public void WebBrowser_Parent_SetWithHandleParentNotVisible_GetReturnsExpected()
        {
            var parent = new Control
            {
                Visible = false
            };
            var browser = new WebBrowser();
            Assert.NotEqual(IntPtr.Zero, browser.Handle);

            browser.Parent = parent;
            Assert.Same(parent, browser.Parent);
            Assert.NotNull(browser.ActiveXInstance);
            Assert.True(browser.IsHandleCreated);

            // Set null.
            browser.Parent = null;
            Assert.Null(browser.Parent);
            Assert.NotNull(browser.ActiveXInstance);
            Assert.True(browser.IsHandleCreated);
        }

        [StaFact]
        public void WebBrowser_Parent_SetWithHandleChildNotVisible_GetReturnsExpected()
        {
            var parent = new Control();
            Assert.NotEqual(IntPtr.Zero, parent.Handle);

            var browser = new WebBrowser
            {
                Visible = false,
                Parent = parent
            };
            Assert.Same(parent, browser.Parent);
            Assert.Null(browser.ActiveXInstance);
            Assert.False(browser.IsHandleCreated);

            // Set null.
            browser.Parent = null;
            Assert.Null(browser.Parent);
            Assert.Null(browser.ActiveXInstance);
            Assert.False(browser.IsHandleCreated);
        }

        [StaFact]
        public void WebBrowser_Parent_SetWithHandler_CallsParentChanged()
        {
            var parent = new WebBrowser();
            var control = new WebBrowser();
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

        [StaFact]
        public void WebBrowser_Parent_SetSame_ThrowsArgumentException()
        {
            var control = new WebBrowser();
            Assert.Throws<ArgumentException>(null, () => control.Parent = control);
            Assert.Null(control.Parent);
        }

        [StaTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetBoolTheoryData))]
        public void WebBrowser_Visible_Set_GetReturnsExpected(bool value)
        {
            var control = new WebBrowser
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

        [StaFact]
        public void WebBrowser_Visible_SetTrue_GetReturnsExpected()
        {
            var control = new WebBrowser
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

        [StaTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetBoolTheoryData))]
        public void WebBrowser_Visible_SetWithHandle_GetReturnsExpected(bool value)
        {
            var control = new WebBrowser();
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

        [StaFact]
        public void WebBrowser_Visible_SetTrueWithHandle_GetReturnsExpected()
        {
            var control = new WebBrowser();
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

        [StaFact]
        public void WebBrowser_Visible_SetWithHandler_CallsVisibleChanged()
        {
            var control = new WebBrowser
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

        [StaTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetBoolTheoryData))]
        public void WebBrowser_CreateSink_InvokeWithInstance_Success(bool allowNavigation)
        {
            var parent = new Control();
            var browser = new SubWebBrowser
            {
                AllowNavigation = allowNavigation,
                Parent = parent
            };
            browser.CreateSink();
            Assert.NotNull(browser.ActiveXInstance);
            Assert.True(browser.IsHandleCreated);

            browser.CreateSink();
            Assert.NotNull(browser.ActiveXInstance);
            Assert.True(browser.IsHandleCreated);
        }

        [StaTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetBoolTheoryData))]
        public void WebBrowser_CreateSink_InvokeWithoutInstance_Nop(bool allowNavigation)
        {
            var browser = new SubWebBrowser
            {
                AllowNavigation = allowNavigation
            };
            browser.CreateSink();
            Assert.Null(browser.ActiveXInstance);
            Assert.False(browser.IsHandleCreated);

            browser.CreateSink();
            Assert.Null(browser.ActiveXInstance);
            Assert.False(browser.IsHandleCreated);
        }

        [StaTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetBoolTheoryData))]
        public void WebBrowser_DetachSink_InvokeWithCreatedSink_Success(bool allowNavigation)
        {
            var parent = new Control();
            var browser = new SubWebBrowser
            {
                AllowNavigation = allowNavigation,
                Parent = parent
            };
            browser.CreateSink();
            Assert.NotNull(browser.ActiveXInstance);
            Assert.True(browser.IsHandleCreated);

            browser.DetachSink();
            Assert.NotNull(browser.ActiveXInstance);
            Assert.True(browser.IsHandleCreated);

            // Call again.
            browser.DetachSink();
            Assert.NotNull(browser.ActiveXInstance);
            Assert.True(browser.IsHandleCreated);
        }

        [StaTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetBoolTheoryData))]
        public void WebBrowser_DetachSink_InvokeWithInstance_Success(bool allowNavigation)
        {
            var parent = new Control();
            var browser = new SubWebBrowser
            {
                AllowNavigation = allowNavigation,
                Parent = parent
            };

            browser.DetachSink();
            Assert.NotNull(browser.ActiveXInstance);
            Assert.True(browser.IsHandleCreated);

            // Call again.
            browser.DetachSink();
            Assert.NotNull(browser.ActiveXInstance);
            Assert.True(browser.IsHandleCreated);
        }

        [StaTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetBoolTheoryData))]
        public void WebBrowser_DetachSink_InvokeWithoutInstance_Nop(bool allowNavigation)
        {
            var browser = new SubWebBrowser
            {
                AllowNavigation = allowNavigation
            };
            browser.DetachSink();
            Assert.Null(browser.ActiveXInstance);
            Assert.False(browser.IsHandleCreated);

            // Call again.
            browser.DetachSink();
            Assert.Null(browser.ActiveXInstance);
            Assert.False(browser.IsHandleCreated);
        }

        private class SubWebBrowser : WebBrowser
        {
            public new void CreateSink() => base.CreateSink();

            public new void DetachSink() => base.DetachSink();
        }
    }
}
