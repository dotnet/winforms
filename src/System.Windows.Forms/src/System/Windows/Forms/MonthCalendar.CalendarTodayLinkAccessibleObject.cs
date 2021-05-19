// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Drawing;
using static Interop;

namespace System.Windows.Forms
{
    public partial class MonthCalendar
    {
        /// <summary>
        ///  Represents an accessible object for the Today button in <see cref="MonthCalendar"/> control.
        /// </summary>
        internal class CalendarTodayLinkAccessibleObject : CalendarButtonAccessibleObject
        {
            // This const is used to get ChildId.
            // It should take into account "Next" and "Previous" buttons and previous calendars IDs.
            // Indices start at 1.
            private const int ChildIdIncrement = 3;

            private readonly MonthCalendarAccessibleObject _monthCalendarAccessibleObject;

            public CalendarTodayLinkAccessibleObject(MonthCalendarAccessibleObject calendarAccessibleObject)
                : base(calendarAccessibleObject)
            {
                _monthCalendarAccessibleObject = calendarAccessibleObject;
            }

            public override Rectangle Bounds
                => _monthCalendarAccessibleObject.GetCalendarPartRectangle(ComCtl32.MCGIP.FOOTER);

            public override string Description => SR.CalendarTodayLinkAccessibleObjectDescription;

            internal override UiaCore.IRawElementProviderFragment? FragmentNavigate(UiaCore.NavigateDirection direction)
                => direction switch
                {
                    UiaCore.NavigateDirection.PreviousSibling
                        => _monthCalendarAccessibleObject.CalendarsAccessibleObjects?.Last?.Value,
                    _ => base.FragmentNavigate(direction)
                };

            internal override int GetChildId()
                => ChildIdIncrement + _monthCalendarAccessibleObject.CalendarsAccessibleObjects?.Count ?? -1;

            public override string Name
                => string.Format(SR.MonthCalendarTodayButtonAccessibleName,
                _monthCalendarAccessibleObject.TodayDate.ToShortDateString());
        }
    }
}
