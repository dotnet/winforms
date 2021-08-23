// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Drawing;
using System.Runtime.InteropServices;
using static Interop;

namespace System.Windows.Forms
{
    public partial class MonthCalendar
    {
        /// <summary>
        ///  Represents an accessible object for buttons in <see cref="MonthCalendar"/> control.
        /// </summary>
        internal abstract class CalendarButtonAccessibleObject : MonthCalendarChildAccessibleObject
        {
            private readonly MonthCalendarAccessibleObject _monthCalendarAccessibleObject;

            public CalendarButtonAccessibleObject(MonthCalendarAccessibleObject calendarAccessibleObject)
                : base(calendarAccessibleObject)
            {
                _monthCalendarAccessibleObject = calendarAccessibleObject;
            }

            public override string DefaultAction => SR.AccessibleActionClick;

            public override void DoDefaultAction() => Invoke();

            internal override object? GetPropertyValue(UiaCore.UIA propertyID)
                => propertyID switch
                {
                    UiaCore.UIA.ControlTypePropertyId => UiaCore.UIA.ButtonControlTypeId,
                    _ => base.GetPropertyValue(propertyID)
                };

            internal override void Invoke() => RaiseMouseClick();

            internal override bool IsPatternSupported(UiaCore.UIA patternId)
                => patternId switch
                {
                    UiaCore.UIA.InvokePatternId => true,
                    _ => base.IsPatternSupported(patternId)
                };

            public override AccessibleObject Parent => _monthCalendarAccessibleObject;

            private void RaiseMouseClick()
            {
                // Make sure that the control is enabled.
                if (!_monthCalendarAccessibleObject.IsHandleCreated
                    || !_monthCalendarAccessibleObject.IsEnabled
                    || !IsEnabled)
                {
                    return;
                }

                RECT rectangle = Bounds;
                int x = rectangle.left + ((rectangle.right - rectangle.left) / 2);
                int y = rectangle.top + ((rectangle.bottom - rectangle.top) / 2);

                RaiseMouseClick(x, y);
            }

            private void RaiseMouseClick(int x, int y)
            {
                Point previousPosition = new();
                BOOL setOldCursorPos = User32.GetPhysicalCursorPos(ref previousPosition);

                bool mouseSwapped = User32.GetSystemMetrics(User32.SystemMetric.SM_SWAPBUTTON) != 0;

                SendMouseInput(x, y, User32.MOUSEEVENTF.MOVE | User32.MOUSEEVENTF.ABSOLUTE);
                SendMouseInput(0, 0, mouseSwapped ? User32.MOUSEEVENTF.RIGHTDOWN : User32.MOUSEEVENTF.LEFTDOWN);
                SendMouseInput(0, 0, mouseSwapped ? User32.MOUSEEVENTF.RIGHTUP : User32.MOUSEEVENTF.LEFTUP);

                Threading.Thread.Sleep(50);

                // Set back the mouse position where it was.
                if (setOldCursorPos.IsTrue())
                {
                    SendMouseInput(previousPosition.X, previousPosition.Y, User32.MOUSEEVENTF.MOVE | User32.MOUSEEVENTF.ABSOLUTE);
                }
            }

            public override AccessibleRole Role => AccessibleRole.PushButton;

            private unsafe void SendMouseInput(int x, int y, User32.MOUSEEVENTF flags)
            {
                if ((flags & User32.MOUSEEVENTF.ABSOLUTE) != 0)
                {
                    int vscreenWidth = User32.GetSystemMetrics(User32.SystemMetric.SM_CXVIRTUALSCREEN);
                    int vscreenHeight = User32.GetSystemMetrics(User32.SystemMetric.SM_CYVIRTUALSCREEN);
                    int vscreenLeft = User32.GetSystemMetrics(User32.SystemMetric.SM_XVIRTUALSCREEN);
                    int vscreenTop = User32.GetSystemMetrics(User32.SystemMetric.SM_YVIRTUALSCREEN);

                    const int DesktopNormalizedMax = 65536;

                    // Absolute input requires that input is in 'normalized' coords - with the entire
                    // desktop being (0,0)...(65535,65536). Need to convert input x,y coords to this
                    // first.
                    //
                    // In this normalized world, any pixel on the screen corresponds to a block of values
                    // of normalized coords - eg. on a 1024x768 screen,
                    // y pixel 0 corresponds to range 0 to 85.333,
                    // y pixel 1 corresponds to range 85.333 to 170.666,
                    // y pixel 2 corresponds to range 170.666 to 256 - and so on.
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
                    x = ((x - vscreenLeft) * DesktopNormalizedMax) / vscreenWidth + DesktopNormalizedMax / (vscreenWidth * 2);
                    y = ((y - vscreenTop) * DesktopNormalizedMax) / vscreenHeight + DesktopNormalizedMax / (vscreenHeight * 2);

                    flags |= User32.MOUSEEVENTF.VIRTUALDESK;
                }

                User32.INPUT mouseInput = new();
                mouseInput.type = User32.INPUTENUM.MOUSE;
                mouseInput.inputUnion.mi.dx = x;
                mouseInput.inputUnion.mi.dy = y;
                mouseInput.inputUnion.mi.mouseData = 0;
                mouseInput.inputUnion.mi.dwFlags = flags;
                mouseInput.inputUnion.mi.time = 0;
                mouseInput.inputUnion.mi.dwExtraInfo = IntPtr.Zero;

                User32.SendInput(1, &mouseInput, Marshal.SizeOf(mouseInput));
            }
        }
    }
}
