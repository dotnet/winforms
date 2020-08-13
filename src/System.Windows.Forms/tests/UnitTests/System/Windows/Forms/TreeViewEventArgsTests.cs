// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using Xunit;

namespace System.Windows.Forms.Tests
{
    // NB: doesn't require thread affinity
    public class TreeViewEventArgsTests : IClassFixture<ThreadExceptionFixture>
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
            var e = new TreeViewEventArgs(node);
            Assert.Equal(node, e.Node);
            Assert.Equal(TreeViewAction.Unknown, e.Action);
        }

        public static IEnumerable<object[]> Ctor_TreeNode_TreeViewAction_TestData()
        {
            yield return new object[] { null, (TreeViewAction)(TreeViewAction.Unknown - 1) };
            yield return new object[] { new TreeNode(), TreeViewAction.ByKeyboard };
        }

        [Theory]
        [MemberData(nameof(Ctor_TreeNode_TreeViewAction_TestData))]
        public void Ctor_TreeNode_TreeViewAction(TreeNode node, TreeViewAction action)
        {
            var e = new TreeViewEventArgs(node, action);
            Assert.Equal(node, e.Node);
            Assert.Equal(action, e.Action);
        }
    }
}
