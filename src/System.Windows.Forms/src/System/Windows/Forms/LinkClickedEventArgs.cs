// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.InteropServices;

namespace System.Windows.Forms
{
    /// <devdoc>
    /// Provides data for the <see cref='System.Windows.Forms.RichTextBox.LinkClicked'/> event.
    /// </devdoc>
    [ComVisible(true)]
    public class LinkClickedEventArgs : EventArgs
    {
        /// <devdoc>
        /// Initializes a new instance of the <see cref='System.Windows.Forms.LinkClickedEventArgs'/> class.
        /// </devdoc>
        public LinkClickedEventArgs(string linkText)
        {
            LinkText = linkText;
        }

        /// <devdoc>
        /// Gets the text of the link being clicked.
        /// </devdoc>
        public string LinkText { get; }
    }
}
