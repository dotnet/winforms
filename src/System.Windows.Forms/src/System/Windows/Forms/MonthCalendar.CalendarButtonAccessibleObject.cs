// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using static Interop;

namespace System.Windows.Forms
{
    public partial class MonthCalendar
    {
        internal abstract class CalendarButtonAccessibleObject : CalendarChildAccessibleObject
        {
            public CalendarButtonAccessibleObject(MonthCalendarAccessibleObject calendarAccessibleObject, int calendarIndex, CalendarButtonType buttonType)
                : base(calendarAccessibleObject, calendarIndex, (CalendarChildType)buttonType)
            {
            }

            protected abstract CalendarButtonType ButtonType { get; }

            protected override RECT CalculateBoundingRectangle()
            {
                ComCtl32.MCGIP dwPart;
                switch (ButtonType)
                {
                    case CalendarButtonType.Previous:
                        dwPart = ComCtl32.MCGIP.PREV;
                        break;

                    case CalendarButtonType.Next:
                        dwPart = ComCtl32.MCGIP.NEXT;
                        break;

                    default:
                        return new RECT();
                }

                _calendarAccessibleObject.GetCalendarPartRectangle(_calendarIndex, dwPart, -1, -1, out RECT rectangle);
                return rectangle;
            }

            internal override bool IsPatternSupported(int patternId) =>
                (patternId == NativeMethods.UIA_InvokePatternId) || base.IsPatternSupported(patternId);

            internal override void Invoke() => RaiseMouseClick();
        }
    }
}
