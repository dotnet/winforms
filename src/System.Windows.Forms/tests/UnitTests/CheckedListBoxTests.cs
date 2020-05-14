// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Xunit;
using System.ComponentModel;
using System.Collections.Generic;
using WinForms.Common.Tests;
using System.Drawing;

namespace System.Windows.Forms.Tests
{
    public class CheckedListBoxTests : IClassFixture<ThreadExceptionFixture>
    {
        [WinFormsFact]
        public void CheckedListBox_Constructor()
        {
            using var box = new CheckedListBox();

            Assert.NotNull(box);
        }

        /// <summary>
        ///  Data for the CheckOnClick test
        /// </summary>
        public static TheoryData<bool> CheckOnClickData =>
            CommonTestHelper.GetBoolTheoryData();

        [WinFormsTheory]
        [MemberData(nameof(CheckOnClickData))]
        public void CheckedListBox_CheckOnClick(bool expected)
        {
            using var box = new CheckedListBox
            {
                CheckOnClick = expected
            };

            Assert.Equal(expected, box.CheckOnClick);
        }

        [WinFormsFact]
        public void CheckedListBox_CheckedIndexCollectionNotNull()
        {
            using var box = new CheckedListBox();

            CheckedListBox.CheckedIndexCollection collection = box.CheckedIndices;

            Assert.NotNull(collection);
        }

        [WinFormsFact]
        public void CheckedListBox_CheckedItemCollectionNotNull()
        {
            using var box = new CheckedListBox();

            CheckedListBox.CheckedItemCollection collection = box.CheckedItems;

            Assert.NotNull(collection);
        }

        /// <summary>
        ///  Data for the DisplayMember test
        /// </summary>
        public static TheoryData<string> DisplayMemberData =>
            CommonTestHelper.GetStringTheoryData();

        [WinFormsTheory]
        [MemberData(nameof(DisplayMemberData))]
        public void CheckedListBox_DisplayMember(string expected)
        {
            using var box = new CheckedListBox
            {
                DisplayMember = expected
            };

            Assert.Equal(expected, box.DisplayMember);
        }

        [WinFormsFact]
        public void CheckedListBox_DrawModeReturnsNormalOnly()
        {
            using var box = new CheckedListBox();

            DrawMode result = box.DrawMode;

            Assert.Equal(DrawMode.Normal, result);
        }

        [WinFormsTheory]
        [InlineData(SelectionMode.None)]
        [InlineData(SelectionMode.One)]
        public void CheckedListBox_SelectionModeGetSet(SelectionMode expected)
        {
            using var box = new CheckedListBox
            {
                SelectionMode = expected
            };

            Assert.Equal(expected, box.SelectionMode);
        }

        [WinFormsTheory]
        [InlineData(SelectionMode.MultiSimple)]
        [InlineData(SelectionMode.MultiExtended)]
        public void CheckedListBox_SelectionModeGetSetInvalidFromEnum(SelectionMode expected)
        {
            using var box = new CheckedListBox();

            ArgumentException ex = Assert.Throws<ArgumentException>(() => box.SelectionMode = expected);
        }

        /// <summary>
        ///  Data for the SelectionModeGetSetInvalid test
        /// </summary>
        public static TheoryData<SelectionMode> SelectionModeGetSetInvalidData =>
            CommonTestHelper.GetEnumTheoryDataInvalid<SelectionMode>();

        [WinFormsTheory]
        [MemberData(nameof(SelectionModeGetSetInvalidData))]
        public void CheckedListBox_CheckStateGetSetInvalid(SelectionMode expected)
        {
            using var box = new CheckedListBox();

            InvalidEnumArgumentException ex = Assert.Throws<InvalidEnumArgumentException>(() => box.SelectionMode = expected);
            Assert.Equal("value", ex.ParamName);
        }

        /// <summary>
        ///  Data for the ThreeDCheckBoxes test
        /// </summary>
        public static TheoryData<bool> ThreeDCheckBoxesData =>
            CommonTestHelper.GetBoolTheoryData();

        [WinFormsTheory]
        [MemberData(nameof(ThreeDCheckBoxesData))]
        public void CheckedListBox_ThreeDCheckBoxes(bool expected)
        {
            using var box = new CheckedListBox
            {
                ThreeDCheckBoxes = expected
            };

            Assert.Equal(expected, box.ThreeDCheckBoxes);
        }

        /// <summary>
        ///  Data for the ValueMember test
        /// </summary>
        public static TheoryData<string> ValueMemberData =>
            CommonTestHelper.GetStringTheoryData();

        [WinFormsTheory]
        [MemberData(nameof(ValueMemberData))]
        public void CheckedListBox_ValueMember(string expected)
        {
            using var box = new CheckedListBox
            {
                ValueMember = expected
            };

            Assert.Equal(expected, box.ValueMember);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetPaddingNormalizedTheoryData))]
        public void CheckedListBox_Padding_Set_GetReturnsExpected(Padding value, Padding expected)
        {
            using var control = new CheckedListBox
            {
                Padding = value
            };
            Assert.Equal(expected, control.Padding);
            Assert.False(control.IsHandleCreated);

            // Set same.
            control.Padding = value;
            Assert.Equal(expected, control.Padding);
            Assert.False(control.IsHandleCreated);
        }

        public static IEnumerable<object[]> Padding_SetWithHandle_TestData()
        {
            yield return new object[] { new Padding(), new Padding(), 0, 0 };
            yield return new object[] { new Padding(1, 2, 3, 4), new Padding(1, 2, 3, 4), 1, 1 };
            yield return new object[] { new Padding(1), new Padding(1), 1, 1 };
            yield return new object[] { new Padding(-1, -2, -3, -4), Padding.Empty, 1, 2 };
        }

        [WinFormsTheory]
        [MemberData(nameof(Padding_SetWithHandle_TestData))]
        public void CheckedListBox_Padding_SetWithHandle_GetReturnsExpected(Padding value, Padding expected, int expectedInvalidatedCallCount1, int expectedInvalidatedCallCount2)
        {
            using var control = new CheckedListBox();
            Assert.NotEqual(IntPtr.Zero, control.Handle);
            int invalidatedCallCount = 0;
            control.Invalidated += (sender, e) => invalidatedCallCount++;
            int styleChangedCallCount = 0;
            control.StyleChanged += (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            control.HandleCreated += (sender, e) => createdCallCount++;

            control.Padding = value;
            Assert.Equal(expected, control.Padding);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(expectedInvalidatedCallCount1, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Set same.
            control.Padding = value;
            Assert.Equal(expected, control.Padding);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(expectedInvalidatedCallCount2, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
        }

        [WinFormsFact]
        public void CheckedListBox_Padding_SetWithHandler_CallsPaddingChanged()
        {
            using var control = new CheckedListBox();
            int callCount = 0;
            EventHandler handler = (sender, e) =>
            {
                Assert.Equal(control, sender);
                Assert.Equal(EventArgs.Empty, e);
                callCount++;
            };
            control.PaddingChanged += handler;

            // Set different.
            var padding1 = new Padding(1);
            control.Padding = padding1;
            Assert.Equal(padding1, control.Padding);
            Assert.Equal(1, callCount);

            // Set same.
            control.Padding = padding1;
            Assert.Equal(padding1, control.Padding);
            Assert.Equal(1, callCount);

            // Set different.
            var padding2 = new Padding(2);
            control.Padding = padding2;
            Assert.Equal(padding2, control.Padding);
            Assert.Equal(2, callCount);

            // Remove handler.
            control.PaddingChanged -= handler;
            control.Padding = padding1;
            Assert.Equal(padding1, control.Padding);
            Assert.Equal(2, callCount);
        }

        [WinFormsTheory]
        [InlineData(1)]
        [InlineData(-1)]
        [InlineData(int.MaxValue)]
        public void CheckedListBox_GetItemCheckStateOutOfRange(int index)
        {
            using var box = new CheckedListBox();

            ArgumentOutOfRangeException ex = Assert.Throws<ArgumentOutOfRangeException>(() => box.GetItemCheckState(index));
            Assert.Equal("index", ex.ParamName);
        }

        [WinFormsTheory]
        [InlineData(1)]
        [InlineData(-1)]
        [InlineData(int.MaxValue)]
        public void CheckedListBox_SetItemCheckStateOutOfRange(int index)
        {
            using var box = new CheckedListBox();

            ArgumentOutOfRangeException ex = Assert.Throws<ArgumentOutOfRangeException>(() => box.SetItemCheckState(index, CheckState.Checked));
            Assert.Equal("index", ex.ParamName);
        }

        /// <summary>
        ///  Data for the SetItemCheckState test
        /// </summary>
        public static TheoryData<CheckState> SetItemCheckStateData =>
            CommonTestHelper.GetEnumTheoryData<CheckState>();

        [WinFormsTheory]
        [MemberData(nameof(SetItemCheckStateData))]
        public void CheckedListBox_SetItemCheckState(CheckState expected)
        {
            using var box = new CheckedListBox();
            box.Items.Add(new CheckBox(), false);

            box.SetItemCheckState(0, expected);

            Assert.Equal(expected, box.GetItemCheckState(0));
        }

        /// <summary>
        ///  Data for the SetItemCheckStateInvalid test
        /// </summary>
        public static TheoryData<CheckState> SetItemCheckStateInvalidData =>
            CommonTestHelper.GetEnumTheoryDataInvalid<CheckState>();

        [WinFormsTheory]
        [MemberData(nameof(SetItemCheckStateInvalidData))]
        public void CheckedListBox_SetItemCheckStateInvalid(CheckState expected)
        {
            using var box = new CheckedListBox();
            box.Items.Add(new CheckBox(), false);

            InvalidEnumArgumentException ex = Assert.Throws<InvalidEnumArgumentException>(() => box.SetItemCheckState(0, expected));
            Assert.Equal("value", ex.ParamName);
        }

        [WinFormsTheory]
        [InlineData(true, CheckState.Checked)]
        [InlineData(false, CheckState.Unchecked)]
        public void CheckedListBox_SetItemChecked(bool send, CheckState expected)
        {
            using var box = new CheckedListBox();
            box.Items.Add(new CheckBox(), false);

            box.SetItemChecked(0, send);

            Assert.Equal(expected, box.GetItemCheckState(0));
        }

        public static IEnumerable<object[]> OnDrawItem_TestData()
        {
            yield return new object[] { null, Rectangle.Empty, 0, DrawItemState.Default, Color.Red, Color.Blue };
            yield return new object[] { null, new Rectangle(1, 2, 3, 4), 1, DrawItemState.None, Color.Red, Color.Blue };
            yield return new object[] { null, new Rectangle(1, 2, 3, 4), 1, DrawItemState.Checked, Color.Red, Color.Blue };
            yield return new object[] { new Font("Arial", 8.25f), new Rectangle(10, 20, 30, 40), 1, DrawItemState.Default, Color.Red, Color.Blue };
        }

        [WinFormsTheory]
        [MemberData(nameof(OnDrawItem_TestData))]
        public void CheckedListBox_OnDrawItem_Invoke_Success(Font font, Rectangle rect, int index, DrawItemState state, Color foreColor, Color backColor)
        {
            using var control = new SubCheckedListBox();
            control.Items.Add("item1");
            control.Items.Add("item2");

            using var image = new Bitmap(10, 10);
            using Graphics graphics = Graphics.FromImage(image);
            var e = new DrawItemEventArgs(graphics, font, rect, index, state, foreColor, backColor);
            control.OnDrawItem(e);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [MemberData(nameof(OnDrawItem_TestData))]
        public void CheckedListBox_OnDrawItem_InvokeWithHandle_Success(Font font, Rectangle rect, int index, DrawItemState state, Color foreColor, Color backColor)
        {
            using var control = new SubCheckedListBox();
            control.Items.Add("item1");
            control.Items.Add("item2");
            Assert.NotEqual(IntPtr.Zero, control.Handle);
            int invalidatedCallCount = 0;
            control.Invalidated += (sender, e) => invalidatedCallCount++;
            int styleChangedCallCount = 0;
            control.StyleChanged += (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            control.HandleCreated += (sender, e) => createdCallCount++;

            using var image = new Bitmap(10, 10);
            using Graphics graphics = Graphics.FromImage(image);
            var e = new DrawItemEventArgs(graphics, font, rect, index, state, foreColor, backColor);
            control.OnDrawItem(e);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
        }

        [WinFormsFact]
        public void CheckedListBox_OnDrawItem_NullE_ThrowsNullReferenceException()
        {
            using var control = new SubCheckedListBox();
            Assert.Throws<NullReferenceException>(() => control.OnDrawItem(null));
        }

        [WinFormsFact]
        public void CheckedListBox_OnDrawItem_NegativeEIndex_Success()
        {
            using var control = new SubCheckedListBox();
            using var image = new Bitmap(10, 10);
            using Graphics graphics = Graphics.FromImage(image);
            var e = new DrawItemEventArgs(graphics, null, new Rectangle(1, 2, 3, 4), -1, DrawItemState.Default);
            control.OnDrawItem(e);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsFact]
        public void CheckedListBox_OnDrawItem_LargeEIndexEmpty_Success()
        {
            using var control = new SubCheckedListBox();
            using var image = new Bitmap(10, 10);
            using Graphics graphics = Graphics.FromImage(image);
            var e = new DrawItemEventArgs(graphics, null, new Rectangle(1, 2, 3, 4), 1, DrawItemState.Default);
            control.OnDrawItem(e);
            Assert.True(control.IsHandleCreated);
        }

        [WinFormsFact]
        public void CheckedListBox_OnDrawItem_LargeEIndexNotEmpty_Success()
        {
            using var control = new SubCheckedListBox();
            control.Items.Add("item1");

            using var image = new Bitmap(10, 10);
            using Graphics graphics = Graphics.FromImage(image);
            var e = new DrawItemEventArgs(graphics, null, new Rectangle(1, 2, 3, 4), 2, DrawItemState.Default);
            control.OnDrawItem(e);
            Assert.True(control.IsHandleCreated);
        }

        private class SubCheckedListBox : CheckedListBox
        {
            public new void OnDrawItem(DrawItemEventArgs e) => base.OnDrawItem(e);
        }
    }
}
