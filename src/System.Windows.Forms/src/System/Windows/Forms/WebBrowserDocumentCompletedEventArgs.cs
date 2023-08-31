// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms;

/// <summary>
///  Provides data for the <see cref="WebBrowser.OnDocumentCompleted"/> event.
/// </summary>
public class WebBrowserDocumentCompletedEventArgs : EventArgs
{
    /// <summary>
    ///  Creates an instance of the <see cref="WebBrowserDocumentCompletedEventArgs"/> class.
    /// </summary>
    public WebBrowserDocumentCompletedEventArgs(Uri? url)
    {
        Url = url;
    }

    /// <summary>
    ///  Url of the Document.
    /// </summary>
    public Uri? Url { get; }
}
