// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel;

namespace System.Windows.Forms
{
    /// <devdoc>
    /// Provides data for the <see cref='System.Windows.Forms.WebBrowser.OnNavigating'/> event.
    /// </devdoc>
    public class WebBrowserNavigatingEventArgs : CancelEventArgs
    {
        private readonly Uri _url;
        private readonly string _targetFrameName;
        
        /// <devdoc>
        /// Creates an instance of the <see cref='System.Windows.Forms.WebBrowserNavigatingEventArgs'/> class.
        /// </devdoc>
        public WebBrowserNavigatingEventArgs(Uri url, string targetFrameName)
        {
            _url = url;
            _targetFrameName = targetFrameName;
        }
        
        /// <devdoc>
        /// Url the browser is navigating to.
        /// </devdoc>
        public Uri Url
        {
            get
            {
                WebBrowser.EnsureUrlConnectPermission(_url);
                return _url;
            }
        }
        
        /// <devdoc>
        /// In case an individual frame is about to be navigated, this contains the frame name.
        /// </devdoc>
        public string TargetFrameName
        {
            get
            {
                WebBrowser.EnsureUrlConnectPermission(_url);
                return _targetFrameName;
            }
        }
    }
}
