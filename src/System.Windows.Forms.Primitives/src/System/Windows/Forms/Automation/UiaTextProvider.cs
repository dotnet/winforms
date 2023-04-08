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

            if (IsExtendedKey(vk))
            {
                keyboardInput.Anonymous.ki.dwFlags |= KEYBD_EVENT_FLAGS.KEYEVENTF_EXTENDEDKEY;
            }

            keyboardInput.Anonymous.ki.time = 0;
            keyboardInput.Anonymous.ki.dwExtraInfo = UIntPtr.Zero;

            return SendInput(1, ref keyboardInput, sizeof(INPUT));
        }

        private static bool IsExtendedKey(VIRTUAL_KEY vk)
        {
            // From the SDK:
            // The extended-key flag indicates whether the keystroke message originated from one of
            // the additional keys on the enhanced keyboard. The extended keys consist of the ALT and
            // CTRL keys on the right-hand side of the keyboard; the INS, DEL, HOME, END, PAGE UP,
            // PAGE DOWN, and arrow keys in the clusters to the left of the numeric keypad; the NUM LOCK
            // key; the BREAK (CTRL+PAUSE) key; the PRINT SCRN key; and the divide (/) and ENTER keys in
            // the numeric keypad. The extended-key flag is set if the key is an extended key.
            //
            // - docs appear to be incorrect. Use of Spy++ indicates that break is not an extended key.
            // Also, menu key and windows keys also appear to be extended.
            return vk is VIRTUAL_KEY.VK_RMENU or
                   VIRTUAL_KEY.VK_RCONTROL or
                   VIRTUAL_KEY.VK_NUMLOCK or
                   VIRTUAL_KEY.VK_INSERT or
                   VIRTUAL_KEY.VK_DELETE or
                   VIRTUAL_KEY.VK_HOME or
                   VIRTUAL_KEY.VK_END or
                   VIRTUAL_KEY.VK_PRIOR or
                   VIRTUAL_KEY.VK_NEXT or
                   VIRTUAL_KEY.VK_UP or
                   VIRTUAL_KEY.VK_DOWN or
                   VIRTUAL_KEY.VK_LEFT or
                   VIRTUAL_KEY.VK_RIGHT or
                   VIRTUAL_KEY.VK_APPS or
                   VIRTUAL_KEY.VK_RWIN or
                   VIRTUAL_KEY.VK_LWIN;
            // Note that there are no distinct values for the following keys:
            // numpad divide
            // numpad enter
        }
    }
}
