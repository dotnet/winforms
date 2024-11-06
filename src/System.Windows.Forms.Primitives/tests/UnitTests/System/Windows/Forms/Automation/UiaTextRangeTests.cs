// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;
using System.Windows.Forms.Automation;
using Moq;
using Windows.Win32.System.Com;
using Windows.Win32.System.Variant;
using Windows.Win32.UI.Accessibility;

namespace System.Windows.Forms.Primitives.Tests.Automation;

public unsafe class UiaTextRangeTests
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
        IRawElementProviderSimple.Interface enclosingElement = new Mock<IRawElementProviderSimple.Interface>(MockBehavior.Strict).Object;
        UiaTextProvider provider = new Mock<UiaTextProvider>(MockBehavior.Strict).Object;
        UiaTextRange textRange = new(enclosingElement, provider, start, end);
        Assert.Equal(start, textRange.Start);
        Assert.Equal(end, textRange.End);

        using ComScope<IRawElementProviderSimple> elementProviderScope = new(null);
        Assert.True(((ITextRangeProvider.Interface)textRange).GetEnclosingElement(elementProviderScope).Succeeded);
        Assert.Equal(enclosingElement, ComHelpers.GetObjectForIUnknown(elementProviderScope));

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
        IRawElementProviderSimple.Interface enclosingElement = new Mock<IRawElementProviderSimple.Interface>(MockBehavior.Strict).Object;
        UiaTextProvider provider = new Mock<UiaTextProvider>(MockBehavior.Strict).Object;
        UiaTextRange textRange = new(enclosingElement, provider, start, end);
        Assert.Equal(expectedStart, textRange.Start);
        Assert.Equal(expectedEnd, textRange.End);
    }

#pragma warning disable CS8625 // UiaTextRange constructor doesn't accept a provider null parameter
    [StaFact]
    public void UiaTextRange_Constructor_Provider_Null_ThrowsException()
    {
        IRawElementProviderSimple.Interface enclosingElement = new Mock<IRawElementProviderSimple.Interface>(MockBehavior.Strict).Object;
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
        IRawElementProviderSimple.Interface enclosingElement = new Mock<IRawElementProviderSimple.Interface>(MockBehavior.Strict).Object;
        UiaTextProvider provider = new Mock<UiaTextProvider>(MockBehavior.Strict).Object;
        UiaTextRange textRange = new(enclosingElement, provider, start, end);
        Assert.True(textRange.Start >= 0);
        Assert.True(textRange.End >= 0);
    }

    [StaTheory]
    [InlineData(0)]
    [InlineData(5)]
    [InlineData(int.MaxValue)]
    public void UiaTextRange_End_Get_ReturnsCorrectValue(int end)
    {
        IRawElementProviderSimple.Interface enclosingElement = new Mock<IRawElementProviderSimple.Interface>(MockBehavior.Strict).Object;
        UiaTextProvider provider = new Mock<UiaTextProvider>(MockBehavior.Strict).Object;
        UiaTextRange textRange = new(enclosingElement, provider, start: 0, end);
        Assert.Equal(end, textRange.End);
    }

    [StaTheory]
    [InlineData(0)]
    [InlineData(5)]
    [InlineData(int.MaxValue)]
    public void UiaTextRange_End_SetCorrectly(int end)
    {
        IRawElementProviderSimple.Interface enclosingElement = new Mock<IRawElementProviderSimple.Interface>(MockBehavior.Strict).Object;
        UiaTextProvider provider = new Mock<UiaTextProvider>(MockBehavior.Strict).Object;
        UiaTextRange textRange = new(enclosingElement, provider, start: 0, end: 0)
        {
            End = end
        };
        int actual = textRange.End < textRange.Start ? textRange.Start : textRange.End;
        Assert.Equal(end, actual);
    }

    [StaFact]
    public void UiaTextRange_End_SetCorrect_IfValueIncorrect()
    {
        IRawElementProviderSimple.Interface enclosingElement = new Mock<IRawElementProviderSimple.Interface>(MockBehavior.Strict).Object;
        UiaTextProvider provider = new Mock<UiaTextProvider>(MockBehavior.Strict).Object;
        UiaTextRange textRange = new(enclosingElement, provider, start: 5, end: 10)
        {
            End = 3  /*Incorrect value*/
        };
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
        IRawElementProviderSimple.Interface enclosingElement = new Mock<IRawElementProviderSimple.Interface>(MockBehavior.Strict).Object;
        UiaTextProvider provider = new Mock<UiaTextProvider>(MockBehavior.Strict).Object;
        UiaTextRange textRange = new(enclosingElement, provider, start, end);
        Assert.Equal(expected, textRange.Length);
    }

    [StaTheory]
    [InlineData(-5, 0)]
    [InlineData(0, -5)]
    [InlineData(-5, -5)]
    [InlineData(10, 5)]
    public void UiaTextRange_Length_ReturnsCorrectValue_IfIncorrectStartEnd(int start, int end)
    {
        IRawElementProviderSimple.Interface enclosingElement = new Mock<IRawElementProviderSimple.Interface>(MockBehavior.Strict).Object;
        UiaTextProvider provider = new Mock<UiaTextProvider>(MockBehavior.Strict).Object;
        UiaTextRange textRange = new(enclosingElement, provider, 3, 10);

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
        IRawElementProviderSimple.Interface enclosingElement = new Mock<IRawElementProviderSimple.Interface>(MockBehavior.Strict).Object;
        UiaTextProvider provider = new Mock<UiaTextProvider>(MockBehavior.Strict).Object;
        UiaTextRange textRange = new(enclosingElement, provider, start: 0, end: 0);

        textRange.TestAccessor()._start = start;

        Assert.Equal(start, textRange.Start);
    }

    [StaTheory]
    [InlineData(0)]
    [InlineData(5)]
    [InlineData(int.MaxValue)]
    public void UiaTextRange_Start_SetCorrectly(int start)
    {
        IRawElementProviderSimple.Interface enclosingElement = new Mock<IRawElementProviderSimple.Interface>(MockBehavior.Strict).Object;
        UiaTextProvider provider = new Mock<UiaTextProvider>(MockBehavior.Strict).Object;
        UiaTextRange textRange = new(enclosingElement, provider, start: 0, end: 0)
        {
            Start = start
        };
        int actual = textRange.Start < textRange.End ? textRange.End : textRange.Start;
        Assert.Equal(start, actual);
    }

    [StaFact]
    public void UiaTextRange_Start_Set_Correct_IfValueIncorrect()
    {
        IRawElementProviderSimple.Interface enclosingElement = new Mock<IRawElementProviderSimple.Interface>(MockBehavior.Strict).Object;
        UiaTextProvider provider = new Mock<UiaTextProvider>(MockBehavior.Strict).Object;
        UiaTextRange textRange = new(enclosingElement, provider, start: 4, end: 8)
        {
            Start = -10
        };
        Assert.Equal(0, textRange.Start);
        Assert.Equal(8, textRange.End);
    }

    [StaFact]
    public void UiaTextRange_Start_Set_Correct_IfValueMoreThanEnd()
    {
        IRawElementProviderSimple.Interface enclosingElement = new Mock<IRawElementProviderSimple.Interface>(MockBehavior.Strict).Object;
        UiaTextProvider provider = new Mock<UiaTextProvider>(MockBehavior.Strict).Object;
        UiaTextRange textRange = new(enclosingElement, provider, start: 4, end: 10)
        {
            Start = 15 // More than End = 10
        };
        Assert.True(textRange.Start <= textRange.End);
    }

    [StaFact]
    public void UiaTextRange_ITextRangeProvider_Clone_ReturnsCorrectValue()
    {
        IRawElementProviderSimple.Interface enclosingElement = new Mock<IRawElementProviderSimple.Interface>(MockBehavior.Strict).Object;
        UiaTextProvider provider = new Mock<UiaTextProvider>(MockBehavior.Strict).Object;
        UiaTextRange textRange = new(enclosingElement, provider, start: 3, end: 9);

        using ComScope<ITextRangeProvider> rangeScope = new(null);
        Assert.True(((ITextRangeProvider.Interface)textRange).Clone(rangeScope).Succeeded);
        UiaTextRange actual = (UiaTextRange)ComHelpers.GetObjectForIUnknown(rangeScope);
        Assert.Equal(textRange.Start, actual.Start);
        Assert.Equal(textRange.End, actual.End);
    }

    [StaTheory]
    [InlineData(3, 9, true)]
    [InlineData(0, 2, false)]
    public void UiaTextRange_ITextRangeProvider_Compare_ReturnsCorrectValue(int start, int end, bool expected)
    {
        IRawElementProviderSimple.Interface enclosingElement = new Mock<IRawElementProviderSimple.Interface>(MockBehavior.Strict).Object;
        UiaTextProvider provider = new Mock<UiaTextProvider>(MockBehavior.Strict).Object;
        UiaTextRange textRange1 = new(enclosingElement, provider, start: 3, end: 9);
        UiaTextRange textRange2 = new(enclosingElement, provider, start, end);
        BOOL actual = default;
        using var textRange2Scope = ComHelpers.GetComScope<ITextRangeProvider>(textRange2);
        Assert.True(((ITextRangeProvider.Interface)textRange1).Compare(textRange2Scope, &actual).Succeeded);
        Assert.Equal(expected, (bool)actual);
    }

    public static IEnumerable<object[]> UiaTextRange_ITextRangeProvider_CompareEndpoints_ReturnsCorrectValue_TestData()
    {
        yield return new object[] { TextPatternRangeEndpoint.TextPatternRangeEndpoint_Start, 3, 9, TextPatternRangeEndpoint.TextPatternRangeEndpoint_Start, 0 };
        yield return new object[] { TextPatternRangeEndpoint.TextPatternRangeEndpoint_End, 3, 9, TextPatternRangeEndpoint.TextPatternRangeEndpoint_Start, 6 };
        yield return new object[] { TextPatternRangeEndpoint.TextPatternRangeEndpoint_Start, 3, 9, TextPatternRangeEndpoint.TextPatternRangeEndpoint_End, -6 };
        yield return new object[] { TextPatternRangeEndpoint.TextPatternRangeEndpoint_End, 3, 9, TextPatternRangeEndpoint.TextPatternRangeEndpoint_End, 0 };
        yield return new object[] { TextPatternRangeEndpoint.TextPatternRangeEndpoint_Start, 0, 0, TextPatternRangeEndpoint.TextPatternRangeEndpoint_Start, 3 };
        yield return new object[] { TextPatternRangeEndpoint.TextPatternRangeEndpoint_End, 0, 0, TextPatternRangeEndpoint.TextPatternRangeEndpoint_Start, 9 };
        yield return new object[] { TextPatternRangeEndpoint.TextPatternRangeEndpoint_End, 1, 15, TextPatternRangeEndpoint.TextPatternRangeEndpoint_End, -6 };
        yield return new object[] { TextPatternRangeEndpoint.TextPatternRangeEndpoint_Start, 1, 15, TextPatternRangeEndpoint.TextPatternRangeEndpoint_End, -12 };
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
        IRawElementProviderSimple.Interface enclosingElement = new Mock<IRawElementProviderSimple.Interface>(MockBehavior.Strict).Object;
        UiaTextProvider provider = new Mock<UiaTextProvider>(MockBehavior.Strict).Object;
        UiaTextRange textRange = new(enclosingElement, provider, start: 3, end: 9);
        UiaTextRange targetRange = new(enclosingElement, provider, start: targetStart, end: targetEnd);
        using var textRangeScope = ComHelpers.GetComScope<ITextRangeProvider>(targetRange);
        int actual;
        Assert.True(((ITextRangeProvider.Interface)textRange).CompareEndpoints((TextPatternRangeEndpoint)endpoint, textRangeScope, (TextPatternRangeEndpoint)targetEndpoint, &actual).Succeeded);
        Assert.Equal(expected, actual);
    }

    [StaTheory]
    [InlineData(2, 2, 2, 3)]
    [InlineData(8, 9, 8, 9)]
    [InlineData(0, 3, 0, 3)]
    public void UiaTextRange_ITextRangeProvider_ExpandToEnclosingUnit_ExpandsToCharacter(int start, int end, int expandedStart, int expandedEnd)
    {
        IRawElementProviderSimple.Interface enclosingElement = new Mock<IRawElementProviderSimple.Interface>(MockBehavior.Strict).Object;
        Mock<UiaTextProvider> providerMock = new(MockBehavior.Strict);
        providerMock.Setup(m => m.TextLength).Returns("words, words, words".Length);
        UiaTextProvider provider = providerMock.Object;
        UiaTextRange textRange = new(enclosingElement, provider, start, end);
        Assert.True(((ITextRangeProvider.Interface)textRange).ExpandToEnclosingUnit(TextUnit.TextUnit_Character).Succeeded);
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
        IRawElementProviderSimple.Interface enclosingElement = new Mock<IRawElementProviderSimple.Interface>(MockBehavior.Strict).Object;
        Mock<UiaTextProvider> providerMock = new(MockBehavior.Strict);
        providerMock.Setup(m => m.Text).Returns(testText);
        providerMock.Setup(m => m.TextLength).Returns(testText.Length);
        UiaTextProvider provider = providerMock.Object;
        UiaTextRange textRange = new(enclosingElement, provider, start, end);
        Assert.True(((ITextRangeProvider.Interface)textRange).ExpandToEnclosingUnit(TextUnit.TextUnit_Word).Succeeded);
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
        IRawElementProviderSimple.Interface enclosingElement = new Mock<IRawElementProviderSimple.Interface>(MockBehavior.Strict).Object;
        Mock<UiaTextProvider> providerMock = new(MockBehavior.Strict);
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
        UiaTextRange textRange = new(enclosingElement, provider, start, end);
        Assert.True(((ITextRangeProvider.Interface)textRange).ExpandToEnclosingUnit(TextUnit.TextUnit_Line).Succeeded);
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
        IRawElementProviderSimple.Interface enclosingElement = new Mock<IRawElementProviderSimple.Interface>(MockBehavior.Strict).Object;
        Mock<UiaTextProvider> providerMock = new(MockBehavior.Strict);
        providerMock.Setup(m => m.Text).Returns(testText);
        providerMock.Setup(m => m.TextLength).Returns(testText.Length);
        UiaTextProvider provider = providerMock.Object;
        UiaTextRange textRange = new(enclosingElement, provider, start, end);
        Assert.True(((ITextRangeProvider.Interface)textRange).ExpandToEnclosingUnit(TextUnit.TextUnit_Paragraph).Succeeded);
        Assert.Equal(expandedStart, textRange.Start);
        Assert.Equal(expandedEnd, textRange.End);
    }

    public static IEnumerable<object[]> UiaTextRange_ITextRangeProvider_ExpandToEnclosingUnit_ExpandsToAllText_TestData()
    {
        yield return new object[] { 5, 8, TextUnit.TextUnit_Page, 0, 72 };
        yield return new object[] { 10, 10, TextUnit.TextUnit_Format, 0, 72 };
        yield return new object[] { 10, 10, TextUnit.TextUnit_Document, 0, 72 };
    }

    [StaTheory]
    [MemberData(nameof(UiaTextRange_ITextRangeProvider_ExpandToEnclosingUnit_ExpandsToAllText_TestData))]
    internal void UiaTextRange_ITextRangeProvider_ExpandToEnclosingUnit_ExpandsToAllText(int start, int end, TextUnit textUnit, int expandedStart, int expandedEnd)
    {
        string testText =
@"This is the first line
this is the second line
this is the third line.";
        IRawElementProviderSimple.Interface enclosingElement = new Mock<IRawElementProviderSimple.Interface>(MockBehavior.Strict).Object;
        Mock<UiaTextProvider> providerMock = new(MockBehavior.Strict);
        providerMock.Setup(m => m.TextLength).Returns(testText.Length);
        UiaTextProvider provider = providerMock.Object;
        UiaTextRange textRange = new(enclosingElement, provider, start, end);
        Assert.True(((ITextRangeProvider.Interface)textRange).ExpandToEnclosingUnit(textUnit).Succeeded);
        Assert.Equal(expandedStart, textRange.Start);
        Assert.Equal(expandedEnd, textRange.End);
    }

    [StaTheory]
    [InlineData(true)]
    [InlineData(false)]
    internal void UiaTextRange_ITextRangeProvider_FindAttribute_Returns_null(bool backward)
    {
        IRawElementProviderSimple.Interface enclosingElement = new Mock<IRawElementProviderSimple.Interface>(MockBehavior.Strict).Object;
        UiaTextProvider provider = new Mock<UiaTextProvider>(MockBehavior.Strict).Object;
        UiaTextRange textRange = new(enclosingElement, provider, start: 0, end: 0);
        Array textAttributeIdentifiers = Enum.GetValues(typeof(UIA_TEXTATTRIBUTE_ID));

        foreach (UIA_TEXTATTRIBUTE_ID textAttributeIdentifier in textAttributeIdentifiers)
        {
            using ComScope<ITextRangeProvider> actual = new(null);
            Assert.True(((ITextRangeProvider.Interface)textRange).FindAttribute(textAttributeIdentifier, VARIANT.Empty, backward, actual).Succeeded);
            Assert.True(actual.IsNull);
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
        IRawElementProviderSimple.Interface enclosingElement = new Mock<IRawElementProviderSimple.Interface>(MockBehavior.Strict).Object;
        Mock<UiaTextProvider> providerMock = new(MockBehavior.Strict);
        providerMock.Setup(m => m.Text).Returns(testText);
        providerMock.Setup(m => m.TextLength).Returns(testText.Length);
        UiaTextProvider provider = providerMock.Object;
        UiaTextRange textRange = new(enclosingElement, provider, start: 0, end: 28);

        using BSTR bstrText = new(textToSearch);
        using ComScope<ITextRangeProvider> actual = new(null);
        Assert.True(((ITextRangeProvider.Interface)textRange).FindText(bstrText, backward, ignoreCase, actual).Succeeded);

        if (foundText is not null)
        {
            Assert.Equal(foundText, actual.Value->GetText(5000).ToStringAndFree());
        }
        else
        {
            Assert.True(actual.IsNull);
        }
    }

    [StaFact]
    internal void UiaTextRange_ITextRangeProvider_FindText_ReturnsNull_IfTextNull()
    {
        using (new NoAssertContext())
        {
            IRawElementProviderSimple.Interface enclosingElement = new Mock<IRawElementProviderSimple.Interface>(MockBehavior.Strict).Object;
            UiaTextProvider provider = new Mock<UiaTextProvider>(MockBehavior.Strict).Object;
            UiaTextRange textRange = new(enclosingElement, provider, start: 0, end: 28);
            using ComScope<ITextRangeProvider> actual = new(null);
            Assert.True(((ITextRangeProvider.Interface)textRange).FindText(default, BOOL.TRUE, BOOL.TRUE, actual).Failed);
            Assert.True(actual.IsNull);
        }
    }

    private static object? s_notSupportedValue;

    internal static object UiaGetReservedNotSupportedValue()
    {
        if (s_notSupportedValue is null)
        {
            IUnknown* unknown;
            PInvoke.UiaGetReservedNotSupportedValue(&unknown).ThrowOnFailure();
            s_notSupportedValue = new VARIANT()
            {
                vt = VARENUM.VT_UNKNOWN,
                data = new() { punkVal = unknown }
            }.ToObject()!;
        }

        return s_notSupportedValue;
    }

    public static IEnumerable<object[]> UiaTextRange_ITextRangeProvider_GetAttributeValue_Returns_Correct_TestData()
    {
        yield return new object[] { UIA_TEXTATTRIBUTE_ID.UIA_BackgroundColorAttributeId, (int)(uint)(COLORREF)PInvoke.GetSysColor(SYS_COLOR_INDEX.COLOR_WINDOW) };
        yield return new object[] { UIA_TEXTATTRIBUTE_ID.UIA_CapStyleAttributeId, (int)CapStyle.None };
        yield return new object[] { UIA_TEXTATTRIBUTE_ID.UIA_FontNameAttributeId, "Segoe UI" };
        yield return new object[] { UIA_TEXTATTRIBUTE_ID.UIA_FontSizeAttributeId, 9.0 };
        yield return new object[] { UIA_TEXTATTRIBUTE_ID.UIA_FontWeightAttributeId, (int)FW.NORMAL };
        yield return new object[] { UIA_TEXTATTRIBUTE_ID.UIA_ForegroundColorAttributeId, (int)(uint)default(COLORREF) };
        yield return new object[] { UIA_TEXTATTRIBUTE_ID.UIA_HorizontalTextAlignmentAttributeId, (int)HorizontalTextAlignment.Left };
        yield return new object[] { UIA_TEXTATTRIBUTE_ID.UIA_IsItalicAttributeId, false };
        yield return new object[] { UIA_TEXTATTRIBUTE_ID.UIA_IsReadOnlyAttributeId, false };
        yield return new object[] { UIA_TEXTATTRIBUTE_ID.UIA_StrikethroughStyleAttributeId, (int)TextDecorationLineStyle.None };
        yield return new object[] { UIA_TEXTATTRIBUTE_ID.UIA_UnderlineStyleAttributeId, (int)TextDecorationLineStyle.None };

        yield return new object[] { UIA_TEXTATTRIBUTE_ID.UIA_AnimationStyleAttributeId, UiaGetReservedNotSupportedValue() };
        yield return new object[] { UIA_TEXTATTRIBUTE_ID.UIA_BulletStyleAttributeId, UiaGetReservedNotSupportedValue() };
        yield return new object[] { UIA_TEXTATTRIBUTE_ID.UIA_CultureAttributeId, UiaGetReservedNotSupportedValue() };
        yield return new object[] { UIA_TEXTATTRIBUTE_ID.UIA_IndentationFirstLineAttributeId, UiaGetReservedNotSupportedValue() };
        yield return new object[] { UIA_TEXTATTRIBUTE_ID.UIA_IndentationLeadingAttributeId, UiaGetReservedNotSupportedValue() };
        yield return new object[] { UIA_TEXTATTRIBUTE_ID.UIA_IndentationTrailingAttributeId, UiaGetReservedNotSupportedValue() };
        yield return new object[] { UIA_TEXTATTRIBUTE_ID.UIA_IsHiddenAttributeId, UiaGetReservedNotSupportedValue() };
        yield return new object[] { UIA_TEXTATTRIBUTE_ID.UIA_IsSubscriptAttributeId, UiaGetReservedNotSupportedValue() };
        yield return new object[] { UIA_TEXTATTRIBUTE_ID.UIA_IsSuperscriptAttributeId, UiaGetReservedNotSupportedValue() };
        yield return new object[] { UIA_TEXTATTRIBUTE_ID.UIA_MarginBottomAttributeId, UiaGetReservedNotSupportedValue() };
        yield return new object[] { UIA_TEXTATTRIBUTE_ID.UIA_MarginLeadingAttributeId, UiaGetReservedNotSupportedValue() };
        yield return new object[] { UIA_TEXTATTRIBUTE_ID.UIA_MarginTopAttributeId, UiaGetReservedNotSupportedValue() };
        yield return new object[] { UIA_TEXTATTRIBUTE_ID.UIA_MarginTrailingAttributeId, UiaGetReservedNotSupportedValue() };
        yield return new object[] { UIA_TEXTATTRIBUTE_ID.UIA_OutlineStylesAttributeId, UiaGetReservedNotSupportedValue() };
        yield return new object[] { UIA_TEXTATTRIBUTE_ID.UIA_OverlineColorAttributeId, UiaGetReservedNotSupportedValue() };
        yield return new object[] { UIA_TEXTATTRIBUTE_ID.UIA_OverlineStyleAttributeId, UiaGetReservedNotSupportedValue() };
        yield return new object[] { UIA_TEXTATTRIBUTE_ID.UIA_StrikethroughColorAttributeId, UiaGetReservedNotSupportedValue() };
        yield return new object[] { UIA_TEXTATTRIBUTE_ID.UIA_TabsAttributeId, UiaGetReservedNotSupportedValue() };
        yield return new object[] { UIA_TEXTATTRIBUTE_ID.UIA_TextFlowDirectionsAttributeId, UiaGetReservedNotSupportedValue() };
        yield return new object[] { UIA_TEXTATTRIBUTE_ID.UIA_UnderlineColorAttributeId, UiaGetReservedNotSupportedValue() };
        yield return new object[] { UIA_TEXTATTRIBUTE_ID.UIA_AnnotationTypesAttributeId, UiaGetReservedNotSupportedValue() };
        yield return new object[] { UIA_TEXTATTRIBUTE_ID.UIA_AnnotationObjectsAttributeId, UiaGetReservedNotSupportedValue() };
        yield return new object[] { UIA_TEXTATTRIBUTE_ID.UIA_StyleNameAttributeId, UiaGetReservedNotSupportedValue() };
        yield return new object[] { UIA_TEXTATTRIBUTE_ID.UIA_StyleIdAttributeId, UiaGetReservedNotSupportedValue() };
        yield return new object[] { UIA_TEXTATTRIBUTE_ID.UIA_LinkAttributeId, UiaGetReservedNotSupportedValue() };
        yield return new object[] { UIA_TEXTATTRIBUTE_ID.UIA_IsActiveAttributeId, UiaGetReservedNotSupportedValue() };
        yield return new object[] { UIA_TEXTATTRIBUTE_ID.UIA_SelectionActiveEndAttributeId, UiaGetReservedNotSupportedValue() };
        yield return new object[] { UIA_TEXTATTRIBUTE_ID.UIA_CaretPositionAttributeId, UiaGetReservedNotSupportedValue() };
        yield return new object[] { UIA_TEXTATTRIBUTE_ID.UIA_CaretBidiModeAttributeId, UiaGetReservedNotSupportedValue() };
        yield return new object[] { UIA_TEXTATTRIBUTE_ID.UIA_LineSpacingAttributeId, UiaGetReservedNotSupportedValue() };
        yield return new object[] { UIA_TEXTATTRIBUTE_ID.UIA_BeforeParagraphSpacingAttributeId, UiaGetReservedNotSupportedValue() };
        yield return new object[] { UIA_TEXTATTRIBUTE_ID.UIA_AfterParagraphSpacingAttributeId, UiaGetReservedNotSupportedValue() };
    }

    [StaTheory]
    [MemberData(nameof(UiaTextRange_ITextRangeProvider_GetAttributeValue_Returns_Correct_TestData))]
    internal void UiaTextRange_ITextRangeProvider_GetAttributeValue_Returns_Correct(UIA_TEXTATTRIBUTE_ID attributeId, object attributeValue)
    {
        IRawElementProviderSimple.Interface enclosingElement = new Mock<IRawElementProviderSimple.Interface>(MockBehavior.Strict).Object;
        Mock<UiaTextProvider> providerMock = new(MockBehavior.Strict);
        using Font font = new("Segoe UI", 9, FontStyle.Regular);
        providerMock.Setup(m => m.Logfont).Returns(font.ToLogicalFont());
        providerMock.Setup(m => m.WindowStyle).Returns(PInvoke.ES_LEFT);
        providerMock.Setup(m => m.IsReadOnly).Returns(false);
        UiaTextProvider provider = providerMock.Object;
        UiaTextRange textRange = new(enclosingElement, provider, start: 0, end: 28);
        using VARIANT actual = default;
        Assert.True(((ITextRangeProvider.Interface)textRange).GetAttributeValue(attributeId, &actual).Succeeded);
        Assert.Equal(attributeValue, actual.ToObject());
    }

    [StaFact]
    public void UiaTextRange_ITextRangeProvider_GetBoundingRectangles_ReturnsEmpty_for_EmptyText()
    {
        Rectangle expected = new(10, 33, 96, 19);
        SafeArrayScope<double> array = new(4);
        array[0] = 10;
        array[1] = 33;
        array[2] = 96;
        array[3] = 19;
        VARIANT variant = new()
        {
            vt = VARENUM.VT_ARRAY | VARENUM.VT_R8,
            data = new() { parray = array.Value }
        };

        MockRawElementProviderSimple enclosingElement = new(variant);
        Mock<UiaTextProvider> providerMock = new(MockBehavior.Strict);
        providerMock.Setup(p => p.Text).Returns("");
        providerMock.Setup(p => p.TextLength).Returns(0);
        UiaTextProvider provider = providerMock.Object;
        UiaTextRange textRange = new(enclosingElement, provider, start: 0, end: 0);

        using SafeArrayScope<double> actual = new((SAFEARRAY*)null);
        Assert.True(((ITextRangeProvider.Interface)textRange).GetBoundingRectangles(actual).Succeeded);
        using SafeArrayScope<double> expectedRectArray = UiaTextProvider.BoundingRectangleAsArray(expected);
        for (int i = 0; i < actual.Length; i++)
        {
            Assert.Equal(expectedRectArray[i], actual[i]);
        }
    }

    [StaFact]
    public void UiaTextRange_ITextRangeProvider_GetBoundingRectangles_ReturnsEmpty_for_DegenerateRange()
    {
        SafeArrayScope<double> array = new(4);
        array[0] = 10;
        array[1] = 33;
        array[2] = 96;
        array[3] = 19;
        VARIANT variant = new()
        {
            vt = VARENUM.VT_ARRAY | VARENUM.VT_R8,
            data = new() { parray = array }
        };

        MockRawElementProviderSimple enclosingElement = new(variant);
        Mock<UiaTextProvider> providerMock = new(MockBehavior.Strict);
        providerMock.Setup(p => p.Text).Returns("abcde");
        providerMock.Setup(p => p.TextLength).Returns(5);
        providerMock.Setup(p => p.IsMultiline).Returns(false);
        UiaTextProvider provider = providerMock.Object;
        UiaTextRange textRange = new(enclosingElement, provider, start: 0, end: 0);
        using SafeArrayScope<double> actual = new((SAFEARRAY*)null);
        Assert.True(((ITextRangeProvider.Interface)textRange).GetBoundingRectangles(actual).Succeeded);
        Assert.True(actual.IsEmpty);
    }

    [StaFact]
    public void UiaTextRange_ITextRangeProvider_GetBoundingRectangles_ReturnsExpected_for_Endline()
    {
        SafeArrayScope<double> array = new(4);
        array[0] = 10;
        array[1] = 33;
        array[2] = 96;
        array[3] = 19;
        VARIANT variant = new()
        {
            vt = VARENUM.VT_ARRAY | VARENUM.VT_R8,
            data = new() { parray = array }
        };

        MockRawElementProviderSimple enclosingElement = new(variant);
        Mock<UiaTextProvider> providerMock = new(MockBehavior.Strict);
        providerMock.Setup(p => p.Text).Returns("abc");
        providerMock.Setup(p => p.TextLength).Returns(3);
        providerMock.Setup(p => p.PointToScreen(It.IsAny<Point>())).Returns(Point.Empty);
        using Font font = new("Arial", 9f, FontStyle.Regular);
        providerMock.Setup(m => m.Logfont).Returns(font.ToLogicalFont());
        UiaTextProvider provider = providerMock.Object;

        UiaTextRange textRange = new(enclosingElement, provider, start: 3, end: 3);
        using SafeArrayScope<double> safeArrayScope = new((SAFEARRAY*)null);
        Assert.True(((ITextRangeProvider.Interface)textRange).GetBoundingRectangles(safeArrayScope).Succeeded); // {X,Y,Width,Height}

        Assert.Equal(UiaTextProvider.EndOfLineWidth, safeArrayScope[2]);
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

        SafeArrayScope<double> array = new(4);
        array[0] = 10;
        array[1] = 33;
        array[2] = 96;
        array[3] = 19;
        VARIANT variant = new()
        {
            vt = VARENUM.VT_ARRAY | VARENUM.VT_R8,
            data = new() { parray = array }
        };
        MockRawElementProviderSimple enclosingElement = new(variant);

        Mock<UiaTextProvider> providerMock = new(MockBehavior.Strict);
        providerMock.Setup(m => m.Text).Returns(testText);
        providerMock.Setup(m => m.TextLength).Returns(testText.Length);
        providerMock.Setup(m => m.IsMultiline).Returns(false);
        providerMock.Setup(m => m.BoundingRectangle).Returns(new Rectangle(1, 1, 94, 15));
        providerMock.Setup(m => m.GetPositionFromChar(3)).Returns(new Point(17, 0));
        providerMock.Setup(m => m.GetPositionFromChar(0)).Returns(new Point(1, 0));
        providerMock.Setup(m => m.GetPositionFromCharForUpperRightCorner(5, testText)).Returns(new Point(28, 0));
        providerMock.Setup(m => m.GetPositionFromCharForUpperRightCorner(1, testText)).Returns(new Point(17, 0));
        UiaTextProvider provider = providerMock.Object;
        UiaTextRange textRange = new(enclosingElement, provider, start, end);
        using SafeArrayScope<double> actual = new((SAFEARRAY*)null);
        Assert.True(((ITextRangeProvider.Interface)textRange).GetBoundingRectangles(actual).Succeeded);

        // Acceptable deviation of 1 px.
        for (int i = 0; i < actual.Length; i++)
        {
            Assert.True(actual[i] >= 0 && actual[i] >= expected[i] - 1 && actual[i] <= expected[i] + 1);
        }
    }

    public static IEnumerable<object[]> UiaTextRange_ITextRangeProvider_GetBoundingRectangles_ReturnsCorrectValue_for_MultiLine_TestData()
    {
        yield return new object[] { 0, 63, new double[] { 32, 131, 103, 12, 32, 148, 85, 12, 32, 165, 83, 12, 32, 182, 90, 12, 32, 199, 40, 12 } }; // Whole text
        yield return new object[] { 23, 53, new double[] { 72, 148, 45, 12, 32, 165, 83, 12, 32, 182, 56, 12 } }; // "xt with several lines and num" text is selected
    }

    /// <remark>All returned values of mock methods and properties were taken from a real TextBox.</remark>
    [StaTheory]
    [MemberData(nameof(UiaTextRange_ITextRangeProvider_GetBoundingRectangles_ReturnsCorrectValue_for_MultiLine_TestData))]
    public void UiaTextRange_ITextRangeProvider_GetBoundingRectangles_ReturnsCorrectValue_for_MultiLine(int start, int end, double[] expected)
    {
        string testText =
@"Some long long
test text with several lines
and numbers 12345";

        SafeArrayScope<double> array = new(4);
        array[0] = 27;
        array[1] = 128;
        array[2] = 128;
        array[3] = 155;
        VARIANT variant = new()
        {
            vt = VARENUM.VT_ARRAY | VARENUM.VT_R8,
            data = new() { parray = array }
        };

        MockRawElementProviderSimple enclosingElement = new(variant);

        Mock<UiaTextProvider> providerMock = new(MockBehavior.Strict);
        providerMock.Setup(m => m.IsReadingRTL).Returns(false);
        providerMock.Setup(m => m.TextLength).Returns(testText.Length);
        providerMock.Setup(m => m.Text).Returns(testText);
        providerMock.Setup(m => m.BoundingRectangle).Returns(new Rectangle(5, 1, 118, 153));
        providerMock.Setup(m => m.IsMultiline).Returns(true);
        providerMock.Setup(m => m.FirstVisibleLine).Returns(0);
        providerMock.Setup(m => m.LinesPerPage).Returns(9);
        using Font font = new("Arial", 9f, FontStyle.Regular);
        providerMock.Setup(m => m.Logfont).Returns(font.ToLogicalFont());

        // Offset by enclosing element's coordinates
        providerMock.Setup(m => m.RectangleToScreen(It.IsAny<Rectangle>()))
            .Returns((Rectangle rect) => new Rectangle(rect.X + 27, rect.Y + 128, rect.Width, rect.Height));

        providerMock.Setup(m => m.GetLineFromCharIndex(0)).Returns(0);
        providerMock.Setup(m => m.GetLineFromCharIndex(14)).Returns(0);
        providerMock.Setup(m => m.GetLineFromCharIndex(23)).Returns(1);
        providerMock.Setup(m => m.GetLineFromCharIndex(31)).Returns(2);
        providerMock.Setup(m => m.GetLineFromCharIndex(44)).Returns(2);
        providerMock.Setup(m => m.GetLineFromCharIndex(52)).Returns(3);
        providerMock.Setup(m => m.GetLineFromCharIndex(58)).Returns(4);
        providerMock.Setup(m => m.GetLineFromCharIndex(62)).Returns(4);

        providerMock.Setup(m => m.GetPositionFromChar(0)).Returns(new Point(5, 1));
        providerMock.Setup(m => m.GetPositionFromChar(16)).Returns(new Point(5, 18));
        providerMock.Setup(m => m.GetPositionFromChar(23)).Returns(new Point(45, 18));
        providerMock.Setup(m => m.GetPositionFromChar(31)).Returns(new Point(5, 35));
        providerMock.Setup(m => m.GetPositionFromChar(46)).Returns(new Point(5, 52));
        providerMock.Setup(m => m.GetPositionFromChar(58)).Returns(new Point(5, 69));

        providerMock.Setup(m => m.GetPositionFromCharForUpperRightCorner(15, testText)).Returns(new Point(108, 1));
        providerMock.Setup(m => m.GetPositionFromCharForUpperRightCorner(30, testText)).Returns(new Point(90, 18));
        providerMock.Setup(m => m.GetPositionFromCharForUpperRightCorner(45, testText)).Returns(new Point(88, 35));
        providerMock.Setup(m => m.GetPositionFromCharForUpperRightCorner(52, testText)).Returns(new Point(61, 52));
        providerMock.Setup(m => m.GetPositionFromCharForUpperRightCorner(57, testText)).Returns(new Point(95, 52));
        providerMock.Setup(m => m.GetPositionFromCharForUpperRightCorner(62, testText)).Returns(new Point(45, 69));

        providerMock.Setup(m => m.GetLineIndex(1)).Returns(16);
        providerMock.Setup(m => m.GetLineIndex(2)).Returns(31);
        providerMock.Setup(m => m.GetLineIndex(3)).Returns(46);
        providerMock.Setup(m => m.GetLineIndex(4)).Returns(58);

        UiaTextRange textRange = new(enclosingElement, providerMock.Object, start, end);
        using SafeArrayScope<double> actual = new((SAFEARRAY*)null);
        Assert.True(((ITextRangeProvider.Interface)textRange).GetBoundingRectangles(actual).Succeeded);
        Assert.True(expected.Length == actual.Length);
        for (int i = 0; i < expected.Length; i++)
        {
            Assert.Equal(expected[i], actual[i]);
        }
    }

    public static IEnumerable<object[]> UiaTextRange_ITextRangeProvider_GetBoundingRectangles_ReturnsCorrectValue_for_MultiLine_And_RTL_TestData()
    {
        yield return new object[] { 0, 63, new double[] { 47, 131, 103, 12, 67, 148, 83, 12, 67, 165, 83, 12, 62, 182, 88, 12, 110, 199, 40, 12 } }; // Whole text
        yield return new object[] { 23, 53, new double[] { 107, 148, 43, 12, 67, 165, 83, 12, 64, 182, 56, 12 } }; // "xt with several lines and num" text is selected
    }

    /// <remark>All returned values of mock methods and properties were taken from a real TextBox.</remark>
    [StaTheory]
    [MemberData(nameof(UiaTextRange_ITextRangeProvider_GetBoundingRectangles_ReturnsCorrectValue_for_MultiLine_And_RTL_TestData))]
    public void UiaTextRange_ITextRangeProvider_GetBoundingRectangles_ReturnsCorrectValue_for_MultiLine_And_RTL(int start, int end, double[] expected)
    {
        string testText =
@"Some long long
test text with several lines
and numbers 12345";

        SafeArrayScope<double> array = new(4);
        array[0] = 27;
        array[1] = 128;
        array[2] = 128;
        array[3] = 155;
        VARIANT variant = new()
        {
            vt = VARENUM.VT_ARRAY | VARENUM.VT_R8,
            data = new() { parray = array }
        };

        MockRawElementProviderSimple enclosingElement = new(variant);

        Mock<UiaTextProvider> providerMock = new(MockBehavior.Strict);
        providerMock.Setup(m => m.IsReadingRTL).Returns(true);
        providerMock.Setup(m => m.TextLength).Returns(testText.Length);
        providerMock.Setup(m => m.Text).Returns(testText);
        providerMock.Setup(m => m.BoundingRectangle).Returns(new Rectangle(5, 1, 118, 153));
        providerMock.Setup(m => m.IsMultiline).Returns(true);
        providerMock.Setup(m => m.FirstVisibleLine).Returns(0);
        providerMock.Setup(m => m.LinesPerPage).Returns(9);
        using Font font = new("Arial", 9f, FontStyle.Regular);
        providerMock.Setup(m => m.Logfont).Returns(font.ToLogicalFont());

        // Offset by enclosing element's coordinates
        providerMock.Setup(m => m.RectangleToScreen(It.IsAny<Rectangle>()))
            .Returns((Rectangle rect) => new Rectangle(rect.X + 27, rect.Y + 128, rect.Width, rect.Height));

        providerMock.Setup(m => m.GetLineFromCharIndex(0)).Returns(0);
        providerMock.Setup(m => m.GetLineFromCharIndex(14)).Returns(0);
        providerMock.Setup(m => m.GetLineFromCharIndex(23)).Returns(1);
        providerMock.Setup(m => m.GetLineFromCharIndex(31)).Returns(2);
        providerMock.Setup(m => m.GetLineFromCharIndex(44)).Returns(2);
        providerMock.Setup(m => m.GetLineFromCharIndex(52)).Returns(3);
        providerMock.Setup(m => m.GetLineFromCharIndex(58)).Returns(4);
        providerMock.Setup(m => m.GetLineFromCharIndex(62)).Returns(4);

        providerMock.Setup(m => m.GetPositionFromChar(0)).Returns(new Point(22, 1));
        providerMock.Setup(m => m.GetPositionFromChar(16)).Returns(new Point(42, 18));
        providerMock.Setup(m => m.GetPositionFromChar(23)).Returns(new Point(82, 18));
        providerMock.Setup(m => m.GetPositionFromChar(31)).Returns(new Point(42, 35));
        providerMock.Setup(m => m.GetPositionFromChar(46)).Returns(new Point(37, 52));
        providerMock.Setup(m => m.GetPositionFromChar(58)).Returns(new Point(83, 69));

        providerMock.Setup(m => m.GetPositionFromCharForUpperRightCorner(15, testText)).Returns(new Point(125, 1));
        providerMock.Setup(m => m.GetPositionFromCharForUpperRightCorner(30, testText)).Returns(new Point(127, 18));
        providerMock.Setup(m => m.GetPositionFromCharForUpperRightCorner(45, testText)).Returns(new Point(125, 35));
        providerMock.Setup(m => m.GetPositionFromCharForUpperRightCorner(52, testText)).Returns(new Point(93, 52));
        providerMock.Setup(m => m.GetPositionFromCharForUpperRightCorner(57, testText)).Returns(new Point(127, 52));
        providerMock.Setup(m => m.GetPositionFromCharForUpperRightCorner(62, testText)).Returns(new Point(123, 69));

        providerMock.Setup(m => m.GetLineIndex(1)).Returns(16);
        providerMock.Setup(m => m.GetLineIndex(2)).Returns(31);
        providerMock.Setup(m => m.GetLineIndex(3)).Returns(46);
        providerMock.Setup(m => m.GetLineIndex(4)).Returns(58);

        UiaTextRange textRange = new(enclosingElement, providerMock.Object, start, end);
        using SafeArrayScope<double> actual = new((SAFEARRAY*)null);
        Assert.True(((ITextRangeProvider.Interface)textRange).GetBoundingRectangles(actual).Succeeded);
        Assert.True(expected.Length == actual.Length);
        for (int i = 0; i < expected.Length; i++)
        {
            Assert.Equal(expected[i], actual[i]);
        }
    }

    [StaFact]
    public void UiaTextRange_ITextRangeProvider_GetEnclosingElement_ReturnsCorrectValue()
    {
        IRawElementProviderSimple.Interface enclosingElement = new Mock<IRawElementProviderSimple.Interface>(MockBehavior.Strict).Object;
        UiaTextProvider provider = new Mock<UiaTextProvider>(MockBehavior.Strict).Object;
        UiaTextRange textRange = new(enclosingElement, provider, start: 0, end: 0);
        using ComScope<IRawElementProviderSimple> actual = new(null);
        Assert.True(((ITextRangeProvider.Interface)textRange).GetEnclosingElement(actual).Succeeded);
        Assert.Equal(enclosingElement, ComHelpers.GetObjectForIUnknown(actual));
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
        IRawElementProviderSimple.Interface enclosingElement = new Mock<IRawElementProviderSimple.Interface>(MockBehavior.Strict).Object;
        Mock<UiaTextProvider> providerMock = new(MockBehavior.Strict);
        providerMock.Setup(m => m.Text).Returns(testText);
        providerMock.Setup(m => m.TextLength).Returns(testText.Length);
        UiaTextProvider provider = providerMock.Object;
        UiaTextRange textRange = new(enclosingElement, provider, start, end);
        using BSTR actual = default;
        Assert.True(((ITextRangeProvider.Interface)textRange).GetText(maxLength, &actual).Succeeded);
        Assert.Equal(expected, actual.ToString());
    }

    public static IEnumerable<object[]> UiaTextRange_ITextRangeProvider_Move_MovesCorrectly_TestData()
    {
        yield return new object[] { 0, 5, TextUnit.TextUnit_Character, 1, 6, 6 };
        yield return new object[] { 1, 6, TextUnit.TextUnit_Character, 5, 11, 11 };
        yield return new object[] { 0, 5, TextUnit.TextUnit_Character, -2, 0, 0 };
        yield return new object[] { 3, 6, TextUnit.TextUnit_Character, -2, 1, 1 };
        yield return new object[] { 1, 2, TextUnit.TextUnit_Word, 1, 4, 4 };
        yield return new object[] { 1, 2, TextUnit.TextUnit_Word, 5, 11, 11 };
        yield return new object[] { 12, 14, TextUnit.TextUnit_Word, -2, 8, 8 };
        yield return new object[] { 12, 14, TextUnit.TextUnit_Word, -10, 0, 0 };
    }

    [StaTheory]
    [MemberData(nameof(UiaTextRange_ITextRangeProvider_Move_MovesCorrectly_TestData))]
    internal void UiaTextRange_ITextRangeProvider_Move_MovesCorrectly(int start, int end, TextUnit unit, int count, int expectedStart, int expectedEnd)
    {
        string testText =
@"This is the text to move on - line 1
This is the line 2
This is the line 3";
        IRawElementProviderSimple.Interface enclosingElement = new Mock<IRawElementProviderSimple.Interface>(MockBehavior.Strict).Object;
        Mock<UiaTextProvider> providerMock = new(MockBehavior.Strict);
        providerMock.Setup(m => m.Text).Returns(testText);
        providerMock.Setup(m => m.TextLength).Returns(testText.Length);
        UiaTextProvider provider = providerMock.Object;
        UiaTextRange textRange = new(enclosingElement, provider, start, end);
        int result;
        Assert.True(((ITextRangeProvider.Interface)textRange).Move(unit, count, &result).Succeeded);
        Assert.Equal(expectedStart, textRange.Start);
        Assert.Equal(expectedEnd, textRange.End);
    }

    public static IEnumerable<object[]> UiaTextRange_ITextRangeProvider_MoveEndpointByUnit_MovesCorrectly_TestData()
    {
        yield return new object[] { 0, 5, TextPatternRangeEndpoint.TextPatternRangeEndpoint_Start, TextUnit.TextUnit_Character, 1, 1, 5 };
        yield return new object[] { 1, 6, TextPatternRangeEndpoint.TextPatternRangeEndpoint_Start, TextUnit.TextUnit_Character, 5, 6, 6 };
        yield return new object[] { 0, 5, TextPatternRangeEndpoint.TextPatternRangeEndpoint_Start, TextUnit.TextUnit_Character, -2, 0, 5 };
        yield return new object[] { 3, 6, TextPatternRangeEndpoint.TextPatternRangeEndpoint_Start, TextUnit.TextUnit_Character, -2, 1, 6 };
        yield return new object[] { 3, 6, TextPatternRangeEndpoint.TextPatternRangeEndpoint_End, TextUnit.TextUnit_Character, 1, 3, 7 };
        yield return new object[] { 3, 6, TextPatternRangeEndpoint.TextPatternRangeEndpoint_End, TextUnit.TextUnit_Character, -1, 3, 5 };
        yield return new object[] { 1, 2, TextPatternRangeEndpoint.TextPatternRangeEndpoint_Start, TextUnit.TextUnit_Word, 1, 4, 4 };
        yield return new object[] { 1, 2, TextPatternRangeEndpoint.TextPatternRangeEndpoint_Start, TextUnit.TextUnit_Word, 5, 11, 11 };
        yield return new object[] { 12, 14, TextPatternRangeEndpoint.TextPatternRangeEndpoint_Start, TextUnit.TextUnit_Word, -1, 11, 14 };
        yield return new object[] { 12, 14, TextPatternRangeEndpoint.TextPatternRangeEndpoint_Start, TextUnit.TextUnit_Word, -2, 8, 14 };
    }

    [StaTheory]
    [MemberData(nameof(UiaTextRange_ITextRangeProvider_MoveEndpointByUnit_MovesCorrectly_TestData))]
    internal void UiaTextRange_ITextRangeProvider_MoveEndpointByUnit_MovesCorrectly(int start, int end, TextPatternRangeEndpoint endpoint, TextUnit unit, int count, int expectedStart, int expectedEnd)
    {
        string testText =
@"This is the text to move on - line 1
This is the line 2
This is the line 3";
        IRawElementProviderSimple.Interface enclosingElement = new Mock<IRawElementProviderSimple.Interface>(MockBehavior.Strict).Object;
        Mock<UiaTextProvider> providerMock = new(MockBehavior.Strict);
        providerMock.Setup(m => m.Text).Returns(testText);
        providerMock.Setup(m => m.TextLength).Returns(testText.Length);
        UiaTextProvider provider = providerMock.Object;
        UiaTextRange textRange = new(enclosingElement, provider, start, end);
        int result;
        ((ITextRangeProvider.Interface)textRange).MoveEndpointByUnit(endpoint, unit, count, &result);
        Assert.Equal(expectedStart, textRange.Start);
        Assert.Equal(expectedEnd, textRange.End);
    }

    public static IEnumerable<object[]> UiaTextRange_ITextRangeProvider_MoveEndpointByRange_MovesCorrectly_TestData()
    {
        yield return new object[] { 0, 5, TextPatternRangeEndpoint.TextPatternRangeEndpoint_Start, 7, 10, TextPatternRangeEndpoint.TextPatternRangeEndpoint_Start, 7, 7 };
        yield return new object[] { 0, 5, TextPatternRangeEndpoint.TextPatternRangeEndpoint_Start, 7, 10, TextPatternRangeEndpoint.TextPatternRangeEndpoint_End, 10, 10 };
        yield return new object[] { 0, 5, TextPatternRangeEndpoint.TextPatternRangeEndpoint_End, 7, 10, TextPatternRangeEndpoint.TextPatternRangeEndpoint_Start, 0, 7 };
        yield return new object[] { 0, 5, TextPatternRangeEndpoint.TextPatternRangeEndpoint_End, 7, 10, TextPatternRangeEndpoint.TextPatternRangeEndpoint_End, 0, 10 };
    }

    [StaTheory]
    [MemberData(nameof(UiaTextRange_ITextRangeProvider_MoveEndpointByRange_MovesCorrectly_TestData))]
    internal void UiaTextRange_ITextRangeProvider_MoveEndpointByRange_MovesCorrectly(int start, int end, TextPatternRangeEndpoint endpoint, int targetRangeStart, int targetRangeEnd, TextPatternRangeEndpoint targetEndpoint, int expectedStart, int expectedEnd)
    {
        IRawElementProviderSimple.Interface enclosingElement = new Mock<IRawElementProviderSimple.Interface>(MockBehavior.Strict).Object;
        UiaTextProvider provider = new Mock<UiaTextProvider>(MockBehavior.Strict).Object;
        UiaTextRange textRange = new(enclosingElement, provider, start, end);
        UiaTextRange targetRange = new(enclosingElement, provider, targetRangeStart, targetRangeEnd);
        using var targetRangeScope = ComHelpers.GetComScope<ITextRangeProvider>(targetRange);
        Assert.True(((ITextRangeProvider.Interface)textRange).MoveEndpointByRange(endpoint, targetRangeScope, targetEndpoint).Succeeded);
        Assert.Equal(expectedStart, textRange.Start);
        Assert.Equal(expectedEnd, textRange.End);
    }

    [StaTheory]
    [InlineData(0, 0)]
    [InlineData(0, 10)]
    [InlineData(5, 10)]
    public void UiaTextRange_ITextRangeProvider_Select_ReturnsCorrectValue(int start, int end)
    {
        IRawElementProviderSimple.Interface enclosingElement = new Mock<IRawElementProviderSimple.Interface>(MockBehavior.Strict).Object;
        Mock<UiaTextProvider> providerMock = new(MockBehavior.Strict);
        providerMock.Setup(m => m.SetSelection(start, end));
        UiaTextProvider provider = providerMock.Object;
        UiaTextRange textRange = new(enclosingElement, provider, start, end);
        Assert.True(((ITextRangeProvider.Interface)textRange).Select().Succeeded);
        providerMock.Verify(m => m.SetSelection(start, end), Times.Once());
    }

    [StaFact]
    public void UiaTextRange_ITextRangeProvider_AddToSelection_DoesntThrowException()
    {
        // Check an app doesn't crash when calling AddToSelection method.
        IRawElementProviderSimple.Interface enclosingElement = new Mock<IRawElementProviderSimple.Interface>(MockBehavior.Strict).Object;
        UiaTextProvider provider = new Mock<UiaTextProvider>(MockBehavior.Strict).Object;
        UiaTextRange textRange = new(enclosingElement, provider, 3, 7);
        Assert.True(((ITextRangeProvider.Interface)textRange).AddToSelection().Succeeded);
    }

    [StaFact]
    public void UiaTextRange_ITextRangeProvider_RemoveFromSelection_DoesntThrowException()
    {
        // Check an app doesn't crash when calling RemoveFromSelection method.
        IRawElementProviderSimple.Interface enclosingElement = new Mock<IRawElementProviderSimple.Interface>(MockBehavior.Strict).Object;
        UiaTextProvider provider = new Mock<UiaTextProvider>(MockBehavior.Strict).Object;
        UiaTextRange textRange = new(enclosingElement, provider, 3, 7);
        Assert.True(((ITextRangeProvider.Interface)textRange).RemoveFromSelection().Succeeded);
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
        IRawElementProviderSimple.Interface enclosingElement = new Mock<IRawElementProviderSimple.Interface>(MockBehavior.Strict).Object;

        Mock<UiaTextProvider> providerMock = new(MockBehavior.Strict);
        providerMock.Setup(p => p.IsMultiline).Returns(true);
        providerMock.Setup(p => p.GetLineFromCharIndex(charIndex)).Returns(lineForCharIndex);
        providerMock.Setup(p => p.LineScroll(charactersHorizontal, linesVertical)).Returns(true);
        providerMock.Setup(p => p.FirstVisibleLine).Returns(firstVisibleLine);

        UiaTextProvider provider = providerMock.Object;
        UiaTextRange textRange = new(enclosingElement, provider, start, end);
        Assert.True(((ITextRangeProvider.Interface)textRange).ScrollIntoView(BOOL.TRUE).Succeeded);
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
        IRawElementProviderSimple.Interface enclosingElement = new Mock<IRawElementProviderSimple.Interface>(MockBehavior.Strict).Object;
        int visibleStart = 40;
        int visibleEnd = 60;

        Mock<UiaTextProvider> providerMock = new(MockBehavior.Strict);
        providerMock.Setup(p => p.IsMultiline).Returns(false);
        providerMock.Setup(p => p.IsScrollable).Returns(scrollable);
        providerMock.Setup(p => p.IsReadingRTL).Returns(readingRTL);
        providerMock.Setup(p => p.GetVisibleRangePoints(out visibleStart, out visibleEnd));

        UiaTextProvider provider = providerMock.Object;
        UiaTextRange textRange = new(enclosingElement, provider, start, end);
        Assert.True(((ITextRangeProvider.Interface)textRange).ScrollIntoView(BOOL.TRUE).Succeeded);
        providerMock.Verify(p => p.GetVisibleRangePoints(out visibleStart, out visibleEnd), Times.Exactly(2));
    }

    [StaFact]
    public void UiaTextRange_ITextRangeProvider_GetChildren_ReturnsCorrectValue()
    {
        IRawElementProviderSimple.Interface enclosingElement = new Mock<IRawElementProviderSimple.Interface>(MockBehavior.Strict).Object;
        UiaTextProvider provider = new Mock<UiaTextProvider>(MockBehavior.Strict).Object;
        UiaTextRange textRange = new(enclosingElement, provider, start: 0, end: 0);
        ComSafeArrayScope<IRawElementProviderSimple> actual = new(null);
        Assert.True(((ITextRangeProvider.Interface)textRange).GetChildren(actual).Succeeded);
        Assert.True(actual.IsEmpty);
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
    [InlineData(PInvoke.ES_CENTER, (int)HorizontalTextAlignment.Centered)]
    [InlineData(PInvoke.ES_LEFT, (int)HorizontalTextAlignment.Left)]
    [InlineData(PInvoke.ES_RIGHT, (int)HorizontalTextAlignment.Right)]
    public void UiaTextRange_private_GetHorizontalTextAlignment_ReturnsCorrectValue(int style, int expected)
    {
        IRawElementProviderSimple.Interface enclosingElement = new Mock<IRawElementProviderSimple.Interface>(MockBehavior.Strict).Object;
        UiaTextProvider provider = new Mock<UiaTextProvider>(MockBehavior.Strict).Object;
        UiaTextRange textRange = new(enclosingElement, provider, 0, 0);

        HorizontalTextAlignment actual = textRange.TestAccessor().GetHorizontalTextAlignment((WINDOW_STYLE)style);

        Assert.Equal((HorizontalTextAlignment)expected, actual);
    }

    [StaTheory]
    [InlineData((PInvoke.ES_UPPERCASE | PInvoke.ES_LEFT | PInvoke.ES_MULTILINE | PInvoke.ES_READONLY | PInvoke.ES_AUTOHSCROLL), (int)CapStyle.AllCap)]
    [InlineData((PInvoke.ES_LOWERCASE | PInvoke.ES_LEFT | PInvoke.ES_MULTILINE | PInvoke.ES_READONLY | PInvoke.ES_AUTOHSCROLL), (int)CapStyle.None)]
    public void UiaTextRange_private_GetCapStyle_ReturnsExpectedValue(int editStyle, int expected)
    {
        IRawElementProviderSimple.Interface enclosingElement = new Mock<IRawElementProviderSimple.Interface>(MockBehavior.Strict).Object;
        UiaTextProvider provider = new Mock<UiaTextProvider>(MockBehavior.Strict).Object;
        UiaTextRange textRange = new(enclosingElement, provider, 0, 0);

        CapStyle actual = textRange.TestAccessor().GetCapStyle((WINDOW_STYLE)editStyle);

        Assert.Equal((CapStyle)expected, actual);
    }

    [StaTheory]
    [InlineData(true)]
    [InlineData(false)]
    public void UiaTextRange_private_GetReadOnly_ReturnsCorrectValue(bool readOnly)
    {
        IRawElementProviderSimple.Interface enclosingElement = new Mock<IRawElementProviderSimple.Interface>(MockBehavior.Strict).Object;
        Mock<UiaTextProvider> providerMock = new(MockBehavior.Strict);
        providerMock.Setup(m => m.IsReadOnly).Returns(readOnly);
        UiaTextProvider provider = providerMock.Object;
        UiaTextRange textRange = new(enclosingElement, provider, 0, 0);

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
    public void UiaTextRange_private_GetFontName_ReturnsExpectedValue(string? faceName)
    {
        LOGFONTW logfont = new()
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
        IRawElementProviderSimple.Interface enclosingElement = new Mock<IRawElementProviderSimple.Interface>(MockBehavior.Strict).Object;
        Mock<UiaTextProvider> providerMock = new(MockBehavior.Strict);
        using Font font = new("Arial", fontSize, FontStyle.Regular);
        providerMock.Setup(m => m.Logfont).Returns(font.ToLogicalFont());
        UiaTextProvider provider = providerMock.Object;
        UiaTextRange textRange = new(enclosingElement, provider, 5, 20);

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
        LOGFONTW logfont = new() { lfWeight = (int)fontWeight };
        FW actual = StaticNullTextRange.TestAccessor().GetFontWeight(logfont);
        Assert.Equal(fontWeight, actual);
    }

    [StaFact]
    public void UiaTextRange_private_GetForegroundColor_ReturnsCorrectValue()
    {
        COLORREF actual = StaticNullTextRange.TestAccessor().GetForegroundColor();
        Assert.Equal(default, actual);
    }

    [StaTheory]
    [InlineData(0, false)]
    [InlineData(5, true)]
    public void UiaTextRange_private_GetItalic_ReturnsCorrectValue(byte ifItalic, bool expected)
    {
        LOGFONTW logfont = new() { lfItalic = ifItalic };

        bool actual = StaticNullTextRange.TestAccessor().GetItalic(logfont);

        Assert.Equal(expected, actual);
    }

    [StaTheory]
    [InlineData(0, (int)TextDecorationLineStyle.None)]
    [InlineData(5, (int)TextDecorationLineStyle.Single)]
    public void UiaTextRange_private_GetStrikethroughStyle_ReturnsCorrectValue(byte ifStrikeOut, int expected)
    {
        LOGFONTW logfont = new() { lfStrikeOut = ifStrikeOut };
        TextDecorationLineStyle actual = StaticNullTextRange.TestAccessor().GetStrikethroughStyle(logfont);

        Assert.Equal((TextDecorationLineStyle)expected, actual);
    }

    [StaTheory]
    [InlineData(0, (int)TextDecorationLineStyle.None)]
    [InlineData(5, (int)TextDecorationLineStyle.Single)]
    public void UiaTextRange_private_GetUnderlineStyle_ReturnsCorrectValue(byte ifUnderline, int expected)
    {
        LOGFONTW logfont = new() { lfUnderline = ifUnderline };
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
        IRawElementProviderSimple.Interface enclosingElement = new Mock<IRawElementProviderSimple.Interface>(MockBehavior.Strict).Object;
        UiaTextProvider provider = new Mock<UiaTextProvider>(MockBehavior.Strict).Object;
        UiaTextRange textRange = new(enclosingElement, provider, 0, 0);

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
        IRawElementProviderSimple.Interface enclosingElement = new Mock<IRawElementProviderSimple.Interface>(MockBehavior.Strict).Object;
        UiaTextProvider provider = new Mock<UiaTextProvider>(MockBehavior.Strict).Object;
        UiaTextRange textRange = new(enclosingElement, provider, 0, 0);

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
        IRawElementProviderSimple.Interface enclosingElement = new Mock<IRawElementProviderSimple.Interface>(MockBehavior.Strict).Object;
        Mock<UiaTextProvider> providerMock = new(MockBehavior.Strict);
        providerMock.Setup(m => m.TextLength).Returns(testText.Length);
        UiaTextProvider provider = providerMock.Object;
        UiaTextRange textRange = new(enclosingElement, provider, start, end);

        textRange.TestAccessor().ValidateEndpoints();

        Assert.Equal(expectedStart, textRange.Start);
        Assert.Equal(expectedEnd, textRange.End);
    }

    private unsafe class MockRawElementProviderSimple : IRawElementProviderSimple.Interface
    {
        private VARIANT _boundingRectangleVariant;

        public MockRawElementProviderSimple(VARIANT test)
        {
            _boundingRectangleVariant = test;
        }

        HRESULT IRawElementProviderSimple.Interface.get_ProviderOptions(ProviderOptions* pRetVal) => throw new NotImplementedException();

        public HRESULT GetPatternProvider(UIA_PATTERN_ID patternId, IUnknown** pRetVal) => HRESULT.E_NOTIMPL;
        public HRESULT GetPropertyValue(UIA_PROPERTY_ID propertyId, VARIANT* pRetVal)
        {
            if (propertyId == UIA_PROPERTY_ID.UIA_BoundingRectanglePropertyId)
            {
                *pRetVal = _boundingRectangleVariant;
                return HRESULT.S_OK;
            }

            return HRESULT.E_NOTIMPL;
        }

        HRESULT IRawElementProviderSimple.Interface.get_HostRawElementProvider(IRawElementProviderSimple** pRetVal) => throw new NotImplementedException();
    }
}
