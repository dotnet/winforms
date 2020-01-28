// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable disable

using static Interop;

namespace System.Windows.Forms
{
    public partial class MonthCalendar
    {
        internal class CalendarPreviousButtonAccessibleObject : CalendarButtonAccessibleObject
        {
            private const int ChildId = 1;

            public CalendarPreviousButtonAccessibleObject(MonthCalendarAccessibleObject calendarAccessibleObject, int calendarIndex)
                : base(calendarAccessibleObject, calendarIndex, CalendarButtonType.Previous)
            {
            }

            protected override CalendarButtonType ButtonType => CalendarButtonType.Previous;

            internal override int GetChildId() => ChildId;

            internal override UiaCore.IRawElementProviderFragment FragmentNavigate(UiaCore.NavigateDirection direction) =>
                direction switch
                {
                    UiaCore.NavigateDirection.NextSibling => _calendarAccessibleObject.GetCalendarChildAccessibleObject(_calendarIndex, CalendarChildType.NextButton),
                    _ => base.FragmentNavigate(direction)
                };

            internal override object GetPropertyValue(UiaCore.UIA propertyID) =>
                propertyID switch
                {
                    UiaCore.UIA.BoundingRectanglePropertyId => BoundingRectangle,
                    UiaCore.UIA.ControlTypePropertyId => UiaCore.UIA.ButtonControlTypeId,
                    UiaCore.UIA.NamePropertyId => SR.MonthCalendarPreviousButtonAccessibleName,
                    _ => base.GetPropertyValue(propertyID)
                };
        }
    }
}
