// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;
using System.Globalization;
using Moq;

namespace System.Windows.Forms.Design.Tests;

public class TabOrderTests : IDisposable
{
    private readonly Mock<IDesignerHost> _mockHost;
    private readonly Mock<IUIService> _mockUIService;
    private readonly Mock<IOverlayService> _mockOverlayService;
    private readonly Mock<IHelpService> _mockHelpService;
    private readonly Mock<IMenuCommandService> _mockMenuCommandService;
    private readonly Mock<IEventHandlerService> _mockEventHandlerService;
    private readonly Mock<IComponentChangeService> _mockComponentChangeService;
    private readonly Font _dialogFont;
    private readonly TabOrder _tabOrder;

    public Mock<IComponent> ComponentMock { get; }
    public Mock<Control> ControlMock { get; }

    public void Dispose()
    {
        _dialogFont.Dispose();
        _tabOrder.Dispose();
    }

    public TabOrderTests()
    {
        _mockHost = new();
        _mockUIService = new();
        _mockOverlayService = new();
        _mockHelpService = new();
        _mockMenuCommandService = new();
        _mockEventHandlerService = new();
        _mockComponentChangeService = new();

        _mockHost.Setup(h => h.GetService(typeof(IUIService))).Returns(_mockUIService.Object);
        _mockHost.Setup(h => h.GetService(typeof(IOverlayService))).Returns(_mockOverlayService.Object);
        _mockHost.Setup(h => h.GetService(typeof(IHelpService))).Returns(_mockHelpService.Object);
        _mockHost.Setup(h => h.GetService(typeof(IMenuCommandService))).Returns(_mockMenuCommandService.Object);
        _mockHost.Setup(h => h.GetService(typeof(IEventHandlerService))).Returns(_mockEventHandlerService.Object);
        _mockHost.Setup(h => h.GetService(typeof(IComponentChangeService))).Returns(_mockComponentChangeService.Object);

        _dialogFont = new("Arial", 8);
        _mockUIService.Setup(u => u.Styles["DialogFont"]).Returns(_dialogFont);

        _tabOrder = new(_mockHost.Object);

        ComponentMock = new();
        ControlMock = new();
    }

    [Fact]
    public void TabOrder_Constructor_InitializesFieldsCorrectly()
    {
        dynamic accessor = _tabOrder.TestAccessor().Dynamic;

        _tabOrder.Should().NotBeNull();
        ((Font)accessor._tabFont).Should().Be(new Font(_dialogFont, FontStyle.Bold));
        ((int)accessor._selSize).Should().Be(DesignerUtils.GetAdornmentDimensions(AdornmentType.GrabHandle).Width);
        ((SolidBrush)accessor._highlightTextBrush).Color.Should().Be(SystemColors.HighlightText);
        ((Pen)accessor._highlightPen).Color.Should().Be(SystemColors.Highlight);
        ((string)accessor._decimalSep).Should().Be(CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator);
        ((object)accessor._commands).Should().NotBeNull();
        ((object)accessor._newCommands).Should().NotBeNull();

        _mockOverlayService.Verify(os => os.PushOverlay(_tabOrder), Times.Once);
        _mockHelpService.Verify(hs => hs.AddContextAttribute("Keyword", "TabOrderView", HelpKeywordType.FilterKeyword), Times.Once);
        _mockMenuCommandService.Verify(mcs => mcs.AddCommand(It.IsAny<MenuCommand>()), Times.Exactly(((MenuCommand[])accessor._newCommands).Length));
        _mockEventHandlerService.Verify(ehs => ehs.PushHandler(_tabOrder), Times.Once);
        _mockComponentChangeService.VerifyAdd(cs => cs.ComponentAdded += It.IsAny<ComponentEventHandler>(), Times.Once);
        _mockComponentChangeService.VerifyAdd(cs => cs.ComponentRemoved += It.IsAny<ComponentEventHandler>(), Times.Once);
        _mockComponentChangeService.VerifyAdd(cs => cs.ComponentChanged += It.IsAny<ComponentChangedEventHandler>(), Times.Once);
    }

    [Fact]
    public void OnMouseDoubleClick_DoesNotThrowException() =>
        Record.Exception(() => _tabOrder.OnMouseDoubleClick(ComponentMock.Object)).Should().BeNull();

    [Fact]
    public void OnMouseDown_SetsNextTabIndex_WhenCtlHoverIsNotNull()
    {
        dynamic accessor = _tabOrder.TestAccessor().Dynamic;
        accessor._ctlHover = ControlMock.Object;

        _tabOrder.OnMouseDown(ComponentMock.Object, MouseButtons.Left, 0, 0);

        ((object)accessor._ctlHover).Should().NotBeNull();
        accessor.SetNextTabIndex(ControlMock.Object);
    }

    [Fact]
    public void OnMouseHover_DoesNotThrowException() =>
        Record.Exception(() => _tabOrder.OnMouseHover(ComponentMock.Object)).Should().BeNull();

    [Fact]
    public void OnMouseMove_SetsNewHoverControl()
    {
        List<Control> tabControls = new() {ControlMock.Object};
        dynamic accessor = _tabOrder.TestAccessor().Dynamic;
        accessor._tabControls = tabControls;

        _tabOrder.OnMouseMove(ComponentMock.Object, 10, 10);

        ((object)accessor._ctlHover).Should().Be(ControlMock.Object.ToString());
    }

    [Fact]
    public void OnMouseUp_DoesNotThrowException() =>
        Record.Exception(() => _tabOrder.OnMouseUp(ComponentMock.Object, MouseButtons.Left)).Should().BeNull();

    [Fact]
    public void OnSetCursor_DoesNotThrowException() =>
        Record.Exception(() => _tabOrder.OnSetCursor(ComponentMock.Object)).Should().BeNull();

    [Fact]
    public void OnSetCursor_SetsAppropriateCursor()
    {
        dynamic accessor = _tabOrder.TestAccessor().Dynamic;

        _tabOrder.OnSetCursor(ComponentMock.Object);

        Cursor.Current.Should().Be(Cursors.Default);

        accessor._ctlHover = new Control();
        _tabOrder.OnSetCursor(ComponentMock.Object);
        Cursor.Current.Should().Be(Cursors.Cross);
    }

    [Fact]
    public void OverrideInvoke_CommandExists_InvokesCommandAndReturnsTrue()
    {
        CommandID commandID = new(Guid.NewGuid(), 1);
        MenuCommand menuCommand = new((sender, e) => { }, commandID);
        dynamic accessor = _tabOrder.TestAccessor().Dynamic;
        accessor._commands = new MenuCommand[]
        {
            menuCommand
        };

        _tabOrder.OverrideInvoke(menuCommand).Should().BeTrue();
        menuCommand.Invoke();
    }

    [Fact]
    public void OverrideInvoke_CommandDoesNotExist_ReturnsFalse()
    {
        CommandID commandID = new(Guid.NewGuid(), 1);
        Mock<MenuCommand> mockCommand = new(null!, commandID);
        dynamic accessor = _tabOrder.TestAccessor().Dynamic;
        accessor._commands = new MenuCommand[]
        {
            new MenuCommand((sender, e) => { }, new CommandID(Guid.NewGuid(), 2))
        };

        _tabOrder.OverrideInvoke(mockCommand.Object).Should().BeFalse();
        mockCommand.Verify(c => c.Invoke(), Times.Never);
    }

    [Fact]
    public void OverrideStatus_CommandDoesNotExist_DisablesCommandAndReturnsTrue()
    {
        CommandID commandID = new(Guid.NewGuid(), 1);
        Mock<MenuCommand> mockCommand = new(null!, commandID);
        dynamic accessor = _tabOrder.TestAccessor().Dynamic;
        accessor._commands = new MenuCommand[]
        {
            new MenuCommand((sender, e) => { }, new CommandID(Guid.NewGuid(), 2)) { Enabled = true }
        };

        _tabOrder.OverrideStatus(mockCommand.Object).Should().BeTrue();
        mockCommand.Object.Enabled.Should().BeFalse();
    }

    [Fact]
    public void OverrideStatus_TabOrderCommand_DisablesCommandAndReturnsTrue()
    {
        CommandID commandID = StandardCommands.TabOrder;
        Mock<MenuCommand> mockCommand = new(null!, commandID);
        dynamic accessor = _tabOrder.TestAccessor().Dynamic;
        accessor._commands = new MenuCommand[]
        {
            new MenuCommand((sender, e) => { }, new CommandID(Guid.NewGuid(), 2)) { Enabled = true }
        };

        _tabOrder.OverrideStatus(mockCommand.Object).Should().BeTrue();
        mockCommand.Object.Enabled.Should().BeFalse();
    }
}
