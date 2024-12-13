// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Windows.Win32.System.Variant;
using Windows.Win32.UI.Accessibility;

namespace System.Windows.Forms;

public partial class MonthCalendar
{
    /// <summary>
    ///  Represents the accessible object of a MonthCalendar control.
    /// </summary>
    internal sealed class MonthCalendarAccessibleObject : ControlAccessibleObject
    {
        private const int MaxCalendarsCount = 12;

        private CalendarCellAccessibleObject? _focusedCellAccessibleObject;
        private CalendarPreviousButtonAccessibleObject? _previousButtonAccessibleObject;
        private CalendarNextButtonAccessibleObject? _nextButtonAccessibleObject;
        private LinkedList<CalendarAccessibleObject>? _calendarsAccessibleObjects;
        private CalendarTodayLinkAccessibleObject? _todayLinkAccessibleObject;

        public MonthCalendarAccessibleObject(MonthCalendar owner) : base(owner)
        {
            owner.DisplayRangeChanged += OnMonthCalendarStateChanged;
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
                if (!this.IsOwnerHandleCreated(out MonthCalendar? _))
                {
                    return null;
                }

                if (_calendarsAccessibleObjects is null)
                {
                    _calendarsAccessibleObjects = new();
                    string previousHeaderName = string.Empty;

                    for (int calendarIndex = 0; calendarIndex < MaxCalendarsCount; calendarIndex++)
                    {
                        string currentHeaderName = GetCalendarPartText(MCGRIDINFO_PART.MCGIP_CALENDARHEADER, calendarIndex);

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

        // This function should be called from a single place in the root of MonthCalendar object that
        // already tests for availability of this API
        internal void DisconnectChildren()
        {
            Debug.Assert(OsVersion.IsWindows8OrGreater());

            PInvoke.UiaDisconnectProvider(_previousButtonAccessibleObject, skipOSCheck: true);
            _previousButtonAccessibleObject = null;

            PInvoke.UiaDisconnectProvider(_nextButtonAccessibleObject, skipOSCheck: true);
            _nextButtonAccessibleObject = null;

            PInvoke.UiaDisconnectProvider(_todayLinkAccessibleObject, skipOSCheck: true);
            _todayLinkAccessibleObject = null;

            PInvoke.UiaDisconnectProvider(_focusedCellAccessibleObject, skipOSCheck: true);
            _focusedCellAccessibleObject = null;

            if (_calendarsAccessibleObjects is null)
            {
                return;
            }

            foreach (CalendarAccessibleObject calendarAccessibleObject in _calendarsAccessibleObjects)
            {
                calendarAccessibleObject.DisconnectChildren();
                PInvoke.UiaDisconnectProvider(calendarAccessibleObject, skipOSCheck: true);
            }

            _calendarsAccessibleObjects.Clear();
            _calendarsAccessibleObjects = null;
        }

        /// <summary>
        ///  Associates Win API day of week values with DateTime values.
        /// </summary>
        private static DayOfWeek CastDayToDayOfWeek(Day day)
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

        internal MONTH_CALDENDAR_MESSAGES_VIEW CalendarView => this.TryGetOwnerAs(out MonthCalendar? owner) ? owner._mcCurView : MONTH_CALDENDAR_MESSAGES_VIEW.MCMV_MONTH;

        internal override int ColumnCount
        {
            get
            {
                if (!this.IsOwnerHandleCreated(out MonthCalendar? _) || CalendarsAccessibleObjects is null)
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

        internal override IRawElementProviderFragment.Interface? ElementProviderFromPoint(double x, double y)
        {
            int innerX = (int)x;
            int innerY = (int)y;

            if (!this.IsOwnerHandleCreated(out MonthCalendar? _))
            {
                return base.ElementProviderFromPoint(x, y);
            }

            MCHITTESTINFO hitTestInfo = GetHitTestInfo(innerX, innerY);

            // See "uHit" kinds in the MS doc:
            // https://docs.microsoft.com/windows/win32/api/commctrl/ns-commctrl-mchittestinfo
            return hitTestInfo.uHit switch
            {
                MCHITTESTINFO_HIT_FLAGS.MCHT_CALENDARCONTROL => this,
                MCHITTESTINFO_HIT_FLAGS.MCHT_TITLEBTNPREV or MCHITTESTINFO_HIT_FLAGS.MCHT_PREV => PreviousButtonAccessibleObject,
                MCHITTESTINFO_HIT_FLAGS.MCHT_TITLEBTNNEXT or MCHITTESTINFO_HIT_FLAGS.MCHT_NEXT => NextButtonAccessibleObject,
                MCHITTESTINFO_HIT_FLAGS.MCHT_CALENDARDATEMIN // The given point was over the minimum date in the calendar
                    => CalendarsAccessibleObjects?.First?.Value is CalendarAccessibleObject firstCalendar
                    && firstCalendar.Bounds.Contains(innerX, innerY)
                        ? firstCalendar
                        : this,
                MCHITTESTINFO_HIT_FLAGS.MCHT_CALENDARDATEMAX // The given point was over the maximum date in the calendar
                    => CalendarsAccessibleObjects?.Last?.Value is CalendarAccessibleObject lastCalendar
                    && lastCalendar.Bounds.Contains(innerX, innerY)
                        ? lastCalendar
                        : this,
                MCHITTESTINFO_HIT_FLAGS.MCHT_CALENDAR
                    // Cast "this" to AccessibleObject because "??" can't cast these operands implicitly
                    => GetCalendarFromPoint(innerX, innerY) ?? (AccessibleObject)this,
                MCHITTESTINFO_HIT_FLAGS.MCHT_TODAYLINK => TodayLinkAccessibleObject,
                MCHITTESTINFO_HIT_FLAGS.MCHT_TITLE or MCHITTESTINFO_HIT_FLAGS.MCHT_TITLEMONTH or MCHITTESTINFO_HIT_FLAGS.MCHT_TITLEYEAR
                    // Cast "this" to AccessibleObject because "??" can't cast these operands implicitly
                    => GetCalendarFromPoint(innerX, innerY)?.CalendarHeaderAccessibleObject ?? (AccessibleObject)this,
                MCHITTESTINFO_HIT_FLAGS.MCHT_CALENDARDAY or MCHITTESTINFO_HIT_FLAGS.MCHT_CALENDARWEEKNUM or MCHITTESTINFO_HIT_FLAGS.MCHT_CALENDARDATE
                or MCHITTESTINFO_HIT_FLAGS.MCHT_CALENDARDATEPREV or MCHITTESTINFO_HIT_FLAGS.MCHT_CALENDARDATENEXT
                    // Get a calendar body's child.
                    // Cast "this" to AccessibleObject because "??" can't cast these operands implicitly
                    => GetCalendarFromPoint(innerX, innerY)?.GetChildFromPoint(hitTestInfo) ?? (AccessibleObject)this,
                MCHITTESTINFO_HIT_FLAGS.MCHT_NOWHERE => this,
                _ => base.ElementProviderFromPoint(x, y)
            };
        }

        internal DayOfWeek FirstDayOfWeek => this.TryGetOwnerAs(out MonthCalendar? owner) ? CastDayToDayOfWeek(owner.FirstDayOfWeek) : CastDayToDayOfWeek(Day.Default);

        internal bool Focused => this.TryGetOwnerAs(out MonthCalendar? owner) && owner.Focused;

        internal CalendarCellAccessibleObject? FocusedCell
            => _focusedCellAccessibleObject ??= this.TryGetOwnerAs(out MonthCalendar? owner) ? GetCellByDate(owner._focusedDate) : null;

        internal override IRawElementProviderFragment.Interface? FragmentNavigate(NavigateDirection direction)
            => direction switch
            {
                NavigateDirection.NavigateDirection_FirstChild => PreviousButtonAccessibleObject,
                NavigateDirection.NavigateDirection_LastChild => ShowToday
                    ? TodayLinkAccessibleObject
                    : CalendarsAccessibleObjects?.Last?.Value,
                _ => base.FragmentNavigate(direction),
            };

        private CalendarAccessibleObject? GetCalendarFromPoint(int x, int y)
        {
            if (!this.IsOwnerHandleCreated(out MonthCalendar? _) || CalendarsAccessibleObjects is null)
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

        internal unsafe SelectionRange? GetCalendarPartDateRange(MCGRIDINFO_PART dwPart, int calendarIndex = 0, int rowIndex = 0, int columnIndex = 0)
        {
            if (!this.IsOwnerHandleCreated(out MonthCalendar? owner))
            {
                return null;
            }

            MCGRIDINFO gridInfo = new()
            {
                cbSize = (uint)sizeof(MCGRIDINFO),
                dwFlags = MCGRIDINFO_FLAGS.MCGIF_DATE,
                dwPart = dwPart,
                iCalendar = calendarIndex,
                iCol = columnIndex,
                iRow = rowIndex
            };

            bool success = PInvokeCore.SendMessage(owner, PInvoke.MCM_GETCALENDARGRIDINFO, 0, ref gridInfo) != 0;

            return success ? new((DateTime)gridInfo.stStart, (DateTime)gridInfo.stEnd) : null;
        }

        internal unsafe RECT GetCalendarPartRectangle(MCGRIDINFO_PART dwPart, int calendarIndex = 0, int rowIndex = 0, int columnIndex = 0)
        {
            if (!this.IsOwnerHandleCreated(out MonthCalendar? owner))
            {
                return default;
            }

            MCGRIDINFO gridInfo = new()
            {
                cbSize = (uint)sizeof(MCGRIDINFO),
                dwFlags = MCGRIDINFO_FLAGS.MCGIF_RECT,
                dwPart = dwPart,
                iCalendar = calendarIndex,
                iCol = columnIndex,
                iRow = rowIndex
            };

            bool success = PInvokeCore.SendMessage(owner, PInvoke.MCM_GETCALENDARGRIDINFO, 0, ref gridInfo) != 0;

            return success ? owner.RectangleToScreen(gridInfo.rc) : default;
        }

        internal unsafe string GetCalendarPartText(MCGRIDINFO_PART dwPart, int calendarIndex = 0, int rowIndex = 0, int columnIndex = 0)
        {
            if (!this.IsOwnerHandleCreated(out MonthCalendar? owner))
            {
                return string.Empty;
            }

            Span<char> name = stackalloc char[20];

            fixed (char* pName = name)
            {
                MCGRIDINFO gridInfo = new()
                {
                    cbSize = (uint)sizeof(MCGRIDINFO),
                    dwFlags = MCGRIDINFO_FLAGS.MCGIF_NAME,
                    dwPart = dwPart,
                    iCalendar = calendarIndex,
                    iCol = columnIndex,
                    iRow = rowIndex,
                    pszName = pName,
                    cchName = (UIntPtr)name.Length - 1
                };

                PInvokeCore.SendMessage(owner, PInvoke.MCM_GETCALENDARGRIDINFO, 0, ref gridInfo);
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
            if (!this.IsOwnerHandleCreated(out MonthCalendar? _) || CalendarsAccessibleObjects is null)
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

        internal override IRawElementProviderSimple.Interface[]? GetColumnHeaders() => null;

        internal SelectionRange? GetDisplayRange(bool visible)
            => this.TryGetOwnerAs(out MonthCalendar? owner) && owner.IsHandleCreated
                ? owner.GetDisplayRange(visible)
                : null;

        internal override IRawElementProviderFragment.Interface? GetFocus() => _focusedCellAccessibleObject;

        public override AccessibleObject? GetFocused() => _focusedCellAccessibleObject;

        private protected override bool IsInternal => true;

        private unsafe MCHITTESTINFO GetHitTestInfo(int xScreen, int yScreen)
        {
            if (!this.IsOwnerHandleCreated(out MonthCalendar? owner))
            {
                return default;
            }

            Point point = owner.PointToClient(new Point(xScreen, yScreen));
            MCHITTESTINFO hitTestInfo = new()
            {
                cbSize = (uint)sizeof(MCHITTESTINFO),
                pt = point
            };

            PInvokeCore.SendMessage(owner, PInvoke.MCM_HITTEST, 0, ref hitTestInfo);

            return hitTestInfo;
        }

        internal override IRawElementProviderSimple.Interface? GetItem(int row, int column)
        {
            if (!this.IsOwnerHandleCreated(out MonthCalendar? _) || CalendarsAccessibleObjects is null)
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

        internal override VARIANT GetPropertyValue(UIA_PROPERTY_ID propertyID)
            => propertyID switch
            {
                UIA_PROPERTY_ID.UIA_ControlTypePropertyId when
                    this.TryGetOwnerAs(out MonthCalendar? owner) && owner.AccessibleRole == AccessibleRole.Default
                    => (VARIANT)(int)UIA_CONTROLTYPE_ID.UIA_CalendarControlTypeId,
                UIA_PROPERTY_ID.UIA_IsKeyboardFocusablePropertyId => (VARIANT)IsEnabled,
                _ => base.GetPropertyValue(propertyID)
            };

        internal override IRawElementProviderSimple.Interface[]? GetRowHeaders() => null;

        public override string Help
        {
            get
            {
                string? help = base.Help;
                if (help is not null)
                {
                    return help;
                }

                if (this.TryGetOwnerAs(out MonthCalendar? owner) && owner.GetType().BaseType is Type baseType)
                {
                    return $"{owner.GetType().Name}({baseType.Name})";
                }

                return string.Empty;
            }
        }

        internal override bool CanGetHelpInternal => false;

        internal bool IsEnabled => this.TryGetOwnerAs(out MonthCalendar? owner) && owner.Enabled;

        internal bool IsHandleCreated => this.IsOwnerHandleCreated(out MonthCalendar? _);

        internal override bool IsPatternSupported(UIA_PATTERN_ID patternId)
            => patternId switch
            {
                UIA_PATTERN_ID.UIA_GridPatternId => true,
                UIA_PATTERN_ID.UIA_TablePatternId => true,
                UIA_PATTERN_ID.UIA_ValuePatternId => true,
                _ => base.IsPatternSupported(patternId)
            };

        internal DateTime MinDate => this.TryGetOwnerAs(out MonthCalendar? owner) ? owner.MinDate : DateTime.MinValue;
        internal DateTime MaxDate => this.TryGetOwnerAs(out MonthCalendar? owner) ? owner.MaxDate : DateTime.MaxValue;

        internal CalendarNextButtonAccessibleObject NextButtonAccessibleObject
            => _nextButtonAccessibleObject ??= new CalendarNextButtonAccessibleObject(this);

        private void OnMonthCalendarStateChanged(object? sender, EventArgs e)
        {
            RebuildAccessibilityTree();
            FocusedCell?.RaiseAutomationEvent(UIA_EVENT_ID.UIA_AutomationFocusChangedEventId);
        }

        internal CalendarPreviousButtonAccessibleObject PreviousButtonAccessibleObject
            => _previousButtonAccessibleObject ??= new CalendarPreviousButtonAccessibleObject(this);

        internal void RaiseAutomationEventForChild(UIA_EVENT_ID automationEventId)
        {
            if (!this.IsOwnerHandleCreated(out MonthCalendar? _))
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
            if (!this.IsOwnerHandleCreated(out MonthCalendar? _) || _calendarsAccessibleObjects is null)
            {
                return;
            }

            if (OsVersion.IsWindows8OrGreater())
            {
                foreach (CalendarAccessibleObject calendar in _calendarsAccessibleObjects)
                {
                    calendar.DisconnectChildren();
                    PInvoke.UiaDisconnectProvider(calendar, skipOSCheck: true);
                }

                PInvoke.UiaDisconnectProvider(_focusedCellAccessibleObject, skipOSCheck: true);
            }

            _calendarsAccessibleObjects = null;
            _focusedCellAccessibleObject = null;

            // Recreate the calendars child collection and check if it is correct
            if (CalendarsAccessibleObjects!.Count > 0)
            {
                // Get the new focused cell accessible object and try to raise the focus event for it
                FocusedCell?.RaiseAutomationEvent(UIA_EVENT_ID.UIA_AutomationFocusChangedEventId);
            }
        }

        public override AccessibleRole Role
            => this.GetOwnerAccessibleRole(AccessibleRole.Table);

        internal override int RowCount
            => ColumnCount > 0 && CalendarsAccessibleObjects is not null
                ? (int)Math.Ceiling((double)CalendarsAccessibleObjects.Count / ColumnCount)
                : 0;

        internal override RowOrColumnMajor RowOrColumnMajor => RowOrColumnMajor.RowOrColumnMajor_RowMajor;

        internal SelectionRange SelectionRange => this.TryGetOwnerAs(out MonthCalendar? owner) ? owner.SelectionRange : new SelectionRange();

        internal override void SetFocus()
            => FocusedCell?.RaiseAutomationEvent(UIA_EVENT_ID.UIA_AutomationFocusChangedEventId);

        internal void SetSelectionRange(DateTime d1, DateTime d2)
        {
            if (this.IsOwnerHandleCreated(out MonthCalendar? owner))
            {
                owner.SetSelectionRange(d1, d2);
            }
        }

        internal bool ShowToday => this.TryGetOwnerAs(out MonthCalendar? owner) && owner.ShowToday;

        internal bool ShowWeekNumbers => this.TryGetOwnerAs(out MonthCalendar? owner) && owner.ShowWeekNumbers;

        internal DateTime TodayDate => this.TryGetOwnerAs(out MonthCalendar? owner) ? owner.TodayDate : DateTime.Today;

        internal CalendarTodayLinkAccessibleObject TodayLinkAccessibleObject
            => _todayLinkAccessibleObject ??= new CalendarTodayLinkAccessibleObject(this);

        public override string? Value
        {
            get
            {
                MonthCalendar? owner;
                SelectionRange? range;

                switch (CalendarView)
                {
                    case MONTH_CALDENDAR_MESSAGES_VIEW.MCMV_MONTH:
                        if (this.TryGetOwnerAs(out owner))
                        {
                            range = owner.SelectionRange;
                            return DateTime.Equals(range.Start.Date, range.End.Date)
                                ? $"{range.Start:D}"
                                : $"{range.Start:D} - {range.End:D}";
                        }

                        return string.Empty;
                    case MONTH_CALDENDAR_MESSAGES_VIEW.MCMV_YEAR:
                        if (this.TryGetOwnerAs(out owner))
                        {
                            return $"{owner.SelectionStart:y}";
                        }

                        return string.Empty;
                    case MONTH_CALDENDAR_MESSAGES_VIEW.MCMV_DECADE:
                        if (this.TryGetOwnerAs(out owner))
                        {
                            return $"{owner.SelectionStart:yyyy}";
                        }

                        return string.Empty;
                    case MONTH_CALDENDAR_MESSAGES_VIEW.MCMV_CENTURY:
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

        internal override bool CanGetValueInternal =>
            CalendarView is not MONTH_CALDENDAR_MESSAGES_VIEW.MCMV_MONTH
                and not MONTH_CALDENDAR_MESSAGES_VIEW.MCMV_YEAR
                and not MONTH_CALDENDAR_MESSAGES_VIEW.MCMV_DECADE
                and not MONTH_CALDENDAR_MESSAGES_VIEW.MCMV_CENTURY;

        internal void UpdateDisplayRange()
        {
            if (!this.TryGetOwnerAs(out MonthCalendar? owner))
            {
                return;
            }
            else
            {
                owner.UpdateDisplayRange();
            }
        }
    }
}
