// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Net;

namespace System.Windows.Forms {

    /// <devdoc>
    ///     <para>
    /// Delegate to the WebBrowser Navigated event.
    ///     </para>
    /// </devdoc>
    public delegate void WebBrowserNavigatedEventHandler(object sender, WebBrowserNavigatedEventArgs e);

    /// <devdoc>
    ///    <para>
    /// Provides data for the <see cref='System.Windows.Forms.WebBrowser.OnNavigated'/> event.
    ///    </para>
    /// </devdoc>
    public class WebBrowserNavigatedEventArgs : EventArgs {
        private Uri url;

        /// <devdoc>
        ///    <para>
        /// Creates an instance of the <see cref='System.Windows.Forms.WebBrowserNavigatedEventArgs'/> class.
        ///    </para>
        /// </devdoc>
        public WebBrowserNavigatedEventArgs(Uri url) {
            this.url = url;
        }
        
        /// <devdoc>
        ///    <para>
        /// Url the browser navigated to.
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

