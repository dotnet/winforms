// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Runtime.InteropServices;
using Windows.Win32.System.Variant;
using Windows.Win32.UI.Accessibility;
using Windows.Win32.UI.Input.KeyboardAndMouse;

namespace System.Windows.Forms;

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

        internal override bool CanGetDefaultActionInternal => false;

        public override void DoDefaultAction() => Invoke();

        internal override VARIANT GetPropertyValue(UIA_PROPERTY_ID propertyID)
            => propertyID switch
            {
                UIA_PROPERTY_ID.UIA_ControlTypePropertyId => (VARIANT)(int)UIA_CONTROLTYPE_ID.UIA_ButtonControlTypeId,
                _ => base.GetPropertyValue(propertyID)
            };

        internal override void Invoke() => RaiseMouseClick();

        internal override bool IsPatternSupported(UIA_PATTERN_ID patternId)
            => patternId switch
            {
                UIA_PATTERN_ID.UIA_InvokePatternId => true,
                _ => base.IsPatternSupported(patternId)
            };

        public override AccessibleObject Parent => _monthCalendarAccessibleObject;

        private protected override bool IsInternal => true;

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
            int x = rectangle.left + (rectangle.Width / 2);
            int y = rectangle.top + (rectangle.Height / 2);

            RaiseMouseClick(x, y);
        }

        private static void RaiseMouseClick(int x, int y)
        {
            BOOL setOldCursorPos = PInvoke.GetPhysicalCursorPos(out Point previousPosition);
            bool mouseSwapped = PInvokeCore.GetSystemMetrics(SYSTEM_METRICS_INDEX.SM_SWAPBUTTON) != 0;

            SendMouseInput(x, y, MOUSE_EVENT_FLAGS.MOUSEEVENTF_MOVE | MOUSE_EVENT_FLAGS.MOUSEEVENTF_ABSOLUTE);
            SendMouseInput(0, 0, mouseSwapped ? MOUSE_EVENT_FLAGS.MOUSEEVENTF_RIGHTDOWN : MOUSE_EVENT_FLAGS.MOUSEEVENTF_LEFTDOWN);
            SendMouseInput(0, 0, mouseSwapped ? MOUSE_EVENT_FLAGS.MOUSEEVENTF_RIGHTUP : MOUSE_EVENT_FLAGS.MOUSEEVENTF_LEFTUP);

            Thread.Sleep(50);

            // Set back the mouse position where it was.
            if (setOldCursorPos)
            {
                SendMouseInput(previousPosition.X, previousPosition.Y, MOUSE_EVENT_FLAGS.MOUSEEVENTF_MOVE | MOUSE_EVENT_FLAGS.MOUSEEVENTF_ABSOLUTE);
            }
        }

        public override AccessibleRole Role => AccessibleRole.PushButton;

        private static unsafe void SendMouseInput(int x, int y, MOUSE_EVENT_FLAGS flags)
        {
            if ((flags & MOUSE_EVENT_FLAGS.MOUSEEVENTF_ABSOLUTE) != 0)
            {
                int vscreenWidth = PInvokeCore.GetSystemMetrics(SYSTEM_METRICS_INDEX.SM_CXVIRTUALSCREEN);
                int vscreenHeight = PInvokeCore.GetSystemMetrics(SYSTEM_METRICS_INDEX.SM_CYVIRTUALSCREEN);
                int vscreenLeft = PInvokeCore.GetSystemMetrics(SYSTEM_METRICS_INDEX.SM_XVIRTUALSCREEN);
                int vscreenTop = PInvokeCore.GetSystemMetrics(SYSTEM_METRICS_INDEX.SM_YVIRTUALSCREEN);

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

                flags |= MOUSE_EVENT_FLAGS.MOUSEEVENTF_VIRTUALDESK;
            }

            INPUT mouseInput = default;
            mouseInput.type = INPUT_TYPE.INPUT_MOUSE;
            mouseInput.Anonymous.mi.dx = x;
            mouseInput.Anonymous.mi.dy = y;
            mouseInput.Anonymous.mi.mouseData = 0;
            mouseInput.Anonymous.mi.dwFlags = flags;
            mouseInput.Anonymous.mi.time = 0;
            mouseInput.Anonymous.mi.dwExtraInfo = UIntPtr.Zero;

            PInvoke.SendInput(1, &mouseInput, Marshal.SizeOf(mouseInput));
        }
    }
}
