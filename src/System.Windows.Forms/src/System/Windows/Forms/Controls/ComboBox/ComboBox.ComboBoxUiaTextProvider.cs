// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;
using System.Windows.Forms.Automation;
using Windows.Win32.System.Com;
using Windows.Win32.System.Variant;
using Windows.Win32.UI.Accessibility;

namespace System.Windows.Forms;

public partial class ComboBox
{
    internal sealed unsafe class ComboBoxUiaTextProvider : UiaTextProvider
    {
        /// <summary>
        ///  Since the TextBox inside the ComboBox is always single-line, for optimization
        ///  we always return 0 as the index of lines
        /// </summary>
        private const int OwnerChildEditLineIndex = 0;

        /// <summary>
        ///  Since the TextBox inside the ComboBox is always single-line, for optimization
        ///  we always return 1 as the number of lines
        /// </summary>
        private const int OwnerChildEditLinesCount = 1;

        private readonly IHandle<HWND> _owningChildEdit;

        private readonly ComboBox _owningComboBox;

        public ComboBoxUiaTextProvider(ComboBox owner)
        {
            _owningComboBox = owner.OrThrowIfNull();
            Debug.Assert(_owningComboBox.IsHandleCreated);

            _owningChildEdit = owner._childEdit!;
        }

        public override Rectangle BoundingRectangle
            => _owningComboBox.IsHandleCreated
                ? GetFormattingRectangle()
                : Rectangle.Empty;

        public override ITextRangeProvider* DocumentRange
            => ComHelpers.GetComPointer<ITextRangeProvider>(
                new UiaTextRange(
                    _owningComboBox.ChildEditAccessibleObject,
                    this,
                    start:
                    0,
                    TextLength));

        public override int FirstVisibleLine
            => _owningComboBox.IsHandleCreated
                ? 0
                : -1;

        public override bool IsMultiline => false;

        public override bool IsReadingRTL
            => _owningComboBox.IsHandleCreated && WindowExStyle.HasFlag(WINDOW_EX_STYLE.WS_EX_RTLREADING);

        public override bool IsReadOnly => false;

        public override bool IsScrollable
        {
            get
            {
                if (!_owningComboBox.IsHandleCreated)
                {
                    return false;
                }

                return ((int)GetWindowStyle(_owningChildEdit) & PInvoke.ES_AUTOHSCROLL) != 0;
            }
        }

        public override int LinesCount
            => _owningComboBox.IsHandleCreated
                ? OwnerChildEditLinesCount
                : -1;

        public override int LinesPerPage
            => !_owningComboBox.IsHandleCreated
                    ? -1
                    : _owningComboBox.ChildEditAccessibleObject.BoundingRectangle.IsEmpty
                        ? 0
                        : OwnerChildEditLinesCount;

        public override LOGFONTW Logfont
            => _owningComboBox.IsHandleCreated
                ? _owningComboBox.Font.ToLogicalFont()
                : default;

        public override SupportedTextSelection SupportedTextSelection => SupportedTextSelection.SupportedTextSelection_Single;

        public override string Text
            => _owningComboBox.IsHandleCreated
                ? PInvokeCore.GetWindowText(_owningChildEdit)
                : string.Empty;

        public override int TextLength
            => _owningComboBox.IsHandleCreated
                ? (int)PInvokeCore.SendMessage(_owningChildEdit, PInvokeCore.WM_GETTEXTLENGTH)
                : -1;

        public override WINDOW_EX_STYLE WindowExStyle
            => _owningComboBox.IsHandleCreated
                ? GetWindowExStyle(_owningChildEdit)
                : WINDOW_EX_STYLE.WS_EX_LEFT;

        public override WINDOW_STYLE WindowStyle
            => _owningComboBox.IsHandleCreated
                ? GetWindowStyle(_owningChildEdit)
                : WINDOW_STYLE.WS_OVERLAPPED;

        public override HRESULT GetCaretRange(BOOL* isActive, ITextRangeProvider** pRetVal)
        {
            if (pRetVal is null || isActive is null)
            {
                return HRESULT.E_POINTER;
            }

            *isActive = false;

            if (!_owningComboBox.IsHandleCreated)
            {
                *pRetVal = null;
                return HRESULT.S_OK;
            }

            object? hasKeyboardFocus = _owningComboBox.ChildEditAccessibleObject.GetPropertyValue(UIA_PROPERTY_ID.UIA_HasKeyboardFocusPropertyId);
            *isActive = hasKeyboardFocus is true;

            *pRetVal = ComHelpers.GetComPointer<ITextRangeProvider>(
                new UiaTextRange(
                    _owningComboBox.ChildEditAccessibleObject,
                    this,
                    _owningComboBox.SelectionStart,
                    _owningComboBox.SelectionStart));
            return HRESULT.S_OK;
        }

        public override int GetLineFromCharIndex(int charIndex)
            => _owningComboBox.IsHandleCreated
                ? OwnerChildEditLineIndex
                : -1;

        public override int GetLineIndex(int line)
            => _owningComboBox.IsHandleCreated
                ? OwnerChildEditLineIndex
                : -1;

        public override Point GetPositionFromChar(int charIndex)
            => _owningComboBox.IsHandleCreated
                ? GetPositionFromCharIndex(charIndex)
                : Point.Empty;

        // A variation on EM_POSFROMCHAR that returns the upper-right corner instead of upper-left.
        public override Point GetPositionFromCharForUpperRightCorner(int startCharIndex, string text)
        {
            if (!_owningComboBox.IsHandleCreated || startCharIndex < 0 || startCharIndex >= text.Length)
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

            if (!_owningComboBox.IsHandleCreated)
            {
                *pRetVal = null;
                return HRESULT.S_OK;
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
            using var selection = ComHelpers.GetComScope<ITextRangeProvider>(new UiaTextRange(_owningComboBox.ChildEditAccessibleObject, this, start, end));
            result[0] = selection;

            *pRetVal = result;
            return HRESULT.S_OK;
        }

        public override void GetVisibleRangePoints(out int visibleStart, out int visibleEnd)
        {
            visibleStart = 0;
            visibleEnd = 0;

            if (!_owningComboBox.IsHandleCreated || IsDegenerate(_owningComboBox.ClientRectangle))
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

            if (!_owningComboBox.IsHandleCreated)
            {
                *pRetVal = SAFEARRAY.CreateEmpty(VARENUM.VT_UNKNOWN);
                return HRESULT.S_OK;
            }

            GetVisibleRangePoints(out int start, out int end);

            ComSafeArrayScope<ITextRangeProvider> result = new(1);
            // Adding to the SAFEARRAY adds a reference
            using var ranges = ComHelpers.GetComScope<ITextRangeProvider>(new UiaTextRange(_owningComboBox.ChildEditAccessibleObject, this, start, end));
            result[0] = ranges;

            *pRetVal = result;
            return HRESULT.S_OK;
        }

        public override bool LineScroll(int charactersHorizontal, int linesVertical)
            // If the EM_LINESCROLL message is sent to a single-line edit control, the return value is FALSE.
            => false;

        public override Point PointToScreen(Point pt)
        {
            PInvokeCore.MapWindowPoints(_owningChildEdit, (HWND)default, ref pt);
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
                    _owningComboBox.ChildEditAccessibleObject,
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
            Debug.Fail("Text edit control cannot have a child element.");
            *pRetVal = null;
            return HRESULT.S_OK;
        }

        public override HRESULT RangeFromPoint(UiaPoint point, ITextRangeProvider** pRetVal)
        {
            if (pRetVal is null)
            {
                return HRESULT.E_POINTER;
            }

            if (!_owningComboBox.IsHandleCreated)
            {
                *pRetVal = default;
                return HRESULT.S_OK;
            }

            Point clientLocation = point;

            // Convert screen to client coordinates.
            // (Essentially ScreenToClient but MapWindowPoints accounts for window mirroring using WS_EX_LAYOUTRTL.)
            if (PInvokeCore.MapWindowPoints((HWND)default, _owningChildEdit, ref clientLocation) == 0)
            {
                *pRetVal = ComHelpers.GetComPointer<ITextRangeProvider>(
                    new UiaTextRange(
                        _owningComboBox.ChildEditAccessibleObject,
                        this,
                        start: 0,
                        end: 0));
                return HRESULT.S_OK;
            }

            // We have to deal with the possibility that the coordinate is inside the window rect
            // but outside the client rect. In that case we just scoot it over so it is at the nearest
            // point in the client rect.
            RECT clientRectangle = _owningComboBox.ChildEditAccessibleObject.BoundingRectangle;

            clientLocation.X = Math.Max(clientLocation.X, clientRectangle.left);
            clientLocation.X = Math.Min(clientLocation.X, clientRectangle.right);
            clientLocation.Y = Math.Max(clientLocation.Y, clientRectangle.top);
            clientLocation.Y = Math.Min(clientLocation.Y, clientRectangle.bottom);

            // Get the character at those client coordinates.
            int start = GetCharIndexFromPosition(clientLocation);

            *pRetVal = ComHelpers.GetComPointer<ITextRangeProvider>(
                new UiaTextRange(
                    _owningComboBox.ChildEditAccessibleObject,
                    this,
                    start,
                    start));
            return HRESULT.S_OK;
        }

        public override Rectangle RectangleToScreen(Rectangle rect) => _owningComboBox.RectangleToScreen(rect);

        public override void SetSelection(int start, int end)
        {
            if (!_owningComboBox.IsHandleCreated)
            {
                return;
            }

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
            int index = (int)PInvokeCore.SendMessage(_owningChildEdit, PInvokeCore.EM_CHARFROMPOS, (WPARAM)0, (LPARAM)pt);
            index = PARAM.LOWORD(index);

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
            PInvokeCore.SendMessage(_owningChildEdit, PInvokeCore.EM_GETRECT, (WPARAM)0, ref rectangle);

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

            fixed (void* pSize = &size)
            {
                // Add the width of the character at that position.
                return PInvoke.GetTextExtentPoint32W(hdc, &item, 1, (SIZE*)pSize);
            }
        }
    }
}
