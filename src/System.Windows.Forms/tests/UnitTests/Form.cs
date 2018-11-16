// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Xunit;
using Moq;
using System.Drawing;

namespace System.Windows.Forms.Tests
{
    public class FormTests
    {
        [Fact]
        public void Constructor()
        {
            // arrange
            var form = new Form();

            // and & assert
            Assert.NotNull(form);
            Assert.False(form.IsRestrictedWindow);
            Assert.True(form.TopLevel);
            Assert.False(form.Visible);
        }

        [Fact]
        public void AcceptButtonGetSet()
        {
            // arrange
            var form = new Form();
            var mock = new Mock<IButtonControl>(MockBehavior.Strict);
            mock.Setup(x => x.NotifyDefault(It.IsAny<bool>()));

            // act
            form.AcceptButton = mock.Object;

            // assert
            Assert.Equal(mock.Object, form.AcceptButton);
        }

        #region Activation

        /// <summary>
        /// Data for the ActiveGetSet test
        /// </summary>
        public static TheoryData<bool> ActiveGetSetData =>
            TestHelper.GetBoolTheoryData();

        [Theory]
        [MemberData(nameof(ActiveGetSetData))]
        public void ActiveGetSet(bool expected)
        {
            // arrange
            var form = new Form();

            // act
            form.Active = expected;

            // assert
            Assert.Equal(expected, form.Active);
        }

        // non deterministic, commenting out for now
        /*[Fact]
        public void ActiveFormNotSetActive()
        {
            // arrange
            var form = new Form();

            // act
            form.Visible = true;
            form.Active = false;

            // assert
            Assert.NotNull(Form.ActiveForm);
            Assert.Equal(form, Form.ActiveForm);
            Assert.False(Form.ActiveForm.Active);
        }*/

        [Fact]
        public void ActiveMdiChildInternalGetSet()
        {
            // arrange
            var form = new Form();
            var child = new Form();

            // act
            form.ActiveMdiChildInternal = child;

            // assert
            Assert.NotNull(form.ActiveMdiChildInternal);
            Assert.Equal(child, form.ActiveMdiChildInternal);
        }

        [Fact]
        public void ActiveMdiChildGetSet()
        {
            // arrange
            var form = new Form();
            var child = new Form();
            child.Visible = true;
            child.Enabled = true;

            // act
            form.ActiveMdiChildInternal = child;

            // assert
            Assert.NotNull(form.ActiveMdiChild);
            Assert.Equal(child, form.ActiveMdiChild);
        }

        [Fact]
        public void ActiveMdiChildGetSetChildNotVisible()
        {
            // arrange
            var form = new Form();
            var child = new Form();
            child.Visible = false;
            child.Enabled = true;

            // act
            form.ActiveMdiChildInternal = child;

            // assert
            Assert.Null(form.ActiveMdiChild);
        }

        [Fact]
        public void ActiveMdiChildGetSetChildNotEnabled()
        {
            // arrange
            var form = new Form();
            var child = new Form();
            child.Visible = true;
            child.Enabled = false;

            // act
            form.ActiveMdiChildInternal = child;

            // assert
            Assert.Null(form.ActiveMdiChild);
        }

        [Fact]
        public void ActiveMdiChildNoSet()
        {
            // arrange
            var form = new Form();

            // act          

            // assert
            Assert.Null(form.ActiveMdiChild);
        }

        #endregion

        /// <summary>
        /// Data for the AllowTransparencyGetSet test
        /// </summary>
        public static TheoryData<bool> AllowTransparencyGetSetData =>
            TestHelper.GetBoolTheoryData();

        [Theory]
        [MemberData(nameof(AllowTransparencyGetSetData))]
        public void AllowTransparencyGetSet(bool expected)
        {
            // arrange
            var form = new Form();

            // act
            form.AllowTransparency = expected;

            // assert
            Assert.Equal(expected, form.AllowTransparency);
        }

        /// <summary>
        /// Data for the AutoScaleModeGetSet test
        /// </summary>
        public static TheoryData<AutoScaleMode> AutoScaleModeGetSetData =>
            TestHelper.GetEnumTheoryData<AutoScaleMode>();

        [Theory]
        [MemberData(nameof(AutoScaleModeGetSetData))]
        public void AutoScaleModeGetSet(AutoScaleMode expected)
        {
            // arrange
            var form = new Form();

            // act
            form.AutoScaleMode = expected;

            // assert
            Assert.Equal(expected, form.AutoScaleMode);
        }

        /// <summary>
        /// Data for the AutoScrollGetSet test
        /// </summary>
        public static TheoryData<bool> AutoScrollGetSetData =>
            TestHelper.GetBoolTheoryData();

        [Theory]
        [MemberData(nameof(AutoScrollGetSetData))]
        public void AutoScrollGetSet(bool expected)
        {
            // arrange
            var form = new Form();

            // act
            form.AutoScroll = expected;

            // assert
            Assert.Equal(expected, form.AutoScroll);
        }

        /// <summary>
        /// Data for the AutoSizeGetSet test
        /// </summary>
        public static TheoryData<bool> AutoSizeGetSetData =>
            TestHelper.GetBoolTheoryData();

        [Theory]
        [MemberData(nameof(AutoSizeGetSetData))]
        public void AutoSizeGetSet(bool expected)
        {
            // arrange
            var form = new Form();

            // act
            form.AutoSize = expected;

            // assert
            Assert.Equal(expected, form.AutoSize);
        }


    }
}
