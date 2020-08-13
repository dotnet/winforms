// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms
{
    /// <summary>
    ///  Controls the automatic sizing of certain objects.  This is typically
    ///  used for the sizing of Tabs in a TabStrip control.
    /// </summary>
    public enum TabSizeMode
    {
        /// <summary>
        ///  Indicates that items are only as wide as they need to be to display
        ///  their information.  Empty space on the right is left as such
        /// </summary>
        Normal = 0,

        /// <summary>
        ///  indicates that the tags are stretched to ensure they reach the far
        ///  right of the strip, if necesary.  This is only applicable to tab
        ///  strips with more than one row.
        /// </summary>
        FillToRight = 1,

        /// <summary>
        ///  Indicates that all tabs are the same width. period.
        /// </summary>
        Fixed = 2,
    }
}
