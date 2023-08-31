// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms.Tests;

// NB: doesn't require thread affinity
public class WebBrowserNavigatedEventArgsTests
{
    [Fact]
    public void Ctor_Uri()
    {
        var url = new Uri("http://google.com");
        var e = new WebBrowserNavigatedEventArgs(url);
        Assert.Equal(url, e.Url);
    }
}
