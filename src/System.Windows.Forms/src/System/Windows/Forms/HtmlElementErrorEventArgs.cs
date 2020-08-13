// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable disable

namespace System.Windows.Forms
{
    /// <summary>
    ///  EventArgs for onerror event of HtmlElement
    /// </summary>
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

        /// <summary>
        ///  Description of error
        /// </summary>
        public string Description { get; }

        /// <summary>
        ///  Gets or sets a value indicating whether the <see cref='HtmlWindow.Error'/>
        ///  event was handled.
        /// </summary>
        public bool Handled { get; set; }

        /// <summary>
        ///  Line number where error occurred
        /// </summary>
        public int LineNumber { get; }

        /// <summary>
        ///  Url where error occurred
        /// </summary>
        public Uri Url => _url ?? (_url = new Uri(_urlString));
    }
}
