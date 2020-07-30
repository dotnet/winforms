// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Threading;
using WFCTestLib.Log;
using ReflectTools;
using System.Windows.Forms.IntegrationTests.Common;
using static Interop.Kernel32;
using static Interop.ComCtl32;
using static Interop.User32;
using System.Drawing;
using static System.Windows.Forms.MonthCalendar;
using System.Reflection;
using static Interop.UiaCore;

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

            SendMessageW(wrapper.Calendar.Handle, WM.REFLECT | WM.NOTIFY, IntPtr.Zero, ref lParam);

            return new ScenarioResult(true);
        }

        [Scenario(true)]
        public ScenarioResult MonthCalendar_GetFromPoint_ReturnsCorrectValue(TParams p)
        {
            using var wrapper = new MonthCalendarWrapper(this);
            Application.DoEvents();
            MonthCalendarAccessibleObject accessibleObject = (MonthCalendarAccessibleObject)wrapper.Calendar.AccessibilityObject;
            CalendarBodyAccessibleObject bodyAccessibleObject = new CalendarBodyAccessibleObject(accessibleObject, 0);
            MCHITTESTINFO info = new MCHITTESTINFO
            {
                uHit = MCHT.CALENDARDAY,
                iRow = 0
            };
            Application.DoEvents();
            CalendarChildAccessibleObject cell = bodyAccessibleObject.GetFromPoint(info);

            return new ScenarioResult(cell != null);
        }

        [Scenario(true)]
        public ScenarioResult CalendarBodyAccessibleObject_GetFromPoint_ReturnsNull_IfCalendarIndexIsIncorrect(TParams p)
        {
            using var wrapper = new MonthCalendarWrapper(this);
            Application.DoEvents();
            MonthCalendarAccessibleObject accessibleObject = (MonthCalendarAccessibleObject)wrapper.Calendar.AccessibilityObject;
            CalendarBodyAccessibleObject bodyAccessibleObject = new CalendarBodyAccessibleObject(accessibleObject, -10);
            MCHITTESTINFO info = new MCHITTESTINFO
            {
                uHit = MCHT.CALENDARDAY,
                iRow = 0
            };
            Application.DoEvents();
            CalendarChildAccessibleObject cell = bodyAccessibleObject.GetFromPoint(info);

            return new ScenarioResult(cell is null);
        }

        [Scenario(true)]
        public ScenarioResult CalendarBodyAccessibleObject_GetFromPoint_ReturnsNull_IfMCHITTESTINFOIsIncorrect(TParams p)
        {
            using var wrapper = new MonthCalendarWrapper(this);
            Application.DoEvents();
            MonthCalendarAccessibleObject accessibleObject = (MonthCalendarAccessibleObject)wrapper.Calendar.AccessibilityObject;
            CalendarBodyAccessibleObject bodyAccessibleObject = new CalendarBodyAccessibleObject(accessibleObject, 0);
            MCHITTESTINFO info = new MCHITTESTINFO
            {
                uHit = MCHT.CALENDARDAY,
                iRow = -10
            };
            Application.DoEvents();
            CalendarChildAccessibleObject cell = bodyAccessibleObject.GetFromPoint(info);

            return new ScenarioResult(cell is null);
        }

        [Scenario(true)]
        public ScenarioResult MonthCalendarAccessibleObject_GetCalendarChildAccessibleObject_ReturnsCorrecObject(TParams p)
        {
            using var wrapper = new MonthCalendarWrapper(this);
            DateTime selectedDate = new DateTime(2020, 4, 10);
            wrapper.Calendar.SetDate(selectedDate);
            Application.DoEvents();
            MonthCalendarAccessibleObject accessibleObject = (MonthCalendarAccessibleObject)wrapper.Calendar.AccessibilityObject;
            Type type = typeof(MonthCalendarAccessibleObject);
            MethodInfo method = type.GetMethod("GetCalendarChildAccessibleObject", BindingFlags.NonPublic | BindingFlags.Instance);

            Application.DoEvents();
            object child = method.Invoke(accessibleObject, new object[] { selectedDate, selectedDate });

            return new ScenarioResult(child != null);
        }

        [Scenario(true)]
        public ScenarioResult MonthCalendarAccessibleObject_GetCalendarChildAccessibleObject_ReturnsNull_IfCalendarIndexIsIncorrect(TParams p)
        {
            using var wrapper = new MonthCalendarWrapper(this);
            Application.DoEvents();
            MonthCalendarAccessibleObject accessibleObject = (MonthCalendarAccessibleObject)wrapper.Calendar.AccessibilityObject;
            Type type = typeof(MonthCalendarAccessibleObject);
            type.GetField("_calendarIndex", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(accessibleObject, -1);
            MethodInfo method = type.GetMethod("GetCalendarChildAccessibleObject", BindingFlags.NonPublic | BindingFlags.Instance);

            Application.DoEvents();
            object child = method.Invoke(accessibleObject, new object[] { new DateTime(2020, 4, 10), new DateTime(2020, 4, 10) });

            return new ScenarioResult(child is null);
        }

        [Scenario(true)]
        public ScenarioResult MonthCalendarAccessibleObject_GetColumnHeaderItems_ReturnsCorrectCollection(TParams p)
        {
            using var wrapper = new MonthCalendarWrapper(this);
            Application.DoEvents();
            MonthCalendarAccessibleObject accessibleObject = (MonthCalendarAccessibleObject)wrapper.Calendar.AccessibilityObject;

            Application.DoEvents();
            IRawElementProviderSimple[] items = accessibleObject.GetColumnHeaderItems();

            return new ScenarioResult(items != null);
        }

        [Scenario(true)]
        public ScenarioResult MonthCalendarAccessibleObject_GetColumnHeaderItems_ReturnsNull_IfCalendarIndexIsIncorrect(TParams p)
        {
            using var wrapper = new MonthCalendarWrapper(this);
            Application.DoEvents();
            MonthCalendarAccessibleObject accessibleObject = (MonthCalendarAccessibleObject)wrapper.Calendar.AccessibilityObject;
            Type type = typeof(MonthCalendarAccessibleObject);
            type.GetField("_calendarIndex", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(accessibleObject, -1);

            Application.DoEvents();
            IRawElementProviderSimple[] items = accessibleObject.GetColumnHeaderItems();

            return new ScenarioResult(items is null);
        }
    }
}
