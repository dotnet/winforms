// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms {

    using System.Diagnostics;
    using System;

    /// <devdoc>
    ///    <para>
    ///       Specifies the return value for HITTEST on treeview.
    ///    </para>
    /// </devdoc>
    public class TreeViewHitTestInfo {

        private TreeViewHitTestLocations loc;
        private TreeNode node;

        /// <devdoc>
        ///     Creates a TreeViewHitTestInfo instance.
        /// </devdoc>
        public TreeViewHitTestInfo(TreeNode hitNode, TreeViewHitTestLocations hitLocation) {
            this.node = hitNode;
            this.loc = hitLocation;
        }


        /// <devdoc>
        ///     This gives the exact location returned by hit test on treeview.
        /// </devdoc>
        public TreeViewHitTestLocations Location {
            get {
                return loc;
            }
        }

        /// <devdoc>
        ///     This gives the node returned by hit test on treeview.
        /// </devdoc>
        public TreeNode Node {
            get {
                return node;
            }
        }
    }
}
