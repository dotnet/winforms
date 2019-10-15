// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

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

            internal override UnsafeNativeMethods.IRawElementProviderFragment FragmentNavigate(UnsafeNativeMethods.NavigateDirection direction) =>
                direction switch
                {
                    UnsafeNativeMethods.NavigateDirection.PreviousSibling =>
                        _calendarAccessibleObject.GetCalendarChildAccessibleObject(_calendarIndex, CalendarChildType.PreviousButton),
                    UnsafeNativeMethods.NavigateDirection.NextSibling => _calendarAccessibleObject.GetCalendarChildAccessibleObject(_calendarIndex, CalendarChildType.CalendarHeader),
                    _ => base.FragmentNavigate(direction)
                };

            internal override object GetPropertyValue(int propertyID) =>
                propertyID switch
                {
                    NativeMethods.UIA_BoundingRectanglePropertyId => BoundingRectangle,
                    NativeMethods.UIA_ControlTypePropertyId => NativeMethods.UIA_ButtonControlTypeId,
                    NativeMethods.UIA_NamePropertyId => SR.MonthCalendarNextButtonAccessibleName,
                    _ => base.GetPropertyValue(propertyID)
                };
        }
    }
}
