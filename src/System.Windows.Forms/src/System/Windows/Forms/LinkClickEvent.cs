// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms {

    using System.Diagnostics;

    using System;
    using System.ComponentModel;
    using System.Drawing;
    using Microsoft.Win32;


    /// <include file='doc\LinkClickEvent.uex' path='docs/doc[@for="LinkClickedEventArgs"]/*' />
    /// <devdoc>
    ///    <para>
    ///       Provides data for the <see cref='System.Windows.Forms.RichTextBox.LinkClicked'/> event.
    ///    </para>
    /// </devdoc>
    [System.Runtime.InteropServices.ComVisible(true)]
    public class LinkClickedEventArgs : EventArgs {
        private string linkText;

        /// <include file='doc\LinkClickEvent.uex' path='docs/doc[@for="LinkClickedEventArgs.LinkText"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Gets the text of the link being clicked.
        ///    </para>
        /// </devdoc>
        public string LinkText {
            get {
                return linkText;
            }
        }

        /// <include file='doc\LinkClickEvent.uex' path='docs/doc[@for="LinkClickedEventArgs.LinkClickedEventArgs"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Initializes a new instance of the <see cref='System.Windows.Forms.LinkClickedEventArgs'/> class.
        ///    </para>
        /// </devdoc>
        public LinkClickedEventArgs(string linkText) {
            this.linkText = linkText;
        }
    }
}

