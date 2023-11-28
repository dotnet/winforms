// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;

namespace System.Windows.Forms;

/// <summary>
///  Provides data for the <see cref="TreeView.OnBeforeCheck"/>,
///  <see cref="TreeView.OnBeforeCollapse"/>,
///  <see cref="TreeView.OnBeforeExpand"/>,
///  or <see cref="TreeView.OnBeforeSelect"/> event.
/// </summary>
public class TreeViewCancelEventArgs : CancelEventArgs
{
    public TreeViewCancelEventArgs(TreeNode? node, bool cancel, TreeViewAction action)
        : base(cancel)
    {
        Node = node;
        Action = action;
    }

    public TreeNode? Node { get; }

    public TreeViewAction Action { get; }
}
