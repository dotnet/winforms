// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#nullable disable

using System.ComponentModel;
using Moq;

namespace System.Windows.Forms.Tests;

public class ContextMenuStripTests
{
    [WinFormsFact]
    public void ContextMenuStrip_Constructor()
    {
        using ContextMenuStrip cms = new();

        Assert.NotNull(cms);
    }

    [WinFormsFact]
    public void ContextMenuStrip_ConstructorIContainer()
    {
        IContainer nullContainer = null;
        Mock<IContainer> mockContainer = new(MockBehavior.Strict);
        mockContainer.Setup(x => x.Add(It.IsAny<ContextMenuStrip>())).Verifiable();

        // act & assert
        ArgumentNullException ex = Assert.Throws<ArgumentNullException>(() => new ContextMenuStrip(nullContainer));
        Assert.Equal("container", ex.ParamName);

        using ContextMenuStrip cms = new(mockContainer.Object);
        Assert.NotNull(cms);
        mockContainer.Verify(x => x.Add(cms));
    }

    [WinFormsFact]
    public void ContextMenuStrip_ShowInternal_KeyboardActivated_SelectsFirstItem()
    {
        using Control control = new();
        control.CreateControl();
        using ContextMenuStrip cms = new();
        cms.Items.Add("First");
        cms.Items.Add("Second");

        cms.ShowInternal(control, new Drawing.Point(1, 1), isKeyboardActivated: true);

        try
        {
            Assert.True(cms.Visible);
            Assert.True(cms.Items[0].Selected);
            Assert.False(cms.Items[1].Selected);
        }
        finally
        {
            cms.Close();
        }
    }

    [WinFormsFact]
    public void ContextMenuStrip_ShowInternal_KeyboardActivated_SkipsSeparator()
    {
        using Control control = new();
        control.CreateControl();
        using ContextMenuStrip cms = new();
        cms.Items.Add(new ToolStripSeparator());
        cms.Items.Add("Second");

        cms.ShowInternal(control, new Drawing.Point(1, 1), isKeyboardActivated: true);

        try
        {
            Assert.True(cms.Items[1].Selected);
        }
        finally
        {
            cms.Close();
        }
    }

    [WinFormsFact]
    public void ContextMenuStrip_ShowInternal_NotKeyboardActivated_DoesNotSelectAnItem()
    {
        using Control control = new();
        control.CreateControl();
        using ContextMenuStrip cms = new();
        cms.Items.Add("First");
        cms.Items.Add("Second");

        cms.ShowInternal(control, new Drawing.Point(1, 1), isKeyboardActivated: false);

        try
        {
            Assert.True(cms.Visible);
            Assert.False(cms.Items[0].Selected);
            Assert.False(cms.Items[1].Selected);
        }
        finally
        {
            cms.Close();
        }
    }

    [WinFormsFact]
    public void ContextMenuStrip_ShowInternal_KeyboardActivated_NoItems_DoesNotThrow()
    {
        using Control control = new();
        control.CreateControl();
        using ContextMenuStrip cms = new();

        cms.ShowInternal(control, new Drawing.Point(1, 1), isKeyboardActivated: true);

        Assert.False(cms.Visible);
    }

    [WinFormsFact]
    public void ContextMenuStrip_SelectItem_AccessibleObjectOfMenuCreated_CreatesAccessibleObjectOfItem()
    {
        // The items of a context menu are on a drop-down without an owner item. Whether to raise the focus event
        // of an item has to be decided from the accessible object of the menu, there is no owner item to ask.
        using Control control = new();
        control.CreateControl();
        using ContextMenuStrip cms = new();
        ToolStripItem item = cms.Items.Add("First");
        Assert.NotNull(cms.AccessibilityObject);
        Assert.False(item.IsAccessibilityObjectCreated);

        cms.ShowInternal(control, new Drawing.Point(1, 1), isKeyboardActivated: true);

        try
        {
            Assert.True(item.Selected);
            Assert.True(item.IsAccessibilityObjectCreated);
        }
        finally
        {
            cms.Close();
        }
    }
}
