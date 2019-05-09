// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms
{
    /// <devdoc>
    /// This enum is used to specify the action that caused a TreeViewEventArgs.
    /// </devdoc>
    public enum TreeViewAction
    {
        /// <devdoc>
        /// The action is unknown.
        /// </devdoc>
        Unknown = 0,

        /// <devdoc>
        /// The event was caused by a keystroke.
        /// </devdoc>
        ByKeyboard = 1,

        /// <devdoc>
        /// The event was caused by a mouse click.
        /// </devdoc>
        ByMouse = 2,

        /// <devdoc>
        /// The tree node is collapsing.
        /// </devdoc>
        Collapse = 3,

        /// <devdoc>
        /// The tree node is expanding.
        /// </devdoc>
        Expand = 4,
    }
}
