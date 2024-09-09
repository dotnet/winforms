// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;
using System.Data;
using Moq;

namespace System.Windows.Forms.Tests;

public class BindingNavigatorTests
{
    [WinFormsFact]
    public void BindingNavigator_Constructor()
    {
        using BindingNavigator bn = new();

        Assert.NotNull(bn);
    }

    [WinFormsFact]
    public void BindingNavigator_ConstructorBindingSource()
    {
        using BindingSource bindingSource = [];
        List<string> data = ["Foo", "Bar"];
        bindingSource.DataSource = data;

        using BindingNavigator bn = new(bindingSource);

        Assert.NotNull(bn);
        Assert.Equal(bindingSource, bn.BindingSource);

        // need more thorough binding source testing
    }

    [WinFormsFact]
    public void BindingNavigator_ConstructorIContainer()
    {
        IContainer nullContainer = null;
        Mock<IContainer> mockContainer = new(MockBehavior.Strict);
        mockContainer.Setup(x => x.Add(It.IsAny<BindingNavigator>())).Verifiable();

        // act & assert
        ArgumentNullException ex = Assert.Throws<ArgumentNullException>(() => new BindingNavigator(nullContainer));
        Assert.Equal("container", ex.ParamName);

        using BindingNavigator bn = new(mockContainer.Object);
        Assert.NotNull(bn);
        mockContainer.Verify(x => x.Add(bn));
    }

    [WinFormsFact]
    public void BindingNavigator_ConstructorBool()
    {
        using BindingNavigator bn = new(true);

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

        List<ToolStripItem> items =
        [
            bn.MoveFirstItem,
            bn.MovePreviousItem,
            bn.MoveNextItem,
            bn.MoveLastItem,
            bn.AddNewItem,
            bn.DeleteItem
        ];

        List<string> itemNames =
        [
            "bindingNavigatorMoveFirstItem",
            "bindingNavigatorMovePreviousItem",
            "bindingNavigatorMoveNextItem",
            "bindingNavigatorMoveLastItem",
            "bindingNavigatorAddNewItem",
            "bindingNavigatorDeleteItem"
        ];

        List<string> itemTexts =
        [
            SR.BindingNavigatorMoveFirstItemText,
            SR.BindingNavigatorMovePreviousItemText,
            SR.BindingNavigatorMoveNextItemText,
            SR.BindingNavigatorMoveLastItemText,
            SR.BindingNavigatorAddNewItemText,
            SR.BindingNavigatorDeleteItemText
        ];

        for (int i = 0; i < items.Count; i++)
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

        int index = 0;
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

    [WinFormsFact]
    public void BindingNavigator_UpdatesItsItems_AfterDataSourceDisposing()
    {
        using BindingNavigator control = new(true);
        int rowsCount = 5;
        BindingSource bindingSource = GetTestBindingSource(rowsCount);
        control.BindingSource = bindingSource;

        Assert.Equal("1", control.PositionItem.Text);
        Assert.Equal($"of {rowsCount}", control.CountItem.Text);

        bindingSource.Dispose();

        // The BindingNavigator updates its PositionItem and CountItem values
        // after its BindingSource is disposed
        Assert.Equal("0", control.PositionItem.Text);
        Assert.Equal("of 0", control.CountItem.Text);
    }

    [WinFormsFact]
    public void BindingNavigator_BindingSource_IsNull_AfterDisposing()
    {
        using BindingNavigator control = new();
        BindingSource bindingSource = GetTestBindingSource(5);
        control.BindingSource = bindingSource;

        Assert.Equal(bindingSource, control.BindingSource);

        bindingSource.Dispose();

        Assert.Null(control.BindingSource);
    }

    [WinFormsFact]
    public void BindingNavigator_BindingSource_IsActual_AfterOldOneIsDisposed()
    {
        using BindingNavigator control = new(true);
        int rowsCount1 = 3;
        BindingSource bindingSource1 = GetTestBindingSource(rowsCount1);
        int rowsCount2 = 5;
        BindingSource bindingSource2 = GetTestBindingSource(rowsCount2);
        control.BindingSource = bindingSource1;

        Assert.Equal(bindingSource1, control.BindingSource);
        Assert.Equal("1", control.PositionItem.Text);
        Assert.Equal($"of {rowsCount1}", control.CountItem.Text);

        control.BindingSource = bindingSource2;

        Assert.Equal(bindingSource2, control.BindingSource);
        Assert.Equal("1", control.PositionItem.Text);
        Assert.Equal($"of {rowsCount2}", control.CountItem.Text);

        bindingSource1.Dispose();

        // bindingSource2 is actual for the BindingNavigator
        // so it will contain correct PositionItem and CountItem values
        // even after bindingSource1 is disposed.
        // This test checks that Disposed events unsubscribed correctly
        Assert.Equal(bindingSource2, control.BindingSource);
        Assert.Equal("1", control.PositionItem.Text);
        Assert.Equal($"of {rowsCount2}", control.CountItem.Text);
    }

    [WinFormsFact]
    public void BindingNavigator_AddStandardItems_AddsCorrectItems()
    {
        using BindingNavigator navigator = new();
        navigator.AddStandardItems();
        navigator.Items[0].Name.Should().Be("bindingNavigatorMoveFirstItem");
        navigator.Items[1].Name.Should().Be("bindingNavigatorMovePreviousItem");
        navigator.Items[2].Name.Should().Be("bindingNavigatorSeparator1");
        navigator.Items[3].Name.Should().Be("bindingNavigatorPositionItem");
        navigator.Items[4].Name.Should().Be("bindingNavigatorCountItem");
        navigator.Items[5].Name.Should().Be("bindingNavigatorSeparator2");
        navigator.Items[6].Name.Should().Be("bindingNavigatorMoveNextItem");
        navigator.Items[7].Name.Should().Be("bindingNavigatorMoveLastItem");
        navigator.Items[8].Name.Should().Be("bindingNavigatorSeparator3");
        navigator.Items[9].Name.Should().Be("bindingNavigatorAddNewItem");
        navigator.Items[10].Name.Should().Be("bindingNavigatorDeleteItem");
    }

    private BindingSource GetTestBindingSource(int rowsCount)
    {
        DataTable dt = new();
        dt.Columns.Add("Name");
        dt.Columns.Add("Age");

        for (int i = 0; i < rowsCount; i++)
        {
            DataRow dr = dt.NewRow();
            dr[0] = $"User{i}";
            dr[1] = i * 3;
            dt.Rows.Add(dr);
        }

        return new() { DataSource = dt };
    }
}
