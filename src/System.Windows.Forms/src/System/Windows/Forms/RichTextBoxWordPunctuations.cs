﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics.CodeAnalysis;

namespace System.Windows.Forms
{
    /// <devdoc>
    /// This class defines the possible kinds of punctuation tables that
    /// can be used with the RichTextBox word wrapping and word breaking features.
    /// </devdoc>
    [SuppressMessage("Microsoft.Design", "CA1027:MarkEnumsWithFlags")]
    public enum RichTextBoxWordPunctuations
    {
        /// <devdoc>
        /// Use pre-defined Level 1 punctuation table as default.
        /// </devdoc>
        Level1 = 0x080,

        /// <devdoc>
        /// Use pre-defined Level 2 punctuation table as default.
        /// </devdoc>
        Level2 = 0x100,

        /// <devdoc>
        /// Use a custom defined punctuation table.
        /// </devdoc>
        Custom = 0x200,

        /// <devdoc>
        /// Used as a mask.
        /// </devdoc>
        All = Level1 | Level2 | Custom,
    }
}
