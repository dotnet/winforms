// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

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
}
