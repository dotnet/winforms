﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;
using System.Windows.Forms.Automation;
using static Interop;

namespace System.Windows.Forms;

public abstract partial class TextBoxBase
{
    internal class TextBoxBaseUiaTextProvider : UiaTextProvider2
    {
        private readonly WeakReference<TextBoxBase> _owner;

        public TextBoxBaseUiaTextProvider(TextBoxBase owner)
        {
            _owner = new(owner.OrThrowIfNull());
        }

        private TextBoxBase? Owner => _owner.TryGetTarget(out TextBoxBase? owner) ? owner : null;

        public override UiaCore.ITextRangeProvider[]? GetSelection()
        {
            if (Owner is null || !Owner.IsHandleCreated)
            {
                return null;
            }

            // First caret position of a selected text
            int start = 0;
            // Last caret position of a selected text
            int end = 0;

            // Returns info about the selected text range.
            // If there is no selection, start and end parameters are the position of the caret.
            PInvoke.SendMessage(Owner, PInvoke.EM_GETSEL, ref start, ref end);

            return new UiaCore.ITextRangeProvider[] { new UiaTextRange(Owner.AccessibilityObject, this, start, end) };
        }

        public override UiaCore.ITextRangeProvider[]? GetVisibleRanges()
        {
            if (Owner is null || !Owner.IsHandleCreated)
            {
                return null;
            }

            GetVisibleRangePoints(out int start, out int end);

            return new UiaCore.ITextRangeProvider[] { new UiaTextRange(Owner.AccessibilityObject, this, start, end) };
        }

        public override UiaCore.ITextRangeProvider? RangeFromChild(UiaCore.IRawElementProviderSimple childElement)
        {
            // We don't have any children so this call returns null.
            Debug.Fail("Text edit control cannot have a child element.");
            return null;
        }

        /// <summary>
        ///  Returns the degenerate (empty) text range nearest to the specified screen coordinates.
        /// </summary>
        /// <param name="screenLocation">The location in screen coordinates.</param>
        /// <returns>A degenerate range nearest the specified location. Null is never returned.</returns>
        public override UiaCore.ITextRangeProvider? RangeFromPoint(Point screenLocation)
        {
            if (Owner is null || !Owner.IsHandleCreated)
            {
                return null;
            }

            Point clientLocation = screenLocation;

            // Convert screen to client coordinates.
            // (Essentially ScreenToClient but MapWindowPoints accounts for window mirroring using WS_EX_LAYOUTRTL.)
            if (PInvoke.MapWindowPoints((HWND)default, Owner, ref clientLocation) == 0)
            {
                return new UiaTextRange(Owner.AccessibilityObject, this, start: 0, end: 0);
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

            return new UiaTextRange(Owner.AccessibilityObject, this, start, start);
        }

        public override Rectangle RectangleToScreen(Rectangle rect) => Owner is not null ? Owner.RectangleToScreen(rect) : Rectangle.Empty;

        public override UiaCore.ITextRangeProvider? DocumentRange => Owner is not null
            ? new UiaTextRange(Owner.AccessibilityObject, this, start: 0, TextLength)
            : null;

        public override UiaCore.SupportedTextSelection SupportedTextSelection => UiaCore.SupportedTextSelection.Single;

        public override UiaCore.ITextRangeProvider? GetCaretRange(out BOOL isActive)
        {
            isActive = false;

            if (Owner is null || !Owner.IsHandleCreated)
            {
                return null;
            }

            var hasKeyboardFocus = Owner.AccessibilityObject.GetPropertyValue(UiaCore.UIA.HasKeyboardFocusPropertyId);
            if (hasKeyboardFocus is bool && (bool)hasKeyboardFocus)
            {
                isActive = true;
            }

            return new UiaTextRange(Owner.AccessibilityObject, this, Owner.SelectionStart, Owner.SelectionStart);
        }

        public override Point PointToScreen(Point pt) => Owner is not null ? Owner.PointToScreen(pt) : Point.Empty;

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
        public override UiaCore.ITextRangeProvider? RangeFromAnnotation(UiaCore.IRawElementProviderSimple annotationElement)
        {
            return Owner is not null ? new UiaTextRange(Owner.AccessibilityObject, this, start: 0, end: 0) : null;
        }

        public override Rectangle BoundingRectangle
            => Owner is not null && Owner.IsHandleCreated
                ? GetFormattingRectangle()
                : Rectangle.Empty;

        public override int FirstVisibleLine
            => Owner is not null && Owner.IsHandleCreated
                ? (int)PInvoke.SendMessage(Owner, PInvoke.EM_GETFIRSTVISIBLELINE)
                : -1;

        public override bool IsMultiline => Owner is not null ? Owner.Multiline : false;

        public override bool IsReadingRTL
            => Owner is not null ? Owner.IsHandleCreated && WindowExStyle.HasFlag(WINDOW_EX_STYLE.WS_EX_RTLREADING) : false;

        public override bool IsReadOnly => Owner is not null ? Owner.ReadOnly : false;

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
                ? (int)PInvoke.SendMessage(Owner, PInvoke.EM_GETLINECOUNT)
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
                ? LOGFONTW.FromFont(Owner.Font)
                : default;

        public override string Text
            => Owner is not null && Owner.IsHandleCreated
                ? Owner.Text
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
                ? (int)PInvoke.SendMessage(Owner, PInvoke.EM_LINEINDEX, (WPARAM)line)
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

                if (ch == '\r' || ch == '\n')
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
            Point ptStart = new Point(rectangle.X + 1, rectangle.Y + 1);
            Point ptEnd = new Point(rectangle.Right - 1, rectangle.Bottom - 1);

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
                && PInvoke.SendMessage(
                    Owner,
                    PInvoke.EM_LINESCROLL,
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

            PInvoke.SendMessage(Owner, PInvoke.EM_SETSEL, (WPARAM)start, (LPARAM)end);
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
            PInvoke.SendMessage(Owner, PInvoke.EM_GETRECT, (WPARAM)0, ref rectangle);
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
