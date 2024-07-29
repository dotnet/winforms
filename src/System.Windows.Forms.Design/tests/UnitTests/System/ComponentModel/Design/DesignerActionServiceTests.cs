// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Collections;
using System.Collections.Specialized;
using System.Windows.Forms.Design;
using Moq;

namespace System.ComponentModel.Design.Tests;

public class DesignerActionServiceTests
{
    [Fact]
    public void Ctor_IServiceProvider()
    {
        Mock<IDesignerHost> mockDesignerHost = new(MockBehavior.Strict);
        mockDesignerHost
            .Setup(h => h.AddService(typeof(DesignerActionService), It.IsAny<DesignerActionService>()))
            .Verifiable();
        Mock<IComponentChangeService> mockComponentChangeService = new(MockBehavior.Strict);
        Mock<ISelectionService> mockSelectionService = new(MockBehavior.Strict);
        Mock<IServiceProvider> mockServiceProvider = new(MockBehavior.Strict);
        mockServiceProvider
            .Setup(p => p.GetService(typeof(IDesignerHost)))
            .Returns(mockDesignerHost.Object)
            .Verifiable();
        mockServiceProvider
            .Setup(p => p.GetService(typeof(IComponentChangeService)))
            .Returns(mockComponentChangeService.Object)
            .Verifiable();
        mockServiceProvider
            .Setup(p => p.GetService(typeof(ISelectionService)))
            .Returns(mockSelectionService.Object)
            .Verifiable();
        DesignerActionService service = new(mockServiceProvider.Object);
        mockServiceProvider.Verify(p => p.GetService(typeof(IDesignerHost)), Times.Once());
        mockServiceProvider.Verify(p => p.GetService(typeof(IComponentChangeService)), Times.Once());
        mockServiceProvider.Verify(p => p.GetService(typeof(ISelectionService)), Times.Once());
        mockDesignerHost.Verify(h => h.AddService(typeof(DesignerActionService), service), Times.Once());
    }

    public static IEnumerable<object[]> Add_ComponentDesignerActionList_TestData()
    {
        CustomDesignerActionList actionList = new(null);
        yield return new object[] { null, new DesignerActionListCollection() };
        yield return new object[] { new DesignerActionList(null), new DesignerActionListCollection() };
        yield return new object[] { new NullCustomDesignerActionList(null), new DesignerActionListCollection() };
        yield return new object[] { actionList, new DesignerActionListCollection { actionList } };
    }

    [Theory]
    [MemberData(nameof(Add_ComponentDesignerActionList_TestData))]
    public void Add_ComponentDesignerActionList_Success(DesignerActionList actionList, DesignerActionListCollection expected)
    {
        SubDesignerActionService service = new(null);
        using Component component = new();
        service.Add(component, actionList);
        Assert.True(service.Contains(component));
        Assert.Equal(expected, service.GetComponentActions(component));
        Assert.Equal(expected, service.GetComponentActions(component, ComponentActionsType.All));
        Assert.Equal(expected, service.GetComponentActions(component, ComponentActionsType.Service));
        Assert.Empty(service.GetComponentActions(component, ComponentActionsType.Component));

        DesignerActionListCollection actionListBuffer1 = new();
        service.GetComponentDesignerActions(component, actionListBuffer1);
        Assert.Empty(actionListBuffer1);

        DesignerActionListCollection actionListBuffer2 = new();
        service.GetComponentServiceActions(component, actionListBuffer2);
        Assert.Equal(expected, actionListBuffer2);
    }

    [Fact]
    public void Add_ComponentDesignerActionListExisting_Success()
    {
        SubDesignerActionService service = new(null);
        Component component = new();
        CustomDesignerActionList actionList1 = new(null);
        CustomDesignerActionList actionList2 = new(null);
        service.Add(component, actionList1);
        service.Add(component, actionList2);
        service.Add(component, (DesignerActionList)null);

        Assert.True(service.Contains(component));
        Assert.Equal(new DesignerActionListCollection { actionList1, actionList2 }, service.GetComponentActions(component));
        Assert.Equal(new DesignerActionListCollection { actionList1, actionList2 }, service.GetComponentActions(component, ComponentActionsType.All));
        Assert.Equal(new DesignerActionListCollection { actionList1, actionList2 }, service.GetComponentActions(component, ComponentActionsType.Service));
        Assert.Empty(service.GetComponentActions(component, ComponentActionsType.Component));

        DesignerActionListCollection actionListBuffer1 = new();
        service.GetComponentDesignerActions(component, actionListBuffer1);
        Assert.Empty(actionListBuffer1);

        DesignerActionListCollection actionListBuffer2 = new();
        service.GetComponentServiceActions(component, actionListBuffer2);
        Assert.Equal(new DesignerActionListCollection { actionList1, actionList2 }, actionListBuffer2);
    }

    [Fact]
    public void Add_NullComponent_ThrowsArgumentNullException()
    {
        DesignerActionService service = new(null);
        Assert.Throws<ArgumentNullException>("comp", () => service.Add(null, (DesignerActionListCollection)null));
        Assert.Throws<ArgumentNullException>("comp", () => service.Add(null, (DesignerActionList)null));
    }

    [Theory]
    [MemberData(nameof(Add_ComponentDesignerActionList_TestData))]
    public void Add_ComponentDesignerActionListCollection_Success(DesignerActionList actionList, DesignerActionListCollection expected)
    {
        SubDesignerActionService service = new(null);
        using Component component = new();
        service.Add(component, new DesignerActionListCollection { actionList });
        Assert.True(service.Contains(component));
        Assert.Equal(expected, service.GetComponentActions(component));
        Assert.Equal(expected, service.GetComponentActions(component, ComponentActionsType.All));
        Assert.Equal(expected, service.GetComponentActions(component, ComponentActionsType.Service));
        Assert.Empty(service.GetComponentActions(component, ComponentActionsType.Component));

        DesignerActionListCollection actionListBuffer1 = new();
        service.GetComponentDesignerActions(component, actionListBuffer1);
        Assert.Empty(actionListBuffer1);

        DesignerActionListCollection actionListBuffer2 = new();
        service.GetComponentServiceActions(component, actionListBuffer2);
        Assert.Equal(expected, actionListBuffer2);
    }

    [Fact]
    public void Add_ComponentDesignerActionListCollectionExisting_Success()
    {
        SubDesignerActionService service = new(null);
        using Component component = new();
        CustomDesignerActionList actionList1 = new(null);
        CustomDesignerActionList actionList2 = new(null);
        service.Add(component, new DesignerActionListCollection { actionList1 });
        service.Add(component, new DesignerActionListCollection { actionList2 });
        service.Add(component, new DesignerActionListCollection { null });

        Assert.True(service.Contains(component));
        Assert.Equal(new DesignerActionListCollection { actionList1, actionList2 }, service.GetComponentActions(component));
        Assert.Equal(new DesignerActionListCollection { actionList1, actionList2 }, service.GetComponentActions(component, ComponentActionsType.All));
        Assert.Equal(new DesignerActionListCollection { actionList1, actionList2 }, service.GetComponentActions(component, ComponentActionsType.Service));
        Assert.Empty(service.GetComponentActions(component, ComponentActionsType.Component));

        DesignerActionListCollection actionListBuffer1 = new();
        service.GetComponentDesignerActions(component, actionListBuffer1);
        Assert.Empty(actionListBuffer1);

        DesignerActionListCollection actionListBuffer2 = new();
        service.GetComponentServiceActions(component, actionListBuffer2);
        Assert.Equal(new DesignerActionListCollection { actionList1, actionList2 }, actionListBuffer2);
    }

    [Fact]
    public void Add_NullDesignerActionListCollection_ThrowsArgumentNullException()
    {
        DesignerActionService service = new(null);
        Assert.Throws<ArgumentNullException>("designerActionListCollection", () => service.Add(new Component(), (DesignerActionListCollection)null));
    }

    [Fact]
    public void Clear_NotEmpty_Success()
    {
        SubDesignerActionService service = new(null);
        Component component = new();
        service.Add(component, new CustomDesignerActionList(null));
        Assert.NotEmpty(service.GetComponentActions(component));
        Assert.True(service.Contains(component));

        service.Clear();
        Assert.Empty(service.GetComponentActions(component));
        Assert.False(service.Contains(component));

        // Clear again.
        service.Clear();
        Assert.Empty(service.GetComponentActions(component));
        Assert.False(service.Contains(component));
    }

    [Fact]
    public void Clear_Empty_Success()
    {
        SubDesignerActionService service = new(null);
        service.Clear();

        // Clear again.
        service.Clear();
    }

    [Fact]
    public void Clear_InvokeWithDesignerActionListsChanged_CallsHandler()
    {
        SubDesignerActionService service = new(null);
        using Component component1 = new();
        using Component component2 = new();
        CustomDesignerActionList actionList = new(null);
        service.Add(component1, actionList);
        service.Add(component2, new DesignerActionListCollection());

        int callCount = 0;
        DesignerActionListsChangedEventHandler handler = (sender, e) =>
        {
            Assert.Same(service, sender);
            Assert.True(e.RelatedObject == component1 || e.RelatedObject == component2);
            Assert.Equal(DesignerActionListsChangedType.ActionListsRemoved, e.ChangeType);
            // Assert.Same(actionList, Assert.Single(e.ActionLists));
            callCount++;
        };
        service.DesignerActionListsChanged += handler;

        service.Clear();
        Assert.Equal(2, callCount);

        // Remove again.
        service.Clear();
        Assert.Equal(2, callCount);

        // Remove handler.
        service.DesignerActionListsChanged -= handler;
        service.Add(component1, actionList);
        service.Add(component2, new DesignerActionListCollection());
        service.Remove(component2);
        Assert.Equal(2, callCount);
    }

    [Fact]
    public void Contains_NoSuchComponentEmpty_ReturnsFalse()
    {
        DesignerActionService service = new(null);
        Assert.False(service.Contains(new Component()));
    }

    [Fact]
    public void Contains_NoSuchComponentNotEmpty_ReturnsFalse()
    {
        DesignerActionService service = new(null);
        service.Add(new Component(), new DesignerActionListCollection());
        Assert.False(service.Contains(new Component()));
    }

    [Fact]
    public void Contains_NullComponent_ThrowsArgumentNullException()
    {
        DesignerActionService service = new(null);
        Assert.Throws<ArgumentNullException>("comp", () => service.Contains(null));
    }

    [Fact]
    public void Dispose_Invoke_Success()
    {
        Mock<IDesignerHost> mockDesignerHost = new(MockBehavior.Strict);
        mockDesignerHost
            .Setup(h => h.AddService(typeof(DesignerActionService), It.IsAny<DesignerActionService>()));
        mockDesignerHost
            .Setup(h => h.RemoveService(typeof(DesignerActionService)));
        Mock<IComponentChangeService> mockComponentChangeService = new(MockBehavior.Strict);
        Mock<ISelectionService> mockSelectionService = new(MockBehavior.Strict);
        Mock<IServiceProvider> mockServiceProvider = new(MockBehavior.Strict);
        mockServiceProvider
            .Setup(p => p.GetService(typeof(IDesignerHost)))
            .Returns(mockDesignerHost.Object);
        mockServiceProvider
            .Setup(p => p.GetService(typeof(IComponentChangeService)))
            .Returns(mockComponentChangeService.Object);
        mockServiceProvider
            .Setup(p => p.GetService(typeof(ISelectionService)))
            .Returns(mockSelectionService.Object);
        DesignerActionService service = new(mockServiceProvider.Object);
        service.Dispose();
        mockDesignerHost.Verify(h => h.RemoveService(typeof(DesignerActionService)), Times.Once());

        // Dispose again.
        service.Dispose();
        mockDesignerHost.Verify(h => h.RemoveService(typeof(DesignerActionService)), Times.Exactly(2));
    }

    [Theory]
    [InlineData(true, 1)]
    [InlineData(false, 0)]
    public void Dispose_InvokeDisposing_Success(bool disposing, int expectedRemoveCallCount)
    {
        Mock<IDesignerHost> mockDesignerHost = new(MockBehavior.Strict);
        mockDesignerHost
            .Setup(h => h.AddService(typeof(DesignerActionService), It.IsAny<DesignerActionService>()));
        mockDesignerHost
            .Setup(h => h.RemoveService(typeof(DesignerActionService)));
        Mock<IComponentChangeService> mockComponentChangeService = new(MockBehavior.Strict);
        Mock<ISelectionService> mockSelectionService = new(MockBehavior.Strict);
        Mock<IServiceProvider> mockServiceProvider = new(MockBehavior.Strict);
        mockServiceProvider
            .Setup(p => p.GetService(typeof(IDesignerHost)))
            .Returns(mockDesignerHost.Object);
        mockServiceProvider
            .Setup(p => p.GetService(typeof(IComponentChangeService)))
            .Returns(mockComponentChangeService.Object);
        mockServiceProvider
            .Setup(p => p.GetService(typeof(ISelectionService)))
            .Returns(mockSelectionService.Object);
        SubDesignerActionService service = new(mockServiceProvider.Object);
        service.Dispose(disposing);
        mockDesignerHost.Verify(h => h.RemoveService(typeof(DesignerActionService)), Times.Exactly(expectedRemoveCallCount));

        // Dispose again.
        service.Dispose(disposing);
        mockDesignerHost.Verify(h => h.RemoveService(typeof(DesignerActionService)), Times.Exactly(expectedRemoveCallCount * 2));
    }

    public static IEnumerable<object[]> Dispose_InvalidServiceProvider_TestData()
    {
        yield return new object[] { null };

        Mock<IServiceProvider> nullMockServiceProvider = new(MockBehavior.Strict);
        nullMockServiceProvider
            .Setup(p => p.GetService(typeof(IDesignerHost)))
            .Returns((IDesignerHost)null);
        nullMockServiceProvider
            .Setup(p => p.GetService(typeof(IComponentChangeService)))
            .Returns((IComponentChangeService)null);
        nullMockServiceProvider
            .Setup(p => p.GetService(typeof(ISelectionService)))
            .Returns((IComponentChangeService)null);
        yield return new object[] { nullMockServiceProvider.Object };

        Mock<IServiceProvider> invalidMockServiceProvider = new(MockBehavior.Strict);
        invalidMockServiceProvider
            .Setup(p => p.GetService(typeof(IDesignerHost)))
            .Returns(new object());
        invalidMockServiceProvider
            .Setup(p => p.GetService(typeof(IComponentChangeService)))
            .Returns(new object());
        invalidMockServiceProvider
            .Setup(p => p.GetService(typeof(ISelectionService)))
            .Returns(new object());
        yield return new object[] { invalidMockServiceProvider.Object };
    }

    [Theory]
    [MemberData(nameof(Dispose_InvalidServiceProvider_TestData))]
    public void Dispose_InvalidServiceProvider_Success(IServiceProvider serviceProvider)
    {
        DesignerActionService service = new(serviceProvider);
        service.Dispose();

        // Dispose again.
        service.Dispose();
    }

    [Theory]
    [EnumData<ComponentActionsType>]
    [InvalidEnumData<ComponentActionsType>]
    public void GetComponentActions_NoSuchComponentNotEmpty_ReturnsEmpty(ComponentActionsType type)
    {
        SubDesignerActionService service = new(null);
        using Component component = new();
        CustomDesignerActionList actionList = new(null);
        service.Add(component, actionList);
        Assert.Empty(service.GetComponentActions(new Component(), type));
        Assert.Empty(service.GetComponentActions(new Component(), type));
    }

    [Theory]
    [EnumData<ComponentActionsType>]
    [InvalidEnumData<ComponentActionsType>]
    public void GetComponentActions_NoSuchComponentEmpty_ReturnsEmpty(ComponentActionsType type)
    {
        SubDesignerActionService service = new(null);
        Assert.Empty(service.GetComponentActions(new Component(), type));
    }

    [Theory]
    [InvalidEnumData<ComponentActionsType>]
    public void GetComponentActions_NoSuchAction_ReturnsEmpty(ComponentActionsType type)
    {
        SubDesignerActionService service = new(null);
        using Component component = new();
        CustomDesignerActionList actionList = new(null);
        service.Add(component, actionList);
        Assert.Empty(service.GetComponentActions(component, type));
    }

    [Fact]
    public void GetComponentActions_NullComponent_ThrowsArgumentNullException()
    {
        DesignerActionService service = new(null);
        Assert.Throws<ArgumentNullException>("component", () => service.GetComponentActions(null));
        Assert.Throws<ArgumentNullException>("component", () => service.GetComponentActions(null, ComponentActionsType.All));
    }

    public static IEnumerable<object[]> GetComponentDesignerActions_TestData()
    {
        yield return new object[] { null, new DesignerActionListCollection() };

        Mock<ISite> mockSite = new(MockBehavior.Strict);
        yield return new object[] { mockSite.Object, new DesignerActionListCollection() };

        Mock<ISite> nullMockSite = new(MockBehavior.Strict);
        nullMockSite
            .As<IServiceContainer>()
            .Setup(s => s.GetService(typeof(DesignerCommandSet)))
            .Returns(null);
        yield return new object[] { nullMockSite.Object, new DesignerActionListCollection() };

        Mock<ISite> invalidMockSite = new(MockBehavior.Strict);
        invalidMockSite
            .As<IServiceContainer>()
            .Setup(s => s.GetService(typeof(DesignerCommandSet)))
            .Returns(new object());
        yield return new object[] { invalidMockSite.Object, new DesignerActionListCollection() };

        Mock<DesignerCommandSet> nullMockDesignerCommandSet = new(MockBehavior.Strict);
        nullMockDesignerCommandSet
            .Setup(c => c.GetCommands("ActionLists"))
            .Returns((ICollection)null);
        nullMockDesignerCommandSet
            .Setup(c => c.GetCommands("Verbs"))
            .Returns((ICollection)null);
        Mock<ISite> nullCommandSetMockSite = new(MockBehavior.Strict);
        nullCommandSetMockSite
            .As<IServiceContainer>()
            .Setup(s => s.GetService(typeof(DesignerCommandSet)))
            .Returns(nullMockDesignerCommandSet.Object);
        yield return new object[] { nullCommandSetMockSite.Object, new DesignerActionListCollection() };

        Mock<DesignerCommandSet> emptyMockDesignerCommandSet = new(MockBehavior.Strict);
        emptyMockDesignerCommandSet
            .Setup(c => c.GetCommands("ActionLists"))
            .Returns(new DesignerActionListCollection());
        emptyMockDesignerCommandSet
            .Setup(c => c.GetCommands("Verbs"))
            .Returns(new DesignerVerbCollection());
        Mock<ISite> emptyCommandSetMockSite = new(MockBehavior.Strict);
        emptyCommandSetMockSite
            .As<IServiceContainer>()
            .Setup(s => s.GetService(typeof(DesignerCommandSet)))
            .Returns(emptyMockDesignerCommandSet.Object);
        yield return new object[] { emptyCommandSetMockSite.Object, new DesignerActionListCollection() };

        DesignerVerb verb = new(null, null);
        DesignerVerbCollection verbs = new() { null, verb };
        CustomDesignerActionList actionList = new(null);
        DesignerActionListCollection actionLists = new() { null, new DesignerActionList(null), new NullCustomDesignerActionList(null), actionList };

        Mock<DesignerCommandSet> actionListsMockDesignerCommandSet = new(MockBehavior.Strict);
        actionListsMockDesignerCommandSet
            .Setup(c => c.GetCommands("ActionLists"))
            .Returns(actionLists);
        actionListsMockDesignerCommandSet
            .Setup(c => c.GetCommands("Verbs"))
            .Returns(verbs);
        Mock<ISite> actionListsCommandSetMockSite = new(MockBehavior.Strict);
        actionListsCommandSetMockSite
            .As<IServiceContainer>()
            .Setup(s => s.GetService(typeof(DesignerCommandSet)))
            .Returns(actionListsMockDesignerCommandSet.Object);
        yield return new object[] { actionListsCommandSetMockSite.Object, new DesignerActionListCollection { actionList } };
    }

    [Theory]
    [MemberData(nameof(GetComponentDesignerActions_TestData))]
    public void GetComponentDesignerActions_Invoke_ReturnsExpected(ISite site, DesignerActionListCollection expected)
    {
        SubDesignerActionService service = new(null);
        Component component = new()
        {
            Site = site
        };

        DesignerActionListCollection actionListBuffer = new();
        service.GetComponentDesignerActions(component, actionListBuffer);
        Assert.Equal(expected, actionListBuffer);
    }

    [Fact]
    public void GetComponentDesignerActions_InvokeVerbs_ReturnsExpected()
    {
        DesignerVerb verb = new(null, null);
        DesignerVerbCollection verbs = new() { null, new DesignerVerb(null, null) { Enabled = false }, new DesignerVerb(null, null) { Visible = false }, verb };
        Mock<DesignerCommandSet> verbsMockDesignerCommandSet = new(MockBehavior.Strict);
        verbsMockDesignerCommandSet
            .Setup(c => c.GetCommands("ActionLists"))
            .Returns(new DesignerActionListCollection());
        verbsMockDesignerCommandSet
            .Setup(c => c.GetCommands("Verbs"))
            .Returns(verbs);
        Mock<ISite> mockSite = new(MockBehavior.Strict);
        mockSite
            .As<IServiceContainer>()
            .Setup(s => s.GetService(typeof(DesignerCommandSet)))
            .Returns(verbsMockDesignerCommandSet.Object);
        SubDesignerActionService service = new(null);
        Component component = new()
        {
            Site = mockSite.Object
        };

        DesignerActionListCollection actionListBuffer = new();
        service.GetComponentDesignerActions(component, actionListBuffer);
        DesignerActionList actionList = Assert.IsAssignableFrom<DesignerActionList>(Assert.Single(actionListBuffer));
        Assert.False(actionList.AutoShow);
        Assert.Null(actionList.Component);

        DesignerActionItemCollection verbActionActionItems = actionList.GetSortedActionItems();
        DesignerActionMethodItem actionItem = Assert.IsAssignableFrom<DesignerActionMethodItem>(Assert.Single(verbActionActionItems));
        Assert.Equal("Verbs", actionItem.Category);
        Assert.False(actionItem.AllowAssociate);
        Assert.Null(actionItem.Description);
        Assert.False(actionItem.IncludeAsDesignerVerb);
        Assert.Null(actionItem.MemberName);
        Assert.Empty(actionItem.DisplayName);
        Assert.Empty(actionItem.Properties);
        Assert.Same(actionItem.Properties, actionItem.Properties);
        Assert.IsType<HybridDictionary>(actionItem.Properties);
        Assert.Null(actionItem.RelatedComponent);
        Assert.True(actionItem.ShowInSourceView);
    }

    [Fact]
    public void GetComponentDesignerActionsVerbs_GetSortedActionItems_ReturnsExpected()
    {
        DesignerVerb verb = new(null, null);
        DesignerVerbCollection verbs = new() { null, new DesignerVerb(null, null) { Enabled = false }, new DesignerVerb(null, null) { Visible = false }, verb };
        Mock<DesignerCommandSet> verbsMockDesignerCommandSet = new(MockBehavior.Strict);
        verbsMockDesignerCommandSet
            .Setup(c => c.GetCommands("ActionLists"))
            .Returns(new DesignerActionListCollection());
        verbsMockDesignerCommandSet
            .Setup(c => c.GetCommands("Verbs"))
            .Returns(verbs);
        Mock<ISite> mockSite = new(MockBehavior.Strict);
        mockSite
            .As<IServiceContainer>()
            .Setup(s => s.GetService(typeof(DesignerCommandSet)))
            .Returns(verbsMockDesignerCommandSet.Object);
        SubDesignerActionService service = new(null);
        Component component = new()
        {
            Site = mockSite.Object
        };

        DesignerActionListCollection actionListBuffer = new();
        service.GetComponentDesignerActions(component, actionListBuffer);
        DesignerActionList actionList = Assert.IsAssignableFrom<DesignerActionList>(Assert.Single(actionListBuffer));
        Assert.Single(actionList.GetSortedActionItems());

        // Not Disabled.
        verb.Enabled = false;
        Assert.Empty(actionList.GetSortedActionItems());
        verb.Enabled = true;

        // Not Visible.
        verb.Visible = false;
        Assert.Empty(actionList.GetSortedActionItems());
        verb.Visible = true;

        // Not Supported.
        verb.Supported = false;
        Assert.Empty(actionList.GetSortedActionItems());
        verb.Supported = true;

        // Back to normal.
        Assert.Single(actionList.GetSortedActionItems());
    }

    [Fact]
    public void GetComponentDesignerActionsVerbs_InvokeActionItem_CallsVerbInvoke()
    {
        DesignerVerb verb = null;
        int invokeCallCount = 0;
        EventHandler handler = (sender, e) =>
        {
            Assert.Same(verb, sender);
            Assert.Same(EventArgs.Empty, e);
            invokeCallCount++;
        };
        verb = new DesignerVerb("text", handler);

        DesignerVerbCollection verbs = new() { null, new DesignerVerb(null, null) { Enabled = false }, new DesignerVerb(null, null) { Visible = false }, verb };
        Mock<DesignerCommandSet> verbsMockDesignerCommandSet = new(MockBehavior.Strict);
        verbsMockDesignerCommandSet
            .Setup(c => c.GetCommands("ActionLists"))
            .Returns(new DesignerActionListCollection());
        verbsMockDesignerCommandSet
            .Setup(c => c.GetCommands("Verbs"))
            .Returns(verbs);
        Mock<ISite> mockSite = new(MockBehavior.Strict);
        mockSite
            .As<IServiceContainer>()
            .Setup(s => s.GetService(typeof(DesignerCommandSet)))
            .Returns(verbsMockDesignerCommandSet.Object);
        SubDesignerActionService service = new(null);
        Component component = new()
        {
            Site = mockSite.Object
        };

        DesignerActionListCollection actionListBuffer = new();
        service.GetComponentDesignerActions(component, actionListBuffer);
        DesignerActionList actionList = Assert.IsAssignableFrom<DesignerActionList>(Assert.Single(actionListBuffer));
        DesignerActionItemCollection verbActionActionItems = actionList.GetSortedActionItems();
        DesignerActionMethodItem actionItem = Assert.IsAssignableFrom<DesignerActionMethodItem>(Assert.Single(actionList.GetSortedActionItems()));

        Assert.Equal(0, invokeCallCount);
        actionItem.Invoke();
        Assert.Equal(1, invokeCallCount);
    }

    [Fact]
    public void GetComponentDesignerActions_NullComponent_ThrowsArgumentNullException()
    {
        SubDesignerActionService service = new(null);
        Assert.Throws<ArgumentNullException>("component", () => service.GetComponentDesignerActions(null, null));
    }

    [Fact]
    public void GetComponentDesignerActions_NullActionLists_ThrowsArgumentNullException()
    {
        SubDesignerActionService service = new(null);
        Assert.Throws<ArgumentNullException>("actionLists", () => service.GetComponentDesignerActions(new Component(), null));
    }

    [Fact]
    public void GetComponentServiceActions_NoSuchComponentNotEmpty_ReturnsEmpty()
    {
        SubDesignerActionService service = new(null);
        Component component = new();
        CustomDesignerActionList actionList = new(null);
        service.Add(component, actionList);
        DesignerActionListCollection actionListBuffer = new();
        service.GetComponentServiceActions(new Component(), actionListBuffer);
        Assert.Empty(actionListBuffer);
    }

    [Fact]
    public void GetComponentServiceActions_NoSuchComponentEmpty_ReturnsEmpty()
    {
        SubDesignerActionService service = new(null);
        DesignerActionListCollection actionListBuffer = new();
        service.GetComponentServiceActions(new Component(), actionListBuffer);
        Assert.Empty(actionListBuffer);
    }

    [Fact]
    public void GetComponentServiceActions_NullComponent_ThrowsArgumentNullException()
    {
        SubDesignerActionService service = new(null);
        Assert.Throws<ArgumentNullException>("component", () => service.GetComponentServiceActions(null, null));
    }

    [Fact]
    public void GetComponentServiceActions_NullActionLists_ThrowsArgumentNullException()
    {
        SubDesignerActionService service = new(null);
        Assert.Throws<ArgumentNullException>("actionLists", () => service.GetComponentServiceActions(new Component(), null));
    }

    [Fact]
    public void Remove_InvokeComponent_Success()
    {
        DesignerActionService service = new(null);
        using Component component = new();
        service.Add(component, new CustomDesignerActionList(null));
        Assert.NotEmpty(service.GetComponentActions(component));
        Assert.True(service.Contains(component));

        service.Remove(component);
        Assert.Empty(service.GetComponentActions(component));
        Assert.False(service.Contains(component));

        // Remove again.
        service.Remove(component);
        Assert.Empty(service.GetComponentActions(component));
        Assert.False(service.Contains(component));
    }

    [Fact]
    public void Remove_InvokeComponentWithDesignerActionListsChanged_CallsHandler()
    {
        DesignerActionService service = new(null);
        using Component component1 = new();
        using Component component2 = new();
        CustomDesignerActionList actionList = new(null);
        service.Add(component1, actionList);
        service.Add(component2, new DesignerActionListCollection());

        int callCount = 0;
        DesignerActionListsChangedEventHandler handler = (sender, e) =>
        {
            Assert.Same(service, sender);
            Assert.Same(component1, e.RelatedObject);
            Assert.Equal(DesignerActionListsChangedType.ActionListsRemoved, e.ChangeType);
            Assert.Empty(e.ActionLists);
            callCount++;
        };
        service.DesignerActionListsChanged += handler;

        service.Remove(component1);
        Assert.False(service.Contains(component1));
        Assert.Equal(1, callCount);

        // Remove again.
        service.Remove(component1);
        Assert.False(service.Contains(component1));
        Assert.Equal(1, callCount);

        // Remove handler.
        service.DesignerActionListsChanged -= handler;
        service.Remove(component2);
        Assert.False(service.Contains(component2));
        Assert.Equal(1, callCount);
    }

    [Fact]
    public void Remove_NoSuchComponentNotEmpty_Nop()
    {
        DesignerActionService service = new(null);
        service.Add(new Component(), new DesignerActionListCollection());
        service.Remove(new Component());
    }

    [Fact]
    public void Remove_NoSuchComponentEmpty_Nop()
    {
        DesignerActionService service = new(null);
        service.Remove(new Component());
    }

    [Fact]
    public void Remove_InvokeDesignerActionList_Success()
    {
        DesignerActionService service = new(null);
        using Component component1 = new();
        using Component component2 = new();
        using Component component3 = new();
        using Component component4 = new();
        CustomDesignerActionList actionList1 = new(null);
        CustomDesignerActionList actionList2 = new(null);
        CustomDesignerActionList actionList3 = new(null);
        CustomDesignerActionList actionList4 = new(null);
        service.Add(component1, actionList1);
        service.Add(component2, actionList2);
        service.Add(component2, actionList3);
        service.Add(component3, actionList3);
        service.Add(component4, actionList4);
        service.Add(component4, actionList4);

        service.Remove(actionList1);
        Assert.Empty(service.GetComponentActions(component1));
        Assert.Equal(new DesignerActionListCollection { actionList2, actionList3 }, service.GetComponentActions(component2));
        Assert.Equal(new DesignerActionListCollection { actionList3 }, service.GetComponentActions(component3));
        Assert.Equal(new DesignerActionListCollection { actionList4, actionList4 }, service.GetComponentActions(component4));
        Assert.False(service.Contains(component1));
        Assert.True(service.Contains(component2));
        Assert.True(service.Contains(component3));
        Assert.True(service.Contains(component4));

        // Remove again.
        service.Remove(actionList1);
        Assert.Empty(service.GetComponentActions(component1));
        Assert.Equal(new DesignerActionListCollection { actionList2, actionList3 }, service.GetComponentActions(component2));
        Assert.Equal(new DesignerActionListCollection { actionList3 }, service.GetComponentActions(component3));
        Assert.Equal(new DesignerActionListCollection { actionList4, actionList4 }, service.GetComponentActions(component4));
        Assert.False(service.Contains(component1));
        Assert.True(service.Contains(component2));
        Assert.True(service.Contains(component3));
        Assert.True(service.Contains(component4));

        // Remove partial.
        service.Remove(actionList2);
        Assert.Empty(service.GetComponentActions(component1));
        Assert.Equal(new DesignerActionListCollection { actionList3 }, service.GetComponentActions(component2));
        Assert.Equal(new DesignerActionListCollection { actionList3 }, service.GetComponentActions(component3));
        Assert.Equal(new DesignerActionListCollection { actionList4, actionList4 }, service.GetComponentActions(component4));
        Assert.False(service.Contains(component1));
        Assert.True(service.Contains(component2));
        Assert.True(service.Contains(component3));
        Assert.True(service.Contains(component4));

        // Remove across multiple components.
        service.Remove(actionList3);
        Assert.Empty(service.GetComponentActions(component1));
        // Random based off ordering of Dictionary keys.
        if (service.GetComponentActions(component2).Count == 1)
        {
            Assert.Equal(new DesignerActionListCollection { actionList3 }, service.GetComponentActions(component2));
            Assert.Empty(service.GetComponentActions(component3));
        }
        else
        {
            Assert.Empty(service.GetComponentActions(component2));
            Assert.Equal(new DesignerActionListCollection { actionList3 }, service.GetComponentActions(component3));
        }

        Assert.Equal(new DesignerActionListCollection { actionList4, actionList4 }, service.GetComponentActions(component4));
        Assert.False(service.Contains(component1));
        if (service.GetComponentActions(component2).Count == 1)
        {
            Assert.True(service.Contains(component2));
            Assert.False(service.Contains(component3));
        }
        else
        {
            Assert.False(service.Contains(component2));
            Assert.True(service.Contains(component3));
        }

        Assert.True(service.Contains(component4));

        // Remove duplicates.
        service.Remove(actionList4);
        Assert.Empty(service.GetComponentActions(component1));
        // Random based off ordering of Dictionary keys.
        if (service.GetComponentActions(component2).Count == 1)
        {
            Assert.Equal(new DesignerActionListCollection { actionList3 }, service.GetComponentActions(component2));
            Assert.Empty(service.GetComponentActions(component3));
        }
        else
        {
            Assert.Empty(service.GetComponentActions(component2));
            Assert.Equal(new DesignerActionListCollection { actionList3 }, service.GetComponentActions(component3));
        }

        Assert.Empty(service.GetComponentActions(component4));
        Assert.False(service.Contains(component1));
        if (service.GetComponentActions(component2).Count == 1)
        {
            Assert.True(service.Contains(component2));
            Assert.False(service.Contains(component3));
        }
        else
        {
            Assert.False(service.Contains(component2));
            Assert.True(service.Contains(component3));
        }

        Assert.True(service.Contains(component4));
    }

    [Fact]
    public void Remove_NoSuchActionListNotEmpty_Nop()
    {
        DesignerActionService service = new(null);
        Component component = new();
        service.Add(component, new CustomDesignerActionList(null));
        service.Remove(new CustomDesignerActionList(null));
        Assert.Single(service.GetComponentActions(component));
    }

    [Fact]
    public void Remove_NoSuchActionListEmpty_Nop()
    {
        DesignerActionService service = new(null);
        service.Remove(new CustomDesignerActionList(null));
    }

    [Fact]
    public void Remove_InvokeDesignerActionListWithDesignerActionListsChanged_CallsHandler()
    {
        DesignerActionService service = new(null);
        using Component component1 = new();
        using Component component2 = new();
        CustomDesignerActionList actionList1 = new(null);
        CustomDesignerActionList actionList2 = new(null);
        service.Add(component1, actionList1);
        service.Add(component2, actionList2);

        int callCount = 0;
        DesignerActionListsChangedEventHandler handler = (sender, e) =>
        {
            Assert.Same(service, sender);
            Assert.Same(component1, e.RelatedObject);
            Assert.Equal(DesignerActionListsChangedType.ActionListsRemoved, e.ChangeType);
            Assert.Empty(e.ActionLists);
            callCount++;
        };
        service.DesignerActionListsChanged += handler;

        service.Remove(actionList1);
        Assert.False(service.Contains(component1));
        Assert.Equal(1, callCount);

        // Remove again.
        service.Remove(actionList1);
        Assert.False(service.Contains(component1));
        Assert.Equal(1, callCount);

        // Remove handler.
        service.DesignerActionListsChanged -= handler;
        service.Remove(actionList2);
        Assert.False(service.Contains(component2));
        Assert.Equal(1, callCount);
    }

    [Fact]
    public void Remove_InvokeComponentDesignerActionList_Success()
    {
        DesignerActionService service = new(null);
        using Component component1 = new();
        using Component component2 = new();
        using Component component3 = new();
        using Component component4 = new();
        CustomDesignerActionList actionList1 = new(null);
        CustomDesignerActionList actionList2 = new(null);
        CustomDesignerActionList actionList3 = new(null);
        CustomDesignerActionList actionList4 = new(null);
        service.Add(component1, actionList1);
        service.Add(component2, actionList2);
        service.Add(component2, actionList3);
        service.Add(component3, actionList3);
        service.Add(component4, actionList4);
        service.Add(component4, actionList4);

        service.Remove(component1, actionList1);
        Assert.Empty(service.GetComponentActions(component1));
        Assert.Equal(new DesignerActionListCollection { actionList2, actionList3 }, service.GetComponentActions(component2));
        Assert.Equal(new DesignerActionListCollection { actionList3 }, service.GetComponentActions(component3));
        Assert.Equal(new DesignerActionListCollection { actionList4, actionList4 }, service.GetComponentActions(component4));
        Assert.False(service.Contains(component1));
        Assert.True(service.Contains(component2));
        Assert.True(service.Contains(component3));
        Assert.True(service.Contains(component4));

        // Remove again.
        service.Remove(component1, actionList1);
        Assert.Empty(service.GetComponentActions(component1));
        Assert.Equal(new DesignerActionListCollection { actionList2, actionList3 }, service.GetComponentActions(component2));
        Assert.Equal(new DesignerActionListCollection { actionList3 }, service.GetComponentActions(component3));
        Assert.Equal(new DesignerActionListCollection { actionList4, actionList4 }, service.GetComponentActions(component4));
        Assert.False(service.Contains(component1));
        Assert.True(service.Contains(component2));
        Assert.True(service.Contains(component3));
        Assert.True(service.Contains(component4));

        // Remove partial.
        service.Remove(component2, actionList2);
        Assert.Empty(service.GetComponentActions(component1));
        Assert.Equal(new DesignerActionListCollection { actionList3 }, service.GetComponentActions(component2));
        Assert.Equal(new DesignerActionListCollection { actionList3 }, service.GetComponentActions(component3));
        Assert.Equal(new DesignerActionListCollection { actionList4, actionList4 }, service.GetComponentActions(component4));
        Assert.False(service.Contains(component1));
        Assert.True(service.Contains(component2));
        Assert.True(service.Contains(component3));
        Assert.True(service.Contains(component4));

        // Remove across multiple components.
        service.Remove(component2, actionList3);
        Assert.Empty(service.GetComponentActions(component1));
        Assert.Empty(service.GetComponentActions(component2));
        Assert.Equal(new DesignerActionListCollection { actionList3 }, service.GetComponentActions(component3));
        Assert.Equal(new DesignerActionListCollection { actionList4, actionList4 }, service.GetComponentActions(component4));
        Assert.False(service.Contains(component1));
        Assert.False(service.Contains(component2));
        Assert.True(service.Contains(component3));
        Assert.True(service.Contains(component4));

        // Remove duplicates.
        service.Remove(actionList4);
        Assert.Empty(service.GetComponentActions(component1));
        Assert.Empty(service.GetComponentActions(component2));
        Assert.Equal(new DesignerActionListCollection { actionList3 }, service.GetComponentActions(component3));
        Assert.Empty(service.GetComponentActions(component4));
        Assert.False(service.Contains(component1));
        Assert.False(service.Contains(component2));
        Assert.True(service.Contains(component3));
        Assert.True(service.Contains(component4));
    }

    [Fact]
    public void Remove_NoSuchComponentActionListNotEmpty_Nop()
    {
        DesignerActionService service = new(null);
        using Component component = new();
        service.Add(component, new CustomDesignerActionList(null));
        service.Remove(new Component(), new CustomDesignerActionList(null));
        service.Remove(component, new CustomDesignerActionList(null));
        Assert.Single(service.GetComponentActions(component));
    }

    [Fact]
    public void Remove_NoSuchComponentActionListEmpty_Nop()
    {
        DesignerActionService service = new(null);
        service.Remove(new Component(), new CustomDesignerActionList(null));
    }

    [Fact]
    public void Remove_InvokeComponentDesignerActionListWithDesignerActionListsChanged_CallsHandler()
    {
        DesignerActionService service = new(null);
        using Component component1 = new();
        using Component component2 = new();
        CustomDesignerActionList actionList1 = new(null);
        CustomDesignerActionList actionList2 = new(null);
        service.Add(component1, actionList1);
        service.Add(component2, actionList2);

        int callCount = 0;
        DesignerActionListsChangedEventHandler handler = (sender, e) =>
        {
            Assert.Same(service, sender);
            Assert.Same(component1, e.RelatedObject);
            Assert.Equal(DesignerActionListsChangedType.ActionListsRemoved, e.ChangeType);
            Assert.Empty(e.ActionLists);
            callCount++;
        };
        service.DesignerActionListsChanged += handler;

        service.Remove(component1, actionList1);
        Assert.False(service.Contains(component1));
        Assert.Equal(1, callCount);

        // Remove again.
        service.Remove(component1, actionList1);
        Assert.False(service.Contains(component1));
        Assert.Equal(1, callCount);

        // Remove handler.
        service.DesignerActionListsChanged -= handler;
        service.Remove(component2, actionList2);
        Assert.False(service.Contains(component2));
        Assert.Equal(1, callCount);
    }

    [Fact]
    public void Remove_NullComponent_ThrowsArgumentNullException()
    {
        DesignerActionService service = new(null);
        Assert.Throws<ArgumentNullException>("comp", () => service.Remove((IComponent)null));
        Assert.Throws<ArgumentNullException>("comp", () => service.Remove(null, null));
    }

    [Fact]
    public void Remove_NullActionList_ThrowsArgumentNullException()
    {
        DesignerActionService service = new(null);
        Assert.Throws<ArgumentNullException>("actionList", () => service.Remove((DesignerActionList)null));
        Assert.Throws<ArgumentNullException>("actionList", () => service.Remove(new Component(), null));
    }

    private class SubDesignerActionService : DesignerActionService
    {
        public SubDesignerActionService(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        public new void Dispose(bool disposing) => base.Dispose(disposing);

        public new void GetComponentDesignerActions(IComponent component, DesignerActionListCollection actionLists)
        {
            base.GetComponentDesignerActions(component, actionLists);
        }

        public new void GetComponentServiceActions(IComponent component, DesignerActionListCollection actionLists)
        {
            base.GetComponentServiceActions(component, actionLists);
        }
    }

    private class CustomDesignerActionList : DesignerActionList
    {
        public CustomDesignerActionList(IComponent component) : base(component)
        {
        }

        public void PublicMethod()
        {
        }
    }

    private class NullCustomDesignerActionList : DesignerActionList
    {
        public NullCustomDesignerActionList(IComponent component) : base(component)
        {
        }

        public override DesignerActionItemCollection GetSortedActionItems() => null;
    }
}
