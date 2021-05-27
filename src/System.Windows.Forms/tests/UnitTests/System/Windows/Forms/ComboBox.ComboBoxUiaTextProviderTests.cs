// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms.Automation;
using Xunit;
using static Interop;
using static Interop.User32;

namespace System.Windows.Forms.Tests
{
    public class ComboBox_ComboBoxUiaTextProviderTests : IClassFixture<ThreadExceptionFixture>
    {
        [WinFormsTheory]
        [InlineData(ComboBoxStyle.DropDown)]
        [InlineData(ComboBoxStyle.Simple)]
        public void ComboBoxUiaTextProvider_Ctor_DoesntCreateControlHandle(ComboBoxStyle dropDownStyle)
        {
            using (new NoAssertContext())
            {
                using ComboBox comboBox = new ComboBox() { DropDownStyle = dropDownStyle };

                Assert.False(comboBox.IsHandleCreated);
                Assert.Null(comboBox.TestAccessor().Dynamic._childEdit);

                ComboBox.ComboBoxUiaTextProvider provider = new ComboBox.ComboBoxUiaTextProvider(comboBox);

                Assert.False(comboBox.IsHandleCreated);
                Assert.Null(comboBox.TestAccessor().Dynamic._childEdit);
            }
        }

        [WinFormsFact]
        public void ComboBoxUiaTextProvider_Ctor_ThrowsException_IfOwnerIsNull()
        {
            Assert.Throws<ArgumentNullException>(() => new ComboBox.ComboBoxUiaTextProvider(null));
        }

        [WinFormsTheory]
        [InlineData(ComboBoxStyle.DropDown)]
        [InlineData(ComboBoxStyle.Simple)]
        public void ComboBoxUiaTextProvider_IsMultiline_IsFalse(ComboBoxStyle dropDownStyle)
        {
            using (new NoAssertContext())
            {
                using ComboBox comboBox = new ComboBox() { DropDownStyle = dropDownStyle };
                ComboBox.ComboBoxUiaTextProvider provider = new ComboBox.ComboBoxUiaTextProvider(comboBox);

                Assert.False(provider.IsMultiline);
                Assert.False(comboBox.IsHandleCreated);
                Assert.Null(comboBox.TestAccessor().Dynamic._childEdit);
            }
        }

        [WinFormsTheory]
        [InlineData(ComboBoxStyle.DropDown)]
        [InlineData(ComboBoxStyle.Simple)]
        public void ComboBoxUiaTextProvider_IsReadOnly_IsFalse(ComboBoxStyle dropDownStyle)
        {
            using (new NoAssertContext())
            {
                using ComboBox comboBox = new ComboBox() { DropDownStyle = dropDownStyle };
                ComboBox.ComboBoxUiaTextProvider provider = new ComboBox.ComboBoxUiaTextProvider(comboBox);

                Assert.False(provider.IsReadOnly);
                Assert.False(comboBox.IsHandleCreated);
                Assert.Null(comboBox.TestAccessor().Dynamic._childEdit);
            }
        }

        [WinFormsTheory]
        [InlineData(ComboBoxStyle.DropDown)]
        [InlineData(ComboBoxStyle.Simple)]
        public void ComboBoxUiaTextProvider_IsScrollable_IsTrue(ComboBoxStyle dropDownStyle)
        {
            using ComboBox comboBox = new ComboBox() { DropDownStyle = dropDownStyle };
            comboBox.CreateControl();
            ComboBox.ComboBoxUiaTextProvider provider = new ComboBox.ComboBoxUiaTextProvider(comboBox);

            Assert.True(provider.IsScrollable);
            Assert.True(comboBox.IsHandleCreated);
            Assert.NotNull(comboBox.TestAccessor().Dynamic._childEdit);
        }

        [WinFormsTheory]
        [InlineData(ComboBoxStyle.DropDown)]
        [InlineData(ComboBoxStyle.Simple)]
        public void ComboBoxUiaTextProvider_IsScrollable_False_WithoutHandle(ComboBoxStyle dropDownStyle)
        {
            using (new NoAssertContext())
            {
                using ComboBox comboBox = new ComboBox() { DropDownStyle = dropDownStyle };
                ComboBox.ComboBoxUiaTextProvider provider = new ComboBox.ComboBoxUiaTextProvider(comboBox);

                Assert.False(provider.IsScrollable);
                Assert.False(comboBox.IsHandleCreated);
                Assert.Null(comboBox.TestAccessor().Dynamic._childEdit);
            }
        }

        [WinFormsTheory]
        [InlineData(ComboBoxStyle.DropDown)]
        [InlineData(ComboBoxStyle.Simple)]
        public void ComboBoxUiaTextProvider_GetWindowStyle_ReturnsNoneForNotInitializedControl(ComboBoxStyle dropDownStyle)
        {
            using (new NoAssertContext())
            {
                using ComboBox comboBox = new ComboBox() { DropDownStyle = dropDownStyle };
                ComboBox.ComboBoxUiaTextProvider provider = new ComboBox.ComboBoxUiaTextProvider(comboBox);

                Assert.Equal(WS.OVERLAPPED, provider.WindowStyle);
                Assert.False(comboBox.IsHandleCreated);
                Assert.Null(comboBox.TestAccessor().Dynamic._childEdit);
            }
        }

        [WinFormsTheory]
        [InlineData(ComboBoxStyle.DropDown, RightToLeft.Yes, true)]
        [InlineData(ComboBoxStyle.DropDown, RightToLeft.No, false)]
        [InlineData(ComboBoxStyle.Simple, RightToLeft.Yes, true)]
        [InlineData(ComboBoxStyle.Simple, RightToLeft.No, false)]
        public void ComboBoxUiaTextProvider_IsReadingRTL_ReturnsCorrectValue(ComboBoxStyle dropDownStyle, RightToLeft rightToLeft, bool expectedResult)
        {
            using ComboBox comboBox = new ComboBox() { DropDownStyle = dropDownStyle, RightToLeft = rightToLeft };
            comboBox.CreateControl();
            ComboBox.ComboBoxUiaTextProvider provider = new ComboBox.ComboBoxUiaTextProvider(comboBox);

            Assert.Equal(expectedResult, provider.IsReadingRTL);
            Assert.True(comboBox.IsHandleCreated);
            Assert.NotNull(comboBox.TestAccessor().Dynamic._childEdit);
        }

        [WinFormsTheory]
        [InlineData(ComboBoxStyle.DropDown, RightToLeft.Yes)]
        [InlineData(ComboBoxStyle.DropDown, RightToLeft.No)]
        [InlineData(ComboBoxStyle.Simple, RightToLeft.Yes)]
        [InlineData(ComboBoxStyle.Simple, RightToLeft.No)]
        public void ComboBoxUiaTextProvider_IsReadingRTL_ReturnsFalse_WithoutHandle(ComboBoxStyle dropDownStyle, RightToLeft rightToLeft)
        {
            using (new NoAssertContext())
            {
                using ComboBox comboBox = new ComboBox() { DropDownStyle = dropDownStyle, RightToLeft = rightToLeft };
                ComboBox.ComboBoxUiaTextProvider provider = new ComboBox.ComboBoxUiaTextProvider(comboBox);

                Assert.False(provider.IsReadingRTL);
                Assert.False(comboBox.IsHandleCreated);
                Assert.Null(comboBox.TestAccessor().Dynamic._childEdit);
            }
        }

        [WinFormsTheory]
        [InlineData(ComboBoxStyle.DropDown)]
        [InlineData(ComboBoxStyle.Simple)]
        public void ComboBoxUiaTextProvider_DocumentRange_IsNotNull_WorksCorrectly(ComboBoxStyle dropDownStyle)
        {
            using (new NoAssertContext())
            {
                using ComboBox comboBox = new ComboBox() { DropDownStyle = dropDownStyle };
                comboBox.CreateControl();
                ComboBox.ComboBoxUiaTextProvider provider = new ComboBox.ComboBoxUiaTextProvider(comboBox);
                Assert.Equal(comboBox.ChildEditAccessibleObject, provider.DocumentRange.GetEnclosingElement().TestAccessor().Dynamic.publicIAccessible);
                Assert.Equal(provider, provider.DocumentRange.TestAccessor().Dynamic._provider);
                Assert.NotNull(comboBox.TestAccessor().Dynamic._childEdit);
            }
        }

        [WinFormsTheory]
        [InlineData(ComboBoxStyle.DropDown)]
        [InlineData(ComboBoxStyle.Simple)]
        public void ComboBoxUiaTextProvider_DocumentRange_IsNull_ThowException(ComboBoxStyle dropDownStyle)
        {
            using (new NoAssertContext())
            {
                using ComboBox comboBox = new ComboBox() { DropDownStyle = dropDownStyle };
                ComboBox.ComboBoxUiaTextProvider provider = new ComboBox.ComboBoxUiaTextProvider(comboBox);
                Assert.Throws<NullReferenceException>(() => provider.DocumentRange);
            }
        }

        [WinFormsTheory]
        [InlineData(ComboBoxStyle.DropDown)]
        [InlineData(ComboBoxStyle.Simple)]
        public void ComboBoxUiaTextProvider_SupportedTextSelection_IsNotNull(ComboBoxStyle dropDownStyle)
        {
            using (new NoAssertContext())
            {
                using ComboBox comboBox = new ComboBox() { DropDownStyle = dropDownStyle };
                ComboBox.ComboBoxUiaTextProvider provider = new ComboBox.ComboBoxUiaTextProvider(comboBox);
                UiaCore.SupportedTextSelection uiaTextRange = provider.SupportedTextSelection;

                Assert.Equal(UiaCore.SupportedTextSelection.Single, uiaTextRange);
                Assert.False(comboBox.IsHandleCreated);
                Assert.Null(comboBox.TestAccessor().Dynamic._childEdit);
            }
        }

        [WinFormsTheory]
        [InlineData(ComboBoxStyle.DropDown)]
        [InlineData(ComboBoxStyle.Simple)]
        public void ComboBoxUiaTextProvider_GetCaretRange_IsNotNull(ComboBoxStyle dropDownStyle)
        {
            using ComboBox comboBox = new ComboBox() { DropDownStyle = dropDownStyle };
            comboBox.CreateControl();
            ComboBox.ComboBoxUiaTextProvider provider = new ComboBox.ComboBoxUiaTextProvider(comboBox);

            UiaCore.ITextRangeProvider uiaTextRange = provider.GetCaretRange(out _);

            Assert.NotNull(uiaTextRange);
            Assert.True(comboBox.IsHandleCreated);
            Assert.NotNull(comboBox.TestAccessor().Dynamic._childEdit);
        }

        [WinFormsTheory]
        [InlineData(ComboBoxStyle.DropDown)]
        [InlineData(ComboBoxStyle.Simple)]
        public void ComboBoxUiaTextProvider_GetCaretRange_IsNull_IfHandleIsNotCreated(ComboBoxStyle dropDownStyle)
        {
            using (new NoAssertContext())
            {
                using ComboBox comboBox = new ComboBox() { DropDownStyle = dropDownStyle };
                ComboBox.ComboBoxUiaTextProvider provider = new ComboBox.ComboBoxUiaTextProvider(comboBox);

                UiaCore.ITextRangeProvider uiaTextRange = provider.GetCaretRange(out _);

                Assert.Null(uiaTextRange);
                Assert.False(comboBox.IsHandleCreated);
                Assert.Null(comboBox.TestAccessor().Dynamic._childEdit);
            }
        }

        [WinFormsTheory]
        [InlineData(ComboBoxStyle.DropDown)]
        [InlineData(ComboBoxStyle.Simple)]
        public void ComboBoxUiaTextProvider_LinesPerPage_ReturnsMinusOne_WithoutHandle(ComboBoxStyle dropDownStyle)
        {
            using (new NoAssertContext())
            {
                using ComboBox comboBox = new ComboBox() { DropDownStyle = dropDownStyle };
                ComboBox.ComboBoxUiaTextProvider provider = new ComboBox.ComboBoxUiaTextProvider(comboBox);

                Assert.Equal(-1, provider.LinesPerPage);
                Assert.False(comboBox.IsHandleCreated);
                Assert.Null(comboBox.TestAccessor().Dynamic._childEdit);
            }
        }

        [WinFormsTheory]
        [InlineData(ComboBoxStyle.DropDown)]
        [InlineData(ComboBoxStyle.Simple)]
        public void ComboBoxUiaTextProvider_LinesPerPage_ReturnsOne_WithHandle(ComboBoxStyle dropDownStyle)
        {
            using ComboBox comboBox = new ComboBox() { DropDownStyle = dropDownStyle };
            comboBox.CreateControl();
            ComboBox.ComboBoxUiaTextProvider provider = new ComboBox.ComboBoxUiaTextProvider(comboBox);

            Assert.Equal(1, provider.LinesPerPage);
            Assert.True(comboBox.IsHandleCreated);
            Assert.NotNull(comboBox.TestAccessor().Dynamic._childEdit);
        }

        [WinFormsTheory]
        [InlineData(ComboBoxStyle.DropDown, 50, 20, 0)]
        [InlineData(ComboBoxStyle.DropDown, 50, 20, 50)]
        [InlineData(ComboBoxStyle.DropDown, 50, 20, 100)]
        [InlineData(ComboBoxStyle.DropDown, 50, 20, -5)]
        [InlineData(ComboBoxStyle.Simple, 50, 20, 0)]
        [InlineData(ComboBoxStyle.Simple, 50, 20, 50)]
        [InlineData(ComboBoxStyle.Simple, 50, 20, 100)]
        [InlineData(ComboBoxStyle.Simple, 50, 20, -5)]
        public void ComboBoxUiaTextProvider_GetLineFromCharIndex_ReturnsZero(
            ComboBoxStyle dropDownStyle,
            int width,
            int height,
            int charIndex)
        {
            using ComboBox comboBox = new ComboBox() { DropDownStyle = dropDownStyle, Size = new Size(width, height) };
            comboBox.CreateControl();
            comboBox.Items.Add("Some test text for testing GetLineFromCharIndex method");
            comboBox.SelectedIndex = 0;
            ComboBox.ComboBoxUiaTextProvider provider = new ComboBox.ComboBoxUiaTextProvider(comboBox);

            int actualLine = provider.GetLineFromCharIndex(charIndex);

            Assert.Equal(0, actualLine);
            Assert.True(comboBox.IsHandleCreated);
            Assert.NotNull(comboBox.TestAccessor().Dynamic._childEdit);
        }

        [WinFormsTheory]
        [InlineData(ComboBoxStyle.DropDown, 50, 20, 5)]
        [InlineData(ComboBoxStyle.Simple, 50, 20, 5)]
        public void ComboBoxUiaTextProvider_GetLineFromCharIndex_ReturnsMinusOne_WithoutHandle(
            ComboBoxStyle dropDownStyle,
            int width,
            int height,
            int charIndex)
        {
            using (new NoAssertContext())
            {
                using ComboBox comboBox = new ComboBox() { DropDownStyle = dropDownStyle, Size = new Size(width, height) };
                comboBox.Items.Add("Some test text for testing GetLineFromCharIndex method");
                comboBox.SelectedIndex = 0;
                ComboBox.ComboBoxUiaTextProvider provider = new ComboBox.ComboBoxUiaTextProvider(comboBox);

                int actualLine = provider.GetLineFromCharIndex(charIndex);

                Assert.Equal(-1, actualLine);
                Assert.False(comboBox.IsHandleCreated);
                Assert.Null(comboBox.TestAccessor().Dynamic._childEdit);
            }
        }

        public static IEnumerable<object[]> ComboBoxUiaTextProvider_GetLineIndex_TestData()
        {
            foreach (ComboBoxStyle comboBoxStyle in Enum.GetValues(typeof(ComboBoxStyle)))
            {
                if (comboBoxStyle == ComboBoxStyle.DropDownList)
                {
                    continue;
                }

                yield return new object[] { comboBoxStyle, 50, 20, 0 };
                yield return new object[] { comboBoxStyle, 50, 20, 3 };
                yield return new object[] { comboBoxStyle, 50, 50, 3 };
                yield return new object[] { comboBoxStyle, 100, 50, 3 };
                yield return new object[] { comboBoxStyle, 50, 50, 100 };
            }
        }

        [WinFormsTheory]
        [MemberData(nameof(ComboBoxUiaTextProvider_GetLineIndex_TestData))]
        public void ComboBoxUiaTextProvider_GetLineIndex_ReturnsCorrectValue(
            ComboBoxStyle dropDownStyle,
            int width,
            int height,
            int lineIndex)
        {
            using ComboBox comboBox = new ComboBox() { DropDownStyle = dropDownStyle, Size = new Size(width, height) };
            comboBox.CreateControl();
            comboBox.Items.Add("Some test text for testing GetLineIndex method");
            comboBox.SelectedIndex = 0;
            ComboBox.ComboBoxUiaTextProvider provider = new ComboBox.ComboBoxUiaTextProvider(comboBox);

            int actualIndex = provider.GetLineIndex(lineIndex);

            Assert.Equal(0, actualIndex);
            Assert.True(comboBox.IsHandleCreated);
            Assert.NotNull(comboBox.TestAccessor().Dynamic._childEdit);
        }

        [WinFormsTheory]
        [MemberData(nameof(ComboBoxUiaTextProvider_GetLineIndex_TestData))]
        public void ComboBoxUiaTextProvider_GetLineIndex_ReturnsMinusOne_WithoutHandle(
            ComboBoxStyle dropDownStyle,
            int width,
            int height,
            int lineIndex)
        {
            using (new NoAssertContext())
            {
                using ComboBox comboBox = new ComboBox() { DropDownStyle = dropDownStyle, Size = new Size(width, height) };
                comboBox.Items.Add("Some test text for testing GetLineIndex method");
                comboBox.SelectedIndex = 0;
                ComboBox.ComboBoxUiaTextProvider provider = new ComboBox.ComboBoxUiaTextProvider(comboBox);

                int actualIndex = provider.GetLineIndex(lineIndex);

                Assert.Equal(-1, actualIndex);
                Assert.False(comboBox.IsHandleCreated);
                Assert.Null(comboBox.TestAccessor().Dynamic._childEdit);
            }
        }

        [WinFormsTheory]
        [InlineData(ComboBoxStyle.DropDown)]
        [InlineData(ComboBoxStyle.Simple)]
        public void ComboBoxUiaTextProvider_GetLogfont_ReturnsCorrectValue(ComboBoxStyle dropDownStyle)
        {
            using (new NoAssertContext())
            {
                using ComboBox comboBox = new ComboBox() { DropDownStyle = dropDownStyle };
                comboBox.CreateControl();
                ComboBox.ComboBoxUiaTextProvider provider = new ComboBox.ComboBoxUiaTextProvider(comboBox);

                LOGFONTW expected = LOGFONTW.FromFont(comboBox.Font);
                LOGFONTW actual = provider.Logfont;
                Assert.False(string.IsNullOrEmpty(actual.FaceName.ToString()));
                Assert.Equal(expected, actual);
                Assert.True(comboBox.IsHandleCreated);
                Assert.NotNull(comboBox.TestAccessor().Dynamic._childEdit);
            }
        }

        [WinFormsTheory]
        [InlineData(ComboBoxStyle.DropDown)]
        [InlineData(ComboBoxStyle.Simple)]
        public void ComboBoxUiaTextProvider_GetLogfont_ReturnsEmpty_WithoutHandle(ComboBoxStyle dropDownStyle)
        {
            using (new NoAssertContext())
            {
                using ComboBox comboBox = new ComboBox() { DropDownStyle = dropDownStyle };
                ComboBox.ComboBoxUiaTextProvider provider = new ComboBox.ComboBoxUiaTextProvider(comboBox);
                LOGFONTW expected = new LOGFONTW();

                LOGFONTW actual = provider.Logfont;

                Assert.True(string.IsNullOrEmpty(actual.FaceName.ToString()));
                Assert.Equal(expected, actual);
                Assert.False(comboBox.IsHandleCreated);
                Assert.Null(comboBox.TestAccessor().Dynamic._childEdit);
            }
        }

        public static IEnumerable<object[]> ComboBoxUiaTextProvider_GetPositionFromChar_TestData()
        {
            yield return new object[] { ComboBoxStyle.DropDown, new Size(50, 20), "Some test text for testing", 0, new Point(1, 0) };
            yield return new object[] { ComboBoxStyle.DropDown, new Size(50, 20), "Some test text for testing", 15, new Point(79, 0) };

            yield return new object[] { ComboBoxStyle.Simple, new Size(50, 20), "Some test text for testing", 0, new Point(1, 0) };
            yield return new object[] { ComboBoxStyle.Simple, new Size(50, 20), "Some test text for testing", 15, new Point(79, 0) };
        }

        [WinFormsTheory]
        [MemberData(nameof(ComboBoxUiaTextProvider_GetPositionFromChar_TestData))]
        public void ComboBoxUiaTextProvider_GetPositionFromChar_ReturnsCorrectValue(ComboBoxStyle dropdownStyle, Size size, string text, int charIndex, Point expectedPoint)
        {
            using ComboBox comboBox = new ComboBox() { Size = size, DropDownStyle = dropdownStyle };
            comboBox.CreateControl();
            comboBox.Items.Add(text);
            comboBox.SelectedIndex = 0;
            ComboBox.ComboBoxUiaTextProvider provider = new ComboBox.ComboBoxUiaTextProvider(comboBox);

            Point actualPoint = provider.GetPositionFromChar(charIndex);

            Assert.True(actualPoint.X >= expectedPoint.X - 1 || actualPoint.X <= expectedPoint.X + 1);
            Assert.True(actualPoint.Y >= expectedPoint.Y - 1 || actualPoint.Y <= expectedPoint.Y + 1);
            Assert.True(comboBox.IsHandleCreated);
            Assert.NotNull(comboBox.TestAccessor().Dynamic._childEdit);
        }

        public static IEnumerable<object[]> ComboBoxUiaTextProvider_GetPositionFromChar_WithoutHandle_TestData()
        {
            yield return new object[] { ComboBoxStyle.DropDown, new Size(50, 20), "Some test text for testing", 0 };
            yield return new object[] { ComboBoxStyle.DropDown, new Size(50, 20), "Some test text for testing", 15 };

            yield return new object[] { ComboBoxStyle.Simple, new Size(50, 20), "Some test text for testing", 0 };
            yield return new object[] { ComboBoxStyle.Simple, new Size(50, 20), "Some test text for testing", 15 };
        }

        [WinFormsTheory]
        [MemberData(nameof(ComboBoxUiaTextProvider_GetPositionFromChar_WithoutHandle_TestData))]
        public void ComboBoxUiaTextProvider_GetPositionFromChar_ReturnsEmpty_WithoutHandle(ComboBoxStyle dropDownStyle, Size size, string text, int charIndex)
        {
            using (new NoAssertContext())
            {
                using ComboBox comboBox = new ComboBox() { DropDownStyle = dropDownStyle, Size = size };
                comboBox.Items.Add(text);
                comboBox.SelectedIndex = 0;
                ComboBox.ComboBoxUiaTextProvider provider = new ComboBox.ComboBoxUiaTextProvider(comboBox);

                Point actualPoint = provider.GetPositionFromChar(charIndex);

                Assert.Equal(Point.Empty, actualPoint);
                Assert.False(comboBox.IsHandleCreated);
                Assert.Null(comboBox.TestAccessor().Dynamic._childEdit);
            }
        }

        public static IEnumerable<object[]> ComboBoxUiaTextProvider_GetPositionFromCharForUpperRightCorner_ReturnsCorrectValue_TestData()
        {
            foreach (ComboBoxStyle comboBoxStyle in Enum.GetValues(typeof(ComboBoxStyle)))
            {
                if (comboBoxStyle == ComboBoxStyle.DropDownList)
                {
                    continue;
                }

                yield return new object[] { comboBoxStyle, new Size(50, 20), "", 0, new Point(0, 0) };
                yield return new object[] { comboBoxStyle, new Size(50, 20), "Some test text", 100, new Point(0, 0) };
                yield return new object[] { comboBoxStyle, new Size(50, 20), "Some test text", -1, new Point(0, 0) };
                yield return new object[] { comboBoxStyle, new Size(50, 20), "Some test text", 12, new Point(71, 0) };
                yield return new object[] { comboBoxStyle, new Size(100, 60), "Some test \n text", 10, new Point(56, 0) };
                yield return new object[] { comboBoxStyle, new Size(100, 60), "Some test \r\n text", 10, new Point(56, 0) };
                yield return new object[] { comboBoxStyle, new Size(100, 60), "Some test \r\n text", 12, new Point(60, 0) };
                yield return new object[] { comboBoxStyle, new Size(100, 60), "Some test \t text", 10, new Point(57, 0) };
            }
        }

        [WinFormsTheory]
        [MemberData(nameof(ComboBoxUiaTextProvider_GetPositionFromCharForUpperRightCorner_ReturnsCorrectValue_TestData))]
        public void ComboBoxUiaTextProvider_GetPositionFromCharForUpperRightCorner_ReturnsCorrectValue(ComboBoxStyle dropDownStyle, Size size, string text, int charIndex, Point expectedPoint)
        {
            using ComboBox comboBox = new ComboBox() { DropDownStyle = dropDownStyle, Size = size };
            comboBox.CreateControl();
            comboBox.Items.Add(text);
            comboBox.SelectedIndex = 0;
            ComboBox.ComboBoxUiaTextProvider provider = new ComboBox.ComboBoxUiaTextProvider(comboBox);

            Point actualPoint = provider.GetPositionFromCharForUpperRightCorner(charIndex, comboBox.Text);

            Assert.True(actualPoint.X >= expectedPoint.X - 1 || actualPoint.X <= expectedPoint.X + 1);
            Assert.True(actualPoint.Y >= expectedPoint.Y - 1 || actualPoint.Y <= expectedPoint.Y + 1);
            Assert.True(comboBox.IsHandleCreated);
            Assert.NotNull(comboBox.TestAccessor().Dynamic._childEdit);
        }

        public static IEnumerable<object[]> ComboBoxUiaTextProvider_GetPositionFromCharForUpperRightCorner_ReturnsCorrectValue_WithoutHandle_TestData()
        {
            foreach (ComboBoxStyle comboBoxStyle in Enum.GetValues(typeof(ComboBoxStyle)))
            {
                if (comboBoxStyle == ComboBoxStyle.DropDownList)
                {
                    continue;
                }

                yield return new object[] { comboBoxStyle, new Size(50, 20), "", 0 };
                yield return new object[] { comboBoxStyle, new Size(50, 20), "Some test text", 100 };
                yield return new object[] { comboBoxStyle, new Size(50, 20), "Some test text", -1 };
                yield return new object[] { comboBoxStyle, new Size(50, 20), "Some test text", 12 };
                yield return new object[] { comboBoxStyle, new Size(100, 60), "Some test \n text", 10 };
                yield return new object[] { comboBoxStyle, new Size(100, 60), "Some test \r\n text", 10 };
                yield return new object[] { comboBoxStyle, new Size(100, 60), "Some test \r\n text", 12 };
                yield return new object[] { comboBoxStyle, new Size(100, 60), "Some test \t text", 10 };
            }
        }

        [WinFormsTheory]
        [MemberData(nameof(ComboBoxUiaTextProvider_GetPositionFromCharForUpperRightCorner_ReturnsCorrectValue_WithoutHandle_TestData))]
        public void ComboBoxUiaTextProvider_GetPositionFromCharForUpperRightCorner_ReturnsMinusOne_WithoutHandle(ComboBoxStyle dropDownStyle, Size size, string text, int charIndex)
        {
            using (new NoAssertContext())
            {
                using ComboBox comboBox = new ComboBox() { DropDownStyle = dropDownStyle, Size = size };
                comboBox.Items.Add(text);
                comboBox.SelectedIndex = 0;
                ComboBox.ComboBoxUiaTextProvider provider = new ComboBox.ComboBoxUiaTextProvider(comboBox);

                Point actualPoint = provider.GetPositionFromCharForUpperRightCorner(charIndex, comboBox.Text);

                Assert.Equal(Point.Empty, actualPoint);
                Assert.False(comboBox.IsHandleCreated);
                Assert.Null(comboBox.TestAccessor().Dynamic._childEdit);
            }
        }

        public static IEnumerable<object[]> ComboBoxUiaTextProvider_GetFormattingRectangle_TestData()
        {
            foreach (ComboBoxStyle comboBoxStyle in Enum.GetValues(typeof(ComboBoxStyle)))
            {
                if (comboBoxStyle == ComboBoxStyle.DropDownList)
                {
                    continue;
                }

                yield return new object[] { comboBoxStyle, new Size(0, 0), new Rectangle(0, 0, 80, 16) };
                int width = comboBoxStyle == ComboBoxStyle.DropDown ? 27 : 44;
                yield return new object[] { comboBoxStyle, new Size(50, 50), new Rectangle(0, 0, width, 15) };
                width = comboBoxStyle == ComboBoxStyle.DropDown ? 227 : 244;
                yield return new object[] { comboBoxStyle, new Size(250, 100), new Rectangle(0, 0, width, 15) };
            }
        }

        [WinFormsTheory]
        [MemberData(nameof(ComboBoxUiaTextProvider_GetFormattingRectangle_TestData))]
        public void ComboBoxUiaTextProvider_GetFormattingRectangle_ReturnsCorrectValue(ComboBoxStyle dropDownStyle, Size size, Rectangle expectedRectangle)
        {
            using ComboBox comboBox = new ComboBox() { DropDownStyle = dropDownStyle, Size = size };
            comboBox.CreateControl();
            ComboBox.ComboBoxUiaTextProvider provider = new ComboBox.ComboBoxUiaTextProvider(comboBox);

            Rectangle providerRectangle = provider.BoundingRectangle;

            Assert.Equal(expectedRectangle, providerRectangle);
            Assert.True(comboBox.IsHandleCreated);
            Assert.NotNull(comboBox.TestAccessor().Dynamic._childEdit);
        }

        [WinFormsTheory]
        [InlineData(ComboBoxStyle.DropDown)]
        [InlineData(ComboBoxStyle.Simple)]
        public void ComboBoxUiaTextProvider_GetFormattingRectangle_ReturnsEmpty_WithoutHandle(ComboBoxStyle dropDownStyle)
        {
            using (new NoAssertContext())
            {
                using ComboBox comboBox = new ComboBox() { DropDownStyle = dropDownStyle, Size = new Size(250, 100) };
                ComboBox.ComboBoxUiaTextProvider provider = new ComboBox.ComboBoxUiaTextProvider(comboBox);

                Rectangle providerRectangle = provider.BoundingRectangle;

                Assert.Equal(Drawing.Rectangle.Empty, providerRectangle);
                Assert.False(comboBox.IsHandleCreated);
                Assert.Null(comboBox.TestAccessor().Dynamic._childEdit);
            }
        }

        public static IEnumerable<object[]> ComboBoxUiaTextProvider_Text_TestData()
        {
            foreach (ComboBoxStyle comboBoxStyle in Enum.GetValues(typeof(ComboBoxStyle)))
            {
                if (comboBoxStyle == ComboBoxStyle.DropDownList)
                {
                    continue;
                }

                yield return new object[] { comboBoxStyle, "" };
                yield return new object[] { comboBoxStyle, "Text" };
                yield return new object[] { comboBoxStyle, "Some test text" };
                yield return new object[] { comboBoxStyle, "Some very very very long test text for testing GetTextLength method" };
            }
        }

        [WinFormsTheory]
        [MemberData(nameof(ComboBoxUiaTextProvider_Text_TestData))]
        public void ComboBoxUiaTextProvider_Text_ReturnsCorrectValue(ComboBoxStyle dropDownStyle, string text)
        {
            using ComboBox comboBox = new ComboBox() { DropDownStyle = dropDownStyle };
            comboBox.CreateControl();
            comboBox.Items.Add(text);
            comboBox.SelectedIndex = 0;
            ComboBox.ComboBoxUiaTextProvider provider = new ComboBox.ComboBoxUiaTextProvider(comboBox);
            string expected = comboBox.Text;

            string actual = provider.Text.Trim('\0');

            Assert.Equal(expected, actual);
            Assert.True(comboBox.IsHandleCreated);
            Assert.NotNull(comboBox.TestAccessor().Dynamic._childEdit);
        }

        [WinFormsTheory]
        [MemberData(nameof(ComboBoxUiaTextProvider_Text_TestData))]
        public void ComboBoxUiaTextProvider_Text_ReturnsEmpty_WithoutHandle(ComboBoxStyle dropDownStyle, string text)
        {
            using (new NoAssertContext())
            {
                using ComboBox comboBox = new ComboBox() { DropDownStyle = dropDownStyle };
                comboBox.Items.Add(text);
                comboBox.SelectedIndex = 0;
                ComboBox.ComboBoxUiaTextProvider provider = new ComboBox.ComboBoxUiaTextProvider(comboBox);

                string actual = provider.Text.Trim('\0');

                Assert.Equal(string.Empty, actual);
                Assert.False(comboBox.IsHandleCreated);
                Assert.Null(comboBox.TestAccessor().Dynamic._childEdit);
            }
        }

        [WinFormsTheory]
        [MemberData(nameof(ComboBoxUiaTextProvider_Text_TestData))]
        public void ComboBoxUiaTextProvider_TextLength_ReturnsCorrectValue(ComboBoxStyle dropDownStyle, string text)
        {
            using ComboBox comboBox = new ComboBox() { DropDownStyle = dropDownStyle };
            comboBox.CreateControl();
            comboBox.Items.Add(text);
            comboBox.SelectedIndex = 0;
            ComboBox.ComboBoxUiaTextProvider provider = new ComboBox.ComboBoxUiaTextProvider(comboBox);

            Assert.Equal(comboBox.Text.Length, provider.TextLength);
            Assert.True(comboBox.IsHandleCreated);
            Assert.NotNull(comboBox.TestAccessor().Dynamic._childEdit);
        }

        [WinFormsTheory]
        [MemberData(nameof(ComboBoxUiaTextProvider_Text_TestData))]
        public void ComboBoxUiaTextProvider_TextLength_ReturnsMinusOne_WithoutHandle(ComboBoxStyle dropDownStyle, string text)
        {
            using (new NoAssertContext())
            {
                using ComboBox comboBox = new ComboBox() { DropDownStyle = dropDownStyle };
                comboBox.Items.Add(text);
                comboBox.SelectedIndex = 0;
                ComboBox.ComboBoxUiaTextProvider provider = new ComboBox.ComboBoxUiaTextProvider(comboBox);

                Assert.Equal(-1, provider.TextLength);
                Assert.False(comboBox.IsHandleCreated);
                Assert.Null(comboBox.TestAccessor().Dynamic._childEdit);
            }
        }

        [WinFormsTheory]
        [InlineData(ComboBoxStyle.DropDown)]
        [InlineData(ComboBoxStyle.Simple)]
        public void ComboBoxUiaTextProvider_WindowExStyle_ReturnsCorrectValue(ComboBoxStyle dropDownStyle)
        {
            using ComboBox comboBox = new ComboBox() { DropDownStyle = dropDownStyle };
            comboBox.CreateControl();
            ComboBox.ComboBoxUiaTextProvider provider = new ComboBox.ComboBoxUiaTextProvider(comboBox);
            WS_EX actual = provider.WindowExStyle;
            Assert.Equal(WS_EX.DEFAULT, actual);
            Assert.True(comboBox.IsHandleCreated);
            Assert.NotNull(comboBox.TestAccessor().Dynamic._childEdit);
        }

        [WinFormsTheory]
        [InlineData(ComboBoxStyle.DropDown)]
        [InlineData(ComboBoxStyle.Simple)]
        public void ComboBoxUiaTextProvider_WindowExStyle_ReturnsLeft_WithoutHandle(ComboBoxStyle dropDownStyle)
        {
            using (new NoAssertContext())
            {
                using ComboBox comboBox = new ComboBox() { DropDownStyle = dropDownStyle };
                ComboBox.ComboBoxUiaTextProvider provider = new ComboBox.ComboBoxUiaTextProvider(comboBox);

                WS_EX actual = provider.WindowExStyle;

                Assert.Equal(WS_EX.LEFT, actual);
                Assert.False(comboBox.IsHandleCreated);
                Assert.Null(comboBox.TestAccessor().Dynamic._childEdit);
            }
        }

        [WinFormsTheory]
        [InlineData(ComboBoxStyle.DropDown)]
        [InlineData(ComboBoxStyle.Simple)]
        public void ComboBoxUiaTextProvider_EditStyle_ReturnsCorrectValue(ComboBoxStyle dropDownStyle)
        {
            using ComboBox comboBox = new ComboBox() { DropDownStyle = dropDownStyle };
            comboBox.CreateControl();
            ComboBox.ComboBoxUiaTextProvider provider = new ComboBox.ComboBoxUiaTextProvider(comboBox);

            ES actual = provider.EditStyle;

            Assert.True(actual.HasFlag(ES.LEFT));
            Assert.True(actual.HasFlag(ES.NOHIDESEL));
            Assert.True(actual.HasFlag(ES.AUTOHSCROLL));
            Assert.True(comboBox.IsHandleCreated);
            Assert.NotNull(comboBox.TestAccessor().Dynamic._childEdit);
        }

        [WinFormsTheory]
        [InlineData(ComboBoxStyle.DropDown)]
        [InlineData(ComboBoxStyle.Simple)]
        public void ComboBoxUiaTextProvider_EditStyle_ReturnsLeft_WithoutHandle(ComboBoxStyle dropDownStyle)
        {
            using (new NoAssertContext())
            {
                using ComboBox comboBox = new ComboBox() { DropDownStyle = dropDownStyle };
                ComboBox.ComboBoxUiaTextProvider provider = new ComboBox.ComboBoxUiaTextProvider(comboBox);

                ES actual = provider.EditStyle;

                Assert.Equal(ES.LEFT, actual);
                Assert.False(comboBox.IsHandleCreated);
                Assert.Null(comboBox.TestAccessor().Dynamic._childEdit);
            }
        }

        public static IEnumerable<object[]> ComboBox_GetVisibleRangePoints_ForSinglelineComboBox_TestData()
        {
            foreach (ComboBoxStyle comboBoxStyle in Enum.GetValues(typeof(ComboBoxStyle)))
            {
                if (comboBoxStyle == ComboBoxStyle.DropDownList)
                {
                    continue;
                }

                yield return new object[] { comboBoxStyle, new Size(0, 0), 0, 0 };
                yield return new object[] { comboBoxStyle, new Size(0, 20), 0, 0 };
                int expectedEnd = comboBoxStyle == ComboBoxStyle.DropDown ? 2 : 4;
                yield return new object[] { comboBoxStyle, new Size(30, 30), 0, expectedEnd };
                expectedEnd = comboBoxStyle == ComboBoxStyle.DropDown ? 4 : 8;
                yield return new object[] { comboBoxStyle, new Size(50, 20), 0, expectedEnd };
                yield return new object[] { comboBoxStyle, new Size(150, 20), 0, 26 };
            }
        }

        [WinFormsTheory]
        [MemberData(nameof(ComboBox_GetVisibleRangePoints_ForSinglelineComboBox_TestData))]
        public void ComboBoxUiaTextProvider_GetVisibleRangePoints_ForSinglelineComboBox_ReturnsCorrectValue(ComboBoxStyle dropDownStyle, Size size, int expectedStart, int expectedEnd)
        {
            using ComboBox comboBox = new ComboBox() { DropDownStyle = dropDownStyle, Size = size };
            comboBox.CreateControl();
            comboBox.Items.Add("Some test text for testing");
            comboBox.SelectedIndex = 0;
            ComboBox.ComboBoxUiaTextProvider provider = new ComboBox.ComboBoxUiaTextProvider(comboBox);

            provider.GetVisibleRangePoints(out int providerVisibleStart, out int providerVisibleEnd);

            Assert.True(providerVisibleStart >= 0);
            Assert.True(providerVisibleStart < comboBox.Text.Length);
            Assert.True(providerVisibleEnd >= 0);
            Assert.True(providerVisibleEnd <= comboBox.Text.Length);

            Assert.Equal(expectedStart, providerVisibleStart);
            Assert.Equal(expectedEnd, providerVisibleEnd);
            Assert.True(comboBox.IsHandleCreated);
            Assert.NotNull(comboBox.TestAccessor().Dynamic._childEdit);
        }

        public static IEnumerable<object[]> ComboBox_GetVisibleRangePoints_ForSinglelineComboBox_WithoutHandle_TestData()
        {
            foreach (ComboBoxStyle comboBoxStyle in Enum.GetValues(typeof(ComboBoxStyle)))
            {
                if (comboBoxStyle == ComboBoxStyle.DropDownList)
                {
                    continue;
                }

                yield return new object[] { comboBoxStyle, new Size(0, 0) };
                yield return new object[] { comboBoxStyle, new Size(0, 20) };
                yield return new object[] { comboBoxStyle, new Size(30, 30) };
                yield return new object[] { comboBoxStyle, new Size(50, 20) };
                yield return new object[] { comboBoxStyle, new Size(150, 20) };
            }
        }

        [WinFormsTheory]
        [MemberData(nameof(ComboBox_GetVisibleRangePoints_ForSinglelineComboBox_WithoutHandle_TestData))]
        public void ComboBoxUiaTextProvider_GetVisibleRangePoints_ReturnsZeros_WithoutHandle(ComboBoxStyle dropDownStyle, Size size)
        {
            using (new NoAssertContext())
            {
                using ComboBox comboBox = new ComboBox() { DropDownStyle = dropDownStyle, Size = size };
                comboBox.Items.Add("Some test text for testing");
                comboBox.SelectedIndex = 0;
                ComboBox.ComboBoxUiaTextProvider provider = new ComboBox.ComboBoxUiaTextProvider(comboBox);

                provider.GetVisibleRangePoints(out int providerVisibleStart, out int providerVisibleEnd);

                Assert.Equal(0, providerVisibleStart);
                Assert.Equal(0, providerVisibleEnd);
                Assert.False(comboBox.IsHandleCreated);
                Assert.Null(comboBox.TestAccessor().Dynamic._childEdit);
            }
        }

        public static IEnumerable<object[]> ComboBoxUiaTextProvider_GetVisibleRanges_TestData()
        {
            yield return new object[] { ComboBoxStyle.DropDown, new Size(0, 0) };
            yield return new object[] { ComboBoxStyle.DropDown, new Size(100, 20) };
            yield return new object[] { ComboBoxStyle.Simple, new Size(0, 0) };
            yield return new object[] { ComboBoxStyle.Simple, new Size(100, 20) };
        }

        [WinFormsTheory]
        [MemberData(nameof(ComboBoxUiaTextProvider_GetVisibleRanges_TestData))]
        public void ComboBoxUiaTextProvider_GetVisibleRanges_ReturnsCorrectValue(ComboBoxStyle dropDownStyle, Size size)
        {
            using ComboBox comboBox = new ComboBox() { DropDownStyle = dropDownStyle, Size = size };
            comboBox.CreateControl();
            comboBox.Items.Add("Some test text for testing");
            comboBox.SelectedIndex = 0;
            ComboBox.ComboBoxUiaTextProvider provider = new ComboBox.ComboBoxUiaTextProvider(comboBox);

            Assert.NotNull(provider.GetVisibleRanges());
            Assert.True(comboBox.IsHandleCreated);
            Assert.NotNull(comboBox.TestAccessor().Dynamic._childEdit);
        }

        [WinFormsTheory]
        [MemberData(nameof(ComboBoxUiaTextProvider_GetVisibleRanges_TestData))]
        public void ComboBoxUiaTextProvider_GetVisibleRanges_ReturnsNull_WithoutHandle(ComboBoxStyle dropDownStyle, Size size)
        {
            using (new NoAssertContext())
            {
                using ComboBox comboBox = new ComboBox() { DropDownStyle = dropDownStyle, Size = size };
                comboBox.Items.Add("Some test text for testing");
                comboBox.SelectedIndex = 0;
                ComboBox.ComboBoxUiaTextProvider provider = new ComboBox.ComboBoxUiaTextProvider(comboBox);

                Assert.Null(provider.GetVisibleRanges());
                Assert.False(comboBox.IsHandleCreated);
                Assert.Null(comboBox.TestAccessor().Dynamic._childEdit);
            }
        }

        [WinFormsTheory]
        [InlineData(ComboBoxStyle.DropDown)]
        [InlineData(ComboBoxStyle.Simple)]
        public void ComboBoxUiaTextProvider_RangeFromChild_DoesntThrowAnException(ComboBoxStyle dropDownStyle)
        {
            using (new NoAssertContext())
            {
                using ComboBox comboBox = new ComboBox() { DropDownStyle = dropDownStyle };
                ComboBox.ComboBoxUiaTextProvider provider = new ComboBox.ComboBoxUiaTextProvider(comboBox);

                // RangeFromChild doesn't throw an exception
                UiaCore.ITextRangeProvider range = provider.RangeFromChild(comboBox.AccessibilityObject);
                // RangeFromChild implementation can be changed so this test can be changed too
                Assert.Null(range);
            }
        }

        public static IEnumerable<object[]> ComboBoxUiaTextProvider_RangeFromPoint_TestData()
        {
            yield return new object[] { ComboBoxStyle.DropDown, Point.Empty };
            yield return new object[] { ComboBoxStyle.DropDown, new Point(10, 10) };
            yield return new object[] { ComboBoxStyle.Simple, Point.Empty };
            yield return new object[] { ComboBoxStyle.Simple, new Point(10, 10) };
        }

        [WinFormsTheory]
        [MemberData(nameof(ComboBoxUiaTextProvider_RangeFromPoint_TestData))]
        public void ComboBoxUiaTextProvider_RangeFromPoint_DoesntThrowAnException(ComboBoxStyle dropDownStyle, Point point)
        {
            using ComboBox comboBox = new ComboBox() { DropDownStyle = dropDownStyle } ;
            comboBox.CreateControl();
            ComboBox.ComboBoxUiaTextProvider provider = new ComboBox.ComboBoxUiaTextProvider(comboBox);

            UiaTextRange textRangeProvider = provider.RangeFromPoint(point) as UiaTextRange;

            Assert.NotNull(textRangeProvider);
            Assert.True(comboBox.IsHandleCreated);
            Assert.NotNull(comboBox.TestAccessor().Dynamic._childEdit);
        }

        [WinFormsTheory]
        [MemberData(nameof(ComboBoxUiaTextProvider_RangeFromPoint_TestData))]
        public void ComboBoxUiaTextProvider_RangeFromPoint_ReturnsNull_WithoutHandle(ComboBoxStyle dropDownStyle, Point point)
        {
            using (new NoAssertContext())
            {
                using ComboBox comboBox = new ComboBox() { DropDownStyle = dropDownStyle };
                ComboBox.ComboBoxUiaTextProvider provider = new ComboBox.ComboBoxUiaTextProvider(comboBox);

                UiaTextRange textRangeProvider = provider.RangeFromPoint(point) as UiaTextRange;

                Assert.Null(textRangeProvider);
                Assert.False(comboBox.IsHandleCreated);
                Assert.Null(comboBox.TestAccessor().Dynamic._childEdit);
            }
        }

        [WinFormsTheory]
        [InlineData(ComboBoxStyle.DropDown, 2, 5)]
        [InlineData(ComboBoxStyle.DropDown, 0, 10)]
        [InlineData(ComboBoxStyle.Simple, 2, 5)]
        [InlineData(ComboBoxStyle.Simple, 0, 10)]
        public void ComboBoxUiaTextProvider_SetSelection_GetSelection_ReturnCorrectValue(ComboBoxStyle dropDownStyle, int start, int end)
        {
            using ComboBox comboBox = new ComboBox() { DropDownStyle = dropDownStyle };
            comboBox.CreateControl();
            comboBox.Items.Add("Some test text for testing");
            comboBox.SelectedIndex = 0;
            ComboBox.ComboBoxUiaTextProvider provider = new ComboBox.ComboBoxUiaTextProvider(comboBox);
            provider.SetSelection(start, end);
            UiaCore.ITextRangeProvider[] selection = provider.GetSelection();
            UiaTextRange textRange = selection[0] as UiaTextRange;

            Assert.NotNull(selection);
            Assert.NotNull(textRange);
            Assert.Equal(start, textRange.Start);
            Assert.Equal(end, textRange.End);
            Assert.Equal(start, comboBox.SelectionStart);
            Assert.Equal(end, comboBox.SelectionStart + comboBox.SelectionLength);
            Assert.True(comboBox.IsHandleCreated);
            Assert.NotNull(comboBox.TestAccessor().Dynamic._childEdit);
        }

        [WinFormsTheory]
        [InlineData(ComboBoxStyle.DropDown, 2, 5)]
        [InlineData(ComboBoxStyle.DropDown, 0, 10)]
        [InlineData(ComboBoxStyle.Simple, 2, 5)]
        [InlineData(ComboBoxStyle.Simple, 0, 10)]
        public void ComboBoxUiaTextProvider_SetSelection_GetSelection_DontWork_WithoutHandle(ComboBoxStyle dropDownStyle, int start, int end)
        {
            using (new NoAssertContext())
            {
                using ComboBox comboBox = new ComboBox() { DropDownStyle = dropDownStyle };
                comboBox.Items.Add("Some test text for testing");
                comboBox.SelectedIndex = 0;
                ComboBox.ComboBoxUiaTextProvider provider = new ComboBox.ComboBoxUiaTextProvider(comboBox);
                provider.SetSelection(start, end);
                Assert.False(comboBox.IsHandleCreated);
                Assert.Null(comboBox.TestAccessor().Dynamic._childEdit);

                UiaCore.ITextRangeProvider[] selection = provider.GetSelection();

                Assert.Null(selection);
                Assert.False(comboBox.IsHandleCreated);
                Assert.Null(comboBox.TestAccessor().Dynamic._childEdit);
                Assert.Equal(0, comboBox.SelectionStart);
                Assert.Equal(0, comboBox.SelectionStart + comboBox.SelectionLength);
                Assert.True(comboBox.IsHandleCreated);
                Assert.NotNull(comboBox.TestAccessor().Dynamic._childEdit);
            }
        }

        [WinFormsTheory]
        [InlineData(ComboBoxStyle.DropDown, -5, 10)]
        [InlineData(ComboBoxStyle.DropDown, 5, 100)]
        [InlineData(ComboBoxStyle.Simple, -5, 10)]
        [InlineData(ComboBoxStyle.Simple, 5, 100)]
        public void ComboBoxUiaTextProvider_SetSelection_DoesntSelectText_IfIncorrectArguments(ComboBoxStyle dropDownStyle, int start, int end)
        {
            using (new NoAssertContext())
            {
                using ComboBox comboBox = new ComboBox() { DropDownStyle = dropDownStyle };
                comboBox.CreateControl();
                comboBox.Items.Add("Some test text for testing");
                comboBox.SelectedIndex = 0;
                ComboBox.ComboBoxUiaTextProvider provider = new ComboBox.ComboBoxUiaTextProvider(comboBox);
                provider.SetSelection(start, end);
                UiaCore.ITextRangeProvider[] selection = provider.GetSelection();
                Assert.NotNull(selection);

                UiaTextRange textRange = selection[0] as UiaTextRange;

                Assert.NotNull(textRange);
                Assert.Equal(0, textRange.Start);
                Assert.Equal(0, textRange.End);
                Assert.Equal(0, comboBox.SelectionStart);
                Assert.Equal(0, comboBox.SelectionStart + comboBox.SelectionLength);
                Assert.True(comboBox.IsHandleCreated);
                Assert.NotNull(comboBox.TestAccessor().Dynamic._childEdit);
            }
        }

        [WinFormsTheory]
        [InlineData(ComboBoxStyle.DropDown, 0)]
        [InlineData(ComboBoxStyle.DropDown, 2)]
        [InlineData(ComboBoxStyle.Simple, 0)]
        [InlineData(ComboBoxStyle.Simple, 2)]
        public void ComboBoxUiaTextProvider_LineScroll_ReturnsFalse(ComboBoxStyle dropDownStyle, int newLine)
        {
            using ComboBox comboBox = new ComboBox() { DropDownStyle = dropDownStyle, Size = new Size(50, 100) };
            comboBox.CreateControl();
            comboBox.Items.Add("Some long long test text for testing GetFirstVisibleLine method");
            comboBox.SelectedIndex = 0;
            ComboBox.ComboBoxUiaTextProvider provider = new ComboBox.ComboBoxUiaTextProvider(comboBox);

            Assert.False(provider.LineScroll(0, newLine));
            Assert.Equal(0, provider.FirstVisibleLine);
            Assert.True(comboBox.IsHandleCreated);
            Assert.NotNull(comboBox.TestAccessor().Dynamic._childEdit);
        }

        [WinFormsTheory]
        [InlineData(ComboBoxStyle.DropDown, 0)]
        [InlineData(ComboBoxStyle.DropDown, 2)]
        [InlineData(ComboBoxStyle.Simple, 0)]
        [InlineData(ComboBoxStyle.Simple, 2)]
        public void ComboBoxUiaTextProvider_LineScroll_DoesntWork_WitoutHandle(ComboBoxStyle dropDownStyle, int newLine)
        {
            using (new NoAssertContext())
            {
                using ComboBox comboBox = new ComboBox() { DropDownStyle = dropDownStyle, Size = new Size(50, 100) };
                comboBox.Items.Add("Some long long test text for testing GetFirstVisibleLine method");
                comboBox.SelectedIndex = 0;
                ComboBox.ComboBoxUiaTextProvider provider = new ComboBox.ComboBoxUiaTextProvider(comboBox);

                Assert.False(provider.LineScroll(0, newLine));
                Assert.Equal(-1, provider.FirstVisibleLine);
                Assert.False(comboBox.IsHandleCreated);
                Assert.Null(comboBox.TestAccessor().Dynamic._childEdit);
            }
        }

        [WinFormsTheory]
        [InlineData(ComboBoxStyle.DropDown)]
        [InlineData(ComboBoxStyle.Simple)]
        public void ComboBoxUiaTextProvider_GetLogfont_ReturnSegoe_ByDefault(ComboBoxStyle dropDownStyle)
        {
            using ComboBox comboBox = new ComboBox() { DropDownStyle = dropDownStyle, Size = new Size(50, 100) };
            comboBox.CreateControl();
            ComboBox.ComboBoxUiaTextProvider provider = new ComboBox.ComboBoxUiaTextProvider(comboBox);
            LOGFONTW logFont = provider.Logfont;

            string actual = logFont.FaceName.ToString();

            Assert.Equal("Segoe UI", actual);
            Assert.True(comboBox.IsHandleCreated);
            Assert.NotNull(comboBox.TestAccessor().Dynamic._childEdit);
        }

        [WinFormsTheory]
        [InlineData(ComboBoxStyle.DropDown)]
        [InlineData(ComboBoxStyle.Simple)]
        public void ComboBoxUiaTextProvider_GetLogfont_ReturnEmpty_WithoutHandle(ComboBoxStyle dropDownStyle)
        {
            using (new NoAssertContext())
            {
                using ComboBox comboBox = new ComboBox() { DropDownStyle = dropDownStyle, Size = new Size(50, 100) };
                ComboBox.ComboBoxUiaTextProvider provider = new ComboBox.ComboBoxUiaTextProvider(comboBox);
                Assert.False(comboBox.IsHandleCreated);
                Assert.Null(comboBox.TestAccessor().Dynamic._childEdit);
                Assert.Equal(new LOGFONTW(), provider.Logfont);
            }
        }

        [WinFormsTheory]
        [InlineData(ComboBoxStyle.DropDown)]
        [InlineData(ComboBoxStyle.Simple)]
        public void ComboBoxUiaTextProvider_FirstVisibleLine_DefaultValueCorrect(ComboBoxStyle dropDownStyle)
        {
            using ComboBox comboBox = new ComboBox() { DropDownStyle = dropDownStyle, Size = new Size(50, 100) };
            comboBox.CreateControl();
            ComboBox.ComboBoxUiaTextProvider provider = new ComboBox.ComboBoxUiaTextProvider(comboBox);

            var actualValie = (int)(long)SendMessageW(comboBox.TestAccessor().Dynamic._childEdit, (WM)EM.GETFIRSTVISIBLELINE);

            Assert.Equal(actualValie, provider.FirstVisibleLine);
        }

        [WinFormsTheory]
        [InlineData(ComboBoxStyle.DropDown)]
        [InlineData(ComboBoxStyle.Simple)]
        public void ComboBoxUiaTextProvider_LinesCount_DefaultValueCorrect(ComboBoxStyle dropDownStyle)
        {
            using ComboBox comboBox = new ComboBox() { DropDownStyle = dropDownStyle, Size = new Size(50, 100) };
            comboBox.CreateControl();
            ComboBox.ComboBoxUiaTextProvider provider = new ComboBox.ComboBoxUiaTextProvider(comboBox);

            var actualValie = (int)(long)SendMessageW(comboBox.TestAccessor().Dynamic._childEdit, (WM)EM.GETLINECOUNT);

            Assert.Equal(actualValie, provider.LinesCount);
        }

        [WinFormsTheory]
        [InlineData(ComboBoxStyle.DropDown, 50, 20, 0)]
        [InlineData(ComboBoxStyle.DropDown, 50, 20, 50)]
        [InlineData(ComboBoxStyle.DropDown, 50, 20, 200)]
        [InlineData(ComboBoxStyle.DropDown, 50, 20, -5)]
        [InlineData(ComboBoxStyle.Simple, 50, 20, 0)]
        [InlineData(ComboBoxStyle.Simple, 50, 20, 50)]
        [InlineData(ComboBoxStyle.Simple, 50, 20, 200)]
        [InlineData(ComboBoxStyle.Simple, 50, 20, -5)]
        public void ComboBoxUiaTextProvider_GetLineFromCharIndex_DefaultValueCorrect(
            ComboBoxStyle dropDownStyle,
            int width,
            int height,
            int charIndex)
        {
            using ComboBox comboBox = new ComboBox() { DropDownStyle = dropDownStyle, Size = new Size(width, height) };
            comboBox.CreateControl();
            comboBox.Items.Add("Some test text for testing GetLineFromCharIndex method");
            comboBox.SelectedIndex = 0;
            ComboBox.ComboBoxUiaTextProvider provider = new ComboBox.ComboBoxUiaTextProvider(comboBox);

            int expectedLine = (int)(long)SendMessageW(comboBox.TestAccessor().Dynamic._childEdit, (WM)EM.LINEFROMCHAR, (IntPtr)charIndex);
            int actualLine = provider.GetLineFromCharIndex(charIndex);

            Assert.Equal(expectedLine, actualLine);
            Assert.True(comboBox.IsHandleCreated);
            Assert.NotNull(comboBox.TestAccessor().Dynamic._childEdit);
        }

        [WinFormsTheory]
        [InlineData(ComboBoxStyle.DropDown, 0)]
        [InlineData(ComboBoxStyle.DropDown, 2)]
        [InlineData(ComboBoxStyle.Simple, 0)]
        [InlineData(ComboBoxStyle.Simple, 2)]
        public void ComboBoxUiaTextProvider_LineScroll_DefaultValueCorrect(ComboBoxStyle dropDownStyle, int newLine)
        {
            using ComboBox comboBox = new ComboBox() { DropDownStyle = dropDownStyle, Size = new Size(50, 100) };
            comboBox.CreateControl();
            comboBox.Items.Add("Some long long test text for testing GetFirstVisibleLine method");
            comboBox.SelectedIndex = 0;
            ComboBox.ComboBoxUiaTextProvider provider = new ComboBox.ComboBoxUiaTextProvider(comboBox);

            var expectedValue = SendMessageW(comboBox.TestAccessor().Dynamic._childEdit, (WM)EM.LINESCROLL, (IntPtr)0, (IntPtr)newLine) != IntPtr.Zero;

            Assert.Equal(expectedValue, provider.LineScroll(0, newLine));
            Assert.True(comboBox.IsHandleCreated);
            Assert.NotNull(comboBox.TestAccessor().Dynamic._childEdit);
        }
    }
}
