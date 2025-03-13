// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Collections;
using System.ComponentModel.Design;
using System.Drawing;
using Moq;

namespace System.Windows.Forms.Design.Tests;

public class ToolStripDesignerUtilsTests : IDisposable
{
    private readonly ToolStrip _toolStrip;
    private readonly ToolStripButton _toolStripButton;

    public ToolStripDesignerUtilsTests()
    {
        _toolStrip = new();
        _toolStripButton = new();
    }

    public void Dispose()
    {
        _toolStrip.Dispose();
        _toolStripButton.Dispose();
    }

    [Fact]
    public void GetAdjustedBounds_AdjustsBoundsCorrectly()
    {
        Rectangle rectangle = new(10, 10, 100, 100);

        ToolStripDesignerUtils.GetAdjustedBounds(_toolStripButton, ref rectangle);

        rectangle.Should().Be(new Rectangle(11, 11, 98, 98));
    }

    [Fact]
    public void GetToolboxBitmap_ReturnsBitmap()
    {
        Type itemType = typeof(ToolStripButton);

        Bitmap bitmap = ToolStripDesignerUtils.GetToolboxBitmap(itemType);

        bitmap.Should().NotBeNull();
    }

    [Fact]
    public void GetToolboxDescription_ReturnsDescription()
    {
        Type itemType = typeof(ToolStripButton);

        string description = ToolStripDesignerUtils.GetToolboxDescription(itemType);

        description.Should().Be("Button");
    }

    [Fact]
    public void GetStandardItemTypes_ReturnsExpectedTypes()
    {
        Type[] types = ToolStripDesignerUtils.GetStandardItemTypes(_toolStrip);

        types.Should().Contain(new[] { typeof(ToolStripButton), typeof(ToolStripLabel) });
    }

    [Fact]
    public void GetCustomItemTypes_WithServiceProvider_ReturnsExpectedTypes()
    {
        Mock<IServiceProvider> serviceProvider = new();
        Mock<ITypeDiscoveryService> discoveryService = new();

        serviceProvider.Setup(sp => sp.GetService(typeof(ITypeDiscoveryService))).Returns(discoveryService.Object);
        discoveryService.Setup(ds => ds.GetTypes(It.IsAny<Type>(), It.IsAny<bool>())).Returns(new[] { typeof(Button), typeof(ToolStripLabel) });

        Type[] types = ToolStripDesignerUtils.GetCustomItemTypes(_toolStrip, serviceProvider.Object);

        types.Should().NotBeNull();
        types.Should().BeEmpty();
    }

    [Fact]
    public void GetCustomItemTypes_WithDiscoveryService_ReturnsExpectedTypes()
    {
        Mock<ITypeDiscoveryService> discoveryService = new();

        discoveryService.Setup(ds => ds.GetTypes(It.IsAny<Type>(), It.IsAny<bool>())).Returns(new[] { typeof(Button), typeof(ToolStripLabel) });

        Type[] types = ToolStripDesignerUtils.GetCustomItemTypes(_toolStrip, discoveryService.Object);

        types.Should().NotBeNull();
        types.Should().BeEmpty();
    }

    [Fact]
    public void GetCustomItemTypes_WithNullServiceProvider_ReturnsEmptyArray()
    {
        IServiceProvider? serviceProvider = null;
        Type[] types = ToolStripDesignerUtils.GetCustomItemTypes(_toolStrip, serviceProvider);

        types.Should().NotBeNull();
        types.Should().BeEmpty();
    }

    [Fact]
    public void GetCustomItemTypes_WithNullDiscoveryService_ReturnsEmptyArray()
    {
        ITypeDiscoveryService? discoveryService = null;
        Type[] types = ToolStripDesignerUtils.GetCustomItemTypes(_toolStrip, discoveryService);

        types.Should().NotBeNull();
        types.Should().BeEmpty();
    }

    [Fact]
    public void GetStandardItemMenuItems_ReturnsExpectedItems()
    {
        EventHandler onClick = (sender, e) => { };

        ToolStripItem[] items = ToolStripDesignerUtils.GetStandardItemMenuItems(_toolStrip, onClick, false);

        items.Should().NotBeNull();
    }

    [Fact]
    public void GetCustomItemMenuItems_ReturnsExpectedItems()
    {
        EventHandler onClick = (sender, e) => { };
        Mock<IServiceProvider> mockServiceProvider = new();

        ToolStripItem[] items = ToolStripDesignerUtils.GetCustomItemMenuItems(_toolStrip, onClick, false, mockServiceProvider.Object);

        items.Should().NotBeNull();
    }

    [Fact]
    public void GetNewItemDropDown_ReturnsExpectedDropDown()
    {
        EventHandler onClick = (sender, e) => { };
        Mock<IServiceProvider> mockServiceProvider = new();

        ToolStripDropDown dropDown = ToolStripDesignerUtils.GetNewItemDropDown(_toolStrip, _toolStripButton, onClick, false, mockServiceProvider.Object, true);

        dropDown.Should().NotBeNull();
    }

    [Fact]
    public void InvalidateSelection_InvokesInvalidate()
    {
        ArrayList originalSelComps = [_toolStripButton];
        using ToolStripButton nextSelection = new();
        Mock<IServiceProvider> mockServiceProvider = new();

        Action act = () => ToolStripDesignerUtils.InvalidateSelection(originalSelComps, nextSelection, mockServiceProvider.Object, false);

        act.Should().NotThrow();
    }
}
