// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms {

    using System.Diagnostics;

    using System;
    using System.ComponentModel;
    using Microsoft.Win32;



    /// <devdoc>
    ///     Defines the possible kinds selection types in a RichTextBox control.
    ///     The actual vale returned by RichTextBox.getSelType() is a combination
    ///     of any of the below options.
    ///
    /// </devdoc>
    [Flags]
    public enum RichTextBoxSelectionTypes {
        /// <devdoc>
        ///     The current selection is empty.
        /// </devdoc>
        Empty            = 0,

        /// <devdoc>
        ///     The current selection is text only.
        /// </devdoc>
        Text             = 1,

        /// <devdoc>
        ///     The current selection contains atleast one OLE object.
        /// </devdoc>
        Object           = 2,

        /// <devdoc>
        ///     The current selection contains more than one character.
        /// </devdoc>
        MultiChar        = 4,

        /// <devdoc>
        ///     The current selection contains more than one OLE object.
        /// </devdoc>
        MultiObject      = 8,

    }
}
