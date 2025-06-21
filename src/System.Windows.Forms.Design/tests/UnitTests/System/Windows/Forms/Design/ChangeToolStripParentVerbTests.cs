// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;
using System.ComponentModel.Design;
using System.Windows.Forms.Design.Behavior;
using System.Windows.Forms.Design.Tests.Mocks;
using Moq;

namespace System.Windows.Forms.Design.Tests;

public class ChangeToolStripParentVerbTests : IDisposable
{
    private DesignerActionService? _designerActionService;
    private DesignerActionUIService? _designerActionUIService;
    private BehaviorService? _behaviorService;
    private readonly Mock<IDesignerHost> _designerHostMock = new();
    private readonly Mock<IServiceProvider> _serviceProviderMock = new();
    private readonly Mock<ISelectionService> _mockSelectionService = new();
    private Mock<ISite>? _siteMock;
    private readonly Mock<IComponentChangeService> _componentChangeServiceMock = new()  ;
    private readonly Mock<DesignerTransaction> _mockTransaction = new(MockBehavior.Loose);
    private readonly ParentControlDesigner _parentControlDesigner = new();
    private readonly ToolStripDesigner _designer = new();
    private ToolStrip _toolStrip = new();

    public void Dispose()
    {
        _toolStrip?.Dispose();
        _parentControlDesigner.Dispose();
        _designerActionService?.Dispose();
        _designerActionUIService?.Dispose();
        _behaviorService!.Dispose();
        _designer.Dispose();
    }

    private ToolStrip MockMinimalControl()
    {
        _mockSelectionService.Setup(s => s.GetComponentSelected(_toolStrip)).Returns(true);
        _siteMock = MockSite.CreateMockSiteWithDesignerHost(_designerHostMock.Object, MockBehavior.Loose);
        _siteMock.Setup(s => s.GetService(typeof(ISelectionService))).Returns(_mockSelectionService.Object);
        _siteMock.Setup(s => s.GetService(typeof(IDesignerHost))).Returns(_designerHostMock.Object);
        _siteMock.Setup(s => s.GetService(typeof(IComponentChangeService))).Returns(_componentChangeServiceMock.Object);
        _behaviorService = new(_serviceProviderMock.Object, new DesignerFrame(_siteMock.Object));
        _siteMock.Setup(s => s.GetService(typeof(BehaviorService))).Returns(_behaviorService);
        _siteMock.Setup(s => s.GetService(typeof(ToolStripAdornerWindowService))).Returns(null!);
        _designerActionService = new(_siteMock.Object);
        _siteMock.Setup(s => s.GetService(typeof(DesignerActionService))).Returns(_designerActionService);

        _siteMock.Setup(s => s.GetService(typeof(IDesignerHost))).Returns(_designerHostMock.Object);
        _designerActionUIService = new(_siteMock.Object);
        _siteMock.Setup(s => s.GetService(typeof(DesignerActionUIService))).Returns(_designerActionUIService);

        _serviceProviderMock.Setup(s => s.GetService(typeof(IDesignerHost))).Returns(_designerHostMock.Object);
        _serviceProviderMock.Setup(s => s.GetService(typeof(IComponentChangeService))).Returns(new Mock<IComponentChangeService>().Object);
        _designerHostMock.Setup(h => h.RootComponent).Returns(_toolStrip);
        _designerHostMock.Setup(h => h.RootComponent).Returns(_toolStrip);
        _designerHostMock.Setup(h => h.CreateTransaction(It.IsAny<string>())).Returns(_mockTransaction.Object);
        _designerHostMock.Setup(h => h.GetService(typeof(IComponentChangeService))).Returns(_componentChangeServiceMock.Object);
        _designerHostMock.Setup(h => h.AddService(typeof(ToolStripKeyboardHandlingService), It.IsAny<object>()));
        _designerHostMock.Setup(h => h.AddService(typeof(ISupportInSituService), It.IsAny<object>()));
        _designerHostMock.Setup(h => h.AddService(typeof(DesignerActionService), It.IsAny<object>()));
        _designerHostMock.Setup(h => h.GetDesigner(_toolStrip)).Returns(_parentControlDesigner);
        _designerHostMock.Setup(h => h.AddService(typeof(DesignerActionUIService), It.IsAny<object>()));

        _toolStrip.Site = _siteMock.Object;
        return _toolStrip;
    }

    [Fact]
    public void ChangeParent_WithNullParent_ChangesParentToToolStripPanel()
    {
        _toolStrip = MockMinimalControl();
        Control? oldParent = _toolStrip.Parent;
        _parentControlDesigner.Initialize(_toolStrip);
        _designer.Initialize(_toolStrip);
        ChangeToolStripParentVerb changeToolStripParentVerb = new(_designer);

        changeToolStripParentVerb.ChangeParent();

        _toolStrip.Parent.Should().NotBe(oldParent);
        _toolStrip.Parent.Should().BeOfType<ToolStripPanel>();
    }

    [Fact]
    public void ChangeParent_WithParent_ChangesParentToToolStripContentPanel()
    {
        _toolStrip = MockMinimalControl();
        _toolStrip.Parent = new ToolStripPanel();
        Control oldParent = _toolStrip.Parent;
        _parentControlDesigner.Initialize(_toolStrip);
        _designer.Initialize(_toolStrip);
        ChangeToolStripParentVerb changeToolStripParentVerb = new(_designer);

        changeToolStripParentVerb.ChangeParent();
        _toolStrip.Parent.Should().NotBe(oldParent);
        _toolStrip.Parent.Should().BeOfType<ToolStripContentPanel>();
    }

    [Fact]
    public void ChangeParent_WhenDesignerActionUIServiceIsNull_DoesNotChangeParent()
    {
        _toolStrip = MockMinimalControl();
        _toolStrip.Parent = new ToolStripPanel();
        _parentControlDesigner.Initialize(_toolStrip);
        _designer.Initialize(_toolStrip);

        _siteMock!.Setup(s => s.GetService(typeof(DesignerActionUIService))).Returns(null!);

        Control oldParent = _toolStrip.Parent;
        var changeToolStripParentVerb = new ChangeToolStripParentVerb(_designer);

        changeToolStripParentVerb.ChangeParent();

        _toolStrip.Parent.Should().Be(oldParent);
    }
}
