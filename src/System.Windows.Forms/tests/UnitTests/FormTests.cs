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
        public void Form_Constructor()
        {
            var form = new Form();

            // and & assert
            Assert.NotNull(form);
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

        #region Activation

        /// <summary>
        /// Data for the ActiveGetSet test
        /// </summary>
        public static TheoryData<bool> ActiveGetSetData =>
            CommonTestHelper.GetBoolTheoryData();

        [Theory]
        [MemberData(nameof(ActiveGetSetData))]
        public void Form_ActiveGetSet(bool expected)
        {
            var form = new Form();

            form.Active = expected;

            Assert.Equal(expected, form.Active);
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
            var child = new Form();
            child.Visible = true;
            child.Enabled = true;

            form.ActiveMdiChildInternal = child;

            Assert.NotNull(form.ActiveMdiChild);
            Assert.Equal(child, form.ActiveMdiChild);
        }

        [Fact]
        public void Form_ActiveMdiChildGetSetChildNotVisible()
        {
            var form = new Form();
            var child = new Form();
            child.Visible = false;
            child.Enabled = true;

            form.ActiveMdiChildInternal = child;

            Assert.Null(form.ActiveMdiChild);
        }

        [Fact]
        public void Form_ActiveMdiChildGetSetChildNotEnabled()
        {
            var form = new Form();
            var child = new Form();
            child.Visible = true;
            child.Enabled = false;

            form.ActiveMdiChildInternal = child;

            Assert.Null(form.ActiveMdiChild);
        }

        [Fact]
        public void Form_ActiveMdiChildNoSet()
        {
            var form = new Form();

            // act          

            Assert.Null(form.ActiveMdiChild);
        }

        #endregion

        /// <summary>
        /// Data for the AllowTransparencyGetSet test
        /// </summary>
        public static TheoryData<bool> AllowTransparencyGetSetData =>
            CommonTestHelper.GetBoolTheoryData();

        [Theory]
        [MemberData(nameof(AllowTransparencyGetSetData))]
        public void Form_AllowTransparencyGetSet(bool expected)
        {
            var form = new Form();

            form.AllowTransparency = expected;

            Assert.Equal(expected, form.AllowTransparency);
        }

        /// <summary>
        /// Data for the AutoScaleModeGetSet test
        /// </summary>
        public static TheoryData<AutoScaleMode> AutoScaleModeGetSetData =>
            CommonTestHelper.GetEnumTheoryData<AutoScaleMode>();

        [Theory]
        [MemberData(nameof(AutoScaleModeGetSetData))]
        public void Form_AutoScaleModeGetSet(AutoScaleMode expected)
        {
            var form = new Form();

            form.AutoScaleMode = expected;

            Assert.Equal(expected, form.AutoScaleMode);
        }

        /// <summary>
        /// Data for the AutoScrollGetSet test
        /// </summary>
        public static TheoryData<bool> AutoScrollGetSetData =>
            CommonTestHelper.GetBoolTheoryData();

        [Theory]
        [MemberData(nameof(AutoScrollGetSetData))]
        public void Form_AutoScrollGetSet(bool expected)
        {
            var form = new Form();

            form.AutoScroll = expected;

            Assert.Equal(expected, form.AutoScroll);
        }

        /// <summary>
        /// Data for the AutoSizeGetSet test
        /// </summary>
        public static TheoryData<bool> AutoSizeGetSetData =>
            CommonTestHelper.GetBoolTheoryData();

        [Theory]
        [MemberData(nameof(AutoSizeGetSetData))]
        public void Form_AutoSizeGetSet(bool expected)
        {
            var form = new Form();

            form.AutoSize = expected;

            Assert.Equal(expected, form.AutoSize);
        }


    }
}
