// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;
using System.ComponentModel.Design;
using System.ComponentModel.Design.Serialization;
using System.Windows.Forms.Design.Behavior;
using Moq;

namespace System.Windows.Forms.Design.Tests;
public class StandardMenuStripVerbTests : IDisposable
{
    private readonly Mock<IDesignerHost> _designerHostMock = new();
    private readonly Mock<IServiceProvider> _serviceProviderMock = new();
    private readonly Mock<ISelectionService> _selectionServiceMock = new();
    private readonly Mock<IComponentChangeService> _componentChangeServiceMock = new();
    private readonly Mock<ISite> _siteMock = new();
    private readonly Mock<DesignerTransaction> _mockTransaction = new(MockBehavior.Loose);

    private readonly DesignerFrame _designerFrame;
    private readonly SelectionManager _selectionManager;
    private readonly BehaviorService _behaviorService;
    private readonly DesignerActionUIService _designerActionUIService;

    private readonly ToolStripDesigner _designer = new();
    private readonly ParentControlDesigner _parentControlDesigner = new();
    private readonly MenuStrip _menuStrip = new();

    public StandardMenuStripVerbTests()
    {
        _siteMock.Setup(s => s.GetService(typeof(INestedContainer))).Returns(new Mock<IContainer>().Object);
        _siteMock.Setup(s => s.Container).Returns(new Mock<ServiceContainer>().Object as IContainer);
        _siteMock.Setup(s => s.GetService(typeof(UndoEngine))).Returns(null!);
        _siteMock.Setup(s => s.GetService(typeof(IDesignerHost))).Returns(_designerHostMock.Object);
        _siteMock.Setup(s => s.GetService(typeof(IComponentChangeService))).Returns(_componentChangeServiceMock.Object);

         _designerFrame = new(_siteMock.Object);
        _behaviorService = new(_serviceProviderMock.Object, _designerFrame);
        _siteMock.Setup(s => s.GetService(typeof(BehaviorService))).Returns(_behaviorService);
        _siteMock.Setup(s => s.GetService(typeof(ToolStripAdornerWindowService))).Returns(null!);
        _siteMock.Setup(s => s.GetService(typeof(DesignerActionService))).Returns(new Mock<DesignerActionService>(_siteMock.Object).Object);

        Mock<INameCreationService> nameCreationServiceMock = new();
        nameCreationServiceMock.Setup(n => n.IsValidName(It.IsAny<string>())).Returns(true);
        _siteMock.Setup(s => s.GetService(typeof(INameCreationService))).Returns(nameCreationServiceMock.Object);
        _designerActionUIService = new(_siteMock.Object);
        _siteMock.Setup(s => s.GetService(typeof(DesignerActionUIService))).Returns(_designerActionUIService);

        _serviceProviderMock.Setup(s => s.GetService(typeof(IDesignerHost))).Returns(_designerHostMock.Object);
        _serviceProviderMock.Setup(s => s.GetService(typeof(IComponentChangeService))).Returns(new Mock<IComponentChangeService>().Object);

        _designerHostMock.Setup(h => h.RootComponent).Returns(_menuStrip);
        _designerHostMock.Setup(h => h.CreateTransaction(It.IsAny<string>())).Returns(_mockTransaction.Object);
        _designerHostMock.Setup(h => h.GetService(typeof(IComponentChangeService))).Returns(_componentChangeServiceMock.Object);
        _designerHostMock.Setup(h => h.AddService(typeof(ToolStripKeyboardHandlingService), It.IsAny<object>()));
        _designerHostMock.Setup(h => h.AddService(typeof(ISupportInSituService), It.IsAny<object>()));
        _designerHostMock.Setup(h => h.AddService(typeof(DesignerActionService), It.IsAny<object>()));
        _designerHostMock.Setup(h => h.GetDesigner(_menuStrip)).Returns(_parentControlDesigner);
        _designerHostMock.Setup(h => h.AddService(typeof(DesignerActionUIService), It.IsAny<object>()));
        Mock<ToolStripMenuItem> toolStripMenuItemMock = new();
        using ToolStripDropDownMenu toolStripDropDownMenu = new();
        toolStripMenuItemMock.Object.DropDown = toolStripDropDownMenu;

        string[] menuItemImageNames =
        [
            "file", "new", "open", "save", "saveAs", "print", "printPreview", "cut", "copy", "paste", "undo", "redo",
            "delete", "selectAll", "edit", "exit", "saveAs", "saveAll", "tool", "tools", "customize", "options", "help",
            "contents", "index", "search", "about", "cut", "copy", "Paste", "undo", "redo"
        ];

        foreach (string item in menuItemImageNames)
        {
            _designerHostMock.Setup(h => h.CreateComponent(typeof(ToolStripMenuItem), item + "ToolStripMenuItem")).Returns(toolStripMenuItemMock.Object);
        }

        _designerHostMock.Setup(h => h.CreateComponent(typeof(ToolStripSeparator), "toolStripSeparator")).Returns(new Mock<ToolStripSeparator>().Object);
        _selectionServiceMock.Setup(s => s.GetComponentSelected(_menuStrip)).Returns(true);
        _siteMock.Setup(s => s.GetService(typeof(ISelectionService))).Returns(_selectionServiceMock.Object);
        _designerHostMock.Setup(h => h.AddService(typeof(ISelectionService), _selectionServiceMock.Object));
        _serviceProviderMock.Setup(s => s.GetService(typeof(ISelectionService))).Returns(_selectionServiceMock.Object);
        _selectionManager = new(_serviceProviderMock.Object, _behaviorService!);
        _siteMock.Setup(s => s.GetService(typeof(SelectionManager))).Returns(_selectionManager);

        _menuStrip.Site = _siteMock.Object;

        _designer.Initialize(_menuStrip);
        _parentControlDesigner.Initialize(_menuStrip);

        IComponent[] components = [];
        ComponentCollection componentCollection = new(components);
        _selectionServiceMock.Setup(s => s.GetSelectedComponents()).Returns(components);

        Mock<IContainer> containerMock = new();
        containerMock.Setup(c => c.Components).Returns(componentCollection);
        _designerHostMock.Setup(d => d.Container).Returns(containerMock.Object);
    }

    public void Dispose()
    {
        _menuStrip?.Dispose();
        _parentControlDesigner.Dispose();
        _behaviorService?.Dispose();
        _selectionManager?.Dispose();
        _designerFrame.Dispose();
        _designer.Dispose();
    }

    [WinFormsFact]
    public void StandardMenuStripVerb_Ctor()
    {
        StandardMenuStripVerb standardMenuStripVerb = new(_designer);
        standardMenuStripVerb.Should().BeOfType<StandardMenuStripVerb>();
        ToolStripDesigner toolStripDesigner = standardMenuStripVerb.TestAccessor().Dynamic._designer;
        toolStripDesigner.Should().Be(_designer);
    }

    [WinFormsFact]
    public void StandardMenuStripVerb_InsertsMenuStrip()
    {
        StandardMenuStripVerb standardMenuStripVerb = new(_designer);

        ToolStripItemCollection toolStripItemCollection = _parentControlDesigner.Component.TestAccessor().Dynamic._toolStripItemCollection;

        toolStripItemCollection.Count.Should().Be(0);

        standardMenuStripVerb.InsertItems();

        toolStripItemCollection = _parentControlDesigner.Component.TestAccessor().Dynamic._toolStripItemCollection;

        toolStripItemCollection.Count.Should().Be(1);
    }
}
