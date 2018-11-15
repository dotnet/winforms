// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms {

    /// <include file='doc\WebBrowserProgressChangedEventHandler.uex' path='docs/doc[@for="WebBrowserProgressChangedEventHandler"]/*' />
    /// <devdoc>
    ///     <para>
    /// Delegate to the WebBrowser ProgressChanged event.
    ///     </para>
    /// </devdoc>
    public delegate void WebBrowserProgressChangedEventHandler(object sender, WebBrowserProgressChangedEventArgs e);

    /// <include file='doc\WebBrowserProgressChangedEventHandler.uex' path='docs/doc[@for="WebBrowserProgressChangedEventArgs"]/*' />
    /// <devdoc>
    ///    <para>
    /// Provides data for the <see cref='System.Windows.Forms.WebBrowser.OnProgressChanged'/> event.
    ///    </para>
    /// </devdoc>
    public class WebBrowserProgressChangedEventArgs : EventArgs {
        private long currentProgress;
        private long maximumProgress;
        
        /// <include file='doc\WebBrowserProgressChangedEventHandler.uex' path='docs/doc[@for="WebBrowserProgressChangedEventArgs.WebBrowserProgressChangedEventArgs"]/*' />
        /// <devdoc>
        ///    <para>
        /// Creates an instance of the <see cref='System.Windows.Forms.WebBrowserProgressChangedEventArgs'/> class.
        ///    </para>
        /// </devdoc>
        public WebBrowserProgressChangedEventArgs(long currentProgress, long maximumProgress) {
            this.currentProgress = currentProgress;
            this.maximumProgress = maximumProgress;
        }
        
        /// <include file='doc\WebBrowserProgressChangedEventHandler.uex' path='docs/doc[@for="WebBrowserProgressChangedEventArgs.CurrentProgress"]/*' />
        /// <devdoc>
        ///    <para>
        /// Specifies current number of bytes donwloaded. CurrentProgress/MaximumProgress*100 = progress percentage.
        ///    </para>
        /// </devdoc>
        public long CurrentProgress {
            get {
                return currentProgress;
            }
        }
        
        /// <include file='doc\WebBrowserProgressChangedEventHandler.uex' path='docs/doc[@for="WebBrowserProgressChangedEventArgs.MaximumProgress"]/*' />
        /// <devdoc>
        ///    <para>
        /// Specifies total number of bytes of item being downloaded.
        /// CurrentProgress/MaximumProgress*100 = progress percentage.
        ///    </para>
        /// </devdoc>
        public long MaximumProgress {
            get {
                return maximumProgress;
            }
        }
    }
}

