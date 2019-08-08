// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Xunit;
using System.ComponentModel;
using WinForms.Common.Tests;

namespace System.Windows.Forms.Tests
{
    using Size = Drawing.Size;

    public class ButtonTests
    {
        [Fact]
        public void Button_Constructor()
        {
            var button = new Button();

            Assert.NotNull(button);
        }

        /// <summary>
        ///  Data for the AutoSizeModeGetSet test
        /// </summary>
        public static TheoryData<AutoSizeMode> AutoSizeModeGetSetData =>
            CommonTestHelper.GetEnumTheoryData<AutoSizeMode>();

        [Theory]
        [MemberData(nameof(AutoSizeModeGetSetData))]
        public void Button_AutoSizeModeGetSet(AutoSizeMode expected)
        {
            var button = new Button
            {
                AutoSizeMode = expected
            };

            Assert.Equal(expected, button.AutoSizeMode);
        }

        /// <summary>
        ///  Data for the AutoSizeModeGetSetInvalid test
        /// </summary>
        public static TheoryData<AutoSizeMode> AutoSizeModeGetSetInvalidData =>
            CommonTestHelper.GetEnumTheoryDataInvalid<AutoSizeMode>();

        [Theory]
        [MemberData(nameof(AutoSizeModeGetSetInvalidData))]
        public void Button_AutoSizeModeGetSetInvalid(AutoSizeMode expected)
        {
            var button = new Button();

            // act & assert
            InvalidEnumArgumentException ex = Assert.Throws<InvalidEnumArgumentException>(() => button.AutoSizeMode = expected);
            Assert.Equal("value", ex.ParamName);
        }

        [Fact]
        public void Button_CreateFlatAdapter()
        {
            var button = new Button();

            ButtonInternal.ButtonBaseAdapter adaptor = button.CreateFlatAdapter();

            Assert.NotNull(adaptor);
        }

        [Fact]
        public void Button_CreatePopupAdapter()
        {
            var button = new Button();

            ButtonInternal.ButtonBaseAdapter adaptor = button.CreatePopupAdapter();

            Assert.NotNull(adaptor);
        }

        [Fact]
        public void Button_CreateStandardAdapter()
        {
            var button = new Button();

            ButtonInternal.ButtonBaseAdapter adaptor = button.CreateStandardAdapter();

            Assert.NotNull(adaptor);
        }

        public static TheoryData<Button, Size, Size> GetPreferredSizeCoreData =>
            GetPreferredSizeCoreTestData();

        [Theory]
        [MemberData(nameof(GetPreferredSizeCoreData))]
        public void Button_GetPreferredSizeCore(Button button, Size proposed, Size expected)
        {
            Size actual = button.GetPreferredSizeCore(proposed);

            Assert.Equal(expected, actual);
        }

        /// <summary>
        ///  Data for the DialogResultGetSet test
        /// </summary>
        public static TheoryData<DialogResult> DialogResultGetSetData =>
            CommonTestHelper.GetEnumTheoryData<DialogResult>();

        [Theory]
        [MemberData(nameof(DialogResultGetSetData))]
        public void Button_DialogResultGetSet(DialogResult expected)
        {
            var button = new Button
            {
                DialogResult = expected
            };

            Assert.Equal(expected, button.DialogResult);
        }

        /// <summary>
        ///  Data for the DialogResultGetSetInvalid test
        /// </summary>
        public static TheoryData<DialogResult> DialogResultGetSetInvalidData =>
            CommonTestHelper.GetEnumTheoryDataInvalid<DialogResult>();

        [Theory]
        [MemberData(nameof(DialogResultGetSetInvalidData))]
        public void Button_DialogResultGetSetInvalid(DialogResult expected)
        {
            var button = new Button();

            // act & assert
            InvalidEnumArgumentException ex = Assert.Throws<InvalidEnumArgumentException>(() => button.DialogResult = expected);
            Assert.Equal("value", ex.ParamName);
        }

        /// <summary>
        ///  Data for the NotifyDefault test
        /// </summary>
        public static TheoryData<bool> NotifyDefaultData =>
            CommonTestHelper.GetBoolTheoryData();

        [Theory]
        [MemberData(nameof(NotifyDefaultData))]
        public void Button_NotifyDefault(bool expected)
        {
            var button = new Button();

            button.NotifyDefault(expected);

            Assert.Equal(expected, button.IsDefault);
        }

        [Fact]
        public void Button_PerformClick()
        {
            var wasClicked = false;
            var button = new Button();
            button.Click += (sender, args) => wasClicked = true;

            button.PerformClick();

            Assert.True(wasClicked);
        }

        [Fact]
        public void Button_ToStringTest()
        {
            var button = new Button
            {
                Text = "Hello World!"
            };
            var expected = "System.Windows.Forms.Button, Text: " + button.Text;

            var actual = button.ToString();

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Button_ProductName()
        {
            var button = new Button();

            Assert.Equal("Microsoft\u00AE .NET", button.ProductName);
        }

        // helper method to generate data for the GetPreferredSizeCore test
        private static TheoryData<Button, Size, Size> GetPreferredSizeCoreTestData()
        {
            var data = new TheoryData<Button, Size, Size>();

            // first code path is FlatStyle != FlatStyle.System, AutoSizeMode = GrowAndShrink
            var b1 = new Button
            {
                FlatStyle = FlatStyle.Flat,
                AutoSizeMode = AutoSizeMode.GrowAndShrink
            };
            var proposed1 = new Size(5, 5);
            var expected1 = new Size(8, 8);
            data.Add(b1, proposed1, expected1);

            // second code path is FlatStyle != FlatStyle.System, AutoSizeMode != GrowAndShrink
            var b2 = new Button
            {
                FlatStyle = FlatStyle.Flat,
                AutoSizeMode = AutoSizeMode.GrowOnly
            };
            var proposed2 = new Size(5, 5);
            var expected2 = new Size(75, 23);
            data.Add(b2, proposed2, expected2);

            // third code path is FlatStyle == FlatStyle.System, button systemSize.Width is invalid
            // and AutoSizeMode = GrowAndShrink
            var b3 = new Button
            {
                // text and font need to be set since the code measures the size of the text
                Text = "Hello World!",
                Font = new Drawing.Font(Drawing.FontFamily.GenericMonospace, 1.5f),
                FlatStyle = FlatStyle.System,
                AutoSizeMode = AutoSizeMode.GrowAndShrink
            };
            var proposed3 = new Size(100, 200);
            var expected3 = new Size(28, 12);
            data.Add(b3, proposed3, expected3);

            // fourth code path is FlatStyle == FlatStyle.System, button systemSize.Width is valid
            // and AutoSizeMode != GrowAndShrink
            var b4 = new Button
            {
                // text and font need to be set since the code measures the size of the text
                Text = "Hello World!",
                Font = new Drawing.Font(Drawing.FontFamily.GenericMonospace, 1.5f),
                FlatStyle = FlatStyle.System,
                AutoSizeMode = AutoSizeMode.GrowOnly
            };
            var proposed4 = new Size(100, 200);
            // call getPreferredSizeCore once so the systemSize gets set
            b4.GetPreferredSizeCore(proposed4);
            var expected4 = new Size(75, 23);
            data.Add(b4, proposed4, expected4);

            return data;
        }
    }
}
