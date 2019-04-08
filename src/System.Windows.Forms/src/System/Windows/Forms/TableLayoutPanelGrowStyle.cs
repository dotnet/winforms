// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms
{
    /// <devdoc>
    /// Specifies if a TableLayoutPanel will gain additional rows or columns once
    /// its existing cells become full.  If the value is 'None' then the
    /// TableLayoutPanel will throw an exception when the TableLayoutPanel is
    /// over-filled.
    /// </devdoc>
    public enum TableLayoutPanelGrowStyle
    {
        /// <devdoc>
        /// The TableLayoutPanel will not allow additional rows or columns once
        /// it is full.
        /// </devdoc>
        FixedSize = 0,

        /// <devdoc>
        /// The TableLayoutPanel will gain additional rows once it becomes full.
        /// </devdoc>
        AddRows = 1,

        /// <devdoc>
        /// The TableLayoutPanel will gain additional columns once it becomes full.
        /// </devdoc>
        AddColumns = 2
    }
}
