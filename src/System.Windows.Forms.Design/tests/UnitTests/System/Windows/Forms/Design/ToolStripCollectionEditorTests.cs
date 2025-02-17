// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#nullable enable

using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design;

namespace System.Windows.Forms.Design.Tests;

public class ToolStripCollectionEditorTests
{
    private readonly ToolStripCollectionEditor _editor;
    private readonly MockServiceProvider _serviceProvider;
    private readonly MockTypeDescriptorContext _context;

    public ToolStripCollectionEditorTests()
    {
        _editor = new();
        _serviceProvider = new();
        _context = new();
    }

    [Fact]
    public void ToolStripCollectionEditor_EditValue_NullProvider_ReturnsNull()
    {
        object? result = _editor.EditValue(context: null, provider: null, value: new object());

        result.Should().BeNull();
    }

    [Fact]
    public void ToolStripCollectionEditor_EditValue_WithProvider_ReturnsExpected()
    {
        object? result = _editor.EditValue(_context, _serviceProvider, new object());

        result.Should().NotBeNull();
    }

    private class MockServiceProvider : IServiceProvider
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

    private class MockSelectionService : ISelectionService
    {
        public event EventHandler? SelectionChanged;
        public event EventHandler? SelectionChanging;

        public ICollection GetSelectedComponents() => Array.Empty<object>();

        public bool GetComponentSelected(object component) => false;

        public object? PrimarySelection => null;

        public int SelectionCount => 0;

        public void SetSelectedComponents(ICollection? components) { }

        public void SetSelectedComponents(ICollection? components, SelectionTypes selectionType) { }
    }

    private class MockDesignerHost : IDesignerHost
    {
        public IContainer Container => new Container();

        public bool InTransaction => false;

        public IComponent? RootComponent => null;

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

        public event EventHandler? Activated;
        public event EventHandler? Deactivated;
        public event EventHandler? LoadComplete;
        public event DesignerTransactionCloseEventHandler? TransactionClosed;
        public event DesignerTransactionCloseEventHandler? TransactionClosing;
        public event EventHandler? TransactionOpened;
        public event EventHandler? TransactionOpening;
    }

    private class MockTypeDescriptorContext : ITypeDescriptorContext
    {
        public IContainer? Container => null;

        public object? Instance => null;

        public PropertyDescriptor? PropertyDescriptor => null;

        public bool OnComponentChanging() => true;

        public void OnComponentChanged() { }

        public object? GetService(Type serviceType) => null;
    }
}
