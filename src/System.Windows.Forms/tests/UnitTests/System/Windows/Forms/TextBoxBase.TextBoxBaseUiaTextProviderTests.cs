// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;
using System.Windows.Forms.Automation;
using Windows.Win32.System.Com;
using Windows.Win32.UI.Accessibility;
using static System.Windows.Forms.TextBoxBase;

namespace System.Windows.Forms.Tests;

public unsafe class TextBoxBase_TextBoxBaseUiaTextProviderTests
{
    [WinFormsFact]
    public void TextBoxBaseUiaTextProvider_ctor_DoesntCreateControlHandle()
    {
        using TextBoxBase textBoxBase = new SubTextBoxBase();
        Assert.False(textBoxBase.IsHandleCreated);

        TextBoxBaseUiaTextProvider provider = new(textBoxBase);
        Assert.False(textBoxBase.IsHandleCreated);
    }

    [WinFormsFact]
    public void TextBoxBaseUiaTextProvider_Ctor_ThrowsException_IfOwnerIsNull()
    {
        Assert.Throws<ArgumentNullException>(() => new TextBoxBaseUiaTextProvider(null));
    }

    [WinFormsFact]
    public void TextBoxBaseUiaTextProvider_IsMultiline_IsCorrect()
    {
        using TextBoxBase textBoxBase = new SubTextBoxBase();
        TextBoxBaseUiaTextProvider provider = new(textBoxBase);

        textBoxBase.Multiline = false;
        Assert.False(provider.IsMultiline);

        textBoxBase.Multiline = true;
        Assert.True(provider.IsMultiline);

        Assert.False(textBoxBase.IsHandleCreated);
    }

    [WinFormsTheory]
    [InlineData(true)]
    [InlineData(false)]
    public void TextBoxBaseUiaTextProvider_IsReadOnly_IsCorrect(bool readOnly)
    {
        using TextBoxBase textBoxBase = new SubTextBoxBase();
        TextBoxBaseUiaTextProvider provider = new(textBoxBase);

        textBoxBase.ReadOnly = readOnly;
        Assert.Equal(readOnly, provider.IsReadOnly);
        Assert.False(textBoxBase.IsHandleCreated);
    }

    [WinFormsFact]
    public void TextBoxBaseUiaTextProvider_IsScrollable_IsCorrect()
    {
        using TextBoxBase textBoxBase = new SubTextBoxBase();
        textBoxBase.CreateControl();
        TextBoxBaseUiaTextProvider provider = new(textBoxBase);
        Assert.True(provider.IsScrollable);
        Assert.True(textBoxBase.IsHandleCreated);
    }

    [WinFormsFact]
    public void TextBoxBaseUiaTextProvider_IsScrollable_False_WithoutHandle()
    {
        using TextBoxBase textBoxBase = new SubTextBoxBase();
        TextBoxBaseUiaTextProvider provider = new(textBoxBase);
        Assert.False(provider.IsScrollable);
        Assert.False(textBoxBase.IsHandleCreated);
    }

    [WinFormsFact]
    public void TextBoxBaseTextProvider_GetWindowStyle_ReturnsNoneForNotInitializedControl()
    {
        using TextBoxBase textBoxBase = new SubTextBoxBase();
        TextBoxBaseUiaTextProvider provider = new(textBoxBase);
        Assert.Equal(WINDOW_STYLE.WS_OVERLAPPED, provider.WindowStyle);
        Assert.False(textBoxBase.IsHandleCreated);
    }

    [WinFormsTheory]
    [InlineData(RightToLeft.Yes, true)]
    [InlineData(RightToLeft.No, false)]
    public void TextBoxBaseUiaTextProvider_IsReadingRTL_ReturnsCorrectValue(RightToLeft rightToLeft, bool expectedResult)
    {
        using TextBoxBase textBoxBase = new SubTextBoxBase();
        textBoxBase.CreateControl();
        textBoxBase.RightToLeft = rightToLeft;

        TextBoxBaseUiaTextProvider provider = new(textBoxBase);
        Assert.Equal(expectedResult, provider.IsReadingRTL);
        Assert.True(textBoxBase.IsHandleCreated);
    }

    [WinFormsTheory]
    [InlineData(RightToLeft.Yes)]
    [InlineData(RightToLeft.No)]
    public void TextBoxBaseUiaTextProvider_IsReadingRTL_ReturnsFalse_WithoutHandle(RightToLeft rightToLeft)
    {
        using TextBoxBase textBoxBase = new SubTextBoxBase();
        textBoxBase.RightToLeft = rightToLeft;
        TextBoxBaseUiaTextProvider provider = new(textBoxBase);
        Assert.False(provider.IsReadingRTL);
        Assert.False(textBoxBase.IsHandleCreated);
    }

    [WinFormsFact]
    public void TextBoxBaseUiaTextProvider_DocumentRange_IsNotNull()
    {
        using TextBoxBase textBoxBase = new SubTextBoxBase();
        TextBoxBaseUiaTextProvider provider = new(textBoxBase);
        using ComScope<ITextRangeProvider> documentRange = new(provider.DocumentRange);
        Assert.False(documentRange.IsNull);
        Assert.False(textBoxBase.IsHandleCreated);
    }

    [WinFormsFact]
    public void TextBoxBaseUiaTextProvider_SupportedTextSelection_IsNotNull()
    {
        using TextBoxBase textBoxBase = new SubTextBoxBase();
        TextBoxBaseUiaTextProvider provider = new(textBoxBase);
        SupportedTextSelection uiaTextRange = provider.SupportedTextSelection;
        Assert.Equal(SupportedTextSelection.SupportedTextSelection_Single, uiaTextRange);
        Assert.False(textBoxBase.IsHandleCreated);
    }

    [WinFormsFact]
    public void TextBoxBaseUiaTextProvider_GetCaretRange_IsNotNull()
    {
        using TextBoxBase textBoxBase = new SubTextBoxBase();
        textBoxBase.CreateControl();
        TextBoxBaseUiaTextProvider provider = new(textBoxBase);
        using ComScope<ITextRangeProvider> uiaTextRange = new(null);
        BOOL isActive = default;
        Assert.True(provider.GetCaretRange(&isActive, uiaTextRange).Succeeded);
        Assert.False(uiaTextRange.IsNull);
        Assert.True(textBoxBase.IsHandleCreated);
    }

    [WinFormsFact]
    public void TextBoxBaseUiaTextProvider_GetCaretRange_IsNull_IfHandleIsNotCreated()
    {
        using TextBoxBase textBoxBase = new SubTextBoxBase();
        TextBoxBaseUiaTextProvider provider = new(textBoxBase);
        using ComScope<ITextRangeProvider> uiaTextRange = new(null);
        BOOL isActive = default;
        Assert.True(provider.GetCaretRange(&isActive, uiaTextRange).Succeeded);
        Assert.True(uiaTextRange.IsNull);
        Assert.False(textBoxBase.IsHandleCreated);
    }

    [WinFormsTheory]
    [InlineData(30, 0, 0)]
    [InlineData(30, 19, 1)] // Only 1 lines are placed at a height equal to 19
    [InlineData(30, 50, 3)] // Only 3 lines are placed at a height equal to 50
    [InlineData(30, 100, 6)] // Only 6 lines are placed at a height equal to 100
    public void TextBoxBaseUiaTextProvider_LinesPerPage_IsCorrect_with_handle(int width, int height, int lines)
    {
        using TextBoxBase textBoxBase = new SubTextBoxBase { Size = new Size(width, height) };
        textBoxBase.CreateControl();
        Assert.True(textBoxBase.IsHandleCreated);
        TextBoxBaseUiaTextProvider provider = new(textBoxBase);

        Assert.Equal(1, provider.LinesPerPage);

        textBoxBase.Multiline = true;
        Assert.Equal(lines, provider.LinesPerPage);
    }

    [WinFormsFact]
    public void TextBoxBaseUiaTextProvider_LinesPerPage_ReturnsMinusOne_WithoutHandle()
    {
        using TextBoxBase textBoxBase = new SubTextBoxBase();
        TextBoxBaseUiaTextProvider provider = new(textBoxBase);

        Assert.Equal(-1, provider.LinesPerPage);
        Assert.False(textBoxBase.IsHandleCreated);
    }

    [WinFormsFact]
    public void TextBoxBaseUiaTextProvider_FirstVisibleLine_Get_ReturnsCorrectValue()
    {
        using TextBoxBase textBoxBase = new SubTextBoxBase();
        textBoxBase.Multiline = true;
        textBoxBase.Size = new Size(50, 100);
        textBoxBase.CreateControl();
        Assert.True(textBoxBase.IsHandleCreated);

        TextBoxBaseUiaTextProvider provider = new(textBoxBase);

        Assert.Equal(0, provider.FirstVisibleLine);

        provider.LineScroll(0, 2);
        Assert.Equal(0, provider.FirstVisibleLine);

        textBoxBase.Text = "Some long long test text for testing GetFirstVisibleLine method";
        provider.LineScroll(0, 2);
        Assert.Equal(2, provider.FirstVisibleLine);
    }

    [WinFormsFact]
    public void TextBoxBaseUiaTextProvider_FirstVisibleLine_Get_ReturnsMinusOne_WithoutHandle()
    {
        using TextBoxBase textBoxBase = new SubTextBoxBase();
        TextBoxBaseUiaTextProvider provider = new(textBoxBase);

        int line = provider.FirstVisibleLine;
        Assert.Equal(-1, line);

        textBoxBase.Multiline = true;
        textBoxBase.Size = new Size(30, 100);

        line = provider.FirstVisibleLine;
        Assert.Equal(-1, line);
        Assert.False(textBoxBase.IsHandleCreated);
    }

    [WinFormsFact]
    public void TextBoxBaseUiaTextProvider_LineCount_Get_ReturnsCorrectValue()
    {
        using TextBoxBase textBoxBase = new SubTextBoxBase();
        textBoxBase.CreateControl();
        TextBoxBaseUiaTextProvider provider = new(textBoxBase);
        Assert.Equal(1, provider.LinesCount);

        textBoxBase.Multiline = true;
        textBoxBase.Size = new Size(30, 50);
        Assert.Equal(1, provider.LinesCount);

        textBoxBase.Text += "1\r\n";
        Assert.Equal(2, provider.LinesCount);

        textBoxBase.Text += "2\r\n";
        Assert.Equal(3, provider.LinesCount);

        textBoxBase.Text += "3\r\n";
        Assert.Equal(4, provider.LinesCount);

        textBoxBase.Text += "4\r\n";
        Assert.Equal(5, provider.LinesCount);

        Assert.True(textBoxBase.IsHandleCreated);
    }

    [WinFormsFact]
    public void TextBoxBaseUiaTextProvider_LineCount_Get_ReturnsMinusOne_WithoutHandle()
    {
        using TextBoxBase textBoxBase = new SubTextBoxBase();
        TextBoxBaseUiaTextProvider provider = new(textBoxBase);
        Assert.Equal(-1, provider.LinesCount);

        textBoxBase.Multiline = true;
        textBoxBase.Size = new Size(30, 50);
        Assert.Equal(-1, provider.LinesCount);

        textBoxBase.Text += "1\r\n";
        Assert.Equal(-1, provider.LinesCount);

        Assert.False(textBoxBase.IsHandleCreated);
    }

    public static IEnumerable<object[]> TextBoxBase_GetLineFromCharIndex_TestData()
    {
        yield return new object[] { new Size(50, 20), false, 0, 0 };
        yield return new object[] { new Size(50, 20), false, 50, 0 };
        yield return new object[] { new Size(100, 50), true, 50, 3 };
        yield return new object[] { new Size(50, 50), true, 50, 8 };
    }

    [WinFormsTheory]
    [MemberData(nameof(TextBoxBase_GetLineFromCharIndex_TestData))]
    public void TextBoxBaseUiaTextProvider_GetLineFromCharIndex_ReturnsCorrectValue(Size size, bool multiline, int charIndex, int expectedLine)
    {
        using SubTextBoxBase textBoxBase = new() { Size = size, Multiline = multiline };
        textBoxBase.CreateControl();
        textBoxBase.Text = "Some test text for testing GetLineFromCharIndex method";
        TextBoxBaseUiaTextProvider provider = new(textBoxBase);
        int actualLine = provider.GetLineFromCharIndex(charIndex);
        Assert.Equal(expectedLine, actualLine);
        Assert.True(textBoxBase.IsHandleCreated);
    }

#pragma warning disable xUnit1026 // Disable xUnit1026 warning: The method doesn't use parameter 'expectedLine'
    [WinFormsTheory]
    [MemberData(nameof(TextBoxBase_GetLineFromCharIndex_TestData))]
    public void TextBoxBaseUiaTextProvider_GetLineFromCharIndex_ReturnsMinusOne_WithoutHandle(Size size, bool multiline, int charIndex, int expectedLine)
    {
        using SubTextBoxBase textBoxBase = new()
        {
            Size = size,
            Multiline = multiline,
            Text = "Some test text for testing GetLineFromCharIndex method"
        };
        TextBoxBaseUiaTextProvider provider = new(textBoxBase);

        int actualLine = provider.GetLineFromCharIndex(charIndex);

        Assert.Equal(-1, actualLine);
        Assert.False(textBoxBase.IsHandleCreated);
    }
#pragma warning restore xUnit1026

    public static IEnumerable<object[]> TextBoxBaseUiaTextProvider_GetLineIndex_TestData()
    {
        yield return new object[] { new Size(50, 20), false, 0, 0 };
        yield return new object[] { new Size(50, 20), false, 3, 0 };
        yield return new object[] { new Size(50, 50), true, 3, 19 };
        yield return new object[] { new Size(100, 50), true, 3, 40 };
        yield return new object[] { new Size(50, 50), true, 100, -1 };
    }

    [WinFormsTheory]
    [MemberData(nameof(TextBoxBaseUiaTextProvider_GetLineIndex_TestData))]
    public void TextBoxBaseUiaTextProvider_GetLineIndex_ReturnsCorrectValue(Size size, bool multiline, int lineIndex, int expectedIndex)
    {
        using SubTextBoxBase textBoxBase = new()
        {
            Size = size,
            Multiline = multiline,
            Text = "Some test text for testing GetLineIndex method"
        };
        textBoxBase.CreateControl();
        TextBoxBaseUiaTextProvider provider = new(textBoxBase);

        int actualIndex = provider.GetLineIndex(lineIndex);

        Assert.Equal(expectedIndex, actualIndex);
        Assert.True(textBoxBase.IsHandleCreated);
    }

#pragma warning disable xUnit1026 // Disable xUnit1026 warning: The method doesn't use parameter 'expectedIndex'
    [WinFormsTheory]
    [MemberData(nameof(TextBoxBaseUiaTextProvider_GetLineIndex_TestData))]
    public void TextBoxBaseUiaTextProvider_GetLineIndex_ReturnsMinusOne_WithoutHandle(Size size, bool multiline, int lineIndex, int expectedIndex)
    {
        using SubTextBoxBase textBoxBase = new()
        {
            Size = size,
            Multiline = multiline,
            Text = "Some test text for testing GetLineIndex method"
        };
        TextBoxBaseUiaTextProvider provider = new(textBoxBase);

        int actualIndex = provider.GetLineIndex(lineIndex);

        Assert.Equal(-1, actualIndex);
        Assert.False(textBoxBase.IsHandleCreated);
    }
#pragma warning restore xUnit1026

    [WinFormsFact]
    public void TextBoxBaseUiaTextProvider_GetLogfont_ReturnsCorrectValue()
    {
        using (new NoAssertContext())
        {
            using TextBoxBase textBoxBase = new SubTextBoxBase();
            textBoxBase.CreateControl();
            TextBoxBaseUiaTextProvider provider = new(textBoxBase);

            LOGFONTW expected = textBoxBase.Font.ToLogicalFont();
            LOGFONTW actual = provider.Logfont;
            Assert.False(string.IsNullOrEmpty(actual.FaceName.ToString()));
            Assert.Equal(expected, actual);
            Assert.True(textBoxBase.IsHandleCreated);
        }
    }

    [WinFormsFact]
    public void TextBoxBaseUiaTextProvider_GetLogfont_ReturnsEmpty_WithoutHandle()
    {
        using TextBoxBase textBoxBase = new SubTextBoxBase();
        TextBoxBaseUiaTextProvider provider = new(textBoxBase);

        LOGFONTW expected = default;
        LOGFONTW actual = provider.Logfont;
        Assert.True(string.IsNullOrEmpty(actual.FaceName.ToString()));
        Assert.Equal(expected, actual);
        Assert.False(textBoxBase.IsHandleCreated);
    }

    public static IEnumerable<object[]> TextBoxBaseUiaTextProvider_GetPositionFromChar_TestData()
    {
        yield return new object[] { new Size(50, 20), "Some test text for testing", false, 0, new Point(1, 0) };
        yield return new object[] { new Size(50, 20), "Some test text for testing", false, 15, new Point(79, 0) };
        yield return new object[] { new Size(50, 20), "Some test text for testing", true, 15, new Point(27, 30) };
        yield return new object[] { new Size(100, 60), "This is a\r\nlong long text\r\nfor testing\r\nGetPositionFromChar method", true, 0, new Point(4, 1) };
        yield return new object[] { new Size(100, 60), "This is a\r\nlong long text\r\nfor testing\r\nGetPositionFromChar method", true, 6, new Point(31, 1) };
        yield return new object[] { new Size(100, 60), "This is a\r\nlong long text\r\nfor testing\r\nGetPositionFromChar method", true, 26, new Point(78, 16) };
    }

    [WinFormsTheory]
    [MemberData(nameof(TextBoxBaseUiaTextProvider_GetPositionFromChar_TestData))]
    public void TextBoxBaseUiaTextProvider_GetPositionFromChar_ReturnsCorrectValue(Size size, string text, bool multiline, int charIndex, Point expectedPoint)
    {
        using SubTextBoxBase textBoxBase = new() { Size = size, Text = text, Multiline = multiline };
        textBoxBase.CreateControl();
        TextBoxBaseUiaTextProvider provider = new(textBoxBase);

        Point actualPoint = provider.GetPositionFromChar(charIndex);

        Assert.True(actualPoint.X >= expectedPoint.X - 1 || actualPoint.X <= expectedPoint.X + 1);
        Assert.True(actualPoint.Y >= expectedPoint.Y - 1 || actualPoint.Y <= expectedPoint.Y + 1);
        Assert.True(textBoxBase.IsHandleCreated);
    }

#pragma warning disable xUnit1026 // Disable xUnit1026 warning: The method doesn't use parameter 'expectedPoint'
    [WinFormsTheory]
    [MemberData(nameof(TextBoxBaseUiaTextProvider_GetPositionFromChar_TestData))]
    public void TextBoxBaseUiaTextProvider_GetPositionFromChar_ReturnsEmpty_WithoutHandle(Size size, string text, bool multiline, int charIndex, Point expectedPoint)
    {
        using SubTextBoxBase textBoxBase = new() { Size = size, Text = text, Multiline = multiline };
        TextBoxBaseUiaTextProvider provider = new(textBoxBase);
        Point actualPoint = provider.GetPositionFromChar(charIndex);
        Assert.Equal(Point.Empty, actualPoint);
        Assert.False(textBoxBase.IsHandleCreated);
    }
#pragma warning restore xUnit1026

    public static IEnumerable<object[]> TextBoxBaseUiaTextProvider_GetPositionFromCharForUpperRightCorner_ReturnsCorrectValue_TestData()
    {
        yield return new object[] { new Size(50, 20), "", false, 0, new Point(0, 0) };
        yield return new object[] { new Size(50, 20), "Some test text", false, 100, new Point(0, 0) };
        yield return new object[] { new Size(50, 20), "Some test text", false, -1, new Point(0, 0) };
        yield return new object[] { new Size(50, 20), "Some test text", false, 12, new Point(71, 0) };
        yield return new object[] { new Size(50, 20), "Some test text", true, 12, new Point(19, 30) };
        yield return new object[] { new Size(100, 60), "Some test \n text", false, 10, new Point(56, 0) };
        yield return new object[] { new Size(100, 60), "Some test \n text", true, 10, new Point(59, 1) };
        yield return new object[] { new Size(100, 60), "Some test \r\n text", false, 10, new Point(56, 0) };
        yield return new object[] { new Size(100, 60), "Some test \r\n text", true, 10, new Point(59, 1) };
        yield return new object[] { new Size(100, 60), "Some test \r\n text", false, 12, new Point(60, 0) };
        yield return new object[] { new Size(100, 60), "Some test \r\n text", true, 12, new Point(7, 16) };
        yield return new object[] { new Size(100, 60), "Some test \t text", false, 10, new Point(57, 0) };
        yield return new object[] { new Size(100, 60), "Some test \t text", true, 10, new Point(60, 1) };
        yield return new object[] { new Size(40, 60), "Some test \t text", true, 12, new Point(8, 46) };
    }

    [WinFormsTheory]
    [MemberData(nameof(TextBoxBaseUiaTextProvider_GetPositionFromCharForUpperRightCorner_ReturnsCorrectValue_TestData))]
    public void TextBoxBaseUiaTextProvider_GetPositionFromCharForUpperRightCorner_ReturnsCorrectValue(Size size, string text, bool multiline, int charIndex, Point expectedPoint)
    {
        using SubTextBoxBase textBoxBase = new() { Size = size, Text = text, Multiline = multiline };
        textBoxBase.CreateControl();
        TextBoxBaseUiaTextProvider provider = new(textBoxBase);
        Point actualPoint = provider.GetPositionFromCharForUpperRightCorner(charIndex, textBoxBase.Text);
        Assert.True(actualPoint.X >= expectedPoint.X - 1 || actualPoint.X <= expectedPoint.X + 1);
        Assert.True(actualPoint.Y >= expectedPoint.Y - 1 || actualPoint.Y <= expectedPoint.Y + 1);
        Assert.True(textBoxBase.IsHandleCreated);
    }

#pragma warning disable xUnit1026 // Disable xUnit1026 warning: The method doesn't use parameter 'expectedPoint'
    [WinFormsTheory]
    [MemberData(nameof(TextBoxBaseUiaTextProvider_GetPositionFromCharForUpperRightCorner_ReturnsCorrectValue_TestData))]
    public void TextBoxBaseUiaTextProvider_GetPositionFromCharForUpperRightCorner_ReturnsMinusOne_WithoutHandle(Size size, string text, bool multiline, int charIndex, Point expectedPoint)
    {
        using SubTextBoxBase textBoxBase = new() { Size = size, Text = text, Multiline = multiline };
        TextBoxBaseUiaTextProvider provider = new(textBoxBase);
        Point actualPoint = provider.GetPositionFromCharForUpperRightCorner(charIndex, textBoxBase.Text);
        Assert.Equal(Point.Empty, actualPoint);
        Assert.False(textBoxBase.IsHandleCreated);
    }
#pragma warning restore xUnit1026

    public static IEnumerable<object[]> TextBoxBaseUiaTextProvider_GetFormattingRectangle_TestData()
    {
        yield return new object[] { false, new Size(0, 0), new Rectangle(1, 0, 78, 16) };
        yield return new object[] { false, new Size(50, 50), new Rectangle(1, 1, 44, 15) };
        yield return new object[] { false, new Size(250, 100), new Rectangle(1, 1, 244, 15) };
        yield return new object[] { true, new Size(0, 0), new Rectangle(4, 0, 72, 16) };

        if (Application.UseVisualStyles)
        {
            yield return new object[] { true, new Size(50, 50), new Rectangle(4, 1, 38, 44) };
            yield return new object[] { true, new Size(250, 100), new Rectangle(4, 1, 238, 94) };
        }
        else
        {
            yield return new object[] { true, new Size(50, 50), new Rectangle(4, 1, 38, 30) };
            yield return new object[] { true, new Size(250, 100), new Rectangle(4, 1, 238, 90) };
        }
    }

    [WinFormsTheory]
    [MemberData(nameof(TextBoxBaseUiaTextProvider_GetFormattingRectangle_TestData))]
    public void TextBoxBaseUiaTextProvider_GetFormattingRectangle_ReturnsCorrectValue(bool multiline, Size size, Rectangle expectedRectangle)
    {
        using SubTextBoxBase textBoxBase = new() { Size = size, Multiline = multiline };
        textBoxBase.CreateControl();
        TextBoxBaseUiaTextProvider provider = new(textBoxBase);

        Rectangle providerRectangle = provider.BoundingRectangle;

        Assert.Equal(expectedRectangle, providerRectangle);
        Assert.True(textBoxBase.IsHandleCreated);
    }

#pragma warning disable xUnit1026 // Disable xUnit1026 warning: The method doesn't use parameter 'expectedRectangle'
    [WinFormsTheory]
    [MemberData(nameof(TextBoxBaseUiaTextProvider_GetFormattingRectangle_TestData))]
    public void TextBoxBaseUiaTextProvider_GetFormattingRectangle_ReturnsEmpty_WithoutHandle(bool multiline, Size size, Rectangle expectedRectangle)
    {
        using SubTextBoxBase textBoxBase = new() { Size = size, Multiline = multiline };
        TextBoxBaseUiaTextProvider provider = new(textBoxBase);

        Rectangle providerRectangle = provider.BoundingRectangle;

        Assert.Equal(Rectangle.Empty, providerRectangle);
        Assert.False(textBoxBase.IsHandleCreated);
    }
#pragma warning restore xUnit1026

    [WinFormsTheory]
    [InlineData("")]
    [InlineData("Text")]
    [InlineData("Some test text")]
    public void TextBoxBaseUiaTextProvider_Text_ReturnsCorrectValue(string text)
    {
        using TextBoxBase textBoxBase = new SubTextBoxBase();
        textBoxBase.Text = text;
        TextBoxBaseUiaTextProvider provider = new(textBoxBase);
        textBoxBase.CreateControl();
        string expected = textBoxBase.Text;
        string actual = provider.Text.Trim('\0');
        Assert.Equal(expected, actual);
        Assert.True(textBoxBase.IsHandleCreated);
    }

    [WinFormsTheory]
    [InlineData("")]
    [InlineData("Text")]
    [InlineData("Some test text")]
    public void TextBoxBaseUiaTextProvider_Text_ReturnsEmpty_WithoutHandle(string text)
    {
        using TextBoxBase textBoxBase = new SubTextBoxBase();
        textBoxBase.Text = text;
        TextBoxBaseUiaTextProvider provider = new(textBoxBase);
        string expected = string.Empty;
        string actual = provider.Text.Trim('\0');
        Assert.Equal(expected, actual);
        Assert.False(textBoxBase.IsHandleCreated);
    }

    [WinFormsTheory]
    [InlineData("")]
    [InlineData("Text")]
    [InlineData("Some test text for testing")]
    [InlineData("Some very very very long test text for testing GetTextLength method")]
    public void TextBoxBaseUiaTextProvider_TextLength_ReturnsCorrectValue(string text)
    {
        using TextBoxBase textBoxBase = new SubTextBoxBase
        {
            Text = text
        };
        textBoxBase.CreateControl();
        TextBoxBaseUiaTextProvider provider = new(textBoxBase);

        Assert.Equal(textBoxBase.Text.Length, provider.TextLength);
        Assert.True(textBoxBase.IsHandleCreated);
    }

    [WinFormsFact]
    public void TextBoxBaseUiaTextProvider_WindowExStyle_ReturnsCorrectValue()
    {
        using TextBoxBase textBoxBase = new SubTextBoxBase();
        textBoxBase.CreateControl();
        TextBoxBaseUiaTextProvider provider = new(textBoxBase);
        WINDOW_EX_STYLE actual = provider.WindowExStyle;
        Assert.Equal(WINDOW_EX_STYLE.WS_EX_CLIENTEDGE, actual);
        Assert.True(textBoxBase.IsHandleCreated);
    }

    [WinFormsFact]
    public void TextBoxBaseUiaTextProvider_WindowExStyle_ReturnsLeft_WithoutHandle()
    {
        using TextBoxBase textBoxBase = new SubTextBoxBase();
        TextBoxBaseUiaTextProvider provider = new(textBoxBase);
        WINDOW_EX_STYLE actual = provider.WindowExStyle;
        Assert.Equal(WINDOW_EX_STYLE.WS_EX_LEFT, actual);
        Assert.False(textBoxBase.IsHandleCreated);
    }

    [WinFormsFact]
    public void TextBoxBaseUiaTextProvider_EditStyle_ReturnsCorrectValue()
    {
        using TextBoxBase textBoxBase = new SubTextBoxBase();
        textBoxBase.CreateControl();
        TextBoxBaseUiaTextProvider provider = new(textBoxBase);
        WINDOW_STYLE actual = provider.WindowStyle;
        Assert.Equal(0, ((int)actual & PInvoke.ES_RIGHT));
        Assert.NotEqual(0, ((int)actual & PInvoke.ES_AUTOVSCROLL));
        Assert.NotEqual(0, ((int)actual & PInvoke.ES_AUTOHSCROLL));
        Assert.True(textBoxBase.IsHandleCreated);
    }

    [WinFormsFact]
    public void TextBoxBaseUiaTextProvider_EditStyle_ReturnsLeft_WithoutHandle()
    {
        using TextBoxBase textBoxBase = new SubTextBoxBase();
        TextBoxBaseUiaTextProvider provider = new(textBoxBase);
        WINDOW_STYLE actual = provider.WindowStyle;
        Assert.Equal(PInvoke.ES_LEFT, (int)actual);
        Assert.False(textBoxBase.IsHandleCreated);
    }

    public static IEnumerable<object[]> TextBoxBase_GetVisibleRangePoints_ForSinglelineTextBox_TestData()
    {
        yield return new object[] { new Size(0, 0), 0, 0 };
        yield return new object[] { new Size(0, 20), 0, 0 };
        yield return new object[] { new Size(30, 30), 0, 4 };
        yield return new object[] { new Size(50, 20), 0, 8 };
        yield return new object[] { new Size(150, 20), 0, 26 };
    }

    [WinFormsTheory]
    [MemberData(nameof(TextBoxBase_GetVisibleRangePoints_ForSinglelineTextBox_TestData))]
    public void TextBoxBaseUiaTextProvider_GetVisibleRangePoints_ForSinglelineTextBox_ReturnsCorrectValue(Size size, int expectedStart, int expectedEnd)
    {
        using SubTextBoxBase textBoxBase = new()
        {
            Multiline = false,
            Text = "Some test text for testing",
            Size = size
        };
        textBoxBase.CreateControl();
        TextBoxBaseUiaTextProvider provider = new(textBoxBase);

        provider.GetVisibleRangePoints(out int providerVisibleStart, out int providerVisibleEnd);

        Assert.True(providerVisibleStart >= 0);
        Assert.True(providerVisibleStart < textBoxBase.Text.Length);
        Assert.True(providerVisibleEnd >= 0);
        Assert.True(providerVisibleEnd <= textBoxBase.Text.Length);

        Assert.Equal(expectedStart, providerVisibleStart);
        Assert.Equal(expectedEnd, providerVisibleEnd);
        Assert.True(textBoxBase.IsHandleCreated);
    }

    public static IEnumerable<object[]> TextBoxBaseUiaTextProvider_GetVisibleRangePoints_ForMultilineTextBox_TestData()
    {
        if (Application.UseVisualStyles)
        {
            yield return new object[] { new Size(0, 0), 0, 0 };
            yield return new object[] { new Size(0, 20), 0, 0 };
            yield return new object[] { new Size(30, 30), 0, 5 };
            yield return new object[] { new Size(50, 20), 0, 5 };
            yield return new object[] { new Size(120, 20), 0, 19 };
            yield return new object[] { new Size(50, 80), 0, 26 };
        }
        else
        {
            yield return new object[] { new Size(0, 0), 0, 0 };
            yield return new object[] { new Size(0, 20), 0, 0 };
            yield return new object[] { new Size(30, 30), 0, 3 };
            yield return new object[] { new Size(50, 20), 0, 6 };
            yield return new object[] { new Size(120, 20), 0, 20 };
            yield return new object[] { new Size(50, 80), 0, 26 };
        }
    }

    [WinFormsTheory]
    [MemberData(nameof(TextBoxBaseUiaTextProvider_GetVisibleRangePoints_ForMultilineTextBox_TestData))]
    public void TextBoxBaseUiaTextProvider_GetVisibleRangePoints_ForMultilineTextBox_ReturnsCorrectValue(
        Size size, int expectedStart, int expectedEnd)
    {
        using SubTextBoxBase textBoxBase = new()
        {
            Multiline = true,
            Text = "Some test text for testing",
            Size = size,
            WordWrap = true
        };
        textBoxBase.CreateControl();
        TextBoxBaseUiaTextProvider provider = new(textBoxBase);

        provider.GetVisibleRangePoints(out int providerVisibleStart, out int providerVisibleEnd);

        Assert.True(providerVisibleStart >= 0);
        Assert.True(providerVisibleStart < textBoxBase.Text.Length);
        Assert.True(providerVisibleEnd >= 0);
        Assert.True(providerVisibleEnd <= textBoxBase.Text.Length);

        Assert.Equal(expectedStart, providerVisibleStart);
        Assert.Equal(expectedEnd, providerVisibleEnd);
        Assert.True(textBoxBase.IsHandleCreated);
    }

#pragma warning disable xUnit1026 // Disable xUnit1026 warning: The method doesn't use parameters 'expectedStart' and 'expectedEnd'
    [WinFormsTheory]
    [MemberData(nameof(TextBoxBaseUiaTextProvider_GetVisibleRangePoints_ForMultilineTextBox_TestData))]
    public void TextBoxBaseUiaTextProvider_GetVisibleRangePoints_ReturnsZeros_WithoutHandle(Size size, int expectedStart, int expectedEnd)
    {
        using SubTextBoxBase textBoxBase = new() { Size = size };
        textBoxBase.Text = "Some test text for testing";
        TextBoxBaseUiaTextProvider provider = new(textBoxBase);

        provider.GetVisibleRangePoints(out int providerVisibleStart, out int providerVisibleEnd);

        Assert.Equal(0, providerVisibleStart);
        Assert.Equal(0, providerVisibleEnd);
        Assert.False(textBoxBase.IsHandleCreated);
    }
#pragma warning restore xUnit1026

    public static IEnumerable<object[]> TextBoxBaseUiaTextProvider_GetVisibleRanges_TestData()
    {
        yield return new object[] { new Size(0, 0) };
        yield return new object[] { new Size(100, 20) };
    }

    [WinFormsTheory]
    [MemberData(nameof(TextBoxBaseUiaTextProvider_GetVisibleRanges_TestData))]
    public void TextBoxBaseUiaTextProvider_GetVisibleRanges_ReturnsCorrectValue(Size size)
    {
        using SubTextBoxBase textBoxBase = new()
        {
            Text = "Some test text for testing",
            Size = size
        };
        textBoxBase.CreateControl();
        TextBoxBaseUiaTextProvider provider = new(textBoxBase);

        using ComSafeArrayScope<ITextRangeProvider> scope = new(null);
        Assert.True(provider.GetVisibleRanges(scope).Succeeded);
        Assert.False(scope.IsEmpty);
        Assert.True(textBoxBase.IsHandleCreated);
    }

    [WinFormsTheory]
    [MemberData(nameof(TextBoxBaseUiaTextProvider_GetVisibleRanges_TestData))]
    public void TextBoxBaseUiaTextProvider_GetVisibleRanges_ReturnsNull_WithoutHandle(Size size)
    {
        using SubTextBoxBase textBoxBase = new()
        {
            Text = "Some test text for testing",
            Size = size
        };
        TextBoxBaseUiaTextProvider provider = new(textBoxBase);

        using ComSafeArrayScope<ITextRangeProvider> scope = new(null);
        Assert.True(provider.GetVisibleRanges(scope).Succeeded);
        Assert.True(scope.IsEmpty);
        Assert.False(textBoxBase.IsHandleCreated);
    }

    [WinFormsFact]
    public void TextBoxBaseUiaTextProvider_RangeFromAnnotation_DoesntThrowAnException()
    {
        using TextBoxBase textBoxBase = new SubTextBoxBase();
        TextBoxBaseUiaTextProvider provider = new(textBoxBase);

        // RangeFromAnnotation doesn't throw an exception
        using ComScope<ITextRangeProvider> range = new(null);
        Assert.True(provider.RangeFromAnnotation(ComHelpers.GetComPointer<IRawElementProviderSimple>(textBoxBase.AccessibilityObject), range).Succeeded);
        // RangeFromAnnotation implementation can be changed so this test can be changed too
        Assert.False(range.IsNull);
    }

    [WinFormsFact]
    public void TextBoxBaseUiaTextProvider_RangeFromChild_DoesntThrowAnException()
    {
        using (new NoAssertContext())
        {
            using TextBoxBase textBoxBase = new SubTextBoxBase();
            TextBoxBaseUiaTextProvider provider = new(textBoxBase);

            // RangeFromChild doesn't throw an exception
            using ComScope<ITextRangeProvider> range = new(null);
            Assert.True(provider.RangeFromChild(ComHelpers.GetComPointer<IRawElementProviderSimple>(textBoxBase.AccessibilityObject), range).Succeeded);
            // RangeFromChild implementation can be changed so this test can be changed too
            Assert.True(range.IsNull);
        }
    }

    public static IEnumerable<object[]> TextBoxBaseUiaTextProvider_RangeFromPoint_TestData()
    {
        yield return new object[] { Point.Empty };
        yield return new object[] { new Point(10, 10) };
    }

    [WinFormsTheory]
    [MemberData(nameof(TextBoxBaseUiaTextProvider_RangeFromPoint_TestData))]
    public void TextBoxBaseUiaTextProvider_RangeFromPoint_DoesntThrowAnException(Point point)
    {
        using TextBoxBase textBoxBase = new SubTextBoxBase();
        textBoxBase.CreateControl();
        TextBoxBaseUiaTextProvider provider = new(textBoxBase);

        using ComScope<ITextRangeProvider> range = new(null);
        Assert.True(provider.RangeFromPoint(point, range).Succeeded);
        Assert.False(range.IsNull);

        Assert.True(textBoxBase.IsHandleCreated);
    }

    [WinFormsTheory]
    [MemberData(nameof(TextBoxBaseUiaTextProvider_RangeFromPoint_TestData))]
    public void TextBoxBaseUiaTextProvider_RangeFromPoint_ReturnsNull_WithoutHandle(Point point)
    {
        using TextBoxBase textBoxBase = new SubTextBoxBase();
        TextBoxBaseUiaTextProvider provider = new(textBoxBase);

        using ComScope<ITextRangeProvider> range = new(null);
        Assert.True(provider.RangeFromPoint(point, range).Succeeded);
        Assert.True(range.IsNull);

        Assert.False(textBoxBase.IsHandleCreated);
    }

    [WinFormsTheory]
    [InlineData(2, 5)]
    [InlineData(0, 10)]
    public void TextBoxBaseUiaTextProvider_SetSelection_GetSelection_ReturnCorrectValue(int start, int end)
    {
        using TextBoxBase textBoxBase = new SubTextBoxBase();
        textBoxBase.CreateControl();
        textBoxBase.Text = "Some test text for testing";
        TextBoxBaseUiaTextProvider provider = new(textBoxBase);
        provider.SetSelection(start, end);
        using ComSafeArrayScope<ITextRangeProvider> selection = new(null);
        Assert.True(provider.GetSelection(selection).Succeeded);
        Assert.False(selection.IsEmpty);

        using ComScope<ITextRangeProvider> range = new(selection[0]);
        UiaTextRange textRange = ComHelpers.GetObjectForIUnknown(range) as UiaTextRange;
        Assert.NotNull(textRange);

        Assert.Equal(start, textRange.Start);
        Assert.Equal(end, textRange.End);

        Assert.True(textBoxBase.IsHandleCreated);
    }

    [WinFormsTheory]
    [InlineData(2, 5)]
    [InlineData(0, 10)]
    public void TextBoxBaseUiaTextProvider_SetSelection_GetSelection_DontWork_WithoutHandle(int start, int end)
    {
        using TextBoxBase textBoxBase = new SubTextBoxBase();
        textBoxBase.Text = "Some test text for testing";
        TextBoxBaseUiaTextProvider provider = new(textBoxBase);
        provider.SetSelection(start, end);
        Assert.False(textBoxBase.IsHandleCreated);
        using ComSafeArrayScope<ITextRangeProvider> selection = new(null);
        Assert.True(provider.GetSelection(selection).Succeeded);
        Assert.True(selection.IsEmpty);
        Assert.False(textBoxBase.IsHandleCreated);
    }

    [WinFormsTheory]
    [InlineData(-5, 10)]
    [InlineData(5, 100)]
    public void TextBoxBaseUiaTextProvider_SetSelection_DoesntSelectText_IfIncorrectArguments(int start, int end)
    {
        using (new NoAssertContext())
        {
            using TextBoxBase textBoxBase = new SubTextBoxBase();
            textBoxBase.CreateControl();
            textBoxBase.Text = "Some test text for testing";
            TextBoxBaseUiaTextProvider provider = new(textBoxBase);
            provider.SetSelection(start, end);
            using ComSafeArrayScope<ITextRangeProvider> selection = new(null);
            Assert.True(provider.GetSelection(selection).Succeeded);
            Assert.False(selection.IsEmpty);

            using ComScope<ITextRangeProvider> range = new(selection[0]);
            UiaTextRange textRange = ComHelpers.GetObjectForIUnknown(range) as UiaTextRange;
            Assert.NotNull(textRange);

            Assert.Equal(0, textRange.Start);
            Assert.Equal(0, textRange.End);

            Assert.True(textBoxBase.IsHandleCreated);
        }
    }

    [WinFormsTheory]
    [InlineData(0)]
    [InlineData(2)]
    public void TextBoxBaseUiaTextProvider_LineScroll_ReturnCorrectValue(int expectedLine)
    {
        using TextBoxBase textBoxBase = new SubTextBoxBase
        {
            Multiline = true,
            Text = "Some long long test text for testing GetFirstVisibleLine method",
            Size = new Size(50, 100)
        };
        textBoxBase.CreateControl();
        TextBoxBaseUiaTextProvider provider = new(textBoxBase);

        Assert.True(provider.LineScroll(0, expectedLine));
        Assert.Equal(expectedLine, provider.FirstVisibleLine);
        Assert.True(textBoxBase.IsHandleCreated);
    }

    [WinFormsTheory]
    [InlineData(0)]
    [InlineData(2)]
    public void TextBoxBaseUiaTextProvider_LineScroll_DoesntWork_WithoutHandle(int expectedLine)
    {
        using TextBoxBase textBoxBase = new SubTextBoxBase
        {
            Multiline = true,
            Size = new Size(50, 100)
        };
        TextBoxBaseUiaTextProvider provider = new(textBoxBase);
        textBoxBase.Text = "Some long long test text for testing GetFirstVisibleLine method";

        Assert.False(provider.LineScroll(0, expectedLine));
        Assert.Equal(-1, provider.FirstVisibleLine);
        Assert.False(textBoxBase.IsHandleCreated);
    }

    [WinFormsFact]
    public void TextBoxBaseUiaTextProvider_GetLogfont_ReturnSegoe_ByDefault()
    {
        using TextBoxBase textBoxBase = new SubTextBoxBase();
        Assert.NotEqual(IntPtr.Zero, textBoxBase.Handle);

        TextBoxBaseUiaTextProvider provider = new(textBoxBase);
        LOGFONTW logFont = provider.Logfont;
        string actual = logFont.FaceName.ToString();
        Assert.Equal("Segoe UI", actual);
        Assert.True(textBoxBase.IsHandleCreated);
    }

    [WinFormsFact]
    public void TextBoxBaseUiaTextProvider_GetLogfont_ReturnEmpty_WithoutHandle()
    {
        using TextBoxBase textBoxBase = new SubTextBoxBase();
        TextBoxBaseUiaTextProvider provider = new(textBoxBase);
        Assert.False(textBoxBase.IsHandleCreated);

        Assert.Equal(default, provider.Logfont);
    }

    private class SubTextBoxBase : TextBoxBase
    { }
}
