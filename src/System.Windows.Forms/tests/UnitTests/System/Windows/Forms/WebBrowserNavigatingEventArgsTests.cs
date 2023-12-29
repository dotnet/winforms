// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms.Tests;

// NB: doesn't require thread affinity
public class WebBrowserNavigatingEventArgsTests
{
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("targetFrameName")]
    public void Ctor_Uri(string targetFrameName)
    {
        Uri url = new("http://google.com");
        WebBrowserNavigatingEventArgs e = new(url, targetFrameName);
        Assert.Equal(url, e.Url);
        Assert.Equal(targetFrameName, e.TargetFrameName);
    }
}
