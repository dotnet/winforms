// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using Xunit;

namespace System.Windows.Forms.Tests
{
    public class TreeViewCancelEventArgsTests
    {
        public static IEnumerable<object[]> Ctor_TreeNode_Bool_TreeViewAction_TestData()
        {
            yield return new object[] { null, false, (TreeViewAction)(TreeViewAction.Unknown -1) };
            yield return new object[] { new TreeNode(), true, TreeViewAction.ByKeyboard };
        }

        [Theory]
        [MemberData(nameof(Ctor_TreeNode_Bool_TreeViewAction_TestData))]
        public void Ctor_TreeNode_Bool_TreeViewAction(TreeNode node, bool cancel, TreeViewAction action)
        {
            var e = new TreeViewCancelEventArgs(node, cancel, action);
            Assert.Equal(node, e.Node);
            Assert.Equal(cancel, e.Cancel);
            Assert.Equal(action, e.Action);
        }
    }
}
