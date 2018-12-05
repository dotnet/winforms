// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


namespace System.Windows.Forms {

    using System.Diagnostics;

    /// <devdoc>
    ///     For a given container control, determines whether the data in child controls
    ///     will automatically be validated when the user attempts to change the focus.
    /// </devdoc>
    public enum AutoValidate {

        /// <devdoc>
        ///     Controls in this container will not be validated when the focus changes.
        /// </devdoc>
        Disable = 0,

        /// <devdoc>
        ///     Controls in this container will be validated when the focus changes.
        ///     If a validation error occurs, the focus is forced to stay in the current control.
        /// </devdoc>
        EnablePreventFocusChange = 1,

        /// <devdoc>
        ///     Controls in this container will be validated when the focus changes.
        ///     If a validation error occurs, the focus is allowed to move to the other control.
        /// </devdoc>
        EnableAllowFocusChange = 2,

        /// <devdoc>
        ///     AutoValidate setting for this container is determined by its parent container.
        /// </devdoc>
        Inherit = -1,
    }
}
