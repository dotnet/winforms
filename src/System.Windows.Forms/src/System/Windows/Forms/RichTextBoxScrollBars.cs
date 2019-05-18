// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms
{
    /// <summary>
    /// Specifies how a <see cref='System.Windows.Forms.RichTextBox'/> control displays scroll bars.
    ///
    /// </devdoc>
    public enum RichTextBoxScrollBars
    {
        /// <summary>
        /// Never display scroll bars.
        /// </devdoc>
        None = 0,

        /// <summary>
        /// Display only a horizontal scroll bar when needed.
        /// </devdoc>
        Horizontal = RichTextBoxConstants.RTB_HORIZ,

        /// <summary>
        /// Display only a vertical scroll bar when needed.
        /// </devdoc>
        Vertical = RichTextBoxConstants.RTB_VERT,

        /// <summary>
        /// Display both a horizontal and a vertical scroll bar when needed.
        /// </devdoc>
        Both = Horizontal | Vertical,

        /// <summary>
        /// Always display only a horizontal scroll bar.
        /// </devdoc>
        ForcedHorizontal = RichTextBoxConstants.RTB_FORCE | Horizontal,

        /// <summary>
        /// Always display only a vertical scroll bar.
        /// </devdoc>
        ForcedVertical = RichTextBoxConstants.RTB_FORCE | Vertical,

        /// <summary>
        /// Always display both a horizontal and a vertical scroll bar.
        /// </devdoc>
        ForcedBoth = ForcedHorizontal | ForcedVertical,

        // Be careful when adding new members -- this enum is part normal, part flags
    }
}
