// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;
using Windows.Win32.UI.Input.KeyboardAndMouse;
using Xunit.Abstractions;
using static System.Windows.Forms.MonthCalendar;

namespace System.Windows.Forms.UITests;

public class MonthCalendarTests : ControlTestBase
{
    private static readonly DateTime s_currentDate = new(2021, 12, 1);
    private static readonly DateTime s_maxDate = new(2021, 12, 3, 18, 0, 0);
    private static readonly DateTime s_minDate = new(2021, 11, 30, 17, 0, 0);

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
                                                .KeyPress(VIRTUAL_KEY.VK_RIGHT)
                                                .KeyPress(VIRTUAL_KEY.VK_RIGHT));
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
                                                .KeyPress(VIRTUAL_KEY.VK_RIGHT)
                                                .KeyPress(VIRTUAL_KEY.VK_RIGHT));

            DateTime selectedDate = new(2020, 4, 10);
            SYSTEMTIME date = new()
            {
                wYear = (ushort)selectedDate.Year,
                wMonth = (ushort)selectedDate.Month,
                wDay = (ushort)selectedDate.Day
            };

            NMSELCHANGE lParam = new()
            {
                nmhdr = new NMHDR
                {
                    code = PInvoke.MCN_SELCHANGE,
                },
                stSelStart = date,
                stSelEnd = date,
            };

            PInvokeCore.SendMessage(calendar, MessageId.WM_REFLECT_NOTIFY, 0, ref lParam);
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
            calendar.SetDate(s_currentDate);
            DateTime newDate = s_currentDate.Date.AddDays(delta);
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
            calendar.SetDate(s_currentDate);
            DateTime newDate = s_currentDate.Date.AddDays(delta);
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
    [InlineData(2018, 12, 8, (int)MCGRIDINFO_PART.MCGIP_NEXT, 2019, 1, 1)]
    [InlineData(2018, 12, 8, (int)MCGRIDINFO_PART.MCGIP_PREV, 2018, 11, 1)]
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
            var rect = GetCalendarGridRect(calendar, (MCGRIDINFO_PART)action);

            // Move the mouse to the center of the 'Next' or 'Prev' buttons
            var centerOfRect = GetCenter(rect);
            var centerOnScreen = calendar.PointToScreen(centerOfRect);
            await MoveMouseAsync(form, centerOnScreen);

            TaskCompletionSource<VoidResult> dateChanged = new(TaskCreationOptions.RunContinuationsAsynchronously);
            calendar.DateChanged += (sender, e) => dateChanged.TrySetResult(default);

            await InputSimulator.SendAsync(
                form,
                inputSimulator => inputSimulator.Mouse.LeftButtonClick());

            await dateChanged.Task;

            // Verify that the next month is selected
            Assert.Equal(expectedDate, calendar.GetDisplayRange(visible: true).Start);
        });

        static unsafe Rectangle GetCalendarGridRect(MonthCalendar control, MCGRIDINFO_PART part)
        {
            MCGRIDINFO result = new()
            {
                cbSize = (uint)sizeof(MCGRIDINFO),
                dwPart = part,
                dwFlags = MCGRIDINFO_FLAGS.MCGIF_RECT,
            };

            Assert.NotEqual(default, PInvokeCore.SendMessage(control, PInvoke.MCM_GETCALENDARGRIDINFO, default, ref result));
            return result.rc;
        }
    }

    private Point GetCellPositionByDate(MonthCalendar calendar, DateTime dateTime)
    {
        MonthCalendarAccessibleObject accessibleObject = (MonthCalendarAccessibleObject)calendar.AccessibilityObject;
        CalendarCellAccessibleObject cell = accessibleObject.TestAccessor().Dynamic.GetCellByDate(dateTime.Date);
        return cell.Bounds.Location + (cell.Bounds.Size / 2);
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
                                            .Sleep(TimeSpan.FromMilliseconds(500))
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
                    MinDate = s_minDate,
                    MaxDate = s_maxDate
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
