// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel.Design.Serialization;
using System.Collections;

namespace System.Windows.Forms.Tests;

public class TreeNodeConverterTests
{
    [Theory]
    [InlineData(typeof(InstanceDescriptor), true)]
    [InlineData(typeof(string), true)]
    [InlineData(typeof(int), false)]
    public void TreeNodeConverter_CanConvertTo_Invoke_ReturnsExpected(Type destinationType, bool expected)
    {
        TreeNodeConverter converter = new();
        converter.CanConvertTo(null, destinationType).Should().Be(expected);
    }

    [Theory]
    [InlineData(typeof(InstanceDescriptor), true)]
    [InlineData(typeof(string), true)]
    [InlineData(typeof(int), false)]
    public void TreeNodeConverter_ConvertTo_Invoke_ReturnsExpected(Type destinationType, bool canConvert)
    {
        TreeNodeConverter converter = new();
        TreeNode node = new("Test");

        if (canConvert)
        {
            object? result = converter.ConvertTo(null, null, node, destinationType);
            result.Should().NotBeNull();

            if (destinationType == typeof(InstanceDescriptor))
            {
                var descriptor = result as InstanceDescriptor;

                var arguments = descriptor!.Arguments as IList;
                arguments![0].Should().Be(node.Text);
            }
            else if (destinationType == typeof(string))
            {
                result.Should().BeOfType<string>();
                result.Should().Be("TreeNode: " + node.Text);
            }
        }
        else
        {
            converter.Invoking(c => c.ConvertTo(null, null, node, destinationType))
                .Should().Throw<NotSupportedException>();
        }
    }

    [Fact]
    public void TreeNodeConverter_ConvertTo_InstanceDescriptor_NoChildNodes_ImageIndexMinusOne_Succeeds()
    {
        TreeNodeConverter converter = new();
        TreeNode node = new("NoChildrenMinusOne")
        {
            ImageIndex = -1,
            SelectedImageIndex = -1
        };

        object? result = converter.ConvertTo(context: null, culture: null, value: node, typeof(InstanceDescriptor));
        InstanceDescriptor descriptor = result.Should().BeOfType<InstanceDescriptor>().Subject;

        // Cast Arguments to IList for indexing.
        var args = (IList)descriptor.Arguments!;
        args.Count.Should().Be(1);
        args[0].Should().Be(node.Text);
    }

    [Fact]
    public void TreeNodeConverter_ConvertTo_InstanceDescriptor_ChildNodes_NoImageIndex_Succeeds()
    {
        TreeNodeConverter converter = new();
        TreeNode parentNode = new("ParentMinusOne") { ImageIndex = -1, SelectedImageIndex = -1 };
        parentNode.Nodes.Add("Child1");
        parentNode.Nodes.Add("Child2");

        object? result = converter.ConvertTo(context: null, culture: null, value: parentNode, typeof(InstanceDescriptor));
        InstanceDescriptor descriptor = result.Should().BeOfType<InstanceDescriptor>().Subject;

        // Cast Arguments to IList for indexing.
        var args = (IList)descriptor.Arguments!;
        args.Count.Should().Be(2);
        args[0].Should().Be(parentNode.Text);

        // The second argument should be an array of child nodes.
        args[1].Should().BeOfType<TreeNode[]>();
        var childNodes = (TreeNode[])args[1]!;
        childNodes.Should().HaveCount(2);
        childNodes.Select(n => n.Text).Should().ContainInOrder("Child1", "Child2");
    }

    [Fact]
    public void TreeNodeConverter_ConvertTo_InstanceDescriptor_ChildNodes_WithImageIndex_Succeeds()
    {
        TreeNodeConverter converter = new();
        TreeNode parentNode = new("ParentWithIndexes")
        {
            ImageIndex = 2,
            SelectedImageIndex = 3
        };

        parentNode.Nodes.Add("ChildA");

        object? result = converter.ConvertTo(context: null, culture: null, value: parentNode, typeof(InstanceDescriptor));
        InstanceDescriptor descriptor = result.Should().BeOfType<InstanceDescriptor>().Subject;

        // Cast Arguments to IList for indexing.
        var args = (IList)descriptor.Arguments!;
        args.Count.Should().Be(4);
        args[0].Should().Be(parentNode.Text);
        args[1].Should().Be(parentNode.ImageIndex);
        args[2].Should().Be(parentNode.SelectedImageIndex);

        // The fourth argument should be an array of child nodes.
        args[3].Should().BeOfType<TreeNode[]>();
        var childNodes = (TreeNode[])args[3]!;
        childNodes.Single().Text.Should().Be("ChildA");
    }
}
