// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using static Interop;

namespace System.Windows.Forms
{
    public partial class MonthCalendar
    {
        /// <summary>
        /// Represents the calendar body accessible object.
        /// </summary>
        internal class CalendarBodyAccessibleObject : CalendarChildAccessibleObject
        {
            private const int ChildId = 4;

            public CalendarBodyAccessibleObject(MonthCalendarAccessibleObject calendarAccessibleObject, int calendarIndex)
                : base(calendarAccessibleObject, calendarIndex, CalendarChildType.CalendarBody)
            {
            }

            protected override RECT CalculateBoundingRectangle()
            {
                _calendarAccessibleObject.GetCalendarPartRectangle(_calendarIndex, ComCtl32.MCGIP.CALENDARBODY, 0, 0, out RECT calendarPartRectangle);
                return calendarPartRectangle;
            }

            internal override int GetChildId() => ChildId;

            internal override UiaCore.IRawElementProviderFragment FragmentNavigate(UiaCore.NavigateDirection direction) =>
                direction switch
                {
                    UiaCore.NavigateDirection.NextSibling => new Func<AccessibleObject>(() =>
                    {
                        MonthCalendar owner = (MonthCalendar)_calendarAccessibleObject.Owner;
                        return owner.ShowToday ? _calendarAccessibleObject.GetCalendarChildAccessibleObject(_calendarIndex, CalendarChildType.TodayLink) : null;
                    })(),
                    UiaCore.NavigateDirection.PreviousSibling => _calendarAccessibleObject.GetCalendarChildAccessibleObject(_calendarIndex, CalendarChildType.CalendarHeader),
                    UiaCore.NavigateDirection.FirstChild => _calendarAccessibleObject.GetCalendarChildAccessibleObject(_calendarIndex, CalendarChildType.CalendarRow, this, _calendarAccessibleObject.HasHeaderRow ? -1 : 0),
                    UiaCore.NavigateDirection.LastChild => _calendarAccessibleObject.GetCalendarChildAccessibleObject(_calendarIndex, CalendarChildType.CalendarRow, this, _calendarAccessibleObject.RowCount - 1),
                    _ => base.FragmentNavigate(direction),

                };

            public CalendarChildAccessibleObject GetFromPoint(ComCtl32.MCHITTESTINFO hitTestInfo)
            {
                switch ((ComCtl32.MCHT)hitTestInfo.uHit)
                {
                    case ComCtl32.MCHT.CALENDARDAY:
                    case ComCtl32.MCHT.CALENDARWEEKNUM:
                    case ComCtl32.MCHT.CALENDARDATE:
                        AccessibleObject rowAccessibleObject = _calendarAccessibleObject.GetCalendarChildAccessibleObject(_calendarIndex, CalendarChildType.CalendarRow, this, hitTestInfo.iRow);
                        return _calendarAccessibleObject.GetCalendarChildAccessibleObject(_calendarIndex, CalendarChildType.CalendarCell, rowAccessibleObject, hitTestInfo.iCol);
                }

                return this;
            }

            internal override object GetPropertyValue(UiaCore.UIA propertyID) =>
                propertyID switch
                {
                    UiaCore.UIA.NamePropertyId => SR.MonthCalendarBodyAccessibleName,
                    UiaCore.UIA.IsGridPatternAvailablePropertyId => true,
                    UiaCore.UIA.IsTablePatternAvailablePropertyId => true,
                    _ => base.GetPropertyValue(propertyID)
                };
        }
    }
}
