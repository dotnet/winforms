// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Drawing;
using System.Windows.Forms.UI.IntegrationTests.Infra;
using Xunit;
using static System.Windows.Forms.MonthCalendar;
using static Interop.ComCtl32;
using static Interop.Kernel32;
using static Interop.User32;

namespace System.Windows.Forms.UI.IntegrationTests
{
    [ConfigureJoinableTaskFactory]
    public class MonthCalendarTests
    {
        [StaFact]
        public void MonthCalendar_SettingDate_DoesntCrashApplication_IfUseMouse()
        {
            RunTest(calendar =>
            {
                calendar.MinDate = new DateTime(2020, 4, 9);
                calendar.MaxDate = new DateTime(2020, 4, 27);
                calendar.SetDate(new DateTime(2020, 4, 14));
                Application.DoEvents();

                Point position = calendar.PointToScreen(new Point(82, 102));
                MouseHelper.SendClick(position.X, position.Y);
                Application.DoEvents();
            });
        }

        [StaFact]
        public void MonthCalendar_SettingDate_DoesntCrashApplication_IfUseKeyboard()
        {
            RunTest(calendar =>
            {
                calendar.MinDate = new DateTime(2020, 4, 9);
                calendar.MaxDate = new DateTime(2020, 4, 27);
                calendar.SetDate(new DateTime(2020, 4, 14));
                Application.DoEvents();

                KeyboardHelper.SendKey(Keys.Right, true);
                Application.DoEvents();

                KeyboardHelper.SendKey(Keys.Right, true);
                Application.DoEvents();
            });
        }

        [StaFact]
        public void MonthCalendar_SettingDate_DoesntCrashApplication_Programmatically()
        {
            RunTest(calendar =>
            {
                calendar.MinDate = new DateTime(2020, 4, 9);
                calendar.MaxDate = new DateTime(2020, 4, 27);
                calendar.SetDate(new DateTime(2020, 4, 14));
                Application.DoEvents();

                KeyboardHelper.SendKey(Keys.Right, true);
                Application.DoEvents();

                KeyboardHelper.SendKey(Keys.Right, true);
                Application.DoEvents();

                DateTime selectedDate = new DateTime(2020, 4, 10);
                SYSTEMTIME date = new SYSTEMTIME
                {
                    wYear = (short)selectedDate.Year,
                    wMonth = (short)selectedDate.Month,
                    wDay = (short)selectedDate.Day
                };

                NMSELCHANGE lParam = new NMSELCHANGE
                {
                    nmhdr = new NMHDR
                    {
                        code = (int)MCN.SELCHANGE,
                    },
                    stSelStart = date,
                    stSelEnd = date,
                };

                SendMessageW(calendar.Handle, WM.REFLECT | WM.NOTIFY, 0, ref lParam);
            });
        }

        [StaFact]
        public void MonthCalendar_GetFromPoint_ReturnsCorrectValue()
        {
            RunTest(calendar =>
            {
                MonthCalendarAccessibleObject accessibleObject = (MonthCalendarAccessibleObject)calendar.AccessibilityObject;
                CalendarAccessibleObject calendarAccessibleObject = new(accessibleObject, 0, "Test name");

                MCHITTESTINFO info = new()
                {
                    uHit = MCHT.CALENDARDAY,
                    iRow = 0
                };
                MonthCalendarChildAccessibleObject cell = calendarAccessibleObject.GetChildFromPoint(info);

                Assert.NotNull(cell);
            });
        }

        [StaFact]
        public void MonthCalendar_Click_Date_InvokeEvents()
        {
            RunTest(calendar =>
            {
                DateTime newDate = DateTime.Today.AddDays(1);
                void SendClick(MonthCalendar c) => MouseHelper.SendClick(GetCellPositionByDate(c, newDate));
                (int CallDateSelectedCount, int CallDateChangedCount, DateTime SelectedDate) testData = MonthCalendar_MinimumMaximum_Action(calendar, SendClick);

                Assert.Equal(newDate.Date, testData.SelectedDate.Date);
                Assert.NotEqual(0, testData.CallDateSelectedCount);
                Assert.NotEqual(0, testData.CallDateChangedCount);
            });
        }

        [StaFact]
        public void MonthCalendar_Click_MinimumDate_InvokeEvents()
        {
            RunTest(calendar =>
            {
                DateTime newDate = DateTime.Today.AddDays(-1);
                void SendClick(MonthCalendar c) => MouseHelper.SendClick(GetCellPositionByDate(c, newDate));
                (int CallDateSelectedCount, int CallDateChangedCount, DateTime SelectedDate) testData = MonthCalendar_MinimumMaximum_Action(calendar, SendClick);

                Assert.Equal(newDate.Date, testData.SelectedDate.Date);
                Assert.NotEqual(0, testData.CallDateSelectedCount);
                Assert.NotEqual(0, testData.CallDateChangedCount);
            });
        }

        [StaFact]
        public void MonthCalendar_Click_MaximumDate_InvokeEvents()
        {
            RunTest(calendar =>
            {
                DateTime newDate = DateTime.Today.AddDays(2);
                void SendClick(MonthCalendar c) => MouseHelper.SendClick(GetCellPositionByDate(c, newDate));
                (int CallDateSelectedCount, int CallDateChangedCount, DateTime SelectedDate) testData = MonthCalendar_MinimumMaximum_Action(calendar, SendClick);

                Assert.NotEqual(0, testData.CallDateSelectedCount);

                Assert.NotEqual(0, testData.CallDateChangedCount);

                Assert.Equal(testData.SelectedDate.Date, newDate.Date);
            });
        }

        private static Point GetCellPositionByDate(MonthCalendar calendar, DateTime dateTime)
        {
            MonthCalendarAccessibleObject accessibleObject = (MonthCalendarAccessibleObject)calendar.AccessibilityObject;
            return accessibleObject.TestAccessor().Dynamic.GetCellByDate(dateTime.Date).Bounds.Location;
        }

        private static (int callDateSelectedCount, int callDateChangedCount, DateTime selectedDate) MonthCalendar_MinimumMaximum_Action(MonthCalendar calendar, Action<MonthCalendar> action)
        {
            int callDateSelectedCount = 0;
            int callDateChangedCount = 0;

            Application.DoEvents();

            calendar.MinDate = DateTime.Now.AddDays(-1);
            calendar.MaxDate = DateTime.Now.AddDays(2);
            calendar.DateSelected += calendar_DateSelected;
            calendar.DateChanged += calendar_DateChanged;

            Application.DoEvents();

            action(calendar);

            Application.DoEvents();

            calendar.MinDate = DateTimePicker.MinimumDateTime;
            calendar.MaxDate = DateTimePicker.MaximumDateTime;
            calendar.DateSelected -= calendar_DateSelected;
            calendar.DateChanged -= calendar_DateChanged;

            return (callDateSelectedCount, callDateChangedCount, calendar.SelectionStart.Date);

            void calendar_DateSelected(object? sender, DateRangeEventArgs e) => callDateSelectedCount++;
            void calendar_DateChanged(object? sender, DateRangeEventArgs e) => callDateChangedCount++;
        }

        private void RunTest(Action<MonthCalendar> runTest)
        {
            UITest.RunControl(
                createControl: form =>
                {
                    MonthCalendar control = new()
                    {
                        Parent = form,
                        Location = new Point(0, 0)
                    };

                    return control;
                },
                runTestAsync: async control =>
                {
                    // Wait for pending operations so the Control is loaded completely before testing it
                    await AsyncTestHelper.JoinPendingOperationsAsync(AsyncTestHelper.UnexpectedTimeout);

                    Assert.NotEqual(IntPtr.Zero, control.Handle);

                    runTest(control);
                });
        }
    }
}
