// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable disable

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

            internal override bool IsPatternSupported(UiaCore.UIA patternId) =>
                (patternId == UiaCore.UIA.InvokePatternId) || base.IsPatternSupported(patternId);

            internal override object GetPropertyValue(UiaCore.UIA propertyID) =>
                propertyID switch
                {
                    UiaCore.UIA.BoundingRectanglePropertyId => BoundingRectangle,
                    UiaCore.UIA.ControlTypePropertyId => UiaCore.UIA.ButtonControlTypeId,
                    UiaCore.UIA.NamePropertyId => _calendarAccessibleObject.GetCalendarChildName(_calendarIndex, CalendarChildType.TodayLink),
                    _ => base.GetPropertyValue(propertyID)
                };

            internal override UiaCore.IRawElementProviderFragment FragmentNavigate(UiaCore.NavigateDirection direction) =>
                direction switch
                {
                    UiaCore.NavigateDirection.PreviousSibling => _calendarAccessibleObject.GetCalendarChildAccessibleObject(_calendarIndex, CalendarChildType.CalendarBody),
                    _ => base.FragmentNavigate(direction)
                };

            internal override void Invoke()
            {
                if (_calendarAccessibleObject.Owner.IsHandleCreated)
                {
                    RaiseMouseClick();
                }
            }
        }
    }
}
