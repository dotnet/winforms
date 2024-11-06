// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;
using System.Windows.Forms.Automation;
using Windows.Win32.System.Com;
using Windows.Win32.UI.Accessibility;

namespace System.Windows.Forms.Tests;

public unsafe class ComboBox_ComboBoxUiaTextProviderTests
{
    [WinFormsTheory]
    [InlineData(ComboBoxStyle.DropDown)]
    [InlineData(ComboBoxStyle.Simple)]
    public void ComboBoxUiaTextProvider_Ctor_DoesNotCreateControlHandle(ComboBoxStyle dropDownStyle)
    {
        using (new NoAssertContext())
        {
            using ComboBox comboBox = new() { DropDownStyle = dropDownStyle };

            Assert.False(comboBox.IsHandleCreated);
            Assert.Null(comboBox.TestAccessor().Dynamic._childEdit);

            ComboBox.ComboBoxUiaTextProvider provider = new(comboBox);

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
            using ComboBox comboBox = new() { DropDownStyle = dropDownStyle };
            ComboBox.ComboBoxUiaTextProvider provider = new(comboBox);

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
            using ComboBox comboBox = new() { DropDownStyle = dropDownStyle };
            ComboBox.ComboBoxUiaTextProvider provider = new(comboBox);

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
        using ComboBox comboBox = new() { DropDownStyle = dropDownStyle };
        comboBox.CreateControl();
        ComboBox.ComboBoxUiaTextProvider provider = new(comboBox);

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
            using ComboBox comboBox = new() { DropDownStyle = dropDownStyle };
            ComboBox.ComboBoxUiaTextProvider provider = new(comboBox);

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
            using ComboBox comboBox = new() { DropDownStyle = dropDownStyle };
            ComboBox.ComboBoxUiaTextProvider provider = new(comboBox);

            Assert.Equal(WINDOW_STYLE.WS_OVERLAPPED, provider.WindowStyle);
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
        using ComboBox comboBox = new() { DropDownStyle = dropDownStyle, RightToLeft = rightToLeft };
        comboBox.CreateControl();
        ComboBox.ComboBoxUiaTextProvider provider = new(comboBox);

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
            using ComboBox comboBox = new() { DropDownStyle = dropDownStyle, RightToLeft = rightToLeft };
            ComboBox.ComboBoxUiaTextProvider provider = new(comboBox);

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
            using ComboBox comboBox = new() { DropDownStyle = dropDownStyle };
            comboBox.CreateControl();
            ComboBox.ComboBoxUiaTextProvider provider = new(comboBox);

            using ComScope<ITextRangeProvider> range = new(provider.DocumentRange);
            using ComScope<IRawElementProviderSimple> elementProvider = new(range.Value->GetEnclosingElement());
            Assert.Equal(comboBox.ChildEditAccessibleObject, ComHelpers.GetObjectForIUnknown(elementProvider));
            UiaTextRange rangeObj = ComHelpers.GetObjectForIUnknown(range) as UiaTextRange;
            Assert.Equal(provider, rangeObj?.TestAccessor().Dynamic._provider);
            Assert.NotNull(comboBox.TestAccessor().Dynamic._childEdit);
        }
    }

    [WinFormsTheory]
    [InlineData(ComboBoxStyle.DropDown)]
    [InlineData(ComboBoxStyle.Simple)]
    public void ComboBoxUiaTextProvider_DocumentRange_IsNull_ThrowException(ComboBoxStyle dropDownStyle)
    {
        using (new NoAssertContext())
        {
            using ComboBox comboBox = new() { DropDownStyle = dropDownStyle };
            ComboBox.ComboBoxUiaTextProvider provider = new(comboBox);
            Assert.Throws<NullReferenceException>(() => _ = provider.DocumentRange);
        }
    }

    [WinFormsTheory]
    [InlineData(ComboBoxStyle.DropDown)]
    [InlineData(ComboBoxStyle.Simple)]
    public void ComboBoxUiaTextProvider_SupportedTextSelection_IsNotNull(ComboBoxStyle dropDownStyle)
    {
        using (new NoAssertContext())
        {
            using ComboBox comboBox = new() { DropDownStyle = dropDownStyle };
            ComboBox.ComboBoxUiaTextProvider provider = new(comboBox);
            SupportedTextSelection uiaTextRange = provider.SupportedTextSelection;

            Assert.Equal(SupportedTextSelection.SupportedTextSelection_Single, uiaTextRange);
            Assert.False(comboBox.IsHandleCreated);
            Assert.Null(comboBox.TestAccessor().Dynamic._childEdit);
        }
    }

    [WinFormsTheory]
    [InlineData(ComboBoxStyle.DropDown)]
    [InlineData(ComboBoxStyle.Simple)]
    public void ComboBoxUiaTextProvider_GetCaretRange_IsNotNull(ComboBoxStyle dropDownStyle)
    {
        using ComboBox comboBox = new() { DropDownStyle = dropDownStyle };
        comboBox.CreateControl();
        ComboBox.ComboBoxUiaTextProvider provider = new(comboBox);

        ComScope<ITextRangeProvider> uiaTextRange = new(null);
        BOOL isActive = default;
        Assert.True(provider.GetCaretRange(&isActive, uiaTextRange).Succeeded);

        Assert.False(uiaTextRange.IsNull);
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
            using ComboBox comboBox = new() { DropDownStyle = dropDownStyle };
            ComboBox.ComboBoxUiaTextProvider provider = new(comboBox);

            using ComScope<ITextRangeProvider> uiaTextRange = new(null);
            BOOL isActive = default;
            Assert.True(provider.GetCaretRange(&isActive, uiaTextRange).Succeeded);

            Assert.True(uiaTextRange.IsNull);
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
            using ComboBox comboBox = new() { DropDownStyle = dropDownStyle };
            ComboBox.ComboBoxUiaTextProvider provider = new(comboBox);

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
        using ComboBox comboBox = new() { DropDownStyle = dropDownStyle };
        comboBox.CreateControl();
        ComboBox.ComboBoxUiaTextProvider provider = new(comboBox);

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
        using ComboBox comboBox = new() { DropDownStyle = dropDownStyle, Size = new(width, height) };
        comboBox.CreateControl();
        comboBox.Items.Add("Some test text for testing GetLineFromCharIndex method");
        comboBox.SelectedIndex = 0;
        ComboBox.ComboBoxUiaTextProvider provider = new(comboBox);

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
            using ComboBox comboBox = new() { DropDownStyle = dropDownStyle, Size = new(width, height) };
            comboBox.Items.Add("Some test text for testing GetLineFromCharIndex method");
            comboBox.SelectedIndex = 0;
            ComboBox.ComboBoxUiaTextProvider provider = new(comboBox);

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
        using ComboBox comboBox = new() { DropDownStyle = dropDownStyle, Size = new(width, height) };
        comboBox.CreateControl();
        comboBox.Items.Add("Some test text for testing GetLineIndex method");
        comboBox.SelectedIndex = 0;
        ComboBox.ComboBoxUiaTextProvider provider = new(comboBox);

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
            using ComboBox comboBox = new() { DropDownStyle = dropDownStyle, Size = new(width, height) };
            comboBox.Items.Add("Some test text for testing GetLineIndex method");
            comboBox.SelectedIndex = 0;
            ComboBox.ComboBoxUiaTextProvider provider = new(comboBox);

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
            using ComboBox comboBox = new() { DropDownStyle = dropDownStyle };
            comboBox.CreateControl();
            ComboBox.ComboBoxUiaTextProvider provider = new(comboBox);

            LOGFONTW expected = comboBox.Font.ToLogicalFont();
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
            using ComboBox comboBox = new() { DropDownStyle = dropDownStyle };
            ComboBox.ComboBoxUiaTextProvider provider = new(comboBox);
            LOGFONTW expected = default;

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
        using ComboBox comboBox = new() { Size = size, DropDownStyle = dropdownStyle };
        comboBox.CreateControl();
        comboBox.Items.Add(text);
        comboBox.SelectedIndex = 0;
        ComboBox.ComboBoxUiaTextProvider provider = new(comboBox);

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
            using ComboBox comboBox = new() { DropDownStyle = dropDownStyle, Size = size };
            comboBox.Items.Add(text);
            comboBox.SelectedIndex = 0;
            ComboBox.ComboBoxUiaTextProvider provider = new(comboBox);

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
        using ComboBox comboBox = new() { DropDownStyle = dropDownStyle, Size = size };
        comboBox.CreateControl();
        comboBox.Items.Add(text);
        comboBox.SelectedIndex = 0;
        ComboBox.ComboBoxUiaTextProvider provider = new(comboBox);

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
            using ComboBox comboBox = new() { DropDownStyle = dropDownStyle, Size = size };
            comboBox.Items.Add(text);
            comboBox.SelectedIndex = 0;
            ComboBox.ComboBoxUiaTextProvider provider = new(comboBox);

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
        using ComboBox comboBox = new() { DropDownStyle = dropDownStyle, Size = size };
        comboBox.CreateControl();
        ComboBox.ComboBoxUiaTextProvider provider = new(comboBox);

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
            ComboBox.ComboBoxUiaTextProvider provider = new(comboBox);

            Rectangle providerRectangle = provider.BoundingRectangle;

            Assert.Equal(Rectangle.Empty, providerRectangle);
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
        using ComboBox comboBox = new() { DropDownStyle = dropDownStyle };
        comboBox.CreateControl();
        comboBox.Items.Add(text);
        comboBox.SelectedIndex = 0;
        ComboBox.ComboBoxUiaTextProvider provider = new(comboBox);
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
            using ComboBox comboBox = new() { DropDownStyle = dropDownStyle };
            comboBox.Items.Add(text);
            comboBox.SelectedIndex = 0;
            ComboBox.ComboBoxUiaTextProvider provider = new(comboBox);

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
        using ComboBox comboBox = new() { DropDownStyle = dropDownStyle };
        comboBox.CreateControl();
        comboBox.Items.Add(text);
        comboBox.SelectedIndex = 0;
        ComboBox.ComboBoxUiaTextProvider provider = new(comboBox);

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
            using ComboBox comboBox = new() { DropDownStyle = dropDownStyle };
            comboBox.Items.Add(text);
            comboBox.SelectedIndex = 0;
            ComboBox.ComboBoxUiaTextProvider provider = new(comboBox);

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
        using ComboBox comboBox = new() { DropDownStyle = dropDownStyle };
        comboBox.CreateControl();
        ComboBox.ComboBoxUiaTextProvider provider = new(comboBox);
        WINDOW_EX_STYLE actual = provider.WindowExStyle;
        Assert.Equal((WINDOW_EX_STYLE)0, actual);
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
            using ComboBox comboBox = new() { DropDownStyle = dropDownStyle };
            ComboBox.ComboBoxUiaTextProvider provider = new(comboBox);

            WINDOW_EX_STYLE actual = provider.WindowExStyle;

            Assert.Equal(WINDOW_EX_STYLE.WS_EX_LEFT, actual);
            Assert.False(comboBox.IsHandleCreated);
            Assert.Null(comboBox.TestAccessor().Dynamic._childEdit);
        }
    }

    [WinFormsTheory]
    [InlineData(ComboBoxStyle.DropDown)]
    [InlineData(ComboBoxStyle.Simple)]
    public void ComboBoxUiaTextProvider_EditStyle_ReturnsCorrectValue(ComboBoxStyle dropDownStyle)
    {
        using ComboBox comboBox = new() { DropDownStyle = dropDownStyle };
        comboBox.CreateControl();
        ComboBox.ComboBoxUiaTextProvider provider = new(comboBox);

        WINDOW_STYLE actual = provider.WindowStyle;

        Assert.Equal(0, ((int)actual & PInvoke.ES_RIGHT));
        Assert.NotEqual(0, ((int)actual & PInvoke.ES_NOHIDESEL));
        Assert.NotEqual(0, ((int)actual & PInvoke.ES_AUTOHSCROLL));
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
            using ComboBox comboBox = new() { DropDownStyle = dropDownStyle };
            ComboBox.ComboBoxUiaTextProvider provider = new(comboBox);

            WINDOW_STYLE actual = provider.WindowStyle;

            Assert.Equal(PInvoke.ES_LEFT, (int)actual);
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
        using ComboBox comboBox = new() { DropDownStyle = dropDownStyle, Size = size };
        comboBox.CreateControl();
        comboBox.Items.Add("Some test text for testing");
        comboBox.SelectedIndex = 0;
        ComboBox.ComboBoxUiaTextProvider provider = new(comboBox);

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
            using ComboBox comboBox = new() { DropDownStyle = dropDownStyle, Size = size };
            comboBox.Items.Add("Some test text for testing");
            comboBox.SelectedIndex = 0;
            ComboBox.ComboBoxUiaTextProvider provider = new(comboBox);

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
        using ComboBox comboBox = new() { DropDownStyle = dropDownStyle, Size = size };
        comboBox.CreateControl();
        comboBox.Items.Add("Some test text for testing");
        comboBox.SelectedIndex = 0;
        ComboBox.ComboBoxUiaTextProvider provider = new(comboBox);

        using ComSafeArrayScope<ITextRangeProvider> result = new(null);
        Assert.True(provider.GetVisibleRanges(result).Succeeded);

        Assert.False(result.IsEmpty);
        Assert.True(comboBox.IsHandleCreated);
        Assert.NotNull(comboBox.TestAccessor().Dynamic._childEdit);
    }

    [WinFormsTheory]
    [MemberData(nameof(ComboBoxUiaTextProvider_GetVisibleRanges_TestData))]
    public void ComboBoxUiaTextProvider_GetVisibleRanges_ReturnsNull_WithoutHandle(ComboBoxStyle dropDownStyle, Size size)
    {
        using (new NoAssertContext())
        {
            using ComboBox comboBox = new() { DropDownStyle = dropDownStyle, Size = size };
            comboBox.Items.Add("Some test text for testing");
            comboBox.SelectedIndex = 0;
            ComboBox.ComboBoxUiaTextProvider provider = new(comboBox);

            using ComSafeArrayScope<ITextRangeProvider> result = new(null);
            Assert.True(provider.GetVisibleRanges(result).Succeeded);

            Assert.True(result.IsEmpty);
            Assert.False(comboBox.IsHandleCreated);
            Assert.Null(comboBox.TestAccessor().Dynamic._childEdit);
        }
    }

    [WinFormsTheory]
    [InlineData(ComboBoxStyle.DropDown)]
    [InlineData(ComboBoxStyle.Simple)]
    public void ComboBoxUiaTextProvider_RangeFromChild_DoesNotThrowAnException(ComboBoxStyle dropDownStyle)
    {
        using (new NoAssertContext())
        {
            using ComboBox comboBox = new() { DropDownStyle = dropDownStyle };
            ComboBox.ComboBoxUiaTextProvider provider = new(comboBox);

            // RangeFromChild doesn't throw an exception
            using ComScope<ITextRangeProvider> range = new(null);
            using var rawElementProvider = ComHelpers.GetComScope<IRawElementProviderSimple>(comboBox.AccessibilityObject);
            Assert.True(provider.RangeFromChild(rawElementProvider, range).Succeeded);
            // RangeFromChild implementation can be changed so this test can be changed too
            Assert.True(range.IsNull);
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
    public void ComboBoxUiaTextProvider_RangeFromPoint_DoesNotThrowAnException(ComboBoxStyle dropDownStyle, Point point)
    {
        using ComboBox comboBox = new() { DropDownStyle = dropDownStyle };
        comboBox.CreateControl();
        ComboBox.ComboBoxUiaTextProvider provider = new(comboBox);

        using ComScope<ITextRangeProvider> range = new(null);
        Assert.True(provider.RangeFromPoint(point, range).Succeeded);

        Assert.False(range.IsNull);
        Assert.True(comboBox.IsHandleCreated);
        Assert.NotNull(comboBox.TestAccessor().Dynamic._childEdit);
    }

    [WinFormsTheory]
    [MemberData(nameof(ComboBoxUiaTextProvider_RangeFromPoint_TestData))]
    public void ComboBoxUiaTextProvider_RangeFromPoint_ReturnsNull_WithoutHandle(ComboBoxStyle dropDownStyle, Point point)
    {
        using (new NoAssertContext())
        {
            using ComboBox comboBox = new() { DropDownStyle = dropDownStyle };
            ComboBox.ComboBoxUiaTextProvider provider = new(comboBox);

            using ComScope<ITextRangeProvider> range = new(null);
            Assert.True(provider.RangeFromPoint(point, range).Succeeded);

            Assert.True(range.IsNull);
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
        using ComboBox comboBox = new() { DropDownStyle = dropDownStyle };
        comboBox.CreateControl();
        comboBox.Items.Add("Some test text for testing");
        comboBox.SelectedIndex = 0;
        ComboBox.ComboBoxUiaTextProvider provider = new(comboBox);
        provider.SetSelection(start, end);

        using ComSafeArrayScope<ITextRangeProvider> selection = new(null);
        Assert.True(provider.GetSelection(selection).Succeeded);
        using ComScope<ITextRangeProvider> range = new(selection[0]);
        UiaTextRange textRange = ComHelpers.GetObjectForIUnknown(range) as UiaTextRange;

        Assert.False(selection.IsNull);
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
            using ComboBox comboBox = new() { DropDownStyle = dropDownStyle };
            comboBox.Items.Add("Some test text for testing");
            comboBox.SelectedIndex = 0;
            ComboBox.ComboBoxUiaTextProvider provider = new(comboBox);
            provider.SetSelection(start, end);
            Assert.False(comboBox.IsHandleCreated);
            Assert.Null(comboBox.TestAccessor().Dynamic._childEdit);

            using ComSafeArrayScope<ITextRangeProvider> selection = new(null);
            Assert.True(provider.GetSelection(selection).Succeeded);

            Assert.True(selection.IsNull);
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
    public void ComboBoxUiaTextProvider_SetSelection_DoesNotSelectText_IfIncorrectArguments(ComboBoxStyle dropDownStyle, int start, int end)
    {
        using (new NoAssertContext())
        {
            using ComboBox comboBox = new() { DropDownStyle = dropDownStyle };
            comboBox.CreateControl();
            comboBox.Items.Add("Some test text for testing");
            comboBox.SelectedIndex = 0;
            ComboBox.ComboBoxUiaTextProvider provider = new(comboBox);
            provider.SetSelection(start, end);

            using ComSafeArrayScope<ITextRangeProvider> selection = new(null);
            Assert.True(provider.GetSelection(selection).Succeeded);
            Assert.False(selection.IsEmpty);

            using ComScope<ITextRangeProvider> range = new(selection[0]);
            UiaTextRange textRange = ComHelpers.GetObjectForIUnknown(range) as UiaTextRange;

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
        using ComboBox comboBox = new() { DropDownStyle = dropDownStyle, Size = new(50, 100) };
        comboBox.CreateControl();
        comboBox.Items.Add("Some long long test text for testing GetFirstVisibleLine method");
        comboBox.SelectedIndex = 0;
        ComboBox.ComboBoxUiaTextProvider provider = new(comboBox);

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
    public void ComboBoxUiaTextProvider_LineScroll_DoesNotWork_WithoutHandle(ComboBoxStyle dropDownStyle, int newLine)
    {
        using (new NoAssertContext())
        {
            using ComboBox comboBox = new() { DropDownStyle = dropDownStyle, Size = new(50, 100) };
            comboBox.Items.Add("Some long long test text for testing GetFirstVisibleLine method");
            comboBox.SelectedIndex = 0;
            ComboBox.ComboBoxUiaTextProvider provider = new(comboBox);

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
        using ComboBox comboBox = new() { DropDownStyle = dropDownStyle, Size = new(50, 100) };
        comboBox.CreateControl();
        ComboBox.ComboBoxUiaTextProvider provider = new(comboBox);
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
            using ComboBox comboBox = new() { DropDownStyle = dropDownStyle, Size = new(50, 100) };
            ComboBox.ComboBoxUiaTextProvider provider = new(comboBox);
            Assert.False(comboBox.IsHandleCreated);
            Assert.Null(comboBox.TestAccessor().Dynamic._childEdit);
            Assert.Equal(default, provider.Logfont);
        }
    }

    [WinFormsTheory]
    [InlineData(ComboBoxStyle.DropDown)]
    [InlineData(ComboBoxStyle.Simple)]
    public void ComboBoxUiaTextProvider_FirstVisibleLine_DefaultValueCorrect(ComboBoxStyle dropDownStyle)
    {
        using ComboBox comboBox = new() { DropDownStyle = dropDownStyle, Size = new(50, 100) };
        comboBox.CreateControl();
        ComboBox.ComboBoxUiaTextProvider provider = new(comboBox);

        int actualValue = (int)PInvokeCore.SendMessage((IHandle<HWND>)comboBox.TestAccessor().Dynamic._childEdit, PInvokeCore.EM_GETFIRSTVISIBLELINE);

        Assert.Equal(actualValue, provider.FirstVisibleLine);
    }

    [WinFormsTheory]
    [InlineData(ComboBoxStyle.DropDown)]
    [InlineData(ComboBoxStyle.Simple)]
    public void ComboBoxUiaTextProvider_LinesCount_DefaultValueCorrect(ComboBoxStyle dropDownStyle)
    {
        using ComboBox comboBox = new() { DropDownStyle = dropDownStyle, Size = new(50, 100) };
        comboBox.CreateControl();
        ComboBox.ComboBoxUiaTextProvider provider = new(comboBox);

        int actualValue = (int)PInvokeCore.SendMessage((IHandle<HWND>)comboBox.TestAccessor().Dynamic._childEdit, PInvokeCore.EM_GETLINECOUNT);

        Assert.Equal(actualValue, provider.LinesCount);
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
        using ComboBox comboBox = new() { DropDownStyle = dropDownStyle, Size = new(width, height) };
        comboBox.CreateControl();
        comboBox.Items.Add("Some test text for testing GetLineFromCharIndex method");
        comboBox.SelectedIndex = 0;
        ComboBox.ComboBoxUiaTextProvider provider = new(comboBox);

        int expectedLine = (int)PInvokeCore.SendMessage((IHandle<HWND>)comboBox.TestAccessor().Dynamic._childEdit, PInvokeCore.EM_LINEFROMCHAR, (WPARAM)charIndex);
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
        using ComboBox comboBox = new() { DropDownStyle = dropDownStyle, Size = new(50, 100) };
        comboBox.CreateControl();
        comboBox.Items.Add("Some long long test text for testing GetFirstVisibleLine method");
        comboBox.SelectedIndex = 0;
        ComboBox.ComboBoxUiaTextProvider provider = new(comboBox);

        bool expectedValue = PInvokeCore.SendMessage((IHandle<HWND>)comboBox.TestAccessor().Dynamic._childEdit, PInvokeCore.EM_LINESCROLL, 0, newLine) != 0;

        Assert.Equal(expectedValue, provider.LineScroll(0, newLine));
        Assert.True(comboBox.IsHandleCreated);
        Assert.NotNull(comboBox.TestAccessor().Dynamic._childEdit);
    }
}
