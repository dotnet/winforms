// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel;

namespace System.Windows.Forms
{
    /// <summary>
    ///  Provides data for the <see cref='WebBrowser.OnNavigating'/> event.
    /// </summary>
    public class WebBrowserNavigatingEventArgs : CancelEventArgs
    {
        private readonly Uri _url;
        private readonly string _targetFrameName;

        /// <summary>
        ///  Creates an instance of the <see cref='WebBrowserNavigatingEventArgs'/> class.
        /// </summary>
        public WebBrowserNavigatingEventArgs(Uri url, string targetFrameName)
        {
            _url = url;
            _targetFrameName = targetFrameName;
        }

        /// <summary>
        ///  Url the browser is navigating to.
        /// </summary>
        public Uri Url
        {
            get
            {
                return _url;
            }
        }

        /// <summary>
        ///  In case an individual frame is about to be navigated, this contains the frame name.
        /// </summary>
        public string TargetFrameName
        {
            get
            {
                return _targetFrameName;
            }
        }
    }
}
