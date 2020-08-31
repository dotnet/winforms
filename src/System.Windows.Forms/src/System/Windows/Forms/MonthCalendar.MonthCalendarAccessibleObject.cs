// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable disable

using System.Diagnostics;
using System.Drawing;
using System.Runtime.InteropServices;
using static Interop;
using static Interop.ComCtl32;

namespace System.Windows.Forms
{
    public partial class MonthCalendar
    {
        internal class MonthCalendarAccessibleObject : ControlAccessibleObject
        {
            internal const int MAX_DAYS = 7;
            internal const int MAX_WEEKS = 6;

            private readonly MonthCalendar _owner;
#pragma warning disable CA1805 // Do not initialize unnecessarily
            private int _calendarIndex = 0;
#pragma warning restore CA1805 // Do not initialize unnecessarily
            private AccessibleObject _focused;

            public MonthCalendarAccessibleObject(Control owner)
                : base(owner)
            {
                _owner = (MonthCalendar)owner;
            }

            public UiaCore.UIA ControlType =>
                string.IsNullOrEmpty(base.Name) ? UiaCore.UIA.CalendarControlTypeId : UiaCore.UIA.TableControlTypeId;

            public bool Enabled => _owner.Enabled;

            public bool HasHeaderRow
            {
                get
                {
                    if (!_owner.IsHandleCreated)
                    {
                        return false;
                    }

                    bool result = GetCalendarGridInfoText(MCGIP.CALENDARCELL, _calendarIndex, -1, 0, out string text);
                    if (!result || string.IsNullOrEmpty(text))
                    {
                        return false;
                    }

                    return true;
                }
            }

            public override AccessibleRole Role =>
                (_owner?.AccessibleRole != AccessibleRole.Default) ? _owner.AccessibleRole : AccessibleRole.Table;

            public override string Help
            {
                get
                {
                    var help = base.Help;
                    if (help != null)
                    {
                        return help;
                    }
                    else
                    {
                        if (_owner != null)
                        {
                            return _owner.GetType().Name + "(" + _owner.GetType().BaseType.Name + ")";
                        }
                    }
                    return string.Empty;
                }
            }

            public override string Name
            {
                get
                {
                    string name = base.Name;
                    if (name != null)
                    {
                        return name;
                    }

                    name = string.Empty;

                    if (_owner._mcCurView == MCMV.MONTH)
                    {
                        if (DateTime.Equals(_owner.SelectionStart.Date, _owner.SelectionEnd.Date))
                        {
                            return string.Format(SR.MonthCalendarSingleDateSelected, _owner.SelectionStart.ToLongDateString());
                        }
                        else
                        {
                            return string.Format(SR.MonthCalendarRangeSelected, _owner.SelectionStart.ToLongDateString(), _owner.SelectionEnd.ToLongDateString());
                        }
                    }
                    else if (_owner._mcCurView == MCMV.YEAR)
                    {
                        if (DateTime.Equals(_owner.SelectionStart.Month, _owner.SelectionEnd.Month))
                        {
                            return string.Format(SR.MonthCalendarSingleDateSelected, _owner.SelectionStart.ToString("y"));
                        }
                        else
                        {
                            return string.Format(SR.MonthCalendarRangeSelected, _owner.SelectionStart.ToString("y"), _owner.SelectionEnd.ToString("y"));
                        }
                    }
                    else if (_owner._mcCurView == MCMV.DECADE)
                    {
                        if (DateTime.Equals(_owner.SelectionStart.Year, _owner.SelectionEnd.Year))
                        {
                            return string.Format(SR.MonthCalendarSingleYearSelected, _owner.SelectionStart.ToString("yyyy"));
                        }
                        else
                        {
                            return string.Format(SR.MonthCalendarYearRangeSelected, _owner.SelectionStart.ToString("yyyy"), _owner.SelectionEnd.ToString("yyyy"));
                        }
                    }
                    else if (_owner._mcCurView == MCMV.CENTURY)
                    {
                        return string.Format(SR.MonthCalendarSingleDecadeSelected, _owner.SelectionStart.ToString("yyyy"));
                    }

                    return name;
                }
            }

            public override string Value
            {
                get
                {
                    try
                    {
                        if (_owner._mcCurView == MCMV.MONTH)
                        {
                            if (System.DateTime.Equals(_owner.SelectionStart.Date, _owner.SelectionEnd.Date))
                            {
                                return _owner.SelectionStart.ToLongDateString();
                            }

                            return string.Format("{0} - {1}", _owner.SelectionStart.ToLongDateString(), _owner.SelectionEnd.ToLongDateString());
                        }

                        if (_owner._mcCurView == MCMV.YEAR)
                        {
                            if (System.DateTime.Equals(_owner.SelectionStart.Month, _owner.SelectionEnd.Month))
                            {
                                return _owner.SelectionStart.ToString("y");
                            }

                            return string.Format("{0} - {1}", _owner.SelectionStart.ToString("y"), _owner.SelectionEnd.ToString("y"));
                        }

                        return string.Format("{0} - {1}", _owner.SelectionRange.Start.ToString(), _owner.SelectionRange.End.ToString());
                    }
                    catch
                    {
                        return base.Value;
                    }
                }
                set => base.Value = value;
            }

            internal override int ColumnCount
            {
                get
                {
                    if (!_owner.IsHandleCreated)
                    {
                        return 0;
                    }

                    GetCalendarGridInfo(
                        MCGIF.RECT,
                        MCGIP.CALENDARBODY,
                        _calendarIndex,
                        -1,
                        -1,
                        out RECT calendarBodyRectangle,
                        out Kernel32.SYSTEMTIME endDate,
                        out Kernel32.SYSTEMTIME startDate);

                    int columnCount = 0;
                    bool success = true;

                    while (success)
                    {
                        success = GetCalendarGridInfo(
                            MCGIF.RECT,
                            MCGIP.CALENDARCELL,
                            _calendarIndex,
                            0,
                            columnCount,
                            out RECT calendarPartRectangle,
                            out endDate,
                            out startDate);

                        // Out of the body, so this is out of the grid column.
                        if (calendarPartRectangle.right > calendarBodyRectangle.right)
                        {
                            break;
                        }

                        columnCount++;
                    }

                    return columnCount;
                }
            }

            internal override int RowCount
            {
                get
                {
                    if (!_owner.IsHandleCreated)
                    {
                        return 0;
                    }

                    GetCalendarGridInfo(
                        MCGIF.RECT,
                        MCGIP.CALENDARBODY,
                        _calendarIndex,
                        -1,
                        -1,
                        out RECT calendarBodyRectangle,
                        out Kernel32.SYSTEMTIME endDate,
                        out Kernel32.SYSTEMTIME startDate);

                    int rowCount = 0;
                    bool success = true;

                    while (success)
                    {
                        success = GetCalendarGridInfo(
                            MCGIF.RECT,
                            MCGIP.CALENDARCELL,
                            _calendarIndex,
                            rowCount,
                            0,
                            out RECT calendarPartRectangle,
                            out endDate,
                            out startDate);

                        // Out of the body, so this is out of the grid row.
                        if (calendarPartRectangle.bottom > calendarBodyRectangle.bottom)
                        {
                            break;
                        }

                        rowCount++;
                    }

                    return rowCount;
                }
            }

            internal override UiaCore.RowOrColumnMajor RowOrColumnMajor => UiaCore.RowOrColumnMajor.RowMajor;

            internal override UiaCore.IRawElementProviderSimple[] GetRowHeaderItems() => null;

            internal override UiaCore.IRawElementProviderFragment ElementProviderFromPoint(double x, double y)
            {
                int innerX = (int)x;
                int innerY = (int)y;

                if (!_owner.IsHandleCreated)
                {
                    return base.ElementProviderFromPoint(x, y);
                }

                MCHITTESTINFO hitTestInfo = GetHitTestInfo(innerX, innerY);

                switch ((MCHT)hitTestInfo.uHit)
                {
                    case MCHT.TITLEBTNPREV:
                        return GetCalendarChildAccessibleObject(_calendarIndex, CalendarChildType.PreviousButton);

                    case MCHT.TITLEBTNNEXT:
                        return GetCalendarChildAccessibleObject(_calendarIndex, CalendarChildType.NextButton);

                    case MCHT.TITLE:
                    case MCHT.TITLEMONTH:
                    case MCHT.TITLEYEAR:
                        return GetCalendarChildAccessibleObject(_calendarIndex, CalendarChildType.CalendarHeader);

                    case MCHT.CALENDARDAY:
                    case MCHT.CALENDARWEEKNUM:
                    case MCHT.CALENDARDATE:
                        // Get calendar body's child.
                        CalendarBodyAccessibleObject calendarBodyAccessibleObject = (CalendarBodyAccessibleObject)GetCalendarChildAccessibleObject(_calendarIndex, CalendarChildType.CalendarBody);
                        return calendarBodyAccessibleObject?.GetFromPoint(hitTestInfo);

                    case MCHT.TODAYLINK:
                        return GetCalendarChildAccessibleObject(_calendarIndex, CalendarChildType.TodayLink);
                }

                return base.ElementProviderFromPoint(x, y);
            }

            internal override UiaCore.IRawElementProviderFragment FragmentNavigate(UiaCore.NavigateDirection direction)
            {
                switch (direction)
                {
                    case UiaCore.NavigateDirection.FirstChild:
                        return GetCalendarChildAccessibleObject(_calendarIndex, CalendarChildType.PreviousButton);
                    case UiaCore.NavigateDirection.LastChild:
                        return _owner.ShowTodayCircle
                            ? GetCalendarChildAccessibleObject(_calendarIndex, CalendarChildType.TodayLink)
                            : GetCalendarChildAccessibleObject(_calendarIndex, CalendarChildType.CalendarBody);
                }

                return base.FragmentNavigate(direction);
            }

            internal override UiaCore.IRawElementProviderFragment GetFocus() => _focused;

            public override AccessibleObject GetFocused() => _focused;

            public unsafe MCHITTESTINFO GetHitTestInfo(int xScreen, int yScreen)
            {
                if (!_owner.IsHandleCreated || HandleInternal == IntPtr.Zero)
                {
                    return new MCHITTESTINFO();
                }

                Point point = new Point(xScreen, yScreen);
                User32.MapWindowPoints(IntPtr.Zero, HandleInternal, ref point, 1);
                var hitTestInfo = new MCHITTESTINFO
                {
                    cbSize = (uint)sizeof(MCHITTESTINFO),
                    pt = point,
                    st = new Kernel32.SYSTEMTIME()
                };

                User32.SendMessageW(_owner, (User32.WM)MCM.HITTEST, IntPtr.Zero, ref hitTestInfo);
                return hitTestInfo;
            }

            public CalendarChildAccessibleObject GetCalendarChildAccessibleObject(int calendarIndex, CalendarChildType calendarChildType, AccessibleObject parentAccessibleObject = null, int index = -1) =>
                 calendarChildType switch
                 {
                     CalendarChildType.PreviousButton => new CalendarPreviousButtonAccessibleObject(this, _calendarIndex),
                     CalendarChildType.NextButton => new CalendarNextButtonAccessibleObject(this, _calendarIndex),
                     CalendarChildType.CalendarHeader => new CalendarHeaderAccessibleObject(this, _calendarIndex),
                     CalendarChildType.CalendarBody => new CalendarBodyAccessibleObject(this, _calendarIndex),
                     CalendarChildType.CalendarRow => GetCalendarRow(calendarIndex, parentAccessibleObject, index),
                     CalendarChildType.CalendarCell => GetCalendarCell(calendarIndex, parentAccessibleObject, index),
                     CalendarChildType.TodayLink => new CalendarTodayLinkAccessibleObject(this, (int)CalendarChildType.TodayLink, calendarChildType),
                     _ => null
                 };

            public string GetCalendarChildName(int calendarIndex, CalendarChildType calendarChildType, AccessibleObject parentAccessibleObject = null, int index = -1)
            {
                switch (calendarChildType)
                {
                    case CalendarChildType.CalendarHeader:
                        GetCalendarGridInfoText(MCGIP.CALENDARHEADER, calendarIndex, 0, 0, out string text);
                        return text;
                    case CalendarChildType.TodayLink:
                        return string.Format(SR.MonthCalendarTodayButtonAccessibleName, _owner.TodayDate.ToShortDateString());
                };

                return string.Empty;
            }

            private CalendarCellAccessibleObject GetCalendarCell(int calendarIndex, AccessibleObject parentAccessibleObject, int columnIndex)
            {
                if (parentAccessibleObject is null ||
                    !_owner.IsHandleCreated ||
                    columnIndex < 0 ||
                    columnIndex >= MAX_DAYS ||
                    columnIndex >= ColumnCount)
                {
                    return null;
                }

                CalendarRowAccessibleObject parentRowAccessibleObject = (CalendarRowAccessibleObject)parentAccessibleObject;
                int rowIndex = parentRowAccessibleObject.RowIndex;
                bool getNameResult = GetCalendarGridInfoText(MCGIP.CALENDARCELL, calendarIndex, rowIndex, columnIndex, out string text);
                bool getDateResult = GetCalendarGridInfo(MCGIF.DATE, MCGIP.CALENDARCELL,
                    calendarIndex,
                    rowIndex,
                    columnIndex,
                    out RECT rectangle,
                    out Kernel32.SYSTEMTIME systemEndDate,
                    out Kernel32.SYSTEMTIME systemStartDate);

                if (getNameResult && !string.IsNullOrEmpty(text))
                {
                    string cellName = string.Empty;

                    if (getDateResult)
                    {
                        DateTime endDate = DateTimePicker.SysTimeToDateTime(systemEndDate).Date;
                        DateTime startDate = DateTimePicker.SysTimeToDateTime(systemStartDate).Date;
                        cellName = GetCalendarCellName(endDate, startDate, text, rowIndex == -1);
                    }

                    // The cell is present on the calendar, so create accessible object for it.
                    return new CalendarCellAccessibleObject(this, calendarIndex, parentAccessibleObject, rowIndex, columnIndex, cellName);
                }

                return null;
            }

            private string GetCalendarCellName(DateTime endDate, DateTime startDate, string defaultName, bool headerCell)
            {
                if (_owner._mcCurView == MCMV.MONTH)
                {
                    if (headerCell)
                    {
                        return startDate.ToString("dddd");
                    }

                    return startDate.ToString("dddd, MMMM dd, yyyy");
                }
                else if (_owner._mcCurView == MCMV.YEAR)
                {
                    return startDate.ToString("MMMM yyyy");
                }

                return defaultName;
            }

            private CalendarRowAccessibleObject GetCalendarRow(int calendarIndex, AccessibleObject parentAccessibleObject, int rowIndex)
            {
                if (parentAccessibleObject is null ||
                    !_owner.IsHandleCreated ||
                    (HasHeaderRow ? rowIndex < -1 : rowIndex < 0) ||
                    rowIndex >= RowCount)
                {
                    return null;
                }

                // Search name for the first cell in the row.
                bool success = GetCalendarGridInfo(
                    MCGIF.DATE,
                    MCGIP.CALENDARCELL,
                    calendarIndex,
                    rowIndex,
                    0,
                    out RECT calendarPartRectangle,
                    out Kernel32.SYSTEMTIME endDate,
                    out Kernel32.SYSTEMTIME startDate);

                if (!success)
                {
                    // Not able to get cell date for the row.
                    return null;
                }

                SelectionRange cellsRange = _owner.GetDisplayRange(false);

                if (cellsRange is null || cellsRange.Start > DateTimePicker.SysTimeToDateTime(endDate) || cellsRange.End < DateTimePicker.SysTimeToDateTime(startDate))
                {
                    // Do not create row if the row's first cell is out of the current calendar's view range.
                    return null;
                }

                return new CalendarRowAccessibleObject(this, calendarIndex, (CalendarBodyAccessibleObject)parentAccessibleObject, rowIndex);
            }

            private unsafe bool GetCalendarGridInfo(
                MCGIF dwFlags,
                MCGIP dwPart,
                int calendarIndex,
                int row,
                int column,
                out RECT rectangle,
                out Kernel32.SYSTEMTIME endDate,
                out Kernel32.SYSTEMTIME startDate)
            {
                Debug.Assert(
                    (dwFlags & ~(MCGIF.DATE | MCGIF.RECT)) == 0,
                    "GetCalendarGridInfo() should be used only to obtain Date and Rect,"
                    + "dwFlags has flag bits other that MCGIF_DATE and MCGIF_RECT");

                if (!_owner.IsHandleCreated)
                {
                    rectangle = default;
                    endDate = default;
                    startDate = default;
                    return false;
                }

                var gridInfo = new MCGRIDINFO
                {
                    cbSize = (uint)sizeof(MCGRIDINFO),
                    dwFlags = dwFlags,
                    dwPart = dwPart,
                    iCalendar = calendarIndex,
                    iCol = column,
                    iRow = row
                };

                try
                {
                    bool result = GetCalendarGridInfo(ref gridInfo);
                    rectangle = gridInfo.rc;
                    endDate = gridInfo.stEnd;
                    startDate = gridInfo.stStart;
                    return result;
                }
                catch
                {
                    rectangle = new RECT();
                    endDate = new Kernel32.SYSTEMTIME();
                    startDate = new Kernel32.SYSTEMTIME();
                    return false;
                }
            }

            private bool GetCalendarGridInfo(ref MCGRIDINFO gridInfo)
            {
                if (!_owner.IsHandleCreated)
                {
                    return false;
                }

                // Do not use this if gridInfo.dwFlags contains MCGIF_NAME;
                // use GetCalendarGridInfoText() instead.
                Debug.Assert(
                    (gridInfo.dwFlags & MCGIF.NAME) == 0,
                    "Param dwFlags contains MCGIF_NAME, use GetCalendarGridInfoText() to retrieve the text of a calendar part.");

                gridInfo.dwFlags &= ~MCGIF.NAME;

                return User32.SendMessageW(_owner, (User32.WM)MCM.GETCALENDARGRIDINFO, IntPtr.Zero, ref gridInfo) != IntPtr.Zero;
            }

            private unsafe bool GetCalendarGridInfoText(MCGIP dwPart, int calendarIndex, int row, int column, out string text)
            {
                if (!_owner.IsHandleCreated)
                {
                    text = string.Empty;
                    return false;
                }

                const int nameLength = 128;
                Span<char> name = stackalloc char[nameLength + 2];

                bool result;
                fixed (char* pName = name)
                {
                    var gridInfo = new MCGRIDINFO
                    {
                        cbSize = (uint)sizeof(MCGRIDINFO),
                        dwFlags = MCGIF.NAME,
                        dwPart = dwPart,
                        iCalendar = calendarIndex,
                        iCol = column,
                        iRow = row,
                        pszName = pName,
                        cchName = (UIntPtr)name.Length - 1
                    };

                    result = User32.SendMessageW(_owner, (User32.WM)MCM.GETCALENDARGRIDINFO, IntPtr.Zero, ref gridInfo) != IntPtr.Zero;
                }

                text = name.SliceAtFirstNull().ToString();
                return result;
            }

            public bool GetCalendarPartRectangle(int calendarIndex, MCGIP dwPart, int row, int column, out RECT calendarPartRectangle)
            {
                if (!_owner.IsHandleCreated)
                {
                    calendarPartRectangle = default;
                    return false;
                }

                bool success = GetCalendarGridInfo(
                    MCGIF.RECT,
                    dwPart,
                    calendarIndex,
                    row,
                    column,
                    out calendarPartRectangle,
                    out Kernel32.SYSTEMTIME endDate, out Kernel32.SYSTEMTIME startDate);

                if (success)
                {
                    success = User32.MapWindowPoints(new HandleRef(this, Owner.Handle), new HandleRef(null, IntPtr.Zero), ref calendarPartRectangle, 2) != 0;
                }

                if (!success)
                {
                    calendarPartRectangle = new RECT();
                }

                return success;
            }

            internal override object GetPropertyValue(UiaCore.UIA propertyID) =>
                propertyID switch
                {
                    UiaCore.UIA.ControlTypePropertyId => ControlType,
                    UiaCore.UIA.NamePropertyId => Name,
                    UiaCore.UIA.IsGridPatternAvailablePropertyId => true,
                    UiaCore.UIA.IsTablePatternAvailablePropertyId => true,
                    UiaCore.UIA.IsLegacyIAccessiblePatternAvailablePropertyId => true,
                    _ => base.GetPropertyValue(propertyID)
                };

            internal override bool IsPatternSupported(UiaCore.UIA patternId) =>
                patternId switch
                {
                    var p when
                        p == UiaCore.UIA.ValuePatternId ||
                        p == UiaCore.UIA.GridPatternId ||
                        p == UiaCore.UIA.TablePatternId => true,
                    _ => base.IsPatternSupported(patternId)
                };

            public void RaiseMouseClick(int x, int y)
            {
                var previousPosition = new Point();
                BOOL setOldCursorPos = User32.GetPhysicalCursorPos(ref previousPosition);

                bool mouseSwapped = User32.GetSystemMetrics(User32.SystemMetric.SM_SWAPBUTTON) != 0;

                SendMouseInput(x, y, User32.MOUSEEVENTF.MOVE | User32.MOUSEEVENTF.ABSOLUTE);
                SendMouseInput(0, 0, mouseSwapped ? User32.MOUSEEVENTF.RIGHTDOWN : User32.MOUSEEVENTF.LEFTDOWN);
                SendMouseInput(0, 0, mouseSwapped ? User32.MOUSEEVENTF.RIGHTUP : User32.MOUSEEVENTF.LEFTUP);

                Threading.Thread.Sleep(50);

                // Set back the mouse position where it was.
                if (setOldCursorPos.IsTrue())
                {
                    SendMouseInput(previousPosition.X, previousPosition.Y, User32.MOUSEEVENTF.MOVE | User32.MOUSEEVENTF.ABSOLUTE);
                }
            }

            private unsafe void SendMouseInput(int x, int y, User32.MOUSEEVENTF flags)
            {
                if ((flags & User32.MOUSEEVENTF.ABSOLUTE) != 0)
                {
                    int vscreenWidth = User32.GetSystemMetrics(User32.SystemMetric.SM_CXVIRTUALSCREEN);
                    int vscreenHeight = User32.GetSystemMetrics(User32.SystemMetric.SM_CYVIRTUALSCREEN);
                    int vscreenLeft = User32.GetSystemMetrics(User32.SystemMetric.SM_XVIRTUALSCREEN);
                    int vscreenTop = User32.GetSystemMetrics(User32.SystemMetric.SM_YVIRTUALSCREEN);

                    const int DesktopNormilizedMax = 65536;

                    // Absolute input requires that input is in 'normalized' coords - with the entire
                    // desktop being (0,0)...(65535,65536). Need to convert input x,y coords to this
                    // first.
                    //
                    // In this normalized world, any pixel on the screen corresponds to a block of values
                    // of normalized coords - eg. on a 1024x768 screen,
                    // y pixel 0 corresponds to range 0 to 85.333,
                    // y pixel 1 corresponds to range 85.333 to 170.666,
                    // y pixel 2 correpsonds to range 170.666 to 256 - and so on.
                    // Doing basic scaling math - (x-top)*65536/Width - gets us the start of the range.
                    // However, because int math is used, this can end up being rounded into the wrong
                    // pixel. For example, if we wanted pixel 1, we'd get 85.333, but that comes out as
                    // 85 as an int, which falls into pixel 0's range - and that's where the pointer goes.
                    // To avoid this, we add on half-a-"screen pixel"'s worth of normalized coords - to
                    // push us into the middle of any given pixel's range - that's the 65536/(Width*2)
                    // part of the formula. So now pixel 1 maps to 85+42 = 127 - which is comfortably
                    // in the middle of that pixel's block.
                    // The key ting here is that unlike points in coordinate geometry, pixels take up
                    // space, so are often better treated like rectangles - and if you want to target
                    // a particular pixel, target its rectangle's midpoint, not its edge.
                    x = ((x - vscreenLeft) * DesktopNormilizedMax) / vscreenWidth + DesktopNormilizedMax / (vscreenWidth * 2);
                    y = ((y - vscreenTop) * DesktopNormilizedMax) / vscreenHeight + DesktopNormilizedMax / (vscreenHeight * 2);

                    flags |= User32.MOUSEEVENTF.VIRTUALDESK;
                }

                var mouseInput = new User32.INPUT();
                mouseInput.type = User32.INPUTENUM.MOUSE;
                mouseInput.inputUnion.mi.dx = x;
                mouseInput.inputUnion.mi.dy = y;
                mouseInput.inputUnion.mi.mouseData = 0;
                mouseInput.inputUnion.mi.dwFlags = flags;
                mouseInput.inputUnion.mi.time = 0;
                mouseInput.inputUnion.mi.dwExtraInfo = IntPtr.Zero;

                User32.SendInput(1, &mouseInput, Marshal.SizeOf(mouseInput));
            }

            public void RaiseAutomationEventForChild(UiaCore.UIA automationEventId, DateTime selectionStart, DateTime selectionEnd)
            {
                if (!_owner.IsHandleCreated)
                {
                    return;
                }

                AccessibleObject calendarChildAccessibleObject = GetCalendarChildAccessibleObject(selectionStart, selectionEnd);

                if (calendarChildAccessibleObject != null)
                {
                    calendarChildAccessibleObject.RaiseAutomationEvent(automationEventId);

                    if (automationEventId == UiaCore.UIA.AutomationFocusChangedEventId)
                    {
                        _focused = calendarChildAccessibleObject;
                    }
                }
            }

            private AccessibleObject GetCalendarChildAccessibleObject(DateTime selectionStart, DateTime selectionEnd)
            {
                if (!_owner.IsHandleCreated)
                {
                    return null;
                }

                AccessibleObject bodyAccessibleObject = GetCalendarChildAccessibleObject(_calendarIndex, CalendarChildType.CalendarBody);

                if (bodyAccessibleObject is null)
                {
                    return null;
                }

                for (int row = 0; row < RowCount; row++)
                {
                    AccessibleObject rowAccessibleObject = GetCalendarChildAccessibleObject(_calendarIndex, CalendarChildType.CalendarRow, bodyAccessibleObject, row);

                    if (rowAccessibleObject is null)
                    {
                        continue;
                    }

                    for (int column = 0; column < ColumnCount; column++)
                    {
                        bool success = GetCalendarGridInfo(
                            MCGIF.DATE,
                            MCGIP.CALENDARCELL,
                            _calendarIndex,
                            row,
                            column,
                            out RECT calendarPartRectangle,
                            out Kernel32.SYSTEMTIME systemEndDate,
                            out Kernel32.SYSTEMTIME systemStartDate);

                        if (!success)
                        {
                            continue;
                        }

                        AccessibleObject cellAccessibleObject = GetCalendarChildAccessibleObject(_calendarIndex, CalendarChildType.CalendarCell, rowAccessibleObject, column);

                        if (cellAccessibleObject is null)
                        {
                            continue;
                        }

                        DateTime endDate = DateTimePicker.SysTimeToDateTime(systemEndDate);
                        DateTime startDate = DateTimePicker.SysTimeToDateTime(systemStartDate);

                        if (DateTime.Compare(selectionEnd, endDate) <= 0 &&
                            DateTime.Compare(selectionStart, startDate) >= 0)
                        {
                            return cellAccessibleObject;
                        }
                    }
                }

                return null;
            }

            internal override UiaCore.IRawElementProviderSimple[] GetRowHeaders() => null;

            internal override UiaCore.IRawElementProviderSimple[] GetColumnHeaderItems()
            {
                if (!_owner.IsHandleCreated || !HasHeaderRow)
                {
                    return null;
                }

                UiaCore.IRawElementProviderSimple[] headers =
                    new UiaCore.IRawElementProviderSimple[MonthCalendarAccessibleObject.MAX_DAYS];

                AccessibleObject bodyAccessibleObject = GetCalendarChildAccessibleObject(_calendarIndex, CalendarChildType.CalendarBody, this, -1);
                AccessibleObject headerRowAccessibleObject = GetCalendarChildAccessibleObject(_calendarIndex, CalendarChildType.CalendarRow, bodyAccessibleObject, -1);

                if (headerRowAccessibleObject is null)
                {
                    return null;
                }

                for (int columnIndex = 0; columnIndex < MonthCalendarAccessibleObject.MAX_DAYS; columnIndex++)
                {
                    headers[columnIndex] = GetCalendarChildAccessibleObject(_calendarIndex, CalendarChildType.CalendarCell, headerRowAccessibleObject, columnIndex);
                }

                return headers;
            }

            internal override UiaCore.IRawElementProviderSimple GetItem(int row, int column)
            {
                AccessibleObject rowAccessibleObject = GetCalendarChildAccessibleObject(_calendarIndex, CalendarChildType.CalendarRow, this, row);

                if (rowAccessibleObject is null)
                {
                    return null;
                }

                return GetCalendarChildAccessibleObject(_calendarIndex, CalendarChildType.CalendarCell, rowAccessibleObject, column);
            }
        }
    }
}
