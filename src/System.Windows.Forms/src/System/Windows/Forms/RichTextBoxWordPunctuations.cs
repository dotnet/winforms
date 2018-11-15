// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms {

    using System.Diagnostics;

    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.ComponentModel;
    using Microsoft.Win32;


    /// <include file='doc\RichTextBoxWordPunctuations.uex' path='docs/doc[@for="RichTextBoxWordPunctuations"]/*' />
    /// <devdoc>
    ///     This class defines the possible kinds of punctuation tables that
    ///     can be used with the RichTextBox word wrapping and word breaking features.
    /// </devdoc>
    


    [SuppressMessage("Microsoft.Design", "CA1027:MarkEnumsWithFlags")]              
    public enum RichTextBoxWordPunctuations {
        /// <include file='doc\RichTextBoxWordPunctuations.uex' path='docs/doc[@for="RichTextBoxWordPunctuations.Level1"]/*' />
        /// <devdoc>
        ///     Use pre-defined Level 1 punctuation table as default.
        /// </devdoc>
        Level1     = 0x080,

        /// <include file='doc\RichTextBoxWordPunctuations.uex' path='docs/doc[@for="RichTextBoxWordPunctuations.Level2"]/*' />
        /// <devdoc>
        ///     Use pre-defined Level 2 punctuation table as default.
        /// </devdoc>
        Level2     = 0x100,

        /// <include file='doc\RichTextBoxWordPunctuations.uex' path='docs/doc[@for="RichTextBoxWordPunctuations.Custom"]/*' />
        /// <devdoc>
        ///     Use a custom defined punctuation table.
        /// </devdoc>
        Custom     = 0x200,

        /// <include file='doc\RichTextBoxWordPunctuations.uex' path='docs/doc[@for="RichTextBoxWordPunctuations.All"]/*' />
        /// <devdoc>
        ///     Used as a mask.
        /// </devdoc>
        All = Level1 | Level2 | Custom,

    }
}
