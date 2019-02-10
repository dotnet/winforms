// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms {


    /// <devdoc>
    ///    <para>
    ///       Specifies responsibility for drawing TreeView nodes.
    ///    </para>
    /// </devdoc>

    public enum TreeViewDrawMode {

        /// <devdoc>
        ///    <para>
        ///       The operating system paints the nodes of the TreeView.
        ///
        ///    </para>
        /// </devdoc>
        Normal = 0,


        /// <devdoc>
        ///    <para>
        ///	The user needs to paint the text only.
        ///    </para>
        /// </devdoc>
        OwnerDrawText = 1,


        /// <devdoc>
        ///    <para>
        ///	The user paints the entire row corresponding to a node, including lines and boxes.
        ///    </para>
        /// </devdoc>
        OwnerDrawAll = 2,
    }
}
