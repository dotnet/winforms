// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms
{
    /// <summary>
    ///  Provides data for the <see cref='WebBrowser.OnProgressChanged'/> event.
    /// </summary>
    public class WebBrowserProgressChangedEventArgs : EventArgs
    {
        /// <summary>
        ///  Creates an instance of the <see cref='WebBrowserProgressChangedEventArgs'/> class.
        /// </summary>
        public WebBrowserProgressChangedEventArgs(long currentProgress, long maximumProgress)
        {
            CurrentProgress = currentProgress;
            MaximumProgress = maximumProgress;
        }

        /// <summary>
        ///  Specifies current number of bytes donwloaded. CurrentProgress/MaximumProgress*100 = progress percentage.
        /// </summary>
        public long CurrentProgress { get; }

        /// <summary>
        ///  Specifies total number of bytes of item being downloaded.
        ///  CurrentProgress/MaximumProgress*100 = progress percentage.
        /// </summary>
        public long MaximumProgress { get; }
    }
}
