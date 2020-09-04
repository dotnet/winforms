// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms.Automation;
using Xunit;
using static Interop;
using static Interop.UiaCore;
using static Interop.Gdi32;
using System.Runtime.InteropServices;
using Moq;
using static Interop.User32;

namespace System.Windows.Forms.Primitives.Tests.Automation
{
    public class UiaTextRangeTests
    {
        // Used to get access to the test accessor for static members
        private const UiaTextRange StaticNullTextRange = null!;

        [StaTheory]
        [InlineData(0, 0)]
        [InlineData(0, 5)]
        [InlineData(5, 10)]
        [InlineData(1, 1)]
        public void UiaTextRange_Constructor_InitializesProvider_And_CorrectEndpoints(int start, int end)
        {
            IRawElementProviderSimple enclosingElement = new Mock<IRawElementProviderSimple>(MockBehavior.Strict).Object;
            UiaTextProvider provider = new Mock<UiaTextProvider>(MockBehavior.Strict).Object;
            UiaTextRange textRange = new UiaTextRange(enclosingElement, provider, start, end);
            Assert.Equal(start, textRange.Start);
            Assert.Equal(end, textRange.End);
            Assert.Equal(enclosingElement, ((ITextRangeProvider)textRange).GetEnclosingElement());

            object actual = textRange.TestAccessor()._provider;

            Assert.Equal(provider, actual);
        }

        [StaTheory]
        [InlineData(-10, 0, 0, 0)]
        [InlineData(0, -10, 0, 0)]
        [InlineData(5, 0, 5, 5)]
        [InlineData(-1, -1, 0, 0)]
        [InlineData(10, 5, 10, 10)]
        [InlineData(-5, 5, 0, 5)]
        [InlineData(5, -5, 5, 5)]
        public void UiaTextRange_Constructor_InitializesProvider_And_CorrectEndpoints_IfEndpointsincorrect(int start, int end, int expectedStart, int expectedEnd)
        {
            IRawElementProviderSimple enclosingElement = new Mock<IRawElementProviderSimple>(MockBehavior.Strict).Object;
            UiaTextProvider provider = new Mock<UiaTextProvider>(MockBehavior.Strict).Object;
            UiaTextRange textRange = new UiaTextRange(enclosingElement, provider, start, end);
            Assert.Equal(expectedStart, textRange.Start);
            Assert.Equal(expectedEnd, textRange.End);
        }

#pragma warning disable CS8625 // UiaTextRange constructor doesn't accept a provider null parameter
        [StaFact]
        public void UiaTextRange_Constructor_Provider_Null_ThrowsException()
        {
            IRawElementProviderSimple enclosingElement = new Mock<IRawElementProviderSimple>(MockBehavior.Strict).Object;
            Assert.Throws<ArgumentNullException>(() => new UiaTextRange(enclosingElement, null, 0, 5));
        }
#pragma warning restore CS8625

#pragma warning disable CS8625 // UiaTextRange constructor doesn't accept an enclosingElement null parameter
        [StaFact]
        public void UiaTextRange_Constructor_Control_Null_ThrowsException()
        {
            UiaTextProvider provider = new Mock<UiaTextProvider>(MockBehavior.Strict).Object;
            Assert.Throws<ArgumentNullException>(() => new UiaTextRange(null, provider, 0, 5));
        }
#pragma warning restore CS8625

        [StaTheory]
        [InlineData(3, -5)]
        [InlineData(-5, 3)]
        [InlineData(-3, -5)]
        public void UiaTextRange_Constructor_SetCorrectValues_IfNegativeStartEnd(int start, int end)
        {
            IRawElementProviderSimple enclosingElement = new Mock<IRawElementProviderSimple>(MockBehavior.Strict).Object;
            UiaTextProvider provider = new Mock<UiaTextProvider>(MockBehavior.Strict).Object;
            UiaTextRange textRange = new UiaTextRange(enclosingElement, provider, start, end);
            Assert.True(textRange.Start >= 0);
            Assert.True(textRange.End >= 0);
        }

        [StaTheory]
        [InlineData(0)]
        [InlineData(5)]
        [InlineData(int.MaxValue)]
        public void UiaTextRange_End_Get_ReturnsCorrectValue(int end)
        {
            IRawElementProviderSimple enclosingElement = new Mock<IRawElementProviderSimple>(MockBehavior.Strict).Object;
            UiaTextProvider provider = new Mock<UiaTextProvider>(MockBehavior.Strict).Object;
            UiaTextRange textRange = new UiaTextRange(enclosingElement, provider, start: 0, end);
            Assert.Equal(end, textRange.End);
        }

        [StaTheory]
        [InlineData(0)]
        [InlineData(5)]
        [InlineData(int.MaxValue)]
        public void UiaTextRange_End_SetCorrectly(int end)
        {
            IRawElementProviderSimple enclosingElement = new Mock<IRawElementProviderSimple>(MockBehavior.Strict).Object;
            UiaTextProvider provider = new Mock<UiaTextProvider>(MockBehavior.Strict).Object;
            UiaTextRange textRange = new UiaTextRange(enclosingElement, provider, start: 0, end: 0);
            textRange.End = end;
            int actual = textRange.End < textRange.Start ? textRange.Start : textRange.End;
            Assert.Equal(end, actual);
        }

        [StaFact]
        public void UiaTextRange_End_SetCorrect_IfValueIncorrect()
        {
            IRawElementProviderSimple enclosingElement = new Mock<IRawElementProviderSimple>(MockBehavior.Strict).Object;
            UiaTextProvider provider = new Mock<UiaTextProvider>(MockBehavior.Strict).Object;
            UiaTextRange textRange = new UiaTextRange(enclosingElement, provider, start: 5, end: 10);
            textRange.End = 3;  /*Incorrect value*/
            Assert.Equal(textRange.Start, textRange.End);

            textRange.End = 6;
            Assert.Equal(6, textRange.End);

            textRange.End = -10; /*Incorrect value*/
            Assert.Equal(textRange.Start, textRange.End);
        }

        [StaTheory]
        [InlineData(0, 0, 0)]
        [InlineData(5, 5, 0)]
        [InlineData(3, 15, 12)]
        [InlineData(0, 10, 10)]
        [InlineData(6, 10, 4)]
        public void UiaTextRange_Length_ReturnsCorrectValue(int start, int end, int expected)
        {
            IRawElementProviderSimple enclosingElement = new Mock<IRawElementProviderSimple>(MockBehavior.Strict).Object;
            UiaTextProvider provider = new Mock<UiaTextProvider>(MockBehavior.Strict).Object;
            UiaTextRange textRange = new UiaTextRange(enclosingElement, provider, start, end);
            Assert.Equal(expected, textRange.Length);
        }

        [StaTheory]
        [InlineData(-5, 0)]
        [InlineData(0, -5)]
        [InlineData(-5, -5)]
        [InlineData(10, 5)]
        public void UiaTextRange_Length_ReturnsCorrectValue_IfIncorrectStartEnd(int start, int end)
        {
            IRawElementProviderSimple enclosingElement = new Mock<IRawElementProviderSimple>(MockBehavior.Strict).Object;
            UiaTextProvider provider = new Mock<UiaTextProvider>(MockBehavior.Strict).Object;
            UiaTextRange textRange = new UiaTextRange(enclosingElement, provider, 3, 10);

            var testAccessor = textRange.TestAccessor();
            testAccessor._start = start;
            testAccessor._end = end;

            Assert.Equal(0, textRange.Length);
        }

        [StaTheory]
        [InlineData(0)]
        [InlineData(5)]
        [InlineData(int.MaxValue)]
        public void UiaTextRange_Start_Get_ReturnsCorrectValue(int start)
        {
            IRawElementProviderSimple enclosingElement = new Mock<IRawElementProviderSimple>(MockBehavior.Strict).Object;
            UiaTextProvider provider = new Mock<UiaTextProvider>(MockBehavior.Strict).Object;
            UiaTextRange textRange = new UiaTextRange(enclosingElement, provider, start: 0, end: 0);

            textRange.TestAccessor()._start = start;

            Assert.Equal(start, textRange.Start);
        }

        [StaTheory]
        [InlineData(0)]
        [InlineData(5)]
        [InlineData(int.MaxValue)]
        public void UiaTextRange_Start_SetCorrectly(int start)
        {
            IRawElementProviderSimple enclosingElement = new Mock<IRawElementProviderSimple>(MockBehavior.Strict).Object;
            UiaTextProvider provider = new Mock<UiaTextProvider>(MockBehavior.Strict).Object;
            UiaTextRange textRange = new UiaTextRange(enclosingElement, provider, start: 0, end: 0);
            textRange.Start = start;
            int actual = textRange.Start < textRange.End ? textRange.End : textRange.Start;
            Assert.Equal(start, actual);
        }

        [StaFact]
        public void UiaTextRange_Start_Set_Correct_IfValueIncorrect()
        {
            IRawElementProviderSimple enclosingElement = new Mock<IRawElementProviderSimple>(MockBehavior.Strict).Object;
            UiaTextProvider provider = new Mock<UiaTextProvider>(MockBehavior.Strict).Object;
            UiaTextRange textRange = new UiaTextRange(enclosingElement, provider, start: 4, end: 8);
            textRange.Start = -10;
            Assert.Equal(0, textRange.Start);
            Assert.Equal(8, textRange.End);
        }

        [StaFact]
        public void UiaTextRange_Start_Set_Correct_IfValueMoreThanEnd()
        {
            IRawElementProviderSimple enclosingElement = new Mock<IRawElementProviderSimple>(MockBehavior.Strict).Object;
            UiaTextProvider provider = new Mock<UiaTextProvider>(MockBehavior.Strict).Object;
            UiaTextRange textRange = new UiaTextRange(enclosingElement, provider, start: 4, end: 10);
            textRange.Start = 15; // More than End = 10
            Assert.True(textRange.Start <= textRange.End);
        }

        [StaFact]
        public void UiaTextRange_ITextRangeProvider_Clone_ReturnsCorrectValue()
        {
            IRawElementProviderSimple enclosingElement = new Mock<IRawElementProviderSimple>(MockBehavior.Strict).Object;
            UiaTextProvider provider = new Mock<UiaTextProvider>(MockBehavior.Strict).Object;
            UiaTextRange textRange = new UiaTextRange(enclosingElement, provider, start: 3, end: 9);
            UiaTextRange actual = (UiaTextRange)((ITextRangeProvider)textRange).Clone();
            Assert.Equal(textRange.Start, actual.Start);
            Assert.Equal(textRange.End, actual.End);
        }

        [StaTheory]
        [InlineData(3, 9, true)]
        [InlineData(0, 2, false)]
        public void UiaTextRange_ITextRangeProvider_Compare_ReturnsCorrectValue(int start, int end, bool expected)
        {
            IRawElementProviderSimple enclosingElement = new Mock<IRawElementProviderSimple>(MockBehavior.Strict).Object;
            UiaTextProvider provider = new Mock<UiaTextProvider>(MockBehavior.Strict).Object;
            UiaTextRange textRange1 = new UiaTextRange(enclosingElement, provider, start: 3, end: 9);
            UiaTextRange textRange2 = new UiaTextRange(enclosingElement, provider, start, end);
            bool actual = ((ITextRangeProvider)textRange1).Compare(textRange2).IsTrue();
            Assert.Equal(expected, actual);
        }

        public static IEnumerable<object[]> UiaTextRange_ITextRangeProvider_CompareEndpoints_ReturnsCorrectValue_TestData()
        {
            yield return new object[] { TextPatternRangeEndpoint.Start, 3, 9, TextPatternRangeEndpoint.Start, 0 };
            yield return new object[] { TextPatternRangeEndpoint.End, 3, 9, TextPatternRangeEndpoint.Start, 6 };
            yield return new object[] { TextPatternRangeEndpoint.Start, 3, 9, TextPatternRangeEndpoint.End, -6 };
            yield return new object[] { TextPatternRangeEndpoint.End, 3, 9, TextPatternRangeEndpoint.End, 0 };
            yield return new object[] { TextPatternRangeEndpoint.Start, 0, 0, TextPatternRangeEndpoint.Start, 3 };
            yield return new object[] { TextPatternRangeEndpoint.End, 0, 0, TextPatternRangeEndpoint.Start, 9 };
            yield return new object[] { TextPatternRangeEndpoint.End, 1, 15, TextPatternRangeEndpoint.End, -6 };
            yield return new object[] { TextPatternRangeEndpoint.Start, 1, 15, TextPatternRangeEndpoint.End, -12 };
        }

        [StaTheory]
        [MemberData(nameof(UiaTextRange_ITextRangeProvider_CompareEndpoints_ReturnsCorrectValue_TestData))]
        public void UiaTextRange_ITextRangeProvider_CompareEndpoints_ReturnsCorrectValue(
            int endpoint,
            int targetStart,
            int targetEnd,
            int targetEndpoint,
            int expected)
        {
            IRawElementProviderSimple enclosingElement = new Mock<IRawElementProviderSimple>(MockBehavior.Strict).Object;
            UiaTextProvider provider = new Mock<UiaTextProvider>(MockBehavior.Strict).Object;
            UiaTextRange textRange = new UiaTextRange(enclosingElement, provider, start: 3, end: 9);
            UiaTextRange targetRange = new UiaTextRange(enclosingElement, provider, start: targetStart, end: targetEnd);
            int actual = ((ITextRangeProvider)textRange).CompareEndpoints((TextPatternRangeEndpoint)endpoint, targetRange, (TextPatternRangeEndpoint)targetEndpoint);
            Assert.Equal(expected, actual);
        }

        [StaTheory]
        [InlineData(2, 2, 2, 3)]
        [InlineData(8, 9, 8, 9)]
        [InlineData(0, 3, 0, 3)]
        public void UiaTextRange_ITextRangeProvider_ExpandToEnclosingUnit_ExpandsToCharacter(int start, int end, int expandedStart, int expandedEnd)
        {
            IRawElementProviderSimple enclosingElement = new Mock<IRawElementProviderSimple>(MockBehavior.Strict).Object;
            Mock<UiaTextProvider> providerMock = new Mock<UiaTextProvider>(MockBehavior.Strict);
            providerMock.Setup(m => m.TextLength).Returns("words, words, words".Length);
            UiaTextProvider provider = providerMock.Object;
            UiaTextRange textRange = new UiaTextRange(enclosingElement, provider, start, end);
            ((ITextRangeProvider)textRange).ExpandToEnclosingUnit(TextUnit.Character);
            Assert.Equal(expandedStart, textRange.Start);
            Assert.Equal(expandedEnd, textRange.End);
        }

        [StaTheory]
        [InlineData(2, 3, 0, 5)]
        [InlineData(8, 8, 7, 12)]
        [InlineData(16, 17, 14, 19)]
        public void UiaTextRange_ITextRangeProvider_ExpandToEnclosingUnit_ExpandsToWord(int start, int end, int expandedStart, int expandedEnd)
        {
            string testText = "words, words, words";
            IRawElementProviderSimple enclosingElement = new Mock<IRawElementProviderSimple>(MockBehavior.Strict).Object;
            Mock<UiaTextProvider> providerMock = new Mock<UiaTextProvider>(MockBehavior.Strict);
            providerMock.Setup(m => m.Text).Returns(testText);
            providerMock.Setup(m => m.TextLength).Returns(testText.Length);
            UiaTextProvider provider = providerMock.Object;
            UiaTextRange textRange = new UiaTextRange(enclosingElement, provider, start, end);
            ((ITextRangeProvider)textRange).ExpandToEnclosingUnit(TextUnit.Word);
            Assert.Equal(expandedStart, textRange.Start);
            Assert.Equal(expandedEnd, textRange.End);
        }

        [StaTheory]
        [InlineData(2, 4, 0, 12)]
        [InlineData(15, 16, 12, 25)]
        [InlineData(27, 28, 25, 36)]
        public void UiaTextRange_ITextRangeProvider_ExpandToEnclosingUnit_ExpandsToLine(int start, int end, int expandedStart, int expandedEnd)
        {
            string testText =
@"First line
second line
third line.";
            IRawElementProviderSimple enclosingElement = new Mock<IRawElementProviderSimple>(MockBehavior.Strict).Object;
            Mock<UiaTextProvider> providerMock = new Mock<UiaTextProvider>(MockBehavior.Strict);
            providerMock.Setup(m => m.Text).Returns(testText);
            providerMock.Setup(m => m.TextLength).Returns(testText.Length);
            providerMock.Setup(m => m.LinesCount).Returns(3);
            providerMock.Setup(m => m.GetLineIndex(0)).Returns(0);
            providerMock.Setup(m => m.GetLineIndex(1)).Returns(12);
            providerMock.Setup(m => m.GetLineIndex(2)).Returns(25);
            providerMock.Setup(m => m.GetLineFromCharIndex(2)).Returns(0);
            providerMock.Setup(m => m.GetLineFromCharIndex(4)).Returns(0);
            providerMock.Setup(m => m.GetLineFromCharIndex(15)).Returns(1);
            providerMock.Setup(m => m.GetLineFromCharIndex(16)).Returns(1);
            providerMock.Setup(m => m.GetLineFromCharIndex(27)).Returns(2);
            providerMock.Setup(m => m.GetLineFromCharIndex(28)).Returns(2);
            UiaTextProvider provider = providerMock.Object;
            UiaTextRange textRange = new UiaTextRange(enclosingElement, provider, start, end);
            ((ITextRangeProvider)textRange).ExpandToEnclosingUnit(TextUnit.Line);
            Assert.Equal(expandedStart, textRange.Start);
            Assert.Equal(expandedEnd, textRange.End);
        }

        [StaTheory]
        [InlineData(2, 4, 0, 24)]
        [InlineData(30, 30, 24, 49)]
        [InlineData(49, 60, 49, 72)]
        public void UiaTextRange_ITextRangeProvider_ExpandToEnclosingUnit_ExpandsToParagraph(int start, int end, int expandedStart, int expandedEnd)
        {
            string testText =
@"This is the first line
this is the second line
this is the third line.";
            IRawElementProviderSimple enclosingElement = new Mock<IRawElementProviderSimple>(MockBehavior.Strict).Object;
            Mock<UiaTextProvider> providerMock = new Mock<UiaTextProvider>(MockBehavior.Strict);
            providerMock.Setup(m => m.Text).Returns(testText);
            providerMock.Setup(m => m.TextLength).Returns(testText.Length);
            UiaTextProvider provider = providerMock.Object;
            UiaTextRange textRange = new UiaTextRange(enclosingElement, provider, start, end);
            ((ITextRangeProvider)textRange).ExpandToEnclosingUnit(TextUnit.Paragraph);
            Assert.Equal(expandedStart, textRange.Start);
            Assert.Equal(expandedEnd, textRange.End);
        }

        public static IEnumerable<object[]> UiaTextRange_ITextRangeProvider_ExpandToEnclosingUnit_ExpandsToAllText_TestData()
        {
            yield return new object[] { 5, 8, TextUnit.Page, 0, 72 };
            yield return new object[] { 10, 10, TextUnit.Format, 0, 72 };
            yield return new object[] { 10, 10, TextUnit.Document, 0, 72 };
        }

        [StaTheory]
        [MemberData(nameof(UiaTextRange_ITextRangeProvider_ExpandToEnclosingUnit_ExpandsToAllText_TestData))]
        internal void UiaTextRange_ITextRangeProvider_ExpandToEnclosingUnit_ExpandsToAllText(int start, int end, TextUnit textUnit, int expandedStart, int expandedEnd)
        {
            string testText =
@"This is the first line
this is the second line
this is the third line.";
            IRawElementProviderSimple enclosingElement = new Mock<IRawElementProviderSimple>(MockBehavior.Strict).Object;
            Mock<UiaTextProvider> providerMock = new Mock<UiaTextProvider>(MockBehavior.Strict);
            providerMock.Setup(m => m.TextLength).Returns(testText.Length);
            UiaTextProvider provider = providerMock.Object;
            UiaTextRange textRange = new UiaTextRange(enclosingElement, provider, start, end);
            ((ITextRangeProvider)textRange).ExpandToEnclosingUnit(textUnit);
            Assert.Equal(expandedStart, textRange.Start);
            Assert.Equal(expandedEnd, textRange.End);
        }

        [StaTheory]
        [InlineData(true)]
        [InlineData(false)]
        internal void UiaTextRange_ITextRangeProvider_FindAttribute_Returns_null(bool backward)
        {
            IRawElementProviderSimple enclosingElement = new Mock<IRawElementProviderSimple>(MockBehavior.Strict).Object;
            UiaTextProvider provider = new Mock<UiaTextProvider>(MockBehavior.Strict).Object;
            UiaTextRange textRange = new UiaTextRange(enclosingElement, provider, start: 0, end: 0);
            Array textAttributeIdentifiers = Enum.GetValues(typeof(TextAttributeIdentifier));

            foreach (int textAttributeIdentifier in textAttributeIdentifiers)
            {
                ITextRangeProvider? actual = ((ITextRangeProvider)textRange).FindAttribute(textAttributeIdentifier, new object(), backward.ToBOOL());
                Assert.Null(actual);
            }
        }

        public static IEnumerable<object?[]> UiaTextRange_ITextRangeProvider_FindText_Returns_Correct_TestData()
        {
            yield return new object?[] { "text", "text", BOOL.FALSE, BOOL.FALSE };
            yield return new object?[] { "other", null, BOOL.FALSE, BOOL.FALSE };
            yield return new object?[] { "TEXT", "text", BOOL.FALSE, BOOL.TRUE };
            yield return new object?[] { "TEXT", null, BOOL.FALSE, BOOL.FALSE };
        }

        [StaTheory]
        [MemberData(nameof(UiaTextRange_ITextRangeProvider_FindText_Returns_Correct_TestData))]
        internal void UiaTextRange_ITextRangeProvider_FindText_Returns_Correct(string textToSearch, string? foundText, BOOL backward, BOOL ignoreCase)
        {
            string testText = "Test text to find something.";
            IRawElementProviderSimple enclosingElement = new Mock<IRawElementProviderSimple>(MockBehavior.Strict).Object;
            Mock<UiaTextProvider> providerMock = new Mock<UiaTextProvider>(MockBehavior.Strict);
            providerMock.Setup(m => m.Text).Returns(testText);
            providerMock.Setup(m => m.TextLength).Returns(testText.Length);
            UiaTextProvider provider = providerMock.Object;
            UiaTextRange textRange = new UiaTextRange(enclosingElement, provider, start: 0, end: 28);

            ITextRangeProvider? actual = ((ITextRangeProvider)textRange).FindText(textToSearch, backward, ignoreCase);

            if (foundText != null)
            {
                Assert.Equal(foundText, actual?.GetText(5000));
            }
            else
            {
                Assert.Null(actual);
            }
        }

#pragma warning disable CS8625 // FindText doesn't accept a text null parameter
        [StaFact]
        internal void UiaTextRange_ITextRangeProvider_FindText_ReturnsNull_IfTextNull()
        {
            using (new NoAssertContext())
            {
                IRawElementProviderSimple enclosingElement = new Mock<IRawElementProviderSimple>(MockBehavior.Strict).Object;
                UiaTextProvider provider = new Mock<UiaTextProvider>(MockBehavior.Strict).Object;
                UiaTextRange textRange = new UiaTextRange(enclosingElement, provider, start: 0, end: 28);
                ITextRangeProvider? actual = ((ITextRangeProvider)textRange).FindText(null, BOOL.TRUE, BOOL.TRUE);
                Assert.Null(actual);
            }
        }
#pragma warning restore CS8625

        private static object? notSupportedValue;

        [DllImport(Libraries.UiaCore, ExactSpelling = true)]
        private static extern int UiaGetReservedNotSupportedValue([MarshalAs(UnmanagedType.IUnknown)] out object notSupportedValue);

        public static object UiaGetReservedNotSupportedValue()
        {
            if (notSupportedValue == null)
            {
                UiaGetReservedNotSupportedValue(out notSupportedValue);
            }

            return notSupportedValue;
        }

        public static IEnumerable<object[]> UiaTextRange_ITextRangeProvider_GetAttributeValue_Returns_Correct_TestData()
        {
            yield return new object[] { TextAttributeIdentifier.BackgroundColorAttributeId, GetSysColor(COLOR.WINDOW) };
            yield return new object[] { TextAttributeIdentifier.CapStyleAttributeId, CapStyle.None };
            yield return new object[] { TextAttributeIdentifier.FontNameAttributeId, "Segoe UI" };
            yield return new object[] { TextAttributeIdentifier.FontSizeAttributeId, 9.0 };
            yield return new object[] { TextAttributeIdentifier.FontWeightAttributeId, FW.NORMAL };
            yield return new object[] { TextAttributeIdentifier.ForegroundColorAttributeId, new COLORREF() };
            yield return new object[] { TextAttributeIdentifier.HorizontalTextAlignmentAttributeId, HorizontalTextAlignment.Left };
            yield return new object[] { TextAttributeIdentifier.IsItalicAttributeId, false };
            yield return new object[] { TextAttributeIdentifier.IsReadOnlyAttributeId, false };
            yield return new object[] { TextAttributeIdentifier.StrikethroughStyleAttributeId, TextDecorationLineStyle.None };
            yield return new object[] { TextAttributeIdentifier.UnderlineStyleAttributeId, TextDecorationLineStyle.None };

            yield return new object[] { TextAttributeIdentifier.AnimationStyleAttributeId, UiaGetReservedNotSupportedValue() };
            yield return new object[] { TextAttributeIdentifier.BulletStyleAttributeId, UiaGetReservedNotSupportedValue() };
            yield return new object[] { TextAttributeIdentifier.CultureAttributeId, UiaGetReservedNotSupportedValue() };
            yield return new object[] { TextAttributeIdentifier.IndentationFirstLineAttributeId, UiaGetReservedNotSupportedValue() };
            yield return new object[] { TextAttributeIdentifier.IndentationLeadingAttributeId, UiaGetReservedNotSupportedValue() };
            yield return new object[] { TextAttributeIdentifier.IndentationTrailingAttributeId, UiaGetReservedNotSupportedValue() };
            yield return new object[] { TextAttributeIdentifier.IsHiddenAttributeId, UiaGetReservedNotSupportedValue() };
            yield return new object[] { TextAttributeIdentifier.IsSubscriptAttributeId, UiaGetReservedNotSupportedValue() };
            yield return new object[] { TextAttributeIdentifier.IsSuperscriptAttributeId, UiaGetReservedNotSupportedValue() };
            yield return new object[] { TextAttributeIdentifier.MarginBottomAttributeId, UiaGetReservedNotSupportedValue() };
            yield return new object[] { TextAttributeIdentifier.MarginLeadingAttributeId, UiaGetReservedNotSupportedValue() };
            yield return new object[] { TextAttributeIdentifier.MarginTopAttributeId, UiaGetReservedNotSupportedValue() };
            yield return new object[] { TextAttributeIdentifier.MarginTrailingAttributeId, UiaGetReservedNotSupportedValue() };
            yield return new object[] { TextAttributeIdentifier.OutlineStylesAttributeId, UiaGetReservedNotSupportedValue() };
            yield return new object[] { TextAttributeIdentifier.OverlineColorAttributeId, UiaGetReservedNotSupportedValue() };
            yield return new object[] { TextAttributeIdentifier.OverlineStyleAttributeId, UiaGetReservedNotSupportedValue() };
            yield return new object[] { TextAttributeIdentifier.StrikethroughColorAttributeId, UiaGetReservedNotSupportedValue() };
            yield return new object[] { TextAttributeIdentifier.TabsAttributeId, UiaGetReservedNotSupportedValue() };
            yield return new object[] { TextAttributeIdentifier.TextFlowDirectionsAttributeId, UiaGetReservedNotSupportedValue() };
            yield return new object[] { TextAttributeIdentifier.UnderlineColorAttributeId, UiaGetReservedNotSupportedValue() };
            yield return new object[] { TextAttributeIdentifier.AnnotationTypesAttributeId, UiaGetReservedNotSupportedValue() };
            yield return new object[] { TextAttributeIdentifier.AnnotationObjectsAttributeId, UiaGetReservedNotSupportedValue() };
            yield return new object[] { TextAttributeIdentifier.StyleNameAttributeId, UiaGetReservedNotSupportedValue() };
            yield return new object[] { TextAttributeIdentifier.StyleIdAttributeId, UiaGetReservedNotSupportedValue() };
            yield return new object[] { TextAttributeIdentifier.LinkAttributeId, UiaGetReservedNotSupportedValue() };
            yield return new object[] { TextAttributeIdentifier.IsActiveAttributeId, UiaGetReservedNotSupportedValue() };
            yield return new object[] { TextAttributeIdentifier.SelectionActiveEndAttributeId, UiaGetReservedNotSupportedValue() };
            yield return new object[] { TextAttributeIdentifier.CaretPositionAttributeId, UiaGetReservedNotSupportedValue() };
            yield return new object[] { TextAttributeIdentifier.CaretBidiModeAttributeId, UiaGetReservedNotSupportedValue() };
            yield return new object[] { TextAttributeIdentifier.LineSpacingAttributeId, UiaGetReservedNotSupportedValue() };
            yield return new object[] { TextAttributeIdentifier.BeforeParagraphSpacingAttributeId, UiaGetReservedNotSupportedValue() };
            yield return new object[] { TextAttributeIdentifier.AfterParagraphSpacingAttributeId, UiaGetReservedNotSupportedValue() };
        }

        [StaTheory]
        [MemberData(nameof(UiaTextRange_ITextRangeProvider_GetAttributeValue_Returns_Correct_TestData))]
        internal void UiaTextRange_ITextRangeProvider_GetAttributeValue_Returns_Correct(int attributeId, object attributeValue)
        {
            IRawElementProviderSimple enclosingElement = new Mock<IRawElementProviderSimple>(MockBehavior.Strict).Object;
            Mock<UiaTextProvider> providerMock = new Mock<UiaTextProvider>(MockBehavior.Strict);
            using Font font = new Font("Segoe UI", 9, FontStyle.Regular);
            providerMock.Setup(m => m.Logfont).Returns(LOGFONTW.FromFont(font));
            providerMock.Setup(m => m.EditStyle).Returns(User32.ES.LEFT);
            providerMock.Setup(m => m.IsReadOnly).Returns(false);
            UiaTextProvider provider = providerMock.Object;
            UiaTextRange textRange = new UiaTextRange(enclosingElement, provider, start: 0, end: 28);
            object? actual = ((ITextRangeProvider)textRange).GetAttributeValue(attributeId);
            Assert.Equal(attributeValue, actual);
        }

        [StaFact]
        public void UiaTextRange_ITextRangeProvider_GetBoundingRectangles_ReturnsEmpty_for_DegenerateRange()
        {
            IRawElementProviderSimple enclosingElement = new Mock<IRawElementProviderSimple>(MockBehavior.Strict).Object;
            Mock<UiaTextProvider> providerMock = new Mock<UiaTextProvider>(MockBehavior.Strict);
            providerMock.Setup(p => p.TextLength).Returns(5);
            UiaTextProvider provider = providerMock.Object;
            UiaTextRange textRange = new UiaTextRange(enclosingElement, provider, start: 0, end: 0);
            var actual = ((ITextRangeProvider)textRange).GetBoundingRectangles();
            Assert.Empty(actual);
        }

        [StaFact]
        public void UiaTextRange_ITextRangeProvider_GetBoundingRectangles_ReturnsExpected_for_Endline()
        {
            IRawElementProviderSimple enclosingElement = new Mock<IRawElementProviderSimple>(MockBehavior.Strict).Object;
            Mock<UiaTextProvider> providerMock = new Mock<UiaTextProvider>(MockBehavior.Strict);
            providerMock.Setup(p => p.TextLength).Returns(3);
            providerMock.Setup(p => p.PointToScreen(It.IsAny<Point>())).Returns(Point.Empty);
            using Font font = new Font("Arial", 9f, FontStyle.Regular);
            providerMock.Setup(m => m.Logfont).Returns(LOGFONTW.FromFont(font));
            UiaTextProvider provider = providerMock.Object;
            UiaTextRange textRange = new UiaTextRange(enclosingElement, provider, start: 3, end: 3);
            double actualWidth = ((ITextRangeProvider)textRange).GetBoundingRectangles()[2]; // {X,Y,Width,Height}
            Assert.Equal(actualWidth, UiaTextProvider.EndOfLineWidth);
        }

        public static IEnumerable<object[]> UiaTextRange_ITextRangeProvider_GetBoundingRectangles_ReturnsCorrectValue_for_SingleLine_TestData()
        {
            yield return new object[] { 3, 6, new double[] { 27, 34, 11, 14 } };
            yield return new object[] { 0, 2, new double[] { 11, 34, 16, 15 } };
        }

        [StaTheory]
        [MemberData(nameof(UiaTextRange_ITextRangeProvider_GetBoundingRectangles_ReturnsCorrectValue_for_SingleLine_TestData))]
        public void UiaTextRange_ITextRangeProvider_GetBoundingRectangles_ReturnsCorrectValue_for_SingleLine(int start, int end, double[] expected)
        {
            string testText = "Test text.";
            Mock<IRawElementProviderSimple> enclosingElementMock = new Mock<IRawElementProviderSimple>(MockBehavior.Strict);
            enclosingElementMock.Setup(m => m.GetPropertyValue(UIA.BoundingRectanglePropertyId)).Returns(new Rectangle(10, 33, 96, 19));
            IRawElementProviderSimple enclosingElement = enclosingElementMock.Object;
            Mock<UiaTextProvider> providerMock = new Mock<UiaTextProvider>(MockBehavior.Strict);
            providerMock.Setup(m => m.Text).Returns(testText);
            providerMock.Setup(m => m.TextLength).Returns(testText.Length);
            providerMock.Setup(m => m.IsMultiline).Returns(false);
            providerMock.Setup(m => m.BoundingRectangle).Returns(new Rectangle(1, 1, 94, 15));
            providerMock.Setup(m => m.GetPositionFromChar(3)).Returns(new Point(17, 0));
            providerMock.Setup(m => m.GetPositionFromChar(0)).Returns(new Point(1, 0));
            providerMock.Setup(m => m.GetPositionFromCharForUpperRightCorner(5, testText)).Returns(new Point(28, 0));
            providerMock.Setup(m => m.GetPositionFromCharForUpperRightCorner(1, testText)).Returns(new Point(17, 0));
            UiaTextProvider provider = providerMock.Object;
            UiaTextRange textRange = new UiaTextRange(enclosingElement, provider, start, end);
            double[] actual = ((ITextRangeProvider)textRange).GetBoundingRectangles();

            // Acceptable deviation of 1 px.
            for (int i = 0; i < actual.Length; i++)
            {
                Assert.True(actual[i] >= 0 && actual[i] >= expected[i] - 1 && actual[i] <= expected[i] + 1);
            }
        }

        public static IEnumerable<object[]> UiaTextRange_ITextRangeProvider_GetBoundingRectangles_ReturnsCorrectValue_for_MultiLine_TestData()
        {
            yield return new object[] { 18, 30, new double[] { 14, 51, 12, 13, 14, 66, 52, 13 } };
            yield return new object[] { 32, 35, new double[] { 74, 66, 20, 13 } };
        }

        [StaTheory]
        [MemberData(nameof(UiaTextRange_ITextRangeProvider_GetBoundingRectangles_ReturnsCorrectValue_for_MultiLine_TestData))]
        public void UiaTextRange_ITextRangeProvider_GetBoundingRectangles_ReturnsCorrectValue_for_MultiLine(int start, int end, double[] expected)
        {
            string testText =
@"Test text on line 1.
Test text on line 2.";
            Mock<IRawElementProviderSimple> enclosingElementMock = new Mock<IRawElementProviderSimple>(MockBehavior.Strict);
            enclosingElementMock.Setup(m => m.GetPropertyValue(UIA.BoundingRectanglePropertyId)).Returns(new Rectangle(10, 33, 96, 56));
            IRawElementProviderSimple enclosingElement = enclosingElementMock.Object;
            Mock<UiaTextProvider> providerMock = new Mock<UiaTextProvider>(MockBehavior.Strict);
            using Font font = new Font("Arial", 9f, FontStyle.Regular);
            providerMock.Setup(m => m.Logfont).Returns(LOGFONTW.FromFont(font));
            providerMock.Setup(m => m.Text).Returns(testText);
            providerMock.Setup(m => m.TextLength).Returns(testText.Length);
            providerMock.Setup(m => m.IsMultiline).Returns(true);
            providerMock.Setup(m => m.BoundingRectangle).Returns(new Rectangle(4, 1, 88, 45));
            providerMock.Setup(m => m.GetLineFromCharIndex(18)).Returns(1);
            providerMock.Setup(m => m.GetLineFromCharIndex(29)).Returns(2);
            providerMock.Setup(m => m.GetLineFromCharIndex(32)).Returns(2);
            providerMock.Setup(m => m.GetLineFromCharIndex(34)).Returns(2);
            providerMock.Setup(m => m.FirstVisibleLine).Returns(0);
            providerMock.Setup(m => m.LinesPerPage).Returns(3);
            providerMock.Setup(m => m.GetLineIndex(1)).Returns(18);
            providerMock.Setup(m => m.GetPositionFromChar(18)).Returns(new Point(4, 16));
            providerMock.Setup(m => m.GetPositionFromChar(32)).Returns(new Point(64, 31));
            providerMock.Setup(m => m.GetLineIndex(2)).Returns(22);
            providerMock.Setup(m => m.GetPositionFromChar(21)).Returns(new Point(16, 16));
            providerMock.Setup(m => m.GetPositionFromChar(22)).Returns(new Point(4, 31));
            providerMock.Setup(m => m.GetPositionFromCharForUpperRightCorner(29, testText)).Returns(new Point(56, 31));
            providerMock.Setup(m => m.GetPositionFromCharForUpperRightCorner(34, testText)).Returns(new Point(84, 31));
            UiaTextProvider provider = providerMock.Object;
            var t = provider.Logfont;
            UiaTextRange textRange = new UiaTextRange(enclosingElement, provider, start, end);
            var actual = ((ITextRangeProvider)textRange).GetBoundingRectangles();
            Assert.Equal(expected, actual);
        }

        [StaFact]
        public void UiaTextRange_ITextRangeProvider_GetEnclosingElement_ReturnsCorrectValue()
        {
            IRawElementProviderSimple enclosingElement = new Mock<IRawElementProviderSimple>(MockBehavior.Strict).Object;
            UiaTextProvider provider = new Mock<UiaTextProvider>(MockBehavior.Strict).Object;
            UiaTextRange textRange = new UiaTextRange(enclosingElement, provider, start: 0, end: 0);
            IRawElementProviderSimple actual = ((ITextRangeProvider)textRange).GetEnclosingElement();
            Assert.Equal(enclosingElement, actual);
        }

        [StaTheory]
        [InlineData(0, 0, 0, "")]
        [InlineData(0, 0, 5, "")]
        [InlineData(0, 10, -5, "Some long ")]
        [InlineData(0, 10, 0, "")]
        [InlineData(0, 10, 10, "Some long ")]
        [InlineData(0, 10, 20, "Some long ")]
        [InlineData(0, 25, 7, "Some lo")]
        [InlineData(0, 300, 400, "Some long long test text")]
        [InlineData(5, 15, 7, "long lo")]
        [InlineData(5, 15, 25, "long long ")]
        [InlineData(5, 15, 300, "long long ")]
        [InlineData(5, 24, 400, "long long test text")]
        [InlineData(5, 25, 0, "")]
        [InlineData(5, 25, 7, "long lo")]
        [InlineData(5, 300, -5, "long long test text")]
        [InlineData(5, 300, 7, "long lo")]
        [InlineData(5, 300, 300, "long long test text")]
        public void UiaTextRange_ITextRangeProvider_GetText_ReturnsCorrectValue(int start, int end, int maxLength, string expected)
        {
            string testText = "Some long long test text";
            IRawElementProviderSimple enclosingElement = new Mock<IRawElementProviderSimple>(MockBehavior.Strict).Object;
            Mock<UiaTextProvider> providerMock = new Mock<UiaTextProvider>(MockBehavior.Strict);
            providerMock.Setup(m => m.Text).Returns(testText);
            providerMock.Setup(m => m.TextLength).Returns(testText.Length);
            UiaTextProvider provider = providerMock.Object;
            UiaTextRange textRange = new UiaTextRange(enclosingElement, provider, start, end);
            string actual = ((ITextRangeProvider)textRange).GetText(maxLength);
            Assert.Equal(expected, actual);
        }

        public static IEnumerable<object[]> UiaTextRange_ITextRangeProvider_Move_MovesCorrectly_TestData()
        {
            yield return new object[] { 0, 5, TextUnit.Character, 1, 6, 6 };
            yield return new object[] { 1, 6, TextUnit.Character, 5, 11, 11 };
            yield return new object[] { 0, 5, TextUnit.Character, -2, 0, 0 };
            yield return new object[] { 3, 6, TextUnit.Character, -2, 1, 1 };
            yield return new object[] { 1, 2, TextUnit.Word, 1, 4, 4 };
            yield return new object[] { 1, 2, TextUnit.Word, 5, 11, 11 };
            yield return new object[] { 12, 14, TextUnit.Word, -2, 8, 8 };
            yield return new object[] { 12, 14, TextUnit.Word, -10, 0, 0 };
        }

        [StaTheory]
        [MemberData(nameof(UiaTextRange_ITextRangeProvider_Move_MovesCorrectly_TestData))]
        internal void UiaTextRange_ITextRangeProvider_Move_MovesCorrectly(int start, int end, TextUnit unit, int count, int expectedStart, int expectedEnd)
        {
            string testText =
@"This is the text to move on - line 1
This is the line 2
This is the line 3";
            IRawElementProviderSimple enclosingElement = new Mock<IRawElementProviderSimple>(MockBehavior.Strict).Object;
            Mock<UiaTextProvider> providerMock = new Mock<UiaTextProvider>(MockBehavior.Strict);
            providerMock.Setup(m => m.Text).Returns(testText);
            providerMock.Setup(m => m.TextLength).Returns(testText.Length);
            UiaTextProvider provider = providerMock.Object;
            UiaTextRange textRange = new UiaTextRange(enclosingElement, provider, start, end);
            int result = ((ITextRangeProvider)textRange).Move(unit, count);
            Assert.Equal(expectedStart, textRange.Start);
            Assert.Equal(expectedEnd, textRange.End);
        }

        public static IEnumerable<object[]> UiaTextRange_ITextRangeProvider_MoveEndpointByUnit_MovesCorrectly_TestData()
        {
            yield return new object[] { 0, 5, TextPatternRangeEndpoint.Start, TextUnit.Character, 1, 1, 5 };
            yield return new object[] { 1, 6, TextPatternRangeEndpoint.Start, TextUnit.Character, 5, 6, 6 };
            yield return new object[] { 0, 5, TextPatternRangeEndpoint.Start, TextUnit.Character, -2, 0, 5 };
            yield return new object[] { 3, 6, TextPatternRangeEndpoint.Start, TextUnit.Character, -2, 1, 6 };
            yield return new object[] { 3, 6, TextPatternRangeEndpoint.End, TextUnit.Character, 1, 3, 7 };
            yield return new object[] { 3, 6, TextPatternRangeEndpoint.End, TextUnit.Character, -1, 3, 5 };
            yield return new object[] { 1, 2, TextPatternRangeEndpoint.Start, TextUnit.Word, 1, 4, 4 };
            yield return new object[] { 1, 2, TextPatternRangeEndpoint.Start, TextUnit.Word, 5, 11, 11 };
            yield return new object[] { 12, 14, TextPatternRangeEndpoint.Start, TextUnit.Word, -1, 11, 14 };
            yield return new object[] { 12, 14, TextPatternRangeEndpoint.Start, TextUnit.Word, -2, 8, 14 };
        }

        [StaTheory]
        [MemberData(nameof(UiaTextRange_ITextRangeProvider_MoveEndpointByUnit_MovesCorrectly_TestData))]
        internal void UiaTextRange_ITextRangeProvider_MoveEndpointByUnit_MovesCorrectly(int start, int end, TextPatternRangeEndpoint endpoint, TextUnit unit, int count, int expectedStart, int expectedEnd)
        {
            string testText =
@"This is the text to move on - line 1
This is the line 2
This is the line 3";
            IRawElementProviderSimple enclosingElement = new Mock<IRawElementProviderSimple>(MockBehavior.Strict).Object;
            Mock<UiaTextProvider> providerMock = new Mock<UiaTextProvider>(MockBehavior.Strict);
            providerMock.Setup(m => m.Text).Returns(testText);
            providerMock.Setup(m => m.TextLength).Returns(testText.Length);
            UiaTextProvider provider = providerMock.Object;
            UiaTextRange textRange = new UiaTextRange(enclosingElement, provider, start, end);
            ((ITextRangeProvider)textRange).MoveEndpointByUnit(endpoint, unit, count);
            Assert.Equal(expectedStart, textRange.Start);
            Assert.Equal(expectedEnd, textRange.End);
        }

        public static IEnumerable<object[]> UiaTextRange_ITextRangeProvider_MoveEndpointByRange_MovesCorrectly_TestData()
        {
            yield return new object[] { 0, 5, TextPatternRangeEndpoint.Start, 7, 10, TextPatternRangeEndpoint.Start, 7, 7 };
            yield return new object[] { 0, 5, TextPatternRangeEndpoint.Start, 7, 10, TextPatternRangeEndpoint.End, 10, 10 };
            yield return new object[] { 0, 5, TextPatternRangeEndpoint.End, 7, 10, TextPatternRangeEndpoint.Start, 0, 7 };
            yield return new object[] { 0, 5, TextPatternRangeEndpoint.End, 7, 10, TextPatternRangeEndpoint.End, 0, 10 };
        }

        [StaTheory]
        [MemberData(nameof(UiaTextRange_ITextRangeProvider_MoveEndpointByRange_MovesCorrectly_TestData))]
        internal void UiaTextRange_ITextRangeProvider_MoveEndpointByRange_MovesCorrectly(int start, int end, TextPatternRangeEndpoint endpoint, int targetRangeStart, int targetRangeEnd, TextPatternRangeEndpoint targetEndpoint, int expectedStart, int expectedEnd)
        {
            IRawElementProviderSimple enclosingElement = new Mock<IRawElementProviderSimple>(MockBehavior.Strict).Object;
            UiaTextProvider provider = new Mock<UiaTextProvider>(MockBehavior.Strict).Object;
            UiaTextRange textRange = new UiaTextRange(enclosingElement, provider, start, end);
            UiaTextRange targetRange = new UiaTextRange(enclosingElement, provider, targetRangeStart, targetRangeEnd);
            ((ITextRangeProvider)textRange).MoveEndpointByRange(endpoint, targetRange, targetEndpoint);
            Assert.Equal(expectedStart, textRange.Start);
            Assert.Equal(expectedEnd, textRange.End);
        }

        [StaTheory]
        [InlineData(0, 0)]
        [InlineData(0, 10)]
        [InlineData(5, 10)]
        public void UiaTextRange_ITextRangeProvider_Select_ReturnsCorrectValue(int start, int end)
        {
            IRawElementProviderSimple enclosingElement = new Mock<IRawElementProviderSimple>(MockBehavior.Strict).Object;
            Mock<UiaTextProvider> providerMock = new Mock<UiaTextProvider>(MockBehavior.Strict);
            providerMock.Setup(m => m.SetSelection(start, end));
            UiaTextProvider provider = providerMock.Object;
            UiaTextRange textRange = new UiaTextRange(enclosingElement, provider, start, end);
            ((ITextRangeProvider)textRange).Select();
            providerMock.Verify(m => m.SetSelection(start, end), Times.Once());
        }

        [StaFact]
        public void UiaTextRange_ITextRangeProvider_AddToSelection_DoesntThrowException()
        {
            // Check an app doesn't crash when calling AddToSelectio method.
            IRawElementProviderSimple enclosingElement = new Mock<IRawElementProviderSimple>(MockBehavior.Strict).Object;
            UiaTextProvider provider = new Mock<UiaTextProvider>(MockBehavior.Strict).Object;
            UiaTextRange textRange = new UiaTextRange(enclosingElement, provider, 3, 7);
            ((ITextRangeProvider)textRange).AddToSelection();
        }

        [StaFact]
        public void UiaTextRange_ITextRangeProvider_RemoveFromSelection_DoesntThrowException()
        {
            // Check an app doesn't crash when calling RemoveFromSelection method.
            IRawElementProviderSimple enclosingElement = new Mock<IRawElementProviderSimple>(MockBehavior.Strict).Object;
            UiaTextProvider provider = new Mock<UiaTextProvider>(MockBehavior.Strict).Object;
            UiaTextRange textRange = new UiaTextRange(enclosingElement, provider, 3, 7);
            ((ITextRangeProvider)textRange).RemoveFromSelection();
        }

        public static IEnumerable<object[]> UiaTextRange_ITextRangeProvider_ScrollIntoView_Multiline_CallsLineScrollCorrectly_TestData()
        {
            yield return new object[] { 30, 35, 30, 1, 30, 0, 1 };
            yield return new object[] { 60, 65, 60, 2, 60, 2, 0 };
        }

        [StaTheory]
        [MemberData(nameof(UiaTextRange_ITextRangeProvider_ScrollIntoView_Multiline_CallsLineScrollCorrectly_TestData))]
        public void UiaTextRange_ITextRangeProvider_ScrollIntoView_Multiline_CallsLineScrollCorrectly(int start, int end, int charIndex, int lineForCharIndex, int charactersHorizontal, int linesVertical, int firstVisibleLine)
        {
            IRawElementProviderSimple enclosingElement = new Mock<IRawElementProviderSimple>(MockBehavior.Strict).Object;

            var providerMock = new Mock<UiaTextProvider>(MockBehavior.Strict);
            providerMock.Setup(p => p.IsMultiline).Returns(true);
            providerMock.Setup(p => p.GetLineFromCharIndex(charIndex)).Returns(lineForCharIndex);
            providerMock.Setup(p => p.LineScroll(charactersHorizontal, linesVertical)).Returns(true);
            providerMock.Setup(p => p.FirstVisibleLine).Returns(firstVisibleLine);

            UiaTextProvider provider = providerMock.Object;
            UiaTextRange textRange = new UiaTextRange(enclosingElement, provider, start, end);
            ((ITextRangeProvider)textRange).ScrollIntoView(BOOL.TRUE);
            providerMock.Verify(e => e.LineScroll(charactersHorizontal, linesVertical), Times.Once());
        }

        public static IEnumerable<object[]> UiaTextRange_ITextRangeProvider_ScrollIntoView_SingleLine_ExecutesCorrectly_TestData()
        {
            yield return new object[] { 0, 30, true, false };
            yield return new object[] { 70, 85, true, false };
        }

        [StaTheory]
        [MemberData(nameof(UiaTextRange_ITextRangeProvider_ScrollIntoView_SingleLine_ExecutesCorrectly_TestData))]
        public void UiaTextRange_ITextRangeProvider_ScrollIntoView_SingleLine_ExecutesCorrectly(int start, int end, bool scrollable, bool readingRTL)
        {
            IRawElementProviderSimple enclosingElement = new Mock<IRawElementProviderSimple>(MockBehavior.Strict).Object;
            int visibleStart = 40;
            int visibleEnd = 60;

            var providerMock = new Mock<UiaTextProvider>(MockBehavior.Strict);
            providerMock.Setup(p => p.IsMultiline).Returns(false);
            providerMock.Setup(p => p.IsScrollable).Returns(scrollable);
            providerMock.Setup(p => p.IsReadingRTL).Returns(readingRTL);
            providerMock.Setup(p => p.GetVisibleRangePoints(out visibleStart, out visibleEnd));

            UiaTextProvider provider = providerMock.Object;
            UiaTextRange textRange = new UiaTextRange(enclosingElement, provider, start, end);
            ((ITextRangeProvider)textRange).ScrollIntoView(BOOL.TRUE);
            providerMock.Verify(p => p.GetVisibleRangePoints(out visibleStart, out visibleEnd), Times.Exactly(2));
        }

        [StaFact]
        public void UiaTextRange_ITextRangeProvider_GetChildren_ReturnsCorrectValue()
        {
            IRawElementProviderSimple enclosingElement = new Mock<IRawElementProviderSimple>(MockBehavior.Strict).Object;
            UiaTextProvider provider = new Mock<UiaTextProvider>(MockBehavior.Strict).Object;
            UiaTextRange textRange = new UiaTextRange(enclosingElement, provider, start: 0, end: 0);
            IRawElementProviderSimple[] actual = ((ITextRangeProvider)textRange).GetChildren();
            Assert.Empty(actual);
        }

        [StaTheory]
        [InlineData("", 0, true)]
        [InlineData("", 5, true)]
        [InlineData("", -5, true)]
        [InlineData("Some text", 0, true)]
        [InlineData("Some text", 5, false)]
        [InlineData("Some text", 6, false)]
        [InlineData("Some text", 99, true)]
        [InlineData("Some text", -5, true)]
        [InlineData("Some, text", 4, false)]
        [InlineData("Some text", 4, false)]
        [InlineData("1dsf'21gj", 3, false)]
        [InlineData("1dsf'21gj", 4, false)]
        [InlineData("1dsf'21gj", 6, false)]
        [InlineData("1d??sf'21gj", 6, false)]
        public void UiaTextRange_private_AtParagraphBoundary_ReturnsCorrectValue(string text, int index, bool expected)
        {
            bool actual = StaticNullTextRange.TestAccessor().AtParagraphBoundary(text, index);
            Assert.Equal(expected, actual);
        }

        [StaTheory]
        [InlineData("", 0, true)]
        [InlineData("", 5, true)]
        [InlineData("", -5, true)]
        [InlineData("Some text", 0, true)]
        [InlineData("Some text", 5, true)]
        [InlineData("Some text", 6, false)]
        [InlineData("Some text", 99, true)]
        [InlineData("Some text", -5, true)]
        [InlineData("Some, text", 4, true)]
        [InlineData("Some text", 4, true)]
        [InlineData("1dsf'21gj", 3, false)]
        [InlineData("1dsf'21gj", 4, false)]
        [InlineData("1dsf'21gj", 6, false)]
        [InlineData("1d??sf'21gj", 6, false)]
        public void UiaTextRange_private_AtWordBoundary_ReturnsCorrectValue(string text, int index, bool expected)
        {
            bool actual = StaticNullTextRange.TestAccessor().AtWordBoundary(text, index);
            Assert.Equal(expected, actual);
        }

        [StaTheory]
        [InlineData('\'', true)]
        [InlineData((char)0x2019, true)]
        [InlineData('\t', false)]
        [InlineData('t', false)]
        public void UiaTextRange_private_IsApostrophe_ReturnsCorrectValue(char ch, bool expected)
        {
            bool actual = StaticNullTextRange.TestAccessor().IsApostrophe(ch);
            Assert.Equal(expected, actual);
        }

        [StaTheory]
        [InlineData((int)ES.CENTER, (int)HorizontalTextAlignment.Centered)]
        [InlineData((int)ES.LEFT, (int)HorizontalTextAlignment.Left)]
        [InlineData((int)ES.RIGHT, (int)HorizontalTextAlignment.Right)]
        public void UiaTextRange_private_GetHorizontalTextAlignment_ReturnsCorrectValue(int style, int expected)
        {
            IRawElementProviderSimple enclosingElement = new Mock<IRawElementProviderSimple>(MockBehavior.Strict).Object;
            UiaTextProvider provider = new Mock<UiaTextProvider>(MockBehavior.Strict).Object;
            UiaTextRange textRange = new UiaTextRange(enclosingElement, provider, 0, 0);

            HorizontalTextAlignment actual = textRange.TestAccessor().GetHorizontalTextAlignment((ES)style);

            Assert.Equal((HorizontalTextAlignment)expected, actual);
        }

        [StaTheory]
        [InlineData((int)(ES.UPPERCASE | ES.LEFT | ES.MULTILINE | ES.READONLY | ES.AUTOHSCROLL), (int)CapStyle.AllCap)]
        [InlineData((int)(ES.LOWERCASE | ES.LEFT | ES.MULTILINE | ES.READONLY | ES.AUTOHSCROLL), (int)CapStyle.None)]
        public void UiaTextRange_private_GetCapStyle_ReturnsExpectedValue(int editStyle, int expected)
        {
            IRawElementProviderSimple enclosingElement = new Mock<IRawElementProviderSimple>(MockBehavior.Strict).Object;
            UiaTextProvider provider = new Mock<UiaTextProvider>(MockBehavior.Strict).Object;
            UiaTextRange textRange = new UiaTextRange(enclosingElement, provider, 0, 0);

            CapStyle actual = textRange.TestAccessor().GetCapStyle((ES)editStyle);

            Assert.Equal((CapStyle)expected, actual);
        }

        [StaTheory]
        [InlineData(true)]
        [InlineData(false)]
        public void UiaTextRange_private_GetReadOnly_ReturnsCorrectValue(bool readOnly)
        {
            IRawElementProviderSimple enclosingElement = new Mock<IRawElementProviderSimple>(MockBehavior.Strict).Object;
            Mock<UiaTextProvider> providerMock = new Mock<UiaTextProvider>(MockBehavior.Strict);
            providerMock.Setup(m => m.IsReadOnly).Returns(readOnly);
            UiaTextProvider provider = providerMock.Object;
            UiaTextRange textRange = new UiaTextRange(enclosingElement, provider, 0, 0);

            bool actual = textRange.TestAccessor().GetReadOnly();

            Assert.Equal(readOnly, actual);
        }

        [StaFact]
        public void UiaTextRange_private_GetBackgroundColor_ReturnsExpectedValue()
        {
            COLORREF actual = StaticNullTextRange.TestAccessor().GetBackgroundColor();
            uint expected = 0x00ffffff; // WINDOW system color
            Assert.Equal(expected, actual);
        }

        [StaTheory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData("Some test text")]
        public void UiaTextRange_private_GetFontName_ReturnsExpectedValue(string faceName)
        {
            LOGFONTW logfont = new LOGFONTW
            {
                FaceName = faceName
            };

            string actual = StaticNullTextRange.TestAccessor().GetFontName(logfont);

            Assert.Equal(faceName ?? "", actual);
        }

        [StaTheory]
        [InlineData(1, 1)]
        [InlineData(5, 5)]
        [InlineData(5.3, 5)]
        [InlineData(9.5, 10)]
        [InlineData(18, 18)]
        [InlineData(18.8, 19)]
        [InlineData(100, 100)]
        public void UiaTextRange_private_GetFontSize_ReturnsCorrectValue(float fontSize, double expected)
        {
            IRawElementProviderSimple enclosingElement = new Mock<IRawElementProviderSimple>(MockBehavior.Strict).Object;
            Mock<UiaTextProvider> providerMock = new Mock<UiaTextProvider>(MockBehavior.Strict);
            using Font font = new Font("Arial", fontSize, FontStyle.Regular);
            providerMock.Setup(m => m.Logfont).Returns(LOGFONTW.FromFont(font));
            UiaTextProvider provider = providerMock.Object;
            UiaTextRange textRange = new UiaTextRange(enclosingElement, provider, 5, 20);

            double actual = textRange.TestAccessor().GetFontSize(provider.Logfont);

            Assert.Equal(expected, actual);
        }

        [StaTheory]
        [InlineData(FW.BLACK)]
        [InlineData(FW.BOLD)]
        [InlineData(FW.DEMIBOLD)]
        [InlineData(FW.DONTCARE)]
        [InlineData(FW.EXTRABOLD)]
        [InlineData(FW.EXTRALIGHT)]
        [InlineData(FW.LIGHT)]
        [InlineData(FW.MEDIUM)]
        [InlineData(FW.NORMAL)]
        [InlineData(FW.THIN)]
        public void UiaTextRange_private_GetFontWeight_ReturnsCorrectValue(object fontWeight)
        {
            LOGFONTW logfont = new LOGFONTW() { lfWeight = (FW)fontWeight };
            FW actual = StaticNullTextRange.TestAccessor().GetFontWeight(logfont);
            Assert.Equal(fontWeight, actual);
        }

        [StaFact]
        public void UiaTextRange_private_GetForegroundColor_ReturnsCorrectValue()
        {
            COLORREF actual = StaticNullTextRange.TestAccessor().GetForegroundColor();
            Assert.Equal(new COLORREF(), actual);
        }

        [StaTheory]
        [InlineData(0, false)]
        [InlineData(5, true)]
        public void UiaTextRange_private_GetItalic_ReturnsCorrectValue(byte ifItalic, bool expected)
        {
            LOGFONTW logfont = new LOGFONTW() { lfItalic = ifItalic };

            bool actual = StaticNullTextRange.TestAccessor().GetItalic(logfont);

            Assert.Equal(expected, actual);
        }

        [StaTheory]
        [InlineData(0, (int)TextDecorationLineStyle.None)]
        [InlineData(5, (int)TextDecorationLineStyle.Single)]
        public void UiaTextRange_private_GetStrikethroughStyle_ReturnsCorrectValue(byte ifStrikeOut, int expected)
        {
            LOGFONTW logfont = new LOGFONTW() { lfStrikeOut = ifStrikeOut };
            TextDecorationLineStyle actual = StaticNullTextRange.TestAccessor().GetStrikethroughStyle(logfont);

            Assert.Equal((TextDecorationLineStyle)expected, actual);
        }

        [StaTheory]
        [InlineData(0, (int)TextDecorationLineStyle.None)]
        [InlineData(5, (int)TextDecorationLineStyle.Single)]
        public void UiaTextRange_private_GetUnderlineStyle_ReturnsCorrectValue(byte ifUnderline, int expected)
        {
            LOGFONTW logfont = new LOGFONTW() { lfUnderline = ifUnderline };
            TextDecorationLineStyle actual = StaticNullTextRange.TestAccessor().GetUnderlineStyle(logfont);

            Assert.Equal((TextDecorationLineStyle)expected, actual);
        }

        [StaTheory]
        [InlineData(0, 0)]
        [InlineData(0, 10)]
        [InlineData(5, 10)]
        [InlineData(5, 100)]
        [InlineData(100, 100)]
        [InlineData(100, 200)]
        public void UiaTextRange_private_MoveTo_SetValuesCorrectly(int start, int end)
        {
            IRawElementProviderSimple enclosingElement = new Mock<IRawElementProviderSimple>(MockBehavior.Strict).Object;
            UiaTextProvider provider = new Mock<UiaTextProvider>(MockBehavior.Strict).Object;
            UiaTextRange textRange = new UiaTextRange(enclosingElement, provider, 0, 0);

            textRange.TestAccessor().MoveTo(start, end);

            Assert.Equal(start, textRange.Start);
            Assert.Equal(end, textRange.End);
        }

        [StaTheory]
        [InlineData(-5, 0)]
        [InlineData(0, -5)]
        [InlineData(-10, -10)]
        [InlineData(10, 5)]
        public void UiaTextRange_private_MoveTo_ThrowsException_IfIncorrectParameters(int start, int end)
        {
            IRawElementProviderSimple enclosingElement = new Mock<IRawElementProviderSimple>(MockBehavior.Strict).Object;
            UiaTextProvider provider = new Mock<UiaTextProvider>(MockBehavior.Strict).Object;
            UiaTextRange textRange = new UiaTextRange(enclosingElement, provider, 0, 0);

            Assert.ThrowsAny<Exception>(() => textRange.TestAccessor().MoveTo(start, end));
        }

        [StaTheory]
        [InlineData(0, 0, 0, 0)]
        [InlineData(0, 10, 0, 10)]
        [InlineData(5, 10, 5, 10)]
        [InlineData(5, 100, 5, 24)]
        [InlineData(100, 100, 24, 24)]
        [InlineData(100, 200, 24, 24)]
        public void UiaTextRange_private_ValidateEndpoints_SetValuesCorrectly(int start, int end, int expectedStart, int expectedEnd)
        {
            string testText = "Some long long test text";
            IRawElementProviderSimple enclosingElement = new Mock<IRawElementProviderSimple>(MockBehavior.Strict).Object;
            Mock<UiaTextProvider> providerMock = new Mock<UiaTextProvider>(MockBehavior.Strict);
            providerMock.Setup(m => m.TextLength).Returns(testText.Length);
            UiaTextProvider provider = providerMock.Object;
            UiaTextRange textRange = new UiaTextRange(enclosingElement, provider, start, end);

            textRange.TestAccessor().ValidateEndpoints();

            Assert.Equal(expectedStart, textRange.Start);
            Assert.Equal(expectedEnd, textRange.End);
        }
    }
}
