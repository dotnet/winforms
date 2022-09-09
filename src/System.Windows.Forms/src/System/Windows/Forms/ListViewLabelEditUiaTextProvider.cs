// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms.Automation;
using static Interop;

namespace System.Windows.Forms
{
    internal class ListViewLabelEditUiaTextProvider : UiaTextProvider2
    {
        /// <summary>
        ///  Since the label edit inside the ListView is always single-line, for optimization
        ///  we always return 0 as the index of lines
        /// </summary>
        private const int OwnerChildEditLineIndex = 0;

        /// <summary>
        ///  Since the label edit inside the ListView is always single-line, for optimization
        ///  we always return 1 as the number of lines
        /// </summary>
        private const int OwnerChildEditLinesCount = 1;

        private readonly IHandle _owningChildEdit;
        private readonly AccessibleObject _owningChildEditAccessibilityObject;
        private readonly ListView _owningListView;

        public ListViewLabelEditUiaTextProvider(ListView owner, IHandle childEdit, AccessibleObject childEditAccessibilityObject)
        {
            _owningListView = owner.OrThrowIfNull();
            _owningChildEdit = childEdit;
            _owningChildEditAccessibilityObject = childEditAccessibilityObject.OrThrowIfNull();
        }

        public override Rectangle BoundingRectangle => GetFormattingRectangle();

        public override UiaCore.ITextRangeProvider DocumentRange => new UiaTextRange(_owningChildEditAccessibilityObject, this, start: 0, TextLength);

        public override User32.ES EditStyle => GetEditStyle(_owningChildEdit);

        public override int FirstVisibleLine => 0;

        public override bool IsMultiline => false;

        public override bool IsReadingRTL => WindowExStyle.HasFlag(User32.WS_EX.RTLREADING);

        public override bool IsReadOnly => false;

        public override bool IsScrollable
        {
            get
            {
                User32.ES extendedStyle = (User32.ES)User32.GetWindowLong(_owningChildEdit, User32.GWL.STYLE);
                return extendedStyle.HasFlag(User32.ES.AUTOHSCROLL);
            }
        }

        public override int LinesCount => OwnerChildEditLinesCount;

        public override int LinesPerPage => _owningChildEditAccessibilityObject.BoundingRectangle.IsEmpty ? 0 : OwnerChildEditLinesCount;

        public override User32.LOGFONTW Logfont => User32.LOGFONTW.FromFont(_owningListView.Font);

        public override UiaCore.SupportedTextSelection SupportedTextSelection => UiaCore.SupportedTextSelection.Single;

        public override string Text => User32.GetWindowText(_owningChildEdit);

        public override int TextLength => (int)User32.SendMessageW(_owningChildEdit, User32.WM.GETTEXTLENGTH);

        public override User32.WS_EX WindowExStyle => GetWindowExStyle(_owningChildEdit);

        public override User32.WS WindowStyle => GetWindowStyle(_owningChildEdit);

        public override UiaCore.ITextRangeProvider? GetCaretRange(out BOOL isActive)
        {
            isActive = BOOL.FALSE;

            object? hasKeyboardFocus = _owningChildEditAccessibilityObject.GetPropertyValue(UiaCore.UIA.HasKeyboardFocusPropertyId);
            if (hasKeyboardFocus is true)
            {
                isActive = BOOL.TRUE;
            }

            // First caret position of a selected text
            int start = 0;
            // Last caret position of a selected text
            int end = 0;

            // Returns info about the selected text range.
            // If there is no selection, start and end parameters are the position of the caret.
            User32.SendMessageW(_owningChildEdit, (User32.WM)User32.EM.GETSEL, ref start, ref end);

            return new UiaTextRange(_owningChildEditAccessibilityObject, this, start, start);
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

                if (ch == '\r' || ch == '\n')
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

        public override UiaCore.ITextRangeProvider[]? GetSelection()
        {
            // First caret position of a selected text
            int start = 0;
            // Last caret position of a selected text
            int end = 0;

            // Returns info about the selected text range.
            // If there is no selection, start and end parameters are the position of the caret.
            User32.SendMessageW(_owningChildEdit, (User32.WM)User32.EM.GETSEL, ref start, ref end);

            return new UiaCore.ITextRangeProvider[] { new UiaTextRange(_owningChildEditAccessibilityObject, this, start, end) };
        }

        public override void GetVisibleRangePoints(out int visibleStart, out int visibleEnd)
        {
            visibleStart = 0;
            visibleEnd = 0;

            if (IsDegenerate(_owningListView.ClientRectangle))
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
            Point ptStart = new Point(rectangle.X + 1, rectangle.Y + 1);
            Point ptEnd = new Point(rectangle.Right - 1, rectangle.Bottom - 1);

            visibleStart = GetCharIndexFromPosition(ptStart);
            visibleEnd = GetCharIndexFromPosition(ptEnd) + 1; // Add 1 to get a caret position after received character

            return;

            bool IsDegenerate(Rectangle rect)
                => rect.IsEmpty || rect.Width <= 0 || rect.Height <= 0;
        }

        public override UiaCore.ITextRangeProvider[]? GetVisibleRanges()
        {
            GetVisibleRangePoints(out int start, out int end);

            return new UiaCore.ITextRangeProvider[] { new UiaTextRange(_owningChildEditAccessibilityObject, this, start, end) };
        }

        public override bool LineScroll(int charactersHorizontal, int linesVertical)
            // If the EM_LINESCROLL message is sent to a single-line edit control, the return value is FALSE.
            => false;

        public override Point PointToScreen(Point pt)
        {
            PInvoke.MapWindowPoints((HWND)_owningChildEdit.Handle, HWND.Null, ref pt);
            return pt;
        }

        /// <summary>
        ///  Exposes a text range that contains the text that is the target of the annotation associated with the specified annotation element.
        /// </summary>
        /// <param name="annotationElement">
        ///  The provider for an element that implements the IAnnotationProvider interface.
        ///  The annotation element is a sibling of the element that implements the <see cref="UiaCore.ITextProvider2"/> interface for the document.
        /// </param>
        /// <returns>
        ///  A text range that contains the annotation target text.
        /// </returns>
        public override UiaCore.ITextRangeProvider RangeFromAnnotation(UiaCore.IRawElementProviderSimple annotationElement)
        {
            return new UiaTextRange(_owningChildEditAccessibilityObject, this, start: 0, end: 0);
        }

        public override UiaCore.ITextRangeProvider? RangeFromChild(UiaCore.IRawElementProviderSimple childElement)
        {
            // We don't have any children so this call returns null.
            Debug.Fail("Label edit control cannot have a child element.");
            return null;
        }

        /// <summary>
        ///  Returns the degenerate (empty) text range nearest to the specified screen coordinates.
        /// </summary>
        /// <param name="screenLocation">The location in screen coordinates.</param>
        /// <returns>A degenerate range nearest the specified location. Null is never returned.</returns>
        public override UiaCore.ITextRangeProvider? RangeFromPoint(Point screenLocation)
        {
            Point clientLocation = screenLocation;

            // Convert screen to client coordinates.
            // (Essentially ScreenToClient but MapWindowPoints accounts for window mirroring using WS_EX_LAYOUTRTL.)
            if (PInvoke.MapWindowPoints(HWND.Null, (HWND)_owningChildEdit.Handle, ref clientLocation) == 0)
            {
                return new UiaTextRange(_owningChildEditAccessibilityObject, this, start: 0, end: 0);
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

            return new UiaTextRange(_owningChildEditAccessibilityObject, this, start, start);
        }

        public override Rectangle RectangleToScreen(Rectangle rect)
        {
            RECT r = rect;
            PInvoke.MapWindowPoints((HWND)_owningChildEdit.Handle, HWND.Null, ref r);
            return Rectangle.FromLTRB(r.left, r.top, r.right, r.bottom);
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

            User32.SendMessageW(_owningChildEdit, (User32.WM)User32.EM.SETSEL, start, end);
        }

        private int GetCharIndexFromPosition(Point pt)
        {
            int index = PARAM.LOWORD(User32.SendMessageW(_owningChildEdit, (User32.WM)User32.EM.CHARFROMPOS, 0, PARAM.FromPoint(pt)));

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
            RECT rectangle = new RECT();
            User32.SendMessageW(_owningChildEdit, (User32.WM)User32.EM.GETRECT, 0, ref rectangle);

            return rectangle;
        }

        private Point GetPositionFromCharIndex(int index)
        {
            if (index < 0 || index >= Text.Length)
            {
                return Point.Empty;
            }

            int i = (int)User32.SendMessageW(_owningChildEdit, (User32.WM)User32.EM.POSFROMCHAR, index);

            return new Point(PARAM.SignedLOWORD(i), PARAM.SignedHIWORD(i));
        }

        private bool GetTextExtentPoint32(char item, out Size size)
        {
            size = new Size();

            using var hdc = new User32.GetDcScope(_owningChildEdit.Handle);
            if (hdc.IsNull)
            {
                return false;
            }

            // Add the width of the character at that position.
            return Gdi32.GetTextExtentPoint32W(hdc, item.ToString(), 1, ref size);
        }
    }
}
