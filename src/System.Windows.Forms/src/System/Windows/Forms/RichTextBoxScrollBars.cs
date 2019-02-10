// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms {

    using System.Diagnostics;

    using System;
    using System.ComponentModel;
    using Microsoft.Win32;


    /// <devdoc>
    ///    <para>
    ///       Specifies how a <see cref='System.Windows.Forms.RichTextBox'/> control displays scroll bars.
    ///
    ///    </para>
    /// </devdoc>
    public enum RichTextBoxScrollBars {

        /// <devdoc>
        ///    <para>
        ///       Never display scroll bars.
        ///    </para>
        /// </devdoc>
        None       = 0,

        /// <devdoc>
        ///    <para>
        ///       Display only a
        ///       horizontal scroll bar when needed.
        ///
        ///    </para>
        /// </devdoc>
        Horizontal = RichTextBoxConstants.RTB_HORIZ,

        /// <devdoc>
        ///    <para>
        ///       Display only a
        ///       vertical scroll bar when needed.
        ///
        ///    </para>
        /// </devdoc>
        Vertical   = RichTextBoxConstants.RTB_VERT,

        /// <devdoc>
        ///    <para>
        ///       Display both a horizontal and a vertical scroll bar when needed.
        ///    </para>
        /// </devdoc>
        Both       = Horizontal | Vertical,

        /// <devdoc>
        ///    <para>
        ///       Always
        ///       display only a horizontal scroll bar.
        ///
        ///    </para>
        /// </devdoc>
        ForcedHorizontal = RichTextBoxConstants.RTB_FORCE | Horizontal,

        /// <devdoc>
        ///    <para>
        ///       Always display only a vertical scroll bar.
        ///    </para>
        /// </devdoc>
        ForcedVertical = RichTextBoxConstants.RTB_FORCE | Vertical,

        /// <devdoc>
        ///    <para>
        ///       Always display both a horizontal and a vertical scroll bar.
        ///    </para>
        /// </devdoc>
        ForcedBoth = ForcedHorizontal | ForcedVertical,

        // Be careful when adding new members -- this enum is part normal, part flags
    }
}
