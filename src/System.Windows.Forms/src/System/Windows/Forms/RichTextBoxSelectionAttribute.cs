// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms {

    using System.Diagnostics;

    using System;
    using System.ComponentModel;
    using Microsoft.Win32;



    /// <devdoc>
    ///    <para>
    ///       Specifies whether any characters in the
    ///       current selection have the style or attribute.
    ///
    ///    </para>
    /// </devdoc>
    public enum RichTextBoxSelectionAttribute {

        /// <devdoc>
        ///    <para>
        ///       Some but not all characters.
        ///    </para>
        /// </devdoc>
        Mixed     = -1,


        /// <devdoc>
        ///    <para>
        ///       No characters.
        ///    </para>
        /// </devdoc>
        None      = 0,


        /// <devdoc>
        ///    <para>
        ///       All characters.
        ///    </para>
        /// </devdoc>
        All       = 1,

    }
}
