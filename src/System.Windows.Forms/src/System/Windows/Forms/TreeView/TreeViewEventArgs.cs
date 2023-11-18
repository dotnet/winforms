// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms;

/// <summary>
///  Provides data for the <see cref="TreeView.OnAfterCheck"/>,
///  <see cref="TreeView.AfterCollapse"/>,
///  <see cref="TreeView.AfterExpand"/>,
///  or <see cref="TreeView.AfterSelect"/> event.
/// </summary>
public class TreeViewEventArgs : EventArgs
{
    public TreeViewEventArgs(TreeNode? node)
    {
        Node = node;
        Action = TreeViewAction.Unknown;
    }

    public TreeViewEventArgs(TreeNode? node, TreeViewAction action)
    {
        Node = node;
        Action = action;
    }

    public TreeNode? Node { get; }

    /// <summary>
    ///  An event specific action-flag.
    /// </summary>
    public TreeViewAction Action { get; }
}
