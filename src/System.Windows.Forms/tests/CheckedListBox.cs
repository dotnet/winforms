// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Xunit;
using System.ComponentModel;


namespace System.Windows.Forms.Tests
{
    public class CheckedListBoxTests
    {
        [Fact]
        public void Constructor()
        {
            // act
            var box = new CheckedListBox();

            // assert
            Assert.NotNull(box);   
        }

        /// <summary>
        /// Data for the CheckOnClick test
        /// </summary>
        public static TheoryData<bool> CheckOnClickData =>
            TestHelper.GetBoolTheoryData();

        [Theory]
        [MemberData(nameof(CheckOnClickData))]
        public void CheckOnClick(bool expected)
        {
            // arrange
            var box = new CheckedListBox();

            // act
            box.CheckOnClick = expected;

            // assert
            Assert.Equal(expected, box.CheckOnClick);
        }

        [Fact]
        public void CheckedIndexCollectionNotNull()
        {
            // arrange
            var box = new CheckedListBox();

            // act
            var collection = box.CheckedIndices;

            // assert
            Assert.NotNull(collection);
        }

        [Fact]
        public void CheckedItemCollectionNotNull()
        {
            // arrange
            var box = new CheckedListBox();

            // act
            var collection = box.CheckedItems;

            // assert
            Assert.NotNull(collection);
        }

        /// <summary>
        /// Data for the DisplayMember test
        /// </summary>
        public static TheoryData<string> DisplayMemberData =>
            TestHelper.GetStringTheoryData();

        [Theory]
        [MemberData(nameof(DisplayMemberData))]
        public void DisplayMember(string expected)
        {
            // arrange
            var box = new CheckedListBox();

            // act
            box.DisplayMember = expected;

            // assert
            Assert.Equal(expected, box.DisplayMember);
        }

        [Fact]
        public void DrawModeReturnsNormalOnly()
        {
            // arrange
            var box = new CheckedListBox();

            // act
            var result = box.DrawMode;

            // assert
            Assert.Equal(DrawMode.Normal, result);
        }

        [Theory]
        [InlineData(SelectionMode.None)]
        [InlineData(SelectionMode.One)]
        public void SelectionModeGetSet(SelectionMode expected)
        {
            // arrange
            var box = new CheckedListBox();

            // act
            box.SelectionMode = expected;

            // assert
            Assert.Equal(expected, box.SelectionMode);
        }

        [Theory]
        [InlineData(SelectionMode.MultiSimple)]
        [InlineData(SelectionMode.MultiExtended)]
        public void SelectionModeGetSetInvalidFromEnum(SelectionMode expected)
        {
            // arrange
            var box = new CheckedListBox();

            var ex = Assert.Throws<ArgumentException>(() => box.SelectionMode = expected);
        }

        /// <summary>
        /// Data for the SelectionModeGetSetInvalid test
        /// </summary>
        public static TheoryData<SelectionMode> SelectionModeGetSetInvalidData =>
            TestHelper.GetEnumTheoryDataInvalid<SelectionMode>();

        [Theory]
        [MemberData(nameof(SelectionModeGetSetInvalidData))]
        public void CheckStateGetSetInvalid(SelectionMode expected)
        {
            // arrange
            var box = new CheckedListBox();

            var ex = Assert.Throws<InvalidEnumArgumentException>(() => box.SelectionMode = expected);
            Assert.Equal("value", ex.ParamName);
        }

        /// <summary>
        /// Data for the ThreeDCheckBoxes test
        /// </summary>
        public static TheoryData<bool> ThreeDCheckBoxesData =>
            TestHelper.GetBoolTheoryData();

        [Theory]
        [MemberData(nameof(ThreeDCheckBoxesData))]
        public void ThreeDCheckBoxes(bool expected)
        {
            // arrange
            var box = new CheckedListBox();

            // act
            box.ThreeDCheckBoxes = expected;

            // assert
            Assert.Equal(expected, box.ThreeDCheckBoxes);
        }

        /// <summary>
        /// Data for the ValueMember test
        /// </summary>
        public static TheoryData<string> ValueMemberData =>
            TestHelper.GetStringTheoryData();

        [Theory]
        [MemberData(nameof(ValueMemberData))]
        public void ValueMember(string expected)
        {
            // arrange
            var box = new CheckedListBox();

            // act
            box.ValueMember = expected;

            // assert
            Assert.Equal(expected, box.ValueMember);
        }

        /// <summary>
        /// Data for the Padding test
        /// </summary>
        public static TheoryData<Padding> PaddingData =>
            TestHelper.GetPaddingTheoryData();

        [Theory]
        [MemberData(nameof(PaddingData))]
        public void Padding(Padding expected)
        {
            // arrange
            var box = new CheckedListBox();

            // act
            box.Padding = expected;

            // assert
            Assert.Equal(expected, box.Padding);
        }

        /// <summary>
        /// Data for the PaddingInvalid test
        /// </summary>
        public static TheoryData<Padding> PaddingDataInvalid =>
            TestHelper.GetPaddingTheoryDataInvalid();

        [Theory]
        [MemberData(nameof(PaddingDataInvalid))]
        public void PaddingInvalid(Padding attempted)
        {
            // arrange
            var box = new CheckedListBox();

            // act
            box.Padding = attempted;

            // assert
            Assert.NotEqual(attempted, box.Padding); //paddings cannot be negative
            Assert.Equal(new Padding(), box.Padding);
        }

        [Theory]
        [InlineData(1)]
        [InlineData(-1)]
        [InlineData(int.MaxValue)]
        public void GetItemCheckStateOutOfRange(int index)
        {
            // arrange
            var box = new CheckedListBox();

            var ex = Assert.Throws<ArgumentOutOfRangeException>(() => box.GetItemCheckState(index));
            Assert.Equal("index", ex.ParamName);
        }

        [Theory]
        [InlineData(1)]
        [InlineData(-1)]
        [InlineData(int.MaxValue)]
        public void SetItemCheckStateOutOfRange(int index)
        {
            // arrange
            var box = new CheckedListBox();

            var ex = Assert.Throws<ArgumentOutOfRangeException>(() => box.SetItemCheckState(index, CheckState.Checked));
            Assert.Equal("index", ex.ParamName);
        }

        /// <summary>
        /// Data for the SetItemCheckState test
        /// </summary>
        public static TheoryData<CheckState> SetItemCheckStateData =>
            TestHelper.GetEnumTheoryData<CheckState>();

        [Theory]
        [MemberData(nameof(SetItemCheckStateData))]
        public void SetItemCheckState(CheckState expected)
        {
            // arrange
            var box = new CheckedListBox();
            box.Items.Add(new CheckBox(), false);

            // act
            box.SetItemCheckState(0, expected);

            // assert
            Assert.Equal(expected, box.GetItemCheckState(0));
        }

        /// <summary>
        /// Data for the SetItemCheckStateInvalid test
        /// </summary>
        public static TheoryData<CheckState> SetItemCheckStateInvalidData =>
            TestHelper.GetEnumTheoryDataInvalid<CheckState>();

        [Theory]
        [MemberData(nameof(SetItemCheckStateInvalidData))]
        public void SetItemCheckStateInvalid(CheckState expected)
        {
            // arrange
            var box = new CheckedListBox();
            box.Items.Add(new CheckBox(), false);

            var ex = Assert.Throws<InvalidEnumArgumentException>(() => box.SetItemCheckState(0,expected));
            Assert.Equal("value", ex.ParamName);
        }

        [Theory]
        [InlineData(true, CheckState.Checked)]
        [InlineData(false, CheckState.Unchecked)]
        public void SetItemChecked(bool send, CheckState expected)
        {
            // arrange
            var box = new CheckedListBox();
            box.Items.Add(new CheckBox(), false);

            // act
            box.SetItemChecked(0, send);

            // assert
            Assert.Equal(expected, box.GetItemCheckState(0));
        }


    }
}
