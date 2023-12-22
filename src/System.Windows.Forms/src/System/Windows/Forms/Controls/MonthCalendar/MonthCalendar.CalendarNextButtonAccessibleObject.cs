// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;
using Windows.Win32.UI.Accessibility;

namespace System.Windows.Forms;

public partial class MonthCalendar
{
    /// <summary>
    ///  Represents an accessible object for the Next button in <see cref="MonthCalendar"/> control.
    /// </summary>
    internal class CalendarNextButtonAccessibleObject : CalendarButtonAccessibleObject
    {
        // The "Next" button is the second in the calendar accessibility tree.
        // Indices start at 1.
        private const int ChildId = 2;

        private readonly MonthCalendarAccessibleObject _monthCalendarAccessibleObject;

        public CalendarNextButtonAccessibleObject(MonthCalendarAccessibleObject calendarAccessibleObject)
            : base(calendarAccessibleObject)
        {
            _monthCalendarAccessibleObject = calendarAccessibleObject;
        }

        public override Rectangle Bounds
            => _monthCalendarAccessibleObject.GetCalendarPartRectangle(MCGRIDINFO_PART.MCGIP_NEXT);

        public override string Description => SR.CalendarNextButtonAccessibleObjectDescription;

        internal override bool CanGetDescriptionInternal => false;

        internal override IRawElementProviderFragment.Interface? FragmentNavigate(NavigateDirection direction)
            => direction switch
            {
                NavigateDirection.NavigateDirection_PreviousSibling => _monthCalendarAccessibleObject.PreviousButtonAccessibleObject,
                NavigateDirection.NavigateDirection_NextSibling => _monthCalendarAccessibleObject.CalendarsAccessibleObjects?.First?.Value,
                _ => base.FragmentNavigate(direction)
            };

        internal override int GetChildId() => ChildId;

        internal override void Invoke()
        {
            // Make sure that the control is enabled.
            if (!_monthCalendarAccessibleObject.IsHandleCreated
                || !_monthCalendarAccessibleObject.IsEnabled
                || !IsEnabled)
            {
                return;
            }

            base.Invoke();
            _monthCalendarAccessibleObject.UpdateDisplayRange();
        }

        private protected override bool IsEnabled
        {
            get
            {
                if (!_monthCalendarAccessibleObject.IsHandleCreated)
                {
                    return false;
                }

                SelectionRange? displayRange = _monthCalendarAccessibleObject.GetDisplayRange(true);

                return displayRange is not null
                    && _monthCalendarAccessibleObject.IsEnabled
                    // If there is an opportunity to move to the next dates
                    && _monthCalendarAccessibleObject.MaxDate > displayRange.End;
            }
        }

        public override string Name => SR.MonthCalendarNextButtonAccessibleName;

        internal override bool CanGetNameInternal => false;
    }
}
