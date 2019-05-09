// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms
{
    /// <devdoc>
    /// Specifies how the <see cref='System.Windows.Forms.RichTextBox.Find'/> method works.
    /// </devdoc>
    [Flags]
    public enum RichTextBoxFinds
    {
        /// <devdoc>
        /// Find the text without any special characteristics.
        /// </devdoc>
        None = 0x00000000,

        /// <devdoc>
        /// Match only a whole word.
        /// </devdoc>
        WholeWord = 0x00000002,

        /// <devdoc>
        /// Match the case exactly.
        /// </devdoc>
        MatchCase = 0x00000004,

        /// <devdoc>
        /// If the text is found, do not highlight it.
        /// </devdoc>
        NoHighlight = 0x00000008,

        /// <devdoc>
        /// Search from the end of the current selection to the beginning of the
        /// document.
        /// </devdoc>
        Reverse = 0x00000010,
    }
}
