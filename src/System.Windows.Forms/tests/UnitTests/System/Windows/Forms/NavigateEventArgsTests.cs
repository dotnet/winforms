// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms.Tests;

// NB: doesn't require thread affinity
public class NavigateEventArgsTests
{
    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void Ctor_Bool(bool isForward)
    {
        NavigateEventArgs e = new(isForward);
        Assert.Equal(isForward, e.Forward);
    }
}
