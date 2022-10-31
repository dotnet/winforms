// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Xunit;
using static System.Windows.Forms.MonthCalendar;
using static Interop;

namespace System.Windows.Forms.Tests
{
    public class MonthCalendar_CalendarWeekNumberCellAccessibleObjectTests : IClassFixture<ThreadExceptionFixture>
    {
        [WinFormsFact]
        public void CalendarWeekNumberCellAccessibleObject_ctor_default()
        {
            using MonthCalendar control = new();
            CalendarWeekNumberCellAccessibleObject cellAccessibleObject = CreateCalendarWeekNumberCellAccessibleObject(control);

            Assert.Equal(0, cellAccessibleObject.TestAccessor().Dynamic._calendarIndex);
            Assert.Equal(0, cellAccessibleObject.TestAccessor().Dynamic._rowIndex);
            Assert.Equal(0, cellAccessibleObject.TestAccessor().Dynamic._columnIndex);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsFact]
        public void CalendarWeekNumberCellAccessibleObject_DateRange_IsNull()
        {
            using MonthCalendar control = new();
            CalendarWeekNumberCellAccessibleObject cellAccessibleObject = CreateCalendarWeekNumberCellAccessibleObject(control);

            Assert.Null(cellAccessibleObject.DateRange);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsFact]
        public void CalendarWeekNumberCellAccessibleObject_DefaultAction_IsEmpty()
        {
            using MonthCalendar control = new();
            CalendarWeekNumberCellAccessibleObject cellAccessibleObject = CreateCalendarWeekNumberCellAccessibleObject(control);

            Assert.Empty(cellAccessibleObject.DefaultAction);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsFact]
        public void CalendarWeekNumberCellAccessibleObject_Description_IsNull()
        {
            using MonthCalendar control = new();
            CalendarWeekNumberCellAccessibleObject cellAccessibleObject = CreateCalendarWeekNumberCellAccessibleObject(control);

            Assert.Null(cellAccessibleObject.Description);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsFact]
        public void CalendarWeekNumberCellAccessibleObject_GetChildId_ReturnsExpected()
        {
            using MonthCalendar control = new();
            CalendarWeekNumberCellAccessibleObject cellAccessibleObject = CreateCalendarWeekNumberCellAccessibleObject(control);

            int actual = cellAccessibleObject.GetChildId();

            Assert.Equal(0, actual);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsFact]
        public void CalendarWeekNumberCellAccessibleObject_ControlType_IsHeader()
        {
            using MonthCalendar control = new();
            CalendarWeekNumberCellAccessibleObject cellAccessibleObject = CreateCalendarWeekNumberCellAccessibleObject(control);

            UiaCore.UIA actual = (UiaCore.UIA)cellAccessibleObject.GetPropertyValue(UiaCore.UIA.ControlTypePropertyId);

            Assert.Equal(UiaCore.UIA.HeaderControlTypeId, actual);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsFact]
        public void CalendarWeekNumberCellAccessibleObject_HasKeyboardFocus_IsFalse()
        {
            using MonthCalendar control = new();
            CalendarWeekNumberCellAccessibleObject cellAccessibleObject = CreateCalendarWeekNumberCellAccessibleObject(control);

            bool actual = (bool)cellAccessibleObject.GetPropertyValue(UiaCore.UIA.HasKeyboardFocusPropertyId);

            Assert.False(actual);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsFact]
        public void CalendarWeekNumberCellAccessibleObject_Role_IsRowHeader()
        {
            using MonthCalendar control = new();
            CalendarWeekNumberCellAccessibleObject cellAccessibleObject = CreateCalendarWeekNumberCellAccessibleObject(control);

            AccessibleRole actual = cellAccessibleObject.Role;

            Assert.Equal(AccessibleRole.RowHeader, actual);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsFact]
        public void CalendarWeekNumberCellAccessibleObject_State_IsNone()
        {
            using MonthCalendar control = new();
            CalendarWeekNumberCellAccessibleObject cellAccessibleObject = CreateCalendarWeekNumberCellAccessibleObject(control);

            AccessibleStates actual = cellAccessibleObject.State;

            Assert.Equal(AccessibleStates.None, actual);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsFact]
        public void CalendarWeekNumberCellAccessibleObject_FragmentNavigate_Parent_ReturnsExpected()
        {
            using MonthCalendar control = new();
            var controlAccessibleObject = (MonthCalendarAccessibleObject)control.AccessibilityObject;
            CalendarAccessibleObject calendar = new(controlAccessibleObject, 0, "");
            CalendarBodyAccessibleObject body = new(calendar, controlAccessibleObject, 0);
            CalendarRowAccessibleObject row = new(body, controlAccessibleObject, 0, 0);
            CalendarWeekNumberCellAccessibleObject cell = new(row, body, controlAccessibleObject, 0, 0, 0, "");

            Assert.Equal(row, cell.FragmentNavigate(UiaCore.NavigateDirection.Parent));
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsFact]
        public void CalendarWeekNumberCellAccessibleObject_FragmentNavigate_Sibling_ReturnsExpected()
        {
            using MonthCalendar control = new()
            {
                ShowWeekNumbers = true
            };
            control.CreateControl();

            var controlAccessibleObject = (MonthCalendarAccessibleObject)control.AccessibilityObject;

            CalendarAccessibleObject calendar = controlAccessibleObject.CalendarsAccessibleObjects.First?.Value;
            Assert.NotNull(calendar);

            CalendarBodyAccessibleObject body = calendar.CalendarBodyAccessibleObject;
            Assert.NotNull(body);

            CalendarRowAccessibleObject secondRow = body.RowsAccessibleObjects?.First?.Next?.Next.Value;
            Assert.NotNull(secondRow);

            CalendarWeekNumberCellAccessibleObject weekNumber = secondRow.WeekNumberCellAccessibleObject;
            CalendarCellAccessibleObject sunday = secondRow.CellsAccessibleObjects?.First?.Value as CalendarCellAccessibleObject;

            Assert.NotNull(weekNumber);
            Assert.NotNull(sunday);

            Assert.Null(weekNumber.FragmentNavigate(UiaCore.NavigateDirection.PreviousSibling));
            Assert.Equal(sunday, weekNumber.FragmentNavigate(UiaCore.NavigateDirection.NextSibling));
        }

        [WinFormsFact]
        public void CalendarWeekNumberCellAccessibleObject_FragmentNavigate_Sibling_ReturnsExpected_IfWeekNumbersVisible()
        {
            using MonthCalendar control = new()
            {
                ShowWeekNumbers = true,
                SelectionStart = new DateTime(2022, 10, 1) // Set a date to have a stable test case
            };
            control.CreateControl();

            var controlAccessibleObject = (MonthCalendarAccessibleObject)control.AccessibilityObject;

            CalendarAccessibleObject calendar = controlAccessibleObject.CalendarsAccessibleObjects.First?.Value;
            Assert.NotNull(calendar);

            CalendarBodyAccessibleObject body = calendar.CalendarBodyAccessibleObject;
            Assert.NotNull(body);

            CalendarRowAccessibleObject daysOfWeekRow = body.RowsAccessibleObjects?.First?.Value;
            Assert.NotNull(daysOfWeekRow);

            CalendarCellAccessibleObject sunday = daysOfWeekRow.CellsAccessibleObjects?.First?.Value;
            CalendarCellAccessibleObject monday = daysOfWeekRow.CellsAccessibleObjects?.First?.Next?.Value;

            Assert.NotNull(sunday);
            Assert.NotNull(monday);

            Assert.Null(sunday.FragmentNavigate(UiaCore.NavigateDirection.PreviousSibling));
            Assert.Equal(monday, sunday.FragmentNavigate(UiaCore.NavigateDirection.NextSibling));
        }

        [WinFormsFact]
        public void CalendarWeekNumberCellAccessibleObject_FragmentNavigate_Child_ReturnsExpected()
        {
            using MonthCalendar control = new();
            CalendarWeekNumberCellAccessibleObject cell = CreateCalendarWeekNumberCellAccessibleObject(control, 0, 0, 0);

            Assert.Null(cell.FragmentNavigate(UiaCore.NavigateDirection.FirstChild));
            Assert.Null(cell.FragmentNavigate(UiaCore.NavigateDirection.LastChild));
            Assert.False(control.IsHandleCreated);
        }

        private CalendarWeekNumberCellAccessibleObject CreateCalendarWeekNumberCellAccessibleObject(MonthCalendar control, int calendarIndex = 0, int rowIndex = 0, int columnIndex = 0)
        {
            MonthCalendarAccessibleObject controlAccessibleObject = (MonthCalendarAccessibleObject)control.AccessibilityObject;
            CalendarAccessibleObject calendarAccessibleObject = new(controlAccessibleObject, calendarIndex, "Test name");
            CalendarBodyAccessibleObject bodyAccessibleObject = new(calendarAccessibleObject, controlAccessibleObject, calendarIndex);
            CalendarRowAccessibleObject rowAccessibleObject = new(bodyAccessibleObject, controlAccessibleObject, calendarIndex, rowIndex);
            CalendarWeekNumberCellAccessibleObject cellAccessibleObject = new(rowAccessibleObject, bodyAccessibleObject, controlAccessibleObject, calendarIndex, rowIndex, columnIndex, "12");

            return cellAccessibleObject;
        }
    }
}
