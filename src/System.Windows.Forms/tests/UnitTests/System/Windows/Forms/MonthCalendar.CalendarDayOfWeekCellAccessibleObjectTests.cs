// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Xunit;
using static System.Windows.Forms.MonthCalendar;
using static Interop;

namespace System.Windows.Forms.Tests
{
    public class MonthCalendar_CalendarDayOfWeekCellAccessibleObjectTests : IClassFixture<ThreadExceptionFixture>
    {
        [WinFormsFact]
        public void CalendarDayOfWeekCellAccessibleObject_ctor_default()
        {
            using MonthCalendar control = new();
            CalendarDayOfWeekCellAccessibleObject cellAccessibleObject = CreateCalendarDayOfWeekCellCellAccessibleObject(control);

            Assert.Equal(0, cellAccessibleObject.TestAccessor().Dynamic._calendarIndex);
            Assert.Equal(0, cellAccessibleObject.TestAccessor().Dynamic._rowIndex);
            Assert.Equal(0, cellAccessibleObject.TestAccessor().Dynamic._columnIndex);
            Assert.Equal("Test name", cellAccessibleObject.Name);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsFact]
        public void CalendarDayOfWeekCellAccessibleObject_DateRange_IsNull()
        {
            using MonthCalendar control = new();
            CalendarDayOfWeekCellAccessibleObject cellAccessibleObject = CreateCalendarDayOfWeekCellCellAccessibleObject(control);

            Assert.Null(cellAccessibleObject.DateRange);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsFact]
        public void CalendarDayOfWeekCellAccessibleObject_DefaultAction_IsEmpty()
        {
            using MonthCalendar control = new();
            CalendarDayOfWeekCellAccessibleObject cellAccessibleObject = CreateCalendarDayOfWeekCellCellAccessibleObject(control);

            Assert.Empty(cellAccessibleObject.DefaultAction);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsFact]
        public void CalendarDayOfWeekCellAccessibleObject_Description_IsNull()
        {
            using MonthCalendar control = new();
            CalendarDayOfWeekCellAccessibleObject cellAccessibleObject = CreateCalendarDayOfWeekCellCellAccessibleObject(control);

            Assert.Null(cellAccessibleObject.Description);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsFact]
        public void CalendarDayOfWeekCellAccessibleObject_ControlType_IsHeader()
        {
            using MonthCalendar control = new();
            CalendarDayOfWeekCellAccessibleObject cellAccessibleObject = CreateCalendarDayOfWeekCellCellAccessibleObject(control);

            UiaCore.UIA actual = (UiaCore.UIA)cellAccessibleObject.GetPropertyValue(UiaCore.UIA.ControlTypePropertyId);

            Assert.Equal(UiaCore.UIA.HeaderControlTypeId, actual);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsFact]
        public void CalendarDayOfWeekCellAccessibleObject_HasKeyboardFocus_IsFalse()
        {
            using MonthCalendar control = new();
            CalendarDayOfWeekCellAccessibleObject cellAccessibleObject = CreateCalendarDayOfWeekCellCellAccessibleObject(control);

            bool actual = (bool)cellAccessibleObject.GetPropertyValue(UiaCore.UIA.HasKeyboardFocusPropertyId);

            Assert.False(actual);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsFact]
        public void CalendarDayOfWeekCellAccessibleObject_Role_IsColumnHeader()
        {
            using MonthCalendar control = new();
            CalendarDayOfWeekCellAccessibleObject cellAccessibleObject = CreateCalendarDayOfWeekCellCellAccessibleObject(control);

            AccessibleRole actual = cellAccessibleObject.Role;

            Assert.Equal(AccessibleRole.ColumnHeader, actual);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsFact]
        public void CalendarDayOfWeekCellAccessibleObject_State_IsNone()
        {
            using MonthCalendar control = new();
            CalendarDayOfWeekCellAccessibleObject cellAccessibleObject = CreateCalendarDayOfWeekCellCellAccessibleObject(control);

            AccessibleStates actual = cellAccessibleObject.State;

            Assert.Equal(AccessibleStates.None, actual);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsFact]
        public void CalendarDayOfWeekCellAccessibleObject_FragmentNavigate_Parent_ReturnsExpected()
        {
            using MonthCalendar control = new();
            MonthCalendarAccessibleObject controlAccessibleObject = (MonthCalendarAccessibleObject)control.AccessibilityObject;
            CalendarAccessibleObject calendar = new(controlAccessibleObject, 0, "");
            CalendarBodyAccessibleObject body = new(calendar, controlAccessibleObject, 0);
            CalendarRowAccessibleObject row = new(body, controlAccessibleObject, 0, 0);
            CalendarDayOfWeekCellAccessibleObject cell = new(row, body, controlAccessibleObject, 0, 0, 0, "");

            Assert.Equal(row, cell.FragmentNavigate(UiaCore.NavigateDirection.Parent));
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsFact]
        public void CalendarDayOfWeekCellAccessibleObject_FragmentNavigate_Sibling_ReturnsExpected()
        {
            using MonthCalendar control = new()
            {
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

            LinkedList<CalendarCellAccessibleObject> days = daysOfWeekRow.CellsAccessibleObjects;
            CalendarDayOfWeekCellAccessibleObject sunday = days?.First?.Value as CalendarDayOfWeekCellAccessibleObject;
            CalendarDayOfWeekCellAccessibleObject monday = days?.First?.Next?.Value as CalendarDayOfWeekCellAccessibleObject;
            CalendarDayOfWeekCellAccessibleObject tuesday = days?.First?.Next?.Next?.Value as CalendarDayOfWeekCellAccessibleObject;
            CalendarDayOfWeekCellAccessibleObject friday = days?.Last?.Previous?.Value as CalendarDayOfWeekCellAccessibleObject;
            CalendarDayOfWeekCellAccessibleObject saturday = days?.Last?.Value as CalendarDayOfWeekCellAccessibleObject;

            Assert.NotNull(sunday);
            Assert.NotNull(monday);
            Assert.NotNull(tuesday);
            Assert.NotNull(friday);
            Assert.NotNull(saturday);

            Assert.Null(sunday.FragmentNavigate(UiaCore.NavigateDirection.PreviousSibling));
            Assert.Equal(monday, sunday.FragmentNavigate(UiaCore.NavigateDirection.NextSibling));

            Assert.Equal(sunday, monday.FragmentNavigate(UiaCore.NavigateDirection.PreviousSibling));
            Assert.Equal(tuesday, monday.FragmentNavigate(UiaCore.NavigateDirection.NextSibling));

            Assert.Equal(friday, saturday.FragmentNavigate(UiaCore.NavigateDirection.PreviousSibling));
            Assert.Null(saturday.FragmentNavigate(UiaCore.NavigateDirection.NextSibling));
        }

        [WinFormsFact]
        public void CalendarDayOfWeekCellAccessibleObject_FragmentNavigate_Sibling_ReturnsExpected_IfWeekNumbersVisible()
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

            LinkedList<CalendarCellAccessibleObject> days = daysOfWeekRow.CellsAccessibleObjects;
            CalendarDayOfWeekCellAccessibleObject sunday = days?.First?.Value as CalendarDayOfWeekCellAccessibleObject;
            CalendarDayOfWeekCellAccessibleObject monday = days?.First?.Next?.Value as CalendarDayOfWeekCellAccessibleObject;

            Assert.NotNull(sunday);
            Assert.NotNull(monday);

            Assert.Null(sunday.FragmentNavigate(UiaCore.NavigateDirection.PreviousSibling));
            Assert.Equal(monday, sunday.FragmentNavigate(UiaCore.NavigateDirection.NextSibling));
        }

        [WinFormsFact]
        public void CalendarDayOfWeekCellAccessibleObject_FragmentNavigate_Child_ReturnsExpected()
        {
            using MonthCalendar control = new();
            CalendarDayOfWeekCellAccessibleObject cell = CreateCalendarDayOfWeekCellCellAccessibleObject(control, 0, 0, 0);

            Assert.Null(cell.FragmentNavigate(UiaCore.NavigateDirection.FirstChild));
            Assert.Null(cell.FragmentNavigate(UiaCore.NavigateDirection.LastChild));
            Assert.False(control.IsHandleCreated);
        }

        private CalendarDayOfWeekCellAccessibleObject CreateCalendarDayOfWeekCellCellAccessibleObject(MonthCalendar control, int calendarIndex = 0, int rowIndex = 0, int columnIndex = 0)
        {
            MonthCalendarAccessibleObject controlAccessibleObject = (MonthCalendarAccessibleObject)control.AccessibilityObject;
            CalendarAccessibleObject calendarAccessibleObject = new(controlAccessibleObject, calendarIndex, "Test name");
            CalendarBodyAccessibleObject bodyAccessibleObject = new(calendarAccessibleObject, controlAccessibleObject, calendarIndex);
            CalendarRowAccessibleObject rowAccessibleObject = new(bodyAccessibleObject, controlAccessibleObject, calendarIndex, rowIndex);
            CalendarDayOfWeekCellAccessibleObject cellAccessibleObject = new(rowAccessibleObject, bodyAccessibleObject, controlAccessibleObject, calendarIndex, rowIndex, columnIndex, "Test name");

            return cellAccessibleObject;
        }
    }
}
