// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms.Tests;

public class LinkLabelTests
{
    [WinFormsFact]
    public void LinkLabel_Constructor()
    {
        using LinkLabel label = new();

        Assert.NotNull(label);
        Assert.True(label.LinkArea.IsEmpty);
        Assert.Equal(0, label.LinkArea.Start);
        Assert.Equal(0, label.LinkArea.Length);
    }
}
