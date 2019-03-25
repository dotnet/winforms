// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel.Design;
using WinForms.Common.Tests;
using Xunit;

namespace System.Windows.Forms.Design.Tests
{
    public class DesignerActionItemCollectionTests
    {
        [Fact]
        public void DesignerActionItemCollection_Constructor()
        {
            DesignerActionItemCollection underTest = new DesignerActionItemCollection();
            Assert.NotNull(underTest);
        }

        [Fact]
        public void DesignerActionItemCollection_Add_Contains_IndexOf()
        {
            DesignerActionItemCollection underTest = new DesignerActionItemCollection();

            DesignerActionItem item1 = new DesignerActionItemTest("name", "category", "description");
            underTest.Add(item1);
            Assert.True(underTest.Contains(item1));
            Assert.Equal(0, underTest.IndexOf(item1));

            DesignerActionItem item2 = new DesignerActionItemTest("name1", "category1", "description1");
            underTest.Add(item2);
            Assert.True(underTest.Contains(item2));
            Assert.Equal(1, underTest.IndexOf(item2));
        }

        [Fact]
        public void DesignerActionItemCollection_Insert_Remove_Count()
        {
            DesignerActionItemCollection underTest = new DesignerActionItemCollection();
            DesignerActionItem item1 = new DesignerActionItemTest("name", "category", "description");
            DesignerActionItem item2 = new DesignerActionItemTest("name1", "category1", "description1");
            DesignerActionItem item3 = new DesignerActionItemTest("name2", "category2", "description2");
            DesignerActionItem item4 = new DesignerActionItemTest("name3", "category3", "description3");

            underTest.Add(item1);
            underTest.Add(item2);

            underTest.Add(item3);
            Assert.Equal(2, underTest.IndexOf(item3));

            underTest.Insert(2, item4);
            Assert.Equal(2, underTest.IndexOf(item4));

            underTest.Remove(item4);
            Assert.False(underTest.Contains(item4));
            Assert.Equal(3, underTest.Count);
        }

        private class DesignerActionItemTest : DesignerActionItem
        {
            public DesignerActionItemTest(string displayName, string category, string description) : base(displayName, category, description)
            {
            }
        }
    }
}
