// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms
{
    /// <summary>
    ///  For a given container control, determines whether the data in child controls
    ///  will automatically be validated when the user attempts to change the focus.
    /// </summary>
    public enum AutoValidate
    {
        /// <summary>
        ///  Controls in this container will not be validated when the focus changes.
        /// </summary>
        Disable = 0,

        /// <summary>
        ///  Controls in this container will be validated when the focus changes.
        ///  If a validation error occurs, the focus is forced to stay in the
        ///  current control.
        /// </summary>
        EnablePreventFocusChange = 1,

        /// <summary>
        ///  Controls in this container will be validated when the focus changes.
        ///  If a validation error occurs, the focus is allowed to move to the other
        ///  control.
        /// </summary>
        EnableAllowFocusChange = 2,

        /// <summary>
        ///  AutoValidate setting for this container is determined by its parent container.
        /// </summary>
        Inherit = -1,
    }
}
