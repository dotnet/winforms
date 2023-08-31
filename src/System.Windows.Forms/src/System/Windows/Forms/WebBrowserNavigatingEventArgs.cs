// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;

namespace System.Windows.Forms;

/// <summary>
///  Provides data for the <see cref="WebBrowser.OnNavigating"/> event.
/// </summary>
public class WebBrowserNavigatingEventArgs : CancelEventArgs
{
    /// <summary>
    ///  Creates an instance of the <see cref="WebBrowserNavigatingEventArgs"/> class.
    /// </summary>
    public WebBrowserNavigatingEventArgs(Uri? url, string? targetFrameName)
    {
        Url = url;
        TargetFrameName = targetFrameName;
    }

    /// <summary>
    ///  Url the browser is navigating to.
    /// </summary>
    public Uri? Url { get; }

    /// <summary>
    ///  In case an individual frame is about to be navigated, this contains the frame name.
    /// </summary>
    public string? TargetFrameName { get; }
}
