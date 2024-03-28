// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;
using System.Globalization;
using Windows.Win32.System.Variant;
using Windows.Win32.UI.Accessibility;

namespace System.Windows.Forms;

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
            int[] id = _calendarRowAccessibleObject.RuntimeId;
            _initRuntimeId = [id[0], id[1], id[2], id[3], id[4], GetChildId()];
        }

        public override Rectangle Bounds
            => _monthCalendarAccessibleObject
            .GetCalendarPartRectangle(MCGRIDINFO_PART.MCGIP_CALENDARCELL, _calendarIndex, _rowIndex, _columnIndex);

        internal int CalendarIndex => _calendarIndex;

        internal override int Column => _columnIndex;

        internal override IRawElementProviderSimple.Interface ContainingGrid => _calendarBodyAccessibleObject;

        internal virtual SelectionRange? DateRange
            => _dateRange ??= _monthCalendarAccessibleObject
            .GetCalendarPartDateRange(MCGRIDINFO_PART.MCGIP_CALENDARCELL, _calendarIndex, _rowIndex, _columnIndex);

        public override string? Description
        {
            get
            {
                // Only date cells in the Month view have Descriptions that based on cells date ranges
                if (!_monthCalendarAccessibleObject.IsHandleCreated
                    || _monthCalendarAccessibleObject.CalendarView != MONTH_CALDENDAR_MESSAGES_VIEW.MCMV_MONTH
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
                    + $", {cellDate:dddd}";
            }
        }

        internal override bool CanGetDescriptionInternal => false;

        internal override IRawElementProviderFragment.Interface? FragmentNavigate(NavigateDirection direction)
            => direction switch
            {
                NavigateDirection.NavigateDirection_NextSibling
                    => _calendarRowAccessibleObject.CellsAccessibleObjects?.Find(this)?.Next?.Value,
                NavigateDirection.NavigateDirection_PreviousSibling
                    => _columnIndex == 0
                        ? _calendarRowAccessibleObject.WeekNumberCellAccessibleObject
                        : _calendarRowAccessibleObject.CellsAccessibleObjects?.Find(this)?.Previous?.Value,
                _ => base.FragmentNavigate(direction)
            };

        internal override int GetChildId() => ChildIdIncrement + _columnIndex;

        internal override IRawElementProviderSimple.Interface[]? GetColumnHeaderItems()
        {
            if (!_monthCalendarAccessibleObject.IsHandleCreated
                || _monthCalendarAccessibleObject.CalendarView != MONTH_CALDENDAR_MESSAGES_VIEW.MCMV_MONTH)
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
                    return [cell];
                }
            }

            return null;
        }

        internal override VARIANT GetPropertyValue(UIA_PROPERTY_ID propertyID)
            => propertyID switch
            {
                UIA_PROPERTY_ID.UIA_ControlTypePropertyId => (VARIANT)(int)UIA_CONTROLTYPE_ID.UIA_DataItemControlTypeId,
                UIA_PROPERTY_ID.UIA_IsKeyboardFocusablePropertyId => (VARIANT)IsEnabled,
                _ => base.GetPropertyValue(propertyID)
            };

        internal override IRawElementProviderSimple.Interface[]? GetRowHeaderItems()
            => _calendarRowAccessibleObject.WeekNumberCellAccessibleObject is AccessibleObject weekNumber
                ? new IRawElementProviderSimple.Interface[1] { weekNumber }
                : null;

        private protected override bool HasKeyboardFocus
            => _monthCalendarAccessibleObject.Focused
                && _monthCalendarAccessibleObject.FocusedCell == this;

        internal override bool IsPatternSupported(UIA_PATTERN_ID patternId)
            => patternId switch
            {
                UIA_PATTERN_ID.UIA_GridItemPatternId => true,
                UIA_PATTERN_ID.UIA_TableItemPatternId => true,
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
                    MONTH_CALDENDAR_MESSAGES_VIEW.MCMV_MONTH => $"{DateRange.Start:D}",
                    MONTH_CALDENDAR_MESSAGES_VIEW.MCMV_YEAR => $"{DateRange.Start:Y}",
                    MONTH_CALDENDAR_MESSAGES_VIEW.MCMV_DECADE => $"{DateRange.Start:yyy}",
                    MONTH_CALDENDAR_MESSAGES_VIEW.MCMV_CENTURY => $"{DateRange.Start:yyy} - {DateRange.End:yyy}",
                    _ => string.Empty,
                };
            }
        }

        internal override bool CanGetNameInternal => false;

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
                if (DateRange is not null && _monthCalendarAccessibleObject.CalendarView == MONTH_CALDENDAR_MESSAGES_VIEW.MCMV_MONTH
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
