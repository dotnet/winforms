// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms
{
    /// <summary>
    ///  Provides data for the <see cref='WebBrowser.OnNavigated'/> event.
    /// </summary>
    public class WebBrowserNavigatedEventArgs : EventArgs
    {
        private readonly Uri _url;

        /// <summary>
        ///  Creates an instance of the <see cref='WebBrowserNavigatedEventArgs'/> class.
        /// </summary>
        public WebBrowserNavigatedEventArgs(Uri url)
        {
            _url = url;
        }

        /// <summary>
        ///  Url the browser navigated to.
        /// </summary>
        public Uri Url
        {
            get
            {
                return _url;
            }
        }
    }
}
