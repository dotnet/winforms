// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Drawing;
using Xunit;
using Xunit.Abstractions;
using static System.Windows.Forms.MonthCalendar;
using static Interop;
using static Interop.ComCtl32;
using static Interop.User32;

namespace System.Windows.Forms.UITests
{
    public class MonthCalendarTests : ControlTestBase
    {
        private static readonly DateTime CurrentDate = new(2021, 12, 1);
        private static readonly DateTime MaxDate = new(2021, 12, 3, 18, 0, 0);
        private static readonly DateTime MinDate = new(2021, 11, 30, 17, 0, 0);

        public MonthCalendarTests(ITestOutputHelper testOutputHelper)
            : base(testOutputHelper)
        {
        }

        [WinFormsFact]
        public async Task MonthCalendar_SetDate_DoesntCrashApplication_IfUseMouseAsync()
        {
            await RunTestAsync(async (form, calendar) =>
            {
                await MoveMouseToControlAsync(calendar);
                await InputSimulator.SendAsync(
                    form,
                    inputSimulator => inputSimulator.Mouse.LeftButtonClick());
            });
        }

        [WinFormsFact]
        public async Task MonthCalendar_SetDate_DoesntCrashApplication_IfUseKeyboardAsync()
        {
            await RunTestAsync(async (form, calendar) =>
            {
                await InputSimulator.SendAsync(
                    form,
                    inputSimulator => inputSimulator.Keyboard
                                                    .KeyPress(WindowsInput.Native.VirtualKeyCode.RIGHT)
                                                    .KeyPress(WindowsInput.Native.VirtualKeyCode.RIGHT));
            });
        }

        [WinFormsFact]
        public async Task MonthCalendar_SetDate_DoesntCrashApplication_ProgrammaticallyAsync()
        {
            await RunTestAsync(async (form, calendar) =>
            {
                await InputSimulator.SendAsync(
                    form,
                    inputSimulator => inputSimulator.Keyboard
                                                    .KeyPress(WindowsInput.Native.VirtualKeyCode.RIGHT)
                                                    .KeyPress(WindowsInput.Native.VirtualKeyCode.RIGHT));

                DateTime selectedDate = new DateTime(2020, 4, 10);
                PInvoke.SYSTEMTIME date = new()
                {
                    wYear = (short)selectedDate.Year,
                    wMonth = (short)selectedDate.Month,
                    wDay = (short)selectedDate.Day
                };

                NMSELCHANGE lParam = new()
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

        [WinFormsTheory]
        [InlineData(-1)] // Min date
        [InlineData(1)]
        [InlineData(2)] // Max date
        public async Task MonthCalendar_Click_Date_InvokeEventsAsync(int delta)
        {
            await RunClickTestAsync(async (form, calendar) =>
            {
                calendar.SetDate(CurrentDate);
                DateTime newDate = CurrentDate.Date.AddDays(delta);
                int callDateSelectedCount = 0;
                int callDateChangedCount = 0;
                calendar.DateSelected += (object? sender, DateRangeEventArgs e) => callDateSelectedCount++;
                calendar.DateChanged += (object? sender, DateRangeEventArgs e) => callDateChangedCount++;
                await ClickOnDateAsync(form, calendar, newDate);

                Assert.Equal(newDate.Date, calendar.SelectionStart.Date);
                Assert.NotEqual(0, callDateSelectedCount);
                Assert.NotEqual(0, callDateChangedCount);
            });
        }

        [WinFormsTheory]
        [InlineData(-1)] // Min date
        [InlineData(1)]
        [InlineData(2)] // Max date
        public async Task MonthCalendar_DoubleClick_Date_InvokeEventsAsync(int delta)
        {
            await RunClickTestAsync(async (form, calendar) =>
            {
                calendar.SetDate(CurrentDate);
                DateTime newDate = CurrentDate.Date.AddDays(delta);
                int callDateSelectedCount = 0;
                int callDateChangedCount = 0;
                calendar.DateSelected += (object? sender, DateRangeEventArgs e) => callDateSelectedCount++;
                calendar.DateChanged += (object? sender, DateRangeEventArgs e) => callDateChangedCount++;
                await ClickOnDateTwiceAsync(form, calendar, newDate);

                Assert.Equal(newDate.Date, calendar.SelectionStart.Date);
                Assert.NotEqual(0, callDateSelectedCount);
                Assert.NotEqual(0, callDateChangedCount);
            });
        }

        [WinFormsTheory]
        [InlineData(2018, 12, 8, (int)MCGIP.NEXT, 2019, 1, 1)]
        [InlineData(2018, 12, 8, (int)MCGIP.PREV, 2018, 11, 1)]
        public async Task MonthCalendar_Click_ToMonthAsync(int givenYear, int givenMonth, int givenDay, int action, int expectedYear, int expectedMonth, int expectedDay)
        {
            await RunTestAsync(async (form, calendar) =>
            {
                DateTime givenDate = new(givenYear, givenMonth, givenDay);
                DateTime expectedDate = new(expectedYear, expectedMonth, expectedDay);

                calendar.TodayDate = givenDate;
                calendar.SetDate(givenDate);

                Assert.Equal(new DateTime(givenYear, givenMonth, 1), calendar.GetDisplayRange(visible: true).Start);

                // Find the position of the 'Next' button
                var rect = GetCalendarGridRect(calendar, (MCGIP)action);

                // Move the mouse to the center of the 'Next' or 'Prev' buttons
                var centerOfRect = new Point(rect.Left, rect.Top) + new Size(rect.Width / 2, rect.Height / 2);
                var centerOnScreen = calendar.PointToScreen(centerOfRect);
                await MoveMouseAsync(form, centerOnScreen);

                TaskCompletionSource<VoidResult> dateChanged = new TaskCompletionSource<VoidResult>(TaskCreationOptions.RunContinuationsAsynchronously);
                calendar.DateChanged += (sender, e) => dateChanged.TrySetResult(default);

                await InputSimulator.SendAsync(
                    form,
                    inputSimulator => inputSimulator.Mouse.LeftButtonClick());

                await dateChanged.Task;

                // Verify that the next month is selected
                Assert.Equal(expectedDate, calendar.GetDisplayRange(visible: true).Start);
            });

            static unsafe Rectangle GetCalendarGridRect(MonthCalendar control, MCGIP part)
            {
                MCGRIDINFO result = new()
                {
                    cbSize = (uint)sizeof(MCGRIDINFO),
                    dwPart = part,
                    dwFlags = MCGIF.RECT,
                };

                Assert.NotEqual(default, User32.SendMessageW(control, (User32.WM)ComCtl32.MCM.GETCALENDARGRIDINFO, default, ref result));
                var rect = Rectangle.FromLTRB(result.rc.left, result.rc.top, result.rc.right, result.rc.bottom);
                return rect;
            }
        }

        private Point GetCellPositionByDate(MonthCalendar calendar, DateTime dateTime)
        {
            MonthCalendarAccessibleObject accessibleObject = (MonthCalendarAccessibleObject)calendar.AccessibilityObject;
            return accessibleObject.TestAccessor().Dynamic.GetCellByDate(dateTime.Date).Bounds.Location;
        }

        private async Task ClickOnDateAsync(Form form, MonthCalendar calendar, DateTime date)
        {
            await MoveMouseAsync(form, GetCellPositionByDate(calendar, date));
            await InputSimulator.SendAsync(
                form,
                inputSimulator => inputSimulator.Mouse.LeftButtonClick());
        }

        private async Task ClickOnDateTwiceAsync(Form form, MonthCalendar calendar, DateTime date)
        {
            await MoveMouseAsync(form, GetCellPositionByDate(calendar, date));
            await InputSimulator.SendAsync(
                form,
                inputSimulator => inputSimulator.Mouse
                                                .LeftButtonClick()
                                                .Sleep(500)
                                                .LeftButtonClick());
        }

        private async Task RunClickTestAsync(Func<Form, MonthCalendar, Task> runTest)
        {
            await RunSingleControlTestAsync(
                testDriverAsync: runTest,
                createControl: () =>
                {
                    MonthCalendar control = new()
                    {
                        Location = new Point(0, 0),
                        MinDate = MinDate,
                        MaxDate = MaxDate
                    };

                    return control;
                },
                createForm: () =>
                {
                    return new()
                    {
                        Size = new(500, 300),
                    };
                });
        }

        private async Task RunTestAsync(Func<Form, MonthCalendar, Task> runTest)
        {
            await RunSingleControlTestAsync(
                testDriverAsync: runTest,
                createControl: () =>
                {
                    MonthCalendar control = new()
                    {
                        Location = new Point(0, 0),
                    };

                    return control;
                },
                createForm: () =>
                {
                    return new()
                    {
                        Size = new(500, 300),
                    };
                });
        }
    }
}
