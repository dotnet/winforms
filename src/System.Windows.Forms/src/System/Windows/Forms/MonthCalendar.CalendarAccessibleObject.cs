// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics;
using System.Drawing;
using static Interop;
using static Interop.ComCtl32;

namespace System.Windows.Forms
{
    public partial class MonthCalendar
    {
        /// <summary>
        ///  Represents an accessible object for a calendar in <see cref="MonthCalendar"/> control.
        /// </summary>
        internal class CalendarAccessibleObject : MonthCalendarChildAccessibleObject
        {
            // This const is used to get ChildId.
            // It should take into account "Next" and "Previous" buttons.
            // Indices start at 1.
            private const int ChildIdIncrement = 3;

            private readonly MonthCalendarAccessibleObject _monthCalendarAccessibleObject;
            private readonly int _calendarIndex;
            private readonly string _initName;
            private CalendarBodyAccessibleObject? _calendarBodyAccessibleObject;
            private CalendarHeaderAccessibleObject? _calendarHeaderAccessibleObject;
            private SelectionRange? _dateRange;

            public CalendarAccessibleObject(MonthCalendarAccessibleObject calendarAccessibleObject, int calendarIndex, string initName)
                : base(calendarAccessibleObject)
            {
                _monthCalendarAccessibleObject = calendarAccessibleObject;
                _calendarIndex = calendarIndex;
                // Name doesn't change if the calendar date range is not changed,
                // otherwise the calendar accessibility tree will be rebuilt.
                // So save this value one time to avoid sending messages to Windows every time.
                _initName = initName;
            }

            internal void DisconnectChildren()
            {
                Debug.Assert(OsVersion.IsWindows8OrGreater());

                UiaCore.UiaDisconnectProvider(_calendarHeaderAccessibleObject);
                _calendarHeaderAccessibleObject = null;

                _calendarBodyAccessibleObject?.DisconnectChildren();
                UiaCore.UiaDisconnectProvider(_calendarBodyAccessibleObject);

                _calendarBodyAccessibleObject = null;
            }

            public override Rectangle Bounds
                => _monthCalendarAccessibleObject.GetCalendarPartRectangle(MCGIP.CALENDAR, _calendarIndex);

            internal CalendarBodyAccessibleObject CalendarBodyAccessibleObject
                => _calendarBodyAccessibleObject ??= new(this, _monthCalendarAccessibleObject, _calendarIndex);

            internal CalendarHeaderAccessibleObject CalendarHeaderAccessibleObject
                => _calendarHeaderAccessibleObject ??= new(this, _monthCalendarAccessibleObject, _calendarIndex);

            internal override int Column
                => _monthCalendarAccessibleObject.IsHandleCreated
                    ? _calendarIndex % _monthCalendarAccessibleObject.ColumnCount
                    : -1;

            internal override UiaCore.IRawElementProviderSimple? ContainingGrid => _monthCalendarAccessibleObject;

            internal SelectionRange? DateRange
            {
                get
                {
                    if (_dateRange is null && _monthCalendarAccessibleObject.IsHandleCreated)
                    {
                        SelectionRange? dateRange = _monthCalendarAccessibleObject.GetCalendarPartDateRange(MCGIP.CALENDAR, _calendarIndex);
                        if (dateRange is null)
                        {
                            return null;
                        }

                        // Add gray dates of the previous or next calendars
                        SelectionRange? displayRange = _monthCalendarAccessibleObject.GetDisplayRange(false);
                        if (displayRange is null)
                        {
                            return null;
                        }

                        if (_calendarIndex == 0 && displayRange.Start < dateRange.Start)
                        {
                            dateRange.Start = displayRange.Start;
                        }

                        if (_monthCalendarAccessibleObject.CalendarsAccessibleObjects?.Last?.Value == this
                            && displayRange.End > dateRange.End)
                        {
                            dateRange.End = displayRange.End;
                        }

                        _dateRange = dateRange;
                    }

                    return _dateRange;
                }
            }

            internal override UiaCore.IRawElementProviderFragment? FragmentNavigate(UiaCore.NavigateDirection direction)
                => direction switch
                {
                    UiaCore.NavigateDirection.NextSibling
                        => _monthCalendarAccessibleObject.CalendarsAccessibleObjects?.Find(this)?.Next?.Value
                        ?? (_monthCalendarAccessibleObject.ShowToday
                            ? (AccessibleObject)_monthCalendarAccessibleObject.TodayLinkAccessibleObject
                            : null),
                    UiaCore.NavigateDirection.PreviousSibling
                        => _calendarIndex == 0
                            ? _monthCalendarAccessibleObject.NextButtonAccessibleObject
                            : _monthCalendarAccessibleObject.CalendarsAccessibleObjects?.Find(this)?.Previous?.Value,
                    UiaCore.NavigateDirection.FirstChild => CalendarHeaderAccessibleObject,
                    UiaCore.NavigateDirection.LastChild => CalendarBodyAccessibleObject,
                    _ => base.FragmentNavigate(direction),
                };

            internal override int GetChildId() => ChildIdIncrement + _calendarIndex;

            internal override UiaCore.IRawElementProviderSimple[]? GetColumnHeaderItems() => null;

            internal MonthCalendarChildAccessibleObject GetChildFromPoint(MCHITTESTINFO hitTestInfo)
            {
                if (!_monthCalendarAccessibleObject.IsHandleCreated
                    || CalendarBodyAccessibleObject.RowsAccessibleObjects is null)
                {
                    return this;
                }

                CalendarRowAccessibleObject? rowAccessibleObject = null;

                foreach (CalendarRowAccessibleObject row in CalendarBodyAccessibleObject.RowsAccessibleObjects)
                {
                    if (row.Row == hitTestInfo.iRow)
                    {
                        rowAccessibleObject = row;
                        break;
                    }
                }

                if (rowAccessibleObject is null)
                {
                    return this;
                }

                if (hitTestInfo.uHit == MCHT.CALENDARWEEKNUM)
                {
                    return rowAccessibleObject.WeekNumberCellAccessibleObject ?? (MonthCalendarChildAccessibleObject)this;
                }

                if (rowAccessibleObject.CellsAccessibleObjects is null)
                {
                    return this;
                }

                CalendarCellAccessibleObject? cellAccessibleObject = null;

                foreach (CalendarCellAccessibleObject cell in rowAccessibleObject.CellsAccessibleObjects)
                {
                    if (cell.Column == hitTestInfo.iCol)
                    {
                        cellAccessibleObject = cell;
                        break;
                    }
                }

                if (cellAccessibleObject is null)
                {
                    return this;
                }

                return cellAccessibleObject;
            }

            internal override object? GetPropertyValue(UiaCore.UIA propertyID)
                => propertyID switch
                {
                    UiaCore.UIA.ControlTypePropertyId => UiaCore.UIA.PaneControlTypeId,
                    UiaCore.UIA.IsKeyboardFocusablePropertyId => IsEnabled,
                    _ => base.GetPropertyValue(propertyID)
                };

            internal override UiaCore.IRawElementProviderSimple[]? GetRowHeaderItems() => null;

            private protected override bool HasKeyboardFocus
                => _monthCalendarAccessibleObject.Focused
                    && _monthCalendarAccessibleObject.FocusedCell?.CalendarIndex == _calendarIndex;

            internal override bool IsPatternSupported(UiaCore.UIA patternId)
                => patternId switch
                {
                    UiaCore.UIA.GridItemPatternId => true,
                    UiaCore.UIA.TableItemPatternId => true,
                    _ => base.IsPatternSupported(patternId)
                };

            public override string Name => _initName;

            public override AccessibleObject Parent => _monthCalendarAccessibleObject;

            public override AccessibleRole Role => AccessibleRole.Client;

            internal override int Row
                => _monthCalendarAccessibleObject.IsHandleCreated
                    ? _calendarIndex / _monthCalendarAccessibleObject.ColumnCount
                    : -1;

            internal override void SetFocus()
            {
                CalendarCellAccessibleObject? focusedCell = _monthCalendarAccessibleObject.FocusedCell;
                if (focusedCell?.CalendarIndex == _calendarIndex)
                {
                    focusedCell.RaiseAutomationEvent(UiaCore.UIA.AutomationFocusChangedEventId);
                }
            }

            public override AccessibleStates State
            {
                get
                {
                    if (!IsEnabled)
                    {
                        return AccessibleStates.None;
                    }

                    AccessibleStates state = AccessibleStates.Focusable | AccessibleStates.Selectable;

                    if (HasKeyboardFocus)
                    {
                        state |= AccessibleStates.Focused | AccessibleStates.Selected;
                    }

                    return state;
                }
            }
        }
    }
}
