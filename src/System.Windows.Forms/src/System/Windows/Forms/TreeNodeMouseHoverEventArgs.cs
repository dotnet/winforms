// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms
{
    /// <devdoc>
    /// Provides data for the <see cref='System.Windows.Forms.TreeView.OnAfterCheck'/>,
    /// <see cref='System.Windows.Forms.TreeView.AfterCollapse'/>, <see cref='System.Windows.Forms.TreeView.AfterExpand'/>,
    /// or <see cref='System.Windows.Forms.TreeView.AfterSelect'/> event.
    /// </devdoc>
    public class TreeViewEventArgs : EventArgs
    {
        public TreeViewEventArgs(TreeNode node) : this(node, TreeViewAction.Unknown)
        {
        }

        public TreeViewEventArgs(TreeNode node, TreeViewAction action)
        {
            Node = node;
            Action = action;
        }

        public TreeNode Node { get; }

        /// <devdoc>
        /// An event specific action-flag.
        /// </devdoc>
        public TreeViewAction Action { get; }
    }
}