// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms.Automation;
using static Interop;
using static Interop.User32;

namespace System.Windows.Forms
{
    public abstract partial class TextBoxBase
    {
        internal class TextBoxBaseUiaTextProvider : UiaTextProvider2
        {
            private readonly TextBoxBase _owningTextBoxBase;

            public TextBoxBaseUiaTextProvider(TextBoxBase owner)
            {
                _owningTextBoxBase = owner ?? throw new ArgumentNullException(nameof(owner));
            }

            public override UiaCore.ITextRangeProvider[]? GetSelection()
            {
                if (!_owningTextBoxBase.IsHandleCreated)
                {
                    return null;
                }

                // First caret position of a selected text
                int start = 0;
                // Last caret position of a selected text
                int end = 0;

                // Returns info about a selected text range.
                // If there is no selection, start and end parameters are the position of the caret.
                SendMessageW(_owningTextBoxBase, (WM)EM.GETSEL, ref start, ref end);

                var internalAccessibleObject = new InternalAccessibleObject(_owningTextBoxBase.AccessibilityObject);
                return new UiaCore.ITextRangeProvider[] { new UiaTextRange(internalAccessibleObject, this, start, end) };
            }

            public override UiaCore.ITextRangeProvider[]? GetVisibleRanges()
            {
                if (!_owningTextBoxBase.IsHandleCreated)
                {
                    return null;
                }

                GetVisibleRangePoints(out int start, out int end);
                var internalAccessibleObject = new InternalAccessibleObject(_owningTextBoxBase.AccessibilityObject);

                return new UiaCore.ITextRangeProvider[] { new UiaTextRange(internalAccessibleObject, this, start, end) };
            }

            public override UiaCore.ITextRangeProvider? RangeFromChild(UiaCore.IRawElementProviderSimple childElement)
            {
                // We don't have any children so this call returns null.
                Debug.Fail("Text edit control cannot have a child element.");
                return null;
            }

            /// <summary>
            /// Returns the degenerate (empty) text range nearest to the specified screen coordinates.
            /// </summary>
            /// <param name="screenLocation">The location in screen coordinates.</param>
            /// <returns>A degenerate range nearest the specified location. Null is never returned.</returns>
            public override UiaCore.ITextRangeProvider? RangeFromPoint(Point screenLocation)
            {
                if (!_owningTextBoxBase.IsHandleCreated)
                {
                    return null;
                }

                Point clientLocation = screenLocation;

                // Convert screen to client coordinates.
                // (Essentially ScreenToClient but MapWindowPoints accounts for window mirroring using WS_EX_LAYOUTRTL.)
                if (MapWindowPoints(default, _owningTextBoxBase, ref clientLocation, 1) == 0)
                {
                    return new UiaTextRange(new InternalAccessibleObject(_owningTextBoxBase.AccessibilityObject), this, 0, 0);
                }

                // We have to deal with the possibility that the coordinate is inside the window rect
                // but outside the client rect. In that case we just scoot it over so it is at the nearest
                // point in the client rect.
                RECT clientRectangle = _owningTextBoxBase.ClientRectangle;

                clientLocation.X = Math.Max(clientLocation.X, clientRectangle.left);
                clientLocation.X = Math.Min(clientLocation.X, clientRectangle.right);
                clientLocation.Y = Math.Max(clientLocation.Y, clientRectangle.top);
                clientLocation.Y = Math.Min(clientLocation.Y, clientRectangle.bottom);

                // Get the character at those client coordinates.
                int start = _owningTextBoxBase.GetCharIndexFromPosition(clientLocation);

                return new UiaTextRange(new InternalAccessibleObject(_owningTextBoxBase.AccessibilityObject), this, start, start);
            }

            public override UiaCore.ITextRangeProvider DocumentRange => new UiaTextRange(new InternalAccessibleObject(_owningTextBoxBase.AccessibilityObject), this, 0, TextLength);

            public override UiaCore.SupportedTextSelection SupportedTextSelection => UiaCore.SupportedTextSelection.Single;

            public override UiaCore.ITextRangeProvider? GetCaretRange(out BOOL isActive)
            {
                isActive = BOOL.FALSE;

                if (!_owningTextBoxBase.IsHandleCreated)
                {
                    return null;
                }

                var hasKeyboardFocus = _owningTextBoxBase.AccessibilityObject.GetPropertyValue(UiaCore.UIA.HasKeyboardFocusPropertyId);
                if (hasKeyboardFocus is bool && (bool)hasKeyboardFocus)
                {
                    isActive = BOOL.TRUE;
                }

                var internalAccessibleObject = new InternalAccessibleObject(_owningTextBoxBase.AccessibilityObject);

                return new UiaTextRange(internalAccessibleObject, this, _owningTextBoxBase.SelectionStart, _owningTextBoxBase.SelectionStart);
            }

            public override Point PointToScreen(Point pt) => _owningTextBoxBase.PointToScreen(pt);

            /// <summary>
            /// Exposes a text range that contains the text that is the target of the annotation associated with the specified annotation element.
            /// </summary>
            /// <param name="annotationElement">
            /// The provider for an element that implements the IAnnotationProvider interface.
            /// The annotation element is a sibling of the element that implements the <see cref="UiaCore.ITextProvider2"/> interface for the document.
            /// </param>
            /// <returns>
            /// A text range that contains the annotation target text.
            /// </returns>
            public override UiaCore.ITextRangeProvider RangeFromAnnotation(UiaCore.IRawElementProviderSimple annotationElement)
            {
                var internalAccessibleObject = new InternalAccessibleObject(_owningTextBoxBase.AccessibilityObject);

                return new UiaTextRange(internalAccessibleObject, this, 0, 0);
            }

            public override Rectangle BoundingRectangle
                => _owningTextBoxBase.IsHandleCreated
                    ? (Rectangle)GetFormattingRectangle()
                    : Rectangle.Empty;

            public override int FirstVisibleLine
                => _owningTextBoxBase.IsHandleCreated
                    ? (int)(long)SendMessageW(_owningTextBoxBase, (WM)EM.GETFIRSTVISIBLELINE)
                    : -1;

            public override bool IsMultiline => _owningTextBoxBase.Multiline;

            public override bool IsReadingRTL
                => _owningTextBoxBase.IsHandleCreated
                    ? WindowExStyle.HasFlag(WS_EX.RTLREADING)
                    : false;

            public override bool IsReadOnly => _owningTextBoxBase.ReadOnly;

            public override bool IsScrollable
            {
                get
                {
                    if (!_owningTextBoxBase.IsHandleCreated)
                    {
                        return false;
                    }

                    ES extendedStyle = (ES)(long)GetWindowLong(_owningTextBoxBase, GWL.STYLE);
                    return extendedStyle.HasFlag(ES.AUTOHSCROLL) || extendedStyle.HasFlag(ES.AUTOVSCROLL);
                }
            }

            public override int LinesCount
                => _owningTextBoxBase.IsHandleCreated
                    ? (int)(long)SendMessageW(_owningTextBoxBase, (WM)EM.GETLINECOUNT)
                    : -1;

            public override int LinesPerPage
            {
                get
                {
                    if (!_owningTextBoxBase.IsHandleCreated)
                    {
                        return -1;
                    }

                    Rectangle rect = _owningTextBoxBase.ClientRectangle;
                    if (rect.IsEmpty)
                    {
                        return 0;
                    }

                    if (!_owningTextBoxBase.Multiline)
                    {
                        return 1;
                    }

                    int fontHeight = _owningTextBoxBase.Font.Height;
                    return fontHeight != 0 ? (int)Math.Ceiling(((double)rect.Height) / fontHeight) : 0;
                }
            }

            public override LOGFONTW Logfont
                => _owningTextBoxBase.IsHandleCreated
                    ? LOGFONTW.FromFont(_owningTextBoxBase.Font)
                    : default;

            public override string Text
                => _owningTextBoxBase.IsHandleCreated
                    ? _owningTextBoxBase.Text
                    : string.Empty;

            public override int TextLength
                => _owningTextBoxBase.IsHandleCreated
                    ? (int)(long)SendMessageW(_owningTextBoxBase, WM.GETTEXTLENGTH)
                    : -1;

            public override WS_EX WindowExStyle
                => _owningTextBoxBase.IsHandleCreated
                    ? GetWindowExStyle(_owningTextBoxBase)
                    : WS_EX.LEFT;

            public override WS WindowStyle
                => _owningTextBoxBase.IsHandleCreated
                    ? GetWindowStyle(_owningTextBoxBase)
                    : WS.OVERLAPPED;

            public override ES EditStyle
                => _owningTextBoxBase.IsHandleCreated
                    ? GetEditStyle(_owningTextBoxBase)
                    : ES.LEFT;

            public override int GetLineFromCharIndex(int charIndex)
                => _owningTextBoxBase.IsHandleCreated
                    ? _owningTextBoxBase.GetLineFromCharIndex(charIndex)
                    : -1;

            public override int GetLineIndex(int line)
                => _owningTextBoxBase.IsHandleCreated
                    ? (int)(long)SendMessageW(_owningTextBoxBase, (WM)EM.LINEINDEX, (IntPtr)line)
                    : -1;

            public override Point GetPositionFromChar(int charIndex)
                => _owningTextBoxBase.IsHandleCreated
                    ? _owningTextBoxBase.GetPositionFromCharIndex(charIndex)
                    : Point.Empty;

            // A variation on EM_POSFROMCHAR that returns the upper-right corner instead of upper-left.
            public override Point GetPositionFromCharForUpperRightCorner(int startCharIndex, string text)
            {
                if (!_owningTextBoxBase.IsHandleCreated || startCharIndex < 0 || startCharIndex >= text.Length)
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
                        return _owningTextBoxBase.GetPositionFromCharIndex(useNext ? startCharIndex + 1 : startCharIndex);
                    }

                    pt = _owningTextBoxBase.GetPositionFromCharIndex(startCharIndex);

                    if (ch == '\r' || ch == '\n')
                    {
                        pt.X += EndOfLineWidth; // add 2 px to show the end of line
                    }

                    // return the UL corner of the rest characters because these characters have no width
                    return pt;
                }

                // get the UL corner of the character
                pt = _owningTextBoxBase.GetPositionFromCharIndex(startCharIndex);

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

                if (!_owningTextBoxBase.IsHandleCreated || IsDegenerate(_owningTextBoxBase.ClientRectangle))
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

                visibleStart = _owningTextBoxBase.GetCharIndexFromPosition(ptStart);
                visibleEnd = _owningTextBoxBase.GetCharIndexFromPosition(ptEnd) + 1; // Add 1 to get a caret position after received character

                return;

                bool IsDegenerate(Rectangle rect)
                    => rect.IsEmpty || rect.Width <= 0 || rect.Height <= 0;
            }

            public override bool LineScroll(int charactersHorizontal, int linesVertical)
                // Sends an EM_LINESCROLL message to scroll it horizontally and/or vertically.
                => _owningTextBoxBase.IsHandleCreated
                    && SendMessageW(_owningTextBoxBase, (WM)EM.LINESCROLL, (IntPtr)charactersHorizontal, (IntPtr)linesVertical) != IntPtr.Zero;

            public override void SetSelection(int start, int end)
            {
                if (!_owningTextBoxBase.IsHandleCreated)
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

                SendMessageW(_owningTextBoxBase, (WM)EM.SETSEL, (IntPtr)start, (IntPtr)end);
            }

            private RECT GetFormattingRectangle()
            {
                Debug.Assert(_owningTextBoxBase.IsHandleCreated);

                // Send an EM_GETRECT message to find out the bounding rectangle.
                RECT rectangle = new RECT();
                SendMessageW(_owningTextBoxBase, (WM)EM.GETRECT, (IntPtr)0, ref rectangle);
                return rectangle;
            }

            private bool GetTextExtentPoint32(char item, out Size size)
            {
                Debug.Assert(_owningTextBoxBase.IsHandleCreated);

                size = new Size();

                using var hdc = new GetDcScope(_owningTextBoxBase.Handle);
                if (hdc.IsNull)
                {
                    return false;
                }

                // Add the width of the character at that position.
                return Gdi32.GetTextExtentPoint32W(hdc, item.ToString(), 1, ref size).IsTrue();
            }
        }
    }
}
