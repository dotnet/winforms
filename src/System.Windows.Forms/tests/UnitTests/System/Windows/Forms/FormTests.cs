// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using Moq;
using Xunit;
using WinForms.Common.Tests;
using static Interop;
using System.Runtime.InteropServices;

namespace System.Windows.Forms.Tests
{
    public class FormTests
    {
        [WinFormsFact]
        public void Form_Ctor_Default()
        {
            using var form = new Form();
            Assert.False(form.Active);
            Assert.Null(form.ActiveMdiChild);
            Assert.False(form.AllowTransparency);
#pragma warning disable 0618
            Assert.True(form.AutoScale);
#pragma warning restore 0618
            Assert.Equal(AutoScaleMode.Inherit, form.AutoScaleMode);
            Assert.False(form.AutoScroll);
            Assert.False(form.AutoSize);
            Assert.False(form.IsRestrictedWindow);
            Assert.True(form.TopLevel);
            Assert.False(form.Visible);
        }

        [WinFormsFact]
        public static void Form_Ctor_show_icon_by_default()
        {
            using var form = new Form();
            Assert.True(form.Handle != IntPtr.Zero);

            IntPtr hSmallIcon = User32.SendMessageW(form, WindowMessages.WM_GETICON, (IntPtr)NativeMethods.ICON_SMALL, IntPtr.Zero);
            Assert.True(hSmallIcon != IntPtr.Zero);

            IntPtr hLargeIcon = User32.SendMessageW(form, WindowMessages.WM_GETICON, (IntPtr)NativeMethods.ICON_BIG, IntPtr.Zero);
            Assert.True(hLargeIcon != IntPtr.Zero);

            // normal form doesn't have WS_EX.DLGMODALFRAME set, and show icon
            int extendedStyle = unchecked((int)(long)UnsafeNativeMethods.GetWindowLong(new HandleRef(form, form.Handle), NativeMethods.GWL_EXSTYLE));
            Assert.True((extendedStyle & NativeMethods.WS_EX_DLGMODALFRAME) == 0);
        }

        [WinFormsFact]
        public void Form_AcceptButtonGetSet()
        {
            using var form = new Form();
            var mock = new Mock<IButtonControl>(MockBehavior.Strict);
            mock.Setup(x => x.NotifyDefault(It.IsAny<bool>()));

            form.AcceptButton = mock.Object;

            Assert.Equal(mock.Object, form.AcceptButton);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetBoolTheoryData))]
        public void Form_Active_Set_GetReturnsExpected(bool value)
        {
            using var form = new Form
            {
                Active = value
            };
            Assert.Equal(value, form.Active);

            // Set same.
            form.Active = value;
            Assert.Equal(value, form.Active);
        }

        // non deterministic, commenting out for now
        /*[Fact]
        public void Form_ActiveFormNotSetActive()
        {
            var form = new Form();

            form.Visible = true;
            form.Active = false;

            Assert.NotNull(Form.ActiveForm);
            Assert.Equal(form, Form.ActiveForm);
            Assert.False(Form.ActiveForm.Active);
        }*/

        [WinFormsFact]
        public void Form_ActiveMdiChildInternalGetSet()
        {
            using var form = new Form();
            using var child = new Form();

            form.ActiveMdiChildInternal = child;

            Assert.NotNull(form.ActiveMdiChildInternal);
            Assert.Equal(child, form.ActiveMdiChildInternal);
        }

        [WinFormsFact]
        public void Form_ActiveMdiChildGetSet()
        {
            using var form = new Form();
            using var child = new Form
            {
                Visible = true,
                Enabled = true
            };

            form.ActiveMdiChildInternal = child;

            Assert.NotNull(form.ActiveMdiChild);
            Assert.Equal(child, form.ActiveMdiChild);
        }

        [WinFormsFact]
        public void Form_ActiveMdiChildGetSetChildNotVisible()
        {
            using var form = new Form();
            using var child = new Form
            {
                Visible = false,
                Enabled = true
            };

            form.ActiveMdiChildInternal = child;

            Assert.Null(form.ActiveMdiChild);
        }

        [WinFormsFact]
        public void Form_ActiveMdiChildGetSetChildNotEnabled()
        {
            using var form = new Form();
            using var child = new Form
            {
                Visible = true,
                Enabled = false
            };

            form.ActiveMdiChildInternal = child;

            Assert.Null(form.ActiveMdiChild);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetBoolTheoryData))]
        public void Form_AllowTransparency_Set_GetReturnsExpected(bool value)
        {
            var form = new Form
            {
                AllowTransparency = value
            };
            Assert.Equal(value, form.AllowTransparency);

            // Set same.
            form.AllowTransparency = value;
            Assert.Equal(value, form.AllowTransparency);
        }

#pragma warning disable 0618

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetBoolTheoryData))]
        public void Form_AutoScale_Set_GetReturnsExpected(bool value)
        {
            var form = new Form
            {
                AutoScale = value
            };
            Assert.Equal(value, form.AutoScale);

            // Set same.
            form.AutoScale = value;
            Assert.Equal(value, form.AutoScale);
        }

#pragma warning restore 0618

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetEnumTypeTheoryData), typeof(AutoScaleMode))]
        public void Form_AutoScaleMode_Set_GetReturnsExpected(AutoScaleMode value)
        {
            var form = new Form
            {
                AutoScaleMode = value
            };
            Assert.Equal(value, form.AutoScaleMode);

            // Set same.
            form.AutoScaleMode = value;
            Assert.Equal(value, form.AutoScaleMode);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetBoolTheoryData))]
        public void Form_AutoScroll_Set_GetReturnsExpected(bool value)
        {
            var form = new Form
            {
                AutoScroll = value
            };
            Assert.Equal(value, form.AutoScroll);

            // Set same.
            form.AutoScroll = value;
            Assert.Equal(value, form.AutoScroll);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetBoolTheoryData))]
        public void Form_AutoSizeSet_GetReturnsExpected(bool value)
        {
            var form = new Form
            {
                AutoSize = value
            };
            Assert.Equal(value, form.AutoSize);

            // Set same.
            form.AutoSize = value;
            Assert.Equal(value, form.AutoSize);
        }
    }
}
