// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using Xunit;

namespace System.Windows.Forms.Tests
{
    public class TreeNodeMouseHoverEventArgsTests
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
            var e = new TreeNodeMouseHoverEventArgs(node);
            Assert.Equal(node, e.Node);
        }
    }
}
