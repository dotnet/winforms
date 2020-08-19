// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable disable

using static Interop;

namespace System.Windows.Forms
{
    public partial class MonthCalendar
    {
        internal class CalendarHeaderAccessibleObject : CalendarChildAccessibleObject
        {
            private const int ChildId = 3;

            public CalendarHeaderAccessibleObject(MonthCalendarAccessibleObject calendarAccessibleObject, int calendarIndex)
                : base(calendarAccessibleObject, calendarIndex, CalendarChildType.CalendarHeader)
            {
            }

            public override string Name => _calendarAccessibleObject.GetCalendarChildName(_calendarIndex, CalendarChildType.CalendarHeader);

            internal override int GetChildId() => ChildId;

            internal override UiaCore.IRawElementProviderFragment FragmentNavigate(UiaCore.NavigateDirection direction) =>
                direction switch
                {
                    UiaCore.NavigateDirection.PreviousSibling => _calendarAccessibleObject.GetCalendarChildAccessibleObject(_calendarIndex, CalendarChildType.NextButton),
                    UiaCore.NavigateDirection.NextSibling => _calendarAccessibleObject.GetCalendarChildAccessibleObject(_calendarIndex, CalendarChildType.CalendarBody),
                    _ => base.FragmentNavigate(direction)
                };

            internal override object GetPropertyValue(UiaCore.UIA propertyID) =>
                propertyID switch
                {
                    UiaCore.UIA.ControlTypePropertyId => UiaCore.UIA.ButtonControlTypeId,
                    UiaCore.UIA.NamePropertyId => Name,
                    _ => base.GetPropertyValue(propertyID)
                };

            protected override RECT CalculateBoundingRectangle()
            {
                _calendarAccessibleObject.GetCalendarPartRectangle(_calendarIndex, ComCtl32.MCGIP.CALENDARHEADER, -1, -1, out RECT rectangle);
                return rectangle;
            }

            internal override bool IsPatternSupported(UiaCore.UIA patternId) =>
                (patternId == UiaCore.UIA.InvokePatternId) || base.IsPatternSupported(patternId);

            internal override void Invoke()
            {
                if (_calendarAccessibleObject.Owner.IsHandleCreated)
                {
                    RaiseMouseClick();
                }
            }
        }
    }
}
