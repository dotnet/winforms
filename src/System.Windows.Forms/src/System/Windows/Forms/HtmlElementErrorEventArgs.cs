// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms
{

    /// <devdoc>
    /// EventArgs for onerror event of HtmlElement
    /// </devdoc>
    public sealed class HtmlElementErrorEventArgs : EventArgs
    {
        private readonly string _urlString;
        private Uri _url;

        internal HtmlElementErrorEventArgs(string description, string urlString, int lineNumber)
        {
            Description = description;
            _urlString = urlString;
            LineNumber = lineNumber;
        }

        /// <devdoc>
        /// Description of error
        /// </devdoc>
        public string Description { get; }

        /// <devdoc>
        /// Gets or sets a value indicating whether the <see cref='System.Windows.Forms.HtmlWindow.Error'/>
        /// event was handled.
        /// </devdoc>
        public bool Handled { get; set; }

        /// <devdoc>
        /// Line number where error occurred
        /// </devdoc>
        public int LineNumber { get; }

        /// <devdoc>
        /// Url where error occurred
        /// </devdoc>
        public Uri Url => _url ?? (_url = new Uri(_urlString));
    }
}

