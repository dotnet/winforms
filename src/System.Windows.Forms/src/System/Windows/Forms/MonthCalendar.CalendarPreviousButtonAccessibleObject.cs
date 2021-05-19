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
        ///  Represents an accessible object for the Previous button in <see cref="MonthCalendar"/> control.
        /// </summary>
        internal class CalendarPreviousButtonAccessibleObject : CalendarButtonAccessibleObject
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
                => _monthCalendarAccessibleObject.GetCalendarPartRectangle(ComCtl32.MCGIP.PREV);

            public override string Description => SR.CalendarPreviousButtonAccessibleObjectDescription;

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

            internal override UiaCore.IRawElementProviderFragment? FragmentNavigate(UiaCore.NavigateDirection direction)
                => direction switch
                {
                    UiaCore.NavigateDirection.NextSibling => _monthCalendarAccessibleObject.NextButtonAccessibleObject,
                    _ => base.FragmentNavigate(direction)
                };

            internal override int GetChildId() => ChildId;

            public override string Name => SR.MonthCalendarPreviousButtonAccessibleName;
        }
    }
}
