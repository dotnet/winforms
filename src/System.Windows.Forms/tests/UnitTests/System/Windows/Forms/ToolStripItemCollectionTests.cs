// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using WinForms.Common.Tests;
using Xunit;

namespace System.Windows.Forms.Tests
{
    public class ToolStripItemCollectionTests : IClassFixture<ThreadExceptionFixture>
    {
        [WinFormsTheory]
        [InlineData("name2")]
        [InlineData("NAME2")]
        public void ToolStripItemCollection_Find_InvokeKeyExists_ReturnsExpected(string key)
        {
            using ToolStripMenuItem toolStrip = new ToolStripMenuItem();

            var child1 = new ToolStripMenuItem
            {
                Name = "name1"
            };
            var child2 = new ToolStripMenuItem
            {
                Name = "name2"
            };
            var child3 = new ToolStripMenuItem
            {
                Name = "name2"
            };

            var grandchild1 = new ToolStripMenuItem
            {
                Name = "name1"
            };
            var grandchild2 = new ToolStripMenuItem
            {
                Name = "name2"
            };
            var grandchild3 = new ToolStripMenuItem
            {
                Name = "name2"
            };
            child3.DropDownItems.Add(grandchild1);
            child3.DropDownItems.Add(grandchild2);
            child3.DropDownItems.Add(grandchild3);
            ToolStripItemCollection collection = toolStrip.DropDownItems;
            collection.Add(child1);
            collection.Add(child2);
            collection.Add(child3);

            // Search all children.
            Assert.Equal(new ToolStripMenuItem[] { child2, child3, grandchild2, grandchild3 }, collection.Find(key, searchAllChildren: true));

            // Call again.
            Assert.Equal(new ToolStripMenuItem[] { child2, child3, grandchild2, grandchild3 }, collection.Find(key, searchAllChildren: true));

            // Don't search all children.
            Assert.Equal(new ToolStripMenuItem[] { child2, child3 }, collection.Find(key, searchAllChildren: false));

            // Call again.
            Assert.Equal(new ToolStripMenuItem[] { child2, child3 }, collection.Find(key, searchAllChildren: false));
        }

        [WinFormsTheory]
        [InlineData("NoSuchName")]
        [InlineData("abcd")]
        [InlineData("abcde")]
        [InlineData("abcdef")]
        public void ToolStripItemCollection_Find_InvokeNoSuchKey_ReturnsEmpty(string key)
        {
            using ToolStripMenuItem toolStrip = new ToolStripMenuItem();

            var child1 = new ToolStripMenuItem()
            {
                Name = "name1"
            };
            var child2 = new ToolStripMenuItem()
            {
                Name = "name2"
            };
            var child3 = new ToolStripMenuItem()
            {
                Name = "name2"
            };
            var collection = toolStrip.DropDown.DisplayedItems;
            collection.Add(child1);
            collection.Add(child2);
            collection.Add(child3);

            Assert.Empty(collection.Find(key, searchAllChildren: true));
            Assert.Empty(collection.Find(key, searchAllChildren: false));
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetNullOrEmptyStringTheoryData))]
        public void ToolStripItemCollection_Find_NullOrEmptyKey_ThrowsArgumentNullException(string key)
        {
            using ToolStripMenuItem toolStrip = new ToolStripMenuItem();
            var collection = toolStrip.DropDown.DisplayedItems;
            Assert.Throws<ArgumentNullException>("key", () => collection.Find(key, searchAllChildren: true));
            Assert.Throws<ArgumentNullException>("key", () => collection.Find(key, searchAllChildren: false));
        }
    }
}
