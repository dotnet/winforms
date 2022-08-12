// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms
{
    /// <summary>
    ///  Provides data for the <see cref="WebBrowser.OnDocumentCompleted"/> event.
    /// </summary>
    [Obsolete(
        Obsoletions.WebBrowserMessage,
        error: false,
        DiagnosticId = Obsoletions.WebBrowserDiagnosticId,
        UrlFormat = Obsoletions.SharedUrlFormat)]
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
}
