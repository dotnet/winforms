// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable disable

namespace System.Windows.Forms
{
    /// <summary>
    ///  Provides data for the <see cref='TreeView.OnBeforeLabelEdit'/>
    ///  or <see cref='TreeView.OnAfterLabelEdit'/> event.
    /// </summary>
    public class NodeLabelEditEventArgs : EventArgs
    {
        public NodeLabelEditEventArgs(TreeNode node) : this(node, null)
        {
        }

        public NodeLabelEditEventArgs(TreeNode node, string label)
        {
            Node = node;
            Label = label;
        }

        public TreeNode Node { get; }

        public string Label { get; }

        public bool CancelEdit { get; set; }
    }
}
