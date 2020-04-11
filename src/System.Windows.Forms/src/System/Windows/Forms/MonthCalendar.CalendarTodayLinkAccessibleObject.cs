// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using static Interop;

namespace System.Windows.Forms
{
    public partial class MonthCalendar
    {
        internal class CalendarTodayLinkAccessibleObject : CalendarChildAccessibleObject
        {
            private const int ChildId = 5;

            public CalendarTodayLinkAccessibleObject(MonthCalendarAccessibleObject calendarAccessibleObject, int calendarIndex, CalendarChildType type)
                : base(calendarAccessibleObject, calendarIndex, CalendarChildType.TodayLink)
            {
            }

            protected override RECT CalculateBoundingRectangle()
            {
                _calendarAccessibleObject.GetCalendarPartRectangle(_calendarIndex, ComCtl32.MCGIP.FOOTER, -1, -1, out RECT calendarPartRectangle);
                return calendarPartRectangle;
            }

            internal override int GetChildId() => ChildId;

            internal override bool IsPatternSupported(int patternId) =>
                (patternId == NativeMethods.UIA_InvokePatternId) || base.IsPatternSupported(patternId);

            internal override object GetPropertyValue(int propertyID) =>
                propertyID switch
                {
                    NativeMethods.UIA_BoundingRectanglePropertyId => BoundingRectangle,
                    NativeMethods.UIA_ControlTypePropertyId => NativeMethods.UIA_ButtonControlTypeId,
                    NativeMethods.UIA_NamePropertyId => _calendarAccessibleObject.GetCalendarChildName(_calendarIndex, CalendarChildType.TodayLink),
                    _ => base.GetPropertyValue(propertyID)
                };

            internal override UnsafeNativeMethods.IRawElementProviderFragment FragmentNavigate(UnsafeNativeMethods.NavigateDirection direction) =>
                direction switch
                {
                    UnsafeNativeMethods.NavigateDirection.PreviousSibling =>
                        _calendarAccessibleObject.GetCalendarChildAccessibleObject(_calendarIndex, CalendarChildType.CalendarBody),
                    _ => base.FragmentNavigate(direction)
                };

            internal override void Invoke() => RaiseMouseClick();
        }
    }
}
