// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms
{

    /// <devdoc>
    /// Provides data for the <see cref='System.Windows.Forms.TreeView.TreeNodeMouseClickEventArgs'/>
    /// or <see cref='System.Windows.Forms.TreeView.OnNodeMouseClick'/> event.
    /// </devdoc>
    public class TreeNodeMouseClickEventArgs : MouseEventArgs
    {
        public TreeNodeMouseClickEventArgs(TreeNode node, MouseButtons button, int clicks, int x, int y) : base(button, clicks, x, y, 0)
        {
            Node = node;
        }

        public TreeNode Node { get; }
    }
}
