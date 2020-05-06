// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Drawing;
using System.Runtime.InteropServices;
using System.Threading;
using static Interop;
using static Interop.User32;

namespace System.Windows.Forms.IntegrationTests.MauiTests
{
    public static class MouseHelper
    {
        public static void SendClick(int x, int y)
        {
            var previousPosition = new Point();
            BOOL setOldCursorPos = GetPhysicalCursorPos(ref previousPosition);

            bool mouseSwapped = GetSystemMetrics(SystemMetric.SM_SWAPBUTTON) != 0;

            SendMouseInput(x, y, MOUSEEVENTF.MOVE | MOUSEEVENTF.ABSOLUTE);
            SendMouseInput(0, 0, mouseSwapped ? MOUSEEVENTF.RIGHTDOWN : MOUSEEVENTF.LEFTDOWN);
            SendMouseInput(0, 0, mouseSwapped ? MOUSEEVENTF.RIGHTUP : MOUSEEVENTF.LEFTUP);

            Thread.Sleep(50);

            // Set back the mouse position where it was.
            if (setOldCursorPos.IsTrue())
            {
                SendMouseInput(previousPosition.X, previousPosition.Y, MOUSEEVENTF.MOVE | MOUSEEVENTF.ABSOLUTE);
            }
        }

        private unsafe static void SendMouseInput(int x, int y, MOUSEEVENTF flags)
        {
            if ((flags & MOUSEEVENTF.ABSOLUTE) != 0)
            {
                int vscreenWidth = GetSystemMetrics(SystemMetric.SM_CXVIRTUALSCREEN);
                int vscreenHeight = GetSystemMetrics(SystemMetric.SM_CYVIRTUALSCREEN);
                int vscreenLeft = GetSystemMetrics(SystemMetric.SM_XVIRTUALSCREEN);
                int vscreenTop = GetSystemMetrics(SystemMetric.SM_YVIRTUALSCREEN);

                const int DesktopNormilizedMax = 65536;

                // Absolute input requires that input is in 'normalized' coords - with the entire
                // desktop being (0,0)...(65535,65536). Need to convert input x,y coords to this
                // first.
                //
                // In this normalized world, any pixel on the screen corresponds to a block of values
                // of normalized coords - eg. on a 1024x768 screen,
                // y pixel 0 corresponds to range 0 to 85.333,
                // y pixel 1 corresponds to range 85.333 to 170.666,
                // y pixel 2 correpsonds to range 170.666 to 256 - and so on.
                // Doing basic scaling math - (x-top)*65536/Width - gets us the start of the range.
                // However, because int math is used, this can end up being rounded into the wrong
                // pixel. For example, if we wanted pixel 1, we'd get 85.333, but that comes out as
                // 85 as an int, which falls into pixel 0's range - and that's where the pointer goes.
                // To avoid this, we add on half-a-"screen pixel"'s worth of normalized coords - to
                // push us into the middle of any given pixel's range - that's the 65536/(Width*2)
                // part of the formula. So now pixel 1 maps to 85+42 = 127 - which is comfortably
                // in the middle of that pixel's block.
                // The key ting here is that unlike points in coordinate geometry, pixels take up
                // space, so are often better treated like rectangles - and if you want to target
                // a particular pixel, target its rectangle's midpoint, not its edge.
                x = ((x - vscreenLeft) * DesktopNormilizedMax) / vscreenWidth + DesktopNormilizedMax / (vscreenWidth * 2);
                y = ((y - vscreenTop) * DesktopNormilizedMax) / vscreenHeight + DesktopNormilizedMax / (vscreenHeight * 2);

                flags |= MOUSEEVENTF.VIRTUALDESK;
            }

            var mouseInput = new INPUT();
            mouseInput.type = INPUTENUM.MOUSE;
            mouseInput.inputUnion.mi.dx = x;
            mouseInput.inputUnion.mi.dy = y;
            mouseInput.inputUnion.mi.mouseData = 0;
            mouseInput.inputUnion.mi.dwFlags = flags;
            mouseInput.inputUnion.mi.time = 0;
            mouseInput.inputUnion.mi.dwExtraInfo = IntPtr.Zero;

            SendInput(1, &mouseInput, Marshal.SizeOf(mouseInput));
        }

        public static (int x, int y) GetCenter(System.Drawing.Rectangle rect)
        {
            int x = rect.Left + ((rect.Right - rect.Left) / 2);
            int y = rect.Top + ((rect.Bottom - rect.Top) / 2);
            return (x, y);
        }
    }
}
