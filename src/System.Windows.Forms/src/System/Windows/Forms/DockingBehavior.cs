// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms {

    /// <devdoc>
    ///    <para>
    ///       Specifies whether any characters in the
    ///       current selection have the style or attribute.
    ///
    ///    </para>
    /// </devdoc>
    public enum DockingBehavior {
        /// <devdoc>
        ///    <para>
        ///       Some but not all characters.
        ///    </para>
        /// </devdoc>
        Never     = 0,

        /// <devdoc>
        ///    <para>
        ///       No characters.
        ///    </para>
        /// </devdoc>
        Ask      = 1,

        /// <devdoc>
        ///    <para>
        ///       All characters.
        ///    </para>
        /// </devdoc>
        AutoDock = 2
    }
}
