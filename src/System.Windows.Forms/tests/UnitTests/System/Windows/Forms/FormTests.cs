// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Xunit;
using Moq;
using System.Drawing;
using WinForms.Common.Tests;

namespace System.Windows.Forms.Tests
{
    public class FormTests
    {
        [Fact]
        public void Form_Ctor_Default()
        {
            var form = new Form();
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

        [Fact]
        public void Form_AcceptButtonGetSet()
        {
            var form = new Form();
            var mock = new Mock<IButtonControl>(MockBehavior.Strict);
            mock.Setup(x => x.NotifyDefault(It.IsAny<bool>()));

            form.AcceptButton = mock.Object;

            Assert.Equal(mock.Object, form.AcceptButton);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetBoolTheoryData))]
        public void Form_Active_Set_GetReturnsExpected(bool value)
        {
            var form = new Form
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

        [Fact]
        public void Form_ActiveMdiChildInternalGetSet()
        {
            var form = new Form();
            var child = new Form();

            form.ActiveMdiChildInternal = child;

            Assert.NotNull(form.ActiveMdiChildInternal);
            Assert.Equal(child, form.ActiveMdiChildInternal);
        }

        [Fact]
        public void Form_ActiveMdiChildGetSet()
        {
            var form = new Form();
            var child = new Form
            {
                Visible = true,
                Enabled = true
            };

            form.ActiveMdiChildInternal = child;

            Assert.NotNull(form.ActiveMdiChild);
            Assert.Equal(child, form.ActiveMdiChild);
        }

        [Fact]
        public void Form_ActiveMdiChildGetSetChildNotVisible()
        {
            var form = new Form();
            var child = new Form
            {
                Visible = false,
                Enabled = true
            };

            form.ActiveMdiChildInternal = child;

            Assert.Null(form.ActiveMdiChild);
        }

        [Fact]
        public void Form_ActiveMdiChildGetSetChildNotEnabled()
        {
            var form = new Form();
            var child = new Form
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
