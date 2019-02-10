// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.MSInternal", "CA905:SystemAndMicrosoftNamespacesRequireApproval", Scope="namespace", Target="System.Windows.Forms.VisualStyles")]

namespace System.Windows.Forms.VisualStyles {


    /// <devdoc>
    ///    <para>
    ///     Determines whether visual styles are enabled.
    ///    </para>
    /// </devdoc>

    [Flags]
    public enum VisualStyleState {

        /// <devdoc>
        ///    <para>
        ///  Visual styles are not enabled.
        ///    </para>
        /// </devdoc>
        NoneEnabled = 0,


        /// <devdoc>
        ///    <para>
        /// Visual styles enabled only for client area.
        ///    </para>
        /// </devdoc>
        ClientAreaEnabled = NativeMethods.STAP_ALLOW_CONTROLS,


        /// <devdoc>
        ///    <para>
        /// Visual styles enabled only for non-client area.
        ///    </para>
        /// </devdoc>
        NonClientAreaEnabled = NativeMethods.STAP_ALLOW_NONCLIENT,


        /// <devdoc>
        ///    <para>
        /// Visual styles enabled only for client and non-client areas. 
        ///    </para>
        /// </devdoc>
       ClientAndNonClientAreasEnabled = NativeMethods.STAP_ALLOW_NONCLIENT | NativeMethods.STAP_ALLOW_CONTROLS
    }
}
