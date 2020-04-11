﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms
{
    public partial class MonthCalendar
    {
        /// <summary>
        /// Represents the calendar grid child accessible object.
        /// </summary>
        internal abstract class CalendarGridChildAccessibleObject : CalendarChildAccessibleObject
        {
            protected readonly AccessibleObject _parentAccessibleObject;

            public CalendarGridChildAccessibleObject(MonthCalendarAccessibleObject calendarAccessibleObject, int calendarIndex, CalendarChildType itemType,
                AccessibleObject parentAccessibleObject, int itemIndex) : base(calendarAccessibleObject, calendarIndex, itemType)
            {
                if (parentAccessibleObject == null)
                {
                    throw new ArgumentNullException(nameof(parentAccessibleObject));
                }

                _parentAccessibleObject = parentAccessibleObject;
            }

            public override AccessibleObject Parent => _parentAccessibleObject;
        }
    }
}
