// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable disable

namespace System.Windows.Forms
{
    /// <summary>
    ///  Provides data for the <see cref='TreeView.OnAfterCheck'/>, <see cref='TreeView.AfterCollapse'/>, <see cref='TreeView.AfterExpand'/>, or <see cref='TreeView.AfterSelect'/> event.
    /// </summary>
    public class TreeViewEventArgs : EventArgs
    {
        readonly TreeNode node;
        readonly TreeViewAction action = TreeViewAction.Unknown;

        public TreeViewEventArgs(TreeNode node)
        {
            this.node = node;
        }

        public TreeViewEventArgs(TreeNode node, TreeViewAction action)
        {
            this.node = node;
            this.action = action;
        }

        public TreeNode Node
        {
            get
            {
                return node;
            }
        }

        /// <summary>
        ///  An event specific action-flag.
        /// </summary>
        public TreeViewAction Action
        {
            get
            {
                return action;
            }
        }
    }
}
