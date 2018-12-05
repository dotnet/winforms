// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Net;

namespace System.Windows.Forms {

    /// <devdoc>
    ///     <para>
    /// Delegate to the WebBrowser DocumentCompleted event.
    ///     </para>
    /// </devdoc>
    public delegate void WebBrowserDocumentCompletedEventHandler(object sender, WebBrowserDocumentCompletedEventArgs e);

    /// <devdoc>
    ///    <para>
    /// Provides data for the <see cref='System.Windows.Forms.WebBrowser.OnDocumentCompleted'/> event.
    ///    </para>
    /// </devdoc>
    public class WebBrowserDocumentCompletedEventArgs : EventArgs {
        private Uri url;

        /// <devdoc>
        ///    <para>
        /// Creates an instance of the <see cref='System.Windows.Forms.WebBrowserDocumentCompletedEventArgs'/> class.
        ///    </para>
        /// </devdoc>
        public WebBrowserDocumentCompletedEventArgs(Uri url) {
            this.url = url;
        }
        
        /// <devdoc>
        ///    <para>
        /// Url of the Document.
        ///    </para>
        /// </devdoc>
        public Uri Url {
            get {
                WebBrowser.EnsureUrlConnectPermission(url);
                return this.url;
            }
        }
    }
}

