// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;

namespace System.Windows.Forms.Tests;

public class TreeNodeCollectionTests
{
    [WinFormsTheory]
    [NormalizedStringData]
    public void TreeNodeCollection_Add_String_Success(string text, string expectedText)
    {
        using TreeView treeView = new();
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

    [WinFormsTheory]
    [InlineData(-1)]
    [InlineData(1)]
    public void TreeNodeCollection_Item_GetInvalidIndex_ThrowsArgumentOutOfRangeException(int index)
    {
        using TreeView treeView = new();
        TreeNodeCollection collection = treeView.Nodes;
        collection.Add("text");
        Assert.Throws<ArgumentOutOfRangeException>("index", () => collection[index]);
    }

    [WinFormsTheory]
    [InlineData(-1)]
    [InlineData(1)]
    public void TreeNodeCollection_Item_SetInvalidIndex_ThrowsArgumentOutOfRangeException(int index)
    {
        using TreeView treeView = new();
        TreeNodeCollection collection = treeView.Nodes;
        collection.Add("text");
        Assert.Throws<ArgumentOutOfRangeException>("index", () => collection[index] = new TreeNode());
    }

    [WinFormsFact]
    public void TreeNodeCollection_Item_SetNullTreeNode_ThrowsArgumentNullException()
    {
        using TreeView treeView = new();
        TreeNodeCollection collection = treeView.Nodes;
        TreeNode node = new("Node 0");
        collection.Add(node);
        Assert.Throws<ArgumentNullException>(() => collection[0] = null);
    }

    [WinFormsFact]
    public void TreeNodeCollection_Item_SetTreeNodeAlreadyAdded_Noop()
    {
        using TreeView treeView = new();
        TreeNodeCollection collection = treeView.Nodes;
        TreeNode node = new("Node 0");
        collection.Add(node);
        collection[0] = node;
        Assert.Single(collection);
    }

    [WinFormsFact]
    public void TreeNodeCollection_Item_SetExistentTreeNodeDifferentIndex_ThrowsArgumentException()
    {
        using TreeView treeView = new();
        TreeNodeCollection collection = treeView.Nodes;
        collection.Add("Node 0");
        collection.Add("Node 1");
        TreeNode node = collection[0];
        Assert.Throws<ArgumentException>(() => collection[1] = node);
    }

    [WinFormsFact]
    public void TreeNodeCollection_Item_SetTreeNodeBoundToAnotherTreeView_ThrowsArgumentException()
    {
        using TreeView anotherTreeView = new();
        anotherTreeView.Nodes.Add("Node 0");

        using TreeView treeView = new();
        TreeNodeCollection collection = treeView.Nodes;
        collection.Add("Node 1");
        TreeNode nodeOfAnotherTreeView = anotherTreeView.Nodes[0];
        Assert.Throws<ArgumentException>(() => collection[0] = nodeOfAnotherTreeView);
    }

    [WinFormsFact]
    public void TreeNodeCollection_Item_SetTreeNodeReplacesExistingOne()
    {
        using TreeView treeView = new();
        IntPtr forcedHandle = treeView.Handle;
        TreeNodeCollection collection = treeView.Nodes;
        collection.Add("Node 1");
        collection[0] = new TreeNode("New node 1");
        Assert.Single(treeView._nodesByHandle);
        Assert.Single(collection);
    }

    [WinFormsTheory]
    [InlineData("name2")]
    [InlineData("NAME2")]
    public void TreeNodeCollection_Find_InvokeKeyExists_ReturnsExpected(string key)
    {
        using TreeView treeView = new();
        TreeNode child1 = new()
        {
            Name = "name1"
        };
        TreeNode child2 = new()
        {
            Name = "name2"
        };
        TreeNode child3 = new()
        {
            Name = "name2"
        };

        TreeNode grandchild1 = new()
        {
            Name = "name1"
        };
        TreeNode grandchild2 = new()
        {
            Name = "name2"
        };
        TreeNode grandchild3 = new()
        {
            Name = "name2"
        };
        child3.Nodes.Add(grandchild1);
        child3.Nodes.Add(grandchild2);
        child3.Nodes.Add(grandchild3);
        TreeNodeCollection collection = treeView.Nodes;
        collection.Add(child1);
        collection.Add(child2);
        collection.Add(child3);

        // Search all children.
        Assert.Equal([child2, child3, grandchild2, grandchild3], collection.Find(key, searchAllChildren: true));

        // Call again.
        Assert.Equal([child2, child3, grandchild2, grandchild3], collection.Find(key, searchAllChildren: true));

        // Don't search all children.
        Assert.Equal([child2, child3], collection.Find(key, searchAllChildren: false));

        // Call again.
        Assert.Equal([child2, child3], collection.Find(key, searchAllChildren: false));
    }

    [WinFormsTheory]
    [InlineData("NoSuchName")]
    [InlineData("abcd")]
    [InlineData("abcde")]
    [InlineData("abcdef")]
    public void TreeNodeCollection_Find_InvokeNoSuchKey_ReturnsEmpty(string key)
    {
        using TreeView treeView = new();
        TreeNode child1 = new()
        {
            Name = "name1"
        };
        TreeNode child2 = new()
        {
            Name = "name2"
        };
        TreeNode child3 = new()
        {
            Name = "name2"
        };
        TreeNodeCollection collection = treeView.Nodes;
        collection.Add(child1);
        collection.Add(child2);
        collection.Add(child3);

        Assert.Empty(collection.Find(key, searchAllChildren: true));
        Assert.Empty(collection.Find(key, searchAllChildren: false));
    }

    [WinFormsFact]
    public void TreeNodeCollection_Sort_ShouldAfterAddingItems()
    {
        using TreeView treeView = new();
        string key = "7";
        TreeNode child1 = new()
        {
            Name = "8",
            Text = "8"
        };
        TreeNode child2 = new()
        {
            Name = "5",
            Text = "5"
        };
        TreeNode child3 = new()
        {
            Name = "7",
            Text = "7"
        };

        treeView.Nodes.Add(child1);
        treeView.Nodes.Add(child2);
        treeView.Nodes.Add(child3);

        treeView.Sort();
        treeView.CreateControl();

        TreeNode[] treeNodeArray =
        [
            new()
            {
                Name = "2",
                Text = "2"
            },
            new()
            {
                Name = "1",
                Text = "1"
            }
        ];

        treeView.Nodes.AddRange(treeNodeArray);

        TreeNode treeNode = treeView.Nodes.Find(key, searchAllChildren: true)[0];
        treeNode.Should().NotBeNull();
        treeView.Nodes.IndexOf(treeNode).Should().Be(3);
    }

    [WinFormsFact]
    public void TreeNodeCollection_TreeNodeCollectionAddRangeRespectsSortOrderSwitch()
    {
        TreeNode child1 = new()
        {
            Name = "8",
            Text = "8"
        };
        TreeNode child2 = new()
        {
            Name = "5",
            Text = "5"
        };
        TreeNode child3 = new()
        {
            Name = "7",
            Text = "7"
        };
        TreeNode[] treeNodeArray =
        [
            new()
            {
                Name = "2",
                Text = "2"
            },
            new()
            {
                Name = "1",
                Text = "1"
            }
        ];

        TreeNode treeNode;
        using (TreeNodeCollectionAddRangeRespectsSortOrderScope scope = new(enable: true))
        {
            using TreeView treeView2 = new();

            treeView2.Nodes.Add(child1);
            treeView2.Nodes.Add(child2);
            treeView2.Nodes.Add(child3);

            treeView2.CreateControl();
            treeView2.Sort();
            treeView2.Nodes.AddRange(treeNodeArray);

            treeNode = treeView2.Nodes.Find("2", searchAllChildren: true)[0];
            treeNode.Should().NotBeNull();
            treeView2.Nodes.IndexOf(treeNode).Should().Be(1);
            treeView2.Nodes.Clear();
        }

        using TreeView treeView3 = new();

        treeView3.Nodes.Add(child1);
        treeView3.Nodes.Add(child2);
        treeView3.Nodes.Add(child3);

        treeView3.CreateControl();
        treeView3.Sort();
        treeView3.Nodes.AddRange(treeNodeArray);

        treeNode = treeView3.Nodes.Find("2", searchAllChildren: true)[0];
        treeNode.Should().NotBeNull();
        treeView3.Nodes.IndexOf(treeNode).Should().Be(1);
    }

    [WinFormsTheory]
    [NullAndEmptyStringData]
    public void TreeNodeCollection_Find_NullOrEmptyKey_ThrowsArgumentNullException(string key)
    {
        using TreeView treeView = new();
        var collection = treeView.Nodes;
        Assert.Throws<ArgumentNullException>("key", () => collection.Find(key, searchAllChildren: true));
        Assert.Throws<ArgumentNullException>("key", () => collection.Find(key, searchAllChildren: false));
    }

    private TreeNodeCollection CreateCollectionWithNodes()
    {
        using TreeView treeView = new();

        TreeNode child1 = new() { Name = "name1" };
        TreeNode child2 = new() { Name = "name2" };
        TreeNodeCollection collection = treeView.Nodes;
        collection.Add(child1);
        collection.Add(child2);

        return collection;
    }

    public static TheoryData<string, string> KeyAndExpectedNodeNameData => new()
    {
        { "name1", "name1" },
        { "NAME1", "name1" },
        { "NoSuchName", null },
        { null, null },
        { string.Empty, null }
    };

    [WinFormsTheory]
    [MemberData(nameof(KeyAndExpectedNodeNameData))]
    public void TreeNodeCollection_Item_GetKey_ReturnsExpected(string key, string expectedNodeName)
    {
        TreeNodeCollection collection = CreateCollectionWithNodes();
        collection[key]?.Name.Should().Be(expectedNodeName);
    }

    [WinFormsFact]
    public void TreeNodeCollection_IsReadOnly_ReturnsExpected()
    {
        using TreeView treeView = new();

        TreeNodeCollection collection = treeView.Nodes;
        collection.IsReadOnly.Should().BeFalse();
    }

    [WinFormsTheory]
    [InlineData("key1", "text1", 1, null)]
    [InlineData("key1", "text1", 1, 2)]
    public void TreeNodeCollection_Add_KeyTextImageIndexSelectedImageIndex_Success(string key, string text, int imageIndex, int? selectedImageIndex)
    {
        using TreeView treeView = new();

        TreeNodeCollection collection = treeView.Nodes;
        TreeNode treeNode;

        if (selectedImageIndex.HasValue)
        {
            treeNode = collection.Add(key, text, imageIndex, selectedImageIndex.Value);
            treeNode.SelectedImageIndex.Should().Be(selectedImageIndex.Value);
        }
        else
        {
            treeNode = collection.Add(key, text, imageIndex);
        }

        treeNode.Should().Be(collection[0]);
        treeNode.Name.Should().Be(key);
        treeNode.Text.Should().Be(text);
        treeNode.ImageIndex.Should().Be(imageIndex);
    }

    [WinFormsTheory]
    [InlineData("key1", "text1", "imageKey1")]
    public void TreeNodeCollection_Add_KeyTextImageKey_Success(string key, string text, string imageKey)
    {
        using TreeView treeView = new();

        TreeNodeCollection collection = treeView.Nodes;
        TreeNode treeNode = collection.Add(key, text, imageKey);

        treeNode.Should().Be(collection[0]);
        treeNode.Name.Should().Be(key);
        treeNode.Text.Should().Be(text);
        treeNode.ImageKey.Should().Be(imageKey);
    }

    [WinFormsTheory]
    [InlineData("key1", "text1", "imageKey1", "selectedImageKey1")]
    public void TreeNodeCollection_Add_KeyTextImageKeySelectedImageKey_Success(string key, string text, string imageKey, string selectedImageKey)
    {
        using TreeView treeView = new();

        TreeNodeCollection collection = treeView.Nodes;
        TreeNode treeNode = collection.Add(key, text, imageKey, selectedImageKey);

        treeNode.Should().Be(collection[0]);
        treeNode.Name.Should().Be(key);
        treeNode.Text.Should().Be(text);
        treeNode.ImageKey.Should().Be(imageKey);
        treeNode.SelectedImageKey.Should().Be(selectedImageKey);
    }

    [WinFormsTheory]
    [InlineData("Node 0", null, null, null)]
    [InlineData("Node 0", "key", null, null)]
    [InlineData("Node 0", "key", "imageKey", null)]
    [InlineData("Node 0", "key", "imageKey", "selectedImageKey")]
    public void TreeNodeCollection_Insert_StringKeyImageKeySelectedImageKey_Success(string text, string key, string imageKey, string selectedImageKey)
    {
        using TreeView treeView = new();

        TreeNodeCollection collection = treeView.Nodes;
        TreeNode treeNode;

        if (key is not null)
        {
            if (imageKey is not null)
            {
                if (selectedImageKey is not null)
                {
                    treeNode = collection.Insert(0, key, text, imageKey, selectedImageKey);
                    treeNode.SelectedImageKey.Should().Be(selectedImageKey);
                }
                else
                {
                    treeNode = collection.Insert(0, key, text, imageKey);
                }

                treeNode.ImageKey.Should().Be(imageKey);
            }
            else
            {
                treeNode = collection.Insert(0, key, text);
            }

            treeNode.Name.Should().Be(key);
        }
        else
        {
            treeNode = collection.Insert(0, text);
        }

        treeNode.Should().Be(collection[0]);
        treeNode.Text.Should().Be(text);
    }

    [WinFormsTheory]
    [InlineData("key", "Node 0", 0, null)]
    [InlineData("key", "Node 0", 0, 1)]
    public void TreeNodeCollection_Insert_StringKeyImageIndexSelectedImageIndex_Success(string key, string text, int imageIndex, int? selectedImageIndex)
    {
        using TreeView treeView = new();

        TreeNodeCollection collection = treeView.Nodes;
        TreeNode treeNode;

        if (selectedImageIndex.HasValue)
        {
            treeNode = collection.Insert(0, key, text, imageIndex, selectedImageIndex.Value);
            treeNode.SelectedImageIndex.Should().Be(selectedImageIndex.Value);
        }
        else
        {
            treeNode = collection.Insert(0, key, text, imageIndex);
        }

        treeNode.Should().Be(collection[0]);
        treeNode.ImageIndex.Should().Be(imageIndex);
    }

    [WinFormsTheory]
    [InlineData("name1", true)]
    [InlineData("name2", true)]
    [InlineData("NonExistentNode", false)]
    public void TreeNodeCollection_Contains_VariousScenarios_ReturnsExpected(string nodeName, bool expected)
    {
        var collection = CreateCollectionWithNodes();
        TreeNode node = new(nodeName);
        collection.Add(node);

        bool result = collection.Contains(collection[nodeName]);

        if (nodeName == "NonExistentNode")
        {
            collection.Remove(node);
        }

        result.Should().Be(expected);
    }

    [WinFormsFact]
    public void TreeNodeCollection_Contains_SpecialCases_ReturnsFalse()
    {
        var collection = CreateCollectionWithNodes();
        var nodeToRemove = collection[0];

        collection.Remove(nodeToRemove);
        collection.Contains(nodeToRemove).Should().BeFalse();

        collection.Clear();
        collection.Contains(nodeToRemove).Should().BeFalse();
    }
}
