// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms
{
    /// <summary>
    ///  Specifies how the <see cref='RichTextBox.Find(string, RichTextBoxFinds)'/> method works.
    /// </summary>
    [Flags]
    public enum RichTextBoxFinds
    {
        /// <summary>
        ///  Find the text without any special characteristics.
        /// </summary>
        None = 0x00000000,

        /// <summary>
        ///  Match only a whole word.
        /// </summary>
        WholeWord = 0x00000002,

        /// <summary>
        ///  Match the case exactly.
        /// </summary>
        MatchCase = 0x00000004,

        /// <summary>
        ///  If the text is found, do not highlight it.
        /// </summary>
        NoHighlight = 0x00000008,

        /// <summary>
        ///  Search from the end of the current selection to the beginning of the
        ///  document.
        /// </summary>
        Reverse = 0x00000010,
    }
}
