// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

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

            internal override UnsafeNativeMethods.IRawElementProviderFragment FragmentNavigate(UnsafeNativeMethods.NavigateDirection direction) =>
                direction switch
                {
                    UnsafeNativeMethods.NavigateDirection.NextSibling =>
                        _calendarAccessibleObject.GetCalendarChildAccessibleObject(_calendarIndex, CalendarChildType.NextButton),
                    _ => base.FragmentNavigate(direction)
                };

            internal override object GetPropertyValue(int propertyID) =>
                propertyID switch
                {
                    NativeMethods.UIA_BoundingRectanglePropertyId => BoundingRectangle,
                    NativeMethods.UIA_ControlTypePropertyId => NativeMethods.UIA_ButtonControlTypeId,
                    NativeMethods.UIA_NamePropertyId => SR.MonthCalendarPreviousButtonAccessibleName,
                    _ => base.GetPropertyValue(propertyID)
                };
        }
    }
}
