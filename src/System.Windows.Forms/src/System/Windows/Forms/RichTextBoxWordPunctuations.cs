// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms
{
    /// <summary>
    ///  This class defines the possible kinds of punctuation tables that
    ///  can be used with the RichTextBox word wrapping and word breaking features.
    /// </summary>
    public enum RichTextBoxWordPunctuations
    {
        /// <summary>
        ///  Use pre-defined Level 1 punctuation table as default.
        /// </summary>
        Level1 = 0x080,

        /// <summary>
        ///  Use pre-defined Level 2 punctuation table as default.
        /// </summary>
        Level2 = 0x100,

        /// <summary>
        ///  Use a custom defined punctuation table.
        /// </summary>
        Custom = 0x200,

        /// <summary>
        ///  Used as a mask.
        /// </summary>
        All = Level1 | Level2 | Custom,
    }
}
