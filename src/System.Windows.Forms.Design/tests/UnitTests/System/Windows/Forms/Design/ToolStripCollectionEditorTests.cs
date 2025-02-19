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

    public ToolStripCollectionEditorTests()
    {
        _editor = new();
    }

    [Fact]
    public void ToolStripCollectionEditor_CreateCollectionForm_DoesNotThrowException()
    {
        Action act = () => _editor.TestAccessor().Dynamic.CreateCollectionForm();
        act.Should().NotThrow();
    }

    [Fact]
    public void ToolStripCollectionEditor_HelpTopic_ReturnsExpectedValue()
    {
        string helpTopic = _editor.TestAccessor().Dynamic.HelpTopic;
        helpTopic.Should().Be("net.ComponentModel.ToolStripCollectionEditor");
    }

    [Fact]
    public void ToolStripCollectionEditor_EditValue_NullProvider_ReturnsNull()
    {
        object? result = _editor.EditValue(context: null, provider: null!, value: new object());

        result.Should().BeNull();
    }

    [Fact]
    public void ToolStripCollectionEditor_EditValue_WithProvider_ReturnsExpected()
    {
        object? result = _editor.EditValue(new MockTypeDescriptorContext(), new MockServiceProvider(), new object());

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

    private class MockDesignerHost : IDesignerHost
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
