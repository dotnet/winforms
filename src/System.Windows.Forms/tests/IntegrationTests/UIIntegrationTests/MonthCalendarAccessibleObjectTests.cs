// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;
using Xunit.Abstractions;
using static System.Windows.Forms.MonthCalendar;

namespace System.Windows.Forms.UITests;

public class MonthCalendarAccessibleObjectTests : ControlTestBase
{
    public MonthCalendarAccessibleObjectTests(ITestOutputHelper testOutputHelper)
        : base(testOutputHelper)
    {
    }

    [WinFormsFact]
    public async Task MonthCalendar_GetFromPoint_ReturnsCorrectValueAsync()
    {
        await RunTestAsync((form, calendar) =>
        {
            MonthCalendarAccessibleObject accessibleObject = (MonthCalendarAccessibleObject)calendar.AccessibilityObject;
            CalendarAccessibleObject calendarAccessibleObject = new(accessibleObject, 0, "Test name");

            MCHITTESTINFO info = new()
            {
                uHit = MCHITTESTINFO_HIT_FLAGS.MCHT_CALENDARDAY,
                iRow = 0
            };
            MonthCalendarChildAccessibleObject cell = calendarAccessibleObject.GetChildFromPoint(info);

            Assert.NotNull(cell);

            return Task.CompletedTask;
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
                    Location = new Point(0, 0)
                };

                return control;
            });
    }
}
