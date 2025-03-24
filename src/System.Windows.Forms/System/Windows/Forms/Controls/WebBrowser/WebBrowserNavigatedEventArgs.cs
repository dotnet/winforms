// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms;

/// <summary>
///  Provides data for the <see cref="WebBrowser.OnNavigated"/> event.
/// </summary>
public class WebBrowserNavigatedEventArgs : EventArgs
{
    /// <summary>
    ///  Creates an instance of the <see cref="WebBrowserNavigatedEventArgs"/> class.
    /// </summary>
    public WebBrowserNavigatedEventArgs(Uri? url)
    {
        Url = url;
    }

    /// <summary>
    ///  Url the browser navigated to.
    /// </summary>
    public Uri? Url { get; }
}
