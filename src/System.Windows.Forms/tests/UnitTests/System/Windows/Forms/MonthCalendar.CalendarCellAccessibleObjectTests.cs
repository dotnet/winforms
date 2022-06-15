// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Drawing;
using Xunit;
using static System.Windows.Forms.MonthCalendar;
using static Interop;
using static Interop.ComCtl32;

namespace System.Windows.Forms.Tests
{
    [UseDefaultXunitCulture]
    public class MonthCalendar_CalendarCellAccessibleObjectTests : IClassFixture<ThreadExceptionFixture>
    {
        [WinFormsFact]
        public void CalendarCellAccessibleObject_ctor_default()
        {
            using MonthCalendar control = new();
            CalendarCellAccessibleObject cellAccessibleObject = CreateCalendarCellAccessibleObject(control);

            Assert.Equal(0, cellAccessibleObject.TestAccessor().Dynamic._calendarIndex);
            Assert.Equal(0, cellAccessibleObject.TestAccessor().Dynamic._rowIndex);
            Assert.Equal(0, cellAccessibleObject.TestAccessor().Dynamic._columnIndex);
            Assert.False(control.IsHandleCreated);
        }

        public static IEnumerable<object[]> CalendarCellAccessibleObject_Bounds_ReturnsExpected_TestData()
        {
            yield return new object[] { 0, 0, new Rectangle(13, 81, 31, 15) };
            yield return new object[] { 0, 1, new Rectangle(44, 81, 31, 15) };
            yield return new object[] { 0, 2, new Rectangle(75, 81, 31, 15) };
            yield return new object[] { 1, 0, new Rectangle(13, 96, 31, 15) };
            yield return new object[] { 1, 1, new Rectangle(44, 96, 31, 15) };
            yield return new object[] { 1, 2, new Rectangle(75, 96, 31, 15) };
        }

        [WinFormsTheory]
        [MemberData(nameof(CalendarCellAccessibleObject_Bounds_ReturnsExpected_TestData))]
        public void CalendarCellAccessibleObject_Bounds_ReturnsExpected(int rowIndex, int columnIndex, Rectangle expected)
        {
            using MonthCalendar control = new();
            CalendarCellAccessibleObject cellAccessibleObject = CreateCalendarCellAccessibleObject(control, 0, rowIndex, columnIndex);

            control.CreateControl();
            Rectangle actual = cellAccessibleObject.Bounds;

            Assert.Equal(expected, actual);
            Assert.True(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        public void CalendarCellAccessibleObject_CalendarIndex_ReturnsExpected(int calendarIndex)
        {
            using MonthCalendar control = new();
            CalendarCellAccessibleObject cellAccessibleObject = CreateCalendarCellAccessibleObject(control, calendarIndex);

            Assert.Equal(calendarIndex, cellAccessibleObject.CalendarIndex);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        public void CalendarCellAccessibleObject_Column_ReturnsExpected(int columnIndex)
        {
            using MonthCalendar control = new();
            CalendarCellAccessibleObject cellAccessibleObject = CreateCalendarCellAccessibleObject(control, 0, 0, columnIndex);

            Assert.Equal(columnIndex, cellAccessibleObject.Column);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsFact]
        public void CalendarCellAccessibleObject_ContainingGrid_ReturnsExpected()
        {
            using MonthCalendar control = new();
            var controlAccessibleObject = (MonthCalendarAccessibleObject)control.AccessibilityObject;
            CalendarAccessibleObject calendarAccessibleObject = new(controlAccessibleObject, 0, "Test name");
            CalendarBodyAccessibleObject bodyAccessibleObject = new(calendarAccessibleObject, controlAccessibleObject, 0);
            CalendarRowAccessibleObject rowAccessibleObject = new(bodyAccessibleObject, controlAccessibleObject, 0, 0);
            CalendarCellAccessibleObject cellAccessibleObject = new(rowAccessibleObject, bodyAccessibleObject, controlAccessibleObject, 0, 0, 0);

            Assert.Equal(bodyAccessibleObject, cellAccessibleObject.ContainingGrid);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        public void CalendarCellAccessibleObject_GetChildId_ReturnsExpected(int columnIndex)
        {
            using MonthCalendar control = new();
            CalendarCellAccessibleObject cellAccessibleObject = CreateCalendarCellAccessibleObject(control, 0, 0, columnIndex);

            int actual = cellAccessibleObject.GetChildId();

            Assert.Equal(columnIndex + 1, actual);
            Assert.False(control.IsHandleCreated);
        }

        private CalendarCellAccessibleObject CreateCalendarCellAccessibleObject(MonthCalendar control, int calendarIndex = 0, int rowIndex = 0, int columnIndex = 0)
        {
            var controlAccessibleObject = (MonthCalendarAccessibleObject)control.AccessibilityObject;
            CalendarAccessibleObject calendarAccessibleObject = new(controlAccessibleObject, calendarIndex, "Test name");
            CalendarBodyAccessibleObject bodyAccessibleObject = new(calendarAccessibleObject, controlAccessibleObject, calendarIndex);
            CalendarRowAccessibleObject rowAccessibleObject = new(bodyAccessibleObject, controlAccessibleObject, calendarIndex, rowIndex);
            CalendarCellAccessibleObject cellAccessibleObject = new(rowAccessibleObject, bodyAccessibleObject, controlAccessibleObject, calendarIndex, rowIndex, columnIndex);

            return cellAccessibleObject;
        }

        [WinFormsFact]
        public void CalendarCellAccessibleObject_Name_IsEmptyString_IfControlIsNotCreated()
        {
            using MonthCalendar control = new();
            CalendarCellAccessibleObject cellAccessibleObject = CreateCalendarCellAccessibleObject(control, 0, 0, 0);

            Assert.Empty(cellAccessibleObject.Name);
            Assert.False(control.IsHandleCreated);
        }

        public static IEnumerable<object[]> CalendarCellAccessibleObject_Name_ReturnsExpected_TestData()
        {
            yield return new object[] { MCMV.MONTH, "Wednesday, June 16, 2021" };
            yield return new object[] { MCMV.YEAR, "November 2021" };
            yield return new object[] { MCMV.DECADE, "2029" };
            yield return new object[] { MCMV.CENTURY, "2090 - 2099" };
        }

        [WinFormsTheory]
        [MemberData(nameof(CalendarCellAccessibleObject_Name_ReturnsExpected_TestData))]
        public void CalendarCellAccessibleObject_Name_ReturnsExpected(int view, string expected)
        {
            using MonthCalendar control = new();
            control.FirstDayOfWeek = Day.Monday;
            control.SelectionStart = new DateTime(2021, 6, 16); // Set a date to have a stable test case

            control.CreateControl();
            User32.SendMessageW(control, (User32.WM)MCM.SETCURRENTVIEW, 0, view);

            CalendarCellAccessibleObject cellAccessibleObject = CreateCalendarCellAccessibleObject(control, 0, 2, 2);

            Assert.Equal(expected, cellAccessibleObject.Name);
            Assert.True(control.IsHandleCreated);
        }
    }
}
