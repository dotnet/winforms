// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms {

    using System.Diagnostics;

    using System;
    using System.ComponentModel;
    using Microsoft.Win32;


    /// <include file='doc\RichTextBoxSelectionAttribute.uex' path='docs/doc[@for="RichTextBoxSelectionAttribute"]/*' />
    /// <devdoc>
    ///    <para>
    ///       Specifies whether any characters in the
    ///       current selection have the style or attribute.
    ///
    ///    </para>
    /// </devdoc>
    public enum RichTextBoxSelectionAttribute {
        /// <include file='doc\RichTextBoxSelectionAttribute.uex' path='docs/doc[@for="RichTextBoxSelectionAttribute.Mixed"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Some but not all characters.
        ///    </para>
        /// </devdoc>
        Mixed     = -1,

        /// <include file='doc\RichTextBoxSelectionAttribute.uex' path='docs/doc[@for="RichTextBoxSelectionAttribute.None"]/*' />
        /// <devdoc>
        ///    <para>
        ///       No characters.
        ///    </para>
        /// </devdoc>
        None      = 0,

        /// <include file='doc\RichTextBoxSelectionAttribute.uex' path='docs/doc[@for="RichTextBoxSelectionAttribute.All"]/*' />
        /// <devdoc>
        ///    <para>
        ///       All characters.
        ///    </para>
        /// </devdoc>
        All       = 1,

    }
}
