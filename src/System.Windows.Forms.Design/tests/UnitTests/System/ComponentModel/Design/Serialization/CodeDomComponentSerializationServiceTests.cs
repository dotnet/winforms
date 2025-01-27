// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

// #define DUMP_STATE

using System.CodeDom;
using System.Collections;
using System.Reflection;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Windows.Forms.Design;
using Moq;
using System.Windows.Forms.TestUtilities;
using CodeDomComponentSerializationState = System.ComponentModel.Design.Serialization.CodeDomComponentSerializationService.CodeDomComponentSerializationState;

namespace System.ComponentModel.Design.Serialization.Tests;

public class CodeDomComponentSerializationServiceTests
{
    private Mock<ISite> GetDefaultMockSite(string name)
    {
        Mock<ISite> mockSite = new(MockBehavior.Strict);
        mockSite
            .Setup(s => s.Name)
            .Returns(name);
        mockSite
            .Setup(s => s.Container)
            .Returns(default(Container));
        mockSite
            .Setup(s => s.GetService(typeof(IDictionaryService)))
            .Returns(null);
        mockSite
            .Setup(s => s.GetService(typeof(IExtenderListService)))
            .Returns(null);
        mockSite
            .Setup(s => s.GetService(typeof(ITypeDescriptorFilterService)))
            .Returns(null);

        return mockSite;
    }

    public static IEnumerable<object[]> Ctor_IServiceProvider_TestData()
    {
        yield return new object[] { null };
        yield return new object[] { new Mock<IServiceProvider>(MockBehavior.Strict).Object };
    }

    [Theory]
    [MemberData(nameof(Ctor_IServiceProvider_TestData))]
    public void Ctor_IServiceProvider(IServiceProvider provider)
    {
        new CodeDomComponentSerializationService(provider);
    }

    private static Dictionary<string, CodeDomComponentSerializationState> GetState(SerializationInfo info)
    {
        return Assert.IsType<Dictionary<string, CodeDomComponentSerializationState>>(info.GetValue("State", typeof(Dictionary<string, CodeDomComponentSerializationState>)));
    }

    private static void AssertNullState(SerializationInfo info)
    {
        Assert.Null(info.GetValue("State", typeof(Dictionary<string, CodeDomComponentSerializationState>)));
    }

    private static void AssertAllNonCodeFieldsArNull(CodeDomComponentSerializationState state)
    {
        Assert.Null(state.Context);
        Assert.Null(state.Events);
        Assert.Null(state.Modifier);
        Assert.Null(state.Properties);
        Assert.Null(state.Resources);
    }

    [Fact]
    public void CreateStore_Invoke_ReturnsExpected()
    {
        CodeDomComponentSerializationService service = new();
        SerializationStore store = service.CreateStore();
        Assert.Empty(store.Errors);
        Assert.NotSame(store.Errors, store.Errors);
    }

    public static IEnumerable<object[]> CreateStore_ServiceProvider_TestData()
    {
        yield return new object[] { null };
        yield return new object[] { new() };
        yield return new object[] { new Mock<IDesignerSerializationManager>(MockBehavior.Strict) };
    }

    [Theory]
    [MemberData(nameof(CreateStore_ServiceProvider_TestData))]
    public void CreateStore_CloseWithProviderMultipleTimes_Success(object result)
    {
        Mock<IServiceProvider> mockServiceProvider = new(MockBehavior.Strict);
        mockServiceProvider
            .Setup(p => p.GetService(typeof(IDesignerSerializationManager)))
            .Returns(result);
        mockServiceProvider
            .Setup(p => p.GetService(typeof(IDesignerHost)))
            .Returns(null);
        CodeDomComponentSerializationService service = new(mockServiceProvider.Object);
        SerializationStore store = service.CreateStore();
        store.Close();
        Assert.Empty(store.Errors);
        mockServiceProvider.Verify(p => p.GetService(typeof(IDesignerSerializationManager)), Times.Once());

        store.Close();
        Assert.Empty(store.Errors);
        mockServiceProvider.Verify(p => p.GetService(typeof(IDesignerSerializationManager)), Times.Once());
    }

    [Fact]
    public void CreateStore_CloseSerialize_Success()
    {
        CodeDomComponentSerializationService service = new();
        SerializationStore store = service.CreateStore();
        ISerializable serializable = Assert.IsAssignableFrom<ISerializable>(store);
        var mockSite1 = GetDefaultMockSite("name1");
        DataClass value1 = new()
        {
            IntValue = 1,
            StringValue = "Value",
            Site = mockSite1.Object
        };
        var mockSite2 = GetDefaultMockSite("name2");
        DataClass value2 = new()
        {
            IntValue = 2,
            StringValue = "OtherValue",
            Site = mockSite2.Object
        };
        service.Serialize(store, value1);
        service.Serialize(store, value2);
        store.Close();
        Assert.Empty(store.Errors);

        SerializationInfo info = new(store.GetType(), new FormatterConverter());
        serializable.GetObjectData(info, default);

        Dictionary<string, CodeDomComponentSerializationState> state = GetState(info);
        Assert.Equal(2, state.Count);
        CodeDomComponentSerializationState valueState1 = state["name1"];
        CodeDomHelpers.AssertEqualCodeStatementCollection(new CodeStatementCollection(
        [
            new CodeVariableDeclarationStatement(typeof(DataClass), "name1"),
            new CodeAssignStatement(new CodeVariableReferenceExpression("name1"), new CodeObjectCreateExpression(typeof(DataClass))),
            new CodeCommentStatement(string.Empty),
            new CodeCommentStatement("name1"),
            new CodeCommentStatement(string.Empty),
            new CodeAssignStatement(new CodePropertyReferenceExpression(new CodeVariableReferenceExpression("name1"), "DefaultStringValue"), new CodePrimitiveExpression(null)),
            new CodeAssignStatement(new CodePropertyReferenceExpression(new CodeVariableReferenceExpression("name1"), "IntValue"), new CodePrimitiveExpression(1)),
            new CodeAssignStatement(new CodePropertyReferenceExpression(new CodeVariableReferenceExpression("name1"), "StringValue"), new CodePrimitiveExpression("Value"))
        ]), Assert.IsType<CodeStatementCollection>(valueState1.Code));
        AssertAllNonCodeFieldsArNull(valueState1);

        CodeDomComponentSerializationState valueState2 = state["name2"];
        CodeDomHelpers.AssertEqualCodeStatementCollection(new CodeStatementCollection(
        [
            new CodeVariableDeclarationStatement(typeof(DataClass), "name2"),
            new CodeAssignStatement(new CodeVariableReferenceExpression("name2"), new CodeObjectCreateExpression(typeof(DataClass))),
            new CodeCommentStatement(string.Empty),
            new CodeCommentStatement("name2"),
            new CodeCommentStatement(string.Empty),
            new CodeAssignStatement(new CodePropertyReferenceExpression(new CodeVariableReferenceExpression("name2"), "DefaultStringValue"), new CodePrimitiveExpression(null)),
            new CodeAssignStatement(new CodePropertyReferenceExpression(new CodeVariableReferenceExpression("name2"), "IntValue"), new CodePrimitiveExpression(2)),
            new CodeAssignStatement(new CodePropertyReferenceExpression(new CodeVariableReferenceExpression("name2"), "StringValue"), new CodePrimitiveExpression("OtherValue"))
        ]), Assert.IsType<CodeStatementCollection>(valueState2.Code));
        AssertAllNonCodeFieldsArNull(valueState2);

        List<string> names = Assert.IsType<List<string>>(info.GetValue("Names", typeof(List<string>)));
        Assert.Equal(["name1", "name2"], names);

        AssemblyName[] assemblies = Assert.IsType<AssemblyName[]>(info.GetValue("Assemblies", typeof(AssemblyName[])));
        Assert.Equal(typeof(DataClass).Assembly.GetName(true).FullName, Assert.Single(assemblies).FullName);

        Assert.Null(info.GetValue("Resources", typeof(Hashtable)));
        Assert.Empty(Assert.IsType<List<string>>(info.GetValue("Shim", typeof(List<string>))));
    }

    public static IEnumerable<object[]> CreateStore_CloseSerializeWithInvalidProvider_TestData()
    {
        yield return new object[] { null };
        yield return new object[] { new() };
    }

    [Theory]
    [MemberData(nameof(CreateStore_CloseSerializeWithInvalidProvider_TestData))]
    public void CreateStore_CloseSerializeWithInvalidProvider_Success(object result)
    {
        Mock<IServiceProvider> mockServiceProvider = new(MockBehavior.Strict);
        mockServiceProvider
            .Setup(p => p.GetService(typeof(IDesignerSerializationManager)))
            .Returns(result)
            .Verifiable();
        mockServiceProvider
            .Setup(p => p.GetService(typeof(IDesignerHost)))
            .Returns(result)
            .Verifiable();
        mockServiceProvider
            .Setup(p => p.GetService(typeof(ITypeResolutionService)))
            .Returns(result);
        mockServiceProvider
            .Setup(p => p.GetService(typeof(IEventBindingService)))
            .Returns(result)
            .Verifiable();
        mockServiceProvider
            .Setup(p => p.GetService(typeof(ComponentCache)))
            .Returns(result)
            .Verifiable();
        mockServiceProvider
            .Setup(p => p.GetService(typeof(IServiceContainer)))
            .Returns(result)
            .Verifiable();
        mockServiceProvider
            .Setup(p => p.GetService(typeof(MemberRelationshipService)))
            .Returns(result)
            .Verifiable();
        mockServiceProvider
            .Setup(p => p.GetService(typeof(TypeDescriptionProviderService)))
            .Returns(result);
        CodeDomComponentSerializationService service = new(mockServiceProvider.Object);
        SerializationStore store = service.CreateStore();
        ISerializable serializable = Assert.IsAssignableFrom<ISerializable>(store);
        var mockSite = GetDefaultMockSite("name");
        DataClass value = new()
        {
            IntValue = 1,
            StringValue = "Value",
            Site = mockSite.Object
        };
        service.Serialize(store, value);
        store.Close();
        Assert.Empty(store.Errors);
        mockServiceProvider.Verify(p => p.GetService(typeof(IDesignerSerializationManager)), Times.Exactly(2));
        mockServiceProvider.Verify(p => p.GetService(typeof(IDesignerHost)), Times.Exactly(2));
        mockServiceProvider.Verify(p => p.GetService(typeof(ComponentCache)), Times.Once());
        mockServiceProvider.Verify(p => p.GetService(typeof(IServiceContainer)), Times.Once());
        mockServiceProvider.Verify(p => p.GetService(typeof(MemberRelationshipService)), Times.Exactly(3));

        SerializationInfo info = new(store.GetType(), new FormatterConverter());
        serializable.GetObjectData(info, default);

        Dictionary<string, CodeDomComponentSerializationState> state = GetState(info);
        CodeDomComponentSerializationState valueState = state["name"];
        CodeDomHelpers.AssertEqualCodeStatementCollection(new CodeStatementCollection(
        [
            new CodeVariableDeclarationStatement(typeof(DataClass), "name"),
            new CodeAssignStatement(new CodeVariableReferenceExpression("name"), new CodeObjectCreateExpression(typeof(DataClass))),
            new CodeCommentStatement(string.Empty),
            new CodeCommentStatement("name"),
            new CodeCommentStatement(string.Empty),
            new CodeAssignStatement(new CodePropertyReferenceExpression(new CodeVariableReferenceExpression("name"), "DefaultStringValue"), new CodePrimitiveExpression(null)),
            new CodeAssignStatement(new CodePropertyReferenceExpression(new CodeVariableReferenceExpression("name"), "IntValue"), new CodePrimitiveExpression(1)),
            new CodeAssignStatement(new CodePropertyReferenceExpression(new CodeVariableReferenceExpression("name"), "StringValue"), new CodePrimitiveExpression("Value"))
        ]), Assert.IsType<CodeStatementCollection>(valueState.Code));
        AssertAllNonCodeFieldsArNull(valueState);

        List<string> names = Assert.IsType<List<string>>(info.GetValue("Names", typeof(List<string>)));
        Assert.Equal("name", Assert.Single(names));

        AssemblyName[] assemblies = Assert.IsType<AssemblyName[]>(info.GetValue("Assemblies", typeof(AssemblyName[])));
        Assert.Equal(typeof(DataClass).Assembly.GetName(true).FullName, Assert.Single(assemblies).FullName);

        Assert.Null(info.GetValue("Resources", typeof(Hashtable)));
        Assert.Empty(Assert.IsType<List<string>>(info.GetValue("Shim", typeof(List<string>))));
    }

    public static IEnumerable<object[]> CreateStore_CloseSerializeWithValidProvider_TestData()
    {
        yield return new object[] { null, null };
        yield return new object[] { new(), new() };

        Mock<IComponentChangeService> mockComponentChangeService = new(MockBehavior.Strict);
        yield return new object[] { mockComponentChangeService.Object, new WindowsFormsDesignerOptionService() };
    }

    [Theory]
    [MemberData(nameof(CreateStore_CloseSerializeWithValidProvider_TestData))]
    public void CreateStore_CloseSerializeWithValidProvider_Success(object componentChangeService, object designerOptionService)
    {
        DesignerSerializationManager manager = new();
        ServiceContainer container = new();
        Mock<IServiceProvider> mockServiceProvider = new(MockBehavior.Strict);
        mockServiceProvider
            .Setup(p => p.GetService(typeof(IDesignerSerializationManager)))
            .Returns(manager)
            .Verifiable();
        mockServiceProvider
            .Setup(p => p.GetService(typeof(IDesignerHost)))
            .Returns(null)
            .Verifiable();
        mockServiceProvider
            .Setup(p => p.GetService(typeof(ITypeResolutionService)))
            .Returns(null);
        mockServiceProvider
            .Setup(p => p.GetService(typeof(IEventBindingService)))
            .Returns(null);
        mockServiceProvider
            .Setup(p => p.GetService(typeof(ComponentCache)))
            .Returns(null)
            .Verifiable();
        mockServiceProvider
            .Setup(p => p.GetService(typeof(IServiceContainer)))
            .Returns(container)
            .Verifiable();
        mockServiceProvider
            .Setup(p => p.GetService(typeof(MemberRelationshipService)))
            .Returns(null)
            .Verifiable();
        mockServiceProvider
            .Setup(p => p.GetService(typeof(TypeDescriptionProviderService)))
            .Returns(null);
        mockServiceProvider
            .Setup(p => p.GetService(typeof(IComponentChangeService)))
            .Returns(componentChangeService)
            .Verifiable();
        mockServiceProvider
            .Setup(p => p.GetService(typeof(DesignerOptionService)))
            .Returns(designerOptionService)
            .Verifiable();
        CodeDomComponentSerializationService service = new(mockServiceProvider.Object);
        SerializationStore store = service.CreateStore();
        ISerializable serializable = Assert.IsAssignableFrom<ISerializable>(store);
        var mockSite = GetDefaultMockSite("name");
        DataClass value = new()
        {
            IntValue = 1,
            StringValue = "Value",
            Site = mockSite.Object
        };
        service.Serialize(store, value);
        store.Close();
        Assert.Empty(store.Errors);
        mockServiceProvider.Verify(p => p.GetService(typeof(IDesignerSerializationManager)), Times.Exactly(2));
        mockServiceProvider.Verify(p => p.GetService(typeof(IDesignerHost)), Times.Exactly(2));
        mockServiceProvider.Verify(p => p.GetService(typeof(ComponentCache)), Times.Once());
        mockServiceProvider.Verify(p => p.GetService(typeof(IServiceContainer)), Times.Once());
        mockServiceProvider.Verify(p => p.GetService(typeof(MemberRelationshipService)), Times.Exactly(3));
        mockServiceProvider.Verify(p => p.GetService(typeof(IComponentChangeService)), Times.Once());
        mockServiceProvider.Verify(p => p.GetService(typeof(DesignerOptionService)), Times.Once());

        SerializationInfo info = new(store.GetType(), new FormatterConverter());
        serializable.GetObjectData(info, default);

        Dictionary<string, CodeDomComponentSerializationState> state = GetState(info);
        CodeDomComponentSerializationState valueState = state["name"];
        CodeDomHelpers.AssertEqualCodeStatementCollection(new CodeStatementCollection(
        [
            new CodeVariableDeclarationStatement(typeof(DataClass), "name"),
            new CodeAssignStatement(new CodeVariableReferenceExpression("name"), new CodeObjectCreateExpression(typeof(DataClass))),
            new CodeCommentStatement(string.Empty),
            new CodeCommentStatement("name"),
            new CodeCommentStatement(string.Empty),
            new CodeAssignStatement(new CodePropertyReferenceExpression(new CodeVariableReferenceExpression("name"), "DefaultStringValue"), new CodePrimitiveExpression(null)),
            new CodeAssignStatement(new CodePropertyReferenceExpression(new CodeVariableReferenceExpression("name"), "IntValue"), new CodePrimitiveExpression(1)),
            new CodeAssignStatement(new CodePropertyReferenceExpression(new CodeVariableReferenceExpression("name"), "StringValue"), new CodePrimitiveExpression("Value"))
        ]), Assert.IsType<CodeStatementCollection>(valueState.Code));
        AssertAllNonCodeFieldsArNull(valueState);

        List<string> names = Assert.IsType<List<string>>(info.GetValue("Names", typeof(List<string>)));
        Assert.Equal("name", Assert.Single(names));

        AssemblyName[] assemblies = Assert.IsType<AssemblyName[]>(info.GetValue("Assemblies", typeof(AssemblyName[])));
        Assert.Equal(typeof(DataClass).Assembly.GetName(true).FullName, Assert.Single(assemblies).FullName);

        Assert.Null(info.GetValue("Resources", typeof(Hashtable)));
        Assert.Empty(Assert.IsType<List<string>>(info.GetValue("Shim", typeof(List<string>))));
    }

    [Fact]
    public void CreateStore_CloseSerializeAbsolute_Success()
    {
        CodeDomComponentSerializationService service = new();
        SerializationStore store = service.CreateStore();
        ISerializable serializable = Assert.IsAssignableFrom<ISerializable>(store);
        var mockSite = GetDefaultMockSite("name");
        DataClass value = new()
        {
            IntValue = 1,
            StringValue = "Value",
            Site = mockSite.Object
        };
        service.SerializeAbsolute(store, value);
        store.Close();
        Assert.Empty(store.Errors);

        SerializationInfo info = new(store.GetType(), new FormatterConverter());
        serializable.GetObjectData(info, default);

        Dictionary<string, CodeDomComponentSerializationState> state = GetState(info);
        CodeDomComponentSerializationState valueState = state["name"];
        CodeDomHelpers.AssertEqualCodeStatementCollection(new CodeStatementCollection(
        [
            new CodeVariableDeclarationStatement(typeof(DataClass), "name"),
            new CodeAssignStatement(new CodeVariableReferenceExpression("name"), new CodeObjectCreateExpression(typeof(DataClass))),
            new CodeCommentStatement(string.Empty),
            new CodeCommentStatement("name"),
            new CodeCommentStatement(string.Empty),
            new CodeAssignStatement(new CodePropertyReferenceExpression(new CodeVariableReferenceExpression("name"), "DefaultStringValue"), new CodePrimitiveExpression(null)),
            new CodeAssignStatement(new CodePropertyReferenceExpression(new CodeVariableReferenceExpression("name"), "IntValue"), new CodePrimitiveExpression(1)),
            new CodeAssignStatement(new CodePropertyReferenceExpression(new CodeVariableReferenceExpression("name"), "StringValue"), new CodePrimitiveExpression("Value"))
        ]), Assert.IsType<CodeStatementCollection>(valueState.Code));
        AssertAllNonCodeFieldsArNull(valueState);

        List<string> names = Assert.IsType<List<string>>(info.GetValue("Names", typeof(List<string>)));
        Assert.Equal("name", Assert.Single(names));

        AssemblyName[] assemblies = Assert.IsType<AssemblyName[]>(info.GetValue("Assemblies", typeof(AssemblyName[])));
        Assert.Equal(typeof(DataClass).Assembly.GetName(true).FullName, Assert.Single(assemblies).FullName);

        Assert.Null(info.GetValue("Resources", typeof(Hashtable)));
        Assert.Empty(Assert.IsType<List<string>>(info.GetValue("Shim", typeof(List<string>))));
    }

    [Fact]
    public void CreateStore_CloseSerializeMember_Success()
    {
        CodeDomComponentSerializationService service = new();
        SerializationStore store = service.CreateStore();
        MemberDescriptor member1 = TypeDescriptor.GetProperties(typeof(DataClass))[nameof(DataClass.IntValue)];
        MemberDescriptor member2 = TypeDescriptor.GetProperties(typeof(DataClass))[nameof(DataClass.DefaultStringValue)];
        MemberDescriptor member3 = TypeDescriptor.GetEvents(typeof(DataClass))[nameof(DataClass.Event)];
        ISerializable serializable = Assert.IsAssignableFrom<ISerializable>(store);
        var mockSite = GetDefaultMockSite("name");
        DataClass value = new()
        {
            IntValue = 1,
            StringValue = "Value",
            Site = mockSite.Object
        };
        service.SerializeMember(store, value, member1);
        service.SerializeMember(store, value, member2);
        service.SerializeMember(store, value, member1);
        service.SerializeMember(store, value, member3);
        store.Close();
        Assert.Empty(store.Errors);

        SerializationInfo info = new(store.GetType(), new FormatterConverter());
        serializable.GetObjectData(info, default);

        Dictionary<string, CodeDomComponentSerializationState> state = GetState(info);
        CodeDomComponentSerializationState valueState = state["name"];
        CodeDomHelpers.AssertEqualCodeStatementCollection(new CodeStatementCollection(
        [
            new CodeAssignStatement(new CodePropertyReferenceExpression(new CodeVariableReferenceExpression("name"), "IntValue"), new CodePrimitiveExpression(1)),
            new CodeAssignStatement(new CodePropertyReferenceExpression(new CodeVariableReferenceExpression("name"), "DefaultStringValue"), new CodePrimitiveExpression(null)),
            new CodeAssignStatement(new CodePropertyReferenceExpression(new CodeVariableReferenceExpression("name"), "IntValue"), new CodePrimitiveExpression(1)),
        ]), Assert.IsType<CodeStatementCollection>(valueState.Code));
        AssertAllNonCodeFieldsArNull(valueState);

        List<string> names = Assert.IsType<List<string>>(info.GetValue("Names", typeof(List<string>)));
        Assert.Equal("name", Assert.Single(names));

        AssemblyName[] assemblies = Assert.IsType<AssemblyName[]>(info.GetValue("Assemblies", typeof(AssemblyName[])));
        Assert.Equal(typeof(DataClass).Assembly.GetName(true).FullName, Assert.Single(assemblies).FullName);

        Assert.Null(info.GetValue("Resources", typeof(Hashtable)));
        Assert.Empty(Assert.IsType<List<string>>(info.GetValue("Shim", typeof(List<string>))));
    }

    [Fact]
    public void CreateStore_CloseSerializeMemberAbsolute_Success()
    {
        CodeDomComponentSerializationService service = new();
        SerializationStore store = service.CreateStore();
        MemberDescriptor member1 = TypeDescriptor.GetProperties(typeof(DataClass))[nameof(DataClass.IntValue)];
        MemberDescriptor member2 = TypeDescriptor.GetProperties(typeof(DataClass))[nameof(DataClass.DefaultStringValue)];
        MemberDescriptor member3 = TypeDescriptor.GetEvents(typeof(DataClass))[nameof(DataClass.Event)];
        ISerializable serializable = Assert.IsAssignableFrom<ISerializable>(store);
        var mockSite = GetDefaultMockSite("name");
        DataClass value = new()
        {
            IntValue = 1,
            StringValue = "Value",
            Site = mockSite.Object
        };
        service.SerializeMemberAbsolute(store, value, member1);
        service.SerializeMemberAbsolute(store, value, member2);
        service.SerializeMemberAbsolute(store, value, member1);
        service.SerializeMemberAbsolute(store, value, member3);
        store.Close();
        Assert.Empty(store.Errors);

        SerializationInfo info = new(store.GetType(), new FormatterConverter());
        serializable.GetObjectData(info, default);

        Dictionary<string, CodeDomComponentSerializationState> state = GetState(info);
        CodeDomComponentSerializationState valueState = state["name"];
        CodeDomHelpers.AssertEqualCodeStatementCollection(new CodeStatementCollection(
        [
            new CodeAssignStatement(new CodePropertyReferenceExpression(new CodeVariableReferenceExpression("name"), "IntValue"), new CodePrimitiveExpression(1)),
            new CodeAssignStatement(new CodePropertyReferenceExpression(new CodeVariableReferenceExpression("name"), "DefaultStringValue"), new CodePrimitiveExpression(null)),
            new CodeAssignStatement(new CodePropertyReferenceExpression(new CodeVariableReferenceExpression("name"), "IntValue"), new CodePrimitiveExpression(1))
        ]), Assert.IsType<CodeStatementCollection>(valueState.Code));
        AssertAllNonCodeFieldsArNull(valueState);

        List<string> names = Assert.IsType<List<string>>(info.GetValue("Names", typeof(List<string>)));
        Assert.Equal("name", Assert.Single(names));

        AssemblyName[] assemblies = Assert.IsType<AssemblyName[]>(info.GetValue("Assemblies", typeof(AssemblyName[])));
        Assert.Equal(typeof(DataClass).Assembly.GetName(true).FullName, Assert.Single(assemblies).FullName);

        Assert.Null(info.GetValue("Resources", typeof(Hashtable)));
        Assert.Empty(Assert.IsType<List<string>>(info.GetValue("Shim", typeof(List<string>))));
    }

    [Fact]
    public void CreateStore_CloseSerializeThenSerializeMember_Success()
    {
        CodeDomComponentSerializationService service = new();
        SerializationStore store = service.CreateStore();
        MemberDescriptor member1 = TypeDescriptor.GetProperties(typeof(DataClass))[nameof(DataClass.IntValue)];
        MemberDescriptor member2 = TypeDescriptor.GetProperties(typeof(DataClass))[nameof(DataClass.DefaultStringValue)];
        ISerializable serializable = Assert.IsAssignableFrom<ISerializable>(store);
        var mockSite = GetDefaultMockSite("name");
        DataClass value = new()
        {
            IntValue = 1,
            StringValue = "Value",
            Site = mockSite.Object
        };
        service.Serialize(store, value);
        service.SerializeMember(store, value, member1);
        service.SerializeMember(store, value, member2);
        store.Close();
        Assert.Empty(store.Errors);

        SerializationInfo info = new(store.GetType(), new FormatterConverter());
        serializable.GetObjectData(info, default);

        Dictionary<string, CodeDomComponentSerializationState> state = GetState(info);
        CodeDomComponentSerializationState valueState = state["name"];
        CodeDomHelpers.AssertEqualCodeStatementCollection(new CodeStatementCollection(
        [
            new CodeVariableDeclarationStatement(typeof(DataClass), "name"),
            new CodeAssignStatement(new CodeVariableReferenceExpression("name"), new CodeObjectCreateExpression(typeof(DataClass))),
            new CodeCommentStatement(string.Empty),
            new CodeCommentStatement("name"),
            new CodeCommentStatement(string.Empty),
            new CodeAssignStatement(new CodePropertyReferenceExpression(new CodeVariableReferenceExpression("name"), "DefaultStringValue"), new CodePrimitiveExpression(null)),
            new CodeAssignStatement(new CodePropertyReferenceExpression(new CodeVariableReferenceExpression("name"), "IntValue"), new CodePrimitiveExpression(1)),
            new CodeAssignStatement(new CodePropertyReferenceExpression(new CodeVariableReferenceExpression("name"), "StringValue"), new CodePrimitiveExpression("Value"))
        ]), Assert.IsType<CodeStatementCollection>(valueState.Code));
        AssertAllNonCodeFieldsArNull(valueState);

        List<string> names = Assert.IsType<List<string>>(info.GetValue("Names", typeof(List<string>)));
        Assert.Equal("name", Assert.Single(names));

        AssemblyName[] assemblies = Assert.IsType<AssemblyName[]>(info.GetValue("Assemblies", typeof(AssemblyName[])));
        Assert.Equal(typeof(DataClass).Assembly.GetName(true).FullName, Assert.Single(assemblies).FullName);

        Assert.Null(info.GetValue("Resources", typeof(Hashtable)));
        Assert.Empty(Assert.IsType<List<string>>(info.GetValue("Shim", typeof(List<string>))));
    }

    [Fact]
    public void CreateStore_CloseSerializeMemberThenSerialize_Success()
    {
        CodeDomComponentSerializationService service = new();
        SerializationStore store = service.CreateStore();
        MemberDescriptor member1 = TypeDescriptor.GetProperties(typeof(DataClass))[nameof(DataClass.IntValue)];
        MemberDescriptor member2 = TypeDescriptor.GetProperties(typeof(DataClass))[nameof(DataClass.DefaultStringValue)];
        ISerializable serializable = Assert.IsAssignableFrom<ISerializable>(store);
        var mockSite = GetDefaultMockSite("name");
        DataClass value = new()
        {
            IntValue = 1,
            StringValue = "Value",
            Site = mockSite.Object
        };
        service.SerializeMember(store, value, member1);
        service.SerializeMember(store, value, member2);
        service.Serialize(store, value);
        store.Close();
        Assert.Empty(store.Errors);

        SerializationInfo info = new(store.GetType(), new FormatterConverter());
        serializable.GetObjectData(info, default);

        Dictionary<string, CodeDomComponentSerializationState> state = GetState(info);
        CodeDomComponentSerializationState valueState = state["name"];
        CodeDomHelpers.AssertEqualCodeStatementCollection(new CodeStatementCollection(
        [
            new CodeVariableDeclarationStatement(typeof(DataClass), "name"),
            new CodeAssignStatement(new CodeVariableReferenceExpression("name"), new CodeObjectCreateExpression(typeof(DataClass))),
            new CodeCommentStatement(string.Empty),
            new CodeCommentStatement("name"),
            new CodeCommentStatement(string.Empty),
            new CodeAssignStatement(new CodePropertyReferenceExpression(new CodeVariableReferenceExpression("name"), "DefaultStringValue"), new CodePrimitiveExpression(null)),
            new CodeAssignStatement(new CodePropertyReferenceExpression(new CodeVariableReferenceExpression("name"), "IntValue"), new CodePrimitiveExpression(1)),
            new CodeAssignStatement(new CodePropertyReferenceExpression(new CodeVariableReferenceExpression("name"), "StringValue"), new CodePrimitiveExpression("Value"))
        ]), Assert.IsType<CodeStatementCollection>(valueState.Code));
        AssertAllNonCodeFieldsArNull(valueState);

        List<string> names = Assert.IsType<List<string>>(info.GetValue("Names", typeof(List<string>)));
        Assert.Equal("name", Assert.Single(names));

        AssemblyName[] assemblies = Assert.IsType<AssemblyName[]>(info.GetValue("Assemblies", typeof(AssemblyName[])));
        Assert.Equal(typeof(DataClass).Assembly.GetName(true).FullName, Assert.Single(assemblies).FullName);

        Assert.Null(info.GetValue("Resources", typeof(Hashtable)));
        Assert.Empty(Assert.IsType<List<string>>(info.GetValue("Shim", typeof(List<string>))));
    }

    [Fact]
    public void CreateStore_CloseSerializeThrows_Success()
    {
        CodeDomComponentSerializationService service = new();
        SerializationStore store = service.CreateStore();
        ISerializable serializable = Assert.IsAssignableFrom<ISerializable>(store);
        Mock<ISite> mockSite = new(MockBehavior.Strict);
        mockSite
            .Setup(s => s.Name)
            .Returns("name");
        mockSite
            .Setup(s => s.GetService(typeof(IDictionaryService)))
            .Throws(new DivideByZeroException());
        DataClass value = new()
        {
            IntValue = 1,
            StringValue = "Value",
            Site = mockSite.Object
        };
        service.Serialize(store, value);
        Assert.Throws<DivideByZeroException>(store.Close);
        Assert.Empty(store.Errors);
    }

    [Fact]
    public void CreateStore_CloseWithoutProviderMultipleTimes_Success()
    {
        CodeDomComponentSerializationService service = new();
        SerializationStore store = service.CreateStore();
        store.Close();
        Assert.Empty(store.Errors);

        store.Close();
        Assert.Empty(store.Errors);
    }

    [Fact]
    public void CreateStore_ISerializableGetObjectDataDefault_Success()
    {
        CodeDomComponentSerializationService service = new();
        SerializationStore store = service.CreateStore();
        ISerializable serializable = Assert.IsAssignableFrom<ISerializable>(store);
        SerializationInfo info = new(store.GetType(), new FormatterConverter());
        serializable.GetObjectData(info, default);

        AssertNullState(info);
        Assert.Empty(Assert.IsType<List<string>>(info.GetValue("Names", typeof(List<string>))));
        Assert.Null(info.GetValue("Assemblies", typeof(AssemblyName[])));
        Assert.Null(info.GetValue("Resources", typeof(Hashtable)));
        Assert.Empty(Assert.IsType<List<string>>(info.GetValue("Shim", typeof(List<string>))));
    }

    [Fact]
    public void CreateStore_ISerializableGetObjectDataSerialized_Success()
    {
        CodeDomComponentSerializationService service = new();
        SerializationStore store = service.CreateStore();
        ISerializable serializable = Assert.IsAssignableFrom<ISerializable>(store);
        object value = new();
        service.Serialize(store, value);

        SerializationInfo info = new(store.GetType(), new FormatterConverter());
        serializable.GetObjectData(info, default);
        AssertNullState(info);
        Assert.NotEmpty(Assert.IsType<List<string>>(info.GetValue("Names", typeof(List<string>))));
        Assert.Null(info.GetValue("Assemblies", typeof(AssemblyName[])));
        Assert.Null(info.GetValue("Resources", typeof(Hashtable)));
        Assert.Empty(Assert.IsType<List<string>>(info.GetValue("Shim", typeof(List<string>))));
    }

    [Fact]
    public void CreateStore_ISerializableGetObjectDataDefaultNullInfo_ThrowsArgumentNullException()
    {
        CodeDomComponentSerializationService service = new();
        SerializationStore store = service.CreateStore();
        ISerializable serializable = Assert.IsAssignableFrom<ISerializable>(store);
        Assert.Throws<ArgumentNullException>("info", () => serializable.GetObjectData(null, default));
    }

    [Theory]
    [BoolData]
    public void LoadStore_SerializedStore_ThrowsSerializationException(bool formatterEnabled)
    {
        using BinaryFormatterScope formatterScope = new(enable: formatterEnabled);
        CodeDomComponentSerializationService service = new();
        SerializationStore store = service.CreateStore();
        using MemoryStream stream = new();
        BinaryFormatter formatter = new();
        if (formatterEnabled)
        {
            Assert.Throws<SerializationException>(() => formatter.Serialize(stream, store));
        }
        else
        {
            Assert.Throws<NotSupportedException>(() => formatter.Serialize(stream, store));
        }
    }

    [Fact]
    public void LoadStore_NullStream_ThrowsPlatformNotSupportedException()
    {
        CodeDomComponentSerializationService service = new();
        Assert.Throws<PlatformNotSupportedException>(() => service.LoadStore(null));
    }

    [Fact]
    public void Serialize_InvokeObject_Success()
    {
        CodeDomComponentSerializationService service = new();
        SerializationStore store = service.CreateStore();
        ISerializable serializable = Assert.IsAssignableFrom<ISerializable>(store);

        object value = new();
        service.Serialize(store, value);
        SerializationInfo info = new(store.GetType(), new FormatterConverter());
        serializable.GetObjectData(info, default);
        AssertNullState(info);
        string nameResult = Assert.IsType<string>(Assert.Single(Assert.IsType<List<string>>(info.GetValue("Names", typeof(List<string>)))));
        Assert.Matches("^object_........_...._...._...._............$", nameResult);
        Assert.Null(info.GetValue("Assemblies", typeof(AssemblyName[])));
        Assert.Null(info.GetValue("Resources", typeof(Hashtable)));
        Assert.Empty(Assert.IsType<List<string>>(info.GetValue("Shim", typeof(List<string>))));

        // Serialize again.
        service.Serialize(store, value);
        info = new SerializationInfo(store.GetType(), new FormatterConverter());
        serializable.GetObjectData(info, default);
        AssertNullState(info);
        nameResult = Assert.IsType<string>(Assert.Single(Assert.IsType<List<string>>(info.GetValue("Names", typeof(List<string>)))));
        Assert.Matches("^object_........_...._...._...._............$", nameResult);
        Assert.Null(info.GetValue("Assemblies", typeof(AssemblyName[])));
        Assert.Null(info.GetValue("Resources", typeof(Hashtable)));
        Assert.Empty(Assert.IsType<List<string>>(info.GetValue("Shim", typeof(List<string>))));
    }

    [Fact]
    public void Serialize_InvokeIComponentWithoutSite_Success()
    {
        CodeDomComponentSerializationService service = new();
        SerializationStore store = service.CreateStore();
        ISerializable serializable = Assert.IsAssignableFrom<ISerializable>(store);

        Mock<IComponent> mockComponent = new(MockBehavior.Strict);
        mockComponent
            .Setup(c => c.Site)
            .Returns<ISite>(null)
            .Verifiable();

        service.Serialize(store, mockComponent.Object);
        mockComponent.Verify(c => c.Site, Times.Once());

        SerializationInfo info = new(store.GetType(), new FormatterConverter());
        serializable.GetObjectData(info, default);
        AssertNullState(info);
        string nameResult = Assert.IsType<string>(Assert.Single(Assert.IsType<List<string>>(info.GetValue("Names", typeof(List<string>)))));
        Assert.Matches("^object_........_...._...._...._............$", nameResult);
        Assert.Null(info.GetValue("Assemblies", typeof(AssemblyName[])));
        Assert.Null(info.GetValue("Resources", typeof(Hashtable)));
        Assert.Empty(Assert.IsType<List<string>>(info.GetValue("Shim", typeof(List<string>))));

        // Serialize again.
        service.Serialize(store, mockComponent.Object);
        mockComponent.Verify(c => c.Site, Times.Once());

        info = new SerializationInfo(store.GetType(), new FormatterConverter());
        serializable.GetObjectData(info, default);
        AssertNullState(info);
        nameResult = Assert.IsType<string>(Assert.Single(Assert.IsType<List<string>>(info.GetValue("Names", typeof(List<string>)))));
        Assert.Matches("^object_........_...._...._...._............$", nameResult);
        Assert.Null(info.GetValue("Assemblies", typeof(AssemblyName[])));
        Assert.Null(info.GetValue("Resources", typeof(Hashtable)));
        Assert.Empty(Assert.IsType<List<string>>(info.GetValue("Shim", typeof(List<string>))));
    }

    [Theory]
    [InlineData(null, 1, "^object_........_...._...._...._............$")]
    [InlineData("", 1, "^object_........_...._...._...._............$")]
    [InlineData("name", 2, "^name$")]
    public void Serialize_InvokeIComponentWithISite_Success(string name, int expectedCallCount, string expectedPattern)
    {
        CodeDomComponentSerializationService service = new();
        SerializationStore store = service.CreateStore();
        ISerializable serializable = Assert.IsAssignableFrom<ISerializable>(store);

        Mock<ISite> mockSite = new(MockBehavior.Strict);
        mockSite
            .Setup(s => s.Name)
            .Returns(name)
            .Verifiable();
        Mock<IComponent> mockComponent = new(MockBehavior.Strict);
        mockComponent
            .Setup(c => c.Site)
            .Returns(mockSite.Object)
            .Verifiable();

        service.Serialize(store, mockComponent.Object);
        mockComponent.Verify(c => c.Site, Times.Once());
        mockSite.Verify(s => s.Name, Times.Exactly(expectedCallCount));

        SerializationInfo info = new(store.GetType(), new FormatterConverter());
        serializable.GetObjectData(info, default);
        AssertNullState(info);
        string nameResult = Assert.IsType<string>(Assert.Single(Assert.IsType<List<string>>(info.GetValue("Names", typeof(List<string>)))));
        Assert.Matches(expectedPattern, nameResult);
        Assert.Null(info.GetValue("Assemblies", typeof(AssemblyName[])));
        Assert.Null(info.GetValue("Resources", typeof(Hashtable)));
        Assert.Empty(Assert.IsType<List<string>>(info.GetValue("Shim", typeof(List<string>))));

        // Serialize again.
        service.Serialize(store, mockComponent.Object);
        mockComponent.Verify(c => c.Site, Times.Once());
        mockSite.Verify(s => s.Name, Times.Exactly(expectedCallCount));

        info = new SerializationInfo(store.GetType(), new FormatterConverter());
        serializable.GetObjectData(info, default);
        AssertNullState(info);
        nameResult = Assert.IsType<string>(Assert.Single(Assert.IsType<List<string>>(info.GetValue("Names", typeof(List<string>)))));
        Assert.Matches(expectedPattern, nameResult);
        Assert.Null(info.GetValue("Assemblies", typeof(AssemblyName[])));
        Assert.Null(info.GetValue("Resources", typeof(Hashtable)));
        Assert.Empty(Assert.IsType<List<string>>(info.GetValue("Shim", typeof(List<string>))));
    }

    [Theory]
    [InlineData(null, 1, null, 1, "^object_........_...._...._...._............$")]
    [InlineData(null, 1, "", 1, "^object_........_...._...._...._............$")]
    [InlineData(null, 1, "name", 2, "^name$")]
    [InlineData("", 1, null, 1, "^object_........_...._...._...._............$")]
    [InlineData("", 1, "", 1, "^object_........_...._...._...._............$")]
    [InlineData("", 1, "name", 2, "^name$")]
    [InlineData("fullName", 2, null, 0, "^fullName$")]
    [InlineData("fullName", 2, "", 0, "^fullName$")]
    [InlineData("fullName", 2, "name", 0, "^fullName$")]
    public void Serialize_InvokeIComponentWithINestedSite_Success(string fullName, int expectedFullNameCallCount, string name, int expectedNameCallCount, string expectedPattern)
    {
        CodeDomComponentSerializationService service = new();
        SerializationStore store = service.CreateStore();
        ISerializable serializable = Assert.IsAssignableFrom<ISerializable>(store);

        Mock<INestedSite> mockNestedSite = new(MockBehavior.Strict);
        mockNestedSite
            .Setup(s => s.Name)
            .Returns(name)
            .Verifiable();
        mockNestedSite
            .Setup(s => s.FullName)
            .Returns(fullName)
            .Verifiable();
        Mock<IComponent> mockComponent = new(MockBehavior.Strict);
        mockComponent
            .Setup(c => c.Site)
            .Returns(mockNestedSite.Object)
            .Verifiable();

        service.Serialize(store, mockComponent.Object);
        mockComponent.Verify(c => c.Site, Times.Once());
        mockNestedSite.Verify(s => s.Name, Times.Exactly(expectedNameCallCount));
        mockNestedSite.Verify(s => s.FullName, Times.Exactly(expectedFullNameCallCount));

        SerializationInfo info = new(store.GetType(), new FormatterConverter());
        serializable.GetObjectData(info, default);
        string nameResult = Assert.IsType<string>(Assert.Single(Assert.IsType<List<string>>(info.GetValue("Names", typeof(List<string>)))));
        Assert.Matches(expectedPattern, nameResult);
        Assert.Null(info.GetValue("Assemblies", typeof(AssemblyName[])));
        Assert.Null(info.GetValue("Resources", typeof(Hashtable)));
        Assert.Empty(Assert.IsType<List<string>>(info.GetValue("Shim", typeof(List<string>))));

        // Serialize again.
        service.Serialize(store, mockComponent.Object);
        mockNestedSite.Verify(s => s.Name, Times.Exactly(expectedNameCallCount));
        mockNestedSite.Verify(s => s.FullName, Times.Exactly(expectedFullNameCallCount));

        info = new SerializationInfo(store.GetType(), new FormatterConverter());
        serializable.GetObjectData(info, default);
        nameResult = Assert.IsType<string>(Assert.Single(Assert.IsType<List<string>>(info.GetValue("Names", typeof(List<string>)))));
        Assert.Matches(expectedPattern, nameResult);
        Assert.Null(info.GetValue("Assemblies", typeof(AssemblyName[])));
        Assert.Null(info.GetValue("Resources", typeof(Hashtable)));
        Assert.Empty(Assert.IsType<List<string>>(info.GetValue("Shim", typeof(List<string>))));
    }

    [Fact]
    public void Serialize_NullStore_ThrowsArgumentNullException()
    {
        CodeDomComponentSerializationService service = new();
        Assert.Throws<ArgumentNullException>("store", () => service.Serialize(null, new object()));
    }

    [Fact]
    public void Serialize_NullValue_ThrowsArgumentNullException()
    {
        CodeDomComponentSerializationService service = new();
        Mock<SerializationStore> mockStore = new(MockBehavior.Strict);
        Assert.Throws<ArgumentNullException>("value", () => service.Serialize(mockStore.Object, null));
    }

    [Fact]
    public void Serialize_InvalidStore_ThrowsInvalidOperationException()
    {
        CodeDomComponentSerializationService service = new();
        Mock<SerializationStore> mockStore = new(MockBehavior.Strict);
        Assert.Throws<InvalidOperationException>(() => service.Serialize(mockStore.Object, new object()));
    }

    [Fact]
    public void Serialize_ClosedStore_ThrowsInvalidOperationException()
    {
        CodeDomComponentSerializationService service = new();
        SerializationStore store = service.CreateStore();
        store.Close();
        Assert.Throws<InvalidOperationException>(() => service.Serialize(store, new object()));
    }

    [Fact]
    public void SerializeAbsolute_InvokeObject_Success()
    {
        CodeDomComponentSerializationService service = new();
        SerializationStore store = service.CreateStore();
        ISerializable serializable = Assert.IsAssignableFrom<ISerializable>(store);

        object value = new();
        service.SerializeAbsolute(store, value);
        SerializationInfo info = new(store.GetType(), new FormatterConverter());
        serializable.GetObjectData(info, default);
        AssertNullState(info);
        string nameResult = Assert.IsType<string>(Assert.Single(Assert.IsType<List<string>>(info.GetValue("Names", typeof(List<string>)))));
        Assert.Matches("^object_........_...._...._...._............$", nameResult);
        Assert.Null(info.GetValue("Assemblies", typeof(AssemblyName[])));
        Assert.Null(info.GetValue("Resources", typeof(Hashtable)));
        Assert.Empty(Assert.IsType<List<string>>(info.GetValue("Shim", typeof(List<string>))));

        // Serialize again.
        service.SerializeAbsolute(store, value);
        info = new SerializationInfo(store.GetType(), new FormatterConverter());
        serializable.GetObjectData(info, default);
        AssertNullState(info);
        nameResult = Assert.IsType<string>(Assert.Single(Assert.IsType<List<string>>(info.GetValue("Names", typeof(List<string>)))));
        Assert.Matches("^object_........_...._...._...._............$", nameResult);
        Assert.Null(info.GetValue("Assemblies", typeof(AssemblyName[])));
        Assert.Null(info.GetValue("Resources", typeof(Hashtable)));
        Assert.Empty(Assert.IsType<List<string>>(info.GetValue("Shim", typeof(List<string>))));
    }

    [Fact]
    public void SerializeAbsolute_InvokeIComponentWithoutSite_Success()
    {
        CodeDomComponentSerializationService service = new();
        SerializationStore store = service.CreateStore();
        ISerializable serializable = Assert.IsAssignableFrom<ISerializable>(store);

        Mock<IComponent> mockComponent = new(MockBehavior.Strict);
        mockComponent
            .Setup(c => c.Site)
            .Returns<ISite>(null)
            .Verifiable();

        service.SerializeAbsolute(store, mockComponent.Object);
        mockComponent.Verify(c => c.Site, Times.Once());

        SerializationInfo info = new(store.GetType(), new FormatterConverter());
        serializable.GetObjectData(info, default);
        AssertNullState(info);
        string nameResult = Assert.IsType<string>(Assert.Single(Assert.IsType<List<string>>(info.GetValue("Names", typeof(List<string>)))));
        Assert.Matches("^object_........_...._...._...._............$", nameResult);
        Assert.Null(info.GetValue("Assemblies", typeof(AssemblyName[])));
        Assert.Null(info.GetValue("Resources", typeof(Hashtable)));
        Assert.Empty(Assert.IsType<List<string>>(info.GetValue("Shim", typeof(List<string>))));

        // Serialize again.
        service.SerializeAbsolute(store, mockComponent.Object);
        mockComponent.Verify(c => c.Site, Times.Once());

        info = new SerializationInfo(store.GetType(), new FormatterConverter());
        serializable.GetObjectData(info, default);
        AssertNullState(info);
        nameResult = Assert.IsType<string>(Assert.Single(Assert.IsType<List<string>>(info.GetValue("Names", typeof(List<string>)))));
        Assert.Matches("^object_........_...._...._...._............$", nameResult);
        Assert.Null(info.GetValue("Assemblies", typeof(AssemblyName[])));
        Assert.Null(info.GetValue("Resources", typeof(Hashtable)));
        Assert.Empty(Assert.IsType<List<string>>(info.GetValue("Shim", typeof(List<string>))));
    }

    [Theory]
    [InlineData(null, 1, "^object_........_...._...._...._............$")]
    [InlineData("", 1, "^object_........_...._...._...._............$")]
    [InlineData("name", 2, "^name$")]
    public void SerializeAbsolute_InvokeIComponentWithISite_Success(string name, int expectedCallCount, string expectedPattern)
    {
        CodeDomComponentSerializationService service = new();
        SerializationStore store = service.CreateStore();
        ISerializable serializable = Assert.IsAssignableFrom<ISerializable>(store);

        Mock<ISite> mockSite = new(MockBehavior.Strict);
        mockSite
            .Setup(s => s.Name)
            .Returns(name)
            .Verifiable();
        Mock<IComponent> mockComponent = new(MockBehavior.Strict);
        mockComponent
            .Setup(c => c.Site)
            .Returns(mockSite.Object)
            .Verifiable();

        service.SerializeAbsolute(store, mockComponent.Object);
        mockComponent.Verify(c => c.Site, Times.Once());
        mockSite.Verify(s => s.Name, Times.Exactly(expectedCallCount));

        SerializationInfo info = new(store.GetType(), new FormatterConverter());
        serializable.GetObjectData(info, default);
        AssertNullState(info);
        string nameResult = Assert.IsType<string>(Assert.Single(Assert.IsType<List<string>>(info.GetValue("Names", typeof(List<string>)))));
        Assert.Matches(expectedPattern, nameResult);
        Assert.Null(info.GetValue("Assemblies", typeof(AssemblyName[])));
        Assert.Null(info.GetValue("Resources", typeof(Hashtable)));
        Assert.Empty(Assert.IsType<List<string>>(info.GetValue("Shim", typeof(List<string>))));

        // Serialize again.
        service.SerializeAbsolute(store, mockComponent.Object);
        mockComponent.Verify(c => c.Site, Times.Once());
        mockSite.Verify(s => s.Name, Times.Exactly(expectedCallCount));

        info = new SerializationInfo(store.GetType(), new FormatterConverter());
        serializable.GetObjectData(info, default);
        AssertNullState(info);
        nameResult = Assert.IsType<string>(Assert.Single(Assert.IsType<List<string>>(info.GetValue("Names", typeof(List<string>)))));
        Assert.Matches(expectedPattern, nameResult);
        Assert.Null(info.GetValue("Assemblies", typeof(AssemblyName[])));
        Assert.Null(info.GetValue("Resources", typeof(Hashtable)));
        Assert.Empty(Assert.IsType<List<string>>(info.GetValue("Shim", typeof(List<string>))));
    }

    [Theory]
    [InlineData(null, 1, null, 1, "^object_........_...._...._...._............$")]
    [InlineData(null, 1, "", 1, "^object_........_...._...._...._............$")]
    [InlineData(null, 1, "name", 2, "^name$")]
    [InlineData("", 1, null, 1, "^object_........_...._...._...._............$")]
    [InlineData("", 1, "", 1, "^object_........_...._...._...._............$")]
    [InlineData("", 1, "name", 2, "^name$")]
    [InlineData("fullName", 2, null, 0, "^fullName$")]
    [InlineData("fullName", 2, "", 0, "^fullName$")]
    [InlineData("fullName", 2, "name", 0, "^fullName$")]
    public void SerializeAbsolute_InvokeIComponentWithINestedSite_Success(string fullName, int expectedFullNameCallCount, string name, int expectedNameCallCount, string expectedPattern)
    {
        CodeDomComponentSerializationService service = new();
        SerializationStore store = service.CreateStore();
        ISerializable serializable = Assert.IsAssignableFrom<ISerializable>(store);

        Mock<INestedSite> mockNestedSite = new(MockBehavior.Strict);
        mockNestedSite
            .Setup(s => s.Name)
            .Returns(name)
            .Verifiable();
        mockNestedSite
            .Setup(s => s.FullName)
            .Returns(fullName)
            .Verifiable();
        Mock<IComponent> mockComponent = new(MockBehavior.Strict);
        mockComponent
            .Setup(c => c.Site)
            .Returns(mockNestedSite.Object)
            .Verifiable();

        service.SerializeAbsolute(store, mockComponent.Object);
        mockComponent.Verify(c => c.Site, Times.Once());
        mockNestedSite.Verify(s => s.Name, Times.Exactly(expectedNameCallCount));
        mockNestedSite.Verify(s => s.FullName, Times.Exactly(expectedFullNameCallCount));

        SerializationInfo info = new(store.GetType(), new FormatterConverter());
        serializable.GetObjectData(info, default);
        string nameResult = Assert.IsType<string>(Assert.Single(Assert.IsType<List<string>>(info.GetValue("Names", typeof(List<string>)))));
        Assert.Matches(expectedPattern, nameResult);
        Assert.Null(info.GetValue("Assemblies", typeof(AssemblyName[])));
        Assert.Null(info.GetValue("Resources", typeof(Hashtable)));
        Assert.Empty(Assert.IsType<List<string>>(info.GetValue("Shim", typeof(List<string>))));

        // Serialize again.
        service.SerializeAbsolute(store, mockComponent.Object);
        mockNestedSite.Verify(s => s.Name, Times.Exactly(expectedNameCallCount));
        mockNestedSite.Verify(s => s.FullName, Times.Exactly(expectedFullNameCallCount));

        info = new SerializationInfo(store.GetType(), new FormatterConverter());
        serializable.GetObjectData(info, default);
        nameResult = Assert.IsType<string>(Assert.Single(Assert.IsType<List<string>>(info.GetValue("Names", typeof(List<string>)))));
        Assert.Matches(expectedPattern, nameResult);
        Assert.Null(info.GetValue("Assemblies", typeof(AssemblyName[])));
        Assert.Null(info.GetValue("Resources", typeof(Hashtable)));
        Assert.Empty(Assert.IsType<List<string>>(info.GetValue("Shim", typeof(List<string>))));
    }

    [Fact]
    public void SerializeAbsolute_NullStore_ThrowsArgumentNullException()
    {
        CodeDomComponentSerializationService service = new();
        Assert.Throws<ArgumentNullException>("store", () => service.SerializeAbsolute(null, new object()));
    }

    [Fact]
    public void SerializeAbsolute_NullValue_ThrowsArgumentNullException()
    {
        CodeDomComponentSerializationService service = new();
        Mock<SerializationStore> mockStore = new(MockBehavior.Strict);
        Assert.Throws<ArgumentNullException>("value", () => service.SerializeAbsolute(mockStore.Object, null));
    }

    [Fact]
    public void SerializeAbsolute_InvalidStore_ThrowsInvalidOperationException()
    {
        CodeDomComponentSerializationService service = new();
        Mock<SerializationStore> mockStore = new(MockBehavior.Strict);
        Assert.Throws<InvalidOperationException>(() => service.SerializeAbsolute(mockStore.Object, new object()));
    }

    [Fact]
    public void SerializeAbsolute_ClosedStore_ThrowsInvalidOperationException()
    {
        CodeDomComponentSerializationService service = new();
        SerializationStore store = service.CreateStore();
        store.Close();
        Assert.Throws<InvalidOperationException>(() => service.SerializeAbsolute(store, new object()));
    }

    [Fact]
    public void SerializeMember_InvokeObject_Success()
    {
        CodeDomComponentSerializationService service = new();
        SerializationStore store = service.CreateStore();
        MemberDescriptor member1 = TypeDescriptor.GetProperties(typeof(DataClass))[nameof(DataClass.IntValue)];
        MemberDescriptor member2 = TypeDescriptor.GetProperties(typeof(DataClass))[nameof(DataClass.StringValue)];
        ISerializable serializable = Assert.IsAssignableFrom<ISerializable>(store);

        object value = new();
        service.SerializeMember(store, value, member1);
        SerializationInfo info = new(store.GetType(), new FormatterConverter());
        serializable.GetObjectData(info, default);
        AssertNullState(info);
        string nameResult = Assert.IsType<string>(Assert.Single(Assert.IsType<List<string>>(info.GetValue("Names", typeof(List<string>)))));
        Assert.Matches("^object_........_...._...._...._............$", nameResult);
        Assert.Null(info.GetValue("Assemblies", typeof(AssemblyName[])));
        Assert.Null(info.GetValue("Resources", typeof(Hashtable)));
        Assert.Empty(Assert.IsType<List<string>>(info.GetValue("Shim", typeof(List<string>))));

        // Serialize again.
        service.SerializeMember(store, value, member1);
        info = new SerializationInfo(store.GetType(), new FormatterConverter());
        serializable.GetObjectData(info, default);
        AssertNullState(info);
        nameResult = Assert.IsType<string>(Assert.Single(Assert.IsType<List<string>>(info.GetValue("Names", typeof(List<string>)))));
        Assert.Matches("^object_........_...._...._...._............$", nameResult);
        Assert.Null(info.GetValue("Assemblies", typeof(AssemblyName[])));
        Assert.Null(info.GetValue("Resources", typeof(Hashtable)));
        Assert.Empty(Assert.IsType<List<string>>(info.GetValue("Shim", typeof(List<string>))));

        // Serialize another.
        service.SerializeMember(store, value, member2);
        info = new SerializationInfo(store.GetType(), new FormatterConverter());
        serializable.GetObjectData(info, default);
        AssertNullState(info);
        nameResult = Assert.IsType<string>(Assert.Single(Assert.IsType<List<string>>(info.GetValue("Names", typeof(List<string>)))));
        Assert.Matches("^object_........_...._...._...._............$", nameResult);
        Assert.Null(info.GetValue("Assemblies", typeof(AssemblyName[])));
        Assert.Null(info.GetValue("Resources", typeof(Hashtable)));
        Assert.Empty(Assert.IsType<List<string>>(info.GetValue("Shim", typeof(List<string>))));
    }

    [Fact]
    public void SerializeMember_InvokeIComponentWithoutSite_Success()
    {
        CodeDomComponentSerializationService service = new();
        SerializationStore store = service.CreateStore();
        MemberDescriptor member1 = TypeDescriptor.GetProperties(typeof(DataClass))[nameof(DataClass.IntValue)];
        MemberDescriptor member2 = TypeDescriptor.GetProperties(typeof(DataClass))[nameof(DataClass.StringValue)];
        ISerializable serializable = Assert.IsAssignableFrom<ISerializable>(store);

        Mock<IComponent> mockComponent = new(MockBehavior.Strict);
        mockComponent
            .Setup(c => c.Site)
            .Returns<ISite>(null)
            .Verifiable();

        service.SerializeMember(store, mockComponent.Object, member1);
        mockComponent.Verify(c => c.Site, Times.Once());

        SerializationInfo info = new(store.GetType(), new FormatterConverter());
        serializable.GetObjectData(info, default);
        AssertNullState(info);
        string nameResult = Assert.IsType<string>(Assert.Single(Assert.IsType<List<string>>(info.GetValue("Names", typeof(List<string>)))));
        Assert.Matches("^object_........_...._...._...._............$", nameResult);
        Assert.Null(info.GetValue("Assemblies", typeof(AssemblyName[])));
        Assert.Null(info.GetValue("Resources", typeof(Hashtable)));
        Assert.Empty(Assert.IsType<List<string>>(info.GetValue("Shim", typeof(List<string>))));

        // Serialize again.
        service.SerializeMember(store, mockComponent.Object, member1);
        mockComponent.Verify(c => c.Site, Times.Once());

        info = new SerializationInfo(store.GetType(), new FormatterConverter());
        serializable.GetObjectData(info, default);
        AssertNullState(info);
        nameResult = Assert.IsType<string>(Assert.Single(Assert.IsType<List<string>>(info.GetValue("Names", typeof(List<string>)))));
        Assert.Matches("^object_........_...._...._...._............$", nameResult);
        Assert.Null(info.GetValue("Assemblies", typeof(AssemblyName[])));
        Assert.Null(info.GetValue("Resources", typeof(Hashtable)));
        Assert.Empty(Assert.IsType<List<string>>(info.GetValue("Shim", typeof(List<string>))));

        // Serialize another.
        service.SerializeMember(store, mockComponent.Object, member2);
        mockComponent.Verify(c => c.Site, Times.Once());

        info = new SerializationInfo(store.GetType(), new FormatterConverter());
        serializable.GetObjectData(info, default);
        AssertNullState(info);
        nameResult = Assert.IsType<string>(Assert.Single(Assert.IsType<List<string>>(info.GetValue("Names", typeof(List<string>)))));
        Assert.Matches("^object_........_...._...._...._............$", nameResult);
        Assert.Null(info.GetValue("Assemblies", typeof(AssemblyName[])));
        Assert.Null(info.GetValue("Resources", typeof(Hashtable)));
        Assert.Empty(Assert.IsType<List<string>>(info.GetValue("Shim", typeof(List<string>))));
    }

    [Theory]
    [InlineData(null, 1, "^object_........_...._...._...._............$")]
    [InlineData("", 1, "^object_........_...._...._...._............$")]
    [InlineData("name", 2, "^name$")]
    public void SerializeMember_InvokeIComponentWithISite_Success(string name, int expectedCallCount, string expectedPattern)
    {
        CodeDomComponentSerializationService service = new();
        SerializationStore store = service.CreateStore();
        MemberDescriptor member1 = TypeDescriptor.GetProperties(typeof(DataClass))[nameof(DataClass.IntValue)];
        MemberDescriptor member2 = TypeDescriptor.GetProperties(typeof(DataClass))[nameof(DataClass.StringValue)];
        ISerializable serializable = Assert.IsAssignableFrom<ISerializable>(store);

        Mock<ISite> mockSite = new(MockBehavior.Strict);
        mockSite
            .Setup(s => s.Name)
            .Returns(name)
            .Verifiable();
        Mock<IComponent> mockComponent = new(MockBehavior.Strict);
        mockComponent
            .Setup(c => c.Site)
            .Returns(mockSite.Object)
            .Verifiable();

        service.SerializeMember(store, mockComponent.Object, member1);
        mockComponent.Verify(c => c.Site, Times.Once());
        mockSite.Verify(s => s.Name, Times.Exactly(expectedCallCount));

        SerializationInfo info = new(store.GetType(), new FormatterConverter());
        serializable.GetObjectData(info, default);
        AssertNullState(info);
        string nameResult = Assert.IsType<string>(Assert.Single(Assert.IsType<List<string>>(info.GetValue("Names", typeof(List<string>)))));
        Assert.Matches(expectedPattern, nameResult);
        Assert.Null(info.GetValue("Assemblies", typeof(AssemblyName[])));
        Assert.Null(info.GetValue("Resources", typeof(Hashtable)));
        Assert.Empty(Assert.IsType<List<string>>(info.GetValue("Shim", typeof(List<string>))));

        // Serialize again.
        service.SerializeMember(store, mockComponent.Object, member1);
        mockComponent.Verify(c => c.Site, Times.Once());
        mockSite.Verify(s => s.Name, Times.Exactly(expectedCallCount));

        info = new SerializationInfo(store.GetType(), new FormatterConverter());
        serializable.GetObjectData(info, default);
        AssertNullState(info);
        nameResult = Assert.IsType<string>(Assert.Single(Assert.IsType<List<string>>(info.GetValue("Names", typeof(List<string>)))));
        Assert.Matches(expectedPattern, nameResult);
        Assert.Null(info.GetValue("Assemblies", typeof(AssemblyName[])));
        Assert.Null(info.GetValue("Resources", typeof(Hashtable)));
        Assert.Empty(Assert.IsType<List<string>>(info.GetValue("Shim", typeof(List<string>))));

        // Serialize another.
        service.SerializeMember(store, mockComponent.Object, member2);
        mockComponent.Verify(c => c.Site, Times.Once());
        mockSite.Verify(s => s.Name, Times.Exactly(expectedCallCount));

        info = new SerializationInfo(store.GetType(), new FormatterConverter());
        serializable.GetObjectData(info, default);
        AssertNullState(info);
        nameResult = Assert.IsType<string>(Assert.Single(Assert.IsType<List<string>>(info.GetValue("Names", typeof(List<string>)))));
        Assert.Matches(expectedPattern, nameResult);
        Assert.Null(info.GetValue("Assemblies", typeof(AssemblyName[])));
        Assert.Null(info.GetValue("Resources", typeof(Hashtable)));
        Assert.Empty(Assert.IsType<List<string>>(info.GetValue("Shim", typeof(List<string>))));
    }

    [Theory]
    [InlineData(null, 1, null, 1, "^object_........_...._...._...._............$")]
    [InlineData(null, 1, "", 1, "^object_........_...._...._...._............$")]
    [InlineData(null, 1, "name", 2, "^name$")]
    [InlineData("", 1, null, 1, "^object_........_...._...._...._............$")]
    [InlineData("", 1, "", 1, "^object_........_...._...._...._............$")]
    [InlineData("", 1, "name", 2, "^name$")]
    [InlineData("fullName", 2, null, 0, "^fullName$")]
    [InlineData("fullName", 2, "", 0, "^fullName$")]
    [InlineData("fullName", 2, "name", 0, "^fullName$")]
    public void SerializeMember_InvokeIComponentWithINestedSite_Success(string fullName, int expectedFullNameCallCount, string name, int expectedNameCallCount, string expectedPattern)
    {
        CodeDomComponentSerializationService service = new();
        SerializationStore store = service.CreateStore();
        MemberDescriptor member1 = TypeDescriptor.GetProperties(typeof(DataClass))[nameof(DataClass.IntValue)];
        MemberDescriptor member2 = TypeDescriptor.GetProperties(typeof(DataClass))[nameof(DataClass.StringValue)];
        ISerializable serializable = Assert.IsAssignableFrom<ISerializable>(store);

        Mock<INestedSite> mockNestedSite = new(MockBehavior.Strict);
        mockNestedSite
            .Setup(s => s.Name)
            .Returns(name)
            .Verifiable();
        mockNestedSite
            .Setup(s => s.FullName)
            .Returns(fullName)
            .Verifiable();
        Mock<IComponent> mockComponent = new(MockBehavior.Strict);
        mockComponent
            .Setup(c => c.Site)
            .Returns(mockNestedSite.Object)
            .Verifiable();

        service.SerializeMember(store, mockComponent.Object, member1);
        mockComponent.Verify(c => c.Site, Times.Once());
        mockNestedSite.Verify(s => s.Name, Times.Exactly(expectedNameCallCount));
        mockNestedSite.Verify(s => s.FullName, Times.Exactly(expectedFullNameCallCount));

        SerializationInfo info = new(store.GetType(), new FormatterConverter());
        serializable.GetObjectData(info, default);
        string nameResult = Assert.IsType<string>(Assert.Single(Assert.IsType<List<string>>(info.GetValue("Names", typeof(List<string>)))));
        Assert.Matches(expectedPattern, nameResult);
        Assert.Null(info.GetValue("Assemblies", typeof(AssemblyName[])));
        Assert.Null(info.GetValue("Resources", typeof(Hashtable)));
        Assert.Empty(Assert.IsType<List<string>>(info.GetValue("Shim", typeof(List<string>))));

        // Serialize again.
        service.SerializeMember(store, mockComponent.Object, member1);
        mockNestedSite.Verify(s => s.Name, Times.Exactly(expectedNameCallCount));
        mockNestedSite.Verify(s => s.FullName, Times.Exactly(expectedFullNameCallCount));

        info = new SerializationInfo(store.GetType(), new FormatterConverter());
        serializable.GetObjectData(info, default);
        nameResult = Assert.IsType<string>(Assert.Single(Assert.IsType<List<string>>(info.GetValue("Names", typeof(List<string>)))));
        Assert.Matches(expectedPattern, nameResult);
        Assert.Null(info.GetValue("Assemblies", typeof(AssemblyName[])));
        Assert.Null(info.GetValue("Resources", typeof(Hashtable)));
        Assert.Empty(Assert.IsType<List<string>>(info.GetValue("Shim", typeof(List<string>))));

        // Serialize another.
        service.SerializeMember(store, mockComponent.Object, member2);
        mockNestedSite.Verify(s => s.Name, Times.Exactly(expectedNameCallCount));
        mockNestedSite.Verify(s => s.FullName, Times.Exactly(expectedFullNameCallCount));

        info = new SerializationInfo(store.GetType(), new FormatterConverter());
        serializable.GetObjectData(info, default);
        nameResult = Assert.IsType<string>(Assert.Single(Assert.IsType<List<string>>(info.GetValue("Names", typeof(List<string>)))));
        Assert.Matches(expectedPattern, nameResult);
        Assert.Null(info.GetValue("Assemblies", typeof(AssemblyName[])));
        Assert.Null(info.GetValue("Resources", typeof(Hashtable)));
        Assert.Empty(Assert.IsType<List<string>>(info.GetValue("Shim", typeof(List<string>))));
    }

    [Fact]
    public void SerializeMember_NullStore_ThrowsArgumentNullException()
    {
        CodeDomComponentSerializationService service = new();
        MemberDescriptor member = TypeDescriptor.GetProperties(typeof(DataClass))[nameof(DataClass.IntValue)];
        Assert.Throws<ArgumentNullException>("store", () => service.SerializeMember(null, new DataClass(), member));
    }

    [Fact]
    public void SerializeMember_NullOwningObject_ThrowsArgumentNullException()
    {
        CodeDomComponentSerializationService service = new();
        SerializationStore store = service.CreateStore();
        MemberDescriptor member = TypeDescriptor.GetProperties(typeof(DataClass))[nameof(DataClass.IntValue)];
        Assert.Throws<ArgumentNullException>("owningObject", () => service.SerializeMember(store, null, member));
    }

    [Fact]
    public void SerializeMember_NullMember_ThrowsArgumentNullException()
    {
        CodeDomComponentSerializationService service = new();
        SerializationStore store = service.CreateStore();
        Assert.Throws<ArgumentNullException>("member", () => service.SerializeMember(store, new DataClass(), null));
    }

    [Fact]
    public void SerializeMember_InvalidStore_ThrowsInvalidOperationException()
    {
        CodeDomComponentSerializationService service = new();
        Mock<SerializationStore> mockStore = new(MockBehavior.Strict);
        MemberDescriptor member = TypeDescriptor.GetProperties(typeof(DataClass))[nameof(DataClass.IntValue)];
        Assert.Throws<InvalidOperationException>(() => service.SerializeMember(mockStore.Object, new DataClass(), member));
    }

    [Fact]
    public void SerializeMember_ClosedStore_ThrowsInvalidOperationException()
    {
        CodeDomComponentSerializationService service = new();
        SerializationStore store = service.CreateStore();
        store.Close();
        MemberDescriptor member = TypeDescriptor.GetProperties(typeof(DataClass))[nameof(DataClass.IntValue)];
        Assert.Throws<InvalidOperationException>(() => service.SerializeMember(store, new DataClass(), member));
    }

    [Fact]
    public void SerializeMemberAbsolute_InvokeObject_Success()
    {
        CodeDomComponentSerializationService service = new();
        SerializationStore store = service.CreateStore();
        MemberDescriptor member1 = TypeDescriptor.GetProperties(typeof(DataClass))[nameof(DataClass.IntValue)];
        MemberDescriptor member2 = TypeDescriptor.GetProperties(typeof(DataClass))[nameof(DataClass.StringValue)];
        ISerializable serializable = Assert.IsAssignableFrom<ISerializable>(store);

        object value = new();
        service.SerializeMemberAbsolute(store, value, member1);
        SerializationInfo info = new(store.GetType(), new FormatterConverter());
        serializable.GetObjectData(info, default);
        AssertNullState(info);
        string nameResult = Assert.IsType<string>(Assert.Single(Assert.IsType<List<string>>(info.GetValue("Names", typeof(List<string>)))));
        Assert.Matches("^object_........_...._...._...._............$", nameResult);
        Assert.Null(info.GetValue("Assemblies", typeof(AssemblyName[])));
        Assert.Null(info.GetValue("Resources", typeof(Hashtable)));
        Assert.Empty(Assert.IsType<List<string>>(info.GetValue("Shim", typeof(List<string>))));

        // Serialize again.
        service.SerializeMemberAbsolute(store, value, member1);
        info = new SerializationInfo(store.GetType(), new FormatterConverter());
        serializable.GetObjectData(info, default);
        AssertNullState(info);
        nameResult = Assert.IsType<string>(Assert.Single(Assert.IsType<List<string>>(info.GetValue("Names", typeof(List<string>)))));
        Assert.Matches("^object_........_...._...._...._............$", nameResult);
        Assert.Null(info.GetValue("Assemblies", typeof(AssemblyName[])));
        Assert.Null(info.GetValue("Resources", typeof(Hashtable)));
        Assert.Empty(Assert.IsType<List<string>>(info.GetValue("Shim", typeof(List<string>))));

        // Serialize another.
        service.SerializeMemberAbsolute(store, value, member2);
        info = new SerializationInfo(store.GetType(), new FormatterConverter());
        serializable.GetObjectData(info, default);
        AssertNullState(info);
        nameResult = Assert.IsType<string>(Assert.Single(Assert.IsType<List<string>>(info.GetValue("Names", typeof(List<string>)))));
        Assert.Matches("^object_........_...._...._...._............$", nameResult);
        Assert.Null(info.GetValue("Assemblies", typeof(AssemblyName[])));
        Assert.Null(info.GetValue("Resources", typeof(Hashtable)));
        Assert.Empty(Assert.IsType<List<string>>(info.GetValue("Shim", typeof(List<string>))));
    }

    [Fact]
    public void SerializeMemberAbsolute_InvokeIComponentWithoutSite_Success()
    {
        CodeDomComponentSerializationService service = new();
        SerializationStore store = service.CreateStore();
        MemberDescriptor member1 = TypeDescriptor.GetProperties(typeof(DataClass))[nameof(DataClass.IntValue)];
        MemberDescriptor member2 = TypeDescriptor.GetProperties(typeof(DataClass))[nameof(DataClass.StringValue)];
        ISerializable serializable = Assert.IsAssignableFrom<ISerializable>(store);

        Mock<IComponent> mockComponent = new(MockBehavior.Strict);
        mockComponent
            .Setup(c => c.Site)
            .Returns<ISite>(null)
            .Verifiable();

        service.SerializeMemberAbsolute(store, mockComponent.Object, member1);
        mockComponent.Verify(c => c.Site, Times.Once());

        SerializationInfo info = new(store.GetType(), new FormatterConverter());
        serializable.GetObjectData(info, default);
        AssertNullState(info);
        string nameResult = Assert.IsType<string>(Assert.Single(Assert.IsType<List<string>>(info.GetValue("Names", typeof(List<string>)))));
        Assert.Matches("^object_........_...._...._...._............$", nameResult);
        Assert.Null(info.GetValue("Assemblies", typeof(AssemblyName[])));
        Assert.Null(info.GetValue("Resources", typeof(Hashtable)));
        Assert.Empty(Assert.IsType<List<string>>(info.GetValue("Shim", typeof(List<string>))));

        // Serialize again.
        service.SerializeMemberAbsolute(store, mockComponent.Object, member1);
        mockComponent.Verify(c => c.Site, Times.Once());

        info = new SerializationInfo(store.GetType(), new FormatterConverter());
        serializable.GetObjectData(info, default);
        AssertNullState(info);
        nameResult = Assert.IsType<string>(Assert.Single(Assert.IsType<List<string>>(info.GetValue("Names", typeof(List<string>)))));
        Assert.Matches("^object_........_...._...._...._............$", nameResult);
        Assert.Null(info.GetValue("Assemblies", typeof(AssemblyName[])));
        Assert.Null(info.GetValue("Resources", typeof(Hashtable)));
        Assert.Empty(Assert.IsType<List<string>>(info.GetValue("Shim", typeof(List<string>))));

        // Serialize another.
        service.SerializeMemberAbsolute(store, mockComponent.Object, member2);
        mockComponent.Verify(c => c.Site, Times.Once());

        info = new SerializationInfo(store.GetType(), new FormatterConverter());
        serializable.GetObjectData(info, default);
        AssertNullState(info);
        nameResult = Assert.IsType<string>(Assert.Single(Assert.IsType<List<string>>(info.GetValue("Names", typeof(List<string>)))));
        Assert.Matches("^object_........_...._...._...._............$", nameResult);
        Assert.Null(info.GetValue("Assemblies", typeof(AssemblyName[])));
        Assert.Null(info.GetValue("Resources", typeof(Hashtable)));
        Assert.Empty(Assert.IsType<List<string>>(info.GetValue("Shim", typeof(List<string>))));
    }

    [Theory]
    [InlineData(null, 1, "^object_........_...._...._...._............$")]
    [InlineData("", 1, "^object_........_...._...._...._............$")]
    [InlineData("name", 2, "^name$")]
    public void SerializeMemberAbsolute_InvokeIComponentWithISite_Success(string name, int expectedCallCount, string expectedPattern)
    {
        CodeDomComponentSerializationService service = new();
        SerializationStore store = service.CreateStore();
        MemberDescriptor member1 = TypeDescriptor.GetProperties(typeof(DataClass))[nameof(DataClass.IntValue)];
        MemberDescriptor member2 = TypeDescriptor.GetProperties(typeof(DataClass))[nameof(DataClass.StringValue)];
        ISerializable serializable = Assert.IsAssignableFrom<ISerializable>(store);

        Mock<ISite> mockSite = new(MockBehavior.Strict);
        mockSite
            .Setup(s => s.Name)
            .Returns(name)
            .Verifiable();
        Mock<IComponent> mockComponent = new(MockBehavior.Strict);
        mockComponent
            .Setup(c => c.Site)
            .Returns(mockSite.Object)
            .Verifiable();

        service.SerializeMemberAbsolute(store, mockComponent.Object, member1);
        mockComponent.Verify(c => c.Site, Times.Once());
        mockSite.Verify(s => s.Name, Times.Exactly(expectedCallCount));

        SerializationInfo info = new(store.GetType(), new FormatterConverter());
        serializable.GetObjectData(info, default);
        AssertNullState(info);
        string nameResult = Assert.IsType<string>(Assert.Single(Assert.IsType<List<string>>(info.GetValue("Names", typeof(List<string>)))));
        Assert.Matches(expectedPattern, nameResult);
        Assert.Null(info.GetValue("Assemblies", typeof(AssemblyName[])));
        Assert.Null(info.GetValue("Resources", typeof(Hashtable)));
        Assert.Empty(Assert.IsType<List<string>>(info.GetValue("Shim", typeof(List<string>))));

        // Serialize again.
        service.SerializeMemberAbsolute(store, mockComponent.Object, member1);
        mockComponent.Verify(c => c.Site, Times.Once());
        mockSite.Verify(s => s.Name, Times.Exactly(expectedCallCount));

        info = new SerializationInfo(store.GetType(), new FormatterConverter());
        serializable.GetObjectData(info, default);
        AssertNullState(info);
        nameResult = Assert.IsType<string>(Assert.Single(Assert.IsType<List<string>>(info.GetValue("Names", typeof(List<string>)))));
        Assert.Matches(expectedPattern, nameResult);
        Assert.Null(info.GetValue("Assemblies", typeof(AssemblyName[])));
        Assert.Null(info.GetValue("Resources", typeof(Hashtable)));
        Assert.Empty(Assert.IsType<List<string>>(info.GetValue("Shim", typeof(List<string>))));

        // Serialize another.
        service.SerializeMemberAbsolute(store, mockComponent.Object, member2);
        mockComponent.Verify(c => c.Site, Times.Once());
        mockSite.Verify(s => s.Name, Times.Exactly(expectedCallCount));

        info = new SerializationInfo(store.GetType(), new FormatterConverter());
        serializable.GetObjectData(info, default);
        AssertNullState(info);
        nameResult = Assert.IsType<string>(Assert.Single(Assert.IsType<List<string>>(info.GetValue("Names", typeof(List<string>)))));
        Assert.Matches(expectedPattern, nameResult);
        Assert.Null(info.GetValue("Assemblies", typeof(AssemblyName[])));
        Assert.Null(info.GetValue("Resources", typeof(Hashtable)));
        Assert.Empty(Assert.IsType<List<string>>(info.GetValue("Shim", typeof(List<string>))));
    }

    [Theory]
    [InlineData(null, 1, null, 1, "^object_........_...._...._...._............$")]
    [InlineData(null, 1, "", 1, "^object_........_...._...._...._............$")]
    [InlineData(null, 1, "name", 2, "^name$")]
    [InlineData("", 1, null, 1, "^object_........_...._...._...._............$")]
    [InlineData("", 1, "", 1, "^object_........_...._...._...._............$")]
    [InlineData("", 1, "name", 2, "^name$")]
    [InlineData("fullName", 2, null, 0, "^fullName$")]
    [InlineData("fullName", 2, "", 0, "^fullName$")]
    [InlineData("fullName", 2, "name", 0, "^fullName$")]
    public void SerializeMemberAbsolute_InvokeIComponentWithINestedSite_Success(string fullName, int expectedFullNameCallCount, string name, int expectedNameCallCount, string expectedPattern)
    {
        CodeDomComponentSerializationService service = new();
        SerializationStore store = service.CreateStore();
        MemberDescriptor member1 = TypeDescriptor.GetProperties(typeof(DataClass))[nameof(DataClass.IntValue)];
        MemberDescriptor member2 = TypeDescriptor.GetProperties(typeof(DataClass))[nameof(DataClass.StringValue)];
        ISerializable serializable = Assert.IsAssignableFrom<ISerializable>(store);

        Mock<INestedSite> mockNestedSite = new(MockBehavior.Strict);
        mockNestedSite
            .Setup(s => s.Name)
            .Returns(name)
            .Verifiable();
        mockNestedSite
            .Setup(s => s.FullName)
            .Returns(fullName)
            .Verifiable();
        Mock<IComponent> mockComponent = new(MockBehavior.Strict);
        mockComponent
            .Setup(c => c.Site)
            .Returns(mockNestedSite.Object)
            .Verifiable();

        service.SerializeMemberAbsolute(store, mockComponent.Object, member1);
        mockComponent.Verify(c => c.Site, Times.Once());
        mockNestedSite.Verify(s => s.Name, Times.Exactly(expectedNameCallCount));
        mockNestedSite.Verify(s => s.FullName, Times.Exactly(expectedFullNameCallCount));

        SerializationInfo info = new(store.GetType(), new FormatterConverter());
        serializable.GetObjectData(info, default);
        string nameResult = Assert.IsType<string>(Assert.Single(Assert.IsType<List<string>>(info.GetValue("Names", typeof(List<string>)))));
        Assert.Matches(expectedPattern, nameResult);
        Assert.Null(info.GetValue("Assemblies", typeof(AssemblyName[])));
        Assert.Null(info.GetValue("Resources", typeof(Hashtable)));
        Assert.Empty(Assert.IsType<List<string>>(info.GetValue("Shim", typeof(List<string>))));

        // Serialize again.
        service.SerializeMemberAbsolute(store, mockComponent.Object, member1);
        mockNestedSite.Verify(s => s.Name, Times.Exactly(expectedNameCallCount));
        mockNestedSite.Verify(s => s.FullName, Times.Exactly(expectedFullNameCallCount));

        info = new SerializationInfo(store.GetType(), new FormatterConverter());
        serializable.GetObjectData(info, default);
        nameResult = Assert.IsType<string>(Assert.Single(Assert.IsType<List<string>>(info.GetValue("Names", typeof(List<string>)))));
        Assert.Matches(expectedPattern, nameResult);
        Assert.Null(info.GetValue("Assemblies", typeof(AssemblyName[])));
        Assert.Null(info.GetValue("Resources", typeof(Hashtable)));
        Assert.Empty(Assert.IsType<List<string>>(info.GetValue("Shim", typeof(List<string>))));

        // Serialize another.
        service.SerializeMemberAbsolute(store, mockComponent.Object, member2);
        mockNestedSite.Verify(s => s.Name, Times.Exactly(expectedNameCallCount));
        mockNestedSite.Verify(s => s.FullName, Times.Exactly(expectedFullNameCallCount));

        info = new SerializationInfo(store.GetType(), new FormatterConverter());
        serializable.GetObjectData(info, default);
        nameResult = Assert.IsType<string>(Assert.Single(Assert.IsType<List<string>>(info.GetValue("Names", typeof(List<string>)))));
        Assert.Matches(expectedPattern, nameResult);
        Assert.Null(info.GetValue("Assemblies", typeof(AssemblyName[])));
        Assert.Null(info.GetValue("Resources", typeof(Hashtable)));
        Assert.Empty(Assert.IsType<List<string>>(info.GetValue("Shim", typeof(List<string>))));
    }

    [Fact]
    public void SerializeMemberAbsolute_NullStore_ThrowsArgumentNullException()
    {
        CodeDomComponentSerializationService service = new();
        MemberDescriptor member = TypeDescriptor.GetProperties(typeof(DataClass))[nameof(DataClass.IntValue)];
        Assert.Throws<ArgumentNullException>("store", () => service.SerializeMemberAbsolute(null, new DataClass(), member));
    }

    [Fact]
    public void SerializeMemberAbsolute_NullOwningObject_ThrowsArgumentNullException()
    {
        CodeDomComponentSerializationService service = new();
        SerializationStore store = service.CreateStore();
        MemberDescriptor member = TypeDescriptor.GetProperties(typeof(DataClass))[nameof(DataClass.IntValue)];
        Assert.Throws<ArgumentNullException>("owningObject", () => service.SerializeMemberAbsolute(store, null, member));
    }

    [Fact]
    public void SerializeMemberAbsolute_NullMember_ThrowsArgumentNullException()
    {
        CodeDomComponentSerializationService service = new();
        SerializationStore store = service.CreateStore();
        Assert.Throws<ArgumentNullException>("member", () => service.SerializeMemberAbsolute(store, new DataClass(), null));
    }

    [Fact]
    public void SerializeMemberAbsolute_InvalidStore_ThrowsInvalidOperationException()
    {
        CodeDomComponentSerializationService service = new();
        Mock<SerializationStore> mockStore = new(MockBehavior.Strict);
        MemberDescriptor member = TypeDescriptor.GetProperties(typeof(DataClass))[nameof(DataClass.IntValue)];
        Assert.Throws<InvalidOperationException>(() => service.SerializeMemberAbsolute(mockStore.Object, new DataClass(), member));
    }

    [Fact]
    public void SerializeMemberAbsolute_ClosedStore_ThrowsInvalidOperationException()
    {
        CodeDomComponentSerializationService service = new();
        SerializationStore store = service.CreateStore();
        store.Close();
        MemberDescriptor member = TypeDescriptor.GetProperties(typeof(DataClass))[nameof(DataClass.IntValue)];
        Assert.Throws<InvalidOperationException>(() => service.SerializeMemberAbsolute(store, new DataClass(), member));
    }

    private class DataClass : Component
    {
        public int IntValue { get; set; }

        public string StringValue { get; set; }

        public string DefaultStringValue { get; set; }

        public event EventHandler Event
        {
            add { }
            remove { }
        }
    }

#if DUMP_STATE
    private static void DumpState(Hashtable state)
    {
        Console.WriteLine("---- DUMPING ----");
        foreach (string key in state.Keys)
        {
            Console.WriteLine(key);
            object[] valueState = (object[])state[key];
            CodeStatementCollection state0 = (CodeStatementCollection)valueState[0];
            if (state0 is null)
            {
                Console.WriteLine("- [0]: null");
            }
            else
            {
                Console.WriteLine($"- [0]: {state0}, {state0.Count} elements");
                foreach (CodeStatement e in state0)
                {
                    Console.WriteLine($"  - {CodeDomHelpers.GetConstructionString(e)}");
                }
            }

            object state1 = valueState[1];
            if (state1 is null)
            {
                Console.WriteLine("- [1]: null");
            }
            else
            {
                Console.WriteLine($"- [1]: {state1}");
            }

            object state2 = valueState[2];
            if (state2 is null)
            {
                Console.WriteLine("- [2]: null");
            }
            else
            {
                Console.WriteLine($"- [2]: {state2}");
            }

            object state3 = valueState[3];
            if (state3 is null)
            {
                Console.WriteLine("- [3]: null");
            }
            else
            {
                Console.WriteLine($"- [3]: {state3}");
            }

            object state4 = valueState[4];
            if (state4 is null)
            {
                Console.WriteLine("- [4]: null");
            }
            else
            {
                Console.WriteLine($"- [4]: {state4}");
            }

            object state5 = valueState[5];
            if (state5 is null)
            {
                Console.WriteLine("- [5]: null");
            }
            else
            {
                Console.WriteLine($"- [5]: {state5}");
            }
        }
#endif
}
