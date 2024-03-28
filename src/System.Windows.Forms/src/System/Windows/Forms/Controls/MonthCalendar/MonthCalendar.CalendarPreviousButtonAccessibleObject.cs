// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;
using Windows.Win32.UI.Accessibility;

namespace System.Windows.Forms;

public partial class MonthCalendar
{
    /// <summary>
    ///  Represents an accessible object for the Previous button in <see cref="MonthCalendar"/> control.
    /// </summary>
    internal sealed class CalendarPreviousButtonAccessibleObject : CalendarButtonAccessibleObject
    {
        // The "Previous" button is the first in the calendar accessibility tree.
        // Indices start at 1.
        private const int ChildId = 1;

        private readonly MonthCalendarAccessibleObject _monthCalendarAccessibleObject;

        public CalendarPreviousButtonAccessibleObject(MonthCalendarAccessibleObject calendarAccessibleObject)
            : base(calendarAccessibleObject)
        {
            _monthCalendarAccessibleObject = calendarAccessibleObject;
        }

        public override Rectangle Bounds
            => _monthCalendarAccessibleObject.GetCalendarPartRectangle(MCGRIDINFO_PART.MCGIP_PREV);

        public override string Description => SR.CalendarPreviousButtonAccessibleObjectDescription;

        internal override bool CanGetDescriptionInternal => false;

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
                    // If there is an opportunity to move to the previous dates
                    && _monthCalendarAccessibleObject.MinDate < displayRange.Start;
            }
        }

        internal override IRawElementProviderFragment.Interface? FragmentNavigate(NavigateDirection direction)
            => direction switch
            {
                NavigateDirection.NavigateDirection_NextSibling => _monthCalendarAccessibleObject.NextButtonAccessibleObject,
                _ => base.FragmentNavigate(direction)
            };

        internal override int GetChildId() => ChildId;

        public override string Name => SR.MonthCalendarPreviousButtonAccessibleName;

        internal override bool CanGetNameInternal => false;
    }
}
