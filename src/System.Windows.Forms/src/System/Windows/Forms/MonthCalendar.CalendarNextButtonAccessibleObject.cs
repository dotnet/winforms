// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable disable

using static Interop;

namespace System.Windows.Forms
{
    public partial class MonthCalendar
    {
        internal class CalendarNextButtonAccessibleObject : CalendarButtonAccessibleObject
        {
            private const int ChildId = 2;

            public CalendarNextButtonAccessibleObject(MonthCalendarAccessibleObject calendarAccessibleObject, int calendarIndex)
                : base(calendarAccessibleObject, calendarIndex, CalendarButtonType.Next)
            {
            }

            protected override CalendarButtonType ButtonType => CalendarButtonType.Next;

            internal override int GetChildId() => ChildId;

            internal override UiaCore.IRawElementProviderFragment FragmentNavigate(UiaCore.NavigateDirection direction) =>
                direction switch
                {
                    UiaCore.NavigateDirection.PreviousSibling => _calendarAccessibleObject.GetCalendarChildAccessibleObject(_calendarIndex, CalendarChildType.PreviousButton),
                    UiaCore.NavigateDirection.NextSibling => _calendarAccessibleObject.GetCalendarChildAccessibleObject(_calendarIndex, CalendarChildType.CalendarHeader),
                    _ => base.FragmentNavigate(direction)
                };

            internal override object GetPropertyValue(UiaCore.UIA propertyID) =>
                propertyID switch
                {
                    UiaCore.UIA.BoundingRectanglePropertyId => BoundingRectangle,
                    UiaCore.UIA.ControlTypePropertyId => UiaCore.UIA.ButtonControlTypeId,
                    UiaCore.UIA.NamePropertyId => SR.MonthCalendarNextButtonAccessibleName,
                    _ => base.GetPropertyValue(propertyID)
                };
        }
    }
}
