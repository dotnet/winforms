// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms
{
    /// <summary>
    ///  Specifies the return value for HITTEST on treeview.
    /// </summary>
    public class TreeViewHitTestInfo
    {
        private readonly TreeViewHitTestLocations loc;
        private readonly TreeNode node;

        /// <summary>
        ///  Creates a TreeViewHitTestInfo instance.
        /// </summary>
        public TreeViewHitTestInfo(TreeNode hitNode, TreeViewHitTestLocations hitLocation)
        {
            node = hitNode;
            loc = hitLocation;
        }

        /// <summary>
        ///  This gives the exact location returned by hit test on treeview.
        /// </summary>
        public TreeViewHitTestLocations Location
        {
            get
            {
                return loc;
            }
        }

        /// <summary>
        ///  This gives the node returned by hit test on treeview.
        /// </summary>
        public TreeNode Node
        {
            get
            {
                return node;
            }
        }
    }
}
