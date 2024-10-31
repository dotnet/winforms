// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;
using System.Windows.Forms.Automation;
using Windows.Win32.System.Com;
using Windows.Win32.UI.Accessibility;

namespace System.Windows.Forms;

internal sealed unsafe class LabelEditUiaTextProvider : UiaTextProvider
{
    /// <summary>
    ///  Since the label edit inside the TreeView/ListView is always single-line, for optimization
    ///  we always return 0 as the index of lines
    /// </summary>
    private const int OwnerChildEditLineIndex = 0;

    /// <summary>
    ///  Since the label edit inside the TreeView/ListView is always single-line, for optimization
    ///  we always return 1 as the number of lines
    /// </summary>
    private const int OwnerChildEditLinesCount = 1;

    private readonly IHandle<HWND> _owningChildEdit;
    private readonly AccessibleObject _owningChildEditAccessibilityObject;
    private readonly WeakReference<Control> _owningControl;

    public LabelEditUiaTextProvider(Control owner, LabelEditNativeWindow childEdit, AccessibleObject childEditAccessibilityObject)
    {
        ArgumentNullException.ThrowIfNull(owner);
        _owningControl = new(owner);
        _owningChildEdit = childEdit;
        _owningChildEditAccessibilityObject = childEditAccessibilityObject.OrThrowIfNull();
    }

    public override Rectangle BoundingRectangle => GetFormattingRectangle();

    public override ITextRangeProvider* DocumentRange
        => ComHelpers.GetComPointer<ITextRangeProvider>(
            new UiaTextRange(_owningChildEditAccessibilityObject,
                this,
                start: 0,
                TextLength));

    public override int FirstVisibleLine => 0;

    public override bool IsMultiline => false;

    public override bool IsReadingRTL => WindowExStyle.HasFlag(WINDOW_EX_STYLE.WS_EX_RTLREADING);

    public override bool IsReadOnly => false;

    public override bool IsScrollable => ((int)GetWindowStyle(_owningChildEdit) & PInvoke.ES_AUTOHSCROLL) != 0;

    public override int LinesCount => OwnerChildEditLinesCount;

    public override int LinesPerPage => _owningChildEditAccessibilityObject.BoundingRectangle.IsEmpty ? 0 : OwnerChildEditLinesCount;

    public override LOGFONTW Logfont => _owningControl.TryGetTarget(out Control? target) ? target.Font.ToLogicalFont() : default;

    public override SupportedTextSelection SupportedTextSelection => SupportedTextSelection.SupportedTextSelection_Single;

    public override string Text => PInvokeCore.GetWindowText(_owningChildEdit);

    public override int TextLength => (int)PInvokeCore.SendMessage(_owningChildEdit, PInvokeCore.WM_GETTEXTLENGTH);

    public override WINDOW_EX_STYLE WindowExStyle => GetWindowExStyle(_owningChildEdit);

    public override WINDOW_STYLE WindowStyle => GetWindowStyle(_owningChildEdit);

    public override HRESULT GetCaretRange(BOOL* isActive, ITextRangeProvider** pRetVal)
    {
        if (isActive is null || pRetVal is null)
        {
            return HRESULT.E_POINTER;
        }

        object? hasKeyboardFocus = _owningChildEditAccessibilityObject.GetPropertyValue(propertyID: UIA_PROPERTY_ID.UIA_HasKeyboardFocusPropertyId);
        *isActive = hasKeyboardFocus is true;

        // First caret position of a selected text
        int start = 0;
        // Last caret position of a selected text
        int end = 0;

        // Returns info about the selected text range.
        // If there is no selection, start and end parameters are the position of the caret.
        PInvokeCore.SendMessage(_owningChildEdit, PInvokeCore.EM_GETSEL, ref start, ref end);

        *pRetVal = ComHelpers.GetComPointer<ITextRangeProvider>(new UiaTextRange(
            _owningChildEditAccessibilityObject,
            this,
            start,
            start));
        return HRESULT.S_OK;
    }

    public override int GetLineFromCharIndex(int charIndex) => OwnerChildEditLineIndex;

    public override int GetLineIndex(int line) => OwnerChildEditLineIndex;

    public override Point GetPositionFromChar(int charIndex) => GetPositionFromCharIndex(charIndex);

    // A variation on EM_POSFROMCHAR that returns the upper-right corner instead of upper-left.
    public override Point GetPositionFromCharForUpperRightCorner(int startCharIndex, string text)
    {
        if (startCharIndex < 0 || startCharIndex >= text.Length)
        {
            return Point.Empty;
        }

        char ch = text[startCharIndex];
        Point pt;

        if (char.IsControl(ch))
        {
            if (ch == '\t')
            {
                // for tabs the calculated width of the character is no help so we use the
                // UL corner of the following character if it is on the same line.
                bool useNext = startCharIndex < TextLength - 1 && GetLineFromCharIndex(startCharIndex + 1) == GetLineFromCharIndex(startCharIndex);
                return GetPositionFromCharIndex(useNext ? startCharIndex + 1 : startCharIndex);
            }

            pt = GetPositionFromCharIndex(startCharIndex);

            if (ch is '\r' or '\n')
            {
                pt.X += EndOfLineWidth; // add 2 px to show the end of line
            }

            // return the UL corner of the rest characters because these characters have no width
            return pt;
        }

        // get the UL corner of the character
        pt = GetPositionFromCharIndex(startCharIndex);

        // add the width of the character at that position.
        if (GetTextExtentPoint32(ch, out Size size))
        {
            pt.X += size.Width;
        }

        return pt;
    }

    public override HRESULT GetSelection(SAFEARRAY** pRetVal)
    {
        if (pRetVal is null)
        {
            return HRESULT.E_POINTER;
        }

        // First caret position of a selected text
        int start = 0;
        // Last caret position of a selected text
        int end = 0;

        // Returns info about the selected text range.
        // If there is no selection, start and end parameters are the position of the caret.
        PInvokeCore.SendMessage(_owningChildEdit, PInvokeCore.EM_GETSEL, ref start, ref end);

        ComSafeArrayScope<ITextRangeProvider> result = new(1);
        // Adding to the SAFEARRAY adds a reference
        using var selection = ComHelpers.GetComScope<ITextRangeProvider>(new UiaTextRange(_owningChildEditAccessibilityObject, this, start, end));
        result[0] = selection;
        *pRetVal = result;

        return HRESULT.S_OK;
    }

    public override void GetVisibleRangePoints(out int visibleStart, out int visibleEnd)
    {
        visibleStart = 0;
        visibleEnd = 0;

        if (IsDegenerate(_owningControl.TryGetTarget(out Control? target) ? target.ClientRectangle : Rectangle.Empty))
        {
            return;
        }

        Rectangle rectangle = GetFormattingRectangle();
        if (IsDegenerate(rectangle))
        {
            return;
        }

        // Formatting rectangle is the boundary, which we need to inflate by 1
        // in order to read characters within the rectangle
        Point ptStart = new(rectangle.X + 1, rectangle.Y + 1);
        Point ptEnd = new(rectangle.Right - 1, rectangle.Bottom - 1);

        visibleStart = GetCharIndexFromPosition(ptStart);
        visibleEnd = GetCharIndexFromPosition(ptEnd) + 1; // Add 1 to get a caret position after received character

        return;

        static bool IsDegenerate(Rectangle rect)
            => rect.IsEmpty || rect.Width <= 0 || rect.Height <= 0;
    }

    public override HRESULT GetVisibleRanges(SAFEARRAY** pRetVal)
    {
        if (pRetVal is null)
        {
            return HRESULT.E_POINTER;
        }

        GetVisibleRangePoints(out int start, out int end);

        ComSafeArrayScope<ITextRangeProvider> result = new(1);
        // Adding to the SAFEARRAY adds a reference
        using var ranges = ComHelpers.GetComScope<ITextRangeProvider>(new UiaTextRange(_owningChildEditAccessibilityObject, this, start, end));
        result[0] = ranges;
        *pRetVal = result;

        return HRESULT.S_OK;
    }

    public override bool LineScroll(int charactersHorizontal, int linesVertical)
        // If the EM_LINESCROLL message is sent to a single-line edit control, the return value is FALSE.
        => false;

    public override Point PointToScreen(Point pt)
    {
        PInvokeCore.MapWindowPoints(_owningChildEdit.Handle, HWND.Null, ref pt);
        return pt;
    }

    public override HRESULT RangeFromAnnotation(IRawElementProviderSimple* annotationElement, ITextRangeProvider** pRetVal)
    {
        if (pRetVal is null)
        {
            return HRESULT.E_POINTER;
        }

        *pRetVal = ComHelpers.GetComPointer<ITextRangeProvider>(
            new UiaTextRange(
                _owningChildEditAccessibilityObject,
                this,
                start: 0,
                end: 0));
        return HRESULT.S_OK;
    }

    public override HRESULT RangeFromChild(IRawElementProviderSimple* childElement, ITextRangeProvider** pRetVal)
    {
        if (pRetVal is null)
        {
            return HRESULT.E_POINTER;
        }

        // We don't have any children so this call returns null.
        Debug.Fail("Label edit control cannot have a child element.");
        *pRetVal = null;
        return HRESULT.S_OK;
    }

    public override HRESULT RangeFromPoint(UiaPoint point, ITextRangeProvider** pRetVal)
    {
        if (pRetVal is null)
        {
            return HRESULT.E_POINTER;
        }

        Point clientLocation = point;

        // Convert screen to client coordinates.
        // (Essentially ScreenToClient but MapWindowPoints accounts for window mirroring using WS_EX_LAYOUTRTL.)
        if (PInvokeCore.MapWindowPoints(HWND.Null, _owningChildEdit.Handle, ref clientLocation) == 0)
        {
            *pRetVal = ComHelpers.GetComPointer<ITextRangeProvider>(
                new UiaTextRange(
                    _owningChildEditAccessibilityObject,
                    this,
                    start: 0,
                    end: 0));
            return HRESULT.S_OK;
        }

        // We have to deal with the possibility that the coordinate is inside the window rect
        // but outside the client rect. In that case we just scoot it over so it is at the nearest
        // point in the client rect.
        RECT clientRectangle = _owningChildEditAccessibilityObject.BoundingRectangle;

        clientLocation.X = Math.Max(clientLocation.X, clientRectangle.left);
        clientLocation.X = Math.Min(clientLocation.X, clientRectangle.right);
        clientLocation.Y = Math.Max(clientLocation.Y, clientRectangle.top);
        clientLocation.Y = Math.Min(clientLocation.Y, clientRectangle.bottom);

        // Get the character at those client coordinates.
        int start = GetCharIndexFromPosition(clientLocation);

        *pRetVal = ComHelpers.GetComPointer<ITextRangeProvider>(
            new UiaTextRange(
                _owningChildEditAccessibilityObject,
                this,
                start,
                start));
        return HRESULT.S_OK;
    }

    public override Rectangle RectangleToScreen(Rectangle rect)
    {
        RECT r = rect;
        PInvokeCore.MapWindowPoints(_owningChildEdit.Handle, HWND.Null, ref r);
        return r;
    }

    public override void SetSelection(int start, int end)
    {
        if (start < 0 || start > TextLength)
        {
            Debug.Fail("SetSelection start is out of text range.");
            return;
        }

        if (end < 0 || end > TextLength)
        {
            Debug.Fail("SetSelection end is out of text range.");
            return;
        }

        PInvokeCore.SendMessage(_owningChildEdit, PInvokeCore.EM_SETSEL, (WPARAM)start, (LPARAM)end);
    }

    private int GetCharIndexFromPosition(Point pt)
    {
        int index = PARAM.LOWORD(PInvokeCore.SendMessage(_owningChildEdit, PInvokeCore.EM_CHARFROMPOS, 0, PARAM.FromPoint(pt)));

        if (index < 0)
        {
            index = 0;
        }
        else
        {
            string t = Text;
            // EM_CHARFROMPOS will return an invalid number if the last character in the RichEdit
            // is a newline.
            if (index >= t.Length)
            {
                index = Math.Max(t.Length - 1, 0);
            }
        }

        return index;
    }

    private RECT GetFormattingRectangle()
    {
        // Send an EM_GETRECT message to find out the bounding rectangle.
        RECT rectangle = default;
        PInvokeCore.SendMessage(_owningChildEdit, PInvokeCore.EM_GETRECT, 0, ref rectangle);

        return rectangle;
    }

    private Point GetPositionFromCharIndex(int index)
    {
        if (index < 0 || index >= Text.Length)
        {
            return Point.Empty;
        }

        int i = (int)PInvokeCore.SendMessage(_owningChildEdit, PInvokeCore.EM_POSFROMCHAR, (WPARAM)index);

        return new Point(PARAM.SignedLOWORD(i), PARAM.SignedHIWORD(i));
    }

    private unsafe bool GetTextExtentPoint32(char item, out Size size)
    {
        size = default;

        using GetDcScope hdc = new(_owningChildEdit.Handle);
        if (hdc.IsNull)
        {
            return false;
        }

        // Add the width of the character at that position.
        fixed (void* psizle = &size)
        {
            return PInvoke.GetTextExtentPoint32W(hdc.HDC, &item, 1, (SIZE*)psizle);
        }
    }
}
