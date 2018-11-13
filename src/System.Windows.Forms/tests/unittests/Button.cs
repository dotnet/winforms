// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Xunit;
using System.ComponentModel;

namespace System.Windows.Forms.Tests
{
    using Size = Drawing.Size;

    public class ButtonTests
    {
        [Fact]
        public void Constructor()
        {
            // act
            var button = new Button();

            // assert
            Assert.NotNull(button);
        }

        /// <summary>
        /// Data for the AutoSizeModeGetSet test
        /// </summary>
        public static TheoryData<AutoSizeMode> AutoSizeModeGetSetData =>
            TestHelper.GetEnumTheoryData<AutoSizeMode>();

        [Theory]
        [MemberData(nameof(AutoSizeModeGetSetData))]
        public void AutoSizeModeGetSet(AutoSizeMode expected)
        {
            // arrange
            var button = new Button();

            // act
            button.AutoSizeMode = expected;

            // assert
            Assert.Equal(expected, button.AutoSizeMode);
        }

        /// <summary>
        /// Data for the AutoSizeModeGetSetInvalid test
        /// </summary>
        public static TheoryData<AutoSizeMode> AutoSizeModeGetSetInvalidData =>
            TestHelper.GetEnumTheoryDataInvalid<AutoSizeMode>();

        [Theory]
        [MemberData(nameof(AutoSizeModeGetSetInvalidData))]
        public void AutoSizeModeGetSetInvalid(AutoSizeMode expected)
        {
            // arrange
            var button = new Button();

            // act & assert
            var ex = Assert.Throws<InvalidEnumArgumentException>(() => button.AutoSizeMode = expected);
            Assert.Equal("value", ex.ParamName);
        }

        [Fact]
        public void CreateFlatAdapter()
        {
            // arrange
            var button = new Button();

            // act
            var adaptor = button.CreateFlatAdapter();

            // assert
            Assert.NotNull(adaptor);
        }

        [Fact]
        public void CreatePopupAdapter()
        {
            // arrange
            var button = new Button();

            // act
            var adaptor = button.CreatePopupAdapter();

            // assert
            Assert.NotNull(adaptor);
        }

        [Fact]
        public void CreateStandardAdapter()
        {
            // arrange
            var button = new Button();

            // act
            var adaptor = button.CreateStandardAdapter();

            // assert
            Assert.NotNull(adaptor);
        }

        public static TheoryData<Button, Size, Size> GetPreferredSizeCoreData =>
            GetPreferredSizeCoreTestData();

        [Theory]
        [MemberData(nameof(GetPreferredSizeCoreData))]
        public void GetPreferredSizeCore(Button button, Size proposed, Size expected)
        {
            // act
            var actual = button.GetPreferredSizeCore(proposed);

            // assert
            Assert.Equal(expected, actual);
        }

        /// <summary>
        /// Data for the DialogResultGetSet test
        /// </summary>
        public static TheoryData<DialogResult> DialogResultGetSetData =>
            TestHelper.GetEnumTheoryData<DialogResult>();

        [Theory]
        [MemberData(nameof(DialogResultGetSetData))]
        public void DialogResultGetSet(DialogResult expected)
        {
            // arrange
            var button = new Button();

            // act
            button.DialogResult = expected;

            // assert
            Assert.Equal(expected, button.DialogResult);
        }

        /// <summary>
        /// Data for the DialogResultGetSetInvalid test
        /// </summary>
        public static TheoryData<DialogResult> DialogResultGetSetInvalidData =>
            TestHelper.GetEnumTheoryDataInvalid<DialogResult>();

        [Theory]
        [MemberData(nameof(DialogResultGetSetInvalidData))]
        public void DialogResultGetSetInvalid(DialogResult expected)
        {
            // arrange
            var button = new Button();

            // act & assert
            var ex = Assert.Throws<InvalidEnumArgumentException>(() => button.DialogResult = expected);
            Assert.Equal("value", ex.ParamName);
        }

        /// <summary>
        /// Data for the NotifyDefault test
        /// </summary>
        public static TheoryData<bool> NotifyDefaultData =>
            TestHelper.GetBoolTheoryData();

        [Theory]
        [MemberData(nameof(NotifyDefaultData))]
        public void NotifyDefault(bool expected)
        {
            // arrange
            var button = new Button();

            // act
            button.NotifyDefault(expected);

            // assert
            Assert.Equal(expected, button.IsDefault);
        }

        [Fact]
        public void PerformClick()
        {
            // arrange
            var wasClicked = false;
            var button = new Button();
            button.Click += (sender, args) => wasClicked = true;

            // act
            button.PerformClick();

            // assert
            Assert.True(wasClicked);
        }

        [Fact]
        public void ToStringTest()
        {
            // arrange
            var button = new Button();
            button.Text = "Hello World!";
            var expected = "System.Windows.Forms.Button, Text: " + button.Text;

            // act
            var actual = button.ToString();

            // assert
            Assert.Equal(expected, actual);
        }

        // helper method to generate data for the GetPreferredSizeCore test
        private static TheoryData<Button, Size, Size> GetPreferredSizeCoreTestData()
        {
            var data = new TheoryData<Button, Size, Size>();

            // first code path is FlatStyle != FlatStyle.System, AutoSizeMode = GrowAndShrink
            var b1 = new Button();
            b1.FlatStyle = FlatStyle.Flat;
            b1.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            var proposed1 = new Size(5, 5);
            var expected1 = new Size(8, 8);
            data.Add(b1, proposed1, expected1);

            // second code path is FlatStyle != FlatStyle.System, AutoSizeMode != GrowAndShrink
            var b2 = new Button();
            b2.FlatStyle = FlatStyle.Flat;
            b2.AutoSizeMode = AutoSizeMode.GrowOnly;
            var proposed2 = new Size(5, 5);
            var expected2 = new Size(75, 23);
            data.Add(b2, proposed2, expected2);

            // third code path is FlatStyle == FlatStyle.System, button systemSize.Width is invalid
            // and AutoSizeMode = GrowAndShrink
            var b3 = new Button();
            // text and font need to be set since the code measures the size of the text
            b3.Text = "Hello World!";
            b3.Font = new Drawing.Font(Drawing.FontFamily.GenericMonospace, 1.5f);
            b3.FlatStyle = FlatStyle.System;
            b3.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            var proposed3 = new Size(100, 200);
            var expected3 = new Size(28, 12);
            data.Add(b3, proposed3, expected3);

            // fourth code path is FlatStyle == FlatStyle.System, button systemSize.Width is valid
            // and AutoSizeMode != GrowAndShrink
            var b4 = new Button();
            // text and font need to be set since the code measures the size of the text
            b4.Text = "Hello World!";
            b4.Font = new Drawing.Font(Drawing.FontFamily.GenericMonospace, 1.5f);
            b4.FlatStyle = FlatStyle.System;
            b4.AutoSizeMode = AutoSizeMode.GrowOnly;
            var proposed4 = new Size(100, 200);
            // call getPreferredSizeCore once so the systemSize gets set
            b4.GetPreferredSizeCore(proposed4);
            var expected4 = new Size(75, 23);
            data.Add(b4, proposed4, expected4);

            return data;
        }
    }
}
