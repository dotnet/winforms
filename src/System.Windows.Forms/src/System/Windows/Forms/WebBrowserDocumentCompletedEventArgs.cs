// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms
{
    /// <devdoc>
    /// Provides data for the <see cref='System.Windows.Forms.WebBrowser.OnDocumentCompleted'/> event.
    /// </devdoc>
    public class WebBrowserDocumentCompletedEventArgs : EventArgs
    {
        private readonly Uri _url;

        /// <devdoc>
        /// Creates an instance of the <see cref='System.Windows.Forms.WebBrowserDocumentCompletedEventArgs'/> class.
        /// </devdoc>
        public WebBrowserDocumentCompletedEventArgs(Uri url)
        {
            _url = url;
        }
        
        /// <devdoc>
        /// Url of the Document.
        /// </devdoc>
        public Uri Url
        {
            get
            {
                WebBrowser.EnsureUrlConnectPermission(_url);
                return _url;
            }
        }
    }
}
