// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms.Automation;
using static Interop;
using static Interop.User32;

namespace System.Windows.Forms
{
    public partial class ComboBox
    {
        internal class ComboBoxUiaTextProvider : UiaTextProvider2
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

                _owningChildEdit = owner._childEdit;
            }

            public override Rectangle BoundingRectangle
                => _owningComboBox.IsHandleCreated
                    ? GetFormattingRectangle()
                    : Rectangle.Empty;

            public override UiaCore.ITextRangeProvider DocumentRange => new UiaTextRange(_owningComboBox.ChildEditAccessibleObject, this, start: 0, TextLength);

            public override ES EditStyle
                => _owningComboBox.IsHandleCreated
                    ? GetEditStyle(_owningChildEdit)
                    : ES.LEFT;

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

                    ES extendedStyle = (ES)PInvoke.GetWindowLong(_owningChildEdit, WINDOW_LONG_PTR_INDEX.GWL_STYLE);
                    return extendedStyle.HasFlag(ES.AUTOHSCROLL);
                }
            }

            public override int LinesCount
                => _owningComboBox.IsHandleCreated
                    ? OwnerChildEditLinesCount
                    : -1;

            public override int LinesPerPage
            {
                get
                {
                    if (!_owningComboBox.IsHandleCreated)
                    {
                        return -1;
                    }

                    if (_owningComboBox.ChildEditAccessibleObject.BoundingRectangle.IsEmpty)
                    {
                        return 0;
                    }

                    return OwnerChildEditLinesCount;
                }
            }

            public override LOGFONTW Logfont
                => _owningComboBox.IsHandleCreated
                    ? LOGFONTW.FromFont(_owningComboBox.Font)
                    : default;

            public override UiaCore.SupportedTextSelection SupportedTextSelection => UiaCore.SupportedTextSelection.Single;

            public override string Text
                => _owningComboBox.IsHandleCreated
                    ? User32.GetWindowText((IHandle)_owningChildEdit)
                    : string.Empty;

            public override int TextLength
                => _owningComboBox.IsHandleCreated
                    ? (int)PInvoke.SendMessage(_owningChildEdit, WM.GETTEXTLENGTH)
                    : -1;

            public override WINDOW_EX_STYLE WindowExStyle
                => _owningComboBox.IsHandleCreated
                    ? GetWindowExStyle(_owningChildEdit)
                    : WINDOW_EX_STYLE.WS_EX_LEFT;

            public override WINDOW_STYLE WindowStyle
                => _owningComboBox.IsHandleCreated
                    ? GetWindowStyle(_owningChildEdit)
                    : WINDOW_STYLE.WS_OVERLAPPED;

            public override UiaCore.ITextRangeProvider? GetCaretRange(out BOOL isActive)
            {
                isActive = false;

                if (!_owningComboBox.IsHandleCreated)
                {
                    return null;
                }

                object? hasKeyboardFocus = _owningComboBox.ChildEditAccessibleObject.GetPropertyValue(UiaCore.UIA.HasKeyboardFocusPropertyId);
                if (hasKeyboardFocus is true)
                {
                    isActive = true;
                }

                return new UiaTextRange(_owningComboBox.ChildEditAccessibleObject, this, _owningComboBox.SelectionStart, _owningComboBox.SelectionStart);
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
                if (!_owningComboBox.IsHandleCreated)
                {
                    return null;
                }

                // First caret position of a selected text
                int start = 0;
                // Last caret position of a selected text
                int end = 0;

                // Returns info about the selected text range.
                // If there is no selection, start and end parameters are the position of the caret.
                PInvoke.SendMessage(_owningChildEdit, (WM)EM.GETSEL, ref start, ref end);

                return new UiaCore.ITextRangeProvider[] { new UiaTextRange(_owningComboBox.ChildEditAccessibleObject, this, start, end) };
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
                if (!_owningComboBox.IsHandleCreated)
                {
                    return null;
                }

                GetVisibleRangePoints(out int start, out int end);

                return new UiaCore.ITextRangeProvider[] { new UiaTextRange(_owningComboBox.ChildEditAccessibleObject, this, start, end) };
            }

            public override bool LineScroll(int charactersHorizontal, int linesVertical)
                // If the EM_LINESCROLL message is sent to a single-line edit control, the return value is FALSE.
                => false;

            public override Point PointToScreen(Point pt)
            {
                PInvoke.MapWindowPoints(_owningChildEdit, (HWND)default, ref pt);
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
                return new UiaTextRange(_owningComboBox.ChildEditAccessibleObject, this, start: 0, end: 0);
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
                if (!_owningComboBox.IsHandleCreated)
                {
                    return null;
                }

                Point clientLocation = screenLocation;

                // Convert screen to client coordinates.
                // (Essentially ScreenToClient but MapWindowPoints accounts for window mirroring using WS_EX_LAYOUTRTL.)
                if (PInvoke.MapWindowPoints((HWND)default, _owningChildEdit, ref clientLocation) == 0)
                {
                    return new UiaTextRange(_owningComboBox.ChildEditAccessibleObject, this, start: 0, end: 0);
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

                return new UiaTextRange(_owningComboBox.ChildEditAccessibleObject, this, start, start);
            }

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

                PInvoke.SendMessage(_owningChildEdit, (WM)EM.SETSEL, (WPARAM)start, (LPARAM)end);
            }

            private int GetCharIndexFromPosition(Point pt)
            {
                int index = (int)PInvoke.SendMessage(_owningChildEdit, (WM)EM.CHARFROMPOS, (WPARAM)0, (LPARAM)pt);
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
                RECT rectangle = new RECT();
                PInvoke.SendMessage(_owningChildEdit, (WM)EM.GETRECT, (WPARAM)0, ref rectangle);

                return rectangle;
            }

            private Point GetPositionFromCharIndex(int index)
            {
                if (index < 0 || index >= Text.Length)
                {
                    return Point.Empty;
                }

                int i = (int)PInvoke.SendMessage(_owningChildEdit, (WM)EM.POSFROMCHAR, (WPARAM)index);

                return new Point(PARAM.SignedLOWORD(i), PARAM.SignedHIWORD(i));
            }

            private bool GetTextExtentPoint32(char item, out Size size)
            {
                size = new Size();

                using var hdc = new GetDcScope(_owningChildEdit.Handle);
                if (hdc.IsNull)
                {
                    return false;
                }

                // Add the width of the character at that position.
                return Gdi32.GetTextExtentPoint32W(hdc, item.ToString(), 1, ref size);
            }
        }
    }
}
