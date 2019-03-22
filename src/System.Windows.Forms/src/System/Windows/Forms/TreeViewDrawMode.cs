// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms
{
    /// <devdoc>
    /// Specifies responsibility for drawing TreeView nodes.
    /// </devdoc>
    public enum TreeViewDrawMode
    {
        /// <devdoc>
        /// The operating system paints the nodes of the TreeView.
        /// </devdoc>
        Normal = 0,

        /// <devdoc>
        ///	The user needs to paint the text only.
        /// </devdoc>
        OwnerDrawText = 1,

        /// <devdoc>
        ///	The user paints the entire row corresponding to a node, including lines and boxes.
        /// </devdoc>
        OwnerDrawAll = 2,
    }
}
