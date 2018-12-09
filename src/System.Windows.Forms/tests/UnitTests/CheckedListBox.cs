// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Xunit;
using System.ComponentModel;
using WinForms.Common.Tests;

namespace System.Windows.Forms.Tests
{
    public class CheckedListBoxTests
    {
        [Fact]
        public void CheckedListBox_Constructor()
        {
            var box = new CheckedListBox();

            Assert.NotNull(box);   
        }

        /// <summary>
        /// Data for the CheckOnClick test
        /// </summary>
        public static TheoryData<bool> CheckOnClickData =>
            CommonTestHelper.GetBoolTheoryData();

        [Theory]
        [MemberData(nameof(CheckOnClickData))]
        public void CheckedListBox_CheckOnClick(bool expected)
        {
            var box = new CheckedListBox();

            box.CheckOnClick = expected;

            Assert.Equal(expected, box.CheckOnClick);
        }

        [Fact]
        public void CheckedListBox_CheckedIndexCollectionNotNull()
        {
            var box = new CheckedListBox();

            var collection = box.CheckedIndices;

            Assert.NotNull(collection);
        }

        [Fact]
        public void CheckedListBox_CheckedItemCollectionNotNull()
        {
            var box = new CheckedListBox();

            var collection = box.CheckedItems;

            Assert.NotNull(collection);
        }

        /// <summary>
        /// Data for the DisplayMember test
        /// </summary>
        public static TheoryData<string> DisplayMemberData =>
            CommonTestHelper.GetStringTheoryData();

        [Theory]
        [MemberData(nameof(DisplayMemberData))]
        public void CheckedListBox_DisplayMember(string expected)
        {
            var box = new CheckedListBox();

            box.DisplayMember = expected;

            Assert.Equal(expected, box.DisplayMember);
        }

        [Fact]
        public void CheckedListBox_DrawModeReturnsNormalOnly()
        {
            var box = new CheckedListBox();

            var result = box.DrawMode;

            Assert.Equal(DrawMode.Normal, result);
        }

        [Theory]
        [InlineData(SelectionMode.None)]
        [InlineData(SelectionMode.One)]
        public void CheckedListBox_SelectionModeGetSet(SelectionMode expected)
        {
            var box = new CheckedListBox();

            box.SelectionMode = expected;

            Assert.Equal(expected, box.SelectionMode);
        }

        [Theory]
        [InlineData(SelectionMode.MultiSimple)]
        [InlineData(SelectionMode.MultiExtended)]
        public void CheckedListBox_SelectionModeGetSetInvalidFromEnum(SelectionMode expected)
        {
            var box = new CheckedListBox();

            var ex = Assert.Throws<ArgumentException>(() => box.SelectionMode = expected);
        }

        /// <summary>
        /// Data for the SelectionModeGetSetInvalid test
        /// </summary>
        public static TheoryData<SelectionMode> SelectionModeGetSetInvalidData =>
            CommonTestHelper.GetEnumTheoryDataInvalid<SelectionMode>();

        [Theory]
        [MemberData(nameof(SelectionModeGetSetInvalidData))]
        public void CheckedListBox_CheckStateGetSetInvalid(SelectionMode expected)
        {
            var box = new CheckedListBox();

            var ex = Assert.Throws<InvalidEnumArgumentException>(() => box.SelectionMode = expected);
            Assert.Equal("value", ex.ParamName);
        }

        /// <summary>
        /// Data for the ThreeDCheckBoxes test
        /// </summary>
        public static TheoryData<bool> ThreeDCheckBoxesData =>
            CommonTestHelper.GetBoolTheoryData();

        [Theory]
        [MemberData(nameof(ThreeDCheckBoxesData))]
        public void CheckedListBox_ThreeDCheckBoxes(bool expected)
        {
            var box = new CheckedListBox();

            box.ThreeDCheckBoxes = expected;

            Assert.Equal(expected, box.ThreeDCheckBoxes);
        }

        /// <summary>
        /// Data for the ValueMember test
        /// </summary>
        public static TheoryData<string> ValueMemberData =>
            CommonTestHelper.GetStringTheoryData();

        [Theory]
        [MemberData(nameof(ValueMemberData))]
        public void CheckedListBox_ValueMember(string expected)
        {
            var box = new CheckedListBox();

            box.ValueMember = expected;

            Assert.Equal(expected, box.ValueMember);
        }

        /// <summary>
        /// Data for the Padding test
        /// </summary>
        public static TheoryData<Padding> PaddingData =>
            TestHelper.GetPaddingTheoryData();

        [Theory]
        [MemberData(nameof(PaddingData))]
        public void CheckedListBox_Padding(Padding expected)
        {
            var box = new CheckedListBox();

            box.Padding = expected;

            Assert.Equal(expected, box.Padding);
        }

        /// <summary>
        /// Data for the PaddingInvalid test
        /// </summary>
        public static TheoryData<Padding> PaddingDataInvalid =>
            TestHelper.GetPaddingTheoryDataInvalid();

        [Theory]
        [MemberData(nameof(PaddingDataInvalid))]
        public void CheckedListBox_PaddingInvalid(Padding attempted)
        {
            var box = new CheckedListBox();

            box.Padding = attempted;

            Assert.NotEqual(attempted, box.Padding); //paddings cannot be negative
            Assert.Equal(new Padding(), box.Padding);
        }

        [Theory]
        [InlineData(1)]
        [InlineData(-1)]
        [InlineData(int.MaxValue)]
        public void CheckedListBox_GetItemCheckStateOutOfRange(int index)
        {
            var box = new CheckedListBox();

            var ex = Assert.Throws<ArgumentOutOfRangeException>(() => box.GetItemCheckState(index));
            Assert.Equal("index", ex.ParamName);
        }

        [Theory]
        [InlineData(1)]
        [InlineData(-1)]
        [InlineData(int.MaxValue)]
        public void CheckedListBox_SetItemCheckStateOutOfRange(int index)
        {
            var box = new CheckedListBox();

            var ex = Assert.Throws<ArgumentOutOfRangeException>(() => box.SetItemCheckState(index, CheckState.Checked));
            Assert.Equal("index", ex.ParamName);
        }

        /// <summary>
        /// Data for the SetItemCheckState test
        /// </summary>
        public static TheoryData<CheckState> SetItemCheckStateData =>
            CommonTestHelper.GetEnumTheoryData<CheckState>();

        [Theory]
        [MemberData(nameof(SetItemCheckStateData))]
        public void CheckedListBox_SetItemCheckState(CheckState expected)
        {
            var box = new CheckedListBox();
            box.Items.Add(new CheckBox(), false);

            box.SetItemCheckState(0, expected);

            Assert.Equal(expected, box.GetItemCheckState(0));
        }

        /// <summary>
        /// Data for the SetItemCheckStateInvalid test
        /// </summary>
        public static TheoryData<CheckState> SetItemCheckStateInvalidData =>
            CommonTestHelper.GetEnumTheoryDataInvalid<CheckState>();

        [Theory]
        [MemberData(nameof(SetItemCheckStateInvalidData))]
        public void CheckedListBox_SetItemCheckStateInvalid(CheckState expected)
        {
            var box = new CheckedListBox();
            box.Items.Add(new CheckBox(), false);

            var ex = Assert.Throws<InvalidEnumArgumentException>(() => box.SetItemCheckState(0,expected));
            Assert.Equal("value", ex.ParamName);
        }

        [Theory]
        [InlineData(true, CheckState.Checked)]
        [InlineData(false, CheckState.Unchecked)]
        public void CheckedListBox_SetItemChecked(bool send, CheckState expected)
        {
            var box = new CheckedListBox();
            box.Items.Add(new CheckBox(), false);

            box.SetItemChecked(0, send);

            Assert.Equal(expected, box.GetItemCheckState(0));
        }


    }
}
