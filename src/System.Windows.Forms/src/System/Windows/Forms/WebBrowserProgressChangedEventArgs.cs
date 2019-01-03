// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms
{
    /// <devdoc>
    /// Provides data for the <see cref='System.Windows.Forms.WebBrowser.OnProgressChanged'/> event.
    /// </devdoc>
    public class WebBrowserProgressChangedEventArgs : EventArgs
    {
        /// <devdoc>
        /// Creates an instance of the <see cref='System.Windows.Forms.WebBrowserProgressChangedEventArgs'/> class.
        /// </devdoc>
        public WebBrowserProgressChangedEventArgs(long currentProgress, long maximumProgress)
        {
            CurrentProgress = currentProgress;
            MaximumProgress = maximumProgress;
        }
        
        /// <devdoc>
        /// Specifies current number of bytes donwloaded. CurrentProgress/MaximumProgress*100 = progress percentage.
        /// </devdoc>
        public long CurrentProgress { get; }
        
        /// <devdoc>
        /// Specifies total number of bytes of item being downloaded.
        /// CurrentProgress/MaximumProgress*100 = progress percentage.
        /// </devdoc>
        public long MaximumProgress { get; }
    }
}
