// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Xunit;
using System.Drawing;
using System.ComponentModel;
using WinForms.Common.Tests;

namespace System.Windows.Forms.Tests
{
    public class CheckBoxTests
    {
        [Fact]
        public void CheckBox_Constructor()
        {
            var box = new CheckBox();

            Assert.NotNull(box);
            Assert.True(box.AutoCheck);
            Assert.Equal(ContentAlignment.MiddleLeft, box.TextAlign);
            Assert.Equal(CheckState.Unchecked, box.CheckState);
        }

        /// <summary>
        ///  Data for the AppearanceGetSet test
        /// </summary>
        public static TheoryData<Appearance> AppearanceGetSetData =>
            CommonTestHelper.GetEnumTheoryData<Appearance>();

        [Theory]
        [MemberData(nameof(AppearanceGetSetData))]
        public void CheckBox_AutoSizeModeGetSet(Appearance expected)
        {
            var box = new CheckBox
            {
                Appearance = expected
            };

            Assert.Equal(expected, box.Appearance);
        }

        /// <summary>
        ///  Data for the AppearanceGetSetInvalid test
        /// </summary>
        public static TheoryData<Appearance> AppearanceGetSetInvalidData =>
            CommonTestHelper.GetEnumTheoryDataInvalid<Appearance>();

        [Theory]
        [MemberData(nameof(AppearanceGetSetInvalidData))]
        public void CheckBox_AppearanceGetSetInvalid(Appearance expected)
        {
            var box = new CheckBox();

            InvalidEnumArgumentException ex = Assert.Throws<InvalidEnumArgumentException>(() => box.Appearance = expected);
            Assert.Equal("value", ex.ParamName);
        }

        /// <summary>
        ///  Data for the AutoCheck test
        /// </summary>
        public static TheoryData<bool> AutoCheckData =>
            CommonTestHelper.GetBoolTheoryData();

        [Theory]
        [MemberData(nameof(AutoCheckData))]
        public void CheckBox_AutoCheck(bool expected)
        {
            var box = new CheckBox
            {
                AutoCheck = expected
            };

            Assert.Equal(expected, box.AutoCheck);
        }

        /// <summary>
        ///  Data for the ContentAlignmentGetSet test
        /// </summary>
        public static TheoryData<ContentAlignment> ContentAlignmentGetSetData =>
            CommonTestHelper.GetEnumTheoryData<ContentAlignment>();

        [Theory]
        [MemberData(nameof(ContentAlignmentGetSetData))]
        public void CheckBox_ContentAlignmentGetSet(ContentAlignment expected)
        {
            var box = new CheckBox
            {
                CheckAlign = expected,
                TextAlign = expected
            };

            Assert.Equal(expected, box.CheckAlign);
            Assert.Equal(expected, box.TextAlign);
        }

        /// <summary>
        ///  Data for the ContentAlignmentGetSetInvalid test
        /// </summary>
        public static TheoryData<ContentAlignment> ContentAlignmentGetSetInvalidData =>
            CommonTestHelper.GetEnumTheoryDataInvalid<ContentAlignment>();

        [Theory]
        [MemberData(nameof(ContentAlignmentGetSetInvalidData))]
        public void CheckBox_ContentAlignmentGetSetInvalid(ContentAlignment expected)
        {
            var box = new CheckBox();

            InvalidEnumArgumentException ex = Assert.Throws<InvalidEnumArgumentException>(() => box.CheckAlign = expected);
            Assert.Equal("value", ex.ParamName);
        }

        [Theory]
        [InlineData(true, CheckState.Checked)]
        [InlineData(false, CheckState.Unchecked)]
        public void CheckBox_CheckedGetSet(bool sent, CheckState expected)
        {
            var box = new CheckBox
            {
                Checked = sent
            };

            Assert.Equal(expected, box.CheckState);
        }

        /// <summary>
        ///  Data for the CheckStateGetSet test
        /// </summary>
        public static TheoryData<CheckState> CheckStateGetSetData =>
            CommonTestHelper.GetEnumTheoryData<CheckState>();

        [Theory]
        [MemberData(nameof(CheckStateGetSetData))]
        public void CheckBox_CheckStateGetSet(CheckState expected)
        {
            var box = new CheckBox
            {
                CheckState = expected
            };

            Assert.Equal(expected, box.CheckState);
        }

        /// <summary>
        ///  Data for the CheckStateGetSetInvalid test
        /// </summary>
        public static TheoryData<CheckState> CheckStateGetSetInvalidData =>
            CommonTestHelper.GetEnumTheoryDataInvalid<CheckState>();

        [Theory]
        [MemberData(nameof(CheckStateGetSetInvalidData))]
        public void CheckBox_CheckStateGetSetInvalid(CheckState expected)
        {
            var box = new CheckBox();

            InvalidEnumArgumentException ex = Assert.Throws<InvalidEnumArgumentException>(() => box.CheckState = expected);
            Assert.Equal("value", ex.ParamName);
        }

        /// <summary>
        ///  Data for the ThreeState test
        /// </summary>
        public static TheoryData<bool> ThreeStateData =>
            CommonTestHelper.GetBoolTheoryData();

        [Theory]
        [MemberData(nameof(ThreeStateData))]
        public void CheckBox_ThreeState(bool expected)
        {
            var box = new CheckBox
            {
                ThreeState = expected
            };

            Assert.Equal(expected, box.ThreeState);
        }

        [Fact]
        public void CheckBox_CreateFlatAdapter()
        {
            var box = new CheckBox();

            ButtonInternal.ButtonBaseAdapter buttonBaseAdptr = box.CreateFlatAdapter();

            Assert.NotNull(buttonBaseAdptr);
        }

        [Fact]
        public void CheckBox_CreatePopupAdapter()
        {
            var box = new CheckBox();

            ButtonInternal.ButtonBaseAdapter checkBoxPopupAdptr = box.CreatePopupAdapter();

            Assert.NotNull(checkBoxPopupAdptr);
        }

        [Fact]
        public void CheckBox_CreateStandardAdapter()
        {
            var box = new CheckBox();

            ButtonInternal.ButtonBaseAdapter checkBoxSndAdptr = box.CreateStandardAdapter();

            Assert.NotNull(checkBoxSndAdptr);
        }

        // the zero here may be an issue with cultural variance
        [Fact]
        public void CheckBox_ToStringTest()
        {
            var box = new CheckBox();
            var expected = "System.Windows.Forms.CheckBox, CheckState: 0";

            var actual = box.ToString();

            Assert.Equal(expected, actual);
        }

    }
}
