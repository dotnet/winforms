// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms
{
    /// <summary>
    ///  Delegate to the WebBrowser Navigating event.
    /// </summary>
    [Obsolete(
        Obsoletions.WebBrowserMessage,
        error: false,
        DiagnosticId = Obsoletions.WebBrowserDiagnosticId,
        UrlFormat = Obsoletions.SharedUrlFormat)]
    public delegate void WebBrowserNavigatingEventHandler(object? sender, WebBrowserNavigatingEventArgs e);
}
