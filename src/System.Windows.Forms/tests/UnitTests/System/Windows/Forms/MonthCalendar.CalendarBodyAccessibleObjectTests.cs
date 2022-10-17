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
    public class MonthCalendar_CalendarBodyAccessibleObjectTests : IClassFixture<ThreadExceptionFixture>
    {
        [WinFormsFact]
        public void CalendarBodyAccessibleObject_ctor_default()
        {
            using MonthCalendar control = new MonthCalendar();
            MonthCalendarAccessibleObject controlAccessibleObject = (MonthCalendarAccessibleObject)control.AccessibilityObject;
            CalendarAccessibleObject calendarAccessibleObject = new(controlAccessibleObject, 0, "Test name");
            CalendarBodyAccessibleObject accessibleObject = new(calendarAccessibleObject, controlAccessibleObject, 0);

            Assert.Equal(calendarAccessibleObject, accessibleObject.Parent);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsFact]
        public void CalendarBodyAccessibleObject_Bounds_HasExpectedSize()
        {
            using MonthCalendar control = new MonthCalendar();

            control.CreateControl();
            CalendarBodyAccessibleObject accessibleObject = CreateCalendarBodyAccessibleObject(control);

            Assert.Equal(217, accessibleObject.Bounds.Width);
            Assert.Equal(106, accessibleObject.Bounds.Height);
            Assert.True(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [InlineData((int)MCMV.MONTH, 7)]
        [InlineData((int)MCMV.YEAR, 4)]
        [InlineData((int)MCMV.DECADE, 4)]
        [InlineData((int)MCMV.CENTURY, 4)]
        public void CalendarBodyAccessibleObject_ColumnCount_ReturnsExpected(int view, int expected)
        {
            using MonthCalendar control = new MonthCalendar();

            control.CreateControl();
            PInvoke.SendMessage(control, (User32.WM)MCM.SETCURRENTVIEW, 0, view);
            CalendarBodyAccessibleObject accessibleObject = CreateCalendarBodyAccessibleObject(control);

            Assert.Equal(expected, accessibleObject.ColumnCount);
            Assert.True(control.IsHandleCreated);
        }

        [WinFormsFact]
        public void CalendarBodyAccessibleObject_GetChildId_ReturnsExpected()
        {
            using MonthCalendar control = new MonthCalendar();
            CalendarBodyAccessibleObject accessibleObject = CreateCalendarBodyAccessibleObject(control);

            Assert.Equal(2, accessibleObject.GetChildId());
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [InlineData((int)MCMV.YEAR)]
        [InlineData((int)MCMV.DECADE)]
        [InlineData((int)MCMV.CENTURY)]
        public void CalendarBodyAccessibleObject_GetColumnHeaders_IsNull_IfViewIsNotMonth(int view)
        {
            using MonthCalendar control = new MonthCalendar();

            control.CreateControl();
            PInvoke.SendMessage(control, (User32.WM)MCM.SETCURRENTVIEW, 0, view);
            CalendarBodyAccessibleObject accessibleObject = CreateCalendarBodyAccessibleObject(control);

            Assert.Null(accessibleObject.GetColumnHeaders());
            Assert.True(control.IsHandleCreated);
        }

        [WinFormsFact]
        public void CalendarBodyAccessibleObject_GetColumnHeaders_HasSevenCells()
        {
            using MonthCalendar control = new MonthCalendar();

            control.CreateControl();
            CalendarBodyAccessibleObject accessibleObject = CreateCalendarBodyAccessibleObject(control);

            Assert.Equal(7, accessibleObject.GetColumnHeaders().Length); // Contains days of week
            Assert.True(control.IsHandleCreated);
        }

        [WinFormsFact]
        public void CalendarBodyAccessibleObject_ControlType_IsTable()
        {
            using MonthCalendar control = new MonthCalendar();
            CalendarBodyAccessibleObject accessibleObject = CreateCalendarBodyAccessibleObject(control);

            Assert.Equal(UiaCore.UIA.TableControlTypeId, accessibleObject.GetPropertyValue(UiaCore.UIA.ControlTypePropertyId));
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [InlineData((int)UiaCore.UIA.GridPatternId)]
        [InlineData((int)UiaCore.UIA.TablePatternId)]
        public void CalendarBodyAccessibleObject_Supports_GridAndTablePatterns(int pattern)
        {
            using MonthCalendar control = new MonthCalendar();
            CalendarBodyAccessibleObject accessibleObject = CreateCalendarBodyAccessibleObject(control);

            Assert.True(accessibleObject.IsPatternSupported((UiaCore.UIA)pattern));
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [InlineData((int)MCMV.MONTH, 0, "January 2021")]
        [InlineData((int)MCMV.MONTH, 1, "February 2021")]
        [InlineData((int)MCMV.MONTH, 2, "March 2021")]
        [InlineData((int)MCMV.MONTH, 3, "April 2021")]
        [InlineData((int)MCMV.YEAR, 0, "2021")]
        [InlineData((int)MCMV.YEAR, 1, "2022")]
        [InlineData((int)MCMV.YEAR, 2, "2023")]
        [InlineData((int)MCMV.YEAR, 3, "2024")]
        [InlineData((int)MCMV.DECADE, 0, "2020-2029")]
        [InlineData((int)MCMV.DECADE, 1, "2030-2039")]
        [InlineData((int)MCMV.DECADE, 2, "2040-2049")]
        [InlineData((int)MCMV.DECADE, 3, "2050-2059")]
        [InlineData((int)MCMV.CENTURY, 0, "2000-2099")]
        [InlineData((int)MCMV.CENTURY, 1, "2100-2199")]
        [InlineData((int)MCMV.CENTURY, 2, "2200-2299")]
        [InlineData((int)MCMV.CENTURY, 3, "2300-2399")]
        public void CalendarBodyAccessibleObject_Name_IsExpected(int view, int calendarIndex, string expected)
        {
            using MonthCalendar control = new MonthCalendar();
            control.FirstDayOfWeek = Day.Sunday;
            control.Size = new Size(450, 450);
            control.SelectionStart = new DateTime(2021, 1, 1);

            control.CreateControl();
            PInvoke.SendMessage(control, (User32.WM)MCM.SETCURRENTVIEW, 0, (nint)view);
            MonthCalendarAccessibleObject controlAccessibleObject = (MonthCalendarAccessibleObject)control.AccessibilityObject;
            LinkedListNode<CalendarAccessibleObject> calendarNode = controlAccessibleObject.CalendarsAccessibleObjects.First;

            for (int i = 1; i <= calendarIndex; i++)
            {
                calendarNode = calendarNode.Next;
            }

            Assert.NotNull(calendarNode);

            CalendarBodyAccessibleObject calendarBody = calendarNode.Value.CalendarBodyAccessibleObject;

            Assert.Equal(expected, calendarBody.Name);
            Assert.True(control.IsHandleCreated);
        }

        [WinFormsFact]
        public void CalendarBodyAccessibleObject_Parent_IsCalendarAccessibleObject()
        {
            using MonthCalendar control = new MonthCalendar();
            MonthCalendarAccessibleObject controlAccessibleObject = (MonthCalendarAccessibleObject)control.AccessibilityObject;
            CalendarAccessibleObject calendarAccessibleObject = new(controlAccessibleObject, 0, "Test name");
            CalendarBodyAccessibleObject accessibleObject = new(calendarAccessibleObject, controlAccessibleObject, 0);

            Assert.Equal(calendarAccessibleObject, accessibleObject.Parent);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsFact]
        public void CalendarBodyAccessibleObject_Role_IsTable()
        {
            using MonthCalendar control = new MonthCalendar();
            CalendarBodyAccessibleObject accessibleObject = CreateCalendarBodyAccessibleObject(control);

            Assert.Equal(AccessibleRole.Table, accessibleObject.Role);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsFact]
        public void CalendarBodyAccessibleObject_State_IsDefault()
        {
            using MonthCalendar control = new MonthCalendar();
            CalendarBodyAccessibleObject accessibleObject = CreateCalendarBodyAccessibleObject(control);

            Assert.Equal(AccessibleStates.Default, accessibleObject.State);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsFact]
        public void CalendarBodyAccessibleObject_FragmentNavigate_Parent_ReturnsExpected()
        {
            using MonthCalendar control = new();
            MonthCalendarAccessibleObject controlAccessibleObject = (MonthCalendarAccessibleObject)control.AccessibilityObject;
            CalendarAccessibleObject calendar = new(controlAccessibleObject, 0, "Test name");
            CalendarBodyAccessibleObject calendarBody = new(calendar, controlAccessibleObject, 0);

            Assert.Equal(calendar, calendarBody.FragmentNavigate(UiaCore.NavigateDirection.Parent));
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsFact]
        public void CalendarBodyAccessibleObject_FragmentNavigate_NextSibling_ReturnsNull()
        {
            using MonthCalendar control = new();
            CalendarBodyAccessibleObject accessibleObject = CreateCalendarBodyAccessibleObject(control, 0);

            Assert.Null(accessibleObject.FragmentNavigate(UiaCore.NavigateDirection.NextSibling));
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsFact]
        public void CalendarBodyAccessibleObject_FragmentNavigate_PreviousSibling_ReturnsExpected()
        {
            using MonthCalendar control = new();
            MonthCalendarAccessibleObject controlAccessibleObject = (MonthCalendarAccessibleObject)control.AccessibilityObject;
            CalendarAccessibleObject calendar = new(controlAccessibleObject, 0, "Test name");
            CalendarBodyAccessibleObject calendarBody = new(calendar, controlAccessibleObject, 0);

            AccessibleObject expected = calendar.CalendarHeaderAccessibleObject;

            Assert.Equal(expected, calendarBody.FragmentNavigate(UiaCore.NavigateDirection.PreviousSibling));
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsFact]
        public void CalendarBodyAccessibleObject_FragmentNavigate_Child_ReturnsExpected()
        {
            using MonthCalendar control = new();
            CalendarBodyAccessibleObject accessibleObject = CreateCalendarBodyAccessibleObject(control, 0);

            AccessibleObject firstRow = accessibleObject.RowsAccessibleObjects?.First();
            AccessibleObject lastRow = accessibleObject.RowsAccessibleObjects?.Last();

            Assert.Equal(firstRow, accessibleObject.FragmentNavigate(UiaCore.NavigateDirection.FirstChild));
            Assert.Equal(lastRow, accessibleObject.FragmentNavigate(UiaCore.NavigateDirection.LastChild));
            Assert.False(control.IsHandleCreated);
        }

        private CalendarBodyAccessibleObject CreateCalendarBodyAccessibleObject(MonthCalendar control, int calendarIndex = 0)
        {
            MonthCalendarAccessibleObject controlAccessibleObject = (MonthCalendarAccessibleObject)control.AccessibilityObject;
            CalendarAccessibleObject calendarAccessibleObject = new(controlAccessibleObject, calendarIndex, "Test name");
            CalendarBodyAccessibleObject bodyAccessibleObject = new(calendarAccessibleObject, controlAccessibleObject, calendarIndex);

            return bodyAccessibleObject;
        }
    }
}
