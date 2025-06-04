// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms;

/// <summary>
///  Specifies the return value for HITTEST on treeview.
/// </summary>
public class TreeViewHitTestInfo
{
    private readonly TreeViewHitTestLocations _location;
    private readonly TreeNode? _node;

    /// <summary>
    ///  Creates a TreeViewHitTestInfo instance.
    /// </summary>
    public TreeViewHitTestInfo(TreeNode? hitNode, TreeViewHitTestLocations hitLocation)
    {
        _node = hitNode;
        _location = hitLocation;
    }

    /// <summary>
    ///  This gives the exact location returned by hit test on treeview.
    /// </summary>
    public TreeViewHitTestLocations Location
    {
        get
        {
            return _location;
        }
    }

    /// <summary>
    ///  This gives the node returned by hit test on treeview.
    /// </summary>
    public TreeNode? Node
    {
        get
        {
            return _node;
        }
    }
}
