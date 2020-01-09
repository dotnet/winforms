// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms
{
    /// <summary>
    ///  Specifies whether a panel on a status bar is owner drawn or system drawn.
    /// </summary>
    public enum StatusBarPanelStyle
    {
        /// <summary>
        ///  The panel is drawn by the system.
        /// </summary>
        Text = 1,

        /// <summary>
        ///  The panel is drawn by the owner.
        /// </summary>
        OwnerDraw = 2,
    }
}
