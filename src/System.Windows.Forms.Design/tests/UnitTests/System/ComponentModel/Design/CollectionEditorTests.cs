// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Collections;
using System.Drawing.Design;
using System.Reflection;
using System.Windows.Forms.Design;
using Moq;
using Moq.Protected;
using System.Windows.Forms.TestUtilities;

namespace System.ComponentModel.Design.Tests;

public class CollectionEditorTests
{
    [Theory]
    [InlineData(typeof(object), typeof(object))]
    [InlineData(typeof(string), typeof(object))]
    [InlineData(typeof(int[]), typeof(object))]
    [InlineData(typeof(IList<int>), typeof(int))]
    [InlineData(typeof(IList), typeof(object))]
    [InlineData(typeof(ClassWithItem), typeof(int))]
    [InlineData(typeof(ClassWithPrivateItem), typeof(object))]
    [InlineData(typeof(ClassWithStaticItem), typeof(object))]
    [InlineData(typeof(ClassWithItems), typeof(int))]
    [InlineData(typeof(ClassWithPrivateItems), typeof(object))]
    [InlineData(typeof(ClassWithStaticItems), typeof(object))]
    public void CollectionEditor_Ctor_Type(Type type, Type expectedItemType)
    {
        SubCollectionEditor editor = new(type);
        Assert.Equal(expectedItemType, editor.CollectionItemType);
        Assert.Same(editor.CollectionItemType, editor.CollectionItemType);
        Assert.Equal(type, editor.CollectionType);
        Assert.Null(editor.Context);
        Assert.Equal("net.ComponentModel.CollectionEditor", editor.HelpTopic);
        Assert.False(editor.IsDropDownResizable);
        Assert.Equal([expectedItemType], editor.NewItemTypes);
    }

    [Fact]
    public void CollectionEditor_Ctor_NullType()
    {
        SubCollectionEditor editor = new(null);
        Assert.Throws<ArgumentNullException>("type", () => editor.CollectionItemType);
        Assert.Null(editor.CollectionType);
        Assert.Null(editor.Context);
        Assert.Equal("net.ComponentModel.CollectionEditor", editor.HelpTopic);
        Assert.False(editor.IsDropDownResizable);
        Assert.Throws<ArgumentNullException>("type", () => editor.NewItemTypes);
    }

    [Fact]
    public void CollectionEditor_CollectionEditor_CancelChanges_Invoke_Nop()
    {
        SubCollectionEditor editor = new(null);
        editor.CancelChanges();
    }

    public static IEnumerable<object[]> CanRemoveInstance_TestData()
    {
        yield return new object[] { "some string" };
        yield return new object[] { 123 };
        yield return new object[] { null };
        yield return new object[] { new Component() };
    }

    [Theory]
    [MemberData(nameof(CanRemoveInstance_TestData))]
    public void CollectionEditor_CanRemoveInstance_Invoke_ReturnsExpected(object value)
    {
        SubCollectionEditor editor = new(null);
        Assert.True(editor.CanRemoveInstance(value));
    }

    public static IEnumerable<object[]> CanRemoveInstance_InheritanceAttribute_TestData()
    {
        yield return new object[] { new InheritanceAttribute(InheritanceLevel.Inherited - 1), false };
        yield return new object[] { new InheritanceAttribute(InheritanceLevel.Inherited), false };
        yield return new object[] { new InheritanceAttribute(InheritanceLevel.InheritedReadOnly), false };
        yield return new object[] { new InheritanceAttribute(InheritanceLevel.NotInherited), true };
        yield return new object[] { new InheritanceAttribute(InheritanceLevel.NotInherited + 1), false };
    }

    [Theory]
    [MemberData(nameof(CanRemoveInstance_InheritanceAttribute_TestData))]
    public void CollectionEditor_CanRemoveInstance_InheritanceAttribute_ReturnsExpected(InheritanceAttribute attribute, bool expected)
    {
        using Component component = new();
        TypeDescriptor.AddAttributes(component, attribute);
        SubCollectionEditor editor = new(null);
        Assert.Equal(expected, editor.CanRemoveInstance(component));
    }

    [Fact]
    public void CollectionEditor_CanSelectMultipleInstances_Invoke_ReturnsFalse()
    {
        SubCollectionEditor editor = new(null);
        Assert.True(editor.CanSelectMultipleInstances());
    }

    [Fact]
    public void CollectionEditor_CreateCollectionForm_Invoke_Success()
    {
        SubCollectionEditor editor = new(typeof(List<int>));
        Form form = editor.CreateCollectionForm();
        Assert.NotSame(form, editor.CreateCollectionForm());
    }

    [Fact]
    public void CollectionEditor_CreateCollectionForm_NullCollectionType_ThrowsArgumentNullException()
    {
        SubCollectionEditor editor = new(null);
        Assert.Throws<ArgumentNullException>("type", editor.CreateCollectionForm);
    }

    [Theory]
    [InlineData(typeof(object), typeof(object))]
    [InlineData(typeof(int[]), typeof(object))]
    [InlineData(typeof(IList<int>), typeof(int))]
    [InlineData(typeof(ClassWithItem), typeof(int))]
    [InlineData(typeof(ClassWithPrivateItem), typeof(object))]
    [InlineData(typeof(ClassWithStaticItem), typeof(object))]
    [InlineData(typeof(ClassWithItems), typeof(int))]
    [InlineData(typeof(ClassWithPrivateItems), typeof(object))]
    [InlineData(typeof(ClassWithStaticItems), typeof(object))]
    public void CollectionEditor_CreateCollectionItemType_Invoke_ReturnsExpected(Type type, Type expected)
    {
        SubCollectionEditor editor = new(type);
        Type itemType = editor.CreateCollectionItemType();
        Assert.Equal(expected, itemType);
        Assert.Same(itemType, editor.CreateCollectionItemType());
    }

    [Fact]
    public void CollectionEditor_CreateCollectionItemType_NullType_ThrowsArgumentNullException()
    {
        SubCollectionEditor editor = new(null);
        Assert.Throws<ArgumentNullException>("type", editor.CreateCollectionItemType);
    }

    public static IEnumerable<object[]> InvalidDesignerHost_TestData()
    {
        yield return new object[] { null };
        yield return new object[] { new() };
    }

    [Theory]
    [MemberData(nameof(InvalidDesignerHost_TestData))]
    public void CollectionEditor_CreateInstance_WithContextWithInvalidDesignerHost_ReturnsExpected(object host)
    {
        Mock<IWindowsFormsEditorService> mockEditorService = new(MockBehavior.Strict);
        mockEditorService
            .Setup(s => s.ShowDialog(It.IsAny<Form>()))
            .Returns(DialogResult.OK);

        Mock<IServiceProvider> mockServiceProvider = new(MockBehavior.Strict);
        mockServiceProvider
            .Setup(p => p.GetService(typeof(IWindowsFormsEditorService)))
            .Returns(mockEditorService.Object);

        Mock<ITypeDescriptorContext> mockContext = new(MockBehavior.Strict);
        mockContext
            .Setup(c => c.GetService(typeof(IDesignerHost)))
            .Returns(host);
        mockContext
            .Setup(c => c.GetService(typeof(AmbientProperties)))
            .Returns(null);
        mockContext
            .Setup(c => c.GetService(typeof(IComponentChangeService)))
            .Returns(null);

        SubCollectionEditor editor = new(typeof(List<int>));
        object value = new();
        Assert.Same(value, editor.EditValue(mockContext.Object, mockServiceProvider.Object, value));
        Assert.Same(mockContext.Object, editor.Context);

        Assert.IsType<Component>(editor.CreateInstance(typeof(Component)));
    }

    public static IEnumerable<object[]> CreateInstance_HostDesigner_TestData()
    {
        yield return new object[] { null };

        Mock<IDesigner> mockDesigner = new(MockBehavior.Strict);
        mockDesigner
            .Setup(d => d.Dispose());
        yield return new object[] { mockDesigner.Object };
    }

    [Theory]
    [MemberData(nameof(CreateInstance_HostDesigner_TestData))]
    public void CollectionEditor_CreateInstance_WithContextWithHostReturningComponent_CallsCreateComponent(IDesigner designer)
    {
        Mock<IWindowsFormsEditorService> mockEditorService = new(MockBehavior.Strict);
        mockEditorService
            .Setup(s => s.ShowDialog(It.IsAny<Form>()))
            .Returns(DialogResult.OK);

        Mock<IServiceProvider> mockServiceProvider = new(MockBehavior.Strict);
        mockServiceProvider
            .Setup(p => p.GetService(typeof(IWindowsFormsEditorService)))
            .Returns(mockEditorService.Object);

        using Component result = new();
        Mock<IDesignerHost> mockHost = new(MockBehavior.Strict);
        mockHost
            .Setup(h => h.CreateTransaction("Add or remove Int32 objects"))
            .Returns((DesignerTransaction)null);
        mockHost
            .Setup(c => c.GetService(typeof(IComponentChangeService)))
            .Returns(null);
        mockHost
            .Setup(h => h.CreateComponent(typeof(Component)))
            .Returns(result);
        mockHost
            .Setup(h => h.GetDesigner(result))
            .Returns(designer);

        Mock<ITypeDescriptorContext> mockContext = new(MockBehavior.Strict);
        mockContext
            .Setup(c => c.GetService(typeof(IDesignerHost)))
            .Returns(mockHost.Object);
        mockContext
            .Setup(c => c.GetService(typeof(AmbientProperties)))
            .Returns(null);
        mockContext
            .Setup(c => c.GetService(typeof(IComponentChangeService)))
            .Returns(null);

        SubCollectionEditor editor = new(typeof(List<int>));
        object value = new();
        Assert.Same(value, editor.EditValue(mockContext.Object, mockServiceProvider.Object, value));
        Assert.Same(mockContext.Object, editor.Context);

        Assert.Same(result, editor.CreateInstance(typeof(Component)));
    }

    [Theory]
    [MemberData(nameof(CreateInstance_HostDesigner_TestData))]
    public void CollectionEditor_CreateInstance_WithContextWithHostReturningNullComponent_CallsCreateComponent(IDesigner designer)
    {
        Mock<IWindowsFormsEditorService> mockEditorService = new(MockBehavior.Strict);
        mockEditorService
            .Setup(s => s.ShowDialog(It.IsAny<Form>()))
            .Returns(DialogResult.OK);

        Mock<IServiceProvider> mockServiceProvider = new(MockBehavior.Strict);
        mockServiceProvider
            .Setup(p => p.GetService(typeof(IWindowsFormsEditorService)))
            .Returns(mockEditorService.Object);

        Mock<IDesignerHost> mockHost = new(MockBehavior.Strict);
        mockHost
            .Setup(h => h.CreateTransaction("Add or remove Int32 objects"))
            .Returns((DesignerTransaction)null);
        mockHost
            .Setup(c => c.GetService(typeof(IComponentChangeService)))
            .Returns(null);
        mockHost
            .Setup(h => h.CreateComponent(typeof(Component)))
            .Returns((IComponent)null);
        mockHost
            .Setup(h => h.GetDesigner(null))
            .Returns(designer);
        mockHost
            .Setup(c => c.GetService(typeof(TypeDescriptionProvider)))
            .Returns(null);

        object result = new();
        Mock<ITypeDescriptorContext> mockContext = new(MockBehavior.Strict);
        mockContext
            .Setup(c => c.GetService(typeof(IDesignerHost)))
            .Returns(mockHost.Object);
        mockContext
            .Setup(c => c.GetService(typeof(AmbientProperties)))
            .Returns(null);
        mockContext
            .Setup(c => c.GetService(typeof(IComponentChangeService)))
            .Returns(null);

        SubCollectionEditor editor = new(typeof(List<int>));
        object value = new();
        Assert.Same(value, editor.EditValue(mockContext.Object, mockServiceProvider.Object, value));
        Assert.Same(mockContext.Object, editor.Context);

        Assert.IsType<Component>(editor.CreateInstance(typeof(Component)));
    }

    [Fact]
    public void CollectionEditor_CreateInstance_WithContextWithHostReturningComponentWithIComponentInitializerDesigner_CallsInitializeNewComponent()
    {
        Mock<IWindowsFormsEditorService> mockEditorService = new(MockBehavior.Strict);
        mockEditorService
            .Setup(s => s.ShowDialog(It.IsAny<Form>()))
            .Returns(DialogResult.OK);

        Mock<IServiceProvider> mockServiceProvider = new(MockBehavior.Strict);
        mockServiceProvider
            .Setup(p => p.GetService(typeof(IWindowsFormsEditorService)))
            .Returns(mockEditorService.Object);

        using Component result = new();
        Mock<IDesigner> mockDesigner = new(MockBehavior.Strict);
        Mock<IComponentInitializer> mockComponentInitializer = mockDesigner.As<IComponentInitializer>();
        mockComponentInitializer
            .Setup(d => d.InitializeNewComponent(null))
            .Verifiable();

        Mock<IDesignerHost> mockHost = new(MockBehavior.Strict);
        mockHost
            .Setup(h => h.CreateTransaction("Add or remove Int32 objects"))
            .Returns((DesignerTransaction)null);
        mockHost
            .Setup(c => c.GetService(typeof(IComponentChangeService)))
            .Returns(null);
        mockHost
            .Setup(h => h.CreateComponent(typeof(Component)))
            .Returns(result);
        mockHost
            .Setup(h => h.GetDesigner(result))
            .Returns(mockDesigner.Object);

        Mock<ITypeDescriptorContext> mockContext = new(MockBehavior.Strict);
        mockContext
            .Setup(c => c.GetService(typeof(IDesignerHost)))
            .Returns(mockHost.Object);
        mockContext
            .Setup(c => c.GetService(typeof(AmbientProperties)))
            .Returns(null);
        mockContext
            .Setup(c => c.GetService(typeof(IComponentChangeService)))
            .Returns(null);

        SubCollectionEditor editor = new(typeof(List<int>));
        object value = new();
        Assert.Same(value, editor.EditValue(mockContext.Object, mockServiceProvider.Object, value));
        Assert.Same(mockContext.Object, editor.Context);

        Assert.Same(result, editor.CreateInstance(typeof(Component)));
        mockComponentInitializer.Verify(d => d.InitializeNewComponent(null), Times.Once());
    }

    public static IEnumerable<object[]> CreateInstance_InvokeWithoutContext_TestData()
    {
        yield return new object[] { typeof(int), 0 };
        yield return new object[] { typeof(string), string.Empty };
    }

    [Theory]
    [MemberData(nameof(CreateInstance_InvokeWithoutContext_TestData))]
    public void CollectionEditor_CreateInstance_InvokeWithoutContext_ReturnsExpected(Type type, object expected)
    {
        SubCollectionEditor editor = new(null);
        Assert.Equal(expected, editor.CreateInstance(type));
    }

    [Fact]
    public void CollectionEditor_CreateInstance_NullItemType_ThrowsArgumentNullException()
    {
        SubCollectionEditor editor = new(null);
        Assert.Throws<ArgumentNullException>("itemType", () => editor.CreateInstance(null));
    }

    [Theory]
    [InlineData(typeof(object), new Type[] { typeof(object) })]
    [InlineData(typeof(int[]), new Type[] { typeof(object) })]
    [InlineData(typeof(IList<int>), new Type[] { typeof(int) })]
    [InlineData(typeof(ClassWithItem), new Type[] { typeof(int) })]
    [InlineData(typeof(ClassWithPrivateItem), new Type[] { typeof(object) })]
    [InlineData(typeof(ClassWithStaticItem), new Type[] { typeof(object) })]
    [InlineData(typeof(ClassWithItems), new Type[] { typeof(int) })]
    [InlineData(typeof(ClassWithPrivateItems), new Type[] { typeof(object) })]
    [InlineData(typeof(ClassWithStaticItems), new Type[] { typeof(object) })]
    public void CollectionEditor_CreateNewItemTypes_Invoke_ReturnsExpected(Type type, Type[] expected)
    {
        SubCollectionEditor editor = new(type);
        Type[] itemTypes = editor.CreateNewItemTypes();
        Assert.Equal(expected, itemTypes);
        Assert.NotSame(itemTypes, editor.CreateNewItemTypes());
    }

    [Fact]
    public void CollectionEditor_CreateNewItemTypes_NullType_ThrowsArgumentNullException()
    {
        SubCollectionEditor editor = new(null);
        Assert.Throws<ArgumentNullException>("type", editor.CreateNewItemTypes);
    }

    public static IEnumerable<object[]> DestroyInstance_NormalObject_TestData()
    {
        yield return new object[] { null };
        yield return new object[] { new() };
    }

    [Theory]
    [MemberData(nameof(DestroyInstance_NormalObject_TestData))]
    public void CollectionEditor_DestroyInstance_NormalObject_Nop(object instance)
    {
        SubCollectionEditor editor = new(null);
        editor.DestroyInstance(instance);
    }

    [Theory]
    [MemberData(nameof(InvalidDesignerHost_TestData))]
    public void CollectionEditor_DestroyInstance_WithContextWithInvalidDesignerHot_CallsDispose(object host)
    {
        Mock<IWindowsFormsEditorService> mockEditorService = new(MockBehavior.Strict);
        mockEditorService
            .Setup(s => s.ShowDialog(It.IsAny<Form>()))
            .Returns(DialogResult.OK);

        Mock<IServiceProvider> mockServiceProvider = new(MockBehavior.Strict);
        mockServiceProvider
            .Setup(p => p.GetService(typeof(IWindowsFormsEditorService)))
            .Returns(mockEditorService.Object);

        Mock<ITypeDescriptorContext> mockContext = new(MockBehavior.Strict);
        mockContext
            .Setup(c => c.GetService(typeof(IDesignerHost)))
            .Returns(host);
        mockContext
            .Setup(c => c.GetService(typeof(AmbientProperties)))
            .Returns(null);
        mockContext
            .Setup(c => c.GetService(typeof(IComponentChangeService)))
            .Returns(null);

        SubCollectionEditor editor = new(typeof(List<int>));
        object value = new();
        Assert.Same(value, editor.EditValue(mockContext.Object, mockServiceProvider.Object, value));
        Assert.Same(mockContext.Object, editor.Context);

        Mock<IComponent> mockComponent = new(MockBehavior.Strict);
        mockComponent
            .Setup(c => c.Dispose())
            .Verifiable();
        editor.DestroyInstance(mockComponent.Object);
        mockComponent.Verify(c => c.Dispose(), Times.Once());
    }

    [Fact]
    public void CollectionEditor_DestroyInstance_WithContextWithInvalidHost_CallsDestroyComponent()
    {
        Mock<IWindowsFormsEditorService> mockEditorService = new(MockBehavior.Strict);
        mockEditorService
            .Setup(s => s.ShowDialog(It.IsAny<Form>()))
            .Returns(DialogResult.OK);

        Mock<IServiceProvider> mockServiceProvider = new(MockBehavior.Strict);
        mockServiceProvider
            .Setup(p => p.GetService(typeof(IWindowsFormsEditorService)))
            .Returns(mockEditorService.Object);

        Mock<IComponent> mockComponent = new(MockBehavior.Strict);
        mockComponent
            .Setup(c => c.Dispose())
            .Verifiable();

        Mock<IDesignerHost> mockHost = new(MockBehavior.Strict);
        mockHost
            .Setup(h => h.CreateTransaction("Add or remove Int32 objects"))
            .Returns((DesignerTransaction)null);
        mockHost
            .Setup(c => c.GetService(typeof(IComponentChangeService)))
            .Returns(null);
        mockHost
            .Setup(h => h.DestroyComponent(mockComponent.Object))
            .Verifiable();

        Mock<ITypeDescriptorContext> mockContext = new(MockBehavior.Strict);
        mockContext
            .Setup(c => c.GetService(typeof(IDesignerHost)))
            .Returns(mockHost.Object);
        mockContext
            .Setup(c => c.GetService(typeof(AmbientProperties)))
            .Returns(null);
        mockContext
            .Setup(c => c.GetService(typeof(IComponentChangeService)))
            .Returns(null);

        SubCollectionEditor editor = new(typeof(List<int>));
        object value = new();
        Assert.Same(value, editor.EditValue(mockContext.Object, mockServiceProvider.Object, value));
        Assert.Same(mockContext.Object, editor.Context);

        editor.DestroyInstance(mockComponent.Object);
        mockComponent.Verify(c => c.Dispose(), Times.Never());
        mockHost.Verify(c => c.DestroyComponent(mockComponent.Object), Times.Once());
    }

    [Fact]
    public void CollectionEditor_DestroyInstance_IComponentWithoutHost_CallsDispose()
    {
        Mock<IComponent> mockComponent = new(MockBehavior.Strict);
        mockComponent
            .Setup(c => c.Dispose())
            .Verifiable();

        SubCollectionEditor editor = new(null);
        editor.DestroyInstance(mockComponent.Object);
        mockComponent.Verify(c => c.Dispose(), Times.Once());
    }

    [Fact]
    public void CollectionEditor_DestroyInstance_IDisposable_CallsDispose()
    {
        Mock<IDisposable> mockDisposable = new(MockBehavior.Strict);
        mockDisposable
            .Setup(d => d.Dispose())
            .Verifiable();

        SubCollectionEditor editor = new(null);
        editor.DestroyInstance(mockDisposable.Object);
        mockDisposable.Verify(d => d.Dispose(), Times.Once());
    }

    [Theory]
    [MemberData(nameof(InvalidDesignerHost_TestData))]
    public void CollectionEditor_EditValue_ValidProviderInvalidHost_ReturnsValue(object host)
    {
        Mock<IWindowsFormsEditorService> mockEditorService = new(MockBehavior.Strict);
        mockEditorService
            .Setup(s => s.ShowDialog(It.IsAny<Form>()))
            .Returns(DialogResult.OK);

        Mock<IServiceProvider> mockServiceProvider = new(MockBehavior.Strict);
        mockServiceProvider
            .Setup(p => p.GetService(typeof(IWindowsFormsEditorService)))
            .Returns(mockEditorService.Object);

        Mock<ITypeDescriptorContext> mockContext = new(MockBehavior.Strict);
        mockContext
            .Setup(c => c.GetService(typeof(IDesignerHost)))
            .Returns(host);
        mockContext
            .Setup(c => c.GetService(typeof(AmbientProperties)))
            .Returns(null);
        mockContext
            .Setup(c => c.GetService(typeof(IComponentChangeService)))
            .Returns(null);

        SubCollectionEditor editor = new(typeof(List<int>));
        object value = new();
        Assert.Same(value, editor.EditValue(mockContext.Object, mockServiceProvider.Object, value));
        Assert.Same(mockContext.Object, editor.Context);

        mockEditorService.Verify(s => s.ShowDialog(It.IsAny<Form>()), Times.Once());
    }

    [Theory]
    [MemberData(nameof(InvalidDesignerHost_TestData))]
    public void CollectionEditor_EditValue_ValidProviderValidHost_ReturnsValue(object changeService)
    {
        Mock<IWindowsFormsEditorService> mockEditorService = new(MockBehavior.Strict);
        mockEditorService
            .Setup(s => s.ShowDialog(It.IsAny<Form>()))
            .Returns(DialogResult.OK);

        Mock<IServiceProvider> mockServiceProvider = new(MockBehavior.Strict);
        mockServiceProvider
            .Setup(p => p.GetService(typeof(IWindowsFormsEditorService)))
            .Returns(mockEditorService.Object);

        Mock<IDesignerHost> mockHost = new(MockBehavior.Strict);
        mockHost
            .Setup(h => h.CreateTransaction("Add or remove Int32 objects"))
            .Returns((DesignerTransaction)null);
        mockHost
            .Setup(c => c.GetService(typeof(IComponentChangeService)))
            .Returns(changeService);

        Mock<ITypeDescriptorContext> mockContext = new(MockBehavior.Strict);
        mockContext
            .Setup(c => c.GetService(typeof(IDesignerHost)))
            .Returns(mockHost.Object);
        mockContext
            .Setup(c => c.GetService(typeof(AmbientProperties)))
            .Returns(null);
        mockContext
            .Setup(c => c.GetService(typeof(IComponentChangeService)))
            .Returns(changeService);

        SubCollectionEditor editor = new(typeof(List<int>));
        object value = new();
        Assert.Same(value, editor.EditValue(mockContext.Object, mockServiceProvider.Object, value));
        Assert.Same(mockContext.Object, editor.Context);

        mockEditorService.Verify(s => s.ShowDialog(It.IsAny<Form>()), Times.Once());
    }

    [Fact]
    public void CollectionEditor_EditValue_ValidProviderValidHostWithTransactionOK_CallsOnCommit()
    {
        Mock<IWindowsFormsEditorService> mockEditorService = new(MockBehavior.Strict);
        mockEditorService
            .Setup(s => s.ShowDialog(It.IsAny<Form>()))
            .Returns(DialogResult.OK);

        Mock<IServiceProvider> mockServiceProvider = new(MockBehavior.Strict);
        mockServiceProvider
            .Setup(p => p.GetService(typeof(IWindowsFormsEditorService)))
            .Returns(mockEditorService.Object);

        Mock<DesignerTransaction> mockTransaction = new(MockBehavior.Strict);
        mockTransaction
            .Protected()
            .Setup("OnCommit")
            .Verifiable();

        Mock<IDesignerHost> mockHost = new(MockBehavior.Strict);
        mockHost
            .Setup(h => h.CreateTransaction("Add or remove Int32 objects"))
            .Returns(mockTransaction.Object);
        mockHost
            .Setup(c => c.GetService(typeof(IComponentChangeService)))
            .Returns(null);

        Mock<ITypeDescriptorContext> mockContext = new(MockBehavior.Strict);
        mockContext
            .Setup(c => c.GetService(typeof(IDesignerHost)))
            .Returns(mockHost.Object);
        mockContext
            .Setup(c => c.GetService(typeof(AmbientProperties)))
            .Returns(null);
        mockContext
            .Setup(c => c.GetService(typeof(IComponentChangeService)))
            .Returns(null);

        SubCollectionEditor editor = new(typeof(List<int>));
        object value = new();
        Assert.Same(value, editor.EditValue(mockContext.Object, mockServiceProvider.Object, value));
        Assert.Same(mockContext.Object, editor.Context);

        mockEditorService.Verify(s => s.ShowDialog(It.IsAny<Form>()), Times.Once());
        mockTransaction.Protected().Verify("OnCommit", Times.Once());
    }

    [Fact]
    public void CollectionEditor_EditValue_ValidProviderValidHostWithTransactionNotOK_CallsOnCancel()
    {
        Mock<IWindowsFormsEditorService> mockEditorService = new(MockBehavior.Strict);
        mockEditorService
            .Setup(s => s.ShowDialog(It.IsAny<Form>()))
            .Returns(DialogResult.Cancel);

        Mock<IServiceProvider> mockServiceProvider = new(MockBehavior.Strict);
        mockServiceProvider
            .Setup(p => p.GetService(typeof(IWindowsFormsEditorService)))
            .Returns(mockEditorService.Object);

        Mock<DesignerTransaction> mockTransaction = new(MockBehavior.Strict);
        mockTransaction
            .Protected()
            .Setup("OnCancel")
            .Verifiable();

        Mock<IDesignerHost> mockHost = new(MockBehavior.Strict);
        mockHost
            .Setup(h => h.CreateTransaction("Add or remove Int32 objects"))
            .Returns(mockTransaction.Object);
        mockHost
            .Setup(c => c.GetService(typeof(IComponentChangeService)))
            .Returns(null);

        Mock<ITypeDescriptorContext> mockContext = new(MockBehavior.Strict);
        mockContext
            .Setup(c => c.GetService(typeof(IDesignerHost)))
            .Returns(mockHost.Object);
        mockContext
            .Setup(c => c.GetService(typeof(AmbientProperties)))
            .Returns(null);
        mockContext
            .Setup(c => c.GetService(typeof(IComponentChangeService)))
            .Returns(null);

        SubCollectionEditor editor = new(typeof(List<int>));
        object value = new();
        Assert.Same(value, editor.EditValue(mockContext.Object, mockServiceProvider.Object, value));
        Assert.Same(mockContext.Object, editor.Context);

        mockEditorService.Verify(s => s.ShowDialog(It.IsAny<Form>()), Times.Once());
        mockTransaction.Protected().Verify("OnCancel", Times.Once());
    }

    [Fact]
    public void CollectionEditor_EditValue_ValidProviderValidHostWithIComponentChangeService_ReturnsValue()
    {
        Mock<IWindowsFormsEditorService> mockEditorService = new(MockBehavior.Strict);
        mockEditorService
            .Setup(s => s.ShowDialog(It.IsAny<Form>()))
            .Returns(DialogResult.OK);

        Mock<IComponentChangeService> mockChangeService = new(MockBehavior.Strict);

        Mock<IServiceProvider> mockServiceProvider = new(MockBehavior.Strict);
        mockServiceProvider
            .Setup(p => p.GetService(typeof(IWindowsFormsEditorService)))
            .Returns(mockEditorService.Object);

        Mock<IDesignerHost> mockHost = new(MockBehavior.Strict);
        mockHost
            .Setup(h => h.CreateTransaction("Add or remove Int32 objects"))
            .Returns((DesignerTransaction)null);
        mockHost
            .Setup(c => c.GetService(typeof(IComponentChangeService)))
            .Returns(mockChangeService.Object);

        Mock<ITypeDescriptorContext> mockContext = new(MockBehavior.Strict);
        mockContext
            .Setup(c => c.GetService(typeof(IDesignerHost)))
            .Returns(mockHost.Object);
        mockContext
            .Setup(c => c.GetService(typeof(AmbientProperties)))
            .Returns(null);
        mockContext
            .Setup(c => c.GetService(typeof(IComponentChangeService)))
            .Returns(mockChangeService.Object);

        SubCollectionEditor editor = new(typeof(List<int>));
        object value = new();
        Assert.Same(value, editor.EditValue(mockContext.Object, mockServiceProvider.Object, value));
        Assert.Same(mockContext.Object, editor.Context);

        mockEditorService.Verify(s => s.ShowDialog(It.IsAny<Form>()), Times.Once());
    }

    [Fact]
    public void CollectionEditor_EditValue_NullType_ThrowsArgumentNullException()
    {
        Mock<IWindowsFormsEditorService> mockEditorService = new(MockBehavior.Strict);
        Mock<IServiceProvider> mockServiceProvider = new(MockBehavior.Strict);
        mockServiceProvider
            .Setup(p => p.GetService(typeof(IWindowsFormsEditorService)))
            .Returns(mockEditorService.Object);
        Mock<ITypeDescriptorContext> mockContext = new(MockBehavior.Strict);
        mockContext
            .Setup(c => c.GetService(typeof(IDesignerHost)))
            .Returns((IDesignerHost)null);
        mockContext
            .Setup(c => c.GetService(typeof(AmbientProperties)))
            .Returns(null);

        SubCollectionEditor editor = new(null);
        object value = new();
        Assert.Throws<ArgumentNullException>("type", () => editor.EditValue(mockContext.Object, mockServiceProvider.Object, value));
    }

    [Theory]
    [CommonMemberData(typeof(CommonTestHelperEx), nameof(CommonTestHelperEx.GetEditValueInvalidProviderTestData))]
    public void CollectionEditor_EditValue_InvalidProvider_ReturnsValue(IServiceProvider provider, object value)
    {
        SubCollectionEditor editor = new(null);
        Assert.Same(value, editor.EditValue(null, provider, value));
        Assert.Null(editor.Context);
    }

    [Theory]
    [CommonMemberData(typeof(CommonTestHelperEx), nameof(CommonTestHelperEx.GetITypeDescriptorContextTestData))]
    public void CollectionEditor_GetEditStyle_Invoke_ReturnsModal(ITypeDescriptorContext context)
    {
        CollectionEditor editor = new(null);
        Assert.Equal(UITypeEditorEditStyle.Modal, editor.GetEditStyle(context));
    }

    [Theory]
    [CommonMemberData(typeof(CommonTestHelperEx), nameof(CommonTestHelperEx.GetITypeDescriptorContextTestData))]
    public void CollectionEditor_GetPaintValueSupported_Invoke_ReturnsFalse(ITypeDescriptorContext context)
    {
        CollectionEditor editor = new(null);
        Assert.False(editor.GetPaintValueSupported(context));
    }

    public static IEnumerable<object[]> GetDisplayText_TestData()
    {
        yield return new object[] { null, null, string.Empty };
        yield return new object[] { null, string.Empty, "String" };
        yield return new object[] { null, "string", "string" };

        yield return new object[] { null, new ClassWithStringName { Name = "CustomName" }, "CustomName" };
        yield return new object[] { null, new ClassWithStringName { Name = string.Empty }, "ClassWithStringName" };
        yield return new object[] { null, new ClassWithStringName { Name = null }, "ClassWithStringName" };
        yield return new object[] { null, new ClassWithNonStringName { Name = 1 }, "ClassWithNonStringName" };
        yield return new object[] { null, new ClassWithNullToString(), "ClassWithNullToString" };

        yield return new object[] { typeof(int), null, string.Empty };
        yield return new object[] { typeof(int), "", "String" };
        yield return new object[] { typeof(int), "value", "value" };
        yield return new object[] { typeof(int), 1, "1" };
        yield return new object[] { typeof(int), new ClassWithStringDefaultProperty { DefaultProperty = "CustomName" }, "ClassWithStringDefaultProperty" };

        yield return new object[] { typeof(ClassWithStringDefaultProperty), null, string.Empty };
        yield return new object[] { typeof(ClassWithStringDefaultProperty), new ClassWithStringDefaultProperty { DefaultProperty = "CustomName" }, "CustomName" };
        yield return new object[] { typeof(ClassWithStringDefaultProperty), new ClassWithStringDefaultProperty { DefaultProperty = string.Empty }, "ClassWithStringDefaultProperty" };
        yield return new object[] { typeof(ClassWithStringDefaultProperty), new ClassWithStringDefaultProperty { DefaultProperty = null }, "ClassWithStringDefaultProperty" };
        yield return new object[] { typeof(ClassWithNonStringDefaultProperty), new ClassWithNonStringDefaultProperty { DefaultProperty = 1 }, "ClassWithNonStringDefaultProperty" };
        yield return new object[] { typeof(ClassWithNoSuchDefaultProperty), new ClassWithNoSuchDefaultProperty { DefaultProperty = "CustomName" }, "ClassWithNoSuchDefaultProperty" };
        yield return new object[] { typeof(List<ClassWithStringDefaultProperty>), new ClassWithStringDefaultProperty { DefaultProperty = "CustomName" }, "ClassWithStringDefaultProperty" };
    }

    [Theory]
    [MemberData(nameof(GetDisplayText_TestData))]
    public void CollectionEditor_GetDisplayText_Invoke_ReturnsExpected(Type type, object value, string expected)
    {
        SubCollectionEditor editor = new(type);
        Assert.Equal(expected, editor.GetDisplayText(value));
    }

    [Fact]
    public void CollectionEditor_GetDisplayText_ValueDoesNotMatchCollectionType_ThrowsTargetException()
    {
        SubCollectionEditor editor = new(typeof(ClassWithStringDefaultProperty));
        TargetInvocationException ex = Assert.Throws<TargetInvocationException>(() => editor.GetDisplayText(new ClassWithNonStringDefaultProperty()));
        Assert.IsType<TargetException>(ex.InnerException);
    }

    public static IEnumerable<object[]> GetItems_TestData()
    {
        yield return new object[] { null, Array.Empty<object>() };
        yield return new object[] { new(), Array.Empty<object>() };
        yield return new object[] { new int[] { 1, 2, 3 }, new object[] { 1, 2, 3, } };
        yield return new object[] { new ArrayList { 1, 2, 3 }, new object[] { 1, 2, 3, } };
    }

    [Theory]
    [MemberData(nameof(GetItems_TestData))]
    public void CollectionEditor_GetItems_Invoke_ReturnsExpected(object editValue, object[] expected)
    {
        SubCollectionEditor editor = new(null);
        object[] items = editor.GetItems(editValue);
        Assert.Equal(expected, items);
        Assert.IsType(expected.GetType(), items);
        Assert.NotSame(editValue, items);
    }

    [Theory]
    [CommonMemberData(typeof(CommonTestHelperEx), nameof(CommonTestHelperEx.GetTypeWithNullTheoryData))]
    public void CollectionEditor_GetService_WithContext_CallsContextGetService(Type serviceType)
    {
        Mock<IWindowsFormsEditorService> mockEditorService = new(MockBehavior.Strict);
        mockEditorService
            .Setup(s => s.ShowDialog(It.IsAny<Form>()))
            .Returns(DialogResult.OK);

        Mock<IServiceProvider> mockServiceProvider = new(MockBehavior.Strict);
        mockServiceProvider
            .Setup(p => p.GetService(typeof(IWindowsFormsEditorService)))
            .Returns(mockEditorService.Object);

        object result = new();
        Mock<ITypeDescriptorContext> mockContext = new(MockBehavior.Strict);
        mockContext
            .Setup(c => c.GetService(typeof(IDesignerHost)))
            .Returns(null);
        mockContext
            .Setup(c => c.GetService(typeof(AmbientProperties)))
            .Returns(null);
        mockContext
            .Setup(c => c.GetService(typeof(IComponentChangeService)))
            .Returns(null);
        mockContext
            .Setup(c => c.GetService(serviceType))
            .Returns(result);

        SubCollectionEditor editor = new(typeof(List<int>));
        object value = new();
        Assert.Same(value, editor.EditValue(mockContext.Object, mockServiceProvider.Object, value));
        Assert.Same(mockContext.Object, editor.Context);

        Assert.Same(result, editor.GetService(serviceType));
    }

    [Theory]
    [CommonMemberData(typeof(CommonTestHelperEx), nameof(CommonTestHelperEx.GetTypeWithNullTheoryData))]
    public void CollectionEditor_GetService_InvokeWithoutContext_ReturnsNull(Type serviceType)
    {
        SubCollectionEditor editor = new(serviceType);
        Assert.Null(editor.GetService(serviceType));
    }

    public static IEnumerable<object[]> GetObjectsFromInstance_TestData()
    {
        yield return new object[] { null };
        yield return new object[] { new() };
    }

    [Theory]
    [MemberData(nameof(GetObjectsFromInstance_TestData))]
    public void CollectionEditor_GetObjectsFromInstance_Invoke_ReturnsExpected(object instance)
    {
        SubCollectionEditor editor = new(null);
        IList objects = editor.GetObjectsFromInstance(instance);
        Assert.Equal(new object[] { instance }, objects);
        Assert.IsType<ArrayList>(objects);
        Assert.NotSame(objects, editor.GetObjectsFromInstance(instance));
    }

    public static IEnumerable<object[]> SetItems_TestData()
    {
        yield return new object[] { null, new object[] { 1, 2, 3 }, null };
        yield return new object[] { null, Array.Empty<object>(), null };
        yield return new object[] { null, null, null };

        object o = new();
        yield return new object[] { o, new object[] { 1, 2, 3 }, o };
        yield return new object[] { o, Array.Empty<object>(), o };
        yield return new object[] { o, null, o };

        yield return new object[] { new int[] { 1, 2, 3 }, Array.Empty<object>(), new object[] { 0, 0, 0 } };
        yield return new object[] { new int[] { 1, 2, 3 }, null, new object[] { 0, 0, 0 } };

        yield return new object[] { new ArrayList { 1, 2, 3 }, new object[] { 1 }, new object[] { 1 } };
        yield return new object[] { new ArrayList { 1, 2, 3 }, Array.Empty<object>(), Array.Empty<object>() };
        yield return new object[] { new ArrayList { 1, 2, 3 }, null, Array.Empty<object>() };
    }

    [Theory]
    [MemberData(nameof(SetItems_TestData))]
    public void CollectionEditor_SetItems_Invoke_ReturnsExpected(object editValue, object[] value, object expected)
    {
        SubCollectionEditor editor = new(null);
        object items = editor.SetItems(editValue, value);
        Assert.Equal(expected, items);
        Assert.Same(editValue, items);
    }

    [Fact]
    public void CollectionEditor_SetItems_InvokeArray_ThrowsNotSupportedException()
    {
        SubCollectionEditor editor = new(null);
        Assert.Throws<NotSupportedException>(() => editor.SetItems(new object[1], new object[1]));
    }

    [Fact]
    public void CollectionEditor_ShowHelp_NoContext_Nop()
    {
        SubCollectionEditor editor = new(typeof(List<int>));
        editor.ShowHelp();
    }

    [Fact]
    public void CollectionEditor_ShowHelp_ValidDesignerHost_CallsShowHelpFromKeyword()
    {
        Mock<IWindowsFormsEditorService> mockEditorService = new(MockBehavior.Strict);
        mockEditorService
            .Setup(s => s.ShowDialog(It.IsAny<Form>()))
            .Returns(DialogResult.OK);

        Mock<IServiceProvider> mockServiceProvider = new(MockBehavior.Strict);
        mockServiceProvider
            .Setup(p => p.GetService(typeof(IWindowsFormsEditorService)))
            .Returns(mockEditorService.Object);

        Mock<IHelpService> mockHelpService = new(MockBehavior.Strict);
        mockHelpService
            .Setup(s => s.ShowHelpFromKeyword("net.ComponentModel.CollectionEditor"))
            .Verifiable();

        Mock<ITypeDescriptorContext> mockContext = new(MockBehavior.Strict);
        mockContext
            .Setup(c => c.GetService(typeof(IDesignerHost)))
            .Returns(null);
        mockContext
            .Setup(c => c.GetService(typeof(IHelpService)))
            .Returns(mockHelpService.Object);
        mockContext
            .Setup(c => c.GetService(typeof(AmbientProperties)))
            .Returns(null);
        mockContext
            .Setup(c => c.GetService(typeof(IComponentChangeService)))
            .Returns(null);

        SubCollectionEditor editor = new(typeof(List<int>));
        object value = new();
        Assert.Same(value, editor.EditValue(mockContext.Object, mockServiceProvider.Object, value));
        Assert.Same(mockContext.Object, editor.Context);

        editor.ShowHelp();

        mockHelpService.Verify(s => s.ShowHelpFromKeyword("net.ComponentModel.CollectionEditor"), Times.Once());
    }

    [Theory]
    [MemberData(nameof(InvalidDesignerHost_TestData))]
    public void CollectionEditor_ShowHelp_InvalidHelpService_Nop(object helpService)
    {
        Mock<IWindowsFormsEditorService> mockEditorService = new(MockBehavior.Strict);
        mockEditorService
            .Setup(s => s.ShowDialog(It.IsAny<Form>()))
            .Returns(DialogResult.OK);

        Mock<IServiceProvider> mockServiceProvider = new(MockBehavior.Strict);
        mockServiceProvider
            .Setup(p => p.GetService(typeof(IWindowsFormsEditorService)))
            .Returns(mockEditorService.Object);

        Mock<ITypeDescriptorContext> mockContext = new(MockBehavior.Strict);
        mockContext
            .Setup(c => c.GetService(typeof(IDesignerHost)))
            .Returns(null);
        mockContext
            .Setup(c => c.GetService(typeof(IHelpService)))
            .Returns(helpService);
        mockContext
            .Setup(c => c.GetService(typeof(AmbientProperties)))
            .Returns(null);
        mockContext
            .Setup(c => c.GetService(typeof(IComponentChangeService)))
            .Returns(null);

        SubCollectionEditor editor = new(typeof(List<int>));
        object value = new();
        Assert.Same(value, editor.EditValue(mockContext.Object, mockServiceProvider.Object, value));
        Assert.Same(mockContext.Object, editor.Context);

        editor.ShowHelp();
    }

    private class SubCollectionEditor : CollectionEditor
    {
        public SubCollectionEditor(Type type) : base(type)
        {
        }

        public new Type CollectionItemType => base.CollectionItemType;

        public new Type CollectionType => base.CollectionType;

        public new ITypeDescriptorContext Context => base.Context;

        public new string HelpTopic => base.HelpTopic;

        public new Type[] NewItemTypes => base.NewItemTypes;

        public new void CancelChanges() => base.CancelChanges();

        public new bool CanRemoveInstance(object value) => base.CanRemoveInstance(value);

        public new bool CanSelectMultipleInstances() => base.CanSelectMultipleInstances();

        public new Form CreateCollectionForm() => base.CreateCollectionForm();

        public new Type CreateCollectionItemType() => base.CreateCollectionItemType();

        public new object CreateInstance(Type itemType) => base.CreateInstance(itemType);

        public new Type[] CreateNewItemTypes() => base.CreateNewItemTypes();

        public new void DestroyInstance(object instance) => base.DestroyInstance(instance);

        public new string GetDisplayText(object value) => base.GetDisplayText(value);

        public new object[] GetItems(object editValue) => base.GetItems(editValue);

        public new object GetService(Type serviceType) => base.GetService(serviceType);

        public new IList GetObjectsFromInstance(object instance) => base.GetObjectsFromInstance(instance);

        public new object SetItems(object editValue, object[] value) => base.SetItems(editValue, value);

        public new void ShowHelp() => base.ShowHelp();
    }

    private class ClassWithItem
    {
        public int Item { get; set; }
    }

    private class ClassWithPrivateItem
    {
#pragma warning disable IDE0051 // Remove unused private members
        private int Item { get; set; }
#pragma warning restore IDE0051
    }

#pragma warning disable CA1052 // Static holder types should be Static or NotInheritable
    private class ClassWithStaticItem
#pragma warning restore CA1052 // Static holder types should be Static or NotInheritable
    {
        public static int Item { get; set; }
    }

    private class ClassWithItems
    {
        public int Items { get; set; }
    }

    private class ClassWithPrivateItems
    {
#pragma warning disable IDE0051 // Remove unused private members
        private int Items { get; set; }
#pragma warning restore IDE0051
    }

#pragma warning disable CA1052 // Static holder types should be Static or NotInheritable
    private class ClassWithStaticItems
#pragma warning restore CA1052 // Static holder types should be Static or NotInheritable
    {
        public static int Items { get; set; }
    }

    private class ClassWithStringName
    {
        public string Name { get; set; }

        public override string ToString() => nameof(ClassWithStringName);
    }

    private class ClassWithNonStringName
    {
        public int Name { get; set; }

        public override string ToString() => nameof(ClassWithNonStringName);
    }

    private class ClassWithNullToString
    {
        public int Name { get; set; }

        public override string ToString() => null;
    }

    [DefaultProperty(nameof(DefaultProperty))]
    private class ClassWithStringDefaultProperty
    {
        public string DefaultProperty { get; set; }

        public override string ToString() => nameof(ClassWithStringDefaultProperty);
    }

    [DefaultProperty(nameof(DefaultProperty))]
    private class ClassWithNonStringDefaultProperty
    {
        public int DefaultProperty { get; set; }

        public override string ToString() => nameof(ClassWithNonStringDefaultProperty);
    }

    [DefaultProperty("NoSuchProperty")]
    private class ClassWithNoSuchDefaultProperty
    {
        public string DefaultProperty { get; set; }

        public override string ToString() => nameof(ClassWithNoSuchDefaultProperty);
    }
}
