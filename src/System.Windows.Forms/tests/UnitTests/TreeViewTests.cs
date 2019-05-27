// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Xunit;

namespace System.Windows.Forms.Tests
{
    public class TreeViewTests
    {
        [Fact]
        public void TreeView_Constructor()
        {
            var tv = new TreeView();

            Assert.NotNull(tv);
            Assert.NotNull(tv.root);
            Assert.NotNull(tv.SelectedImageIndexer);
            Assert.Equal(0, tv.SelectedImageIndexer.Index);
            Assert.NotNull(tv.ImageIndexer);
            Assert.Equal(0, tv.ImageIndexer.Index);
        }

        [Fact]
        public void TreeView_AddExistingNodeAsChild_ThrowsArgumentException()
        {
            var tv = new TreeView();
            var tn1 = new TreeNode();
            tv.Nodes.Add(tn1);

            Assert.Throws<ArgumentException>(() => tn1.Nodes.Add(tn1));
        }
    }
}
