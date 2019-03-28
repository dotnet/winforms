// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel.Design;
using WinForms.Common.Tests;
using Xunit;

namespace System.Windows.Forms.Design.Tests
{
    public class DesignerActionListCollectionTests
    {
        [Fact]
        public void DesignerActionListCollection_Constructor()
        {
            DesignerActionListCollection underTest = new DesignerActionListCollection();
            Assert.NotNull(underTest);
        }

        [Fact]
        public void DesignerActionListCollection_Constructor_DesignerActionList()
        {
            DesignerActionListCollection underTest = CreateNewCollectionOfItems();
            Assert.NotNull(underTest);
        }

        [Fact]
        public void DesignerActionItemCollection_Add_AddRange()
        {
            DesignerActionListCollection underTest = CreateNewCollectionOfItems(5);
            Assert.Equal(5, underTest.Count);

            Button button = new Button();
            DesignerActionList list = new DesignerActionList(button);
            underTest.Add(list);
            Assert.Equal(6, underTest.Count);

            DesignerActionListCollection other = CreateNewCollectionOfItems(3);
            underTest.AddRange(other);
            Assert.Equal(9, underTest.Count);
        }


        [Fact]
        public void DesignerActionItemCollection_Insert_Contains_IndexOf()
        {
            DesignerActionListCollection underTest = CreateNewCollectionOfItems(5);

            Button button = new Button();
            DesignerActionList list = new DesignerActionList(button);
            underTest.Insert(3, list);
            Assert.True(underTest.Contains(list));
            Assert.Equal(3, underTest.IndexOf(list));
        }

        [Fact]
        public void DesignerActionItemCollection_Remove()
        {
            DesignerActionListCollection underTest = CreateNewCollectionOfItems(5);

            Button button = new Button();
            DesignerActionList list = new DesignerActionList(button);
            underTest.Insert(3, list);
            underTest.Remove(list);
            Assert.False(underTest.Contains(list));
            Assert.Equal(5, underTest.Count);
        }

        private DesignerActionListCollection CreateNewCollectionOfItems(int numberOfItems = 1)
        {
            Button button = new Button();
            DesignerActionList[] list = new DesignerActionList[] { new DesignerActionList(button) };
            DesignerActionListCollection underTest = new DesignerActionListCollection(list);

            for (int i = 1; i < numberOfItems; i++)
            {
                underTest.Add(new DesignerActionList(button));
            }

            return underTest;
        }
    }
}
