// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms.Tests;

// NB: doesn't require thread affinity
public class FormClosingEventArgsTests
{
    [Theory]
    [InlineData(CloseReason.None, true)]
    [InlineData((CloseReason.None - 1), false)]
    public void Ctor_CloseReason_Bool(CloseReason closeReason, bool cancel)
    {
        FormClosingEventArgs e = new(closeReason, cancel);
        Assert.Equal(closeReason, e.CloseReason);
        Assert.Equal(cancel, e.Cancel);
    }
}
