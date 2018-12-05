// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.ComponentModel;

namespace System.Windows.Forms {

    /// <devdoc>
    ///    <para>EventArgs for onerror event of HtmlElement</para>
    /// </devdoc>
    public sealed class HtmlElementErrorEventArgs : EventArgs {
        private string description;
        private string urlString;
        private Uri url;
        private int lineNumber;
        private bool handled;

        internal HtmlElementErrorEventArgs(string description, string urlString, int lineNumber)
        {
            this.description = description;
            this.urlString = urlString;
            this.lineNumber = lineNumber;
        }

        /// <devdoc>
        ///    <para>Description of error</para>
        /// </devdoc>
        public string Description
        {
            get
            {
                return description;
            }
        }

        /// <devdoc>
        ///    <para>
        ///       Gets or sets a value indicating whether the <see cref='System.Windows.Forms.HtmlWindow.Error'/>
        ///       event was handled.
        ///    </para>
        /// </devdoc>
        public bool Handled {
            get {
                return handled;
            }
            set {
                handled = value;
            }
        }

        /// <devdoc>
        ///    <para>Line number where error occurred</para>
        /// </devdoc>
        public int LineNumber
        {
            get
            {
                return lineNumber;
            }
        }

        /// <devdoc>
        ///    <para>Url where error occurred</para>
        /// </devdoc>
        public Uri Url
        {
            get
            {
                if (url == null)
                {
                    url = new Uri(urlString);
                }
                return url;
            }
        }
    }
}

