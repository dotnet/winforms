// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;
using System.ComponentModel.Design;
using System.Windows.Forms.Design.Tests.Mocks;
using Moq;

namespace System.Windows.Forms.Design.Tests;

public class BindingNavigatorDesignerTests : IDisposable
{
    private readonly Mock<IDesignerHost> _designerHostMock = new();
    private readonly Mock<IServiceProvider> _serviceProviderMock = new();
    private readonly Mock<IComponentChangeService> _componentChangeServiceMock = new();
    private readonly Mock<DesignerTransaction> _mockTransaction = new(MockBehavior.Loose);
    private Mock<ISite>? _siteMock;
    private readonly BindingNavigatorDesigner _designer = new();
    private readonly BindingNavigator _bindingNavigator = new();

    public BindingNavigatorDesignerTests()
    {
        _bindingNavigator = MockMinimalControl();
        _designer.Initialize(_bindingNavigator);
    }

    public void Dispose()
    {
        _bindingNavigator?.Dispose();
        _designer?.Dispose();
    }

    private BindingNavigator MockMinimalControl()
    {
        _siteMock = MockSite.CreateMockSiteWithDesignerHost(_designerHostMock.Object, MockBehavior.Loose);
        _siteMock.Setup(s => s.GetService(typeof(IDesignerHost))).Returns(_designerHostMock.Object);
        _siteMock.Setup(s => s.GetService(typeof(IComponentChangeService))).Returns(_componentChangeServiceMock.Object);

        _serviceProviderMock.Setup(s => s.GetService(typeof(IDesignerHost))).Returns(_designerHostMock.Object);
        _serviceProviderMock.Setup(s => s.GetService(typeof(IComponentChangeService))).Returns(_componentChangeServiceMock.Object);

        _designerHostMock.Setup(h => h.RootComponent).Returns(_bindingNavigator);
        _designerHostMock.Setup(h => h.CreateTransaction(It.IsAny<string>())).Returns(_mockTransaction.Object);
        _designerHostMock.Setup(h => h.GetService(typeof(IComponentChangeService))).Returns(_componentChangeServiceMock.Object);

        Mock<IContainer> containerMock = new();
        _designerHostMock.Setup(h => h.Container).Returns(containerMock.Object);

        _bindingNavigator.Site = _siteMock.Object;
        return _bindingNavigator;
    }

    [Fact]
    public void InitializeNewComponent_ShouldCallAddStandardItems()
    {
        Dictionary<string, object> defaultValues = new();

        _designer.InitializeNewComponent(defaultValues);

        _bindingNavigator.Items.Count.Should().Be(11);
        _bindingNavigator.ShowItemToolTips.Should().BeTrue();
    }

    [Fact]
    public void InitializeNewComponent_ShouldSiteAllItems()
    {
        Mock<IContainer> containerMock = new();
        _designerHostMock.Setup(h => h.Container).Returns(containerMock.Object);

        Dictionary<string, object> defaultValues = new();

        _designer.InitializeNewComponent(defaultValues);

        containerMock.Verify(c => c.Add(It.IsAny<IComponent>(), It.IsAny<string>()), Times.AtLeastOnce);
    }

    public static IEnumerable<object[]> NavigationItemTestData()
    {
        yield return new object[]
        {
            (Action<BindingNavigator, ToolStripItem>)((nav, item) => nav.MoveFirstItem = item),
            (Func<BindingNavigator, ToolStripItem?>)(nav => nav.MoveFirstItem)
        };
        yield return new object[]
        {
            (Action<BindingNavigator, ToolStripItem>)((nav, item) => nav.MovePreviousItem = item),
            (Func<BindingNavigator, ToolStripItem?>)(nav => nav.MovePreviousItem)
        };
        yield return new object[]
        {
            (Action<BindingNavigator, ToolStripItem>)((nav, item) => nav.MoveNextItem = item),
            (Func<BindingNavigator, ToolStripItem?>)(nav => nav.MoveNextItem)
        };
        yield return new object[]
        {
            (Action<BindingNavigator, ToolStripItem>)((nav, item) => nav.MoveLastItem = item),
            (Func<BindingNavigator, ToolStripItem?>)(nav => nav.MoveLastItem)
        };
        yield return new object[]
        {
            (Action<BindingNavigator, ToolStripItem>)((nav, item) => nav.PositionItem = item),
            (Func<BindingNavigator, ToolStripItem?>)(nav => nav.PositionItem)
        };
        yield return new object[]
        {
            (Action<BindingNavigator, ToolStripItem>)((nav, item) => nav.CountItem = item),
            (Func<BindingNavigator, ToolStripItem?>)(nav => nav.CountItem)
        };
        yield return new object[]
        {
            (Action<BindingNavigator, ToolStripItem>)((nav, item) => nav.AddNewItem = item),
            (Func<BindingNavigator, ToolStripItem?>)(nav => nav.AddNewItem)
        };
        yield return new object[]
        {
            (Action<BindingNavigator, ToolStripItem>)((nav, item) => nav.DeleteItem = item),
            (Func<BindingNavigator, ToolStripItem?>)(nav => nav.DeleteItem)
        };
    }

    [Theory]
    [MemberData(nameof(NavigationItemTestData))]
    public void ComponentChangeService_ComponentRemoved_ShouldSetNavigationItemToNull(
        Action<BindingNavigator, ToolStripItem> setter,
        Func<BindingNavigator, ToolStripItem?> getter)
    {
        using ToolStripButton item = new();
        setter(_bindingNavigator, item);

        getter(_bindingNavigator).Should().Be(item);

        ComponentEventArgs args = new(item);
        _designer.TestAccessor.Dynamic.ComponentChangeService_ComponentRemoved(null, args);

        getter(_bindingNavigator).Should().BeNull();
    }

    [Fact]
    public void ComponentChangeService_ComponentRemovedOtherComponent_ShouldNotChangeProperties()
    {
        using ToolStripButton deleteItem = new();
        using ToolStripButton otherItem = new();
        _bindingNavigator.DeleteItem = deleteItem;

        ComponentEventArgs args = new(otherItem);
        _designer.TestAccessor.Dynamic.ComponentChangeService_ComponentRemoved(null, args);

        _bindingNavigator.DeleteItem.Should().Be(deleteItem);
    }

    [Fact]
    public void ComponentChangeService_ComponentChangedDifferentComponent_ShouldIgnoreChange()
    {
        using ToolStripLabel countItem = new() { Text = "Original format" };
        using ToolStripLabel otherItem = new() { Text = "Other text" };
        _bindingNavigator.CountItem = countItem;
        _bindingNavigator.CountItemFormat = "Original format";

        PropertyDescriptor? textProperty = TypeDescriptor.GetProperties(otherItem)["Text"];
        ComponentChangedEventArgs args = new(otherItem, textProperty, "old text", "Other text");

        _designer.TestAccessor.Dynamic.ComponentChangeService_ComponentChanged(null, args);

        _bindingNavigator.CountItemFormat.Should().Be("Original format");
    }

    [Fact]
    public void ComponentChangeService_ComponentChangedDifferentProperty_ShouldIgnoreChange()
    {
        using ToolStripLabel countItem = new() { Text = "Original format" };
        _bindingNavigator.CountItem = countItem;
        _bindingNavigator.CountItemFormat = "Original format";

        PropertyDescriptor? visibleProperty = TypeDescriptor.GetProperties(countItem)["Visible"];
        ComponentChangedEventArgs args = new(countItem, visibleProperty, false, true);

        _designer.TestAccessor.Dynamic.ComponentChangeService_ComponentChanged(null, args);

        _bindingNavigator.CountItemFormat.Should().Be("Original format");
    }
}
