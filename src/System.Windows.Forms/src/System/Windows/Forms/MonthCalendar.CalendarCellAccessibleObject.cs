// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Drawing;
using System.Globalization;
using static Interop;
using static Interop.ComCtl32;

namespace System.Windows.Forms
{
    public partial class MonthCalendar
    {
        /// <summary>
        ///  Represents an accessible object for a calendar date cell in <see cref="MonthCalendar"/> control.
        /// </summary>
        internal class CalendarCellAccessibleObject : CalendarButtonAccessibleObject
        {
            // This const is used to get ChildId.
            // It should take into account previous cells in a row.
            // Indices start at 1.
            private const int ChildIdIncrement = 1;

            private readonly CalendarRowAccessibleObject _calendarRowAccessibleObject;
            private readonly CalendarBodyAccessibleObject _calendarBodyAccessibleObject;
            private readonly MonthCalendarAccessibleObject _monthCalendarAccessibleObject;
            private readonly int _calendarIndex;
            private readonly int _rowIndex;
            private readonly int _columnIndex;
            private readonly int[] _initRuntimeId;
            private SelectionRange? _dateRange;

            public CalendarCellAccessibleObject(CalendarRowAccessibleObject calendarRowAccessibleObject,
                CalendarBodyAccessibleObject calendarBodyAccessibleObject,
                MonthCalendarAccessibleObject monthCalendarAccessibleObject,
                int calendarIndex, int rowIndex, int columnIndex)
                : base(monthCalendarAccessibleObject)
            {
                _calendarRowAccessibleObject = calendarRowAccessibleObject;
                _calendarBodyAccessibleObject = calendarBodyAccessibleObject;
                _monthCalendarAccessibleObject = monthCalendarAccessibleObject;
                _calendarIndex = calendarIndex;
                _rowIndex = rowIndex;
                _columnIndex = columnIndex;
                // RuntimeId don't change if the calendar date range is not changed,
                // otherwise the calendar accessibility tree will be rebuilt.
                // So save this value one time to avoid recreating new structures and making extra calculations.
                _initRuntimeId = new int[]
                {
                    _calendarRowAccessibleObject.RuntimeId[0],
                    _calendarRowAccessibleObject.RuntimeId[1],
                    _calendarRowAccessibleObject.RuntimeId[2],
                    _calendarRowAccessibleObject.RuntimeId[3],
                    _calendarRowAccessibleObject.RuntimeId[4],
                    GetChildId()
                };
            }

            public override Rectangle Bounds
                => _monthCalendarAccessibleObject
                .GetCalendarPartRectangle(MCGIP.CALENDARCELL, _calendarIndex, _rowIndex, _columnIndex);

            internal int CalendarIndex => _calendarIndex;

            internal override int Column => _columnIndex;

            internal override UiaCore.IRawElementProviderSimple ContainingGrid => _calendarBodyAccessibleObject;

            internal virtual SelectionRange? DateRange
                => _dateRange ??= _monthCalendarAccessibleObject
                .GetCalendarPartDateRange(MCGIP.CALENDARCELL, _calendarIndex, _rowIndex, _columnIndex);

            public override string? Description
            {
                get
                {
                    // Only date cells in the Month view have Descriptions that based on cells date ranges
                    if (!_monthCalendarAccessibleObject.IsHandleCreated
                        || _monthCalendarAccessibleObject.CalendarView != MCMV.MONTH
                        || DateRange is null)
                    {
                        return null;
                    }

                    DateTime cellDate = DateRange.Start;
                    CultureInfo culture = CultureInfo.CurrentCulture;
                    int weekNumber = culture.Calendar.GetWeekOfYear(cellDate,
                        culture.DateTimeFormat.CalendarWeekRule, _monthCalendarAccessibleObject.FirstDayOfWeek);

                    // Used string.Format here to get the correct value from resources
                    // that should be consistent with the rest resources values
                    return string.Format(SR.MonthCalendarWeekNumberDescription, weekNumber)
                        + $", {cellDate.ToString("dddd", culture)}";
                }
            }

            internal override UiaCore.IRawElementProviderFragment? FragmentNavigate(UiaCore.NavigateDirection direction)
                => direction switch
                {
                    UiaCore.NavigateDirection.NextSibling
                        => _calendarRowAccessibleObject.CellsAccessibleObjects?.Find(this)?.Next?.Value,
                    UiaCore.NavigateDirection.PreviousSibling
                        => _columnIndex == 0
                            ? _calendarRowAccessibleObject.WeekNumberCellAccessibleObject
                            : _calendarRowAccessibleObject.CellsAccessibleObjects?.Find(this)?.Previous?.Value,
                    _ => base.FragmentNavigate(direction)
                };

            internal override int GetChildId() => ChildIdIncrement + _columnIndex;

            internal override UiaCore.IRawElementProviderSimple[]? GetColumnHeaderItems()
            {
                if (!_monthCalendarAccessibleObject.IsHandleCreated
                    || _monthCalendarAccessibleObject.CalendarView != MCMV.MONTH)
                {
                    // Column headers are available only in the "Month" view
                    return null;
                }

                CalendarRowAccessibleObject? topRow = _calendarBodyAccessibleObject.RowsAccessibleObjects?.First?.Value;

                if (topRow is null || topRow.CellsAccessibleObjects is null)
                {
                    return null;
                }

                foreach (CalendarCellAccessibleObject cell in topRow.CellsAccessibleObjects)
                {
                    if (cell.Column == _columnIndex)
                    {
                        return new UiaCore.IRawElementProviderSimple[1] { cell };
                    }
                }

                return null;
            }

            internal override object? GetPropertyValue(UiaCore.UIA propertyID)
                => propertyID switch
                {
                    UiaCore.UIA.ControlTypePropertyId
                        => UiaCore.UIA.DataItemControlTypeId,
                    UiaCore.UIA.IsKeyboardFocusablePropertyId
                        => IsEnabled,
                    UiaCore.UIA.IsGridItemPatternAvailablePropertyId
                        => IsPatternSupported(UiaCore.UIA.GridItemPatternId),
                    UiaCore.UIA.IsTableItemPatternAvailablePropertyId
                        => IsPatternSupported(UiaCore.UIA.TableItemPatternId),
                    _ => base.GetPropertyValue(propertyID)
                };

            internal override UiaCore.IRawElementProviderSimple[]? GetRowHeaderItems()
                => _calendarRowAccessibleObject.WeekNumberCellAccessibleObject is AccessibleObject weekNumber
                    ? new UiaCore.IRawElementProviderSimple[1] { weekNumber }
                    : null;

            private protected override bool HasKeyboardFocus
                => _monthCalendarAccessibleObject.Focused
                    && _monthCalendarAccessibleObject.FocusedCell == this;

            internal override bool IsPatternSupported(UiaCore.UIA patternId)
                => patternId switch
                {
                    UiaCore.UIA.GridItemPatternId => true,
                    UiaCore.UIA.TableItemPatternId => true,
                    _ => base.IsPatternSupported(patternId)
                };

            public override string Name
            {
                get
                {
                    if (DateRange is null)
                    {
                        return string.Empty;
                    }

                    return _monthCalendarAccessibleObject.CalendarView switch
                    {
                        MCMV.MONTH => $"{DateRange.Start:D}",
                        MCMV.YEAR => $"{DateRange.Start:Y}",
                        MCMV.DECADE => $"{DateRange.Start:yyy}",
                        MCMV.CENTURY => $"{DateRange.Start:yyy} - {DateRange.End:yyy}",
                        _ => string.Empty,
                    };
                }
            }

            public override AccessibleObject Parent => _calendarRowAccessibleObject;

            public override AccessibleRole Role => AccessibleRole.Cell;

            internal override int Row => _rowIndex;

            internal override int[] RuntimeId => _initRuntimeId;

            public override void Select(AccessibleSelection flags)
            {
                if (DateRange is not null)
                {
                    _monthCalendarAccessibleObject.SetSelectionRange(DateRange.Start, DateRange.End);
                }
            }

            public override AccessibleStates State
            {
                get
                {
                    AccessibleStates state = AccessibleStates.Focusable | AccessibleStates.Selectable;

                    if (_monthCalendarAccessibleObject.Focused && _monthCalendarAccessibleObject.FocusedCell == this)
                    {
                        return state | AccessibleStates.Focused | AccessibleStates.Selected;
                    }

                    // This condition works correctly in Month view only because the cell range is bigger
                    // then the calendar selection range in the rest views.
                    // But in the rest views a user can select only one cell. It means that a focused cell equals one selected cell,
                    // so the correct state will be returned in the condition above for the rest views.
                    if (DateRange is not null && _monthCalendarAccessibleObject.CalendarView == MCMV.MONTH
                        && DateRange.Start >= _monthCalendarAccessibleObject.SelectionRange.Start
                        && DateRange.End <= _monthCalendarAccessibleObject.SelectionRange.End)
                    {
                        state |= AccessibleStates.Selected;
                    }

                    return state;
                }
            }
        }
    }
}
