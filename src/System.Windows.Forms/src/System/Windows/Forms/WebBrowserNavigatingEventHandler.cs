// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Net;
using System.ComponentModel;

namespace System.Windows.Forms {

    /// <devdoc>
    ///     <para>
    /// Delegate to the WebBrowser Navigating event.
    ///     </para>
    /// </devdoc>
    public delegate void WebBrowserNavigatingEventHandler(object sender, WebBrowserNavigatingEventArgs e);

    /// <devdoc>
    ///    <para>
    /// Provides data for the <see cref='System.Windows.Forms.WebBrowser.OnNavigating'/> event.
    ///    </para>
    /// </devdoc>
    public class WebBrowserNavigatingEventArgs : CancelEventArgs {
        private Uri url;
        private string targetFrameName;
        
        /// <devdoc>
        ///    <para>
        /// Creates an instance of the <see cref='System.Windows.Forms.WebBrowserNavigatingEventArgs'/> class.
        ///    </para>
        /// </devdoc>
        public WebBrowserNavigatingEventArgs(Uri url, string targetFrameName) {
            this.url = url;
            this.targetFrameName = targetFrameName;
        }
        
        /// <devdoc>
        ///    <para>
        /// Url the browser is navigating to.
        ///    </para>
        /// </devdoc>
        public Uri Url {
            get {
                WebBrowser.EnsureUrlConnectPermission(url);
                return this.url;
            }
        }
        
        /// <devdoc>
        ///    <para>
        /// In case an individual frame is about to be navigated, this contains the frame name.
        ///    </para>
        /// </devdoc>
        public string TargetFrameName {
            get {
                WebBrowser.EnsureUrlConnectPermission(url);
                return this.targetFrameName;
            }
        }
    }

}

