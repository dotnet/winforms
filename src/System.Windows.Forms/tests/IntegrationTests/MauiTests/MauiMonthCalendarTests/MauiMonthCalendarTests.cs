// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Drawing;
using System.Windows.Forms.IntegrationTests.Common;
using ReflectTools;
using WFCTestLib.Log;
using static System.Windows.Forms.MonthCalendar;
using static Interop.ComCtl32;
using static Interop.Kernel32;
using static Interop.User32;

namespace System.Windows.Forms.IntegrationTests.MauiTests
{
    public class MauiMonthCalendarTests : ReflectBase
    {
        private struct MonthCalendarWrapper : IDisposable
        {
            private ReflectBase _container;

            public MonthCalendarWrapper(ReflectBase container)
            {
                _container = container;
                Calendar = new MonthCalendar();
                _container.Controls.Add(Calendar);
            }

            public MonthCalendar Calendar { get; }

            public void Dispose()
            {
                _container.Controls.Remove(Calendar);
                Calendar.Dispose();
            }
        }

        public MauiMonthCalendarTests(string[] args) : base(args)
        {
            this.BringToForeground();
        }

        public static void Main(string[] args)
        {
            Thread.CurrentThread.SetCulture("en-US");
            Application.Run(new MauiMonthCalendarTests(args));
        }

        [Scenario(true)]
        public ScenarioResult MonthCalendar_SettingDate_DoesntCrashApplication_IfUseMouse(TParams p)
        {
            using var wrapper = new MonthCalendarWrapper(this);
            wrapper.Calendar.MinDate = new DateTime(2020, 4, 9);
            wrapper.Calendar.MaxDate = new DateTime(2020, 4, 27);
            wrapper.Calendar.SetDate(new DateTime(2020, 4, 14));
            Application.DoEvents();

            Point position = wrapper.Calendar.PointToScreen(new Point(82, 102));
            MouseHelper.SendClick(position.X, position.Y);
            Application.DoEvents();

            return new ScenarioResult(true);
        }

        [Scenario(true)]
        public ScenarioResult MonthCalendar_SettingDate_DoesntCrashApplication_IfUseKeyboard(TParams p)
        {
            using var wrapper = new MonthCalendarWrapper(this);
            wrapper.Calendar.MinDate = new DateTime(2020, 4, 9);
            wrapper.Calendar.MaxDate = new DateTime(2020, 4, 27);
            wrapper.Calendar.SetDate(new DateTime(2020, 4, 14));
            Application.DoEvents();

            KeyboardHelper.SendKey(Keys.Right, true);
            Application.DoEvents();

            KeyboardHelper.SendKey(Keys.Right, true);
            Application.DoEvents();

            return new ScenarioResult(true);
        }

        [Scenario(true)]
        public ScenarioResult MonthCalendar_SettingDate_DoesntCrashApplication_Programmatically(TParams p)
        {
            using var wrapper = new MonthCalendarWrapper(this);
            wrapper.Calendar.MinDate = new DateTime(2020, 4, 9);
            wrapper.Calendar.MaxDate = new DateTime(2020, 4, 27);
            wrapper.Calendar.SetDate(new DateTime(2020, 4, 14));
            Application.DoEvents();

            DateTime selectedDate = new DateTime(2020, 4, 10);

            SYSTEMTIME date = new SYSTEMTIME
            {
                wYear = (short)selectedDate.Year,
                wMonth = (short)selectedDate.Month,
                wDay = (short)selectedDate.Day
            };

            if (IntPtr.Zero == wrapper.Calendar.Handle)
            {
                return new ScenarioResult(false);
            }

            NMSELCHANGE lParam = new NMSELCHANGE
            {
                nmhdr = new NMHDR
                {
                    code = (int)MCN.SELCHANGE,
                },
                stSelStart = date,
                stSelEnd = date,
            };

            SendMessageW(wrapper.Calendar.Handle, WM.REFLECT | WM.NOTIFY, 0, ref lParam);

            return new ScenarioResult(true);
        }

        [Scenario(true)]
        public ScenarioResult MonthCalendar_GetFromPoint_ReturnsCorrectValue(TParams p)
        {
            using var wrapper = new MonthCalendarWrapper(this);
            Application.DoEvents();
            MonthCalendarAccessibleObject accessibleObject = (MonthCalendarAccessibleObject)wrapper.Calendar.AccessibilityObject;
            CalendarAccessibleObject calendarAccessibleObject = new CalendarAccessibleObject(accessibleObject, 0, "Test name");
            MCHITTESTINFO info = new MCHITTESTINFO
            {
                uHit = MCHT.CALENDARDAY,
                iRow = 0
            };
            Application.DoEvents();
            MonthCalendarChildAccessibleObject cell = calendarAccessibleObject.GetChildFromPoint(info);

            return new ScenarioResult(cell != null);
        }

        [Scenario(true)]
        public ScenarioResult MonthCalendar_Click_Date_InvokeEvents(TParams p)
        {
            using var wrapper = new MonthCalendarWrapper(this);
            DateTime newDate = DateTime.Today.AddDays(1);
            Action click = () => MouseHelper.SendClick(GetCellPositionByDate(wrapper.Calendar, newDate));
            (int CallDateSelectedCount, int CallDateChangedCount, DateTime SelectedDate) testData = MonthCalendar_MinimumMaximum_Action(wrapper, click);

            if (testData.CallDateSelectedCount == 0)
            {
                return new ScenarioResult(false, "`DateSelected` event is not firing");
            }

            if (testData.CallDateChangedCount == 0)
            {
                return new ScenarioResult(false, "`DateChanged` event is not firing");
            }

            return new ScenarioResult(testData.SelectedDate.Date == newDate.Date, "The selected date has not changed");
        }

        [Scenario(true)]
        public ScenarioResult MonthCalendar_Click_MinimumDate_InvokeEvents(TParams p)
        {
            using var wrapper = new MonthCalendarWrapper(this);
            DateTime newDate = DateTime.Today.AddDays(-1);
            Action click = () => MouseHelper.SendClick(GetCellPositionByDate(wrapper.Calendar, newDate));
            (int CallDateSelectedCount, int CallDateChangedCount, DateTime SelectedDate) testData = MonthCalendar_MinimumMaximum_Action(wrapper, click);

            if (testData.CallDateSelectedCount == 0)
            {
                return new ScenarioResult(false, "`DateSelected` event is not firing");
            }

            if (testData.CallDateChangedCount == 0)
            {
                return new ScenarioResult(false, "`DateChanged` event is not firing");
            }

            return new ScenarioResult(testData.SelectedDate.Date == newDate.Date, "The selected date has not changed");
        }

        [Scenario(true)]
        public ScenarioResult MonthCalendar_Click_MaximumDate_InvokeEvents(TParams p)
        {
            using var wrapper = new MonthCalendarWrapper(this);
            DateTime newDate = DateTime.Today.AddDays(2);
            Action click = () => MouseHelper.SendClick(GetCellPositionByDate(wrapper.Calendar, newDate));
            (int CallDateSelectedCount, int CallDateChangedCount, DateTime SelectedDate) testData = MonthCalendar_MinimumMaximum_Action(wrapper, click);

            if (testData.CallDateSelectedCount == 0)
            {
                return new ScenarioResult(false, "`DateSelected` event is not firing");
            }

            if (testData.CallDateChangedCount == 0)
            {
                return new ScenarioResult(false, "`DateChanged` event is not firing");
            }

            return new ScenarioResult(testData.SelectedDate.Date == newDate.Date, "The selected date has not changed");
        }

        [Scenario(true)]
        public ScenarioResult MonthCalendar_DoubleClick_Date_InvokeEvents(TParams p)
        {
            using var wrapper = new MonthCalendarWrapper(this);
            DateTime newDate = DateTime.Today.AddDays(1);
            Action click = () =>
            {
                MouseHelper.SendClick(GetCellPositionByDate(wrapper.Calendar, newDate));
                Thread.Sleep(500);
                MouseHelper.SendClick(GetCellPositionByDate(wrapper.Calendar, newDate));
            };

            (int CallDateSelectedCount, int CallDateChangedCount, DateTime SelectedDate) testData = MonthCalendar_MinimumMaximum_Action(wrapper, click);

            if (testData.CallDateSelectedCount != 2)
            {
                return new ScenarioResult(false, "`DateSelected` event should be firing twice");
            }

            if (testData.CallDateChangedCount != 1)
            {
                return new ScenarioResult(false, "`DateChanged` event should be firing only one time");
            }

            return new ScenarioResult(testData.SelectedDate.Date == newDate.Date, "The selected date has not changed");
        }

        [Scenario(true)]
        public ScenarioResult MonthCalendar_DoubleClick_MinDate_InvokeEvents(TParams p)
        {
            using var wrapper = new MonthCalendarWrapper(this);
            DateTime newDate = DateTime.Today.AddDays(-1);
            Action click = () =>
            {
                MouseHelper.SendClick(GetCellPositionByDate(wrapper.Calendar, newDate));
                Thread.Sleep(500);
                MouseHelper.SendClick(GetCellPositionByDate(wrapper.Calendar, newDate));
            };

            (int CallDateSelectedCount, int CallDateChangedCount, DateTime SelectedDate) testData = MonthCalendar_MinimumMaximum_Action(wrapper, click);

            if (testData.CallDateSelectedCount != 2)
            {
                return new ScenarioResult(false, "`DateSelected` event should be firing twice");
            }

            if (testData.CallDateChangedCount != 1)
            {
                return new ScenarioResult(false, "`DateChanged` event should be firing only one time");
            }

            return new ScenarioResult(testData.SelectedDate.Date == newDate.Date, "The selected date has not changed");
        }

        [Scenario(true)]
        public ScenarioResult MonthCalendar_DoubleClick_MaxDate_InvokeEvents(TParams p)
        {
            using var wrapper = new MonthCalendarWrapper(this);
            DateTime newDate = DateTime.Today.AddDays(2);
            Action click = () =>
            {
                MouseHelper.SendClick(GetCellPositionByDate(wrapper.Calendar, newDate));
                Thread.Sleep(500);
                MouseHelper.SendClick(GetCellPositionByDate(wrapper.Calendar, newDate));
            };

            (int CallDateSelectedCount, int CallDateChangedCount, DateTime SelectedDate) testData = MonthCalendar_MinimumMaximum_Action(wrapper, click);

            if (testData.CallDateSelectedCount != 2)
            {
                return new ScenarioResult(false, "`DateSelected` event should be firing twice");
            }

            if (testData.CallDateChangedCount != 1)
            {
                return new ScenarioResult(false, "`DateChanged` event should be firing only one time");
            }

            return new ScenarioResult(testData.SelectedDate.Date == newDate.Date, "The selected date has not changed");
        }

        private Point GetCellPositionByDate(MonthCalendar calendar, DateTime dateTime)
        {
            MonthCalendarAccessibleObject accessibleObject = (MonthCalendarAccessibleObject)calendar.AccessibilityObject;
            return accessibleObject.TestAccessor().Dynamic.GetCellByDate(dateTime.Date).Bounds.Location;
        }

        private (int callDateSelectedCount, int callDateChangedCount, DateTime selectedDate) MonthCalendar_MinimumMaximum_Action(MonthCalendarWrapper wrapper, Action action)
        {
            int callDateSelectedCount = 0;
            int callDateChangedCount = 0;

            Application.DoEvents();

            wrapper.Calendar.MinDate = DateTime.Now.AddDays(-1);
            wrapper.Calendar.MaxDate = DateTime.Now.AddDays(2);
            wrapper.Calendar.DateSelected += calendar_DateSelected;
            wrapper.Calendar.DateChanged += calendar_DateChanged;

            Application.DoEvents();

            action();

            Application.DoEvents();

            wrapper.Calendar.MinDate = DateTimePicker.MinimumDateTime;
            wrapper.Calendar.MaxDate = DateTimePicker.MaximumDateTime;
            wrapper.Calendar.DateSelected -= calendar_DateSelected;
            wrapper.Calendar.DateChanged -= calendar_DateChanged;

            return (callDateSelectedCount, callDateChangedCount, wrapper.Calendar.SelectionStart.Date);

            void calendar_DateSelected(object sender, DateRangeEventArgs e) => callDateSelectedCount++;
            void calendar_DateChanged(object sender, DateRangeEventArgs e) => callDateChangedCount++;
        }
    }
}
