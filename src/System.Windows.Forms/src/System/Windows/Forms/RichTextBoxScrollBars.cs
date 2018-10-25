// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms {

    using System.Diagnostics;

    using System;
    using System.ComponentModel;
    using Microsoft.Win32;


    /// <include file='doc\RichTextBoxScrollBars.uex' path='docs/doc[@for="RichTextBoxScrollBars"]/*' />
    /// <devdoc>
    ///    <para>
    ///       Specifies how a <see cref='System.Windows.Forms.RichTextBox'/> control displays scroll bars.
    ///
    ///    </para>
    /// </devdoc>
    public enum RichTextBoxScrollBars {

        /// <include file='doc\RichTextBoxScrollBars.uex' path='docs/doc[@for="RichTextBoxScrollBars.None"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Never display scroll bars.
        ///    </para>
        /// </devdoc>
        None       = 0,

        /// <include file='doc\RichTextBoxScrollBars.uex' path='docs/doc[@for="RichTextBoxScrollBars.Horizontal"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Display only a
        ///       horizontal scroll bar when needed.
        ///
        ///    </para>
        /// </devdoc>
        Horizontal = RichTextBoxConstants.RTB_HORIZ,

        /// <include file='doc\RichTextBoxScrollBars.uex' path='docs/doc[@for="RichTextBoxScrollBars.Vertical"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Display only a
        ///       vertical scroll bar when needed.
        ///
        ///    </para>
        /// </devdoc>
        Vertical   = RichTextBoxConstants.RTB_VERT,

        /// <include file='doc\RichTextBoxScrollBars.uex' path='docs/doc[@for="RichTextBoxScrollBars.Both"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Display both a horizontal and a vertical scroll bar when needed.
        ///    </para>
        /// </devdoc>
        Both       = Horizontal | Vertical,

        /// <include file='doc\RichTextBoxScrollBars.uex' path='docs/doc[@for="RichTextBoxScrollBars.ForcedHorizontal"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Always
        ///       display only a horizontal scroll bar.
        ///
        ///    </para>
        /// </devdoc>
        ForcedHorizontal = RichTextBoxConstants.RTB_FORCE | Horizontal,

        /// <include file='doc\RichTextBoxScrollBars.uex' path='docs/doc[@for="RichTextBoxScrollBars.ForcedVertical"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Always display only a vertical scroll bar.
        ///    </para>
        /// </devdoc>
        ForcedVertical = RichTextBoxConstants.RTB_FORCE | Vertical,

        /// <include file='doc\RichTextBoxScrollBars.uex' path='docs/doc[@for="RichTextBoxScrollBars.ForcedBoth"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Always display both a horizontal and a vertical scroll bar.
        ///    </para>
        /// </devdoc>
        ForcedBoth = ForcedHorizontal | ForcedVertical,

        // Be careful when adding new members -- this enum is part normal, part flags
    }
}
