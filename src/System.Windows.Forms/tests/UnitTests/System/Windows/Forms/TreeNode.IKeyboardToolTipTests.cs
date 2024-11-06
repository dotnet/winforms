// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;
using System.Runtime.InteropServices;

namespace System.Windows.Forms.Tests;

public class TreeNode_TreeNodeIKeyboardToolTipTests
{
    [WinFormsTheory]
    [InlineData(true, true)]
    [InlineData(true, false)]
    [InlineData(false, true)]
    [InlineData(false, false)]
    public void TreeNodeIKeyboardToolTip_InvokeAllowsToolTip_ReturnsExpected(bool insideTreeView, bool showNodeToolTips)
    {
        using TreeView treeView = new() { ShowNodeToolTips = showNodeToolTips };
        TreeNode treeNode = new();
        if (insideTreeView)
        {
            treeView.Nodes.Add(treeNode);
        }

        Assert.True(((IKeyboardToolTip)treeNode).AllowsToolTip());
    }

    [WinFormsTheory]
    [InlineData(true, true, true)]
    [InlineData(true, false, false)]
    [InlineData(false, true, false)]
    [InlineData(false, false, false)]
    public void TreeNodeIKeyboardToolTip_InvokeAllowsChildrenToShowToolTips_ReturnsExpected(
        bool insideTreeView,
        bool showNodeToolTips,
        bool expectedResult)
    {
        using TreeView treeView = new() { ShowNodeToolTips = showNodeToolTips };
        TreeNode treeNode = new();

        if (insideTreeView)
        {
            treeView.Nodes.Add(treeNode);
        }

        Assert.Equal(expectedResult, ((IKeyboardToolTip)treeNode).AllowsChildrenToShowToolTips());
    }

    [WinFormsTheory]
    [InlineData(true, true, true)]
    [InlineData(true, false, false)]
    [InlineData(false, true, false)]
    [InlineData(false, false, false)]
    public void TreeNodeIKeyboardToolTip_InvokeCanShowToolTipsNow_ReturnsExpected(
        bool insideTreeView,
        bool showNodeToolTips,
        bool expectedResult)
    {
        using TreeView treeView = new() { ShowNodeToolTips = showNodeToolTips };
        TreeNode treeNode = new();

        if (insideTreeView)
        {
            treeView.Nodes.Add(treeNode);
        }

        Assert.Equal(expectedResult, ((IKeyboardToolTip)treeNode).CanShowToolTipsNow());
    }

    [WinFormsTheory]
    [InlineData(true, true, "Test tooltip")]
    [InlineData(true, false, "Test tooltip")]
    [InlineData(false, true, "Test tooltip")]
    [InlineData(false, false, "Test tooltip")]
    public void TreeNodeIKeyboardToolTip_InvokeGetCaptionForTool_ReturnsExpected(
        bool insideTreeView,
        bool showNodeToolTips,
        string toolTipText)
    {
        using TreeView treeView = new() { ShowNodeToolTips = showNodeToolTips };
        using ToolTip toolTip = new();
        TreeNode treeNode = new() { ToolTipText = toolTipText };

        if (insideTreeView)
        {
            treeView.Nodes.Add(treeNode);
        }

        Assert.Equal(toolTipText, ((IKeyboardToolTip)treeNode).GetCaptionForTool(toolTip));
    }

    [WinFormsTheory]
    [InlineData(true, true)]
    [InlineData(true, false)]
    [InlineData(false, true)]
    [InlineData(false, false)]
    public void TreeNodeIKeyboardToolTip_InvokeGetNativeScreenRectangle_ReturnsExpected(
        bool insideTreeView,
        bool showNodeToolTips)
    {
        TreeNode treeNode = new();
        using TreeView treeView = new() { ShowNodeToolTips = showNodeToolTips };
        Rectangle expectedBounds = Rectangle.Empty;

        if (insideTreeView)
        {
            treeView.Nodes.Add(treeNode);
            expectedBounds = GetRectangle(treeView, treeNode);
        }

        Assert.Equal(expectedBounds, ((IKeyboardToolTip)treeNode).GetNativeScreenRectangle());
    }

    [WinFormsTheory]
    [InlineData(true)]
    [InlineData(false)]
    public void TreeNodeIKeyboardToolTip_InvokeGetNeighboringToolsRectangles_FirstTreeNode_ReturnsExpected(bool showNodeToolTips)
    {
        List<TreeNode> treeNodesList = GetHierarchyNodes();
        using TreeView treeView = new() { ShowNodeToolTips = showNodeToolTips };
        treeView.Nodes.AddRange([.. treeNodesList]);
        TreeNode firstTreeNode = treeView.Nodes[0];

        IList<Rectangle> neighboringRectangles = ((IKeyboardToolTip)firstTreeNode).GetNeighboringToolsRectangles();

        Assert.Single(neighboringRectangles);
        Assert.Contains(GetRectangle(treeView, treeView.Nodes[1]), neighboringRectangles);
    }

    [WinFormsTheory]
    [InlineData(true)]
    [InlineData(false)]
    public void TreeNodeIKeyboardToolTip_InvokeGetNeighboringToolsRectangles_MiddleTreeNode_ReturnsExpected(bool showNodeToolTips)
    {
        List<TreeNode> treeNodesList = GetHierarchyNodes();
        using TreeView treeView = new() { ShowNodeToolTips = showNodeToolTips };
        treeView.Nodes.AddRange([.. treeNodesList]);
        TreeNode middleTreeNode = treeView.Nodes[1];

        IList<Rectangle> neighboringRectangles = ((IKeyboardToolTip)middleTreeNode).GetNeighboringToolsRectangles();

        Assert.Equal(2, neighboringRectangles.Count);
        Assert.Contains(GetRectangle(treeView, treeView.Nodes[0]), neighboringRectangles);
        Assert.Contains(GetRectangle(treeView, treeView.Nodes[2]), neighboringRectangles);
    }

    [WinFormsTheory]
    [InlineData(true)]
    [InlineData(false)]
    public void TreeNodeIKeyboardToolTip_InvokeGetNeighboringToolsRectangles_LastTreeNode_ReturnsExpected(bool showNodeToolTips)
    {
        List<TreeNode> treeNodesList = GetHierarchyNodes();
        using TreeView treeView = new() { ShowNodeToolTips = showNodeToolTips };
        treeView.Nodes.AddRange([.. treeNodesList]);
        TreeNode lastTreeNode = treeView.Nodes[0];

        IList<Rectangle> neighboringRectangles = ((IKeyboardToolTip)lastTreeNode).GetNeighboringToolsRectangles();

        Assert.Single(neighboringRectangles);
        Assert.Contains(GetRectangle(treeView, treeView.Nodes[1]), neighboringRectangles);
    }

    [WinFormsTheory]
    [InlineData(true)]
    [InlineData(false)]
    public void TreeNodeIKeyboardToolTip_InvokeGetNeighboringToolsRectangles_FirstSubTreeNode_ReturnsExpected(bool showNodeToolTips)
    {
        List<TreeNode> treeNodesList = GetHierarchyNodes();
        using TreeView treeView = new() { ShowNodeToolTips = showNodeToolTips };
        treeView.Nodes.AddRange([.. treeNodesList]);
        TreeNode rootNode = treeView.Nodes[0];
        rootNode.Expand();
        TreeNode firstSubNode = rootNode.Nodes[0];

        IList<Rectangle> neighboringRectangles = ((IKeyboardToolTip)firstSubNode).GetNeighboringToolsRectangles();

        Assert.Equal(2, neighboringRectangles.Count);
        Assert.Contains(GetRectangle(treeView, rootNode), neighboringRectangles);
        Assert.Contains(GetRectangle(treeView, rootNode.Nodes[1]), neighboringRectangles);
    }

    [WinFormsTheory]
    [InlineData(true)]
    [InlineData(false)]
    public void TreeNodeIKeyboardToolTip_InvokeGetNeighboringToolsRectangles_MiddleSubTreeNode_ReturnsExpected(bool showNodeToolTips)
    {
        List<TreeNode> treeNodesList = GetHierarchyNodes();
        using TreeView treeView = new() { ShowNodeToolTips = showNodeToolTips };
        treeView.Nodes.AddRange([.. treeNodesList]);
        TreeNode rootNode = treeView.Nodes[0];
        rootNode.Expand();
        TreeNode middleSubNode = rootNode.Nodes[1];

        IList<Rectangle> neighboringRectangles = ((IKeyboardToolTip)middleSubNode).GetNeighboringToolsRectangles();

        Assert.Equal(2, neighboringRectangles.Count);
        Assert.Contains(GetRectangle(treeView, rootNode.Nodes[0]), neighboringRectangles);
        Assert.Contains(GetRectangle(treeView, rootNode.Nodes[2]), neighboringRectangles);
    }

    [WinFormsTheory]
    [InlineData(true)]
    [InlineData(false)]
    public void TreeNodeIKeyboardToolTip_InvokeGetNeighboringToolsRectangles_LastSubTreeNode_ReturnsExpected(bool showNodeToolTips)
    {
        List<TreeNode> treeNodesList = GetHierarchyNodes();
        using TreeView treeView = new() { ShowNodeToolTips = showNodeToolTips };
        treeView.Nodes.AddRange([.. treeNodesList]);
        TreeNode rootNode = treeView.Nodes[0];
        rootNode.Expand();
        TreeNode lastSubNode = rootNode.Nodes[2];

        IList<Rectangle> neighboringRectangles = ((IKeyboardToolTip)lastSubNode).GetNeighboringToolsRectangles();

        Assert.Equal(2, neighboringRectangles.Count);
        Assert.Contains(GetRectangle(treeView, rootNode.Nodes[1]), neighboringRectangles);
        Assert.Contains(GetRectangle(treeView, treeView.Nodes[1]), neighboringRectangles);
    }

    [WinFormsTheory]
    [InlineData(true)]
    [InlineData(false)]
    public void TreeNodeIKeyboardToolTip_InvokeGetNeighboringToolsRectangles_SubSubTreeNode_ReturnsExpected(bool showNodeToolTips)
    {
        List<TreeNode> treeNodesList = GetHierarchyNodes();
        using TreeView treeView = new() { ShowNodeToolTips = showNodeToolTips };
        treeView.Nodes.AddRange([.. treeNodesList]);
        TreeNode rootNode = treeView.Nodes[0];
        rootNode.Expand();
        TreeNode lastSubNode = rootNode.Nodes[2];
        lastSubNode.Expand();
        TreeNode subSubNode = lastSubNode.Nodes[0];

        IList<Rectangle> neighboringRectangles = ((IKeyboardToolTip)subSubNode).GetNeighboringToolsRectangles();

        Assert.Equal(2, neighboringRectangles.Count);
        Assert.Contains(GetRectangle(treeView, treeView.Nodes[1]), neighboringRectangles);
        Assert.Contains(GetRectangle(treeView, lastSubNode), neighboringRectangles);
    }

    [WinFormsTheory]
    [InlineData(true)]
    [InlineData(false)]
    public void TreeNodeIKeyboardToolTip_InvokeGetNeighboringToolsRectangles_TreeNodeAfterExpandedNode_ReturnsExpected(bool showNodeToolTips)
    {
        List<TreeNode> treeNodesList = GetHierarchyNodes();
        using TreeView treeView = new() { ShowNodeToolTips = showNodeToolTips };
        treeView.Nodes.AddRange([.. treeNodesList]);
        TreeNode firstNode = treeView.Nodes[0];
        firstNode.Expand();
        TreeNode middleNode = treeView.Nodes[1];

        IList<Rectangle> neighboringRectangles = ((IKeyboardToolTip)middleNode).GetNeighboringToolsRectangles();

        Assert.Equal(2, neighboringRectangles.Count);
        Assert.Contains(GetRectangle(treeView, treeView.Nodes[2]), neighboringRectangles);
        Assert.Contains(GetRectangle(treeView, firstNode.Nodes[2]), neighboringRectangles);
    }

    [WinFormsTheory]
    [InlineData(true)]
    [InlineData(false)]
    public void TreeNodeIKeyboardToolTip_InvokeGetNeighboringToolsRectangles_TreeNodeAfterExpandedSubNode_ReturnsExpected(bool showNodeToolTips)
    {
        List<TreeNode> treeNodesList = GetHierarchyNodes();
        using TreeView treeView = new() { ShowNodeToolTips = showNodeToolTips };
        treeView.Nodes.AddRange([.. treeNodesList]);
        TreeNode firstNode = treeView.Nodes[0];
        firstNode.Expand();
        TreeNode subNode = firstNode.Nodes[2];
        subNode.Expand();
        TreeNode middleNode = treeView.Nodes[1];

        IList<Rectangle> neighboringRectangles = ((IKeyboardToolTip)middleNode).GetNeighboringToolsRectangles();

        Assert.Equal(2, neighboringRectangles.Count);
        Assert.Contains(GetRectangle(treeView, treeView.Nodes[2]), neighboringRectangles);
        Assert.Contains(GetRectangle(treeView, subNode.Nodes[0]), neighboringRectangles);
    }

    [WinFormsTheory]
    [InlineData(true)]
    [InlineData(false)]
    public void TreeNodeIKeyboardToolTip_InvokeGetNeighboringToolsRectangles_ExpandedSubTreeNode_ReturnsExpected(bool showNodeToolTips)
    {
        List<TreeNode> treeNodesList = GetHierarchyNodes();
        using TreeView treeView = new() { ShowNodeToolTips = showNodeToolTips };
        treeView.Nodes.AddRange([.. treeNodesList]);
        TreeNode rootNode = treeView.Nodes[0];
        rootNode.Expand();
        TreeNode lastSubNode = rootNode.Nodes[2];
        lastSubNode.Expand();

        IList<Rectangle> neighboringRectangles = ((IKeyboardToolTip)lastSubNode).GetNeighboringToolsRectangles();

        Assert.Equal(2, neighboringRectangles.Count);
        Assert.Contains(GetRectangle(treeView, rootNode.Nodes[1]), neighboringRectangles);
        Assert.Contains(GetRectangle(treeView, lastSubNode.Nodes[0]), neighboringRectangles);
    }

    [WinFormsFact]
    public void TreeNodeIKeyboardToolTip_InvokeGetNeighboringToolsRectangles_WithoutTreeView_ReturnsEmptyList()
    {
        List<TreeNode> treeNodesList = GetHierarchyNodes();

        Assert.Empty(((IKeyboardToolTip)treeNodesList[0]).GetNeighboringToolsRectangles());
        Assert.Empty(((IKeyboardToolTip)treeNodesList[1]).GetNeighboringToolsRectangles());
        Assert.Empty(((IKeyboardToolTip)treeNodesList[2]).GetNeighboringToolsRectangles());
    }

    [WinFormsTheory]
    [InlineData(true)]
    [InlineData(false)]
    public void TreeNodeIKeyboardToolTip_InvokeGetOwnerWindow_ReturnsExpected(bool insideTreeView)
    {
        TreeNode treeNode = new();
        using TreeView treeView = new();
        IWin32Window expectedOwner = null;

        if (insideTreeView)
        {
            treeView.Nodes.Add(treeNode);
            expectedOwner = treeView;
        }

        Assert.Equal(expectedOwner, ((IKeyboardToolTip)treeNode).GetOwnerWindow());
    }

    [WinFormsTheory]
    [InlineData(true, RightToLeft.Yes, true)]
    [InlineData(true, RightToLeft.No, false)]
    [InlineData(true, RightToLeft.Inherit, false)]
    [InlineData(false, RightToLeft.Yes, false)]
    public void TreeNodeIKeyboardToolTip_InvokeHasRtlModeEnabled_ReturnsExpected(
        bool insideTreeView,
        RightToLeft rightToLeft,
        bool expected)
    {
        TreeNode treeNode = new();
        using TreeView treeView = new() { RightToLeft = rightToLeft };
        IWin32Window expectedOwner = null;

        if (insideTreeView)
        {
            treeView.Nodes.Add(treeNode);
            expectedOwner = treeView;
        }

        Assert.Equal(expected, ((IKeyboardToolTip)treeNode).HasRtlModeEnabled());
    }

    [WinFormsTheory]
    [ActiveIssue("https://github.com/dotnet/winforms/issues/11752")]
    [InlineData(true, true, true)]
    [InlineData(true, false, false)]
    [InlineData(false, true, false)]
    [InlineData(false, false, false)]
    public void TreeNodeIKeyboardToolTip_InvokeIsHoveredWithMouse_ReturnsExpected(
        bool insideTreeView,
        bool isHovered,
        bool expected)
    {
        // Skip validation on X64 due to the active issue "https://github.com/dotnet/winforms/issues/11752"
        if (insideTreeView && isHovered && expected && RuntimeInformation.ProcessArchitecture == Architecture.X64)
        {
            return;
        }

        Point initialPosition = Cursor.Position;
        try
        {
            TreeNode treeNode = new();
            using TreeView treeView = new() { Size = new Size(50, 50) };
            treeView.CreateControl();

            if (insideTreeView)
            {
                treeView.Nodes.Add(treeNode);
            }

            Point position = treeView.AccessibilityObject.Bounds.Location;
            if (!isHovered)
            {
                position.X--;
                position.Y--;
            }

            Cursor.Position = position;
            Assert.Equal(expected, ((IKeyboardToolTip)treeNode).IsHoveredWithMouse());
        }
        finally
        {
            Cursor.Position = initialPosition;
        }
    }

    [WinFormsTheory]
    [InlineData(true, true, true)]
    [InlineData(true, false, false)]
    [InlineData(false, true, false)]
    [InlineData(false, false, false)]
    public void TreeNodeIKeyboardToolTip_Invoke_ReturnsExpected(
        bool insideTreeView,
        bool showNodeToolTips,
        bool expectedResult)
    {
        using TreeView treeView = new() { ShowNodeToolTips = showNodeToolTips };
        TreeNode treeNode = new();

        if (insideTreeView)
        {
            treeView.Nodes.Add(treeNode);
        }

        Assert.Equal(expectedResult, ((IKeyboardToolTip)treeNode).ShowsOwnToolTip());
    }

    private List<TreeNode> GetHierarchyNodes()
    {
        List<TreeNode> treeNodesList = [];

        TreeNode rootNode1 = new();
        TreeNode rootNode2 = new();
        TreeNode rootNode3 = new();

        TreeNode subNode1 = new();
        TreeNode subNode2 = new();
        TreeNode subNode3 = new();

        TreeNode subSubNode = new();

        subNode3.Nodes.Add(subSubNode);
        rootNode1.Nodes.AddRange([subNode1, subNode2, subNode3]);
        treeNodesList.AddRange(new TreeNode[] { rootNode1, rootNode2, rootNode3 });
        return treeNodesList;
    }

    private Rectangle GetRectangle(TreeView treeView, TreeNode treeNode)
    {
        return treeView.RectangleToScreen(treeNode.Bounds);
    }
}
