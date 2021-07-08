// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Drawing;
using static Interop;
using static Interop.ComCtl32;

namespace System.Windows.Forms
{
    public partial class MonthCalendar
    {
        /// <summary>
        ///  Represents the accessible object of a MonthCalendar control.
        /// </summary>
        internal class MonthCalendarAccessibleObject : ControlAccessibleObject
        {
            private const int MaxCalendarsCount = 12;

            private readonly MonthCalendar _owningMonthCalendar;
            private CalendarCellAccessibleObject? _focusedCellAccessibleObject;
            private CalendarPreviousButtonAccessibleObject? _previousButtonAccessibleObject;
            private CalendarNextButtonAccessibleObject? _nextButtonAccessibleObject;
            private LinkedList<CalendarAccessibleObject>? _calendarsAccessibleObjects;
            private CalendarTodayLinkAccessibleObject? _todayLinkAccessibleObject;

            public MonthCalendarAccessibleObject(MonthCalendar owner) : base(owner)
            {
                _owningMonthCalendar = owner;

                _owningMonthCalendar.DisplayRangeChanged += OnMonthCalendarStateChanged;
                _owningMonthCalendar.CalendarViewChanged += OnMonthCalendarStateChanged;
            }

            // Use a LinkedList instead a List for the following reasons:
            // 1. We don't require an access to items by indices.
            // 2. We only need the first or the last items, or iterate over all items.
            // 3. New items are only appended to the end of the collection.
            // 4. Simple API for getting an item siblings, e.g. Next or Previous values
            //    returns a real item or null.
            // 5. We have to be consistent with the rest collections of calendar parts accessible objects.
            //
            // If we use a List to store item's siblings we have to have one more variable
            // that stores a real index of the item in the collection, because _calendarIndex
            // doesn't reflect that. Or we would have to get the current index of the item
            // using IndexOf method every time.
            internal LinkedList<CalendarAccessibleObject>? CalendarsAccessibleObjects
            {
                get
                {
                    if (!_owningMonthCalendar.IsHandleCreated)
                    {
                        return null;
                    }

                    if (_calendarsAccessibleObjects is null)
                    {
                        _calendarsAccessibleObjects = new();
                        string previousHeaderName = string.Empty;

                        for (int calendarIndex = 0; calendarIndex < MaxCalendarsCount; calendarIndex++)
                        {
                            string currentHeaderName = GetCalendarPartText(MCGIP.CALENDARHEADER, calendarIndex);

                            if (currentHeaderName == string.Empty || currentHeaderName == previousHeaderName)
                            {
                                // This is a peculiarity of Win API.
                                // It returns the previous calendar name if the current one is invisible.
                                break;
                            }

                            CalendarAccessibleObject calendar = new(this, calendarIndex, currentHeaderName);
                            _calendarsAccessibleObjects.AddLast(calendar);
                            previousHeaderName = currentHeaderName;
                        }
                    }

                    return _calendarsAccessibleObjects;
                }
            }

            /// <summary>
            ///  Associates Win API day of week values with DateTime values.
            /// </summary>
            private DayOfWeek CastDayToDayOfWeek(Day day)
                => day switch
                {
                    Day.Monday => DayOfWeek.Monday,
                    Day.Tuesday => DayOfWeek.Tuesday,
                    Day.Wednesday => DayOfWeek.Wednesday,
                    Day.Thursday => DayOfWeek.Thursday,
                    Day.Friday => DayOfWeek.Friday,
                    Day.Saturday => DayOfWeek.Saturday,
                    Day.Sunday => DayOfWeek.Sunday,
                    Day.Default => DayOfWeek.Sunday,
                    _ => DayOfWeek.Sunday
                };

            internal MCMV CalendarView => _owningMonthCalendar._mcCurView;

            internal override int ColumnCount
            {
                get
                {
                    if (!_owningMonthCalendar.IsHandleCreated || CalendarsAccessibleObjects is null)
                    {
                        return -1;
                    }

                    int currentY = CalendarsAccessibleObjects.First?.Value.Bounds.Y ?? 0;
                    int columnCount = 0;

                    foreach (CalendarAccessibleObject calendar in CalendarsAccessibleObjects)
                    {
                        if (calendar.Bounds.Y > currentY)
                        {
                            break;
                        }

                        columnCount++;
                    }

                    return columnCount;
                }
            }

            internal override UiaCore.IRawElementProviderFragment? ElementProviderFromPoint(double x, double y)
            {
                int innerX = (int)x;
                int innerY = (int)y;

                if (!_owningMonthCalendar.IsHandleCreated)
                {
                    return base.ElementProviderFromPoint(x, y);
                }

                MCHITTESTINFO hitTestInfo = GetHitTestInfo(innerX, innerY);

                // See "uHit" kinds in the MS doc:
                // https://docs.microsoft.com/windows/win32/api/commctrl/ns-commctrl-mchittestinfo
                return hitTestInfo.uHit switch
                {
                    MCHT.CALENDARCONTROL => this,
                    MCHT.TITLEBTNPREV or MCHT.PREV => PreviousButtonAccessibleObject,
                    MCHT.TITLEBTNNEXT or MCHT.NEXT => NextButtonAccessibleObject,
                    MCHT.CALENDARDATEMIN // The given point was over the minimum date in the calendar
                        => CalendarsAccessibleObjects?.First?.Value is CalendarAccessibleObject firstCalendar
                        && firstCalendar.Bounds.Contains(innerX, innerY)
                            ? firstCalendar
                            : this,
                    MCHT.CALENDARDATEMAX // The given point was over the maximum date in the calendar
                        => CalendarsAccessibleObjects?.Last?.Value is CalendarAccessibleObject lastCalendar
                        && lastCalendar.Bounds.Contains(innerX, innerY)
                            ? lastCalendar
                            : this,
                    MCHT.CALENDAR
                        // Cast "this" to AccessibleObject because "??" can't cast these operands implicitly
                        => GetCalendarFromPoint(innerX, innerY) ?? (AccessibleObject)this,
                    MCHT.TODAYLINK => TodayLinkAccessibleObject,
                    MCHT.TITLE or MCHT.TITLEMONTH or MCHT.TITLEYEAR
                        // Cast "this" to AccessibleObject because "??" can't cast these operands implicitly
                        => GetCalendarFromPoint(innerX, innerY)?.CalendarHeaderAccessibleObject ?? (AccessibleObject)this,
                    MCHT.CALENDARDAY or MCHT.CALENDARWEEKNUM or MCHT.CALENDARDATE
                    or MCHT.CALENDARDATEPREV or MCHT.CALENDARDATENEXT
                        // Get a calendar body's child.
                        // Cast "this" to AccessibleObject because "??" can't cast these operands implicitly
                        => GetCalendarFromPoint(innerX, innerY)?.GetChildFromPoint(hitTestInfo) ?? (AccessibleObject)this,
                    MCHT.NOWHERE => this,
                    _ => base.ElementProviderFromPoint(x, y)
                };
            }

            internal DayOfWeek FirstDayOfWeek => CastDayToDayOfWeek(_owningMonthCalendar.FirstDayOfWeek);

            internal bool Focused => _owningMonthCalendar.Focused;

            internal CalendarCellAccessibleObject? FocusedCell
                => UiaCore.UiaClientsAreListening().IsTrue()
                    ? _focusedCellAccessibleObject ??= GetCellByDate(_owningMonthCalendar._focusedDate)
                    : null;

            internal override UiaCore.IRawElementProviderFragment? FragmentNavigate(UiaCore.NavigateDirection direction)
                => direction switch
                {
                    UiaCore.NavigateDirection.FirstChild => PreviousButtonAccessibleObject,
                    UiaCore.NavigateDirection.LastChild => ShowToday
                        ? TodayLinkAccessibleObject
                        : CalendarsAccessibleObjects?.Last?.Value,
                    _ => base.FragmentNavigate(direction),
                };

            private CalendarAccessibleObject? GetCalendarFromPoint(int x, int y)
            {
                if (!_owningMonthCalendar.IsHandleCreated || CalendarsAccessibleObjects is null)
                {
                    return null;
                }

                foreach (CalendarAccessibleObject calendar in CalendarsAccessibleObjects)
                {
                    if (calendar.Bounds.Contains(x, y))
                    {
                        return calendar;
                    }
                }

                return null;
            }

            internal unsafe SelectionRange? GetCalendarPartDateRange(MCGIP dwPart, int calendarIndex = 0, int rowIndex = 0, int columnIndex = 0)
            {
                if (!_owningMonthCalendar.IsHandleCreated)
                {
                    return null;
                }

                MCGRIDINFO gridInfo = new()
                {
                    cbSize = (uint)sizeof(MCGRIDINFO),
                    dwFlags = MCGIF.DATE,
                    dwPart = dwPart,
                    iCalendar = calendarIndex,
                    iCol = columnIndex,
                    iRow = rowIndex
                };

                bool success = User32.SendMessageW(_owningMonthCalendar, (User32.WM)MCM.GETCALENDARGRIDINFO, IntPtr.Zero, ref gridInfo) != IntPtr.Zero;

                return success ? new(gridInfo.stStart, gridInfo.stEnd) : null;
            }

            internal unsafe RECT GetCalendarPartRectangle(MCGIP dwPart, int calendarIndex = 0, int rowIndex = 0, int columnIndex = 0)
            {
                if (!_owningMonthCalendar.IsHandleCreated)
                {
                    return default;
                }

                MCGRIDINFO gridInfo = new()
                {
                    cbSize = (uint)sizeof(MCGRIDINFO),
                    dwFlags = MCGIF.RECT,
                    dwPart = dwPart,
                    iCalendar = calendarIndex,
                    iCol = columnIndex,
                    iRow = rowIndex
                };

                bool success = User32.SendMessageW(_owningMonthCalendar, (User32.WM)MCM.GETCALENDARGRIDINFO, IntPtr.Zero, ref gridInfo) != IntPtr.Zero;

                return success ? _owningMonthCalendar.RectangleToScreen(gridInfo.rc) : default;
            }

            internal unsafe string GetCalendarPartText(MCGIP dwPart, int calendarIndex = 0, int rowIndex = 0, int columnIndex = 0)
            {
                if (!_owningMonthCalendar.IsHandleCreated)
                {
                    return string.Empty;
                }

                const int nameLength = 20;
                Span<char> name = stackalloc char[nameLength];

                fixed (char* pName = name)
                {
                    MCGRIDINFO gridInfo = new()
                    {
                        cbSize = (uint)sizeof(MCGRIDINFO),
                        dwFlags = MCGIF.NAME,
                        dwPart = dwPart,
                        iCalendar = calendarIndex,
                        iCol = columnIndex,
                        iRow = rowIndex,
                        pszName = pName,
                        cchName = (UIntPtr)name.Length - 1
                    };

                    User32.SendMessageW(_owningMonthCalendar, (User32.WM)MCM.GETCALENDARGRIDINFO, IntPtr.Zero, ref gridInfo);
                }

                string text = string.Empty;

                foreach (char ch in name)
                {
                    // Remove special invisible symbols
                    if (ch is not '\0' and not (char)8206 /*empty symbol*/)
                    {
                        text += ch;
                    }
                }

                return text;
            }

            private CalendarCellAccessibleObject? GetCellByDate(DateTime date)
            {
                if (!_owningMonthCalendar.IsHandleCreated || CalendarsAccessibleObjects is null)
                {
                    return null;
                }

                foreach (CalendarAccessibleObject calendar in CalendarsAccessibleObjects)
                {
                    if (calendar.DateRange is null)
                    {
                        continue;
                    }

                    DateTime calendarStart = calendar.DateRange.Start;
                    DateTime calendarEnd = calendar.DateRange.End;

                    if (date < calendarStart || date > calendarEnd)
                    {
                        continue;
                    }

                    LinkedList<CalendarRowAccessibleObject>? rows = calendar.CalendarBodyAccessibleObject.RowsAccessibleObjects;
                    if (rows is null)
                    {
                        return null;
                    }

                    foreach (CalendarRowAccessibleObject row in rows)
                    {
                        if (row.CellsAccessibleObjects is null)
                        {
                            return null;
                        }

                        foreach (CalendarCellAccessibleObject cell in row.CellsAccessibleObjects)
                        {
                            // A cell date range may be null for header cells
                            SelectionRange? cellRange = cell.DateRange;
                            if (cellRange is null)
                            {
                                continue;
                            }

                            if (date >= cellRange.Start && date <= cellRange.End)
                            {
                                return cell;
                            }
                        }
                    }
                }

                return null;
            }

            internal override UiaCore.IRawElementProviderSimple[]? GetColumnHeaders() => null;

            internal SelectionRange? GetDisplayRange(bool visible)
                => _owningMonthCalendar.IsHandleCreated
                    ? _owningMonthCalendar.GetDisplayRange(visible)
                    : null;

            internal override UiaCore.IRawElementProviderFragment? GetFocus() => _focusedCellAccessibleObject;

            public override AccessibleObject? GetFocused() => _focusedCellAccessibleObject;

            private unsafe MCHITTESTINFO GetHitTestInfo(int xScreen, int yScreen)
            {
                if (!_owningMonthCalendar.IsHandleCreated)
                {
                    return default;
                }

                Point point = _owningMonthCalendar.PointToClient(new Point(xScreen, yScreen));
                MCHITTESTINFO hitTestInfo = new()
                {
                    cbSize = (uint)sizeof(MCHITTESTINFO),
                    pt = point
                };

                User32.SendMessageW(_owningMonthCalendar, (User32.WM)MCM.HITTEST, IntPtr.Zero, ref hitTestInfo);

                return hitTestInfo;
            }

            internal override UiaCore.IRawElementProviderSimple? GetItem(int row, int column)
            {
                if (!_owningMonthCalendar.IsHandleCreated || CalendarsAccessibleObjects is null)
                {
                    return null;
                }

                foreach (CalendarAccessibleObject calendar in CalendarsAccessibleObjects)
                {
                    if (calendar.Row == row && calendar.Column == column)
                    {
                        return calendar;
                    }
                }

                return null;
            }

            internal override object? GetPropertyValue(UiaCore.UIA propertyID)
                => propertyID switch
                {
                    UiaCore.UIA.ControlTypePropertyId => _owningMonthCalendar.AccessibleRole == AccessibleRole.Default
                        ? UiaCore.UIA.CalendarControlTypeId
                        : base.GetPropertyValue(propertyID),
                    UiaCore.UIA.NamePropertyId => Name,
                    UiaCore.UIA.IsKeyboardFocusablePropertyId => IsEnabled,
                    UiaCore.UIA.IsGridPatternAvailablePropertyId => IsPatternSupported(UiaCore.UIA.GridPatternId),
                    UiaCore.UIA.IsTablePatternAvailablePropertyId => IsPatternSupported(UiaCore.UIA.TablePatternId),
                    UiaCore.UIA.IsValuePatternAvailablePropertyId => IsPatternSupported(UiaCore.UIA.ValuePatternId),
                    UiaCore.UIA.IsEnabledPropertyId => _owningMonthCalendar.Enabled,
                    UiaCore.UIA.LegacyIAccessibleStatePropertyId => State,
                    _ => base.GetPropertyValue(propertyID)
                };

            internal override UiaCore.IRawElementProviderSimple[]? GetRowHeaders() => null;

            public override string Help
            {
                get
                {
                    string? help = base.Help;
                    if (help is not null)
                    {
                        return help;
                    }

                    if (_owningMonthCalendar.GetType().BaseType is Type baseType)
                    {
                        return $"{_owningMonthCalendar.GetType().Name}({baseType.Name})";
                    }

                    return string.Empty;
                }
            }

            internal bool IsEnabled => _owningMonthCalendar.Enabled;

            internal bool IsHandleCreated => _owningMonthCalendar.IsHandleCreated;

            internal override bool IsPatternSupported(UiaCore.UIA patternId)
                => patternId switch
                {
                    UiaCore.UIA.GridPatternId => true,
                    UiaCore.UIA.TablePatternId => true,
                    UiaCore.UIA.ValuePatternId => true,
                    _ => base.IsPatternSupported(patternId)
                };

            internal DateTime MinDate => _owningMonthCalendar.MinDate;
            internal DateTime MaxDate => _owningMonthCalendar.MaxDate;

            internal CalendarNextButtonAccessibleObject NextButtonAccessibleObject
                => _nextButtonAccessibleObject ??= new CalendarNextButtonAccessibleObject(this);

            private void OnMonthCalendarStateChanged(object? sender, EventArgs e)
            {
                RebuildAccessibilityTree();
                FocusedCell?.RaiseAutomationEvent(UiaCore.UIA.AutomationFocusChangedEventId);
            }

            internal CalendarPreviousButtonAccessibleObject PreviousButtonAccessibleObject
                => _previousButtonAccessibleObject ??= new CalendarPreviousButtonAccessibleObject(this);

            internal void RaiseAutomationEventForChild(UiaCore.UIA automationEventId)
            {
                if (!_owningMonthCalendar.IsHandleCreated)
                {
                    return;
                }

                if (_calendarsAccessibleObjects is null)
                {
                    // It means that there are no any accessibility listeners
                    // that should build the accessibility tree before.
                    // If we try to get the focused cell accessibility object
                    // the accessibility tree will be built even a user doesn't use any accessibility tool.
                    return;
                }

                // Update the focused cell and raise the focus event for it
                _focusedCellAccessibleObject = null;
                FocusedCell?.RaiseAutomationEvent(automationEventId);
            }

            private void RebuildAccessibilityTree()
            {
                if (!_owningMonthCalendar.IsHandleCreated || CalendarsAccessibleObjects is null)
                {
                    return;
                }

                foreach (CalendarAccessibleObject calendar in CalendarsAccessibleObjects)
                {
                    calendar.CalendarBodyAccessibleObject.ClearChildCollection();
                }

                _calendarsAccessibleObjects = null;
                _focusedCellAccessibleObject = null;

                // Recreate the calendars child collection and check if it is correct
                if (CalendarsAccessibleObjects.Count > 0)
                {
                    // Get the new focused cell accessible object and try to raise the focus event for it
                    FocusedCell?.RaiseAutomationEvent(UiaCore.UIA.AutomationFocusChangedEventId);
                }
            }

            public override AccessibleRole Role
                => _owningMonthCalendar.AccessibleRole == AccessibleRole.Default
                    ? AccessibleRole.Table
                    : _owningMonthCalendar.AccessibleRole;

            internal override int RowCount
                => ColumnCount > 0 && CalendarsAccessibleObjects is not null
                    ? (int)Math.Ceiling((double)CalendarsAccessibleObjects.Count / ColumnCount)
                    : 0;

            internal override UiaCore.RowOrColumnMajor RowOrColumnMajor => UiaCore.RowOrColumnMajor.RowMajor;

            internal SelectionRange SelectionRange => _owningMonthCalendar.SelectionRange;

            internal override void SetFocus()
                => FocusedCell?.RaiseAutomationEvent(UiaCore.UIA.AutomationFocusChangedEventId);

            internal void SetSelectionRange(DateTime d1, DateTime d2)
            {
                if (_owningMonthCalendar.IsHandleCreated)
                {
                    _owningMonthCalendar.SetSelectionRange(d1, d2);
                }
            }

            internal bool ShowToday => _owningMonthCalendar.ShowToday;

            internal bool ShowWeekNumbers => _owningMonthCalendar.ShowWeekNumbers;

            internal DateTime TodayDate => _owningMonthCalendar.TodayDate;

            internal CalendarTodayLinkAccessibleObject TodayLinkAccessibleObject
                => _todayLinkAccessibleObject ??= new CalendarTodayLinkAccessibleObject(this);

            public override string? Value
            {
                get
                {
                    SelectionRange? range;

                    switch (CalendarView)
                    {
                        case MCMV.MONTH:
                            range = _owningMonthCalendar.SelectionRange;

                            return DateTime.Equals(range.Start.Date, range.End.Date)
                                ? $"{range.Start:D}"
                                : $"{range.Start:D} - {range.End:D}";
                        case MCMV.YEAR:
                            return $"{_owningMonthCalendar.SelectionStart:y}";
                        case MCMV.DECADE:
                            return $"{_owningMonthCalendar.SelectionStart:yyyy}";
                        case MCMV.CENTURY:
                            range = FocusedCell?.DateRange;
                            if (range is null)
                            {
                                return string.Empty;
                            }

                            return $"{range.Start:yyyy} - {range.End:yyyy}";
                        default:
                            return base.Value;
                    }
                }
            }

            internal void UpdateDisplayRange() => _owningMonthCalendar.UpdateDisplayRange();
        }
    }
}
