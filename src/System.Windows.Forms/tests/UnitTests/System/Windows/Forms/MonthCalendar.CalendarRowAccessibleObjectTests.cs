// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Xunit;
using static System.Windows.Forms.MonthCalendar;
using static Interop;

namespace System.Windows.Forms.Tests
{
    public class MonthCalendar_CalendarRowAccessibleObjectTests : IClassFixture<ThreadExceptionFixture>
    {
        [WinFormsFact]
        public void CalendarRowAccessibleObject_ctor_default()
        {
            using MonthCalendar control = new();
            CalendarRowAccessibleObject rowAccessibleObject = CreateCalendarRowAccessibleObject(control);

            Assert.Equal(0, rowAccessibleObject.TestAccessor().Dynamic._calendarIndex);
            Assert.Equal(0, rowAccessibleObject.TestAccessor().Dynamic._rowIndex);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        public void CalendarRowAccessibleObject_GetChildId_ReturnExpected(int rowIndex)
        {
            using MonthCalendar control = new();
            CalendarRowAccessibleObject rowAccessibleObject = CreateCalendarRowAccessibleObject(control, 0, rowIndex);

            int actual = rowAccessibleObject.GetChildId();

            Assert.Equal(rowIndex + 1, actual);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsFact]
        public void CalendarRowAccessibleObject_ControlType_IsPane()
        {
            using MonthCalendar control = new();
            CalendarRowAccessibleObject rowAccessibleObject = CreateCalendarRowAccessibleObject(control);

            UiaCore.UIA actual = (UiaCore.UIA)rowAccessibleObject.GetPropertyValue(UiaCore.UIA.ControlTypePropertyId);

            Assert.Equal(UiaCore.UIA.PaneControlTypeId, actual);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsFact]
        public void CalendarRowAccessibleObject_Name_IsNull()
        {
            using MonthCalendar control = new();
            CalendarRowAccessibleObject rowAccessibleObject = CreateCalendarRowAccessibleObject(control);

            string actual = rowAccessibleObject.Name;

            Assert.Null(actual);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsFact]
        public void CalendarRowAccessibleObject_Parent_ReturnsExpected()
        {
            using MonthCalendar control = new();
            MonthCalendarAccessibleObject controlAccessibleObject = (MonthCalendarAccessibleObject)control.AccessibilityObject;
            CalendarAccessibleObject calendarAccessibleObject = new(controlAccessibleObject, 0, "Test name");
            CalendarBodyAccessibleObject bodyAccessibleObject = new(calendarAccessibleObject, controlAccessibleObject, 0);
            CalendarRowAccessibleObject rowAccessibleObject = new(bodyAccessibleObject, controlAccessibleObject, 0, 0);

            AccessibleObject actual = rowAccessibleObject.Parent;

            Assert.Equal(bodyAccessibleObject, actual);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsFact]
        public void CalendarRowAccessibleObject_Role_IsRow()
        {
            using MonthCalendar control = new();
            CalendarRowAccessibleObject rowAccessibleObject = CreateCalendarRowAccessibleObject(control);

            AccessibleRole actual = rowAccessibleObject.Role;

            Assert.Equal(AccessibleRole.Row, actual);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        public void CalendarRowAccessibleObject_Row_ReturnsExpected(int rowIndex)
        {
            using MonthCalendar control = new();
            CalendarRowAccessibleObject rowAccessibleObject = CreateCalendarRowAccessibleObject(control, 0, rowIndex);

            int actual = rowAccessibleObject.Row;

            Assert.Equal(rowIndex, actual);
            Assert.False(control.IsHandleCreated);
        }

        private CalendarRowAccessibleObject CreateCalendarRowAccessibleObject(MonthCalendar control, int calendarIndex = 0, int rowIndex = 0)
        {
            MonthCalendarAccessibleObject controlAccessibleObject = (MonthCalendarAccessibleObject)control.AccessibilityObject;
            CalendarAccessibleObject calendarAccessibleObject = new(controlAccessibleObject, calendarIndex, "Test name");
            CalendarBodyAccessibleObject bodyAccessibleObject = new(calendarAccessibleObject, controlAccessibleObject, calendarIndex);
            CalendarRowAccessibleObject rowAccessibleObject = new(bodyAccessibleObject, controlAccessibleObject, calendarIndex, rowIndex);

            return rowAccessibleObject;
        }
    }
}
