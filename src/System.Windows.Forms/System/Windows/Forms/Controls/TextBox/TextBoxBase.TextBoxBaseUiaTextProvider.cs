// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;
using System.Windows.Forms.Automation;
using Windows.Win32.System.Com;
using Windows.Win32.System.Variant;
using Windows.Win32.UI.Accessibility;

namespace System.Windows.Forms;

public abstract partial class TextBoxBase
{
    internal sealed unsafe class TextBoxBaseUiaTextProvider : UiaTextProvider
    {
        private readonly WeakReference<TextBoxBase> _owner;

        public TextBoxBaseUiaTextProvider(TextBoxBase owner)
        {
            _owner = new(owner.OrThrowIfNull());
        }

        private TextBoxBase? Owner => _owner.TryGetTarget(out TextBoxBase? owner) ? owner : null;

        public override HRESULT GetSelection(SAFEARRAY** pRetVal)
        {
            if (pRetVal is null)
            {
                return HRESULT.E_POINTER;
            }

            if (Owner is null || !Owner.IsHandleCreated)
            {
                *pRetVal = SAFEARRAY.CreateEmpty(VARENUM.VT_UNKNOWN);
                return HRESULT.S_OK;
            }

            // First caret position of a selected text
            int start = 0;
            // Last caret position of a selected text
            int end = 0;

            // Returns info about the selected text range.
            // If there is no selection, start and end parameters are the position of the caret.
            PInvokeCore.SendMessage(Owner, PInvokeCore.EM_GETSEL, ref start, ref end);

            ComSafeArrayScope<ITextRangeProvider> result = new(1);
            // Adding to the SAFEARRAY adds a reference
            using var selection = ComHelpers.GetComScope<ITextRangeProvider>(new UiaTextRange(Owner.AccessibilityObject, this, start, end));
            result[0] = selection;

            *pRetVal = result;
            return HRESULT.S_OK;
        }

        public override HRESULT GetVisibleRanges(SAFEARRAY** pRetVal)
        {
            if (pRetVal is null)
            {
                return HRESULT.E_POINTER;
            }

            if (Owner is null || !Owner.IsHandleCreated)
            {
                *pRetVal = SAFEARRAY.CreateEmpty(VARENUM.VT_UNKNOWN);
                return HRESULT.S_OK;
            }

            GetVisibleRangePoints(out int start, out int end);

            ComSafeArrayScope<ITextRangeProvider> result = new(1);
            // Adding to the SAFEARRAY adds a reference
            using var ranges = ComHelpers.GetComScope<ITextRangeProvider>(new UiaTextRange(Owner.AccessibilityObject, this, start, end));
            result[0] = ranges;

            *pRetVal = result;
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

            if (Owner is null || !Owner.IsHandleCreated)
            {
                *pRetVal = default;
                return HRESULT.S_OK;
            }

            Point clientLocation = point;

            // Convert screen to client coordinates.
            // (Essentially ScreenToClient but MapWindowPoints accounts for window mirroring using WS_EX_LAYOUTRTL.)
            if (PInvokeCore.MapWindowPoints((HWND)default, Owner, ref clientLocation) == 0)
            {
                *pRetVal = ComHelpers.GetComPointer<ITextRangeProvider>(
                    new UiaTextRange(
                        Owner.AccessibilityObject,
                        this,
                        start: 0,
                        end: 0));
                return HRESULT.S_OK;
            }

            // We have to deal with the possibility that the coordinate is inside the window rect
            // but outside the client rect. In that case we just scoot it over so it is at the nearest
            // point in the client rect.
            RECT clientRectangle = Owner.ClientRectangle;

            clientLocation.X = Math.Max(clientLocation.X, clientRectangle.left);
            clientLocation.X = Math.Min(clientLocation.X, clientRectangle.right);
            clientLocation.Y = Math.Max(clientLocation.Y, clientRectangle.top);
            clientLocation.Y = Math.Min(clientLocation.Y, clientRectangle.bottom);

            // Get the character at those client coordinates.
            int start = Owner.GetCharIndexFromPosition(clientLocation);

            *pRetVal = ComHelpers.GetComPointer<ITextRangeProvider>(
                new UiaTextRange(
                    Owner.AccessibilityObject,
                    this,
                    start,
                    start));
            return HRESULT.S_OK;
        }

        public override Rectangle RectangleToScreen(Rectangle rect) => Owner is not null ? Owner.RectangleToScreen(rect) : Rectangle.Empty;

        public override ITextRangeProvider* DocumentRange => Owner is not null
            ? ComHelpers.GetComPointer<ITextRangeProvider>(
                new UiaTextRange(
                    Owner.AccessibilityObject,
                    this,
                    start: 0,
                    TextLength))
            : null;

        public override SupportedTextSelection SupportedTextSelection => SupportedTextSelection.SupportedTextSelection_Single;

        public override HRESULT GetCaretRange(BOOL* isActive, ITextRangeProvider** pRetVal)
        {
            if (isActive is null || pRetVal is null)
            {
                return HRESULT.E_POINTER;
            }

            *isActive = false;

            if (Owner is null || !Owner.IsHandleCreated)
            {
                *pRetVal = null;
                return HRESULT.S_OK;
            }

            VARIANT hasKeyboardFocus = Owner.AccessibilityObject.GetPropertyValue(UIA_PROPERTY_ID.UIA_HasKeyboardFocusPropertyId);
            *isActive = !hasKeyboardFocus.IsEmpty && (bool)hasKeyboardFocus;

            *pRetVal = ComHelpers.GetComPointer<ITextRangeProvider>(
                new UiaTextRange(
                    Owner.AccessibilityObject,
                    this,
                    Owner.SelectionStart,
                    Owner.SelectionStart));
            return HRESULT.S_OK;
        }

        public override Point PointToScreen(Point pt) => Owner is not null ? Owner.PointToScreen(pt) : Point.Empty;

        public override HRESULT RangeFromAnnotation(IRawElementProviderSimple* annotationElement, ITextRangeProvider** pRetVal)
        {
            if (pRetVal is null)
            {
                return HRESULT.E_POINTER;
            }

            *pRetVal = Owner is not null
                ? ComHelpers.GetComPointer<ITextRangeProvider>(
                    new UiaTextRange(
                        Owner.AccessibilityObject,
                        this,
                        start: 0,
                        end: 0))
                : null;
            return HRESULT.S_OK;
        }

        public override Rectangle BoundingRectangle
            => Owner is not null && Owner.IsHandleCreated
                ? GetFormattingRectangle()
                : Rectangle.Empty;

        public override int FirstVisibleLine
            => Owner is not null && Owner.IsHandleCreated
                ? (int)PInvokeCore.SendMessage(Owner, PInvokeCore.EM_GETFIRSTVISIBLELINE)
                : -1;

        public override bool IsMultiline => Owner is not null && Owner.Multiline;

        public override bool IsReadingRTL
            => Owner is not null && Owner.IsHandleCreated && WindowExStyle.HasFlag(WINDOW_EX_STYLE.WS_EX_RTLREADING);

        public override bool IsReadOnly => Owner is not null && Owner.ReadOnly;

        public override bool IsScrollable
        {
            get
            {
                if (Owner is null || !Owner.IsHandleCreated)
                {
                    return false;
                }

                return ((int)GetWindowStyle(Owner) & PInvoke.ES_AUTOHSCROLL) != 0;
            }
        }

        public override int LinesCount
            => Owner is not null && Owner.IsHandleCreated
                ? (int)PInvokeCore.SendMessage(Owner, PInvokeCore.EM_GETLINECOUNT)
                : -1;

        public override int LinesPerPage
        {
            get
            {
                if (Owner is null || !Owner.IsHandleCreated)
                {
                    return -1;
                }

                Rectangle rect = Owner.ClientRectangle;
                if (rect.IsEmpty)
                {
                    return 0;
                }

                if (!Owner.Multiline)
                {
                    return 1;
                }

                int fontHeight = Owner.Font.Height;
                return fontHeight != 0 ? (int)Math.Ceiling(((double)rect.Height) / fontHeight) : 0;
            }
        }

        public override LOGFONTW Logfont
            => Owner is not null && Owner.IsHandleCreated
                ? Owner.Font.ToLogicalFont()
                : default;

        public override string Text
            => Owner is not null && Owner.IsHandleCreated
                ? Owner.PasswordProtect
                    ? PasswordString
                    : Owner.Text
                : string.Empty;

        private string PasswordString
            => Owner is not null
                ? new string(Owner is TextBox textBox
                    ? textBox.PasswordChar
                    : ((MaskedTextBox)Owner).PasswordChar, Owner.Text.Length)
                : string.Empty;

        public override int TextLength => Text.Length;

        public override WINDOW_EX_STYLE WindowExStyle
            => Owner is not null && Owner.IsHandleCreated
                ? GetWindowExStyle(Owner)
                : WINDOW_EX_STYLE.WS_EX_LEFT;

        public override WINDOW_STYLE WindowStyle
            => Owner is not null && Owner.IsHandleCreated
                ? GetWindowStyle(Owner)
                : WINDOW_STYLE.WS_OVERLAPPED;

        public override int GetLineFromCharIndex(int charIndex)
            => Owner is not null && Owner.IsHandleCreated
                ? Owner.GetLineFromCharIndex(charIndex)
                : -1;

        public override int GetLineIndex(int line)
            => Owner is not null && Owner.IsHandleCreated
                ? (int)PInvokeCore.SendMessage(Owner, PInvokeCore.EM_LINEINDEX, (WPARAM)line)
                : -1;

        public override Point GetPositionFromChar(int charIndex)
            => Owner is not null && Owner.IsHandleCreated
                ? Owner.GetPositionFromCharIndex(charIndex)
                : Point.Empty;

        // A variation on EM_POSFROMCHAR that returns the upper-right corner instead of upper-left.
        public override Point GetPositionFromCharForUpperRightCorner(int startCharIndex, string text)
        {
            if (Owner is null || !Owner.IsHandleCreated || startCharIndex < 0 || startCharIndex >= text.Length)
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
                    return Owner.GetPositionFromCharIndex(useNext ? startCharIndex + 1 : startCharIndex);
                }

                pt = Owner.GetPositionFromCharIndex(startCharIndex);

                if (ch is '\r' or '\n')
                {
                    pt.X += EndOfLineWidth; // add 2 px to show the end of line
                }

                // return the UL corner of the rest characters because these characters have no width
                return pt;
            }

            // get the UL corner of the character
            pt = Owner.GetPositionFromCharIndex(startCharIndex);

            // add the width of the character at that position.
            if (GetTextExtentPoint32(ch, out Size size))
            {
                pt.X += size.Width;
            }

            return pt;
        }

        public override void GetVisibleRangePoints(out int visibleStart, out int visibleEnd)
        {
            visibleStart = 0;
            visibleEnd = 0;

            if (Owner is null || !Owner.IsHandleCreated || IsDegenerate(Owner.ClientRectangle))
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

            if (IsMultiline)
            {
                visibleStart = GetLineIndex(FirstVisibleLine);

                int lastVisibleLine = FirstVisibleLine + LinesPerPage - 1;
                // Index of the next line is the end caret position of the previous line.
                visibleEnd = GetLineIndex(lastVisibleLine + 1);
                if (visibleEnd == -1)
                {
                    visibleEnd = Text.Length;
                }
            }
            else if (Owner is not null)
            {
                visibleStart = Owner.GetCharIndexFromPosition(ptStart);
                // Add 1 to get a caret position after received character.
                visibleEnd = Owner.GetCharIndexFromPosition(ptEnd) + 1;
            }

            return;

            static bool IsDegenerate(Rectangle rect)
                => rect.IsEmpty || rect.Width <= 0 || rect.Height <= 0;
        }

        public override bool LineScroll(int charactersHorizontal, int linesVertical)
            // Sends an EM_LINESCROLL message to scroll it horizontally and/or vertically.
            => Owner is not null && Owner.IsHandleCreated
                && PInvokeCore.SendMessage(
                    Owner,
                    PInvokeCore.EM_LINESCROLL,
                    (WPARAM)charactersHorizontal,
                    (LPARAM)linesVertical) != 0;

        public override void SetSelection(int start, int end)
        {
            if (Owner is null || !Owner.IsHandleCreated)
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

            PInvokeCore.SendMessage(Owner, PInvokeCore.EM_SETSEL, (WPARAM)start, (LPARAM)end);
        }

        private RECT GetFormattingRectangle()
        {
            if (Owner is null)
            {
                return default;
            }

            Debug.Assert(Owner.IsHandleCreated);

            // Send an EM_GETRECT message to find out the bounding rectangle.
            RECT rectangle = default;
            PInvokeCore.SendMessage(Owner, PInvokeCore.EM_GETRECT, (WPARAM)0, ref rectangle);
            return rectangle;
        }

        private unsafe bool GetTextExtentPoint32(char item, out Size size)
        {
            if (Owner is null)
            {
                size = Size.Empty;
                return false;
            }

            Debug.Assert(Owner.IsHandleCreated);

            size = default;

            using GetDcScope hdc = new(Owner.HWND);
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
