﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using static Interop;
using static Interop.ComCtl32;

namespace System.Windows.Forms
{
    public partial class MonthCalendar
    {
        /// <summary>
        ///  Represents an accessible object for a row in <see cref="MonthCalendar"/> control.
        /// </summary>
        internal class CalendarRowAccessibleObject : MonthCalendarChildAccessibleObject
        {
            // This const is used to get ChildId.
            // It should take into account previous rows in a calendar body.
            // Indices start at 1.
            private const int ChildIdIncrement = 1;

            private readonly CalendarBodyAccessibleObject _calendarBodyAccessibleObject;
            private readonly MonthCalendarAccessibleObject _monthCalendarAccessibleObject;
            private readonly int _calendarIndex;
            private readonly int _rowIndex;
            private readonly int[] _initRuntimeId;
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
                _initRuntimeId = new int[]
                {
                    _calendarBodyAccessibleObject.RuntimeId[0],
                    _calendarBodyAccessibleObject.RuntimeId[1],
                    _calendarBodyAccessibleObject.RuntimeId[2],
                    _calendarBodyAccessibleObject.RuntimeId[3],
                    GetChildId()
                };
            }

            public override Rectangle Bounds
                => _monthCalendarAccessibleObject.GetCalendarPartRectangle(MCGIP.CALENDARROW, _calendarIndex, _rowIndex);

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
                        int end = _monthCalendarAccessibleObject.CelendarView == MCMV.MONTH ? 7 : 4;

                        for (int i = start; i < end; i++)
                        {
                            string name = _monthCalendarAccessibleObject.GetCalendarPartText(MCGIP.CALENDARCELL, _calendarIndex, _rowIndex, i);
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

            internal void ClearChildCollection() => _cellsAccessibleObjects = null;

            public override string? Description
            {
                get
                {
                    // Only day and week number cells have a description
                    if (_rowIndex == -1
                        || _monthCalendarAccessibleObject.IsHandleCreated
                        || _monthCalendarAccessibleObject.CelendarView != MCMV.MONTH)
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

            internal override UiaCore.IRawElementProviderFragment? FragmentNavigate(UiaCore.NavigateDirection direction)
                => direction switch
                {
                    UiaCore.NavigateDirection.NextSibling
                        => _calendarBodyAccessibleObject.RowsAccessibleObjects?.Find(this)?.Next?.Value,
                    UiaCore.NavigateDirection.PreviousSibling
                        => _calendarBodyAccessibleObject.RowsAccessibleObjects?.Find(this)?.Previous?.Value,
                    UiaCore.NavigateDirection.FirstChild
                        => _monthCalendarAccessibleObject.ShowWeekNumbers && _rowIndex != -1
                            ? WeekNumberCellAccessibleObject
                            : CellsAccessibleObjects?.First?.Value,
                    UiaCore.NavigateDirection.LastChild => CellsAccessibleObjects?.Last?.Value,
                    _ => base.FragmentNavigate(direction)
                };

            internal override int GetChildId() => ChildIdIncrement + _rowIndex;

            internal override object? GetPropertyValue(UiaCore.UIA propertyID)
                => propertyID switch
                {
                    UiaCore.UIA.ControlTypePropertyId => UiaCore.UIA.PaneControlTypeId,
                    UiaCore.UIA.IsKeyboardFocusablePropertyId => IsEnabled,
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

            public override AccessibleObject Parent => _calendarBodyAccessibleObject;

            public override AccessibleRole Role => AccessibleRole.Row;

            internal override int Row => _rowIndex;

            internal override int[] RuntimeId => _initRuntimeId;

            internal override void SetFocus()
            {
                CalendarCellAccessibleObject? focusedCell = _monthCalendarAccessibleObject.FocusedCell;
                if (focusedCell is not null
                    && focusedCell.CalendarIndex == _calendarIndex
                    && focusedCell.Row == _rowIndex)
                {
                    focusedCell.RaiseAutomationEvent(UiaCore.UIA.AutomationFocusChangedEventId);
                }
            }

            internal CalendarWeekNumberCellAccessibleObject? WeekNumberCellAccessibleObject
            {
                get
                {
                    if (!_monthCalendarAccessibleObject.ShowWeekNumbers
                        || _monthCalendarAccessibleObject.CelendarView != MCMV.MONTH
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
}
