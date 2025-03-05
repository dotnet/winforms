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
    ///  Represents an accessible object for a row in <see cref="MonthCalendar"/> control.
    /// </summary>
    internal sealed class CalendarRowAccessibleObject : MonthCalendarChildAccessibleObject
    {
        // This const is used to get ChildId.
        // It should take into account previous rows in a calendar body.
        // Indices start at 1.
        private const int ChildIdIncrement = 1;

        private readonly CalendarBodyAccessibleObject _calendarBodyAccessibleObject;
        private readonly MonthCalendarAccessibleObject _monthCalendarAccessibleObject;
        private readonly int _calendarIndex;
        private readonly int _rowIndex;
        private readonly int[] _runtimeId;
        private LinkedList<CalendarCellAccessibleObject>? _cellsAccessibleObjects;
        private CalendarWeekNumberCellAccessibleObject? _weekNumberCellAccessibleObject;

        public CalendarRowAccessibleObject(CalendarBodyAccessibleObject calendarBodyAccessibleObject,
            MonthCalendarAccessibleObject monthCalendarAccessibleObject, int calendarIndex, int rowIndex)
            : base(monthCalendarAccessibleObject)
        {
            _calendarBodyAccessibleObject = calendarBodyAccessibleObject;
            _monthCalendarAccessibleObject = monthCalendarAccessibleObject;
            _calendarIndex = calendarIndex;
            _rowIndex = rowIndex;

            // RuntimeId doesn't change if the calendar date range is not changed,
            // otherwise the calendar accessibility tree will be rebuilt.
            // So save this value one time to avoid recreating new structures
            // and making extra calculations every time.
            int[] id = _calendarBodyAccessibleObject.RuntimeId;
            _runtimeId = [id[0], id[1], id[2], id[3], GetChildId()];
        }

        public override Rectangle Bounds
            => _monthCalendarAccessibleObject.GetCalendarPartRectangle(MCGRIDINFO_PART.MCGIP_CALENDARROW, _calendarIndex, _rowIndex);

        // Use a LinkedList instead a List for the following reasons:
        // 1. We don't require an access to items by indices.
        // 2. We only need the first or the last items, or iterate over all items.
        // 3. New items are only appended to the end of the collection.
        // 4. Simple API for getting an item siblings, e.g. Next or Previous values
        //    returns a real item or null.
        //
        // If we use a List to store item's siblings we have to have one more variable
        // that stores a real index of the item in the collection, because _calendarIndex
        // doesn't reflect that. Or we would have to get the current index of the item
        // using IndexOf method every time.
        internal LinkedList<CalendarCellAccessibleObject>? CellsAccessibleObjects
        {
            get
            {
                if (_cellsAccessibleObjects is null && _monthCalendarAccessibleObject.IsHandleCreated)
                {
                    _cellsAccessibleObjects = new();

                    int start = 0;
                    // A calendar body always has 7 or 4 columns depending on its view
                    int end = _monthCalendarAccessibleObject.CalendarView == MONTH_CALDENDAR_MESSAGES_VIEW.MCMV_MONTH ? 7 : 4;

                    for (int i = start; i < end; i++)
                    {
                        string name = _monthCalendarAccessibleObject.GetCalendarPartText(MCGRIDINFO_PART.MCGIP_CALENDARCELL, _calendarIndex, _rowIndex, i);
                        if (!string.IsNullOrEmpty(name))
                        {
                            CalendarCellAccessibleObject cell =
                                _rowIndex == -1
                                ? new CalendarDayOfWeekCellAccessibleObject(this, _calendarBodyAccessibleObject, _monthCalendarAccessibleObject, _calendarIndex, _rowIndex, i, name)
                                : new CalendarCellAccessibleObject(this, _calendarBodyAccessibleObject, _monthCalendarAccessibleObject, _calendarIndex, _rowIndex, i);
                            _cellsAccessibleObjects.AddLast(cell);
                        }
                    }
                }

                return _cellsAccessibleObjects;
            }
        }

        internal void DisconnectChildren()
        {
            Debug.Assert(OsVersion.IsWindows8OrGreater());

            PInvoke.UiaDisconnectProvider(_weekNumberCellAccessibleObject, skipOSCheck: true);
            _weekNumberCellAccessibleObject = null;

            if (_cellsAccessibleObjects is null)
            {
                return;
            }

            foreach (CalendarCellAccessibleObject cell in _cellsAccessibleObjects)
            {
                PInvoke.UiaDisconnectProvider(cell, skipOSCheck: true);
            }

            _cellsAccessibleObjects.Clear();
            _cellsAccessibleObjects = null;
        }

        public override string? Description
        {
            get
            {
                // Only day and week number cells have a description
                if (_rowIndex == -1
                    || _monthCalendarAccessibleObject.IsHandleCreated
                    || _monthCalendarAccessibleObject.CalendarView != MONTH_CALDENDAR_MESSAGES_VIEW.MCMV_MONTH)
                {
                    return null;
                }

                // Get the first date cell date value to calculate its week number.
                // It's impossible to use WeekNumberCellAccessibleObject because it may be null
                // if ShowWeekNumbers is false but anyway we need to get the week number for the row.
                CalendarCellAccessibleObject? cell = CellsAccessibleObjects?.First?.Value;
                if (cell is null || cell.DateRange is null)
                {
                    return null;
                }

                string weekNumber = GetWeekNumber(cell.DateRange.Start);

                return string.Format(SR.MonthCalendarWeekNumberDescription, weekNumber);
            }
        }

        internal override bool CanGetDescriptionInternal => false;

        internal override IRawElementProviderFragment.Interface? FragmentNavigate(NavigateDirection direction)
            => direction switch
            {
                NavigateDirection.NavigateDirection_NextSibling
                    => _calendarBodyAccessibleObject.RowsAccessibleObjects?.Find(this)?.Next?.Value,
                NavigateDirection.NavigateDirection_PreviousSibling
                    => _calendarBodyAccessibleObject.RowsAccessibleObjects?.Find(this)?.Previous?.Value,
                NavigateDirection.NavigateDirection_FirstChild
                    => _monthCalendarAccessibleObject.ShowWeekNumbers && _rowIndex != -1
                        ? WeekNumberCellAccessibleObject
                        : CellsAccessibleObjects?.First?.Value,
                NavigateDirection.NavigateDirection_LastChild => CellsAccessibleObjects?.Last?.Value,
                _ => base.FragmentNavigate(direction)
            };

        internal override int GetChildId() => ChildIdIncrement + _rowIndex;

        internal override VARIANT GetPropertyValue(UIA_PROPERTY_ID propertyID)
            => propertyID switch
            {
                UIA_PROPERTY_ID.UIA_ControlTypePropertyId => (VARIANT)(int)UIA_CONTROLTYPE_ID.UIA_PaneControlTypeId,
                UIA_PROPERTY_ID.UIA_IsKeyboardFocusablePropertyId => (VARIANT)IsEnabled,
                _ => base.GetPropertyValue(propertyID)
            };

        private string GetWeekNumber(DateTime date)
            => CultureInfo.CurrentCulture.Calendar
            .GetWeekOfYear(date, CultureInfo.CurrentCulture.DateTimeFormat.CalendarWeekRule,
            _monthCalendarAccessibleObject.FirstDayOfWeek).ToString();

        private protected override bool HasKeyboardFocus
        {
            get
            {
                CalendarCellAccessibleObject? focusedCell = _monthCalendarAccessibleObject.FocusedCell;

                return _monthCalendarAccessibleObject.Focused
                    && focusedCell is not null
                    && focusedCell.CalendarIndex == _calendarIndex
                    && focusedCell.Row == _rowIndex;
            }
        }

        public override string? Name => null; // Rows don't have names like in a native calendar

        internal override bool CanGetNameInternal => false;

        public override AccessibleObject Parent => _calendarBodyAccessibleObject;

        private protected override bool IsInternal => true;

        public override AccessibleRole Role => AccessibleRole.Row;

        internal override int Row => _rowIndex;

        internal override int[] RuntimeId => _runtimeId;

        internal override void SetFocus()
        {
            CalendarCellAccessibleObject? focusedCell = _monthCalendarAccessibleObject.FocusedCell;
            if (focusedCell is not null
                && focusedCell.CalendarIndex == _calendarIndex
                && focusedCell.Row == _rowIndex)
            {
                focusedCell.RaiseAutomationEvent(UIA_EVENT_ID.UIA_AutomationFocusChangedEventId);
            }
        }

        internal CalendarWeekNumberCellAccessibleObject? WeekNumberCellAccessibleObject
        {
            get
            {
                if (!_monthCalendarAccessibleObject.ShowWeekNumbers
                    || _monthCalendarAccessibleObject.CalendarView != MONTH_CALDENDAR_MESSAGES_VIEW.MCMV_MONTH
                    || CellsAccessibleObjects?.First is null
                    || CellsAccessibleObjects.First.Value.DateRange is null)
                {
                    return null;
                }

                return _weekNumberCellAccessibleObject ??=
                    new(this, _calendarBodyAccessibleObject, _monthCalendarAccessibleObject, _calendarIndex,
                    _rowIndex, -1, GetWeekNumber(CellsAccessibleObjects.First.Value.DateRange.Start));
            }
        }
    }
}
