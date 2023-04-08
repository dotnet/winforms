// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Drawing;
using Windows.Win32.UI.Input.KeyboardAndMouse;
using static Interop.UiaCore;
using static Interop.User32;

namespace System.Windows.Forms.Automation
{
    internal abstract class UiaTextProvider : ITextProvider
    {
        /// <summary>
        ///  The value of a width of an end of a text line as 2 px to a ScreenReader can show it.
        /// </summary>
        public const int EndOfLineWidth = 2;

        public abstract ITextRangeProvider[]? GetSelection();

        public abstract ITextRangeProvider[]? GetVisibleRanges();

        public abstract ITextRangeProvider? RangeFromChild(IRawElementProviderSimple childElement);

        public abstract ITextRangeProvider? RangeFromPoint(Point screenLocation);

        public abstract ITextRangeProvider? DocumentRange { get; }

        public abstract SupportedTextSelection SupportedTextSelection { get; }

        public abstract Rectangle BoundingRectangle { get; }

        public abstract ES EditStyle { get; }

        public abstract int FirstVisibleLine { get; }

        public abstract bool IsMultiline { get; }

        public abstract bool IsReadingRTL { get; }

        public abstract bool IsReadOnly { get; }

        public abstract bool IsScrollable { get; }

        public abstract int LinesPerPage { get; }

        public abstract int LinesCount { get; }

        public abstract LOGFONTW Logfont { get; }

        public abstract string Text { get; }

        public abstract int TextLength { get; }

        public abstract WINDOW_EX_STYLE WindowExStyle { get; }

        public abstract WINDOW_STYLE WindowStyle { get; }

        public abstract int GetLineFromCharIndex(int charIndex);

        public abstract int GetLineIndex(int line);

        public abstract Point GetPositionFromChar(int charIndex);

        public abstract Point GetPositionFromCharForUpperRightCorner(int startCharIndex, string text);

        public abstract void GetVisibleRangePoints(out int visibleStart, out int visibleEnd);

        public abstract bool LineScroll(int charactersHorizontal, int linesVertical);

        public abstract Point PointToScreen(Point pt);

        public abstract Rectangle RectangleToScreen(Rectangle rect);

        public abstract void SetSelection(int start, int end);

        public static ES GetEditStyle(IHandle<HWND> hWnd) => (ES)PInvoke.GetWindowLong(hWnd, WINDOW_LONG_PTR_INDEX.GWL_STYLE);

        public static WINDOW_EX_STYLE GetWindowExStyle(IHandle<HWND> hWnd) => (WINDOW_EX_STYLE)PInvoke.GetWindowLong(hWnd, WINDOW_LONG_PTR_INDEX.GWL_EXSTYLE);

        public static WINDOW_STYLE GetWindowStyle(IHandle<HWND> hWnd) => (WINDOW_STYLE)PInvoke.GetWindowLong(hWnd, WINDOW_LONG_PTR_INDEX.GWL_STYLE);

        public static double[] RectListToDoubleArray(List<Rectangle> rectArray)
        {
            if (rectArray is null || rectArray.Count == 0)
            {
                return Array.Empty<double>();
            }

            double[] doubles = new double[rectArray.Count * 4];
            int scan = 0;

            for (int i = 0; i < rectArray.Count; i++)
            {
                doubles[scan++] = rectArray[i].X;
                doubles[scan++] = rectArray[i].Y;
                doubles[scan++] = rectArray[i].Width;
                doubles[scan++] = rectArray[i].Height;
            }

            return doubles;
        }

        public int SendInput(int inputs, ref INPUT input, int size)
        {
            Span<INPUT> currentInput = stackalloc INPUT[1];
            currentInput[0] = input;

            return (int)PInvoke.SendInput(currentInput, size);
        }

        public unsafe int SendKeyboardInputVK(VIRTUAL_KEY vk, bool press)
        {
            INPUT keyboardInput = default(INPUT);

            keyboardInput.type = INPUT_TYPE.INPUT_KEYBOARD;
            keyboardInput.Anonymous.ki.wVk = vk;
            keyboardInput.Anonymous.ki.wScan = 0;
            keyboardInput.Anonymous.ki.dwFlags = press ? 0 : KEYBD_EVENT_FLAGS.KEYEVENTF_KEYUP;

            if (VirtualKeyUtilities.IsExtendedKey(vk))
            {
                keyboardInput.Anonymous.ki.dwFlags |= KEYBD_EVENT_FLAGS.KEYEVENTF_EXTENDEDKEY;
            }

            keyboardInput.Anonymous.ki.time = 0;
            keyboardInput.Anonymous.ki.dwExtraInfo = UIntPtr.Zero;

            return SendInput(1, ref keyboardInput, sizeof(INPUT));
        }
    }
}
