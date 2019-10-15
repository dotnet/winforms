// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using static Interop;

namespace System.Windows.Forms
{
    public partial class MonthCalendar
    {
        internal class CalendarRowAccessibleObject : CalendarGridChildAccessibleObject
        {
            int _rowIndex;

            public CalendarRowAccessibleObject(MonthCalendarAccessibleObject calendarAccessibleObject, int calendarIndex, CalendarBodyAccessibleObject parentAccessibleObject, int rowIndex)
                : base(calendarAccessibleObject, calendarIndex, CalendarChildType.CalendarRow, parentAccessibleObject, rowIndex)
            {
                _rowIndex = rowIndex;
            }

            public int RowIndex => _rowIndex;

            internal override int[] RuntimeId =>
                new int[4]
                {
                    RuntimeIDFirstItem,
                    _calendarAccessibleObject.Owner.Handle.ToInt32(),
                    Parent.GetChildId(),
                    GetChildId()
                };

            protected override RECT CalculateBoundingRectangle()
            {
                _calendarAccessibleObject.GetCalendarPartRectangle(_calendarIndex, ComCtl32.MCGIP.CALENDARROW, _rowIndex, -1, out RECT calendarPartRectangle);
                return calendarPartRectangle;
            }

            internal override int GetChildId() => _rowIndex + 1;

            internal override UnsafeNativeMethods.IRawElementProviderFragment FragmentNavigate(UnsafeNativeMethods.NavigateDirection direction) =>
                direction switch
                {
                    UnsafeNativeMethods.NavigateDirection.NextSibling =>
                        _calendarAccessibleObject.GetCalendarChildAccessibleObject(_calendarIndex, CalendarChildType.CalendarRow, _parentAccessibleObject, _rowIndex + 1),
                    UnsafeNativeMethods.NavigateDirection.PreviousSibling =>
                        _calendarAccessibleObject.GetCalendarChildAccessibleObject(_calendarIndex, CalendarChildType.CalendarRow, _parentAccessibleObject, _rowIndex - 1),
                    UnsafeNativeMethods.NavigateDirection.FirstChild =>
                        _calendarAccessibleObject.GetCalendarChildAccessibleObject(_calendarIndex, CalendarChildType.CalendarCell, this, 0),
                    UnsafeNativeMethods.NavigateDirection.LastChild =>
                        _calendarAccessibleObject.GetCalendarChildAccessibleObject(_calendarIndex, CalendarChildType.CalendarCell, this, _calendarAccessibleObject.ColumnCount - 1),
                    _ => base.FragmentNavigate(direction)
                };
        }
    }
}
