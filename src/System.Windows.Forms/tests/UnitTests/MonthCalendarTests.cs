// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Threading;
using Xunit;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using System.Reflection;
using static Interop.Kernel32;
using static System.Windows.Forms.NativeMethods;
using static Interop;

namespace System.Windows.Forms.Tests
{
    public class MonthCalendarTests
    {
        [Fact]
        public void MonthCalendar_Constructor()
        {
            var mc = new MonthCalendar();

            Assert.NotNull(mc);
            Assert.True(mc.TabStop);
        }

        public static IEnumerable<object[]> MonthCalendar_SettingDate_DoesntCrashApplication_TestData()
        {
            yield return new object[] { new MonthCalendar { MinDate = new DateTime(2020, 4, 9) } };
            yield return new object[] { new MonthCalendar { MaxDate = new DateTime(2020, 4, 22) } };
            yield return new object[] { new MonthCalendar() };
        }

        /// <summary>
        ///  We have to check that UseVisualStyles state isn't changed.
        ///  This test name is used to make sure the test will be executed 
        ///  BEFORE <see cref="MonthCalendar_SettingDate_DoesntCrashApplication(MonthCalendar)" /> test.
        /// </summary>
        [Fact]
        public void Application_UseVisualStyles_IsNotSet()
        {
            Assert.False(Application.UseVisualStyles);
        }

        [Theory]
        [MemberData(nameof(MonthCalendar_SettingDate_DoesntCrashApplication_TestData))]
        public void MonthCalendar_SettingDate_DoesntCrashApplication(MonthCalendar calendar)
        {
            Assert.False(Application.UseVisualStyles);
            Exception exception = null;
            ThreadExceptionEventHandler getException =
                    (object sender, ThreadExceptionEventArgs e) => exception = e.Exception;

            try
            {
                /// EnableVisualStyles method changes UseVisualStyles state. 
                /// It mustn't affect other tests, 
                /// so we have to check it in the <see cref="MonthCalendar_SettingDate_UseVisualStyles_IsNotSet()" /> test.
                Application.EnableVisualStyles();
                Application.ThreadException += getException;

                // We need to check if the calendar works well when selecting some date
                // if the calendar has a min/max date or not.
                // In this case, a min/max date is chosen so that the calendar grid doesn't have some dates.
                // This could potentially lead to exceptions when creating the calendar AccessibleObject,
                // so we have to check these cases.
                // Application running and TestForm using are necessary
                // because otherwise there is no way to get a correct result when getting MCGRIDINFO from the calendar.
                Application.Run(new TestForm(calendar));
            }
            finally
            {
                Application.ExitThread();

                // Return UseVisualStyles as it was before.
                PropertyInfo prop = typeof(Application).GetProperty(nameof(Application.UseVisualStyles));
                prop?.SetValue(null, false, BindingFlags.NonPublic | BindingFlags.Static, null, null, null);

                // Verify the process is finished correctly
                Assert.False(Application.UseVisualStyles);
                Assert.Null(exception);
                Assert.False(Application.MessageLoop);
            }
        }

        class TestForm : Form
        {
            private readonly MonthCalendar _monthCalendar;

            public TestForm(MonthCalendar calendar)
            {
                _monthCalendar = calendar;
                Controls.Add(_monthCalendar);
                Load += Form_Load;
            }

            private void Form_Load(object sender, EventArgs e)
            {
                // Simulate the Windows notification sending about changing a selected date.
                try
                {
                    DateTime selectedDate = new DateTime(2020, 4, 10);

                    SYSTEMTIME date = new SYSTEMTIME
                    {
                        wYear = (short)selectedDate.Year,
                        wMonth = (short)selectedDate.Month,
                        wDay = (short)selectedDate.Day
                    };

                    Assert.NotEqual(IntPtr.Zero, _monthCalendar.Handle);

                    NMSELCHANGE lParam = new NMSELCHANGE
                    {
                        nmhdr = new NMHDR
                        {
                            code = MCN_SELCHANGE,
                        },
                        stSelStart = date,
                        stSelEnd = date,
                    };

                    UnsafeNativeMethods.SendMessage(new HandleRef(_monthCalendar, _monthCalendar.Handle), WindowMessages.WM_REFLECT + WindowMessages.WM_NOTIFY, 0, lParam);
                }
                finally
                {
                    Close();
                }
            }
        }

        /// <summary>
        ///  We have to check that UseVisualStyles state isn't changed.
        ///  This test name is used to make sure the test will be executed 
        ///  AFTER <see cref="MonthCalendar_SettingDate_DoesntCrashApplication(MonthCalendar)" /> test.
        /// </summary>
        [Fact]
        public void MonthCalendar_SettingDate_UseVisualStyles_IsNotSet()
        {
            Assert.False(Application.UseVisualStyles);
        }
    }
}
