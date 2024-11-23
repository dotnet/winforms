// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;
using System.Drawing;
using Windows.Win32.System.Com;
using Windows.Win32.System.Variant;
using Windows.Win32.UI.Accessibility;
using Windows.Win32.UI.Input.KeyboardAndMouse;

namespace System.Windows.Forms.Automation;

internal sealed unsafe class UiaTextRange : ITextRangeProvider.Interface, IManagedWrapper<ITextRangeProvider>
{
    // Edit controls always use "\r\n" as the line separator, not "\n".
    // This string is a non-localizable string.
    private const string LineSeparator = "\r\n";

    private readonly IRawElementProviderSimple.Interface _enclosingElement;
    private readonly UiaTextProvider _provider;

    private int _start;
    private int _end;

    /// <param name="start">
    ///  A caret position before the first character from a text range, not an index of an item.
    /// </param>
    /// <param name="end">
    ///  A caret position after the last character from a text range, not an index of an item.
    /// </param>
    /// <remarks>
    ///  <para>If there is a range "Test string", then start = 0, end = 11.
    ///  If start = 2 and end = 9, the range is "et stri".
    ///  If start=end, that points a caret position only, there is no any text range.</para>
    /// </remarks>
    public UiaTextRange(IRawElementProviderSimple.Interface enclosingElement, UiaTextProvider provider, int start, int end)
    {
        _enclosingElement = enclosingElement.OrThrowIfNull();
        _provider = provider.OrThrowIfNull();

        if (start > 0)
        {
            _start = start;
            _end = start;
        }

        if (end > _start)
        {
            _end = end;
        }
    }

    /// <summary>
    ///  Last caret position of this text range.
    /// </summary>
    internal int End
    {
        get => _end;
        set
        {
            // Ensure that we never accidentally get a negative index.
            _end = value < 0 ? 0 : value;

            // Ensure that end never moves before start.
            if (_end < _start)
            {
                _start = _end;
            }
        }
    }

    internal int Length
    {
        get
        {
            if (Start < 0 || End < 0 || Start > End)
            {
                return 0;
            }

            // The subtraction of caret positions returns the length of the text.
            return End - Start;
        }
    }

    /// <summary>
    ///  First caret position of this text range.
    /// </summary>
    internal int Start
    {
        get => _start;
        set
        {
            // Ensure that start never moves after end.
            if (value > _end)
            {
                _end = value;
            }

            // Ensure that we never accidentally get a negative index.
            _start = value < 0 ? 0 : value;
        }
    }

    /// <remarks>
    ///  <para>Strictly only needs to be == since never should _start &gt; _end.</para>
    /// </remarks>
    private bool IsDegenerate => _start == _end;

    HRESULT ITextRangeProvider.Interface.Clone(ITextRangeProvider** pRetVal)
    {
        if (pRetVal is null)
        {
            return HRESULT.E_POINTER;
        }

        // Ensure UiaTextRange is not disposable since _enclosingElement is being shared.
        Debug.Assert(!typeof(UiaTextRange).IsAssignableTo(typeof(IDisposable)));
        *pRetVal = ComHelpers.GetComPointer<ITextRangeProvider>(new UiaTextRange(_enclosingElement, _provider, Start, End));
        return HRESULT.S_OK;
    }

    /// <remarks>
    ///  <para>Ranges come from the same element. Only need to compare endpoints.</para>
    /// </remarks>
    HRESULT ITextRangeProvider.Interface.Compare(ITextRangeProvider* range, BOOL* pRetVal)
    {
        if (pRetVal is null)
        {
            return HRESULT.E_POINTER;
        }

        if (range is null)
        {
            return HRESULT.E_INVALIDARG;
        }

        *pRetVal = ComHelpers.TryGetObjectForIUnknown((IUnknown*)range, out UiaTextRange? editRange) && editRange.Start == Start && editRange.End == End;
        return HRESULT.S_OK;
    }

    HRESULT ITextRangeProvider.Interface.CompareEndpoints(TextPatternRangeEndpoint endpoint, ITextRangeProvider* targetRange, TextPatternRangeEndpoint targetEndpoint, int* pRetVal)
    {
        if (pRetVal is null)
        {
            return HRESULT.E_POINTER;
        }

        if (targetRange is null)
        {
            return HRESULT.E_INVALIDARG;
        }

        if (!ComHelpers.TryGetObjectForIUnknown((IUnknown*)targetRange, out UiaTextRange? editRange))
        {
            *pRetVal = -1;
            return HRESULT.E_INVALIDARG;
        }

        int e1 = (endpoint == (int)TextPatternRangeEndpoint.TextPatternRangeEndpoint_Start) ? Start : End;
        int e2 = (targetEndpoint == (int)TextPatternRangeEndpoint.TextPatternRangeEndpoint_Start) ? editRange.Start : editRange.End;

        *pRetVal = e1 - e2;
        return HRESULT.S_OK;
    }

    HRESULT ITextRangeProvider.Interface.ExpandToEnclosingUnit(TextUnit unit)
    {
        switch (unit)
        {
            case TextUnit.TextUnit_Character:
                // Leave it as it is except the case with 0-range.
                if (IsDegenerate)
                {
                    End = MoveEndpointForward(End, TextUnit.TextUnit_Character, 1, out _);
                }

                break;

            case TextUnit.TextUnit_Word:
                {
                    // Get the word boundaries.
                    string text = _provider.Text;
                    ValidateEndpoints();

                    // Move start left until we reach a word boundary.
                    while (!AtWordBoundary(text, Start))
                    {
                        Start--;
                    }

                    // Move end right until we reach word boundary (different from Start).
                    End = Math.Min(Math.Max(End, Start + 1), text.Length);

                    while (!AtWordBoundary(text, End))
                    {
                        End++;
                    }
                }

                break;

            case TextUnit.TextUnit_Line:
                {
                    if (_provider.LinesCount != 1)
                    {
                        int startLine = _provider.GetLineFromCharIndex(Start);
                        int startIndex = _provider.GetLineIndex(startLine);

                        int endLine = _provider.GetLineFromCharIndex(End);
                        int endIndex;
                        if (endLine < _provider.LinesCount - 1)
                        {
                            endLine++;
                            endIndex = _provider.GetLineIndex(endLine);
                        }
                        else
                        {
                            endIndex = _provider.TextLength;
                        }

                        MoveTo(startIndex, endIndex);
                    }
                    else
                    {
                        MoveTo(0, _provider.TextLength);
                    }
                }

                break;

            case TextUnit.TextUnit_Paragraph:
                {
                    // Get the paragraph boundaries.
                    string text = _provider.Text;
                    ValidateEndpoints();

                    // Move start left until we reach a paragraph boundary.
                    while (!AtParagraphBoundary(text, Start))
                    {
                        Start--;
                    }

                    // Move end right until we reach a paragraph boundary (different from Start).
                    End = Math.Min(Math.Max(End, Start + 1), text.Length);

                    while (!AtParagraphBoundary(text, End))
                    {
                        End++;
                    }
                }

                break;

            case TextUnit.TextUnit_Format:
            case TextUnit.TextUnit_Page:
            case TextUnit.TextUnit_Document:
                MoveTo(0, _provider.TextLength);
                break;

            default:
                return HRESULT.E_INVALIDARG;
        }

        return HRESULT.S_OK;
    }

    HRESULT ITextRangeProvider.Interface.FindAttribute(UIA_TEXTATTRIBUTE_ID attributeId, VARIANT val, BOOL backward, ITextRangeProvider** pRetVal)
    {
        if (pRetVal is null)
        {
            return HRESULT.E_POINTER;
        }

        *pRetVal = null;
        return HRESULT.S_OK;
    }

    HRESULT ITextRangeProvider.Interface.FindText(BSTR text, BOOL backward, BOOL ignoreCase, ITextRangeProvider** pRetVal)
    {
        if (pRetVal is null)
        {
            return HRESULT.E_POINTER;
        }

        if (text.IsNull)
        {
            Debug.Fail("Invalid text range argument. 'text' should not be null.");
            *pRetVal = null;
            return HRESULT.E_INVALIDARG;
        }

        if (text.Length == 0)
        {
            Debug.Fail("Invalid text range argument. 'text' length should be more than 0.");
            *pRetVal = null;
            return HRESULT.E_INVALIDARG;
        }

        ValidateEndpoints();
        ReadOnlySpan<char> rangeText = _provider.Text.AsSpan().Slice(Start, Length);
        StringComparison comparisonType = ignoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal;

        // Do a case-sensitive search for the text inside the range.
        int index = backward ? rangeText.LastIndexOf(text, comparisonType) : rangeText.IndexOf(text, comparisonType);

        // If the text was found then create a new range covering the found text.
        *pRetVal = index >= 0
            ? ComHelpers.GetComPointer<ITextRangeProvider>(new UiaTextRange(_enclosingElement, _provider, Start + index, Start + index + text.Length))
            : null;
        return HRESULT.S_OK;
    }

    HRESULT ITextRangeProvider.Interface.GetAttributeValue(UIA_TEXTATTRIBUTE_ID attributeId, VARIANT* pRetVal)
    {
        if (pRetVal is null)
        {
            return HRESULT.E_POINTER;
        }

        *pRetVal = GetAttributeValue(attributeId);
        return HRESULT.S_OK;
    }

    HRESULT ITextRangeProvider.Interface.GetBoundingRectangles(SAFEARRAY** pRetVal)
    {
        if (pRetVal is null)
        {
            return HRESULT.E_POINTER;
        }

        VARIANT result = default;
        _enclosingElement.GetPropertyValue(UIA_PROPERTY_ID.UIA_BoundingRectanglePropertyId, &result).ThrowOnFailure();
        if (!result.vt.HasFlag(VARENUM.VT_ARRAY & VARENUM.VT_R8)
            || result.data.parray->VarType is not VARENUM.VT_R8
            || result.data.parray->GetBounds().cElements != 4)
        {
            result.Dispose();
            *pRetVal = SAFEARRAY.CreateEmpty(VARENUM.VT_R8);
            return HRESULT.S_OK;
        }

        SafeArrayScope<double> ownerBounds = new(result.data.parray);

        // We accumulate rectangles onto a list.
        List<Rectangle> rectangles = [];
        string text = _provider.Text;

        if (text.Length == 0)
        {
            *pRetVal = ownerBounds;
            return HRESULT.S_OK;
        }

        // If this is an end of a line.
        if (Start == _provider.TextLength
            || (_provider.IsMultiline && End < _provider.TextLength
                && End - Start == 1 && text[End] == '\n'))
        {
            PInvoke.GetCaretPos(out Point endlinePoint);
            endlinePoint = _provider.PointToScreen(endlinePoint);
            Rectangle endlineRectangle = new(endlinePoint.X, endlinePoint.Y + 2, UiaTextProvider.EndOfLineWidth, Math.Abs(_provider.Logfont.lfHeight) + 1);
            *pRetVal = UiaTextProvider.BoundingRectangleAsArray(endlineRectangle);
            ownerBounds.Dispose();
            return HRESULT.S_OK;
        }

        // Return zero rectangles for a degenerate-range. We don't return an empty,
        // but properly positioned, rectangle for degenerate ranges.
        if (IsDegenerate)
        {
            *pRetVal = SAFEARRAY.CreateEmpty(VARENUM.VT_R8);
            ownerBounds.Dispose();
            return HRESULT.S_OK;
        }

        ValidateEndpoints();

        // Get the mapping from client coordinates to screen coordinates.
        Point mapClientToScreen = new((int)ownerBounds[0], (int)ownerBounds[1]);
        ownerBounds.Dispose();

        // Clip the rectangles to the edit control's formatting rectangle.
        Rectangle clippingRectangle = _provider.BoundingRectangle;

        if (_provider.IsMultiline)
        {
            rectangles = GetMultilineBoundingRectangles(clippingRectangle);
            *pRetVal = UiaTextProvider.RectListToDoubleArray(rectangles);
            return HRESULT.S_OK;
        }

        // Figure out the rectangle for this one line.
        Point startPoint = _provider.GetPositionFromChar(Start);
        Point endPoint = _provider.GetPositionFromCharForUpperRightCorner(End - 1, text);

        // Add 2 to Y to get a correct size of a rectangle around a range
        Rectangle rectangle = new(startPoint.X, startPoint.Y + 2, endPoint.X - startPoint.X, clippingRectangle.Height);
        rectangle.Intersect(clippingRectangle);

        if (rectangle.Width > 0 && rectangle.Height > 0)
        {
            rectangle.Offset(mapClientToScreen.X, mapClientToScreen.Y);
            rectangles.Add(rectangle);
        }

        *pRetVal = UiaTextProvider.RectListToDoubleArray(rectangles);
        return HRESULT.S_OK;
    }

    HRESULT ITextRangeProvider.Interface.GetEnclosingElement(IRawElementProviderSimple** pRetVal)
    {
        if (pRetVal is null)
        {
            return HRESULT.E_POINTER;
        }

        *pRetVal = ComHelpers.GetComPointer<IRawElementProviderSimple>(_enclosingElement);
        return HRESULT.S_OK;
    }

    HRESULT ITextRangeProvider.Interface.GetText(int maxLength, BSTR* pRetVal)
    {
        if (pRetVal is null)
        {
            return HRESULT.E_POINTER;
        }

        if (maxLength == -1)
        {
            maxLength = End + 1;
        }

        string text = _provider.Text;
        ValidateEndpoints();
        maxLength = maxLength >= 0 ? Math.Min(Length, maxLength) : Length;

        *pRetVal = text.Length < maxLength - Start
            ? new(text[Start..])
            : new(text.Substring(Start, maxLength));
        return HRESULT.S_OK;
    }

    HRESULT ITextRangeProvider.Interface.Move(TextUnit unit, int count, int* pRetVal)
    {
        if (pRetVal is null)
        {
            return HRESULT.E_POINTER;
        }

        // Positive count means move forward. Negative count means move backwards.
        int moved;

        if (count > 0)
        {
            // If the range is non-degenerate then we need to collapse the range.
            // (See the discussion of Count for ITextRange::Move)
            if (!IsDegenerate)
            {
                // If the count is greater than zero, collapse the range at its end point.
                Start = End;
            }

            // Move the degenerate range forward by the number of units.
            int start = Start;
            Start = MoveEndpointForward(Start, unit, count, out moved);

            // If the start did not change then no move was done.
            if (start != Start)
            {
                *pRetVal = moved;
                return HRESULT.S_OK;
            }
        }

        if (count < 0)
        {
            // If the range is non-degenerate then we need to collapse the range.
            if (!IsDegenerate)
            {
                // If the count is less than zero, collapse the range at the starting point.
                End = Start;
            }

            // Move the degenerate range backward by the number of units.
            int end = End;
            End = MoveEndpointBackward(End, unit, count, out moved);

            // If the end did not change then no move was done.
            if (end != End)
            {
                *pRetVal = moved;
                return HRESULT.S_OK;
            }
        }

        // Moving zero of any unit has no effect.
        *pRetVal = 0;
        return HRESULT.S_OK;
    }

    HRESULT ITextRangeProvider.Interface.MoveEndpointByUnit(TextPatternRangeEndpoint endpoint, TextUnit unit, int count, int* pRetVal)
    {
        if (pRetVal is null)
        {
            return HRESULT.E_POINTER;
        }

        // Positive count means move forward. Negative count means move backwards.
        bool moveStart = endpoint == TextPatternRangeEndpoint.TextPatternRangeEndpoint_Start;
        int moved;
        int start = Start;
        int end = End;

        if (count > 0)
        {
            if (moveStart)
            {
                Start = MoveEndpointForward(Start, unit, count, out moved);

                // If the start did not change then no move was done.
                *pRetVal = start == Start ? 0 : moved;
                return HRESULT.S_OK;
            }

            End = MoveEndpointForward(End, unit, count, out moved);

            // If the end did not change then no move was done.
            *pRetVal = end == End ? 0 : moved;
            return HRESULT.S_OK;
        }

        if (count < 0)
        {
            if (moveStart)
            {
                Start = MoveEndpointBackward(Start, unit, count, out moved);

                // If the start did not change then no move was done.
                *pRetVal = start == Start ? 0 : moved;
                return HRESULT.S_OK;
            }

            End = MoveEndpointBackward(End, unit, count, out moved);

            // If the end did not change then no move was done.
            *pRetVal = end == End ? 0 : moved;
            return HRESULT.S_OK;
        }

        // Moving zero of any unit has no effect.
        *pRetVal = 0;
        return HRESULT.S_OK;
    }

    HRESULT ITextRangeProvider.Interface.MoveEndpointByRange(TextPatternRangeEndpoint endpoint, ITextRangeProvider* targetRange, TextPatternRangeEndpoint targetEndpoint)
    {
        if (!ComHelpers.TryGetObjectForIUnknown((IUnknown*)targetRange, out UiaTextRange? textRange))
        {
            return HRESULT.E_INVALIDARG;
        }

        int e = (targetEndpoint == TextPatternRangeEndpoint.TextPatternRangeEndpoint_Start)
            ? textRange.Start
            : textRange.End;

        if (endpoint == TextPatternRangeEndpoint.TextPatternRangeEndpoint_Start)
        {
            Start = e;
        }
        else
        {
            End = e;
        }

        return HRESULT.S_OK;
    }

    HRESULT ITextRangeProvider.Interface.Select()
    {
        _provider.SetSelection(Start, End);
        return HRESULT.S_OK;
    }

    HRESULT ITextRangeProvider.Interface.AddToSelection() => HRESULT.S_OK;

    HRESULT ITextRangeProvider.Interface.RemoveFromSelection() => HRESULT.S_OK;

    HRESULT ITextRangeProvider.Interface.ScrollIntoView(BOOL alignToTop)
    {
        if (_provider.IsMultiline)
        {
            int newFirstLine = alignToTop
                ? _provider.GetLineFromCharIndex(Start)
                : Math.Max(0, _provider.GetLineFromCharIndex(End) - _provider.LinesPerPage + 1);

            _provider.LineScroll(Start, newFirstLine - _provider.FirstVisibleLine);

            return HRESULT.S_OK;
        }

        if (_provider.IsScrollable)
        {
            _provider.GetVisibleRangePoints(out int visibleStart, out int visibleEnd);
            VIRTUAL_KEY key = Start > visibleStart ? VIRTUAL_KEY.VK_RIGHT : VIRTUAL_KEY.VK_LEFT;

            if (_provider.IsReadingRTL)
            {
                if (Start > visibleStart || Start < visibleEnd)
                {
                    UiaTextProvider.SendKeyboardInputVK(key, true);
                    _provider.GetVisibleRangePoints(out _, out _);
                }

                return HRESULT.S_OK;
            }

            if (Start < visibleStart || Start > visibleEnd)
            {
                UiaTextProvider.SendKeyboardInputVK(key, true);
                _provider.GetVisibleRangePoints(out _, out _);
            }
        }

        return HRESULT.S_OK;
    }

    HRESULT ITextRangeProvider.Interface.GetChildren(SAFEARRAY** pRetVal)
    {
        if (pRetVal is null)
        {
            return HRESULT.E_POINTER;
        }

        *pRetVal = SAFEARRAY.CreateEmpty(VARENUM.VT_UNKNOWN);
        return HRESULT.S_OK;
    }

    /// <remark>
    ///  Returns true if index identifies a paragraph boundary within text.
    /// </remark>
    private static bool AtParagraphBoundary(string text, int index)
        => string.IsNullOrWhiteSpace(text) || index <= 0 || index >= text.Length || ((text[index - 1] == '\n') && (text[index] != '\n'));

    private static bool AtWordBoundary(string text, int index)
    {
        // Returns true if index identifies a word boundary within text.
        // Following richedit & word precedent the boundaries are at the leading edge of the word
        // so the span of a word includes trailing whitespace.

        // We are at a word boundary if we are at the beginning or end of the text.
        if (string.IsNullOrWhiteSpace(text) || index <= 0 || index >= text.Length || AtParagraphBoundary(text, index))
        {
            return true;
        }

        char ch1 = text[index - 1];
        char ch2 = text[index];

        // An apostrophe does *not* break a word if it follows or precedes characters.
        if ((char.IsLetterOrDigit(ch1) && IsApostrophe(ch2))
            || (IsApostrophe(ch1) && char.IsLetterOrDigit(ch2) && index >= 2 && char.IsLetterOrDigit(text[index - 2])))
        {
            return false;
        }

        // The following transitions mark boundaries.
        // Note: these are constructed to include trailing whitespace.
        return (char.IsWhiteSpace(ch1) && !char.IsWhiteSpace(ch2))
            || (char.IsLetterOrDigit(ch1) && !char.IsLetterOrDigit(ch2))
            || (!char.IsLetterOrDigit(ch1) && char.IsLetterOrDigit(ch2))
            || (char.IsPunctuation(ch1) && char.IsWhiteSpace(ch2));
    }

    private static bool IsApostrophe(char ch) => ch is '\'' or ((char)0x2019); // Unicode Right Single Quote Mark

    /// <devdoc>
    ///  Attribute values and their types are defined here -
    ///  https://learn.microsoft.com/windows/win32/winauto/uiauto-textattribute-ids
    /// </devdoc>
    private VARIANT GetAttributeValue(UIA_TEXTATTRIBUTE_ID textAttributeIdentifier)
    {
        object? value = textAttributeIdentifier switch
        {
            UIA_TEXTATTRIBUTE_ID.UIA_BackgroundColorAttributeId => GetBackgroundColor(),
            UIA_TEXTATTRIBUTE_ID.UIA_CapStyleAttributeId => GetCapStyle(_provider.WindowStyle),
            UIA_TEXTATTRIBUTE_ID.UIA_FontNameAttributeId => GetFontName(_provider.Logfont),
            UIA_TEXTATTRIBUTE_ID.UIA_FontSizeAttributeId => GetFontSize(_provider.Logfont),
            UIA_TEXTATTRIBUTE_ID.UIA_FontWeightAttributeId => GetFontWeight(_provider.Logfont),
            UIA_TEXTATTRIBUTE_ID.UIA_ForegroundColorAttributeId => GetForegroundColor(),
            UIA_TEXTATTRIBUTE_ID.UIA_HorizontalTextAlignmentAttributeId => GetHorizontalTextAlignment(_provider.WindowStyle),
            UIA_TEXTATTRIBUTE_ID.UIA_IsItalicAttributeId => GetItalic(_provider.Logfont),
            UIA_TEXTATTRIBUTE_ID.UIA_IsReadOnlyAttributeId => GetReadOnly(),
            UIA_TEXTATTRIBUTE_ID.UIA_StrikethroughStyleAttributeId => GetStrikethroughStyle(_provider.Logfont),
            UIA_TEXTATTRIBUTE_ID.UIA_UnderlineStyleAttributeId => GetUnderlineStyle(_provider.Logfont),
            _ => null
        };

#if DEBUG
        if (value?.GetType() is { } type && type.IsValueType && !type.IsPrimitive && !type.IsEnum)
        {
            // Check to make sure we can actually convert this to a VARIANT, throw otherwise.
            using VARIANT variant = default;
            unsafe
            {
                Runtime.InteropServices.Marshal.GetNativeVariantForObject(value, (nint)(void*)&variant);
            }
        }
#endif

        return value is null ? UiaGetReservedNotSupportedValue() : VARIANT.FromObject(value);
    }

    /// <summary>
    ///  Helper function to accumulate a list of bounding rectangles for a potentially mult-line range.
    /// </summary>
    private List<Rectangle> GetMultilineBoundingRectangles(Rectangle clippingRectangle)
    {
        // Get the starting and ending lines for the range.
        int start = Start;
        int end = End;

        int startLine = _provider.GetLineFromCharIndex(start);
        int endLine = _provider.GetLineFromCharIndex(end - 1);

        // Adjust the start based on the first visible line.
        int firstVisibleLine = _provider.FirstVisibleLine;
        if (firstVisibleLine > startLine)
        {
            startLine = firstVisibleLine;
            start = _provider.GetLineIndex(startLine);
        }

        // Adjust the end based on the last visible line.
        int lastVisibleLine = firstVisibleLine + _provider.LinesPerPage - 1;
        if (lastVisibleLine < endLine)
        {
            endLine = lastVisibleLine;
            end = _provider.GetLineIndex(endLine + 1); // Index of the next line is the end caret position of the previous line.
        }

        // Remember the line height.
        int lineHeight = Math.Abs(_provider.Logfont.lfHeight);

        string text = _provider.Text;
        // Adding a rectangle for each line.
        List<Rectangle> rects = [];

        for (int lineIndex = startLine; lineIndex <= endLine; lineIndex++)
        {
            // Get the left text position in the line.
            int lineStartIndex = lineIndex == startLine ? start : _provider.GetLineIndex(lineIndex);
            Point lineStartPoint = _provider.GetPositionFromChar(lineStartIndex);

            // Get the right text position in the line.
            int lineEndIndex = lineIndex == endLine
                // Just take the end of the range for the last line.
                // `end` is a caret position after the last character in the range,
                // so subtract 1 to get the last character index.
                ? end - 1
                // Or get the first index of the next line and take the previous character.
                // This is a workaround to get the last index of the line,
                // because Windows doesn't provide API for it.
                : _provider.GetLineIndex(lineIndex + 1) - 1;

            Point lineEndPoint = _provider.GetPositionFromCharForUpperRightCorner(lineEndIndex, text);

            if (!_provider.IsReadingRTL)
            {
                // Don't need additional calculations for LeftToRight edit field.
                AddLineRectangle(lineStartPoint, lineEndPoint);
                continue;
            }

            // Windows provides incorrect coordinates in several cases
            // for RightToLeft edit fields. So adjust it.

            // This value can be negative for a RTL edit field, because Windows
            // may provide incorrect start and end point for some broken lines.
            int lineTextLength = lineEndIndex - lineStartIndex + 1;

            // If the line is empty (just contains transition to the next line),
            // we need to offset the start point on end of line width (equals 2 px)
            // to show its rectangle in RTL mode. For LTR mode
            // `GetPositionFromCharForUpperRightCorner` do it for us.
            // Also, we have to check the line text length, because Windows may provide
            // incorrect endpoints for RTL TextBox, in this case we will catch an exception.
            if (lineTextLength > 0
                && (text.Substring(lineStartIndex, lineTextLength) == Environment.NewLine
                    // Or if the range takes more space, than owning control rectangle.
                    // One of the case is when there is a RTL line divided by space (not '\n').
                    || lineEndPoint.X > clippingRectangle.Right))
            {
                lineStartPoint.X -= UiaTextProvider.EndOfLineWidth;
                AddLineRectangle(lineStartPoint, lineEndPoint);

                continue;
            }

            // If Windows provided incorrect coordinates for endpoints in RTL mode.
            if (lineEndPoint.X <= lineStartPoint.X && lineTextLength > 0)
            {
                if (string.IsNullOrWhiteSpace(text.Substring(lineStartIndex, lineTextLength)))
                {
                    // If the line contains whitespaces only, they are in RTL order,
                    // so just swap start and end points, taken from Windows.
                    (lineStartPoint, lineEndPoint) = (lineEndPoint, lineStartPoint);
                    AddLineRectangle(lineStartPoint, lineEndPoint);

                    continue;
                }

                // TextBox in RTL mode may have incorrect character display order,
                // in this case the caret "jumps" between characters in the line
                // instead of direct moving through them. In this case Windows provides
                // incorrect characters indexes and their positions in the line.
                // This is a bug or the native control, because the caret "jums"
                // when selecting and the selected text is torn. Moreover, the caret position
                // is incorrect for some whitespaces, thereby Windows provides incorrect text range endpoints.
                // There are 2 ways to get rectangles:
                //    1) Go through all characters in the line and get min and max positions.
                //       (Used now)
                //    2) Take the owning control rectangle width for the line text range.
                //       (Fast and simple)
                //       lineStartPoint.X = clippingRectangle.Left;
                //       lineEndPoint.X = clippingRectangle.Right;

                // Use the "end" point as min, because it is less than the "start" point for this case.
                int minX = lineEndPoint.X;
                int maxX = minX;

                // Go through all characters in the line and get min and max positions.
                for (int i = lineStartIndex; i <= lineEndIndex; i++)
                {
                    Point pt = _provider.GetPositionFromChar(i);
                    if (pt.X < minX)
                    {
                        minX = pt.X;
                        continue;
                    }

                    pt = _provider.GetPositionFromCharForUpperRightCorner(i, text);
                    if (pt.X > maxX)
                    {
                        maxX = pt.X;
                    }
                }

                lineStartPoint.X = minX;
                lineEndPoint.X = maxX;
            }

            AddLineRectangle(lineStartPoint, lineEndPoint);
        }

        return rects;

        void AddLineRectangle(Point startPoint, Point endPoint)
        {
            // Add a bounding rectangle for a line, if it's nonempty.
            // Increase Y by 2 to Y to get a correct size of the rectangle around a text range.
            // Adding 2px constant to Y doesn't affect rectangles in a high DPI mode,
            // because it just moves the rectangle down a TextBox border, that is 1 px in all DPI modes.
            Rectangle rect = new(startPoint.X, startPoint.Y + 2, endPoint.X - startPoint.X, lineHeight);
            rect.Intersect(clippingRectangle);
            if (rect.Width > 0 && rect.Height > 0)
            {
                rect = _provider.RectangleToScreen(rect);
                rects.Add(rect);
            }
        }
    }

    private static int GetBackgroundColor() => (int)PInvoke.GetSysColor(SYS_COLOR_INDEX.COLOR_WINDOW);

    private static int GetCapStyle(WINDOW_STYLE windowStyle)
        => (int)(((int)windowStyle & PInvoke.ES_UPPERCASE) != 0 ? CapStyle.AllCap : CapStyle.None);

    private static string GetFontName(LOGFONTW logfont) => logfont.FaceName.ToString();

    private static double GetFontSize(LOGFONTW logfont)
    {
        // Note: this assumes integral point sizes. violating this assumption would confuse the user
        // because they set something to 7 point but reports that it is, say 7.2 point, due to the rounding.
        using var dc = GetDcScope.ScreenDC;
        int lpy = PInvokeCore.GetDeviceCaps(dc, GET_DEVICE_CAPS_INDEX.LOGPIXELSY);
        return Math.Round((double)(-logfont.lfHeight) * 72 / lpy);
    }

    private static int GetFontWeight(LOGFONTW logfont) => logfont.lfWeight;

    private static int GetForegroundColor() => (int)PInvoke.GetSysColor(SYS_COLOR_INDEX.COLOR_WINDOWTEXT);

    private static int GetHorizontalTextAlignment(WINDOW_STYLE windowStyle)
        => (int)(((int)windowStyle & PInvoke.ES_CENTER) != 0
            ? HorizontalTextAlignment.Centered
            : ((int)windowStyle & PInvoke.ES_RIGHT) != 0
                ? HorizontalTextAlignment.Right
                : HorizontalTextAlignment.Left);

    private static bool GetItalic(LOGFONTW logfont) => logfont.lfItalic != 0;

    private bool GetReadOnly() => _provider.IsReadOnly;

    private static int GetStrikethroughStyle(LOGFONTW logfont)
        => (int)(logfont.lfStrikeOut != 0 ? TextDecorationLineStyle.Single : TextDecorationLineStyle.None);

    private static int GetUnderlineStyle(LOGFONTW logfont)
        => (int)(logfont.lfUnderline != 0 ? TextDecorationLineStyle.Single : TextDecorationLineStyle.None);

    private static VARIANT UiaGetReservedNotSupportedValue()
    {
        IUnknown* unknown;
        PInvoke.UiaGetReservedNotSupportedValue(&unknown).ThrowOnFailure();
        return new VARIANT()
        {
            vt = VARENUM.VT_UNKNOWN,
            data = new() { punkVal = unknown }
        };
    }

    /// <summary>
    ///  Moves an endpoint forward a certain number of units.
    /// </summary>
    private int MoveEndpointForward(int index, TextUnit unit, int count, out int moved)
    {
        switch (unit)
        {
            case TextUnit.TextUnit_Character:
                {
                    int limit = _provider.TextLength;
                    ValidateEndpoints();

                    moved = Math.Min(count, limit - index);
                    index += moved;

                    index = index > limit ? limit : index;
                }

                break;

            case TextUnit.TextUnit_Word:
                {
                    string text = _provider.Text;
                    ValidateEndpoints();
                    moved = 0;

                    while (moved < count && index < text.Length)
                    {
                        index++;

                        while (!AtWordBoundary(text, index))
                        {
                            index++;
                        }

                        moved++;
                    }
                }

                break;

            case TextUnit.TextUnit_Line:
                {
                    // Figure out what line we are on. If we are in the middle of a line and
                    // are moving left then we'll round up to the next line so that we move
                    // to the beginning of the current line.
                    int line = _provider.GetLineFromCharIndex(index);

                    // Limit the number of lines moved to the number of lines available to move
                    // Note lineMax is always >= 1.
                    int lineMax = _provider.LinesCount;
                    moved = Math.Min(count, lineMax - line - 1);

                    if (moved > 0)
                    {
                        // move the endpoint to the beginning of the destination line.
                        index = _provider.GetLineIndex(line + moved);
                    }
                    else if (moved == 0 && lineMax == 1)
                    {
                        // There is only one line so get the text length as endpoint.
                        index = _provider.TextLength;
                        moved = 1;
                    }
                }

                break;

            case TextUnit.TextUnit_Paragraph:
                {
                    // Just like moving words but we look for paragraph boundaries instead of
                    // word boundaries.
                    string text = _provider.Text;
                    ValidateEndpoints();
                    moved = 0;

                    while (moved < count && index < text.Length)
                    {
                        index++;

                        while (!AtParagraphBoundary(text, index))
                        {
                            index++;
                        }

                        moved++;
                    }
                }

                break;

            case TextUnit.TextUnit_Format:
            case TextUnit.TextUnit_Page:
            case TextUnit.TextUnit_Document:
                {
                    // Since edit controls are plain text moving one uniform format unit will
                    // take us all the way to the end of the document, just like
                    // "pages" and document.
                    int limit = _provider.TextLength;
                    ValidateEndpoints();

                    // We'll move 1 format unit if we aren't already at the end of the
                    // document. Otherwise, we won't move at all.
                    moved = index < limit ? 1 : 0;
                    index = limit;
                }

                break;

            default:
                throw new InvalidEnumArgumentException(nameof(unit), (int)unit, typeof(TextUnit));
        }

        return index;
    }

    /// <summary>
    ///  Moves an endpoint backward a certain number of units.
    /// </summary>
    private int MoveEndpointBackward(int index, TextUnit unit, int count, out int moved)
    {
        switch (unit)
        {
            case TextUnit.TextUnit_Character:
                {
                    ValidateEndpoints();
                    int oneBasedIndex = index + 1;
                    moved = Math.Max(count, -oneBasedIndex);
                    index += moved;
                    index = index < 0 ? 0 : index;
                }

                break;

            case TextUnit.TextUnit_Word:
                {
                    string text = _provider.Text;
                    ValidateEndpoints();

                    for (moved = 0; moved > count && index > 0; moved--)
                    {
                        index--;

                        while (!AtWordBoundary(text, index))
                        {
                            index--;
                        }
                    }
                }

                break;

            case TextUnit.TextUnit_Line:
                {
                    // Note count < 0.

                    // Get 1-based line.
                    int line = _provider.GetLineFromCharIndex(index) + 1;

                    int lineMax = _provider.LinesCount;

                    // Truncate the count to the number of available lines.
                    int actualCount = Math.Max(count, -line);

                    moved = actualCount;

                    if (actualCount == -line)
                    {
                        // We are moving by the maximum number of possible lines,
                        // so we know the resulting index will be 0.
                        index = 0;

                        // If a line other than the first consists of only "\r\n",
                        // you can move backwards past this line and the position changes,
                        // hence this is counted. The first line is special, though:
                        // if it is empty, and you move say from the second line back up
                        // to the first, you cannot move further; however if the first line
                        // is nonempty, you can move from the end of the first line to its
                        // beginning!  This latter move is counted, but if the first line
                        // is empty, it is not counted.

                        // Recalculate the value of "moved".
                        // The first line is empty if it consists only of
                        // a line separator sequence.
                        bool firstLineEmpty = (lineMax == 0 || (lineMax > 1 && _provider.GetLineIndex(1) == LineSeparator.Length));
                        if (moved < 0 && firstLineEmpty)
                        {
                            ++moved;
                        }
                    }
                    else // actualCount > -line
                    {
                        // Move the endpoint to the beginning of the following line,
                        // then back by the line separator length to get to the end
                        // of the previous line, since the Edit control has
                        // no method to get the character index of the end
                        // of a line directly.
                        index = _provider.GetLineIndex(line + actualCount) - LineSeparator.Length;
                    }
                }

                break;

            case TextUnit.TextUnit_Paragraph:
                {
                    // Just like moving words but we look for paragraph boundaries instead of
                    // word boundaries.
                    string text = _provider.Text;
                    ValidateEndpoints();

                    for (moved = 0; moved > count && index > 0; moved--)
                    {
                        index--;

                        while (!AtParagraphBoundary(text, index))
                        {
                            index--;
                        }
                    }
                }

                break;

            case TextUnit.TextUnit_Format:
            case TextUnit.TextUnit_Page:
            case TextUnit.TextUnit_Document:
                {
                    // Since edit controls are plain text moving one uniform format unit will
                    // take us all the way to the beginning of the document, just like
                    // "pages" and document.

                    // We'll move 1 format unit if we aren't already at the beginning of the
                    // document. Otherwise, we won't move at all.
                    moved = index > 0 ? -1 : 0;
                    index = 0;
                }

                break;

            default:
                throw new InvalidEnumArgumentException(nameof(unit), (int)unit, typeof(TextUnit));
        }

        return index;
    }

    /// <summary>
    ///  Method to set both endpoints simultaneously.
    /// </summary>
    private void MoveTo(int start, int end)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(start);
        ArgumentOutOfRangeException.ThrowIfLessThan(end, start);

        _start = start;
        _end = end;
    }

    private void ValidateEndpoints()
    {
        int limit = _provider.TextLength;

        if (Start > limit && limit > 0)
        {
            Start = limit;
        }

        if (End > limit && limit > 0)
        {
            End = limit;
        }
    }
}
