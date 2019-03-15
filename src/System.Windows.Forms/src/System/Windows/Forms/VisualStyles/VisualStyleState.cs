﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics.CodeAnalysis;

[assembly: SuppressMessage("Microsoft.MSInternal", "CA905:SystemAndMicrosoftNamespacesRequireApproval", Scope="namespace", Target="System.Windows.Forms.VisualStyles")]

namespace System.Windows.Forms.VisualStyles
{
    /// <devdoc>
    /// Determines whether visual styles are enabled.
    /// </devdoc>
    [Flags]
    public enum VisualStyleState
    {
        /// <devdoc>
        ///  Visual styles are not enabled.
        /// </devdoc>
        NoneEnabled = 0,

        /// <devdoc>
        /// Visual styles enabled only for client area.
        /// </devdoc>
        ClientAreaEnabled = NativeMethods.STAP_ALLOW_CONTROLS,

        /// <devdoc>
        /// Visual styles enabled only for non-client area.
        /// </devdoc>
        NonClientAreaEnabled = NativeMethods.STAP_ALLOW_NONCLIENT,

        /// <devdoc>
        /// Visual styles enabled only for client and non-client areas. 
        /// </devdoc>
       ClientAndNonClientAreasEnabled = NativeMethods.STAP_ALLOW_NONCLIENT | NativeMethods.STAP_ALLOW_CONTROLS
    }
}
