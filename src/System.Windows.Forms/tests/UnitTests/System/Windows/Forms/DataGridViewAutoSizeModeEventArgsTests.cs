// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms.Tests;

// NB: doesn't require thread affinity
public class DataGridViewAutoSizeModeEventArgsTests
{
    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void Ctor_Bool(bool previousModeAutoSized)
    {
        DataGridViewAutoSizeModeEventArgs e = new(previousModeAutoSized);
        Assert.Equal(previousModeAutoSized, e.PreviousModeAutoSized);
    }
}
