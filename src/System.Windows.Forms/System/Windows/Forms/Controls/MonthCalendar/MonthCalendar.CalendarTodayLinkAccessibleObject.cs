// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;
using Windows.Win32.UI.Accessibility;

namespace System.Windows.Forms;

public partial class MonthCalendar
{
    /// <summary>
    ///  Represents an accessible object for the Today button in <see cref="MonthCalendar"/> control.
    /// </summary>
    internal sealed class CalendarTodayLinkAccessibleObject : CalendarButtonAccessibleObject
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
            => _monthCalendarAccessibleObject.GetCalendarPartRectangle(MCGRIDINFO_PART.MCGIP_FOOTER);

        public override string Description => SR.CalendarTodayLinkAccessibleObjectDescription;

        internal override bool CanGetDescriptionInternal => false;

        internal override IRawElementProviderFragment.Interface? FragmentNavigate(NavigateDirection direction)
            => direction switch
            {
                NavigateDirection.NavigateDirection_PreviousSibling
                    => _monthCalendarAccessibleObject.CalendarsAccessibleObjects?.Last?.Value,
                _ => base.FragmentNavigate(direction)
            };

        internal override int GetChildId()
            => ChildIdIncrement + _monthCalendarAccessibleObject.CalendarsAccessibleObjects?.Count ?? -1;

        public override string Name
            => string.Format(SR.MonthCalendarTodayButtonAccessibleName,
            _monthCalendarAccessibleObject.TodayDate.ToShortDateString());

        internal override bool CanGetNameInternal => false;
    }
}
