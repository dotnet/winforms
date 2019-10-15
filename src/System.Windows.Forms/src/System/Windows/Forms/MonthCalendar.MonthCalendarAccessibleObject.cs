// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Runtime.InteropServices;
using static Interop;

namespace System.Windows.Forms
{
    public partial class MonthCalendar
    {
        [ComVisible(true)]
        internal class MonthCalendarAccessibleObject : ControlAccessibleObject
        {
            internal const int MAX_DAYS = 7;
            internal const int MAX_WEEKS = 6;

            private readonly MonthCalendar _owner;
            private int _calendarIndex = 0;
            private AccessibleObject _focused;

            public MonthCalendarAccessibleObject(Control owner)
                : base(owner)
            {
                _owner = owner as MonthCalendar;
            }

            public int ControlType =>
                string.IsNullOrEmpty(base.Name) ? NativeMethods.UIA_CalendarControlTypeId : NativeMethods.UIA_TableControlTypeId;

            public bool Enabled => _owner.Enabled;

            public bool HasHeaderRow
            {
                get
                {
                    bool result = GetCalendarGridInfoText(ComCtl32.MCGIP.CALENDARCELL, _calendarIndex, -1, 0, out string text);
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
                    if (_owner == null)
                    {
                        return name;
                    }

                    if (_owner.mcCurView == NativeMethods.MONTCALENDAR_VIEW_MODE.MCMV_MONTH)
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
                    else if (_owner.mcCurView == NativeMethods.MONTCALENDAR_VIEW_MODE.MCMV_YEAR)
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
                    else if (_owner.mcCurView == NativeMethods.MONTCALENDAR_VIEW_MODE.MCMV_DECADE)
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
                    else if (_owner.mcCurView == NativeMethods.MONTCALENDAR_VIEW_MODE.MCMV_CENTURY)
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
                    var value = string.Empty;
                    if (_owner == null)
                    {
                        return value;
                    }

                    try
                    {
                        if (_owner.mcCurView == NativeMethods.MONTCALENDAR_VIEW_MODE.MCMV_MONTH)
                        {
                            if (System.DateTime.Equals(_owner.SelectionStart.Date, _owner.SelectionEnd.Date))
                            {
                                value = _owner.SelectionStart.ToLongDateString();
                            }
                            else
                            {
                                value = string.Format("{0} - {1}", _owner.SelectionStart.ToLongDateString(), _owner.SelectionEnd.ToLongDateString());
                            }
                        }
                        else if (_owner.mcCurView == NativeMethods.MONTCALENDAR_VIEW_MODE.MCMV_YEAR)
                        {
                            if (System.DateTime.Equals(_owner.SelectionStart.Month, _owner.SelectionEnd.Month))
                            {
                                value = _owner.SelectionStart.ToString("y");
                            }
                            else
                            {
                                value = string.Format("{0} - {1}", _owner.SelectionStart.ToString("y"), _owner.SelectionEnd.ToString("y"));
                            }
                        }
                        else
                        {
                            value = string.Format("{0} - {1}", _owner.SelectionRange.Start.ToString(), _owner.SelectionRange.End.ToString());
                        }
                    }
                    catch
                    {
                        value = base.Value;
                    }

                    return value;
                }
                set
                {
                    base.Value = value;
                }
            }

            internal override int ColumnCount
            {
                get
                {
                    GetCalendarGridInfo(
                        ComCtl32.MCGIF.RECT,
                        ComCtl32.MCGIP.CALENDARBODY,
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
                            ComCtl32.MCGIF.RECT,
                            ComCtl32.MCGIP.CALENDARCELL,
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
                    GetCalendarGridInfo(
                        ComCtl32.MCGIF.RECT,
                        ComCtl32.MCGIP.CALENDARBODY,
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
                            ComCtl32.MCGIF.RECT,
                            ComCtl32.MCGIP.CALENDARCELL,
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

            internal override UnsafeNativeMethods.RowOrColumnMajor RowOrColumnMajor => UnsafeNativeMethods.RowOrColumnMajor.RowOrColumnMajor_RowMajor;

            internal override UnsafeNativeMethods.IRawElementProviderSimple[] GetRowHeaderItems() => null;

            internal override UnsafeNativeMethods.IRawElementProviderFragment ElementProviderFromPoint(double x, double y)
            {
                int innerX = (int)x;
                int innerY = (int)y;

                ComCtl32.MCHITTESTINFO hitTestInfo = GetHitTestInfo(innerX, innerY);
                switch ((ComCtl32.MCHT)hitTestInfo.uHit)
                {
                    case ComCtl32.MCHT.TITLEBTNPREV:
                        return GetCalendarChildAccessibleObject(_calendarIndex, CalendarChildType.PreviousButton);

                    case ComCtl32.MCHT.TITLEBTNNEXT:
                        return GetCalendarChildAccessibleObject(_calendarIndex, CalendarChildType.NextButton);

                    case ComCtl32.MCHT.TITLE:
                    case ComCtl32.MCHT.TITLEMONTH:
                    case ComCtl32.MCHT.TITLEYEAR:
                        return GetCalendarChildAccessibleObject(_calendarIndex, CalendarChildType.CalendarHeader);

                    case ComCtl32.MCHT.CALENDARDAY:
                    case ComCtl32.MCHT.CALENDARWEEKNUM:
                    case ComCtl32.MCHT.CALENDARDATE:
                        // Get calendar body's child.
                        CalendarBodyAccessibleObject calendarBodyAccessibleObject = (CalendarBodyAccessibleObject)GetCalendarChildAccessibleObject(_calendarIndex, CalendarChildType.CalendarBody);
                        return calendarBodyAccessibleObject.GetFromPoint(hitTestInfo);

                    case ComCtl32.MCHT.TODAYLINK:
                        return GetCalendarChildAccessibleObject(_calendarIndex, CalendarChildType.TodayLink);
                }

                return base.ElementProviderFromPoint(x, y);
            }

            internal override UnsafeNativeMethods.IRawElementProviderFragment FragmentNavigate(UnsafeNativeMethods.NavigateDirection direction)
            {
                switch (direction)
                {
                    case UnsafeNativeMethods.NavigateDirection.FirstChild:
                        return GetCalendarChildAccessibleObject(_calendarIndex, CalendarChildType.PreviousButton);
                    case UnsafeNativeMethods.NavigateDirection.LastChild:
                        return _owner.ShowTodayCircle
                            ? GetCalendarChildAccessibleObject(_calendarIndex, CalendarChildType.TodayLink)
                            : GetCalendarChildAccessibleObject(_calendarIndex, CalendarChildType.CalendarBody);
                }

                return base.FragmentNavigate(direction);
            }

            internal override UnsafeNativeMethods.IRawElementProviderFragment GetFocus() => _focused;

            public override AccessibleObject GetFocused() => _focused;

            public ComCtl32.MCHITTESTINFO GetHitTestInfo(int xScreen, int yScreen)
            {
                ComCtl32.MCHITTESTINFO hitTestInfo = new ComCtl32.MCHITTESTINFO();
                hitTestInfo.cbSize = (int)Marshal.SizeOf(hitTestInfo);
                hitTestInfo.pt = new POINT();
                hitTestInfo.st = new Kernel32.SYSTEMTIME();

                // NativeMethods.GetCursorPos(out Point pt);
                Point point = new Point(xScreen, yScreen);
                NativeMethods.MapWindowPoints(IntPtr.Zero, Handle, ref point, 1);
                hitTestInfo.pt.x = point.X;
                hitTestInfo.pt.y = point.Y;

                UnsafeNativeMethods.SendMessage(new HandleRef(this, Handle), (int)ComCtl32.MCM.HITTEST, 0, ref hitTestInfo);

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
                        GetCalendarGridInfoText(ComCtl32.MCGIP.CALENDARHEADER, calendarIndex, 0, 0, out string text);
                        return text;
                    case CalendarChildType.TodayLink:
                        return string.Format(SR.MonthCalendarTodayButtonAccessibleName, _owner.TodayDate.ToShortDateString());
                };

                return string.Empty;
            }

            private CalendarCellAccessibleObject GetCalendarCell(int calendarIndex, AccessibleObject parentAccessibleObject, int columnIndex)
            {
                if (columnIndex < 0 ||
                    columnIndex >= MAX_DAYS ||
                    columnIndex >= ColumnCount)
                {
                    return null;
                }

                CalendarRowAccessibleObject parentRowAccessibleObject = (CalendarRowAccessibleObject)parentAccessibleObject;
                int rowIndex = parentRowAccessibleObject.RowIndex;
                bool getNameResult = GetCalendarGridInfoText(ComCtl32.MCGIP.CALENDARCELL, calendarIndex, rowIndex, columnIndex, out string text);
                bool getDateResult = GetCalendarGridInfo(ComCtl32.MCGIF.DATE, ComCtl32.MCGIP.CALENDARCELL,
                    calendarIndex,
                    rowIndex,
                    columnIndex,
                    out RECT rectangle,
                    out Kernel32.SYSTEMTIME systemEndDate,
                    out Kernel32.SYSTEMTIME systemStartDate);

                DateTime endDate = DateTimePicker.SysTimeToDateTime(systemEndDate).Date;
                DateTime startDate = DateTimePicker.SysTimeToDateTime(systemStartDate).Date;

                if (getNameResult && !string.IsNullOrEmpty(text))
                {
                    string cellName = GetCalendarCellName(endDate, startDate, text, rowIndex == -1);

                    // The cell is present on the calendar, so create accessible object for it.
                    return new CalendarCellAccessibleObject(this, calendarIndex, parentAccessibleObject, rowIndex, columnIndex, cellName);
                }

                return null;
            }

            private string GetCalendarCellName(DateTime endDate, DateTime startDate, string defaultName, bool headerCell)
            {
                if (_owner.mcCurView == NativeMethods.MONTCALENDAR_VIEW_MODE.MCMV_MONTH)
                {
                    if (headerCell)
                    {
                        return startDate.ToString("dddd");
                    }

                    return startDate.ToString("dddd, MMMM dd, yyyy");
                }
                else if (_owner.mcCurView == NativeMethods.MONTCALENDAR_VIEW_MODE.MCMV_YEAR)
                {
                    return startDate.ToString("MMMM yyyy");
                }

                return defaultName;
            }

            private CalendarRowAccessibleObject GetCalendarRow(int calendarIndex, AccessibleObject parentAccessibleObject, int rowIndex)
            {
                if ((HasHeaderRow ? rowIndex < -1 : rowIndex < 0) ||
                    rowIndex >= RowCount)
                {
                    return null;
                }

                // Search name for the first cell in the row.
                bool success = GetCalendarGridInfo(
                    ComCtl32.MCGIF.DATE,
                    ComCtl32.MCGIP.CALENDARCELL,
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

                if (cellsRange.Start > DateTimePicker.SysTimeToDateTime(endDate) || cellsRange.End < DateTimePicker.SysTimeToDateTime(startDate))
                {
                    // Do not create row if the row's first cell is out of the current calendar's view range.
                    return null;
                }

                return new CalendarRowAccessibleObject(this, calendarIndex, (CalendarBodyAccessibleObject)parentAccessibleObject, rowIndex);
            }

            private bool GetCalendarGridInfo(
                ComCtl32.MCGIF dwFlags,
                ComCtl32.MCGIP dwPart,
                int calendarIndex,
                int row,
                int column,
                out RECT rectangle,
                out Kernel32.SYSTEMTIME endDate,
                out Kernel32.SYSTEMTIME startDate)
            {
                Debug.Assert(
                    (dwFlags & ~(ComCtl32.MCGIF.DATE | ComCtl32.MCGIF.RECT)) == 0,
                    "GetCalendarGridInfo() should be used only to obtain Date and Rect,"
                    + "dwFlags has flag bits other that MCGIF_DATE and MCGIF_RECT");

                ComCtl32.MCGRIDINFO gridInfo = new ComCtl32.MCGRIDINFO();
                gridInfo.dwFlags = dwFlags;
                gridInfo.cbSize = (uint)Marshal.SizeOf(gridInfo);
                gridInfo.dwPart = dwPart;
                gridInfo.iCalendar = calendarIndex;
                gridInfo.iCol = column;
                gridInfo.iRow = row;
                bool result;

                try
                {
                    result = GetCalendarGridInfo(ref gridInfo);
                    rectangle = gridInfo.rc;
                    endDate = gridInfo.stEnd;
                    startDate = gridInfo.stStart;
                }
                catch
                {
                    rectangle = new RECT();
                    endDate = new Kernel32.SYSTEMTIME();
                    startDate = new Kernel32.SYSTEMTIME();
                    result = false;
                }

                return result;
            }

            private bool GetCalendarGridInfo(ref ComCtl32.MCGRIDINFO gridInfo)
            {
                // Do not use this if gridInfo.dwFlags contains MCGIF_NAME;
                // use GetCalendarGridInfoText() instead.
                Debug.Assert(
                    (gridInfo.dwFlags & ComCtl32.MCGIF.NAME) == 0,
                    "Param dwFlags contains MCGIF_NAME, use GetCalendarGridInfoText() to retrieve the text of a calendar part.");

                gridInfo.dwFlags &= ~ComCtl32.MCGIF.NAME;

                return _owner.SendMessage((int)ComCtl32.MCM.GETCALENDARGRIDINFO, 0, ref gridInfo) != IntPtr.Zero;
            }

            private bool GetCalendarGridInfoText(ComCtl32.MCGIP dwPart, int calendarIndex, int row, int column, out string text)
            {
                const int nameLength = 128;

                ComCtl32.MCGRIDINFO gridInfo = new ComCtl32.MCGRIDINFO();
                gridInfo.cbSize = (uint)Marshal.SizeOf(gridInfo);
                gridInfo.dwPart = dwPart;
                gridInfo.iCalendar = calendarIndex;
                gridInfo.iCol = column;
                gridInfo.iRow = row;
                gridInfo.pszName = new string('\0', nameLength + 2);
                gridInfo.cchName = (uint)gridInfo.pszName.Length - 1;

                bool result = GetCalendarGridInfoText(ref gridInfo);
                text = gridInfo.pszName;

                return result;
            }

            // Use to retrieve MCGIF_NAME only.
            private bool GetCalendarGridInfoText(ref ComCtl32.MCGRIDINFO gridInfo)
            {
                Debug.Assert(
                    gridInfo.dwFlags == 0,
                    "gridInfo.dwFlags should be 0 when calling GetCalendarGridInfoText");

                gridInfo.dwFlags = ComCtl32.MCGIF.NAME;

                return _owner.SendMessage((int)ComCtl32.MCM.GETCALENDARGRIDINFO, 0, ref gridInfo) != IntPtr.Zero;
            }

            public bool GetCalendarPartRectangle(int calendarIndex, ComCtl32.MCGIP dwPart, int row, int column, out RECT calendarPartRectangle)
            {
                bool success = GetCalendarGridInfo(
                    ComCtl32.MCGIF.RECT,
                    dwPart,
                    calendarIndex,
                    row,
                    column,
                    out calendarPartRectangle,
                    out Kernel32.SYSTEMTIME endDate, out Kernel32.SYSTEMTIME startDate);

                if (success)
                {
                    success = UnsafeNativeMethods.MapWindowPoints(new HandleRef(this, Owner.Handle), new HandleRef(null, IntPtr.Zero), ref calendarPartRectangle, 2) != 0;
                }

                if (!success)
                {
                    calendarPartRectangle = new RECT();
                }

                return success;
            }

            internal override object GetPropertyValue(int propertyID) =>
                propertyID switch
                {
                    NativeMethods.UIA_ControlTypePropertyId => ControlType,
                    NativeMethods.UIA_NamePropertyId => Name,
                    NativeMethods.UIA_IsGridPatternAvailablePropertyId => true,
                    NativeMethods.UIA_IsTablePatternAvailablePropertyId => true,
                    NativeMethods.UIA_IsLegacyIAccessiblePatternAvailablePropertyId => true,
                    _ => base.GetPropertyValue(propertyID)
                };

            internal override bool IsPatternSupported(int patternId) =>
                patternId switch
                {
                    var p when
                        p == NativeMethods.UIA_ValuePatternId ||
                        p == NativeMethods.UIA_GridPatternId ||
                        p == NativeMethods.UIA_TablePatternId ||
                        p == NativeMethods.UIA_LegacyIAccessiblePatternId => true,
                    _ => base.IsPatternSupported(patternId)
                };

            public void RaiseMouseClick(int x, int y)
            {
                POINT previousPosition = new POINT();
                bool setOldCursorPos = UnsafeNativeMethods.GetPhysicalCursorPos(ref previousPosition);
 
                bool mouseSwapped = User32.GetSystemMetrics(User32.SystemMetric.SM_SWAPBUTTON) != 0;

                SendMouseInput(x, y, User32.MOUSEEVENTF.MOVE | User32.MOUSEEVENTF.ABSOLUTE);
                SendMouseInput(0, 0, mouseSwapped ? User32.MOUSEEVENTF.RIGHTDOWN : User32.MOUSEEVENTF.LEFTDOWN);
                SendMouseInput(0, 0, mouseSwapped ? User32.MOUSEEVENTF.RIGHTUP : User32.MOUSEEVENTF.LEFTUP);

                Threading.Thread.Sleep(50);

                // Set back the mouse position where it was.
                if (setOldCursorPos)
                {
                    SendMouseInput(previousPosition.x, previousPosition.y, User32.MOUSEEVENTF.MOVE | User32.MOUSEEVENTF.ABSOLUTE);
                }
            }

            private void SendMouseInput(int x, int y, User32.MOUSEEVENTF flags)
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

                NativeMethods.INPUT mouseInput = new NativeMethods.INPUT();
                mouseInput.type = NativeMethods.INPUT_MOUSE;
                mouseInput.inputUnion.mi.dx = x;
                mouseInput.inputUnion.mi.dy = y;
                mouseInput.inputUnion.mi.mouseData = 0;
                mouseInput.inputUnion.mi.dwFlags = (int)flags;
                mouseInput.inputUnion.mi.time = 0;
                mouseInput.inputUnion.mi.dwExtraInfo = new IntPtr(0);

                UnsafeNativeMethods.SendInput(1, ref mouseInput, Marshal.SizeOf(mouseInput));
            }

            public void RaiseAutomationEventForChild(int automationEventId, DateTime selectionStart, DateTime selectionEnd)
            {
                AccessibleObject calendarChildAccessibleObject = GetCalendarChildAccessibleObject(selectionStart, selectionEnd);
                if (calendarChildAccessibleObject != null)
                {
                    calendarChildAccessibleObject.RaiseAutomationEvent(automationEventId);

                    if (automationEventId == NativeMethods.UIA_AutomationFocusChangedEventId)
                    {
                        _focused = calendarChildAccessibleObject;
                    }
                }
            }

            private AccessibleObject GetCalendarChildAccessibleObject(DateTime selectionStart, DateTime selectionEnd)
            {
                int columnCount = ColumnCount;

                AccessibleObject bodyAccessibleObject = GetCalendarChildAccessibleObject(_calendarIndex, CalendarChildType.CalendarBody);
                for (int row = 0; row < RowCount; row++)
                {
                    AccessibleObject rowAccessibleObject = GetCalendarChildAccessibleObject(_calendarIndex, CalendarChildType.CalendarRow, bodyAccessibleObject, row);
                    for (int column = 0; column < columnCount; column++)
                    {
                        bool success = GetCalendarGridInfo(
                            ComCtl32.MCGIF.DATE,
                            ComCtl32.MCGIP.CALENDARCELL,
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
                        if (cellAccessibleObject == null)
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

            internal override UnsafeNativeMethods.IRawElementProviderSimple[] GetRowHeaders() => null;

            internal override UnsafeNativeMethods.IRawElementProviderSimple[] GetColumnHeaderItems()
            {
                if (!HasHeaderRow)
                {
                    return null;
                }

                UnsafeNativeMethods.IRawElementProviderSimple[] headers =
                    new UnsafeNativeMethods.IRawElementProviderSimple[MonthCalendarAccessibleObject.MAX_DAYS];
                AccessibleObject headerRowAccessibleObject = GetCalendarChildAccessibleObject(_calendarIndex, CalendarChildType.CalendarRow, this, -1);
                for (int columnIndex = 0; columnIndex < MonthCalendarAccessibleObject.MAX_DAYS; columnIndex++)
                {
                    headers[columnIndex] = GetCalendarChildAccessibleObject(_calendarIndex, CalendarChildType.CalendarCell, headerRowAccessibleObject, columnIndex);
                }

                return headers;
            }

            internal override UnsafeNativeMethods.IRawElementProviderSimple GetItem(int row, int column)
            {
                AccessibleObject rowAccessibleObject = GetCalendarChildAccessibleObject(_calendarIndex, CalendarChildType.CalendarRow, this, row);

                if (rowAccessibleObject == null)
                {
                    return null;
                }

                return GetCalendarChildAccessibleObject(_calendarIndex, CalendarChildType.CalendarCell, rowAccessibleObject, column);
            }
        }
    }
}
