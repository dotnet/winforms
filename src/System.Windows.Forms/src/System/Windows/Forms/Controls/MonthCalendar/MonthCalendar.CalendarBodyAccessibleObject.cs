// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;
using Windows.Win32.System.Variant;
using Windows.Win32.UI.Accessibility;

namespace System.Windows.Forms;

public partial class MonthCalendar
{
    /// <summary>
    ///  Represents an accessible object for a calendar body in <see cref="MonthCalendar"/> control.
    /// </summary>
    internal sealed class CalendarBodyAccessibleObject : MonthCalendarChildAccessibleObject
    {
        // A calendar body is the second in the calendar accessibility tree.
        // Indices start at 1.
        private const int ChildId = 2;

        private readonly CalendarAccessibleObject _calendarAccessibleObject;
        private readonly MonthCalendarAccessibleObject _monthCalendarAccessibleObject;
        private readonly int _calendarIndex;
        private readonly string _initName;
        private readonly int[] _runtimeId;
        private LinkedList<CalendarRowAccessibleObject>? _rowsAccessibleObjects;

        public CalendarBodyAccessibleObject(CalendarAccessibleObject calendarAccessibleObject,
            MonthCalendarAccessibleObject monthCalendarAccessibleObject, int calendarIndex)
            : base(monthCalendarAccessibleObject)
        {
            _calendarAccessibleObject = calendarAccessibleObject;
            _monthCalendarAccessibleObject = monthCalendarAccessibleObject;
            _calendarIndex = calendarIndex;
            // Name and RuntimeId don't change if the calendar date range is not changed,
            // otherwise the calendar accessibility tree will be rebuilt.
            // So save these values one time to avoid sending messages to Windows every time
            // or recreating new structures and making extra calculations.
            _initName = _monthCalendarAccessibleObject.GetCalendarPartText(MCGRIDINFO_PART.MCGIP_CALENDARHEADER, _calendarIndex);

            int[] id = _calendarAccessibleObject.RuntimeId;
            _runtimeId = [id[0], id[1], id[2], GetChildId()];
        }

        public override Rectangle Bounds
            => _monthCalendarAccessibleObject.GetCalendarPartRectangle(MCGRIDINFO_PART.MCGIP_CALENDARBODY, _calendarIndex);

        internal void DisconnectChildren()
        {
            Debug.Assert(OsVersion.IsWindows8OrGreater());
            if (_rowsAccessibleObjects is null)
            {
                return;
            }

            foreach (CalendarRowAccessibleObject row in _rowsAccessibleObjects)
            {
                row.DisconnectChildren();
                PInvoke.UiaDisconnectProvider(row, skipOSCheck: true);
            }

            _rowsAccessibleObjects.Clear();
            _rowsAccessibleObjects = null;
        }

        /// <remark>
        ///  A calendar always have 7 or 4 columns depending on its view.
        /// </remark>
        internal override int ColumnCount => _monthCalendarAccessibleObject.CalendarView == MONTH_CALDENDAR_MESSAGES_VIEW.MCMV_MONTH ? 7 : 4;

        internal override IRawElementProviderFragment.Interface? FragmentNavigate(NavigateDirection direction)
            => direction switch
            {
                NavigateDirection.NavigateDirection_NextSibling => null,
                NavigateDirection.NavigateDirection_PreviousSibling => _calendarAccessibleObject.CalendarHeaderAccessibleObject,
                NavigateDirection.NavigateDirection_FirstChild => RowsAccessibleObjects?.First?.Value,
                NavigateDirection.NavigateDirection_LastChild => RowsAccessibleObjects?.Last?.Value,
                _ => base.FragmentNavigate(direction),

            };

        internal override int GetChildId() => ChildId;

        internal override IRawElementProviderSimple.Interface[]? GetColumnHeaders()
        {
            // A calendar has column headers (days of week) only in the Month view
            if (_monthCalendarAccessibleObject.CalendarView != MONTH_CALDENDAR_MESSAGES_VIEW.MCMV_MONTH)
            {
                return null;
            }

            return RowsAccessibleObjects?.First?.Value.CellsAccessibleObjects?.ToArray();
        }

        internal override IRawElementProviderSimple.Interface? GetItem(int rowIndex, int columnIndex)
        {
            if (!_monthCalendarAccessibleObject.IsHandleCreated || RowsAccessibleObjects is null)
            {
                return null;
            }

            CalendarRowAccessibleObject? rowAccessibleObject = null;

            foreach (CalendarRowAccessibleObject row in RowsAccessibleObjects)
            {
                if (row.Row == rowIndex)
                {
                    rowAccessibleObject = row;
                    break;
                }
            }

            if (rowAccessibleObject is null)
            {
                return null;
            }

            if (rowIndex >= 0 && columnIndex == -1)
            {
                return rowAccessibleObject.WeekNumberCellAccessibleObject;
            }

            if (rowAccessibleObject.CellsAccessibleObjects is null)
            {
                return null;
            }

            foreach (CalendarCellAccessibleObject cell in rowAccessibleObject.CellsAccessibleObjects)
            {
                if (cell.Column == columnIndex)
                {
                    return cell;
                }
            }

            return null;
        }

        internal override VARIANT GetPropertyValue(UIA_PROPERTY_ID propertyID)
            => propertyID switch
            {
                UIA_PROPERTY_ID.UIA_ControlTypePropertyId => (VARIANT)(int)UIA_CONTROLTYPE_ID.UIA_TableControlTypeId,
                UIA_PROPERTY_ID.UIA_IsKeyboardFocusablePropertyId => (VARIANT)IsEnabled,
                _ => base.GetPropertyValue(propertyID)
            };

        /// <remark>
        ///  A calendar has row headers (week numbers) if <see cref="ShowWeekNumbers"/> is true
        ///  and the calendar in the Month view only.
        /// </remark>
        internal override IRawElementProviderSimple.Interface[]? GetRowHeaders()
        {
            if (!_monthCalendarAccessibleObject.IsHandleCreated
                || !_monthCalendarAccessibleObject.ShowWeekNumbers
                || _monthCalendarAccessibleObject.CalendarView != MONTH_CALDENDAR_MESSAGES_VIEW.MCMV_MONTH
                || RowsAccessibleObjects is null)
            {
                return null;
            }

            List<CalendarCellAccessibleObject> headers = [];

            foreach (CalendarRowAccessibleObject row in RowsAccessibleObjects)
            {
                if (row.Row == -1)
                {
                    continue;
                }

                if (row.WeekNumberCellAccessibleObject is null)
                {
                    Debug.Fail($"{nameof(row.WeekNumberCellAccessibleObject)} must not be null if ShowWeekNumbers is true in the Month view!");
                    return null;
                }

                headers.Add(row.WeekNumberCellAccessibleObject);
            }

            return headers.ToArray();
        }

        private protected override bool HasKeyboardFocus
            => _monthCalendarAccessibleObject.Focused
                && _monthCalendarAccessibleObject.FocusedCell?.CalendarIndex == _calendarIndex;

        internal override bool IsPatternSupported(UIA_PATTERN_ID patternId)
            => patternId switch
            {
                UIA_PATTERN_ID.UIA_GridPatternId => true,
                UIA_PATTERN_ID.UIA_TablePatternId => true,
                _ => base.IsPatternSupported(patternId)
            };

        public override string Name => _initName;

        internal override bool CanGetNameInternal => false;

        public override AccessibleObject Parent => _calendarAccessibleObject;

        private protected override bool IsInternal => true;

        public override AccessibleRole Role => AccessibleRole.Table;

        internal override int RowCount => RowsAccessibleObjects?.Count ?? -1;

        internal override RowOrColumnMajor RowOrColumnMajor => RowOrColumnMajor.RowOrColumnMajor_RowMajor;

        // Use a LinkedList instead a List for the following reasons:
        // 1. We don't require an access to items by indices.
        // 2. We only need the first or the last items, or iterate over all items.
        // 3. New items are only appended to the end of the collection.
        // 4. Simple API for getting an item siblings, e.g. Next or Previous values
        //    returns a real item or null.
        //
        // If we use a List to store item's siblings we have to have one more variable
        // that stores a real index of the item in the collection, because _rowIndex
        // doesn't reflect that. Or we would have to get the current index of the item
        // using IndexOf method every time.
        internal LinkedList<CalendarRowAccessibleObject>? RowsAccessibleObjects
        {
            get
            {
                if (_rowsAccessibleObjects is null && _monthCalendarAccessibleObject.IsHandleCreated)
                {
                    _rowsAccessibleObjects = new();

                    // Day of week cells have "-1" row index
                    int start = _monthCalendarAccessibleObject.CalendarView == MONTH_CALDENDAR_MESSAGES_VIEW.MCMV_MONTH ? -1 : 0;
                    // A calendar body always has 6 or 3 columns depending on its view
                    int end = _monthCalendarAccessibleObject.CalendarView == MONTH_CALDENDAR_MESSAGES_VIEW.MCMV_MONTH ? 6 : 3;

                    for (int i = start; i < end; i++)
                    {
                        // Don't add a row if it doesn't have cells
                        CalendarRowAccessibleObject row = new(this, _monthCalendarAccessibleObject, _calendarIndex, i);
                        if (row.CellsAccessibleObjects?.Count > 0)
                        {
                            _rowsAccessibleObjects.AddLast(row);
                        }
                    }
                }

                return _rowsAccessibleObjects;
            }
        }

        internal override int[] RuntimeId => _runtimeId;

        internal override void SetFocus()
        {
            CalendarCellAccessibleObject? focusedCell = _monthCalendarAccessibleObject.FocusedCell;
            if (focusedCell?.CalendarIndex == _calendarIndex)
            {
                focusedCell.RaiseAutomationEvent(UIA_EVENT_ID.UIA_AutomationFocusChangedEventId);
            }
        }

        public override AccessibleStates State => AccessibleStates.Default;
    }
}
