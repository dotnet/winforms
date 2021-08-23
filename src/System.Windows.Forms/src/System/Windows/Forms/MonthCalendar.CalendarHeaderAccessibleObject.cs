﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Drawing;
using static Interop;
using static Interop.ComCtl32;

namespace System.Windows.Forms
{
    public partial class MonthCalendar
    {
        /// <summary>
        ///  Represents an accessible object for a calendar header in <see cref="MonthCalendar"/> control.
        /// </summary>
        internal class CalendarHeaderAccessibleObject : CalendarButtonAccessibleObject
        {
            // A calendar header is the first in the calendar accessibility tree.
            // Indices start at 1.
            private const int ChildId = 1;

            private readonly CalendarAccessibleObject _calendarAccessibleObject;
            private readonly MonthCalendarAccessibleObject _monthCalendarAccessibleObject;
            private readonly int _calendarIndex;
            private readonly string _initName;
            private readonly int[] _initRuntimeId;

            public CalendarHeaderAccessibleObject(CalendarAccessibleObject calendarAccessibleObject,
                MonthCalendarAccessibleObject monthCalendarAccessibleObject, int calendarIndex)
                : base(monthCalendarAccessibleObject)
            {
                _calendarAccessibleObject = calendarAccessibleObject;
                _monthCalendarAccessibleObject = monthCalendarAccessibleObject;
                _calendarIndex = calendarIndex;
                // Name and RuntimeId don't change if the calendar date range is not changed,
                // otherwise the calendar accessibility tree will be rebuilt.
                // So save these values one time to avoid sending messages to Windows every time
                // or recreating new structures and making extra calculations.
                _initName = _monthCalendarAccessibleObject.GetCalendarPartText(MCGIP.CALENDARHEADER, _calendarIndex);
                _initRuntimeId = new int[]
                {
                    _calendarAccessibleObject.RuntimeId[0],
                    _calendarAccessibleObject.RuntimeId[1],
                    _calendarAccessibleObject.RuntimeId[2],
                    GetChildId()
                };
            }

            public override Rectangle Bounds
                => _monthCalendarAccessibleObject.GetCalendarPartRectangle(MCGIP.CALENDARHEADER, _calendarIndex);

            internal override UiaCore.IRawElementProviderFragment? FragmentNavigate(UiaCore.NavigateDirection direction)
                => direction switch
                {
                    UiaCore.NavigateDirection.PreviousSibling => null,
                    UiaCore.NavigateDirection.NextSibling => _calendarAccessibleObject.CalendarBodyAccessibleObject,
                    _ => base.FragmentNavigate(direction)
                };

            internal override int GetChildId() => ChildId;

            public override string Name => _initName;

            public override AccessibleObject Parent => _calendarAccessibleObject;

            internal override int[] RuntimeId => _initRuntimeId;
        }
    }
}
