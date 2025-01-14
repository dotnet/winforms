// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#nullable enable

namespace System.Windows.Forms.Tests;

// NB: doesn't require thread affinity
public class WebBrowserProgressChangedEventArgsTests
{
    [Theory]
    [InlineData(-1, -1)]
    [InlineData(0, 0)]
    [InlineData(1, 2)]
    [InlineData(2, 1)]
    public void Ctor_Long_Long(long currentProgress, long maximumProgress)
    {
        WebBrowserProgressChangedEventArgs e = new(currentProgress, maximumProgress);
        Assert.Equal(currentProgress, e.CurrentProgress);
        Assert.Equal(maximumProgress, e.MaximumProgress);
    }

    [WinFormsFact]
    public void Constructor_ShouldSetPropertiesCorrectly()
    {
        long expectedCurrentProgress = 50;
        long expectedMaximumProgress = 100;

        WebBrowserProgressChangedEventArgs eventArgs = new(expectedCurrentProgress, expectedMaximumProgress);

        eventArgs.CurrentProgress.Should().Be(expectedCurrentProgress, "CurrentProgress property was not set correctly.");
        eventArgs.MaximumProgress.Should().Be(expectedMaximumProgress, "MaximumProgress property was not set correctly.");
    }

    [WinFormsFact]
    public void CurrentProgress_ShouldReturnCorrectValue()
    {
        long expectedCurrentProgress = 75;

        WebBrowserProgressChangedEventArgs eventArgs = new(expectedCurrentProgress, 100);

        eventArgs.CurrentProgress.Should().Be(expectedCurrentProgress);
    }

    [WinFormsFact]
    public void MaximumProgress_ShouldReturnCorrectValue()
    {
        long expectedMaximumProgress = 200;

        WebBrowserProgressChangedEventArgs eventArgs = new(50, expectedMaximumProgress);

        eventArgs.MaximumProgress.Should().Be(expectedMaximumProgress);
    }
}
