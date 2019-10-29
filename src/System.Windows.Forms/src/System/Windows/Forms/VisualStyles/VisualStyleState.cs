// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using static Interop;

namespace System.Windows.Forms.VisualStyles
{
    /// <summary>
    ///  Determines whether visual styles are enabled.
    /// </summary>
    [Flags]
    public enum VisualStyleState
    {
        /// <summary>
        ///  Visual styles are not enabled.
        /// </summary>
        NoneEnabled = 0,

        /// <summary>
        ///  Visual styles enabled only for client area.
        /// </summary>
        ClientAreaEnabled = (int)UxTheme.STAP.ALLOW_CONTROLS,

        /// <summary>
        ///  Visual styles enabled only for non-client area.
        /// </summary>
        NonClientAreaEnabled = (int)UxTheme.STAP.ALLOW_NONCLIENT,

        /// <summary>
        ///  Visual styles enabled only for client and non-client areas.
        /// </summary>
        ClientAndNonClientAreasEnabled = (int)(UxTheme.STAP.ALLOW_NONCLIENT | UxTheme.STAP.ALLOW_CONTROLS)
    }
}
