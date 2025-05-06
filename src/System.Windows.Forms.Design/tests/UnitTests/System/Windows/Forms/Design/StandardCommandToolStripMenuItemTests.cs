﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel.Design;
using System.Drawing;
using Moq;

namespace System.Windows.Forms.Design.Tests;

public class StandardCommandToolStripMenuItemTests : IDisposable
{
    private readonly Mock<IServiceProvider> _serviceProviderMock;
    private readonly Mock<IMenuCommandService> _menuCommandServiceMock;
    private readonly CommandID _commandID;
    private readonly MenuCommand _menuCommand;
    private readonly StandardCommandToolStripMenuItem _item;

    public StandardCommandToolStripMenuItemTests()
    {
        _serviceProviderMock = new();
        _menuCommandServiceMock = new();
        _commandID = new(Guid.NewGuid(), 1);
        _menuCommand = new((sender, e) => { }, _commandID);

        _serviceProviderMock.Setup(sp => sp.GetService(typeof(IMenuCommandService)))
            .Returns(_menuCommandServiceMock.Object);
        _menuCommandServiceMock.Setup(mcs => mcs.FindCommand(_commandID))
            .Returns(_menuCommand);

        _item = new(_commandID, "Test Text", "TestImage", _serviceProviderMock.Object);
    }

    public void Dispose() => _item.Dispose();

    [WinFormsFact]
    public void Constructor_InitializesPropertiesCorrectly()
    {
        _item.MenuService?.FindCommand(_commandID)?.CommandID.Should().Be(_commandID);
        string name = _item.TestAccessor().Dynamic._name;

        _item.Text.Should().Be("Test Text");
        name.Should().BeSameAs("TestImage");
    }

    [WinFormsFact]
    public void RefreshItem_UpdatesProperties()
    {
        _menuCommand.Visible = false;
        _menuCommand.Enabled = false;
        _menuCommand.Checked = true;

        _item.RefreshItem();

        _item.Visible.Should().BeFalse();
        _item.Enabled.Should().BeFalse();
        _item.Checked.Should().BeTrue();
    }

    [WinFormsFact]
    public void MenuService_RetrievesMenuCommandService()
    {
        IMenuCommandService? menuService = _item.MenuService;

        menuService.Should().Be(_menuCommandServiceMock.Object);
    }

    [WinFormsFact]
    public void Image_ReturnsCachedImageOnSubsequentAccesses()
    {
        Image? firstAccessImage = _item.Image;
        Image? secondAccessImage = _item.Image;

        firstAccessImage.Should().BeSameAs(secondAccessImage);
    }

    [WinFormsFact]
    public void Item_OnClick_WithNullMenuCommand()
    {
        Action action = _item.PerformClick;
        action.Should().NotThrow();
    }

    [WinFormsFact]
    public void OnClick_InvokesGlobalCommand_WhenMenuCommandIsNull()
    {
        _menuCommandServiceMock.Setup(mcs => mcs.FindCommand(_commandID))
            .Returns((MenuCommand?)null);
        _menuCommandServiceMock.Setup(mcs => mcs.GlobalInvoke(_commandID))
            .Returns(true);

        StandardCommandToolStripMenuItem item = new(_commandID, "Test Text", "TestImage", _serviceProviderMock.Object);
        item.PerformClick();

        _menuCommandServiceMock.Verify(mcs => mcs.GlobalInvoke(_commandID), Times.Once);
    }
}
