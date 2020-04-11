// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using static Interop;

namespace System.Windows.Forms
{
    public partial class MonthCalendar
    {
        internal class CalendarHeaderAccessibleObject : CalendarChildAccessibleObject
        {
            private const int ChildId = 3;

            public CalendarHeaderAccessibleObject(MonthCalendarAccessibleObject calendarAccessibleObject, int calendarIndex)
                : base(calendarAccessibleObject, calendarIndex, CalendarChildType.CalendarHeader)
            {
            }

            public override string Name => _calendarAccessibleObject.GetCalendarChildName(_calendarIndex, CalendarChildType.CalendarHeader);

            internal override int GetChildId() => ChildId;

            internal override UnsafeNativeMethods.IRawElementProviderFragment FragmentNavigate(UnsafeNativeMethods.NavigateDirection direction) =>
                direction switch
                {
                    UnsafeNativeMethods.NavigateDirection.PreviousSibling =>
                        _calendarAccessibleObject.GetCalendarChildAccessibleObject(_calendarIndex, CalendarChildType.NextButton),
                    UnsafeNativeMethods.NavigateDirection.NextSibling => _calendarAccessibleObject.GetCalendarChildAccessibleObject(_calendarIndex, CalendarChildType.CalendarBody),
                    _ => base.FragmentNavigate(direction)
                };

            internal override object GetPropertyValue(int propertyID) =>
                propertyID switch
                {
                    NativeMethods.UIA_ControlTypePropertyId => NativeMethods.UIA_ButtonControlTypeId,
                    NativeMethods.UIA_NamePropertyId => Name,
                    _ => base.GetPropertyValue(propertyID)
                };

            protected override RECT CalculateBoundingRectangle()
            {
                _calendarAccessibleObject.GetCalendarPartRectangle(_calendarIndex, ComCtl32.MCGIP.CALENDARHEADER, -1, -1, out RECT rectangle);
                return rectangle;
            }

            internal override bool IsPatternSupported(int patternId) => (patternId == NativeMethods.UIA_InvokePatternId) || base.IsPatternSupported(patternId);

            internal override void Invoke() => RaiseMouseClick();
        }
    }
}
