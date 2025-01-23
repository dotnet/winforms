// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace Microsoft.VisualBasic.Devices.Tests;

public class ClockTests
{
    [Fact]
    public void LocalTime()
    {
        Clock clock = new();

        var before = clock.LocalTime;
        Thread.Sleep(10);

        var now = DateTime.Now;
        Thread.Sleep(10);

        var after = clock.LocalTime;

        Assert.True(before <= now);
        Assert.True(now <= after);
    }

    [Fact]
    public void GmtTime()
    {
        Clock clock = new();

        var before = clock.GmtTime;
        Thread.Sleep(10);

        var now = DateTime.UtcNow;
        Thread.Sleep(10);

        var after = clock.GmtTime;

        Assert.True(before <= now);
        Assert.True(now <= after);
    }

    [Fact]
    public void TickCount()
    {
        Clock clock = new();

        int before = clock.TickCount;
        Thread.Sleep(10);

        int after = clock.TickCount;
        Assert.True(before <= after);
    }
}
