// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms.Tests;

// NB: doesn't require thread affinity
public class FormClosedEventArgsTests
{
    [Theory]
    [InlineData(CloseReason.None)]
    [InlineData((CloseReason)(CloseReason.None - 1))]
    public void Ctor_CloseReason(CloseReason closeReason)
    {
        var e = new FormClosedEventArgs(closeReason);
        Assert.Equal(closeReason, e.CloseReason);
    }
}
