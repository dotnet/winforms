// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Drawing;
using WinForms.Common.Tests;
using Xunit;

namespace System.Windows.Forms.Tests
{
    public class TreeNodeCollectionTests
    {
        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetStringNormalizedTheoryData))]
        public void TreeNodeCollection_Add_String_Success(string text, string expectedText)
        {
            var treeView = new TreeView();
            TreeNodeCollection collection = treeView.Nodes;
            TreeNode treeNode = collection.Add(text);
            Assert.Same(treeNode, collection[0]);
            Assert.Same(treeNode, Assert.Single(collection));
            Assert.Equal(Color.Empty, treeNode.BackColor);
            Assert.True(treeNode.Bounds.X > 0);
            Assert.Equal(0, treeNode.Bounds.Y);
            Assert.True(treeNode.Bounds.Width > 0);
            Assert.True(treeNode.Bounds.Height > 0);
            Assert.False(treeNode.Checked);
            Assert.Null(treeNode.ContextMenu);
            Assert.Null(treeNode.ContextMenuStrip);
            Assert.Null(treeNode.FirstNode);
            Assert.Equal(Color.Empty, treeNode.ForeColor);
            Assert.Equal(expectedText, treeNode.FullPath);
            Assert.Equal(-1, treeNode.ImageIndex);
            Assert.Empty(treeNode.ImageKey);
            Assert.Equal(0, treeNode.Index);
            Assert.False(treeNode.IsEditing);
            Assert.False(treeNode.IsExpanded);
            Assert.False(treeNode.IsSelected);
            Assert.True(treeNode.IsVisible);
            Assert.Null(treeNode.LastNode);
            Assert.Equal(0, treeNode.Level);
            Assert.Empty(treeNode.Name);
            Assert.Null(treeNode.NextNode);
            Assert.Null(treeNode.NextVisibleNode);
            Assert.Null(treeNode.NodeFont);
            Assert.Empty(treeNode.Nodes);
            Assert.Same(treeNode.Nodes, treeNode.Nodes);
            Assert.Null(treeNode.Parent);
            Assert.Null(treeNode.PrevNode);
            Assert.Null(treeNode.PrevVisibleNode);
            Assert.Equal(-1, treeNode.SelectedImageIndex);
            Assert.Empty(treeNode.SelectedImageKey);
            Assert.Equal(-1, treeNode.StateImageIndex);
            Assert.Empty(treeNode.StateImageKey);
            Assert.Null(treeNode.Tag);
            Assert.Equal(expectedText, treeNode.Text);
            Assert.Empty(treeNode.ToolTipText);
            Assert.Same(treeView, treeNode.TreeView);
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(1)]
        public void TreeNodeCollection_Item_GetInvalidIndex_ThrowsArgumentOutOfRangeException(int index)
        {
            var treeView = new TreeView();
            TreeNodeCollection collection = treeView.Nodes;
            collection.Add("text");
            Assert.Throws<ArgumentOutOfRangeException>("index", () => collection[index]);
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(1)]
        public void TreeNodeCollection_Item_SetInvalidIndex_ThrowsArgumentOutOfRangeException(int index)
        {
            var treeView = new TreeView();
            TreeNodeCollection collection = treeView.Nodes;
            collection.Add("text");
            Assert.Throws<ArgumentOutOfRangeException>("index", () => collection[index] = new TreeNode());
        }
    }
}
