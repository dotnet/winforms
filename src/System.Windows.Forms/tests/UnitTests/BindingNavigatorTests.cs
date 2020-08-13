// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using Moq;
using Xunit;

namespace System.Windows.Forms.Tests
{
    public class BindingNavigatorTests : IClassFixture<ThreadExceptionFixture>
    {
        [WinFormsFact]
        public void BindingNavigator_Constructor()
        {
            using var bn = new BindingNavigator();

            Assert.NotNull(bn);
        }

        [WinFormsFact]
        public void BindingNavigator_ConstructorBindingSource()
        {
            using var bindingSource = new BindingSource();
            var data = new List<string>() { "Foo", "Bar" };
            bindingSource.DataSource = data;

            using var bn = new BindingNavigator(bindingSource);

            Assert.NotNull(bn);
            Assert.Equal(bindingSource, bn.BindingSource);

            // need more thorough binding source testing
        }

        [WinFormsFact]
        public void BindingNavigator_ConstructorIContainer()
        {
            IContainer nullContainer = null;
            var mockContainer = new Mock<IContainer>(MockBehavior.Strict);
            mockContainer.Setup(x => x.Add(It.IsAny<BindingNavigator>())).Verifiable();

            // act & assert
            ArgumentNullException ex = Assert.Throws<ArgumentNullException>(() => new BindingNavigator(nullContainer));
            Assert.Equal("container", ex.ParamName);

            using var bn = new BindingNavigator(mockContainer.Object);
            Assert.NotNull(bn);
            mockContainer.Verify(x => x.Add(bn));
        }

        [WinFormsFact]
        public void BindingNavigator_ConstructorBool()
        {
            using var bn = new BindingNavigator(true);

            Assert.NotNull(bn);

            Assert.NotNull(bn.PositionItem);
            Assert.NotNull(bn.CountItem);
            Assert.Equal("bindingNavigatorPositionItem", bn.PositionItem.Name);
            Assert.Equal("bindingNavigatorCountItem", bn.CountItem.Name);
            Assert.Equal(SR.BindingNavigatorCountItemTip, bn.CountItem.ToolTipText);
            Assert.Equal(SR.BindingNavigatorPositionItemTip, bn.PositionItem.ToolTipText);
            Assert.False(bn.CountItem.AutoToolTip);
            Assert.False(bn.PositionItem.AutoToolTip);
            Assert.Equal(SR.BindingNavigatorPositionAccessibleName, bn.PositionItem.AccessibleName);

            var items = new List<ToolStripItem>()
            {
                bn.MoveFirstItem,
                bn.MovePreviousItem,
                bn.MoveNextItem,
                bn.MoveLastItem,
                bn.AddNewItem,
                bn.DeleteItem
            };

            var itemNames = new List<string>()
            {
                "bindingNavigatorMoveFirstItem",
                "bindingNavigatorMovePreviousItem",
                "bindingNavigatorMoveNextItem",
                "bindingNavigatorMoveLastItem",
                "bindingNavigatorAddNewItem",
                "bindingNavigatorDeleteItem"
            };

            var itemTexts = new List<string>()
            {
                SR.BindingNavigatorMoveFirstItemText,
                SR.BindingNavigatorMovePreviousItemText,
                SR.BindingNavigatorMoveNextItemText,
                SR.BindingNavigatorMoveLastItemText,
                SR.BindingNavigatorAddNewItemText,
                SR.BindingNavigatorDeleteItemText
            };

            for (var i = 0; i < items.Count; i++)
            {
                ToolStripItem item = items[i];
                Assert.NotNull(item);
                Assert.Equal(itemNames[i], item.Name.Trim());
                Assert.Equal(itemTexts[i], item.Text.Trim());
                Assert.NotNull(item.Image);
                Assert.True(item.RightToLeftAutoMirrorImage);
                Assert.Equal(ToolStripItemDisplayStyle.Image, item.DisplayStyle);
            }

            Assert.False(bn.PositionItem.AutoSize);
            Assert.Equal(50, bn.PositionItem.Width);

            var index = 0;
            Assert.Equal(11, bn.Items.Count);
            Assert.Equal(bn.MoveFirstItem, bn.Items[index++]);
            Assert.Equal(bn.MovePreviousItem, bn.Items[index++]);
            Assert.NotNull(bn.Items[index]);
            Assert.IsType<ToolStripSeparator>(bn.Items[index++]);
            Assert.Equal(bn.PositionItem, bn.Items[index++]);
            Assert.Equal(bn.CountItem, bn.Items[index++]);
            Assert.NotNull(bn.Items[index]);
            Assert.IsType<ToolStripSeparator>(bn.Items[index++]);
            Assert.Equal(bn.MoveNextItem, bn.Items[index++]);
            Assert.Equal(bn.MoveLastItem, bn.Items[index++]);
            Assert.NotNull(bn.Items[index]);
            Assert.IsType<ToolStripSeparator>(bn.Items[index++]);
            Assert.Equal(bn.AddNewItem, bn.Items[index++]);
            Assert.Equal(bn.DeleteItem, bn.Items[index++]);
        }
    }
}
