// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms
{
    /// <summary>
    /// Provides data for the <see cref="TaskDialogPage.HyperlinkClicked"/> event.
    /// </summary>
    public class TaskDialogHyperlinkClickedEventArgs : EventArgs
    {
        internal TaskDialogHyperlinkClickedEventArgs(string hyperlink)
        {
            Hyperlink = hyperlink;
        }

        /// <summary>
        /// The value of the <c>href</c> attribute of the hyperlink that the user clicked.
        /// </summary>
        /// <remarks>
        /// Please note: In order to avoid possible security vulnerabilities when showing content
        /// from unsafe sources in a task dialog, you should always verify the value if this
        /// property before actually opening the link.
        /// </remarks>
        public string Hyperlink
        {
            get;
        }
    }
}
