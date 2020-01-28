// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms
{
    /// <summary>
    ///  Specifies how a <see cref='RichTextBox'/> control displays scroll bars.
    /// </summary>
    public enum RichTextBoxScrollBars
    {
        /// <summary>
        ///  Never display scroll bars.
        /// </summary>
        None = 0,

        /// <summary>
        ///  Display only a horizontal scroll bar when needed.
        /// </summary>
        Horizontal = RichTextBoxConstants.RTB_HORIZ,

        /// <summary>
        ///  Display only a vertical scroll bar when needed.
        /// </summary>
        Vertical = RichTextBoxConstants.RTB_VERT,

        /// <summary>
        ///  Display both a horizontal and a vertical scroll bar when needed.
        /// </summary>
        Both = Horizontal | Vertical,

        /// <summary>
        ///  Always display only a horizontal scroll bar.
        /// </summary>
        ForcedHorizontal = RichTextBoxConstants.RTB_FORCE | Horizontal,

        /// <summary>
        ///  Always display only a vertical scroll bar.
        /// </summary>
        ForcedVertical = RichTextBoxConstants.RTB_FORCE | Vertical,

        /// <summary>
        ///  Always display both a horizontal and a vertical scroll bar.
        /// </summary>
        ForcedBoth = ForcedHorizontal | ForcedVertical,

        // Be careful when adding new members -- this enum is part normal, part flags
    }
}
