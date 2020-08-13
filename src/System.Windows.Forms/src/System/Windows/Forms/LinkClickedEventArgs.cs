// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable disable

using System.Runtime.InteropServices;

namespace System.Windows.Forms
{
    /// <summary>
    ///  Provides data for the <see cref='RichTextBox.LinkClicked'/> event.
    /// </summary>
    public class LinkClickedEventArgs : EventArgs
    {
        /// <summary>
        ///  Initializes a new instance of the <see cref='LinkClickedEventArgs'/> class.
        /// </summary>
        public LinkClickedEventArgs(string linkText)
        {
            LinkText = linkText;
        }

        /// <summary>
        ///  Gets the text of the link being clicked.
        /// </summary>
        public string LinkText { get; }
    }
}
