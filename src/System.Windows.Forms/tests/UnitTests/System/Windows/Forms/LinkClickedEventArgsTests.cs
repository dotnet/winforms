// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms.Tests;

// NB: doesn't require thread affinity
public class LinkClickedEventArgsTests
{
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("linkText")]
    public void Ctor_String(string linkText)
    {
        LinkClickedEventArgs e = new(linkText);
        Assert.Equal(linkText, e.LinkText);
    }
}
