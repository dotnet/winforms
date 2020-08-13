// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable disable

using System.Runtime.InteropServices;

namespace System.Windows.Forms
{
    /// <summary>
    ///  Provides data for the <see cref='TreeView.OnNodeMouseHover'/> event.
    /// </summary>
    public class TreeNodeMouseHoverEventArgs : EventArgs
    {
        public TreeNodeMouseHoverEventArgs(TreeNode node)
        {
            Node = node;
        }

        public TreeNode Node { get; }
    }
}
