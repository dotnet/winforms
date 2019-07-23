// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel;

namespace System.Windows.Forms
{
    /// <summary>
    ///  Provides data for the <see cref='TreeView.OnBeforeCheck'/>,
    ///  <see cref='TreeView.OnBeforeCollapse'/>,
    ///  <see cref='TreeView.OnBeforeExpand'/>,
    ///  or <see cref='TreeView.OnBeforeSelect'/> event.
    /// </summary>
    public class TreeViewCancelEventArgs : CancelEventArgs
    {
        private readonly TreeNode node;
        private readonly TreeViewAction action;

        public TreeViewCancelEventArgs(TreeNode node, bool cancel, TreeViewAction action)
        : base(cancel)
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

        public TreeViewAction Action
        {
            get
            {
                return action;
            }
        }
    }
}
