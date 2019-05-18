// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms
{
    /// <summary>
    /// Specifies whether a panel on a status bar is owner drawn or system drawn.
    /// </devdoc>
    public enum StatusBarPanelStyle
    {
        /// <summary>
        /// The panel is drawn by the system.
        /// </devdoc>
        Text = 1,

        /// <summary>
        /// The panel is drawn by the owner.
        /// </devdoc>
        OwnerDraw = 2,
    }
}
