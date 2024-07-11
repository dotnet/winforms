// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms.Tests;

public class MonthCalendar_DateBoldEventArgsTests
{
    public static readonly TheoryData<DateTime, int> s_dateBoldEventArgs_Constructor_InitializesPropertiesCorrectly_Data = new()
    {
        { DateTime.UtcNow, 5 },
        { DateTime.UtcNow.AddDays(-1), 10 },
        { DateTime.UtcNow.AddDays(1), 0 }
    };

    [WinFormsTheory]
    [MemberData(nameof(s_dateBoldEventArgs_Constructor_InitializesPropertiesCorrectly_Data))]
    public void DateBoldEventArgs_Constructor_InitializesPropertiesCorrectly(DateTime startDate, int size)
    {
        DateBoldEventArgs eventArgs = new(startDate, size);

        eventArgs.StartDate.Should().Be(startDate);
        eventArgs.Size.Should().Be(size);
    }

    public static readonly TheoryData<int[]> s_daysToBold_GetSetWorksCorrectly_Data = new()
    {
        { new int[] {1, 2, 3, 4, 5} },
        { Array.Empty<int>() },
        { new int[] {-1, -2, -3} },
        { null }
    };

    [WinFormsTheory]
    [MemberData(nameof(s_daysToBold_GetSetWorksCorrectly_Data))]
    public void DateBoldEventArgs_DaysToBold_GetSetWorksCorrectly(int[] value)
    {
        DateBoldEventArgs eventArgs = new(DateTime.UtcNow, 5)
        {
            DaysToBold = value
        };

        eventArgs.DaysToBold.Should().Equal(value);
    }
}
