// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms {

    using System.Diagnostics;

    using System;
    using System.ComponentModel;
    using Microsoft.Win32;

    /// <include file='doc\RichTextBoxFinds.uex' path='docs/doc[@for="RichTextBoxFinds"]/*' />
    /// <devdoc>
    ///    <para>
    ///       Specifies how the <see cref='System.Windows.Forms.RichTextBox.Find'/> method works.
    ///
    ///    </para>
    /// </devdoc>
    [Flags]
    public enum RichTextBoxFinds {

        /// <include file='doc\RichTextBoxFinds.uex' path='docs/doc[@for="RichTextBoxFinds.None"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Find the text without any special characteristics.
        ///
        ///    </para>
        /// </devdoc>
        None               = 0x00000000,

        /// <include file='doc\RichTextBoxFinds.uex' path='docs/doc[@for="RichTextBoxFinds.WholeWord"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Match only a whole word.
        ///    </para>
        /// </devdoc>
        WholeWord           = 0x00000002,

        /// <include file='doc\RichTextBoxFinds.uex' path='docs/doc[@for="RichTextBoxFinds.MatchCase"]/*' />
        /// <devdoc>
        ///     Match the case exactly.
        /// </devdoc>
        MatchCase           = 0x00000004,

        /// <include file='doc\RichTextBoxFinds.uex' path='docs/doc[@for="RichTextBoxFinds.NoHighlight"]/*' />
        /// <devdoc>
        ///    <para>
        ///       If the text is found, do not highlight it.
        ///    </para>
        /// </devdoc>
        NoHighlight         = 0x00000008,
        
        /// <include file='doc\RichTextBoxFinds.uex' path='docs/doc[@for="RichTextBoxFinds.Reverse"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Search from the end of the current selection to the beginning of the document.
        ///    </para>
        /// </devdoc>
        Reverse             = 0x00000010,
    }
}
