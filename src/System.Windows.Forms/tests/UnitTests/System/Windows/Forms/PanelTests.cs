// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel;
using System.Drawing;
using WinForms.Common.Tests;
using Xunit;

namespace System.Windows.Forms.Tests
{
    public class PanelTests
    {
        [Fact]
        public void Panel_Ctor_Default()
        {
            var panel = new Panel();
            Assert.False(panel.AutoSize);
            Assert.Equal(AutoSizeMode.GrowOnly, panel.AutoSizeMode);
            Assert.Equal(BorderStyle.None, panel.BorderStyle);
            Assert.False(panel.TabStop);
            Assert.Equal("", panel.Text);
        }

        [Theory]
        [MemberData(nameof(CommonTestHelper.GetBoolTheoryData), MemberType = typeof(CommonTestHelper))]
        public void Panel_AutoSize_Set_GetReturnsExpected(bool value)
        {
            var panel = new Panel();
            panel.AutoSize = value;
            Assert.Equal(value, panel.AutoSize);

            // Set with observer.
            int callCount = 0;
            EventHandler handler = (sender, e) =>
            {
                Assert.Equal(panel, sender);
                callCount++;
            };

            panel.AutoSizeChanged += handler;
            panel.AutoSize = !value;
            Assert.Equal(1, callCount);

            // Should not call if the value is the same.
            panel.AutoSize = !value;
            Assert.Equal(1, callCount);

            // Should not call if the handler is removed.
            panel.AutoSizeChanged -= handler;
            panel.AutoSize = value;
            Assert.Equal(1, callCount);
        }

        [Theory]
        [MemberData(nameof(CommonTestHelper.GetEnumTypeTheoryData), typeof(AutoSizeMode), MemberType = typeof(CommonTestHelper))]
        public void Panel_AutoSizeMode_Set_GetReturnsExpected(AutoSizeMode value)
        {
            var panel = new Panel
            {
                AutoSizeMode = value
            };
            Assert.Equal(value, panel.AutoSizeMode);
        }

        [Theory]
        [MemberData(nameof(CommonTestHelper.GetEnumTypeTheoryDataInvalid), typeof(AutoSizeMode), MemberType = typeof(CommonTestHelper))]
        public void Panel_AutoSizeMode_SetInvalid_ThrowsInvalidEnumArgumentException(AutoSizeMode value)
        {
            var panel = new Panel();
            Assert.Throws<InvalidEnumArgumentException>("value", () => panel.AutoSizeMode = value);
        }

        [Theory]
        [MemberData(nameof(CommonTestHelper.GetEnumTypeTheoryData), typeof(BorderStyle), MemberType = typeof(CommonTestHelper))]
        public void Panel_BorderStyle_Set_GetReturnsExpected(BorderStyle value)
        {
            var panel = new Panel
            {
                BorderStyle = value
            };
            Assert.Equal(value, panel.BorderStyle);
        }

        [Theory]
        [MemberData(nameof(CommonTestHelper.GetEnumTypeTheoryDataInvalid), typeof(BorderStyle), MemberType = typeof(CommonTestHelper))]
        public void Panel_BorderStyle_SetInvalid_ThrowsInvalidEnumArgumentException(BorderStyle value)
        {
            var panel = new Panel();
            Assert.Throws<InvalidEnumArgumentException>("value", () => panel.BorderStyle = value);
        }

        [Fact]
        public void Panel_KeyUp_AddRemove_Success()
        {
            var panel = new Panel();
            KeyEventHandler handler = (sender, e) => { };
            panel.KeyUp += handler;
            panel.KeyUp -= handler;
        }

        [Fact]
        public void Panel_KeyDown_AddRemove_Success()
        {
            var panel = new Panel();
            KeyEventHandler handler = (sender, e) => { };
            panel.KeyDown += handler;
            panel.KeyDown -= handler;
        }

        [Fact]
        public void Panel_KeyPress_AddRemove_Success()
        {
            var panel = new Panel();
            KeyPressEventHandler handler = (sender, e) => { };
            panel.KeyPress += handler;
            panel.KeyPress -= handler;
        }

        [Theory]
        [MemberData(nameof(CommonTestHelper.GetBoolTheoryData), MemberType = typeof(CommonTestHelper))]
        public void Panel_TabStop_Set_GetReturnsExpected(bool value)
        {
            var panel = new Panel();
            panel.TabStop = value;
            Assert.Equal(value, panel.TabStop);

            // Set with observer.
            int callCount = 0;
            EventHandler handler = (sender, e) =>
            {
                Assert.Equal(panel, sender);
                callCount++;
            };

            panel.TabStopChanged += handler;
            panel.TabStop = !value;
            Assert.Equal(1, callCount);

            // Should not call if the value is the same.
            panel.TabStop = !value;
            Assert.Equal(1, callCount);

            // Should not call if the handler is removed.
            panel.TabStopChanged -= handler;
            panel.TabStop = value;
            Assert.Equal(1, callCount);
        }

        [Fact]
        public void Panel_Text_Set_GetReturnsExpected()
        {
            var panel = new Panel();
            panel.Text = "value1";
            Assert.Equal("value1", panel.Text);

            // Set with observer.
            int callCount = 0;
            EventHandler handler = (sender, e) =>
            {
                Assert.Equal(panel, sender);
                callCount++;
            };

            panel.TextChanged += handler;
            panel.Text = "value2";
            Assert.Equal(1, callCount);

            // Should not call if the value is the same.
            panel.Text = "value2";
            Assert.Equal(1, callCount);

            // Should not call if the handler is removed.
            panel.TextChanged -= handler;
            panel.Text = "value3";
            Assert.Equal(1, callCount);
        }

        [Fact]
        public void Panel_ToString_Invoke_ReturnsExpected()
        {
            var panel = new Panel { BorderStyle = BorderStyle.Fixed3D };
            Assert.Equal("System.Windows.Forms.Panel, BorderStyle: System.Windows.Forms.BorderStyle.Fixed3D", panel.ToString());
        }

        [Fact]
        public void Panel_DefaultSize_Get_ReturnsExpected()
        {
            var panel = new SubPanel();
            Assert.Equal(new Size(200, 100), panel.DefaultSizeEntry);
        }

        private class SubPanel : Panel
        {
            public Size DefaultSizeEntry => DefaultSize;
        }
    }
}
