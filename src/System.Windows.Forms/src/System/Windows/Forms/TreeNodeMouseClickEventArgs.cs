// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms
{
    /// <summary>
    ///  Provides data for the <see cref='TreeView.OnNodeMouseClick'/> or
    ///  <see cref='TreeView.OnNodeMouseDoubleClick'/> event.
    /// </summary>
    public class TreeNodeMouseClickEventArgs : MouseEventArgs
    {
        public TreeNodeMouseClickEventArgs(TreeNode node, MouseButtons button, int clicks, int x, int y) : base(button, clicks, x, y, 0)
        {
            Node = node;
        }

        public TreeNode Node { get; }
    }
}
