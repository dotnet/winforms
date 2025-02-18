// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#nullable enable

using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;

namespace System.Windows.Forms.Design.Tests;

public class ToolStripDesignerUtilsTests : IDisposable
{
    private readonly ToolStrip _toolStrip;
    private readonly ToolStripButton _toolStripButton;
    private readonly MockServiceProvider _serviceProvider;

    public ToolStripDesignerUtilsTests()
    {
        _toolStrip = new();
        _toolStripButton = new();
        _serviceProvider = new();
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
        var itemType = typeof(ToolStripButton);

        Bitmap bitmap = ToolStripDesignerUtils.GetToolboxBitmap(itemType);

        bitmap.Should().NotBeNull();
    }

    [Fact]
    public void GetToolboxDescription_ReturnsDescription()
    {
        var itemType = typeof(ToolStripButton);

        var description = ToolStripDesignerUtils.GetToolboxDescription(itemType);

        description.Should().Be("Button");
    }

    [Fact]
    public void GetStandardItemTypes_ReturnsExpectedTypes()
    {
        var types = ToolStripDesignerUtils.GetStandardItemTypes(_toolStrip);

        types.Should().Contain(new[] { typeof(ToolStripButton), typeof(ToolStripLabel) });
    }

    [Fact]
    public void GetCustomItemTypes_ReturnsExpectedTypes()
    {
        var types = ToolStripDesignerUtils.GetCustomItemTypes(_toolStrip, _serviceProvider);

        types.Should().NotBeNull();
    }

    [Fact]
    public void GetStandardItemMenuItems_ReturnsExpectedItems()
    {
        EventHandler onClick = (sender, e) => { };

        var items = ToolStripDesignerUtils.GetStandardItemMenuItems(_toolStrip, onClick, false);

        items.Should().NotBeNull();
    }

    [Fact]
    public void GetCustomItemMenuItems_ReturnsExpectedItems()
    {
        EventHandler onClick = (sender, e) => { };

        var items = ToolStripDesignerUtils.GetCustomItemMenuItems(_toolStrip, onClick, false, _serviceProvider);

        items.Should().NotBeNull();
    }

    [Fact]
    public void GetNewItemDropDown_ReturnsExpectedDropDown()
    {
        EventHandler onClick = (sender, e) => { };

        var dropDown = ToolStripDesignerUtils.GetNewItemDropDown(_toolStrip, _toolStripButton, onClick, false, _serviceProvider, true);

        dropDown.Should().NotBeNull();
    }

    [Fact]
    public void InvalidateSelection_InvokesInvalidate()
    {
        var originalSelComps = new ArrayList { _toolStripButton };
        using ToolStripButton nextSelection = new();

        Action act = () => ToolStripDesignerUtils.InvalidateSelection(originalSelComps, nextSelection, _serviceProvider, false);

        act.Should().NotThrow();
    }
}

internal class MockServiceProvider : IServiceProvider
{
    public object? GetService(Type serviceType)
    {
        if (serviceType == typeof(ISelectionService))
        {
            return new MockSelectionService();
        }

        if (serviceType == typeof(IDesignerHost))
        {
            return new MockDesignerHost();
        }

        return null;
    }
}

internal class MockSelectionService : ISelectionService
{
    public event EventHandler? SelectionChanged
    {
        add { }
        remove { }
    }

    public event EventHandler? SelectionChanging
    {
        add { }
        remove { }
    }

    public ICollection GetSelectedComponents() => Array.Empty<object>();

    public bool GetComponentSelected(object component) => false;

    public object? PrimarySelection => null;

    public int SelectionCount => 0;

    public void SetSelectedComponents(ICollection? components) { }

    public void SetSelectedComponents(ICollection? components, SelectionTypes selectionType) { }
}

internal class MockDesignerHost : IDesignerHost
{
    public IContainer Container => new Container();

    public bool InTransaction => false;

    public IComponent RootComponent => null!;

    public string TransactionDescription => string.Empty;

    public bool Loading => false;

    public string RootComponentClassName => string.Empty;

    public void Activate() { }

    public IComponent CreateComponent(Type componentClass) => throw new NotImplementedException();

    public IComponent CreateComponent(Type componentClass, string name) => throw new NotImplementedException();

    public DesignerTransaction CreateTransaction() => throw new NotImplementedException();

    public DesignerTransaction CreateTransaction(string description) => throw new NotImplementedException();

    public void DestroyComponent(IComponent component) { }

    public IDesigner? GetDesigner(IComponent component) => null;

    public Type GetType(string typeName) => throw new NotImplementedException();

    public void AddService(Type serviceType, ServiceCreatorCallback callback) { }

    public void AddService(Type serviceType, ServiceCreatorCallback callback, bool promote) { }

    public void AddService(Type serviceType, object serviceInstance) { }

    public void AddService(Type serviceType, object serviceInstance, bool promote) { }

    public void RemoveService(Type serviceType) { }

    public void RemoveService(Type serviceType, bool promote) { }

    public object? GetService(Type serviceType) => null;

    public event EventHandler? Activated
    {
        add { }
        remove { }
    }

    public event EventHandler? Deactivated
    {
        add { }
        remove { }
    }

    public event EventHandler? LoadComplete
    {
        add { }
        remove { }
    }

    public event DesignerTransactionCloseEventHandler? TransactionClosed
    {
        add { }
        remove { }
    }

    public event DesignerTransactionCloseEventHandler? TransactionClosing
    {
        add { }
        remove { }
    }

    public event EventHandler? TransactionOpened
    {
        add { }
        remove { }
    }

    public event EventHandler? TransactionOpening
    {
        add { }
        remove { }
    }
}
