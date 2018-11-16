// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Xunit;
using System.Drawing;
using System.ComponentModel;

namespace System.Windows.Forms.Tests
{
    public class CheckBoxTests
    {
        [Fact]
        public void CheckBoxTests_Constructor()
        {
            var box = new CheckBox();

            Assert.NotNull(box);
            Assert.True(box.AutoCheck);
            Assert.Equal(ContentAlignment.MiddleLeft, box.TextAlign);
            Assert.Equal(CheckState.Unchecked, box.CheckState);
        }

        /// <summary>
        /// Data for the AppearanceGetSet test
        /// </summary>
        public static TheoryData<Appearance> AppearanceGetSetData =>
            TestHelper.GetEnumTheoryData<Appearance>();

        [Theory]
        [MemberData(nameof(AppearanceGetSetData))]
        public void CheckBoxTests_AutoSizeModeGetSet(Appearance expected)
        {
            var box = new CheckBox();

            box.Appearance = expected;

            Assert.Equal(expected, box.Appearance);
        }

        /// <summary>
        /// Data for the AppearanceGetSetInvalid test
        /// </summary>
        public static TheoryData<Appearance> AppearanceGetSetInvalidData =>
            TestHelper.GetEnumTheoryDataInvalid<Appearance>();

        [Theory]
        [MemberData(nameof(AppearanceGetSetInvalidData))]
        public void CheckBoxTests_AppearanceGetSetInvalid(Appearance expected)
        {
            var box = new CheckBox();

            var ex = Assert.Throws<InvalidEnumArgumentException>(() => box.Appearance = expected);
            Assert.Equal("value", ex.ParamName);
        }

        /// <summary>
        /// Data for the AutoCheck test
        /// </summary>
        public static TheoryData<bool> AutoCheckData =>
            TestHelper.GetBoolTheoryData();

        [Theory]
        [MemberData(nameof(AutoCheckData))]
        public void CheckBoxTests_AutoCheck(bool expected)
        {
            var box = new CheckBox();

            box.AutoCheck = expected;

            Assert.Equal(expected, box.AutoCheck);
        }

        /// <summary>
        /// Data for the ContentAlignmentGetSet test
        /// </summary>
        public static TheoryData<ContentAlignment> ContentAlignmentGetSetData =>
            TestHelper.GetEnumTheoryData<ContentAlignment>();

        [Theory]
        [MemberData(nameof(ContentAlignmentGetSetData))]
        public void CheckBoxTests_ContentAlignmentGetSet(ContentAlignment expected)
        {
            var box = new CheckBox();

            box.CheckAlign = expected;
            box.TextAlign = expected;

            Assert.Equal(expected, box.CheckAlign);
            Assert.Equal(expected, box.TextAlign);
        }

        /// <summary>
        /// Data for the ContentAlignmentGetSetInvalid test
        /// </summary>
        public static TheoryData<ContentAlignment> ContentAlignmentGetSetInvalidData =>
            TestHelper.GetEnumTheoryDataInvalid<ContentAlignment>();

        [Theory]
        [MemberData(nameof(ContentAlignmentGetSetInvalidData))]
        public void CheckBoxTests_ContentAlignmentGetSetInvalid(ContentAlignment expected)
        {
            var box = new CheckBox();

            var ex = Assert.Throws<InvalidEnumArgumentException>(() => box.CheckAlign = expected);
            Assert.Equal("value", ex.ParamName);

            //var ex2 = Assert.Throws<InvalidEnumArgumentException>(() => box.TextAlign = expected);
            //Assert.Equal("value", ex2.ParamName);
        }

        [Fact]
        public void CheckBoxTests_ShouldBeChecked()
        {
            var box = new CheckBox();

            box.Checked = true;

            Assert.Equal(CheckState.Checked, box.CheckState);
        }

        [Fact]
        public void CheckBoxTests_ShouldBeNotChecked()
        {
            var box = new CheckBox();

            box.Checked = false;

            Assert.NotEqual(CheckState.Checked, box.CheckState);
        }

        /// <summary>
        /// Data for the CheckStateGetSet test
        /// </summary>
        public static TheoryData<CheckState> CheckStateGetSetData =>
            TestHelper.GetEnumTheoryData<CheckState>();

        [Theory]
        [MemberData(nameof(CheckStateGetSetData))]
        public void CheckBoxTests_CheckStateGetSet(CheckState expected)
        {
            var box = new CheckBox();

            box.CheckState = expected;

            Assert.Equal(expected, box.CheckState);
        }

        /// <summary>
        /// Data for the CheckStateGetSetInvalid test
        /// </summary>
        public static TheoryData<CheckState> CheckStateGetSetInvalidData =>
            TestHelper.GetEnumTheoryDataInvalid<CheckState>();

        [Theory]
        [MemberData(nameof(CheckStateGetSetInvalidData))]
        public void CheckBoxTests_CheckStateGetSetInvalid(CheckState expected)
        {
            var box = new CheckBox();

            var ex = Assert.Throws<InvalidEnumArgumentException>(() => box.CheckState = expected);
            Assert.Equal("value", ex.ParamName);
        }

        /// <summary>
        /// Data for the ThreeState test
        /// </summary>
        public static TheoryData<bool> ThreeStateData =>
            TestHelper.GetBoolTheoryData();

        [Theory]
        [MemberData(nameof(ThreeStateData))]
        public void CheckBoxTests_ThreeState(bool expected)
        {
            var box = new CheckBox();

            box.ThreeState = expected;

            Assert.Equal(expected, box.ThreeState);
        }

        [Fact]
        public void CheckBoxTests_CreateFlatAdapter()
        {
            var box = new CheckBox();

            var buttonBaseAdptr = box.CreateFlatAdapter();

            Assert.NotNull(buttonBaseAdptr);
        }

        [Fact]
        public void CheckBoxTests_CreatePopupAdapter()
        {
            var box = new CheckBox();

            var checkBoxPopupAdptr = box.CreatePopupAdapter();

            Assert.NotNull(checkBoxPopupAdptr);
        }

        [Fact]
        public void CheckBoxTests_CreateStandardAdapter()
        {
            var box = new CheckBox();

            var checkBoxSndAdptr = box.CreateStandardAdapter();

            Assert.NotNull(checkBoxSndAdptr);
        }

        // the zero here may be an issue with cultural variance
        [Fact]
        public void CheckBoxTests_ToStringTest()
        {
            var box = new CheckBox();
            var expected = "System.Windows.Forms.CheckBox, CheckState: 0";

            var actual = box.ToString();

            Assert.Equal(expected, actual);
        }

    }
}
