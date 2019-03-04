// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using Xunit;

namespace System.Windows.Forms.Tests
{
    public class NodeLabelEditEventArgsTests
    {
        public static IEnumerable<object[]> Ctor_TreeNode_TestData()
        {
            yield return new object[] { null };
            yield return new object[] { new TreeNode() };
        }

        [Theory]
        [MemberData(nameof(Ctor_TreeNode_TestData))]
        public void Ctor_TreeNode(TreeNode node)
        {
            var e = new NodeLabelEditEventArgs(node);
            Assert.Equal(node, e.Node);
            Assert.Null(e.Label);
            Assert.False(e.CancelEdit);
        }

        public static IEnumerable<object[]> Ctor_TreeNode_String_TestData()
        {
            yield return new object[] { null, null };
            yield return new object[] { new TreeNode(), "" };
            yield return new object[] { new TreeNode(), "label" };
        }

        [Theory]
        [MemberData(nameof(Ctor_TreeNode_String_TestData))]
        public void Ctor_TreeNode_String(TreeNode node, string label)
        {
            var e = new NodeLabelEditEventArgs(node, label);
            Assert.Equal(node, e.Node);
            Assert.Equal(label, e.Label);
            Assert.False(e.CancelEdit);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void CancelEdit_Set_GetReturnsExpected(bool value)
        {
            var e = new NodeLabelEditEventArgs(new TreeNode())
            {
                CancelEdit = value
            };
            Assert.Equal(value, e.CancelEdit);
        }
    }
}
