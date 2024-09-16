// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel.Design.Serialization;
using System.Collections;

namespace System.Windows.Forms.Tests;

public class TreeNodeConverterTests
{
    [WinFormsTheory]
    [InlineData(typeof(InstanceDescriptor), true)]
    [InlineData(typeof(string), true)]
    [InlineData(typeof(int), false)]
    public void TreeNodeConverter_CanConvertTo_Invoke_ReturnsExpected(Type destinationType, bool expected)
    {
        TreeNodeConverter converter = new();
        converter.CanConvertTo(null, destinationType).Should().Be(expected);
    }

    [WinFormsTheory]
    [InlineData(typeof(InstanceDescriptor), true)]
    [InlineData(typeof(string), true)]
    [InlineData(typeof(int), false)]
    public void TreeNodeConverter_ConvertTo_Invoke_ReturnsExpected(Type destinationType, bool canConvert)
    {
        TreeNodeConverter converter = new();
        TreeNode node = new("Test");

        if (canConvert)
        {
            object result = converter.ConvertTo(null, null, node, destinationType);
            if (destinationType == typeof(InstanceDescriptor))
            {
                result.Should().BeOfType<InstanceDescriptor>();
                var descriptor = (InstanceDescriptor)result;
                IList arguments = descriptor.Arguments as IList;
                arguments.Should().NotBeNull();
                arguments[0].Should().Be(node.Text);
            }
            else if (destinationType == typeof(string))
            {
                result.Should().BeOfType<string>();
                string text = (string)result;
                text.Should().Be("TreeNode: " + node.Text);
            }
        }
        else
        {
            converter.Invoking(c => c.ConvertTo(null, null, node, destinationType))
                .Should().Throw<NotSupportedException>();
        }
    }
}
