// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using static System.Windows.Forms.MonthCalendar;
using Point = System.Drawing.Point;

namespace System.Windows.Forms.Tests;

public class MonthCalendarHitTestInfoTests
{
    [WinFormsTheory]
    [InlineData(HitArea.Date, true, "2022/01/01")]
    [InlineData(HitArea.WeekNumbers, true, "2022/01/01")]
    [InlineData(HitArea.Nowhere, false, null)]
    public void HitTestInfo_Constructor_SetsPropertiesCorrectly(HitArea hitArea, bool hasDateTime, string dateTimeStr)
    {
        Point point = new(5, 5);
        DateTime? time = hasDateTime ? DateTime.Parse(dateTimeStr) : null;

        HitTestInfo hitTestInfo = time.HasValue
            ? new HitTestInfo(point, hitArea, time.Value)
            : new HitTestInfo(point, hitArea);

        hitTestInfo.Point.Should().Be(point);
        hitTestInfo.HitArea.Should().Be(hitArea);
        hitTestInfo.Time.Should().Be(time ?? DateTime.MinValue);
    }

    [WinFormsTheory]
    [InlineData(HitArea.Date, true)]
    [InlineData(HitArea.WeekNumbers, true)]
    [InlineData(HitArea.Nowhere, false)]
    public void HitTestInfo_HitAreaHasValidDateTime_ReturnsExpectedResult(HitArea hitArea, bool expectedResult)
    {
        bool result = HitTestInfo.HitAreaHasValidDateTime(hitArea);

        result.Should().Be(expectedResult);
    }

    [WinFormsTheory]
    [InlineData(null)]
    public void HitTestInfo_Constructor_WithNullDateTime_ThrowsException(DateTime? time)
    {
        Point point = new(5, 5);
        HitArea hitArea = HitArea.Date;

        Action action = () => new HitTestInfo(point, hitArea, time!.Value);

        action.Should().Throw<InvalidOperationException>();
    }
}
