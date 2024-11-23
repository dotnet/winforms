// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Collections;
using Moq;
using Moq.Protected;
using System.Windows.Forms.TestUtilities;

namespace System.ComponentModel.Design.Serialization.Tests;

public class DesignerSerializationManagerTests
{
    [Fact]
    public void DesignerSerializationManager_Ctor_Default()
    {
        DesignerSerializationManager manager = new();
        IDesignerSerializationManager iManager = manager;
        Assert.Null(manager.Container);
        Assert.True(manager.PreserveNames);
        Assert.Empty(iManager.Properties);
        Assert.Same(iManager.Properties, iManager.Properties);
        Assert.Null(manager.PropertyProvider);
        Assert.False(manager.RecycleInstances);
        Assert.True(manager.ValidateRecycledTypes);
    }

    public static IEnumerable<object[]> Ctor_IServiceProvider_TestData()
    {
        Mock<IServiceProvider> nullMockServiceProvider = new(MockBehavior.Strict);
        nullMockServiceProvider
            .Setup(p => p.GetService(typeof(IDesignerHost)))
            .Returns((IDesignerHost)null);
        yield return new object[] { nullMockServiceProvider.Object, null };

        Mock<IServiceProvider> invalidMockServiceProvider = new(MockBehavior.Strict);
        invalidMockServiceProvider
            .Setup(p => p.GetService(typeof(IDesignerHost)))
            .Returns(new object());
        yield return new object[] { invalidMockServiceProvider.Object, null };

        Container container = new();
        Mock<IDesignerHost> mockDesignerHost = new(MockBehavior.Strict);
        mockDesignerHost
            .Setup(h => h.Container)
            .Returns(container);
        Mock<IServiceProvider> mockServiceProvider = new(MockBehavior.Strict);
        mockServiceProvider
            .Setup(p => p.GetService(typeof(IDesignerHost)))
            .Returns(mockDesignerHost.Object);
        yield return new object[] { mockServiceProvider.Object, container };
    }

    [Theory]
    [MemberData(nameof(Ctor_IServiceProvider_TestData))]
    public void DesignerSerializationManager_Ctor_IServiceProvider(IServiceProvider provider, IContainer expectedContainer)
    {
        DesignerSerializationManager manager = new(provider);
        IDesignerSerializationManager iManager = manager;
        Assert.Same(expectedContainer, manager.Container);
        Assert.Same(manager.Container, manager.Container);
        Assert.True(manager.PreserveNames);
        Assert.Empty(iManager.Properties);
        Assert.Same(iManager.Properties, iManager.Properties);
        Assert.Null(manager.PropertyProvider);
        Assert.False(manager.RecycleInstances);
        Assert.True(manager.ValidateRecycledTypes);
        ;
    }

    [Fact]
    public void DesignerSerializationManager_Ctor_NullProvider_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>("provider", () => new DesignerSerializationManager(null));
    }

    public static IEnumerable<object[]> Container_Set_TestData()
    {
        yield return new object[] { null };
        yield return new object[] { new Container() };
    }

    [Theory]
    [MemberData(nameof(Container_Set_TestData))]
    public void DesignerSerializationManager_Container_Set_GetReturnsExpected(IContainer value)
    {
        SubDesignerSerializationManager manager = new()
        {
            Container = value
        };
        Assert.Same(value, manager.Container);
        Assert.Same(value, manager.GetService(typeof(IContainer)));

        // Set same.
        manager.Container = value;
        Assert.Same(value, manager.Container);
        Assert.Same(value, manager.GetService(typeof(IContainer)));
    }

    [Fact]
    public void DesignerSerializationManager_Container_SetWithSession_ThrowsInvalidOperationException()
    {
        DesignerSerializationManager manager = new();
        manager.CreateSession();
        Assert.Throws<InvalidOperationException>(() => manager.Container = null);
    }

    [Fact]
    public void DesignerSerializationManager_Context_GetWithSession_ReturnsExpected()
    {
        DesignerSerializationManager manager = new();
        IDesignerSerializationManager iManager = manager;
        _ = manager.CreateSession();
        ContextStack context = iManager.Context;
        Assert.Null(context.Current);
        Assert.Same(context, iManager.Context);
    }

    [Fact]
    public void DesignerSerializationManager_Context_GetNoSessionAfterGetting_ThrowsInvalidOperationException()
    {
        DesignerSerializationManager manager = new();
        IDesignerSerializationManager iManager = manager;
        IDisposable session = manager.CreateSession();
        ContextStack context = iManager.Context;
        Assert.NotNull(context);
        Assert.Same(context, iManager.Context);
        session.Dispose();
        Assert.Throws<InvalidOperationException>(() => iManager.Context);
    }

    [Fact]
    public void DesignerSerializationManager_Context_GetNoSession_ThrowsInvalidOperationException()
    {
        DesignerSerializationManager manager = new();
        IDesignerSerializationManager iManager = manager;
        Assert.Throws<InvalidOperationException>(() => iManager.Context);
    }

    [Fact]
    public void DesignerSerializationManager_Errors_GetWithSession_ReturnsExpected()
    {
        DesignerSerializationManager manager = new();
        _ = manager.CreateSession();
        IList errors = manager.Errors;
        Assert.Empty(errors);
        Assert.Same(errors, manager.Errors);
        Assert.IsAssignableFrom<IList>(errors);
    }

    [Fact]
    public void DesignerSerializationManager_Errors_NoSessionWithPreviousSession_ThrowsInvalidOperationException()
    {
        DesignerSerializationManager manager = new();
        IDisposable session = manager.CreateSession();
        Assert.Empty(manager.Errors);
        session.Dispose();
        Assert.Throws<InvalidOperationException>(() => manager.Errors);
    }

    [Fact]
    public void DesignerSerializationManager_Errors_NoSession_ThrowsInvalidOperationException()
    {
        DesignerSerializationManager manager = new();
        Assert.Throws<InvalidOperationException>(() => manager.Errors);
    }

    [Theory]
    [BoolData]
    public void DesignerSerializationManager_PreserveNames_Set_GetReturnsExpected(bool value)
    {
        DesignerSerializationManager manager = new()
        {
            PreserveNames = value
        };
        Assert.Equal(value, manager.PreserveNames);

        // Set same
        manager.PreserveNames = value;
        Assert.Equal(value, manager.PreserveNames);

        // Set different
        manager.PreserveNames = !value;
        Assert.Equal(!value, manager.PreserveNames);
    }

    [Theory]
    [BoolData]
    public void DesignerSerializationManager_PreserveNames_SetWithSession_ThrowsInvalidOperationException(bool value)
    {
        DesignerSerializationManager manager = new();
        manager.CreateSession();
        Assert.Throws<InvalidOperationException>(() => manager.PreserveNames = value);
        Assert.True(manager.PreserveNames);
    }

    [Fact]
    public void DesignerSerializationManager_Properties_GetWithPropertyProvider_ReturnExpected()
    {
        PropertyProvider provider = new();
        DesignerSerializationManager manager = new()
        {
            PropertyProvider = provider
        };
        IDesignerSerializationManager iManager = manager;
        PropertyDescriptorCollection properties = iManager.Properties;
        Assert.Same(properties, iManager.Properties);
        PropertyDescriptor property = Assert.IsAssignableFrom<PropertyDescriptor>(Assert.Single(properties));
        Assert.NotEmpty(property.Attributes);
        Assert.Equal("Category", property.Category);
        Assert.IsType<Int64Converter>(property.Converter);
        Assert.Equal(typeof(PropertyProvider), property.ComponentType);
        Assert.Equal("Description", property.Description);
        Assert.True(property.DesignTimeOnly);
        Assert.Equal("DisplayName", property.DisplayName);
        Assert.True(property.IsBrowsable);
        Assert.True(property.IsLocalizable);
        Assert.False(property.IsReadOnly);
        Assert.Equal("Value", property.Name);
        Assert.Equal(typeof(int), property.PropertyType);
        Assert.Equal(DesignerSerializationVisibility.Content, property.SerializationVisibility);
        Assert.False(property.SupportsChangeEvents);

        // Should be wrapped.
        Assert.False(property.CanResetValue(new Component()));
        Assert.Equal(0, property.GetValue(new Component()));
        property.SetValue(new Component(), 1);
        Assert.Equal(1, property.GetValue(new Component()));
        property.ResetValue(new Component());
        Assert.Equal(1, property.GetValue(new Component()));
        Assert.True(property.ShouldSerializeValue(new Component()));
    }

    [Fact]
    public void DesignerSerializationManager_Properties_GetWithNullPropertyInPropertyProvider_ThrowsArgumentNullException()
    {
        object provider = new();
        DesignerSerializationManager manager = new()
        {
            PropertyProvider = provider
        };
        IDesignerSerializationManager iManager = manager;
        Mock<CustomTypeDescriptor> mockCustomTypeDescriptor = new(MockBehavior.Strict);
        mockCustomTypeDescriptor
            .Setup(d => d.GetProperties())
            .Returns(new PropertyDescriptorCollection([null]));
        Mock<TypeDescriptionProvider> mockProvider = new(MockBehavior.Strict);
        mockProvider
            .Setup(p => p.GetCache(provider))
            .CallBase();
        mockProvider
            .Setup(p => p.GetExtendedTypeDescriptor(provider))
            .CallBase();
        mockProvider
            .Setup(p => p.GetTypeDescriptor(typeof(object), provider))
            .Returns(mockCustomTypeDescriptor.Object);
        TypeDescriptor.AddProvider(mockProvider.Object, provider);

        Assert.Same(provider, manager.PropertyProvider);
        Assert.Throws<ArgumentNullException>("property", () => iManager.Properties);

        // Call again.
        Assert.Throws<ArgumentNullException>("property", () => iManager.Properties);
    }

    [Theory]
    [StringWithNullData]
    public void DesignerSerializationManager_PropertyProvider_Set_GetReturnsExpected(string value)
    {
        DesignerSerializationManager manager = new()
        {
            PropertyProvider = value
        };
        Assert.Same(value, manager.PropertyProvider);

        // Set same
        manager.PropertyProvider = value;
        Assert.Same(value, manager.PropertyProvider);
    }

    [Theory]
    [StringWithNullData]
    public void DesignerSerializationManager_PropertyProvider_SetWithSession_GetReturnsExpected(string value)
    {
        DesignerSerializationManager manager = new();
        manager.CreateSession();

        manager.PropertyProvider = value;
        Assert.Same(value, manager.PropertyProvider);

        // Set same
        manager.PropertyProvider = value;
        Assert.Same(value, manager.PropertyProvider);
    }

    [Fact]
    public void DesignerSerializationManager_Properties_SetWithExistingProperties_Resets()
    {
        DesignerSerializationManager manager = new()
        {
            PropertyProvider = new PropertyProvider()
        };
        IDesignerSerializationManager iManager = manager;
        PropertyDescriptorCollection properties = iManager.Properties;
        PropertyDescriptor property = Assert.IsAssignableFrom<PropertyDescriptor>(Assert.Single(properties));
        Assert.Equal(nameof(PropertyProvider.Value), property.Name);
        Assert.Same(properties, iManager.Properties);

        OtherPropertyProvider provider = new();
        manager.PropertyProvider = provider;
        Assert.Same(provider, manager.PropertyProvider);
        PropertyDescriptorCollection otherProperties = iManager.Properties;
        PropertyDescriptor otherProperty = Assert.IsAssignableFrom<PropertyDescriptor>(Assert.Single(otherProperties));
        Assert.Equal(nameof(OtherPropertyProvider.OtherValue), otherProperty.Name);
        Assert.Same(otherProperties, iManager.Properties);

        // Set same.
        manager.PropertyProvider = provider;
        Assert.Same(provider, manager.PropertyProvider);
        Assert.Same(otherProperties, iManager.Properties);
    }

    [Theory]
    [BoolData]
    public void DesignerSerializationManager_RecycleInstances_Set_GetReturnsExpected(bool value)
    {
        DesignerSerializationManager manager = new()
        {
            RecycleInstances = value
        };
        Assert.Equal(value, manager.RecycleInstances);

        // Set same
        manager.RecycleInstances = value;
        Assert.Equal(value, manager.RecycleInstances);

        // Set different
        manager.RecycleInstances = !value;
        Assert.Equal(!value, manager.RecycleInstances);
    }

    [Theory]
    [BoolData]
    public void DesignerSerializationManager_RecycleInstances_SetWithSession_ThrowsInvalidOperationException(bool value)
    {
        DesignerSerializationManager manager = new();
        manager.CreateSession();
        Assert.Throws<InvalidOperationException>(() => manager.RecycleInstances = value);
        Assert.False(manager.RecycleInstances);
    }

    [Theory]
    [BoolData]
    public void DesignerSerializationManager_ValidateRecycledTypes_Set_GetReturnsExpected(bool value)
    {
        DesignerSerializationManager manager = new()
        {
            ValidateRecycledTypes = value
        };
        Assert.Equal(value, manager.ValidateRecycledTypes);

        // Set same
        manager.ValidateRecycledTypes = value;
        Assert.Equal(value, manager.ValidateRecycledTypes);

        // Set different
        manager.ValidateRecycledTypes = !value;
        Assert.Equal(!value, manager.ValidateRecycledTypes);
    }

    [Theory]
    [BoolData]
    public void DesignerSerializationManager_ValidateRecycledTypes_SetWithSession_ThrowsInvalidOperationException(bool value)
    {
        DesignerSerializationManager manager = new();
        manager.CreateSession();
        Assert.Throws<InvalidOperationException>(() => manager.ValidateRecycledTypes = value);
        Assert.True(manager.ValidateRecycledTypes);
    }

    [Fact]
    public void DesignerSerializationManager_ResolveName_AddNoSession_ThrowsInvalidOperationException()
    {
        DesignerSerializationManager manager = new();
        IDesignerSerializationManager iManager = manager;
        int callCount = 0;
        ResolveNameEventHandler handler = (sender, e) => callCount++;
        Assert.Throws<InvalidOperationException>(() => iManager.ResolveName += handler);
        iManager.ResolveName -= handler;
        Assert.Equal(0, callCount);
    }

    [Fact]
    public void DesignerSerializationManager_SerializationComplete_AddNoSession_ThrowsInvalidOperationException()
    {
        DesignerSerializationManager manager = new();
        IDesignerSerializationManager iManager = manager;
        int callCount = 0;
        EventHandler handler = (sender, e) => callCount++;
        Assert.Throws<InvalidOperationException>(() => iManager.SerializationComplete += handler);
        iManager.SerializationComplete -= handler;
        Assert.Equal(0, callCount);
    }

    public static IEnumerable<object[]> AddSerializationProvider_TestData()
    {
        yield return new object[] { null, null };
        yield return new object[] { null, new() };
        yield return new object[] { typeof(int), null };
        yield return new object[] { typeof(int), new() };
    }

    [Theory]
    [MemberData(nameof(AddSerializationProvider_TestData))]
    public void DesignerSerializationManager_AddSerializationProvider_NonNullProvider_GetSerializerReturnsExpected(Type objectType, object expected)
    {
        DesignerSerializationManager manager = new();
        IDesignerSerializationManager iManager = manager;
        Mock<IDesignerSerializationProvider> mockDesignerSerializationProvider = new(MockBehavior.Strict);
        mockDesignerSerializationProvider
            .Setup(p => p.GetSerializer(manager, null, objectType, mockDesignerSerializationProvider.Object.GetType()))
            .Returns(expected)
            .Verifiable();
        iManager.AddSerializationProvider(mockDesignerSerializationProvider.Object);
        Assert.Same(expected, iManager.GetSerializer(objectType, mockDesignerSerializationProvider.Object.GetType()));
        mockDesignerSerializationProvider.Verify(p => p.GetSerializer(manager, null, objectType, mockDesignerSerializationProvider.Object.GetType()), Times.Once());

        // Call again.
        Assert.Same(expected, iManager.GetSerializer(objectType, mockDesignerSerializationProvider.Object.GetType()));
        mockDesignerSerializationProvider.Verify(p => p.GetSerializer(manager, null, objectType, mockDesignerSerializationProvider.Object.GetType()), Times.Exactly(2));
    }

    [Fact]
    public void DesignerSerializationManager_AddSerializationProvider_NullProvider_Nop()
    {
        DesignerSerializationManager manager = new();
        IDesignerSerializationManager iManager = manager;
        iManager.AddSerializationProvider(null);
    }

    [Fact]
    public void DesignerSerializationManager_CreateSession_Invoke_Success()
    {
        DesignerSerializationManager manager = new();
        IDisposable session = manager.CreateSession();
        Assert.NotNull(session);
        session.Dispose();

        // Get another.
        IDisposable session2 = manager.CreateSession();
        Assert.NotNull(session2);
        Assert.NotSame(session, session2);
        session2.Dispose();
    }

    [Fact]
    public void DesignerSerializationManager_CreateSession_DisposeMultipleTimes_Success()
    {
        DesignerSerializationManager manager = new();
        IDesignerSerializationManager iManager = manager;
        IDisposable session1 = manager.CreateSession();
        session1.Dispose();
        Assert.Throws<InvalidOperationException>(() => iManager.Context);

        // Dispose again without session.
        session1.Dispose();
        Assert.Throws<InvalidOperationException>(() => iManager.Context);

        // Dispose with session.
        IDisposable session2 = manager.CreateSession();
        session1.Dispose();
        Assert.Throws<InvalidOperationException>(() => iManager.Context);
    }

    [Fact]
    public void DesignerSerializationManager_CreateSession_Invoke_CallsOnSessionCreated()
    {
        Mock<DesignerSerializationManager> mockManager = new(MockBehavior.Strict);
        mockManager
            .Protected()
            .Setup("OnSessionCreated", EventArgs.Empty)
            .Verifiable();
        _ = mockManager.Object.CreateSession();
        mockManager.Protected().Verify("OnSessionCreated", Times.Once(), EventArgs.Empty);
    }

    [Fact]
    public void DesignerSerializationManager_CreateSession_InvokeWithSessionCreated_CallsHandler()
    {
        DesignerSerializationManager manager = new();
        int callCount = 0;
        EventHandler handler = (sender, e) =>
        {
            Assert.Same(manager, sender);
            Assert.Equal(EventArgs.Empty, e);
            callCount++;
        };
        manager.SessionCreated += handler;

        IDisposable session = manager.CreateSession();
        Assert.NotNull(session);
        Assert.Equal(1, callCount);
        session.Dispose();

        // Remove handler.
        manager.SessionCreated -= handler;
        session = manager.CreateSession();
        Assert.NotNull(session);
        Assert.Equal(1, callCount);
    }

    [Fact]
    public void DesignerSerializationManager_CreateSession_Dispose_CallsOnSessionDisposed()
    {
        Mock<DesignerSerializationManager> mockManager = new(MockBehavior.Strict);
        mockManager
            .Protected()
            .Setup("OnSessionCreated", EventArgs.Empty);
        mockManager
            .Protected()
            .Setup("OnSessionDisposed", EventArgs.Empty)
            .Verifiable();
        IDisposable session = mockManager.Object.CreateSession();
        session.Dispose();
        mockManager.Protected().Verify("OnSessionDisposed", Times.Once(), EventArgs.Empty);

        // Dispose again.
        session.Dispose();
        mockManager.Protected().Verify("OnSessionDisposed", Times.Exactly(2), EventArgs.Empty);
    }

    [Fact]
    public void DesignerSerializationManager_CreateSession_Dispose_ClearsErrors()
    {
        DesignerSerializationManager manager = new();
        IDesignerSerializationManager iManager = manager;

        IDisposable session1 = manager.CreateSession();
        IList errors1 = manager.Errors;
        Assert.Empty(errors1);
        Assert.Same(errors1, manager.Errors);
        object errorInformation = new();
        iManager.ReportError(errorInformation);
        Assert.Same(errorInformation, Assert.Single(errors1));

        // Dispose, get another and ensure cleared.
        session1.Dispose();
        _ = manager.CreateSession();
        IList errors2 = manager.Errors;
        Assert.Empty(errors2);
        Assert.Same(errors2, manager.Errors);
        Assert.NotSame(errors1, errors2);
    }

    [Fact]
    public void DesignerSerializationManager_CreateSession_Dispose_ClearsContext()
    {
        DesignerSerializationManager manager = new();
        IDesignerSerializationManager iManager = manager;

        IDisposable session1 = manager.CreateSession();
        ContextStack stack1 = iManager.Context;
        Assert.NotNull(stack1);
        Assert.Same(stack1, iManager.Context);

        // Dispose, get another and ensure cleared.
        session1.Dispose();
        _ = manager.CreateSession();
        ContextStack stack2 = iManager.Context;
        Assert.NotNull(stack2);
        Assert.Same(stack2, iManager.Context);
        Assert.NotSame(stack1, stack2);
    }

    [Fact]
    public void DesignerSerializationManager_CreateSession_Dispose_ClearsSerializers()
    {
        DesignerSerializationManager manager = new();

        IDisposable session1 = manager.CreateSession();
        object serializer1 = manager.GetSerializer(typeof(ClassWithPublicDesignerSerializer), typeof(BaseClass));
        Assert.IsType<PublicDesignerSerializationProvider>(serializer1);
        Assert.Same(serializer1, manager.GetSerializer(typeof(ClassWithPublicDesignerSerializer), typeof(PublicDesignerSerializationProvider)));

        // Dispose and ensure cleared.
        session1.Dispose();
        object serializer2 = manager.GetSerializer(typeof(ClassWithPublicDesignerSerializer), typeof(BaseClass));
        Assert.IsType<PublicDesignerSerializationProvider>(serializer2);
        Assert.NotSame(serializer1, serializer2);
        Assert.NotSame(serializer2, manager.GetSerializer(typeof(ClassWithPublicDesignerSerializer), typeof(BaseClass)));
    }

    [Theory]
    [MemberData(nameof(ResolveNameEventArgs_TestData))]
    public void DesignerSerializationManager_CreateSession_Dispose_ClearsResolveNameEventHandler(ResolveNameEventArgs eventArgs)
    {
        SubDesignerSerializationManager manager = new();
        IDesignerSerializationManager iManager = manager;
        int callCount = 0;
        ResolveNameEventHandler handler = (sender, e) =>
        {
            Assert.Same(manager, sender);
            Assert.Same(eventArgs, e);
            callCount++;
        };
        IDisposable session = manager.CreateSession();
        iManager.ResolveName += handler;

        manager.OnResolveName(eventArgs);
        Assert.Equal(1, callCount);
        session.Dispose();

        // Call again.
        session = manager.CreateSession();
        manager.OnResolveName(eventArgs);
        Assert.Equal(1, callCount);
    }

    [Theory]
    [NewAndDefaultData<EventArgs>]
    public void DesignerSerializationManager_CreateSession_Dispose_ClearsSerializationCompleteEventHandler(EventArgs eventArgs)
    {
        SubDesignerSerializationManager manager = new();
        IDesignerSerializationManager iManager = manager;
        int callCount = 0;
        EventHandler handler = (sender, e) =>
        {
            Assert.Same(manager, sender);
            Assert.Same(eventArgs, e);
            callCount++;
        };
        IDisposable session = manager.CreateSession();
        iManager.SerializationComplete += handler;
        manager.OnSessionDisposed(eventArgs);
        Assert.Equal(1, callCount);
        session.Dispose();

        // Call again.
        session = manager.CreateSession();
        session.Dispose();
        Assert.Equal(1, callCount);
    }

    [Fact]
    public void DesignerSerializationManager_CreateSessionDispose_InvokeWithSessionDisposed_CallsHandler()
    {
        DesignerSerializationManager manager = new();
        int callCount = 0;
        EventHandler handler = (sender, e) =>
        {
            Assert.Same(manager, sender);
            Assert.Same(EventArgs.Empty, e);
            callCount++;
        };
        manager.SessionDisposed += handler;

        IDisposable session = manager.CreateSession();
        session.Dispose();
        Assert.Equal(1, callCount);

        // Call again.
        session = manager.CreateSession();
        session.Dispose();
        Assert.Equal(2, callCount);

        // Remove handler.
        manager.SessionDisposed -= handler;
        session = manager.CreateSession();
        session.Dispose();
        Assert.Equal(2, callCount);
    }

    [Fact]
    public void DesignerSerializationManager_CreateSessionDispose_InvokeWithSerializationComplete_CallsHandler()
    {
        DesignerSerializationManager manager = new();
        IDesignerSerializationManager iManager = manager;
        int callCount = 0;
        EventHandler handler = (sender, e) =>
        {
            Assert.Same(manager, sender);
            Assert.Same(EventArgs.Empty, e);
            callCount++;
        };
        IDisposable session = manager.CreateSession();
        iManager.SerializationComplete += handler;
        session.Dispose();
        Assert.Equal(1, callCount);

        // Call again.
        session = manager.CreateSession();
        session.Dispose();
        Assert.Equal(1, callCount);

        // Remove handler.
        session = manager.CreateSession();
        iManager.SerializationComplete += handler;
        iManager.SerializationComplete -= handler;
        session.Dispose();
        Assert.Equal(1, callCount);
    }

    [Fact]
    public void DesignerSerializationManager_CreateSession_InvokeWithSession_ThrowsInvalidOperationException()
    {
        DesignerSerializationManager manager = new();
        manager.CreateSession();
        Assert.Throws<InvalidOperationException>(manager.CreateSession);
    }

    public static IEnumerable<object[]> GetInstance_NoSuchInstance_TestData()
    {
        Mock<IServiceProvider> mockServiceProvider = new(MockBehavior.Strict);
        mockServiceProvider
            .Setup(p => p.GetService(typeof(IDesignerHost)))
            .Returns(null);

        Component component = new();
        Container container = new();
        container.Add(component, "name");
        Mock<IDesignerHost> mockDesignerHost = new(MockBehavior.Strict);
        mockDesignerHost
            .Setup(h => h.Container)
            .Returns(container);
        Mock<IServiceProvider> mockContainerServiceProvider = new(MockBehavior.Strict);
        mockContainerServiceProvider
            .Setup(p => p.GetService(typeof(IDesignerHost)))
            .Returns(mockDesignerHost.Object);

        yield return new object[] { mockServiceProvider.Object, true, string.Empty, null };
        yield return new object[] { mockServiceProvider.Object, false, string.Empty, null };
        yield return new object[] { mockServiceProvider.Object, true, "NoSuchName", null };
        yield return new object[] { mockServiceProvider.Object, false, "NoSuchName", null };

        yield return new object[] { mockContainerServiceProvider.Object, true, string.Empty, null };
        yield return new object[] { mockContainerServiceProvider.Object, false, string.Empty, null };
        yield return new object[] { mockContainerServiceProvider.Object, true, "NoSuchName", null };
        yield return new object[] { mockContainerServiceProvider.Object, false, "NoSuchName", null };
        yield return new object[] { mockContainerServiceProvider.Object, true, "name", component };
        yield return new object[] { mockContainerServiceProvider.Object, false, "name", null };
        yield return new object[] { mockContainerServiceProvider.Object, true, "Name", component };
        yield return new object[] { mockContainerServiceProvider.Object, false, "Name", null };
    }

    [Theory]
    [MemberData(nameof(GetInstance_NoSuchInstance_TestData))]
    public void DesignerSerializationManager_GetInstance_NoNamedInstances_ReturnsNull(IServiceProvider provider, bool preserveNames, string name, object expected)
    {
        DesignerSerializationManager manager = new(provider)
        {
            PreserveNames = preserveNames
        };
        IDesignerSerializationManager iManager = manager;
        manager.CreateSession();
        Assert.Same(expected, iManager.GetInstance(name));

        // Call again.
        Assert.Same(expected, iManager.GetInstance(name));
    }

    [Theory]
    [StringData]
    public void DesignerSerializationManager_GetInstance_HasNameInstancesNameExists_ReturnsExpected(string name)
    {
        DesignerSerializationManager manager = new();
        IDesignerSerializationManager iManager = manager;
        manager.CreateSession();

        object instance = new();
        iManager.SetName(instance, name);
        Assert.Same(instance, iManager.GetInstance(name));

        // Call again.
        Assert.Same(instance, iManager.GetInstance(name));
    }

    [Theory]
    [InlineData("")]
    [InlineData("Name")]
    [InlineData("NoSuchName")]
    public void DesignerSerializationManager_GetInstance_HasNameInstancesNoSuchName_ReturnsExpected(string name)
    {
        DesignerSerializationManager manager = new();
        IDesignerSerializationManager iManager = manager;
        manager.CreateSession();

        object instance = new();
        iManager.SetName(instance, "name");
        Assert.Null(iManager.GetInstance(name));

        // Call again.
        Assert.Null(iManager.GetInstance(name));
    }

    public static IEnumerable<object[]> GetInstance_InvokeWithResolveName_TestData()
    {
        yield return new object[] { string.Empty, null };
        yield return new object[] { string.Empty, new() };
        yield return new object[] { "NoSuchName", null };
        yield return new object[] { "NoSuchName", new() };
    }

    [Theory]
    [MemberData(nameof(GetInstance_InvokeWithResolveName_TestData))]
    public void DesignerSerializationManager_GetInstance_InvokeWithResolveName_CallsHandler(string name, object value)
    {
        Component component = new();
        Container container = new();
        Mock<IDesignerHost> mockDesignerHost = new(MockBehavior.Strict);
        mockDesignerHost
            .Setup(h => h.Container)
            .Returns(container);
        Mock<IServiceProvider> mockServiceProvider = new(MockBehavior.Strict);
        mockServiceProvider
            .Setup(p => p.GetService(typeof(IDesignerHost)))
            .Returns(mockDesignerHost.Object);
        DesignerSerializationManager manager = new(mockServiceProvider.Object);
        IDesignerSerializationManager iManager = manager;
        int callCount = 0;
        ResolveNameEventHandler handler = (sender, e) =>
        {
            Assert.Same(manager, sender);
            Assert.Same(name, e.Name);
            e.Value = value;
            callCount++;
        };
        manager.CreateSession();
        iManager.ResolveName += handler;

        // With handler.
        Assert.Same(value, iManager.GetInstance(name));
        Assert.Equal(1, callCount);

        // Call again.
        Assert.Same(value, iManager.GetInstance(name));
        Assert.Equal(2, callCount);

        // Does not call if there is a container instance.
        container.Add(component, "name");
        Assert.Same(component, iManager.GetInstance("name"));
        Assert.Equal(2, callCount);

        // Does not call if there is a named instance.
        container.Remove(component);
        iManager.SetName(component, "name");
        Assert.Same(component, iManager.GetInstance("name"));
        Assert.Equal(2, callCount);

        // Remove handler.
        iManager.ResolveName -= handler;
        Assert.Null(iManager.GetInstance(name));
        Assert.Equal(2, callCount);
    }

    [Fact]
    public void DesignerSerializationManager_GetInstance_NullName_ThrowsArgumentNullException()
    {
        DesignerSerializationManager manager = new();
        IDesignerSerializationManager iManager = manager;
        Assert.Throws<ArgumentNullException>("name", () => iManager.GetInstance(null));
    }

    [Theory]
    [StringData]
    public void DesignerSerializationManager_GetInstance_NoSession_ThrowsInvalidOperationException(string name)
    {
        DesignerSerializationManager manager = new();
        IDesignerSerializationManager iManager = manager;
        Assert.Throws<InvalidOperationException>(() => iManager.GetInstance(name));
    }

    public static IEnumerable<object[]> GetName_NoNamedInstance_TestData()
    {
        yield return new object[] { new(), null };
        yield return new object[] { new Component(), null };

        Mock<IComponent> mockNoSiteComponent = new(MockBehavior.Strict);
        mockNoSiteComponent
            .Setup(c => c.Site)
            .Returns((ISite)null);
        mockNoSiteComponent
            .Setup(c => c.Dispose());
        yield return new object[] { mockNoSiteComponent.Object, null };

        foreach (string name in new string[] { null, string.Empty, "name" })
        {
            Mock<ISite> mockSite = new(MockBehavior.Strict);
            mockSite
                .Setup(s => s.Name)
                .Returns(name);
            Mock<IComponent> mockSiteComponent = new(MockBehavior.Strict);
            mockSiteComponent
                .Setup(c => c.Site)
                .Returns(mockSite.Object);
            mockSiteComponent
                .Setup(c => c.Dispose());
            yield return new object[] { mockSiteComponent.Object, name };

            Mock<INestedSite> mockNestedSite = new(MockBehavior.Strict);
            mockNestedSite
                .Setup(s => s.FullName)
                .Returns(name);
            Mock<IComponent> mockNestedSiteComponent = new(MockBehavior.Strict);
            mockNestedSiteComponent
                .Setup(c => c.Site)
                .Returns(mockNestedSite.Object);
            mockNestedSiteComponent
                .Setup(c => c.Dispose());
            yield return new object[] { mockNestedSiteComponent.Object, name };
        }
    }

    [Theory]
    [MemberData(nameof(GetName_NoNamedInstance_TestData))]
    public void DesignerSerializationManager_GetName_NoNamedInstance_ReturnsExpected(object instance, string expected)
    {
        DesignerSerializationManager manager = new();
        IDesignerSerializationManager iManager = manager;
        manager.CreateSession();

        Assert.Same(expected, iManager.GetName(instance));
    }

    [Theory]
    [StringData]
    public void DesignerSerializationManager_GetName_HasNamedInstance_ReturnsExpected(string name)
    {
        DesignerSerializationManager manager = new();
        IDesignerSerializationManager iManager = manager;
        manager.CreateSession();

        object instance = new();
        iManager.SetName(instance, name);
        Assert.Same(name, iManager.GetName(instance));
    }

    [Theory]
    [StringData]
    public void DesignerSerializationManager_GetName_HasNamedComponent_ReturnsExpected(string name)
    {
        DesignerSerializationManager manager = new();
        IDesignerSerializationManager iManager = manager;
        manager.CreateSession();

        Mock<IComponent> mockInstance = new(MockBehavior.Strict);
        mockInstance
            .Setup(i => i.Site)
            .Verifiable();
        iManager.SetName(mockInstance.Object, name);
        Assert.Same(name, iManager.GetName(mockInstance.Object));
        mockInstance.Verify(i => i.Site, Times.Never());
    }

    [Fact]
    public void DesignerSerializationManager_GetName_NullValue_ThrowsArgumentNullException()
    {
        DesignerSerializationManager manager = new();
        IDesignerSerializationManager iManager = manager;
        Assert.Throws<ArgumentNullException>("value", () => iManager.GetName(null));
    }

    [Fact]
    public void DesignerSerializationManager_GetName_InvokeNoSession_ThrowsInvalidOperationException()
    {
        DesignerSerializationManager manager = new();
        IDesignerSerializationManager iManager = manager;
        Assert.Throws<InvalidOperationException>(() => iManager.GetName("value"));
    }

    [Theory]
    [CommonMemberData(typeof(CommonTestHelperEx), nameof(CommonTestHelperEx.GetTypeWithNullTheoryData))]
    public void DesignerSerializationManager_GetService_WithProvider_ReturnsExpected(Type serviceType)
    {
        object service = new();
        Mock<IServiceProvider> mockServiceProvider = new(MockBehavior.Strict);
        mockServiceProvider
            .Setup(p => p.GetService(serviceType))
            .Returns(service)
            .Verifiable();
        SubDesignerSerializationManager manager = new(mockServiceProvider.Object);
        Assert.Same(service, manager.GetService(serviceType));
        mockServiceProvider.Verify(p => p.GetService(serviceType), Times.Once());

        Assert.Same(service, ((IServiceProvider)manager).GetService(serviceType));
        mockServiceProvider.Verify(p => p.GetService(serviceType), Times.Exactly(2));
    }

    [Theory]
    [MemberData(nameof(Ctor_IServiceProvider_TestData))]
    public void DesignerSerializationManager_GetService_IContainer_ReturnsExpected(IServiceProvider provider, object expected)
    {
        SubDesignerSerializationManager manager = new(provider);
        Assert.Same(expected, manager.GetService(typeof(IContainer)));
        Assert.Same(expected, ((IServiceProvider)manager).GetService(typeof(IContainer)));
    }

    [Theory]
    [InlineData(null)]
    [InlineData(typeof(int))]
    [InlineData(typeof(IContainer))]
    public void DesignerSerializationManager_GetService_NoProvider_ReturnsNull(Type serviceType)
    {
        SubDesignerSerializationManager manager = new();
        Assert.Null(manager.GetService(serviceType));
        Assert.Null(((IServiceProvider)manager).GetService(serviceType));
    }

    [Fact]
    public void DesignerSerializationManager_IDesignerSerializationManagerGetService_Invoke_CallsProtectedGetService()
    {
        object service = new();
        Mock<DesignerSerializationManager> mockManager = new(MockBehavior.Strict);
        mockManager
            .Protected()
            .Setup<object>("GetService", typeof(int))
            .Returns(service)
            .Verifiable();
        IDesignerSerializationManager iManager = mockManager.Object;
        Assert.Same(service, iManager.GetService(typeof(int)));
        mockManager.Protected().Verify("GetService", Times.Once(), typeof(int));
    }

    [Theory]
    [CommonMemberData(typeof(CommonTestHelperEx), nameof(CommonTestHelperEx.GetTypeWithNullTheoryData))]
    public void DesignerSerializationManager_IDesignerSerializationManagerGetService_WithProvider_ReturnsExpected(Type serviceType)
    {
        object service = new();
        Mock<IServiceProvider> mockServiceProvider = new(MockBehavior.Strict);
        mockServiceProvider
            .Setup(p => p.GetService(serviceType))
            .Returns(service)
            .Verifiable();
        IDesignerSerializationManager iManager = new DesignerSerializationManager(mockServiceProvider.Object);
        Assert.Same(service, iManager.GetService(serviceType));
        mockServiceProvider.Verify(p => p.GetService(serviceType), Times.Once());

        Assert.Same(service, iManager.GetService(serviceType));
        mockServiceProvider.Verify(p => p.GetService(serviceType), Times.Exactly(2));
    }

    [Theory]
    [MemberData(nameof(Ctor_IServiceProvider_TestData))]
    public void DesignerSerializationManager_IDesignerSerializationManagerGetService_IContainer_ReturnsExpected(IServiceProvider provider, object expected)
    {
        IDesignerSerializationManager iManager = new DesignerSerializationManager(provider);
        Assert.Same(expected, iManager.GetService(typeof(IContainer)));
        Assert.Same(expected, iManager.GetService(typeof(IContainer)));
    }

    [Theory]
    [InlineData(null)]
    [InlineData(typeof(int))]
    [InlineData(typeof(IContainer))]
    public void DesignerSerializationManager_IDesignerSerializationManagerGetService_NoProvider_ReturnsNull(Type serviceType)
    {
        IDesignerSerializationManager iManager = new DesignerSerializationManager();
        Assert.Null(iManager.GetService(serviceType));
        Assert.Null(iManager.GetService(serviceType));
    }

    public static IEnumerable<object[]> GetRuntimeType_ValidProvider_TestData()
    {
        foreach (string typeName in new string[] { null, string.Empty, "typeName" })
        {
            yield return new object[] { typeName, null };
            yield return new object[] { typeName, typeof(string) };
        }
    }

    [Theory]
    [MemberData(nameof(GetRuntimeType_ValidProvider_TestData))]
    public void DesignerSerializationManager_GetRuntimeType_ValidProvider_ReturnsExpected(string typeName, Type resolvedType)
    {
        Mock<ITypeResolutionService> mockTypeResolutionService = new(MockBehavior.Strict);
        mockTypeResolutionService
            .Setup(s => s.GetType(typeName))
            .Returns(resolvedType)
            .Verifiable();
        Mock<IServiceProvider> mockServiceProvider = new(MockBehavior.Strict);
        mockServiceProvider
            .Setup(p => p.GetService(typeof(IDesignerHost)))
            .Returns((IDesignerHost)null);
        mockServiceProvider
            .Setup(p => p.GetService(typeof(ITypeResolutionService)))
            .Returns(mockTypeResolutionService.Object)
            .Verifiable();
        DesignerSerializationManager manager = new(mockServiceProvider.Object);
        Assert.Same(resolvedType, manager.GetRuntimeType(typeName));
        mockServiceProvider.Verify(p => p.GetService(typeof(ITypeResolutionService)), Times.Once());
        mockTypeResolutionService.Verify(s => s.GetType(typeName), Times.Once());

        // Call again.
        Assert.Same(resolvedType, manager.GetRuntimeType(typeName));
        mockServiceProvider.Verify(p => p.GetService(typeof(ITypeResolutionService)), Times.Once());
        mockTypeResolutionService.Verify(s => s.GetType(typeName), Times.Exactly(2));
    }

    public static IEnumerable<object[]> GetRuntimeType_InvalidProvider_TestData()
    {
        Mock<IServiceProvider> nullMockServiceProvider = new(MockBehavior.Strict);
        nullMockServiceProvider
            .Setup(p => p.GetService(typeof(IDesignerHost)))
            .Returns((IDesignerHost)null);
        nullMockServiceProvider
            .Setup(p => p.GetService(typeof(ITypeResolutionService)))
            .Returns((ITypeResolutionService)null);
        nullMockServiceProvider
            .Setup(p => p.GetService(typeof(TypeDescriptionProviderService)))
            .Returns((TypeDescriptionProviderService)null);
        yield return new object[] { nullMockServiceProvider.Object };

        Mock<IServiceProvider> invalidMockServiceProvider = new(MockBehavior.Strict);
        invalidMockServiceProvider
            .Setup(p => p.GetService(typeof(ITypeResolutionService)))
            .Returns(new object());
        invalidMockServiceProvider
            .Setup(p => p.GetService(typeof(ITypeResolutionService)))
            .Returns(new object());
        invalidMockServiceProvider
            .Setup(p => p.GetService(typeof(TypeDescriptionProviderService)))
            .Returns(new object());
        yield return new object[] { invalidMockServiceProvider.Object };
    }

    [Theory]
    [MemberData(nameof(GetRuntimeType_InvalidProvider_TestData))]
    public void DesignerSerializationManager_GetRuntimeType_InvalidProvider_ReturnsExpected(IServiceProvider provider)
    {
        DesignerSerializationManager manager = new(provider);
        Assert.Equal(typeof(int), manager.GetRuntimeType(typeof(int).FullName));
    }

    [Theory]
    [InlineData("System.Int32", typeof(int))]
    [InlineData("system.int32", null)]
    [InlineData("NoSuchType", null)]
    [InlineData("", null)]
    public void DesignerSerializationManager_GetRuntimeType_NoProvider_ReturnsExpected(string typeName, Type expected)
    {
        DesignerSerializationManager manager = new();
        Assert.Same(expected, manager.GetRuntimeType(typeName));
    }

    [Fact]
    public void DesignerSerializationManager_GetRuntimeType_NullTypeName_ThrowsArgumentNullException()
    {
        DesignerSerializationManager manager = new();
        Assert.Throws<ArgumentNullException>("typeName", () => manager.GetRuntimeType(null));
    }

    public static IEnumerable<object[]> GetSerializer_TestData()
    {
        foreach (Type objectType in new Type[] { null, typeof(int) })
        {
            yield return new object[] { objectType, typeof(int), null };
            yield return new object[] { objectType, typeof(IDesignerSerializationProvider), null };
            yield return new object[] { objectType, typeof(PublicDesignerSerializationProvider), null };
            yield return new object[] { objectType, typeof(PrivateDesignerSerializationProvider), null };
            yield return new object[] { objectType, typeof(ClassWithEmptyDefaultSerializationProvider), null };
            yield return new object[] { objectType, typeof(ClassWithNoSuchDefaultSerializationProvider), null };
            yield return new object[] { objectType, typeof(ClassWithInvalidDefaultSerializationProvider), null };
            yield return new object[] { objectType, typeof(ClassWithPublicDesignerSerializationProvider), PublicDesignerSerializationProvider.Serializer };
            yield return new object[] { objectType, typeof(ClassWithPrivateDesignerSerializationProvider), PrivateDesignerSerializationProvider.Serializer };
            yield return new object[] { objectType, typeof(ClassWithNullDesignerSerializationProvider), null };
        }

        yield return new object[] { typeof(ClassWithNullBaseDesignerSerializer), typeof(int), null };
        yield return new object[] { typeof(ClassWithEmptyBaseDesignerSerializer), typeof(int), null };
        yield return new object[] { typeof(ClassWithNoSuchBaseDesignerSerializer), typeof(int), null };
        yield return new object[] { typeof(ClassWithNullSubDesignerSerializer), typeof(int), null };
        yield return new object[] { typeof(ClassWithEmptySubDesignerSerializer), typeof(int), null };
        yield return new object[] { typeof(ClassWithNoSuchSubDesignerSerializer), typeof(int), null };
        yield return new object[] { typeof(ClassWithPublicDesignerSerializer), typeof(int), null };
        yield return new object[] { typeof(ClassWithPublicDesignerSerializer), typeof(object), null };
        yield return new object[] { typeof(ClassWithPublicDesignerSerializer), typeof(SubClass), null };
    }

    [Theory]
    [MemberData(nameof(GetSerializer_TestData))]
    public void DesignerSerializationManager_GetSerializer_CustomIDesignerSerializationProvider_ReturnsExpected(Type objectType, Type serializerType, object expected)
    {
        DesignerSerializationManager manager = new();
        Assert.Same(expected, manager.GetSerializer(objectType, serializerType));

        // Call again.
        Assert.Same(expected, manager.GetSerializer(objectType, serializerType));
    }

    [Theory]
    [MemberData(nameof(GetSerializer_TestData))]
    public void DesignerSerializationManager_GetSerializerWithSession_CustomIDesignerSerializationProvider_ReturnsExpected(Type objectType, Type serializerType, object expected)
    {
        DesignerSerializationManager manager = new();
        manager.CreateSession();

        Assert.Same(expected, manager.GetSerializer(objectType, serializerType));

        // Call again.
        Assert.Same(expected, manager.GetSerializer(objectType, serializerType));
    }

    public static IEnumerable<object[]> GetSerializer_CustomDesignerSerializer_TestData()
    {
        yield return new object[] { typeof(ClassWithPublicDesignerSerializer), typeof(PublicDesignerSerializationProvider) };
        yield return new object[] { typeof(ClassWithPrivateDesignerSerializer), typeof(PrivateDesignerSerializationProvider) };
    }

    [Theory]
    [MemberData(nameof(GetSerializer_CustomDesignerSerializer_TestData))]
    public void DesignerSerializationManager_GetSerializer_CustomDesignerSerializerNoSession_ReturnsExpected(Type objectType, Type expectedSerializerType)
    {
        DesignerSerializationManager manager = new();
        object serializer1 = manager.GetSerializer(objectType, typeof(BaseClass));
        Assert.IsType(expectedSerializerType, serializer1);

        // Call again.
        object serializer2 = manager.GetSerializer(objectType, typeof(BaseClass));
        Assert.IsType(expectedSerializerType, serializer2);
        Assert.NotSame(serializer1, serializer2);

        // Call different serializer type.
        Assert.Null(manager.GetSerializer(objectType, expectedSerializerType));

        // Call again.
        Assert.Null(manager.GetSerializer(objectType, expectedSerializerType));

        // Call invalid serializer type.
        Assert.Null(manager.GetSerializer(objectType, typeof(object)));

        // Call again.
        Assert.Null(manager.GetSerializer(objectType, typeof(object)));

        // Call unrelated serializer type.
        Assert.Null(manager.GetSerializer(objectType, typeof(int)));

        // Call again.
        Assert.Null(manager.GetSerializer(objectType, typeof(int)));
    }

    [Theory]
    [MemberData(nameof(GetSerializer_CustomDesignerSerializer_TestData))]
    public void DesignerSerializationManager_GetSerializer_CustomDesignerSerializerWithSession_ReturnsExpected(Type objectType, Type expectedSerializerType)
    {
        DesignerSerializationManager manager = new();
        manager.CreateSession();
        object serializer1 = manager.GetSerializer(objectType, typeof(BaseClass));
        Assert.IsType(expectedSerializerType, serializer1);

        // Call again.
        object serializer2 = manager.GetSerializer(objectType, typeof(BaseClass));
        Assert.IsType(expectedSerializerType, serializer2);
        Assert.NotSame(serializer1, serializer2);

        // Call different serializer type.
        object serializer3 = manager.GetSerializer(objectType, expectedSerializerType);
        Assert.IsType(expectedSerializerType, serializer3);
        Assert.NotSame(serializer1, serializer2);

        // Call again.
        object serializer4 = manager.GetSerializer(objectType, expectedSerializerType);
        Assert.IsType(expectedSerializerType, serializer4);
        Assert.Same(serializer3, serializer4);

        // Call invalid serializer type.
        object serializer5 = manager.GetSerializer(objectType, typeof(object));
        Assert.IsType(expectedSerializerType, serializer5);
        Assert.Same(serializer4, serializer5);

        // Call again.
        object serializer6 = manager.GetSerializer(objectType, typeof(object));
        Assert.IsType(expectedSerializerType, serializer6);
        Assert.Same(serializer5, serializer6);

        // Call unrelated serializer type.
        Assert.Null(manager.GetSerializer(objectType, typeof(int)));

        // Call again.
        Assert.Null(manager.GetSerializer(objectType, typeof(int)));
    }

    [Fact]
    public void DesignerSerializationManager_GetSerializer_IDesignerSerializationProvider_ThrowsMissingMethodException()
    {
        DesignerSerializationManager manager = new();
        Assert.Throws<MissingMethodException>(() => manager.GetSerializer(null, typeof(ClassWithInterfaceDefaultSerializationProvider)));
    }

    [Theory]
    [CommonMemberData(typeof(CommonTestHelperEx), nameof(CommonTestHelperEx.GetTypeWithNullTheoryData))]
    public void DesignerSerializationManager_GetSerializer_NullSerializerType_ThrowsArgumentNullException(Type objectType)
    {
        DesignerSerializationManager manager = new();
        Assert.Throws<ArgumentNullException>("serializerType", () => manager.GetSerializer(objectType, null));
    }

    [Theory]
    [MemberData(nameof(GetSerializer_TestData))]
    public void DesignerSerializationManager_IDesignerSerializationManagerGetSerializer_CustomIDesignerSerializationProvider_ReturnsExpected(Type objectType, Type serializerType, object expected)
    {
        DesignerSerializationManager manager = new();
        IDesignerSerializationManager iManager = manager;
        Assert.Same(expected, iManager.GetSerializer(objectType, serializerType));

        // Call again.
        Assert.Same(expected, iManager.GetSerializer(objectType, serializerType));
    }

    [Theory]
    [MemberData(nameof(GetSerializer_TestData))]
    public void DesignerSerializationManager_IDesignerSerializationManagerGetSerializerWithSession_CustomIDesignerSerializationProvider_ReturnsExpected(Type objectType, Type serializerType, object expected)
    {
        DesignerSerializationManager manager = new();
        IDesignerSerializationManager iManager = manager;
        manager.CreateSession();

        Assert.Same(expected, iManager.GetSerializer(objectType, serializerType));

        // Call again.
        Assert.Same(expected, iManager.GetSerializer(objectType, serializerType));
    }

    [Theory]
    [MemberData(nameof(GetSerializer_CustomDesignerSerializer_TestData))]
    public void DesignerSerializationManager_IDesignerSerializationManagerGetSerializer_CustomDesignerSerializerNoSession_ReturnsExpected(Type objectType, Type expectedSerializerType)
    {
        DesignerSerializationManager manager = new();
        IDesignerSerializationManager iManager = manager;
        object serializer1 = iManager.GetSerializer(objectType, typeof(BaseClass));
        Assert.IsType(expectedSerializerType, serializer1);

        // Call again.
        object serializer2 = iManager.GetSerializer(objectType, typeof(BaseClass));
        Assert.IsType(expectedSerializerType, serializer2);
        Assert.NotSame(serializer1, serializer2);

        // Call different serializer type.
        Assert.Null(iManager.GetSerializer(objectType, expectedSerializerType));

        // Call again.
        Assert.Null(iManager.GetSerializer(objectType, expectedSerializerType));

        // Call invalid serializer type.
        Assert.Null(iManager.GetSerializer(objectType, typeof(object)));

        // Call again.
        Assert.Null(iManager.GetSerializer(objectType, typeof(object)));

        // Call unrelated serializer type.
        Assert.Null(iManager.GetSerializer(objectType, typeof(int)));

        // Call again.
        Assert.Null(iManager.GetSerializer(objectType, typeof(int)));
    }

    [Theory]
    [MemberData(nameof(GetSerializer_CustomDesignerSerializer_TestData))]
    public void DesignerSerializationManager_IDesignerSerializationManagerGetSerializer_CustomDesignerSerializerWithSession_ReturnsExpected(Type objectType, Type expectedSerializerType)
    {
        DesignerSerializationManager manager = new();
        IDesignerSerializationManager iManager = manager;
        manager.CreateSession();
        object serializer1 = iManager.GetSerializer(objectType, typeof(BaseClass));
        Assert.IsType(expectedSerializerType, serializer1);

        // Call again.
        object serializer2 = iManager.GetSerializer(objectType, typeof(BaseClass));
        Assert.IsType(expectedSerializerType, serializer2);
        Assert.NotSame(serializer1, serializer2);

        // Call different serializer type.
        object serializer3 = iManager.GetSerializer(objectType, expectedSerializerType);
        Assert.IsType(expectedSerializerType, serializer3);
        Assert.NotSame(serializer1, serializer2);

        // Call again.
        object serializer4 = iManager.GetSerializer(objectType, expectedSerializerType);
        Assert.IsType(expectedSerializerType, serializer4);
        Assert.Same(serializer3, serializer4);

        // Call invalid serializer type.
        object serializer5 = iManager.GetSerializer(objectType, typeof(object));
        Assert.IsType(expectedSerializerType, serializer5);
        Assert.Same(serializer4, serializer5);

        // Call again.
        object serializer6 = iManager.GetSerializer(objectType, typeof(object));
        Assert.IsType(expectedSerializerType, serializer6);
        Assert.Same(serializer5, serializer6);

        // Call unrelated serializer type.
        Assert.Null(iManager.GetSerializer(objectType, typeof(int)));

        // Call again.
        Assert.Null(iManager.GetSerializer(objectType, typeof(int)));

        // Call unrelated object type
        Assert.Null(iManager.GetSerializer(typeof(object), typeof(int)));
    }

    [Fact]
    public void DesignerSerializationManager_IDesignerSerializationManagerGetSerializer_IDesignerSerializationProvider_ThrowsMissingMethodException()
    {
        DesignerSerializationManager manager = new();
        IDesignerSerializationManager iManager = manager;
        Assert.Throws<MissingMethodException>(() => iManager.GetSerializer(null, typeof(ClassWithInterfaceDefaultSerializationProvider)));
    }

    [Theory]
    [CommonMemberData(typeof(CommonTestHelperEx), nameof(CommonTestHelperEx.GetTypeWithNullTheoryData))]
    public void DesignerSerializationManager_IDesignerSerializationManagerGetSerializer_NullSerializerType_ThrowsArgumentNullException(Type objectType)
    {
        DesignerSerializationManager manager = new();
        IDesignerSerializationManager iManager = manager;
        Assert.Throws<ArgumentNullException>("serializerType", () => iManager.GetSerializer(objectType, null));
    }

    public static IEnumerable<object[]> GetType_ValidProvider_TestData()
    {
        foreach (string typeName in new string[] { null, string.Empty, "typeName" })
        {
            yield return new object[] { typeName, null, 0, true, null };
            yield return new object[] { typeName, null, 0, false, null };
            yield return new object[] { typeName, typeof(int), 1, true, typeof(int) };
            yield return new object[] { typeName, typeof(int), 1, false, null };
        }
    }

    [Theory]
    [MemberData(nameof(GetType_ValidProvider_TestData))]
    public void DesignerSerializationManager_GetType_ValidProvider_ReturnsExpected(string typeName, Type resolvedType, int typeDescriptionProviderServiceCount, bool supportedType, Type expected)
    {
        Mock<ITypeResolutionService> mockTypeResolutionService = new(MockBehavior.Strict);
        mockTypeResolutionService
            .Setup(s => s.GetType(typeName))
            .Returns(resolvedType)
            .Verifiable();
        Mock<TypeDescriptionProvider> mockTypeDescriptionProvider = new(MockBehavior.Strict);
        mockTypeDescriptionProvider
            .Setup(p => p.IsSupportedType(resolvedType))
            .Returns(supportedType)
            .Verifiable();
        Mock<TypeDescriptionProviderService> mockTypeDescriptionProviderService = new(MockBehavior.Strict);
        mockTypeDescriptionProviderService
            .Setup(s => s.GetProvider(resolvedType))
            .Returns(mockTypeDescriptionProvider.Object)
            .Verifiable();
        Mock<IServiceProvider> mockServiceProvider = new(MockBehavior.Strict);
        mockServiceProvider
            .Setup(p => p.GetService(typeof(IDesignerHost)))
            .Returns((IDesignerHost)null);
        mockServiceProvider
            .Setup(p => p.GetService(typeof(ITypeResolutionService)))
            .Returns(mockTypeResolutionService.Object)
            .Verifiable();
        mockServiceProvider
            .Setup(p => p.GetService(typeof(TypeDescriptionProviderService)))
            .Returns(mockTypeDescriptionProviderService.Object)
            .Verifiable();
        SubDesignerSerializationManager manager = new(mockServiceProvider.Object);
        Assert.Same(expected, manager.GetType(typeName));
        mockServiceProvider.Verify(p => p.GetService(typeof(ITypeResolutionService)), Times.Once());
        mockTypeResolutionService.Verify(s => s.GetType(typeName), Times.Once());
        mockServiceProvider.Verify(p => p.GetService(typeof(TypeDescriptionProviderService)), Times.Exactly(typeDescriptionProviderServiceCount));
        mockTypeDescriptionProviderService.Verify(s => s.GetProvider(resolvedType), Times.Exactly(typeDescriptionProviderServiceCount));
        mockTypeDescriptionProvider.Verify(s => s.IsSupportedType(resolvedType), Times.Exactly(typeDescriptionProviderServiceCount));

        // Call again.
        Assert.Same(expected, manager.GetType(typeName));
        mockServiceProvider.Verify(p => p.GetService(typeof(ITypeResolutionService)), Times.Once());
        mockTypeResolutionService.Verify(s => s.GetType(typeName), Times.Exactly(2));
        mockServiceProvider.Verify(p => p.GetService(typeof(TypeDescriptionProviderService)), Times.Exactly(typeDescriptionProviderServiceCount * 2));
        mockTypeDescriptionProviderService.Verify(s => s.GetProvider(resolvedType), Times.Exactly(typeDescriptionProviderServiceCount * 2));
        mockTypeDescriptionProvider.Verify(s => s.IsSupportedType(resolvedType), Times.Exactly(typeDescriptionProviderServiceCount * 2));
    }

    public static IEnumerable<object[]> GetType_InvalidProvider_TestData()
    {
        Mock<IServiceProvider> nullMockServiceProvider = new(MockBehavior.Strict);
        nullMockServiceProvider
            .Setup(p => p.GetService(typeof(IDesignerHost)))
            .Returns((IDesignerHost)null);
        nullMockServiceProvider
            .Setup(p => p.GetService(typeof(ITypeResolutionService)))
            .Returns((ITypeResolutionService)null);
        nullMockServiceProvider
            .Setup(p => p.GetService(typeof(TypeDescriptionProviderService)))
            .Returns((TypeDescriptionProviderService)null);
        yield return new object[] { nullMockServiceProvider.Object };

        Mock<IServiceProvider> invalidMockServiceProvider = new(MockBehavior.Strict);
        invalidMockServiceProvider
            .Setup(p => p.GetService(typeof(ITypeResolutionService)))
            .Returns(new object());
        invalidMockServiceProvider
            .Setup(p => p.GetService(typeof(ITypeResolutionService)))
            .Returns(new object());
        invalidMockServiceProvider
            .Setup(p => p.GetService(typeof(TypeDescriptionProviderService)))
            .Returns(new object());
        yield return new object[] { invalidMockServiceProvider.Object };

        Mock<TypeDescriptionProviderService> invalidMockTypeDescriptionProviderService = new(MockBehavior.Strict);
        invalidMockTypeDescriptionProviderService
            .Setup(p => p.GetProvider(typeof(int)))
            .Returns((TypeDescriptionProvider)null);
        Mock<IServiceProvider> invalidTypeDescriptionProviderServiceMockServiceProvider = new(MockBehavior.Strict);
        invalidTypeDescriptionProviderServiceMockServiceProvider
            .Setup(p => p.GetService(typeof(ITypeResolutionService)))
            .Returns(null);
        invalidTypeDescriptionProviderServiceMockServiceProvider
            .Setup(p => p.GetService(typeof(ITypeResolutionService)))
            .Returns(null);
        invalidTypeDescriptionProviderServiceMockServiceProvider
            .Setup(p => p.GetService(typeof(TypeDescriptionProviderService)))
            .Returns(invalidMockTypeDescriptionProviderService.Object);
        yield return new object[] { invalidTypeDescriptionProviderServiceMockServiceProvider.Object };
    }

    [Theory]
    [MemberData(nameof(GetType_InvalidProvider_TestData))]
    public void DesignerSerializationManager_GetType_InvalidProvider_ReturnsExpected(IServiceProvider provider)
    {
        SubDesignerSerializationManager manager = new(provider);
        Assert.Equal(typeof(int), manager.GetType(typeof(int).FullName));
    }

    public static IEnumerable<object[]> GetType_NoProvider_TestData()
    {
        yield return new object[] { "System.Int32", typeof(int) };
        yield return new object[] { "system.int32", null };
        yield return new object[] { "NoSuchType", null };
        yield return new object[] { string.Empty, null };
        yield return new object[] { ".System.Int32", null };
        yield return new object[] { "System.Int32.", null };
        yield return new object[] { "Name.System.Int32", null };
        yield return new object[] { "System.Int32.Name", null };
        yield return new object[] { ".", null };
        yield return new object[] { typeof(NestedClass).AssemblyQualifiedName, typeof(NestedClass) };
    }

    [Theory]
    [MemberData(nameof(GetType_NoProvider_TestData))]
    public void DesignerSerializationManager_GetType_NoProvider_ReturnsExpected(string typeName, Type expected)
    {
        SubDesignerSerializationManager manager = new();
        Assert.Same(expected, manager.GetType(typeName));
    }

    [Fact]
    public void DesignerSerializationManager_GetType_NullTypeName_ThrowsArgumentNullException()
    {
        SubDesignerSerializationManager manager = new();
        Assert.Throws<ArgumentNullException>("typeName", () => manager.GetType(null));
    }

    [Theory]
    [MemberData(nameof(GetType_ValidProvider_TestData))]
    public void DesignerSerializationManager_IDesignerSerializationManagerGetType_ValidProvider_ReturnsExpected(string typeName, Type resolvedType, int typeDescriptionProviderServiceCount, bool supportedType, Type expected)
    {
        Mock<ITypeResolutionService> mockTypeResolutionService = new(MockBehavior.Strict);
        mockTypeResolutionService
            .Setup(s => s.GetType(typeName))
            .Returns(resolvedType)
            .Verifiable();
        Mock<TypeDescriptionProvider> mockTypeDescriptionProvider = new(MockBehavior.Strict);
        mockTypeDescriptionProvider
            .Setup(p => p.IsSupportedType(resolvedType))
            .Returns(supportedType)
            .Verifiable();
        Mock<TypeDescriptionProviderService> mockTypeDescriptionProviderService = new(MockBehavior.Strict);
        mockTypeDescriptionProviderService
            .Setup(s => s.GetProvider(resolvedType))
            .Returns(mockTypeDescriptionProvider.Object)
            .Verifiable();
        Mock<IServiceProvider> mockServiceProvider = new(MockBehavior.Strict);
        mockServiceProvider
            .Setup(p => p.GetService(typeof(IDesignerHost)))
            .Returns((IDesignerHost)null);
        mockServiceProvider
            .Setup(p => p.GetService(typeof(ITypeResolutionService)))
            .Returns(mockTypeResolutionService.Object)
            .Verifiable();
        mockServiceProvider
            .Setup(p => p.GetService(typeof(TypeDescriptionProviderService)))
            .Returns(mockTypeDescriptionProviderService.Object)
            .Verifiable();
        DesignerSerializationManager manager = new(mockServiceProvider.Object);
        IDesignerSerializationManager iManager = manager;
        manager.CreateSession();

        Assert.Same(expected, iManager.GetType(typeName));
        mockServiceProvider.Verify(p => p.GetService(typeof(ITypeResolutionService)), Times.Once());
        mockTypeResolutionService.Verify(s => s.GetType(typeName), Times.Once());
        mockServiceProvider.Verify(p => p.GetService(typeof(TypeDescriptionProviderService)), Times.Exactly(typeDescriptionProviderServiceCount));
        mockTypeDescriptionProviderService.Verify(s => s.GetProvider(resolvedType), Times.Exactly(typeDescriptionProviderServiceCount));
        mockTypeDescriptionProvider.Verify(s => s.IsSupportedType(resolvedType), Times.Exactly(typeDescriptionProviderServiceCount));

        // Call again.
        Assert.Same(expected, iManager.GetType(typeName));
        mockServiceProvider.Verify(p => p.GetService(typeof(ITypeResolutionService)), Times.Once());
        mockTypeResolutionService.Verify(s => s.GetType(typeName), Times.Exactly(2));
        mockServiceProvider.Verify(p => p.GetService(typeof(TypeDescriptionProviderService)), Times.Exactly(typeDescriptionProviderServiceCount * 2));
        mockTypeDescriptionProviderService.Verify(s => s.GetProvider(resolvedType), Times.Exactly(typeDescriptionProviderServiceCount * 2));
        mockTypeDescriptionProvider.Verify(s => s.IsSupportedType(resolvedType), Times.Exactly(typeDescriptionProviderServiceCount * 2));
    }

    [Theory]
    [MemberData(nameof(GetType_InvalidProvider_TestData))]
    public void DesignerSerializationManager_IDesignerSerializationManagerGetType_InvalidProvider_ReturnsExpected(IServiceProvider provider)
    {
        DesignerSerializationManager manager = new(provider);
        IDesignerSerializationManager iManager = manager;
        using IDisposable session = manager.CreateSession();
        Assert.Equal(typeof(int), iManager.GetType(typeof(int).FullName));
    }

    [Theory]
    [MemberData(nameof(GetType_NoProvider_TestData))]
    public void DesignerSerializationManager_IDesignerSerializationManagerGetType_NoProvider_ReturnsExpected(string typeName, Type expected)
    {
        DesignerSerializationManager manager = new();
        IDesignerSerializationManager iManager = manager;
        using IDisposable session = manager.CreateSession();
        Assert.Same(expected, iManager.GetType(typeName));
    }

    [Theory]
    [StringWithNullData]
    public void DesignerSerializationManager_IDesignerSerializationManagerGetType_NoSession_ThrowsInvalidOperationException(string typeName)
    {
        IDesignerSerializationManager iManager = new DesignerSerializationManager();
        Assert.Throws<InvalidOperationException>(() => iManager.GetType(typeName));
    }

    [Fact]
    public void DesignerSerializationManager_IDesignerSerializationManagerGetType_NullTypeName_ThrowsArgumentNullException()
    {
        DesignerSerializationManager manager = new();
        IDesignerSerializationManager iManager = manager;
        using IDisposable session = manager.CreateSession();
        Assert.Throws<ArgumentNullException>("typeName", () => iManager.GetType(null));
    }

    public static IEnumerable<object[]> ResolveNameEventArgs_TestData()
    {
        yield return new object[] { null };
        yield return new object[] { new ResolveNameEventArgs("name") };
    }

    [Theory]
    [MemberData(nameof(ResolveNameEventArgs_TestData))]
    public void DesignerSerializationManager_OnResolveName_InvokeWithResolveName_CallsHandler(ResolveNameEventArgs eventArgs)
    {
        SubDesignerSerializationManager manager = new();
        IDesignerSerializationManager iManager = manager;
        int callCount = 0;
        ResolveNameEventHandler handler = (sender, e) =>
        {
            Assert.Same(manager, sender);
            Assert.Same(eventArgs, e);
            callCount++;
        };
        IDisposable session = manager.CreateSession();
        iManager.ResolveName += handler;

        // With handler.
        manager.OnResolveName(eventArgs);
        Assert.Equal(1, callCount);
        session.Dispose();

        // Call again.
        session = manager.CreateSession();
        iManager.ResolveName += handler;
        manager.OnResolveName(eventArgs);
        Assert.Equal(2, callCount);
        session.Dispose();

        // Remove handler.
        session = manager.CreateSession();
        iManager.ResolveName += handler;
        iManager.ResolveName -= handler;
        Assert.Equal(2, callCount);
        session.Dispose();
    }

    [Theory]
    [NewAndDefaultData<EventArgs>]
    public void DesignerSerializationManager_OnSessionCreated_InvokeWithSessionCreated_CallsHandler(EventArgs eventArgs)
    {
        SubDesignerSerializationManager manager = new();
        int callCount = 0;
        EventHandler handler = (sender, e) =>
        {
            Assert.Same(manager, sender);
            Assert.Same(eventArgs, e);
            callCount++;
        };
        manager.SessionCreated += handler;

        // With handler.
        manager.OnSessionCreated(eventArgs);
        Assert.Equal(1, callCount);

        // Call again.
        manager.OnSessionCreated(eventArgs);
        Assert.Equal(2, callCount);

        // Remove handler.
        manager.SessionCreated -= handler;
        Assert.Equal(2, callCount);
    }

    [Fact]
    public void DesignerSerializationManager_OnSessionDisposed_Invoke_ClearsErrors()
    {
        SubDesignerSerializationManager manager = new();
        IDesignerSerializationManager iManager = manager;

        _ = manager.CreateSession();
        IList errors1 = manager.Errors;
        Assert.Empty(errors1);
        Assert.Same(errors1, manager.Errors);
        object errorInformation = new();
        iManager.ReportError(errorInformation);
        Assert.Same(errorInformation, Assert.Single(errors1));

        // Dispose, get another and ensure cleared.
        manager.OnSessionDisposed(EventArgs.Empty);
        _ = manager.CreateSession();
        IList errors2 = manager.Errors;
        Assert.Empty(errors2);
        Assert.Same(errors2, manager.Errors);
        Assert.NotSame(errors1, errors2);
    }

    [Fact]
    public void DesignerSerializationManager_OnSessionDisposed_Invoke_ClearsContext()
    {
        SubDesignerSerializationManager manager = new();
        IDesignerSerializationManager iManager = manager;

        _ = manager.CreateSession();
        ContextStack stack1 = iManager.Context;
        Assert.NotNull(stack1);
        Assert.Same(stack1, iManager.Context);

        // Dispose, get another and ensure cleared.
        manager.OnSessionDisposed(EventArgs.Empty);
        _ = manager.CreateSession();
        ContextStack stack2 = iManager.Context;
        Assert.NotNull(stack2);
        Assert.Same(stack2, iManager.Context);
        Assert.NotSame(stack1, stack2);
    }

    [Fact]
    public void DesignerSerializationManager_OnSessionDisposed_Invoke_ClearsSerializers()
    {
        SubDesignerSerializationManager manager = new();

        _ = manager.CreateSession();
        object serializer1 = manager.GetSerializer(typeof(ClassWithPublicDesignerSerializer), typeof(BaseClass));
        Assert.IsType<PublicDesignerSerializationProvider>(serializer1);
        Assert.Same(serializer1, manager.GetSerializer(typeof(ClassWithPublicDesignerSerializer), typeof(PublicDesignerSerializationProvider)));

        // Dispose and ensure cleared.
        manager.OnSessionDisposed(EventArgs.Empty);
        object serializer2 = manager.GetSerializer(typeof(ClassWithPublicDesignerSerializer), typeof(BaseClass));
        Assert.IsType<PublicDesignerSerializationProvider>(serializer2);
        Assert.NotSame(serializer1, serializer2);
        Assert.NotSame(serializer2, manager.GetSerializer(typeof(ClassWithPublicDesignerSerializer), typeof(BaseClass)));
    }

    [Theory]
    [NewAndDefaultData<EventArgs>]
    public void DesignerSerializationManager_OnSessionDisposed_InvokeWithSessionDisposed_CallsHandler(EventArgs eventArgs)
    {
        SubDesignerSerializationManager manager = new();
        int callCount = 0;
        EventHandler handler = (sender, e) =>
        {
            Assert.Same(manager, sender);
            Assert.Same(eventArgs, e);
            callCount++;
        };
        manager.SessionDisposed += handler;

        // With handler.
        manager.OnSessionDisposed(eventArgs);
        Assert.Equal(1, callCount);

        // Call again.
        manager.OnSessionDisposed(eventArgs);
        Assert.Equal(2, callCount);

        // Remove handler.
        manager.SessionDisposed -= handler;
        manager.OnSessionDisposed(eventArgs);
        Assert.Equal(2, callCount);
    }

    [Theory]
    [NewAndDefaultData<EventArgs>]
    public void DesignerSerializationManager_OnSessionDisposed_InvokeWithSerializationComplete_CallsHandler(EventArgs eventArgs)
    {
        SubDesignerSerializationManager manager = new();
        IDesignerSerializationManager iManager = manager;
        int callCount = 0;
        EventHandler handler = (sender, e) =>
        {
            Assert.Same(manager, sender);
            Assert.Same(eventArgs, e);
            callCount++;
        };
        IDisposable session = manager.CreateSession();
        iManager.SerializationComplete += handler;
        manager.OnSessionDisposed(eventArgs);
        Assert.Equal(1, callCount);
        session.Dispose();

        // Call again.
        session = manager.CreateSession();
        iManager.SerializationComplete += handler;
        manager.OnSessionDisposed(eventArgs);
        Assert.Equal(2, callCount);

        // Remove handler.
        session = manager.CreateSession();
        iManager.SerializationComplete += handler;
        iManager.SerializationComplete -= handler;
        manager.OnSessionDisposed(eventArgs);
        Assert.Equal(2, callCount);
    }

    [Fact]
    public void DesignerSerializationManager_RemoveSerializationProvider_Invoke_GetSerializerReturnsNull()
    {
        object serializer = new();
        DesignerSerializationManager manager = new();
        IDesignerSerializationManager iManager = manager;
        Mock<IDesignerSerializationProvider> mockDesignerSerializationProvider = new(MockBehavior.Strict);
        mockDesignerSerializationProvider
            .Setup(p => p.GetSerializer(manager, null, typeof(int), mockDesignerSerializationProvider.Object.GetType()))
            .Returns(serializer)
            .Verifiable();
        iManager.AddSerializationProvider(mockDesignerSerializationProvider.Object);
        Assert.Same(serializer, iManager.GetSerializer(typeof(int), mockDesignerSerializationProvider.Object.GetType()));
        mockDesignerSerializationProvider.Verify(p => p.GetSerializer(manager, null, typeof(int), mockDesignerSerializationProvider.Object.GetType()), Times.Once());

        iManager.RemoveSerializationProvider(mockDesignerSerializationProvider.Object);
        Assert.Null(iManager.GetSerializer(typeof(int), mockDesignerSerializationProvider.Object.GetType()));
        mockDesignerSerializationProvider.Verify(p => p.GetSerializer(manager, null, typeof(int), mockDesignerSerializationProvider.Object.GetType()), Times.Once());

        // Remove again.
        iManager.RemoveSerializationProvider(mockDesignerSerializationProvider.Object);
        Assert.Null(iManager.GetSerializer(typeof(int), mockDesignerSerializationProvider.Object.GetType()));
        mockDesignerSerializationProvider.Verify(p => p.GetSerializer(manager, null, typeof(int), mockDesignerSerializationProvider.Object.GetType()), Times.Once());
    }

    [Fact]
    public void DesignerSerializationManager_RemoveSerializationProvider_NoSuchProviderNotEmpty_Nop()
    {
        DesignerSerializationManager manager = new();
        IDesignerSerializationManager iManager = manager;
        Mock<IDesignerSerializationProvider> mockDesignerSerializationProvider1 = new(MockBehavior.Strict);
        Mock<IDesignerSerializationProvider> mockDesignerSerializationProvider2 = new(MockBehavior.Strict);
        iManager.AddSerializationProvider(mockDesignerSerializationProvider1.Object);
        iManager.RemoveSerializationProvider(null);
        iManager.RemoveSerializationProvider(mockDesignerSerializationProvider2.Object);
    }

    [Fact]
    public void DesignerSerializationManager_RemoveSerializationProvider_NoSuchProviderEmpty_Nop()
    {
        DesignerSerializationManager manager = new();
        IDesignerSerializationManager iManager = manager;
        Mock<IDesignerSerializationProvider> mockDesignerSerializationProvider = new(MockBehavior.Strict);
        iManager.RemoveSerializationProvider(null);
        iManager.RemoveSerializationProvider(mockDesignerSerializationProvider.Object);
    }

    [Fact]
    public void DesignerSerializationManager_ReportError_NonNullErrorInformation_AddsToErrors()
    {
        DesignerSerializationManager manager = new();
        IDesignerSerializationManager iManager = manager;
        using IDisposable session = manager.CreateSession();
        object errorInformation = new();
        iManager.ReportError(errorInformation);
        Assert.Same(errorInformation, Assert.Single(manager.Errors));
    }

    [Fact]
    public void DesignerSerializationManager_ReportError_NullErrorInformation_Nop()
    {
        DesignerSerializationManager manager = new();
        IDesignerSerializationManager iManager = manager;
        using IDisposable session = manager.CreateSession();
        iManager.ReportError(null);
        Assert.Empty(manager.Errors);
    }

    [Fact]
    public void DesignerSerializationManager_ReportError_NoSession_ThrowsInvalidOperationException()
    {
        DesignerSerializationManager manager = new();
        IDesignerSerializationManager iManager = manager;
        Assert.Throws<InvalidOperationException>(() => iManager.ReportError(null));
    }

    [Theory]
    [StringData]
    public void DesignerSerializationManager_SetName_Invoke_GetNameReturnsExpected(string name)
    {
        DesignerSerializationManager manager = new();
        IDesignerSerializationManager iManager = manager;
        manager.CreateSession();

        object instance1 = new();
        iManager.SetName(instance1, name);
        Assert.Same(instance1, iManager.GetInstance(name));
        Assert.Same(name, iManager.GetName(instance1));

        object instance2 = new();
        iManager.SetName(instance2, "OtherName");
        Assert.Same(instance2, iManager.GetInstance("OtherName"));
        Assert.Equal("OtherName", iManager.GetName(instance2));
    }

    [Fact]
    public void DesignerSerializationManager_SetName_NullInstance_ThrowsArgumentNullException()
    {
        DesignerSerializationManager manager = new();
        IDesignerSerializationManager iManager = manager;
        manager.CreateSession();
        Assert.Throws<ArgumentNullException>("instance", () => iManager.SetName(null, "name"));
    }

    [Fact]
    public void DesignerSerializationManager_SetName_NullName_ThrowsArgumentNullException()
    {
        DesignerSerializationManager manager = new();
        IDesignerSerializationManager iManager = manager;
        manager.CreateSession();
        Assert.Throws<ArgumentNullException>("name", () => iManager.SetName(new object(), null));
    }

    [Theory]
    [StringData]
    public void DesignerSerializationManager_SetName_OtherInstanceHasName_ThrowsArgumentException(string name)
    {
        DesignerSerializationManager manager = new();
        IDesignerSerializationManager iManager = manager;
        manager.CreateSession();
        object instance = new();
        iManager.SetName(instance, name);
        Assert.Throws<ArgumentException>("name", () => iManager.SetName(new object(), name));
        Assert.Equal(name, iManager.GetName(instance));
    }

    [Theory]
    [StringData]
    public void DesignerSerializationManager_SetName_SameInstanceHasName_ThrowsArgumentException(string name)
    {
        DesignerSerializationManager manager = new();
        IDesignerSerializationManager iManager = manager;
        manager.CreateSession();
        object instance = new();
        iManager.SetName(instance, name);
        Assert.Throws<ArgumentException>("name", () => iManager.SetName(instance, name));
        Assert.Throws<ArgumentException>("instance", () => iManager.SetName(instance, "OtherName"));
        Assert.Equal(name, iManager.GetName(instance));
    }

    [Fact]
    public void DesignerSerializationManager_SetName_InvokeNoSession_ThrowsInvalidOperationException()
    {
        DesignerSerializationManager manager = new();
        IDesignerSerializationManager iManager = manager;
        Assert.Throws<InvalidOperationException>(() => iManager.SetName(null, null));
    }

    private class SubDesignerSerializationManager : DesignerSerializationManager
    {
        public SubDesignerSerializationManager() : base()
        {
        }

        public SubDesignerSerializationManager(IServiceProvider provider) : base(provider)
        {
        }

        public new object CreateInstance(Type type, ICollection arguments, string name, bool addToContainer)
        {
            return base.CreateInstance(type, arguments, name, addToContainer);
        }

        public new object GetService(Type serviceType) => base.GetService(serviceType);

        public new Type GetType(string typeName) => base.GetType(typeName);

        public new void OnResolveName(ResolveNameEventArgs e) => base.OnResolveName(e);

        public new void OnSessionCreated(EventArgs e) => base.OnSessionCreated(e);

        public new void OnSessionDisposed(EventArgs e) => base.OnSessionDisposed(e);
    }

    private class PropertyProvider
    {
        [Category("Category")]
        [Description("Description")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        [DesignOnly(true)]
        [DisplayName("DisplayName")]
        [Localizable(true)]
        [TypeConverter(typeof(Int64Converter))]
        public int Value { get; set; }
    }

    private class OtherPropertyProvider
    {
        public int OtherValue { get; set; }
    }

    private class NestedClass
    {
    }

    [DesignerSerializer("System.Int32", (string)null)]
    private class ClassWithNullBaseDesignerSerializer
    {
    }

    [DesignerSerializer("System.Int32", "")]
    private class ClassWithEmptyBaseDesignerSerializer
    {
    }

    [DesignerSerializer("System.Int32", "NoSuchType")]
    private class ClassWithNoSuchBaseDesignerSerializer
    {
    }

    [DesignerSerializer((string)null, typeof(int))]
    private class ClassWithNullSubDesignerSerializer
    {
    }

    [DesignerSerializer("", typeof(int))]
    private class ClassWithEmptySubDesignerSerializer
    {
    }

    [DesignerSerializer("NoSuchType", typeof(int))]
    private class ClassWithNoSuchSubDesignerSerializer
    {
    }

    private class BaseClass
    {
    }

    private class SubClass : BaseClass
    {
    }

    [DesignerSerializer(typeof(PublicDesignerSerializationProvider), typeof(BaseClass))]
    private class ClassWithPublicDesignerSerializer
    {
    }

    [DesignerSerializer(typeof(PrivateDesignerSerializationProvider), typeof(BaseClass))]
    private class ClassWithPrivateDesignerSerializer
    {
    }

    [DefaultSerializationProvider("")]
    private class ClassWithEmptyDefaultSerializationProvider
    {
    }

    [DefaultSerializationProvider("NoSuchType")]
    private class ClassWithNoSuchDefaultSerializationProvider
    {
    }

    [DefaultSerializationProvider(typeof(int))]
    private class ClassWithInvalidDefaultSerializationProvider
    {
    }

    [DefaultSerializationProvider(typeof(IDesignerSerializationProvider))]
    private class ClassWithInterfaceDefaultSerializationProvider
    {
    }

    [DefaultSerializationProvider(typeof(PublicDesignerSerializationProvider))]
    private class ClassWithPublicDesignerSerializationProvider
    {
    }

    [DefaultSerializationProvider(typeof(PrivateDesignerSerializationProvider))]
    private class ClassWithPrivateDesignerSerializationProvider
    {
    }

    [DefaultSerializationProvider(typeof(NullDesignerSerializationProvider))]
    private class ClassWithNullDesignerSerializationProvider
    {
    }

    private class PublicDesignerSerializationProvider : IDesignerSerializationProvider
    {
        public static object Serializer { get; } = new();

        private PublicDesignerSerializationProvider()
        {
        }

        public object GetSerializer(IDesignerSerializationManager manager, object currentSerializer, Type objectType, Type serializerType)
        {
            return Serializer;
        }
    }

    private class PrivateDesignerSerializationProvider : IDesignerSerializationProvider
    {
        public static object Serializer { get; } = new();

        private PrivateDesignerSerializationProvider()
        {
        }

        public object GetSerializer(IDesignerSerializationManager manager, object currentSerializer, Type objectType, Type serializerType)
        {
            return Serializer;
        }
    }

    private class NullDesignerSerializationProvider : IDesignerSerializationProvider
    {
        public object GetSerializer(IDesignerSerializationManager manager, object currentSerializer, Type objectType, Type serializerType)
        {
            return null;
        }
    }
}
